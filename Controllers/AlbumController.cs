 using AlbumManagement.Models;
using AlbumManagement.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlbumManagement.Controllers
{
    //[Route("")]
    //[Route("Album")]
    //[Route("[controller]/[action]")]
    //[ApiController]
    public class AlbumController : ControllerBase
    {
        private ApplicationContext db;
        private readonly UserService userService;
        private readonly AlbumService albumService;

        public AlbumController(ApplicationContext context, UserService userService, AlbumService albumService)
        {
            db = context;
            this.userService = userService;
            this.albumService = albumService;
        }

        public IActionResult Index()
        {
            string res = "Welcome to the club";

            return Ok(res);
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public IActionResult GetAlbums(int id)
        {
            string res = albumService.GetAlbums(id);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult AddAlbum([FromBody] Album model)
        {
             string res = albumService.AddAlbum(model);
            return Ok(res);
        }


        [HttpPost]
        public IActionResult UploadAlbumImage([FromForm] IFormFile uploadedFile)
        {
            try
            {

                string subfolder = HttpContext.Request.Query["login"].ToString();
                string res = albumService.UploadAlbumImage(uploadedFile, subfolder);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //var file = Request.Form.Files[0];
        [HttpGet]
        //  [Route("[controller]/[action]")]
        public IActionResult GetAlbumDetails( int idAlbum)
        {
            string res = albumService.GetAlbumDetails(idAlbum);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult UpdateAlbum([FromBody] Album model)
        {
            string res = albumService.UpdateAlbum(model);
            return Ok(res);
        }

        [HttpGet]
        public IActionResult DeleteAlbum(int idAlbum)
        {
            string res = albumService.DeleteAlbum(idAlbum);
            return Ok(res);
        }

        [HttpGet]
        public IActionResult ValidateAlbumCaptionNotTaken(int idUser, string caption)
        {
            string res = albumService.ValidateAlbumCaptionNotTaken(idUser, caption);
            return Ok(res);
        }
        [HttpPost]
        public IActionResult AlbumExportToExcel([FromBody] Album[] albumList)
        {
            try
            {
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
                    albumService.CreateExcelHeader(worksheet, rowcount, columnCount);

                    foreach (Album album in albumList)
                    {
                        rowcount += 1;
                        albumService.CreateExcelRow(worksheet, rowcount, columnCount, album);
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
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        var content = stream.ToArray();
                        //FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                        //workbook.SaveAs(fileStream);
                        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO
                string s = ex.Message;
                // return Json(new { status = "error", Message = ex.Message });
                HttpContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                 return null;
 
            }





    //            string res = albumService.AlbumExportToExcel(albumList);
//            return Ok(res);
        }

    }
}
