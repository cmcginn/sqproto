//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using Microsoft.AspNet.Identity;
//using Squares.Services;
//using Squares.ViewModels;

//namespace Squares.ApiControllers
//{
//    public class TimerActionController : ApiController
//    {
//        private SquareService _service = new SquareService(new SquaresEntities());
//        // GET: api/TimerAction
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET: api/TimerAction/5
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST: api/TimerAction
//        public TimerActionModel Post(TimerActionModel model)
//        {
//            _service.HandleTimerActionModel(User.Identity.GetUserId(), model);
//            return model;
//        }

//        // PUT: api/TimerAction/5
//        public void Put(int id, [FromBody]string value)
//        {
//        }

//        // DELETE: api/TimerAction/5
//        public void Delete(int id)
//        {
//        }
//    }
//}
