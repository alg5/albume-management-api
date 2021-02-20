using AlbumManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumManagement.Controllers
{
    public class CustomerPrController : ControllerBase
    {
        private readonly CustomerPrService customerPrService;
        public CustomerPrController(CustomerPrService customerPrService)
        {
            this.customerPrService = customerPrService;
        }
        [HttpGet]
        //[ValidateAntiForgeryToken]

        public IActionResult Login( string idUser)
        {

            string res = customerPrService.Login(idUser);
            return Ok(res);
        }
        //public IActionResult Index()
        //{
        //    return "kva";
        //}
    }
}
