using System.ComponentModel.DataAnnotations;
using Chief2moro.SyndicationFeeds.Editor;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;

namespace Chief2moro.SyndicationFeeds.Models
{
    [ContentType(DisplayName = "Syndication Feed", 
        GUID = "7082c0ee-1efa-4f60-ad7f-735f45c42689", 
        Description = "A page displaying a feed of the selected content items in RSS or ATOM feed format")]
    [ImageUrl("~/modules/Chief2moro.SyndicationFeeds/Images/syndicationfeedpagetype-icon.png")]
    public class SyndicationFeedPageType : PageData
    {
        [Display(
            Name = "Description",
            Description = "A description of the content feed.",
            GroupName = FeedPageTypeTabNames.FeedSettings,
            Order = 400)]
       [UIHint(UIHint.Textarea)]
        [Required]
       public virtual string Description { get; set; }

        [Display(
            Name = "Call-to-action",
            Description = "A call to action displayed when the feed page is used within a content area.",
            GroupName = FeedPageTypeTabNames.FeedSettings,
            Order = 500)]
        [Required]
        public virtual string CallToAction { get; set; }
        
        [Display(
            Name = "Feed format",
            Description = "Select the output format for the content feed",
            GroupName = FeedPageTypeTabNames.FeedSettings,
            Order = 10)]
        [BackingType(typeof(PropertyNumber))]
        [EditorDescriptor(EditorDescriptorType = typeof(EnumEditorDescriptor<FeedFormat>))]
        public virtual FeedFormat FeedFormat { get; set; }

        [Display(
          Name = "Maximum items in feed",
          Description = "Sets the maximum number of items to display in the syndication feed",
          GroupName = FeedPageTypeTabNames.FeedSettings,
          Order = 20)]
        [Range(0, int.MaxValue)]
        public virtual int MaximumItems { get; set; }

        [Display(
           Name = "Block rendering tag",
           Description = "Sets the rendering tags for blocks accessed as child elements of this syndication feed",
           GroupName = FeedPageTypeTabNames.FeedSettings,
           Order = 30)]
        public virtual string BlockRenderingTag { get; set; }

        [Display(
           Name = "Cache feed (in seconds)",
           Description = "Sets length of time the feed will be cached for. This will improve performance but limit the time until new items will appear in the syndication feed",
           GroupName = FeedPageTypeTabNames.FeedSettings,
           Order = 40)]
        [Range(0, int.MaxValue)]
        public virtual int CacheFeedforSeconds { get; set; }

        [Display(
            Name = "Content items",
            Description = "Any content items added to this content area will be displayed in the feed",
            GroupName = FeedPageTypeTabNames.Content,
            Order = 10)]
        public virtual ContentArea ContentItems { get; set; }

        [Display(
            Name = "Pages folder items",
            Description = "Selecting a page will add all pages saved as descendents of that page to the content feed",
            GroupName = FeedPageTypeTabNames.Content,
            Order = 20)]
        public virtual ContentReference PageFolder { get; set; }

        [UIHint(UIHint.AssetsFolder)]
        [Display(
            Name = "Media folder items",
            Description = "Selecting a media folder will add all media saved within that folder to the content feed",
            GroupName = FeedPageTypeTabNames.Content,
            Order = 30)]
        public virtual ContentReference MediaFolder { get; set; }

        [UIHint(UIHint.AssetsFolder)]
        [Display(
            Name = "Block folder items",
            Description = "Selecting a block folder will add all blocks saved within that folder to the content feed",
            GroupName = FeedPageTypeTabNames.Content,
            Order = 40)]
        public virtual ContentReference BlockFolder { get; set; }

        [Display(
            GroupName = FeedPageTypeTabNames.Filters, 
            Name = "Only display content items with selected categories",
            Description = "only content types that have categories matching this filter will be shown",
            Order = 10)]
        public virtual CategoryList CategoryFilter { get; set; }

        [Display(
            GroupName = FeedPageTypeTabNames.Filters,
            Name = "Exclude specified content types",
            Description = "Selected content types will be filtered out from the feed",
            Order = 20)]
        [SelectMany(SelectionFactoryType = typeof(ContentTypeSelectionFactory))]
        public virtual string ExcludedContentTypes { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            MaximumItems = 50;
            CacheFeedforSeconds = 0;
        }
    }
}