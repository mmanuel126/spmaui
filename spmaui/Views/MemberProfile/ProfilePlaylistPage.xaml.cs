using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class ProfilePlaylistPage : ContentPage
{
    private readonly ProfilePlaylistViewModel _profilePlayListViewModel;

    public ProfilePlaylistPage(ProfilePlaylistViewModel profilePlaylistViewModel)
	{
		InitializeComponent();

        _profilePlayListViewModel = profilePlaylistViewModel;
        this.BindingContext = profilePlaylistViewModel;

        string headerText = "";
        if (!String.IsNullOrEmpty(Preferences.Get("PlayListTitle","")))
            headerText = Preferences.Get("PlayListTitle","");
        lblHeader.Text = headerText;
    }

    async void OnReturnToPlaylist_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ((CollectionView)sender).SelectedItem = null;
        if (e.CurrentSelection.Count == 0)
            return;

        var current = e.CurrentSelection;
        YoutubeVideosListModel nm = (YoutubeVideosListModel)current[0];
        Preferences.Set("YoutubeVideoID",nm.Id);
        await Navigation.PushModalAsync(new ProfileVideoPlayerPage());
    }
}
