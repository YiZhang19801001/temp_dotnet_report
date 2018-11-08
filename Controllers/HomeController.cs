using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demoBusinessReport.ViewModels;
using Microsoft.AspNetCore.Identity;
using demoBusinessReport.Models;
using demoBusinessReport.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace demoBusinessReport.Controllers
{
    public class HomeController : Controller
    {
        #region - inject services
        private UserManager<IdentityUser> _userManagerService;
        private SignInManager<IdentityUser> _signInManagerService;
        private IHostingEnvironment _hostingEnvironment;

        public HomeController(UserManager<IdentityUser> userManagerService,
                                SignInManager<IdentityUser> signInManagerService,
                                IHostingEnvironment hostingEnvironment)
        {
            _userManagerService = userManagerService;
            _signInManagerService = signInManagerService;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

    }
}