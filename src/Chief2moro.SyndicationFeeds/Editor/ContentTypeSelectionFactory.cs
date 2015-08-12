using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Shell.ObjectEditing;

namespace Chief2moro.SyndicationFeeds.Editor
{
    public class ContentTypeSelectionFactory : ISelectionFactory
    {
        private readonly ContentTypeRepository _pageTypeRepository;

        public ContentTypeSelectionFactory()
        {
            _pageTypeRepository = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<ContentTypeRepository>();
        }

        /// <summary>
        /// Gets PageTypes using DisplayName if not null, else Name and not PageTypes SysRoot or SysRecycleBin
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns>IEnumerable of SelectItems</returns>
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return _pageTypeRepository
                    .List()
                    .Where(c => c.Name != "SysRoot" && c.Name != "SysRecycleBin")
                    .Select(t => new SelectItem()
                    {
                        Text = !string.IsNullOrWhiteSpace(t.DisplayName) ? t.DisplayName : t.Name, 
                        Value = t.ID
                    });
        }

    }
}