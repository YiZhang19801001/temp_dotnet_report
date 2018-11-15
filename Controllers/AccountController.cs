using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demoBusinessReport.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace demoBusinessReport.Controllers
{
    public class AccountController : Controller
    {

        #region - inject services
        private UserManager<IdentityUser> _userManagerService;
        private SignInManager<IdentityUser> _signInManagerService;
        private RoleManager<IdentityRole> _roleManagerService;

        public AccountController(UserManager<IdentityUser> userManagerService,
                                SignInManager<IdentityUser> signInManagerService,
                                RoleManager<IdentityRole> roleManagerService
                                )
        {
            _userManagerService = userManagerService;
            _signInManagerService = signInManagerService;
            _roleManagerService = roleManagerService;
        }
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(HomeIndexViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManagerService.PasswordSignInAsync(vm.user_name, vm.password, false, false);



                if (result.Succeeded)
                {
                    
                    return RedirectToAction("Create","Customer");

                }
                else
                {
                    ModelState.AddModelError("", "Username or password incorrect");
                }
            }
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManagerService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LoginRoute()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Create", "Customer");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}