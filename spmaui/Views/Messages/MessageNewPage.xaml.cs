using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class MessageNewPage : ContentPage
{
    public string connectionID = "0";
    public MessageNewPage()
	{
		InitializeComponent();
       
    }
    private void autoComplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var current = e.CurrentSelection;
        ContactsModel nm = (ContactsModel)current;
        connectionID = nm.connectionID;

        DisplayAlert("Selection Changed", "SelectedIndex: " + connectionID, "OK");
    }
}
