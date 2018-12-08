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
    public class HomeController : Controller
    {
        private BeltExamContext dbContext;

        public HomeController(BeltExamContext context)
        {
            dbContext = context;
        }

        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                var dbUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                if (dbUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                User newUSer = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password
                };
                dbContext.Add(newUSer);
                dbContext.SaveChanges();
                dbUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                return RedirectToAction("Dashboard", "App");
            }
            return View("Index");
        }

        [HttpPost("signin")]
        public IActionResult Signin(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                var dbUser = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
                if (dbUser == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, dbUser.Password, user.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                return RedirectToAction("Dashboard", "App");
            }
            return View("Index");
        }
    }
}
