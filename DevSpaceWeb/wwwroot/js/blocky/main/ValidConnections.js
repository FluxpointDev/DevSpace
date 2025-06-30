class ValidConnections {
    ActionsList = ["action_*", "variables_set", "controls_if", "block_try_catch"];
    OptionsList = ["option_*"];
    AllowedInputs = ["input_*"];
    TextSingle = ["text", "variables_get", "data_string_*", "logic_boolean", "math_number", "color_hex", "color_rgb"];
    TextAll = ["text", "text_multiline", "variables_get", "data_string_*", "logic_boolean", "math_number", "color_hex", "color_hex"];
    Bool = ["logic_boolean", "variables_get"];
    Number = ["math_number", "variables_get"];

    DataJson = ["variables_get", "data_json_active"];
    OutputJson = ["variables_get", "data_json_active"];

    DataColors = ["color_rgb", "color_hex", "color_hex_picker", "variables_get"];
    DataResponse = ['data_response_active', 'variables_get'];

    DataFiles = ["data_file_*", "variables_get"];
    OutputFiles = ["data_file_active", "variables_get"];
}