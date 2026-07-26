using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.DTOs.JobTitles
{
    public class JobTitleDto
    {
        public int Id { get; set; }

        public string NameAr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }

        public string? DescriptionEn { get; set; }

        public bool IsActive { get; set; }
    }
}
