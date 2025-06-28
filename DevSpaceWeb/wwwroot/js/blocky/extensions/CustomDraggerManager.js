/**
 * @license
 * Copyright 2022 MIT
 * SPDX-License-Identifier: Apache-2.0
 */

/**
 * @fileoverview Multiple selection dragger.
 */

import { AutoScroll } from './AutoScroll.js';


import {
    blockSelectionWeakMap, inMultipleSelectionModeWeakMap,
    hasSelectedParent, BaseBlockDraggerWeakMap
} from '../plugins/multi/global.js';

let CandidateScrolls;

export let EdgeScrollOptions;

/** @type {!EdgeScrollOptions} */
const defaultOptions = {
    slowBlockSpeed: 0.28,
    fastBlockSpeed: 1.4,
    slowBlockStartDistance: 0,
    fastBlockStartDistance: 50,
    oversizeBlockThreshold: 0.85,
    oversizeBlockMargin: 5,
    slowMouseSpeed: 0.5,
    fastMouseSpeed: 1.6,
    slowMouseStartDistance: 0,
    fastMouseStartDistance: 35,
};

/**
 * A block dragger that adds the functionality for multiple block to
 * be moved while someone is dragging it.
 */
export class CustomDraggerManager extends Blockly.BlockDragger {
    /** @override */
    constructor(block, workspace) {
        super(block, workspace);
        this.block_ = block;
        this.workspace_ = workspace;
        this.group_ = '';
        this.blockDraggers_ = new Set();
        this.origHighlight_ = Blockly.RenderedConnection.prototype.highlight;
        this.BaseBlockDragger = BaseBlockDraggerWeakMap.get(workspace);
        this.origUpdateBlockAfterMove_ =
            this.BaseBlockDragger.prototype.updateBlockAfterMove_;
        this.blockSelection = blockSelectionWeakMap.get(workspace);

        /**
     * How much the block has been moved due to scrolling.
     * @type {!Blockly.utils.Coordinate}
     * @protected
     */
        this.scrollDelta_ = new Blockly.utils.Coordinate(0, 0);

        /**
         * How much the block has been moved due to dragging.
         * @type {!Blockly.utils.Coordinate}
         * @protected
         */
        this.dragDelta_ = new Blockly.utils.Coordinate(0, 0);

        // TODO(maribethb): Use `isMoveable` etc. to get this list
        /**
         * Possible directions the workspace could be scrolled.
         * @type {!Array<string>}
         * @protected
         */
        this.scrollDirections_ = ['top', 'bottom', 'left', 'right'];
    }

    /**
     * Dispose of this block dragger.
     */
    dispose() {
        super.dispose();
        this.blockDraggers_.forEach((blockDragger) => {
            blockDragger.dispose();
        });
    }

    /**
     * Prepares the block dragger for a new drag.
     * @param {!Blockly.utils.Coordinate} currentDragDeltaXY How far the pointer
     *     has moved from the position at the start of the drag, in pixel units.
     * @param {boolean} healStack Whether or not to heal the stack after
     *     disconnecting.
     */
    startDrag(currentDragDeltaXY, healStack) {
        if (inMultipleSelectionModeWeakMap.get(this.workspace_)) {
            return;
        }

        const blockDraggerList = [];
        if (this.blockSelection.has(this.block_.id)) {
            this.blockSelection.forEach((id) => {
                const element = this.workspace_.getBlockById(id);
                if (!element) {
                    this.blockSelection.delete(id);
                    return;
                }
                // Only drag the parent if it is selected.
                if (hasSelectedParent(element, true)) {
                    return;
                }
                blockDraggerList.push(new this.BaseBlockDragger(element,
                    this.workspace_));
            });
        } else {
            // Dragging a new block that not in the selection list would
            // clear the multiple selections and only drag that block.
            this.blockSelection.forEach((id) => {
                const element = this.workspace_.getBlockById(id);
                if (element) {
                    element.pathObject.updateSelected(false);
                }
            });
            this.blockSelection.clear();
            blockDraggerList.push(new this.BaseBlockDragger(this.block_,
                this.workspace_));
            this.block_.pathObject.updateSelected(true);
        }

        if (blockDraggerList.length > 1) {
            // Disabled the highlighting around connection for multiple blocks
            // dragging because of the bugs.
            Blockly.RenderedConnection.prototype.highlight = function () { };
        }

        blockDraggerList.forEach((blockDragger) => {
            blockDragger.startDrag(currentDragDeltaXY, healStack);
            this.blockDraggers_.add(blockDragger);
        });

        const totalDelta = Blockly.utils.Coordinate.sum(
            this.scrollDelta_,
            currentDragDeltaXY,
        );
        super.startDrag(totalDelta, healStack);
        this.dragDelta_ = currentDragDeltaXY;
    }

