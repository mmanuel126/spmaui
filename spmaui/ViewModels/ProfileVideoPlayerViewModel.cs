using System;
using System.Windows.Input;
using spmaui.Views;
using YoutubeExplode;

namespace spmaui.ViewModels
{
    public class ProfileVideoPlayerViewModel : BaseViewModel
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
            var youtube = new YoutubeClient();
            string id = Preferences.Get("YoutubeVideoID", "");
            SelectedVideo = "https://youtube.com/watch?v=" + id;
        }
    }
}
