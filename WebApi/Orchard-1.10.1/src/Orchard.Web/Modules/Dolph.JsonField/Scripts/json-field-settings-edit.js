(function ($) {
    var container = document.getElementById("jsoneditor"),
        hiddenInput = $("#jsonvalue"),
        initialValue = hiddenInput.val(),
        useTemplate = $('#useTemplate');

    var options = {
        change: function () {
            hiddenInput.val(JSON.stringify(editor.get()));
        },
        modes: ['tree', 'text'],
        mode: 'tree'
    };
    var editor = new JSONEditor(container, options);

    if (initialValue != '') {
        editor.setText(initialValue)
    }
})(jQuery);