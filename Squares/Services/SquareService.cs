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
                    RunningTime = 0,
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
                        if (us.ActivityState == (int)ActivityStateTypes.Paused)
                        {
                            userSquareModel.UserSquareActivityId = lastActivity.Id;
                            userSquareModel.Elapsed = lastActivity.Elapsed;

                        }
                    }


                }
                else
                {
                    userSquareModel.UserSquareActivityId = Guid.NewGuid();
                }
                //allow reset activity to be restarted but keep state if it is not
                if (us.ActivityState == (int)ActivityStateTypes.Stopped)
                    userSquareModel.ActivityState = ActivityStateTypes.None;

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

        public void ResetTimer(string userId, Guid id)
        {
            var target = _context.UserSquares.Where(x => x.UserId == userId && x.Id == id).SingleOrDefault();
            if (target != null)
            {
                //var lastActivity =
                //    target.UserSquareActivities.OrderBy(x => x.BeginMilliseconds).Last();
                //lastActivity.ActivityState =
                //        (int)ActivityStateTypes.Stopped;
                //target.ActivityState = lastActivity.ActivityState;
                target.RunningTime = 0;
                _context.SaveChanges();
            }
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
            var displayOrder = _context.UserSquares.Max(x => x.DisplayOrder);
            var userSquare = new UserSquare
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ActivityState = (int)ActivityStateTypes.None,
                CreratedOnUtc = System.DateTime.UtcNow,
                DisplayOrder = displayOrder += 1,
                RunningTime = 0,
                DisplayName = "New Square",
                Hidden = false
            };
            _context.UserSquares.Add(userSquare);
            _context.SaveChanges();
            var result = new UserSquareViewModel
            {
                ActivityState = ActivityStateTypes.None,
                // BeginMilliseconds = 0,
                Id = userSquare.Id,
                Name = userSquare.DisplayName,
                RunningTime = userSquare.RunningTime,
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
                //  item.MinDate = us.UserSquareActivities.Min(x => x.StartUtc);
                // item.MaxDate = us.UserSquareActivities.Max(x => x.StartUtc);
                item.Name = us.DisplayName;
                //item.TotalDuration = us.UserSquareActivities.Sum(x => x.BeginMilliseconds);
                item.Id = us.Id;
                result.ReportItems.Add(item);
                //us.UserSquareActivities.OrderBy(x => x.StartUtc).ToList().ForEach(usa =>
                //{
                //    var activity = new ActivityRecord();
                //    activity.Id = usa.Id;
                //    activity.Duration = Duration.FromMS(usa.ElapsedMilliseconds);
                //    activity.StartDate = usa.StartUtc;
                //    activity.ActivityState = ((ActivityStateTypes) usa.ActivityState).ToString();
                //    activity.UserSquareId = us.Id;
                //    item.ActivityRecords.Add(activity);

                //});
            });
            return result;
        }

        public void SaveReportItemViewModel(string userId, ReportItemViewModel model)
        {
            var us = _context.UserSquares.Single(x => x.UserId == userId && x.Id == model.Id);

            long elappsedTotal = 0;
            model.ActivityRecords.ForEach(ar =>
            {
                var usa = us.UserSquareActivities.Single(x => x.Id == ar.Id);
                //usa.StartUtc = ar.StartDate;
                var endDate =
                    ar.StartDate.AddDays(ar.Duration.Days)
                        .AddHours(ar.Duration.Hours)
                        .AddMinutes(ar.Duration.Minutes)
                        .AddSeconds(ar.Duration.Seconds);
                //usa.BeginMilliseconds = (long)(endDate - ar.StartDate).TotalMilliseconds;
                //elappsedTotal += usa.BeginMilliseconds;

            });
            us.RunningTime = elappsedTotal;
            _context.SaveChanges();
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

        void SaveUserSquareActivityForTimer(string userId, TimerActionModel model)
        {
            var userSquare = _context.UserSquares.Single(x => x.UserId == userId && x.Id == model.ParentId);
            var userSquareActivity = _context.UserSquareActivities.SingleOrDefault(x => x.Id == model.Id);
            if (userSquareActivity != null)
            {
                userSquare.ActivityState = (int)model.ActivityState;
                userSquareActivity.Elapsed = model.Elapsed;

                userSquareActivity.LastUpdatedUtc = System.DateTime.UtcNow;
                model.ParentId = userSquare.Id;
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

            

        }
        public void HandleTimerActionModel(string userId, TimerActionModel model)
        {
            SaveUserSquareActivityForTimer(userId, model);
        }
    }



}