using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearbyPlaces
{
    public class Place
    {
        public String Vicinity { get; set; }
        public String Name { get; set; }
        public String ImgUrl { get; set; }
        public double LAT { get; set; }
        public double LONG { get; set; }
        public String Distance { get; set; }
    }
}
