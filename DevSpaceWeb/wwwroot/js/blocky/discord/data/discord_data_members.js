const Connections = new ValidDiscordConnections();

Blockly.Blocks['data_selector_member'] = {
    init: function () {
        const properties = [
            ["ID", "id"],
            ["Nickname", "nickname"],
            ['Server Id', 'guild_id'],
        ];

        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .appendField("member:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField("select:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.setInputsInline(true);
        this.setOutput(true, Connections.TextSingle);
        this.setColour(120);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");

    },
    onchange: function (change) {

        if (!window.blazorExtensions.WarningsEnabled && (change.type === "create" && change.json.type === this.type) || (change.type === "move" && change.reason && (change.reason[0] === 'snap' || change.reason[0] === 'connect'))) {

            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }

        if (change.type === "change" && change.name === "property") {
            var prop = change.newValue;
            var outputChecks = null;
            switch (prop) {
                case "id":
                case "nickname":
                case "guild_id":
                    outputChecks = Connections.TextSingle;
                    break;
            }

            if (outputChecks !== null) {
                var previousConnection = this.outputConnection.targetConnection;
                this.outputConnection.setCheck(outputChecks);

                if (previousConnection) {
                    var canConnect = previousConnection.getConnectionChecker().canConnect(this.outputConnection, previousConnection);
                    if (!canConnect) {
                        this.moveBy(80, -40)
                    }
                }
            }
        }
    }
};

Blockly.Blocks['data_member_app'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("App Member");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['data_member_current'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Current Member");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['data_member_active'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Active Member");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['action_set_active_member'] = {
    init: function () {
        this.appendValueInput("member")
            .setCheck(Connections.DataMembers)
            .appendField("Set")
            .appendField("")
            .appendField("Active Member")
            .appendField("")
            .appendField("to");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_member_get'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Member in Server");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("user_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("user id:");
        this.setOutput(true, null);
        this.setColour('#106C50');
        this.setTooltip("");
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