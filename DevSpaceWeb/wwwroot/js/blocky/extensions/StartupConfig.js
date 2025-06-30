Blockly.registry.register(Blockly.registry.Type.TOOLBOX_ITEM, Blockly.ToolboxCategory.registrationName, CustomCategory, true);
Blockly.ContextMenuRegistry.registry.unregister('blockInline');

let MikuCount = 0;
let MikuBlock = null;
let MikuOffset = 0;
document.onkeydown = function (e) {
    e = e || window.event;//Get event

    var workspace = Blockly.getMainWorkspace();
    if (workspace == null || workspace.toolbox == null)
        return;

    if (e.keyCode === 77) {
        MikuCount += 1;

        if (MikuCount === 3) {
            MikuCount = 0;
            
            MikuBlock = workspace.newBlock('miku');
            MikuBlock.initSvg();
            MikuBlock.render();
            MikuBlock.moveBy(MikuOffset, MikuOffset);
            MikuOffset += 20;
        }
    }
    else
        MikuCount = 0;

    // Select the search bar when you press s
    //if (document.activeElement !== 'null' && document.getElementsByClassName('blocklyHtmlInput').length === 0 && document.activeElement.tagName !== "INPUT" && document.activeElement.tagName !== 'TEXTAREA') {
    //    var c = e.which || e.keyCode;//Get key code
    //    switch (c) {
    //        case 83://Block S
    //            e.preventDefault();
    //            e.stopPropagation();
    //            workspace.toolbox.selectItemByPosition(0)
    //            break;
    //    }
    //}
};

//Blockly.Extensions.register(
//    'check_all_inputs',
//    function () {
//        console.log('Load extension check_all_inputs');
//        console.log(this);
//        this.setOnChange(function (changeEvent) {
//            console.log('block change')
//            console.log(this);
//            console.log(changeEvent);
            
//        });

//    }
//)


