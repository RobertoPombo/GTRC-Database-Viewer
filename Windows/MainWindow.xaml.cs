using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

using GTRC_Basics;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (!Directory.Exists(GlobalValues.DataDirectory)) { Directory.CreateDirectory(GlobalValues.DataDirectory); }
            GlobalWinValues.SetCultureInfo();
            GlobalWinValues.UpdateWpfColors(this);
            //InitializeNotifications();
            InitializeComponent();
            Width = GlobalWinValues.screenWidth * 0.6;
            Height = GlobalWinValues.screenHeight * 0.6;
            Left = ((GlobalWinValues.screenWidth / 2) - (Width / 2)) * 1.9;
            Top = ((GlobalWinValues.screenHeight / 2) - (Height / 2)) * 1.8;
            Closing += CloseWindow;
        }

        public void CloseWindow(object? sender, CancelEventArgs e) { }
        /*
        public void InitializeNotifications()
        {
            ThemeColor.Statics.PublishList += UpdateThemeColors;
        }

        public void UpdateThemeColors()
        {
            for (int colorNr = 0; colorNr < ThemeColor.Statics.List.Count; colorNr++)
            {
                SolidColorBrush _color = new(Color.FromArgb(
                    (byte)ThemeColor.Statics.List[colorNr].Alpha,
                    (byte)ThemeColor.Statics.List[colorNr].Red,
                    (byte)ThemeColor.Statics.List[colorNr].Green,
                    (byte)ThemeColor.Statics.List[colorNr].Blue));
                if (colorNr < WpfColors.List.Count) { WpfColors.List[colorNr] = _color; }
                else { WpfColors.List.Add(_color); }
            }
            GlobalWinValues.UpdateWpfColors(this);
        }*/
    }
}