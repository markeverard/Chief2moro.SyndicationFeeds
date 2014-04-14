using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Chief2moro.SyndicationFeeds.Editor
{
    public class EnumEditorDescriptor<TEnum> : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            SelectionFactoryType = typeof(EnumSelectionFactory<TEnum>);
            ClientEditingClass = "epi.cms.contentediting.editors.SelectionEditor";
            base.ModifyMetadata(metadata, attributes);
        }
    }
}