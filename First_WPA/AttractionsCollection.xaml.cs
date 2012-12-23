using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Net.NetworkInformation;

namespace First_WPA
{
    public partial class AttractionsCollection : PhoneApplicationPage
    {

        WebClient clientPlaces = new WebClient();
        WebClient clientCities = new WebClient();
        WebClient clientCountires = new WebClient();

        public AttractionsCollection()
        {
            InitializeComponent();
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                ConnectionSettingsTask cst = new ConnectionSettingsTask();
                cst.ConnectionSettingsType = ConnectionSettingsType.WiFi;
                cst.Show();
            }
            String cityName = PhoneApplicationService.Current.State["City"].ToString();
            clientPlaces.OpenReadCompleted += new OpenReadCompletedEventHandler(clientPlaces_OpenReadCompleted);
            clientPlaces.OpenReadAsync(new Uri("http://pm.studentlive.pl/WebService/GetAttractionsByPlace?name=" + cityName + "&start=0&count=12"), UriKind.Absolute);

            clientCountires.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            clientCountires.OpenReadAsync(new Uri("http://pm.studentlive.pl/WebService/GetCountries?start=0&count=12"), UriKind.Absolute);  
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled) MessageBox.Show(e.Result);

        }



        void ws_DefineCompleted(object sender, ServiceReference1.DefineCompletedEventArgs e)
        {
            for (int i = 0; i < e.Result.Definitions.Length; i++) MessageBox.Show(e.Result.Definitions[i].WordDefinition);
        }

        [DataContract]
        public class Country
        {
            [DataMember]
            public string ID { get; set; }

            [DataMember]
            public string Name { get; set; }
        }

        [DataContract]
        public class Countries
        {
            [DataMember(Name = "countries")]
            public Country[] addresses;

        }
        [DataContract]
        public class Cities
        {
            [DataMember(Name = "places")]
            public Country[] addresses;

        }
        [DataContract]
        public class Attractions
        {
            [DataMember(Name = "attractions")]
            public Country[] addresses;

        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result.);
            var serializer = new DataContractJsonSerializer(typeof(Countries));
            Countries ipResult = (Countries)serializer.ReadObject(e.Result);
            List<AtrractionsList> attractionsList = new List<AtrractionsList>();

            for (int i = 0; i <= ipResult.addresses.Length - 1; i++)
            {

                Random k = new Random();
                Double value = 0;
                String DefaultTitle = "Title";
                String DefaultDescription = "Description";
                String RandomType = "";


                value = k.NextDouble();
                DefaultTitle = ipResult.addresses[i].Name.ToString();
                DefaultDescription = ipResult.addresses[i].ID.ToString();
                RandomType = value < 0.66 ? value < 0.33 ? "Place" : "Monument" : "Church";
                attractionsList.Add(new AtrractionsList(DefaultTitle, DefaultDescription, RandomType));

            }
           
            listBoxCountries.ItemsSource = attractionsList;
        }
        void clientCities_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result.);
            var serializer = new DataContractJsonSerializer(typeof(Cities));
            Cities ipResult = (Cities)serializer.ReadObject(e.Result);
            List<AtrractionsList> citiesList = new List<AtrractionsList>();

            for (int i = 0; i <= ipResult.addresses.Length - 1; i++)
            {

                Random k = new Random();
                Double value = 0;
                String DefaultTitle = "Title";
                String DefaultDescription = "Description";
                String RandomType = "";


                value = k.NextDouble();
                DefaultTitle = ipResult.addresses[i].Name.ToString();
                DefaultDescription = ipResult.addresses[i].ID.ToString();
                RandomType = value < 0.66 ? value < 0.33 ? "Place" : "Monument" : "Church";
                citiesList.Add(new AtrractionsList(DefaultTitle, DefaultDescription, RandomType));
            }
            listBoxCities.ItemsSource = citiesList;
        }
        void clientPlaces_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result.);
            var serializer = new DataContractJsonSerializer(typeof(Attractions));
            Attractions ipResult = (Attractions)serializer.ReadObject(e.Result);
            List<AtrractionsList> attractsByPlace = new List<AtrractionsList>();

            for (int i = 0; i <= ipResult.addresses.Length - 1; i++)
            {

                Random k = new Random();
                Double value = 0;
                String DefaultTitle = "Title";
                String DefaultDescription = "Description";
                String RandomType = "";


                value = k.NextDouble();
                DefaultTitle = ipResult.addresses[i].Name.ToString();
                DefaultDescription = ipResult.addresses[i].ID.ToString();
                RandomType = value < 0.66 ? value < 0.33 ? "Place" : "Monument" : "Church";
                attractsByPlace.Add(new AtrractionsList(DefaultTitle, DefaultDescription, RandomType));
            }
            listBoxAttractions.ItemsSource = attractsByPlace;

        }

      
        private void listBoxCountries_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Get the data object that represents the current selected item
            AtrractionsList data = (sender as ListBox).SelectedItem as AtrractionsList;

            //Get the selected ListBoxItem container instance    
            ListBoxItem selectedItem = this.listBoxCountries.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;
        }
        private void listBoxCities_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Get the data object that represents the current selected item
            AtrractionsList data = (sender as ListBox).SelectedItem as AtrractionsList;

            //Get the selected ListBoxItem container instance    
            ListBoxItem selectedItem = this.listBoxCities.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;
        }

        private void btnSearchAttr_Click(object sender, RoutedEventArgs e)
        {

        }



        private void btnSearchCities_Click(object sender, RoutedEventArgs e)
        {
            clientCities.OpenReadCompleted += new OpenReadCompletedEventHandler(clientCities_OpenReadCompleted);
            clientCities.OpenReadAsync(new Uri("http://pm.studentlive.pl/WebService/GetPlaces?country=" + searchTBox.Text + "&start=0&count=12"), UriKind.Absolute);
        }

        private void btnSearchCountires_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listBoxPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBoxAttractions_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Get the data object that represents the current selected item
            AtrractionsList data = (sender as ListBox).SelectedItem as AtrractionsList;

            //Get the selected ListBoxItem container instance    
            ListBoxItem selectedItem = this.listBoxAttractions.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;
            PhoneApplicationService.Current.State["Name"] = data.Title;
            PhoneApplicationService.Current.State["AttID"] = data.Description;
            NavigationService.Navigate(new Uri(string.Format("/AttractionDetails.xaml"), UriKind.Relative));

        }
    }
}