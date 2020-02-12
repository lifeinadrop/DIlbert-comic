using Microsoft.Win32;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace daily_dilbert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SaveComic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string todaysDate = DateTime.Today.ToString("yyyy-MM-dd");

                // Check to see if the file already exists. If it does, add a random number to the end. This isn't necessary, this was more just a learning exercise for me.
                if (!System.IO.File.Exists(Path(todaysDate + ".png")))
                {
                    
                    String filePath = Path(todaysDate + ".png");

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)comic.Source));
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        encoder.Save(stream);

                    MessageBox.Show("Strip saved to Pictures folder.");
                }
                else
                {
                    Random random = new Random();
                    int randomNum = random.Next(1000, 10000);

                    String filePath = Path(todaysDate + "_" + randomNum + ".png");

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)comic.Source));
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        encoder.Save(stream);

                    MessageBox.Show("Strip saved to Pictures folder.");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Picture did not save.");
                Console.Write(ex.Message);
            }
            
            
        }

        /// <summary>
        /// Generate the HTTP request on application load and set the Image box source to the returned bitmap image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string assetUrl = await GetHtml(new Uri(FormatUrl()));

            var bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(assetUrl);
            bi3.EndInit();

            comic.Source = bi3;
        }

        /// <summary>
        /// Method which sends a GET request to a url and returns the html as a string.
        /// </summary>
        /// <param name="siteUrl">The url to send a GET request to.</param>
        /// <returns>Returns the url of the assest found in the scraper.</returns>
        async Task<string> GetHtml(Uri siteUrl)
        {
            HttpClient client = new HttpClient();
            try
            {
                // Send a GET request to the Dilbert website and return the website as a string.
                string responseBody = await client.GetStringAsync(siteUrl);

                // Scraper to find the img element, and grabs the comic's image source url.
                int position = responseBody.IndexOf("img-comic\"");
                int startPosition = responseBody.IndexOf("src=\"", position) + 5;
                int endPosition = responseBody.IndexOf("\"", startPosition);

                string assetUrl = "https:" + responseBody.Substring(startPosition, endPosition - startPosition);

                return assetUrl;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                MessageBox.Show("You need an internet connection to use this application.");
                return null;
            }
        }

        /// <summary>
        /// Method that handles formatting the URL with the current date.
        /// </summary>
        /// <returns>Returns a formatted string to use as a URL.</returns>

        private string FormatUrl()
        {
            string todaysDate = DateTime.Today.ToString("yyyy-MM-dd");
            string stripUrl = "https://dilbert.com/strip/" + todaysDate;

            return stripUrl;
        }

        /// <summary>
        /// Get the path to the users Pictures directory.
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Returns formatted path to the correct directory.</returns>
        public static string Path(string target)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Pictures\";
            return basePath + target;
        }

        // Minimize to system tray when application is minimized.
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized) this.Hide();

            base.OnStateChanged(e);
        }

    }
}
