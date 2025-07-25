﻿/* eslint-disable */

; (function (root, factory) {

    if (typeof define === 'function' && define.amd) { // AMD

        define([], factory);

    } else if (typeof exports === 'object') { // Node.js

        module.exports = factory();

    } else { // Browser

        var messages = factory();

        for (var key in messages) {

            root.Blockly.Msg[key] = messages[key];

        }

    }

}(this, function () {

    // This file was automatically generated.  Do not modify.


    'use strict';


    var Blockly = Blockly || { Msg: Object.create(null) };


    Blockly.Msg["ABORT_MOVE"] = "Abort move";

    Blockly.Msg["ADD_COMMENT"] = "Add Comment";

    Blockly.Msg["ALT_KEY"] = "Alt";

    Blockly.Msg["CANNOT_DELETE_VARIABLE_PROCEDURE"] = "Can't delete the variable '%1' because it's part of the definition of the function '%2'";

    Blockly.Msg["CHANGE_VALUE_TITLE"] = "Change value:";

    Blockly.Msg["CHROME_OS"] = "ChromeOS";

    Blockly.Msg["CLEAN_UP"] = "Clean up Blocks";

    Blockly.Msg["CLOSE"] = "Close";

    Blockly.Msg["COLLAPSED_WARNINGS_WARNING"] = "Collapsed blocks contain warnings.";

    Blockly.Msg["COLLAPSE_ALL"] = "Collapse Blocks";

    Blockly.Msg["COLLAPSE_BLOCK"] = "Collapse Block";

    Blockly.Msg["COLOUR_BLEND_COLOUR1"] = "colour 1";

    Blockly.Msg["COLOUR_BLEND_COLOUR2"] = "colour 2";

    Blockly.Msg["COLOUR_BLEND_HELPURL"] = "https://meyerweb.com/eric/tools/color-blend/#:::rgbp";

    Blockly.Msg["COLOUR_BLEND_RATIO"] = "ratio";

    Blockly.Msg["COLOUR_BLEND_TITLE"] = "blend";

    Blockly.Msg["COLOUR_BLEND_TOOLTIP"] = "Blends two colours together with a given ratio (0.0 - 1.0).";

    Blockly.Msg["COLOUR_PICKER_HELPURL"] = "https://en.wikipedia.org/wiki/Color";

    Blockly.Msg["COLOUR_PICKER_TOOLTIP"] = "Choose a colour from the palette.";

    Blockly.Msg["COLOUR_RANDOM_HELPURL"] = "http://randomcolour.com";

    Blockly.Msg["COLOUR_RANDOM_TITLE"] = "random colour";

    Blockly.Msg["COLOUR_RANDOM_TOOLTIP"] = "Choose a colour at random.";

    Blockly.Msg["COLOUR_RGB_BLUE"] = "blue";

    Blockly.Msg["COLOUR_RGB_GREEN"] = "green";

    Blockly.Msg["COLOUR_RGB_HELPURL"] = "https://www.december.com/html/spec/colorpercompact.html";

    Blockly.Msg["COLOUR_RGB_RED"] = "red";

    Blockly.Msg["COLOUR_RGB_TITLE"] = "colour with";

    Blockly.Msg["COLOUR_RGB_TOOLTIP"] = "Create a colour with the specified amount of red, green, and blue. All values must be between 0 and 100.";

    Blockly.Msg["COMMAND_KEY"] = "⌘ Command";

    Blockly.Msg["CONTROLS_FLOW_STATEMENTS_HELPURL"] = "https://github.com/google/blockly/wiki/Loops#loop-termination-blocks";

    Blockly.Msg["CONTROLS_FLOW_STATEMENTS_OPERATOR_BREAK"] = "break out of loop";

    Blockly.Msg["CONTROLS_FLOW_STATEMENTS_OPERATOR_CONTINUE"] = "continue with next iteration of loop";

    Blockly.Msg["CONTROLS_FLOW_STATEMENTS_TOOLTIP_BREAK"] = "Break out of the containing loop.";

    Blockly.Msg["CONTROLS_FLOW_STATEMENTS_TOOLTIP_CONTINUE"] = "Skip the rest of this loop, and continue with the next iteration.";

    Blockly.Msg["CONTROLS_FLOW_STATEMENTS_WARNING"] = "Warning: This block may only be used within a loop.";

    Blockly.Msg["CONTROLS_FOREACH_HELPURL"] = "https://github.com/google/blockly/wiki/Loops#for-each";

    Blockly.Msg["CONTROLS_FOREACH_TITLE"] = "for each item %1 in list %2";

    Blockly.Msg["CONTROLS_FOREACH_TOOLTIP"] = "For each item in a list, set the variable '%1' to the item, and then do some statements.";

    Blockly.Msg["CONTROLS_FOR_HELPURL"] = "https://github.com/google/blockly/wiki/Loops#count-with";

    Blockly.Msg["CONTROLS_FOR_TITLE"] = "count with %1 from %2 to %3 by %4";

    Blockly.Msg["CONTROLS_FOR_TOOLTIP"] = "Have the variable '%1' take on the values from the start number to the end number, counting by the specified interval, and do the specified blocks.";

    Blockly.Msg["CONTROLS_IF_ELSEIF_TOOLTIP"] = "Add a condition to the if block.";

    Blockly.Msg["CONTROLS_IF_ELSE_TOOLTIP"] = "Add a final, catch-all condition to the if block.";

    Blockly.Msg["CONTROLS_IF_HELPURL"] = "https://github.com/google/blockly/wiki/IfElse";

    Blockly.Msg["CONTROLS_IF_IF_TOOLTIP"] = "Add, remove, or reorder sections to reconfigure this if block.";

    Blockly.Msg["CONTROLS_IF_MSG_ELSE"] = "else";

    Blockly.Msg["CONTROLS_IF_MSG_ELSEIF"] = "else if";

    Blockly.Msg["CONTROLS_IF_MSG_IF"] = "if";

    Blockly.Msg["CONTROLS_IF_TOOLTIP_1"] = "If a value is true, then do some statements.";

    Blockly.Msg["CONTROLS_IF_TOOLTIP_2"] = "If a value is true, then do the first block of statements. Otherwise, do the second block of statements.";

    Blockly.Msg["CONTROLS_IF_TOOLTIP_3"] = "If the first value is true, then do the first block of statements. Otherwise, if the second value is true, do the second block of statements.";

    Blockly.Msg["CONTROLS_IF_TOOLTIP_4"] = "If the first value is true, then do the first block of statements. Otherwise, if the second value is true, do the second block of statements. If none of the values are true, do the last block of statements.";

    Blockly.Msg["CONTROLS_REPEAT_HELPURL"] = "https://en.wikipedia.org/wiki/For_loop";

    Blockly.Msg["CONTROLS_REPEAT_INPUT_DO"] = "do";

    Blockly.Msg["CONTROLS_REPEAT_TITLE"] = "repeat %1 times";

    Blockly.Msg["CONTROLS_REPEAT_TOOLTIP"] = "Do some statements several times.";

    Blockly.Msg["CONTROLS_WHILEUNTIL_HELPURL"] = "https://github.com/google/blockly/wiki/Loops#repeat";

    Blockly.Msg["CONTROLS_WHILEUNTIL_OPERATOR_UNTIL"] = "repeat until";

    Blockly.Msg["CONTROLS_WHILEUNTIL_OPERATOR_WHILE"] = "repeat while";

    Blockly.Msg["CONTROLS_WHILEUNTIL_TOOLTIP_UNTIL"] = "While a value is false, then do some statements.";

    Blockly.Msg["CONTROLS_WHILEUNTIL_TOOLTIP_WHILE"] = "While a value is true, then do some statements.";

    Blockly.Msg["CONTROL_KEY"] = "Ctrl";

    Blockly.Msg["COPY_SHORTCUT"] = "Copy (%1)";

    Blockly.Msg["CUT_SHORTCUT"] = "Cut (%1)";

    Blockly.Msg["DELETE_ALL_BLOCKS"] = "Delete all %1 blocks?";

    Blockly.Msg["DELETE_BLOCK"] = "Delete Block";

    Blockly.Msg["DELETE_KEY"] = "Del";

    Blockly.Msg["DELETE_SHORTCUT"] = "Delete block (%1)";

    Blockly.Msg["DELETE_VARIABLE"] = "Delete the '%1' variable";

    Blockly.Msg["DELETE_VARIABLE_CONFIRMATION"] = "Delete %1 uses of the '%2' variable?";

    Blockly.Msg["DELETE_X_BLOCKS"] = "Delete %1 Blocks";

    Blockly.Msg["DIALOG_CANCEL"] = "Cancel";

    Blockly.Msg["DIALOG_OK"] = "OK";

    Blockly.Msg["DISABLE_BLOCK"] = "Disable Block";

    Blockly.Msg["DUPLICATE_BLOCK"] = "Duplicate";

    Blockly.Msg["DUPLICATE_COMMENT"] = "Duplicate Comment";

    Blockly.Msg["EDIT_BLOCK_CONTENTS"] = "Edit Block contents (%1)";

    Blockly.Msg["ENABLE_BLOCK"] = "Enable Block";

    Blockly.Msg["EXPAND_ALL"] = "Expand Blocks";

    Blockly.Msg["EXPAND_BLOCK"] = "Expand Block";

    Blockly.Msg["EXTERNAL_INPUTS"] = "External Inputs";

    Blockly.Msg["FINISH_MOVE"] = "Finish move";

    Blockly.Msg["HELP"] = "Help";

    Blockly.Msg["HELP_PROMPT"] = "Press %1 for help on keyboard controls";

    Blockly.Msg["INLINE_INPUTS"] = "Inline Inputs";

    Blockly.Msg["INSERT_BLOCK"] = "Insert Block (%1)";

    Blockly.Msg["LINUX"] = "Linux";

    Blockly.Msg["LISTS_CREATE_EMPTY_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#create-empty-list";

    Blockly.Msg["LISTS_CREATE_EMPTY_TITLE"] = "create empty list";

    Blockly.Msg["LISTS_CREATE_EMPTY_TOOLTIP"] = "Returns a list, of length 0, containing no data records";

    Blockly.Msg["LISTS_CREATE_WITH_CONTAINER_TITLE_ADD"] = "list";

    Blockly.Msg["LISTS_CREATE_WITH_CONTAINER_TOOLTIP"] = "Add, remove, or reorder sections to reconfigure this list block.";

    Blockly.Msg["LISTS_CREATE_WITH_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#create-list-with";

    Blockly.Msg["LISTS_CREATE_WITH_INPUT_WITH"] = "create list with";

    Blockly.Msg["LISTS_CREATE_WITH_ITEM_TOOLTIP"] = "Add an item to the list.";

    Blockly.Msg["LISTS_CREATE_WITH_TOOLTIP"] = "Create a list with any number of items.";

    Blockly.Msg["LISTS_GET_INDEX_FIRST"] = "first";

    Blockly.Msg["LISTS_GET_INDEX_FROM_END"] = "# from end";

    Blockly.Msg["LISTS_GET_INDEX_FROM_START"] = "#";

    Blockly.Msg["LISTS_GET_INDEX_GET"] = "get";

    Blockly.Msg["LISTS_GET_INDEX_GET_REMOVE"] = "get and remove";

    Blockly.Msg["LISTS_GET_INDEX_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#getting-items-from-a-list";

    Blockly.Msg["LISTS_GET_INDEX_LAST"] = "last";

    Blockly.Msg["LISTS_GET_INDEX_RANDOM"] = "random";

    Blockly.Msg["LISTS_GET_INDEX_REMOVE"] = "remove";

    Blockly.Msg["LISTS_GET_INDEX_TAIL"] = "";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_FIRST"] = "Returns the first item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_FROM"] = "Returns the item at the specified position in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_LAST"] = "Returns the last item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_RANDOM"] = "Returns a random item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_FIRST"] = "Removes and returns the first item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_FROM"] = "Removes and returns the item at the specified position in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_LAST"] = "Removes and returns the last item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_GET_REMOVE_RANDOM"] = "Removes and returns a random item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_REMOVE_FIRST"] = "Removes the first item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_REMOVE_FROM"] = "Removes the item at the specified position in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_REMOVE_LAST"] = "Removes the last item in a list.";

    Blockly.Msg["LISTS_GET_INDEX_TOOLTIP_REMOVE_RANDOM"] = "Removes a random item in a list.";

    Blockly.Msg["LISTS_GET_SUBLIST_END_FROM_END"] = "to # from end";

    Blockly.Msg["LISTS_GET_SUBLIST_END_FROM_START"] = "to #";

    Blockly.Msg["LISTS_GET_SUBLIST_END_LAST"] = "to last";

    Blockly.Msg["LISTS_GET_SUBLIST_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#getting-a-sublist";

    Blockly.Msg["LISTS_GET_SUBLIST_START_FIRST"] = "get sub-list from first";

    Blockly.Msg["LISTS_GET_SUBLIST_START_FROM_END"] = "get sub-list from # from end";

    Blockly.Msg["LISTS_GET_SUBLIST_START_FROM_START"] = "get sub-list from #";

    Blockly.Msg["LISTS_GET_SUBLIST_TAIL"] = "";

    Blockly.Msg["LISTS_GET_SUBLIST_TOOLTIP"] = "Creates a copy of the specified portion of a list.";

    Blockly.Msg["LISTS_INDEX_FROM_END_TOOLTIP"] = "%1 is the last item.";

    Blockly.Msg["LISTS_INDEX_FROM_START_TOOLTIP"] = "%1 is the first item.";

    Blockly.Msg["LISTS_INDEX_OF_FIRST"] = "find first occurrence of item";

    Blockly.Msg["LISTS_INDEX_OF_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#finding-items-in-a-list";

    Blockly.Msg["LISTS_INDEX_OF_LAST"] = "find last occurrence of item";

    Blockly.Msg["LISTS_INDEX_OF_TOOLTIP"] = "Returns the index of the first/last occurrence of the item in the list. Returns %1 if item is not found.";

    Blockly.Msg["LISTS_INLIST"] = "in list";

    Blockly.Msg["LISTS_ISEMPTY_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#is-empty";

    Blockly.Msg["LISTS_ISEMPTY_TITLE"] = "%1 is empty";

    Blockly.Msg["LISTS_ISEMPTY_TOOLTIP"] = "Returns true if the list is empty.";

    Blockly.Msg["LISTS_LENGTH_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#length-of";

    Blockly.Msg["LISTS_LENGTH_TITLE"] = "length of %1";

    Blockly.Msg["LISTS_LENGTH_TOOLTIP"] = "Returns the length of a list.";

    Blockly.Msg["LISTS_REPEAT_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#create-list-with";

    Blockly.Msg["LISTS_REPEAT_TITLE"] = "create list with item %1 repeated %2 times";

    Blockly.Msg["LISTS_REPEAT_TOOLTIP"] = "Creates a list consisting of the given value repeated the specified number of times.";

    Blockly.Msg["LISTS_REVERSE_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#reversing-a-list";

    Blockly.Msg["LISTS_REVERSE_MESSAGE0"] = "reverse %1";

    Blockly.Msg["LISTS_REVERSE_TOOLTIP"] = "Reverse a copy of a list.";

    Blockly.Msg["LISTS_SET_INDEX_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#in-list--set";

    Blockly.Msg["LISTS_SET_INDEX_INPUT_TO"] = "as";

    Blockly.Msg["LISTS_SET_INDEX_INSERT"] = "insert at";

    Blockly.Msg["LISTS_SET_INDEX_SET"] = "set";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_INSERT_FIRST"] = "Inserts the item at the start of a list.";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_INSERT_FROM"] = "Inserts the item at the specified position in a list.";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_INSERT_LAST"] = "Append the item to the end of a list.";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_INSERT_RANDOM"] = "Inserts the item randomly in a list.";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_SET_FIRST"] = "Sets the first item in a list.";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_SET_FROM"] = "Sets the item at the specified position in a list.";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_SET_LAST"] = "Sets the last item in a list.";

    Blockly.Msg["LISTS_SET_INDEX_TOOLTIP_SET_RANDOM"] = "Sets a random item in a list.";

    Blockly.Msg["LISTS_SORT_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#sorting-a-list";

    Blockly.Msg["LISTS_SORT_ORDER_ASCENDING"] = "ascending";

    Blockly.Msg["LISTS_SORT_ORDER_DESCENDING"] = "descending";

    Blockly.Msg["LISTS_SORT_TITLE"] = "sort %1 %2 %3";

    Blockly.Msg["LISTS_SORT_TOOLTIP"] = "Sort a copy of a list.";

    Blockly.Msg["LISTS_SORT_TYPE_IGNORECASE"] = "alphabetic, ignore case";

    Blockly.Msg["LISTS_SORT_TYPE_NUMERIC"] = "numeric";

    Blockly.Msg["LISTS_SORT_TYPE_TEXT"] = "alphabetic";

    Blockly.Msg["LISTS_SPLIT_HELPURL"] = "https://github.com/google/blockly/wiki/Lists#splitting-strings-and-joining-lists";

    Blockly.Msg["LISTS_SPLIT_LIST_FROM_TEXT"] = "make list from text";

    Blockly.Msg["LISTS_SPLIT_TEXT_FROM_LIST"] = "make text from list";

    Blockly.Msg["LISTS_SPLIT_TOOLTIP_JOIN"] = "Join a list of texts into one text, separated by a delimiter.";

    Blockly.Msg["LISTS_SPLIT_TOOLTIP_SPLIT"] = "Split text into a list of texts, breaking at each delimiter.";

    Blockly.Msg["LISTS_SPLIT_WITH_DELIMITER"] = "with delimiter";

    Blockly.Msg["LOGIC_BOOLEAN_FALSE"] = "false";

    Blockly.Msg["LOGIC_BOOLEAN_HELPURL"] = "https://github.com/google/blockly/wiki/Logic#values";

    Blockly.Msg["LOGIC_BOOLEAN_TOOLTIP"] = "Returns either true or false.";

    Blockly.Msg["LOGIC_BOOLEAN_TRUE"] = "true";

    Blockly.Msg["LOGIC_COMPARE_HELPURL"] = "https://en.wikipedia.org/wiki/Inequality_(mathematics)";

    Blockly.Msg["LOGIC_COMPARE_TOOLTIP_EQ"] = "Return true if both inputs equal each other.";

    Blockly.Msg["LOGIC_COMPARE_TOOLTIP_GT"] = "Return true if the first input is greater than the second input.";

    Blockly.Msg["LOGIC_COMPARE_TOOLTIP_GTE"] = "Return true if the first input is greater than or equal to the second input.";

    Blockly.Msg["LOGIC_COMPARE_TOOLTIP_LT"] = "Return true if the first input is smaller than the second input.";

    Blockly.Msg["LOGIC_COMPARE_TOOLTIP_LTE"] = "Return true if the first input is smaller than or equal to the second input.";

    Blockly.Msg["LOGIC_COMPARE_TOOLTIP_NEQ"] = "Return true if both inputs are not equal to each other.";

    Blockly.Msg["LOGIC_NEGATE_HELPURL"] = "https://github.com/google/blockly/wiki/Logic#not";

    Blockly.Msg["LOGIC_NEGATE_TITLE"] = "not %1";

    Blockly.Msg["LOGIC_NEGATE_TOOLTIP"] = "Returns true if the input is false. Returns false if the input is true.";

    Blockly.Msg["LOGIC_NULL"] = "null";

    Blockly.Msg["LOGIC_NULL_HELPURL"] = "https://en.wikipedia.org/wiki/Nullable_type";

    Blockly.Msg["LOGIC_NULL_TOOLTIP"] = "Returns null.";

    Blockly.Msg["LOGIC_OPERATION_AND"] = "and";

    Blockly.Msg["LOGIC_OPERATION_HELPURL"] = "https://github.com/google/blockly/wiki/Logic#logical-operations";

    Blockly.Msg["LOGIC_OPERATION_OR"] = "or";

    Blockly.Msg["LOGIC_OPERATION_TOOLTIP_AND"] = "Return true if both inputs are true.";

    Blockly.Msg["LOGIC_OPERATION_TOOLTIP_OR"] = "Return true if at least one of the inputs is true.";

    Blockly.Msg["LOGIC_TERNARY_CONDITION"] = "test";

    Blockly.Msg["LOGIC_TERNARY_HELPURL"] = "https://en.wikipedia.org/wiki/%3F:";

    Blockly.Msg["LOGIC_TERNARY_IF_FALSE"] = "if false";

    Blockly.Msg["LOGIC_TERNARY_IF_TRUE"] = "if true";

    Blockly.Msg["LOGIC_TERNARY_TOOLTIP"] = "Check the condition in 'test'. If the condition is true, returns the 'if true' value; otherwise returns the 'if false' value.";

    Blockly.Msg["MAC_OS"] = "macOS";

    Blockly.Msg["MATH_ADDITION_SYMBOL"] = "+";

    Blockly.Msg["MATH_ARITHMETIC_HELPURL"] = "https://en.wikipedia.org/wiki/Arithmetic";

    Blockly.Msg["MATH_ARITHMETIC_TOOLTIP_ADD"] = "Return the sum of the two numbers.";

    Blockly.Msg["MATH_ARITHMETIC_TOOLTIP_DIVIDE"] = "Return the quotient of the two numbers.";

    Blockly.Msg["MATH_ARITHMETIC_TOOLTIP_MINUS"] = "Return the difference of the two numbers.";

    Blockly.Msg["MATH_ARITHMETIC_TOOLTIP_MULTIPLY"] = "Return the product of the two numbers.";

    Blockly.Msg["MATH_ARITHMETIC_TOOLTIP_POWER"] = "Return the first number raised to the power of the second number.";

    Blockly.Msg["MATH_ATAN2_HELPURL"] = "https://en.wikipedia.org/wiki/Atan2";

    Blockly.Msg["MATH_ATAN2_TITLE"] = "atan2 of X:%1 Y:%2";

    Blockly.Msg["MATH_ATAN2_TOOLTIP"] = "Return the arctangent of point (X, Y) in degrees from -180 to 180.";

    Blockly.Msg["MATH_CHANGE_HELPURL"] = "https://en.wikipedia.org/wiki/Programming_idiom#Incrementing_a_counter";

    Blockly.Msg["MATH_CHANGE_TITLE"] = "change %1 by %2";

    Blockly.Msg["MATH_CHANGE_TOOLTIP"] = "Add a number to variable '%1'.";

    Blockly.Msg["MATH_CONSTANT_HELPURL"] = "https://en.wikipedia.org/wiki/Mathematical_constant";

    Blockly.Msg["MATH_CONSTANT_TOOLTIP"] = "Return one of the common constants: π (3.141…), e (2.718…), φ (1.618…), sqrt(2) (1.414…), sqrt(½) (0.707…), or ∞ (infinity).";

    Blockly.Msg["MATH_CONSTRAIN_HELPURL"] = "https://en.wikipedia.org/wiki/Clamping_(graphics)";

    Blockly.Msg["MATH_CONSTRAIN_TITLE"] = "constrain %1 low %2 high %3";

    Blockly.Msg["MATH_CONSTRAIN_TOOLTIP"] = "Constrain a number to be between the specified limits (inclusive).";

    Blockly.Msg["MATH_DIVISION_SYMBOL"] = "÷";

    Blockly.Msg["MATH_IS_DIVISIBLE_BY"] = "is divisible by";

    Blockly.Msg["MATH_IS_EVEN"] = "is even";

    Blockly.Msg["MATH_IS_NEGATIVE"] = "is negative";

    Blockly.Msg["MATH_IS_ODD"] = "is odd";

    Blockly.Msg["MATH_IS_POSITIVE"] = "is positive";

    Blockly.Msg["MATH_IS_PRIME"] = "is prime";

    Blockly.Msg["MATH_IS_TOOLTIP"] = "Check if a number is an even, odd, prime, whole, positive, negative, or if it is divisible by certain number. Returns true or false.";

    Blockly.Msg["MATH_IS_WHOLE"] = "is whole";

    Blockly.Msg["MATH_MODULO_HELPURL"] = "https://en.wikipedia.org/wiki/Modulo_operation";

    Blockly.Msg["MATH_MODULO_TITLE"] = "remainder of %1 ÷ %2";

    Blockly.Msg["MATH_MODULO_TOOLTIP"] = "Return the remainder from dividing the two numbers.";

    Blockly.Msg["MATH_MULTIPLICATION_SYMBOL"] = "×";

    Blockly.Msg["MATH_NUMBER_HELPURL"] = "https://en.wikipedia.org/wiki/Number";

    Blockly.Msg["MATH_NUMBER_TOOLTIP"] = "A number.";

    Blockly.Msg["MATH_ONLIST_HELPURL"] = "";

    Blockly.Msg["MATH_ONLIST_OPERATOR_AVERAGE"] = "average of list";

    Blockly.Msg["MATH_ONLIST_OPERATOR_MAX"] = "max of list";

    Blockly.Msg["MATH_ONLIST_OPERATOR_MEDIAN"] = "median of list";

    Blockly.Msg["MATH_ONLIST_OPERATOR_MIN"] = "min of list";

    Blockly.Msg["MATH_ONLIST_OPERATOR_MODE"] = "modes of list";

    Blockly.Msg["MATH_ONLIST_OPERATOR_RANDOM"] = "random item of list";

    Blockly.Msg["MATH_ONLIST_OPERATOR_STD_DEV"] = "standard deviation of list";

    Blockly.Msg["MATH_ONLIST_OPERATOR_SUM"] = "sum of list";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_AVERAGE"] = "Return the average (arithmetic mean) of the numeric values in the list.";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_MAX"] = "Return the largest number in the list.";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_MEDIAN"] = "Return the median number in the list.";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_MIN"] = "Return the smallest number in the list.";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_MODE"] = "Return a list of the most common item(s) in the list.";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_RANDOM"] = "Return a random element from the list.";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_STD_DEV"] = "Return the standard deviation of the list.";

    Blockly.Msg["MATH_ONLIST_TOOLTIP_SUM"] = "Return the sum of all the numbers in the list.";

    Blockly.Msg["MATH_POWER_SYMBOL"] = "^";

    Blockly.Msg["MATH_RANDOM_FLOAT_HELPURL"] = "https://en.wikipedia.org/wiki/Random_number_generation";

    Blockly.Msg["MATH_RANDOM_FLOAT_TITLE_RANDOM"] = "random fraction";

    Blockly.Msg["MATH_RANDOM_FLOAT_TOOLTIP"] = "Return a random fraction between 0.0 (inclusive) and 1.0 (exclusive).";

    Blockly.Msg["MATH_RANDOM_INT_HELPURL"] = "https://en.wikipedia.org/wiki/Random_number_generation";

    Blockly.Msg["MATH_RANDOM_INT_TITLE"] = "random integer from %1 to %2";

    Blockly.Msg["MATH_RANDOM_INT_TOOLTIP"] = "Return a random integer between the two specified limits, inclusive.";

    Blockly.Msg["MATH_ROUND_HELPURL"] = "https://en.wikipedia.org/wiki/Rounding";

    Blockly.Msg["MATH_ROUND_OPERATOR_ROUND"] = "round";

    Blockly.Msg["MATH_ROUND_OPERATOR_ROUNDDOWN"] = "round down";

    Blockly.Msg["MATH_ROUND_OPERATOR_ROUNDUP"] = "round up";

    Blockly.Msg["MATH_ROUND_TOOLTIP"] = "Round a number up or down.";

    Blockly.Msg["MATH_SINGLE_HELPURL"] = "https://en.wikipedia.org/wiki/Square_root";

    Blockly.Msg["MATH_SINGLE_OP_ABSOLUTE"] = "absolute";

    Blockly.Msg["MATH_SINGLE_OP_ROOT"] = "square root";

    Blockly.Msg["MATH_SINGLE_TOOLTIP_ABS"] = "Return the absolute value of a number.";

    Blockly.Msg["MATH_SINGLE_TOOLTIP_EXP"] = "Return e to the power of a number.";

    Blockly.Msg["MATH_SINGLE_TOOLTIP_LN"] = "Return the natural logarithm of a number.";

    Blockly.Msg["MATH_SINGLE_TOOLTIP_LOG10"] = "Return the base 10 logarithm of a number.";

    Blockly.Msg["MATH_SINGLE_TOOLTIP_NEG"] = "Return the negation of a number.";

    Blockly.Msg["MATH_SINGLE_TOOLTIP_POW10"] = "Return 10 to the power of a number.";

    Blockly.Msg["MATH_SINGLE_TOOLTIP_ROOT"] = "Return the square root of a number.";

    Blockly.Msg["MATH_SUBTRACTION_SYMBOL"] = "-";

    Blockly.Msg["MATH_TRIG_ACOS"] = "acos";

    Blockly.Msg["MATH_TRIG_ASIN"] = "asin";

    Blockly.Msg["MATH_TRIG_ATAN"] = "atan";

    Blockly.Msg["MATH_TRIG_COS"] = "cos";

    Blockly.Msg["MATH_TRIG_HELPURL"] = "https://en.wikipedia.org/wiki/Trigonometric_functions";

    Blockly.Msg["MATH_TRIG_SIN"] = "sin";

    Blockly.Msg["MATH_TRIG_TAN"] = "tan";

    Blockly.Msg["MATH_TRIG_TOOLTIP_ACOS"] = "Return the arccosine of a number.";

    Blockly.Msg["MATH_TRIG_TOOLTIP_ASIN"] = "Return the arcsine of a number.";

    Blockly.Msg["MATH_TRIG_TOOLTIP_ATAN"] = "Return the arctangent of a number.";

    Blockly.Msg["MATH_TRIG_TOOLTIP_COS"] = "Return the cosine of a degree (not radian).";

    Blockly.Msg["MATH_TRIG_TOOLTIP_SIN"] = "Return the sine of a degree (not radian).";

    Blockly.Msg["MATH_TRIG_TOOLTIP_TAN"] = "Return the tangent of a degree (not radian).";

    Blockly.Msg["MOVE_BLOCK"] = "Move Block (%1)";

    Blockly.Msg["MOVE_DOWN_CONSTRAINED"] = "Move down constrained";

    Blockly.Msg["MOVE_DOWN_UNCONSTRAINED"] = "Move down, unconstrained";

    Blockly.Msg["MOVE_LEFT_CONSTRAINED"] = "Move left, constrained";

    Blockly.Msg["MOVE_LEFT_UNCONSTRAINED"] = "Move left, unconstrained";

    Blockly.Msg["MOVE_RIGHT_CONSTRAINED"] = "Move right constrained";

    Blockly.Msg["MOVE_RIGHT_UNCONSTRAINED"] = "Move right, unconstrained";

    Blockly.Msg["MOVE_UP_CONSTRAINED"] = "Move up, constrained";

    Blockly.Msg["MOVE_UP_UNCONSTRAINED"] = "Move up unconstrained";

    Blockly.Msg["NEW_COLOUR_VARIABLE"] = "Create colour variable...";

    Blockly.Msg["NEW_NUMBER_VARIABLE"] = "Create number variable...";

    Blockly.Msg["NEW_STRING_VARIABLE"] = "Create string variable...";

    Blockly.Msg["NEW_VARIABLE"] = "Create variable...";

    Blockly.Msg["NEW_VARIABLE_TITLE"] = "New variable name:";

    Blockly.Msg["NEW_VARIABLE_TYPE_TITLE"] = "New variable type:";

    Blockly.Msg["OPTION_KEY"] = "⌥ Option";

    Blockly.Msg["ORDINAL_NUMBER_SUFFIX"] = "";

    Blockly.Msg["PASTE_SHORTCUT"] = "Paste (%1)";

    Blockly.Msg["PROCEDURES_ALLOW_STATEMENTS"] = "allow statements";

    Blockly.Msg["PROCEDURES_BEFORE_PARAMS"] = "with:";

    Blockly.Msg["PROCEDURES_CALLNORETURN_HELPURL"] = "https://en.wikipedia.org/wiki/Subroutine";

    Blockly.Msg["PROCEDURES_CALLNORETURN_TOOLTIP"] = "Run the user-defined function '%1'.";

    Blockly.Msg["PROCEDURES_CALLRETURN_HELPURL"] = "https://en.wikipedia.org/wiki/Subroutine";

    Blockly.Msg["PROCEDURES_CALLRETURN_TOOLTIP"] = "Run the user-defined function '%1' and use its output.";

    Blockly.Msg["PROCEDURES_CALL_BEFORE_PARAMS"] = "with:";

    Blockly.Msg["PROCEDURES_CALL_DISABLED_DEF_WARNING"] = "Can't run the user-defined function '%1' because the definition block is disabled.";

    Blockly.Msg["PROCEDURES_CREATE_DO"] = "Create '%1'";

    Blockly.Msg["PROCEDURES_DEFNORETURN_COMMENT"] = "Describe this function...";

    Blockly.Msg["PROCEDURES_DEFNORETURN_DO"] = "";

    Blockly.Msg["PROCEDURES_DEFNORETURN_HELPURL"] = "https://en.wikipedia.org/wiki/Subroutine";

    Blockly.Msg["PROCEDURES_DEFNORETURN_PROCEDURE"] = "do something";

    Blockly.Msg["PROCEDURES_DEFNORETURN_TITLE"] = "to";

    Blockly.Msg["PROCEDURES_DEFNORETURN_TOOLTIP"] = "Creates a function with no output.";

    Blockly.Msg["PROCEDURES_DEFRETURN_HELPURL"] = "https://en.wikipedia.org/wiki/Subroutine";

    Blockly.Msg["PROCEDURES_DEFRETURN_RETURN"] = "return";

    Blockly.Msg["PROCEDURES_DEFRETURN_TOOLTIP"] = "Creates a function with an output.";

    Blockly.Msg["PROCEDURES_DEF_DUPLICATE_WARNING"] = "Warning: This function has duplicate parameters.";

    Blockly.Msg["PROCEDURES_HIGHLIGHT_DEF"] = "Highlight function definition";

    Blockly.Msg["PROCEDURES_IFRETURN_HELPURL"] = "https://c2.com/cgi/wiki?GuardClause";

    Blockly.Msg["PROCEDURES_IFRETURN_TOOLTIP"] = "If a value is true, then return a second value.";

    Blockly.Msg["PROCEDURES_IFRETURN_WARNING"] = "Warning: This block may be used only within a function definition.";

    Blockly.Msg["PROCEDURES_MUTATORARG_TITLE"] = "input name:";

    Blockly.Msg["PROCEDURES_MUTATORARG_TOOLTIP"] = "Add an input to the function.";

    Blockly.Msg["PROCEDURES_MUTATORCONTAINER_TITLE"] = "inputs";

    Blockly.Msg["PROCEDURES_MUTATORCONTAINER_TOOLTIP"] = "Add, remove, or reorder inputs to this function.";

    Blockly.Msg["REDO"] = "Redo";

    Blockly.Msg["REMOVE_COMMENT"] = "Remove Comment";

    Blockly.Msg["RENAME_VARIABLE"] = "Rename variable...";

    Blockly.Msg["RENAME_VARIABLE_TITLE"] = "Rename all '%1' variables to:";

    Blockly.Msg["SHORTCUTS_CODE_NAVIGATION"] = "Code navigation";

    Blockly.Msg["SHORTCUTS_EDITING"] = "Editing";

    Blockly.Msg["SHORTCUTS_GENERAL"] = "General";

    Blockly.Msg["START_MOVE"] = "Start move";

    Blockly.Msg["TEXT_APPEND_HELPURL"] = "https://github.com/google/blockly/wiki/Text#text-modification";

    Blockly.Msg["TEXT_APPEND_TITLE"] = "to %1 append text %2";

    Blockly.Msg["TEXT_APPEND_TOOLTIP"] = "Append some text to variable '%1'.";

    Blockly.Msg["TEXT_CHANGECASE_HELPURL"] = "https://github.com/google/blockly/wiki/Text#adjusting-text-case";

    Blockly.Msg["TEXT_CHANGECASE_OPERATOR_LOWERCASE"] = "to lower case";

    Blockly.Msg["TEXT_CHANGECASE_OPERATOR_TITLECASE"] = "to Title Case";

    Blockly.Msg["TEXT_CHANGECASE_OPERATOR_UPPERCASE"] = "to UPPER CASE";

    Blockly.Msg["TEXT_CHANGECASE_TOOLTIP"] = "Return a copy of the text in a different case.";

    Blockly.Msg["TEXT_CHARAT_FIRST"] = "get first letter";

    Blockly.Msg["TEXT_CHARAT_FROM_END"] = "get letter # from end";

    Blockly.Msg["TEXT_CHARAT_FROM_START"] = "get letter #";

    Blockly.Msg["TEXT_CHARAT_HELPURL"] = "https://github.com/google/blockly/wiki/Text#extracting-text";

    Blockly.Msg["TEXT_CHARAT_LAST"] = "get last letter";

    Blockly.Msg["TEXT_CHARAT_RANDOM"] = "get random letter";

    Blockly.Msg["TEXT_CHARAT_TAIL"] = "";

    Blockly.Msg["TEXT_CHARAT_TITLE"] = "in text %1 %2";

    Blockly.Msg["TEXT_CHARAT_TOOLTIP"] = "Returns the letter at the specified position.";

    Blockly.Msg["TEXT_COUNT_HELPURL"] = "https://github.com/google/blockly/wiki/Text#counting-substrings";

    Blockly.Msg["TEXT_COUNT_MESSAGE0"] = "count %1 in %2";

    Blockly.Msg["TEXT_COUNT_TOOLTIP"] = "Count how many times some text occurs within some other text.";

    Blockly.Msg["TEXT_CREATE_JOIN_ITEM_TOOLTIP"] = "Add an item to the text.";

    Blockly.Msg["TEXT_CREATE_JOIN_TITLE_JOIN"] = "join";

    Blockly.Msg["TEXT_CREATE_JOIN_TOOLTIP"] = "Add, remove, or reorder sections to reconfigure this text block.";

    Blockly.Msg["TEXT_GET_SUBSTRING_END_FROM_END"] = "to letter # from end";

    Blockly.Msg["TEXT_GET_SUBSTRING_END_FROM_START"] = "to letter #";

    Blockly.Msg["TEXT_GET_SUBSTRING_END_LAST"] = "to last letter";

    Blockly.Msg["TEXT_GET_SUBSTRING_HELPURL"] = "https://github.com/google/blockly/wiki/Text#extracting-a-region-of-text";

    Blockly.Msg["TEXT_GET_SUBSTRING_INPUT_IN_TEXT"] = "in text";

    Blockly.Msg["TEXT_GET_SUBSTRING_START_FIRST"] = "get substring from first letter";

    Blockly.Msg["TEXT_GET_SUBSTRING_START_FROM_END"] = "get substring from letter # from end";

    Blockly.Msg["TEXT_GET_SUBSTRING_START_FROM_START"] = "get substring from letter #";

    Blockly.Msg["TEXT_GET_SUBSTRING_TAIL"] = "";

    Blockly.Msg["TEXT_GET_SUBSTRING_TOOLTIP"] = "Returns a specified portion of the text.";

    Blockly.Msg["TEXT_INDEXOF_HELPURL"] = "https://github.com/google/blockly/wiki/Text#finding-text";

    Blockly.Msg["TEXT_INDEXOF_OPERATOR_FIRST"] = "find first occurrence of text";

    Blockly.Msg["TEXT_INDEXOF_OPERATOR_LAST"] = "find last occurrence of text";

    Blockly.Msg["TEXT_INDEXOF_TITLE"] = "in text %1 %2 %3";

    Blockly.Msg["TEXT_INDEXOF_TOOLTIP"] = "Returns the index of the first/last occurrence of the first text in the second text. Returns %1 if text is not found.";

    Blockly.Msg["TEXT_ISEMPTY_HELPURL"] = "https://github.com/google/blockly/wiki/Text#checking-for-empty-text";

    Blockly.Msg["TEXT_ISEMPTY_TITLE"] = "%1 is empty";

    Blockly.Msg["TEXT_ISEMPTY_TOOLTIP"] = "Returns true if the provided text is empty.";

    Blockly.Msg["TEXT_JOIN_HELPURL"] = "https://github.com/google/blockly/wiki/Text#text-creation";

    Blockly.Msg["TEXT_JOIN_TITLE_CREATEWITH"] = "create text with";

    Blockly.Msg["TEXT_JOIN_TOOLTIP"] = "Create a piece of text by joining together any number of items.";

    Blockly.Msg["TEXT_LENGTH_HELPURL"] = "https://github.com/google/blockly/wiki/Text#text-modification";

    Blockly.Msg["TEXT_LENGTH_TITLE"] = "length of %1";

    Blockly.Msg["TEXT_LENGTH_TOOLTIP"] = "Returns the number of letters (including spaces) in the provided text.";

    Blockly.Msg["TEXT_PRINT_HELPURL"] = "https://github.com/google/blockly/wiki/Text#printing-text";

    Blockly.Msg["TEXT_PRINT_TITLE"] = "print %1";

    Blockly.Msg["TEXT_PRINT_TOOLTIP"] = "Print the specified text, number or other value.";

    Blockly.Msg["TEXT_PROMPT_HELPURL"] = "https://github.com/google/blockly/wiki/Text#getting-input-from-the-user";

    Blockly.Msg["TEXT_PROMPT_TOOLTIP_NUMBER"] = "Prompt for user for a number.";

    Blockly.Msg["TEXT_PROMPT_TOOLTIP_TEXT"] = "Prompt for user for some text.";

    Blockly.Msg["TEXT_PROMPT_TYPE_NUMBER"] = "prompt for number with message";

    Blockly.Msg["TEXT_PROMPT_TYPE_TEXT"] = "prompt for text with message";

    Blockly.Msg["TEXT_REPLACE_HELPURL"] = "https://github.com/google/blockly/wiki/Text#replacing-substrings";

    Blockly.Msg["TEXT_REPLACE_MESSAGE0"] = "replace %1 with %2 in %3";

    Blockly.Msg["TEXT_REPLACE_TOOLTIP"] = "Replace all occurances of some text within some other text.";

    Blockly.Msg["TEXT_REVERSE_HELPURL"] = "https://github.com/google/blockly/wiki/Text#reversing-text";

    Blockly.Msg["TEXT_REVERSE_MESSAGE0"] = "reverse %1";

    Blockly.Msg["TEXT_REVERSE_TOOLTIP"] = "Reverses the order of the characters in the text.";

    Blockly.Msg["TEXT_TEXT_HELPURL"] = "https://en.wikipedia.org/wiki/String_(computer_science)";

    Blockly.Msg["TEXT_TEXT_TOOLTIP"] = "A letter, word, or line of text.";

    Blockly.Msg["TEXT_TRIM_HELPURL"] = "https://github.com/google/blockly/wiki/Text#trimming-removing-spaces";

    Blockly.Msg["TEXT_TRIM_OPERATOR_BOTH"] = "trim spaces from both sides of";

    Blockly.Msg["TEXT_TRIM_OPERATOR_LEFT"] = "trim spaces from left side of";

    Blockly.Msg["TEXT_TRIM_OPERATOR_RIGHT"] = "trim spaces from right side of";

    Blockly.Msg["TEXT_TRIM_TOOLTIP"] = "Return a copy of the text with spaces removed from one or both ends.";

    Blockly.Msg["TODAY"] = "Today";

    Blockly.Msg["UNDO"] = "Undo";

    Blockly.Msg["UNKNOWN"] = "Unknown";

    Blockly.Msg["UNNAMED_KEY"] = "unnamed";

    Blockly.Msg["VARIABLES_DEFAULT_NAME"] = "item";

    Blockly.Msg["VARIABLES_GET_CREATE_SET"] = "Create 'set %1'";

    Blockly.Msg["VARIABLES_GET_HELPURL"] = "https://github.com/google/blockly/wiki/Variables#get";

    Blockly.Msg["VARIABLES_GET_TOOLTIP"] = "Returns the value of this variable.";

    Blockly.Msg["VARIABLES_SET"] = "set %1 to %2";

    Blockly.Msg["VARIABLES_SET_CREATE_GET"] = "Create 'get %1'";

    Blockly.Msg["VARIABLES_SET_HELPURL"] = "https://github.com/google/blockly/wiki/Variables#set";

    Blockly.Msg["VARIABLES_SET_TOOLTIP"] = "Sets this variable to be equal to the input.";

    Blockly.Msg["VARIABLE_ALREADY_EXISTS"] = "A variable named '%1' already exists.";

    Blockly.Msg["VARIABLE_ALREADY_EXISTS_FOR_ANOTHER_TYPE"] = "A variable named '%1' already exists for another type: '%2'.";

    Blockly.Msg["VARIABLE_ALREADY_EXISTS_FOR_A_PARAMETER"] = "A variable named '%1' already exists as a parameter in the procedure '%2'.";

    Blockly.Msg["WINDOWS"] = "Windows";

    Blockly.Msg["WORKSPACE_ARIA_LABEL"] = "Blockly Workspace";

    Blockly.Msg["WORKSPACE_COMMENT_DEFAULT_TEXT"] = "Say something...";

    Blockly.Msg["CONTROLS_FOREACH_INPUT_DO"] = Blockly.Msg["CONTROLS_REPEAT_INPUT_DO"];

    Blockly.Msg["CONTROLS_FOR_INPUT_DO"] = Blockly.Msg["CONTROLS_REPEAT_INPUT_DO"];

    Blockly.Msg["CONTROLS_IF_ELSEIF_TITLE_ELSEIF"] = Blockly.Msg["CONTROLS_IF_MSG_ELSEIF"];

    Blockly.Msg["CONTROLS_IF_ELSE_TITLE_ELSE"] = Blockly.Msg["CONTROLS_IF_MSG_ELSE"];

    Blockly.Msg["CONTROLS_IF_IF_TITLE_IF"] = Blockly.Msg["CONTROLS_IF_MSG_IF"];

    Blockly.Msg["CONTROLS_IF_MSG_THEN"] = Blockly.Msg["CONTROLS_REPEAT_INPUT_DO"];

    Blockly.Msg["CONTROLS_WHILEUNTIL_INPUT_DO"] = Blockly.Msg["CONTROLS_REPEAT_INPUT_DO"];

    Blockly.Msg["LISTS_CREATE_WITH_ITEM_TITLE"] = Blockly.Msg["VARIABLES_DEFAULT_NAME"];

    Blockly.Msg["LISTS_GET_INDEX_HELPURL"] = Blockly.Msg["LISTS_INDEX_OF_HELPURL"];

    Blockly.Msg["LISTS_GET_INDEX_INPUT_IN_LIST"] = Blockly.Msg["LISTS_INLIST"];

    Blockly.Msg["LISTS_GET_SUBLIST_INPUT_IN_LIST"] = Blockly.Msg["LISTS_INLIST"];

    Blockly.Msg["LISTS_INDEX_OF_INPUT_IN_LIST"] = Blockly.Msg["LISTS_INLIST"];

    Blockly.Msg["LISTS_SET_INDEX_INPUT_IN_LIST"] = Blockly.Msg["LISTS_INLIST"];

    Blockly.Msg["MATH_CHANGE_TITLE_ITEM"] = Blockly.Msg["VARIABLES_DEFAULT_NAME"];

    Blockly.Msg["PROCEDURES_DEFRETURN_COMMENT"] = Blockly.Msg["PROCEDURES_DEFNORETURN_COMMENT"];

    Blockly.Msg["PROCEDURES_DEFRETURN_DO"] = Blockly.Msg["PROCEDURES_DEFNORETURN_DO"];

    Blockly.Msg["PROCEDURES_DEFRETURN_PROCEDURE"] = Blockly.Msg["PROCEDURES_DEFNORETURN_PROCEDURE"];

    Blockly.Msg["PROCEDURES_DEFRETURN_TITLE"] = Blockly.Msg["PROCEDURES_DEFNORETURN_TITLE"];

    Blockly.Msg["TEXT_APPEND_VARIABLE"] = Blockly.Msg["VARIABLES_DEFAULT_NAME"];

    Blockly.Msg["TEXT_CREATE_JOIN_ITEM_TITLE_ITEM"] = Blockly.Msg["VARIABLES_DEFAULT_NAME"];


    Blockly.Msg["MATH_HUE"] = "230";

    Blockly.Msg["LOOPS_HUE"] = "120";

    Blockly.Msg["LISTS_HUE"] = "260";

    Blockly.Msg["LOGIC_HUE"] = "210";

    Blockly.Msg["VARIABLES_HUE"] = "330";

    Blockly.Msg["TEXTS_HUE"] = "160";

    Blockly.Msg["PROCEDURES_HUE"] = "290";

    Blockly.Msg["COLOUR_HUE"] = "20";

    Blockly.Msg["VARIABLES_DYNAMIC_HUE"] = "310";

    return Blockly.Msg;

}));