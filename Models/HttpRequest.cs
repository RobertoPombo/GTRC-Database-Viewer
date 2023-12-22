using Newtonsoft.Json;
using System.Net.Http;

namespace RequestTest
{
    public class HttpRequest<ModelType>
    {
        private static string baseUrl = "http://localhost:5000/";
        private static string modelType = typeof(ModelType).Name;

        public HttpRequest() { }
        public HttpRequest(string _baseUrl) { baseUrl = _baseUrl; }

        public async Task<string> GetHttpResponse(string path)
        {
            using HttpClient httpClient = new();
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(baseUrl + path);
                    string message = await response.Content.ReadAsStringAsync();
                    return message;
                }
                catch { return ""; }
            }
        }

        public async Task<List<ModelType>> GetAllAsync()
        {
            string message = await GetHttpResponse(modelType + "/");
            List<ModelType> list = JsonConvert.DeserializeObject<List<ModelType>>(message) ?? [];
            return list;
        }


        public async Task<ModelType?> GetByIdAsync(int id)
        {
            string message = await GetHttpResponse(modelType + "/" + id.ToString());
            ModelType? obj = JsonConvert.DeserializeObject<ModelType>(message);
            return obj;
        }
    }
}
