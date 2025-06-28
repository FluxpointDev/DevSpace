registerFieldMultilineInput();

Blockly.Blocks['text_multiline'] = {
    init: function () {
        this.setOutput(true, ['text_multiline', 'String']);
        this.setColour(120);
        this.setTooltip("");
        this.setHelpUrl("https://docs.fluxpoint.dev/nova/workspace/basic-blocks");
        this.appendDummyInput()
            .appendField(
                new FieldMultilineInput(''),
                'TEXT',
            );
    },
};