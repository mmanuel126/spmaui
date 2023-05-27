using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.Services;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class AccountSettingsPage : ContentPage
{
	public AccountSettingsPage()
	{
		InitializeComponent();
        imgMyProfile.Source = App.AppSettings.AppImagesURL + "/images/members/" + Preferences.Get("UserImage", "");
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

    async void PickImage_Clicked(System.Object sender, System.EventArgs e)
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

            Members c = new Members();
            await c.UploadImage(memberID, content, jwtToken);
            imgMyProfile.Source = ImageSource.FromStream(() => stream);

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

                Members c = new Members();
                await c.UploadImage(memberID, content, jwtToken);
                imgMyProfile.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (FeatureNotSupportedException){/*nothing to do here yet*/}
    }

    async void ChangeNameButton_Clicked(System.Object sender, System.EventArgs e)
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
            DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            Settings m = new Settings();

            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "") ))
                memberID = Preferences.Get("UserID", "");

            await m.SaveMemberNameInfo(memberID, FirstName.Text, MiddleName.Text, LastName.Text, jwtToken);
            DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            await DisplayAlert("Name Saved...", "Name was updated successfully!", "Ok");
        }
    }

    async void ChangePwdButton_Clicked(System.Object sender, System.EventArgs e)
    {
        if (String.IsNullOrEmpty(CurrentPassord.Text))
        {
            await DisplayAlert("Current Password Required...", "Please enter your current password!", "OK");
            CurrentPassord.Focus();
        }
        else if (CurrentPassord.Text != Preferences.Get("PWD",""))
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
            DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            Settings m = new Settings();

            string jwtToken = Preferences.Get("AccessToken","");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID","")))
                memberID = Preferences.Get("UserID","");

            await m.SavePasswordInfo(memberID, NewPassword.Text, jwtToken);
            DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            await DisplayAlert("Name Saved...", "Name was updated successfully!", "Ok");
        }
    }

    async void SecQuestButton_Clicked(System.Object sender, System.EventArgs e)
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
            DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            var vm = (SettingsAccountViewModel)BindingContext;
            await vm.SaveSecurityQuestionInfo(Question.SelectedIndex.ToString(), Answer.Text);
            DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            await DisplayAlert("Security Question Saved...", "Security question and answer was saved successfully!", "Ok");
        }
    }

    private async void SaveNotificationsButton_Clicked(object sender, EventArgs e)
    {
        DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
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

        var vm = (SettingsAccountViewModel)BindingContext;
        await vm.SaveNotificationSettings(mod);
        DependencyService.Get<ILoadingPageService>().HideLoadingPage();
        await DisplayAlert("Notifications Settings Saved", "The notifications settings were successfully saved!", "Ok");

    }

    async void DeactivateButton_Clicked(System.Object sender, System.EventArgs e)
    {
        if (Reason.SelectedIndex == -1)
        {
            await DisplayAlert("Reason for Leaving Required...", "Please select from the list of reasons you are leaving sportsprofile!", "OK");
            Reason.Focus();
        }

        else
        {
            //DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            Settings s = new Settings();
            string jwtToken = Preferences.Get("AccessToken","");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                memberID = Preferences.Get("UserID", "");

            await s.DeactivateAccount(memberID, Reason.SelectedIndex.ToString(), Explanation.Text, jwtToken);
            //DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            await DisplayAlert("Deactivating  Account...", "Your account was successfully deactivated!", "Ok");

            DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            Preferences.Get("IsUserLogin","false");
            Preferences.Get("UserID",null);
            Preferences.Get("UserEmail",null);
            Preferences.Get("UserName",null);
            Preferences.Get("UserTitle",null);
            Preferences.Get("AccessToken",null);
            DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            App.Current.MainPage = new sp_maui.Views.Account.LoginPage();
        }
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;
        RecentNewsModel nm = (RecentNewsModel)current[0];
        await Launcher.OpenAsync(nm.navigateUrl);
        ((CollectionView)sender).SelectedItem = null;

        // string monkeyName = (e.CurrentSelection.FirstOrDefault() as Animal).Name;
        // This works because route names are unique in this application.
        //  await Shell.Current.GoToAsync($"monkeydetails?name={monkeyName}");
        // The full route is shown below.
        // await Shell.Current.GoToAsync($"//animals/monkeys/monkeydetails?name={monkeyName}");
    }

    
}
