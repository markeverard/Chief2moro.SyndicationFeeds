Chief2moro.SyndicationFeeds
===========================

Flexible ATOM/RSS feed creation for EPiServer CMS, with custom extension points allowing developers to provide custom methods for filtering and item display within a feed. 

###Editor functionality

* Editors can create multiple feeds (a feed is a Page Type)
* Feeds contain a date ordered (most recently published first) list of content items
* Feed items now contain any categories that are selected for that item
* Editors can specify whether a feed is delivered in RSS or ATOM format
* Editors can specify how many items appear in the feed.
* Editors have granular control over what content items are shared. They can include any of the following in a single feed.
 * Descendents of a specified page in the page tree
 * Child blocks contained within a specified content folder
 * Child media items within a specified media folder
 * Any number of individually selected pages, blocks or media items, via a ContentArea property
* Editors can filter content items that are shared via the following methods
  * Content items can be excluded by ContentType - allowing you to select a folder, but hide content items of a certain type
  * Filter via category.
* Blocks included in a feed are externally routed so their HTML output can be consumed by external systems.
* Feed pages have a partial renderer meaning they can be dragged into Content Areas to display an RSS feed logo and link.
