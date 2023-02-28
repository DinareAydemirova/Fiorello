using Fiorello.DAL;
using Fiorello.Models;
using Fiorello.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db= db;
        }
       
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM= new HomeVM
            {
                Categories=await _db.Categories.Where(x => !x.IsDeactive).ToListAsync(),  
                Products=await _db.Products.Where(x => !x.IsDeactive).ToListAsync(),
            };    
            return View(homeVM);
        }

        
        public IActionResult Error()
        {
            return View();
        }
    }
}
