using Entities;
using ProductTracking.Controllers;
using ProductTracking.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace ProductTracking
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            try
            {
                // Web API configuration and services

                // Web API routes
                config.MapHttpAttributeRoutes();

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
                config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

                config.MessageHandlers.Add(new LogHandler());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
