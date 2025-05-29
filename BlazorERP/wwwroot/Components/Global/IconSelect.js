window.getSelectIndex = function (element) {
    if (element && element.selectedIndex !== undefined) {
        return element.selectedIndex;
    }
    return -1; // Return -1 if the element is not found or does not have a selectedIndex
}

window.initSelect2WithIcons = function (selector) {
    $(selector).select2({
        templateResult: formatState,
        templateSelection: formatState,
        minimumResultsForSearch: Infinity
    });

    function formatState(state) {
        if (!state.id) {
            return state.text;
        }
        const iconName = $(state.element).data("icon");
        return $('<span><span class="material-icons" style="vertical-align: middle; margin-right: 0.5rem;">' + iconName + '</span>' + state.text + '</span>');
    }
};