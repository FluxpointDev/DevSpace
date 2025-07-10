const Connections = new ValidConnections();

Blockly.Blocks['color_hex'] = {
    init: function () {
        this.setColour(120);
        this.appendValueInput('hex')
            .appendField('Color HEX:');
        this.setOutput(true, 'color_hex');
        this.setTooltip('');
        this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
    }
};

Blockly.Blocks['color_hex_picker'] = {
    init: function () {
        var ColorTextValidator = function (newValue) {
            if (this.sourceBlock_ === null)
                return "#ff0000";

            this.sourceBlock_.inputList[0].fieldRow[1].value_ = newValue;
            this.sourceBlock_.inputList[0].fieldRow[1].render_();

            if (newValue && newValue.length > this.sourceBlock_._maxLength)
                return newValue.slice(0, this.sourceBlock_._maxLength);
            else
                return newValue;
        }
        var ColorFieldValidator = function (newValue) {
            if (this.sourceBlock_ === null)
                return "#ff0000";

            window.test = this;
            this.sourceBlock_.inputList[0].fieldRow[2].value_ = newValue;
            this.sourceBlock_.inputList[0].fieldRow[2].render_();
            return newValue;
        }

        this._maxLength = 7;

        this.setColour(120);
        this.appendDummyInput()
            .appendField('Color')
            .appendField(new FieldColour('#ff0000', ColorFieldValidator), 'color')
            .appendField(new Blockly.FieldTextInput('#ff0000', ColorTextValidator), 'custom_color');
        this.setOutput(true, 'color_hex_picker');
        this.setTooltip('');
        this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
    },
    initSvg2: function () {
        base.initSvg();
        console.log(this);
        //this.inputList[0].fieldRow[2].htmlInput_.maxLength = this._maxLength;
    }
};

Blockly.Blocks['color_rgb'] = {
    init: function () {
        this.appendValueInput("red")
            .setCheck("Number")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Color RGB")
            .appendField(" ")
            .appendField("r:");
        this.appendValueInput("green")
            .setCheck("Number")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("g:");
        this.appendValueInput("blue")
            .setCheck("Number")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("b:");
        this.setOutput(true, 'color_rgb');
        this.setColour(120);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['math_number_between'] = {
    init: function () {
        this.appendDummyInput()
            .appendField(new Blockly.FieldNumber(0), "first")
            .appendField(" - ")
            .appendField(new Blockly.FieldNumber(0), "second");
        this.setOutput(true, "math_number_between");
        this.setColour(120);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['miku'] = {
    init: function () {
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("https://cdn.fluxpoint.dev/devspace/miku.webp", 100, 160, { alt: "*", flipRtl: "FALSE" }));
        this.setOutput(true, Connections.TextSingle);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['data_string_response_time'] = {
    init: function () {
        this.itemCheck_ = Connections.TextAll;
        this.appendDummyInput()
            .appendField("Request Response Time");
        this.setOutput(true, Connections.TextSingle);
        this.setColour('#106c22');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['data_string_join'] = {
    init: function () {
        this.itemCheck_ = Connections.TextAll;
        this.appendDummyInput()
            .appendField("String Join")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["space", "space"], ["new line", "newline"], ["list", "list"], ["number", "number"]]
            ), "join_type");
        this.appendEndRowInput();
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};


Blockly.Blocks['action_stop_execution'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Stop Execution");
        this.appendValueInput("obj_message")
            .setCheck("obj_message")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("message obj:");
        this.setPreviousStatement(true, null);
        this.setColour('#a85b35');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['block_try_catch'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Try Actions");
        this.appendValueInput("is_disabled")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField('is disabled:');
        this.appendDummyInput();
        this.appendDummyInput();
        this.appendEndRowInput()
            .appendField("Actions");
        this.appendStatementInput("command_actions")
            .setCheck(Connections.ActionsList);
        this.setPreviousStatement(true, Connections.ActionsList);
        this.setNextStatement(true, Connections.ActionsList);
        this.setColour(220);
        this.setInputsInline(false);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    }
};

Blockly.Blocks['data_string_base64'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Base64")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Encode", "encode"], ["Decode", "decode"]]
            ), "type");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_string_format'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("String Format")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Uppercase", "uppercase"], ["Lowercase", "lowercase"]]
            ), "type");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_string_offset'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("String Offset")
            .appendField(" ")
            .appendField(new Blockly.FieldNumber(0, 0, Infinity, 1), "offset");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_string_offset_end'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("String Offset Between")
            .appendField(" ")
            .appendField(new Blockly.FieldNumber(0, 0, Infinity, 1), "start")
            .appendField(new Blockly.FieldNumber(0, -Infinity, Infinity, 1), "end");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_string_between'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("String Between")
            .appendField(" ")
            .appendField(new Blockly.FieldTextInput(""), "start")
            .appendField("-")
            .appendField(new Blockly.FieldTextInput(""), "end");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {

            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);
        }
        else if (event.type === "selected") {
            
            //window.blazorExtensions.CheckFieldsEmpty(this, [this.inputList[0].fieldRow[2], this.inputList[0].fieldRow[4]]);
        }

    }
};

Blockly.Blocks['data_string_color'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Color Convert")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Hex to RGB", "hex_rgb"], ["RGB to Hex", "rgb_hex"]]
            ), "type");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_string_markdown'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Markdown")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["Heading 1", "heading_1"],
                ["Heading 2", "heading_2"],
                ["Heading 3", "heading_3"],
                ["Heading 4", "heading_4"],
                ["Heading 5", "heading_5"],
                ["Heading 6", "heading_6"],
                ["Bold", "bold"],
                ["Italic", "italic"],
                ["Quote", "quote"],
                ['Code', 'code'],
                ['Code Block', 'code_block']
            ]), "type");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};

Blockly.Blocks['data_string_markdown_link'] = {
    init: function () {
        this.appendDummyInput("")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Markdown Link")
            .appendField(" ")
        this.appendDummyInput()
            .appendField("[");
        this.appendValueInput("name")
            .setCheck(Connections.TextSingle);
        this.appendDummyInput()
            .appendField("](");
        this.appendValueInput("link")
            .setCheck(Connections.TextSingle);
        this.appendDummyInput()
            .appendField(")");
        this.setInputsInline(true);
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[2], this.inputList[4]]);

        }
    }
};

Blockly.Blocks['data_string_markdown_code'] = {
    init: function () {
        this.appendValueInput("string")
            .setCheck(Connections.TextAll)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Markdown Code Block")
            .appendField(" ")
            .appendField(new Blockly.FieldDropdown([
                ["C#", "cs"],
                ["C++", "cpp"],
                ["GO", "go"],
                ["HTML", "html"],
                ["Java", "java"],
                ["Lua", "lua"],
                ["Markdown", "md"],
                ["PHP", "php"],
                ["SQL", "sql"],
                ["XML", "xml"],
                ["Yaml", "yaml"],
                ["JavaScript", "js"],
                ["Python", "py"],
            ]), "type");
        this.setOutput(true, Connections.TextAll);
        this.setColour('#106C50');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
    },
    onchange: function (event) {

        if (!window.blazorExtensions.WarningsEnabled)
            return;

        if ((event.type === "create" && event.json.type === this.type) || (event.type === "move" && event.reason && (event.reason[0] === 'snap' || event.reason[0] === 'connect'))) {


            window.blazorExtensions.CheckInputsEmpty(this, [this.inputList[0]]);

        }
    }
};