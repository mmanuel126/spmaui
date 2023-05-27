using System;

namespace sp_maui.Models
{


    public class RecentNewsModel
    {

        public string imageUrl { get; set; }
        public string headerText { get; set; }
        public DateTime postingDate { get; set; }
        public string textField { get; set; }
        public string navigateUrl { get; set; }
        public int id { get; set; }
        public string imgUrl => App.AppSettings.AppImagesURL  + $"{imageUrl}".Replace("~","").Replace("Images","images");
        public string description => $"{textField}".Substring(0, 120) + "...";
    }
}
