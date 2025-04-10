namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum DockerPermission : ulong
{
    /// <summary>
    /// All docker permissions including system access.
    /// </summary>
    DockerAdministrator = 1L << 0,

    /// <summary>
    /// View all docker volumes for the server.
    /// </summary>
    ViewVolumes = 1L << 1,

    /// <summary>
    /// Create or delete docker volumes on the server.
    /// </summary>
    ManageVolumes = 1L << 12,

    /// <summary>
    /// View all docker networks for the server.
    /// </summary>
    ViewNetworks = 1L << 2,

    /// <summary>
    /// Create, delete or modify docker networks on the server.
    /// </summary>
    ManageNetworks = 1L << 14,

    /// <summary>
    /// View all docker images for the server.
    /// </summary>
    ViewImages = 1L << 3,

    /// <summary>
    /// Create, import and manage docker images on the server.
    /// </summary>
    ManageImages = 1L << 15,

    /// <summary>
    /// View realtime docker events for all containers, images, networks, volumes and plugins.
    /// </summary>
    ViewEvents = 1L << 5,

    /// <summary>
    /// Lets you manage permissions for the container.
    /// </summary>
    UNUSED_1 = 1L << 6,

    /// <summary>
    /// Allows users to manage and control docker features from the API.
    /// </summary>
    UseAPIs = 1L << 7,

    /// <summary>
    /// View a list of premade docker templates to use for stacks and containers.
    /// </summary>
    UseAppTemplates = 1L << 8,

    /// <summary>
    /// View a list of custom docker templates made by other members.
    /// </summary>
    UseCustomTemplates = 1L << 9,

    /// <summary>
    /// Create, delete or modify custom docker templates for other team members to use.
    /// </summary>
    ManageCustomTemplates = 1L << 10,

    /// <summary>
    /// View all docker plugins for the server.
    /// </summary>
    ViewPlugins = 1L << 11,

    /// <summary>
    /// Install, remove and manage docker plugins.
    /// </summary>
    ManagePlugins = 1L << 13,

    /// <summary>
    /// View all docker registries for the server.
    /// </summary>
    ViewRegistries = 1L << 4,

    /// <summary>
    /// Create, delete or remove docker registries for the server.
    /// </summary>
    ManageRegistries = 1L << 16
}

[Flags]
public enum DockerContainerPermission : ulong
{
    /// <summary>
    /// View all containers in the server.
    /// </summary>
    ViewContainers = 1L << 0,

    /// <summary>
    /// Start/stop/restart/kill/pause containers
    /// </summary>
    ControlContainers = 1L << 1,

    /// <summary>
    /// View live container logs.
    /// </summary>
    ViewContainerLogs = 1L << 2,

    /// <summary>
    /// View sensitive container environment variables.
    /// </summary>
    ViewContainerEnvironment = 1L << 3,

    /// <summary>
    /// View files inside the containers.
    /// </summary>
    ViewContainerFiles = 1L << 4,

    /// <summary>
    /// Create and modify files inside the containers.
    /// </summary>
    ManageContainerFiles = 1L << 5,

    /// <summary>
    /// Execute shell commands in the container.
    /// </summary>
    UseContainerConsole = 1L << 6,

    /// <summary>
    /// View file system changes in the containers.
    /// </summary>
    ViewContainerChanges = 1L << 7,

    /// <summary>
    /// Delete and change container settings such as name and restart mode.
    /// </summary>
    ManageContainers = 1L << 8,

    /// <summary>
    /// Manually create containers for the server.
    /// </summary>
    CreateContainers = 1L << 9,

    /// <summary>
    /// View container stats for cpu, ram, io, network and processes.
    /// </summary>
    ViewContainerStats = 1L << 10,

    /// <summary>
    /// View extra container details including command, arguments, directory and ports.
    /// </summary>
    ViewContainerDetails = 1L << 11,

    /// <summary>
    /// View container volumes list.
    /// </summary>
    ViewContainerVolumes = 1L << 12,

    /// <summary>
    /// View container networks list.
    /// </summary>
    ViewContainerNetworks = 1L << 13,

    /// <summary>
    /// View container health log messages.
    /// </summary>
    ViewContainerHealthLogs = 1L << 14,

    /// <summary>
    /// View all stacks in the server.
    /// </summary>
    ViewStacks = 1L << 15,

    /// <summary>
    /// Start, stop, restart, pause and kill containers in a stack.
    /// </summary>
    ControlStacks = 1L << 16,

    /// <summary>
    /// Create stacks for the server.
    /// </summary>
    CreateStacks = 1L << 17,

    /// <summary>
    /// Delete or change stacks.
    /// </summary>
    ManageStacks = 1L << 18,

    /// <summary>
    /// Join or remove networks from containers.
    /// </summary>
    ManageContainerNetworks = 1L << 19
}
