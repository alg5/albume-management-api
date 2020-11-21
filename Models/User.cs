using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static AlbumManagement.Models.Enums;

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
        [DefaultValue("")]
        public string Salt { get; set; }

        public int IssueYearPreferenceFilter { get; set; }

        public SortEnum IssueYearPreferenceSort { get; set; }

        public string NameArtistPreferenceFilter { get; set; }

        public SortEnum NameArtistPreferenceSort { get; set; }
    }
        

}
