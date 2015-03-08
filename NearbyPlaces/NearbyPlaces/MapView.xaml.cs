using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Device.Location;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Microsoft.Phone.Maps.Services;
using System.IO.IsolatedStorage;

namespace NearbyPlaces
{
    public partial class MapView : PhoneApplicationPage
    {
        private static List<Place> PlaceList;
        RouteQuery MyQuery = null;
        MapRoute route;
        List<GeoCoordinate> MyCoordinates;
        Place destination;
        public MapView()
        {
            InitializeComponent();
            PlaceList = (List<Place>)PhoneApplicationService.Current.State["MapPlaces"];
            MapD.Center = new System.Device.Location.GeoCoordinate(App.LAT, App.LONG);
            MapD.ZoomLevel = 13;
            MapLayer layer0 = new MapLayer();

            Pushpin pushpin0 = new Pushpin() { Foreground = new SolidColorBrush(Colors.White) };
            pushpin0.Content = "Me";
            pushpin0.GeoCoordinate = new GeoCoordinate(App.LAT,App.LONG);
            pushpin0.Background = new SolidColorBrush(Colors.Green);
            MapOverlay overlay0 = new MapOverlay();
            overlay0.Content = pushpin0;
            overlay0.GeoCoordinate = new GeoCoordinate(App.LAT, App.LONG);
            layer0.Add(overlay0);

            foreach(Place p in PlaceList)
            {
                Pushpin pushpin1 = new Pushpin() {  Foreground=new SolidColorBrush(Colors.White)};
                pushpin1.GeoCoordinate = new GeoCoordinate(p.LAT,p.LONG);
                pushpin1.Tag = "p"+PlaceList.IndexOf(p);
                pushpin1.Content = p.Name ;
                pushpin1.Background=new SolidColorBrush(Colors.Red);
                MapOverlay overlay1 = new MapOverlay();
                overlay1.Content = pushpin1;
                overlay1.GeoCoordinate = new GeoCoordinate(p.LAT, p.LONG);
                pushpin1.Tap+=pushpin1_Tap;
                layer0.Add(overlay1);                
            }          

           
            MapD.Layers.Add(layer0);
            

        }

        private void pushpin1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            String tag = ((Pushpin)sender).Tag.ToString().Substring(1);
            int idx = int.Parse(tag);
            destination = PlaceList[idx];
            MyCoordinates = new List<GeoCoordinate>();
            MyQuery = new RouteQuery();
            
            MyCoordinates.Add(new GeoCoordinate(App.LAT, App.LONG));
            MyCoordinates.Add(new GeoCoordinate(PlaceList[idx].LAT, PlaceList[idx].LONG));
            MyQuery.Waypoints = MyCoordinates;
            MyQuery.QueryCompleted += MyQuery_QueryCompleted;
            MyQuery.QueryAsync();
            
            
        }

        private void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                if(route!=null)
                    MapD.RemoveRoute(route);
                Route MyRoute = e.Result;
                route = new MapRoute(MyRoute);
                MapD.AddRoute(route);

                List<string> RouteList = new List<string>();
               
                foreach (RouteLeg leg in MyRoute.Legs)
                {
                    foreach (RouteManeuver maneuver in leg.Maneuvers)
                    {
                        RouteList.Add(" • "+maneuver.InstructionText);
                    }
                }

                RouteLLS.ItemsSource = RouteList;
                RouteLLS.Visibility = Visibility.Visible;
                header.Text= destination.Name + " ( " + destination.Vicinity + " )  "+Math.Round(Utilities.Distance(App.LAT,App.LONG,destination.LAT,destination.LONG),2).ToString()+" Km";
                MyQuery.Dispose();
            }
        }

        private void FavAdd(object sender, EventArgs e)
        {
            if(destination!=null)
            {
                String Placename = destination.Name+" ( "+destination.Vicinity+" )";
                List<Place> FavPlaces;
                if (IsolatedStorageSettings.ApplicationSettings.Contains("Favs"))
                    FavPlaces = (List<Place>)IsolatedStorageSettings.ApplicationSettings["Favs"];
                else
                {
                    FavPlaces = new List<Place>();
                    IsolatedStorageSettings.ApplicationSettings.Add("Favs",FavPlaces);
                }
                if(!FavPlaces.Contains(destination))
                {
                    FavPlaces.Add(destination);
                }
                IsolatedStorageSettings.ApplicationSettings["Favs"] = FavPlaces;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        

     
    }
}