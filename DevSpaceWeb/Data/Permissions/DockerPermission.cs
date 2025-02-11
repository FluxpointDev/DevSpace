namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum DockerPermission : ulong
{
    // ALl docker permissions including system access.
    DockerAdministrator = 1L << 0,
    // View all containers, stacks 
    ViewContainers = 1L << 1,
    // Start/stop/restart/kill/pause containers
    ControlContainers = 1L << 2,
    // View container logs
    ViewContainerLogs = 1L << 3,
    // View sensitive container environment variables.
    ViewContainerEnvironment = 1L << 4,
    // View the container files 
    ViewContainerFiles = 1L << 5,
    // Lets you create and modify files in the container
    ManageContainerFiles = 1L << 6,
    // View container volume info
    ViewVolumes = 1L << 7,
    // View container network info
    ViewNetworks = 1L << 8,
    // View container images
    ViewImages = 1L << 9,
    // View host docker registries
    ViewRegistries = 1L << 10,
    // Lets you view permissions for containers
    ViewPermissions = 1L << 12,
    // Lets you manage permissions for the container
    ManagePermissions = 1L << 13,
    // Remote access containers with your permissions with the dev space api
    UseAPIs = 1L << 14,
    // View potentially sensitive open ports for the containers
    ViewContainerPorts = 1L << 15,
    // Execute commands inside the container.
    UseContainerConsole = 1L << 16,
}
