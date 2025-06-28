/**
 * @license
 * Copyright 2022 MIT
 * SPDX-License-Identifier: Apache-2.0
 */


/**
 * @fileoverview Multiple selection svg switch.
 */


import { blockSelectionWeakMap, inMultipleSelectionModeWeakMap } from './global.js';

/**
 * Width of the multi select controls.
 * @type {number}
 * @const
 */
const WIDTH = 32;

/**
 * Height of the multi select control.
 * @type {number}
 * @const
 */
const HEIGHT = 32;

/**
 * Distance between multi select controls and bottom or top edge of workspace.
 * @type {number}
 * @const
 */
const MARGIN_VERTICAL = 20;

/**
 * Distance between multi select controls and right or left edge of workspace.
 * @type {number}
 */
const MARGIN_HORIZONTAL = 20;

/**
 * Class for a multi select controls.
 * @implements {Blockly.IPositionable}
 */
export class MultiselectControls {
    /**
     * @param {!Blockly.WorkspaceSvg} workspace The workspace to sit in.
     * @param {Object} options The icons configuration.
     */
    constructor(workspace, options) {
        /**
         * Icon path of the multi select controls when enabled.
         * @type {string}
         */
        this.enabled_img =
            'https://github.com/mit-cml/workspace-multiselect' +
            '/raw/main/test/media/select.svg';

        /**
         * Icon path of the multi select controls when disabled.
         * @type {string}
         */
        this.disabled_img =
            'https://github.com/mit-cml/workspace-multiselect' +
            '/raw/main/test/media/unselect.svg';

        /**
         * @type {!Blockly.WorkspaceSvg} The workspace to sit in.
         * @private
         */
        this.workspace_ = workspace;

        /**
         * State of the multi select controls.
         * @type {boolean}
         */
        this.enabled = false;

        /**
         * The unique id for this component that is used to register with the
         * ComponentManager.
         * @type {string}
         */
        this.id = 'multiselectControls';

        /**
         * A handle to use to unbind the mouse down event handler for multi select
         *    button. Opaque data returned from browserEvents.conditionalBind.
         * @type {?Blockly.browserEvents.Data}
         * @private
         */
        this.onMultiselectWrapper_ = null;

        /**
         * The multi select svg <g> element.
         * @type {SVGGElement}
         * @private
         */
        this.multiselectGroup_ = null;

        /**
         * Whether this has been initialized.
         * @type {boolean}
         * @private
         */
        this.initialized_ = false;

        /**
         * Store the status of workspace dragging
         * before entering multi select mode.
         * @type {boolean}
         * @private
         */
        this.hasDisableWorkspaceDrag_ = false;

        /**
         * Store the block that just got unselected.
         * @type {!Blockly.BlockSvg}
         * @private
         */
        this.justUnselectedBlock_ = null;

        if (options && options.enabledIcon) {
            this.enabled_img = options.enabledIcon;
        }

        if (options && options.disabledIcon) {
            this.disabled_img = options.disabledIcon;
        }

        this.blockSelection = blockSelectionWeakMap.get(workspace);
    }

    /**
     * Create the multi select controls.
     * @returns {!SVGElement} The multi select controls SVG group.
     */
    createDom() {
        this.svgGroup_ = Blockly.utils.dom.createSvgElement(
            Blockly.utils.Svg.G, {}, null);
        this.createMultiselectSvg_();
        return this.svgGroup_;
    }

    /**
     * Initializes the multi select controls.
     */
    init(hideIcon, weight) {
        if (!hideIcon) {
            let weightValue = 3;
            if (typeof weight === 'number') {
                weightValue = weight;
            }
            this.workspace_.getComponentManager().addComponent({
                component: this,
                weight: weightValue,
                capabilities: [Blockly.ComponentManager.Capability.POSITIONABLE],
            });
        }
        this.initialized_ = true;
        this.workspace_.resize();
    }

