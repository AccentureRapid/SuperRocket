(function ($) {
    var container = document.getElementById("jsoneditor"),
        hiddenInput = $("#jsonvalue"),
        initialValue = hiddenInput.val(),
        useTemplate = $('#useTemplate');

    var modes = ['tree'];

    if (window.dolphJsonUpdateValuesOnly === 'True' && window.dolphJsonCanEditJsonText === 'True') {
        var modes = ['form', 'text'];
    }
    else if (window.dolphJsonUpdateValuesOnly === 'False' && window.dolphJsonCanEditJsonText === 'True') {
        var modes = ['tree', 'text'];
    }
    else if (window.dolphJsonUpdateValuesOnly === 'True' && window.dolphJsonCanEditJsonText === 'False') {
        var modes = ['form'];
    }

    if (useTemplate.length) {
        useTemplate.on('click', function (e) {
            e.preventDefault();
            editor.setText(window.dolphJsonTemplate);
        });
    }

    var options = {
        change: function () {
            hiddenInput.val(JSON.stringify(editor.get()));
        },
        modes: modes,
        mode: modes.slice(0, 1)
    };
    var editor = new JSONEditor(container, options);

    if (initialValue != '') {
        editor.setText(initialValue)
    }
})(jQuery);