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
using Microsoft.Phone.Tasks;

namespace First_WPA
{
    public partial class Invite : PhoneApplicationPage
    {
        public Invite()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            Uri http = new Uri("http://en.wikipedia.org/w/api.php");
            webBrowserG.Navigate(http);
            
            //webBrowserTask.URL = "http://google.navigation:q=1.352566007,103.78921587&mode=w";
            //webBrowserTask.Show(); 
        }
    }
}