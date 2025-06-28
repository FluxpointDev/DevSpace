/**
 * @license
 * Copyright 2022 MIT
 * SPDX-License-Identifier: Apache-2.0
 */

/**
 * @fileoverview Multiple selection context menu.
 */

import {
    blockSelectionWeakMap, hasSelectedParent, copyData,
    connectionDBList, dataCopyToStorage, dataCopyFromStorage,
    blockNumGetFromStorage, registeredContextMenu
} from './global.js';

/**
 * Copy multiple selected blocks to clipboard.
 * @param {boolean} useCopyPasteCrossTab Whether or not to use
 *     cross tab copy paste.
 */
const registerCopy = function (useCopyPasteCrossTab) {
    const id = 'blockCopyToStorage';
    const copyOptions = {
        displayText: function (scope) {
            let workableBlocksLength = 0;
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (copyOptions.check(block)) {
                    workableBlocksLength++;
                }
            });
            if (workableBlocksLength <= 1) {
                return Blockly.Msg['CROSS_TAB_COPY'] ?
                    Blockly.Msg['CROSS_TAB_COPY'] : 'Copy';
            } else {
                return Blockly.Msg['CROSS_TAB_COPY_X_BLOCKS'] ?
                    Blockly.Msg['CROSS_TAB_COPY_X_BLOCKS'].replace(
                        '%1', workableBlocksLength) :
                    (Blockly.Msg['CROSS_TAB_COPY'] ?
                        Blockly.Msg['CROSS_TAB_COPY'] : 'Copy'
                    ) + ' (' +
                    workableBlocksLength + ')';
            }
        },
        preconditionFn: function (scope) {
            const workspace = scope.block.workspace;
            if (workspace.options.readOnly && !useCopyPasteCrossTab) {
                return 'hidden';
            }
            const selected = Blockly.common.getSelected();
            const blockSelection = blockSelectionWeakMap.get(workspace);
            if (!blockSelection.size) {
                if (copyOptions.check(selected)) {
                    return 'enabled';
                } else {
                    return 'disabled';
                }
            }
            for (const id of blockSelection) {
                const block = workspace.getBlockById(id);
                if (copyOptions.check(block)) {
                    return 'enabled';
                }
            }
            return 'disabled';
        },
        check: function (block) {
            return block && block.isDeletable() && block.isMovable() &&
                !hasSelectedParent(block);
        },
        callback: function (scope) {
            const workspace = scope.block.workspace;
            copyData.clear();
            workspace.hideChaff();
            const blockList = [];
            const apply = function (block) {
                if (copyOptions.check(block)) {
                    copyData.add(JSON.stringify(block.toCopyData()));
                    blockList.push(block.id);
                }
            };
            const selected = Blockly.common.getSelected();
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            if (!blockSelection.size) {
                apply(selected);
            }
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                apply(block);
            });
            connectionDBList.length = 0;
            blockList.forEach(function (id) {
                const block = workspace.getBlockById(id);
                const parentBlock = block.getParent();
                if (parentBlock && blockList.indexOf(parentBlock.id) !== -1 &&
                    parentBlock.getNextBlock() === block) {
                    connectionDBList.push([
                        blockList.indexOf(parentBlock.id),
                        blockList.indexOf(block.id)]);
                }
            });
            if (useCopyPasteCrossTab) {
                dataCopyToStorage();
            }
            Blockly.Events.setGroup(false);
            return true;
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
        id,
        weight: 0,
    };
    if (Blockly.ContextMenuRegistry.registry.getItem(id) !== null) {
        Blockly.ContextMenuRegistry.registry.unregister(id);
    }
    Blockly.ContextMenuRegistry.registry.register(copyOptions);
};

/**
 * Modification for context menu 'Duplicate' to be available for
 * multiple blocks.
 */
