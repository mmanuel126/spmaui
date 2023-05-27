namespace sp_maui.Controls
{
    public partial class FlyoutHeader : ContentView
    {
        public FlyoutHeader()
        {
            InitializeComponent();
            imgProfile2.Source =  App.AppSettings.AppImagesURL + "/images/members/" + Preferences.Get("UserImage","").ToString();
            lblName2.Text = Preferences.Get("UserName","").ToString();
            lblTitle2.Text =  Preferences.Get("UserTitle","").ToString();
        }
    }
}