    /**
     * Disposes of this multi select controls.
     * Unlink from all DOM elements to prevent memory leaks.
     */
    dispose() {
        this.disableMultiselect();
        this.workspace_.getComponentManager().removeComponent(this.id);
        if (this.svgGroup_) {
            Blockly.utils.dom.removeNode(this.svgGroup_);
        }
        if (this.onMultiselectWrapper_) {
            Blockly.browserEvents.unbind(this.onMultiselectWrapper_);
        }
    }

    /**
     * Returns the bounding rectangle of the UI element in pixel units relative to
     * the Blockly injection div.
     * @returns {?Blockly.utils.Rect} The UI elements's bounding box. Null if
     *   bounding box should be ignored by other UI elements.
     */
    getBoundingRectangle() {
        const bottom = this.top_ + HEIGHT;
        const right = this.left_ + WIDTH;
        return new Blockly.utils.Rect(this.top_, bottom, this.left_, right);
    }

    /**
     * Positions the multi select controls.
     * It is positioned in the opposite corner to the corner the
     * categories/toolbox starts at.
     * @param {!Blockly.MetricsManager.UiMetrics} metrics The workspace metrics.
     * @param {!Array<!Blockly.utils.Rect>} savedPositions List of rectangles that
     *     are already on the workspace.
     */
    position(metrics, savedPositions) {
        // Not yet initialized.
        if (!this.initialized_) {
            return;
        }

        const cornerPosition =
            Blockly.uiPosition.getCornerOppositeToolbox(this.workspace_, metrics);
        const startRect = Blockly.uiPosition.getStartPositionRect(
            cornerPosition, new Blockly.utils.Size(WIDTH, HEIGHT),
            MARGIN_HORIZONTAL,
            MARGIN_VERTICAL, metrics, this.workspace_);

        const verticalPosition = cornerPosition.vertical;
        const bumpDirection = verticalPosition ===
            Blockly.uiPosition.verticalPosition.TOP ?
            Blockly.uiPosition.bumpDirection.DOWN :
            Blockly.uiPosition.bumpDirection.UP;
        const positionRect = Blockly.uiPosition.bumpPositionRect(
            startRect, MARGIN_VERTICAL, bumpDirection, savedPositions);

        this.top_ = positionRect.top;
        this.left_ = positionRect.left;
        if (this.svgGroup_) {
            this.svgGroup_.setAttribute(
                'transform', 'translate(' + this.left_ + ',' + this.top_ + ')');
        }
    }

    /**
     * Create the multi-select icon and its event handler.
     * @private
     */
    createMultiselectSvg_() {
        /* This markup will be generated and added to the .svgGroup_:
        <g class="blocklyMultiselect">
          <image width="32" height="32" xlink:href="media/unselect.svg"></image>
        </g>
        */
        this.multiselectGroup_ =
            Blockly.utils.dom.createSvgElement(Blockly.utils.Svg.G, {
                'class': 'blocklyMultiselect',
            }, this.svgGroup_);
        const MultiselectSvg = Blockly.utils.dom.createSvgElement(
            Blockly.utils.Svg.IMAGE, {
            'width': WIDTH,
            'height': HEIGHT,
        },
            this.multiselectGroup_);
        MultiselectSvg.setAttributeNS(
            Blockly.utils.dom.XLINK_NS, 'xlink:href', this.disabled_img);

        // Attach event listeners.
        this.onMultiselectWrapper_ = Blockly.browserEvents.conditionalBind(
            this.multiselectGroup_, 'mousedown', null,
            this.switchMultiselect_.bind(this));
    }

    /**
     * Handles a mouse down event on the multi-select button on the workspace.
     * @param {!Event} e A mouse down event.
     * @private
     */
    switchMultiselect_(e) {
        this.workspace_.markFocused();
        this.enabled = !this.enabled;
        // Multiple selection switch.
        if (this.enabled) {
            this.enableMultiselect(true);
        } else {
            this.disableMultiselect(true);
        }
        Blockly.Touch.clearTouchIdentifier(); // Don't block future drags.
        e.stopPropagation(); // Don't start a workspace scroll.
        e.preventDefault(); // Stop double-clicking from selecting text.
    }