const registerDuplicate = function () {
    const duplicateOption = {
        displayText: function (scope) {
            let workableBlocksLength = 0;
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (duplicateOption.check(block)) {
                    workableBlocksLength++;
                }
            });
            if (workableBlocksLength <= 1) {
                return Blockly.Msg['DUPLICATE_BLOCK'];
            } else {
                return Blockly.Msg['DUPLICATE_X_BLOCKS'] ?
                    Blockly.Msg['DUPLICATE_X_BLOCKS'].replace(
                        '%1', workableBlocksLength) :
                    Blockly.Msg['DUPLICATE_BLOCK'] + ' (' +
                    workableBlocksLength + ')';
            }
        },
        preconditionFn: function (scope) {
            const block = scope.block;
            if (!block.isInFlyout && block.isDeletable() && block.isMovable()) {
                if (block.isDuplicatable()) {
                    return 'enabled';
                }
                return 'disabled';
            }
            return 'hidden';
        },
        // Only duplicate-able blocks will be duplicated.
        check: function (block) {
            return block &&
                duplicateOption.preconditionFn({ block: block }) === 'enabled' &&
                !hasSelectedParent(block);
        },
        callback: function (scope) {
            const duplicatedBlocks = {};
            const connectionDBList = [];
            const workspace = scope.block.workspace;
            const apply = function (block) {
                if (duplicateOption.check(block)) {
                    duplicatedBlocks[block.id] = Blockly.clipboard.paste(block.toCopyData(), workspace);
                }
                block.pathObject.updateSelected(false);
            };
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            if (!blockSelection.size) {
                apply(scope.block);
            }
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                apply(block);
            });
            blockSelection.clear();
            for (const [id, block] of Object.entries(duplicatedBlocks)) {
                const origBlock = workspace.getBlockById(id);
                const origParentBlock = origBlock.getParent();
                if (block.id) {
                    if (origParentBlock && origParentBlock.id in duplicatedBlocks &&
                        origParentBlock.getNextBlock() === origBlock) {
                        connectionDBList.push([
                            duplicatedBlocks[origParentBlock.id].nextConnection,
                            block.previousConnection]);
                    }
                    blockSelection.add(block.id);
                    block.pathObject.updateSelected(true);
                }
            }
            connectionDBList.forEach(function (connectionDB) {
                connectionDB[0].connect(connectionDB[1]);
            });
            Blockly.Events.setGroup(false);
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
        id: 'blockDuplicate',
        weight: 1,
    };
    Blockly.ContextMenuRegistry.registry.register(duplicateOption);
};

/**
 * Modification for context menu 'Comment' to be available for multiple blocks.
 */
const registerComment = function () {
    const commentOption = {
        displayText: function (scope) {
            let workableBlocksLength = 0;
            const state = scope.block.hasIcon(Blockly.icons.CommentIcon.TYPE);
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (commentOption.check(block) && state === block.hasIcon(
                    Blockly.icons.CommentIcon.TYPE)) {
                    workableBlocksLength++;
                }
            });
            if (state) {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['REMOVE_COMMENT'];
                } else {
                    return Blockly.Msg['REMOVE_X_COMMENTS'] ?
                        Blockly.Msg['REMOVE_X_COMMENTS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['REMOVE_COMMENT'] + ' (' +
                        workableBlocksLength + ')';
                }
            } else {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['ADD_COMMENT'];
                } else {
                    return Blockly.Msg['ADD_X_COMMENTS'] ?
                        Blockly.Msg['ADD_X_COMMENTS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['ADD_COMMENT'] + ' (' +
                        workableBlocksLength + ')';
                }
            }
        },
        preconditionFn: function (scope) {
            const block = scope.block;
            if (!Blockly.utils.userAgent.IE && !block.isInFlyout &&
                block.workspace.options.comments && !block.isCollapsed() &&
                block.isEditable()) {
                return 'enabled';
            }
            return 'hidden';
        },
        check: function (block) {
            return block &&
                commentOption.preconditionFn({ block: block }) === 'enabled';
        },
        callback: function (scope) {
            const hasCommentIcon = scope.block.hasIcon(
                Blockly.icons.CommentIcon.TYPE);
            const apply = function (block) {
                if (commentOption.check(block)) {
                    if (hasCommentIcon) {
                        block.setCommentText(null);
                    } else {
                        block.setCommentText('');
                    }
                }
            };
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            if (!blockSelection.size) {
                apply(scope.block);
            }
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                apply(block);
            });
            Blockly.Events.setGroup(false);
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
        id: 'blockComment',
        weight: 2,
    };
    Blockly.ContextMenuRegistry.registry.register(commentOption);
};

