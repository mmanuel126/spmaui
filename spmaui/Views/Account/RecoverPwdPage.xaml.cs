namespace spmaui.Views.Account;
using spmaui;
using spmaui.ViewModels;

public partial class RecoverPwdPage : ContentPage
{
     MemberViewModel vm;
    public RecoverPwdPage()
	{
		InitializeComponent();

        this.Title = App.AppSettings.AppName;
        vm = new MemberViewModel();

        //when you touch login label
        var login_tap = new TapGestureRecognizer();
        login_tap.Tapped += Login_tap_Tapped;
        RememberLoginLabel.GestureRecognizers.Add(login_tap);
    }

    private async void Login_tap_Tapped(object sender, EventArgs e)
    {
        var LoginPage = new LoginPage();
        await Navigation.PushModalAsync(new NavigationPage(LoginPage));
    }

    private async void ResetPwdButton_Clicked(object sender, EventArgs e)
    {
        // Check for a valid email address.
        if (String.IsNullOrEmpty(EmailText.Text))
        {
            await DisplayAlert("Email Required...", "Please enter your email address!", "Ok");
            EmailText.Focus();
        }
        else
        {
            try
            {
                // show the loading page...
                aiLayout.IsVisible = true;
                ai.IsRunning = true;

                //call service via vm and do things
                await vm.ResetPassword(EmailText.Text, "");
                Preferences.Set("ResetPwdEmail",EmailText.Text);
                var resetPwdPage = new ResetPasswordPage();
                await Navigation.PushModalAsync(new NavigationPage(resetPwdPage));
                aiLayout.IsVisible = false;
            }
            catch (FormatException ex)
            {
                aiLayout.IsVisible = false;
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
        }
    }
}
