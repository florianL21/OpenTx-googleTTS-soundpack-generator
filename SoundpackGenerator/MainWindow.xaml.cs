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
using NAudio.Wave.SampleProviders;
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
            public string VoiceName { get; set; }

            public LanguagePack()
            {
                SystemVoices = new List<VoiceEntry>();
                UserVoices = new List<VoiceEntry>();
            }
        }

        public class SoundpackConfiguration
        {
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
        public bool generateAllLanguages { get; set; }
        private AudioFileReader SoundOutputFile = null;
        private bool soundIsPlaying = false;


        public MainWindow()
        {
            //Data initialization:
            DataContext = this;
            currentFile = new SoundpackConfiguration();
            generateAllLanguages = false;
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
            if (ComboBox_VoiceName.SelectedIndex == -1)
            {
                ComboBox_VoiceName.SelectedIndex = 0;
            }
            GUIRefresh();
        }

        public LanguagePack createDefaultVoicePack()
        {
            LanguagePack newPack = new LanguagePack();
            newPack.LanguageCode = "en-US";
            newPack.VoiceName = "";
            string[] defaultSystemFileNames = { "0000", "0001", "0002", "0003", "0004", "0005", "0006", "0007", "0008", "0009", "0010", "0011", "0012", "0013", "0014", "0015", "0016", "0017", "0018", "0019", "0020", "0021", "0022", "0023", "0024", "0025", "0026", "0027", "0028", "0029", "0030", "0031", "0032", "0033", "0034", "0035", "0036", "0037", "0038", "0039", "0040", "0041", "0042", "0043", "0044", "0045", "0046", "0047", "0048", "0049", "0050", "0051", "0052", "0053", "0054", "0055", "0056", "0057", "0058", "0059", "0060", "0061", "0062", "0063", "0064", "0065", "0066", "0067", "0068", "0069", "0070", "0071", "0072", "0073", "0074", "0075", "0076", "0077", "0078", "0079", "0080", "0081", "0082", "0083", "0084", "0085", "0086", "0087", "0088", "0089", "0090", "0091", "0092", "0093", "0094", "0095", "0096", "0097", "0098", "0099", "0100", "0101", "0102", "0103", "0104", "0105", "0106", "0107", "0108", "0109", "0110", "0111", "0112", "volt0", "volt1", "amp0", "amp1", "mamp0", "mamp1", "knot0", "knot1", "mps0", "mps1", "fps0", "fps1", "kph0", "kph1", "mph0", "mph1", "meter0", "meter1", "foot0", "foot1", "celsius0", "celsius1", "fahr0", "fahr1", "percent0", "percent1", "mamph0", "mamph1", "watt0", "watt1", "mwatt0", "mwatt1", "db0", "db1", "rpm0", "rpm1", "g0", "g1", "degree0", "degree1", "rad0", "rad1", "ml0", "ml1", "founce0", "founce1", "hour0", "hour1", "minute0", "minute1", "second0", "second1", "0167", "0168", "0169", "0170", "0171", "0172", "0173", "0174", "0175", "0176", "midtrim", "maxtrim", "mintrim", "timovr1", "timovr2", "timovr3", "lowbatt", "inactiv", "thralert", "swalert", "eebad", "hello", "rssi_org", "rssi_red", "swr_red", "telemko", "telemok", "trainko", "trainok", "sensorko", "servoko", "rxko", "modelpwr"};
            string[] defaultSystemTTSText = { "0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31","32","33","34","35","36","37","38","39","40","41","42","43","44","45","46","47","48","49","50","51","52","53","54","55","56","57","58","59","60","61","62","63","64","65","66","67","68","69","70","71","72","73","74","75","76","77","78","79","80","81","82","83","84","85","86","87","88","89","90","91","92","93","94","95","96","97","98","99","100","200","300","400","500","600","700","800","900","1000","", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ".0", ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".8", ".9", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};
            string[] defaultSystemComment = { "0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31","32","33","34","35","36","37","38","39","40","41","42","43","44","45","46","47","48","49","50","51","52","53","54","55","56","57","58","59","60","61","62","63","64","65","66","67","68","69","70","71","72","73","74","75","76","77","78","79","80","81","82","83","84","85","86","87","88","89","90","91","92","93","94","95","96","97","98","99","100","200","300","400","500","600","700","800","900","1000","", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ".0", ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".8", ".9", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};
            for (int i = 0; i < defaultSystemFileNames.Length; i++)
            {
                newPack.SystemVoices.Add(new VoiceEntry { filename = defaultSystemFileNames[i], comment = defaultSystemComment[i], TTSText = defaultSystemTTSText[i] });
            }
            return newPack;
        }

        public LanguagePack createEmptyLanguagePack()
        {
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
                }
                FileLocation = openFileDialog.FileName;
            }
            ComboBox_LanguageCode.Items.Refresh();
            ComboBox_LanguageCode.SelectedIndex = 0;
            DataGrid_FileList.ItemsSource = ((LanguagePack)ComboBox_LanguageCode.SelectedItem).SystemVoices;
            ComboBox_VoiceName.ItemsSource = ListVoices(((LanguagePack)ComboBox_LanguageCode.SelectedItem).LanguageCode);
            if (ComboBox_VoiceName.SelectedIndex == -1)
            {
                ComboBox_VoiceName.SelectedIndex = 0;
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

        private static void ConvertMp3ToWav(string inPath, string outPath)
        {
            //mp3 file is already mono
            using (var mp3 = new AudioFileReader(inPath))
            {
                //resample to 32kHz
                var resampler = new WdlResamplingSampleProvider(mp3, 32000);
                // and store as 16 bit
                WaveFileWriter.CreateWaveFile16(outPath, resampler);
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

        private bool createWavFileAt(string TTSText, string LanguageCode, string VoiceName, string outputPath, string filename)
        {
            string FilePath = System.IO.Path.Combine(outputPath, filename + ".wav");
            if (filename != null && filename != "" && (!File.Exists(FilePath) || currentFile.OverrideSounds) && TTSText != null && TTSText != "")
            {
                generateTTSSoundFile("output.mp3", TTSText, LanguageCode, VoiceName);
                ConvertMp3ToWav("output.mp3", FilePath);
                return true;
            }
            return false;
        }

        private int generateLanguagePack(LanguagePack PackToGenerate, string outputPath)
        {
            int numberOfCreatedFiles = 0;
            int numOfTotalFiles = PackToGenerate.UserVoices.Count + PackToGenerate.SystemVoices.Count;
            int numOfIterationsDone = 0;
            System.IO.Directory.CreateDirectory(outputPath);
            this.Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
            {
                ProgressBar_Progress.Maximum = numOfTotalFiles;
            });
            foreach (var item in PackToGenerate.UserVoices)
            {
                if (createWavFileAt(item.TTSText, PackToGenerate.LanguageCode, PackToGenerate.VoiceName, outputPath, item.filename))
                {
                    numberOfCreatedFiles++;
                }
                numOfIterationsDone++;
                this.Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
                {
                    ProgressBar_Progress.Value = numOfIterationsDone;
                    TextBlock_CurrentFile.Text = "File: " + System.IO.Path.Combine(outputPath, item.filename + ".wav");
                });
            }
            string systemFilePath = System.IO.Path.Combine(outputPath, "SYSTEM");
            System.IO.Directory.CreateDirectory(systemFilePath);
            foreach (var item in PackToGenerate.SystemVoices)
            {
                if (createWavFileAt(item.TTSText, PackToGenerate.LanguageCode, PackToGenerate.VoiceName, systemFilePath, item.filename))
                {
                    numberOfCreatedFiles++;
                }
                numOfIterationsDone++;
                this.Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
                {
                    ProgressBar_Progress.Value = numOfIterationsDone;
                    TextBlock_CurrentFile.Text = "File: " + System.IO.Path.Combine(outputPath, item.filename + ".wav");
                });
            }
            return numberOfCreatedFiles;
        }

        private void generate(List<LanguagePack> packsToGenerate, string rootOutputPath)
        {
            int numOfFilesCreated = 0;
            int totalNumberOfFiles = 0;
            int iteration = 1;
            this.Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
            {
                Grid_MainGrid.IsEnabled = false;
            });
            

            foreach (var item in packsToGenerate)
            {
                totalNumberOfFiles += item.UserVoices.Count + item.SystemVoices.Count;
                string outputPath = System.IO.Path.Combine(rootOutputPath, item.LanguageCode);
                this.Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
                {
                    TextBlock_Progress.Text = "Generating " + iteration +"/" + packsToGenerate.Count + " Language Packs";
                });
                numOfFilesCreated += generateLanguagePack(item, outputPath);
                iteration++;
            }

            MessageBox.Show("Generation Process finished. " + numOfFilesCreated + "/" + totalNumberOfFiles + " files were created.");

            this.Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
            {
                Grid_MainGrid.IsEnabled = true;
                TextBlock_Progress.Text = "Done";
            });
        }

        private bool isSigleLineValid(VoiceEntry entryToValidate)
        {
            if (entryToValidate.filename == null || entryToValidate.filename == "")
            {
                MessageBox.Show("Filename is empty. If moving forward this entry will be ignored.\n Entry comment: " + entryToValidate.comment + "\n Entry TTS: " + entryToValidate.TTSText);
            }
            else if (entryToValidate.filename.Length > 7 || entryToValidate.filename.Any(char.IsUpper))
            {
                MessageBox.Show("Filename is too long or contains uppercase letters. (max 7 lower case characters) If moving forward this file will not be selectable from within the transmitter.\n Entry filename: " + entryToValidate.filename + "\n Entry comment: " + entryToValidate.comment + "\n Entry TTS: " + entryToValidate.TTSText);
            }
            else if (entryToValidate.TTSText == null || entryToValidate.TTSText == "")
            {
                MessageBox.Show("TTS Text is empty for:\n Entry filename: " + entryToValidate.filename + "\n Entry comment: " + entryToValidate.comment);
            }
            else
            {
                return true;
            }
            return false;
        }

        private bool LanguagePackIsValid(LanguagePack packToValidate)
        {
            bool PackIsValid = true;
            if (packToValidate.VoiceName == null || packToValidate.VoiceName == "")
            {
                MessageBox.Show("Please select a voice to use for the language pack " + packToValidate.LanguageCode + ". if no voice is selected the default one fill be used.");
                PackIsValid = false;
            }
            
            foreach(var item in packToValidate.UserVoices)
            {
                if(!isSigleLineValid(item))
                {
                    PackIsValid = false;
                }
            }
            foreach (var item in packToValidate.SystemVoices)
            {
                if (!isSigleLineValid(item))
                {
                    PackIsValid = false;
                }
            }
            return PackIsValid;
        }

        /*******************************************************************************************************************
         * OnClick events
         ******************************************************************************************************************/

        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            string outputPath = TextBox_OutputPath.Text;
            List<LanguagePack> listOfGeneratedPacks = new List<LanguagePack>();
            if(!generateAllLanguages)
            {
                LanguagePack selectedLanguagePack = ComboBox_LanguageCode.SelectedItem as LanguagePack;
                if (!LanguagePackIsValid(selectedLanguagePack))
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to abort and fix the problem? Pressing no will simply skip the generation of the invalid entrys.", "Skip or fix?", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        return;
                    }
                }
                listOfGeneratedPacks.Add(selectedLanguagePack);
            }
            else
            {
                foreach (var item in currentFile.LanguagePacks)
                {
                    if(!LanguagePackIsValid(item))
                    {
                        MessageBoxResult result = MessageBox.Show("Do you want to abort and fix the problem? Pressing no will simply skip the generation of the invalid entrys.", "Skip or fix?", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                    listOfGeneratedPacks.Add(item);
                }
            }
            Task.Factory.StartNew(() => generate(listOfGeneratedPacks, outputPath));
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
                if(openFolderDialog.SelectedPath.EndsWith("SOUND"))
                {
                    TextBox_OutputPath.Text = openFolderDialog.SelectedPath;
                }
                else
                {
                    TextBox_OutputPath.Text = System.IO.Path.Join(openFolderDialog.SelectedPath, "SOUND");
                }
            }
        }

        private void Button_PlaySelected_Click(object sender, RoutedEventArgs e)
        {
            if(ComboBox_VoiceName.SelectedItem == null)
            {
                MessageBox.Show("Please select a voice to use first.");
                return;
            }
            if (DataGrid_FileList.SelectedItem != null)
            {
                try
                {
                    playTTSText(((VoiceEntry)DataGrid_FileList.SelectedItem).TTSText, ((LanguagePack)ComboBox_LanguageCode.SelectedItem).LanguageCode, ComboBox_VoiceName.SelectedItem as string);
                }
                catch
                {
                    MessageBox.Show("Sound preview failed");
                }
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
            }
            Button_SystemVoices_Click(sender, e);
            GUIRefresh();
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
