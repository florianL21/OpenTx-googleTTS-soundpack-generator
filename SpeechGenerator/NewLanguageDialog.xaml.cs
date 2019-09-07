using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
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

namespace SpeechGenerator
{
    /// <summary>
    /// Interaction logic for NewLanguageDialog.xaml
    /// </summary>
    public partial class NewLanguageDialog : Window
    {
        public NewLanguageDialog()
        {
            InitializeComponent();
        }

        public string returnLanguageCode = null;

        private static bool isCountryCodeValid(string desiredLanguageCode )
        {
            TextToSpeechClient client = TextToSpeechClient.Create();

            // Performs the list voices request
            var response = client.ListVoices(new ListVoicesRequest
            {
                LanguageCode = desiredLanguageCode 
            });
            if (response.Voices.Count > 0)
            {
                return true;
            }
            return false;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            returnLanguageCode = TextBox_LanguageCode.Text;
            if(isCountryCodeValid(returnLanguageCode))
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("This language code is either invalid or not supported by the Google Cloud TTS service.");
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
