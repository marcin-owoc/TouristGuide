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
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Phone.Shell;

namespace First_WPA
{
    public partial class UserAttraction : PhoneApplicationPage
    {
        WebClient clientLists = new WebClient();
        public UserAttraction()
        {
            InitializeComponent();
            //if (!DeviceNetworkInformation.IsNetworkAvailable)
            //{
            //    ConnectionSettingsTask cst = new ConnectionSettingsTask();
            //    cst.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            //    cst.Show();
            //}
            var settings = IsolatedStorageSettings.ApplicationSettings;
            String token ="", listId="1";
            if (settings.Contains("tokken") )
            {
                token = settings["tokken"].ToString();
                
            }
            listId = PhoneApplicationService.Current.State["listId"].ToString();
            clientLists.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            clientLists.OpenReadAsync(new Uri("http://localhost:23790/WebService/GetAttractionsByUserList?tokken=" + token+"&listId="+listId), UriKind.Absolute);
        }
        private void listBoxPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled) MessageBox.Show(e.Result);

        }
        private void listBoxPlaces_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Get the data object that represents the current selected item
            AtrractionsList data = (sender as ListBox).SelectedItem as AtrractionsList;

            //Get the selected ListBoxItem container instance    
            ListBoxItem selectedItem = this.lbUserAttractions.ItemContainerGenerator.ContainerFromItem(data) as ListBoxItem;
            //PhoneApplicationService.Current.State["Country"] = data.Title;
            PhoneApplicationService.Current.State["Name"] = data.Title;
            PhoneApplicationService.Current.State["AttID"] = data.Description;
            NavigationService.Navigate(new Uri(string.Format("/AttractionDetails.xaml"), UriKind.Relative));
        }



        void ws_DefineCompleted(object sender, ServiceReference1.DefineCompletedEventArgs e)
        {
            for (int i = 0; i < e.Result.Definitions.Length; i++) MessageBox.Show(e.Result.Definitions[i].WordDefinition);
        }

        [DataContract]
        public class Lists
        {
            [DataMember]
            public string ID { get; set; }

            [DataMember]
            public string Name { get; set; }
        }
        [DataContract]
        public class UserList
        {
            [DataMember(Name = "attractions")]
            public Lists[] lists;

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