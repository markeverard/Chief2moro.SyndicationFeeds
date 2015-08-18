using System.Collections.Generic;
using System.Linq;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer.Core;
using EPiServer.Filters;

namespace Chief2moro.SyndicationFeeds
{
    public class FeedContentFilterer : IFeedContentFilterer
    {
        public IEnumerable<IContent> FilterSyndicationContent(IEnumerable<IContent> syndicationContentItems, SyndicationFeedPageType feedPage)
        {
            //filter by publish rights and access
            var filteredItems = FilterForVisitor.Filter(syndicationContentItems);

            //filter editor set excluded types
            var excludedAllTypes = ParseExcludedIds(feedPage.ExcludedContentTypes);
          
            filteredItems = filteredItems.Where(c => !excludedAllTypes.Contains(c.ContentTypeID));

            //filter by category
            if (feedPage.CategoryFilter == null)
                return filteredItems;

            if (!feedPage.CategoryFilter.IsEmpty)
            {
                filteredItems = filteredItems
                    .Where(c => c is ICategorizable)
                    .Where(c => ((ICategorizable)c).Category.MemberOfAny(feedPage.CategoryFilter));
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