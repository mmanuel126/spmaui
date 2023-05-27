using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using spmaui.Models;
using RestSharp;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace spmaui.Services
{
    public class Messages : IMessages
    {

        static readonly string MESSAGES_SERVICE_URI = App.AppSettings.WebServiceURL + "message/";
        private static readonly string COMMON_SERVICE_URI = App.AppSettings.WebServiceURL + "common/";

        private RestClient _restClient;

        public Messages()
        {
            _restClient = new RestClient(MESSAGES_SERVICE_URI);

        }

        /// <summary>
        /// Gets system notificatoins model data.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="showType"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<SystemNotificationsModel>> GetNotifications(int memberID, string showType, string jwtToken)
        {
            List<SystemNotificationsModel> lst = new List<SystemNotificationsModel>();
            var request = new RestRequest("GetNotifications", Method.Get);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            request.AddParameter("memberID", memberID);
            request.AddParameter("showType", showType);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<SystemNotificationsModel> dynJson = JsonConvert.DeserializeObject<List<SystemNotificationsModel>>(content);
            return dynJson;
        }

        /// <summary>
        /// Sends a message 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toWho"></param>
        /// <param name="sub"></param>
        /// <param name="msg"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<string> SendMessage(string from, string toWho, string sub, string msg, string jwtToken)
        {
            string resource = "CreateMessage?from=" + from + "&to=" + toWho + "&subject=" + sub + "&body=" + msg;
            var request = new RestRequest(resource, Method.Post);
            request.AddParameter("attachment", "");
            request.AddParameter("original", "");
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            return content;
        }

        /// <summary>
        /// Get members messages
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="type"></param>
        /// <param name="showType"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<MessageInfoModel>> GetMemberMessages(int memberID, string type, string showType, string jwtToken)
        {
            var request = new RestRequest("GetMemberMessages/" + memberID.ToString(), Method.Get);
            request.AddParameter("type", type);
            request.AddParameter("showType", showType);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<MessageInfoModel> dynJson = JsonConvert.DeserializeObject<List<MessageInfoModel>>(content);

            for (int i = 0; i < dynJson.Count; i++)
            {
                if (string.IsNullOrEmpty(dynJson[i].senderImage))
                {
                    dynJson[i].senderImage = App.AppSettings.AppMemberImagesURL + "default.png";
                }
                else
                {
                    dynJson[i].senderImage = App.AppSettings.AppMemberImagesURL + dynJson[i].senderImage;
                }

                if (string.IsNullOrEmpty(dynJson[i].senderTitle))
                {
                    dynJson[i].senderTitle = "Unknown Title";
                }

            }
            return dynJson;
        }


        /// <summary>
        /// Get message info by ID
        /// </summary>
        /// <param name="msgID"></param>
        /// <param name="folder"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<MessageDetails>> GetMessageInfoByID(string msgID, string folder, string jwtToken)
        {
            List<MessageInfoModel> info = new List<MessageInfoModel>();
            var request = new RestRequest("GetMessageInfoByID/" + msgID.ToString(), Method.Get);
            request.AddParameter("folder", folder);
            request.AddHeader("authorization", "Bearer " + jwtToken);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            List<MessageDetails> dynJson = JsonConvert.DeserializeObject<List<MessageDetails>>(content);

            for (int i = 0; i < dynJson.Count; i++)
            {
                if (string.IsNullOrEmpty(dynJson[i].SenderPicture))
                {
                    dynJson[i].SenderPicture = App.AppSettings.AppMemberImagesURL + "default.png";
                }
                else
                {
                    dynJson[i].SenderPicture = App.AppSettings.AppMemberImagesURL + dynJson[i].SenderPicture;
                }
            }
            return dynJson;
        }

        /// <summary>
        /// Toggle message state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="msgID"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task<string> ToggleMessageState(string state, string msgID, string folder)
        {
            var request = new RestRequest("ToggleMessageState", Method.Put);
            request.AddParameter("msgID", msgID);
            request.AddParameter("folder", folder);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            var dynJson = JsonConvert.DeserializeObject<string>(content);
            return dynJson.ToString();
        }

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="msgID"></param>
        /// <param name="folder"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<string> DeleteMessage(string msgID, string folder, string jwtToken)
        {
            var request = new RestRequest("DeleteMessage/" + msgID.ToString(), Method.Delete);
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);

            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            return content;
        }

        /// <summary>
        /// Delete notifications
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="notificationID"></param>
        /// <returns></returns>
        public async Task<string> DeleteNotifications(int memberID, string notificationID)
        {
            var request = new RestRequest("DeleteNotification", Method.Delete);
            request.AddParameter("memberID", memberID);
            request.AddParameter("notificationID", notificationID);
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            var dynJson = JsonConvert.DeserializeObject<string>(content);
            return dynJson.ToString();
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

    public interface IMessages
    {
        Task<List<SystemNotificationsModel>> GetNotifications(int memberID, string showType, string jwtToken);
        Task<string> SendMessage(string from, string toWho, string sub, string msg, string jwtToken);
        Task<List<MessageInfoModel>> GetMemberMessages(int memberID, string type, string showType, string jwtToken);
        Task<List<MessageDetails>> GetMessageInfoByID(string msgID, string folder, string jwtToken);
        Task<string> ToggleMessageState(string state, string msgID, string folder);
        Task<string> DeleteMessage(string msgID, string folder, string jwtToken);
        Task<string> DeleteNotifications(int memberID, string notificationID);
        Task LogException(string msg, string stackTrace, string jwt);
    }
}