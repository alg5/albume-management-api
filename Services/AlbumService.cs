using AlbumManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlbumManagement.Services
{
 
    public class AlbumService
    {
        private ApplicationContext db;
        public AlbumService(ApplicationContext context)
        {
            db = context;
        }
        public string GetAlbums(int id)
        {
            int errorCode = 0;
            string result = string.Empty;
            IList<Album> lstAlbums = null;
 
           // int totalRows = 0;
            try
            {
                lstAlbums = (from album in db.AlbumList.Include(u => u.Owner).Include(p => p.Picture)
                             where album.Owner.Id == @id
                             select album).ToList();
            }
            catch(Exception ex)
            {
                errorCode = -1;
            }
            finally
            {
                var objects = new { GetAlbums = lstAlbums, ErrorCode = errorCode };
                result = JsonSerializer.Serialize(objects);
            }
            return result;
        }

    }
}
