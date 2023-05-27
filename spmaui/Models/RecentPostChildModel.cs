using System;

namespace sp_maui.Models
{ 

    public class RecentPostChildModel
    {
        public string postResponseID { get; set; }
        public string postID { get; set; }
        public string description { get; set; }
        public string dateResponded { get; set; }
        public string memberID { get; set; }
        public string memberName { get; set; }
        public string firstName { get; set; }
        public string picturePath { get; set; }

        public string SelectedStateIcon { get; set; }
        public string DeselectedStateIcon { get; set; }
        public bool IsSelected { get; set; }
        public Action<RecentPostChildModel> OnClickListener { get; set; }
        
    }
}
