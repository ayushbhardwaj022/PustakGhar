using E_COM_DataAccess.Repository;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using E_COM_Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.Linq;
using System;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using E_EOM_Utility;

namespace E_ECOM_P.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class Productcontroller : Controller
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
       
        public Productcontroller(IUnitofWork unitofWork,IWebHostEnvironment webHostEnvironment)
            
        {
            _unitofWork = unitofWork;
            _webHostEnvironment=webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitofWork.product.GetAll() });
        }

        #endregion
        public IActionResult Upsert(int? id)//Action Upsert
        
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                categoryList = _unitofWork.category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.id.ToString()

                }),
                coverTypeList = _unitofWork.coverType.GetAll().Select(ct => new SelectListItem()
                {
                    Text = ct.Name,
                    Value = ct.id.ToString()
                })
            };
            if (id == null)return View(productVM);//jb id=o//creating new product
            productVM.Product = _unitofWork.product.get(id.GetValueOrDefault());//jb id hai...toh product fetch hoga
            if(productVM.Product == null)return NotFound(); //If no product is found with the given id, Not Found

            return View(productVM); //result show hoga chahe id new ho ya existing



        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0)//koi file select hui ya nhi form fill krte wqt
                {
                    var filename = Guid.NewGuid().ToString();//guid random filename generate krta hai or vo duplicate nhi hota
                    var Extension = Path.GetExtension(files[0].FileName);//jis nam se or format se file serverr pr save hogi
                    var uploads = Path.Combine(webRootPath, "images\\products");//path of saved images through form
                    if (productVM.Product.id != 0)
                    {
                        var imageexists = _unitofWork.product.get(productVM.Product.id).Imageurl;//pehle image path find hoga agar image exist hogi
                        productVM.Product.Imageurl = imageexists;
                    }
                    if (productVM.Product.Imageurl != null)
                    {
                        var imagepath = Path.Combine(webRootPath, productVM.Product.Imageurl.Trim('\\'));
                        if (System.IO.File.Exists(imagepath))
                        {
                            System.IO.File.Delete(imagepath);//ye code purani image ko delete krne ka hai update/edit case me kam karta hai
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, filename + Extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);// Save new image
                    }


                    productVM.Product.Imageurl = @"\images\products\" + filename + Extension;// Save path to DB

                }
                else
                {
                    if (productVM.Product.id != 0)
                    {
                        var imageexists = _unitofWork.product.get(productVM.Product.id).Imageurl;
                        productVM.Product.Imageurl = imageexists;//If no image was uploaded, and the product exists, it retains the existing image path in the database
                    }


                }
                if (productVM.Product.id == 0)
                {
                    _unitofWork.product.Add(productVM.Product);//Insert or Update the Product
                    }
                else
                {
                    _unitofWork.product.Update(productVM.Product);//// Update existing product
                    
                }
                _unitofWork.save();//// Save changes
                return RedirectToAction("Index");
            }

            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),
                    
                    categoryList = _unitofWork.category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.id.ToString()

                    }),
                    coverTypeList = _unitofWork.coverType.GetAll().Select(ct => new SelectListItem()
                    {
                        Text = ct.Name,
                        Value = ct.id.ToString()
                    })
                };
                if (productVM.Product.id!=0)
                {
                    productVM.Product = _unitofWork.product.get(productVM.Product.id);
                }
                return View(productVM); 
            }
            



        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productindb=_unitofWork.product.get(id);
            if (productindb == null)
                return Json(new { success = false, Message = "Unable to delete data!!!!" });
            _unitofWork.product.Remove(productindb);
            _unitofWork.save();
            //Image DELETE
            if (!string.IsNullOrEmpty(productindb.Imageurl))
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var imagepath = Path.Combine(webRootPath, productindb.Imageurl.Trim('\\'));
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
            }

            return Json(new { success = true, Message = "Data deleted successfully!!!!" });
            // now add searchbar and  show these  products on homepage  via: HomeController

        }

    }

    }

