using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using TwitchLiveVod.Interface;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Data;

namespace TwitchLiveVod
{
    class MediaInfo : INotifyPropertyChanged
    {
        private Dictionary<string, string> _HLSOutput;
        private Logger MainLog;

        public string JSONIn
        {
            get { return JSONProvider.output; }
            private set
            {
                if (JSONIn != value)
                {
                    _HLSOutput.Clear();
                    JSONIn = value;
                }
            }
        }

        private YoutubeDLJSON _JSONProvider;
        public YoutubeDLJSON JSONProvider
        {
            get { return _JSONProvider; }
            set
            {
                if (_JSONProvider != value)
                {
                    if (_HLSOutput != null) { _HLSOutput.Clear(); }
                    _JSONProvider = value;
                    JSONIn = value.output;
                    QualityOptions = ListOfQualities(value.output);
                }
            }
        }

        //public MediaInfo(string JSON)
        //{
        //    MainLog = Logger.Instance;
        //    JSONIn = JSON;
        //}

        private List<string> _QualityOptions = new List<string>();

        public List<string> QualityOptions
        {
            get { return _QualityOptions; }
            set
            {
                _QualityOptions = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string PropertyName = "")
        {
            //The null check here is neccessary because if no one subscribes to the event then it is null.
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        

        public MediaInfo(YoutubeDLJSON JSON)
        {
            MainLog = Logger.Instance;
            JSONProvider = JSON;
            _HLSOutput = new Dictionary<string, string>();
            if (JSON != null)
            {
                QualityOptions = ListOfQualities(JSON.output);
            }
            else { QualityOptions.Add("Source"); }
            if(QualityOptions.Count == 0) { QualityOptions.Add("Source"); }
        }

        public string GetHLSByQuality(string Quality = "Source")
        {
            if (!_HLSOutput.ContainsKey(Quality))
            {
                string url = GetUrlByQuality(JSONProvider.output, Quality);
                string playList;

                using (var web = new System.Net.WebClient())
                    playList = web.DownloadString(url);

                string baseUrl = url.Substring(0, url.LastIndexOf("/")) + "/index-";

                playList = playList.Replace("index-", baseUrl);
                playList += Environment.NewLine;
                playList += @"#EXT-X-ENDLIST";

                _HLSOutput.Add(Quality, playList);

                return playList;
            }

            MainLog.Append(string.Format("Using cached playlist for quality {0}.", Quality));
            return _HLSOutput[Quality];
        }

        public static string GetUrlByQuality(string JSON, string Quality)
        {
            string playList = string.Empty;
            JsonTextReader JsonIn = new JsonTextReader(new StringReader(JSON));

            while (JsonIn.Read())
            {
                if (JsonIn.Value != null)
                {
                    if (JsonIn.TokenType == JsonToken.PropertyName && (string)JsonIn.Value == "format")
                    {
                        JsonIn.Read();
                        if (JsonIn.TokenType == JsonToken.String && ((string)JsonIn.Value).Contains(Quality))
                        {

                            JsonIn.Read();
                            if (JsonIn.TokenType == JsonToken.PropertyName && (string)JsonIn.Value == "url")
                            {
                                JsonIn.Read();

                                playList = (string)JsonIn.Value;
                            }
                        }
                    }
                }
            }
            return playList;
        }

        public static List<string> ListOfQualities(string JSON)
        {
            List<string> QualityList = new List<string>();
            JsonTextReader JsonIn = new JsonTextReader(new StringReader(JSON));

            while (JsonIn.Read())
            {
                if (JsonIn.Value != null)
                {
                    if (JsonIn.TokenType == JsonToken.PropertyName && (string)JsonIn.Value == "format")
                    {
                        JsonIn.Read();
                        if (JsonIn.TokenType == JsonToken.String)
                        {
                            string Quality = (string)JsonIn.Value;

                            JsonIn.Read();
                            if (JsonIn.TokenType == JsonToken.PropertyName && (string)JsonIn.Value == "url")
                            {
                                JsonIn.Read();
                                QualityList.Add(Quality);

                            }
                        }
                    }
                }
            }
            return QualityList;
        }
    }
}
