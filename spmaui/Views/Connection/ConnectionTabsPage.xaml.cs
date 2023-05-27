

using Microsoft.Maui.Controls.PlatformConfiguration;
using spmaui.ViewModels;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Syncfusion.Maui.Core;
using spmaui.Models;
using spmaui.Services;

namespace spmaui.Views.Connection
{
    public partial class ConnectionTabsPage : ContentPage
    {
        private readonly ConnectionViewModel _connectionViewModel;
        public ConnectionTabsPage(ConnectionViewModel connectionViewModel)
        {
            InitializeComponent();
            _connectionViewModel = connectionViewModel;
            this.BindingContext = _connectionViewModel; 
          
            On<iOS>().SetUseSafeArea(true); 
        }

        async void OnRefreshConnection_Clicked(object sender, EventArgs e)
        {
            try
            {
                var x = (ConnectionViewModel)this.BindingContext;
                x.IsRefreshing = true;
                await x.GetMyConnectionsAsync();
                await x.GetMyConRequestsAsync();
                this.BindingContext = x;
                x.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(HttpRequestException))
                {
                    await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                }
                else
                {
                    await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                    _connectionViewModel.LogException(ex.Message, ex.StackTrace, "");
                }
            }
        }

        async void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
        {
            try
            {
                var data = (ContactsModel)e.Parameter;

                Preferences.Set("ProfileID", data.connectionID);
                Preferences.Set("ProfileName", data.friendName);
                Preferences.Set("ProfileTitle", data.titleDesc);
                Preferences.Set("ProfileImage", data.picturePath);
                Preferences.Set("ProfileLoginUser", "no");
                await Shell.Current.Navigation.PushModalAsync(new OthersProfilePage(new ProfileViewModel(new Members(), new Commons())));
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(HttpRequestException))
                {
                    await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                }
                else
                {
                    await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                    _connectionViewModel.LogException(ex.Message, ex.StackTrace, "");
                }
            }
        }
    }
}
