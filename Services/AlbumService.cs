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
//using Microsoft.Office.Interop.Excel;

using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

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

         public IActionResult AlbumExportToExcel(Album[] albums)
        {
            try
            {

                //Create a workbook
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Albums");
                    worksheet.Name = "AlbumList";

                    //TODO
                    //Adding a picture
                    //FileStream imageStream = new FileStream("AdventureCycles-Logo.png", FileMode.Open, FileAccess.Read);
                    //IPictureShape shape = worksheet.Pictures.AddPicture(1, 1, imageStream);

                    //Disable gridlines in the worksheet
                    //worksheet.IsGridLinesVisible = false;

                    worksheet.Cell(1, 1).Value = "My albums";
                    worksheet.Cell(1, 2).Value = "Date : " + DateTime.Now.ToShortDateString();

                    int rowcount = 3;
                    int columnCount = 4;
                    CreateExcelHeader(worksheet, rowcount, columnCount);

                    foreach (Album album in albums)
                    {
                        rowcount += 1;
                        CreateExcelRow(worksheet, rowcount, columnCount, album);
                    }
                    string fileName = string.Format("albumList_{0}.xlsx", DateTime.Now.ToString("MM.dd.yyyy_hh.mm.ss"));
                    string folderName = Path.Combine("wwwroot", "excel");
                    string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    string fullPath = Path.Combine(pathToSave, fileName);
                    if (!Directory.Exists(pathToSave))
                    {
                        Directory.CreateDirectory(pathToSave);
                    }
                    //Saving the Excel to the MemoryStream 
                    // using (var stream = new MemoryStream())
                    //{
                    //    workbook.SaveAs(stream);
                    //    stream.Position = 0;
                    //    var content = stream.ToArray();
                    //    //FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                    //    //workbook.SaveAs(fileStream);
                    //    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //      return  File(content, contentType, fileName);
                    //}

                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return null;

     
        }
        public void CreateExcelRow(IXLWorksheet worksheet, int rowcount, int columnCount, Album album)
        {
            for (int col = 1; col <= columnCount; col++)
            {

                var cellText = col switch
                {
                    1 => album.Caption,
                    2 => album.IssueYear.ToString(),
                    3 => album.NameArtist,
                    4 => album.GenresDesc
                };
                worksheet.Cell(rowcount, col).Value = cellText.ToString();

                //TODO
                //for alternate rows
                //    if (col == columnCount)
                //{
                //    if (rowcount % 2 == 0)
                //    {
                //        IRange range = worksheet.Range[rowcount, 1, rowcount, columnCount];
                //        FormattingExcelCells(range, "#CCCCFF", ExcelKnownColors.Black, false);
                //    }
                //}

            }
        }            
        public void CreateExcelHeader(IXLWorksheet worksheet, int rowcount, int columnCount)
        {
            for (int col = 1; col <= columnCount; col++)
            {
                    var headerText = col switch
                {
                    1 => "Caption",
                    2 => "IssueYear",
                    3 => "NameArtist",
                    4 => "Genres",
                    0 => throw new NotSupportedException(),

                };
                worksheet.Cell(rowcount, col).Value = headerText.ToString();
                //Make the text bold and color black
                worksheet.Cell(rowcount, col).Style.Font.Bold = true;
                worksheet.Cell(rowcount, col).Style.Font.FontColor = XLColor.Black;

            }
        }


        //public void FormattingExcelCells(IRange range, string HTMLcolorCode, ExcelKnownColors fontColor, bool IsFontbool)
        //{
        //    //range.CellStyle.Interior.Color = System.Drawing.ColorTranslator.FromHtml(HTMLcolorCode);
        //    range.CellStyle.Font.Color = fontColor;
        //    if (IsFontbool == true)
        //    {
        //        range.CellStyle.Font.Bold = IsFontbool;
        //    }
        //}
    }
}
