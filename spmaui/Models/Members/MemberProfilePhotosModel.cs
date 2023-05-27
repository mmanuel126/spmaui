namespace spmaui.Models
{
    public class MemberProfilePhotosModel
    {
        public string FileName { get; set; }
        public string IsDefault { get; set; }
        public string MemberID { get; set; }
        public string ProfileID { get; set; }
        public string Removed { get; set; }
    }

    public class MemberProfileAlbumsModel
    {
        public string albumID { get; set; }
        public string albumName { get; set; }
        public string albumPhotoCount { get; set; }
        public string createdDate { get; set; }
        public string albumImage { get; set; }
    }
}