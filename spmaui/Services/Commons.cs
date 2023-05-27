using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using sp_maui.Models;

namespace sp_maui.Services
{
    public class Commons: ICommons
    {
        private string COMMONS_SERVICE_URI = App.AppSettings.WebServiceURL + "common/";
        private RestClient _restClient;

        public Commons()
        {
            _restClient = new RestClient(COMMONS_SERVICE_URI);
        }

        public async Task<List<StatesModel>> GetStates(string jwtToken)
        {
            var request = new RestRequest("GetStates", Method.Get);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            var dynJson = JsonConvert.DeserializeObject<List<StatesModel>>(content);
            return dynJson;
        }

        public async Task<List<SchoolsByStateModel>> GetSchoolsByState(string strState, string instType, string jwt)
        {
             var request = new RestRequest("GetSchoolByState?state=" + strState + "&institutionType=" + instType , Method.Get);
             request.AddHeader("authorization", "Bearer " + jwt);
             RestResponse response = await _restClient.ExecuteAsync(request);
             var content = response.Content;
             var dynJson = JsonConvert.DeserializeObject<List<SchoolsByStateModel>>(content);
            return dynJson;
        }
    }

    public interface ICommons
    {
        Task<List<StatesModel>> GetStates(string jwtToken);
        Task<List<SchoolsByStateModel>> GetSchoolsByState(string strState, string instType, string jwt);
    }
}
