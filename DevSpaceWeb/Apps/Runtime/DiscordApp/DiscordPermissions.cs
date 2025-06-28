using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public static class DiscordPermissions
{
    public static GuildPermission? DoesNotHave(GuildPermissions user, GuildPermissions check)
    {
        if (user.Administrator)
            return null;

        if (check.AddReactions && !user.AddReactions)
            return GuildPermission.AddReactions;

        if (check.Administrator)
            return GuildPermission.Administrator;

        if (check.AttachFiles && !user.AttachFiles)
            return GuildPermission.AttachFiles;

        if (check.BanMembers && !user.BanMembers)
            return GuildPermission.BanMembers;

        if (check.ChangeNickname && !user.ChangeNickname)
            return GuildPermission.ChangeNickname;

        if (check.Connect && !user.Connect)
            return GuildPermission.Connect;

        if (check.CreateEvents && !user.CreateEvents && !user.ManageEvents)
            return GuildPermission.CreateEvents;

        if (check.CreateGuildExpressions && !user.CreateGuildExpressions && !user.ManageEmojisAndStickers)
            return GuildPermission.CreateGuildExpressions;

        if (check.CreateInstantInvite && !user.CreateInstantInvite)
            return GuildPermission.CreateInstantInvite;

        if (check.CreatePrivateThreads && !user.CreatePrivateThreads)
            return GuildPermission.CreatePrivateThreads;

        if (check.CreatePublicThreads && !user.CreatePublicThreads)
            return GuildPermission.CreatePublicThreads;

        if (check.DeafenMembers && !user.DeafenMembers)
            return GuildPermission.DeafenMembers;

        if (check.EmbedLinks && !user.EmbedLinks)
            return GuildPermission.EmbedLinks;

        if (check.KickMembers && !user.KickMembers)
            return GuildPermission.KickMembers;

        if (check.ManageChannels && !user.ManageChannels)
            return GuildPermission.ManageChannels;

        if (check.ManageEmojisAndStickers && !user.ManageEmojisAndStickers)
            return GuildPermission.ManageEmojisAndStickers;

        if (check.ManageEvents && !user.ManageEvents)
            return GuildPermission.ManageEvents;

        if (check.ManageGuild && !user.ManageGuild)
            return GuildPermission.ManageGuild;

        if (check.ManageMessages && !user.ManageMessages)
            return GuildPermission.ManageMessages;

        if (check.ManageNicknames && !user.ManageNicknames)
            return GuildPermission.ManageNicknames;

        if (check.ManageRoles && !user.ManageRoles)
            return GuildPermission.ManageRoles;

        if (check.ManageThreads && !user.ManageThreads)
            return GuildPermission.ManageThreads;

        if (check.ManageWebhooks && !user.ManageWebhooks)
            return GuildPermission.ManageWebhooks;

        if (check.MentionEveryone && !user.MentionEveryone)
            return GuildPermission.MentionEveryone;

        if (check.ModerateMembers && !user.ModerateMembers)
            return GuildPermission.ModerateMembers;

        if (check.MoveMembers && !user.MoveMembers)
            return GuildPermission.MoveMembers;

        if (check.MuteMembers && !user.MuteMembers)
            return GuildPermission.MuteMembers;

        if (check.PrioritySpeaker && !user.PrioritySpeaker)
            return GuildPermission.PrioritySpeaker;

        if (check.ReadMessageHistory && !user.ReadMessageHistory)
            return GuildPermission.ReadMessageHistory;

        if (check.RequestToSpeak && !user.RequestToSpeak)
            return GuildPermission.RequestToSpeak;

        if (check.SendMessages && !user.SendMessages)
            return GuildPermission.SendMessages;

        if (check.SendMessagesInThreads && !user.SendMessagesInThreads)
            return GuildPermission.SendMessagesInThreads;

        if (check.SendTTSMessages && !user.SendTTSMessages)
            return GuildPermission.SendTTSMessages;

        if (check.SendVoiceMessages && !user.SendVoiceMessages)
            return GuildPermission.SendVoiceMessages;

        if (check.SetVoiceChannelStatus && !user.SetVoiceChannelStatus)
            return GuildPermission.SetVoiceChannelStatus;

        if (check.Speak && !user.Speak)
            return GuildPermission.Speak;

        if (check.StartEmbeddedActivities && !user.StartEmbeddedActivities)
            return GuildPermission.StartEmbeddedActivities;

        if (check.Stream && !user.Stream)
            return GuildPermission.Stream;

        if (check.UseApplicationCommands && !user.UseApplicationCommands)
            return GuildPermission.UseApplicationCommands;

        if (check.UseExternalEmojis && !user.UseExternalEmojis)
            return GuildPermission.UseExternalEmojis;

        if (check.UseExternalSounds && !user.UseExternalSounds)
            return GuildPermission.UseExternalSounds;

        if (check.UseExternalStickers && !user.UseExternalStickers)
            return GuildPermission.UseExternalStickers;

        if (check.UseSoundboard && !user.UseSoundboard)
            return GuildPermission.UseSoundboard;

        if (check.UseVAD && !user.UseVAD)
            return GuildPermission.UseVAD;

        if (check.ViewAuditLog && !user.ViewAuditLog)
            return GuildPermission.ViewAuditLog;

        if (check.ViewChannel && !user.ViewChannel)
            return GuildPermission.ViewChannel;

        if (check.ViewGuildInsights && !user.ViewGuildInsights)
            return GuildPermission.ViewGuildInsights;

        if (check.ViewMonetizationAnalytics && !user.ViewMonetizationAnalytics)
            return GuildPermission.ViewMonetizationAnalytics;

        return null;
    }


    public static ChannelPermission? DoesNotHave(GuildPermissions suser, ChannelPermissions user, ChannelPermissions check)
    {
        if (suser.Administrator)
            return null;

        if (check.AddReactions && !user.AddReactions)
            return ChannelPermission.AddReactions;

        if (check.AttachFiles && !user.AttachFiles)
            return ChannelPermission.AttachFiles;

        if (check.Connect && !user.Connect)
            return ChannelPermission.Connect;

        if (check.CreateEvents && !user.CreateEvents)
            return ChannelPermission.CreateEvents;

        if (check.CreateInstantInvite && !user.CreateInstantInvite)
            return ChannelPermission.CreateInstantInvite;

        if (check.CreatePrivateThreads && !user.CreatePrivateThreads)
            return ChannelPermission.CreatePrivateThreads;

        if (check.CreatePublicThreads && !user.CreatePublicThreads)
            return ChannelPermission.CreatePublicThreads;

        if (check.DeafenMembers && !user.DeafenMembers)
            return ChannelPermission.DeafenMembers;

        if (check.EmbedLinks && !user.EmbedLinks)
            return ChannelPermission.EmbedLinks;

        if (check.ManageChannel && !user.ManageChannel)
            return ChannelPermission.ManageChannels;

        if (check.ManageMessages && !user.ManageMessages)
            return ChannelPermission.ManageMessages;

        if (check.ManageRoles && !user.ManageRoles)
            return ChannelPermission.ManageRoles;

        if (check.ManageThreads && !user.ManageThreads)
            return ChannelPermission.ManageThreads;

        if (check.ManageWebhooks && !user.ManageWebhooks)
            return ChannelPermission.ManageWebhooks;

        if (check.MentionEveryone && !user.MentionEveryone)
            return ChannelPermission.MentionEveryone;

        if (check.MoveMembers && !user.MoveMembers)
            return ChannelPermission.MoveMembers;

        if (check.MuteMembers && !user.MuteMembers)
            return ChannelPermission.MuteMembers;

        if (check.PrioritySpeaker && !user.PrioritySpeaker)
            return ChannelPermission.PrioritySpeaker;

        if (check.ReadMessageHistory && !user.ReadMessageHistory)
            return ChannelPermission.ReadMessageHistory;

        if (check.RequestToSpeak && !user.RequestToSpeak)
            return ChannelPermission.RequestToSpeak;

        if (check.SendMessages && !user.SendMessages)
            return ChannelPermission.SendMessages;

        if (check.SendMessagesInThreads && !user.SendMessagesInThreads)
            return ChannelPermission.SendMessagesInThreads;

        if (check.SendTTSMessages && !user.SendTTSMessages)
            return ChannelPermission.SendTTSMessages;

        if (check.SendVoiceMessages && !user.SendVoiceMessages)
            return ChannelPermission.SendVoiceMessages;

        if (check.SetVoiceChannelStatus && !user.SetVoiceChannelStatus)
            return ChannelPermission.SetVoiceChannelStatus;

        if (check.Speak && !user.Speak)
            return ChannelPermission.Speak;

        if (check.StartEmbeddedActivities && !user.StartEmbeddedActivities)
            return ChannelPermission.StartEmbeddedActivities;

        if (check.Stream && !user.Stream)
            return ChannelPermission.Stream;

        if (check.UseApplicationCommands && !user.UseApplicationCommands)
            return ChannelPermission.UseApplicationCommands;

        if (check.UseExternalEmojis && !user.UseExternalEmojis)
            return ChannelPermission.UseExternalEmojis;

        if (check.UseExternalSounds && !user.UseExternalSounds)
            return ChannelPermission.UseExternalSounds;

        if (check.UseExternalStickers && !user.UseExternalStickers)
            return ChannelPermission.UseExternalStickers;

        if (check.UseSoundboard && !user.UseSoundboard)
            return ChannelPermission.UseSoundboard;

        if (check.UseVAD && !user.UseVAD)
            return ChannelPermission.UseVAD;

        if (check.ViewChannel && !user.ViewChannel)
            return ChannelPermission.ViewChannel;

        return null;
    }
}
