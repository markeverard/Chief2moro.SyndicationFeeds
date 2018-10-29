using System.Collections.Generic;
using System.Linq;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Responsible for collating all content references set on the given SyndicationFeedPage
    /// </summary>
    [ServiceConfiguration(ServiceType = typeof(IFeedContentResolver), Lifecycle = ServiceInstanceScope.HttpContext)]
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
        /// <param name="feedContext">The context of the feed inclusing the current page.</param>
        /// <returns></returns>
        public IEnumerable<ContentReference> GetContentReferences(SyndicationFeedContext feedContext)
        {
            var feedPage = feedContext.FeedPageType;

            var dependentContentItems = new List<ContentReference>();

            if (!ContentReference.IsNullOrEmpty(feedPage.PageFolder))
                dependentContentItems.AddRange(GetDescendentsOfType<PageData>(feedPage.PageFolder));

            if (!ContentReference.IsNullOrEmpty(feedPage.BlockFolder))
                dependentContentItems.AddRange(GetDescendentsOfType<BlockData>(feedPage.BlockFolder));

            if (!ContentReference.IsNullOrEmpty(feedPage.MediaFolder))
                dependentContentItems.AddRange(GetDescendentsOfType<MediaData>(feedPage.MediaFolder));

            if (feedPage.ContentItems == null)
                return dependentContentItems;

            var itemsInContentArea = feedPage.ContentItems.Items.Select(contentItem => contentItem.ContentLink);
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