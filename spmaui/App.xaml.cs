using spmaui.Models;
using spmaui.Views.Account;

namespace spmaui;

public partial class App : Application
{
    private static AppSettings appSettings;

    public static AppSettings AppSettings
    {
        get
        {
            if (appSettings == null)
                LoadAppSettings();

            return appSettings;
        }
    }

    public App()
	{
        //Register syncfusion license
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTAzNDEzNkAzMjMwMmUzNDJlMzBOUTNQNUQ0QmRDUVVvbG1LdXU1Y0FxMWJQRDhwMm9Vd0ZQZWdnUmIvR2kwPQ==");
        InitializeComponent();

        MainPage = new LoginPage(new ViewModels.MemberViewModel(new Services.Members()));
    }

    private static void LoadAppSettings()
    {
#if RELEASE
                var settings = new AppSettings
                {
                    AppName = "Sport Profiles",
                    AppNameBackGroundImage = "splogo-blue.jpg",
                    AppImagesURL = "http://www.sportprofiles.net/assets", 
                    WebServiceURL = "http://www.sportprofiles.net/services/",
                    AppMemberImagesURL = "http://www.sportprofiles.net/images/members/"
                };
#else
        var settings = new AppSettings
        {
            AppName = "Sport Profiles",
            AppNameBackGroundImage = "splogo-blue.jpg",
            AppImagesURL = "http://www.sportprofiles.net/assets",
            WebServiceURL = "http://www.sportprofiles.net/services/",
            AppMemberImagesURL = "http://www.sportprofiles.net/images/members/"
        };
#endif

        appSettings = settings;
    }
}

