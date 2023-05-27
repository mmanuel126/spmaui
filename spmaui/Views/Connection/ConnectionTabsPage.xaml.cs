

using Microsoft.Maui.Controls.PlatformConfiguration;
using sp_maui.ViewModels;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Syncfusion.Maui.Core;

namespace sp_maui.Views.Connection
{
    public partial class ConnectionTabsPage : ContentPage
    {
        //ConnectionTabsViewModel model = new ConnectionTabsViewModel();

        public ConnectionTabsPage()
        {
            InitializeComponent();
            //  this.BindingContext = model; 
            busyIndicator.IsRunning = false;
            On<iOS>().SetUseSafeArea(true);

        }
        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*            this.collectionView.SelectedItem = null;
                        if (e.CurrentSelection.Count == 0)
                            return;
                        var current = e.CurrentSelection; 
                        ContactsModel nm = (ContactsModel)current[0];

                       Preferences.Set("ProfileID",nm.connectionID);
                        Preferences.Set("ProfileName", nm.friendName);
                        Preferences.Set("ProfileTitle", nm.titleDesc);
                        Preferences.Set("ProfileImage", nm.picturePath);

                        await Shell.Current.GoToAsync("profile"); */
        }

    }

    
}
