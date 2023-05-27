using System;

namespace spmaui.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string name { get; set; }
    }

    public class StatesModel
    {
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class SchoolsByStateModel
    {
        public string SchoolId { get; set; }
        public string SchoolName { get; set; }
    }

    public class YoutubeDataModel
    {
       public string memberID { get; set; }
        public string channelID { get; set; }
    }

    public class InstagramDataModel
    {
        public string memberID { get; set; }
        public string instagramURL { get; set; }
    }

}
