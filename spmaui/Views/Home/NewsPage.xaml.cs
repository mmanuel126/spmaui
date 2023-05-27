using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class NewsPage : ContentPage
{
    private readonly NewsViewModel _newsViewModel;

	public NewsPage(NewsViewModel newsViewModel)
	{
		InitializeComponent();
        _newsViewModel = newsViewModel;
        this.BindingContext = _newsViewModel;
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;
        RecentNewsModel nm = (RecentNewsModel)current[0];
        await Launcher.OpenAsync(nm.navigateUrl);
        ((CollectionView)sender).SelectedItem = null;
    }

    async void OnRefreshNews_Clicked(object sender, EventArgs e)
    {
        try
        {
            await _newsViewModel.DoRefreshNews();
        }
        catch (Exception ex)
        {
            _newsViewModel.IsRefreshing = false;
            if (ex.GetType() == typeof(HttpRequestException))
            {
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
            else
            {
                await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                _newsViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

}
