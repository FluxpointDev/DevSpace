class CustomCategory extends Blockly.ToolboxCategory {
    /**
     * Constructor for a custom category.
     * @override
     */
    constructor(categoryDef, toolbox, opt_parent) {
        super(categoryDef, toolbox, opt_parent);
    }

    createIconDom_() {
        const img = document.createElement('span');
        if (img) {


            img.classList.add('iconify');
            img.setAttribute('data-icon', this.toolboxItemDef_.icon);
            return img;
        }
        return super.createIconDom_();
    }
}