    /**
     * Moves the block to the specified location.
     * @param {!Event} e The mouseup/touchend event.
     * @param {!Blockly.utils.Coordinate} currentDragDeltaXY How far the pointer
     *     has moved from the position at the start of the drag, in pixel units.
     */
    drag(e, currentDragDeltaXY) {
        this.blockDraggers_.forEach(function (blockDragger_) {
            blockDragger_.drag(e, currentDragDeltaXY);
        });

        const totalDelta = Blockly.utils.Coordinate.sum(
            this.scrollDelta_,
            currentDragDeltaXY,
        );
        super.drag(e, totalDelta);
        this.dragDelta_ = currentDragDeltaXY;

        if (ScrollBlockDragger.edgeScrollEnabled) {
            this.scrollWorkspaceWhileDragging_(e);
        }

        e.preventDefault();
        e.stopPropagation();
    }

    /**
     * Finishes the block drag.
     * @param {!Event} e The mouseup/touchend event.
     * @param {!Blockly.utils.Coordinate} currentDragDeltaXY How far the pointer
     *     has moved from the position at the start of the drag, in pixel units.
     */
    endDrag(e, currentDragDeltaXY) {
        if (this.blockDraggers_.size > 1) {
            this.patchUpdateBlockAfterMove(true);
        }
        this.blockDraggers_.forEach((blockDragger_) => {
            if (Blockly.Events.getGroup()) {
                this.group_ = Blockly.Events.getGroup();
            } else {
                Blockly.Events.setGroup(this.group_);
            }
            blockDragger_.endDrag(e, currentDragDeltaXY);
        });
        if (this.blockDraggers_.size > 1) {
            // Restore the highlighting around connection for multiple blocks.
            Blockly.RenderedConnection.prototype.highlight = this.origHighlight_;
            this.patchUpdateBlockAfterMove(false);
        }

        super.endDrag(e, currentDragDeltaXY);

        this.stopAutoScrolling();
    }

    /**
     * Updates the location of the block that is being dragged.
     * @param {number} deltaX Horizontal offset in pixel units.
     * @param {number} deltaY Vertical offset in pixel units.
     */
    moveBlockWhileDragging(deltaX, deltaY) {
        this.blockDraggers_.forEach(function (blockDragger_) {
            blockDragger_.moveBlockWhileDragging(deltaX, deltaY);
        });

        /* Scroll dragger */
        this.scrollDelta_.x -= deltaX;
        this.scrollDelta_.y -= deltaY;

        // The total amount the block has moved since being picked up.
        const totalDelta = Blockly.utils.Coordinate.sum(
            this.scrollDelta_,
            this.dragDelta_,
        );

        const delta = this.pixelsToWorkspaceUnits_(totalDelta);
        const newLoc = Blockly.utils.Coordinate.sum(this.startXY_, delta);

        // Make the block stay under the cursor.
        this.draggingBlock_.moveDuringDrag(newLoc);

        this.dragIcons_(totalDelta);

        // As we scroll, show the insertion markers.
        this.draggedConnectionManager_.update(
            new Blockly.utils.Coordinate(
                totalDelta.x / this.workspace_.scale,
                totalDelta.y / this.workspace_.scale,
            ),
            null,
        );
    }

