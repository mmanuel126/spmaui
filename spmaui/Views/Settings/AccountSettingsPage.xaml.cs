using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.Services;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class AccountSettingsPage : ContentPage
{
    private readonly SettingsAccountViewModel _settingsAccountViewModel;

    public AccountSettingsPage(SettingsAccountViewModel settingsAccountViewModel)
	{
		InitializeComponent();
        _settingsAccountViewModel = settingsAccountViewModel;
        this.BindingContext = settingsAccountViewModel;

        imgMyProfile.Source = App.AppSettings.AppMemberImagesURL + Preferences.Get("UserImage", "");
        lblMyName.Text = Preferences.Get("UserName","");
        lblMyTitle.Text = Preferences.Get("UserTitle", "");
        string email = Preferences.Get("UserEmail","");
        lblEmail.Text = "You will be emailed at " + email + " whenever someone:";

        try
        {
            MediaPicker.CapturePhotoAsync();
        }
        catch (FeatureNotSupportedException)
        {
            CaptureImage.IsVisible = false;
        }
    }

    void OnRefreshProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var x = (SettingsAccountViewModel)this.BindingContext;
            x.IsRefreshing = true;
            x.GetAccountSettngsInfo();
            x.GetSecurityQuestions();
            x.GetAccountSettingsNotifications();
            x.GetDeactivationReasons();
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
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }

    }

    async void PickImage_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file != null)
            {
                var content = new MultipartFormDataContent();
                file.FileName = "somename.png";
                var stream = await file.OpenReadAsync();
                content.Add(new StreamContent(stream), "file", file.FileName);
                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                    memberID = Preferences.Get("UserID", "");

                await _settingsAccountViewModel.UploadImage(content);
                imgMyProfile.Source = ImageSource.FromStream(() => stream);
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
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void CaptureImage_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            var file = await MediaPicker.CapturePhotoAsync();
            if (file != null)
            {
                var content = new MultipartFormDataContent();
                file.FileName = "somename.png";
                var stream = await file.OpenReadAsync();
                content.Add(new StreamContent(stream), "file", file.FileName);

                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                    memberID = Preferences.Get("UserID", "");

                await _settingsAccountViewModel.UploadImage(content);
                imgMyProfile.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (FeatureNotSupportedException){/*nothing to do here yet*/}
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(HttpRequestException))
            {
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
            else
            {
                await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }

    }

    async void ChangeNameButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(FirstName.Text))
            {
                await DisplayAlert("First Name Required...", "Please enter your first name!", "OK");
                FirstName.Focus();
            }
            else if (String.IsNullOrEmpty(LastName.Text))
            {
                await DisplayAlert("Last Name Required...", "Please enter your last name!", "Ok");
                LastName.Focus();
            }
            else
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                    memberID = Preferences.Get("UserID", "");

                await _settingsAccountViewModel.SaveMemberNameInfo(FirstName.Text, MiddleName.Text, LastName.Text);
                await DisplayAlert("Name Saved...", "Name was updated successfully!", "Ok");
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
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void ChangePwdButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(CurrentPassord.Text))
            {
                await DisplayAlert("Current Password Required...", "Please enter your current password!", "OK");
                CurrentPassord.Focus();
            }
            else if (CurrentPassord.Text != Preferences.Get("PWD", ""))
            {
                await DisplayAlert("Check Password...", "The password you entered is not your vaild password. Please enter correct password!", "Ok");
                CurrentPassord.Focus();
            }
            else if (String.IsNullOrEmpty(NewPassword.Text))
            {
                await DisplayAlert("New Password Required...", "Please enter the new password!", "Ok");
                NewPassword.Focus();
            }
            else if (String.IsNullOrEmpty(ConfirmPassword.Text))
            {
                await DisplayAlert("Confirm Password Required...", "Please enter new password to confirm!", "Ok");
                ConfirmPassword.Focus();
            }
            else if (!NewPassword.Text.Contains(ConfirmPassword.Text))
            {
                await DisplayAlert("Confirm password...", "confirm password must be the same as new password!", "Ok");
                ConfirmPassword.Focus();
            }
            else if (NewPassword.Text.Length < 5)
            {
                await DisplayAlert("New password Length...", "New password Your new password must be between 5-12 characters in length. !", "Ok");
                NewPassword.Focus();
            }
            else
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                    memberID = Preferences.Get("UserID", "");
                await _settingsAccountViewModel.SavePasswordInfo(NewPassword.Text);
                CurrentPassord.Text = ""; NewPassword.Text = ""; ConfirmPassword.Text = "";
                await DisplayAlert("Change Password...", "Password was changed successfully!", "Ok");
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
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void SecQuestButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (Question.SelectedIndex == 0)
            {
                await DisplayAlert("Question Required...", "Please select a question from list!", "OK");
                Question.Focus();
            }
            else if (String.IsNullOrEmpty(Answer.Text))
            {
                await DisplayAlert("Answer Required...", "Please enter answer for the question!", "Ok");
                Answer.Focus();
            }
            else
            {
                await _settingsAccountViewModel.SaveSecurityQuestionInfo(Question.SelectedIndex.ToString(), Answer.Text);
                await DisplayAlert("Security Question Saved...", "Security question and answer was saved successfully!", "Ok");
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
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    private async void SaveNotificationsButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            NotificationsSettingModel mod = new NotificationsSettingModel();
            mod.lG_SendMsg = SendMessage.IsToggled;
            mod.lG_AddAsFriend = AddAsContact.IsToggled;
            mod.lG_ConfirmFriendShipRequest = ContactRequest.IsToggled;
            mod.hE_RepliesToYourHelpQuest = HelpQuestions.IsToggled;
            mod.eV_DateChanged = false;
            mod.eV_InviteToEvent = false;
            mod.gP_ChangesTheNameOfGroupYouBelong = false;
            mod.gP_InviteYouToJoin = false;
            mod.gP_MakesYouAGPAdmin = false;
            mod.gP_RepliesToYourDiscBooardPost = false;
            mod.MemberID = int.Parse(Preferences.Get("UserID", ""));
            await _settingsAccountViewModel.SaveNotificationSettings(mod);
            await DisplayAlert("Notifications Settings Saved", "The notifications settings were successfully saved!", "Ok");
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
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void DeactivateButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (Reason.SelectedIndex == -1)
            {
                await DisplayAlert("Reason for Leaving Required...", "Please select from the list of reasons you are leaving sportsprofile!", "OK");
                Reason.Focus();
            }
            else
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                    memberID = Preferences.Get("UserID", "");
                await _settingsAccountViewModel.DeactivateAccount(Reason.SelectedIndex.ToString(), Explanation.Text);
                await DisplayAlert("Deactivating  Account...", "Your account was successfully deactivated!", "Ok");
                Preferences.Get("IsUserLogin", "false");
                Preferences.Get("UserID", null);
                Preferences.Get("UserEmail", null);
                Preferences.Get("UserName", null);
                Preferences.Get("UserTitle", null);
                Preferences.Get("AccessToken", null);
                App.Current.MainPage = new spmaui.Views.Account.LoginPage(new MemberViewModel(new Services.Members()));
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
                _settingsAccountViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }
    
}
