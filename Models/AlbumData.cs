using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static AlbumManagement.Models.Enums;

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
                User u1 = new User { Name = "Tom", Password = "12345" } ;
                User u2 = new User { Name = "Jerry", Password = "54321" };
                db.Users.AddRange(u1, u2);

                FileModel f1 = new FileModel { Name = "Beatles", Path = "Beatles.jpg" };
                FileModel f2 = new FileModel { Name = "BeatlesRoad", Path = "BeatlesRoad.jpg" };
                FileModel f3 = new FileModel { Name = "RollingStone", Path = "RollingStone.jpg" };
                db.Pictures.AddRange(f1, f2, f3);

                Album a1 = new Album { Caption = "Beatles", IssueYear = 1965, NameArtist = "Jon Lennon", Genres = GenresEnum.RockMusic, Picture = f1, Owner = u1};
                Album a2 = new Album { Caption = "RollingStone", IssueYear = 1970, NameArtist = "Harry Styles", Genres = GenresEnum.ElectronicMusic, Picture = f3, Owner = u1};
                Album a3 = new Album { Caption = "BeatlesRoad", IssueYear = 1969, NameArtist = "Jon Lennon", Genres = GenresEnum.RockMusic, Picture = f2, Owner = u2};
                db.AlbumList.AddRange(a1, a2, a3);
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
}
