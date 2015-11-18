using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitchLiveVod.Interface;

namespace TwitchLiveVod
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();

            
            Logger MainLog = Logger.Instance;


            
            YoutubeDLJSON JSON = new YoutubeDLJSON();
            MediaInfo MediaI = new MediaInfo(JSON);
            MediaSetup MediaS = new MediaSetup();

            MediaOutput MediaOut = MediaOutput.Instance;
            MediaOut.MediaI = MediaI;
            MediaOut.MediaS = MediaS;

            DataContext = new
            {
                MainLog,
                MediaOut.MediaS,
                MediaOut.MediaI
            };

            CommandBinding SavePlayListCommand = new CommandBinding(
                ApplicationCommands.SaveAs,
                MediaOut.SavePlayList,
                MediaOut.CanSavePlaylist);
            CommandBindings.Add(SavePlayListCommand);

            CommandBinding LaunchMediaPlayerCommand = new CommandBinding(
                CustomCommands.LaunchMediaPlayerRoutedCommand,
                MediaOut.LaunchPlayer,
                MediaOut.CanLaunchPlayer);
            CommandBindings.Add(LaunchMediaPlayerCommand);

            CommandBinding RetrieveJSONFileCommand = new CommandBinding(
                CustomCommands.RetrieveJSONFileRoutedCommand,
                MediaOut.RetrieveJSONFile,
                MediaOut.CanRetrieveJSONFile);
            CommandBindings.Add(RetrieveJSONFileCommand);

            CommandBinding SetMediaPlayerLocationCommand = new CommandBinding(
                CustomCommands.SetMediaPlayerLocationRoutedCommand,
                MediaOut.SetMediaPlayerLocation,
                MediaOut.CanSetMediaPlayerLocation);
            CommandBindings.Add(SetMediaPlayerLocationCommand);

            buttonRetrieveURL.Command = CustomCommands.RetrieveJSONFileRoutedCommand;
            buttonRetrieveURL.IsEnabled = true;
            buttonMediaPlayerLoc.Command = CustomCommands.SetMediaPlayerLocationRoutedCommand;
            buttonInstanceLaunchPlayer.Command = CustomCommands.LaunchMediaPlayerRoutedCommand;


        }

    }

    public static class CustomCommands
    {
        //Custom commands!
        public static RoutedCommand LaunchMediaPlayerRoutedCommand = new RoutedCommand();
        public static RoutedCommand RetrieveJSONFileRoutedCommand = new RoutedCommand();
        public static RoutedCommand SetMediaPlayerLocationRoutedCommand = new RoutedCommand();
    }
}
