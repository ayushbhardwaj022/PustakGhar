using E_COM_Models;
using E_ECOM_P.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_ECOM_P.Areas.customer.Controllers
{
    [Area("customer")]

    public class OrderController : Controller
    {

        private readonly IEmailservice _email;
        private readonly ISmsService _sms;
        private readonly IVoiceservice _call;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(IEmailservice email, ISmsService sms, IVoiceservice call, UserManager<IdentityUser> userManager)
        {
            _email = email;
            _sms = sms;
            _call = call;
            _userManager = userManager;
        }


        //public IActionResult OrderConfirm(int orderId)
        //{
        //    return View(orderId);   // passes orderId to the View as model
        //}
        public async Task<IActionResult> OrderConfirm(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            string UserEmail = user.Email;
            string UserPhone = user.PhoneNumber;     // ← AUTOMATICALLY GETS LOGGED-IN USER PHONE

            await _email.SendEmail(UserEmail, "Order Confirmed", $"Order #{orderId} placed successfully");
            await _sms.SendSms(UserPhone, $"Order #{orderId} confirmed");
            await _call.MakeCall(UserPhone, $"Your order {orderId} has been placed successfully");
            return View("OrderConfirm", orderId);
        }
    }
}
