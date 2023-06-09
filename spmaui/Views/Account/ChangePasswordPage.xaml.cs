﻿using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views.Account;

public partial class ChangePasswordPage : ContentPage
{
    private readonly MemberViewModel _memberViewModel;

    public ChangePasswordPage(MemberViewModel memberViewModel)
	{
		InitializeComponent();
        _memberViewModel = memberViewModel;
        this.BindingContext = memberViewModel;

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
                var code = (string)Preferences.Get("ResetPwdCode","");
                var email = (string)Preferences.Get("ResetPwdEmail","");

                //call service via vm
                var user = new RegisterModel
                {
                    code = code,
                    confirmPwd = Pwd.Text,
                    email = email
                };

                var result = await _memberViewModel.ChangePassword(user);

                if (result == "")
                {
                    await DisplayAlert("Unexpected Error...", "An unexpected error occured. Please try again.", "Ok");
                }
                else
                {
                    var confirmResetPwdPage = new ConfirmResetPwdPage(_memberViewModel);
                    await Navigation.PushModalAsync(confirmResetPwdPage);
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
