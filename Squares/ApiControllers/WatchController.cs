using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Squares.ViewModels;

namespace Squares.ApiControllers
{
    public class WatchController : ApiController
    {
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
        public void Post(WatchModel model)
        {
            var x = "Y";
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
