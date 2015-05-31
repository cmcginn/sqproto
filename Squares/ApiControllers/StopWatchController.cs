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
    public class StopWatchController : ApiController
    {
        private SquareService _service = new SquareService(new SquaresEntities());
        // GET: api/Timer
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Timer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Timer
        public void Post(StopWatchModel model)
        {
            _service.SaveStopWatch(User.Identity.GetUserId(), model);
        }

        // PUT: api/Timer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Timer/5
        public void Delete(int id)
        {
        }
    }
}
