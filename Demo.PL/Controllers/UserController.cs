using AutoMapper;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index(string SearchValue = "") 
        {
            ViewBag.SearchValue = string.IsNullOrEmpty(SearchValue) ? "" : SearchValue;
            IEnumerable<ApplicationUser> users;
            if (String.IsNullOrEmpty(SearchValue))
                users = await _userManager.Users.ToListAsync();
            else
                users = await _userManager.Users.Where(u=>
                                                            u.NormalizedEmail.Trim().Contains(SearchValue.ToUpper().Trim())
                                                           || u.NormalizedUserName.Trim().Contains(SearchValue.ToUpper().Trim())).ToListAsync();

            var usersViewModel = _mapper.Map<List<UserViewModel>>(users);
            return View(usersViewModel);
        }

        public async Task<IActionResult> getData(string id,string viewName)
        {
            if (String.IsNullOrEmpty(id))
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if(user is null) 
                return NotFound();
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            };
            return View(viewName, userViewModel);

        } 
        public async Task<IActionResult> Details(string id)
        {
            return await getData(id, "Details");
        }
        public async Task<IActionResult> Update(string id)
        {
            return await getData(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id,UserViewModel userViewModel)
        {
            if(id !=  userViewModel.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if(user is null)
                        return NotFound();
                    else
                    {
                        user.UserName = userViewModel.UserName;
                        user.NormalizedUserName = userViewModel.UserName.ToUpper();
                        var result = await _userManager.UpdateAsync(user);
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
            return View(userViewModel);
        }
        [HttpPost]
        public async Task<JsonResult> Delete(string? id)
        {
            
            if(String.IsNullOrEmpty(id))
                return Json(new { Message = "You must send id", Status = 400 });
            var user = await _userManager.FindByIdAsync(id);
            if (user is  null)
                return Json(new { Message = "There isn't User with this Id", Status = 400 });

            var result = await _userManager.DeleteAsync(user);
            if(result.Succeeded)
                return Json(new { Message = "Success", Status = 200 });
            else
                return Json(new { Message = result.Errors, Status = 401 });
        }
    }
}
