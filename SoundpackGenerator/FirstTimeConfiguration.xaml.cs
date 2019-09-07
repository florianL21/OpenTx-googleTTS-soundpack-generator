using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            if (File.Exists(TextBox_path.Text))
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", TextBox_path.Text);
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
                }
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("The path does not point to an existing file");
            }
        }
    }
}
