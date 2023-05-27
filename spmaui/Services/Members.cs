using System;
using RestSharp;
using Newtonsoft.Json;
using sp_maui.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using sp_maui;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
//using static Android.Provider.MediaStore.Audio;
using System.Web;
//using Org.Apache.Http.Client;
//using Java.Net;

namespace sp_maui.Services
{
    public class Members: IMembers
    {

        private static readonly string MEMBERS_SERVICE_URI = App.AppSettings.WebServiceURL + "member/";
        private static readonly string ACCOUNT_SERVICE_URI = App.AppSettings.WebServiceURL + "account/";
        private static readonly string EVENT_SERVICE_URI = App.AppSettings.WebServiceURL + "event/";
        private static readonly string CONNECTION_SERVICE_URI = App.AppSettings.WebServiceURL + "connection/";
        private static readonly string COMMON_SERVICE_URI = App.AppSettings.WebServiceURL + "common/";

        private static readonly HttpClient _restClient = new HttpClient();
        private static readonly HttpClient _restActClient = new HttpClient();
        private static readonly HttpClient _restConnectionClient = new HttpClient();
        private static readonly HttpClient _restCommonClient = new HttpClient();
        private static readonly HttpClient _restEventClient = new HttpClient();

        private static  readonly RestClient _rstClient = new RestClient(MEMBERS_SERVICE_URI);
        private static readonly RestClient _rstActClient = new RestClient(ACCOUNT_SERVICE_URI);

        private static readonly HttpClient httpClient = new HttpClient();

        public Members()
        {
            

            if (_restClient.BaseAddress == null)
            {
                _restClient.BaseAddress = new Uri(MEMBERS_SERVICE_URI);
            }

            if (_restActClient.BaseAddress == null)
            {
                _restActClient.BaseAddress = new Uri(ACCOUNT_SERVICE_URI);
            }

            _restConnectionClient.BaseAddress = new Uri(CONNECTION_SERVICE_URI);
            _restCommonClient.BaseAddress = new Uri(COMMON_SERVICE_URI);
            _restEventClient.BaseAddress = new Uri(EVENT_SERVICE_URI);

            if (httpClient.BaseAddress == null)
            {
                httpClient.BaseAddress = new Uri(MEMBERS_SERVICE_URI);
            }
        }

        /// <summary>
        /// Gets the recent posts.
        /// </summary>
        /// <returns>The recent posts.</returns>
        /// <param name="memberID">Member identifier.</param>
        public async Task<List<RecentPostsModel>> GetRecentPosts(int memberID, string jwtToken)
        {
            List<RecentPostsModel> lst = new List<RecentPostsModel>();
            var request = "getRecentPosts/" + memberID.ToString();
            var response = await _restClient.GetAsync(request);
            var dynJson = await response.Content.ReadFromJsonAsync<List<RecentPostsModel>>();


           // var request = new RestRequest("getRecentPosts/" + memberID.ToString(), Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
           // request.AddHeader("authorization", "Bearer " + jwtToken);

           // RestResponse response =  await _restClient.ExecuteAsync(request);
           // var content = response.Content;
           // List<RecentPostsModel> dynJson = JsonConvert.DeserializeObject<List<RecentPostsModel>>(content);
       
            for (int i = 0; i < dynJson.Count; i++)
            {
                RecentPostsModel mp = new RecentPostsModel();
                List<RecentPostChildModel> l = await GetChildPosts(Convert.ToInt32(dynJson[i].postID), jwtToken);
                mp.ReplyCount = l.Count;

                if (l.Count != 0)
                {
                    mp.ChildItems = new List<RecentPostsModel>();
                    foreach (var ls in l)
                    {
                        var child = new RecentPostsModel();
                        child.memberName = ls.memberName;
                        child.postID = ls.postResponseID;
                        child.description = ls.description;
                        child.picturePath = ls.picturePath;
                        child.datePosted = ls.dateResponded;
                        child.IsSelected = true;
                        mp.ChildItems.Add(child);
                    }
                }

                lst.Add(mp);
            }
            return dynJson;
        }


        /// <summary>
        /// Gets the child posts.
        /// </summary>
        /// <returns>The child posts.</returns>
        /// <param name="postID">Post identifier.</param>
        public async Task<List<RecentPostChildModel>> GetChildPosts(int postID, string jwtToken)
        {
            List<RecentPostChildModel> lst = new List<RecentPostChildModel>();
            var request = "getRecentPostResponses / " + postID.ToString();

            _restClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _restClient.GetAsync(request);
            var dynJson = await response.Content.ReadFromJsonAsync<List<RecentPostChildModel>>();


            //List<RecentPostChildModel> lst = new List<RecentPostChildModel>();
            //var request = new RestRequest("getRecentPostResponses/" + postID.ToString(), Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);

            //RestResponse response =  await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //List<RecentPostChildModel> dynJson = JsonConvert.DeserializeObject<List<RecentPostChildModel>>(content);
            return dynJson;
        }

