﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Squares.Services;

namespace Squares.Controllers
{
    public class ReportController : Controller
    {

        private SquareService _service = new SquareService(new SquaresEntities());
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DeleteActivityItem(Guid id)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult DeleteItem(Guid id){
            throw new System.NotImplementedException();
        }
    }
}