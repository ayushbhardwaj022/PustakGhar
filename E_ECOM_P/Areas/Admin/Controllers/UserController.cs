using E_COM_DataAccess.Data;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using E_EOM_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Protocol.Plugins;

namespace E_ECOM_P.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitofWork _unitofwork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitofWork unitofwork,ApplicationDbContext context)
        {
            _unitofwork = unitofwork;
            _context = context;

            
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var UserList = _context.ApplicationUsers.ToList();//netuser
            var RoleList = _context.Roles.ToList();//aspnetroles
            var UserRole = _context.UserRoles.ToList();//netuserroles
           foreach (var user in UserList)
            {//this code will fetch role name and company name
                var userRole = UserRole.FirstOrDefault(u=>u.UserId==user.Id);
                if (userRole != null)
                {
                    var role = RoleList.FirstOrDefault(r => r.Id == userRole.RoleId);
                    user.Role = role?.Name;
                }
                if (user.CompanyId != null)
                {
                    var company = _unitofwork.company.get(Convert.ToInt32(user.CompanyId));
                    user.Company = new Company()
                    {

                        Name = company != null ? company.Name : ""
                    };
                }
                if (user.Company == null)//when id is 0
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }             
            }
           var AdminUser=UserList.FirstOrDefault(u=>u.Role==SD.Role_Admin);
          if(AdminUser!=null)   UserList.Remove(AdminUser);
            return Json(new { data = UserList });
        }
        [HttpPost]
        public IActionResult LockUnLock([FromBody] string id)
        {
            bool islocked = false;
            var userindb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userindb == null)
            {
                return Json(new { success = false, Message = "Something Went Wrong While Lock And Unlock!!!" });

            }
            if (userindb != null && userindb.LockoutEnd > DateTime.Now)
            {
                userindb.LockoutEnd = DateTime.Now;
                islocked=false;
            }
            else
            {
                userindb.LockoutEnd = DateTime.Now.AddYears(100);
                islocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, Message = islocked == true ? "User Successfully Locked" : "User Successfully Unlocked" });
        }
        
        #endregion
        
    }
}
