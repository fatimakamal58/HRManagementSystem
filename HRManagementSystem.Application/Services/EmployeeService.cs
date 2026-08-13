using HRManagementSystem.Application.DTOs.Employees;
using HRManagementSystem.Application.Exceptions;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }
       
        public async Task CreateAsync(CreateEmployeeDto dto)
        {
            Normalize(dto);

            await ValidateDuplicateAsync(
                dto.EmployeeNumber,
                dto.NationalId,
                dto.Email,
                dto.PhoneNumber);

            await ValidateReferencesAsync(
                dto.DepartmentId,
                dto.JobTitleId);

            var employee = new Employee();

            MapEmployee(employee, dto);

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

                    DepartmentName = e.Department.NameAr,
                    JobTitleName = e.JobTitle.NameAr,
                    HireDate = e.HireDate,
                    EmploymentStatus = e.EmploymentStatus
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == id && e.IsActive)
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
                    JobTitleName = e.JobTitle.NameAr
                })
                .FirstOrDefaultAsync();

            if (employee is null)
            {
                throw new NotFoundException("الموظف غير موجود.");
            }

            return employee;
        }

        public async Task<EmployeeFormDto> GetCreateFormAsync()
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .Where(d => d.IsActive)
                .OrderBy(d => d.NameAr)
                .Select(d => new LookupDto
                {
                    Id = d.Id,
                    Name = d.NameAr
                })
                .ToListAsync();

            var jobTitles = await _context.JobTitles
                .AsNoTracking()
                .Where(j => j.IsActive)
                .OrderBy(j => j.NameAr)
                .Select(j => new LookupDto
                {
                    Id = j.Id,
                    Name = j.NameAr
                })
                .ToListAsync();

            return new EmployeeFormDto
            {
                Departments = departments,
                JobTitles = jobTitles
            };
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
                    JobTitleId = e.JobTitleId
                })
                .FirstOrDefaultAsync();

            if (employee is null)
            {
                throw new NotFoundException("الموظف غير موجود.");
            }

            return employee;
        }
        public async Task<UpdateEmployeeFormDto> GetEditViewModelAsync(int id)
        {
            var employee = await GetForUpdateAsync(id);

            var formData = await GetFormDataAsync();

            return new UpdateEmployeeFormDto
            {
                Employee = employee,
                Departments = formData.Departments,
                JobTitles = formData.JobTitles
            };
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
                dto.EmployeeNumber,
                dto.NationalId,
                dto.Email,
                dto.PhoneNumber,
                dto.Id);

            await ValidateReferencesAsync(
                dto.DepartmentId,
                dto.JobTitleId);

            MapEmployee(employee, dto);
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


        private async Task<EmployeeFormDto> GetFormDataAsync()
        {
            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .Select(d => new LookupDto { Id = d.Id, Name = d.NameAr })
                .ToListAsync();

            var jobTitles = await _context.JobTitles
                .Where(j => j.IsActive)
                .Select(j => new LookupDto { Id = j.Id, Name = j.NameAr })
                .ToListAsync();

            return new EmployeeFormDto
            {
                Departments = departments,
                JobTitles = jobTitles
            };
        }

        private async Task ValidateDuplicateAsync(
            string employeeNumber,
            string nationalId,
            string email,
            string phoneNumber,
            int? excludedEmployeeId = null)
        {
            var employeeNumberExists = await _context.Employees
                .AnyAsync(e =>
                    e.EmployeeNumber == employeeNumber &&
                    (!excludedEmployeeId.HasValue || e.Id != excludedEmployeeId.Value));

            if (employeeNumberExists)
                throw new DuplicateException("رقم الموظف مستخدم مسبقًا.");

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
            employee.EmployeeNumber = dto.EmployeeNumber;

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
        }
        private static void MapEmployee(
            Employee employee,
            CreateEmployeeDto dto)
        {
            employee.EmployeeNumber = dto.EmployeeNumber;

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
        }

        private static void Normalize(UpdateEmployeeDto dto)
        {
            dto.EmployeeNumber = dto.EmployeeNumber.Trim();
            dto.NationalId = dto.NationalId.Trim();
            dto.Email = dto.Email.Trim().ToLowerInvariant();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
        }
        private static void Normalize(CreateEmployeeDto dto)
        {
            dto.EmployeeNumber = dto.EmployeeNumber.Trim();
            dto.NationalId = dto.NationalId.Trim();
            dto.Email = dto.Email.Trim().ToLowerInvariant();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
        }

    }
}
