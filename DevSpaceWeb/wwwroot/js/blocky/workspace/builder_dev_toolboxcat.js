Blockly.Blocks['blockly_toolbox_category'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Category");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Name:       ")
            .appendField(new Blockly.FieldTextInput(""), "name");
        this.appendDummyInput()
            .appendField("Color:        ")
            .appendField(new Blockly.FieldTextInput(""), "color");
        this.appendDummyInput()
            .appendField("Icon:          ")
            .appendField(new Blockly.FieldTextInput(""), "icon");
        this.appendDummyInput()
            .appendField("Expanded: ")
            .appendField(new Blockly.FieldCheckbox("FALSE"), "expanded");
        this.appendDummyInput();
        this.appendStatementInput("category")
            .setCheck(["blockly_toolbox_category", "blockly_toolbox_block", "blockly_toolbox_label", "blockly_toolbox_seperator"]);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['blockly_toolbox_label'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Label");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("text:       ")
            .appendField(new Blockly.FieldTextInput(""), "text");
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour(30);
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['blockly_toolbox_block'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Block");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Type:        ")
            .appendField(new Blockly.FieldTextInput(""), "type");
        this.appendDummyInput()
            .appendField("Editable:   ")
            .appendField(new Blockly.FieldCheckbox("TRUE"), "editable");
        this.appendDummyInput()
            .appendField("Movable:  ")
            .appendField(new Blockly.FieldCheckbox("TRUE"), "movable");
        this.appendDummyInput()
            .appendField("Deletable:")
            .appendField(new Blockly.FieldCheckbox("TRUE"), "deletable");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Inputs:");
        this.appendStatementInput("inputs")
            .setCheck("blockly_toolbox_input");
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour(135);
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['blockly_toolbox_seperator'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Seperator");
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour(30);
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['blockly_toolbox_input_block'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Block");
        this.appendDummyInput();
        this.appendDummyInput()
            .appendField("Type:        ")
            .appendField(new Blockly.FieldTextInput(""), "type");
        this.appendDummyInput()
            .appendField("Editable:   ")
            .appendField(new Blockly.FieldCheckbox("TRUE"), "editable");
        this.appendDummyInput()
            .appendField("Movable:  ")
            .appendField(new Blockly.FieldCheckbox("TRUE"), "movable");
        this.appendDummyInput()
            .appendField("Deletable:")
            .appendField(new Blockly.FieldCheckbox("TRUE"), "deletable");
        this.appendDummyInput()
            .appendField("Inputs:");
        this.appendStatementInput("inputs")
            .setCheck("blockly_toolbox_input");
        this.setOutput(true, null);
        this.setColour(135);
        this.setTooltip("");
        this.setHelpUrl("");
    }
};

Blockly.Blocks['blockly_toolbox_input'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Input");
        this.appendValueInput("NAME")
            .setCheck("blockly_toolbox_input_block")
            .appendField("Block:");
        this.appendDummyInput()
            .appendField("Name:")
            .appendField(new Blockly.FieldTextInput(""), "name");
        this.appendDummyInput()
            .appendField("Is Shadow:")
            .appendField(new Blockly.FieldCheckbox("FALSE"), "shadow");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour(230);
        this.setTooltip("");
        this.setHelpUrl("");
    }
};