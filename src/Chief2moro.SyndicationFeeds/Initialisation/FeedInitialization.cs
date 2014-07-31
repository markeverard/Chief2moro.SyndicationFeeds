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
            var resolver = ServiceLocator.Current.GetInstance<TemplateResolver>();
            resolver.TemplateResolved += TemplateResolverOnTemplateResolved;
        }

        public static void TemplateResolverOnTemplateResolved(object sender, TemplateResolverEventArgs templateResolverEventArgs)
        {
            if (templateResolverEventArgs.ContentType == null)
                return;
            
            if (templateResolverEventArgs.ContentType.ID != 26)
                return;
            
            Debug.WriteLine(templateResolverEventArgs.ContentType);
            Debug.WriteLine(templateResolverEventArgs.ItemToRender);

            if (templateResolverEventArgs.SelectedTemplate == null)
                return;

            Debug.WriteLine(templateResolverEventArgs.SelectedTemplate.Name);
            Debug.WriteLine(templateResolverEventArgs.SelectedTemplate.TemplateType);

        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}