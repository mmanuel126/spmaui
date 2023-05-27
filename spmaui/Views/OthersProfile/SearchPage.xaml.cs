using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class OthersProfilePage : ContentPage
{
    
    public OthersProfilePage()
    {
        InitializeComponent();
        imgProfile.Source = Preferences.Get("ProfileImage", "");
        lblName.Text = Preferences.Get("ProfileName", "");
        lblTitle.Text = Preferences.Get("ProfileTitle", "");
    }


    async void OnRefreshProfile_Clicked(object sender, EventArgs e)
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
}
