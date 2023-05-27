using spmaui.ViewModels;

namespace spmaui.Views;

public partial class PostsPage : ContentPage
{
	 
	public PostsPage(PostsViewModel postsViewModel)
	{
		InitializeComponent();
		this.BindingContext = postsViewModel;
		postsViewModel.IsRefreshing = true;
	}
}
