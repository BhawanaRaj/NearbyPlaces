using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NearbyPlaces
{
    public class Utilities
    {
        public static double Distance(double lat1,double lon1,double lat2,double lon2) 
        {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2-lat1);  // deg2rad below
            var dLon = deg2rad(lon2-lon1); 
            var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
            var d = R * c; // Distance in km
            return d;
        }

        private static double deg2rad(double deg) 
        {
            return deg * (Math.PI / 180);
        }

        public static async Task<List<Place>> GetPlaces(params String[] types)
        {
            String Types = "";
            foreach(String type in types)
            {
                Types += type.ToLower().Replace(' ','_') + "|";
            }
            Types=Types.Remove(Types.Length - 1);
            List<Place> Places = new List<Place>();
            String url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location="+App.LAT.ToString()+","+App.LONG.ToString()+"&rankby=distance&types="+Types+"&key=AIzaSyD3jfeMZK1SWfRFDgMfxn_zrGRSjE7S8Vg";
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string result = response.Content.ReadAsStringAsync().Result;
                var json = (JObject)JsonConvert.DeserializeObject(result);
                var results = (JArray)JsonConvert.DeserializeObject(json["results"].ToString());
                foreach (var place in results)
                {
                    Place p = new Place();
                    
                    p.Name = place["name"].ToString();
                    p.Vicinity = place["vicinity"].ToString();
                    p.LAT =(double)place["geometry"]["location"]["lat"];
                    p.LONG = (double)place["geometry"]["location"]["lng"];
                    p.Distance = Math.Round(Utilities.Distance(App.LAT, App.LONG, p.LAT, p.LONG),2).ToString()+"Km";
                    Places.Add(p);
                }
            }
            catch (Exception ex)
            {

            }
            return Places;
        }
    }
}
