using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.Services;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class MessagePage : ContentPage
{
    private readonly MessageViewModel _messageViewModel;
	public MessagePage(MessageViewModel messageViewModel)
	{
		InitializeComponent();
        _messageViewModel = messageViewModel;
        this.BindingContext = messageViewModel;
    }

    async void OnItemClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new MessageNewPage(new ConnectionAutocompleteViewModel()));
    }

    async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
    {
        var label = sender as Label;
        var data = label.BindingContext as MessageInfoModel;

        Preferences.Set("ProfileID", data.fromID);
        Preferences.Set("ProfileName", data.contactName);
        Preferences.Set("ProfileTitle", data.senderTitle);
        Preferences.Set("ProfileImage", data.senderImage);
        Preferences.Set("ProfileLoginUser", "no");
        await Shell.Current.Navigation.PushModalAsync(new OthersProfilePage(new ProfileViewModel(new Members(), new Commons())));
    }
}
