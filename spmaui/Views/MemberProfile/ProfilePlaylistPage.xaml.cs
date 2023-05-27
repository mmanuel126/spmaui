using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class ProfilePlaylistPage : ContentPage
{
	public ProfilePlaylistPage()
	{
		InitializeComponent();

        string headerText = "";
        if (!String.IsNullOrEmpty(Preferences.Get("PlayListTitle","")))
            headerText = Preferences.Get("PlayListTitle","");
        lblHeader.Text = headerText;
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ((CollectionView)sender).SelectedItem = null;
        if (e.CurrentSelection.Count == 0)
            return;

        var current = e.CurrentSelection;
        YoutubeVideosListModel nm = (YoutubeVideosListModel)current[0];
        Preferences.Set("YoutubeVideoID",nm.Id);
        await Shell.Current.GoToAsync("playlistvideoplayer");
    }
}
