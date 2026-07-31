using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Packaging.Signing;
using E_EOM_Utility;

namespace E_ECOM_P.Areas.Admin.Controllers

{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitofWork _unitofWork;   //_unitofwork field to access unitofwork 
        public CoverTypeController(IUnitofWork unitofWork)//constructor
        {
            _unitofWork = unitofWork;   //initialized
        }

        public IActionResult Index()
        {
            return View();
        }
        #region
        [HttpGet] //(APIs) api bangyi ab hum covertype.js bnayenge
        public IActionResult GetAll()
        {
            return Json(new { data = _unitofWork.coverType.GetAll() });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var covertypeindb=_unitofWork.coverType.get(id);   // pehle id find huyii 
            if (covertypeindb==null)//agar id null hai
                return Json(new {success=false,Message="Unable to Delete Data!!!!!"});
            _unitofWork.coverType.Remove(covertypeindb);//agar id null nahi hai toh delete hona chahiye
            _unitofWork.save();
            return Json(new { success = true, Message = "Data Deleted Successfully" });//jab delete ho jaye toh message ana chahiye
            //now call delete api in covertype.js
        }
            

            #endregion
        
        public IActionResult Upsert(int? id)//upsert action
        {
            CoverType coverType = new CoverType();//new covertype for new entry
            if (id == null) return View(coverType);//create ( If no id is passed you're creating a new covertype)
            coverType = _unitofWork.coverType.get(id.GetValueOrDefault());//not null or edit( If id is there it will search from db to edit)
            if (coverType == null) return NotFound();//If no entry is found , it returns Not Found 
            return View(coverType);// If the entry exists, it returns the view  ==>Now Make Upsert View
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)    // 1=modelname 2=the variable jisde vich form cho data auna//
        {
            if (coverType == null) return BadRequest();//agar id null hai
            if(!ModelState.IsValid)return View(coverType);//agar state valid nahi(Check the model submitted fields (like missing required fields)
            if(coverType.id==0)_unitofWork.coverType.Add(coverType);////agar modelstate valid hai or id 0 hai toh add ka code
            else
                _unitofWork.coverType.Update(coverType);////agar id 0 nahi toh update ka code
            _unitofWork.save();
            return RedirectToAction("Index");
        }






    }

}
