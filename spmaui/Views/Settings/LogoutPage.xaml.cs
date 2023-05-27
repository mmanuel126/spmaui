using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class LogoutPage : ContentPage
{
	public LogoutPage()
    {
		InitializeComponent();
        Preferences.Set("IsUserLogin", "false");
        Preferences.Set("UserID", null);
        Preferences.Set("UserEmail", null);
        Preferences.Set("UserName", null);
        Preferences.Set("UserTitle", null);
        Preferences.Set("AccessToken", null);
        App.Current.MainPage = new sp_maui.Views.Account.LoginPage();
    }
}
