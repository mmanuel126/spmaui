using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using spmaui.Models;
using spmaui.Services;

namespace spmaui.ViewModels
{
    public class MessageDetailsViewModel : INotifyPropertyChanged
    {
        //public ICommand SendMessageCommand { get; private set; }

        MessageDetails _messageDetail;

        public MessageDetails MessageDetails
        {
            get { return _messageDetail; }
            set { _messageDetail = value; OnPropertyChanged(); }
        }

        private string messageText;
        public string MessageText
        {
            get { return messageText; }
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                this.isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        private readonly IMessages _messagesSvc;
        public MessageDetailsViewModel(IMessages messagesSvc)
        {
            _messagesSvc = messagesSvc;
            //SendMessageCommand = new Command(OnSendMessageCommandAsync);
            Task.Run(() => GetMessageDetails());
        }
        /*
        void OnSendMessageCommandAsync(object msgObj)
        {
            MessageDetailsViewModel instance = this;
            if (!string.IsNullOrWhiteSpace(instance.MessageText))
            {
                Task.Run(() => SendMessage());
            }
        }
        */
        public async Task SendMessage(MessageDetails msgData)
        {
            try
            {
                IsLoading = true;
                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (Preferences.Get("UserID", "") != null)
                {
                    memberID = Preferences.Get("UserID", "");
                }
                await _messagesSvc.SendMessage(memberID, msgData.SenderID, msgData.Subject, msgData.Body, jwtToken);
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
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
        /*
        private  async Task  SendMessge()
        { 
            IsLoading = true;
            MessageDetailsViewModel instance = this;
            MessageDetails msgData = instance.MessageDetails;

            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (Preferences.Get("UserID", "") != null)
            {
                memberID = Preferences.Get("UserID", "");
            }
            await _messagesSvc.SendMessage(memberID, msgData.SenderID, msgData.Subject, instance.messageText, jwtToken);
            IsLoading = false;
        }
        */
        public async Task GetMessageDetails()
        {
            try
            {
                IsLoading = true;
                string jwtToken = Preferences.Get("AccessToken", "");
                string messageID = Preferences.Get("MessageID", "");
                string senderID = Preferences.Get("SenderID", "");
                var pcInfoLst = await _messagesSvc.GetMessageInfoByID(messageID, "Inbox", jwtToken);
                if (pcInfoLst != null && pcInfoLst.Count != 0)
                {
                    MessageDetails = pcInfoLst[0];
                    MessageDetails.SenderID = senderID;
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
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

        public async void LogException(string msg, string stackTrace, string jwt)
        {
            await _messagesSvc.LogException(msg, stackTrace, jwt);
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
