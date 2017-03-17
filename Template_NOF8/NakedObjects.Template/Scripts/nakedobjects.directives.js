/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="typings/moment/moment.d.ts"/>
var NakedObjects;
(function (NakedObjects) {
    var Link = NakedObjects.Models.Link;
    var Value = NakedObjects.Models.Value;
    var toDateString = NakedObjects.Models.toDateString;
    var EntryType = NakedObjects.Models.EntryType;
    NakedObjects.app.directive("geminiDatepicker", function (mask, $timeout) {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            // to make sure dynamic ids on element get picked up
            transclude: true,
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: "&" // Bind the select function we refer to the right scope
            },
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel)
                    return;
                // only add datepicker if date field not supported 
                if (element.prop("type") === "date")
                    return;
                var parent = scope.$parent;
                var viewModel = parent.parameter || parent.property;
                // adding parser at the front that converts to a format angluar parsers understand
                ngModel.$parsers.reverse();
                ngModel.$parsers.push(function (val) {
                    var dt1 = moment(val, NakedObjects.supportedDateFormats, "en-GB", true);
                    if (dt1.isValid()) {
                        var dt = dt1.toDate();
                        return toDateString(dt);
                    }
                    return "";
                });
                ngModel.$parsers.reverse();
                // add our formatter that converts from date to our format
                ngModel.$formatters = [];
                // use viewmodel filter if we've been given one 
                var localFilter = viewModel && viewModel.localFilter ? viewModel.localFilter : mask.defaultLocalFilter("date");
                ngModel.$formatters.push(function (val) { return localFilter.filter(val); });
                // put on viewmodel for error message formatting
                if (viewModel && !viewModel.localFilter) {
                    viewModel.localFilter = localFilter;
                }
                // also for dynamic ids - need to wrap link in timeout. 
                $timeout(function () {
                    var updateModel = function (dateTxt) {
                        scope.$apply(function () {
                            // Call the internal AngularJS helper to
                            // update the two way binding
                            ngModel.$setViewValue(dateTxt);
                            element.change(); // do this to trigger gemini-clear directive  
                        });
                    };
                    var onSelect = function (dateTxt) { return updateModel(dateTxt); };
                    var optionsObj = {
                        dateFormat: "d M yy",
                        onSelect: onSelect,
                        showOn: "button",
                        buttonImage: "images/calendar.png",
                        buttonImageOnly: true,
                        buttonText: "Select date"
                    };
                    element.datepicker(optionsObj);
                });
            }
        };
    });
    NakedObjects.app.directive("geminiTimepicker", function (mask, $timeout) {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            // to make sure dynamic ids on element get picked up
            transclude: true,
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: "&" // Bind the select function we refer to the right scope
            },
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel)
                    return;
                // only add datepicker if time field not supported 
                if (element.prop("type") === "time")
                    return;
                var parent = scope.$parent;
                var viewModel = parent.parameter || parent.property;
                // also for dynamic ids - need to wrap link in timeout. 
                $timeout(function () {
                    var updateModel = function () {
                        scope.$apply(function () {
                            // Call the internal AngularJS helper to
                            // update the two way binding
                            ngModel.$setViewValue(element.val());
                            element.change(); // do this to trigger gemini-clear directive 
                        });
                        return true;
                    };
                    var optionsObj = {
                        timeFormat: "H:i",
                        showOn: null
                    };
                    element.timepicker(optionsObj);
                    element.on("changeTime", updateModel);
                    var button = $("<img class='ui-datepicker-trigger' src='images/calendar.png' alt='Select time' title='Select time'>");
                    button.on("click", function () { return element.timepicker("show"); });
                    element.after(button);
                });
            }
        };
    });
    NakedObjects.app.directive("geminiAutocomplete", function (color) {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: "&" // Bind the select function we refer to the right scope
            },
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel)
                    return;
                var optionsObj = {};
                var parent = scope.$parent;
                var viewModel = parent.parameter || parent.property;
                function render(initialChoice) {
                    var cvm = ngModel.$modelValue || initialChoice;
                    if (cvm) {
                        ngModel.$parsers.push(function () { return cvm; });
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    }
                }
                ;
                ngModel.$render = render;
                var updateModel = function (cvm) {
                    scope.$apply(function () {
                        viewModel.clear();
                        ngModel.$parsers.push(function () { return cvm; });
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    });
                };
                optionsObj.source = function (request, response) {
                    scope.$apply(function () {
                        return scope.select({ request: request.term }).
                            then(function (cvms) { return response(_.map(cvms, function (cvm) { return ({ "label": cvm.name, "value": cvm }); })); }).
                            catch(function () { return response([]); });
                    });
                };
                optionsObj.select = function (event, ui) {
                    updateModel(ui.item.value);
                    return false;
                };
                optionsObj.focus = function () { return false; };
                optionsObj.autoFocus = true;
                optionsObj.minLength = viewModel.minLength;
                var clearHandler = function () {
                    var value = $(this).val();
                    if (value.length === 0) {
                        updateModel(NakedObjects.ChoiceViewModel.create(new Value(""), ""));
                    }
                };
                element.keyup(clearHandler);
                element.autocomplete(optionsObj);
                render(viewModel.selectedChoice);
                ngModel.$validators.geminiAutocomplete = function (modelValue, viewValue) {
                    // return OK if no value or value is of correct type.
                    if (viewModel.optional && !viewValue) {
                        // optional with no value
                        viewModel.resetMessage();
                        viewModel.clientValid = true;
                    }
                    else if (!viewModel.optional && !viewValue) {
                        // mandatory with no value
                        viewModel.resetMessage();
                        viewModel.clientValid = false;
                    }
                    else if (modelValue instanceof NakedObjects.ChoiceViewModel) {
                        // has view model check if it's valid                       
                        if (!modelValue.name) {
                            viewModel.setMessage(NakedObjects.pendingAutoComplete);
                            viewModel.clientValid = false;
                        }
                    }
                    else {
                        // has value but not ChoiceViewModel so must be invalid 
                        viewModel.setMessage(NakedObjects.pendingAutoComplete);
                        viewModel.clientValid = false;
                    }
                    return viewModel.clientValid;
                };
            }
        };
    });
    NakedObjects.app.directive("geminiConditionalchoices", function () {
        return {
            // up the priority of this directive to that viewmodel is set before ng-options - 
            // then angular doesn't add an empty entry on dropdown
            priority: 10,
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: "&" // Bind the select function we refer to the right scope
            },
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel)
                    return;
                var parent = scope.$parent;
                var viewModel = parent.parameter || parent.property;
                var pArgs = _.omit(viewModel.promptArguments, "x-ro-nof-members");
                var paneId = viewModel.onPaneId;
                var currentOptions = [];
                function isDomainObjectViewModel(object) {
                    return object && "properties" in object;
                }
                function mapValues(args, parmsOrProps) {
                    return _.mapValues(pArgs, function (v, n) {
                        var pop = _.find(parmsOrProps, function (p) { return p.argId === n; });
                        return pop.getValue();
                    });
                }
                function populateArguments() {
                    var dialog = parent.dialog;
                    var object = parent.object;
                    if (!dialog && !object) {
                        throw { message: "Expect dialog or object in geminiConditionalchoices", stack: "" };
                    }
                    var parmsOrProps = [];
                    if (dialog) {
                        parmsOrProps = dialog.parameters;
                    }
                    if (isDomainObjectViewModel(object)) {
                        parmsOrProps = object.properties;
                    }
                    return mapValues(pArgs, parmsOrProps);
                }
                function populateDropdown() {
                    var nArgs = populateArguments();
                    var prompts = scope.select({ args: nArgs });
                    prompts.then(function (cvms) {
                        // if unchanged return 
                        if (cvms.length === currentOptions.length && _.every(cvms, function (c, i) { return c.equals(currentOptions[i]); })) {
                            return;
                        }
                        element.find("option").remove();
                        if (viewModel.optional) {
                            var emptyOpt = $("<option></option>");
                            element.append(emptyOpt);
                        }
                        _.forEach(cvms, function (cvm) {
                            var opt = $("<option></option>");
                            opt.val(cvm.getValue().toValueString());
                            opt.text(cvm.name);
                            element.append(opt);
                        });
                        currentOptions = cvms;
                        if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
                            var vals = _.map(viewModel.selectedMultiChoices, function (c) { return c.getValue().toValueString(); });
                            $(element).val(vals);
                        }
                        else if (viewModel.selectedChoice) {
                            $(element).val(viewModel.selectedChoice.getValue().toValueString());
                        }
                        else {
                            $(element).val("");
                        }
                        setTimeout(function () {
                            $(element).change();
                        }, 1);
                    }).catch(function () {
                        // error clear everything 
                        element.find("option").remove();
                        viewModel.selectedChoice = null;
                        currentOptions = [];
                    });
                }
                function wrapReferences(val) {
                    if (val && viewModel.type === "ref") {
                        return { href: val };
                    }
                    return val;
                }
                function optionChanged() {
                    if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
                        var options = $(element).find("option:selected");
                        var kvps_1 = [];
                        options.each(function (n, e) { return kvps_1.push({ key: $(e).text(), value: $(e).val() }); });
                        var cvms = _.map(kvps_1, function (o) { return NakedObjects.ChoiceViewModel.create(new Value(wrapReferences(o.value)), viewModel.id, o.key); });
                        viewModel.selectedMultiChoices = cvms;
                    }
                    else {
                        var option = $(element).find("option:selected");
                        var val = option.val();
                        var key = option.text();
                        var cvm_1 = NakedObjects.ChoiceViewModel.create(new Value(wrapReferences(val)), viewModel.id, key);
                        viewModel.selectedChoice = cvm_1;
                        scope.$apply(function () {
                            ngModel.$parsers.push(function () { return cvm_1; });
                            ngModel.$setViewValue(cvm_1.name);
                        });
                    }
                }
                function setListeners() {
                    _.forEach(pArgs, function (v, n) { return $("#" + n + paneId).on("change", function () { return populateDropdown(); }); });
                    $(element).on("change", optionChanged);
                }
                ngModel.$render = function () { }; // do on the next event loop,
                setTimeout(function () {
                    setListeners();
                    // initial populate
                    // do this initially so that there is a valid model 
                    // otherwise angular will insert another empty value giving two 
                    element.find("option").remove();
                    if (viewModel.optional) {
                        var emptyOpt = $("<option></option>");
                        element.append(emptyOpt);
                        $(element).val("");
                    }
                    populateDropdown();
                }, 1);
            }
        };
    });
    //The 'right-click' functionality is also triggered by shift-enter
    NakedObjects.app.directive("geminiRightclick", function ($parse) { return function (scope, element, attrs) {
        var fn = $parse(attrs.geminiRightclick);
        element.bind("contextmenu", function (event) {
            scope.$apply(function () {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
        element.bind("keydown keypress", function (event) {
            var enterKeyCode = 13;
            if (event.keyCode === enterKeyCode && event.shiftKey) {
                scope.$apply(function () { return scope.$eval(attrs.geminiRightclick); });
                event.preventDefault();
            }
        });
    }; });
    var draggableVmKey = "dvmk";
    NakedObjects.app.directive("geminiDrag", function ($compile) { return function (scope, element) {
        var cloneDraggable = function () {
            var cloned;
            // make the dragged element look like a reference 
            if ($(element)[0].nodeName.toLowerCase() === "tr") {
                var title = $(element).find("td.cell:first").text();
                cloned = $("<div>" + title + "</div>");
            }
            else {
                cloned = $(element).clone();
            }
            cloned.removeClass();
            cloned.addClass("reference");
            return cloned;
        };
        element.draggable({
            helper: cloneDraggable,
            zIndex: 9999
        });
        element.on("dragstart", function (event, ui) {
            var draggableVm = scope.property || scope.item || scope.$parent.object;
            // make sure dragged element is correct color (object will not be set yet)
            ui.helper.addClass(draggableVm.color);
            // add vm to helper and original elements as accept and drop use different ones 
            ui.helper.data(draggableVmKey, draggableVm);
            element.data(draggableVmKey, draggableVm);
        });
        element.on("keydown keypress", function (event) {
            var cKeyCode = 67;
            if (event.keyCode === cKeyCode && event.ctrlKey) {
                var draggableVm = scope.property || scope.item || scope.$parent.object;
                var compiledClone = $compile("<div class='reference " + draggableVm.color + "' gemini-drag=''>" + element[0].textContent + "</div>")(scope);
                compiledClone.data(draggableVmKey, draggableVm);
                $("div.footer div.currentcopy").empty();
                $("div.footer div.currentcopy").append("<span>Copying...</span>").append(compiledClone);
                event.preventDefault();
            }
        });
    }; });
    NakedObjects.app.directive("geminiEnter", function () { return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            var enterKeyCode = 13;
            if (event.which === enterKeyCode && !event.shiftKey) {
                scope.$apply(function () { return scope.$eval(attrs.geminiEnter); });
                event.preventDefault();
            }
        });
    }; });
    NakedObjects.app.directive("geminiPlaceholder", function ($parse) { return function (scope, element, attrs) {
        var fn = $parse(attrs.geminiPlaceholder);
        element.attr("placeholder", fn(scope));
    }; });
    NakedObjects.app.directive("geminiFocuson", function ($timeout, focusManager) { return function (scope, elem) {
        scope.$on(NakedObjects.geminiFocusEvent, function (e, target, index, paneId, count) {
            $timeout(function () {
                var focusElements;
                switch (target) {
                    case NakedObjects.FocusTarget.Menu:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.home div.menu, div.single div.home div.menu");
                        break;
                    case NakedObjects.FocusTarget.SubAction:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.actions div.action, div.single div.actions div.action");
                        break;
                    case NakedObjects.FocusTarget.Action:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.action, div.single div.action");
                        break;
                    case NakedObjects.FocusTarget.ObjectTitle:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.object div.title, div.single div.object div.title");
                        break;
                    case NakedObjects.FocusTarget.Dialog:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.parameters div.parameter :input[type!='hidden'], div.single div.parameters div.parameter :input[type!='hidden']");
                        break;
                    case NakedObjects.FocusTarget.ListItem:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.collection td.reference, div.single div.collection td.reference");
                        break;
                    case NakedObjects.FocusTarget.Property:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.properties div.property :input[type!='hidden'], div.single div.properties div.property :input[type!='hidden']");
                        break;
                    case NakedObjects.FocusTarget.TableItem:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.collection tbody tr, div.single div.collection tbody tr");
                        break;
                    case NakedObjects.FocusTarget.Input:
                        focusElements = $(elem).find("input");
                        break;
                    case NakedObjects.FocusTarget.CheckBox:
                        focusElements = $(elem).find("#pane" + paneId + ".split div.collection td.checkbox input, div.single div.collection td.checkbox input");
                        break;
                    case NakedObjects.FocusTarget.MultiLineDialogRow:
                        focusElements = $(elem).find("div.parameters :input.value:first-child");
                        break;
                }
                if (focusElements) {
                    if (focusElements.length >= index) {
                        $(focusElements[index]).focus();
                    }
                    else {
                        // haven't found anything to focus - try again - but not forever
                        if (count < 10) {
                            focusManager.focusOn(target, index, paneId, ++count);
                        }
                    }
                }
            }, 0, false);
        });
    }; });
    NakedObjects.app.directive("geminiDrop", function ($timeout) { return function (scope, element) {
        var propertyScope = function () { return scope.$parent.$parent.$parent; };
        var parameterScope = function () { return scope.$parent.$parent; };
        var accept = function (draggable) {
            var droppableVm = propertyScope().property || parameterScope().parameter;
            var draggableVm = draggable.data(draggableVmKey);
            if (draggableVm) {
                draggableVm.canDropOn(droppableVm.returnType).
                    then(function (canDrop) {
                    if (canDrop) {
                        element.addClass("candrop");
                    }
                    else {
                        element.removeClass("candrop");
                    }
                }).
                    catch(function () { return element.removeClass("candrop"); });
                return true;
            }
            return false;
        };
        element.droppable({
            tolerance: "touch",
            hoverClass: "dropping",
            accept: accept
        });
        element.on("drop", function (event, ui) {
            if (element.hasClass("candrop")) {
                var droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                var droppableVm_1 = droppableScope.property || droppableScope.parameter;
                var draggableVm_1 = ui.draggable.data(draggableVmKey);
                droppableScope.$apply(function () {
                    droppableVm_1.drop(draggableVm_1);
                    $timeout(function () { return $(element).change(); });
                });
            }
        });
        element.on("keydown keypress", function (event) {
            var vKeyCode = 86;
            var deleteKeyCode = 46;
            if (event.keyCode === vKeyCode && event.ctrlKey) {
                var droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                var droppableVm_2 = droppableScope.property || droppableScope.parameter;
                var draggableVm_2 = $("div.footer div.currentcopy .reference").data(draggableVmKey);
                if (draggableVm_2) {
                    // only consume event if we are actually dropping on a field
                    event.preventDefault();
                    droppableScope.$apply(function () { return droppableVm_2.drop(draggableVm_2); });
                }
            }
            if (event.keyCode === deleteKeyCode) {
                var droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                var droppableVm = droppableScope.property || droppableScope.parameter;
                scope.$apply(droppableVm.clear);
            }
        });
    }; });
    NakedObjects.app.directive("geminiViewattachment", function (error) {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel) {
                    return;
                }
                ngModel.$render = function () {
                    var attachment = ngModel.$modelValue;
                    if (attachment) {
                        var title_1 = attachment.title;
                        element.empty();
                        attachment.downloadFile().
                            then(function (blob) {
                            var reader = new FileReader();
                            reader.onloadend = function () { return element.html("<img src='" + reader.result + "' alt='" + title_1 + "' />"); };
                            reader.readAsDataURL(blob);
                        }).
                            catch(function (reject) { return error.handleError(reject); });
                    }
                };
            }
        };
    });
    // this is a very simple implementation unlikely to work with large files, no chunking etc   
    NakedObjects.app.directive("geminiFileupload", function () {
        return {
            restrict: "A",
            scope: true,
            require: "?ngModel",
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel) {
                    return;
                }
                element.bind("change", function () {
                    var file = element[0].files[0];
                    var fileReader = new FileReader();
                    fileReader.onloadend = function () {
                        var link = new Link({
                            href: fileReader.result,
                            type: file.type,
                            title: file.name
                        });
                        ngModel.$setViewValue(link);
                    };
                    fileReader.readAsDataURL(file);
                });
            }
        };
    });
    NakedObjects.app.directive("geminiAttachment", function ($compile, $window, error) {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel) {
                    return;
                }
                var clickHandler = function () {
                    var attachment = ngModel.$modelValue;
                    if (!attachment.displayInline()) {
                        attachment.downloadFile().
                            then(function (blob) {
                            if (window.navigator.msSaveBlob) {
                                // internet explorer 
                                window.navigator.msSaveBlob(blob, attachment.title);
                            }
                            else {
                                var burl = URL.createObjectURL(blob);
                                $window.location.href = burl;
                            }
                        }).
                            catch(function (reject) { return error.handleError(reject); });
                    }
                    return false;
                };
                ngModel.$render = function () {
                    var attachment = ngModel.$modelValue;
                    if (attachment) {
                        var title_2 = attachment.title;
                        element.empty();
                        if (attachment.displayInline()) {
                            attachment.downloadFile().
                                then(function (blob) {
                                var reader = new FileReader();
                                reader.onloadend = function () {
                                    if (reader.result) {
                                        element.html("<img src='" + reader.result + "' alt='" + title_2 + "' />");
                                    }
                                };
                                reader.readAsDataURL(blob);
                            }).
                                catch(function (reject) { return error.handleError(reject); });
                        }
                        else {
                            element.html("<div>" + title_2 + "</div>");
                            attachment.doClick = clickHandler;
                        }
                    }
                    else {
                        element.append("<div>Attachment not yet supported on transient</div>");
                    }
                };
            }
        };
    });
    NakedObjects.app.directive("ciceroDown", function () { return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            var enterKeyCode = 40;
            if (event.which === enterKeyCode) {
                scope.$apply(function () { return scope.$eval(attrs.ciceroDown); });
                event.preventDefault();
            }
        });
    }; });
    NakedObjects.app.directive("ciceroUp", function () { return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            var enterKeyCode = 38;
            if (event.which === enterKeyCode) {
                scope.$apply(function () { return scope.$eval(attrs.ciceroUp); });
                event.preventDefault();
            }
        });
    }; });
    NakedObjects.app.directive("ciceroSpace", function () { return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            var tabKeyCode = 32;
            if (event.which === tabKeyCode) {
                scope.$apply(function () { return scope.$eval(attrs.ciceroSpace); });
                event.preventDefault();
            }
        });
    }; });
    NakedObjects.app.directive("geminiFieldvalidate", function () { return ({
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {
            ctrl.$validators.geminiFieldvalidate = function (modelValue, viewValue) {
                var parent = scope.$parent;
                var viewModel = parent.parameter || parent.property;
                return viewModel.validate(modelValue, viewValue, false);
            };
        }
    }); });
    NakedObjects.app.directive("geminiFieldmandatorycheck", function () { return ({
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {
            ctrl.$validators.geminiFieldmandatorycheck = function (modelValue, viewValue) {
                var parent = scope.$parent;
                var viewModel = parent.parameter || parent.property;
                var val;
                if (viewValue instanceof NakedObjects.ChoiceViewModel) {
                    val = viewValue.getValue().toValueString();
                }
                else if (viewValue instanceof Array) {
                    if (viewValue.length) {
                        return _.every(viewValue, function (v) { return ctrl.$validators.geminiFieldmandatorycheck(v, v); });
                    }
                    val = "";
                }
                else {
                    val = viewValue;
                }
                return viewModel.validate(modelValue, val, true);
            };
        }
    }); });
    NakedObjects.app.directive("geminiBoolean", function () { return ({
        require: "?ngModel",
        link: function (scope, el, attrs, ctrl) {
            var parent = scope.$parent;
            var viewModel = parent.parameter || parent.property;
            ctrl.$formatters = [];
            ctrl.$parsers = [];
            ctrl.$validators.geminiBoolean = function (modelValue, viewValue) {
                return viewModel.validate(modelValue, viewValue, true);
            };
            ctrl.$render = function () {
                var d = ctrl.$viewValue;
                el.data("checked", d);
                switch (d) {
                    case true:
                        el.prop("indeterminate", false);
                        el.prop("checked", true);
                        break;
                    case false:
                        el.prop("indeterminate", false);
                        el.prop("checked", false);
                        break;
                    default:
                        el.prop("indeterminate", true);
                }
            };
            var triStateClick = function () {
                var d;
                var checkedBool = el.data("checked");
                switch (checkedBool) {
                    case false:
                        d = true;
                        break;
                    case true:
                        d = null;
                        break;
                    default:
                        d = false;
                }
                ctrl.$setViewValue(d);
                scope.$apply(ctrl.$render);
            };
            var twoStateClick = function () {
                var d;
                var checkedBool = el.data("checked");
                switch (checkedBool) {
                    case true:
                        d = false;
                        break;
                    default:
                        d = true;
                }
                ctrl.$setViewValue(d);
                scope.$apply(ctrl.$render);
            };
            el.bind("click", viewModel.optional ? triStateClick : twoStateClick);
        }
    }); });
    NakedObjects.app.directive("geminiClear", function ($timeout) {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: function (scope, elm, attrs, ngModel) {
                if (!ngModel) {
                    return;
                }
                // wrap in timeout or we won't see initial value 
                $timeout(function () {
                    $(elm).addClass("ng-clearable");
                    if (elm.val()) {
                        $(elm).addClass("ng-x");
                    }
                    else {
                        $(elm).removeClass("ng-x");
                    }
                });
                elm.on("input change", function () {
                    $(this).addClass("ng-clearable");
                    if (this.value) {
                        $(this).addClass("ng-x");
                    }
                    else {
                        $(this).removeClass("ng-x");
                    }
                }).on("mousemove", function (e) {
                    if (elm.hasClass("ng-x")) {
                        var onX = this.offsetWidth - 18 < e.clientX - this.getBoundingClientRect().left;
                        if (onX) {
                            $(this).addClass("ng-onX");
                        }
                        else {
                            $(this).removeClass("ng-onX");
                        }
                    }
                }).on("touchstart click", function (ev) {
                    var _this = this;
                    if ($(this).hasClass("ng-onX")) {
                        ev.preventDefault();
                        $(this).removeClass("ng-x ng-onX");
                        scope.$apply(function () {
                            var parent = scope.$parent;
                            var viewModel = parent.parameter || parent.property;
                            viewModel.clear();
                            ngModel.$setViewValue("");
                            $(_this).val("");
                            // ick but only way I can get color to clear on freeform droppable fields
                            $timeout(function () { return viewModel.color = ""; });
                        });
                    }
                });
            }
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.directives.js.map