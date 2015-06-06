using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Squares.ApiControllers
{
    public class ScratchController : ApiController
    {
        public class DemoModel
        {
            public DateTime StartDate { get; set; }
        }

        // GET: api/Scratch
        public List<UserSquare> Get()
        {
            var ctx = new SquaresEntities();
            var model = ctx.UserSquares.Where(x => x.UserId == User.Identity.GetUserId()).ToList();
            return model;

        }

        // GET: api/Scratch/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Scratch
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Scratch/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Scratch/5
        public void Delete(int id)
        {
        }
    }
}
