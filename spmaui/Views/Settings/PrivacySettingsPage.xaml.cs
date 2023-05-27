using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.Services;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class PrivacySettingsPage : ContentPage
{
    private readonly SettingsPrivacyViewModel _settingsPrivacyViewModel;

	public PrivacySettingsPage(SettingsPrivacyViewModel settingsPrivacyViewModel)
	{
		InitializeComponent();
        _settingsPrivacyViewModel = settingsPrivacyViewModel;
        this.BindingContext = settingsPrivacyViewModel;

    }

    //method to save profile settings when button is clicked.
    async void SaveProfileSettingsButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            Settings s = new Settings();
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
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

            _settingsPrivacyViewModel.SaveProfileSettings(psm);
            await DisplayAlert("Saving Privacy Settings...", "Privacy settings were successfully saved!", "Ok");
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
                _settingsPrivacyViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    //method to save privacy settings when save button is clicked or tapped 
    async void SavePrivacySearchButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            PrivacySettingsModel psm = new PrivacySettingsModel();
            if (Visibility.SelectedIndex != -1)
                psm.Visibility = Visibility.SelectedIndex.ToString();
            else psm.Visibility = "0";
            psm.ViewProfilePicture = ProfilePicture.IsToggled;
            psm.ViewFriendsList = ConnectionsList.IsToggled;
            psm.ViewLinksToRequestAddingYouAsFriend = RequestToAdd.IsToggled;
            psm.ViewLinkTSendYouMsg = SendYouMsg.IsToggled;
            _settingsPrivacyViewModel.SaveSearchSettings(psm);
            await DisplayAlert("Saving Privacy Settings...", "Privacy settings were successfully saved!", "Ok");
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
                _settingsPrivacyViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }
     
    //method to pull data to refresh it on page
    void OnRefreshSettings_Clicked(object sender, EventArgs e)
    {
        try
        {
            var x = (SettingsPrivacyViewModel)this.BindingContext;
            x.IsRefreshing = true;
            x.GetPrivacySettingsInfo();
            x.GetProfilePrivacyTypes();
            this.BindingContext = x;
            x.IsRefreshing = false;
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(HttpRequestException))
            {
                DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
            else
            {
                DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                _settingsPrivacyViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

}
