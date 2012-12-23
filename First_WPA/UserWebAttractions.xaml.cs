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
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO.IsolatedStorage;

namespace First_WPA
{
    public partial class UserWebAttractions : PhoneApplicationPage
    {
        WebClient clientLists = new WebClient();
        String token = "";
        
        public UserWebAttractions()
        {
            InitializeComponent();
            //if (!DeviceNetworkInformation.IsNetworkAvailable)
            //{
            //    ConnectionSettingsTask cst = new ConnectionSettingsTask();
            //    cst.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            //    cst.Show();
            //}
            var settings = IsolatedStorageSettings.ApplicationSettings;
            
            if (settings.Contains("tokken"))
            {
                token = settings["tokken"].ToString();
            }
            clientLists.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            clientLists.OpenReadAsync(new Uri("http://localhost:23790/WebService/GetUserAttractionLists?tokken="+token), UriKind.Absolute);
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled) MessageBox.Show(e.Result);

        }
        
       


        void ws_DefineCompleted(object sender, ServiceReference1.DefineCompletedEventArgs e)
        {
            for (int i = 0; i < e.Result.Definitions.Length; i++) MessageBox.Show(e.Result.Definitions[i].WordDefinition);
        }
        private void listBoxPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        [DataContract]
        public class Lists
        {
            [DataMember]
            public string ID { get; set; }

            [DataMember]
            public string UserId { get; set; }

            [DataMember]
            public string Name { get; set; }
        }
        [DataContract]
        public class UserList
        {
            [DataMember(Name = "attractionLists")]
            public Lists[] lists;

        }
        private void listBoxPlaces_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Get the data object that represents the current selected item
            AtrractionsList data = (sender as ListBox).SelectedItem as AtrractionsList;

            //Get the selected ListBoxItem container instance    
            ListBoxItem selectedItem = this.lbUserAttractions.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;
            PhoneApplicationService.Current.State["listId"] = data.Description;
            

            //PhoneApplicationService.Current.State["Desc"] = "Perfect attraction";
            NavigationService.Navigate(new Uri(string.Format("/UserAttraction.xaml"), UriKind.Relative));
        }
      
        


        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result.);
            var serializer = new DataContractJsonSerializer(typeof(UserList));
            UserList ipResult = (UserList)serializer.ReadObject(e.Result);
            List<AtrractionsList> userLists = new List<AtrractionsList>();

            for (int i = 0; i <= ipResult.lists.Length - 1; i++)
            {

                Random k = new Random();
                Double value = 0;
                String DefaultTitle = "Title";
                String DefaultDescription = "Description";
                String RandomType = "";


                value = k.NextDouble();
                DefaultTitle = ipResult.lists[i].Name.ToString();
                DefaultDescription = ipResult.lists[i].ID.ToString();
                RandomType = "Place";
                userLists.Add(new AtrractionsList(DefaultTitle, DefaultDescription, RandomType));

            }
            lbUserAttractions.ItemsSource = userLists;
            
        }
       
        
    }
}