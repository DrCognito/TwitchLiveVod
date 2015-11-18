using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TwitchLiveVod.Interface;

namespace TwitchLiveVod
{
    class MediaOutput 
    {
        private static object _ThreadLock = new object();
        private static MediaOutput _MediaOut = null;

        public MediaSetup MediaS { get; set; }
        public MediaInfo MediaI { get; set; }

        Logger MainLog = Logger.Instance;

        public static MediaOutput Instance
        {
            get
            {
                if (_MediaOut == null)
                {
                    //Make sure only one thread can proceed for thread safety
                    lock (_ThreadLock)
                    {
                        //In case another thread gets here later after the lock.
                        if (_MediaOut == null)
                        {
                            _MediaOut = new MediaOutput();
                        }
                    }
                }
                return _MediaOut;
            }
        }

        private MediaOutput()
        {
            MediaS = null;
            MediaI = null;
        }

        public void LaunchPlayer(object target, ExecutedRoutedEventArgs e)
        {
            string LaunchOptions = MediaS.MediaPlayerLaunchOptions;
            string TempPlayListPath = Path.GetTempFileName();

            using (StreamWriter outPutFile = new StreamWriter(TempPlayListPath))
            {
                outPutFile.Write(MediaI.GetHLSByQuality(MediaS.SelectedQuality));
            }
            LaunchOptions = LaunchOptions.Replace(@"%PlayList", TempPlayListPath);
            MainLog.Append(string.Format("Launching player at {0} with args {1} for quality {2}.", 
                MediaS.MediaPlayerLocation, LaunchOptions, MediaS.SelectedQuality));


            Process p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = MediaS.MediaPlayerLocation;
            p.StartInfo.Arguments = LaunchOptions;
            p.Start();


        }
        public void CanLaunchPlayer(object target, CanExecuteRoutedEventArgs e)
        {
            if (MediaS.MediaPlayerLocation != "Media player location") { e.CanExecute = true; }
            else { e.CanExecute = false; }
        }


        public void SavePlayList(object target, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Filter = @"Playlists (m3u8)|*.m3u8";
            //SaveFile.RestoreDirectory = true;
            SaveFile.InitialDirectory = (string)Properties.Settings.Default["LastSaveLocation"];
            if (SaveFile.ShowDialog() == true)
            {
                File.WriteAllText(SaveFile.FileName, MediaI.GetHLSByQuality(MediaS.SelectedQuality));
                Properties.Settings.Default["LastSaveLocation"] = SaveFile.FileName;
                Properties.Settings.Default.Save();
            }
        }
        public void CanSavePlaylist(object target, CanExecuteRoutedEventArgs e)
        {
            if (MediaS.URLPath != "Enter URL...") { e.CanExecute = true; }
            else { e.CanExecute = false; }
        }

        public void RetrieveJSONFile(object target, ExecutedRoutedEventArgs e)
        {
            MainLog.Append(string.Format("Retrieving JSON at {0}", MediaS.URLPath));
            MediaI.JSONProvider = new YoutubeDLJSON(MediaS.URLPath);
            Properties.Settings.Default["LastVODURL"] = MediaS.URLPath;
            Properties.Settings.Default.Save();
        }
        public void CanRetrieveJSONFile(object target, CanExecuteRoutedEventArgs e)
        {
            if (MediaS.URLPath != "Enter URL...") { e.CanExecute = true; }
            else { e.CanExecute = false; }
        }

        public void SetMediaPlayerLocation(object target, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog FindPlayer = new OpenFileDialog();

            if (FindPlayer.ShowDialog() == true)
            {
                MediaS.MediaPlayerLocation = FindPlayer.FileName;
                Properties.Settings.Default["MediaPlayerURL"] = FindPlayer.FileName;
                Properties.Settings.Default.Save();
            }
        }
        public void CanSetMediaPlayerLocation(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
