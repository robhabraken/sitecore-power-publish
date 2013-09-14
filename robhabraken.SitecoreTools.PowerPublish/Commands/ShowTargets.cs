using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using Sitecore.Shell.Framework.Commands;
using robhabraken.SitecoreTools.PowerPublish;
using Sitecore;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.HtmlControls;

namespace robhabraken.SitecoreTools.PowerPublish.Commands
{

    /// <summary>
    /// This command shows if the current item is published to one or more targets.
    /// </summary>
    public class ShowTargets : Command
    {
        public override string GetIcon(CommandContext context, string icon)
        {
            Item item = context.Items[0];
            var publishingHelper = new PublishingHelper();

            // Show a green icon if all targets are up to date with the latest revision
            if (publishingHelper.AllPublishingTargetsUpToDate(item))
            {
                return "Other/32x32/bullet_ball_glass_green.png";
            }

            // Show a red icon if the current item doesn't appear in any publishing targets
            else if(publishingHelper.ListPublishedTargets(item).Count == 0)
            {
                return "Other/32x32/bullet_ball_glass_red.png";
            }
            
            // If the current item is published to one or more targets, but not all of them,
            // or if the current item is changed after publishing, show an orange icon
            else
            {
                return "Other/32x32/bullet_ball_glass_yellow.png";
            }
        }

        public override void Execute(CommandContext context)
        {
        }

    }
}
