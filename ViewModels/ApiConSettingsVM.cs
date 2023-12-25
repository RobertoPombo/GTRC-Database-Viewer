using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

using GTRC_Basics;
using GTRC_Database_Viewer.Models;
using GTRC_WPF;

namespace GTRC_Database_Viewer.ViewModels
{
    public class ApiConSettingsVM : ObservableObject
    {
        private static readonly string path = GlobalValues.DataDirectory + "config dbapi.json";

        private ApiConSettings? activeConnection;
        private ApiConSettings? selectedConnection;
        private List<ApiConSettings> listConSettings = [];

        public ApiConSettingsVM()
        {
            if (!File.Exists(path)) { File.WriteAllText(path, JsonConvert.SerializeObject(ApiConSettings.List, Formatting.Indented), Encoding.Unicode); }
            RestoreSettingsCmd = new UICmd((o) => RestoreSettings());
            SaveSettingsCmd = new UICmd((o) => SaveSettings());
            AddPresetCmd = new UICmd((o) => AddPreset());
            DelPresetCmd = new UICmd((o) => DelPreset());
            RestoreSettings();
        }
        public ObservableCollection<ApiConSettings> ListConSettings
        {
            get { ObservableCollection<ApiConSettings> _list = []; foreach (ApiConSettings apiCon in ApiConSettings.List) { _list.Add(apiCon); } return _list; }
            set { }
        }

        public ObservableCollection<ProtocolType> ProtocolTypes
        {
            get { ObservableCollection<ProtocolType> _list = []; foreach (ProtocolType type in Enum.GetValues(typeof(ProtocolType))) { _list.Add(type); } return _list; }
            set { }
        }

        public ObservableCollection<NetworkType> NetworkTypes
        {
            get { ObservableCollection<NetworkType> _list = []; foreach (NetworkType type in Enum.GetValues(typeof(NetworkType))) { _list.Add(type); } return _list; }
            set { }
        }

        public ObservableCollection<IpAdressType> IpAdressTypes
        {
            get { ObservableCollection<IpAdressType> _list = []; foreach (IpAdressType type in Enum.GetValues(typeof(IpAdressType))) { _list.Add(type); } return _list; }
            set { }
        }


        public ApiConSettings? ActiveConnection { get { return activeConnection; } }

        public ApiConSettings? SelectedConnection
        {
            get { return selectedConnection; }
            set
            {
                selectedConnection = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(NetworkType));
                RaisePropertyChanged(nameof(IpAdressType));
                RaisePropertyChanged(nameof(IsActive));
                RaisePropertyChanged(nameof(VisibilityIpAdressType));
                RaisePropertyChanged(nameof(VisibilityIpv4));
                RaisePropertyChanged(nameof(VisibilityIpv6));
            }
        }

        public NetworkType NetworkType
        {
            get { return SelectedConnection?.NetworkType ?? NetworkType.IpAdress; }
            set
            {
                if (SelectedConnection is not null) { SelectedConnection.NetworkType = value; }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(VisibilityIpAdressType));
                RaisePropertyChanged(nameof(VisibilityIpv4));
                RaisePropertyChanged(nameof(VisibilityIpv6));
            }
        }

        public IpAdressType IpAdressType
        {
            get { return SelectedConnection?.IpAdressType ?? IpAdressType.IPv4; }
            set
            {
                if (SelectedConnection is not null) { SelectedConnection.IpAdressType = value; }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(VisibilityIpv4));
                RaisePropertyChanged(nameof(VisibilityIpv6));
            }
        }

        public bool IsActive
        {
            get { return SelectedConnection?.IsActive ?? false; }
            set
            {
                if (SelectedConnection is not null && value != SelectedConnection.IsActive)
                {
                    SelectedConnection.IsActive = value;
                    UpdateActiveConnection();
                    RaisePropertyChanged();
                }
            }
        }

        public Visibility VisibilityIpAdressType
        {
            get
            {
                if (SelectedConnection?.NetworkType != NetworkType.Localhost) { return Visibility.Visible; }
                else { return Visibility.Collapsed; }
            }
        }

        public Visibility VisibilityIpv4
        {
            get
            {
                if (VisibilityIpAdressType == Visibility.Visible && SelectedConnection?.IpAdressType == IpAdressType.IPv4) { return Visibility.Visible; }
                else { return Visibility.Collapsed; }
            }
        }

        public Visibility VisibilityIpv6
        {
            get
            {
                if (VisibilityIpAdressType != VisibilityIpv4) { return Visibility.Visible; }
                else { return Visibility.Collapsed; }
            }
        }

        public void UpdateActiveConnection()
        {
            ApiConSettings? nextActiveConnection = null;
            foreach (ApiConSettings apiCon in ApiConSettings.List)
            {
                if (apiCon.IsActive)
                {
                    foreach (ApiConSettings _apiCon in ApiConSettings.List)
                    {
                        if (apiCon != _apiCon) { _apiCon.IsActive = false; }
                    }
                    nextActiveConnection = apiCon;
                    break;
                }
            }
            if (nextActiveConnection != activeConnection)
            {
                activeConnection = nextActiveConnection;
            }
            if (activeConnection is null) { GlobalValues.CurrentLogText = "Not connected to GTRC-Database-API."; }
            else { GlobalValues.CurrentLogText = "Connection to GTRC-Database-API succeded."; }
        }

        public void RestoreSettings()
        {
            try
            {
                ApiConSettings.List.Clear();
                _ = JsonConvert.DeserializeObject<List<ApiConSettings>>(File.ReadAllText(path, Encoding.Unicode)) ?? [];
                GlobalValues.CurrentLogText = "GTRC-Database-API connection settings restored.";
            }
            catch { GlobalValues.CurrentLogText = "Restore GTRC-Database-API connection settings failed!"; }
            if (ApiConSettings.List.Count == 0) { _ = new ApiConSettings(); }
            RaisePropertyChanged(nameof(ListConSettings));
            UpdateActiveConnection();
            if (ActiveConnection == null) { SelectedConnection = ApiConSettings.List[0]; } else { SelectedConnection = ActiveConnection; }
        }

        public void SaveSettings()
        {
            string text = JsonConvert.SerializeObject(ApiConSettings.List, Formatting.Indented);
            File.WriteAllText(path, text, Encoding.Unicode);
            GlobalValues.CurrentLogText = "GTRC-Database-API connection settings saved.";
        }

        public void AddPreset()
        {
            ApiConSettings newCon = new();
            RaisePropertyChanged(nameof(ListConSettings));
            SelectedConnection = newCon;
        }

        public void DelPreset()
        {
            if (SelectedConnection is not null && ApiConSettings.List.Count > 1 && !SelectedConnection.IsActive)
            {
                ApiConSettings.List.Remove(SelectedConnection);
                RaisePropertyChanged(nameof(ListConSettings));
                SelectedConnection = ApiConSettings.List[0];
            }
        }

        public static event Notify? ApiConnectionEstablished;

        public static void OnApiConnectionEstablished() { ApiConnectionEstablished?.Invoke(); }

        public UICmd RestoreSettingsCmd { get; set; }
        public UICmd SaveSettingsCmd { get; set; }
        public UICmd AddPresetCmd { get; set; }
        public UICmd DelPresetCmd { get; set; }
    }
}
