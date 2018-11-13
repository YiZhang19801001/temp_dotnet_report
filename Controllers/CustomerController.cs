using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demoBusinessReport.ViewModels;
using Microsoft.AspNetCore.Identity;
using demoBusinessReport.Services;
using demoBusinessReport.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace demoBusinessReport.Controllers
{
    public class CustomerController : Controller
    {
        #region - injection
        private UserManager<IdentityUser> _userManagerService;
        private SignInManager<IdentityUser> _signInManagerService;

        private IDataService<Shop> _shopDataService;
        private IDataService<UserShop> _userShopDataService;
       

        public CustomerController(UserManager<IdentityUser> userManagerService,
                                SignInManager<IdentityUser> signInManagerService,
                                IDataService<Shop> shopDataService,
                                IDataService<UserShop> userShopDataService
                               )
        {
            _userManagerService = userManagerService;
            _signInManagerService = signInManagerService;
            _shopDataService = shopDataService;
            _userShopDataService = userShopDataService;
        }
        #endregion
        #region - create new customer
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IdentityUser user = new IdentityUser(vm.user_name);
                    IdentityResult result = await _userManagerService.CreateAsync(user, vm.password);

                    if (result.Succeeded)
                    {
                        await _userManagerService.AddToRoleAsync(user, "Customer");

                        //get the user back to find the id
                        user = await _userManagerService.FindByNameAsync(vm.user_name);

                        //return to home page
                        return RedirectToAction("create", "customer");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    throw;
                }

            }
            return View(vm);
        }
        #endregion
        #region - link shop to customer
        [HttpGet]
        public async Task<IActionResult> Link() {

            //1. get data from DB [customer_list, shop_list]
            List<SelectListItem> customers = new List<SelectListItem>();
            List<ShopShortInfo> shop_infos = new List<ShopShortInfo>();
            IEnumerable<IdentityUser> users = await _userManagerService.GetUsersInRoleAsync("Customer");
            foreach (var user in users)
            {
                SelectListItem customer = new SelectListItem { Value = user.Id, Text = user.UserName };
                customers.Add(customer);
            }

            IEnumerable<Shop> shops = await _shopDataService.GetAll();
            foreach (var shop in shops)
            {
                ShopShortInfo info = new ShopShortInfo { shop_id = shop.shop_id, isPicked=false,shop_name=shop.shop_name};
                shop_infos.Add(info);
            }
            //2. create view model and mapping data
            CustomerLinkViewModel vm = new CustomerLinkViewModel { customer_list=customers,shop_info_list=shop_infos};
            //3. return view model to view
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Link(CustomerLinkViewModel vm) {
            /**0. aim -> add new record to UserShop{user_id, shop_id} table
             * read user_id straight away from vm,
             * loop the shoplist to found out with shop is marked isPicked true
             */
            //1. read data from request
            string user_id = vm.customer_id;
            //1-1. create filter
            IEnumerable<UserShop>  query  =await _userShopDataService.Query(us => us.user_id == user_id);
            List<int> exist_shopIds = query.Select(us => us.shop_id).ToList();
            //2. update db
            foreach (var shop in vm.shop_info_list)
            {
                if (shop.isPicked == true && !exist_shopIds.Contains(shop.shop_id))
                {
                    UserShop new_usershop = new UserShop();
                    new_usershop.user_id = user_id;
                    new_usershop.shop_id = shop.shop_id;
                    await _userShopDataService.Create(new_usershop);
                }
            }
            //3. return to view add more
            return View(vm);
        }
        #endregion
    }
}