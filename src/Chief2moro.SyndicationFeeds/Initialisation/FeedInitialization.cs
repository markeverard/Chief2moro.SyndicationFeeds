using System.Diagnostics;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace Chief2moro.SyndicationFeeds.Initialisation
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class FeedInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            FeedInformationHandler.SetDefaultBehaviour();
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}