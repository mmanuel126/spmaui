using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        /*
        Preferences.Set("UserID", obj.memberID);
        Preferences.Set("UserEmail", obj.email);
        Preferences.Set("UserName", obj.name);
        Preferences.Set("UserTitle", obj.title);
        Preferences.Set("AccessToken", obj.accessToken);

        if (obj.picturePath != "")
        {
            Preferences.Set("UserImage", obj.picturePath);*/
        imgProfile.Source = App.AppSettings.AppImagesURL + "/images/members/" + Preferences.Get("UserImage", "");
        lblName.Text = Preferences.Get("UserName", "");
        lblTitle.Text = Preferences.Get("UserTitle", "");
    }

    async void OnEducationSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;

        MemberProfileEducationModel nm = (MemberProfileEducationModel)current[0];
        await Launcher.OpenAsync(nm.schoolWebSite);
        ((CollectionView)sender).SelectedItem = null;


        //this.collectionView.SelectedItem = null;
        //if (e.CurrentSelection.Count == 0)
        //    return;
        //var current = e.CurrentSelection;
        //ContactsModel nm = (ContactsModel)current[0];

        //Application.Current.Properties["ProfileID"] = nm.connectionID;
        //Application.Current.Properties["ProfileName"] = nm.friendName;
        //Application.Current.Properties["ProfileTitle"] = nm.titleDesc;
        //Application.Current.Properties["ProfileImage"] = nm.picturePath;
        //await Shell.Current.GoToAsync("profile");
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
