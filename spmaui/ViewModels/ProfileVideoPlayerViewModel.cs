using System;
using System.Windows.Input;
using sp_maui.Views;

namespace sp_maui.ViewModels
{
    public class ProfileVideoPlayerViewModel: BaseViewModel
    {

        private string selectedVideo;

        public string SelectedVideo
        {
            get { return selectedVideo; }
            set
            {
                selectedVideo = value;
                OnPropertyChanged();
            }
        }

        public ProfileVideoPlayerViewModel()
        {
            string id = Preferences.Get("YoutubeVideoID", "");
            SelectedVideo = "https://www.youtube.com/watch?v=" + id;
        }
    }
}
