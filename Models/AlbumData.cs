using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static AlbumManagement.Models.Enums;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AlbumManagement.Models
{
    public class AlbumData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var db = new ApplicationContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationContext>>()))
            {
                // Look for any album.
                if (db.AlbumList.Any())
                {
                    return;   // DB has been created
                }
                User u1 = new User { Login = "Tom",  Password = "12345", Name = "Tom" } ;
                User u2 = new User { Login = "Jerry", Password = "54321", Name = "Jerry" };
                db.Users.AddRange(u1, u2);

                //  string imgUrl = $"{Request.Scheme}://{Request.Host}{this.Request.PathBase}//images//{subfolder}//";

                string baseUrl1 = "http://localhost:5000//images//tom//";
                string baseUrl2 = "http://localhost:5000//images//jerry//";
                FileModel f1 = new FileModel { Name = "Beatles.jpg", Path = $"{baseUrl1}Beatles.jpg" };
                FileModel f2 = new FileModel { Name = "BeatlesRoad.jpg", Path = $"{baseUrl1}BeatlesRoad.jpg" };
                FileModel f3 = new FileModel { Name = "RollingStone.jpg", Path = $"{baseUrl1}RollingStone.jpg" };
                FileModel f4 = new FileModel { Name = "MichaelJackson.jpg", Path = $"{baseUrl1}MichaelJackson.jpg" };
                FileModel f5 = new FileModel { Name = "BeatlesRoad.jpg", Path = $"{baseUrl2}BeatlesRoad.jpg" };
                db.Pictures.AddRange(f1, f2, f3, f4);

                Album a1 = new Album { Caption = "Beatles", IssueYear = 1965, NameArtist = "Jon Lennon", Genres = GenresEnum.RockMusic, GenresDesc = GenresEnum.RockMusic.GetAttribute <DisplayAttribute> ().Name, Picture = f1, Owner = u1};
                Album a2 = new Album { Caption = "Rolling Stone", IssueYear = 1970, NameArtist = "Harry Styles", Genres = GenresEnum.ElectronicMusic, GenresDesc = GenresEnum.ElectronicMusic.GetAttribute<DisplayAttribute>().Name, Picture = f3, Owner = u1};
                Album a3 = new Album { Caption = "Beatles Road", IssueYear = 1966, NameArtist = "Jon Lennon", Genres = GenresEnum.RockMusic, GenresDesc = GenresEnum.RockMusic.GetAttribute<DisplayAttribute>().Name, Picture = f5, Owner = u2};
                Album a4 = new Album { Caption = "Michael Jackson Cover", IssueYear = 1969, NameArtist = "Michael Jackson", Genres = GenresEnum.ElectronicMusic, GenresDesc = GenresEnum.ElectronicMusic.GetAttribute<DisplayAttribute>().Name, Picture = f4, Owner = u1};
                Album a5 = new Album { Caption = "Beatles Road", IssueYear = 1969, NameArtist = "Jon Lennon", Genres = GenresEnum.RockMusic, GenresDesc = GenresEnum.RockMusic.GetAttribute<DisplayAttribute>().Name, Picture = f2, Owner = u1 };
                db.AlbumList.AddRange(a1, a2, a3, a4, a5);
                db.SaveChanges();


            }
        }

        private string getSalt()
        {
            var random = new RNGCryptoServiceProvider();

            // Maximum length of salt
            int max_length = 32;

            // Empty salt array
            byte[] salt = new byte[max_length];

            // Build the random bytes
            random.GetNonZeroBytes(salt);

            // Return the string encoded salt
            return Convert.ToBase64String(salt);
        }
    }
    public static class Extensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }

}
