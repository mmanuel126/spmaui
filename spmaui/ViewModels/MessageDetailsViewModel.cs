using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using sp_maui.Models;
using sp_maui.Services;

namespace sp_maui.ViewModels
{
    public class MessageDetailsViewModel : INotifyPropertyChanged
    {
        public ICommand SendMessageCommand { get; private set; }

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

        public MessageDetailsViewModel()
        {
            SendMessageCommand = new Command(OnSendMessageCommandAsync);
            GetMessageDetails();
        }

        void OnSendMessageCommandAsync(object msgObj)
        {
            MessageDetailsViewModel instance = this;
            if (!string.IsNullOrWhiteSpace(instance.MessageText))
            {
                SendMessge();
            }
        }

        private  async Task  SendMessge()
        { 
            IsLoading = true;
            sp_maui.Services.Messages msgSvc = new sp_maui.Services.Messages();

            MessageDetailsViewModel instance = this;
            MessageDetails cm = instance.MessageDetails;

            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (Preferences.Get("UserID", "") != null)
            {
                memberID = Preferences.Get("UserID", "");
            }
            await msgSvc.SendMessage(memberID, cm.SenderID, cm.Subject, instance.messageText, jwtToken);
            IsLoading = false;
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task GetMessageDetails()
        {
            IsLoading = true;
            string jwtToken = Preferences.Get("AccessToken", "");
            string messageID = Preferences.Get("MessageID", "");
            string senderID = Preferences.Get("SenderID", "");
            sp_maui.Services.Messages svc = new sp_maui.Services.Messages();
            var pcInfoLst = await svc.GetMessageInfoByID(messageID,"Inbox", jwtToken);
            if (pcInfoLst != null && pcInfoLst.Count != 0)
            {
                MessageDetails = pcInfoLst[0];
                MessageDetails.SenderID = senderID;
            }
            IsLoading = false;
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
