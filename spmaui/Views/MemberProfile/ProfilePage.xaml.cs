using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.Services;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _profileViewModel;

    public ProfilePage(ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        _profileViewModel = profileViewModel;
        BindingContext = profileViewModel;
        
        Preferences.Set("ProfileLoginUser", "yes");
        imgProfile.Source = App.AppSettings.AppMemberImagesURL + Preferences.Get("UserImage", "");
        lblName.Text = Preferences.Get("UserName", "");
        lblTitle.Text = Preferences.Get("UserTitle", "");
    }


    async void OnRefreshProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var x = (ProfileViewModel)this.BindingContext;
            x.IsRefreshing = true;
            await x.GetMemberBasicInfo();
            await x.GetMemberContactInfo();
            await x.GetMemberEducation();
            await x.GetPlayList();
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
                _profileViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void OnEducationSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;

        MemberProfileEducationModel nm = (MemberProfileEducationModel)current[0];
        await Launcher.OpenAsync(nm.webSite);
        ((CollectionView)sender).SelectedItem = null;
    }

    async void OnPhotosButtonClicked(object sender, EventArgs args)
    {
        try
        {
            var somevariablefromviewmodel = ((ProfileViewModel)BindingContext).ProfileContactInfo.Instagram;
            string url = somevariablefromviewmodel;
            Uri outUri;

            if (String.IsNullOrEmpty(url))
            {
                await DisplayAlert("Profile Settings", "The profiler instagram URL information not  provided!", "Ok");
            }
            else if (Uri.TryCreate(url, UriKind.Absolute, out outUri)
                && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
            {
                await Browser.OpenAsync(url);
            }
            else
            {
                await DisplayAlert("Profile Settings", "The profile instagram URL:" + url + " provided is not valid. Please go to edit profile and update your instagram URL.", "Ok");
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
                await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                _profileViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void OnVideoSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ((CollectionView)sender).SelectedItem = null;
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;
        YoutubePlayListModel nm = (YoutubePlayListModel)current[0];

        Preferences.Set("PlayListID", nm.Id);
        Preferences.Set("PlayListTitle", nm.Title);
        await Navigation.PushModalAsync(new ProfilePlaylistPage(new ProfilePlaylistViewModel(new Members())));
    }
}
