using System.Text.RegularExpressions;
using spmaui.Models;
using spmaui.ViewModels;

namespace spmaui.Views.Account;

public partial class RegisterPage : ContentPage
{
    MemberViewModel vm;
  
    public RegisterPage()
    {
        InitializeComponent();
       
        //load gender picker
        var genderList = new List<string>();
        genderList.Add("Female");
        genderList.Add("Male");
        Gender.ItemsSource = genderList;


        //load profile type picker
        var profiePickerList = new List<string>();
        profiePickerList.Add("Agent");
        profiePickerList.Add("Athlete(Amateur)");
        profiePickerList.Add("Athlete(Professional?");
        profiePickerList.Add("Athletic Trainer");
        profiePickerList.Add("Coach");
        profiePickerList.Add("Management");
        profiePickerList.Add("Referee");
        profiePickerList.Add("Retired");
        profiePickerList.Add("Scout");
        profiePickerList.Add("Sports Fanatic");
        ProfileType.ItemsSource = profiePickerList;


        vm = new MemberViewModel();
        this.Title = "Create Account";

        //when you touch recover password label
        var recoverpassword_tap = new TapGestureRecognizer();
        recoverpassword_tap.Tapped += RecoverPassword_tap_Tapped;
        regForgetLabel.GestureRecognizers.Add(recoverpassword_tap);

        //when you touch login label
        var login_tap = new TapGestureRecognizer();
        login_tap.Tapped += Login_tap_Tapped;
        existingActLabel.GestureRecognizers.Add(login_tap);
    }

    private async void RecoverPassword_tap_Tapped(object sender, EventArgs e)
    {
        var recoverPage = new RecoverPwdPage();
        await Navigation.PushModalAsync(new NavigationPage(recoverPage));
    }

    private async void Login_tap_Tapped(object sender, EventArgs e)
    {
        var loginPage = new LoginPage();
        await Navigation.PushModalAsync(new NavigationPage(loginPage));


    }

    private async void SignUpButton_Clicked(object sender, EventArgs e)
    {
        // Check for a valid email address.
        if (String.IsNullOrEmpty(FirstName.Text))
        {
            await DisplayAlert("First Name Required...", "Please enter your first name!", "Ok");
            FirstName.Focus();
        }
        else if (String.IsNullOrEmpty(FirstName.Text))
        {
            await DisplayAlert("Last Name Required...", "Please enter your last name!", "Ok");
            FirstName.Focus();
        }
        else if (String.IsNullOrEmpty(Email.Text))
        {
            await DisplayAlert("Email Required...", "Please enter your email address!", "Ok");
            Email.Focus();
        }
        else if (!IsEmailValid(Email.Text))
        {
            await DisplayAlert("Invalid Email...", "Please enter a valid email!", "Ok");
            Email.Focus();
        }
        // Check for a valid password, if the user entered one.
        else if (String.IsNullOrEmpty(Pwd.Text))
        {
            await DisplayAlert("Password Required...", "Please enter your password!", "Ok");
            Pwd.Focus();
        }
        else if (String.IsNullOrEmpty(PwdReEntered.Text))
        {
            await DisplayAlert("Re Enter Password...", "Please re-enter your password!", "Ok");
            PwdReEntered.Focus();
        }
        else if (!Pwd.Text.Equals(PwdReEntered.Text))
        {
            await DisplayAlert("Passwords Entered Not The Same...", "Password and re-entered password must be the same!", "Ok");
            PwdReEntered.Focus();
        }
        else if (BirthDate.Date.ToString().Length == 0)
        {
            await DisplayAlert("Select Birth Date...", "Please select birth date!", "Ok");
            Gender.Focus();
        }
        else if (Gender.SelectedIndex == -1)
        {
            await DisplayAlert("Select Gender...", "Please select gender!", "Ok");
            Gender.Focus();
        }
        else if (ProfileType.SelectedIndex == -1)
        {
            await DisplayAlert("Select Profile Type...", "Please select  profile type!", "Ok");
            ProfileType.Focus();
        }
        else
        {
            try
            {
                //make the activeindicator control shows activity processing to user 
                aiLayout.IsVisible = true;
                ai.IsRunning = true;

                var gen = Gender.Items[Gender.SelectedIndex];
                var profile = ProfileType.Items[ProfileType.SelectedIndex];
                string bday = BirthDate.Date.Day.ToString();
                string bmonth = BirthDate.Date.Month.ToString();
                string byear = BirthDate.Date.Year.ToString();

                //call service via vm and do things
                var user = new RegisterModel
                {
                    firstName = FirstName.Text,
                    lastName = LastName.Text,
                    email = Email.Text,
                    password = Pwd.Text,
                    gender = gen,
                    profileType = profile,
                    day = bday,
                    month = bmonth,
                    year = byear
                };

                var result = await vm.register(user);

                //var result = "ExistingEmail";
                if (result == "ExistingEmail")
                {
                    aiLayout.IsVisible = false;
                    await DisplayAlert("Existing Email Error...", "The email you entered already exists in our system! Check your email to see if you may have already registerd.", "Ok");
                }
                else if (result == "NewEmail")
                {
                    // confirm register page
                    Preferences.Set("RegisteredEmail",Email.Text);
                    var confirmRegisterPage = new ConfirmRegisterPage();
                    await Navigation.PushModalAsync(new NavigationPage(confirmRegisterPage));
                    aiLayout.IsVisible = false;
                }
            }
            catch (FormatException ex)
            {
                aiLayout.IsVisible = false;
                await DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                
            }
        }

    }

    public bool IsEmailValid(string email)
    {
        Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (string.IsNullOrWhiteSpace(email))
            return false;
        return EmailRegex.IsMatch(email);
    }

}