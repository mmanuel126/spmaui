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
    public class NewsViewModel : INotifyPropertyChanged
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

        List<RecentNewsModel> _News;

        public List<RecentNewsModel> News
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
            News = new List<RecentNewsModel>();
            IsRefreshing = true;
            this.News = await this.GetRecentNews();
            IsRefreshing = false;
        }

        private Members _membersSvc;

        public NewsViewModel()
        {
            RefreshCommand = new Command(OnRefreshCommandExecuted);
            _membersSvc = new Members();
            News = new List<RecentNewsModel>();
            GetNewsAsync();
        }

        async Task GetNewsAsync()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "").ToString();
                List<RecentNewsModel> rn = await GetRecentNews();
                News = rn;
            }
            catch (Exception ex)
            {
                var x = ex.Message;

            }
        }

        public async Task<List<RecentNewsModel>> GetRecentNews()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "").ToString();

                return await _membersSvc.GetRecentNews(jwtToken);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return null;
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
