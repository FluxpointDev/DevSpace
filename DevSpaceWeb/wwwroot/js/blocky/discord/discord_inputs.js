const Connections = new ValidDiscordConnections();

Blockly.Blocks['input_string'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("String Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("obj_input_choices_list")
            .setCheck("obj_input_choices_list")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("choices:");
        this.appendValueInput("arg_minmax")
            .setCheck("arg_minmax")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("length limit:");
        this.appendValueInput("output_variable")
            .setCheck("variables_get")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output variable:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_integer'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Integer Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("obj_input_choices_list")
            .setCheck("obj_input_choices_list")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("choices:");
        this.appendValueInput("arg_minmax")
            .setCheck("arg_minmax")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("number limit:");
        this.appendValueInput("output_variable")
            .setCheck("variables_get")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output variable:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_boolean'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Boolean Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("output_variable")
            .setCheck("variables_get")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output variable:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_user'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("User Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("output_user")
            .setCheck(Connections.OutputUsers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output user:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_member'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Member Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("output_member")
            .setCheck(Connections.OutputMembers)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output member:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_channel'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Channel Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("output_channel")
            .setCheck(Connections.OutputChannels)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output channel:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_role'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Role Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("output_role")
            .setCheck(Connections.OutputRoles)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output role:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_mentionable'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Mentionable Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("output_mentionable")
            .setCheck(["variables_get", "data_user_active", "data_member_active", "data_role_active"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output mentionable:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_number'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Number Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("obj_input_choices_list")
            .setCheck("obj_input_choices_list")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("choices:");
        this.appendValueInput("arg_minmax")
            .setCheck("arg_minmax")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("number limit:");
        this.appendValueInput("output_variable")
            .setCheck("variables_get")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output variable:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['input_attachment'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("Attachment Input");
        this.appendValueInput("name")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("name:");
        this.appendValueInput("is_required")
            .setCheck("logic_boolean")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("description")
            .setCheck("text")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("output_file")
            .setCheck(Connections.OutputFile)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output file:");
        this.setPreviousStatement(true, Connections.AllowedInputs);
        this.setNextStatement(true, Connections.AllowedInputs);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[1]]);

        }
    }
};

Blockly.Blocks['obj_input_choices_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_input_choices_item"];
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Choices List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("item:");
        this.setOutput(true, 'obj_input_choices_list');
        this.setColour('#b29655');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
        this.appendArrayElementInput();
    }
};

Blockly.Blocks['obj_input_choices_item'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("name:")
            .appendField(new Blockly.FieldTextInput(""), "name")
            .appendField(" ")
            .appendField("value:")
            .appendField(new Blockly.FieldTextInput(""), "value");
        this.setOutput(true, 'obj_input_choices_item');
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    },
    onchange: function (event) {
        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || event.type === "selected") {

            window.blazorExtensions.CheckFieldsEmpty(this, [this.inputList[0].fieldRow[1], this.inputList[0].fieldRow[4] ]);

        }
    }
};

Blockly.Blocks['obj_input_minmax'] = {
    init: function () {
        this.appendDummyInput()
            .appendField("min:")
            .appendField(new Blockly.FieldTextInput("0"), "min")
            .appendField(" ")
            .appendField("max:")
            .appendField(new Blockly.FieldTextInput("0"), "max");
        this.setOutput(true, 'obj_input_minmax');
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/v/discord/workspace/input-blocks");
    }
};