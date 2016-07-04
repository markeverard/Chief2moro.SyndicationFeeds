using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Chief2moro.SyndicationFeeds
{
    [ServiceConfiguration(ServiceType = typeof(IItemDescriptionProvider), Lifecycle = ServiceInstanceScope.HttpContext)]
    public class ItemDescriptionProvider : IItemDescriptionProvider
    {
        public virtual string ItemDescripton(IContent content)
        {
            return string.Format("An src link to content with id = '{0}' and name = '{1}'", content.ContentLink.ID, content.Name);
        }
    }
}
