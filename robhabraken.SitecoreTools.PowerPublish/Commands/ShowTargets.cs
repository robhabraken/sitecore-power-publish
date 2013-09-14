using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using Sitecore.Shell.Framework.Commands;
using robhabraken.SitecoreTools.PowerPublish;
using Sitecore;

namespace robhabraken.SitecoreTools.PowerPublish.Commands
{
    /// <summary>
    /// This command shows if the current item is published to one or more targets.
    /// </summary>
    public class ShowTargets : Command
    {

        public override void Execute(CommandContext context)
        {
        }

        public override CommandState QueryState(CommandContext context)
        {
            var item = context.Items[0];

            var publishedTargets = new PublishingHelper().ListPublishedTargets(item);

            return publishedTargets.Count > 0 ? CommandState.Enabled : CommandState.Disabled; // : base.QueryState(context);
        }

    }
}
