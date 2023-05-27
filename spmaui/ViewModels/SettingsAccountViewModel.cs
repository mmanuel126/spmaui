using System;
using spmaui.Services;
using spmaui.Models;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace spmaui.ViewModels
{
    public class SettingsAccountViewModel: INotifyPropertyChanged
    {
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

        private readonly ISettings _settingsSvc;

        public SettingsAccountViewModel(ISettings settingsSvc)
        {
            try
            {
                IsRefreshing = true;
                _settingsSvc = settingsSvc;
                GetAccountSettngsInfo();
                GetSecurityQuestions();
                GetAccountSettingsNotifications();
                GetDeactivationReasons();
                IsRefreshing = false;
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (ex.GetType() == typeof(HttpRequestException))
                    {
                        await App.Current.MainPage.DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                    }
                    else
                    {
                        LogException(ex.Message, ex.StackTrace, "");
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                    }
                });
            }
        }

        public async void GetAccountSettngsInfo()
        {
            string jwtToken = Preferences.Get("AccessToken","");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            AccountSettingsInfo = await _settingsSvc.GetMemberNameInfo(memberID, jwtToken);
        }

        public  void GetSecurityQuestions()
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

        public async void GetAccountSettingsNotifications()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            AccountSettingsNotifications = await _settingsSvc.GetMemberNotifications(memberID, jwtToken);
        }

        public async Task SaveNotificationSettings(NotificationsSettingModel body)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.SaveNotificationSettings(memberID, body, jwtToken);
        }

        public void GetDeactivationReasons()
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

        public async Task UploadImage(MultipartFormDataContent content)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.UploadImage(memberID, content, jwtToken);
        }

        public async Task SaveMemberNameInfo(string firstName, string middleName, string lastName)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.SaveMemberNameInfo(memberID,firstName,middleName,lastName, jwtToken);
        }

        public async Task SavePasswordInfo(string newPassword)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.SavePasswordInfo(memberID, newPassword, jwtToken);
        }

        public async Task SaveSecurityQuestionInfo(string question, string answer)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.SaveSecurityQuestionInfo(memberID, question, answer, jwtToken);
        }

        public async Task SaveNotificationsSettings(NotificationsSettingModel mod)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.SaveNotificationSettings(memberID, mod, jwtToken);
        }

        public async Task DeactivateAccount(string reason, string  explanation)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.DeactivateAccount(memberID, reason, explanation, jwtToken);
        }

        public async void LogException(string msg, string stackTrace, string jwt)
        {
            await _settingsSvc.LogException(msg, stackTrace, jwt);
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
