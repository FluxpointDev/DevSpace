const Connections = new ValidConnections();

var initControlsIf = Blockly.Blocks["controls_if"].init;

Blockly.Blocks['controls_if'].init = function () {
    initControlsIf.call(this);
    this.setColour(225);
    this.setHelpUrl("https://docs.fluxpoint.dev/devspace/apps");

    this.inputList.forEach(elm => {
        if (elm.type === 1) {
            elm.connection.setCheck(['logic_condition_and', 'logic_condition_or', 'logic_check_*']);
        }
        else if (elm.type === 3) {
            elm.connection.setCheck(Connections.ActionsList);
        }
    });

    this.nextConnection.setCheck(Connections.ActionsList);
    this.previousConnection.setCheck(Connections.ActionsList);

    var appendThis = this.updateShape_;
    
    this.updateShape_ = function () {
        appendThis.call(this);

        this.inputList.forEach(elm => {
            if (elm.type === 1) {
                elm.connection.setCheck(['logic_condition_and', 'logic_condition_or', 'logic_check_*']);
            }
            else if (elm.type === 3) {
                elm.connection.setCheck(Connections.ActionsList);
            }
        });
    }
};

var initText = Blockly.Blocks["text"].init;

Blockly.Blocks['text'].init = function () {
    initText.call(this);
    this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
    this.setOutput(true, ['text', 'String']);
};

var initBool = Blockly.Blocks["logic_boolean"].init;

Blockly.Blocks['logic_boolean'].init = function () {
    initBool.call(this);
    if (this.isInFlyout) {
        this.inputList[0].fieldRow[0].setValue("FALSE");
    }
    this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
    this.setOutput(true, ['Boolean', 'logic_boolean'])
};

var initVariableSet = Blockly.Blocks["variables_set"].init;

Blockly.Blocks['variables_set'].init = function () {
    initVariableSet.call(this);
    this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
    this.nextConnection.setCheck(Connections.ActionsList);
    this.previousConnection.setCheck(Connections.ActionsList);

    this.inputList[0].setCheck(
        ['text', 'text_multiline', 'data_string_*', 'data_permission_*', 'math_number',
            'logic_boolean', 'variables_get', 'data_message_*', 'data_file_*', 'data_emoji_*',
            'data_channel_*', 'data_category_*', 'data_webhook_*', 'data_server_*', 'data_role_*',
            'data_member_*', 'data_user_*', 'data_json_*', 'data_response_*']
    );
};

var initMathChange = Blockly.Blocks["math_change"].init;

Blockly.Blocks['math_change'].init = function () {
    initMathChange.call(this);
    this.setDisabledReason('Disabled');
    this.setTooltip('Not used');
    this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
};

var initMathNumber = Blockly.Blocks["math_number"].init;

Blockly.Blocks['math_number'].init = function () {
    initMathNumber.call(this);
    this.setOutput(true, ['math_number', 'Number']);
    this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
};

var initVariablesGet = Blockly.Blocks["variables_get"].init;

Blockly.Blocks['variables_get'].init = function () {
    initVariablesGet.call(this);
    this.setHelpUrl('https://docs.fluxpoint.dev/devspace');
};