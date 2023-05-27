namespace sp_maui.Models
{
    public class MemberProfileContactInfoModel
    {
        public string Email { get; set; }
        public string OtherEmail { get; set; }
        //public string IMDisplayName { get; set; }        
        //public string IMServiceType { get; set; }
        public string Website { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        //public string OtherPhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool ShowAddress { get; set; }
        public bool ShowEmailToMembers { get; set; }
        public bool ShowCellPhone { get; set; }
        public bool ShowHomePhone { get; set; }
        //public string ShowLinks { get; set; }

        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
    }

    public class SaveMemberProfileContactInfoModel
    {
        public string Email { get; set; }
        public string OtherEmail { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string Website { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
       
        public string Address { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string ShowAddress { get; set; }
        public string ShowEmailToMembers { get; set; }
        public string ShowCellPhone { get; set; }
        public string ShowHomePhone { get; set; }
    }
}