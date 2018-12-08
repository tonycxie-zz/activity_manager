using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BeltExam.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BeltExam.Controllers
{
    public class AppController : Controller
    {
        private BeltExamContext dbContext;

        public AppController(BeltExamContext context)
        {
            dbContext = context;
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
            ViewBag.Message = null;
            ViewBag.Activities = dbContext.Activities
                .Include(a => a.Coordinator)
                .Include(a => a.Participants)
                // .Where(a =>
                // {
                //     DateTime date = a.Date;
                //     DateTime time = a.Time;
                //     DateTime combined = date.Date.Add(time.TimeOfDay);
                //     return combined > DateTime.Now;
                // })
                .OrderBy(a => a.Date); 
            return View();
        }

        [HttpGet("new")]
        public IActionResult New()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");            
            return View();
        }

        [HttpPost("new_something")]
        public IActionResult NewActivity(Activity activity)
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");                        
            if (ModelState.IsValid)
            {
                DateTime date = activity.Date;
                DateTime time = activity.Time;
                DateTime combined = date.Date.Add(time.TimeOfDay);
                if (combined < DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Date and Time must be in the future");
                    return View("New");
                }
                dbContext.Add(activity);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("New");
        }

        [HttpGet("join/{UserId}/{ActivityId}")]
        public IActionResult Join(int UserId, int ActivityId)
        {
            ViewBag.Activities = dbContext.Activities
                .Include(a => a.Coordinator)
                .Include(a => a.Participants)
                .OrderBy(a => a.Date); 
            var User = dbContext.Users
                .Include(u => u.AttendedActivities)
                .ThenInclude(p => p.Activity)
                .FirstOrDefault(u => u.UserId == UserId);
            ViewBag.User = User;
            var Activity = dbContext.Activities.FirstOrDefault(a => a.ActivityId == ActivityId);
            DateTime activityDate = Activity.Date;
            DateTime activityTime = Activity.Time;
            DateTime activityDateTime = activityDate.Add(activityTime.TimeOfDay);
            DateTime activityTotalDuration = activityDateTime;
            if (Activity.DurationType == "Days")
            {
                activityTotalDuration = activityTotalDuration.AddDays(Activity.Duration);
            } 
            else if (Activity.DurationType == "Hours")
            {
                activityTotalDuration = activityTotalDuration.AddHours(Activity.Duration);
            } 
            else 
            {
                activityTotalDuration = activityTotalDuration.AddMinutes(Activity.Duration);
            }
            foreach (var userActivity in User.AttendedActivities)
            {
                DateTime userDate = userActivity.Activity.Date;
                DateTime userTime = userActivity.Activity.Time;
                DateTime userDateTime = userDate.Add(userTime.TimeOfDay);
                DateTime userTotalDuration = userDateTime;
                if (userActivity.Activity.DurationType == "Days")
                {
                    userTotalDuration = userTotalDuration.AddDays(Activity.Duration);
                } 
                else if (userActivity.Activity.DurationType == "Hours")
                {
                    userTotalDuration = userTotalDuration.AddHours(Activity.Duration);
                } 
                else 
                {
                    userTotalDuration = userTotalDuration.AddMinutes(Activity.Duration);
                }
                if (userDateTime >= activityDateTime && userDateTime <= activityTotalDuration)
                {
                    @ViewBag.Message = "Cannot join activity because it overlaps with an activity you already joined";
                    return View("Dashboard");
                }
                else if (activityDateTime >= userDateTime && activityDateTime <= userTotalDuration)
                {
                    @ViewBag.Message = "Cannot join activity because it overlaps with an activity you already joined";
                    return View("Dashboard");
                } 
                else if (userTotalDuration >= activityDateTime && userTotalDuration <= activityTotalDuration)
                {
                    @ViewBag.Message = "Cannot join activity because it overlaps with an activity you already joined";
                    return View("Dashboard");
                }
                else if (activityTotalDuration >= userDateTime && activityTotalDuration <= userTotalDuration)
                {
                    @ViewBag.Message = "Cannot join activity because it overlaps with an activity you already joined";
                    return View("Dashboard");
                }
            }
            Participant newParticipant = new Participant
            {
                UserId = UserId,
                ActivityId = ActivityId
            };
            dbContext.Add(newParticipant);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{UserId}/{ActivityId}")]
        public IActionResult Leave(int UserId, int ActivityId)
        {
            Participant removeParticipant = dbContext.Participants
                .SingleOrDefault(p => (p.UserId == UserId) &&
                                      (p.ActivityId == ActivityId));
            dbContext.Participants.Remove(removeParticipant);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Activity removeActivity = dbContext.Activities.SingleOrDefault(a => a.ActivityId == id);
            dbContext.Activities.Remove(removeActivity);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("activity/{id}")]
        public IActionResult Show(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
            ViewBag.Activity = dbContext.Activities
                .Include(a => a.Coordinator)
                .FirstOrDefault(a => a.ActivityId == id);
            var Users = dbContext.Users
                .Include(u => u.AttendedActivities)
                .ThenInclude(u => u.Activity)
                .ToList();
            ViewBag.Participants = new List<User>();
            foreach (var user in Users)
            {
                foreach (var activity in user.AttendedActivities)
                {
                    if (activity.ActivityId == id)
                    {
                        ViewBag.Participants.Add(user);
                    }
                }
            }
            return View();
        }
    }
}