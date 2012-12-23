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
using System.IO;
using System.Text;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using RestSharp;

namespace First_WPA
{
    public partial class MainPage : PhoneApplicationPage
    {
        //String token;
        // Constructor
        String user, pass;
        Boolean credentialCorrect = false;
        public MainPage()
        {
            InitializeComponent();        
            
        }

               private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Button b = sender as Button;

            int col = Grid.GetColumn(b);
            int row = Grid.GetRow(b);

            if (col == row)
            {
                Grid.SetColumn(b, ++col % 2);
            }
            else
            {
                Grid.SetRow(b, ++row % 2);
            }
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri(string.Format("/PanoramicMain.xaml"), UriKind.Relative));
            user = tbLogin.Text.ToString();
            pass= passwordBox1.Password.ToString();
            char[] characters = { '"', '/' };
            var settings = IsolatedStorageSettings.ApplicationSettings;
            RestRequest request = new RestRequest("http://localhost:23790/WebService/MobileLogOn", Method.POST);
            request.AddParameter("user", user);
            request.AddParameter("pass", pass);
            //calling server with restClient
            RestClient restClient = new RestClient();
            restClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //upload successfull
                    if (!response.Content.Contains("error") && response != null)
                    {
                        settings["tokken"] = response.Content.Trim(characters);
                       // MessageBox.Show("Upload completed succesfully...\n" + response.Content);
                        NavigationService.Navigate(new Uri(string.Format("/PanoramicMain.xaml"), UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Credentials are not correct!");
                    }
                }
                else
                {
                    //error ocured during upload
                    MessageBox.Show(response.StatusCode + "\n" + response.StatusDescription);
                }
            });         
            
        }
        
       private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/RouteDrawing.xaml"), UriKind.Relative));
            //MessageBox.Show("Nie mam czasu!! " );
        }

       

        

       
    }
}