    /**
     * Patch the this.BaseBlockDragger.updateBlockAfterMove_ function.
     * @param {boolean} on To start the patch or restore.
     */
    patchUpdateBlockAfterMove(on) {
        if (!on) {
            this.BaseBlockDragger.prototype.updateBlockAfterMove_ =
                this.origUpdateBlockAfterMove_;
        } else {
            this.BaseBlockDragger.prototype.updateBlockAfterMove_ = function () {
                this.fireMoveEvent_();
                if (this.draggedConnectionManager_.wouldConnectBlock()) {
                    // We have to ensure that we can't connect to a block
                    // that is in dragging.
                    const closest = this.draggedConnectionManager_.activeCandidate
                        .closest;
                    if (!blockSelectionWeakMap.get(this.workspace_).has(
                        closest.sourceBlock_.id) &&
                        !hasSelectedParent(closest.sourceBlock_, true)) {
                        // Applying connections also rerenders the relevant blocks.
                        this.draggedConnectionManager_.applyConnections();
                    } else {
                        // We have to hide preview if any.
                        // Don't fire events for insertion markers.
                        Blockly.Events.disable();
                        this.draggedConnectionManager_.hidePreview();
                        Blockly.Events.enable();
                    }
                }

                // TODO: As App Inventor uses a different rendering
                // algorithm than base Blockly, we will have to verify
                // if this is still OKay/neccessary.

                /**
                 * Fix when you drag the selected children blocks from their
                 * unselected parent the children blocks of the selected ones
                 * can be out of the position while still connected
                 * (do should remain connected).
                 *
                 * Each time you render a block it rerenders all of that block's
                 * parents as well.
                 */
                this.draggingBlock_.getDescendants(false).forEach(function (block) {
                    if (!block.getChildren().length) {
                        block.queueRender();
                    }
                });

                this.draggingBlock_.scheduleSnapAndBump();
            };
        }
    }

    /**
     * Get a list of the insertion markers that currently exist.  Drags have 0, 1,
     * or 2 insertion markers.
     * @returns {!Array<!Blockly.BlockSvg>} A possibly empty list of insertion
     *     marker blocks.
     */
    getInsertionMarkers() {
        const insertionMarkers = [];
        this.blockDraggers_.forEach(function (blockDragger_) {
            insertionMarkers.push(...blockDragger_.getInsertionMarkers());
        });
        return insertionMarkers;
    }


    /**
   * Passes the total amount the block has moved (both from dragging and from
   * scrolling) since it was picked up.
   * @override
   */
    getNewLocationAfterDrag_(currentDragDeltaXY) {
        const newValues = {};
        const totalDelta = Blockly.utils.Coordinate.sum(
            this.scrollDelta_,
            currentDragDeltaXY,
        );
        newValues.delta = this.pixelsToWorkspaceUnits_(totalDelta);
        newValues.newLocation = Blockly.utils.Coordinate.sum(
            this.startXY_,
            newValues.delta,
        );

        return newValues;
    }

