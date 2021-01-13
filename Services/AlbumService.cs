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
using Microsoft.Office.Interop.Excel;

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

        public string AlbumExportToExcel(Album[] albums)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel;
                Microsoft.Office.Interop.Excel.Workbook excelworkBook;
                Microsoft.Office.Interop.Excel.Worksheet excelSheet;
                Microsoft.Office.Interop.Excel.Range excelCellrange;

                // Start Excel and get Application object.  
                excel = new Microsoft.Office.Interop.Excel.Application();
                // for making Excel visible  
                excel.Visible = false;
                excel.DisplayAlerts = false;
                // Creation a new Workbook  
                excelworkBook = excel.Workbooks.Add(Type.Missing);
                // Workk sheet  
                excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = "AlbumList";

                excelSheet.Cells[1, 1] = "My albums";
                excelSheet.Cells[1, 2] = "Date : " + DateTime.Now.ToShortDateString();

                int rowcount = 2;
                int columnCount = 4;
                foreach (Album album in albums)
                {
                    rowcount += 1;
                    for (int i = 1; i <= columnCount; i++)
                    {
                        // on the first iteration we add the column headers
                        if (rowcount == 3)
                        {
                            var headerText = i switch
                            {
                                1 => "Caption",
                                2 => "IssueYear",
                                3 => "NameArtist",
                                4 => "Genres",
                                0 => throw new NotSupportedException(),

                            };
                            excelSheet.Cells[2, i] = headerText.ToString();
                            excelSheet.Cells.Font.Color = System.Drawing.Color.Black;

                        }
                        else
                        {
                            var cellText = i switch
                            {
                                1 => album.Caption,
                                2 => album.IssueYear.ToString(),
                                3 => album.NameArtist,
                                4 => album.GenresDesc
                            };
                             excelSheet.Cells[rowcount, i] = cellText.ToString();
                        }



                        //for alternate rows
                        if (rowcount > 3)
                        {
                            if (i == columnCount)
                            {
                                if (rowcount % 2 == 0)
                                {
                                    excelCellrange = excelSheet.Range[excelSheet.Cells[rowcount, 1], excelSheet.Cells[rowcount, columnCount]];
                                    FormattingExcelCells(excelCellrange, "#CCCCFF", System.Drawing.Color.Black, false);
                                }

                            }
                        }

                    }

                }



                // now we resize the columns  
                // loop through each row and add values to our sheet

                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowcount, columnCount]];
                excelCellrange.EntireColumn.AutoFit();
                Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
                border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                border.Weight = 2d;
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[2, columnCount]];
                FormattingExcelCells(excelCellrange, "#000099", System.Drawing.Color.White, true);
                //now save the workbook and exit Excel

                string folderName = Path.Combine("wwwroot", "excel");
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                excelworkBook.SaveAs(pathToSave); ;
                excelworkBook.Close();
                excel.Quit();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

            return "";
        }
        public void FormattingExcelCells(Microsoft.Office.Interop.Excel.Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        {
            range.Interior.Color = System.Drawing.ColorTranslator.FromHtml(HTMLcolorCode);
            range.Font.Color = System.Drawing.ColorTranslator.ToOle(fontColor);
            if (IsFontbool == true)
            {
                range.Font.Bold = IsFontbool;
            }
        }

    }
}
