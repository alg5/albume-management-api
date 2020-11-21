using AlbumManagement.Models;
using AlbumManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumManagement.Controllers
{
    [Route("")]
    [Route("Album")]
    [Route("[controller]/[action]")]
    [ApiController]
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


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
