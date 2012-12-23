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
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;
using Phone.Controls;
using System.Windows.Media.Imaging;
using Tomers.Phone.Controls;
using RestSharp;
using System.IO;
using System.Text;


namespace First_WPA
{
    public partial class AttractionDetails : PhoneApplicationPage
    {
        WebClient clientAttraction = new WebClient();
        String attractionID;
        String attractionName;
        String latitude;
        String longtitude;
        String token = "";
        Location location = new Location();  // Global declaration
        GeoCoordinateWatcher watcher;
        PhotoChooserTask photoChooserTask; 
        private string[] data = new string[] { "Gallery", "Take a photo" };
        private int selectedIndex;
        
        public AttractionDetails()
        {
            InitializeComponent();           
            attractionName = PhoneApplicationService.Current.State["Name"].ToString();
            attractionHeader.Header = attractionName;
            attractionID=  PhoneApplicationService.Current.State["AttID"].ToString();
            
            var settings = IsolatedStorageSettings.ApplicationSettings;            
            if (settings.Contains("tokken"))
            {
                token = settings["tokken"].ToString();

            }
            clientAttraction.OpenReadCompleted += new OpenReadCompletedEventHandler(clientAttraction_OpenReadCompleted);
            clientAttraction.OpenReadAsync(new Uri("http://pm.studentlive.pl/WebService/GetAttraction?ID=" + attractionID), UriKind.Absolute);

            this.photoChooserTask = new PhotoChooserTask();
            this.photoChooserTask.Completed += new EventHandler<PhotoResult>(photoCameraCapture_Completed); 
        }
        [DataContract]
        public class Details
        {
            [DataMember]
            public string ID { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember(Name = "Coordinates")]
             public Coordinates coordinatesDet;

            [DataMember(Name = "Address")]
             public Address address;

            [DataMember(Name = "Reviews")]
            public ReviewsDetails[] reviews;

           [DataMember]
           public string Country { get; set; }

           [DataMember]
           public string Images { get; set; }

           [DataMember]
           public string Video { get; set; } 

           [DataMember]
           public string AttractionType { get; set; } 

           [DataMember]
           public double AvgRating { get; set; } 
            
        }

        [DataContract]
        public class ReviewsDetails
        {
            [DataMember]
            public string ID { get; set; }

            [DataMember]
            public string Author { get; set; }

            [DataMember]
            public string Date { get; set; }

            [DataMember]
            public double Rating { get; set; }

            [DataMember]
            public string Text { get; set; }


        }

        [DataContract]
        public class Address
        {
            [DataMember]
            public string ID { get; set; }

            [DataMember]
            public string Region { get; set; }

            [DataMember]
            public string City { get; set; }

            [DataMember]
            public string Country { get; set; }

            [DataMember]
            public string Street { get; set; }

            [DataMember]
            public int BuildingNumber { get; set; }


        }

        [DataContract]
        public class Coordinates
        {
            [DataMember]
            public int ID { get; set; }

            [DataMember]
            public double Latitude { get; set; }

            [DataMember]
            public double Longitude { get; set; }


        }
        [DataContract]
        public class Attraction
        {
            [DataMember(Name = "attraction")]
            public Details details;

        }
        private void addFavorites_Click(object sender, RoutedEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
           // bool isF = ;
            if (settings.Contains("favorites") && attractionID!=null)
            {
                var k = settings["favorites"].ToString();
                k += ", " + attractionID;
               
                
                settings["favorites"] = k.ToString();
            }
            else
            {
                settings["favorites"] = latitude + "," + longtitude;//attractionID;
            }
            MessageBox.Show(attractionName + " was added to favorites list!");
        }

        private void showMap_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/BingMaps.xaml"), UriKind.Relative));
        }
        void clientAttraction_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var serializer = new DataContractJsonSerializer(typeof(Attraction));
            Attraction ipResult = (Attraction)serializer.ReadObject(e.Result);
            List<Reviews> attractionsReviews = new List<Reviews>(); 
               
                for (int j = 0; j < ipResult.details.reviews.Length - 1; j++)
                {
                    attractionsReviews.Add(new Reviews(ipResult.details.reviews[j].Author,ipResult.details.reviews[j].Date,
                        ipResult.details.reviews[j].Rating,ipResult.details.reviews[j].Text));
                }
                
                tbDescription.Text = ipResult.details.Description.ToString();
                latitude = ipResult.details.coordinatesDet.Latitude.ToString();
                longtitude = ipResult.details.coordinatesDet.Longitude.ToString();
                //attractionID = latitude + "," + longtitude;
                //lat = lat.Replace(".",",");
                //lon = lon.Replace(".",",");
                double Lat = double.Parse(latitude);
                double Lon = double.Parse(longtitude);                
                //if (Lat < 0 )
                //    Lat = Math.Sqrt(Lat * Lat);
                //if (Lon < 0)
                //    Lon = Math.Sqrt(Lon * Lon);
                
