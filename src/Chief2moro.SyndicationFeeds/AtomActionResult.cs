﻿using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace Chief2moro.SyndicationFeeds
{
    /// <summary>
    /// Returns an Atom feed to the response stream.
    /// </summary>
    public class AtomActionResult : ActionResult
    {
        readonly SyndicationFeed _feed;

        /// <summary>
        /// Constructor to set up the action result feed.
        /// </summary>
        /// <param name="feed">Accepts a <see cref="SyndicationFeed"/>.</param>
        public AtomActionResult(SyndicationFeed feed)
        {
            _feed = feed;
        }

        /// <summary>
        /// Executes the call to the ActionResult method and returns the created feed to the output response.
        /// </summary>
        /// <param name="context">Accepts the current <see cref="ControllerContext"/>.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/atom+xml";
            var formatter = _feed.GetAtom10Formatter();

            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}
