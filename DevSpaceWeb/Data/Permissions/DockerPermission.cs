namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum DockerPermission : ulong
{
    DockerController = 1L << 0,
    DockerManager = 1L << 1,
    DockerAdministrator = 1L << 2,
    ViewStacksAndContainers = 1L << 3,
    ControlStacksAndContainers = 1L << 4,
    ManageStacksAndContainers = 1L << 5,
    ViewImages = 1L << 6,
    ViewNetworks = 1L << 7,
    ViewVolumes = 1L << 8,
    ViewTemplates = 1L << 9,
    ViewCustomTemplates = 1L << 10,
    ManageCustomTemplates = 1L << 11,
    ViewPlugins = 1L << 12,
    ManagePlugins = 1L << 13,
    ContainerLogs = 1L << 14,
    ContainerInspect = 1L << 15,
    ContainerStats = 1L << 16,
    ContainerConsole = 1L << 17,
    ManageImages = 1L << 18,
    ManageNetworks = 1L << 19,
    ManageVolumes = 1L << 20,
    ViewEvents = 1L << 21,
    ViewRegistries = 1L << 22,
    ManageRegistries = 1L << 23,
    ManageSettings = 1L << 24,
}
