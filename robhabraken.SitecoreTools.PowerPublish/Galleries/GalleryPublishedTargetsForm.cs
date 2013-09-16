
namespace robhabraken.SitecoreTools.PowerPublish.Galleries
{
    using Sitecore;
    using Sitecore.Collections;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Links;
    using Sitecore.Resources;
    using Sitecore.Shell;
    using Sitecore.Shell.Applications.ContentManager.Galleries;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI;
    using Sitecore.Web;

    public class GalleryPublishedTargetsForm : GalleryForm
    {
        protected Menu Targets;

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull((object)message, "message");
            if (message.Name.Equals("item:load"))
                this.Invoke(message, true);
            else
                base.HandleMessage(message);
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                Item currentItem = GalleryPublishedTargetsForm.GetCurrentItem();
                if (currentItem != null)
                {
                    var targets = new PublishingHelper().ListTargets(currentItem);
                    foreach (var target in targets.Keys)
                    {
                        var menuItem = new MenuItem();
                        menuItem.Header = target;
                        if(targets[target].Equals(PublishingHelper.PublishState.Published)) 
                        {
                            menuItem.Icon = "Other/32x32/bullet_ball_glass_green.png";
                        }
                        else if (targets[target].Equals(PublishingHelper.PublishState.Changed))
                        {
                            menuItem.Icon = "Other/32x32/bullet_ball_glass_yellow.png";
                        }
                        else
                        {
                            menuItem.Icon = "Other/32x32/bullet_ball_glass_red.png";
                        }
                        this.Targets.Controls.Add(menuItem);
                    }
                }
            }
        }

        private static Item GetCurrentItem()
        {
            string dbQueryString = WebUtil.GetQueryString("db");
            string idQueryString = WebUtil.GetQueryString("id");
            var language = Language.Parse(WebUtil.GetQueryString("la"));
            var version = Sitecore.Data.Version.Parse(WebUtil.GetQueryString("vs"));
            var database = Factory.GetDatabase(dbQueryString);
            Assert.IsNotNull((object)database, dbQueryString);
            return database.Items[idQueryString, language, version];
        }
    }
}
