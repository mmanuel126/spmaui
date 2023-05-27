using System;
using System.Collections.Generic;
using spmaui.Models;


namespace spmaui.Views.Connection
{
    public partial class ConnectionRequestPage : ContentPage
    {
        public ConnectionRequestPage()
        {
            InitializeComponent();

        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.collectionViewRq.SelectedItem = null;
            if (e.CurrentSelection.Count == 0)
                return;
            var current = e.CurrentSelection;
            ContactsModel nm = (ContactsModel)current[0];

            Preferences.Set("ProfileID", nm.connectionID);
            Preferences.Set("ProfileName", nm.friendName);
            Preferences.Set("ProfileTitle", nm.titleDesc);
            Preferences.Set("ProfileImage", nm.picturePath);
            await Shell.Current.GoToAsync("memberprofile");
        }
    }

    
}
