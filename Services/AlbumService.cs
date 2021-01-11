using AlbumManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlbumManagement.Services
{
 
    public class AlbumService
    {
        private ApplicationContext db;
        HttpRequest Request;
        public AlbumService(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            db = context;
            Request = httpContextAccessor.HttpContext.Request;
        }
        public string GetAlbums(int id)
        {
            int errorCode = 0;
            string result = string.Empty;
            IList<Album> lstAlbums = null;
            //User user = null;
 
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

        public string UploadAlbumImage( IFormFile file, string subFolder)
        {
            string result = string.Empty;
            string errorMessage = string.Empty;
            int errorCode = 0;
            try
            {
 
                string folderName = Path.Combine("wwwroot", "images", subFolder);
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string fileName = string.Empty;
                string fullPath = string.Empty;
                string dbPath;

                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fullPath = Path.Combine(pathToSave, fileName);
                    dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                }
                string path = $"{Request.Scheme}://{this.Request.Host}{this.Request.PathBase}//images//{subFolder}//{fileName}";

                FileModel f = new FileModel { Name = fileName, Path = path };
                var objects = new { UploadAlbumImage = f, ErrorCode = errorCode, ErroMessage = errorMessage };
                result = JsonSerializer.Serialize(objects);
                return result;
            }
            catch (Exception ex)
            {
                errorCode = -1;
                errorMessage = ex.Message;
                throw new Exception(ex.Message);
                //return BadRequest(ex);
            }
 

        }


        public string AddAlbum(Album albumSrc)
        {
            int errorCode = 0;
            string result = string.Empty;
            Album newAlbum = null;
            FileModel picture = null;
            try
            {
                picture = albumSrc.Picture;
                db.Pictures.Add(picture);
                db.SaveChanges();

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT AlbumList(Caption, IssueYear, NameArtist, Genres, GenresDesc, PictureId, OwnerId)");
                sb.Append("VALUES (");
                sb.AppendFormat("'{0}', ", albumSrc.Caption);
                sb.AppendFormat("{0}, ", albumSrc.IssueYear);
                sb.AppendFormat("'{0}', ", albumSrc.NameArtist);
                sb.AppendFormat("{0}, ", (int)albumSrc.Genres);
                sb.AppendFormat("'{0}', ", albumSrc.GenresDesc);
                sb.AppendFormat("{0}, ", albumSrc.Picture.Id);
                sb.AppendFormat("{0} ", albumSrc.Owner.Id);
                sb.Append(")");
                string commandText = sb.ToString();
                db.Database.ExecuteSqlRaw(commandText);
                newAlbum = db.AlbumList.Include(u => u.Owner).Include(p => p.Picture).FirstOrDefault(album => album.Id == db.AlbumList.Max(x => x.Id));
                //(from album in db.AlbumList.Include(u => u.Owner).Include(p => p.Picture)
                //            where album.Id == Max
                //            select album)
            }
            catch (Exception ex)
            {
                //TODO
                string s = ex.Message;
                errorCode = -1;
            }
            finally
            {
                var objects = new { AddAlbum = newAlbum, ErrorCode = errorCode };
                result = JsonSerializer.Serialize(objects);
            }
            return result;
        }

        public string GetAlbumDetails(int idAlbum)
        {
            int errorCode = 0;
            string result = string.Empty;
            Album album = null;
            try
            {
 
                album = db.AlbumList.Include(u => u.Owner).Include(p => p.Picture).FirstOrDefault(album => album.Id == idAlbum);
                if( null == album)
                {
                    errorCode = -1;

                }
 
            }
            catch (Exception ex)
            {
                errorCode = -1;
                //TODO
                string s = ex.Message;
            }
            finally
            {
                var objects = new { GetAlbumDetails = album, ErrorCode = errorCode };
                result = JsonSerializer.Serialize(objects);
            }



            return result;

        }

        public string UpdateAlbum(Album albumSrc)
        {
            int errorCode = 0;
            string result = string.Empty;
            Album album = null;
            FileModel picture = null;
            //FileModel picture = null;
            try
            {
                //album = db.AlbumList.Include(u => u.Owner).Include(p => p.Picture).FirstOrDefault(album => album.Id == albumSrc.Id);
                album = db.AlbumList.Include(p => p.Picture).FirstOrDefault(album => album.Id == albumSrc.Id);
                if (null == album)
                {
                    errorCode = -1;
                    throw new Exception("album not found");

                }
                album.Caption = albumSrc.Caption;
                album.IssueYear = albumSrc.IssueYear;
                album.NameArtist = albumSrc.NameArtist;
                album.Genres = albumSrc.Genres;
                album.GenresDesc = albumSrc.GenresDesc;
                album.Picture.Name = albumSrc.Picture.Name;
                album.Picture.Path = albumSrc.Picture.Path;
                db.AlbumList.Update(album);
                //picture = db.Pictures.FirstOrDefault(pic => pic.Id == album.Picture.Id);
                //if (picture == null)
                //{
                //    errorCode = -1;
                //    throw new Exception("picture not found");

                //}
                //picture.Name = album.Picture.Name;
                //picture.Path = album.Picture.Path;
                //db.Pictures.Update(picture);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                //TODO
                string s = ex.Message;
                errorCode = -1;
            }
            finally
            {
                var objects = new { UpdateAlbum = album, ErrorCode = errorCode };
                result = JsonSerializer.Serialize(objects);
            }



            return result;

        }

        public string DeleteAlbum( int idAlbum)
        {
            int errorCode = 0;
            string result = string.Empty;
            Album album = null;
            try
            {

                album = db.AlbumList.Include(u => u.Owner).Include(p => p.Picture).FirstOrDefault(album => album.Id == idAlbum);
                if (null == album)
                {
                    errorCode = -1;
                    throw new Exception("album not found");

                }
                db.AlbumList.Remove(album);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                //TODO
                string s = ex.Message;
                errorCode = -1;
            }
            finally
            {
                var objects = new { DeleteAlbum = album, ErrorCode = errorCode };
                result = JsonSerializer.Serialize(objects);
            }



            return result;

        }

        public string ValidateAlbumCaptionNotTaken(int idUser, string albumCaption)
        {
            int errorCode = 0;
            string result = string.Empty;
            string msg = string.Empty;
            Album album = null;
            bool b = false;
            try
            {

                album = db.AlbumList.FirstOrDefault(album => album.Owner.Id == idUser && album.Caption == albumCaption);
                if (null == album)
                {
                    b = true; 

                }

            }
            catch (Exception ex)
            {
                errorCode = -1;
                //TODO
                msg = ex.Message;
            }
            finally
            {
                var objects = new { ValidateAlbumCaptionNotTaken = b, ErrorCode = errorCode, Message = msg };
                result = JsonSerializer.Serialize(objects);
            }



            return result;

        }


    }
}
