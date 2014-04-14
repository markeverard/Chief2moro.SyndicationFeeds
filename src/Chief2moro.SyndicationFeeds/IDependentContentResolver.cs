using System.Collections.Generic;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds
{
    public interface IDependentContentResolver
    {
        IEnumerable<ContentReference> GetContentReferences(SyndicationFeedPageType currentPage);
    }
}