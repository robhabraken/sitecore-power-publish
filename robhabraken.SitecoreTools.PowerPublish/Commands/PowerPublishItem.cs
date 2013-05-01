using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using Sitecore.Shell.Framework.Commands;
using robhabraken.SitecoreTools.PowerPublish;
using Sitecore;
using Sitecore.Data.Fields;

namespace robhabraken.SitecoreTools.PowerPublish.Commands
{
    public class PowerPublishItem : Command
    {

        public override void Execute(CommandContext context)
        {
            if (context != null && context.Items.Length > 0 && context.Items[0] != null)
            {
                var item = context.Items[0];

                if (item.Publishing.NeverPublish)
                {
                    item.Editing.BeginEdit();
                    item.Publishing.NeverPublish = false;
                    item.Editing.EndEdit();
                }

                var publishingHelper = new PublishingHelper();

                Globals.LinkDatabase.UpdateReferences(item);
                var itemReferences = Globals.LinkDatabase.GetItemReferences(item, true);
                foreach (var itemLink in itemReferences)
                {
                    if (itemLink != null)
                    {
                        var referencedItem = itemLink.GetTargetItem();
                        if (referencedItem != null)
                        {
                            publishingHelper.PublishReversedRecursive(referencedItem);
                        }
                    }
                }

                // yet to test publishing parents of resources

                new PublishingHelper().PublishAll(item, false, false);

                Sitecore.Context.ClientPage.ClientResponse.Alert("The item and its resources are being published");
            }
        }

    }
}
