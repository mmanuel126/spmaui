using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class MessagePage : ContentPage
{
	public MessagePage()
	{
		InitializeComponent();
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        collectionView.SelectedItem = null;
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;
        MessageInfoModel nm = (MessageInfoModel)current[0];

        Preferences.Set("MessageID", nm.messageID);
        Preferences.Set("SenderID", nm.fromID);
        await Shell.Current.GoToAsync("messagedetails");
    }

    async void OnItemClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("messagenew");
    }

}
