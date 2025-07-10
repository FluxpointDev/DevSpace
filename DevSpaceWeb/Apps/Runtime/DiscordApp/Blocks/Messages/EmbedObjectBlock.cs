using Discord;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class EmbedObjectBlock : DiscordBlock
{
    public async Task<EmbedBuilder> Embed()
    {
        EmbedBuilder embed = new EmbedBuilder();

        WorkspaceBlockConnection? block;
        if (Block.inputs.TryGetValue("title", out block) && block.block != null)
            embed.Title = await Runtime.GetStringFromBlock(block.block);

        if (Block.inputs.TryGetValue("author_name", out block) && block.block != null)
        {
            embed.Author = new EmbedAuthorBuilder
            {
                Name = await Runtime.GetStringFromBlock(block.block)
            };
        }

        if (Block.inputs.TryGetValue("author_icon", out block) && block.block != null)
        {
            if (embed.Author == null)
                embed.Author = new EmbedAuthorBuilder();
            embed.Author.IconUrl = await Runtime.GetStringFromBlock(block.block);
        }

        if (Block.inputs.TryGetValue("desc", out block) && block.block != null)
            embed.Description = await Runtime.GetStringFromBlock(block.block);

        if (Block.inputs.TryGetValue("color", out block) && block.block != null)
            embed.Color = await Runtime.GetColorFromBlock(block.block);

        if (Block.inputs.TryGetValue("image", out block) && block.block != null)
            embed.ImageUrl = await Runtime.GetStringFromBlock(block.block);

        if (Block.inputs.TryGetValue("thumbnail", out block) && block.block != null)
            embed.ThumbnailUrl = await Runtime.GetStringFromBlock(block.block);

        if (Block.inputs.TryGetValue("footer_text", out block) && block.block != null)
        {
            embed.Footer = new EmbedFooterBuilder
            {
                Text = await Runtime.GetStringFromBlock(block.block)
            };
        }

        if (Block.inputs.TryGetValue("footer_icon", out block) && block.block != null)
        {
            if (embed.Footer == null)
                embed.Footer = new EmbedFooterBuilder();
            embed.Footer.IconUrl = await Runtime.GetStringFromBlock(block.block);
        }

        if (Block.inputs.TryGetValue("obj_embed_fields_list", out block) && block.block != null)
        {
            foreach (KeyValuePair<string, WorkspaceBlockConnection> i in block.block.inputs)
            {
                if (i.Value.block == null)
                    continue;
                EmbedFieldBuilder field = new EmbedFieldBuilder();
                WorkspaceBlockConnection? innerBlock;
                if (i.Value.block.inputs.TryGetValue("name", out innerBlock) && innerBlock.block != null)
                {
                    field.Name = await Runtime.GetStringFromBlock(innerBlock.block);
                }

                if (i.Value.block.inputs.TryGetValue("text", out innerBlock) && innerBlock.block != null)
                {
                    field.Value = Runtime.GetStringFromBlock(innerBlock.block);
                }
                if (i.Value.block.inputs.TryGetValue("is_inline", out innerBlock) && innerBlock.block != null)
                {
                    field.IsInline = (await Runtime.GetBoolFromBlock(innerBlock.block)).GetValueOrDefault(false);
                }
                if (!string.IsNullOrEmpty(field.Name) && field.Value != null && !string.IsNullOrEmpty(field.Value.ToString()))
                    embed.AddField(field);
            }
        }
        return embed;
    }
}
