namespace DevSpaceWeb.Data.Servers;

public static class Capabilities
{
    public static Dictionary<string, string> List = new Dictionary<string, string>
    {
        { "CAP_AUDIT_CONTROL", "Enable and disable kernel auditing; change auditing filter rules; retrieve auditing status and filtering rules." },
        { "CAP_AUDIT_READ", "Allow reading the audit log via a multicast netlink socket." },
        { "CAP_AUDIT_WRITE", "Write records to kernel auditing log." },
        { "CAP_BLOCK_SUSPEND", "Employ features that can block system suspend." },
        { "CAP_BPF", "Employ privileged BPF operations." },
        { "CAP_CHECKPOINT_RESTORE", "Employ the set_tid feature and read symbolic links in proc for other processes." },
        { "CAP_CHOWN", "Make arbitrary changes to file UIDs and GIDs (Ownership and Permissions)" },
        { "CAP_DAC_OVERRIDE", " Bypass file read, write, and execute permission checks." },
        { "CAP_DAC_READ_SEARCH", "Bypass file read permission checks and directory read and execute permission checks." },
        { "CAP_FOWNER", "Bypass permission checks on operations that normally require the filesystem UID of the process to match the UID of the file." },
        { "CAP_FSETID", "Set the set-group-ID bit for a file whose GID does not match the filesystem or any of the supplementary GIDs of the calling process." },
        { "CAP_IPC_LOCK", "Lock and allocate memory." },
        { "CAP_IPC_OWNER", "Bypass permission checks for operations on System V IPC objects." },
        { "CAP_KILL", "Bypass permission checks for sending signals." },
        { "CAP_LEASE", "Establish leases on arbitrary files." },
        { "CAP_LINUX_IMMUTABLE", "Set the FS_APPEND_FL and FS_IMMUTABLE_FL inode flags." },
        { "CAP_MAC_ADMIN", "Allow MAC configuration or state changes. Implemented for the Smack Linux Security Module (LSM)" },
        { "CAP_MAC_OVERRIDE", "Override Mandatory Access Control (MAC). Implemented for the Smack Linux Security Module (LSM)" },
        { "CAP_MKNOD", "Create special files." },
        { "CAP_NET_ADMIN", "Perform various network-related operations: Interface, Firewall, IPTables, Routing, Driver and Binding." },
        { "CAP_NET_BIND_SERVICE", "Bind a socket to Internet domain privileged ports (port numbers less than 1024" },
        { "CAP_NET_BROADCAST", "Make socket broadcasts, and listen to multicasts." },
        { "CAP_NET_RAW", "Use RAW and PACKET sockets." },
        { "CAP_PERFMON", "Employ various performance-monitoring mechanisms." },
        { "CAP_SETGID", "Make arbitrary manipulations of process GIDs and namespaces." },
        { "CAP_SETFCAP", "Set arbitrary capabilities on a file." },
        { "CAP_SETPCAP", "Add any capability from the calling thread's bounding set to its inheritable set; drop capabilities from the bounding set, make changes to the securebits flags" },
        { "CAP_SETUID", "Make arbitrary manipulations of process UIDs and namespaces." },
        { "CAP_SYS_ADMIN", "Grant a lot of system privilages." },
        { "CAP_SYS_BOOT", "Access to reboot." },
        { "CAP_SYS_CHROOT", "Change root directory and change mount namespaces." },
        { "CAP_SYS_MODULE", "Load and unload kernel modules." },
        { "CAP_SYS_NICE", "Set CPU affinity, IO scheduling and process priority." },
        { "CAP_SYS_PACCT", "Toggle process accounting (Crash logging)" },
        { "CAP_SYS_PTRACE", "Trace processes: inspect or process memory access." },
        { "CAP_SYS_RAWIO", "Perform various IO operations." },
        { "CAP_SYS_RESOURCE", "Resource controlling: disk usage, quota, limits, overrides and OOM." },
        { "CAP_SYS_TIME", "Set system time." },
        { "CAP_SYS_TTY_CONFIG", "Employ various privileged operations on virtual terminals." },
        { "CAP_SYSLOG", "Perform privileged system logging and kernal addresses." },
        { "CAP_WAKE_ALARM", "Trigger something that will wake up the system." }
    };
}
