using HRManagementSystem.Infrastructure.Identity;
using HRManagementSystem.Infrastructure.Identity.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(
        IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateUserDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _userManagementService.CreateAsync(model);

            TempData["SuccessMessage"] =
                "تم إنشاء حساب المستخدم بنجاح.";

            return RedirectToAction(nameof(Create));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(model);
        }
    }
}