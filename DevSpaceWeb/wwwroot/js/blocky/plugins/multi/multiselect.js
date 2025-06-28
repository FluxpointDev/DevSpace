
import * as ContextMenu from './multiselect_contextmenu.js';
import * as Shortcut from './multiselect_shortcut.js';
import {
    blockSelectionWeakMap, inMultipleSelectionModeWeakMap,
    hasSelectedParent, BaseBlockDraggerWeakMap,
    multiselectControlsList
} from './global.js';
import { MultiselectControls } from './multiselect_controls.js';

/**
 * Class for using multiple select blocks on workspace.
 */
export class Multiselect {
    /**
     * Initalize the class data structure.
     * @param {!Blockly.WorkspaceSvg} workspace The workspace to sit in.
     */
    constructor(workspace) {
        this.workspace_ = workspace;
        this.origHandleWsStart_ = Blockly.Gesture.prototype.handleWsStart;

        blockSelectionWeakMap.set(this.workspace_, new Set());
        this.blockSelection_ = blockSelectionWeakMap.get(this.workspace_);
        inMultipleSelectionModeWeakMap.set(this.workspace_, false);
        BaseBlockDraggerWeakMap.set(this.workspace_, Blockly.BlockDragger);
        this.useCopyPasteCrossTab_ = true;
        this.useCopyPasteMenu_ = true;
    }

    /**
     * Bind the events and replace registration.
     * @param {Object} options
     * to set.
     */
    init(options) {
        const injectionDiv = this.workspace_.getInjectionDiv();
        this.onKeyDownWrapper_ = Blockly.browserEvents.conditionalBind(
            injectionDiv, 'keydown', this, this.onKeyDown_);
        this.onKeyUpWrapper_ = Blockly.browserEvents.conditionalBind(
            injectionDiv, 'keyup', this, this.onKeyUp_);
        this.onFocusOutWrapper_ = Blockly.browserEvents.conditionalBind(
            injectionDiv, 'focusout', this, this.onBlur_);
        injectionDiv.addEventListener('mouseenter', () => {
            
            if (document.activeElement === this.workspace_.svgGroup_.parentElement ||
                document.activeElement.nodeName.toLowerCase() === 'input' ||
                document.activeElement.nodeName.toLowerCase() === 'textarea') {
                return;
            }
            this.workspace_.svgGroup_.parentElement.focus();
        });
        this.eventListenerWrapper_ = this.eventListener_.bind(this);
        this.workspace_.addChangeListener(this.eventListenerWrapper_);

        if (options.multiselectCopyPaste &&
            options.multiselectCopyPaste.crossTab === false) {
            this.useCopyPasteCrossTab_ = false;
        }

        if (options.multiselectCopyPaste &&
            options.multiselectCopyPaste.menu === false) {
            this.useCopyPasteMenu_ = false;
        }

        if (!Blockly.ContextMenuRegistry.registry.registry_.workspaceSelectAll) {
            ContextMenu.unregisterContextMenu();
            ContextMenu.registerOurContextMenu(this.useCopyPasteMenu_,
                this.useCopyPasteCrossTab_);
            Shortcut.unregisterShortcut();
            Shortcut.registerOurShortcut(this.useCopyPasteCrossTab_);
        }

        this.controls_ = new MultiselectControls(
            this.workspace_, options.multiselectIcon, this);
        multiselectControlsList.add(this.controls_);
        if (!options.multiselectIcon || !options.multiselectIcon.hideIcon) {
            const svgControls = this.controls_.createDom();
            this.workspace_.getParentSvg().appendChild(svgControls);
        }
        this.controls_.init(options.multiselectIcon.hideIcon, options.multiselectIcon.weight);

        if (options.useDoubleClick) {
            this.useDoubleClick_(true);
        }

        if (options.baseBlockDragger) {
            BaseBlockDraggerWeakMap.set(this.workspace_, options.baseBlockDragger);
        }

        if (!options.bumpNeighbours) {
            this.origBumpNeighbours = Blockly.BlockSvg.prototype.bumpNeighbours;
            Blockly.BlockSvg.prototype.bumpNeighbours = function () { };
        }
    }

    /**
     * Unbind the events and replace with original registration.
     * @param {boolean} keepRegistry Keep the context menu and shortcut registry.
     */
    dispose(keepRegistry = false) {
        if (this.onKeyDownWrapper_) {
            Blockly.browserEvents.unbind(this.onKeyDownWrapper_);
            this.onKeyDownWrapper_ = null;
        }
        if (this.onKeyUpWrapper_) {
            Blockly.browserEvents.unbind(this.onKeyUpWrapper_);
            this.onKeyUpWrapper_ = null;
        }
        if (this.onFocusOutWrapper_) {
            Blockly.browserEvents.unbind(this.onFocusOutWrapper_);
            this.onFocusOutWrapper_ = null;
        }
        if (this.eventListenerWrapper_) {
            this.workspace_.removeChangeListener(this.eventListenerWrapper_);
            this.eventListenerWrapper_ = null;
        }
        if (!keepRegistry) {
            ContextMenu.unregisterContextMenu();
            if (this.useCopyPasteMenu_) {
                Blockly.ContextMenuRegistry.registry.unregister('blockCopyToStorage');
                Blockly.ContextMenuRegistry.registry
                    .unregister('blockPasteFromStorage');
            }
            Blockly.ContextMenuRegistry.registry.unregister('workspaceSelectAll');
            ContextMenu.registerOrigContextMenu();

            Shortcut.unregisterShortcut();
            Blockly.ShortcutRegistry.registry.unregister('selectall');
            Shortcut.registerOrigShortcut();
        }

        if (this.controls_) {
            multiselectControlsList.delete(this.controls_);
            this.controls_.dispose();
            this.controls_ = null;
        }

        this.useDoubleClick_(false);

        if (this.origBumpNeighbours) {
            Blockly.BlockSvg.prototype.bumpNeighbours = this.origBumpNeighbours;
        }
    }

