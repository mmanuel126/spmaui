using System;
using sp_maui.Services;
using sp_maui.Models;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace sp_maui.ViewModels
{
    public class SettingsPrivacyViewModel: INotifyPropertyChanged
    {
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

        public SettingsPrivacyViewModel()
        {
            GetPrivacySettingsInfo();
            GetProfilePrivacyTypes();
        }

        private async void GetPrivacySettingsInfo()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Settings s = new Settings();
            PrivacySettingsInfo = await s.GetProfileSettings(memberID, jwtToken);
        }

        private  void GetProfilePrivacyTypes()
        {
            List<ProfilePrivacyTypesModel> lst = new List<ProfilePrivacyTypesModel>();
            var question = new ProfilePrivacyTypesModel { Id = 0, Desc = "Select..." }; lst.Add(question);
            question = new ProfilePrivacyTypesModel { Id = 1, Desc = "Public" }; lst.Add(question);
            question = new ProfilePrivacyTypesModel { Id = 2, Desc = "Private" }; lst.Add(question);
            question = new ProfilePrivacyTypesModel { Id = 3, Desc = "Only Connections" }; lst.Add(question);
           
            ProfilePrivacyTypes = lst;
        }

        public async Task SaveProfileSettings(PrivacySettingsModel body)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Settings s = new Settings();
            await s.SaveProfileSettings (memberID, body,  jwtToken);
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
