using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Responsible for providing description/summary information for all content before inclusion in the feed 
    /// </summary>
    public interface IItemDescriptionProvider
    {
        string ItemDescripton(IContent content);
    }
}
