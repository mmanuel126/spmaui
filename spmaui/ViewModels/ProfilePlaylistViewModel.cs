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
    public class ProfilePlaylistViewModel : INotifyPropertyChanged
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

        List<YoutubeVideosListModel> _Videos;

        public List<YoutubeVideosListModel> Videos
        {
            get
            {
                return _Videos;
            }
            set
            {
                _Videos = value;
                OnPropertyChanged();
            }
        }

        async Task DoRefreshPosts()
        {
            Videos.Clear();
            Videos = new List<YoutubeVideosListModel>();

            IsRefreshing = true;
            this.Videos = await this.GetPlaylistVideos();
            IsRefreshing = false;
        }

        private IMembers _membersSvc;

        public ProfilePlaylistViewModel(IMembers membersSvc)
        {
            RefreshCommand = new Command(OnRefreshCommandExecuted);
            _membersSvc = membersSvc;
            Videos = new List<YoutubeVideosListModel>();
            Task.Run(() => GetPlaylistVideosAsync());
        }

        async Task GetPlaylistVideosAsync()
        {
            List<YoutubeVideosListModel> rn = await GetPlaylistVideos();
            Videos = rn;
        }

        public async Task<List<YoutubeVideosListModel>> GetPlaylistVideos()
        {
            try
            {
                string profileID = Preferences.Get("PlayListID", "");
                string jwtToken = Preferences.Get("AccessToken", "");
                return await _membersSvc.GetPlaylistVideos(profileID, jwtToken);
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
                return new List<YoutubeVideosListModel>(); ;
            }
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
