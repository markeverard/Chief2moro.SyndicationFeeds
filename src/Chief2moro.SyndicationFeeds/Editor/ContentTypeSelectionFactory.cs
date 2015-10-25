using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

namespace Chief2moro.SyndicationFeeds.Editor
{
    public class ContentTypeSelectionFactory : ISelectionFactory
    {
        public Injected<IContentTypeRepository> _typeRepository;
 
        /// <summary>
        /// Gets PageTypes using DisplayName if not null, else Name and not PageTypes SysRoot or SysRecycleBin
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns>IEnumerable of SelectItems</returns>
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var typeRepository = _typeRepository.Service;
            return typeRepository
                    .List()
                    .Where(c => c.Name != "SysRoot" && c.Name != "SysRecycleBin" && c.Name != "SysContentFolder" & c.Name != "SysContentAssetFolder")
                    .Select(t => new SelectItem()
                    {
                        Text = !string.IsNullOrWhiteSpace(t.DisplayName) ? t.DisplayName : t.Name, 
                        Value = t.ID.ToString()
                    });
        }
    }
}