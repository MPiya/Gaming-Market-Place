﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using F2Play.DataAccess.Repository.IRepository;
using F2Play.WebApp.Areas.DTOs;
using F2Play.Models.ViewModels;
using F2Play.Utility;

namespace F2Play.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork db, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = db;

            _hostEnvironment = hostEnvironment;
        }


        public IActionResult Index()
        {



            return View();
        }

        //Get

        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                Product = new(),

                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                { Text = u.Name, Value = u.Id.ToString() }),


            };

            if (id == null || id == 0)
            {
                //create product
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                //updae product

                return View(productVM);
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile file)
        {


            if (ModelState.IsValid)
            {

                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    // check if the imageurl exist and remove it
                    if (obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }


                // when Id is 0 it means new product is added, if itsn ot then user want to upddate the product
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";

                return RedirectToAction("Index");
            }

            return View(obj);

        }







        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            var productDTOList = productList.Select(p => ProductDTOs.MapProductToDTO(p)).ToList();

            return Json(new { data = productDTOList });
        }

        //Post
        [HttpDelete]

        public IActionResult DeletePost(int? id)
        {

            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });



        }

        #endregion
    }
}
