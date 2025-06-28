Blockly.registry.register(Blockly.registry.Type.TOOLBOX_ITEM, Blockly.ToolboxCategory.registrationName, CustomCategory, true);
Blockly.ContextMenuRegistry.registry.unregister('blockInline');

let MikuCount = 0;
let MikuBlock = null;
let MikuOffset = 0;
document.onkeydown = function (e) {
    e = e || window.event;//Get event
    // console.log('Test');
    // console.log(document.activeElement);
    // console.log(document);

    if (e.keyCode === 77) {
        MikuCount += 1;

        if (MikuCount === 3) {
            MikuCount = 0;
            
            MikuBlock = Blockly.getMainWorkspace().newBlock('miku');
            MikuBlock.initSvg();
            MikuBlock.render();
            MikuBlock.moveBy(MikuOffset, MikuOffset);
            MikuOffset += 20;
        }
    }
    else
        MikuCount = 0;

    if (document.activeElement !== 'null' && document.getElementsByClassName('blocklyHtmlInput').length === 0 && document.activeElement.tagName !== "INPUT" && document.activeElement.tagName !== 'TEXTAREA') {
        var c = e.which || e.keyCode;//Get key code
        switch (c) {
            case 83://Block S
                e.preventDefault();
                e.stopPropagation();
                Blockly.getMainWorkspace().toolbox.selectItemByPosition(0)
                break;
        }
    }
};

window.allowedBlocks = {
    messageTypes: ["data_message"]
}

Blockly.Extensions.register(
    'check_all_inputs',
    function () {
        console.log('Load extension check_all_inputs');
        console.log(this);
        this.setOnChange(function (changeEvent) {
            console.log('block change')
            console.log(this);
            console.log(changeEvent);
            
        });

    }
)


