!function (global) {
    'use strict';

    var previousMediaUpload = global.MediaUpload;

    function MediaUpload(options) {
        this.options = options || {};

        var $this = this;
        var bind_MediaUpload = function () {
            var $upload_form = $("#upload_form");
            var upload_form_submitting = false;
            $upload_form.submit(function (e) {
                if (!upload_form_submitting) {
                    upload_form_submitting = true;
                    $upload_form.find("button[type=submit]").prop('disabled', true);
                    $upload_form.ajaxSubmit({
                        dataType: ajaxsubmit_datatype,
                        accept: ajaxsubmit_accept,
                        headers: { Accept: ajaxsubmit_accept },
                        // 1h timeout
                        timeout: 3600000,

                        data: { p: 'json' },
                        success: function (data) {
                            if (data == "OK") {
                                var message = "Template has been generated successfully.";
                                bootbox.alert(message, function () {
                                    window.location.reload();
                                });
                            }
                            else {
                                var message = "Error occured, please try again.";
                                bootbox.alert(message, function () {
                                    window.location.reload();
                                });
                                $upload_form.reset();
                            }
                            $upload_form.find("button[type=submit]").prop('disabled', false);
                            upload_form_submitting = false;
                        },
                        error: function (xhr) {
                            var data = getAjaxResponseText(xhr);
                            if (data) {
                                bootbox.alert(bootbox.default_internal_system_error_message || "An internal system error occurred.");
                            }
                            else {
                                bootbox.alert(bootbox.default_internal_system_error_message || "An internal system error occurred.");
                            }
                            $upload_form.find("button[type=submit]").prop('disabled', false);
                            upload_form_submitting = false;
                        }
                    });
                }
                e.preventDefault();
            });
        };
        this.bind = bind_MediaUpload;
    }

    MediaUpload.noConflict = function noConflict() {
        global.MediaUpload = previousMediaUpload;
        return MediaUpload;
    };

    global.MediaUpload = MediaUpload;
}(this);

$(function () {
    var mediaupload = new MediaUpload();
    mediaupload.bind();
});