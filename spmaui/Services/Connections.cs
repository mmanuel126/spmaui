
using System;
using RestSharp;
using Newtonsoft.Json;
using spmaui.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;

namespace spmaui.Services
{
    public class Connections : IConnections
    {
        private string CONTACTS_SERVICE_URI = App.AppSettings.WebServiceURL + "connection/";
        private static readonly string COMMON_SERVICE_URI = App.AppSettings.WebServiceURL + "common/";

        private RestClient _restClient;

        private static readonly HttpClient httpClient = new HttpClient();

        private static readonly HttpClient conClient = new HttpClient();

        public Connections()
        {
            if (conClient.BaseAddress == null)
            {
                conClient.BaseAddress = new Uri(CONTACTS_SERVICE_URI);
            }

            _restClient = new RestClient(CONTACTS_SERVICE_URI);
            if (httpClient.BaseAddress == null)
            {
                httpClient.BaseAddress = new Uri(CONTACTS_SERVICE_URI);
            }
        }

        /// <summary>
        /// Get member connections list
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public ObservableCollection<ContactsModel> GetMyConnectionsList(string memberID, string jwtToken)
        {
            try
            {
                var request = new RestRequest("GetMemberConnections?memberID=" + memberID + "&show=", Method.Get);
                request.AddHeader("authorization", "Bearer " + jwtToken);
                RestResponse response = _restClient.Get(request);
                var content = response.Content;
                ObservableCollection<ContactsModel> dynJson = JsonConvert.DeserializeObject<ObservableCollection<ContactsModel>>(content);
                return dynJson;
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Search member connections.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="searchText"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<ContactsModel>> SearchMemberConnections(string memberID, string searchText, string jwtToken)
        {
            var request = new RestRequest("SearchMemberConnections?memberID=" + memberID + "&searchText=" + searchText, Method.Get);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;
        }

        /// <summary>
        /// get my connections.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<ContactsModel>> GetMyConnections(string memberID, string jwtToken)
        {

            httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);
            var rsp = await httpClient.GetAsync("GetMemberConnections?memberID=" + memberID + "&show=");
            var dynJson = await rsp.Content.ReadFromJsonAsync<List<ContactsModel>>();
            return dynJson;
        }

        /// <summary>
        /// get search result.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwtToken"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<ContactsModel> GetSearchResult(string memberID, string jwtToken, string query)
        {
            httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);
            var request = new RestRequest("GetMemberConnections?memberID=" + memberID + "&show=", Method.Get);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = _restClient.Get(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;
        }

        /// <summary>
        /// delete connection.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="contactID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task DeleteConnection(string memberID, string contactID, string jwtToken)
        {
            var request = new RestRequest("DeleteConnection?memberID=" + memberID + "&contactID=" + contactID, Method.Delete);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// get connection requests.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<ContactsModel>> GetConnectionRequests(string memberID, string jwtToken)
        {
            httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            var rsp = await httpClient.GetAsync("GetMemberConnections?memberID=" + memberID + "&show=Requests");
            var dynJson = await rsp.Content.ReadFromJsonAsync<List<ContactsModel>>();
            return dynJson;
        }

        /// <summary>
        /// get connection suggestions.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<ContactsModel>> GetConnectionSuggestions(string memberID, string jwtToken)
        {
            var request = new RestRequest("GetMemberConnectionSuggestions?memberID=" + memberID, Method.Get);
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;
        }

        /// <summary>
        /// accept request.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="contactID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task AcceptRequest(string memberID, string contactID, string jwtToken)
        {
            var request = new RestRequest("AcceptRequest?memberID=" + memberID + "&contactID=" + contactID, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// reject request.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="contactID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task RejectRequest(string memberID, string contactID, string jwtToken)
        {
            var request = new RestRequest("RejectRequest?memberID=" + memberID + "&contactID=" + contactID, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// get search connections
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="searchText"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<ContactsModel>> GetSearchConnections(string memberID, string searchText, string jwtToken)
        {
            var request = new RestRequest("GetSearchConnections?userID=" + memberID + "&searchText=" + searchText, Method.Get);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;
        }

        /// <summary>
        /// add connection.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="contactID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task AddConnection(string memberID, string contactID, string jwtToken)
        {
            var request = new RestRequest("SendRequestConnection?memberID=" + memberID + "&contactID=" + contactID, Method.Post);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// get search list
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="searchText"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public ObservableCollection<SearchModel> GetSearchList(string memberID, string searchText, string jwtToken)
        {
            var request = new RestRequest("SearchResults?memberID=" + memberID + "&searchText=" + searchText, Method.Get);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = _restClient.Execute(request);
            var content = response.Content;
            ObservableCollection<SearchModel> dynJson = JsonConvert.DeserializeObject<ObservableCollection<SearchModel>>(content);

            for (var i = 0; i < dynJson.Count; i++)
            {
                if (String.IsNullOrEmpty(dynJson[i].picturePath))
                {
                    dynJson[i].picturePath = App.AppSettings.AppMemberImagesURL + "default.png";
                }
                else
                {
                    dynJson[i].picturePath = App.AppSettings.AppMemberImagesURL + dynJson[i].picturePath;
                }
                var st = dynJson[i].Params;
                if (String.IsNullOrEmpty(dynJson[i].Params))
                {
                    dynJson[i].Params = "Unknown title";
                }
                else if (String.IsNullOrWhiteSpace(dynJson[i].Params))
                {
                    dynJson[i].Params = "Unknown title";
                }

                if (dynJson[i].labelText == "Add as Contact")
                {
                    dynJson[i].labelText = "True"; dynJson[i].ParamsAV = "False";
                }
                else
                {
                    dynJson[i].labelText = "False"; dynJson[i].ParamsAV = "True";
                }

            }
            return dynJson;
        }

        public async Task LogException(string msg, string stackTrace, string jwt)
        {
            msg = "MOBILE ERROR: " + msg; stackTrace = "MOBILE ERROR: " + stackTrace;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var requestUrl = COMMON_SERVICE_URI + "Logs?message=" + msg + "&stack=" + stackTrace;
            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response = await httpClient.PostAsync(requestUrl, requestContent);
        }
    }
    public interface IConnections
    {
        ObservableCollection<ContactsModel> GetMyConnectionsList(string memberID, string jwtToken);
        Task<List<ContactsModel>> SearchMemberConnections(string memberID, string searchText, string jwtToken);
        Task<List<ContactsModel>> GetMyConnections(string memberID, string jwtToken);
        Task DeleteConnection(string memberID, string contactID, string jwtToken);
        Task<List<ContactsModel>> GetConnectionRequests(string memberID, string jwtToken);
        Task<List<ContactsModel>> GetConnectionSuggestions(string memberID, string jwtToken);
        Task AcceptRequest(string memberID, string contactId, string jwtToken);
        Task RejectRequest(string memberID, string contactId, string jwtToken);
        Task<List<ContactsModel>> GetSearchConnections(string memberID, string searchText, string jwtToken);
        Task AddConnection(string memberID, string contactID, string jwtToken);
        ObservableCollection<SearchModel> GetSearchList(string memberID, string searchText, string jwtToken);
        Task LogException(string msg, string stackTrace, string jwt);
    }

}