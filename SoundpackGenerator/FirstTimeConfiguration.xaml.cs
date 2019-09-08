using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace SoundpackGenerator
{
    /// <summary>
    /// Interaction logic for FirstTimeConfiguration.xaml
    /// </summary>
    public partial class FirstTimeConfiguration : Window
    {
        public FirstTimeConfiguration()
        {
            InitializeComponent();
            TextBox_path.Text = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Button_Appy_Click(object sender, RoutedEventArgs e)
        {
            string pathToJsonFile = TextBox_path.Text;
            if (File.Exists(pathToJsonFile))
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToJsonFile);
                try
                {
                    TextToSpeechClient client = TextToSpeechClient.Create();
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("The file you specified does not seem like it is valid. Full error message: " + ex.Message);
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", null);
                    return;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Unknown exception: " + ex.Message);
                    return;
                }
                MessageBoxResult result = MessageBox.Show("The application will ask for the location of the json file every time. If you don't want to be asked again the value can be saved permanently as an environment variable. Do you want to do this now?", "copy to clipboard?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.ProcessStartInfo myProcessInfo = new System.Diagnostics.ProcessStartInfo();
                    myProcessInfo.FileName = "cmd.exe";
                    myProcessInfo.Arguments = "/C setx GOOGLE_APPLICATION_CREDENTIALS " + pathToJsonFile;
                    myProcessInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    System.Diagnostics.Process.Start(myProcessInfo);
                }
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("The path does not point to an existing file");
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenBrowser(e.Uri.ToString());
        }

        //fix for dotnet core taken from: https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