    /**
     * Maintain the selected blocks set list when updating.
     * @param {!Blockly.BlockSvg} block The block to update.
     * @private
     */
    updateBlocks_(block) {
        if (block &&
            (block.isDeletable() || block.isMovable())) {
            if (this.blockSelection.has(block.id)) {
                this.blockSelection.delete(block.id);
                this.justUnselectedBlock_ = block;
                block.pathObject.updateSelected(false);
            } else {
                this.blockSelection.add(block.id);
                this.justUnselectedBlock_ = null;
                block.pathObject.updateSelected(true);
                block.bringToFront();
            }
        }
    }

    /**
     * update the multiple selection blocks status.
     */
    updateMultiselect() {
        if (!inMultipleSelectionModeWeakMap.get(this.workspace_)) {
            if (!Blockly.getSelected() || this.workspace_.id !==
                Blockly.getMainWorkspace().id || (Blockly.getSelected() &&
                    !this.blockSelection.has(Blockly.getSelected().id))) {
                // When not in multiple selection mode and Blockly selects a block not
                // in currently selected set or unselects, clear the selected set.
                this.blockSelection.forEach((id) => {
                    const element = this.workspace_.getBlockById(id);
                    if (element) {
                        element.pathObject.updateSelected(false);
                    }
                });
                this.blockSelection.clear();
                if (this.workspace_.id === Blockly.getMainWorkspace().id) {
                    this.updateBlocks_(Blockly.getSelected());
                }
            }
        } else if (this.justUnselectedBlock_ && Blockly.getSelected() &&
            this.workspace_.id === Blockly.getMainWorkspace().id &&
            Blockly.getSelected().id === this.justUnselectedBlock_.id) {
            // Update the Blockly selected block when that block is
            // no longer selected in our set.
            if (this.blockSelection.size) {
                Blockly.common.setSelected(
                    this.workspace_.getBlockById(
                        this.blockSelection.keys().next().value));
            } else {
                Blockly.common.setSelected(null);
            }
            this.justUnselectedBlock_.pathObject.updateSelected(false);
            this.justUnselectedBlock_ = null;
        }

        // Update the selection highlight.
        this.blockSelection.forEach((id) => {
            const element = this.workspace_.getBlockById(id);
            if (element) {
                element.pathObject.updateSelected(true);
            }
        });
    }

