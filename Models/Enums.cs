using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumManagement.Models
{
    public class Enums
    {
        public enum GenresEnum
        {
            [Display(Name = "Rock music")]
            RockMusic = 1,
            [Display(Name = "Electronic music")]
            ElectronicMusic,
            [Display(Name = "Soul music/R&B")]
            SoulMusic,
            [Display(Name = "Funk")]
            Funk,
            [Display(Name = "Country music")]
            CountryMusic,
            [Display(Name = "Latin music")]
            LatinMusic,
            [Display(Name = "Reggae")]
            Reggae,
            [Display(Name = "Hip hop music")]
            HipHopMusic,
            [Display(Name = "Polka")]
            Polka

        }
        public enum SortEnum
        {
            None,
            Asc,
            Desc
        }
    }
}
