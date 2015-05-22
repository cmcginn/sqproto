using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Squares.Services;
using Squares.ViewModels;

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
        [HttpPost]
        public void Rename(UserSquareViewModel model)
        {
            _service.RenameSquare(User.Identity.GetUserId(), model);

        }
        [HttpPost]
        public void ResetTimer(Guid id)
        {
            _service.ResetTimer(User.Identity.GetUserId(), id);
        }

        [HttpPost]
        public void HideUserSquare(Guid id)
        {
            _service.HideUserSquare(User.Identity.GetUserId(), id);
        }
    }
}