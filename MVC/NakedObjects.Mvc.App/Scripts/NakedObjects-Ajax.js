var nakedObjects = (function () {

    var api = {};
    api.ajaxCount = 0;
    api.usePopupDialogs = true; 
    var disabledSubmits;
    var inAjaxLink;
    var updateLocationFlag = false;

    var curTransientKey = "currentTransient:";

    var fileUploadTimeOut = 60 * 1000; // one minute 
    var tempStoreQuota = 1024 * 1024 * 1024; // One Gigabyte

    function safeDecrementAjaxCount() {
        if (api.ajaxCount > 0) {
            api.ajaxCount--;
        } else {
            api.ajaxCount = 0; 
        }
    }

    api.executeAndCloseButtonQueryOnlyText = "OK"; //Recommended alternative label:  "Go"
    api.executeWithoutClosingButtonText = "Apply";    //Recommended alternative label:  "Show"
    api.executeAndCloseButtonDefaultText = "OK";   //Recommended alternative label:  "Do"

    api.getDisabledSubmits = function () {
        return disabledSubmits;
    };

    // see select handler for autocomplete
    var ignoreNextEnter = false;

    api.checkForEnter = function (event) {

        if (event.keyCode === 13) { // enter key 

            if (ignoreNextEnter) {
                ignoreNextEnter = false; 
                return false;
            }

            if (this.nodeName.toLowerCase() === "button") {
                this.click();
                return false;
            }

            var form = $(this).closest("form");

            var okButton = form.find("button.nof-ok:first");
            if (okButton.length > 0) {
                okButton.get(0).focus();
                okButton.get(0).click();
                return false;
            }

            var submitButton = form.find("button:submit:not([name]):first");
            if (submitButton.length > 0) {
                submitButton.get(0).focus();
                submitButton.get(0).click();
                return false;
            }
            return true;
        } else {
            // for ie8 and earlier the enter button event is not triggering here on an input field. 
            // This means when form is submitted wrong button is selected. So clear previous button here 
            // and pick up in 'getButton'. 
            $("form button.nof-lastClicked").removeClass("nof-lastClicked");
            return true;
        }
    };

    function focusOnFirstText(parent) {

        var firstText;
        var validationError;
        var textSelector = "input[type='text'], input[type='checkbox'], select, textarea";
        var validationSelector = ".input-validation-error";

        if (parent) {
            firstText = parent.find(textSelector).filter(":enabled").first();
            validationError = parent.find(validationSelector).filter(":enabled").first();
        } else {
            firstText = $(textSelector).filter(":enabled").first();
            validationError = $(validationSelector).filter(":enabled").first();
        }

        firstText.focus();
        validationError.focus();
    }

    api.focusOnFirst = function () {

        var okButton = $("button.nof-ok:first");
        var saveButton = $("button.nof-save:first");

        if (okButton.length > 0) {
            var dialog = okButton.closest(".nof-actiondialog, .nof-dialog, .nof-dialog-file");
            focusOnFirstText(dialog);
        } else if (saveButton.length > 0) {
            var object = saveButton.closest(".nof-objectedit");
            focusOnFirstText(object);
        } else {
            focusOnFirstText();
        }
    };

    api.clearHistory = function () {

        api.ajaxCount++;
        $.post($(this).attr("action"), $(this).serialize(), function (response) {

            // check attribute value exists for ie8 support no need ie9+ and ff/chrome
            if ($(".nof-history button").attr("value") && $(".nof-history button").attr("value").split("=")[1].toLowerCase() === 'true') {
                $(".nof-history div.nof-object").replaceWith("");
                $(".nof-history > form").replaceWith($(""));
            } else {
                $(".nof-history div.nof-object:not(:last)").replaceWith("");
                $(".nof-history div.nof-object:first").replaceWith($(".nof-history div.nof-object:first", response));
                $(".nof-history button").attr("value", "clearAll=False"); // keep last value of clearAll flag (which must be false or no button)
            }

            bindToNewHtml(false);
        }).always(function () {
            safeDecrementAjaxCount();
        });
        
        return false;
    };
    
    api.clearTabbedHistory = function () {

        api.ajaxCount++;
        $.post($(this).attr("action"), $(this).serialize(), function (response) {

            handleLoginForm(response);
            replacePageBody(response);
            replaceFormValues();
            bindToNewHtml(true);
        }).always(function () {
            safeDecrementAjaxCount();
        });
        
        return false;
    };


    api.clearHistoryItem = function () {

        api.ajaxCount++;
        
        // if clearing active tab redraw otherwise just remove tab from history 

        var clearingActive = false;
        var tab = $(this).closest(".nof-tab");
        if (tab.hasClass("active")) {
            clearingActive = true;
            $.ajaxSetup({ cache: false });
            api.cacheAllFormValues();
        }

        $.post($(this).attr("action"), $(this).serialize(), function (response) {

            if (clearingActive) {
                handleLoginForm(response);
                replacePageBody(response);
                replaceFormValues();
                bindToNewHtml(true);
            } else {
                replace(".nof-tabbed-history", response);
                drawHistoryMenus();
            }    
        }).fail(function (jqXHR) {
            var response = jqXHR.responseText;
            if (response) {
                replacePageBody(response);
                
            }
        }).always(function () {
            safeDecrementAjaxCount();
        });
        
        return false;
    };

    api.clearHistoryOthers = function () {

        api.ajaxCount++;

        var alreadyActive = false;
        var tab = $(this).closest(".nof-tab");
        if (tab.hasClass("active")) {
            alreadyActive = true;    
        } else {
            $.ajaxSetup({ cache: false });
            api.cacheAllFormValues();
        }

        $.post($(this).attr("action"), $(this).serialize(), function (response) {

            if (alreadyActive) {
                // just redraw history
                replace(".nof-tabbed-history", response);
                drawHistoryMenus();
            } else {          
                handleLoginForm(response);
                replacePageBody(response);
                replaceFormValues();
                bindToNewHtml(true);
            }
        }).always(function () {
            safeDecrementAjaxCount();
        });
        
        return false;
    };

    api.closeHistoryMenus = function (event, currentTab) {
        if (event) {
            var links = currentTab.find("li a");
            if (event.target == links.get(0) || event.target == links.get(1) || event.target == links.get(2)) {
                return true; 
            }
        }

        $(".nof-tab").find("ul").remove();
        $(document).off("mousedown");
        return true;
    };

    function drawHistoryMenus() {
     
        
        $(".nof-tab").mousedown(function (event) {
           
            switch (event.which) {
                case 3:
                    api.closeHistoryMenus();
                    var currentTab = $(this);

                    currentTab.append("<ul><li><a href='#'>Close This</a></li><li><a href='#'>Close Others</a></li><li><a href='#'>Close All</a></li></ul>");
                    currentTab.find("ul").menu();
                    // wire up 

                    currentTab.find("li:eq(0) a").click(function () {
                        currentTab.find("button.nof-clear-item").click();
                    });
                    currentTab.find("li:eq(1) a").click(function () {
                        currentTab.find("button.nof-clear-others").click();
                    });
                    currentTab.find("li:eq(2) a").click(function () {
                        currentTab.find("button.nof-clear").click();
                    });

                    $(document).on("mousedown", function(evt) { return api.closeHistoryMenus(evt, currentTab); });
                    return false;
                default:
                    return true;
            }
        });

    }


    function getButton(event) {

        var button = $("form button.nof-lastClicked").get(0);

        if (!button) {
            // for ie8 and earlier the enter button event is not triggering on an input field. 
            // This means when form is submitted wrong button is selected. Previous button was cleared in 
            // 'checkForEnter' so look for OK or submit button here. 

            var form1 = $(event.target).closest("form");
            var okButton1 = form1.find("button.nof-ok:first");
            if (okButton1.length > 0) {
                button = okButton1.get(0);
            } else {
                var submitButton1 = form1.find("button:submit[name='']:first");
                if (submitButton1.length > 0) {
                    button = submitButton1.get(0);
                }
            }
        }
        return button;
    }

    function errorDialog(title, msg) {

        $(".main-content").append("<div id='_errorMessage' title='" + title + "'>" + msg + "</div>");
        $("#_errorMessage").dialog({ draggable: true, height: '500', width: '1000', close: function () { $("#_errorMessage").remove(); } });
    }

    function startSubmitFeedBack(button) {

        $(button).effect("highlight", {}, 500);
        disabledSubmits = $(":submit, this");
        disabledSubmits.attr("disabled", "disabled");
        $("body").css("cursor", "progress");
    }

    function endSubmitFeedBack() {
        if (disabledSubmits) {
            disabledSubmits.removeAttr("disabled");
            $("body").css("cursor", "auto");
            disabledSubmits = null;
            return true;
        }
        return false;
    }

    function startLinkFeedBack() {
        inAjaxLink = true;
        $("a").css("cursor", "progress");
    }

    function endLinkFeedBack() {
        if (inAjaxLink) {
            inAjaxLink = null;
            $("a").css("cursor", "pointer");
            return true;
        }
        return false;
    }

    api.bindAjaxError = function () {
        $(".main-content").ajaxError(function (e, xhr, settings) {
            // check if we were doing a ajax call - if not ignore - must have been a validate 
            if (endSubmitFeedBack() || endLinkFeedBack()) {
                safeDecrementAjaxCount();
            }
            errorDialog('Ajax Error', "Error in: " + settings.url + " \n" + "error:\n" + xhr.responseText);
        });
    };

    function setCollectionButtonStates() {
        $("div.nof-collection-table button.nof-summary").show();
        $("div.nof-collection-table button.nof-list").show();
        $("div.nof-collection-table button.nof-table").hide();
        $("div.nof-collection-list button.nof-summary").show();
        $("div.nof-collection-list button.nof-list").hide();
        $("div.nof-collection-list button.nof-table").show();
        $("div.nof-collection-summary button.nof-summary").hide();
        $("div.nof-collection-summary button.nof-list").show();
        $("div.nof-collection-summary button.nof-table").show();
    }

    function setMinMaxButtonStates() {
        $("div.nof-property > div.nof-object > a").closest("div.nof-property").find("> div.nof-object > form button.nof-minimize").hide();
        $("div.nof-property > div.nof-object > a").closest("div.nof-property").find("> div.nof-object > form button.nof-maximize").show();
        $("div.nof-property > div.nof-object > div.nof-propertylist").closest("div.nof-property").find("> div.nof-object > form button.nof-maximize").hide();
        $("div.nof-property > div.nof-object > div.nof-propertylist").closest("div.nof-property").find("> div.nof-object > form button.nof-minimize").show();
    }

    function prefixAttribute(elements, attributeName, prefix) {
        elements.each(function (index, elem) {
            var existingId = $(elem).attr(attributeName);
            if (existingId) {
                $(elem).attr(attributeName, prefix + existingId);
            }
        });
    }


    api.redisplayInlineProperty = function (event) {
        api.ajaxCount++;

        var button = getButton(event);

        if (!button || button.name !== "Redisplay") {
            safeDecrementAjaxCount();
            return true;
        }

        startSubmitFeedBack(button);

        var formSerialized = $(this).serializeArray();
        formSerialized[formSerialized.length] = { name: button.name, value: button.value };

        $.post($(this).attr("action"), formSerialized, function(response) {

            handleLoginForm(response);
            var property = $(button).closest("div.nof-property");
            var obj = property.find("> div.nof-object");

            var propertyList = $("div.nof-propertylist", response).filter(":first");
            var editButton = propertyList.find("> form.nof-action").filter(":first");

            var prefix = property.attr("id") + "-";
            prefixAttribute(propertyList.find("*"), "id", prefix);

            if ($(button).hasClass("nof-maximize")) {
                obj.append(editButton);
                obj.append(propertyList);
            } else {
                obj.find("> form.nof-action").remove();
                obj.find("> div.nof-propertylist").remove();
            }

            setMinMaxButtonStates();
            setCollectionButtonStates();

        }).fail(function (jqXHR) {
            var response = jqXHR.responseText;
            if (response) {
                replacePageBody(response);
                
            }
        }).always(function () {
            endSubmitFeedBack();
            safeDecrementAjaxCount();
        });

        return false;
    };

    function truncateId(idToTruncate) {
        var subIds = idToTruncate.split("-");
        var count = subIds.length;
        return subIds[count - 2] + "-" + subIds[count - 1];
    }


    api.redisplayProperty = function (event) {
        api.ajaxCount++;

        var button = getButton(event);

        if (!button || button.name !== "Redisplay") {
            safeDecrementAjaxCount();
            return true;
        }

        startSubmitFeedBack(button);

        var formSerialized = $(this).serializeArray();
        formSerialized[formSerialized.length] = { name: button.name, value: button.value };

        $.post($(this).attr("action"), formSerialized, function (response) {
            handleLoginForm(response);
            var property = $(button).closest("div.nof-property");
            var idToMatch = truncateId(property.attr("id"));
            var replaceWith = $(response).find("div#" + idToMatch);

            if (replaceWith.length > 0) {
                // replace the actual property
                property.replaceWith(replaceWith);
                // replace the hidden input fields to persist the setting 
                var value = $("input[id$='-displayFormats']:first", response).attr("value");
                $("form  input[id$='-displayFormats']").attr("value", value);
            } else {
                // replace the main body of the page
                $(".main-content").replaceWith($(".main-content", response));
            }

            setMinMaxButtonStates();
            setCollectionButtonStates();
        }).fail(function (jqXHR) {
            var response = jqXHR.responseText;
            if (response) {
                replacePageBody(response);
               
            }
        }).always(function () {
            endSubmitFeedBack();
            safeDecrementAjaxCount();
        });

        return false;
    };

    api.updateTitle = function (response) {
        var pattern = /<title>\s*?(.*?)\s*?<\/title>/;
        var matches = response.match(pattern);
        var title = matches ? matches[1] : "";
        document.title = $.trim(title);
    };

    function replace(tag, response) {

        var newvalue = $(tag, response);

        if (newvalue.length > 0) {
            $(tag).replaceWith(newvalue);
            return true;
        }
        return false;
    }

    // close popup if exists and is marked for closure 
    function closePopupDialog() {
        var popupElement = $("div.popup-dialog");

        if (popupElement.length > 0) {
            var closing = popupElement.attr("data-closing");
            if (closing) {
                popupElement.dialog("close");
            }
        }
    }

    function addApplyButton(okButton) {
        if (okButton.closest(".nof-parameterlist").hasClass("nof-queryonly")) {
            okButton.text(api.executeAndCloseButtonQueryOnlyText);
            okButton.attr("title", api.executeAndCloseButtonQueryOnlyText);

            var applyButton = okButton.clone();
            applyButton.removeClass("nof-ok");
            applyButton.addClass("nof-apply");
            applyButton.text(api.executeWithoutClosingButtonText);
            applyButton.attr("title", api.executeWithoutClosingButtonText);
            okButton.after(applyButton);

            applyButton.click(function() {
                applyButton.closest("form").siblings("div.validation-summary-errors").remove();
                applyButton.closest("form").find("span.field-validation-error").remove();
                return true;
            });
        } else {
            okButton.text(api.executeAndCloseButtonDefaultText);
            okButton.attr("title", api.executeAndCloseButtonDefaultText);
        }
    }

    // popup is already open so just update with new content 
    function redrawPopupDialog(popupElement, response) {
        
        var errors = $("div.validation-summary-errors", response);
        var content = $("form.nof-dialog, form.nof-dialog-file", response);
        var title = $("div.nof-actionname", response);

        popupElement.attr("title", title.text());
        popupElement.removeAttr("data-closing");
        popupElement.find("div.validation-summary-errors").remove();
        popupElement.find("form.nof-dialog, form.nof-dialog-file").remove();
        
        popupElement.append(errors);
        popupElement.append(content);

        // update initial choices
        $(popupElement).find(":input").each(api.updateChoices);

        $(".ui-dialog-titlebar span").text(title.text());

        var okButton = $("div.popup-dialog button.nof-ok");
        addApplyButton(okButton);
        okButton.on("click", function () { popupElement.attr("data-closing", true); });
    }

    function popupDialog(response) {

        if (api.usePopupDialogs) {

            var popupElement = $("div.popup-dialog");

            if (popupElement.length === 0) {
                $("section.main-content").append("<div class='popup-dialog'></div>");
                popupElement = $("div.popup-dialog");

                var content = $("form.nof-dialog, form.nof-dialog-file", response);
                var title = $("div.nof-actionname", response);
                var errors = $("div.validation-summary-errors", response);

                popupElement.attr("title", title.text());

                // create new popup with contents from response 
                popupElement.dialog({
                    open: function () {                       
                        
                        popupElement.append(errors);
                        popupElement.append(content);
                     
                        var okButton = $("div.popup-dialog button.nof-ok");       
                        
                        // ok button will close dialog, show will not 
                        okButton.on("click", function() {
                            popupElement.attr("data-closing", true);
                        });
                        addApplyButton(okButton);
                    },
                    close: function() { popupElement.remove(); },
                    modal: true,
                    width: 'auto',
                    height: 'auto'
                });

                // update initial choices
                $(popupElement).find(":input").each(api.updateChoices);

            } else {
                redrawPopupDialog(popupElement, response);
            }
          
            $.validator.unobtrusive.parse(popupElement);
           

            return true;
        }
        return false;
    }

    function replacePageBody(response) {
        
        if ($("form.nof-dialog, form.nof-dialog-file", response).length > 0) {
            if (popupDialog(response)) {
                // handled by popup code 
                return;
            }
        }

        api.updateTitle(response);
     
        // close existing popup if marked for closure 
        closePopupDialog();   
        
        if (replace(".main-content", response)) {
            // update initial choices
            $(".main-content").find(":input").each(api.updateChoices);
            return;
        }
        if (replace("body", response)) {
            // update initial choices
            $("body").find(":input").each(api.updateChoices);
            return;
        }
        
        errorDialog("Unhandled response", response);
    }

    function isTransientId() {
        if ($("div.nof-objectedit").hasClass("nof-transient")) {
            var action = $("div.nof-objectedit form.nof-edit").attr("action");
            if (action) {
                return getIdFromUrl(action);
            }
        }
        return false;
    }

    api.getLinkFromHistory = function (context) {
        var isDialog = $(".main-content > div.nof-actiondialog", context).length > 0;
        var transientId = isTransientId();

        var link = null;
        if (isDialog) {
            // for dialogs with a FileAttachment or Image we drop out of ajax. 
            // when finished we want to restore the ajax style url - that's what the updateLocationFlag is. 
            if ($(":input[type=file]", context).length === 0) {
                link = $("div.nof-actiondialog > form", context).attr("action");
            } else {
                updateLocationFlag = true; 
            }
        } else if (transientId) {
            link = "/Transient?id=" + escape(transientId);
        } else {
            link = $(".nof-history div.nof-object:last a", context).attr("href") || $(".nof-tabbed-history div.nof-tab.active a", context).attr("href") || "";
            var isEdit = $(".main-content > div.nof-objectedit", context).length > 0;

            if (isEdit) {
                link = link.replace("/Details?", "/EditObject?");
            }
        }

        return link;
    };

    function updateLinkFromHistory() {
        var link = api.getLinkFromHistory();
        $.address.value(link);
    }

    function clearTransientsFromCache() {
        var tid = $.jStorage.get(curTransientKey);
        if (tid) {
            $.jStorage.deleteKey(tid);
            $.jStorage.deleteKey(curTransientKey);
        }
    }

    function clearAllButTransientsFromCache() {
        var tid = $.jStorage.get(curTransientKey);
        var content;
        if (tid) {
            content = $.jStorage.get(tid);
        }

        $.jStorage.flush();

        if (content) {
            setTransientInCache(tid, content);
        }
    }

    function setTransientInCache(tid, content) {
        $.jStorage.set(curTransientKey, tid);
        $.jStorage.set(tid, content);
    }

    function cacheIfTransient() {
        var transientId = isTransientId();
        if (transientId) {
            clearTransientsFromCache();
            var tid = "transient:" + transientId;
            var content = $(".main-content").html();

            setTransientInCache(tid, content);
            return true;
        }
        return false;
    }

    function cacheFormValues(formSerialized) {
        $("form div.nof-parameter:has(div.nof-object), form div.nof-property:has(div.nof-object)").each(function (index, element) {
            $.jStorage.set(element.id, $(element).html());
        });

        var nameValues = {};

        for (var i = 0; i < formSerialized.length; i++) {

            // only write value if it evaluates to true or no previous value has been written (ie true or any value takes priority
            // over false or null/undefined)  
            if ((formSerialized[i].value.toLowerCase() !== 'false') || !nameValues[formSerialized[i].name]) {
                nameValues[formSerialized[i].name] = formSerialized[i].value;
            }
        }

        for (nv in nameValues) {
            $.jStorage.set(nv, nameValues[nv]);
        }
    }

    api.cacheAllFormValues = function () {
        var isTransient = cacheIfTransient();
        var forms = $("form.nof-edit, form.nof-dialog, form.nof-dialog-file");

        if (forms.length) {

            if (!isTransient) {
                // this is too complex ! 
                // if it's atransient don't clear teh cache as previous page may have been dialog and want to keep 
                // dialog values for back. 
                clearAllButTransientsFromCache();
            }
            forms.each(function () {
                var formSerialized = $(this).serializeArray();
                cacheFormValues(formSerialized);
            });
        }

    };

    function replaceFormValues() {
        // do not replace form values on view models
        $("div.nof-objectedit > form.nof-edit, form.nof-dialog, form.nof-dialog-file").each(function () {


            $(this).find("div.nof-parameter:has(div.nof-object), div.nof-property:has(div.nof-object)").each(function(index, element) {
                var replaceWith = $.jStorage.get(element.id, null);
                if (replaceWith) {
                    $(element).html(replaceWith);
                }
            });

            $(this).find("input, select, textarea").each(function(index, element) {
                var replaceWith = $.jStorage.get($(element).attr("name"), null);
                if (replaceWith) {
                    if ($(element).attr("type") === 'checkbox') {
                        if (replaceWith.toLowerCase() === 'true') {
                            $(element).attr('checked', 'checked');
                        } else {
                            $(element).removeAttr('checked');
                        }
                    } else if ($(element).attr("type") !== 'hidden' || !!$(element).id) {
                        $(element).val(replaceWith);
                    }
                }
            });
        });
    }

    api.updateChoices = function () {
        api.ajaxCount++;

        var choicesData = $(this).closest("div[data-choices]");

        var selects = choicesData.find("select");

        if (choicesData.length == 0 || selects.length == 0) {
            safeDecrementAjaxCount();
            return true;
        }

        var form = $(this).closest("form");
        var url = choicesData.attr("data-choices");
        var parmsString = choicesData.attr("data-choices-parameters");
        var parms = parmsString.split(",");
        var encParms = $.map(parms, function (item) {
            return "-encryptedField-" + item;
        });

        if (!parmsString || ($.inArray($(this).attr('id'), parms) === -1 && $.inArray($(this).attr('id'), encParms) === -1)) {
            // not monitoring this field so return
            safeDecrementAjaxCount();
            return true;
        }

        var inData = {};

        var formSerialized = form.serializeArray();

        function findValues(id) {
            var valueMap = {};
            var valueIndex = 0; 

            for (var j = 0; j < formSerialized.length; j++) {
                var o = formSerialized[j];
                if (o.name === id) {
                   valueMap[id + valueIndex++] = o.value;
                }
            }
            return valueMap;
        }

        for (var i = 0; i < parms.length; i++) {
            var parmId = parms[i];
            var encryptParmId = "-encryptedField-" + parmId;
            var encryptValues = findValues(encryptParmId);

            if (Object.keys(encryptValues).length > 0) {
                for (var v in encryptValues) {
                    inData[v] = encryptValues[v];
                }
            } else {
                var rawValues = findValues(parmId);
                for (var vv in rawValues) {
                    inData[vv] = rawValues[vv];
                }
            }
        }

        $.ajaxSetup({ cache: false });
        $.getJSON(url, inData, function (data) {

            selects.each(function () {

                var id = $(this).attr("id");

                if (typeof data[id] !== "undefined") {
                    var content = data[id][1];
                    var options = $(this).find("option");

                    function contentEqualsOptions() {

                        if (content.length + 1 != options.length) {
                            return false;
                        }

                        for (var k = 0; k < content.length; k++) {
                            // manually single space here as ie8 handles differently from FF ie9 etc
                            // also trim as any white space on end will not be returned by '.text' 

                            var contentSingleSpace = $.trim(content[k].replace(/\s+/g, ' '));
                            var optionSingleSpace = $.trim(options[k + 1].text.replace(/\s+/g, ' '));

                            if (contentSingleSpace != optionSingleSpace) {
                                return false;
                            }
                        }

                        return true;
                    }

                    if (!contentEqualsOptions()) {
                        var values = data[id][0];
                        options.replaceWith("");
                        $(this).append("<option/>");
                        for (var j = 0; j < values.length; j++) {
                            $(this).append($("<option value='" + values[j] + "'>" + content[j] + "</option>"));
                        }
                        if ($(this).attr("multiple")) {
                            $(this).attr("size", values.length + 1);
                        }
                    }
                }
            });
        
        }).always(function () {
          
            safeDecrementAjaxCount();
        });

        return true; // ie8 needs two tabs to leave field otherwise  
    };

    function handleLoginForm(response) {
        // catch case when a login form comes back from the server (session timeout)
        // reload page to get a full login page. 

        if ($("section#loginForm", response).length > 0) {
            $.jStorage.flush();
            window.location.reload(true);
        }
    }

    api.updatePageFromAction = function (event) {
        
        var button = getButton(event);

        if (!button || button.name === "Redisplay") {
            return true;
        }

        if ($(this).find(":input[type=file]").length > 0) {
            return setFile($(this), button);
        }
        
        api.ajaxCount++;

        // cache before disabled
        api.cacheAllFormValues();
        startSubmitFeedBack(button);
        var formSerialized = $(this).serializeArray();
        formSerialized[formSerialized.length] = { name: button.name, value: button.value };
        var isDialog = $(this).attr("class").indexOf("nof-dialog") === 0;

        $.post($(this).attr("action"), formSerialized, function (response) {

            if (updateLocationFlag) {
                // just reload the page                                            
                updateLocation(api.getLinkFromHistory(response));
                return;
            }


            handleLoginForm(response);

            if (button.name === "Finder" || button.name === "Selector" || button.name === "ActionAsFinder" || button.name === "InvokeActionAsFinder" || button.name === "InvokeActionAsSave") {

                var divClass = isDialog ? "nof-parameter" : "nof-property";

                var propertySelector = "div." + divClass + ":has(button[value='" + $(button).attr("value") + "'])";
                var propertyId = $(propertySelector).attr("id");

                var replaceWith = $("div#" + propertyId, response);

                if (replaceWith.length > 0) {
                    $(propertySelector).replaceWith(replaceWith);
                    $("#" + propertyId).find(":input").each(api.updateChoices);
                } else {
                    replacePageBody(response);
                }
            } else {           
                replacePageBody(response);
            }

            bindToNewHtml(true);
            cacheIfTransient();
        }).fail(function (jqXHR) {
            var response = jqXHR.responseText;
            if (response) {
                replacePageBody(response);           
            }
        }).always(function () {
            endSubmitFeedBack();
            safeDecrementAjaxCount();
        });
        
        return false;
    };

    function getIdFromUrl(url) {
        var id = url.substring(url.indexOf('id=') + 3);
        return unescape(id);
    }

    api.isValid = function (draggable, droppable) {
        api.ajaxCount++;

        var url = droppable.attr("data-validate");

        if (!url) {
            safeDecrementAjaxCount();
            return;
        }

        var draggableUrl = draggable.find("a").attr("href");
        var value = getIdFromUrl(draggableUrl);
        var inData = { value: value };

        $.ajaxSetup({ cache: false });
        $.getJSON(url, inData, function (data) {
            if (data === true) {
                droppable.addClass("nof-validdrop");

                // if we go valid when already inside droppable trigger hover
                var dd = $.ui.ddmanager.current;

                $.ui.ddmanager.prepareOffsets(dd);
                $.each($.ui.ddmanager.droppables[dd.options.scope] || [], function () {
                    if ($.ui.intersect(dd, this, dd.options.tolerance || 'intersect')) {
                        if (this.element.hasClass("nof-validdrop")) {
                            this.element.addClass("nof-withindrop");
                        }
                    }
                });
            }
        }).always(function () {
            endSubmitFeedBack();
            safeDecrementAjaxCount();
        });
    };

    function updateHiddenValueBehindField(hiddenInput, newValue) {
        $(hiddenInput).attr("value", newValue);

        // if encrypted remove indication 
        var name = $(hiddenInput).attr("name");

        if (name.indexOf("-encryptedField-") === 0) {
            name = name.substring(16);
            $(hiddenInput).attr("name", name);
        }

        $(hiddenInput).each(api.updateChoices);
    }


    api.updateOnSelect = function () {
        var propOrParm = $(this).closest("div.nof-property > div.nof-object");

        if (propOrParm.length == 0) {
            propOrParm = $(this).closest("div.nof-parameter > div.nof-object");
        }

        var newObject = $(this).closest("div.nof-object");

        var autoComplete = propOrParm.find("input[data-completions]");
        var a = propOrParm.find("> a");
        var newA = newObject.find("a");
        var newObjectUrl = newA.attr("href");
        var value = getIdFromUrl(newObjectUrl);

        var linkTitle = newObject.contents().filter(function () { return this.nodeType == 3; }).text();


        if (autoComplete.length > 0) {
            $(autoComplete).val(linkTitle);
        } else {
            if (a.length > 0) {
                a.replaceWith(newA);
            } else {
                propOrParm.prepend(newA);
            }
            propOrParm.find("> a").text(linkTitle);
        }

        var img = propOrParm.find("> img");
        var newImg = newObject.find("img");

        if (img.length > 0) {
            img.replaceWith(newImg);
        } else {
            propOrParm.prepend(newImg);
        }

        var hiddenInput = propOrParm.find("input:last");

        updateHiddenValueBehindField(hiddenInput, value);

        $(this).closest("div.nof-collection-list").remove();

        $(hiddenInput).each(api.updateChoices);
    };

    function setupAutoComplete(index, item) {
        var sourceHandler = function (request, response) {
            api.ajaxCount++;

            var inData = {};

            inData["autoCompleteParm"] = request.term;
            var url = $(item).attr("data-completions");


            $.ajaxSetup({ cache: false });
            $.getJSON(url, inData, function (data) {
                response(data);
            }).always(function () {     
                safeDecrementAjaxCount();
            });
            return true;
        };

        var selectHandler = function (event, ui) {

            if ($(item).parent(".nof-object").length > 0) {
                // refence fielsd so update image and underlying value 

                var img = $(item).siblings("img");

                if (img.length == 0) {
                    $(item).before($("<img/>"));
                }

                $(item).siblings("img").attr("src", ui.item.src);
                $(item).siblings("img").attr("alt", ui.item.alt);

                var hiddenInput = $(item).siblings("input");

                updateHiddenValueBehindField(hiddenInput, ui.item.link);
            }

            // kludge - want enter to select item but not submit form 
            // ignore next enter key if select was done with an enter 
            ignoreNextEnter = event.keyCode && event.keyCode === 13;
            return true;
        };

        var clearHandler = function () {
            var value = $(this).val();

            if (value.length == 0) {
                updateHiddenValueBehindField($(this).siblings("input:last"), "");           
            }
        };

        var minLength = $(item).attr("data-completions-minlength");

        $(item).autocomplete({ autoFocus: true, minLength: minLength, source: sourceHandler, select: selectHandler });

        $(item).change(clearHandler);
    }

    function bindToNewHtml(updateLink) {

        $("div.nof-history div.nof-object").draggable({
            helper: 'clone',
            start: function () {
                var draggable = $(this);
                $(".ui-droppable").each(function () {
                    api.isValid(draggable, $(this));
                });
            }
        });

        $("form div.nof-object").droppable({

            drop: function (event, ui) {
                var draggableUrl = ui.helper.find("a").attr("href");
                var value = getIdFromUrl(draggableUrl);

                var a = $(this).find("a");
                var newA = ui.helper.find("a");
                var input = $(this).find("input[data-completions]");

                if (a.length > 0) {
                    a.replaceWith(newA);
                }
                else if (input.length > 0) {
                    $(input).attr("value", newA.text());
                }
                else {
                    $(this).prepend(newA);
                }

                var img = $(this).find("img");
                var newImg = ui.helper.find("img");

                if (img.length > 0) {
                    img.replaceWith(newImg);
                }
                else {
                    $(this).prepend(newImg);
                }

                var hiddenInput = $(this).find("input:last");

                updateHiddenValueBehindField(hiddenInput, value);

            },

            deactivate: function () {
                $(".nof-validdrop").removeClass("nof-validdrop");
                $(".nof-withindrop").removeClass("nof-withindrop");
            },

            accept: function () {
                return $(this).hasClass("nof-validdrop");
            },

            hoverClass: 'nof-withindrop'

        });
        api.focusOnFirst();
        if (updateLink) {
            updateLinkFromHistory();
            
        }

        $.validator.unobtrusive.parse($(".main-content"));
        // if we are reloading html from cache may still have hasDatepicker attribute. This will prevent 
        // date picker being attached 
        $("input.datetime").removeClass("hasDatepicker");
        $("input.datetime").datepicker();

        // if datepicker has focus a click will not pop it up - so manually do it 
        $("input.datetime:focus").click(function (e) { $(e).datepicker('show'); });

        // bug in Chrome means that date validation doesn't work if not US (chrome defaults to US dates). 
        // so for the moment just turn off date validation on chrome browsers - date will still be validated on the server
        // there are a bunch of fixes around (eg using dateITA in additional-methods) but so far all have tested out unsatisfactory
        // just turning off for chrome seems safest pending a proper fix 
        // https://github.com/jzaefferer/jquery-validation/issues/549

        // rather than check browser check if underlying bug shows up
        // http://geekswithblogs.net/EltonStoneman/archive/2009/10/29/jquery-date-validation-in-chrome.aspx

        if (isNaN(new Date('29/10/2009').getTime())) {
            // browser cannot handle non-us dates - turn off date validation 
            $.validator.methods["date"] = function () { return true; };
        }

        // autocomplete 

        $("input[data-completions]").each(setupAutoComplete);

        setMinMaxButtonStates();
        setCollectionButtonStates();

        drawHistoryMenus();

        api.bindAjaxError();
    }

    function updatePage(doc, updateLink) {
        api.ajaxCount++;
        $.ajaxSetup({ cache: false });
        var link = doc.attr("href");

        api.cacheAllFormValues();

        if (link.substring(0, 14) === '/Transient?id=') {

            var id = unescape(link.substring(14));
            var cachedPage = $.jStorage.get("transient:" + id);

            if (cachedPage) {
                $(".main-content").html(cachedPage);
                replaceFormValues();
                bindToNewHtml(updateLink);
                safeDecrementAjaxCount();
                return false;
            }
        }

        $.get(link, function (response) {

            handleLoginForm(response);

            replacePageBody(response);
            replaceFormValues();
            bindToNewHtml(updateLink);        
        
        }).fail(function (jqXHR) {
            var response = jqXHR.responseText;
            if (response) {
                replacePageBody(response);
                
            }
        }).always(function () {
            endLinkFeedBack();
            safeDecrementAjaxCount();
        });
        
        return false;
    }

    api.updatePageFromLink = function () {
        startLinkFeedBack();
        return updatePage($(this), true);
    };
 
    function uploadProgress(evt) {
        if (evt.lengthComputable) {
            var percentComplete = Math.round(evt.loaded * 100 / evt.total);
            $("#progressIndicator").progressbar({ value: percentComplete });
        }
    }

    function uploadComplete(evt) {    
        replacePageBody(evt.target.responseText);
        bindToNewHtml(true);
        endSubmitFeedBack();
    }

    function uploadFailed(evt) {
        replacePageBody(evt.target.responseText);
        bindToNewHtml(true);
        endSubmitFeedBack();
    }
    
    function uploadTimedOut(evt) {
        replacePageBody(evt.target.responseText);
        bindToNewHtml(true);
        endSubmitFeedBack();
        errorDialog("Timeout", "File upload timedout");
    }

    function uploadFile(url, fd) {
        var xhr = new XMLHttpRequest();

        xhr.upload.addEventListener("progress", uploadProgress, false);
        xhr.addEventListener("load", uploadComplete, false);
        xhr.addEventListener("error", uploadFailed, false);
        xhr.addEventListener("abort", uploadFailed, false);
        xhr.addEventListener("timeout", uploadTimedOut, false);

        xhr.open("POST", url);

        // a minute ?
        xhr.timeout = fileUploadTimeOut;
        xhr.send(fd);
    }

    function downloadComplete(evt) {

        var headers = evt.target.getAllResponseHeaders();
        var dispRe = /content-disposition:/i;

        if (dispRe.exec(headers)) { // match on content-disposition header 
            downloadSucceeded(evt);
        } else {
            // file contents actually contains error response from server      
            // need to convert to string to display as html

            var reader = new FileReader();
            
            reader.onloadend = function (revt) {
                replacePageBody(revt.target.result);
                bindToNewHtml(true);
            };

            reader.readAsText(evt.target.response);
        }
    }

    function dialogDownloadComplete(evt) {
        closePopupDialog();
        downloadComplete(evt);
    }

    function downloadFailed(evt) {
        replacePageBody(evt.target.responseText);
        bindToNewHtml(true);
    }
   
    function getFileName(evt) {
        var headers = evt.target.getAllResponseHeaders();
        var re = /filename=(.*)/;
        var match = re.exec(headers);
        return match.length == 2 ? match[1] : "untitledFile.txt";
    }

    function fileErrorHandler(e) {
        errorDialog(e.name || "Unknown", e.message || "Unknown");
    }

    window.requestFileSystem = window.requestFileSystem || window.webkitRequestFileSystem;

    function downloadSucceeded(evt) {
            
        if (window.navigator.msSaveBlob) {
            // internet explorer 
            window.navigator.msSaveBlob(evt.target.response, getFileName(evt));
        }
        else if (window.requestFileSystem) {
            // Chrome 
            var toWrite = evt.target.response;
            var bytesNeeded = toWrite.size;
            var quota = tempStoreQuota;
            
            window.webkitStorageInfo.requestQuota(window.TEMPORARY, quota, function (grantedBytes) {
                console.log('Granted', grantedBytes);
  
                window.requestFileSystem(window.TEMPORARY, bytesNeeded,
                    function (fs) {
                        fs.root.getFile(getFileName(evt), { create: true }, function (fileEntry) {

                            // Create a FileWriter object for our FileEntry (
                            fileEntry.createWriter(function (fileWriter) {

                                fileWriter.onwriteend = function (e) {
                                    if (!e.target.error) {
                                        var url = fileEntry.toURL();
                                        window.location = url;
                                    }
                                };

                                fileWriter.onerror = function (e) {
                                    fileErrorHandler(e.target.error);
                                };

                                fileWriter.write(toWrite);

                            }, fileErrorHandler);

                        }, fileErrorHandler);

                    },
                    fileErrorHandler);

            }, fileErrorHandler);
        }
        else
        {
            // Firefox and browsers that don't support FileWriter
            var burl = URL.createObjectURL(evt.target.response);
            window.location = burl;
            //URL.revokeObjectURL(burl);
        }
    }

    function supportsHtml5FileHandling() {
        return window.navigator.msSaveBlob || window.requestFileSystem || typeof(URL) != 'undefined';
    }

    function downloadFile(url, method, fd, success, failure) {
        var xhr = new XMLHttpRequest();
        xhr.open(method, url, true);
        xhr.responseType = "blob";
        
        // when uploading 
      
        xhr.upload.addEventListener("progress", uploadProgress, false);
        
        xhr.addEventListener("load", success, false);
        xhr.addEventListener("error", failure, false);
        xhr.addEventListener("abort", failure, false);

        xhr.send(fd);
    }

    function getFile(url, method, fd, success, failure) {
        downloadFile(url, method, fd, success, failure);
        return false;
    };

    api.getFileFromDialog = function (event) {
        if (supportsHtml5FileHandling()) {
            var button = getButton(event);
            startSubmitFeedBack(button);

            var formElement = $(this).get()[0];
            var fd = new FormData(formElement);

            if ($(this).find(":input[type=file]").length > 0) {
                // uploading file so add progress bar
                addProgressIndicator($(this));
            }

            return getFile($(this).attr("action"), 'POST', fd,
                function(evt) {
                    endSubmitFeedBack();
                    dialogDownloadComplete(evt);
                },
                function(evt) {
                    endSubmitFeedBack();
                    downloadFailed(evt);
                });
        }
        errorDialog("Unsupported action", "This browser does not support downloading files from a dialog");
        return false;
    };
    

    api.getFileFromAction = function (event) {
        if (supportsHtml5FileHandling()) {

            var button = getButton(event);
            startSubmitFeedBack(button);

            return getFile($(this).attr("action"), 'POST', null,
                function(evt) {
                    endSubmitFeedBack();
                    downloadComplete(evt);
                },
                function(evt) {
                    endSubmitFeedBack();
                    downloadFailed(evt);
                });
        }
        return true;
    };

    api.getFileFromLink = function () {
        if (supportsHtml5FileHandling()) {
            startLinkFeedBack();
            return getFile($(this).attr("href"), 'GET', null,
                function(evt) {
                    endLinkFeedBack();
                    downloadComplete(evt);
                },
                function(evt) {
                    endLinkFeedBack();
                    downloadFailed(evt);
                });
        }
        return true;
    };

    function addProgressIndicator(form) {
        form.append("<div id='progressIndicator'></div>");
        $("#progressIndicator").progressbar({ value: 0 });
    }

    function setFile(form, button) {

        if (new XMLHttpRequest().upload) {

            startSubmitFeedBack(button);

            var url = form.attr("action");
            var formElement = form.get()[0];
            var fd = new FormData(formElement);

            addProgressIndicator(form);

            uploadFile(url, fd);

            return false;
        }
        return true; 
    }

    api.syncPageToAddress = function () {
        startLinkFeedBack();
        return updatePage($(this), false);
    };

    api.markedClicked = function () {
        $("form button.nof-lastClicked").removeClass("nof-lastClicked");
        $(this).addClass("nof-lastClicked");
        return true;
    };

    api.allowSubmit = function () {
        // this allows these buttons to submit form even if invalid - for finders/selectors etc
        $(this).closest("form").validate().cancelSubmit = true;
        return true;
    };

    api.resetLocation = function (link) {
        if (link) {
            var homepath = $("div.no-home-path > a").attr("href") || $("header nav a").attr("href");
            if (location.pathname !== homepath) {
                location.href = location.protocol + "//" + location.host + homepath + "/#" + link;
            }
        }
    };


    function updateLocation(link) {
       
        if (updateLocationFlag) {
            // this is when we've left a non-ajax dialog and want to restore the ajax style url.
            api.resetLocation(link);       
        }
        updateLocationFlag = false;
    }


    return api;
} ());

