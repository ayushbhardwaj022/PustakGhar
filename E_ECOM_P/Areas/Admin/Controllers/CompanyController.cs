using E_COM_DataAccess.Repository;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using E_EOM_Utility;

namespace E_ECOM_P.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    
    public class CompanyController : Controller
    {
        private readonly IUnitofWork _unitofwork;
            public CompanyController(IUnitofWork unitofWork)
            {
            _unitofwork = unitofWork;
            }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new {data=_unitofwork.company.GetAll()});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyindb = _unitofwork.company.get(id);
            if (companyindb == null)
                return Json(new { success = false, Message = "Unable to Delete Data!!!!!" });
            _unitofwork.company.Remove(companyindb);
            _unitofwork.save();
            return Json(new { success = true, Message = "Data Deleted Successfully" });
            
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Company company=new Company();
            if(id==null)return View(company);
            company=_unitofwork.company.get(id.GetValueOrDefault());
            if(company==null)return NotFound();
            return View(company);
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if(company==null)return BadRequest();
            if(!ModelState.IsValid) return View(company);
            if(company.Id==0)
                _unitofwork.company.Add(company);
            else
                _unitofwork.company.Update(company);
            _unitofwork.save();
            return RedirectToAction("Index");
        }
        public IActionResult Index()
          
        {
            return View();
        }
    }
}
