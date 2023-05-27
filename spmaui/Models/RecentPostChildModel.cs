using System;

namespace spmaui.Models
{ 

    public class RecentPostChildModel
    {
        public int postResponseID { get; set; }
        public int postID { get; set; }
        public string description { get; set; }
        public string dateResponded { get; set; }
        public int memberID { get; set; }
        public string memberName { get; set; }
        public string firstName { get; set; }
        public string picturePath { get; set; }
        public string SelectedStateIcon { get; set; }
        public string DeselectedStateIcon { get; set; }
        public bool IsSelected { get; set; }
        public Action<RecentPostChildModel> OnClickListener { get; set; }
        
    }
}