$.address.externalChange(function (event) {
    if (event.value === "/") {
        // ignore root as it causes problems on IIS installations 
        return;
    }
    $("<a href='" + event.value + "'></a>").click(nakedObjects.syncPageToAddress).click();
});

$(window).unload(function () {
   nakedObjects.cacheAllFormValues(); 
});


$(window).load(function () {
    var url = location.href;
    if (url.indexOf('#') === -1) {
        var link = nakedObjects.getLinkFromHistory();
        nakedObjects.resetLocation(link);
    }
    return true;
});

window.onerror = function (msg, url, linenumber) {
    alert('Error message: ' + msg + '\nURL: ' + url + '\nLine Number: ' + linenumber);
    return true;
};

nakedObjects.bindAjaxError();

// jquery live binds
$(function () { $(document).on("click", "form button", nakedObjects.markedClicked); });
$(function () { $(document).on('click', "form button[name=Finder], form button[name=Redisplay], form button[name=Selector], form button[name=ActionAsFinder], form button[name=InvokeActionAsFinder], form button[name=InvokeActionAsSave]", nakedObjects.allowSubmit); });
$(function () { $(document).on('click', "form button[title=Select]", nakedObjects.updateOnSelect); });
$(function () { $(document).on("change", ":input", nakedObjects.updateChoices); });
$(function () { $(document).on("submit", ".nof-history form", nakedObjects.clearHistory); });
$(function () { $(document).on("submit", ".nof-tab form:has('button.nof-clear')", nakedObjects.clearTabbedHistory); });
$(function () { $(document).on("submit", ".nof-tab form:has('button.nof-clear-item')", nakedObjects.clearHistoryItem); });
$(function () { $(document).on("submit", ".nof-tab form:has('button.nof-clear-others')", nakedObjects.clearHistoryOthers); });
$(function () { $(document).on("submit", "form:has(button.nof-summary), form.nof-edit", nakedObjects.redisplayProperty); });
$(function () { $(document).on("submit", "form:has(button.nof-maximize)", nakedObjects.redisplayInlineProperty); });
$(function () { $(document).on("submit", ".nof-menu form.nof-action, .nof-propertylist form.nof-action, .nof-objectedit form.nof-action, .nof-standalonetable form.nof-action, form.nof-edit, form.nof-dialog, .nof-actiondialog form.nof-action", nakedObjects.updatePageFromAction); });
$(function () { $(document).on("submit", "form.nof-action-file", nakedObjects.getFileFromAction); });
$(function () { $(document).on("submit", "form.nof-dialog-file", nakedObjects.getFileFromDialog); });
$(function () { $(document).on("click", "div.nof-object > a, div.nof-tab > a", nakedObjects.updatePageFromLink); });
$(function () { $(document).on("click", "div.nof-value > a", nakedObjects.getFileFromLink); });
$(function () { $(document).on("change", '#checkboxAll', function() {
     $("input.nof-checkbox").prop('checked', $('#checkboxAll').is(':checked'));
}); }); // use change not click for ie8
$(function () { $(document).on("keydown", "form :input:not(textarea)", nakedObjects.checkForEnter); });

//Comment-out this function to restore operation of browser context menu
$(function () { $(document).on("contextmenu", ".nof-tab", function () { return false; }); });