using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chief2moro.SyndicationFeeds
{
    public class FeedDescriptionProvider : IFeedDescriptionProvider
    {
        public virtual string ItemDescripton(IContent content)
        {
            return FeedInformationHandler.SetItemDescription(content);
        }
    }
}
