using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Responsible for providing description/summary information for all content before inclusion in the feed 
    /// </summary>
    public interface IFeedDescriptionProvider
    {
        string ItemDescripton(IContent content);
    }
}
