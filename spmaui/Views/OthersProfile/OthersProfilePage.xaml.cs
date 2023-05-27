using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class OthersProfilePage : ContentPage
{
    private readonly ProfileViewModel _profileViewModel;

    public OthersProfilePage(ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        _profileViewModel = profileViewModel;
        BindingContext = profileViewModel;
        imgProfile.Source = Preferences.Get("ProfileImage", "");
        lblName.Text = Preferences.Get("ProfileName", "");
        lblTitle.Text = Preferences.Get("ProfileTitle", "");
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
        var somevariablefromviewmodel = ((ProfileViewModel)BindingContext).ProfileContactInfo.Instagram;
        string url = somevariablefromviewmodel;
        if (String.IsNullOrEmpty(url))
        {
            await DisplayAlert("Profile Settings", "The profiler instagram URL information not  provided!", "Ok");
        }
        else
        {
            await Browser.OpenAsync(url);
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
        await Shell.Current.GoToAsync("playlistvideos");
    }

    async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}
