using Docker.DotNet.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace DevSpaceWeb;

public static class ExtensionMethods
{
    public static string ToEnabledString(this bool value)
    {
        return value ? "Enabled" : "Disabled";
    }

    public static List<KeyValuePair<string, string?>> ParseEnvironment(this IList<string> variables)
    {
        List<KeyValuePair<string, string?>> List = [];
        foreach (string i in variables)
        {
            string[] Split = i.Split('=');
            if (Split.Length == 1)
                List.Add(new KeyValuePair<string, string?>(Split[0], null));
            else
                List.Add(new KeyValuePair<string, string?>(Split[0], Split[1]));
        }
        return List;
    }

    public static string? GetExitMessage(this ContainerInspectResponse container)
    {
        if (container == null || container.State.Running)
            return null;
        if (container.State.OOMKilled)
            return "Out of Memory";

        if (container.State.Paused)
            return "Container Paused";

        switch (container.State.ExitCode)
        {
            case 0:
                return "Process Finished";
            case 125:
                return "Container Error" + (!string.IsNullOrEmpty(container.State.Error) ? $": {container.State.Error}" : null);
            case 126:
            case 127:
                return "Command Error" + (!string.IsNullOrEmpty(container.State.Error) ? $": {container.State.Error}" : null);
            case 134:
                return "Terminated" + (!string.IsNullOrEmpty(container.State.Error) ? $": {container.State.Error}" : null);
            case 137:
            case 143:
                return "Container Stopped";
            case 139:
                return "Segmentation Fault" + (!string.IsNullOrEmpty(container.State.Error) ? $": {container.State.Error}" : null);
            case 1:
                return "Application Error" + (!string.IsNullOrEmpty(container.State.Error) ? $": {container.State.Error}" : null);
        }

        if (string.IsNullOrEmpty(container.State.Error))
            return $"Code {container.State.ExitCode}";

        return $"Code {container.State.ExitCode}: {container.State.Error}";
    }
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
