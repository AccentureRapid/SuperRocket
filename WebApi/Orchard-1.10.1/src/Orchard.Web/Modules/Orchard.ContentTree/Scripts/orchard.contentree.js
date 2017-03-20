//Core
(function ($, window, ui) {
	"use strict";
	
	$(function () {
		var ui = window.orchard.contenttree.ui;

		ui.initialize();
	});
})(jQuery, window);

//UI
(function ($, window, ui) {
	"use strict";

	var ui = {
		initialize: function () {
		    $('.blanket-actions a.Expand_All').on('click', { ui: this }, this.onExpandAll);
		    $('.blanket-actions a.Collapse_All').on('click', { ui: this }, this.onCollapseAll);
		    $('.blanket-actions a.Toggle_Editable').on('click', { ui: this }, this.onToggleEditable);
		    

		    $('.content-tree').on('click', '.tree-expando', { ui: this }, this.onExpandCollapse);
		    $('.content-tree').on('click', '.tree-item .related .manage-actions a', { ui: this }, this.onActionClick);
		    $('.content-tree').on('click', '.tree-item .related a.Undo_Actions', { ui: this }, this.onUndoActions);
		    $('.content-tree').on('click', '.tree-item .related a.Save_Actions', { ui: this }, this.onSaveActions);

		    //First time load of tree via AJAX to improve user feedback.
		    $.get('ContentTreeAsync').success(this.onInitialTreeLoad).error(this.onInitialTreeLoad);
		},
		onInitialTreeLoad: function (e) {
		    $('.content-tree').html(e);
		},
		onExpandAll: function (e) {
			$('.content-tree ul').each(function () {
				var list = $(this);
				var expando = list.siblings('a.tree-expando');
				list.show();
				expando.removeClass('closed');
			});

			return false;
		},
		onCollapseAll: function (e) {
			$('.content-tree ul').each(function () {
				var list = $(this);
				var expando = list.siblings('a.tree-expando');
				list.hide();
				expando.addClass('closed');
			});

			return false;
		},
		onToggleEditable: function (e) {
		    var state = $(this).text();
		    if (state == 'Show only items I can edit') {
		        $('.content-tree .tree-item.can-hide').hide();
		        $('.content-tree .tree-item.can-hide').siblings('.tree-expando').hide();
		        $(this).text('Show all items');
		    }
		    else {
		        $('.content-tree .tree-item.can-hide').show();
		        $('.content-tree .tree-item.can-hide').siblings('.tree-expando').show();
		        $(this).text('Show only items I can edit');
		    }

		    return false;
		},
		onExpandCollapse: function(e) {
			var target = $(this);

			var list = target.siblings('ul');
			if (list.is(':visible')) {
				list.slideUp('fast', function () {
					target.addClass('closed');
				});
			}
			else {
				list.slideDown('fast', function () {
					target.removeClass('closed');
				});
			}

			return false;
		},
		onActionClick: function(e) {
			var target = $(this);
			var action = target.text().replace(' ', '');

			if (action == 'Delete' || action == 'Unpublish' || action == 'Publish' || action == 'PublishDraft') {
				var item = target.parent().parent().parent();

				item.removeClass('Delete');
				item.removeClass('Unpublish');
				item.addClass(action);

				var hidden = target.parent().siblings('input.actions');
				hidden.val(action);

				e.data.ui.addSaveUndo(item);

				return false;
			}
			else if (action == 'Edit' || action == 'View' || action == 'Preview') {
			    window.open(target.attr('href'));

			    return false;
			}

			return true;
		},
		addSaveUndo: function (container) {
		    var div = $('<div class="commit-actions"></div>');
		    div.append('<a href="#" class="button primaryAction Undo_Actions">Undo</a>');
		    div.append('<a href="#" class="button primaryAction Save_Actions">Save</a>');

		    container.find('.related').append(div);
		},
		removeSaveUndo: function (container) {
		    container.find('.commit-actions').remove();
		},
		addSpinner: function (container) {
			container.find('.related').append('<span class="content-tree-spinner"></span>');
		},
		onSaveActions: function (e) {
			var target = $(this);
			var item = target.parent().parent().parent();

			var hidden = target.parent().siblings('input.actions');
			var actions = hidden.val();
			var id = hidden.attr('name').replace (/[^\d]/g, '');

			var url = 'ContentTree/SaveActions?id=' + id + '&actions=' + actions;

			e.data.ui.removeSaveUndo(item);
			e.data.ui.addSpinner(item);

			$.get(url).success(e.data.ui.onSaveActionsComplete).error(e.data.ui.onSaveActionsComplete);

			return false;
		},
		onSaveActionsComplete: function (data) {
			var id = parseInt(data.Id);

			if (id !== NaN) {
				var hidden = $('#Actions_' + id);
				var item = hidden.parents('.tree-item')
			    
				item.find('.related .content-tree-spinner').remove();

				if (data.Action == 'Delete') {
					item.parent().slideUp(400, function () {
						item.parent().remove();
					});
				}
				else if (data.Action == 'Unpublish') {
					item.removeClass('Unpublish');
					item.addClass('Unpublished');

					var updatedItem = $('<div></div>');
					updatedItem.html(data.ItemHtml);

					item.html(updatedItem.find('.tree-item').html());
				}
				else if (data.Action == 'Publish' || data.Action == 'PublishDraft') {
				    item.removeClass('Unpublish');
				    item.removeClass('Unpublished');

				    var updatedItem = $('<div></div>');
				    updatedItem.html(data.ItemHtml);

				    item.html(updatedItem.find('.tree-item').html());
				}
			} else {
				alert('There was a problem commiting this action. This may be because the tree is out-of-sync. Please refresh the page and try again.');
			}

			return false;
		},
		onUndoActions: function (e) {
			var target = $(this);
			var item = target.parent().parent().parent();

			item.removeClass('Delete');
			item.removeClass('Unpublish');

			var hidden = target.parent().siblings('input.actions');
			hidden.val('');

			e.data.ui.removeSaveUndo(item);

			return false;
		}
	};

	if (!window.orchard) {
		window.orchard = {};
	}

	if (!window.orchard.contenttree) {
		window.orchard.contenttree = {};
	}

	window.orchard.contenttree.ui = ui;
})(jQuery, window);