using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Squares.Services;

namespace Squares.Controllers
{
    public class HomeController : Controller
    {
        private SquareService _service = new SquareService(new SquaresEntities());
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

     
        public ActionResult Scratch()
        {
            
            return View();
        }

        public PartialViewResult UserSquareEdit(Guid userSquareId)
        {
            var model = _service.GetUserSquareViewModelById(userSquareId);
            return PartialView("UserSquareEdit", model);
        }
    }
}