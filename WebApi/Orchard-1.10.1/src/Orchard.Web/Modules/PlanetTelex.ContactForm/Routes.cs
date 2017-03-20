﻿using System.Collections.Generic;
using Orchard.Mvc.Routes;
using System.Web.Routing;
using System.Web.Mvc;

namespace PlanetTelex.ContactForm
{
    public class Routes : IRouteProvider
    {
        #region Implementation of IRouteProvider

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            yield return new RouteDescriptor
            {
                Priority = 15,
                Route = new Route(
                    "contact-form/send-email",
                    new RouteValueDictionary 
                    { 
                        {"area", "PlanetTelex.ContactForm"},    
                        {"controller","contactform"},
                        {"action","sendcontactemail"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary
                    {
                        {"area", "PlanetTelex.ContactForm"}
                    },
                    new MvcRouteHandler())
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var descriptor in GetRoutes())
                routes.Add(descriptor);
        }

        #endregion
    }
}