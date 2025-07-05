const Connections = new ValidConnections();
const Logic = new MainLogic();

Blockly.Blocks['logic_condition_and'] = {
    init: function () {
        this.itemCheck_ = ["logic_check_*"];
        this.appendDummyInput()
            .appendField("Logic And");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour(210);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true)
    }
};

Blockly.Blocks['logic_condition_or'] = {
    init: function () {
        this.itemCheck_ = ["logic_check_*"];
        this.appendDummyInput()
            .appendField("Logic Or");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour(210);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true)
    }
};

Blockly.Blocks['logic_check_basic'] = {
    init: function () {
        const properties = [
            ["Basic Type", "basic_type"],
            ['Data Type', 'data_type'],
            ["String", "string"],
            ["Boolean", "boolean"],
            ['Number', 'number']
        ]

        const propertiesMapping = {
            basic_type: [
                ['Is String', 'is_string'],
                ['Is Boolean', 'is_boolean'],
                ['Is Number', 'is_number'],
                ['Is Integer', 'is_integer'],
                ['Is Double', 'is_double'],
            ],
            data_type: [
                ['Is File', 'is_file'],
                ['Is Json', 'is_json'],
                ['Is Response', 'is_response']
            ],
            string: Logic.stringProperties,
            boolean: Logic.boolProperties,
            number: Logic.numberProperties
        };

        this.appendValueInput("variable")
            .setCheck(['variables_get', 'data_string_*'])
            .appendField("variable:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, [['Is String', 'is_string']]), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "text", "text_multiline", "variables_get"])
            .appendField(new Blockly.FieldImage("", 5, 5));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(210);
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

Blockly.Blocks['logic_check_file'] = {
    init: function () {
        const properties = [
            ["File", "file"],
            ['Name', 'name'],
            ['Length', 'length'],
            ['Type', 'type']
        ]

        const propertiesMapping = {
            file: [
                ['Exists', 'exists'],
                ['Is Image', 'is_image']
            ],
            name: Logic.stringProperties,
            length: Logic.numberProperties,
            type: [
                ['Is png', 'is_png'],
                ['Is jpg', 'is_jpg'],
                ['Is webp', 'is_webp']
            ]
        };

        this.appendValueInput("file")
            .setCheck(Connections.DataFiles)
            .appendField("file:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.boolProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("", 5, 5));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(210);
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

Blockly.Blocks['logic_check_json'] = {
    init: function () {
        const properties = [
            ["Json", "json"],
            ['Length', 'length'],
            ['Key', 'key']
        ]

        const propertiesMapping = {
            json: Logic.boolProperties,
            length: Logic.numberProperties,
            key: [
                ['Has', 'has'],
                ['Not Has', 'not_has']
            ]
        };

        this.appendValueInput("json")
            .setCheck(Connections.DataJson)
            .appendField("json:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.boolProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("", 5, 5));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(210);
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

Blockly.Blocks['logic_check_response'] = {
    init: function () {
        const properties = [
            ["Response", "response"],
            ['Status Code', 'status_code'],
            ['Body Length', 'body_length'],
            ['Body Type', 'body_type'],
            ['Headers', 'headers']
        ]

        const propertiesMapping = {
            response: [
                ['Exists', 'exists'],
                ['Is Success', 'is_success'],
                ['Has Body', 'has_body'],
            ],
            status_code: Logic.numberProperties,
            body_length: Logic.numberProperties,
            body_type: Logic.stringProperties,
            headers: [
                ['Contains', 'contains'],
                ['Not Contains', 'not_contains']
            ]
        };

        this.appendValueInput("response")
            .setCheck(Connections.DataResponse)
            .appendField("response:");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField("property:")
            .appendField(new Blockly.FieldDropdown(properties), "property");
        this.appendDummyInput()
            .appendField(new Blockly.FieldImage("", 5, 5))
            .appendField(new FieldDependentDropdown("property", propertiesMapping, Logic.boolProperties), "compare");
        this.appendValueInput("value")
            .setCheck(["Boolean", "Number", "String", "variables_get"])
            .appendField(new Blockly.FieldImage("", 5, 5));
        this.setInputsInline(true);
        this.setOutput(true, null);
        this.setColour(210);
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