using AlbumManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumManagement.Services
{
    public class CustomerPrService
    {
        IMemoryCache _memoryCache = null;
        string key_users = "users";
        public CustomerPrService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public string Login(string idUser)
        {
            string result = string.Empty;
            int errorCode = 0;
            CustomerPr user = null;
            try
            {
                List<CustomerPr> users = null;
                if (!_memoryCache.TryGetValue(key_users, out users))
                {
                    // Key not in cache, so get data.
                    users = FillUserListDefault();
                    if (users == null)
                    {
                       throw new Exception("User list is empty");
                    }

                    // Save data in cache.
                    _memoryCache.Set(key_users, users);
                    //debug
                    string usersAsJson = System.Text.Json.JsonSerializer.Serialize(users);
                }
                user = users.Where(user => user.id == idUser).FirstOrDefault();
                if (user == null)
                {
                    errorCode = -1;
                }
            }
            catch (Exception ex)
            {
                errorCode = -1;
            }
            finally
            {
                var objects = new { GetUserById = user, ErrorCode = errorCode };
                result = System.Text.Json.JsonSerializer.Serialize(objects);
            }

            return result;

        }
        private List<CustomerPr> FillUserListDefault()
        {
            try
            {
                List<Package> packages1 = new List<Package>
                {
                    new Package { id = 1, name = "SMS", amount = 130, used = 15 },
                    new Package { id = 2, name = "Calls", amount = 2000, used = 50 },
                    new Package { id = 3, name = "Internet", amount = 30, used = 27 }

                };
                List<Package> packages2 = new List<Package>
                {
                    new Package { id = 1, name = "SMS", amount = 630, used = 12 },
                    new Package { id = 2, name = "Calls", amount = 200, used = 50 },
                    new Package { id = 3, name = "Internet", amount = 5000, used = 300 },
                    new Package { id = 3, name = "CallAbroad", amount = 440, used = 127 },
                };
                //List<Package> packages3 = packages2.ToList();
                List<Package> packages3 = new List<Package>
                {
                    new Package { id = 1, name = "SMS", amount = 730, used = 12 },
                    new Package { id = 3, name = "Internet", amount = 5120, used = 300 },
                    new Package { id = 3, name = "CallAbroad", amount = 4401, used = 127 },
                };

                List<Abonement> abonements1 = new List<Abonement>
                {
                    new Abonement { id = 1, name = "HOT Mobile", packages = packages1},
                    new Abonement { id = 2, name = "Golan Telecom", packages = packages2},
                    new Abonement { id = 3, name = "Partner", packages = packages1},
                 };
                List<Abonement> abonements2 = new List<Abonement>
                {
                    new Abonement { id = 1, name = "HOT Mobile", packages = packages1},
                    new Abonement { id = 4, name = "012", packages = packages2},
                };
                List<Abonement> abonements3 = new List<Abonement>
                {
                    new Abonement { id = 1, name = "HOT Mobile", packages = packages3},
                };
                List<CustomerPr> users = new List<CustomerPr>
                {
                    new CustomerPr { id = "123456789", username = "Adam", abonements = abonements1},
                    new CustomerPr { id = "987654321", username = "Hava", abonements = abonements2},
                    new CustomerPr { id = "111111111", username = "Me", abonements = abonements3},
                };
                return users;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    
}
