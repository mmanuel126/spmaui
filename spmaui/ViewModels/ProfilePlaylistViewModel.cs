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
    public class ProfilePlaylistViewModel : INotifyPropertyChanged
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

        private Members _membersSvc;

        public ProfilePlaylistViewModel()
        {
            RefreshCommand = new Command(OnRefreshCommandExecuted);
            _membersSvc = new Members();
            Videos = new List<YoutubeVideosListModel>();
            GetPlaylistVideosAsync();
        }

        async Task GetPlaylistVideosAsync()
        {
            List<YoutubeVideosListModel> rn = await GetPlaylistVideos();
            Videos = rn;
        }

        public async Task<List<YoutubeVideosListModel>> GetPlaylistVideos()
        {
            string profileID = Preferences.Get("PlayListID","");
            string jwtToken = Preferences.Get("AccessToken", "");
            return await  _membersSvc.GetPlaylistVideos(profileID, jwtToken);
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
