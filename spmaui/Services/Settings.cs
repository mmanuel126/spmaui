using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using sp_maui.Models;

namespace sp_maui.Services
{
    public class Settings: ISettings
    {
        private string SETTINGS_SERVICE_URI = App.AppSettings.WebServiceURL + "setting/";
        private RestClient _restClient;

        private string MEMBERS_SERVICE_URI = App.AppSettings.WebServiceURL + "member/";
        private RestClient _client;


        public Settings()
        {
            _restClient = new RestClient(SETTINGS_SERVICE_URI);
        }

        public async Task SaveMemberNameInfo(string memberId, string firstName, string middleName, string lastName, string jwtToken)
        {
            var reqUrl = "SaveMemberNameInfo/" + memberId + "?memberID=" + memberId + "&fName=" + firstName + "&mName=" + middleName + "&lName=" + lastName;
            var request = new RestRequest(reqUrl, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task SavePasswordInfo(string memberId, string password, string jwtToken)
        {
            var  postBody = new 
            {
                memberID = memberId,
                pwd = password
            };

            var request = new RestRequest("SavePasswordInfo", Method.Put);
            request.AddJsonBody(postBody);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Preferences.Set("PWD", password);
                //Application.Current.Properties["PWD"] = password;
            }
        }

        public async Task UploadImage(string memberID, MultipartFormDataContent content, string jwt)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var requestUrl = MEMBERS_SERVICE_URI + "UploadProfilePhoto/" + memberID;
            var response = await httpClient.PostAsync(requestUrl, content);
        }

        public async Task<List<AccountSettingsInfoModel>> GetMemberNameInfo(string memberID,string jwtToken)
        {
            var request = new RestRequest("GetMemberNameInfo/" + memberID, Method.Get);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<AccountSettingsInfoModel> dynJson = JsonConvert.DeserializeObject<List<AccountSettingsInfoModel>>(content);
            return dynJson;
        }

        public async Task SaveSecurityQuestionInfo(string memberID, string question, string answer, string jwt)
        {
            var request = new RestRequest("SaveSecurityQuestionInfo/" + memberID + "?questionID=" + question + "&answer=" + answer , Method.Put);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task<NotificationsSettingModel> GetMemberNotifications(string memberID, string jwt)
        {
            var request = new RestRequest("GetMemberNotifications/" + memberID, Method.Get);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<NotificationsSettingModel> dynJson = JsonConvert.DeserializeObject<List<NotificationsSettingModel>>(content);
            if (dynJson != null)
                if (dynJson.Count != 0)
                    return dynJson[0];
                else
                    return null;
            else
                return null;
        }

        public async Task SaveNotificationSettings(string memberID, NotificationsSettingModel body, string jwt)
        {
            var request = new RestRequest("SaveNotificationSettings/" + memberID, Method.Put);
            request.AddJsonBody(body);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task DeactivateAccount(string memberID, string reason, string explanation, string jwt)
        {
            bool futureEmail = false;
            string url = "DeactivateAccount/" + memberID + "?reason=" + reason + "&explanation=" + explanation + "&futureEmail=" + futureEmail;
            var request = new RestRequest(url, Method.Put);
            
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task<PrivacySettingsModel> GetProfileSettings(string memberID, string  jwt)
        {
            var request = new RestRequest("GetProfileSettings/" + memberID, Method.Get);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<PrivacySettingsModel> dynJson = JsonConvert.DeserializeObject<List<PrivacySettingsModel>>(content);

            if (dynJson != null)
                if (dynJson.Count != 0)
                    return dynJson[0];
                else
                    return null;
            else
                return null;
        }

        public async Task SaveProfileSettings(string memberID, PrivacySettingsModel body, string jwt)
        {
            var request = new RestRequest("SaveProfileSettings/" + memberID, Method.Put);
            request.AddJsonBody(body);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task SaveSearchSettings(string memberID, PrivacySettingsModel body,string jwt)
        {
            var url = "SavePrivacySearchSettings/" + memberID + "?visibility=" + body.Visibility;
            url = url + "&viewProfilePicture=" + body.ViewProfilePicture + "&viewFriendsList=" + body.ViewFriendsList;
            url = url + "&viewLinkToRequestAddingYouAsFriend=" + body.ViewLinksToRequestAddingYouAsFriend;
            url = url + "&viewLinkToSendYouMsg=" + body.ViewLinkTSendYouMsg;
            var request = new RestRequest(url, Method.Put);
            //request.AddJsonBody(body);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }
    }

    public interface ISettings
    {
        Task UploadImage(string memberID, MultipartFormDataContent content, string jwt);
        Task SaveMemberNameInfo(string memberId, string firstName, string middleName, string lastName, string jwtToken);
        Task<List<AccountSettingsInfoModel>> GetMemberNameInfo(string memberID, string jwtToken);
        Task SaveSecurityQuestionInfo(string memberID, string question, string answer, string jwt);
        Task SaveNotificationSettings(string memberID, NotificationsSettingModel body, string jwt);
        Task<PrivacySettingsModel> GetProfileSettings(string memberID, string jwt);
        Task SaveProfileSettings(string memberID, PrivacySettingsModel body, string jwt);

    }


}
