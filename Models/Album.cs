using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static AlbumManagement.Models.Enums;

namespace AlbumManagement.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        public int IssueYear { get; set; }

        [Required]
        public string NameArtist { get; set; }

        public GenresEnum Genres { get; set; }
        public string GenresDesc { get; set; }

        //[Required]
        //[BindProperty]
        public FileModel Picture { get; set; }


       // [Required]
       // [BindProperty]
        public User Owner { get; set; }



    }
}
