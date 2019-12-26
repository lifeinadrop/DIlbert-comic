using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DIlbert
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var assetUrl = await GetHtml(new Uri("https://dilbert.com/strip/2019-12-23"));

            var bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(assetUrl, UriKind.Absolute);
            bi3.EndInit();

            comic.Source = bi3;
            
        }

        async Task<string> GetHtml(Uri siteUrl)
        {
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(siteUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                int position = responseBody.IndexOf("img-comic\"");
                int startPosition = responseBody.IndexOf("src=\"", position) + 5;
                int endPosition = responseBody.IndexOf("\"", startPosition);

                string assetUrl = "https:" + responseBody.Substring(startPosition, endPosition - startPosition);

                return assetUrl;

                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
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