/**
 * Modification for context menu 'Inline' to be available for multiple blocks.
 */
const registerInline = function () {
    const inlineOption = {
        displayText: function (scope) {
            let workableBlocksLength = 0;
            const state = scope.block.getInputsInline();
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (inlineOption.check(block) &&
                    block.getInputsInline() === state) {
                    workableBlocksLength++;
                }
            });
            if (state) {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['EXTERNAL_INPUTS'];
                } else {
                    return Blockly.Msg['EXTERNAL_X_INPUTS'] ?
                        Blockly.Msg['EXTERNAL_X_INPUTS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['EXTERNAL_INPUTS'] + ' (' +
                        workableBlocksLength + ')';
                }
            } else {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['INLINE_INPUTS'];
                } else {
                    return Blockly.Msg['INLINE_X_INPUTS'] ?
                        Blockly.Msg['INLINE_X_INPUTS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['INLINE_INPUTS'] + ' (' +
                        workableBlocksLength + ')';
                }
            }
        },
        preconditionFn: function (scope) {
            const block = scope.block;
            if (!block.isInFlyout && block.isMovable() && !block.isCollapsed()) {
                for (let i = 1; i < block.inputList.length; i++) {
                    // Only display this option if there are two value or dummy inputs
                    // next to each other.
                    if (block.inputList[i - 1].type !== Blockly.inputTypes.STATEMENT &&
                        block.inputList[i].type !== Blockly.inputTypes.STATEMENT) {
                        return 'enabled';
                    }
                }
            }
            return 'hidden';
        },
        check: function (block) {
            return block &&
                inlineOption.preconditionFn({ block: block }) === 'enabled';
        },
        callback: function (scope) {
            const state = !scope.block.getInputsInline();
            const apply = function (block) {
                if (inlineOption.check(block)) {
                    block.setInputsInline(state);
                }
            };
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            if (!blockSelection.size) {
                apply(scope.block);
            }
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                apply(block);
            });
            Blockly.Events.setGroup(false);
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
        id: 'blockInline',
        weight: 3,
    };
    Blockly.ContextMenuRegistry.registry.register(inlineOption);
};

/**
 * Modification for context menu 'Collapse/Expand' to be available for
 * multiple blocks.
 */
