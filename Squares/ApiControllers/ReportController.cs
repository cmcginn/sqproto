using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Squares.Services;
using Squares.ViewModels;

namespace Squares.ApiControllers
{
    public class ReportController : ApiController
    {
        private SquareService _service = new SquareService(new SquaresEntities());
        // GET: api/Report
        public ReportViewModel Get()
        {
            return _service.GetReportViewModel(User.Identity.GetUserId());
        }

        // GET: api/Report/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Report
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Report/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Report/5
        public void Delete(int id)
        {
        }
    }
}
