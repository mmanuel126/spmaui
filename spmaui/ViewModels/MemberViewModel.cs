using System;
using sp_maui.Services;
using sp_maui.Models;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace sp_maui.ViewModels
{

    public class MemberViewModel: INotifyPropertyChanged
    {
        
        public ICommand RefreshCommand { get; set; }

        private void OnRefreshCommandExecuted() => DoRefreshPosts();
       
        bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
           
            set
            {
                isRefreshing = value;

                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        ObservableCollection<RecentNewsModel> _News;
        public ObservableCollection<RecentNewsModel> News
        {
            get
            {
                return _News;
            }
            set
            {
                _News = value;
                OnPropertyChanged();
            }
        }

        async Task DoRefreshPosts()
        {

            News.Clear();
            News = new ObservableCollection<RecentNewsModel>();

            IsRefreshing = true;
            //  DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
         //   this.News = await this.GetRecentNews();
            //DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            IsRefreshing = false;
        }

        private Members _membersSvc;

        public MemberViewModel()
        {
            RefreshCommand = new   Command(OnRefreshCommandExecuted);
            _membersSvc = new Members();
            News = new ObservableCollection<RecentNewsModel>();
            GetNewsAsync();

        }

        async Task GetNewsAsync()
        {
            string jwtToken = Preferences.Get("AccessToken","");
            List<RecentNewsModel> rn = await GetRecentNews();
            //News = rn;
        }

        public async  Task<List<RecentNewsModel>> GetRecentNews()
        {
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            return  await _membersSvc.GetRecentNews(jwtToken);
        }

        public async Task<List<MemberProfileBasicInfoModel>> GetMemberBasicInfo(string memberID, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";
            return null; //await _membersSvc.GetMemberBasicInfo(int.Parse(memberID), jwtToken);
        }

        public async Task<MemberProfileContactInfoModel> GetMemberContactInfo(string memberID, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";

            return await _membersSvc.GetMemberContactInfo(int.Parse(memberID), jwtToken);
        }

        public async Task<List<MemberProfileEducationModel>> GetMemberEducationInfo(string memberID, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";
           
            return await _membersSvc.GetMemberEducationInfo(int.Parse(memberID), jwtToken);
        }

        public async Task<List<MemberProfileEmploymentModel>> GetMemberEmploymentInfo(string memberID, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";

            return await _membersSvc.GetMemberEmploymentInfo(int.Parse(memberID), jwtToken);
        }

        public async Task<List<MemberProfileAboutModel>> GetMemberAboutInfo(string memberID, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";

            return await _membersSvc.GetMemberAboutInfo(int.Parse(memberID), jwtToken);
        }

        //public async Task<List<EventsModel>> GetMemberEventsInfo(string memberID, string jwtToken)
        //{
        //    int res;
        //    if (int.TryParse(memberID, out res) == false)
        //        memberID = "0";
        //    return await _membersSvc.GetMemberEventsInfo(int.Parse(memberID), jwtToken);
        //}

        //public List<PhotosModel> GetAlbumPhotos(int albumID)
        //{
        //    //MemberAPIService srv = new MemberAPIService();
        //    //return srv.GetAlbumPhotos(albumID);
        //    return null;
        //}

        //public List<NetworksModel> GetMemberNetworkInfo(string memberID, string jwtToken)
        //{
        //    int res;
        //    if (int.TryParse(memberID, out res) == false)
        //        memberID = "0";

        //    //NetworkWCFServices srv = new NetworkWCFServices();
        //    //return srv.GetMyNetworks (int.Parse(memberID), jwtToken) ;
        //    return null;
        //}

        public async Task<List<ContactsModel>> GetMyContacts(string memberID, string v, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";
            return await _membersSvc.GetMyContacts(int.Parse(memberID), v, jwtToken);
        }


        //public List<Models.PhotosAlbumModel> GetMemberAlbums(string memberID)
        //{
        //    //int res;
        //    //if (int.TryParse(memberID, out res) == false)
        //    //    memberID = "0";

        //    //MemberAPIService srv = new MemberAPIService();
        //    //return srv.GetMemberAlbums(int.Parse(memberID));
        //    return null;
        //}

        public async Task<string> IsFriendByContactID(string memberID, string contactID, string jwtToken)
        {
            return await _membersSvc.IsFriendByContactID(memberID, contactID, jwtToken);
        }

        public async Task<string> IsContactRequestSent(string memberID, string contactID, string jwtToken)
        {
            return await _membersSvc.IsContactRequestSent(memberID, contactID, jwtToken);
        }

        public void SaveMemberGeneralInfo(string memberID, MemberProfileBasicInfoModel basicInfo, string jwtToken)
        {
            _membersSvc.SaveMemberGeneralInfo(memberID, basicInfo, jwtToken);
        }

        public void SaveMemberContactInfo(string memberID, MemberProfileContactInfoModel contactInfo, string jwtToken)
        {
            _membersSvc.SaveMemberContactInfo(memberID, contactInfo, jwtToken);
        }

        public void SaveMemberAboutData(string memberID, string aboutMe, string activities, string hobbies, string specialSkills, string jwtToken)
        {
            _membersSvc.SaveMemberAboutData(memberID, aboutMe, activities, hobbies, specialSkills, jwtToken);
        }

        public async Task<UserModel> AuthenticateLGUser(string username, string pwd, string rememberMe, string screen)
        {
            return await _membersSvc.AuthenticateLGUser(username, pwd, rememberMe, screen);
        }

        public async Task<string> register(RegisterModel register)
        {
            return await _membersSvc.RegisterUser(register);
        }

        public async Task<string> ResetPassword(string email, string jwt)
        {
            return await _membersSvc.ResetPassword(email, jwt);
        }

        public async Task<string> IsResetCodeExpired(string code, string jwt)
        {
            return await _membersSvc.IsResetCodeExpired(code, jwt);
        }

        public async Task<string> ChangePassword(RegisterModel register, string jwt)
        {
            return await _membersSvc.ChangePassword(register, jwt);
        }



        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }


}
