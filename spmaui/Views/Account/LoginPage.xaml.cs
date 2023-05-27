using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using spmaui.Models;
using spmaui.ViewModels;


namespace spmaui.Views.Account
{
    public partial class LoginPage : ContentPage
    {
        private readonly MemberViewModel _memberViewModel;

        public LoginPage(MemberViewModel memberViewModel)
        {
            InitializeComponent();
            _memberViewModel = memberViewModel;
            this.BindingContext = _memberViewModel;
            string year = DateTime.Now.Year.ToString();
            lblCopyright.Text = "© " + year + " SportProfiles.net."; 

        }

        private async void OnTapGestureRecognizerTapped_ForgetLabel(object sender, EventArgs e)
        {
            var recoverPage = new RecoverPwdPage(_memberViewModel);
            await Navigation.PushModalAsync(recoverPage);
        }

        private async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            var registerPage = new RegisterPage(_memberViewModel);
            await Navigation.PushModalAsync(registerPage);
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            var email = txtEmail.Text;
            var pwd = txtPwd.Text;

            // Check for a valid email address.
            if (String.IsNullOrEmpty(email))
            {
                await DisplayAlert("Email Required...", "Please enter your email address!", "Ok");
                txtEmail.Focus();
            }
            else if (!IsEmailValid(email))
            {
                await DisplayAlert("Invalid Email...", "Please enter a valid email!", "Ok");
                txtEmail.Focus();
            }

            // Check for a valid password, if the user entered one.
            else if (String.IsNullOrEmpty(pwd))
            {
                await DisplayAlert("Password Required...", "Please enter your password!", "Ok");
                txtPwd.Focus();
            }
            else
            {
                try
                {
                    // show the loading page...
                    aiLayout.IsVisible = true;
                    ai.IsRunning = true;

                    //call service via vm and do things
                    var obj = await _memberViewModel.AuthenticateLGUser(email, pwd, "", "");

                    if (!String.IsNullOrEmpty(obj.memberID))
                    {
                        if (obj.currentStatus == "2")  //active
                        {
                            Preferences.Set("IsUserLogin", "true");
                            Preferences.Set("UserID", obj.memberID);
                            Preferences.Set("UserEmail", obj.email);
                            Preferences.Set("UserName", obj.name);
                            Preferences.Set("UserTitle", obj.title);
                            Preferences.Set("AccessToken", obj.accessToken);
                            Preferences.Set("ProfileLoginUser", "yes");

                            if (obj.picturePath != "")
                            {
                                Preferences.Set("UserImage", obj.picturePath);
                            }
                            else
                            {
                                Preferences.Set("UserImage", "default.png");
                            }

                            Preferences.Set("PWD", pwd);
                            Application.Current.MainPage = new AppShell();
                            aiLayout.IsVisible = false;
                        }
                        else if (obj.currentStatus == "3") //deactivated
                        {
                            aiLayout.IsVisible = false;
                            await DisplayAlert("Deactivated Account..", "Your account was deactivated recently or sometime ago. To re-activate your account, please log in the site using the link www.sportsprofile.net and follow the direction provided.", "Ok");
                        }
                    }
                    else
                    {
                        aiLayout.IsVisible = false;
                        Preferences.Set("IsUserLogin", "false");
                        await DisplayAlert("Incorrect Email/Password..", "The password or email you entered is incorrect. Try again.", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    aiLayout.IsVisible = false;

                    if (ex.GetType() == typeof(HttpRequestException))
                    {
                        await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                    }
                    else
                    {
                        await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        _memberViewModel.LogException(ex.Message, ex.StackTrace,"");
                    }
                }
            }
        }

        public bool IsEmailValid(string email)
        {
            Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (string.IsNullOrWhiteSpace(email))
                return false;
            return EmailRegex.IsMatch(email);
        }

    }
}
