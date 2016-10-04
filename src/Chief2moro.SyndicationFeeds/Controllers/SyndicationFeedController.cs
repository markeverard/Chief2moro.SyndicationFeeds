using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using EPiServer.Framework.Cache;
using System;
using EPiServer.Security;

namespace Chief2moro.SyndicationFeeds.Controllers
{
    public class SyndicationFeedController : PageController<SyndicationFeedPageType>
    {
        protected CategoryRepository CatRepository;
        protected IContentLoader ContentLoader;
        protected IFeedContentResolver FeedContentResolver;
        protected IFeedContentFilterer FeedFilterer;
        protected IItemModifier ItemModifier;

        public SyndicationFeedController()
        {
            ContentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            FeedContentResolver = ServiceLocator.Current.GetInstance<IFeedContentResolver>();
            FeedFilterer = ServiceLocator.Current.GetInstance<IFeedContentFilterer>();

            ItemModifier = ServiceLocator.Current.GetInstance<IItemModifier>();

            CatRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();
        }

        public SyndicationFeedController(IContentLoader contentLoader, IFeedContentResolver feedContentResolver, IFeedContentFilterer feedContentFilterer, IItemModifier itemModifier, CategoryRepository categoryRepository)
        {
            ContentLoader = contentLoader;
            FeedContentResolver = feedContentResolver;
            FeedFilterer = feedContentFilterer;
            ItemModifier = itemModifier;
            CatRepository = categoryRepository;
        }

        public ActionResult Index(SyndicationFeedPageType currentPage, string categories)
        {
            var parsedCategories = ParseCategories(categories);
            var feedContext = new SyndicationFeedContext(currentPage, parsedCategories.ToList());

            var siteUrl = SiteDefinition.Current.SiteUrl.ToString().TrimEnd('/');
            var currentUri = new Uri(siteUrl + UrlResolver.Current.GetUrl(currentPage.ContentLink));

            var syndicationFactory = new SyndicationItemFactory(ContentLoader, FeedContentResolver, FeedFilterer, ItemModifier, feedContext);

            var items = GetFromCacheOrFactory(syndicationFactory, currentPage, parsedCategories);
            
            var feed = new SyndicationFeed
            {
                Items = items,
                Id = siteUrl + UrlResolver.Current.GetUrl(ContentReference.StartPage),         
                Title = new TextSyndicationContent(currentPage.PageName),
                Description = new TextSyndicationContent(currentPage.Description),
                Language = currentPage.LanguageBranch,
                Generator = "http://nuget.episerver.com/en/OtherPages/Package/?packageId=Chief2moro.SyndicationFeeds"
            };

            feed.Links.Add(new SyndicationLink() { Uri = currentUri, RelationshipType = "self" });
      
            if (currentPage.Category != null)
            {
                var categoryRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();
                foreach (var category in currentPage.Category)
                {
                    feed.Categories.Add(new SyndicationCategory(categoryRepository.Get(category).Description));
                }
            }

            if (feed.Items.Any())
                feed.LastUpdatedTime = feed.Items.Max(m => m.LastUpdatedTime);

            if (currentPage.FeedFormat == FeedFormat.Atom)
                return new AtomActionResult(feed);
           
            return new RssActionResult(feed);
        }

        public ActionResult Item(SyndicationFeedPageType currentPage, int? contentId)
        {
            if (!contentId.HasValue)
                return HttpNotFound("No content Id specified");

            var contentReference = ContentReference.Parse(contentId.Value.ToString());
            var feedContext = new SyndicationFeedContext(currentPage);

            var referencedContent = FeedContentResolver.GetContentReferences(feedContext);
            if (!referencedContent.Contains(contentReference))
                return HttpNotFound("Content Id not exposed in this feed");
            
            var contentArea = new ContentArea();
            var item = new ContentAreaItem {ContentLink = contentReference};
            contentArea.Items.Add(item);

            var contentItem = ContentLoader.Get<IContent>(contentReference);

            var model = new ContentHolderModel { Tag = currentPage.BlockRenderingTag, ContentArea = contentArea, Content = contentItem};
            return View("~/modules/Chief2moro.SyndicationFeeds/Views/Item.cshtml", model);
        }

        private IEnumerable<Category> ParseCategories(string categories)
        {
            if (!string.IsNullOrEmpty(categories))
            {
                foreach (var categoryQuery in categories.Split(','))
                {
                        var category = CatRepository.Get(categoryQuery);

                        if (category != null)
                            yield return category;
                }
            }
        }
        
        private IEnumerable<SyndicationItem> GetFromCacheOrFactory(SyndicationItemFactory syndicationFactory, SyndicationFeedPageType currentPage, IEnumerable<Category> parsedCategories)
        {
            var cacheTime = currentPage.CacheFeedforSeconds;

            string categoryQuery = string.Empty;
            foreach (var category in parsedCategories)
            {
                categoryQuery += category.Name;
            }

            var cacheKey = string.Format("SyndicationFeedPageType_{0}_{1}", currentPage.ContentLink.ToString(), categoryQuery);

            var cachedItems = CacheManager.Get(cacheKey) as IEnumerable<SyndicationItem>;
            if (cachedItems == null)
            {
                cachedItems = syndicationFactory.GetSyndicationItems();

                if (cacheTime > 0 && !PrincipalInfo.HasEditorAccess)
                { 
                    var cachePolicy = new CacheEvictionPolicy(new[] { DataFactoryCache.PageCommonCacheKey(currentPage.ContentLink) }
                                                            , new TimeSpan(0, 0, cacheTime),
                                                            CacheTimeoutType.Absolute);

                    CacheManager.Insert(cacheKey, syndicationFactory.GetSyndicationItems(), cachePolicy);
                }
            }

            return cachedItems;
        }
    }
}