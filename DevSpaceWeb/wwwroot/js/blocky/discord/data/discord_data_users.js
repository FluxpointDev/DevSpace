const Connections = new ValidDiscordConnections();

Blockly.Blocks['data_selector_user'] = {
    init: function () {
        const properties = [
            ["ID", "id"],
            ["Username", "username"],
            ['Display Name', 'display_name'],
            ['Avatar Url', 'avatar_url']
        ];

        this.appendValueInput("user")
            .setCheck(Connections.DataUsers)
            .appendField("user:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("h", 5, 5, { alt: "*", flipRtl: "FALSE" }))
            .appendField("select:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.setInputsInline(true);
        this.setOutput(true, Connections.TextSingle);
        this.setColour(120);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");

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
                case "username":
                case "display_name":
                case "avatar_url":
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

Blockly.Blocks['data_user_app'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("App User");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_user_current'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Current User");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_user_active'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Active User");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['action_set_active_user'] = {
    init: function () {
        this.appendValueInput("user")
            .setCheck(Connections.DataUsers)
            .appendField("Set")
            .appendField("")
            .appendField("Active User")
            .appendField("")
            .appendField("to");
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_user_get'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get User");
        this.appendValueInput("user_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("user id:");
        this.setOutput(true, null);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['data_channel_private_get'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Private Channel");
        this.appendValueInput("user_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("user id:");
        this.setOutput(true, null);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['data_channel_group_get'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Group Channel");
        this.appendValueInput("channel_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("group id:");
        this.setOutput(true, null);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};