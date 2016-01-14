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
using System.Diagnostics;
using System.Collections.Generic;

namespace Chief2moro.SyndicationFeeds.Controllers
{
    public class SyndicationFeedController : PageController<SyndicationFeedPageType>
    {
        protected CategoryRepository CatRepository;
        protected IContentLoader ContentLoader;
        protected IFeedContentResolver FeedContentResolver;
        protected IFeedContentFilterer FeedFilterer;
        protected IFeedDescriptionProvider FeedDescriptionProvider;

        public SyndicationFeedController()
        {
            ContentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            FeedContentResolver = ServiceLocator.Current.GetInstance<IFeedContentResolver>();
            FeedFilterer = ServiceLocator.Current.GetInstance<IFeedContentFilterer>();
            FeedDescriptionProvider = ServiceLocator.Current.GetInstance<IFeedDescriptionProvider>();
            CatRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();
        }

        public SyndicationFeedController(IContentLoader contentLoader, IFeedContentResolver feedContentResolver, IFeedContentFilterer feedContentFilterer, IFeedDescriptionProvider feedDescriptionProvider, CategoryRepository categoryRepository)
        {
            ContentLoader = contentLoader;
            FeedContentResolver = feedContentResolver;
            FeedFilterer = feedContentFilterer;
            FeedDescriptionProvider = feedDescriptionProvider;
            CatRepository = categoryRepository;
        }

        [OutputCache(Duration = 60)]
        public ActionResult Index(SyndicationFeedPageType currentPage, string categories)
        {
            var parsedCategories = ParseCategories(categories);
            var feedContext = new SyndicationFeedContext(currentPage, parsedCategories.ToList());

            var syndicationFactory = new SyndicationItemFactory(ContentLoader, FeedContentResolver, FeedFilterer, FeedDescriptionProvider, feedContext);
            
            var feed = new SyndicationFeed
            {
                Items = syndicationFactory.GetSyndicationItems(),
                Id = SiteDefinition.Current.SiteUrl.ToString().TrimEnd('/') + UrlResolver.Current.GetUrl(ContentReference.StartPage),         
                Title = new TextSyndicationContent(currentPage.PageName),
                Description = new TextSyndicationContent(currentPage.Description),
                Language = currentPage.LanguageBranch,
                Generator = "http://nuget.episerver.com/en/OtherPages/Package/?packageId=Chief2moro.SyndicationFeeds"
            };

            if (currentPage.Category != null)
            {
                var categoryRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();
                foreach (var category in currentPage.Category)
                {
                    feed.Categories.Add(new SyndicationCategory(categoryRepository.Get(category).Description));
                }
            }

            if (feed.Items.Any())
                feed.LastUpdatedTime = feed.Items.Max(m => m.PublishDate);

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
     }
}