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

        StopWatch CreateDefaultStopWatch(Guid userSqureId)
        {
            var result = new StopWatch
            {
                CreatedOnUtc = System.DateTime.UtcNow,
                Elapsed = 0,
                Id = Guid.NewGuid(),
                Started = 0,
                State = (int)ActivityStateTypes.None,
                UserSquareId = userSqureId

            };
            return result;
        }

        UserSquare CreateDefaultUserSquare(string userId)
        {
            var result = new UserSquare
            {
                UserId = userId,
                CreratedOnUtc = System.DateTime.UtcNow,
                DisplayOrder = 0,
                DisplayName = String.Format("New Square"),
                Id = Guid.NewGuid(),
                Hidden = false

            };
            var sw = CreateDefaultStopWatch(result.Id);
            sw.CreatedOnUtc = result.CreratedOnUtc;
            result.StopWatches.Add(sw);
            return result;
        }
        List<UserSquare> CreateDefaultUserSquares(string userId)
        {
            var result = new List<UserSquare>();
            for (int i = 0; i < 1; i++)
            {
                var square = CreateDefaultUserSquare(userId);
                square.DisplayOrder = i;
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

        public UserSquareViewModel GetUserSquareViewModel(string userId, Guid id)
        {
            var target = _context.UserSquares.SingleOrDefault(x => x.Id == id);
            if (target == null)
            {
                target = CreateDefaultUserSquare(userId);
                target.DisplayOrder = _context.UserSquares.Where(x => x.UserId == userId).Max(x => x.DisplayOrder) + 1;
                _context.UserSquares.Add(target);
                _context.SaveChanges();
            }
            var result = new UserSquareViewModel
            {
                Id = target.Id,
                Name = target.DisplayName

            };
            var sw = target.StopWatches.OrderBy(x => x.CreatedOnUtc).Last();
            result.StopWatch = new StopWatchModel
            {
                Id = sw.Id,
                Started = sw.Started,
                State = (ActivityStateTypes) sw.State,
                Time = sw.Elapsed
            };
            return result;
        }
        public void SaveUserSquare(string userId, UserSquareViewModel model)
        {
            if (!String.IsNullOrWhiteSpace(model.Name))
            {
                var target = _context.UserSquares.Single(x => x.Id == model.Id);
                target.DisplayName = model.Name.Trim();
                _context.SaveChanges();
            }
        }
        public void SaveStopWatch(string userId, StopWatchModel model)
        {
           
         
                var target = _context.StopWatches.Single(x => x.Id == model.Id);
                target.LastUpdatedUtc = System.DateTime.UtcNow;
                target.Started = model.Started;
                target.State = (int) model.State;
                target.Elapsed = model.Time;
                if (model.State == ActivityStateTypes.Stopped)
                {
                    var sw = CreateDefaultStopWatch(target.UserSquareId);
                    _context.StopWatches.Add(sw);
                    model.Id = sw.Id;
                    model.Started = sw.Started;
                    model.State = (ActivityStateTypes)sw.Started;
                    model.Time = sw.Elapsed;
                    
                }
                _context.SaveChanges();
           
        }
    }


}