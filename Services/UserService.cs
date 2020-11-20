using AlbumManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlbumManagement.Services
{

    public class UserService
    {
        private ApplicationContext db;
        public UserService(ApplicationContext context)
        {
            db = context;
        }
        public string Login(User userLog)
        {
            int errorCode = 0;
            string result = string.Empty;
            User user = null;
            try
            {
                user = (from u in db.Users
                        where u.Name == userLog.Name && u.Password == userLog.Password
                        select u).First();
                if (user != null)
                {
                    //await Authenticate(model.Name); 
                }
            }
            catch (Exception ex)
            {
                errorCode = -1;
            }
            finally
            {
                var objects = new { Login = user, ErrorCode = errorCode };
                result = JsonSerializer.Serialize(objects);
            }
            return result;
        }

        // private async Task Authenticate(string userName)
        //{
        //    // create one claim
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
        //    };
        //    // создаем объект ClaimsIdentity
        //    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        //    // установка аутентификационных куки
        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        //}

        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction("Login", "User");
        //}
    }
}