    /**
   * May scroll the workspace as a block is dragged.
   * If a block is dragged near the edge of the workspace, this method will
   * cause the workspace to scroll in the direction the block is being
   * dragged. The workspace will not resize as the block is dragged. The
   * workspace should appear to move out from under the block, i.e., the block
   * should stay under the user's mouse.
   * @param {!Event} e The mouse/touch event for the drag.
   * @protected
   */
    scrollWorkspaceWhileDragging_(e) {
        /**
         * Unit vector for each direction that could be scrolled. This vector will
         * be scaled to get the calculated velocity in each direction. Must be a
         * dict because the properties are accessed based on the members of
         * `this.scrollDirections_`.
         * @dict
         * @private
         */
        this.SCROLL_DIRECTION_VECTORS_ = {
            top: new Blockly.utils.Coordinate(0, 1),
            bottom: new Blockly.utils.Coordinate(0, -1),
            left: new Blockly.utils.Coordinate(1, 0),
            right: new Blockly.utils.Coordinate(-1, 0),
        };
        const mouse = Blockly.utils.svgMath.screenToWsCoordinates(
            this.workspace_,
            new Blockly.utils.Coordinate(e.clientX, e.clientY),
        );

        /**
         * List of possible scrolls in each direction. This will be modified in
         * place. Must be a dict because the properties are accessed based on the
         * members of `this.scrollDirections_`.
         * @dict
         * @type {!CandidateScrolls}
         */
        const candidateScrolls = {
            top: [],
            bottom: [],
            left: [],
            right: [],
        };

        // Get ViewMetrics in workspace coordinates.
        const viewMetrics = this.workspace_
            .getMetricsManager()
            .getViewMetrics(true);

        // Get possible scroll velocities based on the location of both the block
        // and the mouse.

        this.computeBlockCandidateScrolls_(candidateScrolls, viewMetrics, mouse);
        this.computeMouseCandidateScrolls_(candidateScrolls, viewMetrics, mouse);
        // Calculate the final scroll vector we should actually use.
        const overallScrollVector = this.getOverallScrollVector_(candidateScrolls);

        // If the workspace should not be scrolled any longer, cancel the
        // autoscroll.
        if (
            Blockly.utils.Coordinate.equals(
                overallScrollVector,
                new Blockly.utils.Coordinate(0, 0),
            )
        ) {
            this.stopAutoScrolling();
            return;
        }

        // Update the autoscroll or start a new one.
        this.activeAutoScroll_ =
            this.activeAutoScroll_ || new AutoScroll(this.workspace_);
        this.activeAutoScroll_.updateProperties(overallScrollVector);
    }

    /**
   * There could be multiple candidate scrolls for each direction, such as one
   * for block position and one for mouse position. We should first find the
   * fastest scroll in each direction. Then, we sum those to find the overall
   * scroll vector.
   *
   * For example, we may have a fast block scroll and a slow
   * mouse scroll candidate in both the top and left directions. First, we
   * reduce to only the fast block scroll. Then, we sum the vectors in each
   * direction to get a resulting fast scroll in a diagonal direction to the
   * top left.
   * @param {!CandidateScrolls} candidateScrolls Existing lists of candidate
   *     scrolls. Will be modified in place.
   * @returns {!Blockly.utils.Coordinate} Overall scroll vector.
   * @protected
   */
    getOverallScrollVector_(candidateScrolls) {
        let overallScrollVector = new Blockly.utils.Coordinate(0, 0);
        for (const direction of this.scrollDirections_) {
            const fastestScroll = candidateScrolls[direction].reduce(
                (fastest, current) => {
                    if (!fastest) {
                        return current;
                    }
                    return Blockly.utils.Coordinate.magnitude(fastest) >
                        Blockly.utils.Coordinate.magnitude(current)
                        ? fastest
                        : current;
                },
                new Blockly.utils.Coordinate(0, 0),
            ); // Initial value
            overallScrollVector = Blockly.utils.Coordinate.sum(
                overallScrollVector,
                fastestScroll,
            );
        }
        return overallScrollVector;
    }

