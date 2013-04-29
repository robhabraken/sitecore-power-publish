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
    public class UnpublishItem : Command
    {

        public override void Execute(CommandContext context)
        {
            if(context != null && context.Items.Length > 0 && context.Items[0] != null)
            {
                var item = context.Items[0];    

                item.Editing.BeginEdit();
                item.Publishing.NeverPublish = true;
                item.Editing.EndEdit();

                new PublishingHelper().PublishAll(item, false, false);

                Sitecore.Context.ClientPage.ClientResponse.Alert("The item is now unpublished");
            }
        }

    }
}
