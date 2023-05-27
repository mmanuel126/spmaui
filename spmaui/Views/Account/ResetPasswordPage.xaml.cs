using spmaui.ViewModels;

namespace spmaui.Views.Account;

public partial class ResetPasswordPage : ContentPage
{
    MemberViewModel vm;
    public ResetPasswordPage()
	{
		InitializeComponent();
        vm = new MemberViewModel();

        //when you touch login label
        var login_tap = new TapGestureRecognizer();
        login_tap.Tapped += Login_tap_Tapped;
        RememberLoginLabel.GestureRecognizers.Add(login_tap);

        this.Title = App.AppSettings.AppName;

        var email = (string)Preferences.Get("ResetPwdEmail","");

        lblInstruction1.Text = "An email describing how to get to your new password has been sent to you at " + email + ".";
        lblInstruction2.Text = "The delivery of email may be delayed so please be patient. Confirm that the email above is correct and check your junk or spam folder if you did not received it in your inbox folder.";
        lblInstruction3.Text = "A confirmation reset code is provided with the email. You can enter this code below to continue with resetting your password.";
    }

    private async void Login_tap_Tapped(object sender, EventArgs e)
    {
        var LoginPage = new LoginPage();
        await Navigation.PushModalAsync(new NavigationPage(LoginPage));
    }

    private async void ResetPwdButton_Clicked(object sender, EventArgs e)
    {
        // Check for a valid email address.
        if (String.IsNullOrEmpty(Code.Text))
        {
            await DisplayAlert("Reset Code Required...", "Please enter reset code that was sent to you via email!", "Ok");
            Code.Focus();
        }
        else
        {
            try
            {
                // show the loading page...
                DependencyService.Get<ILoadingPageService>().ShowLoadingPage();

                //call service via vm and do things

                var email = (string)Preferences.Get("ResetPwdEmail","");
                var result = await vm.IsResetCodeExpired(Code.Text, "");
                if (result == "yes")
                {
                    await DisplayAlert("Invalid or Expired Code...", "The code entered is invalid or expired! Try again or go back to previous screens to get a new code.", "Ok");
                    Code.Focus();
                }
                else
                {
                    Preferences.Set("ResetPwdCode",Code.Text);
                    var changePwdPage = new ChangePasswordPage();
                    await Navigation.PushAsync(new NavigationPage(changePwdPage));
                }
                DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            }
            catch (FormatException ex)
            {
                DependencyService.Get<ILoadingPageService>().HideLoadingPage();
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
        }
    }
}
