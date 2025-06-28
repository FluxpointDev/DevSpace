class CustomConnectionManager extends Blockly.ConnectionChecker {
    constructor() {
        super();
        this.Debug = false;
    }

    /**
     * Type check arrays must either intersect or both be null.
     * @override
     */
    doTypeChecks(a, b) {
        let checkArrayOne = a.getCheck();
        let checkArrayTwo = b.getCheck();
        
        if (checkArrayOne) {
            if (a.sourceBlock_.type === 'text') {
                checkArrayOne = checkArrayOne.concat(['text']);
            }
            if (a.sourceBlock_.type === 'text_multiline') {
                checkArrayOne = checkArrayOne.concat(['text_multiline']);
            }
            
        }

        if (checkArrayTwo) {
            if (b.sourceBlock_.type === 'text') {
                checkArrayTwo = checkArrayTwo.concat(['text']);
            }
            if (b.sourceBlock_.type === 'text_multiline') {
                checkArrayTwo = checkArrayTwo.concat(['text_multiline']);
            }
        }
        
        if (this.Debug) {
            console.log('Check type');
            console.log(' ');
            console.log("-------")
            console.log(a.sourceBlock_.type);
            console.log(a);
            console.log(checkArrayOne);
            console.log('>')
            console.log(b.sourceBlock_.type);
            console.log(b);
            console.log(checkArrayTwo);
            console.log("-------")
            console.log(' ');
        }
        

        // Both nulls can connect
        if (!checkArrayOne && !checkArrayTwo) {
            if (this.Debug) {
                console.log('>>> NULL CONNECTED');
            }
            return true;
        }

        // Check if inputs can connect
        if ((a.type === 2 || a.type === 4) && checkArrayTwo && (checkArrayTwo.indexOf(a.sourceBlock_.type) != -1 || checkArrayTwo.filter(w => w.endsWith('*') && a.sourceBlock_.type.startsWith(w.slice(0, -1))).length != 0)) {
            if (this.Debug) {
                console.log('>>> INPUT CONNECTED');
            }
            return true;
        }

        // Check if outputs can connect
        if ((a.type === 1 || a.type === 3) && checkArrayOne && (checkArrayOne.indexOf(b.sourceBlock_.type) != -1 || checkArrayOne.filter(w => w.endsWith('*') && b.sourceBlock_.type.startsWith(w.slice(0, -1))).length != 0)) {
            if (this.Debug) {
                console.log('>>> OUTPUT CONNECTED');
            }
            return true;
        }

        if (checkArrayOne && checkArrayTwo) {
            // Find any intersection in the check lists.
            for (let i = 0; i < checkArrayOne.length; i++) {
                if (checkArrayTwo.indexOf(checkArrayOne[i]) != -1) {
                    if (this.Debug) {
                        console.log('>>> ARRAY CONNECTED');
                    }
                    return true;
                }
            }
        }
        if (this.Debug) {
            console.log('>>> FALSE CONNECTED');
        }
        return false;
    }
}
Blockly.registry.register(
    Blockly.registry.Type.CONNECTION_CHECKER,
    'CustomConnectionManager',
    CustomConnectionManager,
);


class TestClass {
    constructor() {
        Object.defineProperty(test, "1", {
            value: "1",
            writable: false,
            enumerable: true,
            configurable: true
        });
    }
    static test = "Hello"
}

class CustomFieldTextInput extends Blockly.FieldTextInput {
    constructor(value, validator) {
        super(value, validator);
    }
    setMaxLength(number) {
        this._maxLength = number;
    }
    setMinLength(number) {
        this._minLength = number;
    }
    showEditor_() {
        super.showEditor_();
        if (this._maxLength !== 0)
            this.htmlInput_.maxLength = this._maxLength;
    }
    fromJson(options) {
        const value = Blockly.utils.parsing.replaceMessageReferences(
            options['value']);
        return new CustomFields.CustomFieldTextInput(value);

    }
}
Blockly.fieldRegistry.register('field_textinput', CustomFieldTextInput);
