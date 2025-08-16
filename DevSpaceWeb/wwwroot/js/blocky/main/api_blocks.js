const Connections = new ValidConnections();

Blockly.Blocks['action_api'] = {
    init: function () {
        this.appendValueInput("request")
            .setCheck("obj_api")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Send API Request");
        this.setInputsInline(false);
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['data_response_active'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Active Response");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};

Blockly.Blocks['data_selector_response'] = {
    init: function () {
        const properties = [
            ["Is Success", "is_success"],
            ['Status Code', 'status_code'],
            ['Has Content', 'has_content'],
            ['Content Length', 'content_length'],
            ['Mime Type', 'mime_type']
        ];

        this.appendValueInput("response")
            .setCheck(Connections.DataResponse)
            .appendField("response:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField("select:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.setInputsInline(true);
        this.setOutput(true, Connections.Bool);
        this.setColour(120);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");

        
    },
    onchange: function (change) {

        if (!window.blazorExtensions.WarningsEnabled && (change.type === "create" && change.json.type === this.type) || (change.type === "move" && change.reason && (change.reason[0] === 'snap' || change.reason[0] === 'connect'))) {

            window.blazorExtensions.CheckInputsEmpty(this, [ this.inputList[0] ]);
                
        }

        if (change.type === "change" && change.name === "property") {
            var prop = change.newValue;
            var outputChecks = null;
            switch (prop) {
                case "id":
                case "name":
                    outputChecks = Connections.TextSingle;
                    break;
                case "is_success":
                case "has_content":
                    outputChecks = Connections.Bool;
                    break;
                case "status_code":
                case "content_length":
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

Blockly.Blocks["data_string_header"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Select Header String");
        this.appendValueInput("response")
            .setCheck(Connections.DataResponse)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("response:");
        this.appendValueInput("header")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("header:");
        this.setOutput(true, null);
        this.setColour('#106c22');
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

Blockly.Blocks["data_number_header"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Select Header Number");
        this.appendValueInput("response")
            .setCheck(Connections.DataResponse)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("response:");
        this.appendValueInput("header")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("header:");
        this.setOutput(true, null);
        this.setColour('#106c22');
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



Blockly.Blocks['obj_api'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Request");
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("type:")
            .appendField(new Blockly.FieldDropdown([
                ["Get", "get"],
                ["Post", "post"],
                ["Put", "put"],
                ["Patch", "patch"],
                ["Delete", "delete"]
            ]), "api_type");
        this.appendValueInput("url")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("url:");
        this.appendValueInput("obj_api_query_list")
            .setCheck("obj_api_query_list")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("query list:");
        this.appendValueInput("obj_api_headers_list")
            .setCheck("obj_api_headers_list")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("headers list:");
        this.appendValueInput("body")
            .setCheck(Connections.TextAll.concat('obj_api_json_list'))
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("body:");
        this.appendValueInput("response")
            .setCheck(Connections.DataResponse)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("response:");
        this.appendValueInput("is_fail")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("fail on error:");
        this.appendValueInput("output_data")
            .setCheck(['variables_get', 'data_json_active', 'data_file_active'])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output data:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");

        console.log('API');
        console.log(this);
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[2]]);

        }
    }
};


Blockly.Blocks['obj_api_list_auth'] = {
    init: function () {
        this.appendValueInput('value')
            .setCheck(Connections.TextSingle)
            .appendField('Authorization:');
        this.setOutput(true, null);
        this.setColour(120);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
    }
};


Blockly.Blocks['obj_api_list_item'] = {
    init: function () {
        this.appendValueInput('value')
            .setCheck(Connections.TextSingle.concat('obj_api_json_list'))
            .appendField(new Blockly.FieldTextInput(""), "key")
            .appendField(':');
        this.setOutput(true, null);
        this.setColour(120);
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

Blockly.Blocks['obj_api_query_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_api_list_item"];
        this.appendDummyInput()
            .appendField("Query List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};

Blockly.Blocks['obj_api_headers_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_api_list_item", "obj_api_list_auth"];
        this.appendDummyInput()
            .appendField("Headers List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/cloudfrost-dev/apps");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};