        /// <summary>
        /// Authenticates the LGU ser.
        /// </summary>
        /// <returns>The LGU ser.</returns>
        /// <param name="username">Email.</param>
        /// <param name="pwd">Pwd.</param>
        /// <param name="rememberMe">Remember me.</param>
        /// <param name="screen">Screen.</param>
        public async Task<UserModel> AuthenticateLGUser(string username, string pwd, string rememberMe, string screen)
        {
            

            var body = new LoginModel
            {
                email = username,
                password = pwd
            };
 
            var response = await _restActClient.PostAsJsonAsync("login", body);
            var dynJson = await response.Content.ReadFromJsonAsync<UserModel>();
            return dynJson;


            //var request = new RestRequest("login", Method.Post);
            ////request.AddHeader("Content-Type", "application/json");
            //request.RequestFormat = DataFormat.Json;
            //request.AddJsonBody(new LoginModel
            //{
            //    email = username,
            //    password = pwd
            //});

            //RestResponse response = await _restActClient.ExecuteAsync(request);
            //var content = response.Content;

            //UserModel dynJson = JsonConvert.DeserializeObject<UserModel>(content);
            //return dynJson;
        }

        public async Task<UserModel> RefreshToken(string jwtToken)
        {
            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response = await _restClient.PostAsync("refreshLogin?accessToken=" + jwtToken, requestContent);
            var dynJson = await response.Content.ReadFromJsonAsync<UserModel>();
            return dynJson;

            //var request = new RestRequest("refreshLogin?accessToken=" + jwtToken, Method.Post);
            //RestResponse response = await _restActClient.ExecuteAsync(request);
            //var content = response.Content;

            //UserModel dynJson = JsonConvert.DeserializeObject<UserModel>(content);
            //return dynJson;
        }

        /// <summary>
        /// Registers the user to lg.
        /// </summary>
        /// <returns>existing or newemail string.</returns>
        /// <param name="register">Register.</param>
        public async Task<string> RegisterUser(RegisterModel register)
        {
            //var response = await _restClient.PostAsJsonAsync("register", register);
            // var result = await response.Content.ReadAsStringAsync();
            // return result;

            RestClient _restClient = new RestClient(ACCOUNT_SERVICE_URI);



            var request = new RestRequest("register", Method.Post);
            //request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(register); register.confirmPwd = "";register.code = "";
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;

            var result = JsonConvert.DeserializeObject<string>(content).ToString();
            // return dynJson.ToString();
            return result;
        }

        /// <summary>
        /// Gets the recent news.
        /// </summary>
        /// <returns>The recent news.</returns>
        public async Task<List<RecentNewsModel>> GetRecentNews(string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _restClient.GetAsync("getRecentNews");
            var dynJson = await response.Content.ReadFromJsonAsync<List<RecentNewsModel>>();
            return dynJson;

            //var request = new RestRequest("getRecentNews", Method.Get);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //var res = await _restClient.ExecuteAsync<List<RecentNewsModel>>(request);
            //return res.Data;
        }

        
        public async Task<List<YoutubePlayListModel>> GetMemberPlaylists(string memberID, string jwtToken)
        {
            try
            {
                _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await _restClient.GetAsync("GetVideoPlayList/" + memberID);
                var dynJson = await response.Content.ReadFromJsonAsync<List<YoutubePlayListModel>>();
                return dynJson;

                //var request = new RestRequest("GetVideoPlayList/" + memberID, Method.Get);
                ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
                //request.AddHeader("authorization", "Bearer " + jwtToken);

                //var response = await _restClient.ExecuteAsync(request);
                //var content = response.Content;
                //var dynJson = JsonConvert.DeserializeObject<List<YoutubePlayListModel>>(content);
                //return dynJson;
            }
            catch (Exception e)
            {
                string x = e.Message; return null;
            }
            //var res = await _restClient.ExecuteAsync<List<YoutubePlayListModel>>(request);


            //return res.Data;
        }


        public async Task<List<YoutubeVideosListModel>> GetPlaylistVideos(string playListID, string jwtToken)
        {
            try
            {
                _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await _restClient.GetAsync("GetVideosList/" + playListID);
                var dynJson = await response.Content.ReadFromJsonAsync<List<YoutubeVideosListModel>>();

                //var request = new RestRequest("GetVideosList/" + playListID, Method.Get);
                //request.AddHeader("Content-Type", "application/json; charset=utf-8");
                //request.AddHeader("authorization", "Bearer " + jwtToken);

                //var response = await _restClient.ExecuteAsync(request);
                //var content = response.Content;
                //var dynJson = JsonConvert.DeserializeObject<List<YoutubeVideosListModel>>(content);
                return dynJson;
            }
            catch (Exception e)
            {
                string x = e.Message; return null;
            }
        }


        /// <summary>
        /// Gets the member basic info.
        /// </summary>
        /// <returns>The member basic info.</returns>
        /// <param name="memberID">Member identifier.</param>
        public async Task<MemberProfileBasicInfoModel> GetMemberBasicInfo(int memberID, string jwtToken)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization
                            = new AuthenticationHeaderValue("Bearer", jwtToken);

                var rsp = await httpClient.GetAsync("GetMemberGeneralInfoV2/" + memberID.ToString());
                var dynJson = await rsp.Content.ReadFromJsonAsync<MemberProfileBasicInfoModel>();
                //return dynJson;

