window.WorkspaceUtils = {
    Plugins: {},
    ToggleMinimap: function () {
        if (document.getElementById('WorkspaceDiv').classList.contains('workspace-disableminimap')) {
            document.getElementById('WorkspaceDiv').classList.remove('workspace-disableminimap');
        }
        else {
            document.getElementById('WorkspaceDiv').classList.add('workspace-disableminimap');
        }
    },
    ToggleGrid: function () {
        if (Blockly.getMainWorkspace().options.gridPattern.style.display === "none")
            Blockly.getMainWorkspace().options.gridPattern.style.display = "";
        else
            Blockly.getMainWorkspace().options.gridPattern.style.display = "none"; 
    },
    ToggleSnap: function () {
        if (Blockly.getMainWorkspace().grid)
            Blockly.getMainWorkspace().grid.snapToGrid = !Blockly.getMainWorkspace().grid.snapToGrid;
    },
    ToggleHighlight: function () {
        if (this.Plugins.Highlighter.background) {
            if (this.Plugins.Highlighter.background.attributes.fill.value === '#353535') {
                this.Plugins.Highlighter.background.setAttribute('fill', '#1e1e1e')
            }
            else {
                this.Plugins.Highlighter.background.setAttribute('fill', '#353535')
            }
        }
        else {
            this.Plugins.Highlighter.init();
        }
    },
    SetMaxBlocks: function (number) {
        console.log('Set max: ' + number);
        this.MaxBlocks = number;
        if (Blockly.getMainWorkspace())
            Blockly.getMainWorkspace().options.maxBlocks = number;
    }
}