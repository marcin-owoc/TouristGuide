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
using System.Device.Location;
using First_WPA.routeservice;
using Microsoft.Phone.Controls.Maps;
using System.Collections.ObjectModel;

namespace First_WPA
{
    public partial class RouteDrawing : PhoneApplicationPage
    {
        public RouteDrawing()
        {
            InitializeComponent();

            List<GeoCoordinate> locations = new List<GeoCoordinate>();

            RouteServiceClient routeService = new RouteServiceClient("BasicHttpBinding_IRouteService");

            routeService.CalculateRouteCompleted += (sender, e) =>
            {
                var points = e.Result.Result.RoutePath.Points;
                var coordinates = points.Select(x => new GeoCoordinate(x.Latitude, x.Longitude));

                var routeColor = Colors.Blue;
                var routeBrush = new SolidColorBrush(routeColor);

                var routeLine = new MapPolyline()
                {
                    Locations = new LocationCollection(),
                    Stroke = routeBrush,
                    Opacity = 0.65,
                    StrokeThickness = 5.0,
                };

                foreach (var location in points)
                {
                    routeLine.Locations.Add(new GeoCoordinate(location.Latitude, location.Longitude));
                }

                RouteLayer.Children.Add(routeLine);
            };

            RouteBingMap.SetView(LocationRect.CreateLocationRect(locations));

            routeService.CalculateRouteAsync(new RouteRequest()
            {
                Credentials = new Credentials()
                {
                    ApplicationId = "Ak-j1fNexs-uNWG_YoP2WZlthezPoUWsRvSexDLTGfjQ1XqKgnfR1nqeC2YbZZSn"
                },
                Options = new RouteOptions()
                {
                    RoutePathType = RoutePathType.Points
                },
                Waypoints = new ObservableCollection<Waypoint>(
                    locations.Select(x => new Waypoint()
                    {
                        Location = x.Location
                    }))
            });
        }
    }
}