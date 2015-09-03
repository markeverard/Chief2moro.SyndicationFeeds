using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace Chief2moro.SyndicationFeeds
{
    public class FeedContentFilterer : IFeedContentFilterer
    {
        public IEnumerable<IContent> FilterSyndicationContent(IEnumerable<IContent> syndicationContentItems, SyndicationFeedPageType feedPage)
        {
            //filter editor set excluded types
            var excludedAllTypes = ParseExcludedIds(feedPage.ExcludedContentTypes);
            var filteredItems = syndicationContentItems.Where(c => !excludedAllTypes.Contains(c.ContentTypeID)).ToList();

            //filter by category
            if (feedPage.CategoryFilter != null)
            {
                if (!feedPage.CategoryFilter.IsEmpty)
                {
                    filteredItems = filteredItems
                        .Where(c => c is ICategorizable)
                        .Where(c => ((ICategorizable) c).Category.MemberOfAny(feedPage.CategoryFilter)).ToList();
                }
            }

            //block types are alkways removed by filter for visitor. We want to see them and respect access rights
            var currentlyFiltered = new IContent[filteredItems.Count];
            filteredItems.CopyTo(currentlyFiltered);
            
            //filter by published, access and template
            var published = ServiceLocator.Current.GetInstance<IPublishedStateAssessor>();
            var access = (IContentFilter) new FilterAccess();
            var template = (IContentFilter) new FilterTemplate();

            foreach (var filteredItem in currentlyFiltered)
            {
                var shouldFilter = false;

                shouldFilter = access.ShouldFilter(filteredItem);
                shouldFilter = shouldFilter || !published.IsPublished(filteredItem);

                if (!(filteredItem is BlockData))
                    shouldFilter = shouldFilter || template.ShouldFilter(filteredItem);
                
            
                if (shouldFilter)
                    filteredItems.Remove(filteredItem);
            }
           
            return filteredItems;
        }

        private IEnumerable<int> ParseExcludedIds(string excludedContentPropertyValue)
        {
            return !string.IsNullOrEmpty(excludedContentPropertyValue)
                ? excludedContentPropertyValue.Split(',').Select(int.Parse).ToList()
                : new List<int>();
        } 
    }
}