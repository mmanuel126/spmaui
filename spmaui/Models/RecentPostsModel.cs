using System;
using System.Collections.Generic;
namespace sp_maui.Models
{
    public class RecentPostsModel
    {
        public string postID { get; set; }
        public string description { get; set; }
        public string datePosted { get; set; }
        public string picturePath { get; set; }
        public string memberName { get; set; }
        public string firstName { get; set; }
        public string memberID { get; set; }

        public string SelectedStateIcon { get; set; }
        public string DeselectedStateIcon { get; set; }
        public bool IsSelected { get; set; }
        public Action<RecentPostsModel> OnClickListener { get; set; }
        public List<RecentPostsModel> ChildItems { get; set; }
        public int ReplyCount { get; set; }


        private List<RecentPostChildModel> children;
        public List<RecentPostChildModel> getChildren()
        {
            return children;
        }

        public void setChildren(List<RecentPostChildModel> children)
        {
            this.children = children;
        }
    }

}
