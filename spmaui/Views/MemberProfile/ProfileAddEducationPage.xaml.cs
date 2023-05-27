using System.Windows.Input;
using Microsoft.Maui.Controls;
using sp_maui.Models;
using sp_maui.Services;
using sp_maui.ViewModels;

namespace sp_maui.Views;

public partial class ProfileAddEducationPage : ContentPage
{
    public List<SchoolsByStateModel> Schools;
    ProfileViewModel vm = new ProfileViewModel();
    public ProfileAddEducationPage()
	{
		InitializeComponent();
        BindingContext = vm;
        // Subscribe to a message (which the ViewModel has also subscribed to) to display an alert
        MessagingCenter.Subscribe<ProfileAddEducationPage, string>(this, "RefreshEducation", async (sender, arg) =>
        {

        });
    }
    async void OnCancel_Clicked(object sender, EventArgs args)
    {
        await Navigation.PopModalAsync();
    }

    async void OnAddNew_Clicked(object sender, EventArgs args)
    {
        //validate input
        if (InstTypePicker.SelectedItem == null)
        {
            await DisplayAlert("Institution Type Required...", "The institution type you attended is required! Please select type.", "Ok");
            InstTypePicker.Focus();
        }
        else if (StatePicker.SelectedItem == null)
        {
            await DisplayAlert("Institution State Required...", "The institution state you attended is required! Please Select state", "Ok");
            StatePicker.Focus();
        }
        else if (SchoolPicker.SelectedItem == null)
        {
            await DisplayAlert("Institution is Required...", "The institution you attended is required! Please select school.", "Ok");
            SchoolPicker.Focus();
        }
        else if (String.IsNullOrEmpty(txtMajor.Text))
        {
            await DisplayAlert("Major is Required...", "What you majored at the institution is required! Please enter your major.", "Ok");
            txtMajor.Focus();
        }
        else if (DegreePicker.SelectedItem == null)
        {
            await DisplayAlert("Degree is Required...", "The degree received at the institution is required! Please select your degree.", "Ok");
            DegreePicker.Focus();
        }
        else if (YearPicker.SelectedItem == null)
        {
            await DisplayAlert("Graduation year is Required...", "The graduation year is required! Please select the year.", "Ok");
            YearPicker.Focus();
        }
        else if (SportLevelPicker.SelectedItem == null)
        {
            await DisplayAlert("Competition Level is Required...", "The sport competition level is required! Please select a competition level.", "Ok");
            SportLevelPicker.Focus();
        }
        else
        {
            //DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            ProfileViewModel p = new ProfileViewModel();
            MemberProfileEducationModel educationModel = new MemberProfileEducationModel();


            var selectedSchool = (SchoolsByStateModel)SchoolPicker.SelectedItem;

            var selectedDegree = DegreePicker.SelectedItem.ToString();
            if (selectedDegree == "Undergraduate")
                educationModel.degree = "1";
            else if (selectedDegree == "Post Graduate")
                educationModel.degree = "2";
            else if (selectedDegree == "High School Diploma")
                educationModel.degree = "3";
            else if (selectedDegree == "GED")
                educationModel.degree = "4";

            educationModel.schoolID = selectedSchool.SchoolId;
            educationModel.major = txtMajor.Text;
            educationModel.yearClass = YearPicker.SelectedItem.ToString();
            educationModel.sportLevelType = SportLevelPicker.SelectedItem.ToString();

            if (InstTypePicker.SelectedItem.ToString() == "Colleges")
                educationModel.schoolType = "3";
            else if (InstTypePicker.SelectedItem.ToString() == "Public High School")
                educationModel.schoolType = "1";
            else
                educationModel.schoolType = "2";

            if (String.IsNullOrEmpty(educationModel.Societies))
                educationModel.Societies = "";

            educationModel.schoolName = selectedSchool.SchoolName;

            await p.AddNewEducation(educationModel);
            await Navigation.PopModalAsync();
            MessagingCenter.Send<ProfileAddEducationPage>(this, "RefreshEducation");
            //DependencyService.Get<ILoadingPageService>().HideLoadingPage();
        }
    }


    async void OnSelectedState(object sender, EventArgs e)
    {
        StatesModel selectedState = (StatesModel)StatePicker.SelectedItem;
        var state = selectedState.abbreviation;
        string instType;

        if (InstTypePicker.SelectedItem.ToString() == "Colleges")
            instType = "3";
        else if (InstTypePicker.SelectedItem.ToString() == "Public High School")
            instType = "1";
        else
            instType = "2";

        Commons co = new Commons();
        string jwtToken = Preferences.Get("AccessToken","");
        vm.Schools = await co.GetSchoolsByState(state, instType, jwtToken);

        SchoolPicker.ItemsSource = vm.Schools;
        SchoolPicker.ItemDisplayBinding = new Binding("SchoolName");
    }

}
