
using System;
using RestSharp;
using Newtonsoft.Json;
using sp_maui.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;

namespace sp_maui.Services
{
    public class Connections: IConnections
    {
        private string CONTACTS_SERVICE_URI = App.AppSettings.WebServiceURL + "connection/";

        private RestClient _restClient;

        private static readonly HttpClient httpClient = new HttpClient();

        public Connections()
        {
            _restClient = new RestClient(CONTACTS_SERVICE_URI);
            if (httpClient.BaseAddress == null)
            {
                httpClient.BaseAddress = new Uri(CONTACTS_SERVICE_URI);
            }
        }

        public async Task<ObservableCollection<ContactsModel>> GetMyConnectionsList(string memberID, string jwtToken)
        {
            var request = new RestRequest("GetMemberConnections?memberID=" + memberID + "&show=", Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response =  await _restClient.GetAsync(request);
            var content = response.Content;
            ObservableCollection<ContactsModel> dynJson = JsonConvert.DeserializeObject<ObservableCollection<ContactsModel>>(content);
            return dynJson;
        }

        public async Task<List<ContactsModel>> SearchMemberConnections(string memberID, string searchText, string jwtToken)
        {
            var request = new RestRequest("SearchMemberConnections?memberID=" + memberID + "&searchText=" + searchText, Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;
        }

        public async Task<List<ContactsModel>> GetMyConnections(string memberID, string jwtToken) {

            httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            var rsp = await httpClient.GetAsync("GetMemberConnections?memberID=" + memberID + "&show=");
            var dynJson = await rsp.Content.ReadFromJsonAsync<List<ContactsModel>>();
            return dynJson;


           /* var request = new RestRequest("GetMemberConnections?memberID=" + memberID + "&show=", Method.Get);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response = await _restClient.GetAsync(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;*/
        }

        public async Task DeleteConnection(string memberID, string contactID, string jwtToken) {

            var request = new RestRequest("DeleteConnection?memberID=" + memberID + "&contactID=" + contactID, Method.Delete);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }


        public async  Task<List<ContactsModel>> GetConnectionRequests(string memberID, string jwtToken)
        {
            httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            var rsp = await httpClient.GetAsync("GetMemberConnections?memberID=" + memberID + "&show=Requests");
            var dynJson = await rsp.Content.ReadFromJsonAsync<List<ContactsModel>>();
            return dynJson;



            /*var request = new RestRequest("GetMemberConnections?memberID=" + memberID + "&show=Requests", Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response = await _restClient.GetAsync(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;*/
        }

        public async Task<List<ContactsModel>> GetConnectionSuggestions(string memberID, string jwtToken)
        {
            var request = new RestRequest("GetMemberConnectionSuggestions?memberID=" + memberID , Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<ContactsModel> dynJson = JsonConvert.DeserializeObject<List<ContactsModel>>(content);
            return dynJson;
        }

        public async Task AcceptRequest(string memberID, string contactID, string jwtToken)
        {
            var request = new RestRequest("AcceptRequest?memberID=" + memberID + "&contactID=" + contactID, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task RejectRequest(string memberID, string contactID, string jwtToken) {
            var request = new RestRequest("RejectRequest?memberID=" + memberID + "&contactID=" + contactID, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

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

        public async Task AddConnection(string memberID, string contactID, string jwtToken)
        {
            var request = new RestRequest("SendRequestConnection?memberID=" + memberID + "&contactID=" + contactID, Method.Post);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }


        public async Task<List<SearchModel>> GetSearchList(string memberID, string searchText, string jwtToken)
        {
            var request = new RestRequest("SearchResults?memberID=" + memberID + "&searchText=" + searchText, Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<SearchModel> dynJson = JsonConvert.DeserializeObject<List<SearchModel>>(content);
            return dynJson;
        }

    }


    public interface IConnections
    {
        Task<ObservableCollection<ContactsModel>> GetMyConnectionsList(string memberID, string jwtToken);
        Task<List<ContactsModel>> SearchMemberConnections(string memberID, string searchText, string jwtToken);
        Task<List<ContactsModel>> GetMyConnections(string memberID, string jwtToken);
        Task DeleteConnection(string memberID, string contactID, string jwtToken);
        Task<List<ContactsModel>> GetConnectionRequests(string memberID, string jwtToken);
        Task<List<ContactsModel>> GetConnectionSuggestions(string memberID, string jwtToken);
        Task AcceptRequest(string memberID, string contactId, string jwtToken);
        Task RejectRequest(string memberID, string contactId, string jwtToken);
        Task<List<ContactsModel>> GetSearchConnections(string memberID, string searchText, string jwtToken);
        Task AddConnection(string memberID, string contactID, string jwtToken);
        Task<List<SearchModel>> GetSearchList(string memberID, string searchText, string jwtToken);


    }
}