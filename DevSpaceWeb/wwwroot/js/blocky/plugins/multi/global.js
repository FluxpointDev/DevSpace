/**
 * @license
 * Copyright 2022 MIT
 * SPDX-License-Identifier: Apache-2.0
 */

/**
 * @fileoverview Global data structure.
 */


/**
 * Set for storing the current selected blockSvg ids.
 */
export const blockSelectionWeakMap = new WeakMap();

/**
 * Store the current selection mode.
 */
export const inMultipleSelectionModeWeakMap = new WeakMap();

/**
 * Store the base BlockDragger.
 */
export const BaseBlockDraggerWeakMap = new WeakMap();

/**
 * Store the multi-select controls instances.
 */
export const multiselectControlsList = new Set();

/**
 * Store the copy data.
 */
export const copyData = new Set();

/**
 * Store the copied connections list.
 */
export const connectionDBList = [];

/**
 * Store the registered context menu list.
 */
export const registeredContextMenu = [];

/**
 * Store the registered shortcut list.
 */
export const registeredShortcut = [];

/**
 * Store the copy time.
 */
let timestamp = 0;

/**
 * Check if the current selected blockSvg set already contains the parents.
 * @param {!Blockly.BlockSvg} block to check.
 * @param {boolean} move Whether or not in moving.
 * @returns {boolean} true if the block's parents are selected.
 */
export const hasSelectedParent = function (block, move = false) {
    while (block) {
        if (move) {
            block = block.getParent();
        } else {
            block = block.getSurroundParent();
        }

        if (block && blockSelectionWeakMap.get(block.workspace).has(block.id)) {
            return true;
        }
    }
    return false;
};

/**
 * Store copy information for blocks in localStorage.
 */
export const dataCopyToStorage = function () {
    const storage = [];
    copyData.forEach((data) => {
        delete data['source'];
        storage.push(data);
    });
    timestamp = Date.now();
    localStorage.setItem('blocklyStashMulti', JSON.stringify(storage));
    localStorage.setItem('blocklyStashConnection',
        JSON.stringify(connectionDBList));
    localStorage.setItem('blocklyStashTime', timestamp);
};

/**
 * Get copy information for blocks from localStorage.
 */
export const dataCopyFromStorage = function () {
    const storage = JSON.parse(localStorage.getItem('blocklyStashMulti'));
    const connection = JSON.parse(localStorage.getItem('blocklyStashConnection'));
    const time = localStorage.getItem('blocklyStashTime');
    if (storage && parseInt(time) > timestamp) {
        timestamp = time;
        copyData.clear();
        storage.forEach((data) => {
            copyData.add(data);
        });
        connectionDBList.length = 0;
        connection.forEach((data) => {
            connectionDBList.push(data);
        });
    }
};

/**
 * Get blocks number in the clipboard from localStorage.
 * @param {boolean} useCopyPasteCrossTab Whether or not to use
 *     cross tab copy/paste.
 * @returns {number} The number of blocks in the clipboard.
 */
export const blockNumGetFromStorage = function (useCopyPasteCrossTab) {
    if (!useCopyPasteCrossTab) {
        return copyData.size;
    }
    const storage = JSON.parse(localStorage.getItem('blocklyStashMulti'));
    const time = localStorage.getItem('blocklyStashTime');
    if (storage && parseInt(time) > timestamp) {
        return storage.length;
    }
    return copyData.size;
};
