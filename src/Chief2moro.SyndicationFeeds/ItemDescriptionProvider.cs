using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds
{
    public class ItemDescriptionProvider : IItemDescriptionProvider
    {
        public virtual string ItemDescripton(IContent content)
        {
            return string.Format("An src link to content with id = '{0}' and name = '{1}'", content.ContentLink.ID, content.Name);
        }
    }
}
