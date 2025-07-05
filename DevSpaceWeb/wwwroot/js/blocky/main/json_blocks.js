Blockly.Blocks['obj_api_json_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_api_list_item"];
        this.appendDummyInput()
            .appendField("Json List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};

Blockly.Blocks['data_json_active'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Active Json");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks["data_selector_json"] = {
    init: function () {
        this.appendValueInput('json')
            .setCheck(Connections.DataJson)
            .appendField('json:');
        this.appendDummyInput()
            .appendField("");
        this.appendDummyInput('type')
            .appendField('type:')
            .appendField(new Blockly.FieldDropdown([
                ['String', 'string'],
                ['Number', 'number'],
                ['Bool', 'bool']
            ]), 'type');
        this.appendDummyInput()
            .appendField("");
        this.appendValueInput('select')
            .setCheck(Connections.TextSingle)
            .appendField('select:');
        this.setInputsInline(true)
        this.setOutput(true, null);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
        this.setColour(120);
    }
};

Blockly.Blocks["data_string_json"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Select Json String");
        this.appendValueInput("json")
            .setCheck(Connections.DataJson)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("json:");
        this.appendValueInput("key")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("key:");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};

Blockly.Blocks["data_number_json"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Select Json Number");
        this.appendValueInput("json")
            .setCheck(Connections.OutputJson)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("json:");
        this.appendValueInput("key")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("key:");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};

Blockly.Blocks["data_bool_json"] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Select Json Boolean");
        this.appendValueInput("json")
            .setCheck(Connections.OutputJson)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("json:");
        this.appendValueInput("key")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("key:");
        this.setOutput(true, null);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1], this.inputList[2]]);

        }
    }
};