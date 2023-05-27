using System.ComponentModel;
using System.Runtime.CompilerServices;
using spmaui.Services;
using spmaui.Models;
using spmaui.Views;
using YoutubeExplode.Channels;

namespace spmaui.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public Command<MemberProfileEducationModel> DeleteCommand { get; set; }
        public Command<MemberProfileEducationModel> EditCommand { get; set; }
        public Command AddNewCommand { get; set; }
        public Command<YoutubePlayListModel> GotoPlaylistCommand { get; set; }

        public Command RefreshCommand { get; set; }

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

        List<MemberProfileEducationModel> _memberEducation;
        public List<MemberProfileEducationModel> ProfileEducation
        {
            get { return _memberEducation; }
            set { _memberEducation = value; OnPropertyChanged(); }
        }

        public Item SelectedSport { get; set; }
        List<Item> _sportsList;
        public List<Item> SportsList
        {
            get { return _sportsList; }
            set { _sportsList = value; OnPropertyChanged(); }
        }

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

        private readonly IMembers _membersSvc;
        private readonly ICommons _commonsSvc;

        public ProfileViewModel(IMembers membersSvc, ICommons commonsSvc)
        {
            try
            {
                _membersSvc = membersSvc;
                _commonsSvc = commonsSvc;

                IsRefreshing = true;
                Task.Run(() => GetMemberBasicInfo().Wait());
                //GetSportsList();
                Task.Run(() => GetMemberContactInfo().Wait());

                DeleteCommand = new Command<MemberProfileEducationModel>(OnDeleteEducation);
                EditCommand = new Command<MemberProfileEducationModel>(OnEditEducation);
                AddNewCommand = new Command(OnAddNewEducation);
                GotoPlaylistCommand = new Command<YoutubePlayListModel>(OnGotoPlaylistPage);

                Task.Run(() => GetMemberEducation().Wait());
                Task.Run(() => GetStates().Wait());
                //GetSchools();

                MessagingCenter.Subscribe<ProfileUpdateEducationPage>(this, "RefreshEducation", (sender) =>
                {
                    Task.Run(() => GetMemberEducation().Wait());
                });

                MessagingCenter.Subscribe<ProfileAddEducationPage>(this, "RefreshEducation", (sender) =>
                {
                    Task.Run(() => GetMemberEducation().Wait());
                });

                RefreshCommand = new Command(OnRefreshCommandExecuted);
                Playlist = new List<YoutubePlayListModel>();
                Task.Run(() => GetPlayListAsync().Wait());
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        private void OnRefreshCommandExecuted() => Task.Run(() => DoRefreshPosts());

        async Task DoRefreshPosts()
        {
            try
            {
                Playlist.Clear();
                Playlist = new List<YoutubePlayListModel>();
                this.Playlist = await this.GetPlayList();
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task GetSportsList()
        {
            string jwtToken = Preferences.Get("AccessToken", "");
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
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                ProfileBasicInfo = await _membersSvc.GetMemberBasicInfo(int.Parse(GetMemberID()), jwtToken);

                var lst  = await _membersSvc.GetMemberPlaylists(GetMemberID(), jwtToken);
                if (lst != null )
                {
                    if (lst.Count != 0) {
                        ProfileBasicInfo.ProfilePlayList = lst;
                    }
                }
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task SaveMemberGeneralInfo(MemberProfileBasicInfoModel model)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            await _membersSvc.SaveMemberGeneralInfo(GetMemberID(), model, jwtToken);
        }

        public async Task GetMemberContactInfo()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                var pcInfoLst = await _membersSvc.GetMemberContactInfo(int.Parse(GetMemberID()), jwtToken);
                ProfileContactInfo = pcInfoLst;
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task SaveMemberContactInfo(MemberProfileContactInfoModel model)
        {
            string jwtToken = Preferences.Get("AccessToken","");
            await _membersSvc.SaveMemberContactInfo(GetMemberID(), model, jwtToken);
        }

        async void OnDeleteEducation(MemberProfileEducationModel educationModel)
        {
            try
            {
                await DeleteEducation(educationModel);
                await GetMemberEducation();
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        async void OnEditEducation(MemberProfileEducationModel educationModel)
        {
            Preferences.Set("schoolimage", educationModel.schoolImage);
            Preferences.Set("major",educationModel.major);
            Preferences.Set("degree",educationModel.degree);
            Preferences.Set("year",educationModel.yearClass);
            Preferences.Set("competitionlevel", educationModel.sportLevelType);
            Preferences.Set("schoolType", educationModel.schoolType);
            Preferences.Set("schoolID", educationModel.schoolID);
            Preferences.Set("schoolName", educationModel.schoolName);
            await Application.Current.MainPage.Navigation.PushModalAsync(new ProfileUpdateEducationPage(this));
        }

        async void OnAddNewEducation()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ProfileAddEducationPage(this));
        }

        async void OnGotoPlaylistPage(YoutubePlayListModel p)
        {
            Preferences.Set("PlayListID", p.Id);
            Preferences.Set("PlayListTitle", p.Title);
            await Shell.Current.GoToAsync("playlistvideos");
        }

        public async Task DeleteEducation(MemberProfileEducationModel educationModel)
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                await _membersSvc.RemoveSchool(GetMemberID(), educationModel.schoolID, educationModel.schoolType, jwtToken);
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task UpdateEducation(MemberProfileEducationModel schoolInfo)
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                await _membersSvc.UpdateSchool(GetMemberID(), schoolInfo, jwtToken);
                await GetMemberEducation();
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task AddNewEducation(MemberProfileEducationModel schoolInfo)
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                await _membersSvc.AddNewSchool(GetMemberID(), schoolInfo, jwtToken);
                await GetMemberEducation();
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task GetMemberEducation()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                ProfileEducation = await _membersSvc.GetMemberEducationInfo(int.Parse(GetMemberID()), jwtToken);
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        private async Task GetStates()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                States = await _commonsSvc.GetStates(jwtToken);
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        private async Task GetSchools()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                Schools = await _commonsSvc.GetSchoolsByState("AZ", "3", jwtToken);
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        async Task GetPlayListAsync()
        {
            List<YoutubePlayListModel> rn = await GetPlayList();
            Playlist = rn;
            IsRefreshing = false;
        }

        public async Task<List<YoutubePlayListModel>> GetPlayList()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                return await _membersSvc.GetMemberPlaylists(GetMemberID(), jwtToken);
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
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
            return new List<YoutubePlayListModel>();
        }

        public async Task SaveChannelID(string channelID)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            await _membersSvc.SaveChannelID(GetMemberID(), channelID, jwtToken);
        }

        public async Task SaveInstagramURL(string url)
        {
            string jwtToken = Preferences.Get("AccessToken", "");
            await _membersSvc.SaveInstagramURL(GetMemberID(), url, jwtToken);
        }

        private string GetMemberID()
        {
            string memberID = "0";
            string isLoginUser = Preferences.Get("ProfileLoginUser", "yes");
            if (isLoginUser == "yes")
            {
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                {
                    memberID = Preferences.Get("UserID", "");
                }
            }
            else if (isLoginUser== "no")
            {
                if (!String.IsNullOrEmpty(Preferences.Get("ProfileID", "")))
                {
                    memberID = Preferences.Get("ProfileID", "");
                }
            }
            return memberID;
        }

        public async void LogException(string msg, string stackTrace, string jwt)
        {
            await _membersSvc.LogException(msg, stackTrace, jwt);
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
