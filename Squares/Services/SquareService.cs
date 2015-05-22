using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Squares.ViewModels;


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
            for (int i = 0; i < 3; i++)
            {
                result.Add(new UserSquare
                {
                    UserId = userId,
                    CreratedOnUtc = System.DateTime.UtcNow,
                    DisplayOrder = i,
                    DisplayName = String.Format("Square {0}", i + 1),
                    Id = Guid.NewGuid()
                });
            }
            return result;
        }
        public UserSquaresViewModel GetUserSquaresViewModelByUserId(string userId)
        {
            var result = new UserSquaresViewModel { UserSquares = new List<UserSquareViewModel>() };
            var userSquares = _context.UserSquares.Where(x => x.UserId == userId).ToList();
            if (!userSquares.Any())
            {
                userSquares = CreateDefaultUserSquares(userId);
                _context.UserSquares.AddRange(userSquares);
                _context.SaveChanges();

            }
            userSquares.OrderBy(x=>x.DisplayOrder).ToList().ForEach(us =>
            {
                
                var userSquareModel = new UserSquareViewModel
                {
                    Id = us.Id,
                    Name = us.DisplayName
                };
                if (us.UserSquareActivities.Any())
                {
                    var running =
                        us.UserSquareActivities.SingleOrDefault(x => x.ActivityState == (int) ActivityStateTypes.Started);
                    if (running != null)
                    {
                        
                        var lastRunning = us.UserSquareActivities.Where(x => x.ActivityState != (int) ActivityStateTypes.Started)
                            .OrderBy(x => x.StartUtc)
                            .ToList()
                            .LastOrDefault();
                        if (lastRunning != null)
                        {
                            if (lastRunning.ActivityState == (int) ActivityStateTypes.Stopped)
                                userSquareModel.Elapsed = 0;
                            else
                            {
                                userSquareModel.Elapsed =
                                    (long) (System.DateTime.UtcNow - running.StartUtc).TotalMilliseconds +
                                    running.ElapsedMilliseconds;
                                userSquareModel.ActivityState = ActivityStateTypes.Started;
                            }
                        }
                    }
                    else
                    {
                        userSquareModel.Elapsed =
                            us.UserSquareActivities.OrderBy(x => x.StartUtc).ToList().Last().ElapsedMilliseconds;
                    }

                }
                result.UserSquares.Add(userSquareModel);
            });
            return result;
        }

        public void SaveUserSquaresViewModel(UserSquaresViewModel model)
        {
            var userSquareActivities =
                _context.UserSquares.Where(x => x.UserId == model.UserId).SelectMany(x => x.UserSquareActivities).ToList();
            //stop all running
            userSquareActivities.Where(x => x.ActivityState == (int)ActivityStateTypes.Started).ToList().ForEach(usa =>
            {
                var m = model.UserSquares.Single(x => x.Id == usa.UserSquareId);
                usa.ElapsedMilliseconds = m.Elapsed;
                usa.ActivityState = (int) ActivityStateTypes.Paused;
            });

            var started = model.UserSquares.SingleOrDefault(x => x.ActivityState == ActivityStateTypes.Started);
            if(started != null)
            {
                //new entry for started
                var startedActivity = new UserSquareActivity
                {
                    Id = Guid.NewGuid(),
                    StartUtc = DateTime.Parse("1/1/1970").Date.AddMilliseconds(started.StartDate.GetValueOrDefault()),
                    UserSquareId = started.Id,
                    ElapsedMilliseconds=0,
                    ActivityState = (int)ActivityStateTypes.Started
                };
               
                _context.UserSquareActivities.Add(startedActivity);
            }

            _context.SaveChanges();


        }

        public UserSquareViewModel GetUserSquareViewModelById(Guid id)
        {
            UserSquareViewModel result = null;
            var src = _context.UserSquares.SingleOrDefault(x => x.Id == id);
            if (src != null)
            {
                result = new UserSquareViewModel
                {
                    Id=src.Id,
                    Name = src.DisplayName
                };
            }
            return result;
        }

        public void ResetTimer(string userId, Guid id)
        {
            var target = _context.UserSquares.Where(x => x.UserId == userId && x.Id == id).SingleOrDefault();
            if (target != null)
            {
                var usa = new UserSquareActivity
                {
                    ActivityState = (int) ActivityStateTypes.Stopped,
                    ElapsedMilliseconds = 0,
                    Id = Guid.NewGuid(),
                    StartUtc = System.DateTime.UtcNow,
                    UserSquareId = target.Id
                };
                _context.UserSquareActivities.Add(usa);
                _context.SaveChanges();
            }
        }
    }



}