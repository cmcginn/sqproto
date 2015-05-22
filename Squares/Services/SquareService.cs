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
                    Id = Guid.NewGuid(),
                    RunningTime = 0,
                    ActivityState = (int)ActivityStateTypes.None
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
                userSquares = _context.UserSquares.Where(x => x.UserId == userId).ToList();

            }
            
                userSquares.OrderBy(x => x.DisplayOrder).ToList().ForEach(us =>
                {

                    var userSquareModel = new UserSquareViewModel
                    {
                        Id = us.Id,
                        Name = us.DisplayName,
                        ActivityState = (ActivityStateTypes) us.ActivityState
                    };
                    if (us.UserSquareActivities.Any())
                    {
                        var lastActivity = us.UserSquareActivities.OrderBy(x => x.StartUtc).ToList().Last();
                        if (lastActivity != null && lastActivity.ActivityState == (int)ActivityStateTypes.Started)
                        {
                            lastActivity.ElapsedMilliseconds =
                                (long)(System.DateTime.UtcNow - lastActivity.StartUtc).TotalMilliseconds;
                            us.RunningTime += lastActivity.ElapsedMilliseconds;
                            _context.SaveChanges();
                        }
                    }
                    //allow reset activity to be restarted but keep state if it is not
                    if (us.ActivityState == (int) ActivityStateTypes.Stopped)
                        userSquareModel.ActivityState = ActivityStateTypes.None;
                    userSquareModel.RunningTime = us.RunningTime;
                    result.UserSquares.Add(userSquareModel);
                });
           
            return result;
        }

        public void SaveUserSquaresViewModel(UserSquaresViewModel model)
        {
            var now = System.DateTime.UtcNow;
            //pause everything started
            var userSquares = _context.UserSquares.Where(x => x.UserId == model.UserId).ToList();
            userSquares.ForEach(us =>
            {
                var lastActivity = us.UserSquareActivities.OrderBy(x => x.StartUtc).ToList().LastOrDefault();
                var squareModel = model.UserSquares.Single(x => x.Id == us.Id);
                if (lastActivity != null)
                {
                    //pause all started
                    if (lastActivity.ActivityState == (int) ActivityStateTypes.Started)
                    {
                        lastActivity.ActivityState = (int) ActivityStateTypes.Paused;
                        lastActivity.ElapsedMilliseconds = squareModel.Elapsed;
                        us.RunningTime += squareModel.Elapsed;
                        us.ActivityState = lastActivity.ActivityState;
                    }
                }

            });

            var started = model.UserSquares.SingleOrDefault(x => x.ActivityState == ActivityStateTypes.Started);
            if (started != null)
            {
                var startedActivity = new UserSquareActivity
                {
                    Id = Guid.NewGuid(),
                    StartUtc = DateTime.Parse("1/1/1970").Date.AddMilliseconds(started.StartDate.GetValueOrDefault()),
                    UserSquareId = started.Id,
                    ElapsedMilliseconds = 0,
                    ActivityState = (int)ActivityStateTypes.Started
                };
                var us = _context.UserSquares.Single(x => x.Id == started.Id);
                us.ActivityState = startedActivity.ActivityState;
                us.UserSquareActivities.Add(startedActivity);
               // _context.UserSquareActivities.Add(startedActivity);
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
                var lastActivity =
                    target.UserSquareActivities.OrderBy(x => x.StartUtc).Last();
                lastActivity.ActivityState =
                        (int)ActivityStateTypes.Stopped;
                target.ActivityState = lastActivity.ActivityState;
                target.RunningTime = 0;
                _context.SaveChanges();
            }
        }

        public void RenameSquare(string userId, UserSquareViewModel model)
        {
            var target = _context.UserSquares.Single(x => x.UserId == userId && x.Id == model.Id);
            if (target != null && target.DisplayName != model.Name &!String.IsNullOrWhiteSpace(model.Name))
            {
                target.DisplayName = model.Name.Trim();
                _context.SaveChanges();
            }
        }
    }



}