using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Chief2moro.SyndicationFeeds.Initialisation
{
[ModuleDependency(typeof(ServiceContainerInitialization))]
[InitializableModule]
    public class FeedContainerInitialisation : IConfigurableModule
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

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(x => x.For<IFeedContentResolver>().Use<FeedContentResolver>());
            context.Container.Configure(x => x.For<IFeedContentFilterer>().Use<FeedContentFilterer>());
            context.Container.Configure(x => x.For<IFeedDescriptionProvider>().Use<FeedDescriptionProvider>());
        }
    }
}