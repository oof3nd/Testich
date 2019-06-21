using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testich.Models;

namespace Testich.Controllers
{
    public class HomeController : Controller
    {

       // [Authorize(Roles= "Admin, User")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles="Admin,User")]
        public IActionResult Tests()
        {

            return View();
        }

        [Authorize(Roles="User")]
        public IActionResult About()
        {

        //  ViewData["Message"] = "Описание";

            return View();
        }

        public IActionResult Contact()
        {
         //   ViewData["Message"] = "Контакты";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
