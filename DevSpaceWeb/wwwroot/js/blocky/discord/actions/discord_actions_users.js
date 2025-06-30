const Connections = new ValidDiscordConnections();

Blockly.Blocks["action_user_create_dm_channel"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Private Channel");
        this.appendValueInput("user")
            .setCheck(Connections.DataUsers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("user:");
        this.appendValueInput("output_channel")
            .setCheck(Connections.OutputChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output channel:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};