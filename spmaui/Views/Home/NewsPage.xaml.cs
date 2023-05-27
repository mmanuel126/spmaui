using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class NewsPage : ContentPage
{
	public NewsPage()
	{
		InitializeComponent();
       
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;
        RecentNewsModel nm = (RecentNewsModel)current[0];
        await Launcher.OpenAsync(nm.navigateUrl);
        ((CollectionView)sender).SelectedItem = null;

        // string monkeyName = (e.CurrentSelection.FirstOrDefault() as Animal).Name;
        // This works because route names are unique in this application.
        //  await Shell.Current.GoToAsync($"monkeydetails?name={monkeyName}");
        // The full route is shown below.
        // await Shell.Current.GoToAsync($"//animals/monkeys/monkeydetails?name={monkeyName}");
    }

    
}
