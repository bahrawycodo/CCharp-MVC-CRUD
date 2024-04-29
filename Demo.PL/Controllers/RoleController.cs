using AutoMapper;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin")]

    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<RoleController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleViewModel = _mapper.Map<List<RoleViewModel>>(roles);
            return View(roleViewModel);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var role = new ApplicationRole
                    {
                        Name = roleViewModel.Name,
                        NormalizedName = roleViewModel.Name.ToUpper(),
                    };
                    var result = await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex}");
                }

            }
            return View(roleViewModel);
        }

        public async Task<IActionResult> getData(string id,string viewName)
        {
            if(String.IsNullOrEmpty(id))
                return NotFound();
            var role = await _roleManager.FindByIdAsync(id);
            if(role is null)
                return NotFound();
            var roleViewModel = new RoleViewModel
            {
                Id = id,
                Name = role.Name,
                CreateAt = role.CreateAt
            };
            return View(viewName,roleViewModel);
        }
        public async Task<IActionResult> Update(string id)
        {
            return await getData(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id, RoleViewModel roleViewModel)
        {
            if (id != roleViewModel.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(id);

                    if (role is null)
                        return NotFound();
                    else
                    {
                        role.Name = roleViewModel.Name;
                        role.NormalizedName = roleViewModel.Name.ToUpper();
                        var result = await _roleManager.UpdateAsync(role);
                        if (result.Succeeded)
                            return RedirectToAction("Index");
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                _logger.LogError(error.Description);
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex.Message}");
                }
            }

            return View(roleViewModel);
        }
        public async Task<IActionResult> Details(string id)
        {
            return await getData(id, "Details");
        }
        [HttpPost]
        public async Task<JsonResult> Delete(string? id)
        {

            if (String.IsNullOrEmpty(id))
                return Json(new { Message = "You must send id", Status = 400 });
            var user = await _roleManager.FindByIdAsync(id);
            if (user is null)
                return Json(new { Message = "There isn't Role with this Id", Status = 400 });

            var result = await _roleManager.DeleteAsync(user);
            if (result.Succeeded)
                return Json(new { Message = "Success", Status = 200 });
            else
                return Json(new { Message = result.Errors, Status = 401 });
        }
        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
        {
            ViewBag.RoleId = roleId;
            if (String.IsNullOrEmpty(roleId))
                return NotFound();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                return NotFound();
            var usersInRole = new List<UserInRoleViewModel>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                usersInRole.Add( new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name),
                });
            }
            return View(usersInRole);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string roleId, List<UserInRoleViewModel> users)
        {
            ViewBag.RoleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role is null)
                return NotFound();
            if(ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appUser = await _userManager.FindByIdAsync(user.UserId);
                    if(appUser is not null)
                    {
                        bool userInRole = await _userManager.IsInRoleAsync(appUser, role.Name);
                        if (user.IsSelected && !userInRole)
                            await _userManager.AddToRoleAsync(appUser, role.Name);
                        else if(!user.IsSelected && userInRole)
                            await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                    }

                }
                return RedirectToAction("Update", new {id= roleId });
            }
            return View(users);
        }
    }
}