    /**
   * Gets the candidate scrolls based on the position of the block on the
   * workspace. If the block is near/over the edge, a candidate scroll will be
   * added based on the options provided.
   *
   * This method can be overridden to further customize behavior, e.g. To add
   * a third speed option.
   * @param {!CandidateScrolls} candidateScrolls Existing list of candidate
   *     scrolls. Will be modified in place.
   * @param {!Blockly.MetricsManager.ContainerRegion} viewMetrics View metrics
   *     for the workspace.
   * @param {!Blockly.utils.Coordinate} mouse Mouse coordinates.
   * @protected
   */
    computeBlockCandidateScrolls_(candidateScrolls, viewMetrics, mouse) {
        const blockOverflows = this.getBlockBoundsOverflows_(viewMetrics, mouse);
        for (const direction of this.scrollDirections_) {
            const overflow = blockOverflows[direction];
            if (overflow > ScrollBlockDragger.options.slowBlockStartDistance) {
                const speed =
                    overflow > ScrollBlockDragger.options.fastBlockStartDistance
                        ? ScrollBlockDragger.options.fastBlockSpeed
                        : ScrollBlockDragger.options.slowBlockSpeed;
                const scrollVector = this.SCROLL_DIRECTION_VECTORS_[direction]
                    .clone()
                    .scale(speed);
                candidateScrolls[direction].push(scrollVector);
            }
        }
    }

    /**
   * Gets the candidate scrolls based on the position of the mouse cursor
   * relative to the workspace. If the mouse is near/over the edge, a
   * candidate scroll will be added based on the options provided.
   *
   * This method can be overridden to further customize behavior, e.g. To add
   * a third speed option.
   * @param {!CandidateScrolls} candidateScrolls Existing list of candidate
   *     scrolls. Will be modified in place.
   * @param {!Blockly.MetricsManager.ContainerRegion} viewMetrics View metrics
   *     for the workspace.
   * @param {!Blockly.utils.Coordinate} mouse Mouse coordinates.
   * @protected
   */
    computeMouseCandidateScrolls_(candidateScrolls, viewMetrics, mouse) {
        const mouseOverflows = this.getMouseOverflows_(viewMetrics, mouse);
        for (const direction of this.scrollDirections_) {
            const overflow = mouseOverflows[direction];
            if (overflow > ScrollBlockDragger.options.slowMouseStartDistance) {
                const speed =
                    overflow > ScrollBlockDragger.options.fastMouseStartDistance
                        ? ScrollBlockDragger.options.fastMouseSpeed
                        : ScrollBlockDragger.options.slowMouseSpeed;
                const scrollVector = this.SCROLL_DIRECTION_VECTORS_[direction]
                    .clone()
                    .scale(speed);
                candidateScrolls[direction].push(scrollVector);
            }
        }
    }

    /**
     * Gets the amount of overflow of a box relative to the workspace viewport.
     *
     * The value for each direction will be how far the given block edge is from
     * the given edge of the viewport. If the block edge is outside the
     * viewport, the value will be positive. If the block edge is inside the
     * viewport, the value will be negative.
     *
     * This method also checks for oversized blocks. If the block is very large
     * relative to the viewport size, then we will actually use a small zone
     * around the cursor, rather than the edge of the block, to calculate the
     * overflow values. This calculation is done independently in both the
     * horizontal and vertical directions. These values can be configured in the
     * options for the plugin.
     * @param {!Blockly.MetricsManager.ContainerRegion} viewMetrics View metrics
     *     for the workspace.
     * @param {!Blockly.utils.Coordinate} mouse Mouse coordinates.
     * @returns {!Object<string, number>} An object describing the amount of
     *     overflow in each direction.
     * @protected
     */
    getBlockBoundsOverflows_(viewMetrics, mouse) {
        const blockBounds = this.draggingBlock_.getBoundingRectangle();

        // Handle large blocks. If the block is nearly as tall as the viewport,
        // use a margin around the cursor rather than the height of the block.
        const blockHeight = blockBounds.bottom - blockBounds.top;
        if (
            blockHeight >
            viewMetrics.height * ScrollBlockDragger.options.oversizeBlockThreshold
        ) {
            blockBounds.top = Math.max(
                blockBounds.top,
                mouse.y - ScrollBlockDragger.options.oversizeBlockMargin,
            );
            blockBounds.bottom = Math.min(
                blockBounds.bottom,
                mouse.y + ScrollBlockDragger.options.oversizeBlockMargin,
            );
        }

        // Same logic, but for block width.
        const blockWidth = blockBounds.right - blockBounds.left;
        if (
            blockWidth >
            viewMetrics.width * ScrollBlockDragger.options.oversizeBlockThreshold
        ) {
            blockBounds.left = Math.max(
                blockBounds.left,
                mouse.x - ScrollBlockDragger.options.oversizeBlockMargin,
            );
            blockBounds.right = Math.min(
                blockBounds.right,
                mouse.x + ScrollBlockDragger.options.oversizeBlockMargin,
            );
        }

        // The coordinate system is negative in the top and left directions, and
        // positive in the bottom and right directions. Therefore, the direction
        // of the comparison must be switched for bottom and right.
        return {
            top: viewMetrics.top - blockBounds.top,
            bottom: -(viewMetrics.top + viewMetrics.height - blockBounds.bottom),
            left: viewMetrics.left - blockBounds.left,
            right: -(viewMetrics.left + viewMetrics.width - blockBounds.right),
        };
    }

