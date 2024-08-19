using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

using GTRC_Basics;
using GTRC_Database_Viewer.ViewModels;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Scripts.CreateDirectories();
            GlobalWinValues.SetCultureInfo();
            GlobalWinValues.UpdateWpfColors(this);
            InitializeNotifications();
            InitializeComponent();
            Width = GlobalWinValues.screenWidth * 0.75;
            Height = GlobalWinValues.screenHeight * 0.75;
            Left = ((GlobalWinValues.screenWidth / 2) - (Width / 2)) * 1;
            Top = ((GlobalWinValues.screenHeight / 2) - (Height / 2)) * 1;
            Closing += CloseWindow;
        }

        public void CloseWindow(object? sender, CancelEventArgs e) { DatabaseVM.SaveFilters(); }

        public void InitializeNotifications()
        {
            DatabaseTableVM<GTRC_Basics.Models.Color>.PublishList += UpdateThemeColors;
        }

        public void UpdateThemeColors()
        {
            List<GTRC_Basics.Models.Color> colors = DatabaseVM.DictDatabaseTableVM[typeof(GTRC_Basics.Models.Color)].ObjList;
            for (int colorNr = 0; colorNr < colors.Count; colorNr++)
            {
                SolidColorBrush _color = new(Color.FromArgb( colors[colorNr].Alpha, colors[colorNr].Red, colors[colorNr].Green, colors[colorNr].Blue));
                if (colorNr < WpfColors.List.Count) { WpfColors.List[colorNr] = _color; }
                else { WpfColors.List.Add(_color); }
            }
            GlobalWinValues.UpdateWpfColors(this);
        }
    }
}