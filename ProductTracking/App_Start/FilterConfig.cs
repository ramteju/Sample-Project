using Entities;
using ProductTracking.Logging;
using System.Web;
using System.Web.Mvc;

namespace ProductTracking
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            try
            {
                filters.Add(new HandleErrorAttribute());
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
