const Connections = new ValidDiscordConnections();

Blockly.Blocks['option_allowmentions'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Allow Mentions")
            .appendField(" ");
        this.appendEndRowInput()
            .appendField(new Blockly.FieldDropdown([
                ["Reply", "mention_reply"],
                ["All Users", "mention_users"],
                ["All Roles", "mention_roles"],
                ["@Everyone & @Here", "mention_everyone"],
                ["Everything", "mention_everything"]
            ]), "mention_type");
        
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_allowmentions_user'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Allow User Mention");
        this.appendValueInput("user")
            .setCheck(Connections.DataUsers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("user:");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['option_allowmentions_role'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Allow Role Mention");
        this.appendValueInput("role")
            .setCheck(Connections.DataRoles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role:");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {

            
            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['option_open_modal'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Open Modal");
        this.appendValueInput("name")
            .setCheck(['text', 'variables_get'])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("data")
            .setCheck(["data_message_active", "data_channel_active",
                "data_category_active", "data_server_active", "data_role_active", "data_member_active", "data_user_active", "data_message_current"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason &&
            (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {

            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['option_require_app_premium'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Require Discord App Premium");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_ephemeral'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Ephemeral Response");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_nsfw_only'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Nsfw Channel Only!");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_app_owner_only'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("App Owner Only!");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_app_developer_only'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("App Developer Only!");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_server_owner_only'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Server Owner Only!");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_require_server'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Require Server");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_require_private_channel'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Require Private Channel");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_require_group_channel'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Require Group Channel");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_allow_user_apps'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Allow User Apps");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['option_require_server_permission'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Require Server Permissions");
        this.appendValueInput("permission")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permission:");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};

Blockly.Blocks['option_require_channel_permission'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.ALIGN_CENTER)
            .appendField("Require Channel Permissions");
        this.appendValueInput("permission")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permission:");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#a9902c');
        this.setTooltip("Message is optional");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};