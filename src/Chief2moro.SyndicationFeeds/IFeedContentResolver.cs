using System.Collections.Generic;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Responsible for collating all content references set on the given SyndicationFeedPage
    /// </summary>
    public interface IFeedContentResolver
    {
        IEnumerable<ContentReference> GetContentReferences(SyndicationFeedPageType currentPage);
    }
}