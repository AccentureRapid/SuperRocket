(function ($) {
	$.fn.extend({
		reset: function (options) {
		    $(this).find('input, textarea, select').each(function (index, elem) {
		        var $elem = $(elem);
		        type = $elem.attr('type');
		        switch (type) {
		            case 'text':
		            case 'textarea':
		            case 'password':
		                $elem.val('');
		                break;
		            case 'radio':
		            case 'checkbox':
		                $elem.prop('checked', false);
		                break;
		            case 'select-one':
		                $elem.val('');
		                break;
		            case 'select-multiple':
		                $elem.val('');
		                break;
		            default:
		                $elem.val('');
		        }
		    });

			return this;
		}
	});
})(jQuery);