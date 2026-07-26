using HRManagementSystem.Application.DTOs.EmployeeSalaries;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Infrastructure.Services
{
    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeSalaryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<EmployeeSalaryDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeSalaryDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(CreateEmployeeSalaryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UpdateEmployeeSalaryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
