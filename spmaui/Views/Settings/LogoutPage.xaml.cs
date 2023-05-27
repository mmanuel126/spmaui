using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views;

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
        App.Current.MainPage = new spmaui.Views.Account.LoginPage(new MemberViewModel (new Services.Members()));
    }
}
