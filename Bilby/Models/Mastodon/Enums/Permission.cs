using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon.Enums;

[Flags]
[JsonConverter(typeof(EnumAsFlagIndexConverter<Permission>))]
public enum Permission
{
    None = 0x0,

    Administrator = 0x1,
    Devops = 0x2,
    ViewAuditLog = 0x4,
    ViewDashboard = 0x8,
    ManageReports = 0x10,
    ManageFederation = 0x20,
    ManageSettings = 0x40,
    ManageBlocks = 0x80,
    ManageTaxonomies = 0x100,
    ManageAppeals = 0x200,
    ManageUsers = 0x400,
    ManageInvites = 0x800,
    ManageRules = 0x1000,
    ManageAnnouncements = 0x2000,
    ManageCustomEmojis = 0x4000,
    ManageWebhooks = 0x8000,
    InviteUsers = 0x1_0000,
    ManageRoles = 0x2_0000,
    ManageUserAccess = 0x4_0000,
    DeleteUserData = 0x8_0000,

    All = 0xF_FFFF
}
