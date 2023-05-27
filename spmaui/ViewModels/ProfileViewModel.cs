using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using sp_maui.Services;
using sp_maui.Models;
using sp_maui.Views;
using Microsoft.Extensions.Caching.Memory;

namespace sp_maui.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public Item SelectedSport { get; set; }

        MemberProfileBasicInfoModel _memberProfileBasicInfo;
        public MemberProfileBasicInfoModel ProfileBasicInfo
        {
            get { return _memberProfileBasicInfo; }
            set { _memberProfileBasicInfo = value; OnPropertyChanged(); }
        }

        MemberProfileContactInfoModel _memberProfileContactInfo;
        public MemberProfileContactInfoModel ProfileContactInfo
        {
            get { return _memberProfileContactInfo; }
            set { _memberProfileContactInfo = value; OnPropertyChanged(); }
        }

        List<Item> _sportsList;
        public List<Item> SportsList
        {
            get { return _sportsList; }
            set { _sportsList = value; OnPropertyChanged(); }
        }

        public Command<MemberProfileEducationModel> DeleteCommand { get; set; }
        public Command<MemberProfileEducationModel> EditCommand { get; set; }
        public Command AddNewCommand { get; set; }

        List<MemberProfileEducationModel> _memberEducation;

        List<StatesModel> _states;
        public List<StatesModel> States
        {
            get { return _states; }
            set { _states = value; OnPropertyChanged(); }
        }


        List<SchoolsByStateModel> _schools;
        public List<SchoolsByStateModel> Schools
        {
            get { return _schools; }
            set { _schools = value; OnPropertyChanged(); }
        }


        public List<MemberProfileEducationModel> ProfileEducation
        {
            get { return _memberEducation; }
            set { _memberEducation = value; OnPropertyChanged(); }
        }

        public Command RefreshCommand { get; set; }

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

        List<YoutubePlayListModel> _Playlist;

        public List<YoutubePlayListModel> Playlist
        {
            get
            {
                return _Playlist;
            }
            set
            {
                _Playlist = value;
                OnPropertyChanged();
            }
        }


        public ProfileViewModel()
        {
            GetMemberBasicInfo();
            //GetSportsList();
            GetMemberContactInfo();
            /* MessagingCenter.Subscribe<ProfileEditPhotosPage>(this, "RefreshContacts", (sender) =>
             {
                 GetMemberContactInfo();
             });*/

            DeleteCommand = new Command<MemberProfileEducationModel>(OnDeleteEducation);
            EditCommand = new Command<MemberProfileEducationModel>(OnEditEducation);
            AddNewCommand = new Command(OnAddNewEducation);

            GetMemberEducation();
            GetStates();
            //GetSchools();

            //MessagingCenter.Subscribe<ProfileEditEducationUpdatePage>(this, "RefreshEducation", (sender) =>
            //{
            //    GetMemberEducation();
            //});

            //MessagingCenter.Subscribe<ProfileEditEducationAddNewPage>(this, "RefreshEducation", (sender) =>
            //{
            //    GetMemberEducation();
            //});

            RefreshCommand = new Command(OnRefreshCommandExecuted);
            Playlist = new List<YoutubePlayListModel>();
            GetPlayListAsync();

        }



        public async Task GetSportsList()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            Members _membersSvc = new Members();
            SportsList = await _membersSvc.GetSportsList(jwtToken);

            foreach(var x in SportsList)
            {
                if (x.name == ProfileBasicInfo.Sport)
                {
                    SelectedSport = x; return ;
                }
            }
        }

        public async Task GetMemberBasicInfo()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Members _membersSvc = new Members();
            ProfileBasicInfo = await _membersSvc.GetMemberBasicInfo(int.Parse(memberID), jwtToken);
            
            ProfileBasicInfo.ProfilePlayList = await _membersSvc.GetMemberPlaylists(memberID, jwtToken);
        }

        public async Task SaveMemberGeneralInfo(MemberProfileBasicInfoModel model)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Members _membersSvc = new Members();
            await _membersSvc.SaveMemberGeneralInfo(memberID, model, jwtToken);
        }

        public async Task GetMemberContactInfo()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Members _membersSvc = new Members();
            var pcInfoLst = await _membersSvc.GetMemberContactInfo(int.Parse(memberID), jwtToken);
            ProfileContactInfo = pcInfoLst;
        }

        public async Task SaveMemberContactInfo(MemberProfileContactInfoModel model)
        {
            string jwtToken = Preferences.Get("AccessToken","");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID","")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Members _membersSvc = new Members();
            await _membersSvc.SaveMemberContactInfo(memberID, model, jwtToken);
        }

        async void OnDeleteEducation(MemberProfileEducationModel educationModel)
        {
            await DeleteEducation(educationModel);
            await GetMemberEducation();
        }

        async void OnEditEducation(MemberProfileEducationModel educationModel)
        {
            Preferences.Set("schoolimage", educationModel.schoolImage);
            Preferences.Set("major",educationModel.major);
            Preferences.Set("degree",educationModel.degree);
            Preferences.Set("year",educationModel.yearClass);
            Preferences.Set("competionlevel", educationModel.sportLevelType);
            Preferences.Set("schoolID", educationModel.schoolID);
            Preferences.Set("schoolName", educationModel.schoolName);
            await Application.Current.MainPage.Navigation.PushModalAsync(new ProfileUpdateEducationPage());
        }

        async void OnAddNewEducation()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ProfileAddEducationPage());
        }

        public async Task DeleteEducation(MemberProfileEducationModel educationModel)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Members svc = new Members();
            await svc.RemoveSchool(memberID, educationModel.schoolID, educationModel.schoolType, jwtToken);
        }

        public async Task UpdateEducation(MemberProfileEducationModel schoolInfo)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }

            Members _membersSvc = new Members();
            await _membersSvc.UpdateSchool(memberID, schoolInfo, jwtToken);
            await GetMemberEducation();
        }

        public async Task AddNewEducation(MemberProfileEducationModel schoolInfo)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }

            Members _membersSvc = new Members();
            await _membersSvc.AddNewSchool(memberID, schoolInfo, jwtToken);
            await GetMemberEducation();
        }

        public async Task GetMemberEducation()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID","")))
            {
                memberID = Preferences.Get("UserID", "");
            }

            Members _membersSvc = new Members();
            ProfileEducation = await _membersSvc.GetMemberEducationInfo(int.Parse(memberID), jwtToken);
        }

        private async Task GetStates()
        {
            Commons svc = new Commons();
            string jwtToken = Preferences.Get("AccessToken", "");
            States = await svc.GetStates(jwtToken);
        }

        private async Task GetSchools()
        {
            Commons svc = new Commons();
            string jwtToken = Preferences.Get("AccessToken","");
            Schools = await svc.GetSchoolsByState("AZ", "3", jwtToken);
        }

        async Task DoRefreshPosts()
        {
            Playlist.Clear();
            Playlist = new List<YoutubePlayListModel>();

            IsRefreshing = true;
            this.Playlist = await this.GetPlayList();
            IsRefreshing = false;
        }

        async Task GetPlayListAsync()
        {
            List<YoutubePlayListModel> rn = await GetPlayList();
            Playlist = rn;
        }

        public async Task<List<YoutubePlayListModel>> GetPlayList()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID","")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Members _membersSvc = new Members();
            return await _membersSvc.GetMemberPlaylists(memberID, jwtToken);
        }

        public async Task SaveChannelID(string channelID)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            Members _membersSvc = new Members();
            await _membersSvc.SaveChannelID(memberID, channelID, jwtToken);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
