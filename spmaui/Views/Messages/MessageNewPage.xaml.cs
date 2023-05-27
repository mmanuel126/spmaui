using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class MessageNewPage : ContentPage
{
    public string connectionID = "0";

    private readonly ConnectionAutocompleteViewModel _connectionAutocompleteViewModel;
    public MessageNewPage(ConnectionAutocompleteViewModel connectionAutocompleteViewModel)
    {
        InitializeComponent();
        _connectionAutocompleteViewModel = connectionAutocompleteViewModel;
        this.BindingContext = connectionAutocompleteViewModel;
    }

    private void autoComplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var current = e.CurrentSelection;
        ContactsModel nm = (ContactsModel)current[0];
        connectionID = nm.connectionID;
    }

    /*
    async void OnTapRecognizerTapped(object sender, TappedEventArgs args)
    {
        if (connectionID == "0")
        {
            await DisplayAlert("Message 'To' is Required...", "Please select a connection to send the message to!", "Ok");
            txtMessage.Focus();
        }
        else if (String.IsNullOrWhiteSpace(txtSubject.Text))
        {
            await DisplayAlert("Subject Text Required...", "Please enter a subject text for the message!", "Ok");
            txtSubject.Focus();
        }
        else if (String.IsNullOrWhiteSpace(txtMessage.Text))
        {
            await DisplayAlert("Message Text Required...", "Please enter a message text!", "Ok");
            txtMessage.Focus();
        }
        else
        {
            ConnectionAutocompleteViewModel vm = new ConnectionAutocompleteViewModel();
            await vm.SendMessage(connectionID, txtSubject.Text, txtMessage.Text);
        }
    }*/

    async void OnCancel_Clicked(object sender, EventArgs args)
    {
        await Navigation.PopModalAsync();
    }

    async void OnAddNew_Clicked(object sender, EventArgs args)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(txtSubject.Text))
            {
                await DisplayAlert("Subject Text Required...", "Please enter a subject text for the message!", "Ok");
                txtSubject.Focus();
            }
            else if (String.IsNullOrWhiteSpace(txtMessage.Text))
            {
                await DisplayAlert("Subject Message Required...", "Please enter a message text!", "Ok");
                txtMessage.Focus();
            }
            else if (connectionID == "0")
            {
                await DisplayAlert("Message 'To' is Required...", "Please select a connection to send the message to!", "Ok");
                txtMessage.Focus();
            }
            else
            {
                await _connectionAutocompleteViewModel.SendMessage(connectionID, txtSubject.Text, txtMessage.Text);
            }
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(HttpRequestException))
            {
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
            }
            else
            {
                await DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                _connectionAutocompleteViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }
}