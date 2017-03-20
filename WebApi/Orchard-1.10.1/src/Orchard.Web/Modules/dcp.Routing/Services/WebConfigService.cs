using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using dcp.Routing.Models;
using Orchard;
using Orchard.Environment.Extensions;

namespace dcp.Routing.Services
{
    public interface IWebConfigService : IDependency
    {
        bool AddRedirectRules(IEnumerable<RedirectRule> redirects, string filePath);
    }

    [OrchardFeature("dcp.Routing.Redirects")]
    public class WebConfigService : IWebConfigService
    {
        public bool AddRedirectRules(IEnumerable<RedirectRule> redirects, string filePath)
        {
            var config = XDocument.Load(filePath);
            var webServerX = config.Descendants("system.webServer").FirstOrDefault();
            if (webServerX == null)
                return false;

            var rewriteX = webServerX.Descendants("rewrite").FirstOrDefault();
            if (rewriteX == null)
                return false;

            var rulesX = rewriteX.Descendants("rules").FirstOrDefault();
            if (rulesX == null)
                return false;

            foreach (var redirect in redirects)
            {
                rulesX.Add(new XElement("rule",
                        new XAttribute("name", "RedirectRule" + redirect.Id),
                        new XAttribute("enabled", "true"),
                        new XAttribute("stopProcessing", "true"),
                        new XElement("match",
                            new XAttribute("url", "^" + redirect.SourceUrl)),
                        new XElement("action",
                            new XAttribute("type", "Redirect"),
                            new XAttribute("url", redirect.DestinationUrl),
                            new XAttribute("appendQueryString", "true"),
                            new XAttribute("redirectType", redirect.IsPermanent ? "Permanent" : "Found")
                        )
                    )
                );
            }

            config.Save(filePath);
            return true;
        }
    }
}