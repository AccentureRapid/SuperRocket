/* Datwendo pickerSelector.js */
jQuery(function ($) {
    $(".dropdown-culture-selector").change(function (e) {
        var href = $(".dropdown-culture-selector > option:selected")[0].value;
        window.location = href;
    });
});
