using E_COM_DataAccess.Repository;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using E_COM_Models.ViewModels;
using E_EOM_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Stripe; // Stripe SDK for payment processing
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace E_ECOM_P.Areas.customer.Controllers
{
    [Area("customer")]

    public class CartController : Controller
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IEmailSender _emailsender;
        private readonly UserManager<IdentityUser> _userManager;
        private static bool isEmailconfirm = false;

        public CartController(IUnitofWork unitofWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitofwork = unitofWork;
            _emailsender = emailSender;
            _userManager = userManager;
        }

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public IActionResult Index()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    Listcart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }

            ShoppingCartVM = new ShoppingCartVM()
            {
                Listcart = _unitofwork.shoppingCart.GetAll(sp => sp.ApplicationUserid == claim.Value,
            includeproperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofwork.ApplicationUser.FirstorDefult(au => au.Id == claim.Value);

            foreach (var list in ShoppingCartVM.Listcart)
            {
                list.price = SD.GetPriceBasedOnQuantity(list.count, list.Product.price, list.Product.price50, list.Product.price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.count * list.price);

                if (list.Product.Description.Length > 100)
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(List<int> selectedproducts)
        {
            if (selectedproducts == null || !selectedproducts.Any())
            {
                TempData["Error"] = "Please select at least one book to place an order.";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("selectedProducts", string.Join(",", selectedproducts));
            return RedirectToAction("Summary");
        }

        public IActionResult plus(int id)
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return NotFound();

            var cart = _unitofwork.shoppingCart.get(id);
            if (cart == null || cart.ApplicationUserid != claim.Value) return NotFound();
            cart.count += 1;
            _unitofwork.save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int id)
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return NotFound();

            var cart = _unitofwork.shoppingCart.get(id);

            if (cart == null || cart.ApplicationUserid != claim.Value)
            {
                return NotFound();
            }

            if (cart.count > 1)
            {
                cart.count--;
                _unitofwork.save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult delete(int id)
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return NotFound();

            var cart = _unitofwork.shoppingCart.get(id);
            if (cart == null || cart.ApplicationUserid != claim.Value) return NotFound();
            _unitofwork.shoppingCart.Remove(cart);
            _unitofwork.save();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Customer/Cart/summary")]
        public IActionResult Summary()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null) return NotFound();

            var userId = claims.Value;

            var selectedIdsRaw = HttpContext.Session.GetString("selectedProducts");
            if (string.IsNullOrEmpty(selectedIdsRaw))
            {
                TempData["Error"] = "No products selected.";
                return RedirectToAction("Index");
            }

            var selectedIds = selectedIdsRaw.Split(',').Select(int.Parse).ToList();

            var filteredCartItems = _unitofwork.shoppingCart.GetAll(
                sc => sc.ApplicationUserid == userId && selectedIds.Contains(sc.id),
                includeproperties: "Product"
            );

            ShoppingCartVM = new ShoppingCartVM
            {
                Listcart = filteredCartItems,
                OrderHeader = new OrderHeader()
            };

            var user = _unitofwork.ApplicationUser.FirstorDefult(au => au.Id == userId);
            ShoppingCartVM.OrderHeader.ApplicationUser = user;

            ShoppingCartVM.RecentAddresses = _unitofwork.orderHeader
             .GetAll(o => o.ApplicationUserid == userId)
             .Select(o => new AddressVM
             {
                 StreetAddress = o.StreetAddress,
                 City = o.City,
                 State = o.State,
                 PostalCode = o.PostalCode
             }).Distinct().ToList();

            foreach (var list in ShoppingCartVM.Listcart)
            {
                list.price = SD.GetPriceBasedOnQuantity(list.count, list.Product.price, list.Product.price50, list.Product.price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.price * list.count);

                if (list.Product.Description.Length >= 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }

            ShoppingCartVM.OrderHeader.Name = user.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            ShoppingCartVM.OrderHeader.State = user.State;
            ShoppingCartVM.OrderHeader.City = user.City;
            ShoppingCartVM.OrderHeader.PostalCode = user.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("summary")]
        public async Task<IActionResult> SummaryPost(string paymentOption)
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null) return NotFound();

            var userId = claims.Value;

            var selectedIdsRaw = HttpContext.Session.GetString("selectedProducts");
            if (string.IsNullOrEmpty(selectedIdsRaw))
            {
                TempData["Error"] = "No products selected for order.";
                return RedirectToAction("Index");
            }

            var selectedIds = selectedIdsRaw.Split(',').Select(int.Parse).ToList();

            ShoppingCartVM.Listcart = _unitofwork.shoppingCart.GetAll(
                sc => sc.ApplicationUserid == claims.Value && selectedIds.Contains(sc.id),
                includeproperties: "Product"
            );

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofwork.ApplicationUser.FirstorDefult(au => au.Id == userId);

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserid = userId;
            _unitofwork.orderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitofwork.save();

            foreach (var list in ShoppingCartVM.Listcart)
            {
                list.price = SD.GetPriceBasedOnQuantity(list.count, list.Product.price, list.Product.price50, list.Product.price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.price * list.count);

                OrderDetail orderDetail = new OrderDetail()
                {
                    Productid = list.Productid,
                    OrderHeaderid = ShoppingCartVM.OrderHeader.Id,
                    price = list.price,
                    count = list.count,
                };

                _unitofwork.orderDetail.Add(orderDetail);
                _unitofwork.save();
            }

            _unitofwork.shoppingCart.removerange(ShoppingCartVM.Listcart);
            _unitofwork.save();

            HttpContext.Session.SetInt32(SD.ss_CartSessionCount, 0);

            if (paymentOption == "later")
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                _unitofwork.save();

                string htmlBodyLater = $@"
                <div style='font-family:Inter,Arial,sans-serif; max-width:480px; margin:auto; border:1px solid #e2e8f0; border-radius:12px; overflow:hidden;'>
                    <div style='background:#0f172a; padding:24px; text-align:center;'>
                        <h2 style='color:#38bdf8; margin:0; font-family:Georgia,serif;'>PustakGhar</h2>
                    </div>
                    <div style='padding:28px; color:#334155;'>
                        <h3 style='color:#0f172a; margin-top:0;'>Order Confirmed!</h3>
                        <p>Hi {ShoppingCartVM.OrderHeader.Name},</p>
                        <p>Your order <b>#{ShoppingCartVM.OrderHeader.Id}</b> has been placed successfully.</p>
                        <p><b>Payment Status:</b> Pay Later (due within 30 days)</p>
                        <p><b>Total Amount:</b> &#8377;{ShoppingCartVM.OrderHeader.OrderTotal}</p>
                        <p style='margin-top:24px; color:#64748b; font-size:14px;'>Thank you for shopping with PustakGhar!</p>
                    </div>
                </div>";

                await _emailsender.SendEmailAsync(
                    ShoppingCartVM.OrderHeader.ApplicationUser.Email,
                    "Order Confirmed",
                    htmlBodyLater
                );

                return RedirectToAction("OrderConfirm", "Cart", new { area = "customer", id = ShoppingCartVM.OrderHeader.Id });
            }
            else
            {
                var domain = $"{Request.Scheme}://{Request.Host}";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                    {
                        new Stripe.Checkout.SessionLineItemOptions
                        {
                            PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(ShoppingCartVM.OrderHeader.OrderTotal * 100),
                                Currency = "usd",
                                ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "PustakGhar Order #" + ShoppingCartVM.OrderHeader.Id
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = domain + "/Customer/Cart/PaymentSuccess?orderId=" + ShoppingCartVM.OrderHeader.Id,
                    CancelUrl = domain + "/Customer/Cart/Index",
                };

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
        }

        public async Task<IActionResult> PaymentSuccess(int orderId)
        {
            var orderHeader = _unitofwork.orderHeader.FirstorDefult(o => o.Id == orderId, includeproperties: "ApplicationUser");
            if (orderHeader == null) return NotFound();

            orderHeader.PaymentStatus = SD.PaymentStatusApproved;
            orderHeader.OrderStatus = SD.OrderStatusApproved;
            orderHeader.OrderDate = DateTime.Now;
            _unitofwork.save();

            string htmlBodySuccess = $@"
            <div style='font-family:Inter,Arial,sans-serif; max-width:480px; margin:auto; border:1px solid #e2e8f0; border-radius:12px; overflow:hidden;'>
                <div style='background:#0f172a; padding:24px; text-align:center;'>
                    <h2 style='color:#38bdf8; margin:0; font-family:Georgia,serif;'>PustakGhar</h2>
                </div>
                <div style='padding:28px; color:#334155;'>
                    <h3 style='color:#0f172a; margin-top:0;'>Payment Received &mdash; Order Confirmed!</h3>
                    <p>Hi {orderHeader.ApplicationUser.Name},</p>
                    <p>Your order <b>#{orderId}</b> has been placed successfully and payment was received.</p>
                    <p><b>Total Amount:</b> &#8377;{orderHeader.OrderTotal}</p>
                    <p style='margin-top:24px; color:#64748b; font-size:14px;'>Thank you for shopping with PustakGhar!</p>
                </div>
            </div>";

            await _emailsender.SendEmailAsync(
                orderHeader.ApplicationUser.Email,
                "Order Confirmed",
                htmlBodySuccess
            );

            return RedirectToAction("OrderConfirm", "Cart", new { area = "customer", id = orderId });
        }

        public IActionResult OrderConfirm(int id)
        {
            return View(id);
        }

    }

}