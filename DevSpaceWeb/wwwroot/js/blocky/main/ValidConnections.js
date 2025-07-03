class ValidConnections {
    ActionsList = ["action_*", "variables_set", "controls_if", "block_try_catch"];
    OptionsList = ["option_*"];
    AllowedInputs = ["input_*"];
    TextSingle = ["text", "variables_get", "data_string_*", "logic_boolean", "math_number", "color_*", "data_selector_json"];
    TextAll = ["text", "text_multiline", "variables_get", "data_string_*", "logic_boolean", "math_number", "color_*", "data_selector_json"];
    Bool = ["logic_boolean", "variables_get", "data_selector_json"];
    Number = ["math_number", "variables_get", "data_selector_json"];

    DataJson = ["variables_get", "data_json_active"];
    OutputJson = ["variables_get", "data_json_active"];

    DataColors = ["color_*", "variables_get"];
    DataResponse = ['data_response_active', 'variables_get'];

    DataFiles = ["data_file_*", "variables_get"];
    OutputFiles = ["data_file_active", "variables_get"];
}