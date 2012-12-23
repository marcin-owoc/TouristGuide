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
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone;
using System.Text;

namespace First_WPA
{
    public partial class CapturingPhoto : PhoneApplicationPage
    {
        BitmapImage photo;
        private string boundary;  
        public CapturingPhoto()
        {
            InitializeComponent();
           // this.boundary = boundary; 
            //Loaded += new RoutedEventHandler(POST_Loaded);
        }
        private void ShowCameraCaptureTask()
        {
            CameraCaptureTask photoCameraCapture = new CameraCaptureTask();
            photoCameraCapture = new CameraCaptureTask();
            photoCameraCapture.Completed += new EventHandler<PhotoResult>(photoCameraCapture_Completed);
            photoCameraCapture.Show();
        }

        void photoCameraCapture_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                SaveToIsolatedStorage(e.ChosenPhoto, "image1.jpg");
                MessageBox.Show("Saving photo");
            }
        }

        private void SaveToIsolatedStorage(Stream imageStream, string fileName)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(fileName))
                {
                    myIsolatedStorage.DeleteFile(fileName);
                }

                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(fileName);
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(imageStream);
                photo = bitmap;

                WriteableBitmap wb = new WriteableBitmap(bitmap);
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                fileStream.Close();
            }
        }
        private void ReadFromIsolatedStorage(string fileName)
        {
            WriteableBitmap bitmap = new WriteableBitmap(200, 200);
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Decode the JPEG stream.
                    bitmap = PictureDecoder.DecodeJpeg(fileStream);
                }
            }
            this.img.Source = bitmap;
        }       

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            ShowCameraCaptureTask();
        }

        private void btnLoad_Click_1(object sender, RoutedEventArgs e)
        {
           // this.ReadFromIsolatedStorage("image1.jpg");
            POST_Loaded();
        }
        void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            // End the stream request operation
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);
            var settings = IsolatedStorageSettings.ApplicationSettings;
            String token ="";
            if (settings.Contains("tokken"))
            {
                token = settings["tokken"].ToString();
            }
            // Create the post data
            string postData = "tokken=" + token + "&attrId=30";

           MemoryStream ms = new MemoryStream();
        WriteableBitmap btmMap = new WriteableBitmap(photo.PixelWidth,photo.PixelHeight);

        // write an image into the stream
        Extensions.SaveJpeg(btmMap, ms,
            photo.PixelWidth, photo.PixelHeight, 0, 100);
    
            //this.WriteEntry(postStream, "image",ms.ToArray());
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            // Start the web request
            myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
        }
        void GetResponsetStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult);
            using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
            {
                string result = httpWebStreamReader.ReadToEnd();
                //For debug: show results
               // Debug.WriteLine(result);
            }
        }
        void POST_Loaded()
        {
            System.Uri myUri = new System.Uri("http://localhost:23790/WebService/AddPhotoToAttraction");
            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(myUri);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), myRequest);
        }
        private void WriteEntry(StreamWriter writer, string key, object value)
        {
            if (value != null)
            {
                writer.Write("--");
                writer.WriteLine(boundary);
                if (value is byte[])
                {
                    byte[] ba = value as byte[];

                    writer.WriteLine(@"Content-Disposition: form-data; image=""{0}""; filename=""{1}""", key, "sentPhoto.jpg");
                    writer.WriteLine(@"Content-Type: application/octet-stream");
                    writer.WriteLine(@"Content-Length: " + ba.Length);
                    writer.WriteLine();
                    writer.Flush();
                    Stream output = writer.BaseStream;

                    output.Write(ba, 0, ba.Length);
                    output.Flush();
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""", key);
                    writer.WriteLine();
                    writer.WriteLine(value.ToString());
                }
            }
        }

        
    }
    
}