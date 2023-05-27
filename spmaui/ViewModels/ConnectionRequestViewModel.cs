
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
//using Syncfusion.XForms.PopupLayout;

namespace sp_maui.ViewModels
{
    public class ConnectionRequestViewModel : INotifyPropertyChanged
    {
        //private SfPopupLayout popupLayout;

        public ICommand RefreshCommand { get; set; }
        public Command <ContactsModel> AcceptCommand { get; set; }
        public Command<ContactsModel> RejectCommand { get; set; }

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

        List<ContactsModel> _ConnectionRequests;

        public List<ContactsModel> ConnectionRequests
        {
            get
            {
                return _ConnectionRequests;
            }
            set
            {
                _ConnectionRequests = value;
                OnPropertyChanged();
            }
        }

        async Task DoRefresh()
        {

            ConnectionRequests.Clear();
            ConnectionRequests = new List<ContactsModel>();
            IsRefreshing = true;
            this.ConnectionRequests = await this.GetMyConnectionRequests();
            IsRefreshing = false;
        }

        private ContactsModel _connectionsSvc;

        public ConnectionRequestViewModel()
        {
           // popupLayout = new SfPopupLayout();
            AcceptCommand = new Command<ContactsModel>(OnAcceptConnection);
            RejectCommand = new Command<ContactsModel> (OnRejectConnection);

            RefreshCommand = new Command (OnRefreshCommandExecuted);
            _connectionsSvc = new ContactsModel();
            ConnectionRequests = new List<ContactsModel>();
            GetConnectionRequestsAsync();
        }

        async void OnAcceptConnection(ContactsModel contacts)
        {
            await AcceptConnection(contacts.connectionID);
            await DoRefresh();
        }

        async void OnRejectConnection(ContactsModel contacts)
        {
            await RejectConnection(contacts.connectionID);
            await DoRefresh();
        }


        async Task GetConnectionRequestsAsync()
        {
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            List<ContactsModel> rn = await GetMyConnectionRequests();
            ConnectionRequests = rn;
        }

        public async Task AcceptConnection(string connectionID)
        {
            Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID","").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID","").ToString());

            await svc.AcceptRequest (memberID.ToString(),connectionID, jwtToken);

        }

        public async Task RejectConnection(string connectionID)
        {
            Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID","").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID","").ToString());

            await svc.RejectRequest (memberID.ToString(), connectionID, jwtToken);

        }

        public async Task<List<ContactsModel>> GetMyConnectionRequests()
        {
            Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID","").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID","").ToString());

            List<ContactsModel> result = await svc.GetConnectionRequests  (memberID.ToString(), jwtToken);

            var myConnectionList = new ObservableCollection<Conversation>();

            if (result != null)
            {
                //Conversation conv = new Conversation();
                int  i = 0;
                foreach (var r in result)
                {
                    
                    string img = App.AppSettings.AppImagesURL + "/images/members/default.png";
                    if (r.picturePath != null || r.picturePath != "")
                    {
                        img = App.AppSettings.AppImagesURL + "/images/members/" + r.picturePath;
                    }
                    result[i].picturePath = img;

                    if (r.titleDesc == null || r.titleDesc=="")
                    {
                        result[i].titleDesc = "Unknown Title";
                    }

                    i++;
                }
            }

            return result;
        }


        public void DisplayAlert2(string displayText, string bodyText, string accepttext, string declinetext)
        {

            Label popupContent;

            DataTemplate contentTemplateView = new DataTemplate(() =>
            {
                popupContent = new Label();
                popupContent.VerticalOptions = LayoutOptions.Center;
                popupContent.HorizontalOptions = LayoutOptions.Center;
                popupContent.Text = bodyText;
                return popupContent;
            });

            //popupLayout.PopupView.HeaderTitle = displayText;
            //popupLayout.PopupView.ContentTemplate = contentTemplateView;
            //popupLayout.PopupView.ShowFooter = true;
            //popupLayout.PopupView.ShowCloseButton = false;
            //popupLayout.PopupView.AutoSizeMode = AutoSizeMode.Height;
            //popupLayout.PopupView.AppearanceMode = AppearanceMode.TwoButton;

            ////// Configure our Accept button   
            //popupLayout.PopupView.AcceptButtonText = accepttext;
            //popupLayout.PopupView.AcceptCommand = new Command(() =>
            //{
            //    popupLayout.IsOpen = false;
            //});
            //popupLayout.PopupView.PopupStyle.AcceptButtonTextColor = Color.Black;
            //popupLayout.PopupView.PopupStyle.AcceptButtonBackgroundColor = Color.White;

            //// Configure our Decline button   
            //popupLayout.PopupView.DeclineButtonText = declinetext;
            //popupLayout.PopupView.DeclineCommand = new Command(() =>
            //{
            //    popupLayout.IsOpen = false;
            //});
            //popupLayout.PopupView.PopupStyle.DeclineButtonTextColor = Color.Black;
            //popupLayout.PopupView.PopupStyle.DeclineButtonBackgroundColor = Color.White;

            //// Show the popup and wait for user to close   
            //popupLayout.IsOpen = true;
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
