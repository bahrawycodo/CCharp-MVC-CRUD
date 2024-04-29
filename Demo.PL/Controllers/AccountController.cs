using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel input)
        {
            if (ModelState.IsValid) {
                var user = new ApplicationUser
                {
                    Email = input.Email,
                    UserName = input.Email.Split("@")[0],
                    IsActive = true,
                };
                var result = await _userManager.CreateAsync(user,input.Password);
                if (result.Succeeded)
                    return RedirectToAction("Login");
                foreach (var error in result.Errors)
                {
                    //_logger.LogError(error.Description);
                    ModelState.AddModelError("",error.Description);
                }
            }
            return View(input);
        }

		public IActionResult Login()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user is null)
                    ModelState.AddModelError("", "Email Does Not Exist");
                
                if(user is not null && await _userManager.CheckPasswordAsync(user, input.Password))
                {
                    var result = await _signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
					

				}
			}
            return View(input);
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel input)
        {
           if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user is null)
                    ModelState.AddModelError("", "Email Does Not Exist");
                if(user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPawwordLink = Url.Action("ResetPassword", "Account", new {Email=input.Email, Token=token }, Request.Scheme);
                    var email = new Email
                    {
                        Title = "Reset Password",
                        Body =resetPawwordLink,
                        To = input.Email
                    };
                    // Send Email
                    EmailSettings.SendEmail(email);
                    return RedirectToAction("CompleteForgetPassword");
                }
            }           
            return View(input);
        }
        public IActionResult CompleteForgetPassword()
        {
            return View();
        }    
        public IActionResult ResetPassword(string email,string token)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user is null)
                    ModelState.AddModelError("", "Email Does not Exist");
                if(user is not null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, input.Token, input.Password);
                    if (result.Succeeded)
                        return RedirectToAction("Login");
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError(error.Description);
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(input);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