                map1.Center = new GeoCoordinate(Lat,Lon);
                map1.ZoomLevel = 15;
                map1.ZoomBarVisibility = System.Windows.Visibility.Visible;
                lBReviews.ItemsSource = attractionsReviews;
        }
        private void addNavigate_Click(object sender, RoutedEventArgs e)
        {
            var geoCoordinate = new GeoCoordinate(Double.Parse(latitude), Double.Parse(longtitude));
            // geoCoordinate = map1.ViewportPointToLocation(point);

            OpenDirectionTo(geoCoordinate);
        }
        private void LoadWatcher()
        {
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.MovementThreshold = 20;

                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            }

            watcher.Start();
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            map1.SetView(e.Position.Location, 10);
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    //statusTextBlock.Text = "location is not functioning on this device";
                    break;

                case GeoPositionStatus.Initializing:
                    //statusTextBlock.Text = "Initializing";
                    break;

                case GeoPositionStatus.NoData:
                    //statusTextBlock.Text = "location data is not available.";
                    break;

                case GeoPositionStatus.Ready:
                    //statusTextBlock.Text = "location data is available.";
                    break;
            }
        }
        private void OpenDirectionTo(GeoCoordinate locationY)
        {
            BingMapsDirectionsTask directionTask = new BingMapsDirectionsTask();
            directionTask.End = new LabeledMapLocation(attractionName, locationY);
            directionTask.Show();
        }
        private void btnUploadPics_Click(object sender, RoutedEventArgs e)
        {
            // Display dialog
            //dialog.SelectedIndex = selectedIndex;
            //dialog.Show();
           // photoChooserTask.Show();
            NotificationBox.Show(
                "Upload a photo",
                "How you want upload photo ?",
                new NotificationBoxCommand("Load", () => { photoChooserTask.Show(); }),
                new NotificationBoxCommand("Take", () => { ShowCameraCaptureTask(); }));
        }
        //private void photoChooserTask_Completed(object sender, PhotoResult e)
        //{
        //    BitmapImage image = new BitmapImage();
        //    image.SetSource(e.ChosenPhoto);
        //    this.image1.Source = image;
        //}
        private void ShowCameraCaptureTask()
        {
            CameraCaptureTask photoCameraCapture = new CameraCaptureTask();
            photoCameraCapture = new CameraCaptureTask();
            photoCameraCapture.Completed += new EventHandler<PhotoResult>(photoCameraCapture_Completed);
            photoCameraCapture.Show();
        }

        //void photoCameraCapture_Completed(object sender, PhotoResult e)
        //{
        //    if (e.TaskResult == TaskResult.OK)
        //    {
        //        BitmapImage image = new BitmapImage();
        //        image.SetSource(e.ChosenPhoto);
        //        this.image1.Source = image;
        //        //MessageBox.Show("Saving photo");
        //    }
        //}
        void photoCameraCapture_Completed(object sender, PhotoResult e)
        {
            
            //checking if everything went fine when capturing a photo
            if (e.TaskResult != TaskResult.OK)
                return;


            BitmapImage image = new BitmapImage();
            image.SetSource(e.ChosenPhoto);
            this.image1.Source = image;


            //preparing RestRequest by adding server url, parameteres and files...
            //RestRequest request = new RestRequest("http://localhost:23790/WebService/AddPhotoToAttraction", Method.POST);
            //request.AddParameter("tokken",token );
            //request.AddParameter("attrId", attractionID);
            //request.AddFile("image", ReadToEnd(e.ChosenPhoto), "photo.jpg", "image/jpeg");

            
            var client = new WebClient();
            var uri = new Uri("http://localhost:23790/WebService/AddPhotoToAttraction?tokken="+token+"&attrId="+attractionID);
            client.OpenWriteCompleted += (sender1, e1) =>
            {
                var buffer = (byte[])e1.UserState;
                e1.Result.Write(buffer, 0, buffer.Length);
                e1.Result.Close();
                MessageBox.Show("Image has been succesfully uploaded !");
            };
            client.OpenWriteAsync(uri, "POST", ReadToEnd(e.ChosenPhoto));


            //calling server with restClient
            //RestClient restClient = new RestClient();
            //restClient.ExecuteAsync(request, (response) =>
            //{
            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        //upload successfull
            //        MessageBox.Show("Upload completed succesfully...\n" + response.Content);
            //    }
            //    else
            //    {
            //        //error ocured during upload
            //        MessageBox.Show(response.StatusCode + "\n" + response.StatusDescription);
            //    }
            //});
        }
        

        //method for converting stream to byte[]
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/PanoramicMain.xaml?"), UriKind.Relative));
            e.Cancel = true;
        }

     }
}