const Connections = new ValidDiscordConnections();

Blockly.Blocks['action_send_response_message'] = {
    init: function () {
        this.appendDummyInput()
            .appendField('Send Interaction Message')
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
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['action_update_response_message'] = {
    init: function () {
        this.appendDummyInput()
            .appendField('Update Interaction Message')
        this.appendValueInput("obj_message")
            .setCheck("obj_message")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['action_send_message'] = {
    init: function () {
        this.appendDummyInput()
            .appendField('Send Channel Message')
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
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
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['action_modify_message'] = {
    init: function () {
        this.appendDummyInput()
            .appendField('Modify Message')
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.appendValueInput("obj_message")
            .setCheck("obj_message")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['action_update_components'] = {
    init: function () {
        this.appendDummyInput()
            .appendField('Update Components')
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.appendValueInput("components")
            .setCheck("obj_component_row")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("components:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_delete_interaction_message"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Interaction Message");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_delete_message"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Message");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_add_reaction_self"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Add Reaction with App");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_remove_reaction_self"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Remove Reaction for App");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_remove_reaction_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Remove Reaction from Member");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_remove_all_reactions_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Remove All Reactions from Member");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_remove_reactions"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Remove Reactions with Emoji");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_remove_all_reactions"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Remove All Reactions");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_pin_message"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Pin Message");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["action_crosspost_message"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Publish Message");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};


Blockly.Blocks["action_unpin_message"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Unpin Message");
        this.appendValueInput("message")
            .setCheck(Connections.DataMessages)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['obj_message'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Message Object");
        this.appendValueInput("content")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("content:");
        this.appendValueInput("obj_embed")
            .setCheck("obj_embed")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("embed obj:");
        this.appendValueInput("file")
            .setCheck(Connections.DataFiles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("file:");
        this.appendValueInput("is_file_spoiler")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is file spoiler:");
        this.appendValueInput("obj_components_list")
            .setCheck("obj_components_list")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("components:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};



Blockly.Blocks['obj_embed'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Embed Object");
        this.appendValueInput("title")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("title:");
        this.appendValueInput("author_name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("author name:");
        this.appendValueInput("author_icon")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("author icon url:");
        this.appendValueInput("desc")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("obj_embed_fields_list")
            .setCheck('obj_embed_fields_list')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("fields list:");
        this.appendValueInput("color")
            .setCheck(Connections.DataColors)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("color:");
        this.appendValueInput("image")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("image url:");
        this.appendValueInput("thumbnail")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("thumbnail url:");
        this.appendValueInput("footer_text")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("footer text:");
        this.appendValueInput("footer_icon")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("footer icon url:");
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['obj_embed_fields_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_embed_field_item"];
        this.appendDummyInput()
            .appendField("Fields List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true)
    }
};

Blockly.Blocks['obj_embed_field_item'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Embed Field");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("text")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("text:");
        this.appendValueInput("is_inline")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is inline:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};