                /*
                _restClient.DefaultRequestHeaders.Authorization
                           = new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await _restClient.GetAsync("GetMemberGeneralInfoV2/" + memberID.ToString());
                MemberProfileBasicInfoModel dynJson = await response.Content.ReadFromJsonAsync<MemberProfileBasicInfoModel>();
                */

                //MemberProfileBasicInfoModel lst = new MemberProfileBasicInfoModel();
                //var request = new RestRequest("GetMemberGeneralInfoV2/" + memberID.ToString(), Method.Get);
                ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
                //request.AddHeader("authorization", "Bearer " + jwtToken);

                //RestResponse response =  await _restClient.ExecuteAsync(request);
                //var content = response.Content;
                //MemberProfileBasicInfoModel dynJson = JsonConvert.DeserializeObject<MemberProfileBasicInfoModel>(content);

                dynJson.memberProfileTitle = dynJson.CurrentStatus;
                dynJson.memProfileName = dynJson.FirstName + " " + dynJson.MiddleName + " " + dynJson.LastName;


                if (String.IsNullOrEmpty(dynJson.memProfileImage))
                {
                    dynJson.memProfileImage = App.AppSettings.AppImagesURL + "images/members/default.png";
                }
                else
                {
                    dynJson.memProfileImage = App.AppSettings.AppImagesURL + "/" + dynJson.memProfileImage;
                }

                dynJson.memProfileDOB = dynJson.DOBMonth + "/" + dynJson.DOBDay + "/" + dynJson.DOBYear;


                string str = "";
                if (dynJson.LookingForEmployment)
                    str = "Employment, ";

                if (dynJson.LookingForNetworking)
                    str += "Networking, ";

                if (dynJson.LookingForPartnership)
                    str += "Partnership, ";

                if (dynJson.LookingForRecruitment)
                    str += "Recruitment, ";

                dynJson.memProfileLookingFor = str.TrimEnd().TrimEnd(',');

                if (dynJson.InterestedInType == "8")
                    dynJson.InterestedDesc = "Athletes and Sports";
                else if (dynJson.InterestedInType == "9")
                    dynJson.InterestedDesc = "Athletic Trainers";
                else if (dynJson.InterestedInType == "39")
                    dynJson.InterestedDesc = "Exercise Physiologists";
                else if (dynJson.InterestedInType == "43")
                    dynJson.InterestedDesc = "Fitness Entrepreneur";
                else if (dynJson.InterestedInType == "90")
                    dynJson.InterestedDesc = "Recreation Leader";
                else if (dynJson.InterestedInType == "101")
                    dynJson.InterestedDesc = "Sports Announcers";
                else if (dynJson.InterestedInType == "102")
                    dynJson.InterestedDesc = "Sports Coaches and Teachers";
                else if (dynJson.InterestedInType == "103")
                    dynJson.InterestedDesc = "Sportscaster";


                //get youtube channel id
                //MemberProfileBasicInfoModel lst = new MemberProfileBasicInfoModel();
                //var request2 = new RestRequest("GetYoutubeChannel/" + memberID.ToString(), Method.Get);
                //request2.AddHeader("Content-Type", "application/json; charset=utf-8");
                //request2.AddHeader("authorization", "Bearer " + jwtToken);
                
                
                var response2 = await _restClient.GetAsync("GetYoutubeChannel/" + memberID.ToString());
                var result2 = await response2.Content.ReadAsStringAsync();
              


                //RestResponse response2 = await _restClient.ExecuteAsync(request2);
                //var content2 = response2.Content;

                //var result2 = JsonConvert.DeserializeObject<string>(content2).ToString();

                var channelID = "";

                if (result2 != null)
                    channelID = result2;
                dynJson.ChannelID = channelID;

