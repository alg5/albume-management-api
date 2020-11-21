using AlbumManagement.Models;
using AlbumManagement.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlbumManagement.Controllers
{
    public class UserController : ControllerBase
    {
        private ApplicationContext db;
        private readonly UserService userService;
        public UserController(ApplicationContext context, UserService userService)
        {
            db = context;
            this.userService = userService;
        }
 
        [HttpPost]
        //[ValidateAntiForgeryToken]
 
        public IActionResult Login([FromBody] User model)
        {

            string res = userService.Login(model);
            return Ok(res);
        }
        [HttpPost]
       public IActionResult ChangePreference([FromBody] User model)
        {

            string res = userService.ChangePreference(model);
            return Ok(res);
        }
        private async Task Authenticate(string userName)
        {
            // create one claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

    }
}
