const Connections = new ValidDiscordConnections();
const Logic = new DiscordLogic();


Blockly.Blocks['logic_check_discord_channel'] = {
    init: function () {
        const properties = [
            ["Channel", "channel"],
            ['Type', 'type'],
            ["ID", "id"],
            ["Name", "name"],
            ['Category Id', 'category_id'],
            ['Guild Id', 'guild_id'],
            ['Position', 'position']
        ]
        const propertiesMapping = {
            channel: [
                ['Exists', 'exists'],
                ['Is In Category', 'is_in_category'],
                ['Is Nsfw', 'is_nsfw']
            ],
            type: [
                ['Is DM', 'is_dm'],
                ['Is Group', 'is_group'],
                ['Is Any Textable Channel', 'is_any_textable_channel'],
                ['Is Any Guild Channel', 'is_any_guild_channel'],
                ['Is Any Guild Textable Channel', 'is_any_guild_textable_channel'],
                ['Is Any Thread Channel', 'is_any_thread_channel'],
                ['Is Any Forum Channel', 'is_any_forum_channel'],
                ['Is Text Channel', 'is_text_channel'],
                ['Is Voice Channel', 'is_voice_channel'],
                ['Is News Channel', 'is_news_channel'],
                ['Is Forum Channel', 'is_forum_channel'],
                ['Is Media Channel', 'is_media_channel'],
                ['Is Stage Channel', 'is_stage_channel'],
                ['Is Public Thread', 'is_public_thread'],
                ['Is Private Thread', 'is_private_thread']
            ],
            id: Logic.stringProperties,
            name: Logic.stringProperties,
            category_id: Logic.stringProperties,
            guild_id: Logic.stringProperties,
            position: Logic.numberProperties
        };

        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .appendField("channel:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
        
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_server'] = {
    init: function () {
        const properties = [
            ["Server", "server"],
            ["ID", "id"],
            ["Name", "name"],
            ["Owner Id", "owner_id"]
        ]

        const propertiesMapping = {
            server: Logic.existsProperties,
            id: Logic.stringProperties,
            name: Logic.stringProperties,
        };

        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .appendField("server:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_message'] = {
    init: function () {
        const properties = [
            ["Message", "message"],
            ['Type', 'type'],
            ["ID", "id"],
            ["Content", "content"],
        ]

        const propertiesMapping = {
            message: [
                ['Exists', 'exists'],
                ['Has Attachments', 'has_attachments']
            ],
            type: [
                ['Is User Message', 'is_user_message'],
                ['Is Bot Message', 'is_bot_message'],
                ['Is System Message', 'is_system_message'],
                ['Is Interaction Message', 'is_interaction_message'],
                ['Is Webhook Message', 'is_webhook_message']
            ],
            id: Logic.stringProperties,
            content: Logic.stringOptionalProperties,

        };

        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .appendField("message:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_member'] = {
    init: function () {
        const properties = [
            ["Member", "member"],
            ["ID", "id"],
            ['Nickname', 'nickname']
        ]

        const propertiesMapping = {
            member: [
                ['Exists', 'exists'],
                ['Has Permission', 'has_permission'],
                ['Not Has Permission', 'not_has_permission']
            ],
            id: Logic.stringProperties,
            nickname: Logic.stringOptionalProperties,

        };

        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .appendField("member:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get", "data_permission_list", "data_permission_manage", "data_permission_mod", "data_permission_server", "data_permission_text", "data_permission_voice"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_user'] = {
    init: function () {
        const properties = [
            ["User", "user"],
            ["ID", "id"],
            ["Username", "username"],
            ['Display Name', 'display_name'],
            ['Flags', 'flags']
        ]

        const propertiesMapping = {
            user: [
                ['Exists', 'exists'],
                ['Is User', 'is_user'],
                ['Is Bot', 'is_bot']
            ],
            id: Logic.stringProperties,
            username: Logic.stringProperties,
            display_name: Logic.stringOptionalProperties,
            flags: [
                ['Is Active Developer', 'is_active_developer'],
                ['Is Early Verified Developer', 'is_early_verified_developer']
            ]
        };

        this.appendValueInput("user")
            .setCheck(Connections.DataUsers)
            .appendField("user:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_role'] = {
    init: function () {
        const properties = [
            ["Role", "role"],
            ["ID", "id"],
            ["Name", "name"],

        ]

        const propertiesMapping = {
            role: Logic.existsProperties,
            id: Logic.stringProperties,
            name: Logic.stringProperties,

        };

        this.appendValueInput("role")
            .setCheck(Connections.DataRoles)
            .appendField("role:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_category'] = {
    init: function () {
        const properties = [
            ["Category", "category"],
            ["ID", "id"],
            ["Name", "name"],

        ]

        const propertiesMapping = {
            category: Logic.existsProperties,
            id: Logic.stringProperties,
            name: Logic.stringProperties,

        };

        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .appendField("category:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_webhook'] = {
    init: function () {
        const properties = [
            ["Webhook", "webhook"],
            ["ID", "id"]

        ]

        const propertiesMapping = {
            webhook: Logic.existsProperties,
            id: Logic.stringProperties

        };

        this.appendValueInput("webhook")
            .setCheck(Connections.DataWebhooks)
            .appendField("wrbhook:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_emoji'] = {
    init: function () {
        const properties = [
            ["Emoji", "emoji"],
            ["ID", "id"],
            ["Name", "name"],

        ]

        const propertiesMapping = {
            emoji: Logic.existsProperties,
            id: Logic.stringProperties,
            name: Logic.stringProperties,

        };

        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .appendField("emoji:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.existsProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['logic_check_discord_variable'] = {
    init: function () {
        const properties = [
            ['Data Type', 'data_type']
        ]

        const propertiesMapping = {
            data_type: [
                ['Is Message', 'is_message'],
                ['Is Channel', 'is_channel'],
                ['Is Category', 'is_category'],
                ['Is Webhook', 'is_webhook'],
                ['Is Server', 'is_server'],
                ['Is Emoji', 'is_emoji'],
                ['Is Role', 'is_role'],
                ['Is Member', 'is_member'],
                ['Is User', 'is_user']
            ]
        };

        this.appendValueInput("variable")
            .setCheck(['variables_get', "data_message_*", "data_channel_*", "data_category_*", "data_webhook_*", "data_server_*", "data_emoji_*", "data_role_*", "data_member_*", "data_user_*"])
            .appendField("discord data:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, [['Is String', 'is_string']]), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(220);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/logic-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

