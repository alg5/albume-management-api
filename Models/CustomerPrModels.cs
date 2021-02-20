using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumManagement.Models
{
    public class Package
    {
        public int id { get; set; }
        public string name { get; set; }
        public int amount { get; set; }
        public int used { get; set; }
    }
    public class Abonement
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Package> packages { get; set; }
    }
    public class CustomerPr
    {
        public string id { get; set; }
        public string username { get; set; }
        public List<Abonement> abonements { get; set; }
    }
}
