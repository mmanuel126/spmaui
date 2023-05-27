
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
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        public ICommand RefreshCommand { get; set; }
        public Command<ContactsModel> DropCommand { get; set; }
        public Command<ContactsModel> AcceptCommand { get; set; }
        public Command<ContactsModel> RejectCommand { get; set; }

        public static List<ContactsModel> ConnectionsSearchList { get; private set; } = new List<ContactsModel>();

        private void OnRefreshCommandExecuted() => Task.Run(() => DoRefreshCon());

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

        List<ContactsModel> _Connections;
        List<ContactsModel> _ConRequests;

        public List<ContactsModel> Connection
        {
            get
            {
                return _Connections;
            }
            set
            {
                _Connections = value;
                OnPropertyChanged();
            }
        }

        public List<ContactsModel> ConRequest
        {
            get
            {
                return _ConRequests;
            }
            set
            {
                _ConRequests = value;
                OnPropertyChanged();
            }
        }

        async Task DoRefreshCon()
        {
            Connection.Clear(); ConRequest.Clear();
            Connection = new List<ContactsModel>(); ConRequest= new List<ContactsModel>();
            IsRefreshing = true;
            this.Connection = await this.GetMyConnections("connections"); this.ConRequest = await this.GetMyConnections("requests");
           IsRefreshing = false;
        }

        private readonly IConnections _conSvc;
       
        public ConnectionViewModel(IConnections conSvc)
        {
            _conSvc = conSvc;
            IsRefreshing = true;
            DropCommand = new Command<ContactsModel>(OnDropConnection);
            AcceptCommand = new Command<ContactsModel>(OnAcceptConnection);
            RejectCommand = new Command<ContactsModel>(OnRejectConnection);

            RefreshCommand = new Command(OnRefreshCommandExecuted);
            _conSvc = new Connections();
            Connection = new List<ContactsModel>();
            ConRequest = new List<ContactsModel>();
            Task.Run(() => GetMyConnectionsAsync());
            Task.Run(() => GetMyConRequestsAsync());
           
            ConnectionsSearchList = Connection;
        }

        async void OnDropConnection(ContactsModel contacts)
        {
            isRefreshing = true;
            await DeleteConnection(contacts.connectionID);
            await DoRefreshCon();
        }

        async void OnAcceptConnection(ContactsModel contacts)
        {
            isRefreshing = true;
            await AcceptConnection(contacts.connectionID);
            await DoRefreshCon();
        }

        async void OnRejectConnection(ContactsModel contacts)
        {
            isRefreshing = true;
            await RejectConnection(contacts.connectionID);
            await DoRefreshCon();
        }

        public async Task GetMyConnectionsAsync()
        {
            List<ContactsModel> rn = await GetMyConnections("connections");
            Connection = rn;
            ConnectionsSearchList = Connection;
        }

        public async Task GetMyConRequestsAsync()
        {
            List<ContactsModel> rn = await GetMyConnections("requests");
            ConRequest = rn;
            IsRefreshing = false;
        }

        public async Task<List<ContactsModel>> GetMyConnections(string type)
        {
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            string memberID = "0";
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Preferences.Get("UserID", "").ToString();

            List<ContactsModel> result;
            if (type== "connections")
               result = await _conSvc.GetMyConnections(memberID, jwtToken);
            else 
               result = await _conSvc.GetConnectionRequests (memberID, jwtToken);

            if (result != null)
            {
                int i = 0;
                foreach (var r in result)
                {
                    string img = App.AppSettings.AppMemberImagesURL + "default.png";
                    if (r.picturePath != null || r.picturePath != "")
                    {
                        img = App.AppSettings.AppMemberImagesURL + r.picturePath;
                    }
                    result[i].picturePath = img;

                    if (r.titleDesc == null || r.titleDesc == "")
                    {
                        result[i].titleDesc = "Unknown Title";
                    }
                    i++;
                }
            }
            
            return result;
        }

        public async Task DeleteConnection(string connectionID)
        { 
            //Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            string memberID = "0";
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Preferences.Get("UserID", "").ToString();
            await _conSvc.DeleteConnection(memberID.ToString(), connectionID, jwtToken);
        }

        public async Task AcceptConnection(string connectionID)
        {
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID", "").ToString());

            await _conSvc.AcceptRequest(memberID.ToString(), connectionID, jwtToken);

        }

        public async Task RejectConnection(string connectionID)
        { 
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID", "").ToString());

            await _conSvc.RejectRequest(memberID.ToString(), connectionID, jwtToken);
        }

        public async void LogException(string msg, string stackTrace, string jwt)
        {
            await _conSvc.LogException(msg, stackTrace, jwt);
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