using demoBusinessReport.Dtos;
using demoBusinessReport.Entities;
using demoBusinessReport.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using demoBusinessReport.Helpers;
using System;

namespace demoBusinessReport.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ValueController : Controller
    {
        #region - constructor, depedency injections
        private IDataService<Shop> _shopDataService;
        private IDataService<UserShop> _userShopDataService;
        private UserManager<IdentityUser> _userManagerService;
        //private IDataService<Docket> _docketDataService;
        //private IDataService<Return> _returnDataService;
        //private IDataService<ReturnsLine> _returnsLineDataService;
        //private IDataService<DocketLine> _docketLineDataService;
        //private IDataService<Stock> _stockDataService;
        //private IDataService<Audit> _auditDataService;
        //private IDataService<Staff> _staffDataService;
        //private IDataService<SalesOrder> _salesOrderDataService;


        public ValueController(IDataService<Shop> shopDataService,
            UserManager<IdentityUser> userManagerService,
            IDataService<UserShop> userShopDataService
            //IDataService<Docket> docketDataService,
            //IDataService<Return> returnDataService,
            //IDataService<ReturnsLine> returnsLineDataService,
            //IDataService<DocketLine> docketLineDataService,
            //IDataService<Stock> stockDataService,
            //IDataService<Audit> auditDataService,
            //IDataService<Staff> staffDataService,
            //IDataService<SalesOrder> salesOrderDataService
            )
        {
            _shopDataService = shopDataService;
            _userManagerService = userManagerService;
            _userShopDataService = userShopDataService;
            //_docketDataService = docketDataService;
            //_returnDataService = returnDataService;
            //_returnsLineDataService = returnsLineDataService;
            //_docketLineDataService = docketLineDataService;
            //_stockDataService = stockDataService;
            //_auditDataService = auditDataService;
            //_staffDataService = staffDataService;
            //_salesOrderDataService = salesOrderDataService;
        }
        #endregion

        #region - for index page list of shops
        [HttpGet("getshops")]
        public async Task<JsonResult> GetShops([FromHeader] string Authorization)
        {
            //task - return user shops
            //1. get shops form database
            //1-1. find user
            IdentityUser user = await getUserFromTokenHelper(Authorization);
            //1-2. find shops
            //1-2-1. find shop ids
            List<int> shop_ids = _userShopDataService.Query(shop => shop.user_id == user.Id).Select(shop => shop.shop_id).ToList();

            //1-2-2. find shop list
            List<Shop> shopList = _shopDataService.Query(s => shop_ids.Contains(s.shop_id)).ToList();

            //2. create Dto
            ShopsDto dto = new ShopsDto { shops = shopList };

            //3. return dto
            return Json(dto); 
        }
        #endregion

        #region - get summary for single shop
        [HttpPost("getsummary")]
        public async Task<JsonResult> getsummary([FromBody] SearchCondition scon)
        {

            //0. connect to shop db
            await setConnectionString(scon.ShopId); //set connection string for shop DB
            var _docketDataService = new ShopDataService<Docket>();
            var _returnDataService = new ShopDataService<Return>();
            var _returnsLineDataService = new ShopDataService<ReturnsLine>();

            //1. create dto
            SummaryDto dto = new SummaryDto();

            //2. mapping values
            IEnumerable<Docket> dockets = _docketDataService.Query(d => d.docket_date >= scon.DateFrom && d.docket_date <= scon.DateTo).ToList();
            IEnumerable<Return> returns = _returnDataService.Query(d => d.returns_date >= scon.DateFrom && d.returns_date<= scon.DateTo).ToList();
            IEnumerable<int> returns_ids = returns.Select(r => r.returns_id).ToList();
            IEnumerable<ReturnsLine> returnslines = _returnsLineDataService.Query(rl=>returns_ids.Contains(rl.returns_id));

            dto.ShopId                  = scon.ShopId;
            dto.Number_Docket           = dockets.Count();
            dto.Total_Discount          = dockets.Sum(d => d.discount);
            dto.Total_Amount            = dockets.Sum(d => d.total_inc);
            dto.Refund_Amount           = returns.Sum(r => r.total_inc);
            dto.Refund_Number_Of_Item   = returnslines.Sum(rl => rl.quantity);
            
            //return dto
            return Json(dto);
        }
        #endregion

        #region - sales by day
        [HttpPost("getbyday")]
        public async Task<IEnumerable<SalesByDayDto>> getSalesByDay([FromBody] SearchCondition scon)
        {
            List<SalesByDayDto> list = new List<SalesByDayDto>();
            //0. connect to DB
            await setConnectionString(scon.ShopId);
            var _docketDataService = new ShopDataService<Docket>();
            //1. fetch data from DB
            List<Docket> dockets = _docketDataService.Query(d => d.docket_date >= scon.DateFrom && d.docket_date <= scon.DateTo).ToList();
            List<DateTime> dates = dockets.Select(d => d.docket_date).Distinct().ToList();
            //2. create view model
            foreach (var item in dates)
            {
                SalesByDayDto vm = new SalesByDayDto();
                vm.Single_date = item;
                vm.Sum_amount = 0;
                foreach (var d in dockets)
                {
                    if (d.docket_date == item)
                    {
                        vm.Sum_amount += d.total_inc;
                    }
                }
                list.Add(vm);
            }
            //3. return view model
            return list;
        }

        #endregion

        #region - sales by category
        [HttpPost("getbycategory")]
        public async Task<JsonResult> getSalesByCategory([FromBody] SearchCondition scon)
        {
            //0. connect to shop db
            await setConnectionString(scon.ShopId);
            //return Json(cn);
            //1. create result container
            List<SalesByCategoryDto> list = new List<SalesByCategoryDto>();
            var _docketDataService = new ShopDataService<Docket>();
            var _docketLineDataService = new ShopDataService<DocketLine>();
            var _stockDataService = new ShopDataService<Stock>();

            //2. mapping value
            
            List<int> docket_ids = _docketDataService.Query(d => d.docket_date >= scon.DateFrom && d.docket_date <= scon.DateTo).Select(p => p.docket_id).ToList();
            List<DocketLine> docket_lines = _docketLineDataService.Query(dl => docket_ids.Contains(dl.docket_id)).ToList();
            List<double> item_ids = docket_lines.Select(d => d.stock_id).ToList();
            List<string> categories = _stockDataService.Query(s => item_ids.Contains(s.stock_id)).Select(si => si.cat1).Distinct().ToList();
            foreach (var cate in categories)
            {
                SalesByCategoryDto dto = new SalesByCategoryDto();
                dto.Category_name = cate;
                dto.Total_amount = 0;
                List<double> ids = _stockDataService.Query(s => s.cat1 == cate).Select(s => s.stock_id).ToList();
                foreach (var item in docket_lines)
                {
                    if (ids.Contains(item.stock_id))
                    {
                        dto.Total_amount += item.sell_inc;
                    }
                }

                list.Add(dto);
            }

            //3. return result
            return Json(list);
        }
        #endregion

        #region - sales by item
        [HttpPost("getbyitem")]
        public async Task<IEnumerable<SalesByItemDto>> getSalesByItem([FromBody] SearchCondition scon)
        {
            List<SalesByItemDto> list = new List<SalesByItemDto>();
            //0. connect to DB
            await setConnectionString(scon.ShopId);
            var _docketDataService = new ShopDataService<Docket>();
            var _docketLineDataService = new ShopDataService<DocketLine>();
            var _stockDataService = new ShopDataService<Stock>();
            //1. fetch data from DB
            List<int> ids = _docketDataService.Query(d => d.docket_date >= scon.DateFrom && d.docket_date <= scon.DateTo).Select(d => d.docket_id).ToList();
            List<DocketLine> dls = _docketLineDataService.Query(dl => ids.Contains(dl.docket_id)).ToList();
            List<double> stock_ids = _docketLineDataService.Query(dl => ids.Contains(dl.docket_id)).Select(dl => dl.stock_id).ToList();
            List<Stock> stocks = _stockDataService.Query(s => stock_ids.Contains(s.stock_id)).ToList();

            //2. create view model
            foreach (var item in stocks)
            {
                SalesByItemDto vm = new SalesByItemDto();
                vm.ItemName = item.description;
                vm.Total_amount = 0;
                vm.Quantity = 0;
                foreach (var dl in dls)
                {
                    if (dl.stock_id == item.stock_id)
                    {
                        vm.Total_amount += dl.sell_inc;
                        vm.Quantity += dl.quantity;
                    }
                }

                list.Add(vm);
            }

            //3. return view model
            return list;
        }

        #endregion

        #region - void log
        [HttpPost("getvoidlog")]
        public async Task<IEnumerable<VoidLogDto>> getVoidLog([FromBody] SearchCondition scon)
        {
            List<VoidLogDto> list = new List<VoidLogDto>();
            //0. connect to DB
            await setConnectionString(scon.ShopId);
            var _auditDataService = new ShopDataService<Audit>();
            var _stockDataService = new ShopDataService<Stock>();
            var _staffDataService = new ShopDataService<Staff>();
            //1. fetch data from DB
            List<Audit> audits = _auditDataService.Query(a => a.audit_date >= scon.DateFrom && a.audit_date <= scon.DateTo).Where(a => a.tran_type == "AV").ToList();


            //2. create view model
            foreach (var record in audits)
            {
                VoidLogDto vm = new VoidLogDto();
                vm.item_name = _stockDataService.Query(s => s.stock_id == record.stock_id).Select(s => s.description).First();

                vm.quantity = record.movement;

                Staff target_staff = _staffDataService.Query(s => s.staff_id == record.source_id).First();
                vm.staff_name = target_staff.surname + " " + target_staff.given_names;

                list.Add(vm);
            }

            //3. return view model
            return list;
        }

        #endregion

        #region
        [HttpPost("getunpaidorder")]
        public async Task<IEnumerable<UnpaidOrderDto>> getUnpaidOrder([FromBody] SearchCondition scon)
        {
            List<UnpaidOrderDto> list = new List<UnpaidOrderDto>();
            //0. connect to DB
            await setConnectionString(scon.ShopId);
            var _docketDataService = new ShopDataService<Docket>();
            var _salesOrderDataService = new ShopDataService<SalesOrder>();
            //1. fetch data from DB
            IEnumerable<Docket> dockets = await _docketDataService.GetAll();
            List<int> ids = dockets.Select(d=>d.original_id).ToList();
            List<SalesOrder> orders = _salesOrderDataService
              .Query(o => o.salesorder_date >= scon.DateFrom && o.salesorder_date <= scon.DateTo)
              .Where(o => !ids.Contains(o.salesorder_id))
              .ToList();
            List<string> customs = orders.Select(o => o.custom).Distinct().ToList();


            //2. create view model
            foreach (var custom in customs)
            {
                UnpaidOrderDto vm = new UnpaidOrderDto();
                vm.customer = custom;
                vm.total_amount = 0;

                foreach (var order in orders)
                {
                    if (order.custom == custom)
                    {
                        vm.total_amount += order.total_inc;
                    }
                }

                list.Add(vm);
            }

            //3. return view model
            return list;
        }

        #endregion

        #region - private helper methods
        private async Task<IdentityUser> getUserFromTokenHelper(string Authorization)
        {
            //cut the prefix "bearer " to get real token string
            string token = Authorization.Substring(7);
            //decode token string to token object
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            #region- only for testing & debug
            //return tokenS
            #endregion
            //find value in token object
            var username = tokenS.Claims.First(claim => claim.Type == "unique_name").Value;
            //find user by calling identity framework build-in method
            IdentityUser user = await _userManagerService.FindByIdAsync(username);
            //return result
            return user;
        }
        private async Task<string> setConnectionString(int id)
        {
            //1. find shop 
            Shop select_shop = await _shopDataService.GetSingle(s => s.shop_id == id);
            //2. get connection string
            //string select_shop_db_server = select_shop.Server;
            //string select_shop_db_database = select_shop.Database;
            //string cn = "Server=" + select_shop_db_server + "; Database=" + select_shop_db_database + ";Trusted_Connection=True; ConnectRetryCount=0";

            string cn = select_shop.db_path + select_shop.db_password;
            //3. set connection string
            DataHelper.con = cn;
            //4. return for async
            return cn;
        }
        #endregion
    }
}