/**
 * A class that provides methods for indexing and searching blocks.
 */
export class BlockSearcher {
    blockCreationWorkspace = new Blockly.Workspace()
    trigramsToBlocks = new Map()

    /**
     * Populates the cached map of trigrams to the blocks they correspond to.
     *
     * This method must be called before blockTypesMatching(). Behind the
     * scenes, it creates a workspace, loads the specified block types on it,
     * indexes their types and human-readable text, and cleans up after
     * itself.
     *
     * @param blockTypes A list of block types to index.
     */
    indexBlocks(blockTypes) {
        const blockCreationWorkspace = new Blockly.Workspace()
        blockTypes.forEach(blockType => {
            const block = blockCreationWorkspace.newBlock(blockType)
            this.indexBlockText(blockType.replaceAll("_", " "), blockType)
            block.inputList.forEach(input => {
                input.fieldRow.forEach(field => {
                    this.indexDropdownOption(field, blockType)
                    this.indexBlockText(field.getText(), blockType)
                })
            })
        })
    }

    /**
     * Check if the field is a dropdown, and index every text in the option
     *
     * @param field We need to check the type of field
     * @param blockType The block type to associate the trigrams with.
     */
    indexDropdownOption(field, blockType) {
        if (field instanceof Blockly.FieldDropdown) {
            field.getOptions(true).forEach(option => {
                if (typeof option[0] === "string") {
                    this.indexBlockText(option[0], blockType)
                } else if ("alt" in option[0]) {
                    this.indexBlockText(option[0].alt, blockType)
                }
            })
        }
    }

    /**
     * Filters the available blocks based on the current query string.
     *
     * @param query The text to use to match blocks against.
     * @returns A list of block types matching the query.
     */
    blockTypesMatching(query) {
        return [
            ...this.generateTrigrams(query)
                .map(trigram => {
                    return this.trigramsToBlocks.get(trigram) ?? new Set()
                })
                .reduce((matches, current) => {
                    return this.getIntersection(matches, current)
                })
                .values()
        ]
    }

    /**
     * Generates trigrams for the given text and associates them with the given
     * block type.
     *
     * @param text The text to generate trigrams of.
     * @param blockType The block type to associate the trigrams with.
     */
    indexBlockText(text, blockType) {
        this.generateTrigrams(text).forEach(trigram => {
            const blockSet = this.trigramsToBlocks.get(trigram) ?? new Set()
            blockSet.add(blockType)
            this.trigramsToBlocks.set(trigram, blockSet)
        })
    }

    /**
     * Generates a list of trigrams for a given string.
     *
     * @param input The string to generate trigrams of.
     * @returns A list of trigrams of the given string.
     */
    generateTrigrams(input) {
        const normalizedInput = input.toLowerCase()
        if (!normalizedInput) return []
        if (normalizedInput.length <= 3) return [normalizedInput]

        const trigrams = []
        for (let start = 0; start < normalizedInput.length - 3; start++) {
            trigrams.push(normalizedInput.substring(start, start + 3))
        }

        return trigrams
    }

    /**
     * Returns the intersection of two sets.
     *
     * @param a The first set.
     * @param b The second set.
     * @returns The intersection of the two sets.
     */
    getIntersection(a, b) {
        return new Set([...a].filter(value => b.has(value)))
    }
}


/**
 * A toolbox category that provides a search field and displays matching blocks
 * in its flyout.
 */
class ToolboxSearchCategory extends Blockly.ToolboxCategory {
    static START_SEARCH_SHORTCUT = 'startSearch';
    static SEARCH_CATEGORY_KIND = 'search';
    searchField = undefined;
    blockSearcher = new BlockSearcher();

    /**
     * Initializes a ToolboxSearchCategory.
     *
     * @param categoryDef The information needed to create a category in the
     *     toolbox.
     * @param parentToolbox The parent toolbox for the category.
     * @param opt_parent The parent category or null if the category does not have
     *     a parent.
     */
    constructor(
        categoryDef,
        parentToolbox,
        opt_parent
    ) {
        super(categoryDef, parentToolbox, opt_parent);
        this.initBlockSearcher();
        this.registerShortcut();
    }

