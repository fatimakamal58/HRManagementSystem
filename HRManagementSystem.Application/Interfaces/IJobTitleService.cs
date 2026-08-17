using HRManagementSystem.Application.DTOs.JobTitles;
using HRManagementSystem.Application.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IJobTitleService
    {
        Task<List<JobTitleDto>> GetAllAsync();
        Task<int> CreateAsync(CreateJobTitleDto dto);
        Task<JobTitleDto> GetByIdAsync(int id);
        Task UpdateAsync(UpdateJobTitleDto dto);
        Task DeleteAsync(int id);
        Task<List<LookupDto>> GetLookupAsync();
    }
}
