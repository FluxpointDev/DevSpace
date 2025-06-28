const Connections = new ValidDiscordConnections();

Blockly.Blocks["action_role_create"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Create Role");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("obj_role")
            .setCheck("obj_role")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role obj:");
        this.appendValueInput("output_role")
            .setCheck(Connections.OutputRoles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output role:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/roles");
    }
};

Blockly.Blocks['obj_role'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Role Object");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("color")
            .setCheck(Connections.DataColors)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("color:");
        this.appendValueInput("is_hoisted")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is hoisted:");
        this.appendValueInput("permissions")
            .setCheck(Connections.DataPermissions)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("permissions:");
        this.appendValueInput("is_mentionable")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is mentionable:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/roles");
    }
};

Blockly.Blocks["action_role_modify"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Modify Role");
        this.appendValueInput("role")
            .setCheck(Connections.DataRoles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role:");
        this.appendValueInput("obj_role")
            .setCheck("obj_role")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role obj:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/roles");
    }
};

Blockly.Blocks["action_role_delete"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Delete Role");
        this.appendValueInput("role")
            .setCheck(Connections.DataRoles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/roles");
    }
};

Blockly.Blocks['obj_roles_list'] = {
    init: function () {
        this.itemCheck_ = Connections.DataRoles;
        this.appendDummyInput()
            .appendField("Roles List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/roles");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};