                return dynJson;
            }
            catch (Exception x)
            {
                var msg = x.Message;
                return null; 
            }
        }

        /// <summary>
        /// Gets the registered members.
        /// </summary>
        /// <returns>The registered members.</returns>
        /// <param name="status">Status.</param>
        public async Task<List<RegisteredMembersModel>> GetRegisteredMembers(int status)
        {
            var response = await _restClient.GetAsync("GetRegisteredMembers?status=" + status);
            var dynJson = await response.Content.ReadFromJsonAsync<List<RegisteredMembersModel>>();
            return dynJson;

            //var request = new RestRequest("GetRegisteredMembers", Method.Get);
            //request.AddParameter("status", status);
            //var res = await _restClient.ExecuteAsync<List<RegisteredMembersModel>>(request);
            //return res.Data;
        }

        /// <summary>
        /// Saves the posts.
        /// </summary>
        /// <returns>The posts.</returns>
        /// <param name="memberID">Member identifier.</param>
        /// <param name="postID">Post identifier.</param>
        /// <param name="postMsg">Post message.</param>
        public async Task<string> SavePosts(int memberID, int postID, string postMsg, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);
            string resource = "SavePosts/" + memberID + "/" + postID + "?postMsg=" + postMsg;
            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response = await _restClient.PostAsync(resource, requestContent);
            var result = await response.Content.ReadAsStringAsync();
            return result;

            //string resource = "SavePosts/" + memberID + "/" + postID + "?postMsg=" + postMsg;
            //var request = new RestRequest(resource, Method.Post);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //request.RequestFormat = DataFormat.Json;
            //request.AddJsonBody(new PostModel
            //{
            //    memberID = memberID.ToString(),
            //    postID = postID.ToString(),
            //    postMsg = postMsg,
            //});
            //RestResponse response =  await _restClient.ExecuteAsync(request);
            //return response.Content;
        }

        /// <summary>
        /// Gets the member contact info.
        /// </summary>
        /// <returns>The member contact info.</returns>
        /// <param name="memberID">Member identifier.</param>
        public async Task<MemberProfileContactInfoModel> GetMemberContactInfo(int memberID, string jwtToken)
        {
            try
            {
                _restClient.DefaultRequestHeaders.Authorization
                             = new AuthenticationHeaderValue("Bearer", jwtToken);

                string resource = "GetMemberContactInfo/" + memberID.ToString();
                var response = await _restClient.GetAsync(resource);
                var dynJson = await response.Content.ReadFromJsonAsync<MemberProfileContactInfoModel>();
                return dynJson;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }

            //var requestContent = new StringContent("Encoding.UTF8, application/json");
            //var response = await _restClient.PostAsync(resource, requestContent);

            //// List<MemberProfileContactInfoModel> dat = new List<MemberProfileContactInfoModel>();

            //var request = new RestRequest("GetMemberContactInfo/" + memberID.ToString(), Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);


            //RestResponse response = await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //var data = JsonConvert.DeserializeObject<MemberProfileContactInfoModel>(content);
            //return data;
        }

        /// <summary>
        /// Gets the member employment info.
        /// </summary>
        /// <returns>The member employment info.</returns>
        /// <param name="memberID">Member identifier.</param>
        public async Task<List<MemberProfileEmploymentModel>> GetMemberEmploymentInfo(int memberID, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            string resource = "GetMemberEmploymentInfo/" + memberID.ToString();
            var response = await _restClient.GetAsync(resource);
            var dynJson = await response.Content.ReadFromJsonAsync<List<MemberProfileEmploymentModel>>();
            return dynJson;

            //List<MemberProfileEmploymentModel> dat = new List<MemberProfileEmploymentModel>();
            //var request = new RestRequest("GetMemberEmploymentInfo/" + memberID.ToString(), Method.Get);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);

            //RestResponse response =  await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //var res = await _restClient.ExecuteAsync<List<MemberProfileEmploymentModel>>(request);
            //return res.Data;
        }

        /// <summary>
        /// Gets the member education info.
        /// </summary>
        /// <returns>The member education info.</returns>
        /// <param name="memberID">Member identifier.</param>
        public async Task<List<MemberProfileEducationModel>> GetMemberEducationInfo(int memberID, string jwtToken)
        {
            //List<MemberProfileEducationModel> dat = new List<MemberProfileEducationModel>();

            _restClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            string resource = "GetMemberEducationInfo/" + memberID.ToString();
            var response = await _restClient.GetAsync(resource);
            var res = await response.Content.ReadFromJsonAsync<List<MemberProfileEducationModel>>();
            

            //var request = new RestRequest("GetMemberEducationInfo/" + memberID.ToString(), Method.Get);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //var res = await _restClient.ExecuteAsync<List<MemberProfileEducationModel>>(request);

            //dat = (List<MemberProfileEducationModel>)res.Data;

            for (var i = 0; i < res.Count-1; i++)
            {
               // MemberProfileEducationModel d = new MemberProfileEducationModel();
                //d = res.Data[i];
                //    //  foreach (var d in dat)
                //    //{
                //    if (d.schoolImage == null)
                //    {
                //        d.schoolWebSite = "";
                //    }

                //    if (d.schoolWebSite.IndexOf("http") == -1)
                //    {
                //        d.schoolWebSite = "http://" + d.schoolWebSite;
                //    }

               


                if (res[i].schoolImage != null)
                {
                    if (res[i].schoolImage != "")
                    {
                        res[i].schoolWebSite = res[i].schoolImage;

                        if (res[i].schoolWebSite.IndexOf("http") == -1)
                        {
                            res[i].schoolWebSite = "http://" + res[i].schoolWebSite;
                        }

                        res[i].schoolImage = "https://www.google.com/s2/favicons?domain=" + res[i].schoolImage.ToString();
                    }
                    else
                    {
                       res[i].schoolImage = App.AppSettings.AppImagesURL + "images/members/default.png";
                    }
                }

                if (res[i].yearClass == null)
                {
                    res[i].yearClass = "";
                }

                if (res[i].major != null)
                {
                    res[i].major = res[i].major + " - " + res[i].yearClass;
                }

               // dat.Add(d);
            }
            return res;

        }

        /// <summary>
        /// Gets the member about info.
        /// </summary>
        /// <returns>The member about info.</returns>
        /// <param name="memberID">Member identifier.</param>
        public async Task<List<MemberProfileAboutModel>> GetMemberAboutInfo(int memberID, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwtToken);

            string resource = "GetMemberPersonalInfo / " + memberID.ToString();
            var response = await _restClient.GetAsync(resource);
            var res = await response.Content.ReadFromJsonAsync<List<MemberProfileAboutModel>>();
            return res;
            //List<MemberProfileAboutModel> dat = new List<MemberProfileAboutModel>();
            //var request = new RestRequest("GetMemberPersonalInfo/" + memberID.ToString(), Method.Get);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //var res = await _restClient.ExecuteAsync<List<MemberProfileAboutModel>>(request);
            //return res.Data;
        }

        /// <summary>
        /// Gets my contacts.
        /// </summary>
        /// <returns>The my contacts.</returns>
        /// <param name="memberID">Member identifier.</param>
        /// <param name="show">Show.</param>
        public async Task<List<ContactsModel>> GetMyContacts(int memberID, string show, string jwtToken)
        {

            _restConnectionClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwtToken);

            string resource = "GetMemberConnections?memberID=" + memberID + "&show=" + show;
            var response = await _restConnectionClient.GetAsync(resource);
            var res = await response.Content.ReadFromJsonAsync<List<ContactsModel>>();
            return res;



            //List<ContactsModel> dat = new List<ContactsModel>();


           
            //var request = new RestRequest("GetMemberConnections", Method.Get);
            //request.AddParameter("memberID", memberID);
            //request.AddParameter("show", show);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //var res =   await _restConnectionClient.ExecuteAsync<List<ContactsModel>>(request);
            //return res.Data;
        }

        /// <summary>
        /// Gets the member events info.
        /// </summary>
        /// <returns>The member events info.</returns>
        /// <param name="memberID">Member identifier.</param>
        //public async Task<List<EventsModel>> GetMemberEventsInfo(int memberID, string jwtToken)
        //{
        //    List<EventsModel> dat = new List<EventsModel>();

        //    var request = new RestRequest("GetMemberEvents/" + memberID.ToString(), Method.GET);
        //    request.AddHeader("Content-Type", "application/json; charset=utf-8");
        //    request.AddHeader("authorization", "Bearer " + jwtToken);
        //    var res = await _restEventClient.ExecuteAsync<List<EventsModel>>(request);
        //    return res.Data;
        //}

        /// <summary>
        /// Updates the profile picture.
        /// </summary>
        /// <returns>The profile picture.</returns>
        /// <param name="memberID">Member identifier.</param>
        /// <param name="fileName">File name.</param>
        public async Task<string> UpdateProfilePicture(int memberID, string fileName)
        {
            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response2 = await _restClient.PutAsync ("UploadProfilePhoto?memberID=" + memberID.ToString() + "&fileName=" + fileName, requestContent);
            var result = await response2.Content.ReadAsStringAsync();
            return result;

            //var request = new RestRequest("UploadProfilePhoto", Method.Put);
            //request.AddParameter("memberID", memberID);
            //request.AddParameter("fileName", fileName);

            //RestResponse response =  await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //return content.ToString();
        }

        public async Task<string> IsFriendByContactID(string memberID, string contactID, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwtToken);

            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response2 = await _restClient.GetAsync("IsFriendByContactID/" + memberID.ToString() + "/" + contactID.ToString());
            var result = await response2.Content.ReadAsStringAsync();
            return result;



            //var request = new RestRequest("IsFriendByContactID/" + memberID.ToString() + "/" + contactID.ToString(), Method.Get);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);

            //var response =   await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //return content.ToString();
        }

        public async Task <string> IsContactRequestSent(string memberID, string contactID, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwtToken);

            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response2 = await _restClient.GetAsync("IsContactRequestSent" + memberID.ToString() + "/" + contactID.ToString());
            var result = await response2.Content.ReadAsStringAsync();
            return result;


            //var request = new RestRequest("IsContactRequestSent" + memberID.ToString() + "/" + contactID.ToString(), Method.Get);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);

            //var response =   await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //return content.ToString();
        }

        public async Task SaveMemberGeneralInfo(string memberID, MemberProfileBasicInfoModel basicInfo, string jwtToken)
        {
            if (basicInfo.InterestedDesc == "Athletes and Sports")
                basicInfo.InterestedInType = "8";
            else if (basicInfo.InterestedDesc == "Athletic Trainers")
                basicInfo.InterestedInType = "9";
            else if (basicInfo.InterestedDesc == "Exercise Physiologists")
                basicInfo.InterestedInType = "39";
            else if (basicInfo.InterestedDesc == "Fitness Entrepreneur")
                basicInfo.InterestedInType = "43";
            else if (basicInfo.InterestedDesc == "Recreation Leader")
                basicInfo.InterestedInType = "90";
            else if (basicInfo.InterestedDesc == "Sports Announcers")
                basicInfo.InterestedInType = "101";
            else if (basicInfo.InterestedDesc == "Sports Coaches and Teachers")
                basicInfo.InterestedInType = "102";
            else if (basicInfo.InterestedDesc == "Sportscaster")
                basicInfo.InterestedInType = "103";

            //var request = new RestRequest("SaveMemberGeneralInfo/" + memberID.ToString(), Method.Post);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");

            //request.AddHeader("Content-Type", "application/json; charset=utf-8");

            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //request.RequestFormat = DataFormat.Json;

            SaveMemberProfileGenInfoModel body = new SaveMemberProfileGenInfoModel();
            body.FirstName = basicInfo.FirstName;
            body.MiddleName = basicInfo.MiddleName;
            body.LastName = basicInfo.LastName;
            body.TitleDesc = basicInfo.TitleDesc;
            body.CurrentStatus = basicInfo.CurrentStatus;
            body.Sport = basicInfo.Sport;
            body.PreferredPosition = basicInfo.PreferredPosition;
            body.SecondaryPosition = basicInfo.SecondaryPosition;
            body.LeftRightHandFoot = basicInfo.LeftRightHandFoot;
            body.Height = basicInfo.Height;
            body.Weight = basicInfo.Weight;
            body.Sex = basicInfo.Sex;
            if (basicInfo.ShowSexInProfile)
                body.ShowSexInProfile = true;
            else
                body.ShowSexInProfile = false;
            body.InterestedInType = basicInfo.InterestedInType;
            body.InterestedDesc = basicInfo.InterestedDesc;
            body.Bio = basicInfo.Bio;
            body.DOBDay = basicInfo.DOBDay;
            body.DOBMonth = basicInfo.DOBMonth;
            body.DOBYear = basicInfo.DOBYear;
            if (basicInfo.LookingForEmployment)
              body.LookingForEmployment = true;
            else
                body.LookingForEmployment = false;

            if (basicInfo.LookingForNetworking)
                body.LookingForNetworking = true;
            else
                body.LookingForNetworking = false;

            if (basicInfo.LookingForPartnership)
                body.LookingForPartnership = true;
            else
                body.LookingForPartnership = false;

            if(basicInfo.LookingForRecruitment)
                body.LookingForRecruitment = true;
            else
                body.LookingForRecruitment = false;   

           // request.AddJsonBody(body);

            _restClient.DefaultRequestHeaders.Authorization
                      = new AuthenticationHeaderValue("Bearer", jwtToken);
            await _restClient.PostAsJsonAsync("SaveMemberGeneralInfo/" + memberID.ToString(), body);

           //await _restClient.ExecuteAsync(request);
           
        }

        public async Task SaveMemberContactInfo(string memberID, MemberProfileContactInfoModel contactInfo, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                     = new AuthenticationHeaderValue("Bearer", jwtToken);

            var builder = new UriBuilder("SaveMemberContactInfoV2 / " + memberID.ToString());
            var query = HttpUtility.ParseQueryString(builder.Query);
          
            query["Address"]=contactInfo.Address;
            query["CellPhone"]= contactInfo.CellPhone;
            query["City"] = contactInfo.City;
            query["Email"] = contactInfo.Email;
            query["OtherEmail"]= contactInfo.OtherEmail;
            query["Facebook"]= contactInfo.Facebook;
            query["HomePhone"]=  contactInfo.HomePhone;
            query["Instagram"]= contactInfo.Instagram;
            query["Neighborhood"] = contactInfo.Neighborhood;
          /*  query["ShowAddress"] = contactInfo.ShowAddress;
            query["ShowCellPhone"]= contactInfo.ShowCellPhone;
            query["ShowEmailToMembers"]= contactInfo.ShowEmailToMembers;
            query["SbowHomePhone"]= contactInfo.ShowHomePhone;*/
            query["State"]= contactInfo.State;
            query["Twitter"]= contactInfo.Twitter;
            query["Website"]= contactInfo.Website;
            query["Zip"]= contactInfo.Zip;

            builder.Query = query.ToString();
            string url = builder.ToString();
            var content = new StringContent("Encoding.UTF8, application/json");
            await _restClient.PostAsync(url,content);


            //var request = new RestRequest("SaveMemberContactInfoV2/" + memberID.ToString(), Method.Post);
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //query["Address", contactInfo.Address);
            //query["CellPhone", contactInfo.CellPhone);
            //query["City", contactInfo.City);
            //query["Email", contactInfo.Email);
            //query["OtherEmail", contactInfo.OtherEmail);
            //query["Facebook", contactInfo.Facebook);
            //query["HomePhone", contactInfo.HomePhone);
            //query["Instagram", contactInfo.Instagram);
            //query["Neighborhood", contactInfo.Neighborhood);
            //query["ShowAddress", contactInfo.ShowAddress);
            //query["ShowCellPhone", contactInfo.ShowCellPhone);
            //query["ShowEmailToMembers", contactInfo.ShowEmailToMembers);
            //query["SbowHomePhone", contactInfo.ShowHomePhone);
            //query["State", contactInfo.State);
            //query["Twitter", contactInfo.Twitter);
            //query["Website", contactInfo.Website);
            //query["Zip", contactInfo.Zip);

            //RestResponse response = await _restClient.ExecuteAsync (request);
            //var content = response.Content;

            /*


            var request = new RestRequest("SaveMemberContactInfo/" + memberID.ToString(), Method.Post);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwtToken);
            //request.RequestFormat = DataFormat.Json;

            var body = new SaveMemberProfileContactInfoModel
            {
                address = contactInfo.Address,
                cellPhone = contactInfo.CellPhone,
                city = contactInfo.City,
                email  = contactInfo.Email,
                otherEmail = contactInfo.OtherEmail,
                facebook = contactInfo.Facebook,
                homePhone = contactInfo.HomePhone,
                instagram = contactInfo.Instagram,
                neighborhood = contactInfo.Neighborhood,
                showAddress = contactInfo.ShowAddress,
                showCellPhone = contactInfo.ShowCellPhone,
                showEmailToMembers = contactInfo.ShowEmailToMembers,
                showHomePhone = contactInfo.ShowHomePhone,
                state = contactInfo.State,
                twitter = contactInfo.Twitter,
                website = contactInfo.Website,
                zip = contactInfo.Zip

            };
            var b = JsonConvert.SerializeObject(body);

            request.AddJsonBody(body);
            RestResponse response = await _restClient.ExecuteAsync(request);
            */
            /*
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.RequestFormat = DataFormat.Json;
           // request.AddJsonBody(contactInfo);

            var json = request.JsonSerializer.Serialize(contactInfo);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //var res = await _restClient.ExecuteAsync(request); */
        }

        public async Task SaveMemberAboutData(string memberID, string aboutMe, string activities, string hobbies, string specialSkills, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", jwtToken);

            var request = "SaveMemberPersonalInfo?memberID=" + memberID.ToString() + "&activities=" + activities + " &interests=" + hobbies + " &specialSkills=" + specialSkills + " &aboutMe=" + aboutMe;
            var content = new StringContent("Encoding.UTF8, application/json");
            await _restClient.PostAsync(request, content);

            //var request = new RestRequest("SaveMemberPersonalInfo?memberID=" + memberID.ToString() + "&activities=" + activities + " &interests=" + hobbies + " &specialSkills=" + specialSkills + " &aboutMe=" + aboutMe, Method.Post);
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //RestResponse response =  await _restClient.ExecuteAsync(request);
        }

        public async Task<string> ResetPassword(string email, string jwt)
        {
           // _restClient.DefaultRequestHeaders.Authorization
           //        = new AuthenticationHeaderValue("Bearer", jwt);

            var request = new RestRequest("ResetPassword?email = " + email,Method.Get) ;
            //var response = await _restClient.GetAsync(request);
            //var result = await response.Content.ReadAsStringAsync();
            //return result;

            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.AddHeader("authorization", "Bearer " + jwt);

            RestResponse response = await _rstClient.ExecuteAsync(request);
            var content = response.Content;
            return content.ToString();
        }

        public async Task<string> IsResetCodeExpired(string code, string jwt)
        {
            _restClient.DefaultRequestHeaders.Authorization
                      = new AuthenticationHeaderValue("Bearer", jwt);

            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response2 = await _restClient.GetAsync("IsResetCodeExpired?code=" + code);
            var result = await response2.Content.ReadAsStringAsync();
            return result;



            //var request = new RestRequest("IsResetCodeExpired?code=" + code, Method.Get);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwt);

            //RestResponse response = await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //return content.ToString();
        }

        public async Task<string> ChangePassword(RegisterModel model, string jwt)
        {
            _restClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwt);
            var request = "ChangePassword?pwd=" + model.confirmPwd + "&email=" + model.email + "&code=" + model.code;

            var response = await _restClient.GetAsync(request);
            var dynJson = await response.Content.ReadAsStringAsync();
            return dynJson;

            //var request = new RestRequest("ChangePassword?pwd=" + model.confirmPwd + "&email=" + model.email + "&code=" + model.code,
            //    Method.Get);
            //request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwt);

            //RestResponse response = await _restClient.ExecuteAsync(request);
            //var content = response.Content;
            //return content.ToString();
        }

        public async Task<string> SetMemberStatus(string MemberId, string status,string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwtToken);
            var request = "SetMemberStatus?memberId=" + MemberId.ToString() + "&status=" + status;
            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response = await _restClient.PostAsync(request, requestContent);
            var dynJson = await response.Content.ReadAsStringAsync();
            return dynJson;

            //var request = new RestRequest("SetMemberStatus?memberId=" + MemberId.ToString() + "&status=" + status , Method.Post);
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //RestResponse response = await _restClient.ExecuteAsync(request);

            //var content = response.Content;
            //return content.ToString();
        }

        public async Task<List<Item>>  GetSportsList(string jwtToken)
        {
            _restConnectionClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _restConnectionClient.GetAsync("GetSportsList");
            var dynJson = await response.Content.ReadFromJsonAsync<List<Item>>();
            return dynJson;


            //var request = new RestRequest("GetSportsList", Method.Get);

            ////request.AddParameter("memberID", memberID);
            ////request.AddParameter("show", show);
            ////request.AddHeader("Content-Type", "application/json; charset=utf-8");
            //request.AddHeader("authorization", "Bearer " + jwtToken);
            //var res = await _restConnectionClient.ExecuteAsync<List<Item>>(request);
            //return res.Data;
        }

        public async Task AddNewSchool(string memberId, MemberProfileEducationModel body, string jwt)
        {
            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.PostAsJsonAsync("AddMemberSchool/" + memberId.ToString(), body);

            //var request = new RestRequest("AddMemberSchool/" + memberId, Method.Post);
            //request.AddHeader("authorization", "Bearer " + jwt);
            //request.RequestFormat = DataFormat.Json;
            //request.AddJsonBody(body);
            //RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task UpdateSchool( string memberId, MemberProfileEducationModel body, string jwt)
        {
            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.PostAsJsonAsync("UpdateMemberSchool/" + memberId.ToString(), body);

            //var request = new RestRequest("UpdateMemberSchool/"+ memberId, Method.Put);
            //request.AddHeader("authorization", "Bearer " + jwt);
            //request.RequestFormat = DataFormat.Json;
            //request.AddJsonBody(body);
            //RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task RemoveSchool(string memberId, string schoolId, string  instType, string jwt)
        {
            _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.DeleteAsync("RemoveMemberSchool?memberID=" + memberId + "&instID=" + schoolId + "&instType=" + instType);


            //var request = new RestRequest("RemoveMemberSchool?memberID=" + memberId + "&instID=" + schoolId + "&instType=" + instType, Method.Delete);
            //request.AddHeader("authorization", "Bearer " + jwt);
            //request.RequestFormat = DataFormat.Json;
            //RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task SaveChannelID(string memberID, string channelID, string jwt)
        {
            var body = new YoutubeDataModel
            {
                memberID = memberID,
                channelID = channelID
            };

            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.PutAsJsonAsync("SetYoutubeChannel/" + memberID.ToString(), body);

            //var request = new RestRequest("SetYoutubeChannel", Method.Put);
            //request.AddHeader("authorization", "Bearer " + jwt);
            //request.RequestFormat = DataFormat.Json;
            //request.AddJsonBody(body);
            //RestResponse response = await _restClient.ExecuteAsync(request);

        }

        public async Task SaveInstagramURL(string memberID, string instagramURL, string jwt)
        {
            var body = new InstagramDataModel
            {
                memberID = memberID,
                instagramURL = instagramURL
            };


            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.PutAsJsonAsync("SetInstagramURL/" + memberID.ToString(), body);


            //var request = new RestRequest("SetInstagramURL", Method.Put);
            //request.AddHeader("authorization", "Bearer " + jwt);
            //request.RequestFormat = DataFormat.Json;
            //request.AddJsonBody(body);
            //RestResponse response = await _restClient.ExecuteAsync(request);
        }

        public async Task UploadImage(string memberID, MultipartFormDataContent content, string jwt)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var requestUrl = MEMBERS_SERVICE_URI + "UploadProfilePhoto/" + memberID;
            var response = await httpClient.PostAsync(requestUrl, content);
        }

    }

    public interface IMembers
    {
        Task<List<Item>> GetSportsList(string jwtToken);
        Task<List<RecentPostsModel>> GetRecentPosts(int memberID, string jwtToken);
        Task<List<RecentPostChildModel>> GetChildPosts(int postID, string jwtToken);
        Task<UserModel> AuthenticateLGUser(string username, string pwd, string rememberMe, string screen);
        Task<UserModel> RefreshToken(string jwtToken);
        Task<string> RegisterUser(RegisterModel register);
        Task<List<RecentNewsModel>> GetRecentNews(string jwtToken);
        Task<MemberProfileBasicInfoModel> GetMemberBasicInfo(int memberID, string jwtToken);
        Task<List<RegisteredMembersModel>> GetRegisteredMembers(int status);
        Task<string> SavePosts(int memberID, int postID, string postMsg, string jwtToken);
        Task<MemberProfileContactInfoModel> GetMemberContactInfo(int memberID, string jwtToken);
        Task<List<MemberProfileEmploymentModel>> GetMemberEmploymentInfo(int memberID, string jwtToken);
        Task<List<MemberProfileEducationModel>> GetMemberEducationInfo(int memberID, string jwtToken);
        Task<List<MemberProfileAboutModel>> GetMemberAboutInfo(int memberID, string jwtToken);
        Task<List<ContactsModel>> GetMyContacts(int memberID, String show, string jwtToken);
        //Task<List<EventsModel>> GetMemberEventsInfo(int memberID, string jwtToken);
        Task<string> UpdateProfilePicture(int memberID, string fileName);
        Task<string> IsFriendByContactID(string memberID, string contactID, string jwtToken);
        Task<string> IsContactRequestSent(string memberID, string contactID, string jwtToken);
        Task SaveMemberGeneralInfo(string memberID, MemberProfileBasicInfoModel basicInfo, string jwtToken);
        Task SaveMemberContactInfo(string memberID, MemberProfileContactInfoModel contactInfo, string jwtToken);
        Task SaveMemberAboutData(string memberID, string aboutMe, string activities, string hobbies, string specialSkills, string jwtToken);

        Task<string> ResetPassword(string email, string jwt);
        Task<string> IsResetCodeExpired(string code, string jwt);
        Task<string> ChangePassword(RegisterModel model, string jwt);
        Task<string> SetMemberStatus(string MemberId,string status, string jwt);
        Task UpdateSchool(string memberId, MemberProfileEducationModel body, string jwt);
        Task UploadImage(string memberId, MultipartFormDataContent content, string jwt);
    }
}
