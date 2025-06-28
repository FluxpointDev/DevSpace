
import {
    blockSelectionWeakMap, inMultipleSelectionModeWeakMap,
    hasSelectedParent, BaseBlockDraggerWeakMap
} from './global.js';

/**
 * A block dragger that adds the functionality for multiple block to
 * be moved while someone is dragging it.
 */
export class MultiselectBlockDragger extends Blockly.BlockDragger {
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
}

Blockly.registry.register(Blockly.registry.Type.BLOCK_DRAGGER,
    'MultiselectBlockDragger', MultiselectBlockDragger);