const registerCollapseExpandBlock = function () {
    const collapseExpandOption = {
        displayText: function (scope) {
            let workableBlocksLength = 0;
            const state = scope.block.isCollapsed();
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (collapseExpandOption.check(block) &&
                    block.isCollapsed() === state) {
                    workableBlocksLength++;
                }
            });
            if (state) {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['EXPAND_BLOCK'];
                } else {
                    return Blockly.Msg['EXPAND_X_BLOCKS'] ?
                        Blockly.Msg['EXPAND_X_BLOCKS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['EXPAND_BLOCK'] + ' (' +
                        workableBlocksLength + ')';
                }
            } else {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['COLLAPSE_BLOCK'];
                } else {
                    return Blockly.Msg['COLLAPSE_X_BLOCKS'] ?
                        Blockly.Msg['COLLAPSE_X_BLOCKS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['COLLAPSE_BLOCK'] + ' (' +
                        workableBlocksLength + ')';
                }
            }
        },
        preconditionFn: function (scope) {
            const block = scope.block;
            if (!block.isInFlyout && block.isMovable() &&
                block.workspace.options.collapse) {
                return 'enabled';
            }
            return 'hidden';
        },
        check: function (block) {
            return block &&
                collapseExpandOption.preconditionFn({ block: block }) ===
                'enabled' && (!hasSelectedParent(block) ||
                    block.isCollapsed());
        },
        callback: function (scope) {
            const state = !scope.block.isCollapsed();
            const apply = function (block) {
                if (collapseExpandOption.check(block)) {
                    block.setCollapsed(state);
                }
            };
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            if (!blockSelection.size) {
                apply(scope.block);
            }
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                apply(block);
            });
            Blockly.Events.setGroup(false);
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
        id: 'blockCollapseExpand',
        weight: 4,
    };
    Blockly.ContextMenuRegistry.registry.register(collapseExpandOption);
};

/**
 * Modification for context menu 'Disable' to be available for multiple blocks.
 */
const registerDisable = function () {
    const disableOption = {
        displayText: function (scope) {
            let workableBlocksLength = 0;
            const state = scope.block.isEnabled();
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (disableOption.check(block) &&
                    block.isEnabled() === state) {
                    workableBlocksLength++;
                }
            });
            if (state) {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['DISABLE_BLOCK'];
                } else {
                    return Blockly.Msg['DISABLE_X_BLOCKS'] ?
                        Blockly.Msg['DISABLE_X_BLOCKS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['DISABLE_BLOCK'] + ' (' +
                        workableBlocksLength + ')';
                }
            } else {
                if (workableBlocksLength <= 1) {
                    return Blockly.Msg['ENABLE_BLOCK'];
                } else {
                    return Blockly.Msg['ENABLE_X_BLOCKS'] ?
                        Blockly.Msg['ENABLE_X_BLOCKS'].replace(
                            '%1', workableBlocksLength) :
                        Blockly.Msg['ENABLE_BLOCK'] + ' (' +
                        workableBlocksLength + ')';
                }
            }
        },
        preconditionFn: function (scope) {
            const block = scope.block;
            if (!block.isInFlyout && block.workspace.options.disable &&
                block.isEditable()) {
                if (block.getInheritedDisabled()) {
                    return 'disabled';
                }
                return 'enabled';
            }
            return 'hidden';
        },
        check: function (block) {
            return block &&
                disableOption.preconditionFn({ block: block }) === 'enabled' &&
                (!hasSelectedParent(block) || !block.isEnabled());
        },
        callback: function (scope) {
            const state = !scope.block.isEnabled();
            const apply = function (block) {
                if (disableOption.check(block)) {
                    block.setEnabled(state);
                }
            };
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            if (!blockSelection.size) {
                apply(scope.block);
            }
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                apply(block);
            });
            Blockly.Events.setGroup(false);
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
        id: 'blockDisable',
        weight: 5,
    };
    Blockly.ContextMenuRegistry.registry.register(disableOption);
};

/**
 * Modification for context menu 'Delete' to be available for multiple blocks.
 */
