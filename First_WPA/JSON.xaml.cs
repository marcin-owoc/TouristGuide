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
using System.IO;
using System.Text;


namespace First_WPA
{
    public partial class JSON : PhoneApplicationPage
    {
        string vgJson = "{vg:["
                + "{id:\"1234\",quantity:\"100\",tariff:\"1.23\",currency:\"PLN\",shortcode:\"501106769\",keyword:\"TQM\",additionalinfo:\"\",billingtype:\"mo\"},"
                + "{id:\"1235\",quantity:\"300\",tariff:\"2.46\",currency:\"PLN\",shortcode:\"501106769\",keyword:\"TQM\",additionalinfo:\"\",billingtype:\"mo\"},"
                + "{id:\"1236\",quantity:\"1000\",tariff:\"4.92\",currency:\"PLN\",shortcode:\"501106769\",keyword:\"TQM\",additionalinfo:\"\",billingtype:\"mo\"},"
                + "{id:\"1237\",quantity:\"2000\",tariff:\"12.30\",currency:\"PLN\",shortcode:\"501106769\",keyword:\"TQM\",additionalinfo:\"\",billingtype:\"mo\"},"
                + "{id:\"1238\",quantity:\"10001\",tariff:\"123.00\",currency:\"PLN\",shortcode:\"501106769\",keyword:\"TQM\",additionalinfo:\"\",billingtype:\"mo\"}"
                + "]}";
        public JSON()
        {
            InitializeComponent();
        }
        [DataContract]
        public class detailedAddress
        {
            //[DataMember]
            //public string adminCode1 { get; set; }

            [DataMember]
            public int id { get; set; }

            [DataMember]
            public int quantity { get; set; }

            [DataMember]
            public float tariff { get; set; }

            [DataMember]
            public string currency { get; set; }

            [DataMember]
            public string keyword { get; set; }

            [DataMember]
            public string billingtype { get; set; }

            [DataMember]
            public string additionalinfo { get; set; }

       }

        [DataContract]
        public class Postal_Result
        {
            [DataMember(Name = "vg")]
            public detailedAddress[] addresses;
        }
       
 

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //object nawna = (object)vgJson; 
            //var serializer = new DataContractJsonSerializer(typeof(Postal_Result));
            //Postal_Result ipResult = (Postal_Result)serializer.ReadObject(e.Result);
            //MessageBox.Show(e.ToString());
            //MessageBox.Show(ipResult.addresses.ToString());
            var ds = new DataContractJsonSerializer(typeof(Postal_Result));
            var msnew = new MemoryStream(Encoding.UTF8.GetBytes(vgJson));
            Postal_Result[] items = (Postal_Result[])ds.ReadObject(msnew);
            for (int i=0;i< items.Length;i++)
            {
                MessageBox.Show(items[i].addresses[i].currency.ToString());// binding name in to combobox
                MessageBox.Show(items[i].addresses[i].quantity.ToString());
            }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(new Uri("http://ws.geonames.org/postalCodeLookupJSON?postalcode=6600&country=AT"), UriKind.Absolute);
        }
    }
}