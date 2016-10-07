using EPiServer.Core;
using EPiServer.DataAbstraction;
using System.Collections.Generic;

namespace Chief2moro.SyndicationFeeds.Models
{
    public class SyndicationFeedContext
    {
        public SyndicationFeedContext(SyndicationFeedPageType feedPageType)
        {
            FeedPageType = feedPageType;
            CategoriesFilter = feedPageType.CategoryFilter != null 
                ? feedPageType.CategoryFilter.CreateWritableClone() 
                : new CategoryList();
        }

        public SyndicationFeedContext(SyndicationFeedPageType feedPageType, List<Category> categoriesFilter)
        {
            FeedPageType = feedPageType;
            CategoriesFilter = feedPageType.CategoryFilter != null
               ? feedPageType.CategoryFilter.CreateWritableClone()
               : new CategoryList();
            
            AddContextCategories(categoriesFilter);                    
        }

        public SyndicationFeedPageType FeedPageType { get; internal set; }
        public CategoryList CategoriesFilter { get; set; }

        private void AddContextCategories(List<Category> categoriesFilter)
        {
            if (categoriesFilter == null)
                return;

            foreach (var category in categoriesFilter)
            {
                if (!CategoriesFilter.Contains(category.ID))
                    CategoriesFilter.Add(category.ID);
            }

        }
    }

}
