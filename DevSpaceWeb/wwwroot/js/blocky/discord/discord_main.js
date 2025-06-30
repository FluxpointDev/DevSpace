const Connections = new ValidDiscordConnections();

Blockly.Blocks['block_command'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/866044602153828404.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Slash Command:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput()
            .appendField("Inputs");
        this.appendStatementInput("command_inputs")
            .setCheck(Connections.AllowedInputs);
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['block_user_command'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("User Command:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_user")
            .setCheck(["data_member_active", "data_user_active", "variables_get"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member/user:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['block_message_command'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912031377422172160.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Message Command:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_message")
            .setCheck(Connections.OutputMessage)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_permission_list'] = {
    init: function () {
        this.itemCheck_ = ["data_permission_manage", "data_permission_mod", "data_permission_server", "data_permission_text", "data_permission_voice"];
        this.appendDummyInput()
            .appendField("Permissions List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, 'data_permission_list');
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true)
    }
};

Blockly.Blocks['data_permission_manage'] = {
    init: function () {
        this.appendEndRowInput()
            .appendField("Manage Permission")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Administrator", "administrator"],
                ["Manage Server", "manage_server"],
                ["Manage Channels", "manage_channels"],
                ["Manage Roles", "manage_roles"],
                ["Manage Webhooks", "manage_webhooks"],
                ["Create Expressions", "create_expressions"],
                ["Manage Expressions", "manage_expressions"],
                ["Create Events", "create_events"],
                ["Manage Events", "manage_events"],
            ]), "permission_type");
        this.setOutput(true, 'data_permission_manage');
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_permission_mod'] = {
    init: function () {
        this.appendEndRowInput()
            .appendField("Mod Permission")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Manage Nicknames", "manage_nicknames"],
                ["Manage Messages", "manage_messages"],
                ["Manage Threads", "manage_threads"],
                ["Kick Members", "kick_members"],
                ["Ban Members", "ban_members"],
                ["Moderate Members", "moderate_members"],
                ["Mention @everyone & Roles", "mention_everyone"],
            ]), "permission_type");
        this.setOutput(true, 'data_permission_mod');
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_permission_server'] = {
    init: function () {
        this.appendEndRowInput()
            .appendField("Server Permission")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Create Invites", "create_invites"],
                ["Change Nickname", "change_nickname"],
                ["View Audit Log", "view_audit_log"],
                ["View Server Insights", "view_server_insights"],
                ["View Creator Monetization", "view_creator_monetization"]
            ]), "permission_type");
        this.setOutput(true, 'data_permission_server');
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_permission_text'] = {
    init: function () {
        this.appendEndRowInput()
            .appendField("Text Permission")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["View Channels", "view_channels"],
                ["Read Message History", "read_message_history"],
                ["Send Messages", "send_messages"],
                ["Send Messages in Threads", "send_messages_threads"],
                ["Create Public Threads", "create_public_threads"],
                ["Create Private Threads", "create_private_threads"],
                ["Embed Links", "embed_links"],
                ["Attach Files", "attach_files"],
                ["Use External Emojis", "use_external_emojis"],
                ["Use External Stickers", "use_external_stickers"],
                ["Send TTS Messages", "send_tts_messages"],
                ["Use Application Commands", "use_application_commands"],
                ["Send Voice Messages", "send_voice_messages"],
                ["Use Activities", "use_activities"],
            ]), "permission_type");
        this.setOutput(true, 'data_permission_text');
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_permission_voice'] = {
    init: function () {
        this.appendEndRowInput()
            .appendField("Voice Permission")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Voice Connect", "voice_connect"],
                ["Voice Speak", "voice_speak"],
                ["Voice Video", "voice_video"],
                ["Use Soundboard", "use_soundboard"],
                ["Use External Sounds", "use_external_sounds"],
                ["Use Voice Activity", "use_voice_activity"],
                ["Voice Priority Speaker", "voice_priority_speaker"],
                ["Voice Mute Members", "voice_mute_members"],
                ["Voice Deafen Members", "voice_deafen_members"],
                ["Voice Move Members", "voice_move_members"],
                ["Set Voice Channel Status", "voice_set_status"],
                ["Voice Request to Speak", "voice_request_to_speak"],

                
                
            ]), "permission_type");
        this.setOutput(true, 'data_permission_voice');
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};


Blockly.Blocks['block_interaction_button'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Interaction Button:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_data")
            .setCheck(Connections.DataInteraction)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};

Blockly.Blocks['block_interaction_select_string'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Interaction Text Select:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_string")
            .setCheck(["variables_get"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("option text id:");
        this.appendValueInput("output_data")
            .setCheck(["variables_get"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['block_interaction_select_user'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Interaction User Select:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_user")
            .setCheck(["variables_get"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("option user/member:");
        this.appendValueInput("output_data")
            .setCheck(["variables_get"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['block_interaction_select_role'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Interaction Role Select:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_role")
            .setCheck(["variables_get"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role:");
        this.appendValueInput("output_data")
            .setCheck(["variables_get"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['block_interaction_select_channel'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Interaction Channel Select:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_channel")
            .setCheck(Connection.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("output_data")
            .setCheck(Connections.DataInteraction)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['block_interaction_select_mentionable'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Interaction Mentionable Select:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Output");
        this.appendValueInput("output_mentionable")
            .setCheck(Connections.DataInteraction)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("mentionable:");
        this.appendValueInput("output_data")
            .setCheck(Connections.DataInteraction)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['block_interaction_modal'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField(new Blockly.FieldImage("https://cdn.discordapp.com/emojis/912027882799374356.webp?size=128&quality=lossless", 16, 16, { alt: "*", flipRtl: "FALSE" }))
            .appendField("Interaction Modal:")
            .appendField(new Blockly.FieldLabelSerializable(""), "name");
        this.appendValueInput("modal_title")
            .setCheck(["text"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("title:");
        this.appendValueInput("output_data")
            .setCheck(Connections.DataInteraction)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output data:");
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Fields");
        this.appendStatementInput("modal_fields")
            .setCheck(['obj_component_modal_input_text']);
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Options");
        this.appendStatementInput("command_options")
            .setCheck("option_*");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setColour(240);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[5]]);

        }
    }
};