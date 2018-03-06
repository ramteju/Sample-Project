using ProductTracking.App_Start;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ProductTracking
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Bootstrapper.Initialise();

            var binder = new DateTimeModelBinder("dd-MM-yyyy");
            ModelBinders.Binders.Add(typeof(DateTime), binder);
            ModelBinders.Binders.Add(typeof(DateTime?), binder);
            InitData.init(); 
        }
    }

    public class DateTimeModelBinder : DefaultModelBinder
    {
        private string customFormat;

        public DateTimeModelBinder(string customFormat)
        {
            this.customFormat = customFormat;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (!String.IsNullOrEmpty(value.AttemptedValue))
                return DateTime.ParseExact(value.AttemptedValue, customFormat, CultureInfo.InvariantCulture);
            return null;
        }
    }
}
