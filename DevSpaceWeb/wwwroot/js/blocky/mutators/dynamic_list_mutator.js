//function getExtraBlockState(block) {
//    // TODO: This is a dupe of the BlockChange.getExtraBlockState code, do we
//    //    want to make that public?
//    if (block.saveExtraState) {
//        const state = block.saveExtraState()
//        return state ? JSON.stringify(state) : ''
//    } else if (block.mutationToDom) {
//        const state = block.mutationToDom()
//        return state ? Blockly.Xml.domToText(state) : ''
//    }
//    return ''
//}

const dynamicListMutator = {
    itemCount_: 1,
    onchange: function (e) {
        if (e.reason != null && Array.isArray(e.reason) && e.reason.includes("connect")) {
            if (this.allInputsAreFilled()) {
                this.appendArrayElementInput();
            }
        }
        else if ((e.reason != null && Array.isArray(e.reason) && e.reason.includes("disconnect")) || e.type === "delete") {
            var FirstInputEmpty = false;
            for (var i in this.inputList) {
                let Input = this.inputList[i];
                if (Input && Input.type === 1 && Input.connection !== 'undefined' && Input.connection != null && (Input.name === e.oldInputName || Input.connection.targetConnection === null)) {

                    if (FirstInputEmpty) {
                        this.deleteArrayElementInput(Input);
                    }
                    else {
                        FirstInputEmpty = true;
                    }
                }

            }
        }
    },

    allInputsAreFilled() {
        return this.inputList.findIndex(x => x.type === 1 && x.connection.targetConnection === null) === -1;
    },

    saveExtraState: function () {
        return {
            itemCount: this.itemCount_,
        }
    },

    loadExtraState: function (state) {
        this.itemCount_ = state['itemCount'];
        try {
            this.removeInput('item');
        }
        catch { }
        this.updateShape();
    },

    appendArrayElementInput: function () {
        //Blockly.Events.setGroup(true)
        //const oldExtraState = getExtraBlockState(this)
        this.itemCount_ += 1
        //const newExtraState = getExtraBlockState(this)

        //Blockly.Events.fire(new Blockly.Events.BlockChange(this, 'mutation', null, oldExtraState, newExtraState));
        //Blockly.Events.setGroup(false)
        this.updateShape()
    },
    deleteArrayElementInput: function (inputToDelete) {
        //const oldExtraState = getExtraBlockState(this)
        //Blockly.Events.setGroup(true)

        var inputNameToDelete = inputToDelete.name
        var inputIndexToDelete = Number(inputNameToDelete.match(/\d+/)[0])
        var substructure = this.getInputTargetBlock(inputNameToDelete)
        if (substructure) substructure.dispose(true, true)
        this.removeInput(inputNameToDelete)
        this.itemCount_ -= 1
        // rename all the subsequent element-inputs
        for (var i = inputIndexToDelete + 1; i <= this.itemCount_; i++) {
            var input = this.getInput('item_' + i)
            input.name = 'item_' + (i - 1)
        }

        //const newExtraState = getExtraBlockState(this)
        //Blockly.Events.fire(new Blockly.Events.BlockChange(this, 'mutation', null, oldExtraState, newExtraState))
        //Blockly.Events.setGroup(false)
    },

    updateShape: function () {
        for (let i = 0; i < this.itemCount_; i++) {
            if (!this.getInput('item_' + i)) {
                this.appendValueInput('item_' + i)
                    .setCheck(this.itemCheck_);
                    
            }
        }
    },

};

const dynamicListHelper = function () {
    try {
        this.removeInput('item');
    }
    catch { }
    this.updateShape();
};

Blockly.Extensions.registerMutator(
    'dynamic_list_mutator',
    dynamicListMutator,
    dynamicListHelper,
    null
);