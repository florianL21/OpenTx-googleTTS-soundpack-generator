using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace SoundpackGenerator
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
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

        public SoundpackConfiguration currentFile { get; set; }
        public string FileLocation = "";

        public MainPage()
        {
            DataContext = this;
            currentFile = new SoundpackConfiguration();
            createEmptyLanguagePack();
            this.InitializeComponent();
            DataGrid_FileList.ItemsSource = currentFile.LanguagePacks[0].SystemVoices;
        }

        public LanguagePack createDefaultVoicePack()
        {
            LanguagePack newPack = new LanguagePack();
            newPack.LanguageCode = "en-US";
            string[] defaultSystemFileNames = { "0000", "0001", "0002", "0003", "0004", "0005", "0006", "0007", "0008", "0009", "0010", "0011", "0012", "0013", "0014", "0015", "0016", "0017", "0018", "0019", "0020", "0021", "0022", "0023", "0024", "0025", "0026", "0027", "0028", "0029", "0030", "0031", "0032", "0033", "0034", "0035", "0036", "0037", "0038", "0039", "0040", "0041", "0042", "0043", "0044", "0045", "0046", "0047", "0048", "0049", "0050", "0051", "0052", "0053", "0054", "0055", "0056", "0057", "0058", "0059", "0060", "0061", "0062", "0063", "0064", "0065", "0066", "0067", "0068", "0069", "0070", "0071", "0072", "0073", "0074", "0075", "0076", "0077", "0078", "0079", "0080", "0081", "0082", "0083", "0084", "0085", "0086", "0087", "0088", "0089", "0090", "0091", "0092", "0093", "0094", "0095", "0096", "0097", "0098", "0099", "0100", "0101", "0102", "0103", "0104", "0105", "0106", "0107", "0108", "0109", "0110", "0111", "0112", "volt0", "volt1", "amp0", "amp1", "mamp0", "mamp1", "knot0", "knot1", "mps0", "mps1", "fps0", "fps1", "kph0", "kph1", "mph0", "mph1", "meter0", "meter1", "foot0", "foot1", "celsius0", "celsius1", "fahr0", "fahr1", "percent0", "percent1", "mamph0", "mamph1", "watt0", "watt1", "mwatt0", "mwatt1", "db0", "db1", "rpm0", "rpm1", "g0", "g1", "degree0", "degree1", "rad0", "rad1", "ml0", "ml1", "founce0", "founce1", "hour0", "hour1", "minute0", "minute1", "second0", "second1", "0167", "0168", "0169", "0170", "0171", "0172", "0173", "0174", "0175", "0176", "midtrim", "maxtrim", "mintrim", "timovr1", "timovr2", "timovr3", "lowbatt", "inactiv", "thralert", "swalert", "eebad", "hello", "rssi_org", "rssi_red", "swr_red", "telemko", "telemok", "trainko", "trainok", "sensorko", "servoko", "rxko", "modelpwr" };
            string[] defaultSystemTTSText = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "100", "200", "300", "400", "500", "600", "700", "800", "900", "1000", "and", "minus", "point", "volt", "volts", "amp", "amps", "milliamp", "milliamps", "knot", "knots", "meter per second", "meters per second", "foot per second", "feet per second", "kilometer per hour", "kilometers per hour", "mile per hour", "miles per hour", "meter", "meters", "foot", "feet", "degree celsius", "degrees celsius", "degree fahrenheit", "degrees fahrenhaeit", "percent", "percent", "milliamp hour", "milliamp hours", "watt", "watts", "milli watt", "milli watts", "db", "db", "rpm", "rpm", "g", "g", "degree", "degrees", "radian", "radians", "milliliter", "milliliters", "fluid ounce", "fluid ounces", "hour", "hours", "minute", "minutes", "second", "seconds", ".0", ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".8", ".9", "trim center", "maximum trim reached", "minimum trim reached", "timer 1 elapsed", "timer 2 elapsed", "timer 3 elapsed", "transmitter battery low", "inactivity alarm", "throttle warning", "switch warning", "bad eeprom", "Welcome to Open-TX", "RSSI, low", "RSSI, critical", "radio antenna defective", "telemetry lost", "telemetry recovered", "trainer signal lost", "trainer signal recovered", "sensor lost", "servo overload", "power overload", "receiver still connected" };
            string[] defaultSystemComment = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "100", "200", "300", "400", "500", "600", "700", "800", "900", "1000", "and", "minus", "point", "volt", "volts", "amp", "amps", "milliamp", "milliamps", "knot", "knots", "meter per second", "meters per second", "foot per second", "feet per second", "kmh", "kmh", "mile per hour", "miles per hour", "meter", "meters", "foot", "feet", "degree celsius", "degrees celsius", "degree fahrenheit", "degrees fahrenhaeit", "percent", "percent", "mah", "mah", "watt", "watts", "milli watt", "milli watts", "db", "db", "rpm", "rpm", "g", "g", "degree", "degrees", "radian", "radians", "milliliter", "milliliters", "fluid ounce", "fluid ounces", "hour", "hours", "minute", "minutes", "second", "seconds", ".0", ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".8", ".9", "trim center", "maximum trim reached", "minimum trim reached", "timer 1 elapsed", "timer 2 elapsed", "timer 3 elapsed", "transmitter battery low", "inactivity alarm", "throttle warning", "switch warning", "bad eeprom", "Start Sound", "rf signal low", "rf signal critical", "radio antenna defective", "telemetry lost", "telemetry recover", "trainer signal lost", "trainer signal recovered", "sensor lost", "servo overload", "power overload", "receiver still connected" };
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
    }
}
