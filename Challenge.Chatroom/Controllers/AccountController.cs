using System;
using System.Collections.Generic;
using System.Security.Claims;
using Challenge.Chatroom.Db.Context;
using Challenge.Chatroom.Db.Model;
using Challenge.Chatroom.Models.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Challenge.Chatroom.Controllers
{
    public class AccountController : Controller
    {
        public DataAccess db;

        public AccountController(IConfiguration Configuration)
        {
            var connectionString = Configuration.GetConnectionString("connection");
            db = new DataAccess(connectionString);
        }        

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(User user)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                var infoUser = ValidateUser(user.Identification, user.Password);

                if (infoUser.Id > 0)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("FullName", infoUser.FullName),
                        new Claim("Identification", infoUser.Identification),
                        new Claim("IdUser", infoUser.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                        IsPersistent = true
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return View("~/Views/Chat/Index.cshtml");

                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult RegisterUser(User user)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                db.RegisterUser(user);
                return View("Login");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private User ValidateUser(string Identification, string Password)
        {
            var result = new User();
            var user = db.GetUserByIdentification(Identification);
            if (user != null)
            {
                var password = Utilities.EncryptMd5(Password);
                if (password == user.Password)
                {
                    result = new User()
                    {
                        Id = user.Id,
                        Identification = user.Identification,
                        FullName = user.FullName
                    };
                }
            }

            return result;
        }

        public User GetUserInformation()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userInfo = new User();

            if (claimsIdentity.IsAuthenticated)
            {
                var claims = claimsIdentity.Claims;
                foreach (var claim in claims)
                {
                    var x = claim;
                    switch (claim.Type)
                    {
                        case "FullName": userInfo.FullName = claim.Value; break;
                        case "Identification": userInfo.Identification = claim.Value; break;
                        case "IdUser": userInfo.Id = Int32.Parse(claim.Value); break;
                    }
                }
            }
            return userInfo;
        }
    }
}