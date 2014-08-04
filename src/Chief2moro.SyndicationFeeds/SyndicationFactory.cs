using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using log4net;

namespace Chief2moro.SyndicationFeeds
{
    public class SyndicationFactory
    {
        private readonly IContentLoader _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        private readonly IFeedContentResolver _feedContentResolver = ServiceLocator.Current.GetInstance<IFeedContentResolver>();
        private readonly SyndicationFeedPageType _feedPage;
        private readonly ILog _log = LogManager.GetLogger(typeof(SyndicationFactory));
      
        public SyndicationFactory(SyndicationFeedPageType feedPage)
        {
            _feedPage = feedPage;
        }

        public IEnumerable<SyndicationItem> GetSyndicationItems()
        {
            return _feedContentResolver.GetContentReferences(_feedPage)
                .Select(CreateItemFromReference)
                .Where(i => i != null)
                .Where(i => _feedPage.IncludeItemsWithoutSummary || !string.IsNullOrEmpty(i.Summary.Text))
                .OrderByDescending(c => c.PublishDate)
                .Take(_feedPage.MaximumItems)
                .ToList();
        }

        private SyndicationItem CreateItemFromReference(ContentReference contentReference)
        {
            var content = _contentLoader.Get<IContent>(contentReference);
            var changeTrackable = content as IChangeTrackable;
            if (changeTrackable == null)
            {
                return null;
            }

            var mimeType = GetMimeType(content);
            var url = GetItemUrl(content);
            var publishDate = _feedPage.OrderByCreatedDate ? changeTrackable.Created : changeTrackable.Saved;
            var summary = GetSummary(content, _feedPage.PropertyContainingSummary);

            var item = new SyndicationItem
            {
                Id = content.ContentLink.ID.ToString(),
                Title = new TextSyndicationContent(content.Name),
                Summary = new TextSyndicationContent(summary),
                PublishDate = publishDate,
                LastUpdatedTime = publishDate,
                Content = new UrlSyndicationContent(url, mimeType),
            };

            item.AddPermalink(url);
            item.Authors.Add(new SyndicationPerson(string.Empty, changeTrackable.ChangedBy, string.Empty));

            return item;
        }

        private string GetSummary(IContent content, string propertyName)
        {
            if (propertyName == null)
            {
                return content.Name;
            }

            var pageData = content as PageData;

            if (pageData == null)
            {
                return "";
            }

            var summary = pageData[propertyName];

            return summary == null ? "" : summary.ToString();
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
            var feedPageUrl = UrlResolver.Current.GetUrl(_feedPage.ContentLink);

            var contentUrl = content is BlockData
                ? string.Format("{0}item?contentId={1}", feedPageUrl, content.ContentLink.ID)
                : UrlResolver.Current.GetUrl(content.ContentLink);

            var absoluteSiteUrl = SiteDefinition.Current.SiteUrl.ToString().TrimEnd('/');
            return new Uri(absoluteSiteUrl + contentUrl);
        }
    }
}