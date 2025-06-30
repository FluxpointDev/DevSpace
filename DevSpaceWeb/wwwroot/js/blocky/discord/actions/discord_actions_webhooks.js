const Connections = new ValidDiscordConnections();

Blockly.Blocks['action_send_webhook_message'] = {
    init: function () {
        this.appendDummyInput()
            .appendField('Send Webhook Message')
        this.appendValueInput("webhook")
            .setCheck(Connections.DataWebhooks)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook:");
        this.appendValueInput("obj_webhook_message")
            .setCheck("obj_webhook_message")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook msg obj:");
        this.appendValueInput("obj_message")
            .setCheck("obj_message")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message obj:");
        this.appendValueInput("output_message")
            .setCheck(Connections.OutputMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};


Blockly.Blocks["action_create_webhook"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Webhook");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
        this.appendValueInput("obj_webhook")
            .setCheck('obj_webhook')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook obj:");
        this.appendValueInput("output_webhook")
            .setCheck(Connections.OutputWebhooks)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output webhook:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks["action_modify_webhook_message"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Webhook Message");
        this.appendValueInput("webhook")
            .setCheck(Connections.DataWebhooks)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook:");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.appendValueInput("obj_webhook")
            .setCheck('obj_webhook')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks["action_modify_webhook"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Webhook");
        this.appendValueInput("webhook")
            .setCheck(Connections.DataWebhooks)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook:");
        this.appendValueInput("obj_webhook")
            .setCheck('obj_webhook')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks["action_delete_webhook_message"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Webhook Message");
        this.appendValueInput("webhook")
            .setCheck(Connections.DataWebhooks)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook:");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks["action_delete_webhook"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Webhook");
        this.appendValueInput("webhook")
            .setCheck(Connections.DataWebhooks)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("webhook:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['obj_webhook'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Webhook Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("avatar")
            .setCheck(Connections.DataFiles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("avatar file:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['obj_webhook_message'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Webhook Message Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("avatar_url")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("avatar url:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("");
    }
};