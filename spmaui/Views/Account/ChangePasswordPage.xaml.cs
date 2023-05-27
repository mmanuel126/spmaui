using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views.Account;

public partial class ChangePasswordPage : ContentPage
{
    MemberViewModel vm;
    public ChangePasswordPage()
	{
		InitializeComponent();
        this.Title = App.AppSettings.AppName;
        vm = new MemberViewModel();

        lblInstruction1.Text = "Please use the form below to change your password. It is required that you follow the guideline below:";
        lblInstruction2.Text = "Your new password must be between 5 - 12 characters in length.Use a combination of letters and numbers." +
                               " Passwords are case-sensitive.Remember to check your CAPS lock key.";
    }

    private async void ChangePwdButton_Clicked(object sender, EventArgs e)
    {
        // Check for a valid password, if the user entered one.
        if (String.IsNullOrEmpty(Pwd.Text))
        {
            await DisplayAlert("Password Required...", "Please enter the new password!", "Ok");
            Pwd.Focus();
        }
        else if (Pwd.Text.Length < 5)
        {
            await DisplayAlert("Password Minimum Length...", "Please enter a password that is 5 or more characters long!", "Ok");
            Pwd.Focus();
        }
        else if (String.IsNullOrEmpty(PwdReEntered.Text))
        {
            await DisplayAlert("Re Enter Password...", "Please re-enter the new password!", "Ok");
            PwdReEntered.Focus();
        }
        else if (!Pwd.Text.Equals(PwdReEntered.Text))
        {
            await DisplayAlert("Passwords Entered Not The Same...", "Password and re-entered password must be the same!", "Ok");
            PwdReEntered.Focus();
        }
        else
        {
            try
            {
                // show the loading page...
                DependencyService.Get<ILoadingPageService>().ShowLoadingPage();

                var code = (string)Preferences.Get("ResetPwdCode","");
                var email = (string)Preferences.Get("ResetPwdEmail","");

                //call service via vm and do things
                var user = new RegisterModel
                {
                    code = code,
                    confirmPwd = Pwd.Text,
                    email = email
                };

                var result = await vm.ChangePassword(user, "");

                if (result == "")
                {
                    await DisplayAlert("Unexpected Error...", "An unexpected error occured. Please try again.", "Ok");
                }
                else
                {
                    var confirmResetPwdPage = new ConfirmResetPwdPage();
                    await Navigation.PushModalAsync(new NavigationPage(confirmResetPwdPage));
                }
                DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            }
            catch (FormatException ex)
            {
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            }
        }

    }

}
