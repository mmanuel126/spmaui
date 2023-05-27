
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
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        public ICommand RefreshCommand { get; set; }
        public Command<ContactsModel> DropCommand { get; set; }
        public Command<ContactsModel> AcceptCommand { get; set; }
        public Command<ContactsModel> RejectCommand { get; set; }

        private void OnRefreshCommandExecuted() => DoRefreshCon();

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

        private readonly Connections _conSvc;
       
        public ConnectionViewModel()
        {
            DropCommand = new Command<ContactsModel>(OnDropConnection);
            AcceptCommand = new Command<ContactsModel>(OnAcceptConnection);
            RejectCommand = new Command<ContactsModel>(OnRejectConnection);

            RefreshCommand = new Command(OnRefreshCommandExecuted);
            _conSvc = new Connections();
            Connection = new List<ContactsModel>();
            ConRequest = new List<ContactsModel>();
            IsRefreshing = true;
            GetMyConnectionsAsync();
            GetMyConRequestsAsync();
            IsRefreshing = false;
        }

        async void OnDropConnection(ContactsModel contacts)
        {
            await DeleteConnection(contacts.connectionID);
            await DoRefreshCon();
        }

        async void OnAcceptConnection(ContactsModel contacts)
        {
            await AcceptConnection(contacts.connectionID);
            await DoRefreshCon();
        }

        async void OnRejectConnection(ContactsModel contacts)
        {
            await RejectConnection(contacts.connectionID);
            await DoRefreshCon();
        }

        async Task GetMyConnectionsAsync()
        {
            List<ContactsModel> rn = await GetMyConnections("connections");
            Connection = rn;
        }

        async Task GetMyConRequestsAsync()
        {
            List<ContactsModel> rn = await GetMyConnections("requests");
            ConRequest = rn;
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
                    string img = App.AppSettings.AppImagesURL + "/images/members/default.png";
                    if (r.picturePath != null || r.picturePath != "")
                    {
                        img = App.AppSettings.AppImagesURL + "/images/members/" + r.picturePath;
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
            Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            string memberID = "0";
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Preferences.Get("UserID", "").ToString();
            await svc.DeleteConnection(memberID.ToString(), connectionID, jwtToken);
        }

        public async Task AcceptConnection(string connectionID)
        {
            Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID", "").ToString());

            await svc.AcceptRequest(memberID.ToString(), connectionID, jwtToken);

        }

        public async Task RejectConnection(string connectionID)
        {
            Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID", "").ToString());

            await svc.RejectRequest(memberID.ToString(), connectionID, jwtToken);

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


/* using System;
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
    public class ConnectionViewModel : BaseViewModel
    { 
        private readonly IConnections connectionService;
        public ObservableCollection<ContactsModel> Connections { get; set; } = new () ;

        public Command GetMyConnectionsCommand { get; }
        public ConnectionViewModel(IConnections connService)
        {
            this.connectionService = connService;
            GetMyConnectionsCommand = new Command(async () => await GetMyConnetionsAsync());
        }

        public async Task GetMyConnetionsAsync()
        {
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            string memberID = "0";
            if (Preferences.Get("UserID","").ToString() != null)
                memberID = Preferences.Get("UserID", "").ToString();
            var data = await connectionService.GetMyConnections(memberID, jwtToken);
            if (Connections.Count != 0)
            {
                Connections.Clear();
            }
            foreach (var connection in Connections)
            {
                Connections.Add(connection);
            }
            //Connections = new ObservableCollection<ContactsModel>(data);
            //PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Connections)));
        }

*/
        //public ICommand RefreshCommand { get; set; }
        //public Command <ContactsModel> DropCommand { get; set; }

        //async private void OnRefreshCommandExecuted() => await DoRefresh();
        
        //bool isRefreshing;
        //public bool IsRefreshing
        //{
        //    get => isRefreshing;

        //    set
        //    {
        //        isRefreshing = value;

        //        OnPropertyChanged(nameof(IsRefreshing));
        //    }
        //}

        //List<ContactsModel> _Connections;
        //public List<ContactsModel> Connections
        //{
        //    get
        //    {
        //        return _Connections;
        //    }
        //    set
        //    {
        //        _Connections = value;
        //        OnPropertyChanged();
        //    }
        //}

        //async Task DoRefresh()
        //{
        //    Connections.Clear();
        //    Connections = new List<ContactsModel>();
        //    IsRefreshing = true;
        //    this.Connections = await this.GetMyConnections();
        //    IsRefreshing = false;
        //}

        //private ContactsModel _connectionsSvc;

        //public ConnectionViewModel()
        //{
        //    DropCommand = new  Command<ContactsModel>  (OnDropConnection);
        //    RefreshCommand = new Command (OnRefreshCommandExecuted);
        //    _connectionsSvc = new ContactsModel();
        //    Connections = new List<ContactsModel>();
        //    GetConnectionsAsync();
            
        //}

        //async void OnDropConnection(ContactsModel contacts)
        //{
        //    await DeleteConnection(contacts.connectionID);
        //    await DoRefresh();
        //}


        //async Task GetConnectionsAsync()
        //{
        //    string jwtToken = Preferences.Get("AccessToken","").ToString();
        //    List<ContactsModel> rn = await GetMyConnections();
        //    Connections = rn;
        //}

        //public async Task DeleteConnection(string connectionID)
        //{
        //    Connections svc = new Connections();
        //    string jwtToken = Preferences.Get("AccessToken","").ToString();
        //    int memberID = 0;
        //    if (Preferences.Get("UserID","").ToString() != null)
        //        memberID = Convert.ToInt32(Preferences.Get("UserID","").ToString());

        //    await svc.DeleteConnection (memberID.ToString(),connectionID, jwtToken);

        //}

        //public async Task<List<ContactsModel>> GetMyConnections()
        //{
        //    Connections svc = new Connections();
        //    string jwtToken = Preferences.Get("AccessToken","").ToString();
        //    int memberID = 0;
        //    if (Preferences.Get("UserID","").ToString() != null)
        //        memberID = Convert.ToInt32(Preferences.Get("UserID","").ToString());

        //    return await svc.GetMyConnections(memberID.ToString(), jwtToken);
        //  /*  List<ContactsModel> result = await svc.GetMyConnections (memberID.ToString(), jwtToken);
            

        //    var myConnectionList = new ObservableCollection<Conversation>();

        //    if (result != null)
        //    {
        //        //Conversation conv = new Conversation();
        //        int  i = 0;
        //        foreach (var r in result)
        //        {
        //            string img = App.AppSettings.AppImagesURL + "images/members/default.png";
        //            if (r.picturePath != null || r.picturePath != "")
        //            {
        //                img = App.AppSettings.AppImagesURL + "images/members/" + r.picturePath;
        //            }
        //            result[i].picturePath = img;

        //            if (r.titleDesc == null || r.titleDesc=="")
        //            {
        //                result[i].titleDesc = "Unknown Title";
        //            }

        //            i++;
        //        }
        //    }

        //    return result;*/
        //}

        //#region INotifyPropertyChanged
        //public event PropertyChangedEventHandler PropertyChanged;

        //void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        //#endregion
    //}
//}
