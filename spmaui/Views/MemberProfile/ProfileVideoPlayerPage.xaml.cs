using System.Net;
using System.Web;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;
using spmaui.Models;
using spmaui.ViewModels;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace spmaui.Views;

public partial class ProfileVideoPlayerPage : ContentPage
{
    public YoutubeExplode.Videos.Video myVid;

	public ProfileVideoPlayerPage()
	{
		InitializeComponent();
		this.Title = Preferences.Get("VideoTitle", "");
        string id = Preferences.Get("YoutubeVideoID", "");
        var videoId = id;
        var videoURL = $"https://youtube.com/watch?v={videoId}";
        Launcher.OpenAsync(videoURL);
    }

    public string GetYouTubeUrl(string videoId)
    {
        var videoInfoUrl = $"https://www.youtube.com/get_video_info?video_id={videoId}";
        using (var client = new HttpClient())
        {
            var videoPageContent = client.GetStringAsync(videoInfoUrl).Result;
            var videoParameters = HttpUtility.ParseQueryString(videoPageContent);
            var encodedStreamsDelimited1 = WebUtility.HtmlDecode(videoParameters["player_response"]);
            JObject jObject = JObject.Parse(encodedStreamsDelimited1);
            string url = (string)jObject["streamingData"]["formats"][0]["url"];
            return url;
        }
    }

    async void OnReturnToPlayVideoPage_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
