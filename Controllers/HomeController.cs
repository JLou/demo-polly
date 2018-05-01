using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demo_polly.Models;
using demo_polly.Services;

namespace demo_polly.Controllers
{
    public class HomeController : Controller
    {
        private INameService _nameService;

        [BindProperty]
        public InputViewModel Input { get; set; }

        public HomeController(INameService nameService)
        {
            _nameService = nameService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // if(!ModelState.IsValid) {
            return View(new InputViewModel());
            // }

            // return View(Input);            
        }

        [HttpPost]
        public async Task<IActionResult> IndexPost()
        {
            var info = await _nameService.GetNameInfoAsync(Input.Name);
            return View("index", 
            new InputViewModel
            {
                Name = Input.Name,
                Info = info
            });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
