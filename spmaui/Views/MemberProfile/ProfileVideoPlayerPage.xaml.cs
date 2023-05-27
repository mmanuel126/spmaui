using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class ProfileVideoPlayerPage : ContentPage
{
	public ProfileVideoPlayerPage()
	{
		InitializeComponent();
		this.Title = Preferences.Get("VideoTitle", "");
    }
}
