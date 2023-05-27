using System;
namespace spmaui.Models
{
    public class PrivacySettingsModel
    {
        public string  Id { get; set; }
        public string MemberID { get; set; }
        public string Profile { get; set; }
        public string BasicInfo { get; set; }
        public string PersonalInfo { get; set; }
        public string PhotosTagOfYou { get; set; }
        public string VideosTagOfYou { get; set; }
        public string ContactInfo { get; set; }
        public string Education { get; set; }
        public string WorkInfo { get; set; }
        public string IMdisplayName { get; set; }
        public string MobilePhone { get; set; }
        public string OtherPhone { get; set; }
        public string EmailAddress { get; set; }
        public string Visibility { get; set; }
        public bool ViewProfilePicture { get; set; }
        public bool ViewFriendsList { get; set; }
        public bool ViewLinksToRequestAddingYouAsFriend { get; set; }
        public bool ViewLinkTSendYouMsg { get; set; }
        public string Email { get; set; }
    }

    public class ProfilePrivacyTypesModel
    {
        public int Id { get; set; }
        public string Desc { get; set; }
    }
}
