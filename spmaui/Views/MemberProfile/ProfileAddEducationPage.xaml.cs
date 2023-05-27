using System.Windows.Input;
using Microsoft.Maui.Controls;
using spmaui.Models;
using spmaui.Services;
using spmaui.ViewModels;

namespace spmaui.Views;

public partial class ProfileAddEducationPage : ContentPage
{
    public List<SchoolsByStateModel> Schools;
   
    private readonly ProfileViewModel _profileViewModel;

    public ProfileAddEducationPage(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
        _profileViewModel = profileViewModel;
        this.BindingContext = profileViewModel;
    }

    async void OnCancel_Clicked(object sender, EventArgs args)
    {
        await Navigation.PopModalAsync();
    }

    async void OnAddNew_Clicked(object sender, EventArgs args)
    {
        //validate input
        try
        {
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

                await _profileViewModel.AddNewEducation(educationModel);
                await Navigation.PopModalAsync();
                MessagingCenter.Send<ProfileAddEducationPage>(this, "RefreshEducation");
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

    async void OnSelectedState(object sender, EventArgs e)
    {
        try
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
            string jwtToken = Preferences.Get("AccessToken", "");
            _profileViewModel.Schools = await co.GetSchoolsByState(state, instType, jwtToken);

            SchoolPicker.ItemsSource = _profileViewModel.Schools;
            SchoolPicker.ItemDisplayBinding = new Binding("SchoolName");
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
