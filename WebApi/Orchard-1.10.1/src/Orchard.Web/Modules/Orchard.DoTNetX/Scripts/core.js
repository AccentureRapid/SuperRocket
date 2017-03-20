$(function () {
    var REGULARRESPONSETIME = 1000;

    bootbox.setDefaults({
        locale: "en",
    });

    $.ajaxSetup({
        // Disable caching of AJAX responses
        cache: false
    });

    var loading = 0;

    $(document).ajaxSend(function (event, request, settings) {
        loading += 1;
        setTimeout(function () {
            var $loader = $('#ajax_loader_dialog');
            if (loading == 1) {
                $loader.modal("show");
            }
        }, REGULARRESPONSETIME);
    });

    $(document).ajaxComplete(function (event, request, settings) {
        loading -= 1;
        if (loading <= 0) {
            $('#ajax_loader_dialog').modal("hide");
            loading = 0;
        }
    });
});

var ajaxsubmit_datatype = "json";
var ajaxsubmit_accept = 'application/json, text/html';

$.ajaxSetup({
    error: function (xhr, status, error) {
        bootbox.alert(bootbox.default_internal_system_error_message || "An internal system error occurred.");
    }
});


function isIE() {
    var ua = navigator.userAgent;
    var re = new RegExp("MSIE [6-9]");
    result = re.exec(ua);
    return (result != null)
}

function getAjaxResponseText(xhr) {
    return isIE() ? (xhr.responseXML == undefined ? xhr.responseText : xhr.responseXML)
                  : xhr.responseText;
}