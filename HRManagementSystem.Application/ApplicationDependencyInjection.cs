using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // Add application services
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IJobTitleService, JobTitleService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IEmployeeSalaryService, EmployeeSalaryService>();
            return services;
        }
    }
}
