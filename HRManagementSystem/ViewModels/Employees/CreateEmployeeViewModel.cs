using HRManagementSystem.Application.DTOs.Employees;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRManagementSystem.Web.ViewModels.Employees
{
    public class CreateEmployeeViewModel
    {
        public CreateEmployeeDto Employee { get; set; } = new();

        public List<SelectListItem> Departments { get; set; } = [];

        public List<SelectListItem> JobTitles { get; set; } = [];
    }
}
