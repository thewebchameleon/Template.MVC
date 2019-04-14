// bootstrap-select library used in conjunction with the aspcore MultiSelect tag helper
$(".selectpicker").on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
    var container = $('#' + e.target.id + '-container');
    container.empty();

    var selectedValues = $(this).val();
    selectedValues.forEach(function (value, index) {
        var elementName = e.target.id.replace('_', '.');
        $(container).append("<input type='hidden' name='" + elementName + "' value=" + value + " />");
    });
});