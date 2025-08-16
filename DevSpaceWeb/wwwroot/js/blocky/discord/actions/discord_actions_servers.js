const Connections = new ValidDiscordConnections();

Blockly.Blocks["action_server_leave"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Leave Server");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks["action_server_create_emoji"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Emoji");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("file")
            .setCheck(Connections.DataFiles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("file:");
        this.appendValueInput("obj_emoji")
            .setCheck("obj_emoji")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji obj:");
        this.appendValueInput("output_emoji")
            .setCheck(Connections.OutputEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output emoji:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks["action_server_modify_emoji"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Emoji");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("obj_emoji")
            .setCheck("obj_emoji")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['obj_emoji'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Emoji Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("obj_roles_list")
            .setCheck("obj_roles_list")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("restricted roles:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks["action_server_delete_emoji"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Emoji");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};