/* Datwendo AutorouteComplete.js */
jQuery(function ($) {
        // grab the slug input
        var slug = $("#Autoroute_Localized_CurrentUrl");
        var setCultureInSlug = function (newCulture, defaultCult) {
            var slugValue = slug.val();
            newCulture = newCulture ? newCulture : defaultCult;
            var newSlugVal;
            if (slugValue && slugValue.match("^" + currentCulture)) {
                newSlugVal=slugValue.replace(new RegExp("^" + currentCulture, "i"), newCulture+"/");
            }
            else {
                if (slugValue.match(/^\//)) {
                    newSlugVal=newCulture + slugValue;
                } else
                {
                    var oldCulture          = slug.val().match(/^([a-z]{2}-[a-z]{2}|[a-z]{2}){1}/i);
                    if (oldCulture == null) oldCulture = "";
                    if (oldCulture.length > 0) {
                        oldCulture = oldCulture[0];
                        slugValue = slugValue.replace(oldCulture + "/", "");
                    }
                    newSlugVal=slugValue ? newCulture + "/" + slugValue : newCulture+"/";
                }
            };
            newSlugVal = newSlugVal.replace('//', '/');
            slug.val(newSlugVal);
        };
        if (slug) {
            // grab the current culture
            var culture = $("#SelectedCulture");
            if (culture.length > 0) {
                var defaultCulture  = slug.val().match(/^([a-z]{2}-[a-z]{2}|[a-z]{2}){1}/i);
                if (defaultCulture == null) defaultCulture = "";
                if (defaultCulture.length > 0) defaultCulture = defaultCulture[0];
                var currentCulture  = defaultCulture;
                var nCulture = culture.val();
                if (nCulture != defaultCulture)
                {
                    setCultureInSlug(nCulture, defaultCulture);
                    var viewLnk = $("#viewLink");
                    if ( viewLnk != null )
                        viewLnk.hide();
                }
                // when the culture is changed update the slug
                culture.change(function () {
                    var newCulture = $(this).val().toLowerCase();
                    setCultureInSlug(newCulture,defaultCulture);                   
                    currentCulture = newCulture;
                });
            }
        }
    });
