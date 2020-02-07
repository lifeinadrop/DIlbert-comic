﻿using System;
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

        private void SetAsWallpaper_Click(object sender, RoutedEventArgs e)
        {
            // Set as wallpaper. Work in progress.

            saveFileDialog.ShowDialog();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get today's date and format it the way the url has the date formatted.
            string todaysDate = DateTime.Today.ToString("yyyy-MM-dd");
            string stripUrl = "https://dilbert.com/strip/" + todaysDate;
            string assetUrl = await GetHtml(new Uri(stripUrl));

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
                return null;
            }
        }
    }
}
