using System.Web.Mvc;
using Chief2moro.SyndicationFeeds.Models;
using EPiServer.Web.Mvc;

namespace Chief2moro.SyndicationFeeds.Controllers
{
    public class SyndicationFeedPartialController : PartialContentController<SyndicationFeedPageType>
    {
        public override ActionResult Index(SyndicationFeedPageType currentContent)
        {
            return PartialView("~/modules/Chief2moro.SyndicationFeed/Views/Partial.cshtml", currentContent);
        }
    }
}