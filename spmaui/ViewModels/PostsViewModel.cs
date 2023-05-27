
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using spmaui.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using spmaui.Services;
using System.Collections.Generic;
using spmaui.Views;
using spmaui.Helper;
using TreeView.Maui;
using TreeView.Maui.Core;

namespace spmaui.ViewModels
{
    public class PostsViewModel : INotifyPropertyChanged
    {
        public ICommand RefreshCommand { get; set; }

        bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;

            set
            {
                isRefreshing = value;
                RaisedOnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private string conversationMessage = "";
        private ObservableCollection<Conversation> conversations;
        private string sendIcon;
        private string replyIcon;

        public ObservableCollection<Conversation> Conversations
        {
            get { return conversations; }
            set
            {
                conversations = value;
                RaisedOnPropertyChanged("Conversations");
            }
        }

        public string ReplyIcon
        {
            get { return replyIcon; }
            set
            {
                replyIcon = value;
                RaisedOnPropertyChanged("ReplyIcon");
            }
        }

        public string SendIcon
        {
            get { return sendIcon; }
            set
            {
                sendIcon = value;
                RaisedOnPropertyChanged("SendIcon");
            }
        }

        public string ConversationMessage
        {
            get { return conversationMessage; }
            set
            {
                conversationMessage = value;
                RaisedOnPropertyChanged("ConversationMessage");
            }
        }

        public string UserIcon
        {
            get;
            set;
        }

        public ICommand NewConversationCommand { get; private set; }

        public ICommand NewReplyCommand { get; private set; }

        public ICommand ReplyEditCommand { get; private set; }

        public ICommand ExpandActionCommand { get; private set; }

        public event EventHandler<ChatEventArgs> ConversationAdded;

        private void OnRefreshCommandExecuted() => Task.Run(() => DoRefreshPosts());

        async Task DoRefreshPosts()
        {
            try
            {
                IsRefreshing = true;
                await Initialize();
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (ex.GetType() == typeof(HttpRequestException))
                    {
                        await App.Current.MainPage.DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        protected virtual void OnConversationAdded(ChatEventArgs e)
        {
            Task.Run(() => DoRefreshPosts());
            EventHandler<ChatEventArgs> handler = ConversationAdded;
            handler?.Invoke(this, e);
        }

        public event EventHandler<ReplyEditEventArgs> ReplyTapped;

        protected virtual void OnReplyTapped(ReplyEditEventArgs e)
        {
            EventHandler<ReplyEditEventArgs> handler = ReplyTapped;
            handler?.Invoke(this, e);
        }

        public event EventHandler<ChatEventArgs> ReplyAdded;

        protected virtual void OnReplyAdded(ChatEventArgs e)
        {
            EventHandler<ChatEventArgs> handler = ReplyAdded;
            handler?.Invoke(this, e);
        }

        private readonly IMembers _membersSvc;

        public PostsViewModel(IMembers membersSvc)
        {
            _membersSvc = membersSvc;

            RefreshCommand = new Command(OnRefreshCommandExecuted);
            NewConversationCommand = new Command(OnConversationAdding);
            Conversations = new ObservableCollection<Conversation>();
            Task.Run(() => Initialize());
        }

        async Task Initialize()
        {
            this.Conversations = await this.GenerateConversations();
            ReplyEditCommand = new Command(OnInitializeReply);
            ExpandActionCommand = new Command(OnExpandAction);
            NewReplyCommand = new Command(OnReplyConversation);
            IsRefreshing = false;
        }

        private void OnReplyConversation(object sender)
        {
            try
            {
                var treeViewNode = sender as TreeViewNode;

                var content = (IChatMessageInfo)treeViewNode;

                Conversation conversation = null;
                if (content is Conversation)
                {
                    conversation = (Conversation)content;
                }
                else if (content is Reply)
                {
                    conversation = (Conversation)treeViewNode.Children;
                }
                if (conversation != null && !string.IsNullOrWhiteSpace(content.ReplyMessage))
                {
                    var replies = conversation.Replies;

                    string userName = Preferences.Get("UserName", "");
                    string userImage = Preferences.Get("UserImage", "");
                    string img = App.AppSettings.AppMemberImagesURL + "default.png";
                    if (userImage != null || userImage != "")
                    {
                        img = App.AppSettings.AppMemberImagesURL + userImage;
                    }

                    replies.Insert(replies.Count, new Reply
                    {
                        Message = content.ReplyMessage,
                        Date = DateTime.Now,
                        Name = userName,
                        ProfileIcon = img

                    });

                    var msg = content.ReplyMessage;
                    var pid = conversation.PostID;

                    //add it to database
                    string jwtToken = Preferences.Get("AccessToken", "");
                    int memberID = 0;
                    if (Preferences.Get("UserID", "") != null)
                    {
                        memberID = Convert.ToInt32(Preferences.Get("UserID", ""));
                    }

                    //refresh or display
                    conversation.Replies = replies;
                    content.IsInEditMode = false;
                    if (content is Conversation)
                        conversation.IsNeedExpand = true;

                    _membersSvc.SavePosts(memberID, pid, msg, jwtToken);
                    Task.Run(() => DoRefreshPosts());

                    OnReplyAdded(new ChatEventArgs() { ChatMessageItem = content, Conversation = conversation });
                }
                content.ReplyMessage = null;
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (ex.GetType() == typeof(HttpRequestException))
                    {
                        await App.Current.MainPage.DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        private void OnExpandAction(object sender)
        {
            var node = sender as TreeViewNode;
            node.IsExtended = !node.IsExtended;
        }

        private void ResetEditMode()
        {
            foreach (Conversation conversation in this.Conversations)
            {
                if (conversation.IsInEditMode)
                {
                    conversation.IsInEditMode = false;
                    conversation.ReplyMessage = null;
                }
                foreach (Reply reply in conversation.Replies)
                {
                    if (reply.IsInEditMode)
                    {
                        reply.IsInEditMode = false;
                        reply.ReplyMessage = null;
                        break;
                    }
                }
            }
        }

        private void OnInitializeReply(object sender)
        {
            var content = (sender as TreeViewNode).Children;
            this.ResetEditMode();
            if (content is Conversation)
            {
                var conversation = (Conversation)content;
                conversation.IsInEditMode = true;
                conversation.IsNeedExpand = false;
            }
            else if (content is Reply)
            {
                Reply reply = (Reply)content;
                reply.IsInEditMode = true;
            }
            OnReplyTapped(new ReplyEditEventArgs() { Content = content });
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisedOnPropertyChanged(string _PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
            }
        }

        #endregion

        private void OnConversationAdding()
        {
            try
            {
                PostsViewModel instance = this;
                if (!string.IsNullOrWhiteSpace(instance.ConversationMessage))
                {
                    string userName = Preferences.Get("UserName", "");
                    string userImage = Preferences.Get("UserImage", "");
                    string img = App.AppSettings.AppMemberImagesURL + "default.png";
                    if (userImage != null || userImage != "")
                    {
                        img = App.AppSettings.AppMemberImagesURL + userImage;
                    }

                    Conversation conversation = new Conversation
                    {
                        Message = instance.ConversationMessage,
                        Date = DateTime.Now,
                        Name = userName,
                        ProfileIcon = img
                    };

                    //add to database
                    string jwtToken = Preferences.Get("AccessToken", "");
                    int memberID = 0;
                    if (Preferences.Get("UserID", "") != null)
                    {
                        memberID = Convert.ToInt32(Preferences.Get("UserID", ""));
                    }
                    _membersSvc.SavePosts(memberID, 0, conversation.Message, jwtToken);

                    //add to current UI instance
                    OnConversationAdded(new ChatEventArgs() { Conversation = conversation });
                    Task.Run(() => DoRefreshPosts());
                }
                instance.ConversationMessage = null;
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (ex.GetType() == typeof(HttpRequestException))
                    {
                        await App.Current.MainPage.DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
            }
        }

        public async Task<ObservableCollection<Conversation>> GenerateConversations()
        {
            try
            {
                string jwtToken = Preferences.Get("AccessToken", "");
                int memberID = 0;
                if (Preferences.Get("UserID", "") != null)
                    memberID = Convert.ToInt32(Preferences.Get("UserID", ""));

                List<RecentPostsModel> result = await _membersSvc.GetRecentPosts(memberID, jwtToken);

                var conversationList = new ObservableCollection<Conversation>();

                if (result != null)
                {
                    foreach (var r in result)
                    {
                        string img = App.AppSettings.AppMemberImagesURL + "default.png";
                        if (r.picturePath != null || r.picturePath != "")
                        {
                            img = App.AppSettings.AppMemberImagesURL  + r.picturePath;
                        }

                        var conv = new Conversation() { Name = r.memberName, Message = r.description, Date = Convert.ToDateTime(r.datePosted), ProfileIcon = img, IsNeedExpand = true, IsExtended = true, PostID = Convert.ToInt32(r.postID) };
                        if (conv != null)
                        {
                            //get children for post
                            List<RecentPostChildModel> cResult = await _membersSvc.GetChildPosts(Convert.ToInt32(r.postID), jwtToken);
                            if (cResult != null)
                            {
                                if (cResult.Count != 0)
                                    conv.IsNeedExpand = true;

                                foreach (var c in cResult)
                                {
                                    string img2 = App.AppSettings.AppMemberImagesURL + "default.png";
                                    if (c.picturePath != null || c.picturePath != "")
                                    {
                                        img2 = App.AppSettings.AppMemberImagesURL +  c.picturePath;
                                    }
                                    conv.Replies.Add(new Reply() { Name = c.memberName, Message = c.description, Date = Convert.ToDateTime(c.dateResponded), ProfileIcon = img2, PostID = Convert.ToInt32(r.postID) });
                                    conv.Children.Add(new Conversation() { Name = c.memberName, Message = c.description, Date = Convert.ToDateTime(c.dateResponded), ProfileIcon = img2, PostID = Convert.ToInt32(r.postID) });
                                }
                            }
                        }

                        conversationList.Add(conv);
                    }
                }
                return conversationList;
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (ex.GetType() == typeof(HttpRequestException))
                    {
                        await App.Current.MainPage.DisplayAlert("Network Error...", "Error accessing network or services. Check internet connection and then try again.", "Ok");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(" General Error...", "A general error occured while you were using the application. The error has been logged and recorded for a specialist to look at. Try again in a bit later.", "Ok");
                        await _membersSvc.LogException(ex.Message, ex.StackTrace, "");
                    }
                });
                return null;
            }
        }
    }

    public class ReplyEditEventArgs : EventArgs
    {
        public object Content { get; set; }
    }

    public class ChatEventArgs : EventArgs
    {
        public object ChatMessageItem { get; set; }
        public Conversation Conversation { get; set; }
    }
}