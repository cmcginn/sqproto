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
        public UserSquareViewModel Get(Guid id)
        {
            return _service.GetUserSquareViewModel(User.Identity.GetUserId(), id);
        }

        //POST: api/Square
        public UserSquareViewModel Post(UserSquareViewModel model)
        {
            _service.SaveUserSquare(User.Identity.GetUserId(), model);
            return model;

        }

        // PUT: api/Square/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Square/5
        public void Delete(int id)
        {
        }



    }
}
