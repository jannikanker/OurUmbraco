﻿using OurUmbraco.Forum;
using OurUmbraco.Forum.Services;
using OurUmbraco.NotificationsCore;
using OurUmbraco.NotificationsWeb.Services;
using Umbraco.Core;
using Umbraco.Web.Security;

namespace OurUmbraco.NotificationsWeb.EventHandlers
{
    public class Forum : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //sub comment author to topic
            CommentService.Created += CommentService_Created;

            //sub owner to topic
            TopicService.Created += TopicService_Created;

            //If its a project forum, subscribe the owner to all topics
            ForumService.Created += ForumService_Created;

            //remove all forum subs
            ForumService.Deleted += ForumService_Deleted;
        }

        void ForumService_Deleted(object sender, ForumEventArgs e)
        {
            var ns = new NotificationService(ApplicationContext.Current.DatabaseContext);
            ns.RemoveAllForumSubscriptions(e.Forum.Id);
        }

        void ForumService_Created(object sender, ForumEventArgs e)
        {
            var content = Umbraco.Web.UmbracoContext.Current.Application.Services.ContentService.GetById(e.Forum.ParentId);
            if (content.ContentType.Alias == "Project")
            {
                var owner = content.GetValue<int>("owner");
                //NotificationsWeb.BusinessLogic.Forum.Subscribe(e.Forum.Id, owner);
                var ns = new NotificationService(ApplicationContext.Current.DatabaseContext);
                ns.SubscribeToForum(e.Forum.Id, owner);
            }
        }

        void TopicService_Created(object sender, TopicEventArgs e)
        {
           
            var ns = new NotificationService(ApplicationContext.Current.DatabaseContext);
            ns.SubscribeToForumTopic(e.Topic.Id, e.Topic.MemberId);

            //send notification
            InstantNotification not = new InstantNotification();

            //data for notification:
            var membershipHelper = new MembershipHelper(Umbraco.Web.UmbracoContext.Current);
            var member = membershipHelper.GetById(e.Topic.MemberId);
            var memberName = string.Empty;
            if (member != null)
                memberName = member.Name;
            
            not.Invoke(Config.ConfigurationFile, Config.AssemblyDir, "NewTopic", e.Topic, e.Topic.GetUrl(), memberName);
        }

        void CommentService_Created(object sender, CommentEventArgs e)
        {
            var ts = new TopicService(ApplicationContext.Current.DatabaseContext);

            //Subscribe to topic
           var ns = new NotificationService(ApplicationContext.Current.DatabaseContext);
            ns.SubscribeToForumTopic(e.Comment.TopicId, e.Comment.MemberId);
            
            //data for notification:
            var membershipHelper = new MembershipHelper(Umbraco.Web.UmbracoContext.Current);
            var member = membershipHelper.GetById(e.Comment.MemberId);
            var memberName = string.Empty;
            if (member != null)
                memberName = member.Name;
            var topic = ts.GetById(e.Comment.TopicId);

            //send notifications
            InstantNotification not = new InstantNotification();
            not.Invoke(Config.ConfigurationFile, Config.AssemblyDir, "NewComment", e.Comment, topic, topic.GetUrl(), memberName);
        }


    }
}
