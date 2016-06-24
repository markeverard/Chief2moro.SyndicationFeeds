using System.Collections.Generic;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Responsible for filtering all content before inclusion in the feed 
    /// </summary>
    public interface IFeedContentFilterer
    {
        IEnumerable<IContent> FilterSyndicationContent(IEnumerable<IContent> syndicationContentItems, SyndicationFeedContext feedContext);
    }
}