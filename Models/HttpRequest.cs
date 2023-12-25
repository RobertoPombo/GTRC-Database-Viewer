using Newtonsoft.Json;
using System.Net.Http;

using GTRC_Basics;

namespace GTRC_Database_Viewer.Models
{
    public class HttpRequest<ModelType>
    {
        private static readonly string s = "/";
        private static string baseUrl = "http://localhost:5000/";
        private static string modelType = typeof(ModelType).Name;

        public HttpRequest() { }
        public HttpRequest(string _baseUrl) { baseUrl = _baseUrl; }

        public async Task<Tuple<string, string>> SendHttpRequest(HttpRequestType requestType, string? path = null, dynamic? objDto = null)
        {
            HttpResponseMessage? _response;
            path ??= string.Empty;
            string url = string.Join(s, [baseUrl, modelType, requestType.ToString()]);
            Tuple<string, string> response = Tuple.Create(string.Empty, "500");
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
                        string status = _response.StatusCode.ToString();
                        string message = await _response.Content.ReadAsStringAsync();
                        response = Tuple.Create(status, message);
                    }
                }
                catch { }
            }
            return response;
        }

        public Tuple<string, ModelType?> GetObject(Tuple<string, string> response)
        {
            return Tuple.Create(response.Item1, JsonConvert.DeserializeObject<ModelType>(response.Item2));
        }

        public Tuple<string, List<ModelType>> GetList(Tuple<string, string> response)
        {
            return Tuple.Create(response.Item1, JsonConvert.DeserializeObject<List<ModelType>>(response.Item2) ?? []);
        }

        public async Task<Tuple<string, List<ModelType>>> GetAll()
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Get);
            return GetList(response);
        }

        public async Task<Tuple<string, ModelType?>> GetById(int id)
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Get, s + id.ToString());
            return GetObject(response);
        }

        public async Task<Tuple<string, ModelType?>> GetByUniqProps(dynamic objDto)
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Get, s + "ByUniqProps", objDto);
            return GetObject(response);
        }

        public async Task<Tuple<string, List<ModelType>>> GetByProps(dynamic objDto)
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Get, s + "ByProps", objDto);
            return GetList(response);
        }

        public async Task<Tuple<string, List<ModelType>>> GetByFilter(dynamic objDto)
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Get, s + "ByFilter", objDto);
            return GetList(response);
        }

        public async Task<Tuple<string, ModelType?>> GetTemp()
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Get, s + "Temp");
            return GetObject(response);
        }
        
        public async Task<Tuple<string, ModelType?>> Add(dynamic objDto)
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Add, objDto: objDto);
            return GetObject(response);
        }

        public async Task<string> Delete(int id, bool force = false)
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Delete, s + id.ToString() + s + force.ToString());
            return response.Item1;
        }

        public async Task<Tuple<string, ModelType?>> Update(dynamic objDto)
        {
            Tuple<string, string> response = await SendHttpRequest(HttpRequestType.Update, objDto: objDto);
            return GetObject(response);
        }
    }
}
