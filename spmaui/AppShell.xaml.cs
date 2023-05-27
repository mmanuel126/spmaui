using spmaui.Views;
namespace spmaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("memberprofile", typeof(ProfilePage));
        Routing.RegisterRoute("othersprofile", typeof(OthersProfilePage));
        Routing.RegisterRoute("playlistvideos", typeof(ProfilePlaylistPage));
        Routing.RegisterRoute("playlistvideoplayer", typeof(ProfileVideoPlayerPage));
        Routing.RegisterRoute("messagedetails", typeof(MessageDetailsPage));
        Routing.RegisterRoute("messagenew", typeof(MessageNewPage));
        /* Routing.RegisterRoute("settingsedit", typeof(SettingsEditPage)); */
    }
}

