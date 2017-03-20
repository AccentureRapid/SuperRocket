$(function () {
    var on_editdialogshown = function (e) {
        $('.datetimepicker').datetimepicker(
        {
            onClose: function () {
                this.focus();
            }
        });
    };

    var ajaxedit = new AjaxEdit({
        searchdialog: '#search_dialog',
        searchdialogform: '#search_dialog_form',
        editdialogform: '#edit_dialog_form',
        tablesection: "#table_section",
        datatable: '#main_table',
        createbutton: '#create_button',
        deletebutton: '.delete-button',
        editbutton: '.edit-button',
        clearbutton: '#clear_button',

        on_editdialogshown: on_editdialogshown,
    });
    ajaxedit.bind();
});

