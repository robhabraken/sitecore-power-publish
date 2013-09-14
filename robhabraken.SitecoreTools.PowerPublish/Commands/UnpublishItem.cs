
namespace robhabraken.SitecoreTools.PowerPublish.Commands
{
    using robhabraken.SitecoreTools.PowerPublish;
    using Sitecore.Shell.Framework.Commands;

    /// <summary>
    /// This button enables you to unpublish an item with a single click. It will change the Publish Restrictions
    /// as it unchecks the Publishable option on the Item tab and publishes the item after that, using a full Republish without Subitems.
    /// </summary>
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
