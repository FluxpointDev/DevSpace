const Connections = new ValidDiscordConnections();

Blockly.Blocks['data_selector_channel'] = {
    init: function () {
        const properties = [
            ["ID", "id"],
            ["Name", "name"],
            ['Category Id', 'category_id'],
            ['Server Id', 'guild_id'],
            ['Position', 'position']
        ];

        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .appendField("channel:");
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
            switch (prop)
            {
                case "id":
                case "name":
                case "category_id":
                case "guild_id":
                    outputChecks = Connections.TextSingle;
                    break;
                case "position":
                    outputChecks = Connections.Number;
                    break;
            }

            if (outputChecks !== null)
            {
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

Blockly.Blocks['data_channel_current'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Current Channel");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_channel_active'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Active Channel");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_selector_category'] = {
    init: function () {
        const properties = [
            ["ID", "id"],
            ["Name", "name"],
            ['Server Id', 'guild_id'],
            ['Position', 'position']
        ];

        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .appendField("category:");
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
                case "name":
                case "guild_id":
                    outputChecks = Connections.TextSingle;
                    break;
                case "position":
                    outputChecks = Connections.Number;
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

Blockly.Blocks['data_category_current'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Current Category");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['data_category_active'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Active Category");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['action_set_active_channel'] = {
    init: function () {
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .appendField("Set")
            .appendField("")
            .appendField("Active Channel")
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

Blockly.Blocks['data_channel_get'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Channel");
        this.appendValueInput("channel_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel id:");
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

Blockly.Blocks['data_channel_get_in_server'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Channel in Server");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("channel_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel id:");
        this.setOutput(true, null);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};

Blockly.Blocks['action_set_active_category'] = {
    init: function () {
        this.appendValueInput("category")
            .setCheck(Connections.DataCategory)
            .appendField("Set")
            .appendField("")
            .appendField("Active Category")
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

Blockly.Blocks['data_category_get'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Category");
        this.appendValueInput("category_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category id:");
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

Blockly.Blocks['data_category_get_from_channel'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Category from Channel");
        this.appendValueInput("channel")
            .setCheck(Connections.DataChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("channel:");
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

Blockly.Blocks['data_category_get_in_server'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Get Category in Server");
        this.appendValueInput("server")
            .setCheck(Connections.DataServers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("server:");
        this.appendValueInput("category_id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("category id:");
        this.setOutput(true, null);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};