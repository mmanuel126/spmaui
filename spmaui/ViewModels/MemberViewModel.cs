using System;
using spmaui.Services;
using spmaui.Models;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace spmaui.ViewModels
{

    public class MemberViewModel: INotifyPropertyChanged
    {
        public ICommand RefreshCommand { get; set; }

        private void OnRefreshCommandExecuted() => Task.Run(() => DoRefreshPosts());
       
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

        async Task DoRefreshPosts()
        {
            IsRefreshing = true;
            
            IsRefreshing = false;
        }

        private readonly IMembers _membersSvc;

        public MemberViewModel(IMembers membersSvc)
        {
            RefreshCommand = new   Command(OnRefreshCommandExecuted);
            _membersSvc = membersSvc;
        }
        /*
        public async Task<List<MemberProfileBasicInfoModel>> GetMemberBasicInfo(string memberID, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";
            return null; //await _membersSvc.GetMemberBasicInfo(int.Parse(memberID), jwtToken);
        }
        */
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

        public async Task<List<ContactsModel>> GetMyContacts(string memberID, string v, string jwtToken)
        {
            int res;
            if (int.TryParse(memberID, out res) == false)
                memberID = "0";
            return await _membersSvc.GetMyContacts(int.Parse(memberID), v, jwtToken);
        }

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

        public async Task<string> ResetPassword(string email)
        {
            return await _membersSvc.ResetPassword(email);
        }

        public async Task<string> IsResetCodeExpired(string code)
        {
            return await _membersSvc.IsResetCodeExpired(code);
        }

        public async Task<string> ChangePassword(RegisterModel register)
        {
            return await _membersSvc.ChangePassword(register);
        }

        public async void LogException(string msg, string stackTrace, string jwt)
        {
            await _membersSvc.LogException(msg, stackTrace, jwt);
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
