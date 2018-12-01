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
    [Authorize]
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
            IEnumerable<UserShop> query_user_shop = await _userShopDataService.Query(shop => shop.user_id == user.Id);
            List<int> shop_ids = query_user_shop.Select(shop => shop.shop_id).ToList();

            //1-2-2. find shop list
            IEnumerable<Shop> query_shop = await _shopDataService.Query(s => shop_ids.Contains(s.shop_id));
            List<Shop> shopList = query_shop.ToList();

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

            //1. create dto
            SummaryDto dto = new SummaryDto();

            //2. call helper method to get data set
            SummaryDetailDto select_detail = await GetSummaryHelper(scon.ShopId, scon.DateFrom, scon.DateTo);
            SummaryDetailDto compare_detail = await GetSummaryHelper(scon.ShopId, scon.Compare_DateFrom, scon.Compare_DateTo);

            //3. mapping values
            dto.ShopId = scon.ShopId;

            dto.Summary_Items = new List<SummaryItem>();

            dto.Summary_Items.Add(new SummaryItem
            {
                Name = "Total Sales",
                Compare_Value = compare_detail.Total_Sales,
                Value = select_detail.Total_Sales
            });

            dto.Summary_Items.Add(new SummaryItem
            {
                Name = "Number Of Transactions",
                Compare_Value = compare_detail.Number_Of_Transactions,
                Value = select_detail.Number_Of_Transactions
            });

            dto.Summary_Items.Add(new SummaryItem
            {
                Name = "Total Refund",
                Compare_Value = compare_detail.Total_Refund,
                Value = select_detail.Total_Refund
            });

            dto.Summary_Items.Add(new SummaryItem
            {
                Name = "Total Discount",
                Compare_Value = compare_detail.Total_Discount,
                Value = select_detail.Total_Discount
            });

            dto.Summary_Items.Add(new SummaryItem
            {
                Name = "Avg. Sales Value",
                Compare_Value = compare_detail.Avg_Sales_Value,
                Value = select_detail.Avg_Sales_Value
            });

            dto.Summary_Items.Add(new SummaryItem
            {
                Name = "Avg. Item Per Sale",
                Compare_Value = compare_detail.Avg_Item_Per_Sale,
                Value = select_detail.Avg_Item_Per_Sale
            });

            dto.Hourly_Summary = new HourlySummary
            {
                Name = "Sales by Hour",
                Compare_Value = compare_detail.Hourly_Sales,
                Value = select_detail.Hourly_Sales
            };

            dto.Payment_Summary = new PaymentSummary
            {
                Name = "Payment Summary",
                Compare_Value = compare_detail.PaymentSum,
                Value = select_detail.PaymentSum
            };

            dto.Custom_Data_Group = new CustomDataGroup
            {
                Name = "Custom Data Group",
                Compare_Value = compare_detail.CustomDataSum,
                Value = select_detail.CustomDataSum
            };

            //return dto
            return Json(dto);
        }

        private async Task<SummaryDetailDto> GetSummaryHelper(int shopId, DateTime DateFrom, DateTime DateTo)
        {
            //0. connect to shop db

            var _docketDataService = new ShopDataService<Docket>();
            var _returnDataService = new ShopDataService<Return>();
            var _returnsLineDataService = new ShopDataService<ReturnsLine>();
            var _docketLineDataService = new ShopDataService<DocketLine>();
            var _paymentDataService = new ShopDataService<Payments>();
            var _stockDataService = new ShopDataService<Stock>();

            //1. create dto
            SummaryDetailDto dto = new SummaryDetailDto();

            //2. fetch values from DB
            IEnumerable<Docket> dockets = await _docketDataService.Query(d => d.docket_date >= DateFrom && d.docket_date <= DateTo);
            IEnumerable<int> docket_ids = dockets.Select(d => d.docket_id).ToList();
            IEnumerable<Return> returns = await _returnDataService.Query(d => d.returns_date >= DateFrom && d.returns_date <= DateTo);
            IEnumerable<int> returns_ids = returns.Select(r => r.returns_id).ToList();
            IEnumerable<ReturnsLine> returnslines = await _returnsLineDataService.Query(rl => returns_ids.Contains(rl.returns_id));
            IEnumerable<DocketLine> docketLines = await _docketLineDataService.Query(dl => docket_ids.Contains(dl.docket_id));
            

            #region - calculate Avg_Item_Per_Sale
            /** calculate Avg_Item_Per_Sale */
            List<double> sum_sales_by_quantity = new List<double>(); //mean: save the total quantity for every sales
            foreach (var temp_id in docket_ids)
            {
                double sum_dl = 0;
                foreach (var dl in docketLines)
                {
                    if (temp_id == dl.docket_id)
                    {
                        sum_dl += dl.quantity;
                    }
                }
                sum_sales_by_quantity.Add(sum_dl);
            }

            //will be called in value mapping of dto.Avg_Item_Per_Sale
            #endregion

            #region - get payment data
            IEnumerable<Payments> payments = await _paymentDataService.Query(p => p.docket_date >= DateFrom && p.docket_date <= DateTo);

            List<PaymentDetail> paymentDetails = new List<PaymentDetail>();

            foreach (var item in payments.GroupBy(p => p.paymenttype))
            {
                PaymentDetail pd = new PaymentDetail();
                pd.paymenttype = item.Select(i => i.paymenttype).First();
                pd.amount = item.Sum(i => i.amount);
                paymentDetails.Add(pd);
            }
            #endregion

            #region - create hourly summary
            double[] arr_hour_sale = new double[24];
            for (int i = 1; i < 25; i++)
            {
                arr_hour_sale[i - 1] = (double)dockets.Where(d => d.docket_date.Hour > i - 1 && d.docket_date.Hour <= i).Sum(d => d.total_inc);
            }
            #endregion

            #region -custom data group
            var custom1_db = await _stockDataService.GetSingle(s => s.custom1 != null);
            var custom2_db = await _stockDataService.GetSingle(s => s.custom2 != null);


            CustomDataItem custom1 = new CustomDataItem
            {
                Name = custom1_db.custom1,
                Quantity = 0,
                Amount = 0
            };

            CustomDataItem custom2 = new CustomDataItem
            {
                Name = custom2_db.custom2,
                Quantity = 0,
                Amount = 0
            };

            CustomDataItem others = new CustomDataItem
            {
                Name = "others",
                Quantity = 0,
                Amount = 0
            };

            foreach (var dl in docketLines)
            {
                Stock stock_row =await _stockDataService.GetSingle(s => s.stock_id == dl.stock_id);
                if (stock_row.cat1 != "TASTE" && stock_row.cat1 != "EXTRA")
                {
                    if (dl.size_level == 1)
                    {
                        custom1.Quantity = custom1.Quantity + dl.quantity;
                        custom1.Amount = custom1.Amount + (double)dl.sell_inc * dl.quantity;
                    }
                    else if (dl.size_level == 2)
                    {
                        custom2.Quantity = custom2.Quantity + dl.quantity;
                    }
                    else
                    {
                        others.Quantity = others.Quantity + dl.quantity;
                    }
                }
                
            }
            List<CustomDataItem> customer_data_sum = new List<CustomDataItem>();
            customer_data_sum.Add(custom1);
            customer_data_sum.Add(custom2);
            customer_data_sum.Add(others);

            #endregion
            dto.Total_Sales = (double)dockets.Sum(d => d.total_inc);
            dto.Number_Of_Transactions = dockets.Count();
            dto.Total_Refund = (double)returns.Sum(r => r.total_inc);
            dto.Total_Discount = (double)dockets.Sum(d => d.discount);
            if (dto.Number_Of_Transactions == 0)
            {
                dto.Avg_Sales_Value = 0;
            }
            else
            {
                dto.Avg_Sales_Value = Math.Round(dto.Total_Sales / dto.Number_Of_Transactions, 2);
            }
            dto.Avg_Item_Per_Sale =(sum_sales_by_quantity.Count()>0)?Math.Round(sum_sales_by_quantity.Average(), 2):0;
            dto.Hourly_Sales = arr_hour_sale;
            dto.PaymentSum = paymentDetails;
            dto.CustomDataSum = customer_data_sum;
            //return dto
            return dto;
        }
        #endregion

        #region - sales by day
        [HttpPost("getbyday")]
        public async Task<IEnumerable<SalesByDateWithCompareDto>> getSalesByDay([FromBody] SearchCondition scon)
        {
            List<SalesByDateWithCompareDto> list = new List<SalesByDateWithCompareDto>();
            //0. connect to DB
            await setConnectionString(scon.ShopId);
            var _docketDataService = new ShopDataService<Docket>();
            //1. fetch data from DB
            IEnumerable<Docket> dockets = await _docketDataService.Query(d => d.docket_date >= scon.DateFrom && d.docket_date <= scon.DateTo);
            IEnumerable<Docket> dockets_compared = await _docketDataService.Query(d => d.docket_date >= scon.Compare_DateFrom&& d.docket_date <= scon.Compare_DateTo);

            List<string> dates = dockets.Select(d =>d.docket_date.ToShortDateString()).Distinct().ToList();
            List<string> dates_compared = dockets_compared.Select(d => d.docket_date.ToShortDateString()).Distinct().ToList();
            //2. create dto

            for (int i = 0; i <dates.Count()-1; i++)
            {
                SalesByDateWithCompareDto dto_res = new SalesByDateWithCompareDto();
                SalesByDayDto dto = new SalesByDayDto();
                dto.Single_date = dates[i];
                dto.Sum_amount = 0;
                foreach (var d in dockets)
                {
                    if (d.docket_date.ToShortDateString() == dates[i])
                    {
                        dto.Sum_amount += d.total_inc;
                    }
                }

                SalesByDayDto dto_compared = new SalesByDayDto();
                dto_compared.Single_date = dates_compared[i];
                dto_compared.Sum_amount = 0;
                foreach (var dc in dockets_compared)
                {
                    if (dc.docket_date.ToShortDateString() == dates_compared[i])
                    {
                        dto_compared.Sum_amount += dc.total_inc;
                    }
                }
                dto_res.Id = i;
                dto_res.Value = dto;
                dto_res.Value_Compared = dto_compared;

                list.Add(dto_res);
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
            IEnumerable<Docket> query_docket = await _docketDataService.Query(d => d.docket_date >= scon.DateFrom && d.docket_date <= scon.DateTo);
            List<int> docket_ids = query_docket.Select(p => p.docket_id).ToList();
            IEnumerable<Docket> query_docket_compare = await _docketDataService.Query(d => d.docket_date >= scon.Compare_DateFrom && d.docket_date <= scon.Compare_DateTo);
            List<int> docket_ids_compare = query_docket_compare.Select(p => p.docket_id).ToList();

            IEnumerable<DocketLine> docket_lines = await _docketLineDataService.Query(dl => docket_ids.Contains(dl.docket_id));
            List<double> item_ids = docket_lines.Select(d => d.stock_id).ToList();
            IEnumerable<DocketLine> docket_lines_compare = await _docketLineDataService.Query(dl => docket_ids_compare.Contains(dl.docket_id));

            IEnumerable<Stock> query_stock = await _stockDataService.Query(s => item_ids.Contains(s.stock_id));
            List<string> categories = query_stock.Select(si => si.cat1).Distinct().ToList();


            foreach (var cate in categories)
            {
                SalesByCategoryDto dto = new SalesByCategoryDto();
                dto.Category_name = cate;
                dto.Total_amount = 0;
                dto.Total_amount_compare = 0;

                List<double> ids = query_stock.Where(s => s.cat1 == cate).Select(s => s.stock_id).ToList();
                foreach (var item in docket_lines)
                {
                    if (ids.Contains(item.stock_id))
                    {
                        dto.Total_amount += item.sell_inc;
                    }
                }
                foreach (var item in docket_lines_compare)
                {
                    if (ids.Contains(item.stock_id))
                    {
                        dto.Total_amount_compare += item.sell_inc;
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
            //1. create dto
            var _docketDataService = new ShopDataService<Docket>();
            var _docketLineDataService = new ShopDataService<DocketLine>();
            var _stockDataService = new ShopDataService<Stock>();
            //1. fetch data from DB
            IEnumerable<Docket> query_docket = await _docketDataService.Query(d => d.docket_date >= scon.DateFrom && d.docket_date <= scon.DateTo);
            List<int> ids = query_docket.Select(d => d.docket_id).ToList();
            IEnumerable<Docket> query_docket_compare = await _docketDataService.Query(d => d.docket_date >= scon.Compare_DateFrom&& d.docket_date <= scon.Compare_DateTo);
            List<int> ids_compare = query_docket_compare.Select(d => d.docket_id).ToList();
            IEnumerable<DocketLine> dls = await _docketLineDataService.Query(dl => ids.Contains(dl.docket_id));
            IEnumerable<DocketLine> dls_compare = await _docketLineDataService.Query(dl => ids_compare.Contains(dl.docket_id));
            List<double> stock_ids = dls.Where(dl => ids.Contains(dl.docket_id)).Select(dl => dl.stock_id).ToList();
            IEnumerable<Stock> stocks = await _stockDataService.Query(s => stock_ids.Contains(s.stock_id));

            //2. create view model
            foreach (var item in stocks)
            {
                SalesByItemDto dto = new SalesByItemDto();
                dto.ItemName = item.description;
                dto.DataSet = new SalesItem();
                dto.DataSet.Quantity = 0;
                dto.DataSet.Total_amount = 0;
                dto.DataSet_Compared = new SalesItem();
                dto.DataSet_Compared.Quantity = 0;
                dto.DataSet_Compared.Total_amount = 0;
                foreach (var dl in dls)
                {
                    if (dl.stock_id == item.stock_id)
                    {
                        dto.DataSet.Quantity += dl.quantity;
                        dto.DataSet.Total_amount += dl.sell_inc;
                    }
                }
                foreach (var dl_compare in dls_compare)
                {
                    if (dl_compare.stock_id == item.stock_id)
                    {
                        dto.DataSet_Compared.Quantity += dl_compare.quantity;
                        dto.DataSet_Compared.Total_amount += dl_compare.sell_inc;
                    }
                }

                list.Add(dto);
            }

            //3. return view model
            return list.OrderByDescending(l=>l.DataSet.Total_amount).Take(20);
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
            IEnumerable<Audit> audits = await _auditDataService.Query(a => a.audit_date >= scon.DateFrom && a.audit_date <= scon.DateTo && a.tran_type == "AV");

            //2. create view model
            foreach (var record in audits)
            {
                VoidLogDto vm = new VoidLogDto();
                Stock target_stock = await _stockDataService.GetSingle(s => s.stock_id == record.stock_id);
                vm.item_name = target_stock.description;

                vm.quantity = record.movement;

                Staff target_staff = await _staffDataService.GetSingle(s => s.staff_id == record.source_id);
                vm.staff_name = target_staff.surname + " " + target_staff.given_names;

                list.Add(vm);
            }

            //3. return view model
            return list;
        }

        #endregion

        #region - get unpaid order
        [HttpPost("getunpaidorder")]
        public async Task<JsonResult> getUnpaidOrder([FromBody] SearchCondition scon)
        {
            List<UnpaidOrderDto> list = new List<UnpaidOrderDto>();
            //0. connect to DB
            await setConnectionString(scon.ShopId);

            var _salesOrderDataService = new ShopDataService<SalesOrder>();

            //1. fetch data from DB

            IEnumerable<SalesOrder> orders = await _salesOrderDataService
              .Query(o=>o.salesorder_date > scon.DateFrom && o.status<10);

            List<string> customs = orders.Select(o => o.custom).Distinct().ToList();

            decimal sum = orders.Sum(o=>o.total_inc);
            

            //2. create view model
            foreach (var custom in customs)
            {
                UnpaidOrderDto dto = new UnpaidOrderDto();
                dto.customer = custom;
                dto.total_amount = 0;

                foreach (var order in orders)
                {
                    if (order.custom == custom)
                    {
                        dto.total_amount += order.total_inc;
                    }
                }

                list.Add(dto);
            }

            //3. return view model
            return Json(new {sum_total = sum,detail_list = list});
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