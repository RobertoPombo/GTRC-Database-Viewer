using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

using GTRC_Basics;
using GTRC_Database_Client;
using GTRC_WPF;

namespace GTRC_Database_Viewer.ViewModels
{
    public class ClientConnectionSettingsVM : ObservableObject
    {
        private static readonly string path = GlobalValues.DataDirectory + "config dbapi.json";

        private connectionSettings? selectedConSet;

        public ClientConnectionSettingsVM()
        {
            if (!File.Exists(path)) { File.WriteAllText(path, JsonConvert.SerializeObject(connectionSettings.List, Formatting.Indented), Encoding.Unicode); }
            RestoreJsonCmd = new UICmd((o) => RestoreJson());
            SaveJsonCmd = new UICmd((o) => SaveJson());
            AddPresetCmd = new UICmd((o) => AddPreset());
            DelPresetCmd = new UICmd((o) => DelPreset());
            RestoreJson();
        }
        public ObservableCollection<connectionSettings> ListConSettings
        {
            get { ObservableCollection<connectionSettings> _list = []; foreach (connectionSettings conSet in connectionSettings.List) { _list.Add(conSet); } return _list; }
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

        public connectionSettings? SelectedConSet
        {
            get { return selectedConSet; }
            set
            {
                selectedConSet = value;
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
            get { return SelectedConSet?.NetworkType ?? NetworkType.IpAdress; }
            set
            {
                if (SelectedConSet is not null) { SelectedConSet.NetworkType = value; }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(VisibilityIpAdressType));
                RaisePropertyChanged(nameof(VisibilityIpv4));
                RaisePropertyChanged(nameof(VisibilityIpv6));
            }
        }

        public IpAdressType IpAdressType
        {
            get { return SelectedConSet?.IpAdressType ?? IpAdressType.IPv4; }
            set
            {
                if (SelectedConSet is not null) { SelectedConSet.IpAdressType = value; }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(VisibilityIpv4));
                RaisePropertyChanged(nameof(VisibilityIpv6));
            }
        }

        public bool IsActive
        {
            get { return SelectedConSet?.IsActive ?? false; }
            set
            {
                if (SelectedConSet is not null && value != SelectedConSet.IsActive)
                {
                    SelectedConSet.IsActive = value;
                    ConfirmActiveConnection();
                    RaisePropertyChanged();
                }
            }
        }

        public Visibility VisibilityIpAdressType
        {
            get
            {
                if (SelectedConSet?.NetworkType != NetworkType.Localhost) { return Visibility.Visible; }
                else { return Visibility.Collapsed; }
            }
        }

        public Visibility VisibilityIpv4
        {
            get
            {
                if (VisibilityIpAdressType == Visibility.Visible && SelectedConSet?.IpAdressType == IpAdressType.IPv4) { return Visibility.Visible; }
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

        public connectionSettings? ConfirmActiveConnection()
        {
            OnConfirmApiConnectionEstablished();
            connectionSettings? activeConSet = connectionSettings.GetActiveConnectionSettings();
            if (activeConSet is null) { GlobalValues.CurrentLogText = "Not connected to GTRC-Database-API."; }
            else { GlobalValues.CurrentLogText = "Connection to GTRC-Database-API succeded."; }
            return activeConSet;
        }

        public void RestoreJson()
        {
            try
            {
                connectionSettings.List.Clear();
                _ = JsonConvert.DeserializeObject<List<connectionSettings>>(File.ReadAllText(path, Encoding.Unicode)) ?? [];
                GlobalValues.CurrentLogText = "GTRC-Database-API connection settings restored.";
            }
            catch { GlobalValues.CurrentLogText = "Restore GTRC-Database-API connection settings failed!"; }
            if (connectionSettings.List.Count == 0) { _ = new connectionSettings(); }
            RaisePropertyChanged(nameof(ListConSettings));
            connectionSettings? activeConSet = ConfirmActiveConnection();
            if (activeConSet is null) { SelectedConSet = connectionSettings.List[0]; } else { SelectedConSet = activeConSet; }
        }

        public void SaveJson()
        {
            string text = JsonConvert.SerializeObject(connectionSettings.List, Formatting.Indented);
            File.WriteAllText(path, text, Encoding.Unicode);
            GlobalValues.CurrentLogText = "GTRC-Database-API connection settings saved.";
        }

        public void AddPreset()
        {
            connectionSettings newConSet = new();
            RaisePropertyChanged(nameof(ListConSettings));
            SelectedConSet = newConSet;
        }

        public void DelPreset()
        {
            if (SelectedConSet is not null && connectionSettings.List.Count > 1 && !SelectedConSet.IsActive)
            {
                connectionSettings.List.Remove(SelectedConSet);
                RaisePropertyChanged(nameof(ListConSettings));
                SelectedConSet = connectionSettings.List[0];
            }
        }

        public static event Notify? ConfirmApiConnectionEstablished;

        public static void OnConfirmApiConnectionEstablished() { ConfirmApiConnectionEstablished?.Invoke(); }

        public UICmd RestoreJsonCmd { get; set; }
        public UICmd SaveJsonCmd { get; set; }
        public UICmd AddPresetCmd { get; set; }
        public UICmd DelPresetCmd { get; set; }
    }
}
