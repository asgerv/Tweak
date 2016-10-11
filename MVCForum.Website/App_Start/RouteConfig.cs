using System.Web.Mvc;
using System.Web.Routing;
using MVCForum.Domain.Constants;

namespace MVCForum.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.LowercaseUrls = true;
            RouteTable.Routes.AppendTrailingSlash = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            // API Attribute Routes
            //routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "categoryUrls", // Route name
                string.Concat(SiteConstants.Instance.CategoryUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Category", action = "Show", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "categoryRssUrls", // Route name
                string.Concat(SiteConstants.Instance.CategoryUrlIdentifier, "/rss/{slug}"), // URL with parameters
                new { controller = "Category", action = "CategoryRss", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "topicUrls", // Route name
                string.Concat(SiteConstants.Instance.TopicUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Topic", action = "Show", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "memberUrls", // Route name
                string.Concat(SiteConstants.Instance.MemberUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Members", action = "GetByName", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "tagUrls", // Route name
                string.Concat(SiteConstants.Instance.TagsUrlIdentifier, "/{tag}"), // URL with parameters
                new { controller = "Topic", action = "TopicsByTag", tag = UrlParameter.Optional } // Parameter defaults
            );

            // Custom

            routes.MapRoute(
                "articleUrls", // Route name
                "artikel/{slug}", // URL with parameters
                new { controller = "Article", action = "Show", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "rssUrl", // Route name
                "rss", // URL with parameters
                new { controller = "Article", action = "LatestRss" } // Parameter defaults
            );
            routes.MapRoute(
                "aboutUrl", // Route name
                "om", // URL with parameters
                new { controller = "Home", action = "About" } // Parameter defaults
            );
            routes.MapRoute(
                "teamUrl", // Route name
                "team", // URL with parameters
                new { controller = "Home", action = "Team" } // Parameter defaults
            );
            routes.MapRoute(
                "contactUrl", // Route name
                "kontakt", // URL with parameters
                new { controller = "Home", action = "Contact" } // Parameter defaults
            );
            routes.MapRoute(
                "searchUrl", // Route name
                "søg", // URL with parameters
                new { controller = "Article", action = "Search" } // Parameter defaults
            );

            routes.MapRoute(
                "articleTagUrls", // Route name
                "tag/{tag}", // URL with parameters
                new { controller = "ArticleTag", action = "Index", tag = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "articleNyhedUrl", // Route name
                "artikler/sektion/nyheder", // URL with parameters
                new { controller = "Article", action = "Nyheder" } // Parameter defaults
            );
            routes.MapRoute(
                "articleVideoUrl", // Route name
                "artikler/sektion/video", // URL with parameters
                new { controller = "Article", action = "Video" } // Parameter defaults
            );
            routes.MapRoute(
                "articleTestUrl", // Route name
                "artikler/sektion/test", // URL with parameters
                new { controller = "Article", action = "Test" } // Parameter defaults
            );
            routes.MapRoute(
               "articleCategoryUrl", // Route name
               "artikler/{section}/{slug}", // URL with parameters
               new { controller = "Article", action = "Category", slug = UrlParameter.Optional } // Parameter defaults
           );

            // End på custom

            routes.MapRoute(
                "topicXmlSitemap", // Route name
                "topicxmlsitemap", // URL with parameters
                new { controller = "Home", action = "GoogleSitemap" } // Parameter defaults
            );

            routes.MapRoute(
                "categoryXmlSitemap", // Route name
                "categoryxmlsitemap", // URL with parameters
                new { controller = "Home", action = "GoogleCategorySitemap" } // Parameter defaults
            );

            routes.MapRoute(
                "memberXmlSitemap", // Route name
                "memberxmlsitemap", // URL with parameters
                new { controller = "Home", action = "GoogleMemberSitemap" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            
            //.RouteHandler = new SlugRouteHandler()
        }
    }
}
