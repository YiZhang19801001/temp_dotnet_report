using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demoBusinessReport.ViewModels;
using demoBusinessReport.Services;
using demoBusinessReport.Entities;

namespace demoBusinessReport.Controllers
{
    public class ShopController : Controller
    {
        private IDataService<Shop> _shopDataService;

        public ShopController(IDataService<Shop> shopDataService)
        {
            _shopDataService = shopDataService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ShopCreateViewModel vm)
        {
            //1. create row in database
            //1-1. new shop obj
            Shop new_shop = new Shop {
                db_password = vm.db_password,
                db_path = vm.db_path,
                shop_name = vm.shop_name
      
            };
            //1-2. save to DB
            _shopDataService.Create(new_shop);

            return View(vm);
        }
    }
}