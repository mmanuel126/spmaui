namespace spmaui.Models
{
    public class UserModel
    {
        public string name { get; set; }
        public string email { get; set; }
        public string memberID { get; set; }
        public string picturePath { get; set; }
        public string accessToken { get; set; }
        public string title { get; set; }
        public string expiredDate { get; set; }
        public string currentStatus { get; set; }
    }

    public class LoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class RegisterModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPwd { get; set; }
        public string gender { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string year { get; set; }
        public string code { get; set; }
        public string profileType { get; set; }
    }

    public class PostModel
    {
        public string memberID { get; set; }
        public string postID { get; set; }
        public string postMsg { get; set; }
    }

}