    /**
     * Gets the amount of overflow of the mouse coordinates relative to the
     * viewport.
     *
     * The value for each direction will be how far the pointer is from
     * the given edge of the viewport. If the pointer is outside the viewport,
     * the value will be positive. If the pointer is inside the viewport, the
     * value will be negative.
     * @param {!Blockly.MetricsManager.ContainerRegion} viewMetrics View metrics
     *     for the workspace.
     * @param {!Blockly.utils.Coordinate} mouse Mouse coordinates.
     * @returns {!Object<string, number>} An object describing the amount of
     *     overflow in each direction.
     * @protected
     */
    getMouseOverflows_(viewMetrics, mouse) {
        // The coordinate system is negative in the top and left directions, and
        // positive in the bottom and right directions. Therefore, the direction
        // of the comparison must be switched for bottom and right.
        return {
            top: viewMetrics.top - mouse.y,
            bottom: -(viewMetrics.top + viewMetrics.height - mouse.y),
            left: viewMetrics.left - mouse.x,
            right: -(viewMetrics.left + viewMetrics.width - mouse.x),
        };
    }

    /**
     * Cancel any AutoScroll. This must be called when there is no need to
     * scroll further, e.g., when no longer dragging near the edge of the
     * workspace, or when no longer dragging at all.
     */
    stopAutoScrolling() {
        if (this.activeAutoScroll_) {
            this.activeAutoScroll_.stopAndDestroy();
        }
        this.activeAutoScroll_ = null;
    }
}

/**
* Whether the behavior to scroll the workspace when a block is dragged near
* the edge is enabled.
* @type {boolean}
*/
ScrollBlockDragger.edgeScrollEnabled = true;

/**
 * Configuration options for the scroll-options settings.
 * @type {!EdgeScrollOptions}
 */
ScrollBlockDragger.options = defaultOptions;

/**
 * Update the scroll options. Only the properties actually included in the
 * `options` parameter will be set. Any unspecified options will use the
 * previously set value (where the initial value is from `defaultOptions`).
 * Therefore, do not pass in any options with explicit `undefined` or `null`
 * values. The plugin will break. Just leave them out of the object if you
 * don't want to change the default value.
 *
 * This method is safe to call multiple times. Subsequent calls will add onto
 * previous calls, not completely overwrite them. That is, if you call this
 * with:
 *
 *     `updateOptions({fastMouseSpeed: 5});
 *     updateOptions({slowMouseSpeed: 2});`.
 *
 * Then the final options used will include both `fastMouseSpeed: 5` and
 * `slowMouseSpeed: 2` with all other options being the default values.
 * @param {!EdgeScrollOptions} options Object containing any or all of
 *     the available options. Any properties not present will use the existing
 *     value.
 */
ScrollBlockDragger.updateOptions = function (options) {
    ScrollBlockDragger.options = { ...ScrollBlockDragger.options, ...options };
};

/**
 * Resets the options object to the default options.
 */
ScrollBlockDragger.resetOptions = function () {
    ScrollBlockDragger.options = defaultOptions;
};