    /**
     * Initializes the search field toolbox category.
     *
     * @returns The <div> that will be displayed in the toolbox.
     */
    createDom_() {
        const dom = super.createDom_();
        this.searchField = document.createElement('input');
        this.searchField.type = 'search';
        this.searchField.placeholder = 'Search';
        this.workspace_.RTL
            ? (this.searchField.style.marginRight = '8px')
            : (this.searchField.style.marginLeft = '8px');
        this.searchField.addEventListener('keyup', (event) => {
            if (event.key === 'Escape') {
                this.parentToolbox_.clearSelection();
                return true;
            }

            this.matchBlocks();
        });
        this.rowContents_?.replaceChildren(this.searchField);
        return dom;
    }

    /**
     * Returns the numerical position of this category in its parent toolbox.
     *
     * @returns The zero-based index of this category in its parent toolbox, or -1
     *    if it cannot be determined, e.g. if this is a nested category.
     */
    getPosition() {
        const categories = this.workspace_.options.languageTree?.contents || [];
        for (let i = 0; i < categories.length; i++) {
            if (categories[i].kind === ToolboxSearchCategory.SEARCH_CATEGORY_KIND) {
                return i;
            }
        }

        return -1;
    }

    /**
     * Registers a shortcut for displaying the toolbox search category.
     */
    registerShortcut() {
        const shortcut = Blockly.ShortcutRegistry.registry.createSerializedKey(
            Blockly.utils.KeyCodes.S,
            [Blockly.utils.KeyCodes.CTRL],
        );
        Blockly.ShortcutRegistry.registry.register({
            name: ToolboxSearchCategory.START_SEARCH_SHORTCUT,
            callback: () => {
                const position = this.getPosition();
                if (position < 0) return false;
                    Blockly.getFocusManager().focusNode(this);
                return true;
            },
            keyCodes: [shortcut],
        });
    }

    /**
     * Returns a list of block types that are present in the toolbox definition.
     *
     * @param schema A toolbox item definition.
     * @param allBlocks The set of all available blocks that have been encountered
     *     so far.
     */
    getAvailableBlocks(
        schema,
        allBlocks
    ) {
        if ('contents' in schema) {
            schema.contents.forEach((contents) => {
                this.getAvailableBlocks(contents, allBlocks);
            });
        } else if (schema.kind.toLowerCase() === 'block') {
            if ('type' in schema && schema.type) {
                allBlocks.add(schema.type);
            }
        }
    }

    /**
     * Builds the BlockSearcher index based on the available blocks.
     */
    initBlockSearcher() {
        const availableBlocks = new Set();
        this.workspace_.options.languageTree?.contents?.forEach((item) =>
            this.getAvailableBlocks(item, availableBlocks),
        );
        this.blockSearcher.indexBlocks([...availableBlocks]);
    }

    /** See IFocusableNode.getFocusableElement. */
    getFocusableElement() {
        if (!this.searchField) {
            throw new Error('This field currently has no representative DOM element.');
        }
        return this.searchField;
    }

    /** See IFocusableNode.onNodeFocus. */
    onNodeFocus() {
        this.matchBlocks();
    }

    /** See IFocusableNode.onNodeBlur. */
    onNodeBlur() {
        if (!this.searchField) return;
        this.searchField.value = '';
    }

    /**
     * Filters the available blocks based on the current query string.
     */
    matchBlocks() {
        const query = this.searchField ? this.searchField.value : '';

        this.flyoutItems_ = query
            ? this.blockSearcher.blockTypesMatching(query).map((blockType) => {
                return {
                    kind: 'block',
                    type: blockType,
                };
            })
            : [];

        if (!this.flyoutItems_.length) {
            this.flyoutItems_.push({
                kind: 'label',
                text:
                    query.length < 3
                        ? 'Type to search for blocks'
                        : 'No matching blocks found',
            });
        }
        this.parentToolbox_.refreshSelection();
    }

    /**
     * Disposes of this category.
     */
    dispose() {
        super.dispose();
        Blockly.ShortcutRegistry.registry.unregister(
            ToolboxSearchCategory.START_SEARCH_SHORTCUT,
        );
    }
}

Blockly.registry.register(
    Blockly.registry.Type.TOOLBOX_ITEM,
    ToolboxSearchCategory.SEARCH_CATEGORY_KIND,
    ToolboxSearchCategory,
);