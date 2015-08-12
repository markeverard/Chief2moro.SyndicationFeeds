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
            var description = content.Property["MetaDescription"];

            if (description == null || string.IsNullOrWhiteSpace(description.ToString()))
            {
                description = description ?? content.Property["Description"];
            }

            return description != null && !string.IsNullOrWhiteSpace(description.ToString())
                       ? description.ToString()
                       : content.Name;
        }
    }
}