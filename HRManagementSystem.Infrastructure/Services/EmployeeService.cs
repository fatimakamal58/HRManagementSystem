using HRManagementSystem.Application.DTOs.Employees;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Infrastructure.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task CreateAsync(CreateEmployeeDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmployeeDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UpdateEmployeeDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
