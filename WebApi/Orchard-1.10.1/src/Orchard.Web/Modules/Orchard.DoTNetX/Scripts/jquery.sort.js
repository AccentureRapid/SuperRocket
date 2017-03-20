(function ($) {
	$.fn.extend({
		sort: function (options) {
			var defaults = {
				form: '#search_form'
			};
			var options = $.extend(defaults, options);
			var $this = $(this);
			$this.find('th').on('click', function (e) {
				var sort_name = $(this).data('sort-name');
				if (sort_name) {
				    $sort_field = $(sort_name);
				    if ($sort_field) {
				        var ordering = $sort_field.val();
				        $this.clearSort();
				        switch (ordering) {
				            case 'Ascending': {
				                $sort_field.val('Descending');
				                break;
				            }
				            case 'Descending': {
				                $sort_field.val('');
				                break;
				            }
				            default: {
				                $sort_field.val('Ascending');
				                break;
				            }
				        }

				        var $form = $(options.form);
				        if ($form) {
				            $form.submit();
				        }
				    }
				}
			});

			return this;
		},
        
		clearSort: function () {
		    var $this = $(this);
		    $this.find('th').each(function (index, elem) {
		        var sort_name = $(elem).data('sort-name');
		        if (sort_name) {
		            var $sort_field = $(sort_name);
		            if ($sort_field) {
		                $sort_field.val('');
		            }
		        }
		    });
		    return this;
		}
	});
})(jQuery);