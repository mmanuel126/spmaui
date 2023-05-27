using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.Services;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class ProfileEditPage : ContentPage
{
    private readonly ProfileViewModel _profileViewModel;

    public ProfileEditPage(ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        _profileViewModel = profileViewModel;
        BindingContext = profileViewModel; 
        imgProfile.Source = App.AppSettings.AppMemberImagesURL + Preferences.Get("UserImage", "");
        lblName.Text = Preferences.Get("UserName", "");
        lblTitle.Text = Preferences.Get("UserTitle", "");
    }

    async void OnRefreshProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var x = (ProfileViewModel)this.BindingContext;
            x.IsRefreshing = true;
            await x.GetMemberBasicInfo();
            await x.GetMemberContactInfo();
            await x.GetMemberEducation();
            await x.GetPlayList();
            this.BindingContext = x;
            x.IsRefreshing = false;
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
                _profileViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void OnEducationSelectionChanged(object sender, SelectionChangedEventArgs e)
    { 
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;

        MemberProfileEducationModel nm = (MemberProfileEducationModel)current[0];
        await Launcher.OpenAsync(nm.webSite);
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (String.IsNullOrEmpty(FirstName.Text))
            {
                await DisplayAlert("First Name Required...", "Please enter your first name!", "OK");
                FirstName.Focus();
            }
            else if (String.IsNullOrEmpty(LastName.Text))
            {
                await DisplayAlert("Last Name Required...", "Please enter your last name!", "Ok");
                LastName.Focus();
            }
            else if (String.IsNullOrEmpty(Title.Text))
            {
                await DisplayAlert("Title Required...", "Please enter your title!", "Ok");
                Title.Focus();
            }
            else if (String.IsNullOrEmpty(MainSport.Title))
            {
                await DisplayAlert("Main Sport Required...", "Please enter your main sport!", "Ok");
                MainSport.Focus();
            }
            else if (String.IsNullOrEmpty(CurrentStatus.SelectedItem.ToString()))
            {
                await DisplayAlert("Current Status Required...", "Your current status is required!", "Ok");
                MainSport.Focus();
            }
            else if (String.IsNullOrEmpty(Gender.SelectedItem.ToString()))
            {
                await DisplayAlert("Gender Required...", "Your gender is required!", "Ok");
                MainSport.Focus();
            }
            else if (String.IsNullOrEmpty(BirthDate.ToString()))
            {
                await DisplayAlert("Birth Date...", "Your birthdate is required!", "Ok");
                BirthDate.Focus();
            }
            else
            {
                MemberProfileBasicInfoModel mod = new MemberProfileBasicInfoModel();
                mod.FirstName = FirstName.Text;
                mod.MiddleName = MiddleName.Text;
                mod.LastName = LastName.Text;
                mod.TitleDesc = Title.Text;
                mod.Sport = MainSport.Title;
                mod.CurrentStatus = CurrentStatus.SelectedItem.ToString();
                mod.LeftRightHandFoot = leftRightHandFoot.SelectedItem.ToString();
                mod.PreferredPosition = PreferredPosition.Text;
                mod.SecondaryPosition = SecondaryPosition.Text;
                mod.Height = HeightBI.SelectedItem.ToString();
                mod.Weight = WeightBI.SelectedItem.ToString();
                mod.InterestedDesc = OtherInterest.SelectedItem.ToString();
                mod.Sex = Gender.SelectedItem.ToString();
                if (showGender.IsToggled)
                    mod.ShowSexInProfile = true;
                else
                    mod.ShowSexInProfile = false;
                mod.DOBDay = BirthDate.Date.Day.ToString();
                mod.DOBMonth = BirthDate.Date.Month.ToString();
                mod.DOBYear = BirthDate.Date.Year.ToString();

                if (lookingForNet.IsToggled)
                    mod.LookingForNetworking = true;
                else
                    mod.LookingForNetworking = false;

                if (lookingForPart.IsToggled)
                    mod.LookingForPartnership = true;
                else
                    mod.LookingForPartnership = false;

                if (lookingForRec.IsToggled)
                    mod.LookingForRecruitment = true;
                else
                    mod.LookingForRecruitment = false;

                if (LookingForEmp.IsToggled)
                    mod.LookingForEmployment = true;
                else
                    mod.LookingForEmployment = false;

                mod.Bio = EditBio.Text;

                if (CurrentStatus.SelectedItem.ToString() != "Athlete (Amateur)" && CurrentStatus.SelectedItem.ToString() != "Athlete (Professional)")
                {
                    mod.LeftRightHandFoot = "";
                    mod.PreferredPosition = "";
                    mod.SecondaryPosition = "";
                    mod.Height = "";
                    mod.Weight = "";

                }
                await _profileViewModel.SaveMemberGeneralInfo(mod);
                await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
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
                _profileViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    private async void SaveContactButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            MemberProfileContactInfoModel mod = new MemberProfileContactInfoModel();
            mod.Email = Email.Text;
            mod.OtherEmail = OtherEmaill.Text;
            mod.Facebook = Facebook.Text;
            mod.Instagram = Instagram.Text;
            mod.Twitter = Twitter.Text;
            mod.Website = WebsiteUrl.Text;
            mod.HomePhone = HomePhone.Text;
            mod.CellPhone = CellPhone.Text;
            mod.Address = Address.Text;
            mod.City = City.Text;
            mod.State = State.Text;
            mod.Zip = ZipCode.Text;
            mod.ShowAddress = true;
            mod.ShowCellPhone = true;
            mod.ShowEmailToMembers = true;
            mod.ShowHomePhone = true;
            mod.Neighborhood = "";
            await _profileViewModel.SaveMemberContactInfo(mod);
            await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
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
                _profileViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void OnPhotosButtonClicked(object sender, EventArgs args)
    {
        try
        {
            if (String.IsNullOrEmpty(InstagramURL.Text))
            {
                await DisplayAlert("Instagram URL Required", "Please enter your instagram URL before saving it!", "Ok");
            }
            else
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                {
                    memberID = Preferences.Get("UserID", "");
                }
                await _profileViewModel.SaveInstagramURL(InstagramURL.Text);
                await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
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
                _profileViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }

    async void OnVideosButtonClicked(object sender, EventArgs args)
    {
        try
        {
            if (String.IsNullOrEmpty(ChannelID.Text))
            {
                await DisplayAlert("Youtube Channel ID Required", "Please enter your youtube chanel ID before saving it!", "Ok");
            }
            else
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                string memberID = "0";
                if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
                {
                    memberID = Preferences.Get("UserID", "");
                }
                await _profileViewModel.SaveChannelID(ChannelID.Text);
                await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
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
                _profileViewModel.LogException(ex.Message, ex.StackTrace, "");
            }
        }
    }
}
