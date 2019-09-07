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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using NAudio.Wave;
// using NAudio.Wave;

namespace SoundpackGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public class VoiceEntry
        {
            public string filename { get; set; }
            public string comment { get; set; }
            public string TTSText { get; set; }
        }

        public class LanguagePack
        {
            public string LanguageCode { get; set; }
            public List<VoiceEntry> SystemVoices { get; set; }
            public List<VoiceEntry> UserVoices { get; set; }

            public LanguagePack()
            {
                SystemVoices = new List<VoiceEntry>();
                UserVoices = new List<VoiceEntry>();
            }
        }

        public class SoundpackConfiguration
        {
            public string VoiceName { get; set; }
            public string outputPath { get; set; }
            public bool GenerateSystemSounds { get; set; }
            public bool GenerateUserSounds { get; set; }
            public bool OverrideSounds { get; set; }
            public List<LanguagePack> LanguagePacks { get; set; }

            public SoundpackConfiguration()
            {
                LanguagePacks = new List<LanguagePack>();
            }
        }

        public SoundpackConfiguration currentFile {get; set;}
        public string FileLocation = "";
        public TextToSpeechClient client = null;
        private AudioFileReader SoundOutputFile = null;
        private bool soundIsPlaying = false;

        public MainWindow()
        {
            //Data initialization:
            DataContext = this;
            currentFile = new SoundpackConfiguration();
            createEmptyLanguagePack();
            string pathToGoogleAuthFile = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (pathToGoogleAuthFile == null || pathToGoogleAuthFile =="")
            {
                FirstTimeConfiguration envVarNotSetWindow = new FirstTimeConfiguration();
                this.Hide();
                if(envVarNotSetWindow.ShowDialog() == true)
                {
                    this.Show();
                }
                else
                {
                    this.Close();
                    return;
                }
            }
            try
            {
                client = TextToSpeechClient.Create();
            }
            catch(Exception e)
            {
                MessageBox.Show("Google cloud API could not be initialized. Please make sure it is setup correctly and you have an internet connection.\nFull error message: \n\n" + e.Message);
                Close();
                return;
            }

            //UI initialization:
            InitializeComponent();

            ComboBox_LanguageCode.SelectedIndex = 0;
            DataGrid_FileList.ItemsSource = ((LanguagePack)ComboBox_LanguageCode.SelectedItem).SystemVoices;
            ComboBox_VoiceName.ItemsSource = ListVoices(((LanguagePack)ComboBox_LanguageCode.SelectedItem).LanguageCode);
            ComboBox_VoiceName.SelectedItem = currentFile.VoiceName;
        }

        public LanguagePack createDefaultVoicePack()
        {
            LanguagePack newPack = new LanguagePack();
            newPack.LanguageCode = "en-US";
            string[] defaultSystemFileNames = { "0000", "0001", "0002", "0003", "0004", "0005", "0006", "0007", "0008", "0009", "0010", "0011", "0012", "0013", "0014", "0015", "0016", "0017", "0018", "0019", "0020", "0021", "0022", "0023", "0024", "0025", "0026", "0027", "0028", "0029", "0030", "0031", "0032", "0033", "0034", "0035", "0036", "0037", "0038", "0039", "0040", "0041", "0042", "0043", "0044", "0045", "0046", "0047", "0048", "0049", "0050", "0051", "0052", "0053", "0054", "0055", "0056", "0057", "0058", "0059", "0060", "0061", "0062", "0063", "0064", "0065", "0066", "0067", "0068", "0069", "0070", "0071", "0072", "0073", "0074", "0075", "0076", "0077", "0078", "0079", "0080", "0081", "0082", "0083", "0084", "0085", "0086", "0087", "0088", "0089", "0090", "0091", "0092", "0093", "0094", "0095", "0096", "0097", "0098", "0099", "0100", "0101", "0102", "0103", "0104", "0105", "0106", "0107", "0108", "0109", "0110", "0111", "0112", "volt0", "volt1", "amp0", "amp1", "mamp0", "mamp1", "knot0", "knot1", "mps0", "mps1", "fps0", "fps1", "kph0", "kph1", "mph0", "mph1", "meter0", "meter1", "foot0", "foot1", "celsius0", "celsius1", "fahr0", "fahr1", "percent0", "percent1", "mamph0", "mamph1", "watt0", "watt1", "mwatt0", "mwatt1", "db0", "db1", "rpm0", "rpm1", "g0", "g1", "degree0", "degree1", "rad0", "rad1", "ml0", "ml1", "founce0", "founce1", "hour0", "hour1", "minute0", "minute1", "second0", "second1", "0167", "0168", "0169", "0170", "0171", "0172", "0173", "0174", "0175", "0176", "midtrim", "maxtrim", "mintrim", "timovr1", "timovr2", "timovr3", "lowbatt", "inactiv", "thralert", "swalert", "eebad", "hello", "rssi_org", "rssi_red", "swr_red", "telemko", "telemok", "trainko", "trainok", "sensorko", "servoko", "rxko", "modelpwr"};
            string[] defaultSystemTTSText = { "0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31","32","33","34","35","36","37","38","39","40","41","42","43","44","45","46","47","48","49","50","51","52","53","54","55","56","57","58","59","60","61","62","63","64","65","66","67","68","69","70","71","72","73","74","75","76","77","78","79","80","81","82","83","84","85","86","87","88","89","90","91","92","93","94","95","96","97","98","99","100","200","300","400","500","600","700","800","900","1000","and","minus","point","volt","volts","amp","amps","milliamp","milliamps","knot","knots","meter per second","meters per second","foot per second","feet per second","kilometer per hour","kilometers per hour","mile per hour","miles per hour","meter","meters","foot","feet","degree celsius","degrees celsius","degree fahrenheit","degrees fahrenhaeit","percent","percent","milliamp hour","milliamp hours","watt","watts","milli watt","milli watts","db","db","rpm","rpm","g","g","degree","degrees","radian","radians","milliliter","milliliters","fluid ounce","fluid ounces","hour","hours","minute","minutes","second","seconds",".0",".1",".2",".3",".4",".5",".6",".7",".8",".9","trim center","maximum trim reached","minimum trim reached","timer 1 elapsed","timer 2 elapsed","timer 3 elapsed","transmitter battery low","inactivity alarm","throttle warning","switch warning","bad eeprom","Welcome to Open-TX","RSSI, low","RSSI, critical","radio antenna defective","telemetry lost","telemetry recovered","trainer signal lost","trainer signal recovered","sensor lost","servo overload","power overload","receiver still connected"};
            string[] defaultSystemComment = { "0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31","32","33","34","35","36","37","38","39","40","41","42","43","44","45","46","47","48","49","50","51","52","53","54","55","56","57","58","59","60","61","62","63","64","65","66","67","68","69","70","71","72","73","74","75","76","77","78","79","80","81","82","83","84","85","86","87","88","89","90","91","92","93","94","95","96","97","98","99","100","200","300","400","500","600","700","800","900","1000","and","minus","point","volt","volts","amp","amps","milliamp","milliamps","knot","knots","meter per second","meters per second","foot per second","feet per second","kmh","kmh","mile per hour","miles per hour","meter","meters","foot","feet","degree celsius","degrees celsius","degree fahrenheit","degrees fahrenhaeit","percent","percent","mah","mah","watt","watts","milli watt","milli watts","db","db","rpm","rpm","g","g","degree","degrees","radian","radians","milliliter","milliliters","fluid ounce","fluid ounces","hour","hours","minute","minutes","second","seconds",".0",".1",".2",".3",".4",".5",".6",".7",".8",".9","trim center","maximum trim reached","minimum trim reached","timer 1 elapsed","timer 2 elapsed","timer 3 elapsed","transmitter battery low","inactivity alarm","throttle warning","switch warning","bad eeprom","Start Sound","rf signal low","rf signal critical","radio antenna defective","telemetry lost","telemetry recover","trainer signal lost","trainer signal recovered","sensor lost","servo overload","power overload","receiver still connected"};
            for (int i = 0; i < defaultSystemFileNames.Length; i++)
            {
                newPack.SystemVoices.Add(new VoiceEntry { filename = defaultSystemFileNames[i], comment = defaultSystemComment[i], TTSText = defaultSystemTTSText[i] });
            }
            return newPack;
        }

        public LanguagePack createEmptyLanguagePack()
        {
            currentFile.VoiceName = "";
            currentFile.outputPath = "SOUNDS";
            currentFile.GenerateSystemSounds = true;
            currentFile.GenerateUserSounds = true;
            currentFile.OverrideSounds = false;
            LanguagePack newPack = createDefaultVoicePack();
            currentFile.LanguagePacks.Add(newPack);
            return newPack;
        }

        public void GUIRefresh()
        {
            ComboBox_LanguageCode.Items.Refresh();
            DataGrid_FileList.Items.Refresh();
            ComboBox_VoiceName.Items.Refresh();
        }

        public List<string> ListVoices(string desiredLanguageCode)
        {
            List<string> supportedVoiceNames = new List<string>();

            // Performs the list voices request
            var response = client.ListVoices(new ListVoicesRequest
            {
                LanguageCode = desiredLanguageCode
            });

            foreach (Voice voice in response.Voices)
            {
                supportedVoiceNames.Add(voice.Name);
            }
            return supportedVoiceNames;
        }

        void save(bool forcePickLocation)
        {
            if(forcePickLocation || FileLocation == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Soundpack files (*.json)|*.json";
                if (saveFileDialog.ShowDialog() == true)
                {
                    FileLocation = saveFileDialog.FileName;
                }
            }
            if(FileLocation != "")
            {
                string serializedData = JsonConvert.SerializeObject(currentFile, Formatting.Indented);
                using (StreamWriter outputStream = new StreamWriter(FileLocation))
                {
                    outputStream.Write(serializedData);
                }
            }
        }

        void open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Soundpack files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    currentFile = JsonConvert.DeserializeObject<SoundpackConfiguration>(sr.ReadToEnd());

                    ComboBox_VoiceName.SelectedItem = currentFile.VoiceName;
                }
                FileLocation = openFileDialog.FileName;
            }
            GUIRefresh();
        }

        public void playTTSText(string ssml, string usedLanguageCode, string voiceName)
        {
            if(!soundIsPlaying)
            {
                //TODO: Maybe improve by playing the stream directly without saving it as a file first
                generateTTSSoundFile("output.mp3", ssml, usedLanguageCode, voiceName);
                PlaySound("output.mp3");
            }
        }

        public void generateTTSSoundFile(string outputFileName, string ssml, string usedLanguageCode, string voiceName)
        {
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Ssml = ssml
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = usedLanguageCode,
                    Name = voiceName
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create(outputFileName))
            {
                response.AudioContent.WriteTo(output);
            }
        }

        private static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        {
            using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(_outPath_, pcm);
                }
            }
        }

        public void PlaySound(string filename)
        {
            if(!soundIsPlaying)
            {
                DirectSoundOut soundOutput = new DirectSoundOut(200);
                soundOutput.PlaybackStopped += soundOutput_PlaybackStopped;
                SoundOutputFile = new AudioFileReader(filename) {Volume = 1};
                soundOutput.Init(new WaveChannel32(SoundOutputFile) { PadWithZeroes = false});
                soundOutput.Play();
                soundIsPlaying = true;
                Button_PlaySelected.IsEnabled = false;
            }
        }

        private void soundOutput_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            SoundOutputFile.Close();
            soundIsPlaying = false;
            Button_PlaySelected.IsEnabled = true;
        }

        

        /*******************************************************************************************************************
         * OnClick events
         ******************************************************************************************************************/

        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            LanguagePack selectedLanguagePack = ComboBox_LanguageCode.SelectedItem as LanguagePack;
            string outputPath = System.IO.Path.Combine(TextBox_OutputPath.Text, selectedLanguagePack.LanguageCode);
            System.IO.Directory.CreateDirectory(outputPath);
            foreach (var item in selectedLanguagePack.UserVoices)
            {
                generateTTSSoundFile("output.mp3", item.TTSText, selectedLanguagePack.LanguageCode, ComboBox_VoiceName.SelectedItem as string);
                ConvertMp3ToWav("output.mp3", System.IO.Path.Combine(outputPath, item.filename + ".wav"));
            }
            outputPath = System.IO.Path.Combine(outputPath, "SYSTEM");
            System.IO.Directory.CreateDirectory(outputPath);
            foreach (var item in selectedLanguagePack.SystemVoices)
            {
                generateTTSSoundFile("output.mp3", item.TTSText, selectedLanguagePack.LanguageCode, ComboBox_VoiceName.SelectedItem as string);
                ConvertMp3ToWav("output.mp3", System.IO.Path.Combine(outputPath, item.filename + ".wav"));
            }
        }

        private void Button_SystemVoices_Click(object sender, RoutedEventArgs e)
        {
            Button_UserVoices.IsEnabled = true;
            Button_SystemVoices.IsEnabled = false;
            DataGridTextColumn_Filename.IsReadOnly = true;
            LanguagePack currentLanguagePack = ComboBox_LanguageCode.SelectedItem as LanguagePack;
            if (currentLanguagePack != null)
            {
                DataGrid_FileList.ItemsSource = currentLanguagePack.SystemVoices;
            }
            else
            {
                DataGrid_FileList.ItemsSource = null;
            }
        }

        private void Button_UserVoices_Click(object sender, RoutedEventArgs e)
        {
            Button_UserVoices.IsEnabled = false;
            Button_SystemVoices.IsEnabled = true;
            DataGridTextColumn_Filename.IsReadOnly = false;
            LanguagePack currentLanguagePack = ComboBox_LanguageCode.SelectedItem as LanguagePack;
            if (currentLanguagePack != null)
            {
                DataGrid_FileList.ItemsSource = currentLanguagePack.UserVoices;
            }
            else
            {
                DataGrid_FileList.ItemsSource = null;
            }
        }

        private void Button_BrowseOutputPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //TODO: Add check if it path ends with SOUNDS
                TextBox_OutputPath.Text = openFolderDialog.SelectedPath;
                
            }
        }

        private void Button_PlaySelected_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_FileList.SelectedItem != null)
            {
                playTTSText(((VoiceEntry)DataGrid_FileList.SelectedItem).TTSText, ((LanguagePack)ComboBox_LanguageCode.SelectedItem).LanguageCode, ComboBox_VoiceName.SelectedItem as string);
            }
        }

        /*******************************************************************************************************************
         * Change events
         ******************************************************************************************************************/

        private void ComboBox_LanguageCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguagePack currentLanguagePack = ComboBox_LanguageCode.SelectedItem as LanguagePack;
            if(currentLanguagePack != null)
            {
                ComboBox_VoiceName.ItemsSource = ListVoices(currentLanguagePack.LanguageCode);
                ComboBox_VoiceName.SelectedIndex = 0;
            }
            Button_SystemVoices_Click(sender, e);
            GUIRefresh();
        }

        private void ComboBox_VoiceName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentFile.VoiceName = ComboBox_VoiceName.SelectedItem as string;
        }

        /*******************************************************************************************************************
         * Menu item functions:
         ******************************************************************************************************************/

        //File
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save the current file before creating a new one?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    save(false);
                    currentFile = new SoundpackConfiguration();
                    createEmptyLanguagePack();
                    break;
                case MessageBoxResult.No:
                    currentFile = new SoundpackConfiguration();
                    createEmptyLanguagePack();
                    break;
            }
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save the current file before opening another one?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    save(false);
                    open();
                    break;
                case MessageBoxResult.No:
                    open();
                    break;
            }
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            save(false);
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            save(true);
        }


        //Edit
        private void MenuItem_CreateEmpty_Click(object sender, RoutedEventArgs e)
        {
            NewLanguageDialog NewLanguageWindow = new NewLanguageDialog();
            if(NewLanguageWindow.ShowDialog() == true)
            {
                LanguagePack newLanguagePack = createEmptyLanguagePack();
                newLanguagePack.LanguageCode = NewLanguageWindow.returnLanguageCode;
                ComboBox_LanguageCode.SelectedItem = newLanguagePack;
                GUIRefresh();
            }
            
        }

        private void MenuItem_Duplicate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the currently selected soundpack?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                 currentFile.LanguagePacks.Remove((LanguagePack)ComboBox_LanguageCode.SelectedItem);
                 ComboBox_LanguageCode.SelectedIndex = 0;
                 GUIRefresh();
            }
        }

        /*******************************************************************************************************************
         * Window events
         ******************************************************************************************************************/

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save the current file before quitting?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    save(false);
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }
    }
}
