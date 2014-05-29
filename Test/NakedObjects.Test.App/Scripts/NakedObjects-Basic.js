var nakedObjectsBasic = (function () {

    var api = {};
    api.checkForEnter = function (event) {
        if (event.keyCode === 13) { // 13 = Enter
            // Prevent the standard 'enter' action from occurring, and click on the first found OK button instead.
            $("button.nof-ok").first().click();
            // Return false to prevent the standard 'enter' action.
            return false;
        }
        else {
            return true;
        }
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

    api.focusOnFirst = function () {
        setMinMaxButtonStates();
        setCollectionButtonStates();

        $("input[type='text'],input[type='checkbox'],select,textarea").first().focus();
        $("div.nof-actiondialog input[type='text'], div.nof-finddialog input[type='text']").keydown(checkForEnter);
    };

    return api;
} ());

$(document).ready(nakedObjectsBasic.focusOnFirst);
$(function () { $(document).on("click", '#checkboxAll', function () { $("input[type='checkbox']").attr('checked', $('#checkboxAll').is(':checked')); }); });
$(function () { $('.datetime').datepicker(); });
$(document).keypress(nakedObjectsBasic.checkForEnter);