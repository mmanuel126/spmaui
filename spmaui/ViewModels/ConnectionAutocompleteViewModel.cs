using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using spmaui.Models;
using spmaui.Services;

namespace spmaui.ViewModels
{
    
    public class ConnectionAutocompleteViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ContactsModel> Connections { get; set; }

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
            this.Connections = GetConnectionsAsync(); 
        }
        /*
        void OnSendMessageCommandAsync(object msgObj)
        { 
            ConnectionAutocompleteViewModel instance = this;
            ContactsModel cm = instance.Connections[0];
            string connectionID = cm.connectionID;
            if (!string.IsNullOrWhiteSpace(instance.MessageText))
            {
               // SendMessage(connectionID, instance.MessageText, instance.SubjectText);
            }
        }
        */
        public  async Task SendMessage(string connectionID, string msg, string subject)
        {
            IsLoading = true;
            spmaui.Services.Messages msgSvc = new spmaui.Services.Messages();
            string jwtToken = Preferences.Get("AccessToken","");
            string memberID = "0";
            if (Preferences.Get("UserID","") != null)
            {
                memberID = Preferences.Get("UserID","").ToString();
            }
            await msgSvc.SendMessage(memberID, connectionID, subject, msg, jwtToken);
            IsLoading = false;
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public ObservableCollection<ContactsModel> GetConnectionsAsync()
        {
            ObservableCollection<ContactsModel> rn = GetAllConnectionAsync();
            return rn;
        }

        public ObservableCollection<ContactsModel> GetAllConnectionAsync()
        {
            try
            {
                Connections svc = new Connections();
                string jwtToken = Preferences.Get("AccessToken", "").ToString();
                int memberID = 0;
                if (Preferences.Get("UserID", "").ToString() != null)
                    memberID = Convert.ToInt32(Preferences.Get("UserID", "").ToString());

                ObservableCollection<ContactsModel> result = svc.GetMyConnectionsList(memberID.ToString(), jwtToken);

                var myConnectionList = new ObservableCollection<Conversation>();

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

                        result[i].Name = result[i].friendName;
                        result[i].Image = result[i].picturePath;

                        i++;
                    }
                }
                return result;
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
                return new ObservableCollection<ContactsModel>();
            }
        }

        public async void LogException(string msg, string stackTrace, string jwt)
        {
            Connections svc = new Connections();
            await svc.LogException(msg, stackTrace, jwt);
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
