using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace DevSpaceWeb
{
    public static class ExtensionMethods
    {
        public static NameValueCollection ParseQuery(this NavigationManager nav)
        {
            return System.Web.HttpUtility.ParseQueryString(new Uri(nav.Uri).Query);
        }
    }
}
