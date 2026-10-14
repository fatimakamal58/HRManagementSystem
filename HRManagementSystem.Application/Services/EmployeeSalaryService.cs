using HRManagementSystem.Application.DTOs.EmployeeSalaries;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Infrastructure.Data;
using HRManagementSystem.Application.Exceptions;
using HRManagementSystem.Domain.Entities.HR;
using HRManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.Application.Services
{
    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeSalaryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateInitialSalaryAsync(
            CreateInitialSalaryDto dto)
        {
            ValidateSalary(
                dto.BasicSalary,
                dto.EffectiveFrom);

            await ValidateActiveEmployeeAsync(dto.EmployeeId);

            var hasSalary = await _context.EmployeeSalaries
                .AnyAsync(s => s.EmployeeId == dto.EmployeeId);

            if (hasSalary)
            {
                throw new DuplicateException(
                    "Employee already has an initial salary.");
            }

            var salary = new EmployeeSalary
            {
                EmployeeId = dto.EmployeeId,
                BasicSalary = dto.BasicSalary,
                EffectiveFrom = dto.EffectiveFrom.Date,
                EffectiveTo = null,
                ChangeReason = SalaryChangeReason.NewHire,
                Notes = dto.Notes?.Trim()
            };

            _context.EmployeeSalaries.Add(salary);

            await _context.SaveChangesAsync();
        }

        public async Task ChangeSalaryAsync(ChangeEmployeeSalaryDto dto)
        {
            ValidateSalary(
                dto.BasicSalary,
                dto.EffectiveFrom);

            await ValidateActiveEmployeeAsync(dto.EmployeeId);

            ValidateChangeReason(dto.ChangeReason);

            var latestSalary =
                await GetLatestSalaryAsync(dto.EmployeeId);

            if (dto.EffectiveFrom.Date <= latestSalary.EffectiveFrom.Date)
            {
                throw new ArgumentException(
                    "The new salary effective date must be after the latest salary effective date.");
            }

            latestSalary.EffectiveTo =
                dto.EffectiveFrom.Date.AddDays(-1);

            var newSalary = new EmployeeSalary
            {
                EmployeeId = dto.EmployeeId,
                BasicSalary = dto.BasicSalary,
                EffectiveFrom = dto.EffectiveFrom.Date,
                EffectiveTo = null,
                ChangeReason = dto.ChangeReason,
                Notes = dto.Notes?.Trim()
            };

            _context.EmployeeSalaries.Add(newSalary);

            await _context.SaveChangesAsync();
        }

        public async Task<EmployeeSalaryDto?> GetCurrentSalaryAsync(int employeeId)
        {
            await ValidateActiveEmployeeAsync(employeeId);
            
            var today = DateTime.Today;

            return await _context.EmployeeSalaries
                .AsNoTracking()
                .Where(s =>
                    s.EmployeeId == employeeId &&
                    s.EffectiveFrom <= today &&
                    (s.EffectiveTo == null || s.EffectiveTo >= today))
                .OrderByDescending(s => s.EffectiveFrom)
                .Select(s => new EmployeeSalaryDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    BasicSalary = s.BasicSalary,
                    EffectiveFrom = s.EffectiveFrom,
                    EffectiveTo = s.EffectiveTo,
                    ChangeReason = s.ChangeReason,
                    Notes = s.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmployeeSalaryDto>> GetSalaryHistoryAsync(int employeeId)
        {
            await ValidateEmployeeExistsAsync(employeeId);

            return await _context.EmployeeSalaries
                .AsNoTracking()
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.EffectiveFrom)
                .Select(s => new EmployeeSalaryDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    BasicSalary = s.BasicSalary,
                    EffectiveFrom = s.EffectiveFrom,
                    EffectiveTo = s.EffectiveTo,
                    ChangeReason = s.ChangeReason,
                    Notes = s.Notes
                })
                .ToListAsync();
        }
        
        //********** private methods ***********
        private static void ValidateSalary(
            decimal basicSalary,
            DateTime effectiveFrom)
        {
            if (basicSalary <= 0)
            {
                throw new ArgumentException(
                    "Basic salary must be greater than zero.");
            }

            if (effectiveFrom == default)
            {
                throw new ArgumentException(
                    "Effective date is required.");
            }
        }
        
        private async Task ValidateActiveEmployeeAsync(int employeeId)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e =>
                    e.Id == employeeId &&
                    e.IsActive);

            if (!employeeExists)
            {
                throw new NotFoundException(
                    "Employee not found.");
            }
        }
        
        private async Task ValidateEmployeeExistsAsync(int employeeId)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.Id == employeeId);

            if (!employeeExists)
            {
                throw new NotFoundException(
                    "Employee not found.");
            }
        }
        
        private async Task<EmployeeSalary> GetLatestSalaryAsync(int employeeId)
        {
            var latestSalary = await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (latestSalary is null)
            {
                throw new NotFoundException(
                    "Employee does not have an existing salary.");
            }

            return latestSalary;
        }
        
        private static void ValidateChangeReason(
            SalaryChangeReason changeReason)
        {
            if (!Enum.IsDefined(changeReason))
            {
                throw new ArgumentException(
                    "Invalid salary change reason.");
            }
            if (changeReason == SalaryChangeReason.NewHire)
            {
                throw new ArgumentException(
                    "NewHire cannot be used when changing an existing salary.");
            }
        }
    }
}
