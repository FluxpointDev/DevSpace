﻿namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum TeamPermission : ulong
{
    ViewMembers = 1L << 0,
    ManageMembers = 1L << 1,
    AssignRoles = 1L << 2,
    ManageRoles = 1L << 3,
    ViewPermissions = 1L << 4,
    ManagePermissions = 1L << 5,
    ManageTeam = 1L << 6,
    TeamAdministrator = 1L << 7,
    GlobalAdministrator = 1L << 8,
    UNUSED_ = 1L << 9,
    ViewAuditLogs = 1L << 10,
    UNUSED1_ = 1L << 11,
    UNUSED2_ = 1L << 12,
    UNUSED3_ = 1L << 13,
    ViewRoles = 1L << 14,
    UNUSED4_ = 1L << 15
}
