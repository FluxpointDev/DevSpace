const Connections = new ValidDiscordConnections();

Blockly.Blocks["action_add_role_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Add Role to Member");
        this.appendValueInput("role")
            .setCheck(Connections.DataRoles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role:");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("reason")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("reason:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/members");
    }
};

Blockly.Blocks["action_set_nickname_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Set Nickname for Member");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("reason")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("reason:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/members");
    }
};

Blockly.Blocks["action_remove_timeout_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Remove Timeout for Member");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("reason")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("reason:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/members");
    }
};

Blockly.Blocks["action_set_timeout_member"] = {
    init: function () {
        const timeoutTime = [
            ['Seconds', 'seconds'],
            ['Minutes', 'minutes'],
            ['Hours', 'hours'],
            ['Days', 'days']
        ];

        this.appendDummyInput()
            .appendField("Set Timeout for Member");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("time:")
            .appendField(new Blockly.FieldDropdown(timeoutTime), "type")
        this.appendValueInput("number")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField('number:')
            .setCheck(Connections.Number);
        this.appendValueInput("reason")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("reason:");
        this.setInputsInline(false);
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/members");
    }
};

Blockly.Blocks["action_remove_role_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Remove Role from Member");
        this.appendValueInput("role")
            .setCheck(Connections.DataRoles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("role:");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("reason")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("reason:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/members");
    }
};

Blockly.Blocks["action_kick_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Kick Member");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("reason")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("reason:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/members");
    }
};

Blockly.Blocks["action_ban_member"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Ban Member");
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("member:");
        this.appendValueInput("prune")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField('prune days:')
            .setCheck(Connections.Number);
        this.appendValueInput("reason")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("reason:");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#972f40');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/action-blocks/members");
    }
};

