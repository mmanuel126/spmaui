using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.Services;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class ProfileUpdateEducationPage : ContentPage
{
   MemberProfileEducationModel educationModel = new MemberProfileEducationModel();
    public ProfileUpdateEducationPage()
	{
		InitializeComponent();
        // Subscribe to a message (which the ViewModel has also subscribed to) to display an alert
        MessagingCenter.Subscribe<ProfileUpdateEducationPage, string>(this, "RefreshEducation", async (sender, arg) =>
        {
            //await DisplayAlert("Message received", "arg=" + arg, "OK");
        });

        educationModel.schoolImage = Preferences.Get("schoolimage", "");
        educationModel.major = Preferences.Get("major", "");
        educationModel.degree = Preferences.Get("degree", "");
        educationModel.yearClass = Preferences.Get("year", "");
        educationModel.sportLevelType = Preferences.Get("competitionlevel", "");
        educationModel.schoolID = Preferences.Get("schoolID", "");
        educationModel.schoolName = Preferences.Get("schoolName", "");

        imgProfile.Source = educationModel.schoolImage;

        int i = educationModel.major.IndexOf('-');
        string sMajor;
        if (i == -1)
            sMajor = educationModel.major;
        else
            sMajor = educationModel.major.Substring(0, i - 1);

        if (educationModel.schoolName.Length >= 30)
            educationModel.schoolName = educationModel.schoolName.Substring(0, 30) + "...";

        lblName.Text = educationModel.schoolName;
        lblMajor.Text = sMajor;

        if (educationModel.degree == "1")
            educationModel.degree = "Undergraduate";
        else if (educationModel.degree == "2")
            educationModel.degree = "Post Graduate";
        else if (educationModel.degree == "3")
            educationModel.degree = "High School Diploma";
        else if (educationModel.degree == "4")
            educationModel.degree = "GED";

        DegreePicker.SelectedItem = educationModel.degree;
        YearPicker.SelectedItem = educationModel.yearClass;
        SportLevelPicker.SelectedItem = educationModel.sportLevelType;
    }

    async void OnCancel_Clicked(object sender, EventArgs args)
    {
        await Navigation.PopModalAsync();
    }

    async void OnUpdate_Clicked(object sender, EventArgs args)
    {
        //do update here
        //DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
        ProfileViewModel p = new ProfileViewModel();
        educationModel.schoolName = lblName.Text;
        educationModel.major = lblMajor.Text;
        educationModel.degree = DegreePicker.SelectedItem.ToString();
        educationModel.yearClass = YearPicker.SelectedItem.ToString();
        educationModel.sportLevelType = SportLevelPicker.SelectedItem.ToString();

        if (String.IsNullOrEmpty(educationModel.Societies))
            educationModel.Societies = "";

        if (educationModel.degree == "Undergraduate")
            educationModel.degree = "1";
        else if (educationModel.degree == "Post Graduate")
            educationModel.degree = "2";
        else if (educationModel.degree == "High School Diploma")
            educationModel.degree = "3";
        else if (educationModel.degree == "GED")
            educationModel.degree = "4";

        await p.UpdateEducation(educationModel);
        await Navigation.PopModalAsync();
        MessagingCenter.Send<ProfileUpdateEducationPage>(this, "RefreshEducation");
       // DependencyService.Get<ILoadingPageService>().HideLoadingPage();
    }



}
