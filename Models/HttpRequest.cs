using Newtonsoft.Json;
using System.Net.Http;

using GTRC_Basics;
using System.Net;

namespace GTRC_Database_Viewer.Models
{
    public class HttpRequest<ModelType>
    {
        private static readonly string s = "/";
        private static string baseUrl = "http://localhost:5000/";
        private static string modelType = typeof(ModelType).Name;

        public HttpRequest() { }
        public HttpRequest(string _baseUrl) { baseUrl = _baseUrl; }

        public async Task<Tuple<HttpStatusCode, string>> SendHttpRequest(HttpRequestType requestType, string? path = null, dynamic? objDto = null)
        {
            HttpResponseMessage? _response;
            path ??= string.Empty;
            string url = string.Join(s, [baseUrl, modelType, requestType.ToString()]);
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

        public Tuple<HttpStatusCode, ModelType?> GetObject(Tuple<HttpStatusCode, string> response)
        {
            return Tuple.Create(response.Item1, JsonConvert.DeserializeObject<ModelType>(response.Item2));
        }

        public Tuple<HttpStatusCode, List<ModelType>> GetList(Tuple<HttpStatusCode, string> response)
        {
            return Tuple.Create(response.Item1, JsonConvert.DeserializeObject<List<ModelType>>(response.Item2) ?? []);
        }

        public async Task<Tuple<HttpStatusCode, List<ModelType>>> GetAll()
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Get);
            return GetList(response);
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> GetById(int id)
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Get, s + id.ToString());
            return GetObject(response);
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> GetByUniqProps(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Get, s + "ByUniqProps", objDto);
            return GetObject(response);
        }

        public async Task<Tuple<HttpStatusCode, List<ModelType>>> GetByProps(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Get, s + "ByProps", objDto);
            return GetList(response);
        }

        public async Task<Tuple<HttpStatusCode, List<ModelType>>> GetByFilter(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Get, s + "ByFilter", objDto);
            return GetList(response);
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> GetTemp()
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Get, s + "Temp");
            return GetObject(response);
        }
        
        public async Task<Tuple<HttpStatusCode, ModelType?>> Add(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Add, objDto: objDto);
            return GetObject(response);
        }

        public async Task<HttpStatusCode> Delete(int id, bool force = false)
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Delete, s + id.ToString() + s + force.ToString());
            return response.Item1;
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> Update(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await SendHttpRequest(HttpRequestType.Update, objDto: objDto);
            return GetObject(response);
        }
    }
}
