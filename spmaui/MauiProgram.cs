using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using CommunityToolkit.Maui;
using spmaui.Services;
using spmaui.ViewModels;
using spmaui.Views;
using spmaui.Views.Connection;
using spmaui.Views.Account;

namespace spmaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureSyncfusionCore().UseMauiCommunityToolkitMediaElement().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit()
        .RegisterAppServices()
        .RegisterModelViews()
        .RegisterViews()
        ;
        
        #if DEBUG
            builder.Logging.AddDebug();
        #endif
            return builder.Build();
    }

    public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<IMembers, Members>();
        mauiAppBuilder.Services.AddSingleton<IConnections, Connections>();
        mauiAppBuilder.Services.AddSingleton<ICommons, Commons>();
        mauiAppBuilder.Services.AddSingleton<IMessages, spmaui.Services.Messages>();
        mauiAppBuilder.Services.AddSingleton<ISettings, Settings>();
        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterModelViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddTransient<NewsViewModel>();
        mauiAppBuilder.Services.AddTransient<PostsViewModel>();
        mauiAppBuilder.Services.AddTransient<MemberViewModel>();
        mauiAppBuilder.Services.AddTransient<ProfileViewModel>();
        mauiAppBuilder.Services.AddTransient<MessageViewModel>();
        mauiAppBuilder.Services.AddTransient<ConnectionViewModel>();
        mauiAppBuilder.Services.AddTransient<ConnectionAutocompleteViewModel>();
        mauiAppBuilder.Services.AddTransient<MessageDetailsViewModel>();
        mauiAppBuilder.Services.AddTransient<ProfilePlaylistViewModel>();
        mauiAppBuilder.Services.AddTransient<SettingsPrivacyViewModel>();
        mauiAppBuilder.Services.AddTransient<SettingsAccountViewModel>();
        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddTransient<NewsPage>();
        mauiAppBuilder.Services.AddTransient<PostsPage>();
        mauiAppBuilder.Services.AddTransient<LoginPage>();
        mauiAppBuilder.Services.AddTransient<RegisterPage>();
        mauiAppBuilder.Services.AddTransient<ChangePasswordPage>();
        mauiAppBuilder.Services.AddTransient<RecoverPwdPage>();
        mauiAppBuilder.Services.AddTransient<ResetPasswordPage>();
        mauiAppBuilder.Services.AddTransient<ConfirmRegisterPage>();
        mauiAppBuilder.Services.AddTransient<ConfirmResetPwdPage>();

        mauiAppBuilder.Services.AddTransient<ConnectionTabsPage>();
        mauiAppBuilder.Services.AddTransient<ProfilePage>();
        mauiAppBuilder.Services.AddTransient<ProfilePlaylistPage>();
        mauiAppBuilder.Services.AddTransient<OthersProfilePage>();
        mauiAppBuilder.Services.AddTransient<ProfileEditPage>();
        mauiAppBuilder.Services.AddTransient<ProfileAddEducationPage>();
        mauiAppBuilder.Services.AddTransient<ProfileUpdateEducationPage>();

        mauiAppBuilder.Services.AddTransient<MessagePage>();
        mauiAppBuilder.Services.AddTransient<MessageNewPage>();
        mauiAppBuilder.Services.AddTransient<MessageDetailsPage>();

        mauiAppBuilder.Services.AddTransient<PrivacySettingsPage>();
        mauiAppBuilder.Services.AddTransient<AccountSettingsPage>();
        return mauiAppBuilder;
    }

}