const registerDelete = function () {
    const deleteOption = {
        displayText: function (scope) {
            let descendantCount = 0;
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (block && !hasSelectedParent(block)) {
                    // Count the number of blocks that are nested in this block.
                    descendantCount += block.getDescendants(false).length;
                    const nextBlock = block.getNextBlock();
                    if (nextBlock) {
                        // Blocks in the current stack would survive this block's deletion.
                        descendantCount -= nextBlock.getDescendants(false).length;
                    }
                }
            });
            return (descendantCount === 1) ?
                Blockly.Msg['DELETE_BLOCK'] :
                Blockly.Msg['DELETE_X_BLOCKS'].replace('%1', String(descendantCount));
        },
        preconditionFn: function (scope) {
            if (!scope.block.isInFlyout && scope.block.isDeletable()) {
                return 'enabled';
            }
            return 'hidden';
        },
        check: function (block) {
            return block && !block.workspace.isFlyout &&
                deleteOption.preconditionFn({ block: block }) === 'enabled' &&
                !hasSelectedParent(block);
        },
        callback: function (scope) {
            const apply = function (block) {
                if (deleteOption.check(block)) {
                    block.workspace.hideChaff();
                    if (block.outputConnection) {
                        block.dispose(false, true);
                    } else {
                        block.dispose(true, true);
                    }
                }
            };
            const workspace = scope.block.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            if (!blockSelection.size) {
                apply(scope.block);
            }
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                apply(block);
            });
            Blockly.Events.setGroup(false);
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.BLOCK,
        id: 'blockDelete',
        weight: 6,
    };
    Blockly.ContextMenuRegistry.registry.register(deleteOption);
};

/**
 * Paste multiple selected blocks from clipboard.
 * @param {boolean} useCopyPasteCrossTab Whether to use cross tab copy paste.
 */
