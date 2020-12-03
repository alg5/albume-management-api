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
                        where u.Login == userLog.Login && u.Password == userLog.Password
                        select u).First();
                if (user != null)
                {
                    user.Password = string.Empty;
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

        public string ChangePreference(User userPref)
        {
            int errorCode = 0;
            string result = string.Empty;
            User user = null;
            try
            {
                user = (from u in db.Users
                        where u.Id == userPref.Id
                        select u).First();
                if (user != null)
                {
                    user.IssueYearPreferenceFilter = userPref.IssueYearPreferenceFilter;
                    user.IssueYearPreferenceSort = userPref.IssueYearPreferenceSort;
                    user.NameArtistPreferenceFilter = userPref.NameArtistPreferenceFilter;
                    user.NameArtistPreferenceSort = userPref.NameArtistPreferenceSort;
                    db.Users.Update(user);
                    db.SaveChanges();
                    user.Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                errorCode = -1;
            }
            finally
            {
                var objects = new { ChangePreference = user, ErrorCode = errorCode };
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
