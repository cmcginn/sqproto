using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Squares.Services;
using Squares.ViewModels;

namespace Squares.ApiControllers
{
    public class SquareController : ApiController
    {
        private SquareService _service = new SquareService(new SquaresEntities());
        // GET: api/Square
        public UserSquaresViewModel Get()
        {
            return _service.GetUserSquaresViewModelByUserId(User.Identity.GetUserId());
        }

        // GET: api/Square/5
        public string Get(int id)
        {
            return "value";
        }

         //POST: api/Square
        public void Post(UserSquaresViewModel model)
        {
            model.UserId = User.Identity.GetUserId();
            _service.SaveUserSquaresViewModel(model);
        
        }

        // PUT: api/Square/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Square/5
        public void Delete(int id)
        {
        }

        //public UserSquareActivityViewModel UpdateUserSquareActivity(UserSquareActivityViewModel model)
        //{
        //    _service.UpdateActivity(model);
        //    return model;

        //}
    }
}
