using Chief2moro.SyndicationFeeds.Models;
using EPiServer.Shell;

namespace Chief2moro.SyndicationFeeds.Editor
{
    [UIDescriptorRegistration]
    public class FeedUIDescriptor : UIDescriptor<SyndicationFeedPageType>
    {  
        public FeedUIDescriptor()
        {
            DefaultView = CmsViewNames.AllPropertiesView;
        }
    } 
}