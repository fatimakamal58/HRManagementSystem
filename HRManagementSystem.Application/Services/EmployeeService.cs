using HRManagementSystem.Application.DTOs.Employees;
using HRManagementSystem.Application.Exceptions;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Infrastructure.Data;
using HRManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HRManagementSystem.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public EmployeeService(ApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }
       
        public async Task CreateAsync(CreateEmployeeDto dto)
        {
            Normalize(dto);

            await ValidateDuplicateAsync(
                dto.NationalId,
                dto.Email,
                dto.PhoneNumber);

            await ValidateReferencesAsync(
                dto.DepartmentId,
                dto.JobTitleId);

            await ValidateManagerAsync(dto.ManagerId);

            await ValidateUserAccountAsync(dto.UserId);

            var employee = new Employee();

            MapEmployee(employee, dto);

            employee.EmployeeNumber = await GenerateEmployeeNumberAsync();
            employee.UserId = string.IsNullOrWhiteSpace(dto.UserId)
                    ? null
                    : dto.UserId;
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.IsActive)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    EmployeeNumber = e.EmployeeNumber,

                    FullNameAr =
                        e.FirstNameAr + " " +
                        (e.SecondNameAr ?? "") + " " +
                        (e.ThirdNameAr ?? "") + " " +
                        e.LastNameAr,

                    FullNameEn =
                        e.FirstNameEn + " " +
                        (e.SecondNameEn ?? "") + " " +
                        (e.ThirdNameEn ?? "") + " " +
                        e.LastNameEn,

                    NationalId = e.NationalId,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,

                    Gender = e.Gender,
                    BirthDate = e.BirthDate,
                    HireDate = e.HireDate,

                    EmploymentStatus = e.EmploymentStatus,

                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department.NameAr,

                    JobTitleId = e.JobTitleId,
                    JobTitleName = e.JobTitle.NameAr,

                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == id && e.IsActive)
                .Select(EmployeeProjection)
                .FirstOrDefaultAsync();

            if (employee is null)
            {
                throw new NotFoundException("الموظف غير موجود.");
            }

            return employee;
        }

        public async Task<UpdateEmployeeDto> GetForUpdateAsync(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == id && e.IsActive)
                .Select(e => new UpdateEmployeeDto
                {
                    Id = e.Id,
                    EmployeeNumber = e.EmployeeNumber,

                    FirstNameAr = e.FirstNameAr,
                    SecondNameAr = e.SecondNameAr,
                    ThirdNameAr = e.ThirdNameAr,
                    LastNameAr = e.LastNameAr,

                    FirstNameEn = e.FirstNameEn,
                    SecondNameEn = e.SecondNameEn,
                    ThirdNameEn = e.ThirdNameEn,
                    LastNameEn = e.LastNameEn,

                    NationalId = e.NationalId,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,

                    Gender = e.Gender,
                    BirthDate = e.BirthDate,
                    HireDate = e.HireDate,
                    EmploymentStatus = e.EmploymentStatus,

                    DepartmentId = e.DepartmentId,
                    JobTitleId = e.JobTitleId,

                    ManagerId = e.ManagerId,
                    UserId = e.UserId
                })
                .FirstOrDefaultAsync();

            if (employee is null)
            {
                throw new NotFoundException("الموظف غير موجود.");
            }

            return employee;
        }
        
        public async Task UpdateAsync(UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (employee is null)
            {
                throw new NotFoundException("الموظف غير موجود.");
            }

            Normalize(dto);

            await ValidateDuplicateAsync(
                dto.NationalId,
                dto.Email,
                dto.PhoneNumber,
                dto.Id);

            await ValidateReferencesAsync(
                dto.DepartmentId,
                dto.JobTitleId);

            await ValidateManagerAsync(
                dto.ManagerId,
                dto.Id);

            await ValidateUserAccountAsync(
                dto.UserId,
                employee.Id);

            MapEmployee(employee, dto);
            employee.UserId = string.IsNullOrWhiteSpace(dto.UserId)
                                ? null
                                : dto.UserId;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

            if (employee is null)
            {
                throw new NotFoundException(
                    "الموظف غير موجود أو تم حذفه مسبقًا.");
            }

            employee.IsActive = false;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task<List<EmployeeDto>> GetDeletedAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => !e.IsActive)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    EmployeeNumber = e.EmployeeNumber,

                    FullNameAr =
                        e.FirstNameAr + " " +
                        (e.SecondNameAr ?? "") + " " +
                        (e.ThirdNameAr ?? "") + " " +
                        e.LastNameAr,

                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,

                    DepartmentName = e.Department.NameAr,
                    JobTitleName = e.JobTitle.NameAr
                })
                .ToListAsync();
        }
        public async Task RestoreAsync(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsActive);

            if (employee is null)
            {
                throw new NotFoundException(
                    "الموظف غير موجود أو ليس محذوفًا.");
            }

            employee.IsActive = true;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        
        public async Task<EmployeeDto> GetByIdIncludingInactiveAsync(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(EmployeeProjection)
                .FirstOrDefaultAsync();

            if (employee is null)
            {
                throw new NotFoundException("الموظف غير موجود.");
            }

            return employee;
        }
        
        //******** private methods **************
        
        private static readonly Expression<Func<Employee, EmployeeDto>>
            EmployeeProjection = e => new EmployeeDto
            {
                Id = e.Id,
                EmployeeNumber = e.EmployeeNumber,

                FullNameAr =
                    e.FirstNameAr + " " +
                    (e.SecondNameAr ?? "") + " " +
                    (e.ThirdNameAr ?? "") + " " +
                    e.LastNameAr,

                FullNameEn =
                    e.FirstNameEn + " " +
                    (e.SecondNameEn ?? "") + " " +
                    (e.ThirdNameEn ?? "") + " " +
                    e.LastNameEn,

                NationalId = e.NationalId,
                PhoneNumber = e.PhoneNumber,
                Email = e.Email,
                Gender = e.Gender,
                BirthDate = e.BirthDate,
                HireDate = e.HireDate,
                EmploymentStatus = e.EmploymentStatus,

                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.NameAr,

                JobTitleId = e.JobTitleId,
                JobTitleName = e.JobTitle.NameAr,

                IsActive = e.IsActive
            };
        
        private async Task<string> GenerateEmployeeNumberAsync()
        {
            var lastEmployee = await _context.Employees
                .OrderByDescending(e => e.Id)
                .FirstOrDefaultAsync();

            if (lastEmployee is null)
            {
                return "EMP0001";
            }

            int lastNumber = int.Parse(lastEmployee.EmployeeNumber[3..]);

            lastNumber++;

            return $"EMP{lastNumber:D4}";
        }

        private async Task ValidateDuplicateAsync(
            string nationalId,
            string email,
            string phoneNumber,
            int? excludedEmployeeId = null)
        {
            
            var nationalIdExists = await _context.Employees
                .AnyAsync(e =>
                    e.NationalId == nationalId &&
                    (!excludedEmployeeId.HasValue || e.Id != excludedEmployeeId.Value));

            if (nationalIdExists)
                throw new DuplicateException("الهوية الوطنية مستخدمة مسبقًا.");

            var emailExists = await _context.Employees
                .AnyAsync(e =>
                    e.Email == email &&
                    (!excludedEmployeeId.HasValue || e.Id != excludedEmployeeId.Value));

            if (emailExists)
                throw new DuplicateException("البريد الإلكتروني مستخدم مسبقًا.");

            var phoneNumberExists = await _context.Employees
                .AnyAsync(e =>
                    e.PhoneNumber == phoneNumber &&
                    (!excludedEmployeeId.HasValue || e.Id != excludedEmployeeId.Value));

            if (phoneNumberExists)
                throw new DuplicateException("رقم الجوال مستخدم مسبقًا.");
        }
        private async Task ValidateReferencesAsync(
            int departmentId,
            int jobTitleId)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == departmentId && d.IsActive);

            if (!departmentExists)
                throw new NotFoundException("القسم غير موجود أو غير نشط.");

            var jobTitleExists = await _context.JobTitles
                .AnyAsync(j => j.Id == jobTitleId && j.IsActive);

            if (!jobTitleExists)
                throw new NotFoundException("المسمى الوظيفي غير موجود أو غير نشط.");
        }
        
        private static void MapEmployee(
            Employee employee,
            UpdateEmployeeDto dto)
        {

            employee.FirstNameAr = dto.FirstNameAr.Trim();
            employee.SecondNameAr = dto.SecondNameAr?.Trim();
            employee.ThirdNameAr = dto.ThirdNameAr?.Trim();
            employee.LastNameAr = dto.LastNameAr.Trim();

            employee.FirstNameEn = dto.FirstNameEn.Trim();
            employee.SecondNameEn = dto.SecondNameEn?.Trim();
            employee.ThirdNameEn = dto.ThirdNameEn?.Trim();
            employee.LastNameEn = dto.LastNameEn.Trim();

            employee.NationalId = dto.NationalId;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.Email = dto.Email;

            employee.Gender = dto.Gender;
            employee.BirthDate = dto.BirthDate;
            employee.HireDate = dto.HireDate;
            employee.EmploymentStatus = dto.EmploymentStatus;

            employee.DepartmentId = dto.DepartmentId;
            employee.JobTitleId = dto.JobTitleId;
            employee.ManagerId = dto.ManagerId;
        }
        private static void MapEmployee(
            Employee employee,
            CreateEmployeeDto dto)
        {

            employee.FirstNameAr = dto.FirstNameAr.Trim();
            employee.SecondNameAr = dto.SecondNameAr?.Trim();
            employee.ThirdNameAr = dto.ThirdNameAr?.Trim();
            employee.LastNameAr = dto.LastNameAr.Trim();

            employee.FirstNameEn = dto.FirstNameEn.Trim();
            employee.SecondNameEn = dto.SecondNameEn?.Trim();
            employee.ThirdNameEn = dto.ThirdNameEn?.Trim();
            employee.LastNameEn = dto.LastNameEn.Trim();

            employee.NationalId = dto.NationalId;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.Email = dto.Email;

            employee.Gender = dto.Gender;
            employee.BirthDate = dto.BirthDate;
            employee.HireDate = dto.HireDate;
            employee.EmploymentStatus = dto.EmploymentStatus;

            employee.DepartmentId = dto.DepartmentId;
            employee.JobTitleId = dto.JobTitleId;
            employee.ManagerId = dto.ManagerId;
        }

        private async Task ValidateManagerAsync(
     int? managerId,
     int? employeeId = null)
        {
            if (!managerId.HasValue)
                return;

            if (employeeId.HasValue &&
                managerId.Value == employeeId.Value)
            {
                throw new InvalidOperationException(
                    "Employee cannot be their own manager.");
            }

            var managerExists = await _context.Employees
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == managerId.Value &&
                    x.IsActive &&
                    x.EmploymentStatus == EmploymentStatus.Active);

            if (!managerExists)
            {
                throw new InvalidOperationException(
                    "The selected manager does not exist or is not active.");
            }
        }

        private static void Normalize(UpdateEmployeeDto dto)
        {
            dto.NationalId = dto.NationalId.Trim();
            dto.Email = dto.Email.Trim().ToLowerInvariant();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
        }
        private static void Normalize(CreateEmployeeDto dto)
        {
            dto.NationalId = dto.NationalId.Trim();
            dto.Email = dto.Email.Trim().ToLowerInvariant();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
        }

        private async Task ValidateUserAccountAsync(
    string? userId,
    int? currentEmployeeId = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return;

            var userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(x => x.Id == userId);

            if (!userExists)
                throw new InvalidOperationException(
                    "The selected user account does not exist.");

            var alreadyLinked = await _context.Employees
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    (!currentEmployeeId.HasValue ||
                     x.Id != currentEmployeeId.Value));

            if (alreadyLinked)
                throw new InvalidOperationException(
                    "The selected user account is already linked to another employee.");

            var isAdmin = await _userContextService
                .IsInRoleAsync(userId, "Admin");

            if (isAdmin)
                throw new InvalidOperationException(
                    "Admin accounts cannot be linked to employees.");
        }

    }
}
