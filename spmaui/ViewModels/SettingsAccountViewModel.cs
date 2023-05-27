using System;
using sp_maui.Services;
using sp_maui.Models;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace sp_maui.ViewModels
{
    public class SettingsAccountViewModel: INotifyPropertyChanged
    {
        List<AccountSettingsInfoModel> _AccountSettingsInfo;
        public List<AccountSettingsInfoModel> AccountSettingsInfo
        {
            get
            {
                return _AccountSettingsInfo;
            }
            set
            {
                _AccountSettingsInfo = value;
                OnPropertyChanged();
            }
        }

        List<SecurityQuestions> _SecQuestions;
        public List<SecurityQuestions> SecQuestions
        {
            get
            {
                return _SecQuestions;
            }
            set
            {
                _SecQuestions = value;
                OnPropertyChanged();
            }
        }

        List<DeactivationReasonsModel> _DeactivationReasons;
        public List<DeactivationReasonsModel> DeactivationReasons
        {
            get
            {
                return _DeactivationReasons;
            }
            set
            {
                _DeactivationReasons = value;
                OnPropertyChanged();
            }
        }

        NotificationsSettingModel _AccountSettingsNotifications;
        public NotificationsSettingModel AccountSettingsNotifications
        {
            get
            {
                return _AccountSettingsNotifications;
            }
            set
            {
                _AccountSettingsNotifications = value;
                OnPropertyChanged();
            }
        }

        private readonly Members _membersSvc;

        public SettingsAccountViewModel()
        {
            _membersSvc = new Members();
            GetAccountSettngsInfo();
            GetSecurityQuestions();
            GetAccountSettingsNotifications();
            GetDeactivationReasons();
        }

        private async void GetAccountSettngsInfo()
        {
            string jwtToken = Preferences.Get("AccessToken","");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Settings s = new Settings();
            AccountSettingsInfo = await s.GetMemberNameInfo(memberID, jwtToken);
        }

        private  void GetSecurityQuestions()
        {
            List<SecurityQuestions> lst = new List<SecurityQuestions>();
            var question = new SecurityQuestions { Id = 0, Desc = "Select..." }; lst.Add(question);
            question = new SecurityQuestions { Id = 1, Desc = "Who was your first love?" }; lst.Add(question);
            question = new SecurityQuestions { Id = 2, Desc = "Who was your favorite teacher?" }; lst.Add(question);
            question = new SecurityQuestions { Id = 3, Desc = "What is your mother's name?" }; lst.Add(question);
            question = new SecurityQuestions { Id = 4, Desc = "What was the name of your first pet?" }; lst.Add(question);
            question = new SecurityQuestions { Id = 5, Desc = "What neighborhood did your grow up on?" }; lst.Add(question);
            question = new SecurityQuestions { Id = 6, Desc = "What is your father's nick name?" }; lst.Add(question);

            SecQuestions = lst;
        }


        private async void GetAccountSettingsNotifications()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Settings s = new Settings();
            AccountSettingsNotifications = await s.GetMemberNotifications(memberID, jwtToken);
        }

        public async Task SaveNotificationSettings(NotificationsSettingModel body)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Settings s = new Settings();
            await s.SaveNotificationSettings(memberID, body, jwtToken);
        }


        private void GetDeactivationReasons()
        {
            List<DeactivationReasonsModel> lst = new List<DeactivationReasonsModel>();
            var reason = new DeactivationReasonsModel { Id = 0, Desc = "Other." }; lst.Add(reason);
            reason = new DeactivationReasonsModel { Id = 1, Desc = "I am leaving temporarily. I will be back." }; lst.Add(reason);
            reason = new DeactivationReasonsModel { Id = 2, Desc = "I have another account that I will start using." }; lst.Add(reason);
            reason = new DeactivationReasonsModel { Id = 3, Desc = "I get too many emails, invites, and requests." }; lst.Add(reason);
            reason = new DeactivationReasonsModel { Id = 4, Desc = "I do not have enough privacy and security." }; lst.Add(reason);
            reason = new DeactivationReasonsModel { Id = 5, Desc = "I do not have enough privacy and security." }; lst.Add(reason);
            reason = new DeactivationReasonsModel { Id = 6, Desc = "I do not understand hot to use it." }; lst.Add(reason);
            DeactivationReasons = lst;
        }

        public async Task SaveSecurityQuestionInfo(string question, string answer)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Settings s = new Settings();
            await s.SaveSecurityQuestionInfo(memberID, question, answer, jwtToken);
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
