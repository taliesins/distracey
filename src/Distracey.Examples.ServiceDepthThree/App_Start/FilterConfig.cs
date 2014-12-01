using System.Web.Mvc;

namespace Distracey.Examples.ServiceDepthThree
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}