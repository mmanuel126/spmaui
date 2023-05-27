using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.Services;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class PrivacySettingsPage : ContentPage
{
	public PrivacySettingsPage()
	{
		InitializeComponent();
    }

    async void SaveProfileSettingsButton_Clicked(System.Object sender, System.EventArgs e)
    {
        //DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
        Settings s = new Settings();
        string jwtToken = Preferences.Get("AccessToken", "");
        string memberID = "0";
        if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            memberID = Preferences.Get("UserID", "");

        PrivacySettingsModel psm = new PrivacySettingsModel();
        if (BasicInfo.SelectedIndex != -1)
            psm.BasicInfo = BasicInfo.SelectedIndex.ToString();
        else psm.BasicInfo = "0";

        if (PersonalInfo.SelectedIndex != -1)
            psm.PersonalInfo = PersonalInfo.SelectedIndex.ToString();
        else
            psm.PersonalInfo = "0";

        if (Connections.SelectedIndex != -1)
            psm.ContactInfo = Connections.SelectedIndex.ToString();
        else
            psm.ContactInfo = "0";

        if (Education.SelectedIndex != -1)
            psm.Education = Education.SelectedIndex.ToString();
        else
            psm.Education = "0";

        if (MobilePhone.SelectedIndex != -1)
            psm.MobilePhone = MobilePhone.SelectedIndex.ToString();
        else
            psm.MobilePhone = "0";

        if (OtherPhone.SelectedIndex != -1)
            psm.OtherPhone = OtherPhone.SelectedIndex.ToString();
        else
            psm.OtherPhone = "0";

        if (EmailAddress.SelectedIndex != -1)
            psm.EmailAddress = EmailAddress.SelectedIndex.ToString();
        else
            psm.EmailAddress = "0";

        psm.Profile = "1";
        psm.PhotosTagOfYou = "0";
        psm.VideosTagOfYou = "0";
        psm.WorkInfo = "0";
        psm.IMdisplayName = "0";

        await s.SaveProfileSettings(memberID, psm, jwtToken);
        //DependencyService.Get<ILoadingPageService>().HideLoadingPage();
        await DisplayAlert("Saving Privacy Settings...", "Privacy settings were successfully saved!", "Ok");
    }

    async void PrivacySearchButton_Clicked(System.Object sender, System.EventArgs e)
    {
        DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
        Settings s = new Settings();
        string jwtToken = Preferences.Get("AccessToken", "");
        string memberID = "0";
        if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
            memberID = Preferences.Get("UserID", "");

        PrivacySettingsModel psm = new PrivacySettingsModel();
        if (Visibility.SelectedIndex != -1)
            psm.Visibility = Visibility.SelectedIndex.ToString();
        else psm.Visibility = "0";

        psm.ViewProfilePicture = ProfilePicture.IsToggled;
        psm.ViewFriendsList = ConnectionsList.IsToggled;
        psm.ViewLinksToRequestAddingYouAsFriend = RequestToAdd.IsToggled;
        psm.ViewLinkTSendYouMsg = SendYouMsg.IsToggled;

        await s.SaveSearchSettings(memberID, psm, jwtToken);
       // DependencyService.Get<ILoadingPageService>().HideLoadingPage();
        await DisplayAlert("Saving Privacy Settings...", "Privacy settings were successfully saved!", "Ok");
    }


}