const registerPaste = function (useCopyPasteCrossTab) {
    const id = 'blockPasteFromStorage';
    const pasteOption = {
        displayText: function () {
            const workableBlocksLength = blockNumGetFromStorage(useCopyPasteCrossTab);
            if (workableBlocksLength <= 1) {
                return Blockly.Msg['CROSS_TAB_PASTE'] ?
                    Blockly.Msg['CROSS_TAB_PASTE'] : 'Paste';
            } else {
                return Blockly.Msg['CROSS_TAB_PASTE_X_BLOCKS'] ?
                    Blockly.Msg['CROSS_TAB_PASTE_X_BLOCKS'].replace(
                        '%1', workableBlocksLength) :
                    (Blockly.Msg['CROSS_TAB_PASTE'] ?
                        Blockly.Msg['CROSS_TAB_PASTE'] : 'Paste'
                    ) + ' (' +
                    workableBlocksLength + ')';
            }
        },
        preconditionFn: function (scope) {
            return scope.workspace.options.readOnly ?
                'hidden' : (blockNumGetFromStorage(useCopyPasteCrossTab) < 1 ?
                    'disabled' : 'enabled');
        },
        callback: function (scope) {
            let workspace = scope.workspace;
            const blockSelection = blockSelectionWeakMap.get(workspace);
            Blockly.Events.setGroup(true);
            blockSelection.forEach(function (id) {
                const block = workspace.getBlockById(id);
                if (block) {
                    block.pathObject.updateSelected(false);
                }
            });
            blockSelection.clear();
            const blockList = [];
            if (useCopyPasteCrossTab) {
                dataCopyFromStorage();
            }
            copyData.forEach(function (stringData) {
                // Pasting always pastes to the main workspace, even if the copy
                // started in a flyout workspace.
                const data = JSON.parse(stringData);
                if (data.source) {
                    workspace = data.source;
                }
                if (workspace.isFlyout) {
                    workspace = workspace.targetWorkspace;
                }
                if (data.typeCounts &&
                    workspace.isCapacityAvailable(data.typeCounts)) {
                    const block = Blockly.clipboard.paste(data, workspace);
                    blockList.push(block);
                    block.pathObject.updateSelected(true);
                    blockSelectionWeakMap.get(block.workspace).add(block.id);
                }
            });
            connectionDBList.forEach(function (connectionDB) {
                blockList[connectionDB[0]].nextConnection.connect(
                    blockList[connectionDB[1]].previousConnection);
            });
            Blockly.Events.setGroup(false);
            return true;
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.WORKSPACE,
        id,
        weight: 0,
    };
    if (Blockly.ContextMenuRegistry.registry.getItem(id) !== null) {
        Blockly.ContextMenuRegistry.registry.unregister(id);
    }
    Blockly.ContextMenuRegistry.registry.register(pasteOption);
};

/**
 * Add context menu 'Select all Blocks' for workspace.
 */
const registerSelectAll = function () {
    const id = 'workspaceSelectAll';
    const selectAllOption = {
        displayText: function () {
            return 'Select all Blocks';
        },
        preconditionFn: function (scope) {
            return scope.workspace.getTopBlocks().some(
                (b) => selectAllOption.check(b)) ? 'enabled' : 'disabled';
        },
        check: function (block) {
            return block &&
                (block.isDeletable() || block.isMovable()) &&
                !block.isInsertionMarker();
        },
        callback: function (scope) {
            const blockSelection = blockSelectionWeakMap.get(scope.workspace);
            if (Blockly.getSelected() &&
                !blockSelection.has(Blockly.getSelected().id)) {
                Blockly.getSelected().pathObject.updateSelected(false);
                Blockly.common.setSelected(null);
            }
            const blockList = [];
            scope.workspace.getTopBlocks().forEach(function (block) {
                if (selectAllOption.check(block)) {
                    blockList.push(block);
                    let nextBlock = block.getNextBlock();
                    while (nextBlock) {
                        blockList.push(nextBlock);
                        nextBlock = nextBlock.getNextBlock();
                    }
                }
            });
            blockList.forEach(function (block) {
                blockSelectionWeakMap.get(block.workspace).add(block.id);
                if (!Blockly.common.getSelected()) {
                    Blockly.common.setSelected(block);
                }
                block.pathObject.updateSelected(true);
            });
        },
        scopeType: Blockly.ContextMenuRegistry.ScopeType.WORKSPACE,
        id,
        weight: 5,
    };
    if (Blockly.ContextMenuRegistry.registry.getItem(id) !== null) {
        Blockly.ContextMenuRegistry.registry.unregister(id);
    }
    Blockly.ContextMenuRegistry.registry.register(selectAllOption);
};

/**
 * Unregister context menu item, should be called before registering.
 */
export const unregisterContextMenu = function () {
    registeredContextMenu.length = 0;
    for (const id of ['blockDuplicate', 'blockComment', 'blockInline',
        'blockCollapseExpand', 'blockDisable', 'blockDelete']) {
        if (Blockly.ContextMenuRegistry.registry.getItem(id) !== null) {
            Blockly.ContextMenuRegistry.registry.unregister(id);
            registeredContextMenu.push(id);
        }
    }
};

/**
 * Register default context menu item.
 */
export const registerOrigContextMenu = function () {
    const map = {
        blockDuplicate: Blockly.ContextMenuItems.registerDuplicate,
        blockComment: Blockly.ContextMenuItems.registerComment,
        blockInline: Blockly.ContextMenuItems.registerInline,
        blockCollapseExpand: Blockly.ContextMenuItems.registerCollapseExpandBlock,
        blockDisable: Blockly.ContextMenuItems.registerDisable,
        blockDelete: Blockly.ContextMenuItems.registerDelete,
    };
    for (const id of registeredContextMenu) {
        map[id]();
    }
};

/**
 * Registers all modified context menu item.
 * @param {boolean} useCopyPasteMenu Whether to use copy/paste menu.
 * @param {boolean} useCopyPasteCrossTab Whether to use cross tab copy/paste.
 */
export const registerOurContextMenu = function (useCopyPasteMenu, useCopyPasteCrossTab) {
    if (useCopyPasteMenu) {
        registerCopy(useCopyPasteCrossTab);
        registerPaste(useCopyPasteCrossTab);
    }
    const map = {
        blockDuplicate: registerDuplicate,
        blockComment: registerComment,
        blockInline: registerInline,
        blockCollapseExpand: registerCollapseExpandBlock,
        blockDisable: registerDisable,
        blockDelete: registerDelete,
    };
    for (const id of registeredContextMenu) {
        map[id]();
    }
    registerSelectAll();
};
