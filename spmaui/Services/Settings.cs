using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using spmaui.Models;

namespace spmaui.Services
{
    /// <summary>
    /// implementation of the Settings service class deriving from the ISettings interface
    /// </summary>
    public class Settings: ISettings
    {
        //http api url paths
        private string SETTINGS_SERVICE_URI = App.AppSettings.WebServiceURL + "setting/";
        private static readonly string COMMON_SERVICE_URI = App.AppSettings.WebServiceURL + "common/";

        private RestClient _restClient;

        private string MEMBERS_SERVICE_URI = App.AppSettings.WebServiceURL + "member/";
        private RestClient _client;

        public Settings()
        {
            _restClient = new RestClient(SETTINGS_SERVICE_URI);
        }

        /// <summary>
        /// Save member name info.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="lastName"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task SaveMemberNameInfo(string memberId, string firstName, string middleName, string lastName, string jwtToken)
        {
            var reqUrl = "SaveMemberNameInfo/" + memberId + "?memberID=" + memberId + "&fName=" + firstName + "&mName=" + middleName + "&lName=" + lastName;
            var request = new RestRequest(reqUrl, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// save password information.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="password"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
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
            }
        }

        /// <summary>
        /// Upload image.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="content"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task UploadImage(string memberID, MultipartFormDataContent content, string jwt)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var requestUrl = MEMBERS_SERVICE_URI + "UploadProfilePhoto/" + memberID;
            var response = await httpClient.PostAsync(requestUrl, content);
        }

        /// <summary>
        /// Get member name info.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<AccountSettingsInfoModel>> GetMemberNameInfo(string memberID,string jwtToken)
        {
            var request = new RestRequest("GetMemberNameInfo/" + memberID, Method.Get);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<AccountSettingsInfoModel> dynJson = JsonConvert.DeserializeObject<List<AccountSettingsInfoModel>>(content);
            return dynJson;
        }

        /// <summary>
        /// save security questions.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task SaveSecurityQuestionInfo(string memberID, string question, string answer, string jwt)
        {
            var request = new RestRequest("SaveSecurityQuestionInfo/" + memberID + "?questionID=" + question + "&answer=" + answer , Method.Put);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// Get member notifications.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
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

        /// <summary>
        /// save notifications settings.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="body"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task SaveNotificationSettings(string memberID, NotificationsSettingModel body, string jwt)
        {
            var request = new RestRequest("SaveNotificationSettings/" + memberID, Method.Put);
            request.AddJsonBody(body);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// deactivate account.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="reason"></param>
        /// <param name="explanation"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task DeactivateAccount(string memberID, string reason, string explanation, string jwt)
        {
            bool futureEmail = false;
            string url = "DeactivateAccount/" + memberID + "?reason=" + reason + "&explanation=" + explanation + "&futureEmail=" + futureEmail;
            var request = new RestRequest(url, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// Get profile settings.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Save profile settings..
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="body"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task SaveProfileSettings(string memberID, PrivacySettingsModel body, string jwt)
        {
            var request = new RestRequest("SaveProfileSettings/" + memberID, Method.Put);
            request.AddJsonBody(body);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
        }

        /// <summary>
        /// save search settings.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="body"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task SaveSearchSettings(string memberID, PrivacySettingsModel body,string jwt)
        {
            var url = "SavePrivacySearchSettings/" + memberID + "?visibility=" + body.Visibility;
            url = url + "&viewProfilePicture=" + body.ViewProfilePicture + "&viewFriendsList=" + body.ViewFriendsList;
            url = url + "&viewLinkToRequestAddingYouAsFriend=" + body.ViewLinksToRequestAddingYouAsFriend;
            url = url + "&viewLinkToSendYouMsg=" + body.ViewLinkTSendYouMsg;
            var request = new RestRequest(url, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwt);
            RestResponse response = await _restClient.ExecuteAsync(request);
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

    /// <summary>
    /// Interfaces for the Settings class
    /// </summary>
    public interface ISettings
    {
        Task UploadImage(string memberID, MultipartFormDataContent content, string jwt);
        Task SaveMemberNameInfo(string memberId, string firstName, string middleName, string lastName, string jwtToken);
        Task<List<AccountSettingsInfoModel>> GetMemberNameInfo(string memberID, string jwtToken);
        Task SaveSecurityQuestionInfo(string memberID, string question, string answer, string jwt);
        Task SaveNotificationSettings(string memberID, NotificationsSettingModel body, string jwt);
        Task<PrivacySettingsModel> GetProfileSettings(string memberID, string jwt);
        Task SaveProfileSettings(string memberID, PrivacySettingsModel body, string jwt);
        Task<NotificationsSettingModel> GetMemberNotifications(string memberID, string jwt);
        Task SavePasswordInfo(string memberId, string password, string jwtToken);
        Task DeactivateAccount(string memberID, string reason, string explanation, string jwt);
        Task LogException(string msg, string stackTrace, string jwt);
    }

}
