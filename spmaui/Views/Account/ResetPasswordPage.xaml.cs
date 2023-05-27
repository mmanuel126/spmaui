using System.Text;
using spmaui.ViewModels;

namespace spmaui.Views.Account;

public partial class ResetPasswordPage : ContentPage
{
    public string inst1;

    private readonly MemberViewModel _memberViewModel;

    public ResetPasswordPage(MemberViewModel memberViewModel)
    {
        InitializeComponent();
        _memberViewModel = memberViewModel;
        this.BindingContext = memberViewModel;

        //when you touch login label
        var login_tap = new TapGestureRecognizer();
        login_tap.Tapped += Login_tap_Tapped;
        RememberLoginLabel.GestureRecognizers.Add(login_tap);

        var email = Preferences.Get("ResetPwdEmail", "");

        lblInstruction1.Text = "An email describing how to get to your new password has been sent to you at:";
        lblInstruction2.Text = "The delivery of email may be delayed so please be patient. Confirm that the email above is correct and check your junk or spam folder if you did not received it in your inbox folder.";
        lblInstruction3.Text = "A confirmation reset code is provided with the email. Please enter this code below to continue with resetting your password.";
        txtEmail.Text = email;
    }

    private async void Login_tap_Tapped(object sender, EventArgs e)
    {
        var LoginPage = new LoginPage(_memberViewModel);
        await Navigation.PushModalAsync(LoginPage);
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
                var email = (string)Preferences.Get("ResetPwdEmail","");
                var result = await _memberViewModel.IsResetCodeExpired(Code.Text);
                if (result == "yes")
                {
                    await DisplayAlert("Invalid or Expired Code...", "The code entered is invalid or expired! Try again or go back to previous screens to get a new code.", "Ok");
                    Code.Focus();
                }
                else
                {
                    Preferences.Set("ResetPwdCode",Code.Text);
                    var changePwdPage = new ChangePasswordPage(_memberViewModel);
                    await Navigation.PushAsync(new NavigationPage(changePwdPage));
                }
            }
            catch (FormatException ex)
            {
                if (ex.GetType() == typeof(HttpRequestException))
                {
                    await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                }
                else
                {
                    await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                    _memberViewModel.LogException(ex.Message, ex.StackTrace, "");
                }
            }
        }
    }
}
