
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
using sp_maui;

namespace sp_maui.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        public ICommand RefreshCommand { get; set; }
        public Command <MessageInfoModel> DropCommand { get; set; }

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

        public MessageViewModel()
        {
            DropCommand = new  Command<MessageInfoModel>  (OnDropMessage);
            RefreshCommand = new Command (OnRefreshCommandExecuted);
            _connectionsSvc = new ContactsModel();
            Messages = new List<MessageInfoModel>();
            GetMessagesAsync();
            
        }

        async void OnDropMessage(MessageInfoModel message)
        {
            await DeleteMessage(message.messageID);
            await DoRefresh();
        }


        async Task GetMessagesAsync()
        {
           
            List<MessageInfoModel> rn = await GetMyMessages();
            Messages = rn;
        }

        public async Task DeleteMessage(string messageID)
        {
            sp_maui.Services.Messages svc = new sp_maui.Services.Messages();
            string jwtToken = Preferences.Get("AccessToken", "");
            await svc.DeleteMessage (messageID,"", jwtToken);
        }

        public async Task<List<MessageInfoModel>> GetMyMessages()
        {
            sp_maui.Services.Messages svc = new sp_maui.Services.Messages();
            string jwtToken = Preferences.Get("AccessToken", "");
            int memberID = 0;
            if (Preferences.Get("UserID", "") != null)
                memberID = int.Parse( Preferences.Get("UserID", ""));

            List<MessageInfoModel> result = await svc.GetMemberMessages (memberID,"Inbox","All", jwtToken);
            return result;
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
