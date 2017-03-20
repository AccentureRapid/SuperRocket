#Localization module for Orchard.

>Version 1.2 8/18/14 : Fixed a dependency problem ( Issue N° 7) that crashes site if you do not enable the feature  'Datwendo Browser culture selector' when enabling other features.

>Version 1.2 5/29/14 : Added country culture alternate. Fixed Pb related to Default Site culture changed, this necessitated rewriting Orchard.Localization over buggy AdminController and LocalizationPart Drivers.

>Version 1.2 5/20/14 : Improved the Dawendo.Localization.Alternates feature.

>Version 1.2 5/14/14 : Added the Dawendo.Localization.Alternates feature and enforceUrl settings for proxy-caching pbs.

Problem with Orchard.Localization is that this module looks like 'not finished', when you open its Admin Controller for action Translate, you see that it expects a 'to' parameter....which is never provided. (We will try to remedy to this) 

We can also be puzzled about the fact that the front-end Localization.ContentTranslations.chtml is always displaying all available translations? 

It means that Orchard's default behavior is to show all content independently of what culture the browsing user could understand. 
And if we implement some system to provide this user only the data he could understand, we have to disable or replace this template or the links it shows will be broken.

This module started as an evolution based on the work done in Orchard.CulturePicker (no, this is not an Orchard core module) and RM.Localization but it moved to its own identity.

Originally it was just a Home Page utility but it evolved to handle the whole site content.

I reengineered the code to get rid of the ‘not always well-behaving controller solution’ used by the previous modules, preferring an ActionFilter and OnActionExecuting method to set the page redirection based on user selection.

On the first version I separated 3 selectors: Browser, Cookie and Content. 
In the previous solutions, content was mixed either with cookie, either with browser.
Having a selector able to change current culture based on the content you are browsing seems a common requirement, sometimes.

Then following some discussion on Orchard forum, I added 2 more selectors: Admin and User.
Admin culture selector provides the ability to have a fixed admin culture, different from the default Site culture. It is active only in Admin mode and will preferably use the current user default if is exists.

User selector is for authenticated user mode, it relies on the fact that you are able to provide a ‘preferred culture’ for each user. 
As this is not implemented by default in Orchard User management, this will rely on an ‘interface IProfileProviderEventHandler’ you have to provide in your code, your implementation should bring back the preferred culture name for the current user (see the interface model).

I also added the possibility to adapt the priorities of each selector from the module settings, and included a notion of fallback.
Concerning priorities, remember that the ‘Default Site Culture Selector’ has a fixed priority of -5.
I suggest to adopt on the start something as:

- Content: 10
- User: 25
- Browser: 20
- Cookie: 30
- Admin: 80

We also need to manage fallback.

Idea of fallback is to decide something when there is no translated content for the actually selected culture. If some translation exists, which one to prefer?
In this module  the fallback process is selected in settings: either default Site culture, either 1st content culture, either page presenting all the possible translations (from RM), either a regex, but without overriding the DefaultCultureManager.

The module provides a very classical version of Cookie Widget and I added a MenuItem named MenuCultureLink.
This last one allows inserting a culture menu inside an orchard Menu: just go to navigation and add a Culture Link.
Code is not very efficient in the template, using a BuildDisplay to rebuild the submenu, but it runs Ok.
I got very interesting results with bootstrap nav bar with this MenuItem.

Also included, taken from RM, the idea to have the cookie selector itself falling back to browser.

Concerning Menu filtering, the settings propose 2 processes to decide what to do with MenuItems without any culture (not LocalizationPart attached): 

+	Either they are considered as Menu reserved for the default Site culture and appear only for this culture,

+	Either they are considered as ‘international’ menus and they appear for any culture.

Last Addition is a feature to insert alternates based on culture for the main elements of Orchard: Layout, Zone, Content, Menu, MenuItem, MenuItemsLink,  LocalMenu, LocalMenuItem, Widget, BodyPart.

With this feature enabled you can define in your theme alternate templates for each culture you use, after a first version allowing some templates, the actual version take a different way in CultureAlternatFactory, it traks the Shape OnDisplaying event and add a culture oriented alternate to each one existing.

Four kind of alternates are added, for example:

+ Content\_[DisplayType]\_\_[ContentType] is used to create Content\_[DisplayType]\_\_[Culture]\_\_[ContentType], this for current culture and its parent language version, this mean when en-US culture is current that you can create templates named
from Content_Product.Summary we will add Content-enus-Product.Summary.cshtml and Content-en-Product.Summary.cshtml

+ Next there will be a more explicit tuple using the [ShapeType]\_\_culture\_\_[Culture] alternates format, with the same previous base we be able to use templates named Content-culture-enus.cshtml and content-culture-en.cshtml, similarly you will have Parts\_Common\_Body-culture-enus.cshtml and Parts\_Common\_Body-culture-en.cshtml, etc.

+ Last addition: the multiCultural homepage. It will detect the real '~/' and the translated ('~/fr-FR/' as example) homepages and generate alternates named with format [ShapeType]\_\_CultureHomepage\_\_[Culture], with the same previous base we will be able to use templates named layout-cultureHomepage-enus.cshtml, layout-cultureHomepage-en.cshtml, layout-cultureHomepage-us.cshtml or layout-cultureHomepage.cshtml which will catch the home page layout for all the cultures.

Please note that the culture short names are transformed to remove '-' so en-US became enus.

As an example if you use a widget of type ContactWidget and have 3 cultures declared en-US, fr-FR, es-US, you can create for each of them, placing the files in the view folder of your theme:

+ widget-enus-contactwidget.cshtml

+ widget-frfr-contactwidget.cshtml

+ widget-esus-contactwidget.cshtml


or you can use a global widget for USA  country

+ widget-us-contactwidget.cshtml

or you can use a global widget for spanish

+ widget-es-contactwidget.cshtml


This version is fully implemented and I tested on several kind of sites.
Beware if you got the previous version, it has run through many changes and cleaning actions. So before to installing it, remove any previous version.
I will appreciate yours questions & comments, do not hesitate to write them here in the issue section and not in the orchard's Codeplex discussions or issues.

To integrate it into your site, you will have to duplicate the front-end cshtml files and enable alternates in your theme folder, the one provided here are just to take as examples (the MenuItemLink-CulturepickerLink.chtml is written for bootstrap and may be you don't use it … but it runs in base Orchard).
Don’t forget to enable all the features you need.

With all these ideas the module is quite efficient and customizable, do not hesitate to send me your feed-back.

Removing the controller made it more 'elegant', certainly faster and safe.

Thanks for inspirations to the authors of Orchard.CulturePicker which I used long time before needing some improvements, and RM.Localization which contains very interesting ideas.

CS
DATWENDO