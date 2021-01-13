 using AlbumManagement.Models;
using AlbumManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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
            string res = albumService.AlbumExportToExcel(albumList);
            return Ok(res);
        }

    }
}
