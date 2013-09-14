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

    /// <summary>
    /// Helper methods for easy publishing from code, that are not natively available in the Sitecore API.
    /// </summary>
    public class PublishingHelper
    {

        /// <summary>
        /// Publishes an item in all languages to all publishing targets.
        /// </summary>
        /// <param name="item">The item to publish</param>
        /// <param name="deep">If deep is set to true, all publishable descendants will be published as well</param>
        /// <param name="compareRevisions">If compareRevisions is set to true, the Revision field in the versions of the item in source and target database are compared. If the Revision fields are the same, the item will not be published (also known as Smart Publish)</param>
        public void PublishAll(Item item, bool deep, bool compareRevisions)
        {
            var targets = this.GetPublishingTargets(item);
            PublishManager.PublishItem(item, targets.ToArray(), item.Languages, deep, compareRevisions);
        }

        /// <summary>
        /// Publishes an item in all languages to all publishing targets, while also publishing ancestors that are not yet published and thus are blocking this item from being published.
        /// This could be useful if a content editor creates a new folder in the media library for example, uploads a new file into that folder and publishes the file, but forgets to publish the folder itself (a common mistake when new to Sitecore).
        /// Since there would be no reason at all a content editor wants to publish a certain item, but doesn't want it to be shown by not publishing the parent folder, it's safe to assume this is what the content editor actually wants to happen.
        /// 
        /// This function doesn't publish descendants (subitems) because that would not only cause an infinite loop, but that would, in theory, publish the whole item tree.
        /// Therefore, this function only works to publish a single item, guaranteeing it will be visible in all publishing targets. However, publish restrictions are respected by this function, so be aware you use those restrictions correctly. 
        /// </summary>
        /// <param name="item">The item to publish, including its ancestors</param>
        public void PublishReversedRecursive(Item item)
        {
            if (item != null && !PresentInAllPublishingTargets(item))
            {
                this.PublishReversedRecursive(item.Parent);
                this.PublishAll(item, false, false);
            }
        }

        /// <summary>
        /// Verifies if the given item is present in all publishing targets.
        /// </summary>
        /// <param name="item">The item to look for in the publishing targets</param>
        /// <returns>True if the item is present in all publishing targets (and thus published), false if the item isn't present in one or more publishing targets</returns>
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

        /// <summary>
        /// Returns a list of publishing targets that the given item is actually published to.
        /// </summary>
        /// <param name="item">The item to look for in the publishing targets</param>
        /// <returns>A list of all publishing targets where the given item is found</returns>
        public List<string> ListPublishedTargets(Item item)
        {
            var publishedTargets = new List<string>();

            var publishingTargets = this.GetPublishingTargets(item);
            foreach (var database in publishingTargets)
            {
                if (database.SelectSingleItem(item.ID.ToString()) != null)
                {
                    publishedTargets.Add(database.Name);
                }
            }

            return publishedTargets;
        }

        /// <summary>
        /// Returns true if the latest revision of the current item is published to all publishing targets.
        /// </summary>
        /// <param name="item">The item to look for in the publishing targets</param>
        /// <returns>True if the item is up to date in all publishing targets</returns>
        public bool AllPublishingTargetsUpToDate(Item item)
        {
            bool allUpToDate = true;

            var publishingTargets = this.GetPublishingTargets(item);
            foreach (var database in publishingTargets)
            {
                var remoteItem = database.SelectSingleItem(item.ID.ToString());
                allUpToDate &= (remoteItem != null && item.Statistics.Revision.Equals(remoteItem.Statistics.Revision));
            }

            return allUpToDate;
        }

        /// <summary>
        /// Returns all publishing targets as a List of Database objects.
        /// This is actually the list to publish all items to, as it is incorrect to assume there is only one publishing target, called 'web'.
        /// </summary>
        /// <param name="item">The item to publish, used to determine the source database</param>
        /// <returns>A list of publishing targets for the database of the given item</returns>
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
