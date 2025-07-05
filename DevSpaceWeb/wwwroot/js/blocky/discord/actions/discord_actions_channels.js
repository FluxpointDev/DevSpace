const Connections = new ValidDiscordConnections();

Blockly.Blocks["action_delete_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_delete_category"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Category");
        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_create_text_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Text Channel");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("obj_text_channel")
            .setCheck("obj_text_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("text channel obj:");
        this.appendValueInput("output_channel")
            .setCheck(Connections.OutputChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output channel:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_modify_text_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Text Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("obj_text_channel")
            .setCheck("obj_text_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("text channel obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_create_voice_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Voice Channel");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("obj_voice_channel")
            .setCheck("obj_voice_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("voice channel obj:");
        this.appendValueInput("output_channel")
            .setCheck(Connections.OutputChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output channel:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_modify_voice_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Voice Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("obj_voice_channel")
            .setCheck("obj_voice_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("voice channel obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_create_category_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Category Channel");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("obj_category_channel")
            .setCheck("obj_category_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category channel obj:");
        this.appendValueInput("output_category")
            .setCheck(Connections.OutputCategory)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output category:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_modify_category_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Category Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("obj_category_channel")
            .setCheck("obj_category_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category channel obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['obj_text_channel'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Text Channel Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category:");
        this.appendValueInput("topic")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("topic:");
        this.appendValueInput("slowmode")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("slowmode:");
        this.appendValueInput("position")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("position:");
        this.appendValueInput("permissions")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permissions:");
        this.appendValueInput("is_nsfw")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is nsfw:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['obj_voice_channel'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Voice Channel Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category:");
        this.appendValueInput("slowmode")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("slowmode:");
        this.appendValueInput("position")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("position:");
        this.appendValueInput("permissions")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permissions:");
        this.appendValueInput("is_nsfw")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is nsfw:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['obj_category_channel'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Category Channel Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("position")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("position:");
        this.appendValueInput("permissions")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permissions:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_create_forum_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Forum Channel");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("obj_forum_channel")
            .setCheck("obj_forum_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("forum channel obj:");
        this.appendValueInput("output_channel")
            .setCheck(Connections.OutputChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output channel:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_modify_forum_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Forum Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("obj_forum_channel")
            .setCheck("obj_forum_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("forum channel obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['obj_forum_channel'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Forum Channel Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category:");
        this.appendValueInput("slowmode")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("slowmode:");
        this.appendValueInput("position")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("position:");
        this.appendValueInput("permissions")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permissions:");
        this.appendEndRowInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("view:")
            .appendField(new Blockly.FieldDropdown([
                ["List View", "list_view" ],
                ["Gallery View", "gallery_view" ]
            ]), "view_type");
        this.appendValueInput("is_nsfw")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is nsfw:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_create_announcement_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Announcement Channel");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("obj_text_channel")
            .setCheck("obj_text_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("text channel obj:");
        this.appendValueInput("output_channel")
            .setCheck(Connections.OutputChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output channel:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_modify_announcement_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Announcement Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("obj_text_channel")
            .setCheck("obj_text_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("text channel obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};


Blockly.Blocks["action_create_stage_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Stage Channel");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("obj_stage_channel")
            .setCheck("obj_stage_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("stage channel obj:");
        this.appendValueInput("output_channel")
            .setCheck(Connections.OutputChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output channel:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_modify_stage_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Stage Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("obj_stage_channel")
            .setCheck("obj_stage_channel")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("stage channel obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['obj_stage_channel'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Stage Channel Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category:");
        this.appendValueInput("slowmode")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("slowmode:");
        this.appendValueInput("position")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("position:");
        this.appendValueInput("permissions")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permissions:");
        this.appendValueInput("is_nsfw")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is nsfw::");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};