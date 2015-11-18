using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TwitchLiveVod
{
    class MediaSetup : INotifyPropertyChanged
    {
        private string _MediaPlayerLocation = "Media player location";
        private string _URLPath = "Enter URL...";
        private string _MediaPlayerLaunchOptions = @"%PlayList";

       


        public MediaSetup()
        {
            //Load up the last settings if they exist...
            
            URLPath = (string)Properties.Settings.Default["LastVODURL"];
            MediaPlayerLocation = (string)Properties.Settings.Default["MediaPlayerURL"];
            MediaPlayerLaunchOptions = (string)Properties.Settings.Default["MediaPlayerLaunchOptions"];
        }

        public string MediaPlayerLocation
        {
            get { return _MediaPlayerLocation; }
            set
            {
                _MediaPlayerLocation = value;
                RaisePropertyChanged();
            }
        }

        public string URLPath
        {
            get { return _URLPath; }
            set
            {
                _URLPath = value;
                RaisePropertyChanged();
            }
        }

        public string MediaPlayerLaunchOptions
        {
            get { return _MediaPlayerLaunchOptions; }
            set
            {
                _MediaPlayerLaunchOptions = value;
                Properties.Settings.Default["MediaPlayerLaunchOptions"] = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        private string _SelectedQuality = "Source";

        public string SelectedQuality
        {
            get { return _SelectedQuality; }
            set
            {
                _SelectedQuality = value;
                RaisePropertyChanged();
            }
        }


        //This is how we raise an event when something changes in the code behind for the UI.
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string PropertyName = "")
        {
            //The null check here is neccessary because if no one subscribes to the event then it is null.
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
