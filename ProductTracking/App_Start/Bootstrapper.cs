using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using System.Web.Mvc;
using System.Web.Http;
using ProductTracking.Controllers.Auth;

namespace ProductTracking.App_Start
{
    public class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            try
            {
                var container = BuildUnityContainer();
                DependencyResolver.SetResolver(new UnityDependencyResolver(container));
                // register dependency resolver for WebAPI
                GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
                return container;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static IUnityContainer BuildUnityContainer()
        {
            try
            {
                var container = new UnityContainer();

                container.RegisterType<AccountController>(new InjectionConstructor());

                //service classes


                RegisterTypes(container);
                return container;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static void RegisterTypes(IUnityContainer container)
        {

        }
    }
}