using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

//http://www.codeproject.com/Articles/21338/A-C-Central-Logging-Mechanism-using-the-Observer-a

namespace TwitchLiveVod
{
    class Logger : INotifyPropertyChanged
    {
        private static object _ThreadLock = new object();
        private static Logger _Logger = null;

        public static Logger Instance
        {
            get
            {
                if (_Logger == null)
                {
                    //Make sure only one thread can proceed for thread safety
                    lock (_ThreadLock)
                    {
                        //In case another thread gets here later after the lock.
                        if (_Logger == null)
                        {
                            _Logger = new Logger();
                        }
                    }
                }
                return _Logger;
            }
        }

        private string _log;

        public void Append(string LogSnippet)
        {
            Log += LogSnippet;
            Log += Environment.NewLine;
        }


        public string Log
        {
            get { return _log; }
            set
            {
                _log = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        private Logger()
        {
            _ThreadLock = new object();
        }
    }
}
