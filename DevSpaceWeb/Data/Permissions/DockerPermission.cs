namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum DockerPermission : ulong
{
    DockerManager = 1L << 0,
    DockerAdministrator = 1L << 1,
    ViewContainers = 1L << 2,
    ControlContainers = 1L << 3,
    ManageContainers = 1L << 4,
    ViewImages = 1L << 5,
    ViewNetworks = 1L << 6,
    ViewVolumes = 1L << 7,
    ViewTemplates = 1L << 8,
    ViewCustomTemplates = 1L << 9,
    ManageCustomTemplates = 1L << 10,
    ViewPlugins = 1L << 11,
    ManagePlugins = 1L << 12,
    ContainerLogs = 1L << 13,
    ContainerInspect = 1L << 14,
    ContainerStats = 1L << 15,
    ContainerConsole = 1L << 16,
    ManageImages = 1L << 17,
    ManageNetworks = 1L << 18,
    ManageVolumes = 1L << 19,
    ViewEvents = 1L << 20,
    ViewRegistries = 1L << 21,
    ManageRegistries = 1L << 22,
    ManageSettings = 1L << 23,
}
