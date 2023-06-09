﻿using spmaui.ViewModels;

namespace spmaui.Views.Account;

public partial class ConfirmResetPwdPage : ContentPage
{
    private readonly MemberViewModel _memberViewModel;
	public ConfirmResetPwdPage(MemberViewModel memberViewModel)
	{
		InitializeComponent();
        _memberViewModel = memberViewModel;
        this.BindingContext = memberViewModel;
        //when you touch return to login screen label
        var register_confirm_tap = new TapGestureRecognizer();
        register_confirm_tap.Tapped += register_confirm_tap_Tapped;
        ReturnToLogin.GestureRecognizers.Add(register_confirm_tap);
    }

    private async void register_confirm_tap_Tapped(object sender, EventArgs e)
    {
        var loginPage = new LoginPage(_memberViewModel);
        await Navigation.PushModalAsync(loginPage);
    }
}
