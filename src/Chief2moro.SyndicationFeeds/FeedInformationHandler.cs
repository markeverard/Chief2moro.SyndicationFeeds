using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds
{
    public delegate string SetItemDescription(IContent contentData);
    
    public static class FeedInformationHandler
    {
        public static SetItemDescription SetItemDescription;
       
        public static void SetDefaultBehaviour()
        {
            SetItemDescription = DefaultItemDescription;
        }

        private static string DefaultItemDescription(IContent content)
        {
            return string.Format("An src link to content with id = '{0}' and name = '{1}'", content.ContentLink.ID, content.Name);
        }
    }
}