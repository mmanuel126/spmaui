using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;
using spmaui.Services;

namespace spmaui.Views;

public partial class SearchPage : ContentPage
{
    
    public SearchPage()
    {
        InitializeComponent();
    }

    async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        var data = label.BindingContext as SearchModel;
        
        Preferences.Set("ProfileID", data.entityID);
        Preferences.Set("ProfileName", data.entityName);
        if (String.IsNullOrEmpty(data.Params)) data.Params = "Unknown Title";
        Preferences.Set("ProfileTitle", data.Params);
        Preferences.Set("ProfileImage", data.picturePath);
        Preferences.Set("ProfileLoginUser", "no");
        await Shell.Current.Navigation.PushModalAsync(new OthersProfilePage(new ProfileViewModel(new Members(), new Commons())));
       // await Shell.Current.GoToAsync("othersprofile");
    }

    async void OnConnectClicked(object sender, EventArgs e)
    {
        try
        {
            var swipeItem = sender as SwipeItem;
            var data = swipeItem.BindingContext as SearchModel;

            bool ans = await DisplayAlert("Connection Request", "Please note the member will have to confirm your request. You should send this request only if you know this person. Are you sure you want to send this connection request?", "Yes", "No");
            if (ans)
            {
                if (!String.IsNullOrEmpty(searchBar.Text))
                {
                    Connections conSvc = new Connections();
                    string jwtToken = Preferences.Get("AccessToken", "").ToString();
                    string memberID = "0";
                    if (Preferences.Get("UserID", "").ToString() != null)
                        memberID = Preferences.Get("UserID", "").ToString();
                    await conSvc.AddConnection(memberID, data.entityID, jwtToken);
                    var Result = conSvc.GetSearchList(memberID, searchBar.Text, jwtToken);
                    searchList.ItemsSource = Result;
                }
                else
                {
                    searchList.ItemsSource = null;
                }
            }
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(HttpRequestException))
            {
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
            else
            {
                Connections conSvc = new Connections();
                await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                await conSvc.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    void OnTextChanged(object sender, EventArgs e)
    {
        try
        {
            if (!String.IsNullOrEmpty(searchBar.Text))
            {
                Connections conSvc = new Connections();
                string jwtToken = Preferences.Get("AccessToken", "").ToString();
                string memberID = "0";
                if (Preferences.Get("UserID", "").ToString() != null)
                    memberID = Preferences.Get("UserID", "").ToString();
                var Result = conSvc.GetSearchList(memberID, searchBar.Text, jwtToken);
                searchList.ItemsSource = Result;
            }
            else
            {
                searchList.ItemsSource = null;
            }
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(HttpRequestException))
            {
               DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
            else
            {
                Connections conSvc = new Connections();
                DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                Task.Run(() => conSvc.LogException(ex.Message, ex.StackTrace, ""));
            }
        }
    }

}
