
using Syncfusion.TreeView.Engine;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using sp_mobile.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using sp_mobile.Services;
using System.Collections.Generic;
using sp_mobile.Views;
using sp_mobile.Helper;

namespace sp_mobile.ViewModels
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

        private void OnRefreshCommandExecuted() => DoRefreshPosts();

        async Task DoRefreshPosts()
        {
            Conversations.Clear();
            Conversations = new ObservableCollection<Conversation>();
            IsRefreshing = true;
            this.Conversations = await this.GenerateConversations();
            IsRefreshing = false;
        }

        protected virtual void OnConversationAdded(ChatEventArgs e)
        {
            DoRefreshPosts();
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

        public PostsViewModel()
        {

            RefreshCommand = new Xamarin.Forms.Command(OnRefreshCommandExecuted);
            NewConversationCommand = new Command(OnConversationAdding);
            Conversations = new ObservableCollection<Conversation>();
            Initialize();
        }

        async Task Initialize()
        {
            DependencyService.Get<ILoadingPageService>().ShowLoadingPage();
            this.Conversations = await this.GenerateConversations();
            ReplyEditCommand = new Command(OnInitializeReply);
            ExpandActionCommand = new Command(OnExpandAction);
            NewReplyCommand = new Command(OnReplyConversation);
            // hiding page progress...
            DependencyService.Get<ILoadingPageService>().HideLoadingPage();
        }

        private void OnReplyConversation(object sender)
        {
            var treeViewNode = sender as TreeViewNode;
            
            var content = (IChatMessageInfo)treeViewNode.Content;
            
            Conversation conversation = null;
            if (content is Conversation)
            { 
                conversation = (Conversation)content;
            }
            else if (content is Reply)
            {
                conversation = (Conversation)treeViewNode.ParentNode.Content;
            }
            if (conversation != null && !string.IsNullOrWhiteSpace(content.ReplyMessage))
            {
                var replies = conversation.Replies;

                string userName = Application.Current.Properties["UserName"].ToString();
                string userImage = Application.Current.Properties["UserImage"].ToString();
                string img = App.AppSettings.AppImagesURL + "images/members/default.png";
                if (userImage != null || userImage != "")
                {
                    img = App.AppSettings.AppImagesURL + "images/members/" + userImage;
                }

                replies.Insert(replies.Count, new Reply
                {
                    Message = content.ReplyMessage,
                    Date = DateTime.Now,
                    Name = userName,
                    ProfileIcon = img

                }); ;

                
                var msg = content.ReplyMessage;
                var pid = conversation.PostID;

                //add it to database
                Members svc = new Members();
                string jwtToken = Application.Current.Properties["AccessToken"].ToString();
                int memberID = 0;
                if (Application.Current.Properties["UserID"].ToString() != null)
                {
                    memberID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString());
                }
               
                //refresh or display
                conversation.Replies = replies;
                content.IsInEditMode = false;
                if (content is Conversation)
                    conversation.IsNeedExpand = true;

                svc.SavePosts(memberID,pid, msg, jwtToken);
                DoRefreshPosts();

                OnReplyAdded(new ChatEventArgs() { ChatMessageItem = content, Conversation = conversation });
            }
            content.ReplyMessage = null;
        }

        private void OnExpandAction(object sender)
        {
            var node = sender as TreeViewNode;
            node.IsExpanded = !node.IsExpanded;
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
            var content = (sender as TreeViewNode).Content;
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
            PostsViewModel  instance = this;
            if (!string.IsNullOrWhiteSpace(instance.ConversationMessage))
            {
                string userName = Application.Current.Properties["UserName"].ToString();
                string userImage = Application.Current.Properties["UserImage"].ToString();
                string img = App.AppSettings.AppImagesURL + "images/members/default.png";
                if (userImage != null || userImage != "")
                {
                    img = App.AppSettings.AppImagesURL + "images/members/" + userImage;
                }
               
                Conversation conversation = new Conversation
                {
                     
                    Message = instance.ConversationMessage,
                    Date = DateTime.Now,
                    Name = userName,
                    ProfileIcon = img,
                    //TextColor = Color.FromHex("#f23518")
                };

                //add to database
                Members svc = new Members();
                string jwtToken = Application.Current.Properties["AccessToken"].ToString();
                int memberID = 0;
                if (Application.Current.Properties["UserID"].ToString() != null)
                {
                    memberID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString());
                }
                svc.SavePosts(memberID,0,conversation.Message, jwtToken);
                DoRefreshPosts();
                
                //add to current UI instance
                //  instance.Conversations.Add(conversation);
                OnConversationAdded(new ChatEventArgs() { Conversation = conversation });
            }
            instance.ConversationMessage = null;
        }

        public async Task<ObservableCollection<Conversation>> GenerateConversations()
        {
            Members svc = new Members();
            string jwtToken = Application.Current.Properties["AccessToken"].ToString();
            int memberID = 0;
            if (Application.Current.Properties["UserID"].ToString() != null)
                memberID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString());


            // show the loading page...
           // DependencyService.Get<ILoadingPageService>().ShowLoadingPage();

            List<RecentPostsModel> result = await svc.GetRecentPosts(memberID, jwtToken);

            // show the loading page...
            //DependencyService.Get<ILoadingPageService>().HideLoadingPage();

            var conversationList = new ObservableCollection<Conversation>();

            if (result != null)
            {
                //Conversation conv = new Conversation();
                foreach (var r in result)
                {
                    string img = App.AppSettings.AppImagesURL + "images/members/default.png";
                    if (r.picturePath != null || r.picturePath != "")
                    {
                        img = App.AppSettings.AppImagesURL + "images/members/" + r.picturePath;
                    }

                    var conv = new Conversation() { Name = r.memberName, Message = r.description, Date = Convert.ToDateTime(r.datePosted), ProfileIcon = img, IsNeedExpand = false, PostID=Convert.ToInt32(r.postID) };
                    if (conv != null)
                    {
                        //get children for post
                        List<RecentPostChildModel> cResult = await svc.GetChildPosts(Convert.ToInt32(r.postID), jwtToken);
                        if (cResult != null)
                        {
                            if (cResult.Count != 0)
                                conv.IsNeedExpand = true;

                            foreach (var c in cResult)
                            {
                                string img2 = App.AppSettings.AppImagesURL + "images/members/default.png";
                                if (c.picturePath != null || c.picturePath != "")
                                {
                                    img2 = App.AppSettings.AppImagesURL + "images/members/" + c.picturePath;
                                }
                                conv.Replies.Add(new Reply() { Name = c.memberName, Message = c.description, Date = Convert.ToDateTime(c.dateResponded), ProfileIcon = img2, PostID=Convert.ToInt32 (r.postID )});
                            }
                        }

                    }
                    
                    conversationList.Add(conv);
                }
            }
            return conversationList;
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