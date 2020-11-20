using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        //[NotMapped]
        //[DataType(DataType.Password)]
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Password { get; set; }
        [DefaultValue("qwerty")]
        public string Salt { get; set; }
    }

}
