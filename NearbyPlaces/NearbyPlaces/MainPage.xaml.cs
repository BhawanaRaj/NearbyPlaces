using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace NearbyPlaces
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static String[] PlaceTypes = {"accounting", "airport", "amusement_park", "aquarium", "art_gallery", "atm", "bakery", "bank", "bar", "beauty_salon", "bicycle_store", "book_store", "bowling_alley", "bus_station", "cafe", "campground", "car_dealer", "car_rental", "car_repair", "car_wash", "casino", "cemetery", "church", "city_hall", "clothing_store", "convenience_store", "courthouse", "dentist", "department_store", "doctor", "electrician", "electronics_store", "embassy", "establishment", "finance", "fire_station", "florist", "food", "funeral_home", "furniture_store", "gas_station", "general_contractor", "grocery_or_supermarket", "gym", "hair_care", "hardware_store", "health", "hindu_temple", "home_goods_store", "hospital", "insurance_agency", "jewelry_store", "laundry", "lawyer", "library", "liquor_store", "local_government_office", "locksmith", "lodging", "meal_delivery", "meal_takeaway", "mosque", "movie_rental", "movie_theater", "moving_company", "museum", "night_club", "painter", "park", "parking", "pet_store", "pharmacy", "physiotherapist", "place_of_worship", "plumber", "police", "post_office", "real_estate_agency", "restaurant", "roofing_contractor", "rv_park", "school", "shoe_store", "shopping_mall", "spa", "stadium", "storage", "store", "subway_station", "synagogue", "taxi_stand", "train_station", "travel_agency", "university", "veterinary_care", "zoo" };
        public static String[] Categories = {"accounting", "airport", "amusement park", "aquarium", "art gallery", "atm", "bakery", "bank", "bar", "beauty salon", "bicycle store", "book store", "bowling alley", "bus station", "cafe", "campground", "car dealer", "car rental", "car repair", "car wash", "casino", "cemetery", "church", "city hall", "clothing store", "convenience store", "courthouse", "dentist", "department store", "doctor", "electrician", "electronics store", "embassy", "establishment", "finance", "fire station", "florist", "food", "funeral home", "furniture store", "gas station", "general contractor", "grocery or supermarket", "gym", "hair care", "hardware store", "health", "hindu temple", "home goods store", "hospital", "insurance agency", "jewelry store", "laundry", "lawyer", "library", "liquor store", "local government office", "locksmith", "lodging", "meal delivery", "meal takeaway", "mosque", "movie rental", "movie theater", "moving company", "museum", "night club", "painter", "park", "parking", "pet store", "pharmacy", "physiotherapist", "place of worship", "plumber", "police", "post office", "real estate agency", "restaurant", "roofing contractor", "rv park", "school", "shoe store", "shopping mall", "spa", "stadium", "storage", "store", "subway station", "synagogue", "taxi stand", "train station", "travel agency", "university", "veterinary care", "zoo"};
        ObservableCollection<Place> FavPlaces;
        public MainPage()
        {
            InitializeComponent();
            List<String> stations = Categories.ToList();


            List<AlphaKeyGroup<String>> DataSource = AlphaKeyGroup<String>.CreateGroups(stations,
                System.Threading.Thread.CurrentThread.CurrentUICulture,
                (String s) => { return s; }, true);

            CategoryList.ItemsSource = DataSource;

            LoadFaourites();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadFaourites();
        }

        private void LoadFaourites()
        {
            pbar.Visibility = Visibility.Visible;
            loadingtext.Visibility = Visibility.Visible;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork+=worker_DoWork;
            worker.RunWorkerCompleted+=worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbar.Visibility = Visibility.Collapsed;
            loadingtext.Visibility = Visibility.Collapsed;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FavPlaces= new ObservableCollection<Place>();
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Favs"))
                FavPlaces = new ObservableCollection<Place>((List<Place>)IsolatedStorageSettings.ApplicationSettings["Favs"]);
            Dispatcher.BeginInvoke(() =>
            {
                FavPlacesList.DataContext = FavPlaces;
            });
           
        }

        private void TypeTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("CategorySelected"))
                PhoneApplicationService.Current.State["CategorySelected"] = ((TextBlock)sender).Text.ToString();
            else
                PhoneApplicationService.Current.State.Add("CategorySelected", ((TextBlock)sender).Text.ToString());
            this.NavigationService.Navigate(new Uri("/CategoryView.xaml", UriKind.Relative));
        }

        private void FavSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavPlacesList.SelectedIndex == -1)
                return;
            List<Place> selected = new List<Place>();
            selected.Add(FavPlaces[FavPlacesList.SelectedIndex]);
            FavPlacesList.SelectedIndex = -1;

            if (PhoneApplicationService.Current.State.ContainsKey("MapPlaces"))
                PhoneApplicationService.Current.State["MapPlaces"] = selected;
            else
                PhoneApplicationService.Current.State.Add("MapPlaces", selected);
            this.NavigationService.Navigate(new Uri("/MapView.xaml", UriKind.Relative));
            
        }

       

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = this.FavPlacesList.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
            int idx =FavPlacesList.Items.IndexOf(item.DataContext);
            FavPlaces.RemoveAt(idx);
            IsolatedStorageSettings.ApplicationSettings["Favs"] = FavPlaces.ToList();
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

       


       
        
    }
}