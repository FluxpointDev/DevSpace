Blockly.Blocks['obj_components_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_component_row"];
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Components List");
        this.appendValueInput("item")
            .setCheck("obj_component_row")
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("row:");
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};

Blockly.Blocks['obj_component_row'] = {
    init: function () {
        this.itemCheck_ = ["obj_component_button", "obj_component_select", "obj_component_button_link"];
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Component Row");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};

Blockly.Blocks['obj_component_button'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Interaction Button");
        this.appendValueInput("id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("id:");
        this.appendValueInput("label")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("label:");
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("style:")
            .appendField(new Blockly.FieldDropdown([
                ["Primary", "primary"],
                ["Secondary", "secondary"],
                ["Success", "success"],
                ["Link", "link"],
                ["Danger", "danger"]
            ]), "style_type");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("is_disabled")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is disabled:");
        this.appendValueInput("data")
            .setCheck(["data_message_*", "data_channel_*", "data_category_*",
                "data_server_*", "data_role_*", "data_member_*", "data_user_*"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['obj_component_modal_input_text'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Modal Text Field");
        this.appendValueInput("id")
            .setCheck('text')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("id:");
        this.appendValueInput("label")
            .setCheck('text')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("label:");
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("style:")
            .appendField(new Blockly.FieldDropdown([
                ["Short", "short"],
                ["Paragraph", "paragraph"]
            ]), "input_type");
        this.appendValueInput("min_length")
            .setCheck('math_number')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("min length:");
        this.appendValueInput("max_length")
            .setCheck('math_number')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("max length:");
        this.appendValueInput("is_required")
            .setCheck('logic_boolean')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is required:");
        this.appendValueInput("default_value")
            .setCheck(['text', 'text_multiline'])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("default value:");
        this.appendValueInput("placeholder")
            .setCheck('text')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("placeholder:");
        this.appendValueInput("output_text")
            .setCheck('variables_get')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("output text:");
        this.setInputsInline(false);
        this.setPreviousStatement(true, null);
        this.setNextStatement(true, null);
        this.setColour('#9e7d49');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['obj_component_button_link'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Link Button");
        this.appendValueInput("label")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("label:");
        this.appendValueInput("url")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("url:");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("is_disabled")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is disabled:");
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['obj_component_input_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_component_modal_input_text"];
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Modal Inputs List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};

Blockly.Blocks['obj_component_select'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Interaction Select Menu");
        this.appendValueInput("id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("id:");
        this.appendValueInput("obj_component_select_list")
            .setCheck('obj_component_select_list')
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("options list:");
        this.appendValueInput("placeholder")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("placeholder:");
        this.appendValueInput("is_disabled")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is disabled:");
        this.appendValueInput("min_value")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("min value:");
        this.appendValueInput("max_value")
            .setCheck(Connections.Number)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("min value:");
        this.appendValueInput("data")
            .setCheck(["data_message_*", "data_channel_*", "data_category_*",
                "data_server_*", "data_role_*", "data_member_*", "data_user_*"])
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("data:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};

Blockly.Blocks['obj_component_select_list'] = {
    init: function () {
        this.itemCheck_ = ["obj_component_select_item"];
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Options List");
        this.appendValueInput("item")
            .setCheck(this.itemCheck_);
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
        Blockly.Extensions.apply('dynamic_list_mutator', this, true);
    }
};

Blockly.Blocks['obj_component_select_item'] = {
    init: function () {
        this.appendDummyInput()
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("Option Item");
        this.appendValueInput("id")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("id:");
        this.appendValueInput("label")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("label:");
        this.appendValueInput("description")
            .setCheck(Connections.TextSingle)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("description:");
        this.appendValueInput("emoji")
            .setCheck(Connections.DataEmojis)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("emoji:");
        this.appendValueInput("is_default")
            .setCheck(Connections.Bool)
            .setAlign(Blockly.inputs.Align.RIGHT)
            .appendField("is default:");
        this.setInputsInline(false);
        this.setOutput(true, null);
        this.setColour('#146a90');
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/devspace");
    }
};