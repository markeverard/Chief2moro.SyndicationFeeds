using EPiServer.Core;
using System.ServiceModel.Syndication;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Responsible for modifying / extended item from content before return  
    /// </summary>
    public class ItemNullModifier : IItemModifier
    {
        public SyndicationItem Modify(SyndicationItem item, IContent content)
        {
            return item;
        }
    }
}
