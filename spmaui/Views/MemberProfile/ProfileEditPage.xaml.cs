using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.Services;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class ProfileEditPage : ContentPage
{
    ProfileViewModel vm = new ProfileViewModel();
    public ProfileEditPage()
    {
        InitializeComponent();
        BindingContext = vm;
        /*
        Preferences.Set("UserID", obj.memberID);
        Preferences.Set("UserEmail", obj.email);
        Preferences.Set("UserName", obj.name);
        Preferences.Set("UserTitle", obj.title);
        Preferences.Set("AccessToken", obj.accessToken);

        if (obj.picturePath != "")
        {
            Preferences.Set("UserImage", obj.picturePath);*/
        imgProfile.Source = App.AppSettings.AppImagesURL + "/images/members/" + Preferences.Get("UserImage", "");
        lblName.Text = Preferences.Get("UserName", "");
        lblTitle.Text = Preferences.Get("UserTitle", "");

        // Subscribe to a message (which the ViewModel has also subscribed to) to display an alert
       // MessagingCenter.Subscribe<ProfileEditContactInfoPage, string>(this, "RefreshContacts", async (sender, arg) =>
       // {
       //     //await DisplayAlert("Message received", "arg=" + arg, "OK");
       // });
    }

    async void OnEducationSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;
        var current = e.CurrentSelection;

        MemberProfileEducationModel nm = (MemberProfileEducationModel)current[0];
        await Launcher.OpenAsync(nm.schoolWebSite);
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
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
            //DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
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
            await vm.SaveMemberGeneralInfo(mod);
           // DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
        }
    }

    private async void SaveContactButton_Clicked(object sender, EventArgs e)
    {
        //DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
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
        await vm.SaveMemberContactInfo(mod);
        //DependencyService.Get<ILoadingPageService>().HideLoadingPage();
        await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
    }

    async void OnPhotosButtonClicked(object sender, EventArgs args)
    {
        //var somevariablefromviewmodel = ((ProfileContactInfoViewModel)BindingContext).ProfileContactInfo.Instagram;

        //string url = somevariablefromviewmodel;
        if (String.IsNullOrEmpty(InstagramURL.Text))
        {
            await DisplayAlert("Instagram URL Required", "Please enter your instagram URL before saving it!", "Ok");
        }
        else
        {
            Members m = new Members();
            string jwtToken = Preferences.Get("AccessToken","");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID","")))
            {
                memberID = Preferences.Get("UserID","");
            }
            await m.SaveInstagramURL(memberID, InstagramURL.Text, jwtToken);
           //MessagingCenter.Send<ProfileEditPhotosPage>(this, "RefreshContacts");
            //DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
        }
    }

    async void OnVideosButtonClicked(object sender, EventArgs args)
    {
        if (String.IsNullOrEmpty(ChannelID.Text))
        {
            await DisplayAlert("Youtube Channel ID Required", "Please enter your youtube chanel ID before saving it!", "Ok");
        }
        else
        {
            Members v = new Members();
            string jwtToken = Preferences.Get("AccessToken", "");
            string memberID = "0";
            if (!String.IsNullOrEmpty(Preferences.Get("UserID", "")))
            {
                memberID = Preferences.Get("UserID", "");
            }
            await v.SaveChannelID(memberID, ChannelID.Text, jwtToken);
            //DependencyService.Get<ILoadingPageService>().HideLoadingPage();
            await DisplayAlert("Data Saved", "Your information was successfully saved!", "Ok");
        }
    }

}
