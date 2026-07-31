using System.Diagnostics;
using System.Security.Claims;
using E_COM_DataAccess.Repository;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using E_COM_Models.ViewModels;
using E_EOM_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_ECOM_P.Areas.customer.Controllers
{
    [Area("customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;// readonly private field
        private readonly IUnitofWork _unitofwork;//readonly private field

        public HomeController(ILogger<HomeController> logger, IUnitofWork unitofWork)//This is the constructor
                                                                                     //two parameters logger,unitofwork
        {
            _logger = logger;
            _unitofwork = unitofWork;
        }

        //public IActionResult Index()
        //{//to show cart count
        //    var ClaimsIdentity = (ClaimsIdentity)User.Identity;
        //    var Claims=ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    if(Claims!=null)
        //    {
        //        var count = _unitofwork.shoppingCart.GetAll(s => s.ApplicationUserid == Claims.Value).ToList().Count;
        //        //add in session
        //        HttpContext.Session.SetInt32(SD.ss_CartSessionCount, count);

        //    }

        //    var productlist = _unitofwork.product.GetAll(includeproperties: "category,coverType");
        //    return View(productlist);
        //}
        //TO SHOW SOLD COUNT AND SORTING ORDER
        public IActionResult Index()
        {
            // --- Maintain cart count in session ---
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitofwork.shoppingCart
                    .GetAll(s => s.ApplicationUserid == claim.Value)
                    .Count();

                HttpContext.Session.SetInt32(SD.ss_CartSessionCount, count);
            }

            // --- Fetch all products ---
            var products = _unitofwork.product.GetAll(includeproperties: "category,coverType").ToList();

            // --- Fetch sales data (from OrderDetail) and calculate total sold count per product ---
            var soldData = _unitofwork.orderDetail.GetAll()
                .GroupBy(od => od.Productid)
                .Select(g => new
                {
                    ProductId = g.Key,
                    SoldCount = g.Sum(od => od.count)
                })
                .ToList();

            // --- Join products with their sold count (default = 0 if never sold) ---
            var productWithSales = products
                .GroupJoin(
                    soldData,
                    p => p.id,
                    s => s.ProductId,
                    (p, s) => new ProductWithSalesVM
                    {
                        Product = p,
                        SoldCount = s.FirstOrDefault()?.SoldCount ?? 0
                    }
                )
                .OrderByDescending(x => x.SoldCount) // Most sold first
                .ToList();

            return View(productWithSales);
        }

        public IActionResult Details(int id)
        {

            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var Claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (Claims != null)
            {
                var count = _unitofwork.shoppingCart.GetAll(s => s.ApplicationUserid == Claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ss_CartSessionCount, count);

            }
            var productindb = _unitofwork.product.FirstorDefult(p => p.id == id, includeproperties: "category,coverType");
            if (productindb == null) return NotFound();
            var shoppingcart = new ShoppingCart()
            {
                Product = productindb,
                Productid = id
            };




            return View(shoppingcart);
        }
       [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingcart)//for add to cart how it works
        {
            shoppingcart.id = 0;
            if (ModelState.IsValid)
            {
                var ClaimsIdentity=(ClaimsIdentity)User.Identity;
                var claims=ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if(claims == null) return NotFound();
                shoppingcart.ApplicationUserid=claims.Value;
               var shoppingcartindb= _unitofwork.shoppingCart.FirstorDefult(s=>s.ApplicationUserid==claims.Value 
                                   && s.Productid==shoppingcart.Productid);
                //Add
                if (shoppingcartindb == null) _unitofwork.shoppingCart.Add(shoppingcart);
                else
                {  //Update
                    shoppingcartindb.count += shoppingcart.count;
                }
                _unitofwork.save();
                return RedirectToAction("Index");

            }
            else
            {
                var productindb = _unitofwork.product.FirstorDefult(p => p.id == shoppingcart.id, includeproperties: "category,coverType");
                if(productindb == null) return NotFound();
                var shoppingcartedit = new ShoppingCart()
                {
                    Product = productindb,
                    Productid = shoppingcart.Productid
                };
                return View (shoppingcartedit);




            }






        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
