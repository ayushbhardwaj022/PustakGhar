// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using E_EOM_Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace E_ECOM_P.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;//Role
        public readonly IUnitofWork _unitofwork;// for dropdown

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole>RoleManager,
           IUnitofWork unitofWork )//constructor
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager=RoleManager;// roles
            _unitofwork = unitofWork;//initialized for dropdown
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
                ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            //Add Custom Columns
            [Required]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should only contain letters and spaces.")]
            public string Name { get; set; }
            [Required]
            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string State { get; set; }
            [Required]
            [RegularExpression(@"^\d{6}$", ErrorMessage = "Postal code must be exactly 6 digits.")]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }
            [Required]
            [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
            [Display(Name ="Company")]
            public int? CompanyId { get; set; }
            public string Role { get; set; }
            // add dropdown list for roles and companies
            public IEnumerable<SelectListItem> RoleList { get; set; }
            public IEnumerable<SelectListItem> CompanyList{ get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Input = new InputModel()// dropdown list data accesed from
            {
                CompanyList = _unitofwork.company.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                RoleList=_roleManager.Roles.Where(r=>r.Name!=SD.Role_Individual).Select(r=>r.Name).Select(rl=>new SelectListItem()
                {
                    Text=rl,
                    Value=rl
                })
            };




        }          

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                Input.Name = Input.Name?.Trim();
                Input.Email = Input.Email?.Trim();
                Input.StreetAddress = Input.StreetAddress?.Trim();
                Input.City = Input.City?.Trim();
                Input.State = Input.State?.Trim();

                //var user = CreateUser();
                var user = new ApplicationUser()
                {
                    Name=Input.Name,
                        UserName=Input.Email,
                        Email=Input.Email,
                        PhoneNumber=Input.PhoneNumber,
                        StreetAddress=Input.StreetAddress,
                        City=Input.City,
                        State=Input.State,
                        PostalCode=Input.PostalCode,
                    CompanyId = Input.CompanyId,
                        Role=Input.Role
                };

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if(!await _roleManager.RoleExistsAsync(SD.Role_Individual))//If the role "Individual" does not exist, create a new role named "Individual"
                     //and save it in the ASP.NET Identity roles table
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Individual));//This ensures the role "Individual" is added to your system only once if it doesn’t already exist.
                    }
                    if(!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Company))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
                    }
                    //Admin role to user
                    //  await _userManager.AddToRoleAsync(user, SD.Role_Admin);


                    if (Input.Role == null && Input.CompanyId == null)//agr koi role select na hua toh individual and if company id 0 se jyada to company user
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Individual);
                    }
                    else
                    {
                        if (Input.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_Company);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, Input.Role);//employee role
                        }

                    }


                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else//jb admin user ko role deta hai to vo login ho jata hai ...vo login ki bajaye user dashboard pr rahe ..is liye
                    {
                        if(Input.Role==null && Input.CompanyId == null)
                            {
                        await _signInManager.SignInAsync(user,isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }else
                        {
                            return RedirectToAction("Index", "User", new { Area = "Admin" });

                        }
                        //    await _signInManager.SignInAsync(user, isPersistent: false);
                        //return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
