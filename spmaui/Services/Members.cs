using System;
using RestSharp;
using Newtonsoft.Json;
using spmaui.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using spmaui;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Web;

namespace spmaui.Services
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
                        child.postID = ls.postResponseID.ToString();
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
            try
            {

                httpClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);
                var rsp = await httpClient.GetAsync("getRecentPostResponses/" + postID.ToString());
                var dynJson = await rsp.Content.ReadFromJsonAsync<List<RecentPostChildModel>>();
                //dynJson.memberProfileTitle = dynJson.CurrentStatus;
                //dynJson.memProfileName = dynJson.FirstName + " " + dynJson.MiddleName + " " + dynJson.LastName;

                /*

                List<RecentPostChildModel> lst = new List<RecentPostChildModel>();
                var request = "getRecentPostResponses/" + postID.ToString();

                _restClient.DefaultRequestHeaders.Authorization
                             = new AuthenticationHeaderValue("Bearer", jwtToken);
                var response = await _restClient.GetAsync(request);
                var dynJson = await response.Content.ReadFromJsonAsync<List<RecentPostChildModel>>();
                */

                return dynJson;
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                return null;
            }
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
        }

        public async Task<UserModel> RefreshToken(string jwtToken)
        {
            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response = await _restClient.PostAsync("refreshLogin?accessToken=" + jwtToken, requestContent);
            var dynJson = await response.Content.ReadFromJsonAsync<UserModel>();
            return dynJson;
        }

        /// <summary>
        /// Registers the user to lg.
        /// </summary>
        /// <returns>existing or newemail string.</returns>
        /// <param name="register">Register.</param>
        public async Task<string> RegisterUser(RegisterModel register)
        {
            RestClient _restClient = new RestClient(ACCOUNT_SERVICE_URI);
            var request = new RestRequest("register", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(register); register.confirmPwd = "";register.code = "";
            RestResponse response = await _restClient.ExecuteAsync(request);
            var content = response.Content;
            var result = JsonConvert.DeserializeObject<string>(content).ToString();
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
            }
            catch (Exception e)
            {
                string x = e.Message; return null;
            }
        }


        public async Task<List<YoutubeVideosListModel>> GetPlaylistVideos(string playListID, string jwtToken)
        {
            try
            {
                _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await _restClient.GetAsync("GetVideosList/" + playListID);
                var dynJson = await response.Content.ReadFromJsonAsync<List<YoutubeVideosListModel>>();
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
                dynJson.memberProfileTitle = dynJson.CurrentStatus;
                dynJson.memProfileName = dynJson.FirstName + " " + dynJson.MiddleName + " " + dynJson.LastName;

                if (String.IsNullOrEmpty(dynJson.memProfileImage))
                {
                    dynJson.memProfileImage = App.AppSettings.AppMemberImagesURL + "default.png";
                }
                else
                {
                    dynJson.memProfileImage = App.AppSettings.AppMemberImagesURL + dynJson.memProfileImage;
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

                var response2 = await _restClient.GetAsync("GetYoutubeChannel/" + memberID.ToString());
                var result2 = await response2.Content.ReadAsStringAsync();
              
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
        }

        /// <summary>
        /// Gets the member education info.
        /// </summary>
        /// <returns>The member education info.</returns>
        /// <param name="memberID">Member identifier.</param>
        public async Task<List<MemberProfileEducationModel>> GetMemberEducationInfo(int memberID, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", jwtToken);

            string resource = "GetMemberEducationInfo/" + memberID.ToString();
            var response = await _restClient.GetAsync(resource);
            var res = await response.Content.ReadFromJsonAsync<List<MemberProfileEducationModel>>();
            
            for (var i = 0; i < res.Count-1; i++)
            {
                if (res[i].schoolName != null)
                {
                    if (res[i].schoolName.Length > 40)
                    {
                        res[i].schoolName = res[i].schoolName.Substring(0, 40) + "...";
                    }
                }

                if (res[i].schoolName != null)
                {
                    if (res[i].schoolAddress.Length > 48)
                    {
                        res[i].schoolAddress = res[i].schoolAddress.Substring(0, 48) + "...";
                    }
                }

                if (res[i].schoolImage != null)
                {
                    if (res[i].schoolImage != "")
                    {
                        res[i].webSite = res[i].schoolImage;

                        if (res[i].webSite.IndexOf("http") == -1)
                        {
                            res[i].webSite = "http://" + res[i].webSite;
                        }

                        res[i].schoolImage = "https://www.google.com/s2/favicons?domain=" + res[i].schoolImage.ToString();
                    }
                    else
                    {
                       res[i].schoolImage = App.AppSettings.AppMemberImagesURL + "default.png";
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

            string resource = "GetMemberPersonalInfo/" + memberID.ToString();
            var response = await _restClient.GetAsync(resource);
            var res = await response.Content.ReadFromJsonAsync<List<MemberProfileAboutModel>>();
            return res;
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
        }

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
        }

        /// <summary>
        /// returns true or false if contact is a friend of member.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="contactID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<string> IsFriendByContactID(string memberID, string contactID, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwtToken);

            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response2 = await _restClient.GetAsync("IsFriendByContactID/" + memberID.ToString() + "/" + contactID.ToString());
            var result = await response2.Content.ReadAsStringAsync();
            return result;
        }

        /// <summary>
        /// determines if contact request is sent.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="contactID"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task <string> IsContactRequestSent(string memberID, string contactID, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwtToken);

            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response2 = await _restClient.GetAsync("IsContactRequestSent" + memberID.ToString() + "/" + contactID.ToString());
            var result = await response2.Content.ReadAsStringAsync();
            return result;
        }

        /// <summary>
        /// saves member general information.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="basicInfo"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
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

            _restClient.DefaultRequestHeaders.Authorization
                      = new AuthenticationHeaderValue("Bearer", jwtToken);
            await _restClient.PostAsJsonAsync("SaveMemberGeneralInfo/" + memberID.ToString(), body);
        }

        /// <summary>
        /// saves member contact information.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="contactInfo"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task SaveMemberContactInfo(string memberID, MemberProfileContactInfoModel contactInfo, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                     = new AuthenticationHeaderValue("Bearer", jwtToken);

            var builder = new UriBuilder(MEMBERS_SERVICE_URI + "SaveMemberContactInfoV2/" + memberID.ToString());
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
            query["State"]= contactInfo.State;
            query["Twitter"]= contactInfo.Twitter;
            query["Website"]= contactInfo.Website;
            query["Zip"]= contactInfo.Zip;
            builder.Query = query.ToString();
            string url = builder.ToString();
            var content = new StringContent("Encoding.UTF8, application/json");
            await _restClient.PostAsync(url,content);

        }

        /// <summary>
        /// Saves member biography data.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="aboutMe"></param>
        /// <param name="activities"></param>
        /// <param name="hobbies"></param>
        /// <param name="specialSkills"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task SaveMemberAboutData(string memberID, string aboutMe, string activities, string hobbies, string specialSkills, string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", jwtToken);

            var request = "SaveMemberPersonalInfo?memberID=" + memberID.ToString() + "&activities=" + activities + " &interests=" + hobbies + " &specialSkills=" + specialSkills + " &aboutMe=" + aboutMe;
            var content = new StringContent("Encoding.UTF8, application/json");
            await _restClient.PostAsync(request, content);
        }

        /// <summary>
        /// reset password.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<string> ResetPassword(string email)
        {
            var response = await _restClient.GetAsync("ResetPassword?email=" + email);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        /// <summary>
        /// checks if reset code expired.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> IsResetCodeExpired(string code)
        {
            var response = await _restClient.GetAsync("IsResetCodeExpired?code=" + code);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        /// <summary>
        /// changes password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> ChangePassword(RegisterModel model)
        {
            var request = "ChangePassword?pwd=" + model.confirmPwd + "&email=" + model.email + "&code=" + model.code;
            var response = await _restClient.GetAsync(request);
            var dynJson = await response.Content.ReadAsStringAsync();
            return dynJson;
        }

        /// <summary>
        /// set member status.
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="status"></param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<string> SetMemberStatus(string MemberId, string status,string jwtToken)
        {
            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwtToken);
            var request = "SetMemberStatus?memberId=" + MemberId.ToString() + "&status=" + status;
            var requestContent = new StringContent("Encoding.UTF8, application/json");
            var response = await _restClient.PostAsync(request, requestContent);
            var dynJson = await response.Content.ReadAsStringAsync();
            return dynJson;
        }

        /// <summary>
        /// get sport list.
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public async Task<List<Item>>  GetSportsList(string jwtToken)
        {
            _restConnectionClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _restConnectionClient.GetAsync("GetSportsList");
            var dynJson = await response.Content.ReadFromJsonAsync<List<Item>>();
            return dynJson;
        }

        /// <summary>
        /// add new school.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="body"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task AddNewSchool(string memberId, MemberProfileEducationModel body, string jwt)
        {
            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.PostAsJsonAsync("AddMemberSchool/" + memberId.ToString(), body);
        }

        /// <summary>
        /// update school.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="body"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task UpdateSchool( string memberId, MemberProfileEducationModel body, string jwt)
        {
         
            var request = new RestRequest("UpdateMemberSchool/"+ memberId, Method.Put);
            request.AddHeader("authorization", "Bearer " + jwt);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(body);
            RestResponse response = await _rstClient.ExecuteAsync(request);
        }

        /// <summary>
        /// remove school.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="schoolId"></param>
        /// <param name="instType"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task RemoveSchool(string memberId, string schoolId, string  instType, string jwt)
        {
            _restClient.DefaultRequestHeaders.Authorization
                       = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.DeleteAsync("RemoveMemberSchool?memberID=" + memberId + "&instID=" + schoolId + "&instType=" + instType);
        }

        /// <summary>
        /// save youtube channel ID.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="channelID"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task SaveChannelID(string memberID, string channelID, string jwt)
        {
            var body = new YoutubeDataModel
            {
                memberID = memberID,
                channelID = channelID
            };

            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.PutAsJsonAsync("SetYoutubeChannel", body);
        }

        /// <summary>
        /// save instagram URL.
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="instagramURL"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task SaveInstagramURL(string memberID, string instagramURL, string jwt)
        {
            var body = new InstagramDataModel
            {
                memberID = memberID,
                instagramURL = instagramURL
            };
            _restClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", jwt);
            await _restClient.PutAsJsonAsync("SetInstagramURL", body);
        }

        /// <summary>
        /// upload image.
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
        Task<string> UpdateProfilePicture(int memberID, string fileName);
        Task<string> IsFriendByContactID(string memberID, string contactID, string jwtToken);
        Task<string> IsContactRequestSent(string memberID, string contactID, string jwtToken);
        Task SaveMemberGeneralInfo(string memberID, MemberProfileBasicInfoModel basicInfo, string jwtToken);
        Task SaveMemberContactInfo(string memberID, MemberProfileContactInfoModel contactInfo, string jwtToken);
        Task SaveMemberAboutData(string memberID, string aboutMe, string activities, string hobbies, string specialSkills, string jwtToken);
        Task<string> ResetPassword(string email);
        Task<string> IsResetCodeExpired(string code);
        Task<string> ChangePassword(RegisterModel model);
        Task<string> SetMemberStatus(string MemberId,string status, string jwt);
        Task UpdateSchool(string memberId, MemberProfileEducationModel body, string jwt);
        Task UploadImage(string memberId, MultipartFormDataContent content, string jwt);
        Task<List<YoutubePlayListModel>> GetMemberPlaylists(string memberID, string jwtToken);
        Task RemoveSchool(string memberId, string schoolId, string instType, string jwt);
        Task AddNewSchool(string memberId, MemberProfileEducationModel body, string jwt);
        Task SaveChannelID(string memberID, string channelID, string jwt);
        Task SaveInstagramURL(string memberID, string instagramURL, string jwt);
        Task<List<YoutubeVideosListModel>> GetPlaylistVideos(string playListID, string jwtToken);
        Task LogException(string msg, string stackTrace, string jwt);
    }
}
