
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
using spmaui;
using spmaui.Views;

namespace spmaui.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        public ICommand RefreshCommand { get; set; }
        public Command <MessageInfoModel> DropCommand { get; set; }
        public Command<MessageInfoModel> OpenCommand { get; set; }

        async private void OnRefreshCommandExecuted() => await DoRefresh();
        
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

        List<MessageInfoModel> _Messages;
        public List<MessageInfoModel> Messages
        {
            get
            {
                return _Messages;
            }
            set
            {
                _Messages = value;
                OnPropertyChanged();
            }
        }

        async Task DoRefresh()
        {
            Messages.Clear();
            Messages = new List<MessageInfoModel>();
            IsRefreshing = true;
            this.Messages = await this.GetMyMessages();
            IsRefreshing = false;
        }

        private readonly ContactsModel _connectionsSvc;

        private readonly IMessages _messagesSvc; 
        public MessageViewModel(IMessages messagesSvc)
        {
            _messagesSvc = messagesSvc;
            IsRefreshing = true;
            DropCommand = new  Command<MessageInfoModel>  (OnDropMessage);
            OpenCommand = new Command<MessageInfoModel>(OnOpenMessage);
            RefreshCommand = new Command (OnRefreshCommandExecuted);
            _connectionsSvc = new ContactsModel();
            Messages = new List<MessageInfoModel>();
            Task.Run(() => GetMessagesAsync().Wait());
        }

        async void OnDropMessage(MessageInfoModel message)
        {
            await DeleteMessage(message.messageID);
            await DoRefresh();
        }

        async void OnOpenMessage(MessageInfoModel message)
        {
            await OpenMessage(message);
            await DoRefresh();
        }

        public async Task OpenMessage(MessageInfoModel message)
        {
            Preferences.Set("MessageID", message.messageID);
            Preferences.Set("SenderID", message.fromID);
            await  Application.Current.MainPage.Navigation.PushModalAsync(new MessageDetailsPage(new MessageDetailsViewModel(new spmaui.Services.Messages())));
        }

        async Task GetMessagesAsync()
        {
            List<MessageInfoModel> rn = await GetMyMessages();
            Messages = rn;
            IsRefreshing = false;
        }

        public async Task DeleteMessage(string messageID)
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                await _messagesSvc.DeleteMessage(messageID, "", jwtToken);
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
                        LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task<List<MessageInfoModel>> GetMyMessages()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                int memberID = 0;
                if (Preferences.Get("UserID", "") != null)
                    memberID = int.Parse(Preferences.Get("UserID", ""));

                List<MessageInfoModel> result = await _messagesSvc.GetMemberMessages(memberID, "Inbox", "All", jwtToken);
                return result;
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
                        LogException(ex.Message, ex.StackTrace, "");
                    }
                });
                return new List<MessageInfoModel>();
            }
        }

        public async void LogException(string msg, string stackTrace, string jwt)
        {
            await _messagesSvc.LogException(msg, stackTrace, jwt);
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
