using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class MessageDetailsPage : ContentPage
{
    private readonly MessageDetailsViewModel _messageDetailsViewModel;

    public MessageDetailsPage(MessageDetailsViewModel messageDetailsViewModel)
    {
        InitializeComponent();
        _messageDetailsViewModel = messageDetailsViewModel;
        this.BindingContext = messageDetailsViewModel;

    }

    async void OnCancel_Clicked(object sender, EventArgs args)
    {
        await Navigation.PopModalAsync();
    }

    async void OnSendMsg_Clicked(object sender, EventArgs args)
    {
        try
        {
            if (String.IsNullOrEmpty(txtMsg.Text))
            {
                await DisplayAlert("Message Text Required", "Please enter the message text.", "Ok");
            }
            else
            {
                MessageDetailsViewModel instance = _messageDetailsViewModel;
                MessageDetails msg = instance.MessageDetails;

                msg.Body = txtMsg.Text;
                await _messageDetailsViewModel.SendMessage(msg);
                await Navigation.PopModalAsync();
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
                _messageDetailsViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }
}