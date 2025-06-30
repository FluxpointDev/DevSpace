Blockly.ContextMenuRegistry.registry.register({
    displayText: 'Disconnect Block',
    preconditionFn: function (scope) {
        if (scope.block.isInFlyout == false && scope.block.movable_ && scope.block.parentBlock_ !== null) {
            return 'enabled';
        }
        return 'disabled';
    },
    callback: function (scope) {
        const block = scope.block;
        block.unplug();
        block.bumpNeighbours();
    },
    scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
    id: 'disconnectBlock',
    weight: 10,
});

Blockly.ContextMenuRegistry.registry.register({
    displayText: 'Delete All Inputs',
    preconditionFn: function (scope) {
        if (scope.block.isInFlyout == false && scope.block.inputList.findIndex(x => x.connection !== null && x.connection.targetConnection !== null && x.connection.targetConnection.sourceBlock_) !== -1) {
            return 'enabled';
        }
        return 'disabled';
    },
    callback: function (scope) {
        const block = scope.block;
        

        if (block.inputList.findIndex(x => x.connection && x.connection.targetConnection && x.connection.targetConnection.sourceBlock_) !== -1) {
            block.inputList.forEach(x => {
                if (x.deletable_ === true && x.connection && x.connection.targetConnection && x.connection.targetConnection.sourceBlock_) {
                    x.connection.targetConnection.sourceBlock_.dispose();
                }
            });
        }
    },
    scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
    id: 'deleteInputs',
    weight: 9,
});