    /**
     * Add double click to expand/collapse blocks (MIT App Inventor Supported).
     * @param {!boolean} on Whether to turn on the mode.
     * @private
     */
    useDoubleClick_(on) {
        if (!on) {
            Blockly.Gesture.prototype.handleWsStart = this.origHandleWsStart_;
            return;
        }

        Blockly.Gesture.prototype.handleWsStart = (function (func) {
            if (func.isWrapped) {
                return func;
            }

            const wrappedFunc = function (e, ws) {
                func.call(this, e, ws);
                if (this.targetBlock && e.buttons === 1 &&
                    !inMultipleSelectionModeWeakMap.get(ws)) {
                    const preCondition = function (block) {
                        return !block.isInFlyout && block.isMovable() &&
                            block.workspace.options.collapse;
                    };
                    if (Blockly.getSelected() && preCondition(Blockly.getSelected())) {
                        if (ws.doubleClickPid_) {
                            clearTimeout(ws.doubleClickPid_);
                            ws.doubleClickPid_ = undefined;
                            if (Blockly.getSelected().id === ws.doubleClickBlock_) {
                                const state = !Blockly.getSelected().isCollapsed();
                                const maybeCollapse = function (block) {
                                    if (block && preCondition(block) &&
                                        !hasSelectedParent(block)) {
                                        block.setCollapsed(state);
                                    }
                                };
                                Blockly.Events.setGroup(true);
                                const blockSelection = blockSelectionWeakMap.get(ws);
                                if (Blockly.getSelected() && !blockSelection.size) {
                                    maybeCollapse(Blockly.getSelected());
                                }
                                blockSelection.forEach(function (id) {
                                    const block = ws.getBlockById(id);
                                    if (block) {
                                        maybeCollapse(block);
                                    }
                                });
                                Blockly.Events.setGroup(false);
                                return;
                            }
                        }
                        if (!ws.doubleClickPid_) {
                            ws.doubleClickBlock_ = Blockly.getSelected().id;
                            ws.doubleClickPid_ = setTimeout(function () {
                                ws.doubleClickPid_ = undefined;
                            }, 500);
                        }
                    }
                }
            };
            wrappedFunc.isWrapped = true;
            return wrappedFunc;
        })(Blockly.Gesture.prototype.handleWsStart);
    }

    /**
     * Handle workspace events.
     * @param {!Event} e Blockly event.
     * @private
     */
    eventListener_(e) {
        // on Block Selected
        if (e.type === Blockly.Events.SELECTED) {
            multiselectControlsList.forEach((controls) => {
                controls.updateMultiselect();
            });
            // on Block field changed
        } else if (e.type === Blockly.Events.CHANGE &&
            e.element === 'field' && e.recordUndo &&
            this.blockSelection_.has(e.blockId)) {
            const inGroup = !!e.group;
            if (!inGroup) {
                Blockly.Events.setGroup(true);
                e.group = Blockly.Events.getGroup();
            }
            try {
                const blockType = this.workspace_.getBlockById(e.blockId).type;
                // Update the fields to the same value for
                // the selected blocks with same type.
                this.blockSelection_.forEach((id) => {
                    const block = this.workspace_.getBlockById(id);
                    if (block.type === blockType) {
                        block.setFieldValue(e.newValue, e.name);
                    }
                });
            } finally {
                if (!inGroup) {
                    Blockly.Events.setGroup(false);
                }
            }
        }
    }

    /**
     * Handle a key-down on the workspace.
     * @param {KeyboardEvent} e The keyboard event.
     * @private
     */
    onKeyDown_(e) {
        if (e.keyCode === Blockly.utils.KeyCodes.CTRL &&
            !inMultipleSelectionModeWeakMap.get(this.workspace_)) {
            this.controls_.enableMultiselect();
        }
    }

    /**
     * Handle a key-up on the workspace.
     * @param {KeyboardEvent} e The keyboard event.
     * @private
     */
    onKeyUp_(e) {
        if (e.keyCode === Blockly.utils.KeyCodes.CTRL) {
            this.controls_.disableMultiselect();
        }
    }

    /**
     * Handle a blur on the workspace.
     * @private
     */
    onBlur_() {
        if (inMultipleSelectionModeWeakMap.get(this.workspace_)) {
            this.controls_.disableMultiselect();
        }
    }
}