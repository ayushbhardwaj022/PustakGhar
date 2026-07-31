using Azure;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using E_EOM_Utility;

namespace E_ECOM_P.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]//area defined 1area=1mvc project
    public class CategoryController : Controller//now we will show data in a table through API after index made//
    {
        private readonly IUnitofWork _unitofwork;//_unitofwork field to access unitofwork
        public CategoryController(IUnitofWork unitofWork)//constructor
        {
            _unitofwork = unitofWork;//initialized
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs //apis made here
        [HttpGet]//retrieve data

        public IActionResult GetAll()
        {
            var categorylist = _unitofwork.category.GetAll();//(getall=generic method -  returns all records of type Category) 

            return Json(new { data = categorylist });//now make javascript file for GetAll to call API

        }
        [HttpDelete]
        public IActionResult Delete(int id)//Delete api
        {
            var categoryindb = _unitofwork.category.get(id);// pehle id find huyii
            if (categoryindb == null)//agar id null hai
                return Json(new { success = false, Message = "something went wrong while deleting data" });
            _unitofwork.category.Remove(categoryindb);//agar id null nahi hai toh delete hona chahiye
            _unitofwork.save();
            //jab delete ho jaye toh message ana chahiye
            return Json(new { success = true, Message = "Data Deleted Successfully" });//api made now call in javascript
        }
        #endregion
        public IActionResult Upsert(int? id)//Action Upsert
        {
            Category category = new Category();//new category for new entry
            if (id == null) return View(category);//create ( If no id is passed you're creating a new category)

            category = _unitofwork.category.get(id.GetValueOrDefault());//not null or edit( If id is there it will search from db to edit)
            if (category == null) return NotFound(); //If no entry is found , it returns Not Found 

            return View(category);// If the entry exists, it returns the view  ==>Now Make Upsert View

        }
        //Now code for save buuton to save enteries//
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null) return BadRequest();//agar id null hai
            if(!ModelState.IsValid)return View(category);    //agar state valid nahi(Check the model submitted fields (like missing required fields))
            if (category.id==0)_unitofwork.category.Add(category);//agar modelstate valid hai or id 0 hai toh add ka code
            else
                _unitofwork.category.Update(category);//agar id 0 nahi toh update ka code
            _unitofwork.save();
            return RedirectToAction("Index");
            
        }

    }




}

