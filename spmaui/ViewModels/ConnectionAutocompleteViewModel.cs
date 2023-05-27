using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using sp_maui.Models;
using sp_maui.Services;

//namespace sp_mobile.ViewModels
//{
//    public class ConnectionAutocompleteViewModel
//    {
//        public ConnectionAutocompleteViewModel()
//        {
//        }
//    }
//}

namespace sp_maui.ViewModels
{
    //public class Employee
    //{
    //    private string image;
    //    public string Image
    //    {
    //        get { return image; }
    //        set { image = value; }
    //    }
    //    private string name;
    //    public string Name
    //    {
    //        get { return name; }
    //        set { name = value; }
    //    }
    //}
    // Create EmployeeViewModel class holds the collection of employee data.

    public class ConnectionAutocompleteViewModel : INotifyPropertyChanged
    {

        public ICommand SendMessageCommand { get; private set; }

        private ObservableCollection<ContactsModel> connectionCollection;

        public ObservableCollection<ContactsModel> ConnectionCollection
        {
            get { return connectionCollection; }
            set { connectionCollection = value; }
        }


        private string messageText;
        public string MessageText
        {
            get { return messageText; }
            set
            {
                messageText = value;
                RaisePropertyChanged("MessageText");
            }
        }

        private string subjectText;
        public string SubjectText
        {
            get { return subjectText; }
            set
            {
                subjectText = value;
                RaisePropertyChanged("SubjectText");
            }
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                this.isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public  ConnectionAutocompleteViewModel()
        {
            SendMessageCommand = new Command(OnSendMessageCommandAsync);
            GetConnectionsAsync();
        }

        void OnSendMessageCommandAsync(object msgObj)
        { 
            ConnectionAutocompleteViewModel instance = this;
            ContactsModel cm = instance.connectionCollection[0];
            string connectionID = cm.connectionID;
            if (!string.IsNullOrWhiteSpace(instance.MessageText))
            {
                SendMessge(connectionID, instance.MessageText, instance.SubjectText);
            }
        }


        private async Task SendMessge(string connectionID, string msg, string subject)
        {
            //IsLoading = true;
            //Messages msgSvc = new Messages();
            //Preferences.Set("IsUserLogin", "true");
            //string jwtToken = Preferences.Get("AccessToken","");
            //string memberID = "0";
            //if (Preferences.Get("UserID","") != null)
            //{
            //    memberID = Preferences.Get("UserID","").ToString();
            //}
            //await msgSvc.SendMessage(memberID, connectionID, subject, msg, jwtToken);
            //IsLoading = false;
            //await Application.Current.MainPage.Navigation.PopAsync();
        }


        async Task GetConnectionsAsync()
        {
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            ObservableCollection<ContactsModel> rn = await GetAllConnectionAsync();
            ConnectionCollection = rn;
        }

        public async Task<ObservableCollection<ContactsModel>> GetAllConnectionAsync()
        {
            Connections svc = new Connections();
            string jwtToken = Preferences.Get("AccessToken","").ToString();
            int memberID = 0;
            if (Preferences.Get("UserID","").ToString() != null)
                memberID = Convert.ToInt32(Preferences.Get("UserID","").ToString());

            ObservableCollection<ContactsModel> result =    await svc.GetMyConnectionsList(memberID.ToString(), jwtToken);

            var myConnectionList = new ObservableCollection<Conversation>();

            if (result != null)
            {
                //Conversation conv = new Conversation();
                int i = 0;
                foreach (var r in result)
                {
                    string img = App.AppSettings.AppImagesURL + "images/members/default.png";
                    if (r.picturePath != null || r.picturePath != "")
                    {
                        img = App.AppSettings.AppImagesURL + "images/members/" + r.picturePath;
                    }
                    result[i].picturePath = img;

                    if (r.titleDesc == null || r.titleDesc == "")
                    {
                        result[i].titleDesc = "Unknown Title";
                    }

                    result[i].Name = result[i].friendName;
                    result[i].Image = result[i].picturePath;

                    i++;
                }
            }
            
            return  result;
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
