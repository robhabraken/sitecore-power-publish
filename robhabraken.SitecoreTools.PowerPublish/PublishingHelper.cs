using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using Sitecore.Shell.Framework.Commands;

namespace robhabraken.SitecoreTools.PowerPublish
{
    public class PublishingHelper
    {

        // publish all languages to all targets
        public void PublishAll(Item item, bool deep, bool compareRevisions)
        {
            var targets = this.GetPublishingTargets(item);
            PublishManager.PublishItem(item, targets.ToArray(), item.Languages, deep, compareRevisions);
        }

        public void PublishReversedRecursive(Item item)
        {
            if (!PresentInAllPublishingTargets(item))
            {
                this.PublishReversedRecursive(item.Parent);
                this.PublishAll(item, false, false);
            }
        }

        public bool PresentInAllPublishingTargets(Item item)
        {
            bool published = true;

            var publishingTargets = this.GetPublishingTargets(item);
            foreach (var database in publishingTargets)
            {
                published &= database.SelectSingleItem(item.ID.ToString()) != null;
            }

            return published;
        }

        public List<Database> GetPublishingTargets(Item item)
        {
            var publishingTargets = new List<Database>();

            var publishingTargetsItem = item.Database.GetItem("/sitecore/system/publishing targets");
            if (publishingTargetsItem != null)
            {
                var children = publishingTargetsItem.GetChildren();
                foreach (Item child in children)
                {
                    var targetDatabase = Factory.GetDatabase(child["target database"]);
                    if (targetDatabase != null)
                    {
                        publishingTargets.Add(targetDatabase);
                    }
                }
            }

            return publishingTargets;
        }

    }
}