    /**
     * Enable the multiple select mode.
     * @param {!boolean} byIcon Whether to simulate a keyboard event.
     */
    enableMultiselect(byIcon = false) {
        // Ensure that we only restore drag to move the workspace behavior
        // when it is enabled.
        if (!this.hasDisableWorkspaceDrag_ &&
            this.workspace_.options.moveOptions &&
            this.workspace_.options.moveOptions.drag) {
            this.workspace_.options.moveOptions.drag = false;
            this.hasDisableWorkspaceDrag_ = true;
        }
        this.dragSelect_ = new DragSelect({
            selectables: this.workspace_.getInjectionDiv()
                .querySelectorAll(
                    'g.blocklyDraggable:not(.blocklyInsertionMarker)' +
                    '> path.blocklyPath'),
            area: this.workspace_.svgGroup_,
            multiselectMode: true,
            draggability: false,
            usePointerEvents: true,
        });
        // Filter out the parent block when selecting child blocks
        // to mitigate the invisible rectangles issue.
        const filterParent = (list, rect) => {
            const toRemove = [];
            for (const [parent, parentRect] of list.entries()) {
                for (const [child, childRect] of list.entries()) {
                    if (parent === child ||
                        !DragSelect.isCollision(childRect, parentRect, 0) ||
                        parent.parentNode === null || child.parentNode === null ||
                        parent.parentNode === child.parentNode) continue;
                    else if (parent.parentNode.contains(child.parentNode) &&
                        // Continue to select if user draws a rectangle that
                        // covers more than the child itself
                        DragSelect.isCollision(rect, childRect, 1)) {
                        toRemove.push(parent);
                        break;
                    }
                }
            }
            return toRemove;
        };
        this.dragSelect_.Selection.filterSelected = (
            { selectorRect, select: _select, unselect: _unselect }) => {
            const select = _select; const unselect = _unselect;
            const toRemove = filterParent(select, selectorRect);
            toRemove.forEach((el) => {
                const rect = select.get(el);
                select.delete(el);
                unselect.set(el, rect);
            });
            return { select, unselect };
        };

        this.dragSelect_.subscribe('elementselect', (info) => {
            const element = info.item.parentElement;
            if (inMultipleSelectionModeWeakMap.get(this.workspace_) &&
                element && element.dataset && element.dataset.id) {
                this.updateBlocks_(
                    this.workspace_.getBlockById(element.dataset.id));
            }
        });
        this.dragSelect_.subscribe('elementunselect', (info) => {
            const element = info.item.parentElement;
            if (inMultipleSelectionModeWeakMap.get(this.workspace_) &&
                element && element.dataset && element.dataset.id) {
                this.updateBlocks_(
                    this.workspace_.getBlockById(element.dataset.id));
            }
        });
        if (byIcon) {
            this.workspace_.getInjectionDiv().dispatchEvent(
                new KeyboardEvent('keydown', { 'key': 'meta' }));
        }
        this.updateMultiselectIcon(true);
        inMultipleSelectionModeWeakMap.set(this.workspace_, true);
    }

    /**
     * Disable the multiple select mode.
     * @param {!boolean} byIcon Whether to simulate a keyboard event.
     */
    disableMultiselect(byIcon = false) {
        inMultipleSelectionModeWeakMap.set(this.workspace_, false);
        if (this.dragSelect_) {
            if (byIcon) {
                this.workspace_.getInjectionDiv().dispatchEvent(
                    new KeyboardEvent('keyup', { 'key': 'meta' }));
            }
            this.dragSelect_.stop();
            this.dragSelect_ = null;
        }
        // Ensure that at least Blockly select one of the blocks in the
        // selection set, or clear the Blockly selection if our set is empty.
        if (!this.blockSelection.size && Blockly.getSelected()) {
            Blockly.common.setSelected(null);
        } else if (this.blockSelection.size && !Blockly.getSelected()) {
            Blockly.common.setSelected(
                this.workspace_.getBlockById(
                    this.blockSelection.keys().next().value));
        }
        if (this.hasDisableWorkspaceDrag_) {
            this.workspace_.options.moveOptions.drag = true;
            this.hasDisableWorkspaceDrag_ = false;
        }
        this.updateMultiselectIcon(false);
    }

    /**
     * Updates the multi select icon.
     * @param {boolean} enable Whether the multi select is enabled.
     */
    updateMultiselectIcon(enable) {
        if (!this.multiselectGroup_) {
            return;
        }
        this.enabled = enable;
        if (enable) {
            this.multiselectGroup_.firstElementChild.setAttributeNS(
                Blockly.utils.dom.XLINK_NS, 'xlink:href', this.enabled_img);
        } else {
            this.multiselectGroup_.firstElementChild.setAttributeNS(
                Blockly.utils.dom.XLINK_NS, 'xlink:href', this.disabled_img);
        }
    }
}

/**
 * CSS for multi select controls.  See css.js for use.
 */
Blockly.Css.register(`
.blocklyMultiselect>image, .blocklyMultiselect>svg>image {
  opacity: .2;
}

.blocklyMultiselect>image:hover, .blocklyMultiselect>svg>image:hover {
  opacity: .4;
}

.blocklyMultiselect>image:active, .blocklyMultiselect>svg>image:active {
  opacity: .6;
}
`);
