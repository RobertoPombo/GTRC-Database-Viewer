using Newtonsoft.Json;
using System.Net;

using GTRC_Basics;

namespace GTRC_Database_Viewer.Models
{
    public class HttpRequest<ModelType>(ApiConSettings apiCon)
    {
        private static readonly string model = typeof(ModelType).Name;

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
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Get);
            return GetList(response);
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> GetById(int id)
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Get, "/" + id.ToString());
            return GetObject(response);
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> GetByUniqProps(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Get, "/ByUniqProps", objDto);
            return GetObject(response);
        }

        public async Task<Tuple<HttpStatusCode, List<ModelType>>> GetByProps(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Get, "/ByProps", objDto);
            return GetList(response);
        }

        public async Task<Tuple<HttpStatusCode, List<ModelType>>> GetByFilter(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Get, "/ByFilter", objDto);
            return GetList(response);
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> GetTemp()
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Get, "/Temp");
            return GetObject(response);
        }
        
        public async Task<Tuple<HttpStatusCode, ModelType?>> Add(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Add, objDto: objDto);
            return GetObject(response);
        }

        public async Task<HttpStatusCode> Delete(int id, bool force = false)
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Delete, "/" + id.ToString() + "/" + force.ToString());
            return response.Item1;
        }

        public async Task<Tuple<HttpStatusCode, ModelType?>> Update(dynamic objDto)
        {
            Tuple<HttpStatusCode, string> response = await apiCon.SendHttpRequest(model, HttpRequestType.Update, objDto: objDto);
            return GetObject(response);
        }
    }
}
