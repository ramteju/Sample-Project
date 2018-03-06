using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using Entities;
using ProductTracking.Logging;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ProductTracking.App_Start.UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(ProductTracking.App_Start.UnityWebActivator), "Shutdown")]

namespace ProductTracking.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() 
        {
            try
            {
                var container = UnityConfig.GetConfiguredContainer();

                FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
                FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

                DependencyResolver.SetResolver(new UnityDependencyResolver(container));

                // TODO: Uncomment if you want to use PerRequestLifetimeManager
                // Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
               // throw;
            }
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}