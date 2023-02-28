using Fiorello.DAL;
using Fiorello.Helpers;
using Fiorello.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _db.Products.Include(x => x.Category).ToListAsync();
            return View(products);
        }
        #endregion

        #region Create

        #region Create get
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }
        #endregion

        #region Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product ,int catId)
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();

            bool isExist = await _db.Products.AnyAsync(x => x.Name == product.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This product is already exist");
                return View();
            }

            if (product.Photo == null)

            {
                ModelState.AddModelError("Photo", "Please select Image");
                return View();
            }
            if (!product.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please select Image file");
                return View();
            }
            if (product.Photo.IsOlder2MB())
            {
                ModelState.AddModelError("Photo", "It must be max 2mb");
                return View();
            }
           
            string folder = Path.Combine(_env.WebRootPath, "img");
            product.Image = await product.Photo.SaveImageAsync(folder);
            product.CategoryId = catId; 
            await _db.Products.AddAsync(product);


            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();   
            return RedirectToAction("Index");

        }

        #endregion

        #endregion


        #region Update

        #region get 

        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            Product dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);
            if (dbProduct == null)
            {
                return BadRequest();
            }
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(dbProduct);
        }

        #endregion

        #region post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? Id, Product product, int catId)
        {
            if (Id == null)
            {
                return NotFound();
            }
            Product dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == Id);
            if (dbProduct == null)
            {
                return BadRequest();
            }
            ViewBag.Categories = await _db.Categories.ToListAsync();
            bool isExist = await _db.Products.AnyAsync(x => x.Name == product.Name&&x.Id!=Id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This product is already exist");
                return View(dbProduct);
            }

            if (product.Photo != null)

            {
                if (!product.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please select Image file");
                    return View(dbProduct);
                }
                if (product.Photo.IsOlder2MB())
                {
                    ModelState.AddModelError("Photo", "It must be max 2mb");
                    return View(dbProduct);
                }
                string folder = Path.Combine(_env.WebRootPath, "img");
                dbProduct.Image = await product.Photo.SaveImageAsync(folder);
            }
           

            dbProduct.Name= product.Name;   
            dbProduct.Price= product.Price;
            dbProduct.CategoryId = catId;


            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #endregion



        #region Activity
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (dbProduct == null)
            {
                return BadRequest();
            }
            if (dbProduct.IsDeactive)
            {
                dbProduct.IsDeactive = false;
            }
            else
            {
                dbProduct.IsDeactive = true;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
