using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace Chief2moro.SyndicationFeeds
{
    public class SyndicationItemFactory
    {
        protected IContentLoader ContentLoader;

        protected IFeedContentResolver FeedContentResolver;
        protected IFeedContentFilterer FeedFilterer;
        protected IItemDescriptionProvider ItemDescriptionProvider;
        protected IItemModifier ItemModifier;

        protected SyndicationFeedContext FeedContext;
      
        public SyndicationItemFactory(IContentLoader contentLoader, IFeedContentResolver feedContentResolver, 
                                        IFeedContentFilterer feedFilterer, IItemDescriptionProvider itemDescriptionProvider,
                                        IItemModifier itemModifier, SyndicationFeedContext feedContext)
        {
            ContentLoader = contentLoader;
            FeedContentResolver = feedContentResolver ?? new FeedContentResolver(ContentLoader);
            FeedFilterer = feedFilterer ?? new FeedContentFilterer();
            ItemDescriptionProvider = itemDescriptionProvider ?? new ItemDescriptionProvider();
            ItemModifier = itemModifier ?? new ItemNullModifier();
            FeedContext = feedContext;
        }

        /// <summary>
        /// Gets a list of populated syndication items created from the dependent content references on the gived SyndicationFeedPage.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SyndicationItem> GetSyndicationItems()
        {
            var contentReferences = FeedContentResolver.GetContentReferences(FeedContext);
            var contentItems = ContentLoader.GetItems(contentReferences, new LoaderOptions {LanguageLoaderOption.Fallback()});
            var filteredItems = FeedFilterer.FilterSyndicationContent(contentItems, FeedContext);
            var syndicationItems = filteredItems.Select(CreateSyndicationItem).ToList();

            return syndicationItems.OrderByDescending(c => c.LastUpdatedTime).Take(FeedContext.FeedPageType.MaximumItems);
        }

        private SyndicationItem CreateSyndicationItem(IContent content)
        {
            var changeTrackable = content as IChangeTrackable;
            var versionable = content as IVersionable;

            var changed = DateTime.Now;
            var changedby = string.Empty;
            
            if (changeTrackable != null)
            {
                changed = changeTrackable.Saved;
                changedby = changeTrackable.ChangedBy;
            }
            
            var item = new SyndicationItem
            {
                Title = new TextSyndicationContent(content.Name),
                Summary = new TextSyndicationContent(ItemDescriptionProvider.ItemDescripton(content)),
                LastUpdatedTime = changed
            };

            if (versionable != null)
            {
                var published = versionable.StartPublish;
                if (published.HasValue)
                    item.PublishDate = published.Value;
            }
            
            var categorizable = content as ICategorizable;
            if (categorizable != null)
            {
                var categoryRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();
                foreach (var category in categorizable.Category)
                {
                    item.Categories.Add(new SyndicationCategory(categoryRepository.Get(category).Description));
                }
            }         

            var mimeType = GetMimeType(content);
            Uri url = GetItemUrl(content);

            item.Content = new UrlSyndicationContent(url, mimeType);
            item.AddPermalink(url);
            item.Authors.Add(new SyndicationPerson(string.Empty, changedby, string.Empty));

            item = ItemModifier.Modify(item, content);


            return item;
        }

        private string GetMimeType(IContent content)
        {
            if (!(content is MediaData)) 
                return "text/html";
            
            var mediaContent = content as MediaData;
            return mediaContent.MimeType;
        }

        private Uri GetItemUrl(IContent content)
        {
            var feedPageUrl = UrlResolver.Current.GetUrl(FeedContext.FeedPageType.ContentLink);

            string contentUrl = content is BlockData
                ? string.Format("{0}item?contentId={1}", feedPageUrl, content.ContentLink.ID)
                : UrlResolver.Current.GetUrl(content.ContentLink);

            var absoluteSiteUrl = SiteDefinition.Current.SiteUrl.ToString().TrimEnd('/');
            return new Uri(absoluteSiteUrl + contentUrl);
        }
    }
}