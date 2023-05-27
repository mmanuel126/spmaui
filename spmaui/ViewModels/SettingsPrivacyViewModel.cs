using System;
using spmaui.Services;
using spmaui.Models;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace spmaui.ViewModels
{
    public class SettingsPrivacyViewModel: INotifyPropertyChanged
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

        PrivacySettingsModel _PrivacySettingsInfo;
        public PrivacySettingsModel PrivacySettingsInfo
        {
            get
            {
                return _PrivacySettingsInfo;
            }
            set
            {
                _PrivacySettingsInfo = value;
                OnPropertyChanged();
            }
        }
        
        List<ProfilePrivacyTypesModel> _ProfilePrivacyTypes;
        public List<ProfilePrivacyTypesModel> ProfilePrivacyTypes
        {
            get
            {
                return _ProfilePrivacyTypes;
            }
            set
            {
                _ProfilePrivacyTypes = value;
                OnPropertyChanged();
            }
        }

        private readonly ISettings _settingsSvc;

        public SettingsPrivacyViewModel(ISettings settingsSvc)
        {
            try
            {
                _settingsSvc = settingsSvc;
                GetPrivacySettingsInfo();
                GetProfilePrivacyTypes();
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

        public async void GetPrivacySettingsInfo()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            PrivacySettingsInfo = await _settingsSvc.GetProfileSettings(memberID, jwtToken);
        }

        public void GetProfilePrivacyTypes()
        {
            List<ProfilePrivacyTypesModel> lst = new List<ProfilePrivacyTypesModel>();
            var question = new ProfilePrivacyTypesModel { Id = 0, Desc = "Select..." }; lst.Add(question);
            question = new ProfilePrivacyTypesModel { Id = 1, Desc = "Public" }; lst.Add(question);
            question = new ProfilePrivacyTypesModel { Id = 2, Desc = "Private" }; lst.Add(question);
            question = new ProfilePrivacyTypesModel { Id = 3, Desc = "Only Connections" }; lst.Add(question);
           
            ProfilePrivacyTypes = lst;
        }

        public async void SaveProfileSettings(PrivacySettingsModel body)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _settingsSvc.SaveProfileSettings (memberID, body,  jwtToken);
        }

        public async void SaveSearchSettings(PrivacySettingsModel psm)
        {
            Settings s = new Settings();
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                memberID = Preferences.Get("UserID", "");

            await s.SaveSearchSettings(memberID, psm, jwtToken);
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
