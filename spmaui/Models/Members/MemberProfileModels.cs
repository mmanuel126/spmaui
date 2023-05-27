using System.Collections.Generic;

namespace sp_maui.Models
{
    public class MemberProfileBasicInfoModel
    {
        public string memProfileImage { get; set; }
        public string memProfileName { get; set; }
        public string memberProfileTitle { get; set; }
        public string memProfileBusinessSector { get; set; }
        public string memProfileIndustry { get; set; }
        public string memProfileStatus { get; set; }
        public string memProfileGender { get; set; }
        public string memProfileDOB { get; set; }
        public string memProfileInterestedInc { get; set; }
        public string memProfileLookingFor { get; set; }
        public string CurrentCity { get; set; }
        public string CurrentStatus { get; set; }
        public string DOBDay { get; set; }
        public string DOBMonth { get; set; }
        public string DOBYear { get; set; }
        public string FirstName { get; set; }
        public string HomeNeighborhood { get; set; }
        public string Hometown { get; set; }
        public string InterestedInType { get; set; }
        public string JoinedDate { get; set; }
        public string LastName { get; set; }
        public bool LookingForEmployment { get; set; }
        public bool LookingForNetworking { get; set; }
        public bool LookingForPartnership { get; set; }
        public bool LookingForRecruitment { get; set; }
        public string MemberID { get; set; }
        public string MiddleName { get; set; }
        public string PoliticalView { get; set; }
        public string ReligiousView { get; set; }
        public string Sex { get; set; }
        public bool ShowDOBType { get; set; }
        public bool ShowSexInProfile { get; set; }
        public string GetLGEntitiesCount { get; set; }
        public string Sport { get; set; }

        public string LeftRightHandFoot { get; set; }
        public string PreferredPosition { get; set; }
        public string SecondaryPosition { get; set; }
        public string InterestedDesc { get; set; }

        public string Height { get; set; }
        public string Weight { get; set; }
        public string Bio { get; set; }
        public List<YoutubePlayListModel> ProfilePlayList;
        public string TitleDesc { get; set; }
        public string ChannelID { get; set; }
    }

    public class SaveMemberProfileGenInfoModel
    {
        public string MemberID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public bool ShowSexInProfile { get; set; }
        public string DOBMonth { get; set; }
        public string DOBDay { get; set; }
        public string DOBYear { get; set; }
        public bool ShowDOBType { get; set; }
        public string Hometown { get; set; }
        public string HomeNeighborhood { get; set; }
        public string CurrentStatus { get; set; }
        public string InterestedInType { get; set; }
        public bool LookingForEmployment { get; set; }
        public bool LookingForRecruitment { get; set; }
        public bool LookingForPartnership { get; set; }
        public bool LookingForNetworking { get; set; }
        public string PicturePath { get; set; }
        public string JoinedDate { get; set; }
        public string CurrentCity { get; set; }
        public string TitleDesc { get; set; }
        public string Sport { get; set; }
        public string Bio { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string LeftRightHandFoot { get; set; }
        public string PreferredPosition { get; set; }
        public string SecondaryPosition { get; set; }
        public string InterestedDesc { get; set; }
    }



    public class SaveMemberBasicInfoBodyModel
    {
        public string PicturePath { get; set; }
        public string memProfileName { get; set; }
        public string TitleDesc { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public string memProfileStatus { get; set; }
        public string memProfileGender { get; set; }
        public string memProfileDOB { get; set; }
        public string InterestedDesc { get; set; }
        public string memProfileLookingFor { get; set; }
        public string CurrentCity { get; set; }
        public string CurrentStatus { get; set; }
        public string DOBDay { get; set; }
        public string DOBMonth { get; set; }
        public string DOBYear { get; set; }
        public string FirstName { get; set; }
        public string HomeNeighborhood { get; set; }
        public string Hometown { get; set; }
        public string InterestedInType { get; set; }
        public string JoinedDate { get; set; }
        public string LastName { get; set; }
        public string LookingForEmployment { get; set; }
        public string LookingForNetworking { get; set; }
        public string LookingForPartnership { get; set; }
        public string LookingForRecruitment { get; set; }
        public string MemberID { get; set; }
        public string MiddleName { get; set; }
        public string PoliticalView { get; set; }
        public string ReligiousView { get; set; }
        public string Sex { get; set; }
        public string ShowDOBType { get; set; }
        public string ShowSexInProfile { get; set; }
        public string GetLGEntitiesCount { get; set; }
    }

    public class SaveMemberContactInfoBodyModel
    {
        public string Email { get; set; }
        public string OtherEmail { get; set; }
        public string IMDisplayName { get; set; }
        public string IMServiceType { get; set; }
        public string WebsiteLink { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string OtherPhone { get; set; }
        public string Address { get; set; }
        public string CityTown { get; set; }
        public string Neighborhood { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string ShowAddress { get; set; }
        public string ShowEmailToMembers { get; set; }
        public string ShowCellPhone { get; set; }
        public string ShowHomePhone { get; set; }
        public string ShowLinks { get; set; }
    }
}
