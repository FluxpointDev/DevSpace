using Docker.DotNet.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace DevSpaceWeb;

public static class ExtensionMethods
{
    public static NameValueCollection ParseQuery(this NavigationManager nav)
    {
        return System.Web.HttpUtility.ParseQueryString(new Uri(nav.Uri).Query);
    }
    public static string GetName(this ContainerListResponse container)
    {
        return container.Names.First().Substring(1);
    }
    public static bool IsDead(this ContainerListResponse container)
    {
        if (container.State == "exited" || container.State == "dead")
            return true;
        return false;
    }

    public static string GetImageExtra(this ContainerListResponse container)
    {
        string[] split = container.Image.Split(':');
        return $"{split[0]} ({split[1]})";
    }
}
