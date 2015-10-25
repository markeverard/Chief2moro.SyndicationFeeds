using System.Collections.Generic;
using System.Linq;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer;
using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Responsible for collating all content references set on the given SyndicationFeedPage
    /// </summary>
    public class FeedContentResolver : IFeedContentResolver
    {
        protected IContentLoader ContentLoader;

        public FeedContentResolver(IContentLoader contentLoader)
        {
            ContentLoader = contentLoader;
        }

        /// <summary>
        /// Gets the content items that are referenced in the passed SyndicationFeedPage.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns></returns>
        public IEnumerable<ContentReference> GetContentReferences(SyndicationFeedPageType currentPage)
        {
            var dependentContentItems = new List<ContentReference>();

            if (!ContentReference.IsNullOrEmpty(currentPage.PageFolder))
                dependentContentItems.AddRange(GetDescendentsOfType<PageData>(currentPage.PageFolder));

            if (!ContentReference.IsNullOrEmpty(currentPage.BlockFolder))
                dependentContentItems.AddRange(GetDescendentsOfType<BlockData>(currentPage.BlockFolder));

            if (!ContentReference.IsNullOrEmpty(currentPage.MediaFolder))
                dependentContentItems.AddRange(GetDescendentsOfType<MediaData>(currentPage.MediaFolder));

            if (currentPage.ContentItems == null)
                return dependentContentItems;

            var itemsInContentArea = currentPage.ContentItems.Items.Select(contentItem => contentItem.ContentLink);
            dependentContentItems.AddRange(itemsInContentArea);

            return dependentContentItems.Distinct();
        }

        private IEnumerable<ContentReference> GetDescendentsOfType<T>(ContentReference contentReference) where T : ContentData
        {     
            foreach (var reference in ContentLoader.GetDescendents(contentReference))
            {
                var contentItem = ContentLoader.Get<ContentData>(reference) as T;
                if (contentItem == null)
                    continue;

                yield return reference;
            }
        }
    }
}