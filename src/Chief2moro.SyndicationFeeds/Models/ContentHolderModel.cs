using EPiServer.Core;

namespace Chief2moro.SyndicationFeeds.Models
{
    public class ContentHolderModel
    {
        public ContentArea ContentArea { get; set; }
        public string Tag { get; set; }
        public IContent Content { get; set; }
    }
}