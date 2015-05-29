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
                result.Add(new UserSquare
                {
                    UserId = userId,
                    CreratedOnUtc = System.DateTime.UtcNow,
                    DisplayOrder = i,
                    DisplayName = String.Format("Square {0}", i + 1),
                    Id = Guid.NewGuid(),
                    ActivityState = (int)ActivityStateTypes.None,
                    Hidden = false

                });
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
                var thisDate=
                System.DateTime.UtcNow;
                var userSquareModel = new UserSquareViewModel
                {
                    Id = us.Id,
                    Name = us.DisplayName,
                    ActivityState = (ActivityStateTypes)us.ActivityState,
                    RunningTime=0,
                    UserSquareActivityId=Guid.NewGuid(),
                    Elapsed=0,
                    Visible = !us.Hidden
                };
                if (us.UserSquareActivities.Any())
                {
                    
                    var lastActivity = us.UserSquareActivities.OrderBy(x => x.CreatedOnUtc).ToList().Last();

                    if (lastActivity != null)
                    {
                        if(us.ActivityState != (int)ActivityStateTypes.Stopped)
                            userSquareModel.UserSquareActivityId = lastActivity.Id;

                        if (us.ActivityState == (int)ActivityStateTypes.Paused)
                        {
                            
                            userSquareModel.Elapsed = lastActivity.Elapsed;

                        }
                        else if (us.ActivityState == (int) ActivityStateTypes.Running)
                        {

                            Resume(userSquareModel, thisDate);
                            //_context.SaveChanges();
                        }
                    }


                }

                result.UserSquares.Add(userSquareModel);
            });

            return result;
        }

        public void SaveUserSquaresViewModel(UserSquaresViewModel model)
        {
            //try
            //{
            //    var userSquares = _context.UserSquares.Where(x => x.UserId == model.UserId).ToList();

            //    model.UserSquares.ForEach(userSquareModel =>
            //    {
            //        var userSquare = userSquares.Single(x => x.Id == userSquareModel.Id);
            //        var userSquareActivity = userSquare.UserSquareActivities.Any() ? userSquare.UserSquareActivities.OrderBy(x => x.CreatedOnUtc).ToList().Last() : null;
            //        if (userSquareModel.ActivityState == ActivityStateTypes.Paused && userSquareActivity != null)
            //        {

            //            userSquareActivity.Elapsed = userSquareModel.Elapsed;
            //            userSquareActivity.Milliseconds = userSquareModel.Milliseconds;
            //            userSquareActivity.ActivityState = (int)ActivityStateTypes.Paused;
            //            userSquareActivity.LastUpdatedUtc = System.DateTime.UtcNow;
            //        }
            //        else if (userSquareModel.ActivityState == ActivityStateTypes.Running)
            //        {
            //            userSquareActivity = new UserSquareActivity
            //            {
            //                CreatedOnUtc = System.DateTime.UtcNow,
            //                ActivityState = (int)ActivityStateTypes.Running,
            //                Milliseconds = userSquareModel.Milliseconds,
            //                Elapsed = 0,
            //                Id = Guid.NewGuid(),
            //                UserSquareId = userSquare.Id,

            //            };
            //            _context.UserSquareActivities.Add(userSquareActivity);
            //        }

            //    });
            //    if (_context.ChangeTracker.HasChanges())
            //        _context.SaveChanges();
            //}
            //catch (System.Exception ex)
            //{
            //    var z = "Y";
            //    throw (ex);
            //}



        }

        public UserSquareViewModel GetUserSquareViewModelById(Guid id)
        {
            UserSquareViewModel result = null;
            var src = _context.UserSquares.SingleOrDefault(x => x.Id == id);
            if (src != null)
            {
                result = new UserSquareViewModel
                {
                    Id = src.Id,
                    Name = src.DisplayName
                };
            }
            return result;
        }

        public void RenameSquare(string userId, UserSquareViewModel model)
        {
            var target = _context.UserSquares.Single(x => x.UserId == userId && x.Id == model.Id);
            if (target != null && target.DisplayName != model.Name & !String.IsNullOrWhiteSpace(model.Name))
            {
                target.DisplayName = model.Name.Trim();
                _context.SaveChanges();
            }
        }

        public UserSquareViewModel AddNewUserSquare(string userId)
        {
            var displayOrder = _context.UserSquares.Max(x => x.DisplayOrder)+1;
            var userSquare = new UserSquare
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ActivityState = (int)ActivityStateTypes.None,
                CreratedOnUtc = System.DateTime.UtcNow,
                DisplayOrder = displayOrder,
                DisplayName = "New Square",
                Hidden = false
            };
            _context.UserSquares.Add(userSquare);
            _context.SaveChanges();
            var result = new UserSquareViewModel
            {
                ActivityState = ActivityStateTypes.None,
                Elapsed=0,
                Milliseconds=0,
                Id = userSquare.Id,
                UserSquareActivityId=Guid.NewGuid(),
                Name = userSquare.DisplayName,
                Visible = !userSquare.Hidden
            };
            return result;
        }

        public void HideUserSquare(string userId, Guid id)
        {
            var target = _context.UserSquares.SingleOrDefault(x => x.UserId == userId && x.Id == id);
            if (target != null && target.ActivityState != (int)ActivityStateTypes.Running)
            {
                target.Hidden = true;
                _context.SaveChanges();
            }

        }

        public ReportViewModel GetReportViewModel(string userId)
        {
            var userSquares = _context.UserSquares.Where(x => x.UserId == userId && x.UserSquareActivities.Any()).ToList();
            var result = new ReportViewModel();
            userSquares.ForEach(us =>
            {
                var item = new ReportItemViewModel();
                item.MinDate = us.UserSquareActivities.Min(x => x.Milliseconds);
                item.MaxDate = us.UserSquareActivities.Max(x => x.Milliseconds);
                item.Name = us.DisplayName;
                item.TotalDuration = us.UserSquareActivities.Sum(x => x.Elapsed);
                item.Id = us.Id;
                result.ReportItems.Add(item);
                us.UserSquareActivities.OrderBy(x => x.Milliseconds).ToList().ForEach(usa =>
                {
                    var activity = new ActivityRecord();
                    activity.Id = usa.Id;
                    activity.Duration = Duration.FromMS(usa.Elapsed);
                    activity.StartDate = usa.Milliseconds;
                    activity.UserSquareId = us.Id;
                    item.ActivityRecords.Add(activity);

                });
            });
            return result;
        }

        public void SaveReportItemViewModel(string userId, ReportItemViewModel model)
        {
            //var us = _context.UserSquares.Single(x => x.UserId == userId && x.Id == model.Id);

            //long elappsedTotal = 0;
            //model.ActivityRecords.ForEach(ar =>
            //{
            //    var usa = us.UserSquareActivities.Single(x => x.Id == ar.Id);

            //    var endDate =
            //        ar.StartDate.AddDays(ar.Duration.Days)
            //            .AddHours(ar.Duration.Hours)
            //            .AddMinutes(ar.Duration.Minutes)
            //            .AddSeconds(ar.Duration.Seconds);


            //});
            //_context.SaveChanges();
        }

        public void DeleteUserSquareActivity(string userId, Guid id)
        {
            var target = _context.UserSquareActivities.Single(x => x.Id == id && x.UserSquare.UserId == userId);
            _context.UserSquareActivities.Remove(target);
            _context.SaveChanges();
        }

        public void DeleteUserSquare(string userId, Guid id)
        {
            var target = _context.UserSquares.Single(x => x.UserId == userId && x.Id == id);
            _context.UserSquares.Remove(target);
            _context.SaveChanges();
        }

        void SaveUserSquareActivityActionForTimer(UserSquareActivity activity,ActivityStateTypes state)
        {
            var target = new UserSquareActivityAction
            {
                ActivityState = (int) state,
                CreatedOnUtc = System.DateTime.UtcNow,
                Id = Guid.NewGuid(),
                UserSquareActivityId = activity.Id,
                Milliseconds = activity.Elapsed
            };
            _context.UserSquareActivityActions.Add(target);
            _context.SaveChanges();
        }

        void ModifyTime(string userId, TimerActionModel model)
        {
            var userSquare = _context.UserSquares.Single(x => x.UserId == userId && x.Id == model.ParentId);
            var userSquareActivity = _context.UserSquareActivities.SingleOrDefault(x => x.Id == model.Id);
            if (userSquareActivity != null)
            {
                var currentTime = userSquareActivity.Milliseconds;
                var currentElapsed = userSquareActivity.Elapsed;

                var started = model.Time - model.Elapsed;
                userSquareActivity.Milliseconds = started;
                userSquareActivity.Elapsed = model.Elapsed;
                userSquareActivity.LastUpdatedUtc = System.DateTime.UtcNow;

                var action = new UserSquareActivityAction
                {
                    ActivityState = userSquare.ActivityState,
                    CreatedOnUtc = System.DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    UserSquareActivityId = userSquareActivity.Id,
                    Milliseconds = model.Modified,
                    Description = String.Format("Duration Changed. Elapsed: {0} Changed To: {1}. Start: {2} Changed To: {3}",currentElapsed,model.Elapsed,currentTime,started)
                };
                _context.UserSquareActivityActions.Add(action);
                _context.SaveChanges();
                model.Time = currentTime;


            }
        }
        void SaveUserSquareActivityForTimer(string userId, TimerActionModel model)
        {
            if (model.Modified>0)
            {
                ModifyTime(userId, model);
                return;
            }
            var userSquare = _context.UserSquares.Single(x => x.UserId == userId && x.Id == model.ParentId);
            var userSquareActivity = _context.UserSquareActivities.SingleOrDefault(x => x.Id == model.Id);
            if (userSquareActivity != null)
            {
                userSquare.ActivityState = (int)model.ActivityState;
                userSquareActivity.Elapsed = model.Elapsed;

                userSquareActivity.LastUpdatedUtc = System.DateTime.UtcNow;
                model.ParentId = userSquare.Id;
                
                if (model.ActivityState == ActivityStateTypes.Stopped)
                {
                    model.Id = Guid.NewGuid();
                    model.Elapsed = 0;
                 
                }
                _context.SaveChanges();
            }
            else if (model.ActivityState == ActivityStateTypes.Running)
            {
                userSquareActivity = new UserSquareActivity
                {
                    UserSquareId = model.ParentId,
                    CreatedOnUtc = System.DateTime.UtcNow,
                    Milliseconds = model.Time,
                    Id = model.Id,
                    Elapsed = 0
                };
                userSquare.ActivityState = (int)ActivityStateTypes.Running;
                model.ParentId = userSquare.Id;
                _context.UserSquareActivities.Add(userSquareActivity);
                _context.SaveChanges();
            }
            
            if (userSquareActivity != null)
                SaveUserSquareActivityActionForTimer(userSquareActivity, model.ActivityState);


        }
        public void HandleTimerActionModel(string userId, TimerActionModel model)
        {
            SaveUserSquareActivityForTimer(userId, model);
        }

        void Resume(UserSquareViewModel model,DateTime now)
        {
            var epoch = (long)(now - System.DateTime.Parse("1/1/1970")).TotalMilliseconds;
            var lastStarted =
                _context.UserSquareActivityActions.Where(x => x.UserSquareActivityId == model.UserSquareActivityId)
                    .OrderBy(x => x.CreatedOnUtc)
                    .ToList()
                    .Last();
            var usa = _context.UserSquareActivities.Single(x => x.Id == model.UserSquareActivityId);
            var ss = System.DateTime.Parse("1/1/1970").Date.AddMilliseconds(usa.Milliseconds+lastStarted.Milliseconds);
            model.Elapsed = (long)(System.DateTime.UtcNow - ss).TotalMilliseconds;
            


        }
    }



}