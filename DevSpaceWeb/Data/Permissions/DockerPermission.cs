namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum DockerPermission : ulong
{
    // ALl docker permissions including system access.
    DockerAdministrator = 1L << 0,
    // View container volume info
    ViewVolumes = 1L << 1,
    // View container network info
    ViewNetworks = 1L << 2,
    // View container images
    ViewImages = 1L << 3,
    // View host docker registries
    ViewRegistries = 1L << 4,
    // Lets you view permissions for containers
    ViewPermissions = 1L << 5,
    // Lets you manage permissions for the container
    ManagePermissions = 1L << 6,
    // Remote access containers with your permissions with the dev space api
    UseAPIs = 1L << 7,
    UseAppTemplates = 1L << 8,
    UseCustomTemplates = 1L << 9,
    ManageCustomTemplates = 1L << 10,
    ViewPlugins = 1L << 11,
    ManageVolumes = 1L << 12,
    ManagePlugins = 1L << 13,
    ManageNetworks = 1L << 14,
    ManageImages = 1L << 15,
    ManageRegistries = 1L << 16
}

[Flags]
public enum DockerContainerPermission : ulong
{
    // View all containers, stacks 
    ViewContainers = 1L << 0,

    // Start/stop/restart/kill/pause containers
    ControlContainers = 1L << 1,

    // View container logs
    ViewContainerLogs = 1L << 2,

    // View sensitive container environment variables.
    ViewContainerEnvironment = 1L << 3,

    // View the container files 
    ViewContainerFiles = 1L << 4,

    // Lets you create and modify files in the container
    ManageContainerFiles = 1L << 5,

    // Execute commands inside the container.

    UseContainerConsole = 1L << 6,

    ViewContainerChanges = 1L << 7,

    ManageContainers = 1L << 8,

    CreateContainers = 1L << 9,

    ViewContainerStats = 1L << 10,

    ViewContainerDetails = 1L << 11,

    ViewContainerVolumes = 1L << 12,

    ViewContainerNetworks = 1L << 13,

    ViewContainerHealthLogs = 1L << 14,

    ViewStacks = 1L << 15,

    ControlStacks = 1L << 16,

    CreateStacks = 1L << 17,

    ManageStacks = 1L << 18,
}
