using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Squares.ViewModels;
using WebGrease.Css.Extensions;


namespace Squares.Services
{
    public class SquareService
    {
        private readonly SquaresEntities _context;
        public SquareService(SquaresEntities context)
        {
            _context = context;
        }

        List<UserSquare> CreateDefaultUserSquares(string userId)
        {
            var result = new List<UserSquare>();
            for (int i = 0; i < 1; i++)
            {
                var square = new UserSquare
                {
                    UserId = userId,
                    CreratedOnUtc = System.DateTime.UtcNow,
                    DisplayOrder = i,
                    DisplayName = String.Format("Square {0}", i + 1),
                    Id = Guid.NewGuid(),
                    Hidden = false

                };
                var sw = new StopWatch
                {
                    CreatedOnUtc = square.CreratedOnUtc,
                    Elapsed = 0,
                    Id = Guid.NewGuid(),
                    Started = 0,
                    State = (int) ActivityStateTypes.None,
                    UserSquareId = square.Id

                };
                square.StopWatches.Add(sw);
                result.Add(square);
            }
            return result;
        }

        public UserSquaresViewModel GetUserSquaresViewModelByUserId(string userId)
        {
            var result = new UserSquaresViewModel { UserSquares = new List<UserSquareViewModel>() };
            var userSquares = _context.UserSquares.Where(x => x.UserId == userId & !x.Hidden).ToList();
            if (!userSquares.Any())
            {
                userSquares = CreateDefaultUserSquares(userId);
                _context.UserSquares.AddRange(userSquares);
                _context.SaveChanges();
                userSquares = _context.UserSquares.Where(x => x.UserId == userId).ToList();

            }

            userSquares.OrderBy(x => x.DisplayOrder).ToList().ForEach(us =>
            {
                var thisDate =
                System.DateTime.UtcNow;
                var userSquareModel = new UserSquareViewModel
                {
                    Id = us.Id,
                    Name = us.DisplayName,
                };
                var sw = us.StopWatches.OrderBy(x => x.CreatedOnUtc).ToList().Last();
                userSquareModel.StopWatch = new StopWatchModel
                {
                    Id = sw.Id,
                    Started = sw.Started,
                    State = (ActivityStateTypes) sw.State,
                    Time = sw.Elapsed
                };
                result.UserSquares.Add(userSquareModel);
            });

            return result;
        }

        public void SaveStopWatch(string userId, StopWatchModel model)
        {
            var target = _context.StopWatches.Single(x => x.Id == model.Id);
            target.LastUpdatedUtc = System.DateTime.UtcNow;
            target.Started = model.Started;
            target.State = (int)model.State;
            target.Elapsed = model.Time;
            _context.SaveChanges();
        }
    }


}