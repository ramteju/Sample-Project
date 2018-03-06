using Entities;
using ProductTracking.Logging;
using System.Web.Mvc;

namespace ProductTracking.Util
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                base.OnActionExecuting(filterContext);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}