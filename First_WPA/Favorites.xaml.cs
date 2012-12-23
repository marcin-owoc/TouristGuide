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
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;




namespace First_WPA
{
    
    public partial class Favorites : PhoneApplicationPage
    {

        internal geocodeservice.GeocodeResult[] geocodeResults;
        Location location = new Location();  // Global declaration
        GeoCoordinateWatcher watcher;
        Double latitude,longtitude;

        public Favorites()
        {
            InitializeComponent();
            LoadWatcher();
            map1.LogoVisibility = Visibility.Collapsed;
            map1.CopyrightVisibility = Visibility.Collapsed;
            map1.Mode = new AerialMode();
            map1.ZoomLevel = 5;
            map1.ZoomBarVisibility = System.Windows.Visibility.Visible;

            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("favorites"))
            {
                var location = settings["favorites"].ToString();
                latitude = double.Parse(location.Substring(0, location.IndexOf(",") - 1));
                longtitude =  double.Parse(location.Substring(location.IndexOf(",") + 1));
                var locationsList = new LocationCollection();
                //locationsList.Add(new GeoCoordinate(56.5845698,40.5489514));
                //locationsList.Add(new GeoCoordinate(60.4885213, 80.785426));
                locationsList.Add(new GeoCoordinate(latitude,longtitude));
                locationsList.Add(new GeoCoordinate(latitude+0.85412, longtitude+0.12564));
                MapPolyline polyline = new MapPolyline();
                polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black); 
                polyline.StrokeThickness = 5;
                polyline.Opacity = 0.8;
                polyline.Locations = locationsList;

                //Pushpin pin = new Pushpin();

                //MessageBox.Show(location.Substring(0, location.IndexOf(",") - 1) + " \n  " + location.Substring(location.IndexOf(",") + 1));
                ////---set the location for the pushpin---
                //pin.Location = new GeoCoordinate(double.Parse(location.Substring(0, location.IndexOf(",") - 1)), double.Parse(location.Substring(location.IndexOf(",") + 1)));
                //pin.Name = "tmp";

                ////---add the pushpin to the map---

                //pin.Content = new Ellipse()
                //{
                //    //Fill = image,

                //    StrokeThickness = 10,
                //    Height = 100,
                //    Width = 100
                //};
                //pin.MouseLeftButtonUp += new MouseButtonEventHandler(Pushpin_MouseLeftButtonUp);

                //---add the pushpin to the map---
                map1.Children.Add(polyline);


                MessageBox.Show(location.ToString());
            }

            // settings["myemail"] = "support@windowsphonnegeek.com";
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

        //private void map1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    var position = e.GetPosition(map1);
        //    var geoCoordinate = new GeoCoordinate();
        //    geoCoordinate = map1.ViewportPointToLocation(position);

        //    OpenDirectionTo(geoCoordinate);
        //}

        //private void OpenDirectionTo(GeoCoordinate locationY)
        //{
        //    BingMapsDirectionsTask directionTask = new BingMapsDirectionsTask();
        //    directionTask.End = new LabeledMapLocation("Your tapped location", locationY);
        //    directionTask.Show();
        //}
        //private AsyncCallback void DrawPath()
        //{
        //    //dataContext.LoadingVisibility = Visibility.Visible;

        //    var url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
        //                dataContext.StartCoordinates +
        //                "&wp.1=" +
        //                dataContext.EndCoordinates +
        //                "&optmz=distance&rpo=Points&key=" + "Ak-j1fNexs-uNWG_YoP2WZlthezPoUWsRvSexDLTGfjQ1XqKgnfR1nqeC2YbZZSn";

        //    Uri geocodeRequest = new Uri(url);
        //    BingMapHelper.Response r = await GetResponse(geocodeRequest);

        //    if (r.StatusCode != 404)
        //    {
        //        map1.Children.Clear();
        //        map1.ShapeLayers.Clear();

        //        Geolocator geolocator = new Geolocator();
                
        //        MapPolyline routeLine = new MapPolyline();
        //        routeLine.Locations = new LocationCollection();
        //        routeLine.Color = Windows.UI.Colors.DarkOrchid;
        //        routeLine.Width = 5.0;

        //        int bound = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
        //            RoutePath.Line.Coordinates.GetUpperBound(0);

        //        sourceLatitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
        //            RoutePath.Line.Coordinates[0][0];
        //        sourceLongitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
        //            RoutePath.Line.Coordinates[0][1];

        //        destinationLatitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
        //            RoutePath.Line.Coordinates[bound][0];
        //        destinationLongitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
        //            RoutePath.Line.Coordinates[bound][1];

        //        var sourcePin = new Pushpin();
        //        var sourceLocation = new Bing.Maps.Location(sourceLatitude, sourceLongitude);
        //        MapLayer.SetPosition(sourcePin, sourceLocation);
        //        map1.Children.Add(sourcePin);

                
        //        var destinationLocation = new Bing.Maps.Location(destinationLatitude, destinationLongitude);
        //        MapLayer.SetPosition(pin, destinationLocation);
        //        map1.Children.Add(pin);

        //        map1.SetView(sourceLocation, map1.ZoomLevel);

        //        for (int i = 0; i < bound; i++)
        //        {
        //            routeLine.Locations.Add(new Location
        //            {
        //                Latitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
        //                RoutePath.Line.Coordinates[i][0],
        //                Longitude = ((BingMapHelper.Route)(r.ResourceSets[0].Resources[0])).
        //                RoutePath.Line.Coordinates[i][1]
        //            });
        //        }

        //        MapShapeLayer shapeLayer = new MapShapeLayer();
        //        shapeLayer.Shapes.Add(routeLine);
        //        bingMap.ShapeLayers.Add(shapeLayer);
        //    }
        //    else
        //    {
        //        MessageDialog dialog = new MessageDialog(r.errorDetails[0], "Route Path");
        //        await dialog.ShowAsync();
        //    }

        //    dataContext.LoadingVisibility = Visibility.Collapsed;
        //}

        //private async Task<BingMapHelper.Response> GetResponse(Uri uri)
        //{
        //    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        //    var response = await client.GetAsync(uri);
        //    using (var stream = await response.Content.ReadAsStreamAsync())
        //    {
        //        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(BingMapHelper.Response));
        //        return ser.ReadObject(stream) as BingMapHelper.Response;
        //    }
        //}

        private void btnShowRoute_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    
}