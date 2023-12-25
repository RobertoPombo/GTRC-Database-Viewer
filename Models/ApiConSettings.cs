using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

using GTRC_Basics;

namespace GTRC_Database_Viewer.Models
{
    public class ApiConSettings
    {
        public static readonly List<ApiConSettings> List = [];

        public ApiConSettings() { List.Add(this); Name = name; }

        private string name = "Preset #1";
        private string ipv4 = "127.0.0.1";
        private string ipv6 = "0:0:0:0:0:0:0:1";
        private bool isActive = false;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                int nr = 1;
                string delimiter = " #";
                string defName = name;
                string[] defNameList = defName.Split(delimiter);
                if (defNameList.Length > 1 && Int32.TryParse(defNameList[^1], out _)) { defName = defName[..^(defNameList[^1].Length + delimiter.Length)]; }
                while (!IsUniqueName())
                {
                    name = defName + delimiter + nr.ToString();
                    nr++; if (nr == int.MaxValue) { break; }
                }
            }
        }

        public ProtocolType ProtocolType { get; set; } = ProtocolType.http;

        public NetworkType NetworkType { get; set; } = NetworkType.Localhost;

        public IpAdressType IpAdressType { get; set; } = IpAdressType.IPv4;

        public string Ipv4
        {
            get { return ipv4; }
            set { if (value.Split(".").Length == 4 && long.TryParse(value.Replace(".",""), out _)) { ipv4 = value; } }
        }

        public string Ipv6
        {
            get { return ipv6; }
            set { if (value.Split(":").Length == 8 && value.Split(":").All(i => i.Length < 5)) { ipv6 = value.ToLower(); } }
        }

        public ushort Port { get; set; } = 5000;

        public bool IsActive
        {
            get { return isActive; }
            set { if (value != isActive) { if (value) { foreach (ApiConSettings apiCon in List) { if (apiCon.IsActive) { apiCon.isActive = false; } } } isActive = value; } }
        }

        [JsonIgnore] public string BaseUrl
        {
            get
            {
                string baseUrl = string.Empty;
                if (ProtocolType != ProtocolType.None) { baseUrl += ProtocolType.ToString() + "://"; }
                if (NetworkType == NetworkType.Localhost) { baseUrl += NetworkType.ToString().ToLower(); }
                else { baseUrl += NetworkType.ToString().ToLower(); }
                return baseUrl + ":" + Port.ToString() + "/";
            }
        }

        public async Task<Tuple<HttpStatusCode, string>> SendHttpRequest(string modelTypename, HttpRequestType requestType, string? path = null, dynamic? objDto = null)
        {
            HttpResponseMessage? _response;
            path ??= string.Empty;
            string url = string.Join("/", [BaseUrl, modelTypename, requestType.ToString()]);
            Tuple<HttpStatusCode, string> response = Tuple.Create(HttpStatusCode.InternalServerError, string.Empty);
            using HttpClient httpClient = new();
            {
                try
                {
                    _response = requestType switch
                    {
                        HttpRequestType.Get => await httpClient.GetAsync(url + path),
                        HttpRequestType.Add => await httpClient.PostAsync(url + path, objDto),
                        HttpRequestType.Delete => await httpClient.DeleteAsync(url + path),
                        HttpRequestType.Update => await httpClient.DeleteAsync(url + path, objDto),
                        _ => null,
                    };
                    if (_response is not null)
                    {
                        HttpStatusCode status = _response.StatusCode;
                        string message = await _response.Content.ReadAsStringAsync();
                        response = Tuple.Create(status, message);
                    }
                }
                catch { }
            }
            return response;
        }

        public bool IsUniqueName()
        {
            int listIndexThis = List.IndexOf(this);
            for (int apiConNr = 0; apiConNr < List.Count; apiConNr++)
            {
                if (List[apiConNr].Name == name && apiConNr != listIndexThis) { return false; }
            }
            return true;
        }
    }
}
