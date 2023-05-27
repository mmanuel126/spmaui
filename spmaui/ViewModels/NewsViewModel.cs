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
    public class NewsViewModel : INotifyPropertyChanged
    {
        public ICommand RefreshCommand { get; set; }

        private void OnRefreshCommandExecuted() => Task.Run(() => DoRefreshNews()); 

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

        public async Task DoRefreshNews()
        {
            IsRefreshing = true;
            this.News = await this.GetRecentNews();
            IsRefreshing = false;
        }

        private readonly IMembers _membersSvc;

        public NewsViewModel(IMembers membersSvc)
        {
            RefreshCommand = new Command(OnRefreshCommandExecuted);
            _membersSvc = membersSvc;
            News = new List<RecentNewsModel>();
            Task.Run(() => GetNewsAsync().Wait());
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
                IsRefreshing = false;
                if (ex.GetType() == typeof(HttpRequestException))
                {
                    await App.Current.MainPage.DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                    await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                }
            }
        }

        public async Task<List<RecentNewsModel>> GetRecentNews()
        {
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            return await _membersSvc.GetRecentNews(jwtToken);
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
