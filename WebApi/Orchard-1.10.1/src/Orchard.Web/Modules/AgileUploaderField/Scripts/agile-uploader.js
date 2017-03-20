/*
* Adapted from the jQuery plugin written by Shift8 Creative
* http://www.shift8creative.com/pages/projects/agile-uploader/index.html
* by Claire Botman
* refactored jQuery namespace & customised for Orchard
*/

(function ($) {
    var noUri;
    var opts;

    var methods = {
        init: function (options) {
            var defaults = {
                flashSrc: 'agile-uploader.swf',
                expressInstallSrc: 'expressInstall.swf',
                flashWidth: 25,
                flashHeight: 22,
                flashParams: { allowscriptaccess: 'always' },
                flashAttributes: { id: "agileUploaderSWF" },
                flashVars: {
                    max_height: 500,
                    max_width: 500,
                    jpg_quality: 85,
                    preview_max_height: 50,
                    preview_max_width: 50,
                    show_encode_progress: true,
                    js_get_form_data: 'SerializeFormData',
                    js_event_handler: 'AgileUploaderEventHandler',
                    return_submit_response: true,
                    file_filter: '*.jpg;*.jpeg;*.gif;*.png;*.JPG;*.JPEG;*.GIF;*.PNG;*.zip',
                    file_filter_description: 'Files',
                    // max post size is in bytes (note: all file size values are in bytes)
                    max_post_size: (1536 * 1024),
                    file_limit: -1,
                    button_up: 'add-file.png',
                    button_over: 'add-file.png',
                    button_down: 'add-file.png'
                },
                progressBarColor: '#000000',
                attachScrollSpeed: 1000,
                removeIcon: 'trash-icon.png',
                genericFileIcon: 'file-icon.png',
                maxPostSizeMessage: 'Attachments exceed maximum size limit.',
                maxFileMessage: 'File limit hit, try removing a file first.',
                duplicateFileMessage: 'This file has already been attached.',
                notReadyMessage: 'The form can not be submitted yet because there are still files being resized.',
                removeAllText: 'remove all',
                updateDiv: ''
            };

            opts = $.extend({}, defaults, options);
            opts.flashVars = $.extend({}, defaults.flashVars, options.flashVars);
            opts.flashParams = $.extend({}, defaults.flashParams, options.flashParams);
            opts.flashAttributes = $.extend({}, defaults.flashAttributes, options.flashAttributes);

            return this.each(function () {
                // We know IE6 & IE7 don't have data URI support
                if ($.browser.msie && (parseInt($.browser.version) < 8)) {
                    noUri = true;
                }
                else {
                    // If it's another browser, test data URI support
                    var data = new Image();
                    data.onload = data.onerror = function () {
                        if (this.width != 1 || this.height != 1) {
                            noUri = true;
                        }
                    };
                    data.src = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
                }
                // end data uri check
                $('#' + this.id).append('<div id="agileUploaderAttachArea"><div id="agileUploaderEMBED">This function requires Flash 10+ - <a href="http://get.adobe.com/flashplayer/" target="_blank">Get Flash</a></div><div id="agileUploaderMessages"></div></div>');

                methods.embed(); // embed

                // Add the file queue list
                $('#' + this.id).prepend('<div id="agileUploaderRemoveAll"></div><div id="agileUploaderInfo"><ul id="agileUploaderFileList"></ul></div>');
            });
        },
        embed: function () {
            // Breaks up cache. When redirecting back to the page that embeds the swf, some browsers will have a problem. Randomizing the name seems to help.
            var flashSrcCacheBust = opts.flashSrc + '?' + Math.floor(Math.random() * 9999 + 1);
            window.swfobject.embedSWF(flashSrcCacheBust, 'agileUploaderEMBED', opts.flashWidth, opts.flashHeight, "10.0.0", opts.expressInstallSrc, opts.flashVars, opts.flashParams, opts.flashAttributes);
        },
        swfEvent: function (event) {
            switch (event.type) {
                case 'attach':
                    methods.attachFile(event.file);
                    break;
                case 'progress':
                    methods.currentEncodeProgress(event.file);
                    break;
                case 'preview':
                    methods.preview(event.file);
                    break;
                case 'file_removed':
                    methods.detachFile(event.file);
                    break;
                case 'server_response':
                    methods.serverResponse(event.response);
                    break;
                case 'http_status':
                    methods.httpResponse(event.response);
                    break;
                case 'max_post_size_reached':
                    methods.maxPostSize(event.file);
                    break;
                case 'file_limit_reached':
                    methods.fileLimit(event.file);
                    break;
                case 'file_already_attached':
                    methods.fileAlreadyAttached(event.file);
                    break;
                case 'encoding_still_in_progress':
                    methods.uploaderNotReady(event.file);
                    break;
            }
        },
        attachFile: function (file) {
            // if in single file replace mode just empty the list visually, only the last attached file will be submitted by flash (rare, this shouldn't be w/ multiple uploads)
            if (opts.flashVars.file_limit == -1) {
                $('#agileUploaderFileList').empty();
            }
            $("#agileUploaderInfo").animate({ scrollTop: $("#agileUploaderInfo").attr("scrollHeight") }, opts.attachScrollSpeed);
            var alt = '';
            if ($('#agileUploaderFileList li').size() % 2 == 0) { alt = 'alt'; }
            $('#agileUploaderFileList').append('<li id="id-' + file.uid + '" class="' + alt + '"><div class="agileUploaderFilePreview" style="display: none;"></div><div class="agileUploaderFileName" style="display: none;">' + file.fileName + '</div><div id="' + file.uid + 'CurrentProgress" class="agileUploaderCurrentProgress"></div><div class="agileUploaderFileSize" style="display: none;"></div><div class="agileUploaderRemoveFile" style="display:none;"><a href="#" id="remove-' + file.uid + '" onClick="document.getElementById(\'agileUploaderSWF\').removeFile(\'' + file.uid + '\'); return false;"><img class="agileUploaderRemoveIcon" src="' + opts.removeIcon + '" alt="remove" /></a></div></li>');
            // Check for IE, change css special for IE.
            if (/msie/i.test(navigator.userAgent) && !/opera/i.test(navigator.userAgent) === true) {
                $('#id-' + file.uid).css('height', opts.flashVars.preview_max_height + 5);
            } else {
                $('#id-' + file.uid).css('height', opts.flashVars.preview_max_height);
            }

            // If using a bar, the background gets the value of opts.progressBar, it can be '#123456' or 'url:("image.jpg")'  ... NOTE: no ending ;
            if ((typeof (opts.progressBar) == 'string') && (opts.progressBar != 'percent')) {
                $('#' + file.uid + 'CurrentProgress').css('background', opts.progressBarColor);
            }

            $('#agileUploaderFileInputText').val(file.fileName);
        },
        currentEncodeProgress: function (file) {
            if (opts.progressBar == 'percent') {
                $('#' + file.uid + 'CurrentProgress').text(parseInt(file.percentEncoded) + '%');
            } else {
                $('#' + file.uid + 'CurrentProgress').css('width', parseInt(file.percentEncoded) + '%');
                $('#agileUploaderProgressBar').css('width', parseInt(file.percentEncoded) + '%');
            }

            if (file.percentEncoded >= 100) {
                $('#' + file.uid + 'CurrentProgress').remove();
                // add the file size
                var sizeKb = ((file.finalSize / 1024) * 100) / 100;
                $('#id-' + file.uid + ' .agileUploaderFileSize').text('(' + sizeKb.toFixed(2) + 'Kb)');

                $('.agileUploaderFileName, .agileUploaderRemoveFile, .agileUploaderFileSize, .agileUploaderFilePreview').show();
                // add remove all
                $('#agileUploaderRemoveAll').html('<a href="#" onClick="document.getElementById(\'agileUploaderSWF\').removeAllFiles(); $(\'#agileUploaderFileList\').empty(); $(\'#agileUploaderRemoveAll\').empty(); return false;">' + opts.removeAllText + '</a>');
            }
        },
        preview: function (file) {
            if ((typeof (file.base64Thumbnail) != 'undefined') && (noUri !== true) && (file.base64Thumbnail != null)) {
                $('#id-' + file.uid + ' .agileUploaderFilePreview').html('<img src="' + file.base64Thumbnail + '" />');
            } else {
                $('#id-' + file.uid + ' .agileUploaderFilePreview').html('<img src="' + opts.genericFileIcon + '" />');
            }
            // adjust the file size
            var sizeKb = ((file.finalSize / 1024) * 100) / 100;
            $('#id-' + file.uid + ' .agileUploaderFileSize').text('(' + sizeKb.toFixed(2) + 'Kb)');
        },
        detachFile: function (file) {
            $('#id-' + file.uid).remove();
            if ($('#agileUploaderFileList li').length < 1) {
                $('#agileUploaderRemoveAll').empty();
            }
        },
        serverResponse: function (response) {
            var updateDiv = $("#" + opts.updateDiv);
            if (updateDiv.val() != '') {
                updateDiv.val(updateDiv.val().split(';').concat(response.split(';')).join(';'));
            }
            else {
                updateDiv.val(response);
            }
            $('#agileUploaderFileList').empty();
            $("form").trigger("auUploaded");
        },
        httpResponse: function (response) {
            if (response == "200") {
                $('#agileUploaderRemoveAll').empty();
            }
        },
        maxPostSize: function (file) {
            $('#id-' + file.uid).remove(); // in case the row was visually added because it had a progress bar (it's already removed in Flash, well, it was never added actually)
            $("#agileUploaderMessages").show();
            $('#agileUploaderMessages').text(opts.maxPostSizeMessage);
            clearTimeout();
            setTimeout('$("#agileUploaderMessages").fadeOut()', 3000);
        },
        fileLimit: function (file) {
            $('#id-' + file.uid).remove(); // in case the row was visually added because it had a progress bar		
            $("#agileUploaderMessages").show();
            $('#agileUploaderMessages').text(opts.maxFileMessage);
        },
        fileAlreadyAttached: function () {
            $("#agileUploaderMessages").show();
            $('#agileUploaderMessages').text(opts.duplicateFileMessage);
            clearTimeout();
            setTimeout('$("#agileUploaderMessages").fadeOut()', 3000);
        },
        uploaderNotReady: function () {
            $("#agileUploaderMessages").show();
            $('#agileUploaderMessages').text(opts.notReadyMessage);
            clearTimeout();
            setTimeout('$("#agileUploaderMessages").fadeOut()', 3000);
        },
        submitAgileUploader: function () {
            if ($.browser.msie && $.browser.version == '6.0') {
                window.document.agileUploaderSWF.submit();
            } else {
                document.getElementById('agileUploaderSWF').submit();
                return false;
            }
            return true;
        }
    };

    $.fn.agileUploader = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        }
        else {
            $.error('Method ' + method + ' does not exist on jQuery.agileUploader');
            return false;
        }
    };

})(jQuery);