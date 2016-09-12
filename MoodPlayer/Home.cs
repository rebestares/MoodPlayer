using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.IO;
using System.Linq;
using System.Text;
using WMPLib;

namespace MoodPlayer
{
    public partial class MoodPlayer : Form
    {
        readonly List<string> _songs = new List<string>() { "sad", "happy", "alive", "wait", "play", "volume down", "volume up" };
        private const string Directory = "E:\\kantanantanan";
        readonly WMPLib.WindowsMediaPlayer _wplayer = new WMPLib.WindowsMediaPlayer();
        public MoodPlayer()
        {
            InitializeComponent();
            SpeechRecognizer recognizer = new SpeechRecognizer();

            var filePath = DirSearch(Directory);
            // The command that you can use
            var commands = new Choices();
            commands.Add(_songs.Where(a => a != string.Empty).ToArray());

            // Create a GrammarBuilder object and append the Choices object.
            var gb = new GrammarBuilder();
            gb.Append(commands);

            // Create the Grammar instance and load it into the speech recognition engine.
            var g = new Grammar(gb);
            recognizer.LoadGrammar(g);

            // Register a handler for the SpeechRecognized event.
            recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            _wplayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(stateChanged_WindowsPlayer);
        }
   
        // Create a simple handler for the SpeechRecognized event.
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var command = e.Result.Text;
            AnalyzeCommand(command);
        }

        void RandomSong()
        {
            var randomizer = new Random();
            var range = randomizer.Next(5, 15);
            var song = _songs[range];
            _wplayer.URL = DirSearch(Directory, song + ".mp3");
            _wplayer.controls.play();
        }

        void stateChanged_WindowsPlayer(int newState)
        {
            var state = newState;
            if (state == (int) WMPLib.WMPPlayState.wmppsMediaEnded)
            {
               // RandomSong();
            }
        }

        private void AnalyzeCommand(string command)
        {
            var url = string.Empty;

            switch (command)
            {
                case "wait":
                    _wplayer.controls.pause();
                    break;
                case "play":
                    _wplayer.controls.play();
                    break;
                default:
                    _wplayer.URL = DirSearch(Directory, command + ".mp3");
                    _wplayer.controls.play();
                    break;
            }
        }

        string DirSearch(string sDir)
        {
            var url = string.Empty;
            var file = string.Empty;
            try
            {
                foreach (var d in System.IO.Directory.GetDirectories(sDir))
                {
                    foreach (var filePath in System.IO.Directory.GetFiles(d, "*.mp3"))
                    {
                        _songs.Add(RemoveSpecialCharacters(filePath.Replace(d, "").Replace(".mp3", "")));
                    }
                    DirSearch(d);

                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return null;
        }

        static string DirSearch(string sDir, string fileName)
        {
            var url = string.Empty;
            var file = string.Empty;
            try
            {
                foreach (var d in System.IO.Directory.GetDirectories(sDir))
                {
                    foreach (var filePath in System.IO.Directory.GetFiles(d, "*" + fileName))
                    {
                        return filePath;
                    }
                    DirSearch(d, fileName);

                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return null;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str.Where(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == ' ') || (c == '-')))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
