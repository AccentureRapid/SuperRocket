!function (global) {
    'use strict';

    var previousAjaxEdit = global.AjaxEdit;

    function AjaxEdit(options) {
        this.options = options || {};

        var show_editdialog = function (href, updatesection, triggerbutton, editdialogform, updatedatatable, oneditdialogshown) {
            $.get(href, { p: 'json' }, function (data) {

                var $dialog = $($.trim(data));

                $dialog.on('hidden.bs.modal', function (e) {
                    $dialog.remove();
                });

                $dialog.on('shown.bs.modal', function (e) {
                    if (oneditdialogshown) {
                        oneditdialogshown(e);
                    }
                });

                $dialog.modal("show");

                if (createbutton_editting) {
                    triggerbutton.attr("disabled", false);
                    createbutton_editting = false;
                }

                var $edit_dialog_form = $dialog.find(editdialogform);
                var edit_dialog_form_submitting = false;
                $edit_dialog_form.submit(function (e) {
                    if (!edit_dialog_form_submitting) {
                        edit_dialog_form_submitting = true;
                        $edit_dialog_form.find("button[type=submit]").prop('disabled', true);
                        $edit_dialog_form.ajaxSubmit({
                            dataType: ajaxsubmit_datatype,
                            accept: ajaxsubmit_accept,
                            headers: { Accept: ajaxsubmit_accept },

                            data: { p: 'json' },
                            success: function (data) {
                                if ((data == "OK") || (data == "\"OK\"")) {
                                    updatedatatable(updatesection);
                                    $dialog.modal("hide");
                                }
                                else {
                                    bootbox.alert(global.AjaxEdit.internal_system_error_message || "An internal system error occurred.");
                                    var $dialog_data = $($.trim(data));
                                    var $dialog_body = $dialog.find(".modal-body");
                                    var $dialog_data_body = $dialog_data.find(".modal-body");
                                    $dialog_body.html($dialog_data_body.html());
                                }
                                $edit_dialog_form.find("button[type=submit]").prop('disabled', false);
                                edit_dialog_form_submitting = false;
                            },
                            error: function (xhr) {
                                var data = getAjaxResponseText(xhr);
                                if (data) {
                                    bootbox.alert(global.AjaxEdit.internal_system_error_message || "An internal system error occurred.");
                                    var $dialog_data = $($.trim(data));
                                    var $dialog_body = $dialog.find(".modal-body");
                                    var $dialog_data_body = $dialog_data.find(".modal-body");
                                    $dialog_body.html($dialog_data_body.html());
                                }
                                else {
                                    bootbox.alert(global.AjaxEdit.internal_system_error_message || "An internal system error occurred.");
                                }
                                $edit_dialog_form.find("button[type=submit]").prop('disabled', false);
                                edit_dialog_form_submitting = false;
                            }
                        });
                    }
                    e.preventDefault();
                });
            });
        };

        var update_datatable = function (updatesection, updateurl, searchdialogform, updatetablehtml) {
            if (!updateurl) {
                var $search_dialog_form = $(searchdialogform);
                $search_dialog_form.submit();
            }
            else {
                var $updatesection = $(updatesection);
                updateurl = updateurl || $updatesection.data("updateurl");
                $.get(updateurl, $.url('?p', updateurl) ? {} : { p: 'json' }, function (data) {
                    updatetablehtml($updatesection, data);
                });
            }
        };


        var bind_deletebutton = function (deletebutton, updatedatatable) {
            deletebutton.off('click');
            deletebutton.on('click', function (e) {

                var $button = $(this);
                var $currentrow = $button.closest("tr");
                $currentrow.addClass("currentrow");

                var deleteurl = $(this).attr('href');
                var updatesection = $(this).data("updatesection");
                var message = global.AjaxEdit.delete_confirm_message || "Are you sure you want to delete the selected data?";
                bootbox.confirm(message, function (result) {
                    if (result) {
                        $.get(deleteurl, { p: 'json' }, function (data) {
                            updatedatatable(updatesection);
                        });
                    }
                    $currentrow.removeClass("currentrow");
                });
                e.preventDefault();
            });
        };

        var bind_editbutton = function (editbutton, showeditdialog) {
            editbutton.off('click');
            editbutton.on('click', function (e) {
                var href = $(this).attr('href');
                var updatesection = $(this).data("updatesection");
                showeditdialog(href, updatesection);
                e.preventDefault();
            });
        };

        var bind_pagerbutton = function (pagerbutton, updatedatatable, tablesection) {
            pagerbutton.off('click');
            pagerbutton.on('click', function (e) {
                var href = $(this).attr('href');
                updatedatatable(tablesection, href);
                e.preventDefault();
            });
        };

        var bind_search = function (searchdialogform, updatetablehtml, $tablesection, $searchdialog, $clearbutton) {
            var $search_dialog_form = $(searchdialogform);
            $search_dialog_form.submit(function (e) {
                $search_dialog_form.ajaxSubmit({
                    data: { p: 'json' },
                    success: function (data) {
                        updatetablehtml($tablesection, data);
                        $searchdialog.modal("hide");
                    },
                    error: function (xhr) {
                        bootbox.alert(global.AjaxEdit.internal_system_error_message || "An internal system error occurred.");
                    }
                });
                e.preventDefault();
            });

            $clearbutton.on('click', function (e) {
                $search_dialog_form.reset();
                e.preventDefault();
            });
        };

        var createbutton_editting = false;
        var bind_createbutton = function ($createbutton, showeditdialog) {
            $createbutton.on('click', function (e) {
                if (!createbutton_editting) {
                    createbutton_editting = true;
                    $(this).attr("disabled", true);

                    var href = $(this).attr('href');
                    var updatesection = $(this).data("updatesection");
                    showeditdialog(href, updatesection, $createbutton);
                }
                e.preventDefault();
            });
        };


        this.showeditdialog = show_editdialog;
        this.updatedatatable = update_datatable;

        this.binddeletebutton = bind_deletebutton;
        this.bindeditbutton = bind_editbutton;
        this.bindpagerbutton = bind_pagerbutton;
        this.bindsearch = bind_search;

        this.bindcreatebutton = bind_createbutton;

        var $this = this;
        var bind_ajaxedit = function () {
            var searchdialog = this.options.searchdialog;
            var searchdialogform = this.options.searchdialogform;
            var editdialogform = this.options.editdialogform;
            var tablesection = this.options.tablesection;
            var datatable = this.options.datatable;
            var createbutton = this.options.createbutton;
            var deletebutton = this.options.deletebutton;
            var editbutton = this.options.editbutton;
            var clearbutton = this.options.clearbutton;
            var on_editdialogshown = this.options.on_editdialogshown;
            var on_tablehtml_updated = this.options.on_tablehtmlupdated;

            var update_ajaxedittablehtml = function ($updatesection, html) {
                $updatesection.html(html);

                bind_ajaxeditdeletebutton();
                bind_ajaxediteditbutton();
                bind_ajaxeditpagerbutton();
                bind_ajaxeditsort();

                update_searchform_pager(html);

                if(on_tablehtml_updated)
                {
                    on_tablehtml_updated(html);
                }
            };

            var update_ajaxedittable = function (updatesection, updateurl) {
                $this.updatedatatable(updatesection, updateurl, searchdialogform, update_ajaxedittablehtml);
            };
            $this.updateajaxedittable = update_ajaxedittable;

            var show_ajaxediteditdialog = function (href, updatesection, triggerbutton) {
                $this.showeditdialog(href, updatesection, triggerbutton, editdialogform, update_ajaxedittable, on_editdialogshown);
            };


            var bind_ajaxeditdeletebutton = function () {
                var $deletebutton = $(deletebutton);
                $this.binddeletebutton($deletebutton, update_ajaxedittable);
            };
            bind_ajaxeditdeletebutton();

            var bind_ajaxediteditbutton = function () {
                var $editbutton = $(editbutton);
                $editbutton.each(function (index, elem) {
                    var $elem = $(elem);
                    if ($elem.data('ajax') && $elem.data('ajax') == true) {
                        $this.bindeditbutton($elem, show_ajaxediteditdialog);
                    }
                });
            };
            bind_ajaxediteditbutton();

            var bind_ajaxeditpagerbutton = function () {
                var $pagerbutton = $('.pager a');
                $this.bindpagerbutton($pagerbutton, update_ajaxedittable, tablesection);
            };
            bind_ajaxeditpagerbutton();

            var bind_ajaxeditsort = function () {
                $(datatable).sort({ form: searchdialogform });
            };
            bind_ajaxeditsort();

            var update_searchform_pager = function (html) {
                var $datatable = $(datatable);
                var page = $datatable.data('pager-page');
                var pagesize = $datatable.data('pager-pagesize');
                var $searchdialogform = $(searchdialogform);
                var $page = $searchdialogform.find('input[name=Page]');
                $page.val(page);
                var $pagesize = $searchdialogform.find('input[name=PageSize]');
                $pagesize.val(pagesize);
            }

            $this.bindsearch($(searchdialogform), update_ajaxedittablehtml, $(tablesection), $(searchdialog), $(clearbutton));

            var $createbutton = $(createbutton);
            if ($createbutton.data('ajax') && $createbutton.data('ajax') == true) {
                $this.bindcreatebutton($createbutton, show_ajaxediteditdialog);
            }
        };

        this.bind = bind_ajaxedit;
    }

    AjaxEdit.noConflict = function noConflict() {
        global.AjaxEdit = previousAjaxEdit;
        return AjaxEdit;
    };

    global.AjaxEdit = AjaxEdit;
}(this);