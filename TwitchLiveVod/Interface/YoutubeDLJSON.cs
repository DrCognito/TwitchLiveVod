using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TwitchLiveVod.Interface
{
    class YoutubeDLJSON : object
    {
        public string url { get; set; }

        private string _output;
        public string output
        {
            get
            {
                if(_output == null)
                {
                    _output = getJSON(url);
                }
                return _output;
            }

        }

        Logger MainLog;


        public YoutubeDLJSON(string inUrl)
        {
            url = inUrl;
            MainLog = Logger.Instance;
        }

        public YoutubeDLJSON()
        {
            url = null;
            MainLog = Logger.Instance;
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            YoutubeDLJSON p = obj as YoutubeDLJSON;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return url == p.url;
        }

        public override int GetHashCode()
        {
            return url.GetHashCode();
        }

        
        private string getJSON(string inUrl)
        {
            //http://stackoverflow.com/questions/206323
            string temp;
            using (Process p = new Process())
            {

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "youtube-dl.exe";
                p.StartInfo.Arguments = inUrl + " -J";
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                temp = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            MainLog.Append(string.Format("Got JSON from {0}", inUrl));
            return temp;
        }

        public void Update()
        {
            getJSON(url);
        }
    }
}
