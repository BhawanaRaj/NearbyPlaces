using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Globalization;
using System.Text;
using System.Net.Http;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Tasks;

namespace NearbyPlaces
{
    public partial class CategoryView : PhoneApplicationPage
    {
        private static String CategoryName;
        private static List<Place> NearbyPlaces;
        bool busy;
        public CategoryView()
        {
            InitializeComponent();
            CategoryName = (string)PhoneApplicationService.Current.State["CategorySelected"];
            busy = true;

            CategoryName = toTitleCase(CategoryName);
            CategoryTitle.Text = CategoryName;

            LoadList();
        }

        private async void LoadList()
        {
            NearbyPlaces = await Utilities.GetPlaces(CategoryName);
            Dispatcher.BeginInvoke(() =>
            {
                if (NearbyPlaces.Count > 0)
                    PlacesList.DataContext = NearbyPlaces;
                else
                    ErrorBlock.Visibility = Visibility.Visible;
            });
            loadingtext.Visibility = Visibility.Collapsed;
            pbar.Visibility = Visibility.Collapsed;
            busy = false;
        }


        private string toTitleCase(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }

        private void MapAll_Click(object sender, EventArgs e)
        {
            if (busy == true)
                return;
            if (PhoneApplicationService.Current.State.ContainsKey("MapPlaces"))
                PhoneApplicationService.Current.State["MapPlaces"] = NearbyPlaces;
            else
                PhoneApplicationService.Current.State.Add("MapPlaces", NearbyPlaces);
            this.NavigationService.Navigate(new Uri("/MapView.xaml", UriKind.Relative));
            
        }

       
        private void PlacesList_SelectionChanged(object sender, SelectionChangedEventArgs e)        
        {
            if (PlacesList.SelectedIndex == -1)
                return;
            List<Place> selected = new List<Place>();
            selected.Add(NearbyPlaces[PlacesList.SelectedIndex]);
            PlacesList.SelectedIndex = -1;

            if (PhoneApplicationService.Current.State.ContainsKey("MapPlaces"))
                PhoneApplicationService.Current.State["MapPlaces"] = selected;
            else
                PhoneApplicationService.Current.State.Add("MapPlaces", selected);
            this.NavigationService.Navigate(new Uri("/MapView.xaml", UriKind.Relative));
        }

    }
}