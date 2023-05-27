using System;
namespace sp_maui.Models
{
    public class AccountSettingsInfoModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public string PassWord { get; set; }
    }

    public class SecurityQuestions
    {
        public int Id { get; set; }
        public string Desc { get; set; }
    }

    public class DeactivationReasonsModel
    {
        public int Id { get; set; }
        public string Desc { get; set; }
    }

    public class NotificationsSettingModel
    {
        public int MemberID { get; set; }
        public bool lG_SendMsg { get; set; }
        public bool lG_AddAsFriend { get; set; }
        public bool lG_ConfirmFriendShipRequest { get; set; }
        public bool gP_InviteYouToJoin { get; set; }
        public bool gP_MakesYouAGPAdmin { get; set; }
        public bool gP_RepliesToYourDiscBooardPost { get; set; }
        public bool gP_ChangesTheNameOfGroupYouBelong { get; set; }
        public bool eV_InviteToEvent { get; set; }
        public bool eV_DateChanged { get; set; }
        public bool hE_RepliesToYourHelpQuest { get; set; }
    }
}
