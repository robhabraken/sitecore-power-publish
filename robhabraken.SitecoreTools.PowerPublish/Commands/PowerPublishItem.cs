
namespace robhabraken.SitecoreTools.PowerPublish.Commands
{
    using robhabraken.SitecoreTools.PowerPublish;
    using Sitecore;
    using Sitecore.Shell.Framework.Commands;

    /// <summary>
    /// First of all, this publish button will force the item being published, regardless the state of the Publishable option on the Item tab in the Publish restrictions,
    /// so you can use it to publish items that you've unpublished via the Unpublish button of this add-on (it does respect other Publishing restrictions though).
    /// But more importantly, this publish button also publishes all resources used by the current item. So if you've included media items like an image or a PDF,
    /// but forgot to publish them, clicking this button will also publish those items. It even works if you've publised the media item itself, but forgot to publish its parent folder...
    /// 
    /// This function will not publish linked pages in your site that are not published, but only resources like media library items and data sources used in item fields, as those are needed to display the item you want to publish correctly.
    /// This method even publishes the templates and layouts used by the current item, if that's not done yet.
    /// </summary>
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
                var itemReferences = Globals.LinkDatabase.GetReferences(item);
                foreach (var itemLink in itemReferences)
                {
                    if (itemLink != null)
                    {
                        var referencedItem = itemLink.GetTargetItem();
                        if (referencedItem != null)
                        {
                            publishingHelper.PublishAll(referencedItem, false, false);
                            if (referencedItem.Parent != null)
                            {
                                publishingHelper.PublishReversedRecursive(referencedItem.Parent);
                            }
                        }
                    }
                }

                publishingHelper.PublishAll(item, false, false);

                Sitecore.Context.ClientPage.ClientResponse.Alert("The item and its resources are being published");
            }
        }

    }
}
