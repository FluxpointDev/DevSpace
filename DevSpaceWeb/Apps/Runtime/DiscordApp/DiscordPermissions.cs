using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public static class DiscordPermissions
{
    public static bool HasPermission(GuildPermissions check, GuildPermission perm)
    {
        return check.Has(perm);
    }
    public static bool HasPermission(GuildPermission check, GuildPermissions perm)
    {
        return perm.Has(check);
    }
    public static bool HasPermission(GuildPermissions check, GuildPermissions user)
    {
        if (user.Administrator)
            return true;

        if (check.AddReactions && user.AddReactions)
            return true;

        if (check.AttachFiles && user.AttachFiles)
            return true;

        if (check.BanMembers && user.BanMembers)
            return true;

        if (check.ChangeNickname && user.ChangeNickname)
            return true;

        if (check.Connect && user.Connect)
            return true;

        if (check.CreateEvents && user.CreateEvents)
            return true;

        if (check.CreateGuildExpressions && user.CreateGuildExpressions)
            return true;

        if (check.CreateInstantInvite && user.CreateInstantInvite)
            return true;

        if (check.CreatePrivateThreads && user.CreatePrivateThreads)
            return true;

        if (check.CreatePublicThreads && user.CreatePublicThreads)
            return true;

        if (check.DeafenMembers && user.DeafenMembers)
            return true;

        if (check.EmbedLinks && user.EmbedLinks)
            return true;

        if (check.KickMembers && user.KickMembers)
            return true;

        if (check.ManageChannels && user.ManageChannels)
            return true;

        if (check.ManageEmojisAndStickers && user.ManageEmojisAndStickers)
            return true;

        if (check.ManageEvents && user.ManageEvents)
            return true;

        if (check.ManageGuild && user.ManageGuild)
            return true;

        if (check.ManageMessages && user.ManageMessages)
            return true;

        if (check.ManageNicknames && user.ManageNicknames)
            return true;

        if (check.ManageRoles && user.ManageRoles)
            return true;

        if (check.ManageThreads && user.ManageThreads)
            return true;

        if (check.ManageWebhooks && user.ManageWebhooks)
            return true;

        if (check.MentionEveryone && user.MentionEveryone)
            return true;

        if (check.ModerateMembers && user.ModerateMembers)
            return true;

        if (check.MoveMembers && user.MoveMembers)
            return true;

        if (check.MuteMembers && user.MuteMembers)
            return true;

        if (check.PrioritySpeaker && user.PrioritySpeaker)
            return true;

        if (check.ReadMessageHistory && user.ReadMessageHistory)
            return true;

        if (check.RequestToSpeak && user.RequestToSpeak)
            return true;

        if (check.SendMessages && user.SendMessages)
            return true;

        if (check.SendMessagesInThreads && user.SendMessagesInThreads)
            return true;

        if (check.SendTTSMessages && user.SendTTSMessages)
            return true;

        if (check.SendVoiceMessages && user.SendVoiceMessages)
            return true;

        if (check.SetVoiceChannelStatus && user.SetVoiceChannelStatus)
            return true;

        if (check.Speak && user.Speak)
            return true;

        if (check.StartEmbeddedActivities && user.StartEmbeddedActivities)
            return true;

        if (check.Stream && user.Stream)
            return true;

        if (check.UseApplicationCommands && user.UseApplicationCommands)
            return true;

        if (check.UseExternalEmojis && user.UseExternalEmojis)
            return true;

        if (check.UseExternalSounds && user.UseExternalSounds)
            return true;

        if (check.UseExternalStickers && user.UseExternalStickers)
            return true;

        if (check.UseSoundboard && user.UseSoundboard)
            return true;

        if (check.UseVAD && user.UseVAD)
            return true;

        if (check.ViewAuditLog && user.ViewAuditLog)
            return true;

        if (check.ViewChannel && user.ViewChannel)
            return true;

        if (check.ViewGuildInsights && user.ViewGuildInsights)
            return true;

        if (check.ViewMonetizationAnalytics && user.ViewMonetizationAnalytics)
            return true;

        return false;
    }

    public static bool HasPermission(ChannelPermission check, ChannelPermissions perm)
    {
        return perm.Has(check);
    }
    public static bool HasPermission(ChannelPermissions check, ChannelPermissions user)
    {
        if (check.AddReactions && user.AddReactions)
            return true;

        if (check.AttachFiles && user.AttachFiles)
            return true;

        if (check.Connect && user.Connect)
            return true;

        if (check.CreateEvents && user.CreateEvents)
            return true;

        if (check.CreateInstantInvite && user.CreateInstantInvite)
            return true;

        if (check.CreatePrivateThreads && user.CreatePrivateThreads)
            return true;

        if (check.CreatePublicThreads && user.CreatePublicThreads)
            return true;

        if (check.DeafenMembers && user.DeafenMembers)
            return true;

        if (check.EmbedLinks && user.EmbedLinks)
            return true;

        if (check.ManageChannel && user.ManageChannel)
            return true;

        if (check.ManageMessages && user.ManageMessages)
            return true;

        if (check.ManageRoles && user.ManageRoles)
            return true;

        if (check.ManageThreads && user.ManageThreads)
            return true;

        if (check.ManageWebhooks && user.ManageWebhooks)
            return true;

        if (check.MentionEveryone && user.MentionEveryone)
            return true;

        if (check.MoveMembers && user.MoveMembers)
            return true;

        if (check.MuteMembers && user.MuteMembers)
            return true;

        if (check.PrioritySpeaker && user.PrioritySpeaker)
            return true;

        if (check.ReadMessageHistory && user.ReadMessageHistory)
            return true;

        if (check.RequestToSpeak && user.RequestToSpeak)
            return true;

        if (check.SendMessages && user.SendMessages)
            return true;

        if (check.SendMessagesInThreads && user.SendMessagesInThreads)
            return true;

        if (check.SendTTSMessages && user.SendTTSMessages)
            return true;

        if (check.SendVoiceMessages && user.SendVoiceMessages)
            return true;

        if (check.SetVoiceChannelStatus && user.SetVoiceChannelStatus)
            return true;

        if (check.Speak && user.Speak)
            return true;

        if (check.StartEmbeddedActivities && user.StartEmbeddedActivities)
            return true;

        if (check.Stream && user.Stream)
            return true;

        if (check.UseApplicationCommands && user.UseApplicationCommands)
            return true;

        if (check.UseExternalEmojis && user.UseExternalEmojis)
            return true;

        if (check.UseExternalSounds && user.UseExternalSounds)
            return true;

        if (check.UseExternalStickers && user.UseExternalStickers)
            return true;

        if (check.UseSoundboard && user.UseSoundboard)
            return true;

        if (check.UseVAD && user.UseVAD)
            return true;

        if (check.ViewChannel && user.ViewChannel)
            return true;

        return false;
    }
}
