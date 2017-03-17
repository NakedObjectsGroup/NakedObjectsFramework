/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="typings/moment/moment.d.ts"/>

namespace NakedObjects {
    import Link = Models.Link;
    import Value = Models.Value;
    import toDateString = Models.toDateString;
    import EntryType = Models.EntryType;
    import ErrorWrapper = Models.ErrorWrapper;

    interface ISelectObj {
        request?: string;
        args?: _.Dictionary<Value>;
        date?: string;
    }

    interface ISelectScope extends ng.IScope {
        select: (obj: ISelectObj) => ng.IPromise<ChoiceViewModel[]>;
    }

    interface IPropertyOrParameterScope extends INakedObjectsScope {
        property?: IFieldViewModel;
        parameter?: IFieldViewModel;
    }

    app.directive("geminiDatepicker", (mask: IMask, $timeout: ng.ITimeoutService): ng.IDirective => {
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
            link(scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

                if (!ngModel) return;
                // only add datepicker if date field not supported 
                if (element.prop("type") === "date") return;

                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;

                // adding parser at the front that converts to a format angluar parsers understand
                ngModel.$parsers.reverse();
                ngModel.$parsers.push(val => {

                    const dt1 = moment(val, supportedDateFormats, "en-GB", true);

                    if (dt1.isValid()) {
                        const dt = dt1.toDate();
                        return toDateString(dt);
                    }

                    return "";
                });
                ngModel.$parsers.reverse();

                // add our formatter that converts from date to our format
                ngModel.$formatters = [];

                // use viewmodel filter if we've been given one 
                const localFilter = viewModel && viewModel.localFilter ? viewModel.localFilter : mask.defaultLocalFilter("date");
                ngModel.$formatters.push(val => localFilter.filter(val));

                // put on viewmodel for error message formatting
                if (viewModel && !viewModel.localFilter) {
                    viewModel.localFilter = localFilter;
                }

                // also for dynamic ids - need to wrap link in timeout. 
                $timeout(() => {

                    const updateModel = (dateTxt: any) => {
                        scope.$apply(() => {
                            // Call the internal AngularJS helper to
                            // update the two way binding

                            ngModel.$setViewValue(dateTxt);
                            element.change(); // do this to trigger gemini-clear directive  
                        });
                    };

                    const onSelect = (dateTxt: any) => updateModel(dateTxt);

                    const optionsObj = {
                        dateFormat: "d M yy", // datepicker format
                        onSelect: onSelect,
                        showOn: "button",
                        buttonImage: "images/calendar.png",
                        buttonImageOnly: true,
                        buttonText: "Select date"
                    };

                    (element as any).datepicker(optionsObj);

                });
            }
        };
    });

    app.directive("geminiTimepicker", (mask: IMask, $timeout: ng.ITimeoutService): ng.IDirective => {
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
            link(scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

                if (!ngModel) return;
                // only add datepicker if time field not supported 
                if (element.prop("type") === "time") return;

                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;

                // also for dynamic ids - need to wrap link in timeout. 
                $timeout(() => {

                    const updateModel = () => {
                        scope.$apply(() => {
                            // Call the internal AngularJS helper to
                            // update the two way binding

                            ngModel.$setViewValue(element.val());
                            element.change(); // do this to trigger gemini-clear directive 
                        });
                        return true;
                    };
             
                    const optionsObj = {
                        timeFormat: "H:i", // timepicker format
                        showOn : null as any        
                    };

                    (element as any).timepicker(optionsObj);
                    element.on("changeTime", updateModel);
                   
                    const button = $("<img class='ui-datepicker-trigger' src='images/calendar.png' alt='Select time' title='Select time'>");

                    button.on("click", () => (element as any).timepicker("show"));
            
                    element.after(button);
                });
            }
        };
    });

    app.directive("geminiAutocomplete", (color: IColor): ng.IDirective => {
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

            link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
                if (!ngModel) return;

                const optionsObj: { autoFocus?: boolean; minLength?: number; source?: Function; select?: Function; focus?: Function } = {};
                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;

                function render(initialChoice?: IChoiceViewModel) {
                    const cvm = ngModel.$modelValue as IChoiceViewModel || initialChoice;

                    if (cvm) {
                        ngModel.$parsers.push(() => cvm);
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    }
                };

                ngModel.$render = render;

                const updateModel = (cvm: IChoiceViewModel) => {

                    scope.$apply(() => {
                        viewModel.clear();                        
                        ngModel.$parsers.push(() => cvm);
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);      
                    });
                };

                optionsObj.source = (request: any, response: any) => {
                    scope.$apply(() =>
                        scope.select({ request: request.term }).
                            then((cvms: ChoiceViewModel[]) => response(_.map(cvms, cvm => ({ "label": cvm.name, "value": cvm })))).
                            catch(() => response([])));
                };

                optionsObj.select = (event: any, ui: any) => {
                    updateModel(ui.item.value);
                    return false;
                };

                optionsObj.focus = () => false;
                optionsObj.autoFocus = true;
                optionsObj.minLength = viewModel.minLength;

                const clearHandler = function () {
                    const value = $(this).val();
                    if (value.length === 0) {
                        updateModel(ChoiceViewModel.create(new Value(""), ""));
                    }
                };

                element.keyup(clearHandler);
                (element as any).autocomplete(optionsObj);
                render(viewModel.selectedChoice);

                (ngModel as any).$validators.geminiAutocomplete = (modelValue: any, viewValue: string) => {
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
                    else if (modelValue instanceof ChoiceViewModel) {
                        // has view model check if it's valid                       
                        if (!modelValue.name) {
                            viewModel.setMessage(pendingAutoComplete);
                            viewModel.clientValid = false;
                        }
                    }
                    else { 
                        // has value but not ChoiceViewModel so must be invalid 
                        viewModel.setMessage(pendingAutoComplete);
                        viewModel.clientValid = false;
                    }

                    return viewModel.clientValid;
                };
            }
        };
    });

    app.directive("geminiConditionalchoices", (): ng.IDirective => {
        return {
            // up the priority of this directive to that viewmodel is set before ng-options - 
            // then angular doesn't add an empty entry on dropdown
            priority : 10,
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
            link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
                if (!ngModel) return;

                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;
                const pArgs = _.omit(viewModel.promptArguments, "x-ro-nof-members") as _.Dictionary<Value>;
                const paneId = viewModel.onPaneId;
                let currentOptions: ChoiceViewModel[] = [];

                function isDomainObjectViewModel(object: any): object is DomainObjectViewModel {
                    return object && "properties" in object;
                }

                function mapValues(args: _.Dictionary<Value>, parmsOrProps: { argId: string, getValue: () => Value }[]) {
                    return _.mapValues(pArgs, (v, n) => {
                        const pop = _.find(parmsOrProps, p => p.argId === n);
                        return pop.getValue();
                    });
                }


                function populateArguments() {

                    const dialog = parent.dialog;
                    const object = parent.object;

                    if (!dialog && !object) {
                        throw { message: "Expect dialog or object in geminiConditionalchoices", stack: "" };
                    }

                    let parmsOrProps: { argId: string, getValue: () => Value }[] = [];

                    if (dialog) {
                        parmsOrProps = dialog.parameters;
                    }

                    if (isDomainObjectViewModel(object)) {
                        parmsOrProps = object.properties;
                    }

                    return mapValues(pArgs, parmsOrProps);
                }

                function populateDropdown() {
                    const nArgs = populateArguments();
                    const prompts = scope.select({ args: nArgs });
                    prompts.then((cvms: ChoiceViewModel[]) => {
                        // if unchanged return 
                        if (cvms.length === currentOptions.length && _.every(cvms, (c, i) => c.equals(currentOptions[i]))) {
                            return;
                        }

                        element.find("option").remove();

                        if (viewModel.optional) {
                            const emptyOpt = $("<option></option>");
                            element.append(emptyOpt);
                        }

                        _.forEach(cvms, cvm => {

                            const opt = $("<option></option>");
                            opt.val(cvm.getValue().toValueString());
                            opt.text(cvm.name);

                            element.append(opt);
                        });

                        currentOptions = cvms;

                        if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
                            const vals = _.map(viewModel.selectedMultiChoices, c => c.getValue().toValueString());
                            $(element).val(vals);
                        } else if (viewModel.selectedChoice) {
                            $(element).val(viewModel.selectedChoice.getValue().toValueString());
                        }
                        else {
                            $(element).val("");
                        }

                        setTimeout(() => {
                            $(element).change();
                        }, 1);


                    }).catch(() => {
                        // error clear everything 
                        element.find("option").remove();
                        viewModel.selectedChoice = null;
                        currentOptions = [];
                    });
                }

                function wrapReferences(val: string) : string | RoInterfaces.ILink {
                    if (val && viewModel.type === "ref") {
                        return { href: val };
                    }
                    return val;
                }

                function optionChanged() {

                    if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
                        const options = $(element).find("option:selected");
                        const kvps = [] as any[];

                        options.each((n, e) => kvps.push({ key: $(e).text(), value: $(e).val() }));
                        const cvms = _.map(kvps, o => ChoiceViewModel.create(new Value(wrapReferences(o.value)), viewModel.id, o.key));
                        viewModel.selectedMultiChoices = cvms;

                    } else {
                        const option = $(element).find("option:selected");
                        const val = option.val();
                        const key = option.text();
                        const cvm = ChoiceViewModel.create(new Value(wrapReferences(val)), viewModel.id, key);
                        viewModel.selectedChoice = cvm;
                        scope.$apply(() => {
                            ngModel.$parsers.push(() => cvm);
                            ngModel.$setViewValue(cvm.name);
                        });
                    }
                }


                function setListeners() {
                    _.forEach(pArgs, (v, n) => $(`#${n}${paneId}`).on("change", () => populateDropdown()));
                    $(element).on("change", optionChanged);
                }

                ngModel.$render = () => { }; // do on the next event loop,

                setTimeout(() => {
                    setListeners();
                    // initial populate

                    // do this initially so that there is a valid model 
                    // otherwise angular will insert another empty value giving two 
                    element.find("option").remove();
                    if (viewModel.optional) {
                        const emptyOpt = $("<option></option>");
                        element.append(emptyOpt);
                        $(element).val("");
                    }

                    populateDropdown();
                }, 1);
            }
        };
    });

    interface IGeminiRightclick extends ng.IAttributes {
        geminiRightclick: string;
    }

    //The 'right-click' functionality is also triggered by shift-enter
    app.directive("geminiRightclick", $parse => (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: IGeminiRightclick) => {


        const fn = $parse(attrs.geminiRightclick);
        element.bind("contextmenu", (event: JQueryEventObject) => {
            scope.$apply(() => {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
        element.bind("keydown keypress", (event: JQueryEventObject) => {
            const enterKeyCode = 13;
            if (event.keyCode === enterKeyCode && event.shiftKey) {
                scope.$apply(() => scope.$eval(attrs.geminiRightclick));
                event.preventDefault();
            }
        });
    });

    const draggableVmKey = "dvmk";

    app.directive("geminiDrag", ($compile) => (scope: any, element: any) => {

        const cloneDraggable = () => {
            let cloned: JQuery;

            // make the dragged element look like a reference 
            if ($(element)[0].nodeName.toLowerCase() === "tr") {
                const title = $(element).find("td.cell:first").text();
                cloned = $(`<div>${title}</div>`);
            } else {
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

        element.on("dragstart", (event: any, ui: any) => {
            const draggableVm = scope.property || scope.item || scope.$parent.object;

            // make sure dragged element is correct color (object will not be set yet)
            ui.helper.addClass(draggableVm.color);

            // add vm to helper and original elements as accept and drop use different ones 
            ui.helper.data(draggableVmKey, draggableVm);
            element.data(draggableVmKey, draggableVm);
        });

        element.on("keydown keypress", (event: any) => {
            const cKeyCode = 67;
            if (event.keyCode === cKeyCode && event.ctrlKey) {
                const draggableVm = scope.property || scope.item || scope.$parent.object;
                const compiledClone = $compile(`<div class='reference ${draggableVm.color}' gemini-drag=''>${element[0].textContent}</div>`)(scope);
                compiledClone.data(draggableVmKey, draggableVm);
                $("div.footer div.currentcopy").empty();
                $("div.footer div.currentcopy").append("<span>Copying...</span>").append(compiledClone);
                event.preventDefault();
            }
        });
    });

    interface IGeminiEnter extends ng.IAttributes {
        geminiEnter: string;
    }

    app.directive("geminiEnter", () => (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: IGeminiEnter) => {
        element.bind("keydown keypress", (event: JQueryEventObject) => {
            const enterKeyCode = 13;
            if (event.which === enterKeyCode && !event.shiftKey) {
                scope.$apply(() => scope.$eval(attrs.geminiEnter));
                event.preventDefault();
            }
        });
    });

    interface IGeminiPlaceholder extends ng.IAttributes {
        geminiPlaceholder: string;
    }

    app.directive("geminiPlaceholder", $parse => (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: IGeminiPlaceholder) => {
        const fn = $parse(attrs.geminiPlaceholder);
        element.attr("placeholder", fn(scope));
    });

    app.directive("geminiFocuson", ($timeout: ng.ITimeoutService, focusManager: IFocusManager) => (scope: ng.IScope, elem: ng.IAugmentedJQuery) => {
        scope.$on(geminiFocusEvent, (e: any, target: FocusTarget, index: number, paneId: number, count: number) => {

            $timeout(() => {

                let focusElements: JQuery;

                switch (target) {
                    case FocusTarget.Menu:
                        focusElements = $(elem).find(`#pane${paneId}.split div.home div.menu, div.single div.home div.menu`);
                        break;
                    case FocusTarget.SubAction:
                        focusElements = $(elem).find(`#pane${paneId}.split div.actions div.action, div.single div.actions div.action`);
                        break;
                    case FocusTarget.Action:
                        focusElements = $(elem).find(`#pane${paneId}.split div.action, div.single div.action`);
                        break;
                    case FocusTarget.ObjectTitle:
                        focusElements = $(elem).find(`#pane${paneId}.split div.object div.title, div.single div.object div.title`);
                        break;
                    case FocusTarget.Dialog:
                        focusElements = $(elem).find(`#pane${paneId}.split div.parameters div.parameter :input[type!='hidden'], div.single div.parameters div.parameter :input[type!='hidden']`);
                        break;
                    case FocusTarget.ListItem:
                        focusElements = $(elem).find(`#pane${paneId}.split div.collection td.reference, div.single div.collection td.reference`);
                        break;
                    case FocusTarget.Property:
                        focusElements = $(elem).find(`#pane${paneId}.split div.properties div.property :input[type!='hidden'], div.single div.properties div.property :input[type!='hidden']`);
                        break;
                    case FocusTarget.TableItem:
                        focusElements = $(elem).find(`#pane${paneId}.split div.collection tbody tr, div.single div.collection tbody tr`);
                        break;
                    case FocusTarget.Input:
                        focusElements = $(elem).find("input");
                        break;
                    case FocusTarget.CheckBox:
                        focusElements = $(elem).find(`#pane${paneId}.split div.collection td.checkbox input, div.single div.collection td.checkbox input`);
                        break;
                    case FocusTarget.MultiLineDialogRow:
                        focusElements = $(elem).find(`div.parameters :input.value:first-child`);
                        break;
                }

                if (focusElements) {
                    if (focusElements.length >= index) {
                        $(focusElements[index]).focus();
                    } else {
                        // haven't found anything to focus - try again - but not forever
                        if (count < 10) {
                            focusManager.focusOn(target, index, paneId, ++count);
                        }
                    }
                }
            }, 0, false);
        });
    });

    app.directive("geminiDrop", ($timeout: ng.ITimeoutService) => (scope: ng.IScope, element: ng.IAugmentedJQuery) => {

        const propertyScope = () => scope.$parent.$parent.$parent as IPropertyOrParameterScope;
        const parameterScope = () => scope.$parent.$parent as IPropertyOrParameterScope;

        const accept = (draggable: any) => {
            const droppableVm: IFieldViewModel = propertyScope().property || parameterScope().parameter;
            const draggableVm: IDraggableViewModel = draggable.data(draggableVmKey);

            if (draggableVm) {
                draggableVm.canDropOn(droppableVm.returnType).
                    then((canDrop: boolean) => {
                        if (canDrop) {
                            element.addClass("candrop");
                        } else {
                            element.removeClass("candrop");
                        }
                    }).
                    catch(() => element.removeClass("candrop"));
                return true;
            }
            return false;
        };

        (element as any).droppable({
            tolerance: "touch",
            hoverClass: "dropping",
            accept: accept
        });

        element.on("drop", (event: any, ui: any) => {

            if (element.hasClass("candrop")) {

                const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                const droppableVm: IFieldViewModel = droppableScope.property || droppableScope.parameter;
                const draggableVm = <IDraggableViewModel>ui.draggable.data(draggableVmKey);

                droppableScope.$apply(() => {
                    droppableVm.drop(draggableVm);
                    $timeout(() => $(element).change());
                });
            }
        });

        element.on("keydown keypress", (event: any) => {
            const vKeyCode = 86;
            const deleteKeyCode = 46;
            if (event.keyCode === vKeyCode && event.ctrlKey) {
             

                const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                const droppableVm: IFieldViewModel = droppableScope.property || droppableScope.parameter;
                const draggableVm = <IDraggableViewModel>($("div.footer div.currentcopy .reference").data(draggableVmKey) as any);

                if (draggableVm) {
                    // only consume event if we are actually dropping on a field
                    event.preventDefault();
                    droppableScope.$apply(() => droppableVm.drop(draggableVm));
                }
            }
            if (event.keyCode === deleteKeyCode) {
                const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                const droppableVm: IFieldViewModel = droppableScope.property || droppableScope.parameter;

                scope.$apply(droppableVm.clear);
            }
        });
    });

    app.directive("geminiViewattachment", (error : IError): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
                if (!ngModel) {
                    return;
                }

                ngModel.$render = () => {
                    const attachment: IAttachmentViewModel = ngModel.$modelValue;
                    if (attachment) {
                        const title = attachment.title;
                        element.empty();
                        attachment.downloadFile().
                            then(blob => {
                                const reader = new FileReader();
                                reader.onloadend = () => element.html(`<img src='${reader.result}' alt='${title}' />`);
                                reader.readAsDataURL(blob);
                            }).
                            catch((reject: ErrorWrapper) => error.handleError(reject));
                    }
                };
            }
        };
    });

    // this is a very simple implementation unlikely to work with large files, no chunking etc   
    app.directive("geminiFileupload", () => {
        return {
            restrict: "A",
            scope: true,
            require: "?ngModel",
            link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {

                if (!ngModel) {
                    return;
                }

                element.bind("change", () => {
                    const file = (element[0] as any).files[0] as File;

                    const fileReader = new FileReader();
                    fileReader.onloadend = () => {
                        const link = new Link({
                            href: fileReader.result,
                            type: file.type,
                            title: file.name
                        } as RoInterfaces.ILink);

                        ngModel.$setViewValue(link);
                    };

                    fileReader.readAsDataURL(file);
                });
            }
        };
    });

    app.directive("geminiAttachment", ($compile : ng.ICompileService, $window: ng.IWindowService, error : IError): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
                if (!ngModel) {
                    return;
                }

                const clickHandler = () => {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;

                    if (!attachment.displayInline()) {
                        attachment.downloadFile().
                            then(blob => {
                                if (window.navigator.msSaveBlob) {
                                    // internet explorer 
                                    window.navigator.msSaveBlob(blob, attachment.title);
                                } else {
                                    const burl = URL.createObjectURL(blob);
                                    $window.location.href = burl;
                                }
                            }).
                            catch((reject: ErrorWrapper) => error.handleError(reject));
                    }

                    return false;
                };

                ngModel.$render = () => {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;

                    if (attachment) {

                        const title = attachment.title;

                        element.empty();

                     
                        if (attachment.displayInline()) {
                            attachment.downloadFile().
                                then(blob => {
                                    const reader = new FileReader();
                                    reader.onloadend = () => {
                                        if (reader.result) {
                                            element.html(`<img src='${reader.result}' alt='${title}' />`);
                                        }
                                    }
                                    reader.readAsDataURL(blob);
                                }).
                                catch((reject: ErrorWrapper) => error.handleError(reject));
                        } else {
                            element.html(`<div>${title}</div>`);
                            attachment.doClick = clickHandler;
                        }

                    } else {
                        element.append("<div>Attachment not yet supported on transient</div>");
                    }
                };
            }
        };
    });

    interface ICiceroDown extends ng.IAttributes {
        ciceroDown: string;
    }

    app.directive("ciceroDown", () => (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ICiceroDown) => {
        element.bind("keydown keypress", (event: JQueryEventObject) => {
            const enterKeyCode = 40;
            if (event.which === enterKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroDown));
                event.preventDefault();
            }
        });
    });

    interface ICiceroUp extends ng.IAttributes {
        ciceroUp: string;
    }

    app.directive("ciceroUp", () => (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ICiceroUp) => {
        element.bind("keydown keypress", (event: JQueryEventObject) => {
            const enterKeyCode = 38;
            if (event.which === enterKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroUp));
                event.preventDefault();
            }
        });
    });

    interface ICiceroSpace extends ng.IAttributes {
        ciceroSpace: string;
    }

    app.directive("ciceroSpace", () => (scope: ng.IScope, element: ng.IAugmentedJQuery, attrs: ICiceroSpace) => {
        element.bind("keydown keypress", (event: JQueryEventObject) => {
            const tabKeyCode = 32;
            if (event.which === tabKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroSpace));
                event.preventDefault();
            }
        });
    });

    app.directive("geminiFieldvalidate", () => ({
        require: "ngModel",
        link(scope: ng.IScope, elm: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) {
            ctrl.$validators.geminiFieldvalidate = (modelValue: any, viewValue: string) => {
                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;
                return viewModel.validate(modelValue, viewValue, false);
            };
        }
    }));

    app.directive("geminiFieldmandatorycheck", () => ({
        require: "ngModel",
        link(scope: ng.IScope, elm: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) {
            ctrl.$validators.geminiFieldmandatorycheck = (modelValue: any, viewValue: string | ChoiceViewModel | string[] | ChoiceViewModel[]) => {
                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;
                let val: string;

                if (viewValue instanceof ChoiceViewModel) {
                    val = viewValue.getValue().toValueString();
                }
                else if (viewValue instanceof Array) {
                    if (viewValue.length) {
                        return _.every(viewValue as (string | ChoiceViewModel)[], (v: any) => ctrl.$validators.geminiFieldmandatorycheck(v, v));
                    }
                    val = "";
                }
                else {
                    val = viewValue as string;
                }

                return viewModel.validate(modelValue, val, true);
            };
        }
    }));

    app.directive("geminiBoolean", () => ({
        require: "?ngModel",
        link(scope: ng.IScope, el: ng.IAugmentedJQuery, attrs: ng.IAttributes, ctrl: any) {

            const parent = scope.$parent as IPropertyOrParameterScope;
            const viewModel = parent.parameter || parent.property;

            ctrl.$formatters = [];
            ctrl.$parsers = [];

            ctrl.$validators.geminiBoolean = (modelValue: any, viewValue: string) => {
                return viewModel.validate(modelValue, viewValue, true);
            };

            ctrl.$render = () => {
                const d = ctrl.$viewValue as boolean;
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
                    default: // null
                        el.prop("indeterminate", true);
                }
            };

            const triStateClick = () => {
                let d: boolean;
                const checkedBool : boolean = el.data("checked") as any;
                switch (checkedBool) {
                    case false:
                        d = true;
                        break;
                    case true:
                        d = null;
                        break;
                    default: // null
                        d = false;
                }
                ctrl.$setViewValue(d);
                scope.$apply(ctrl.$render);
            };

            const twoStateClick = () => {
                let d: boolean;
                const checkedBool: boolean = el.data("checked") as any;
                switch (checkedBool) {
                    case true:
                        d = false;
                        break;
                    default: // false or null
                        d = true;
                }
                ctrl.$setViewValue(d);
                scope.$apply(ctrl.$render);
            };

            el.bind("click", viewModel.optional ? triStateClick : twoStateClick);
        }
    }));

    app.directive("geminiClear", ($timeout : ng.ITimeoutService): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: (scope: ISelectScope, elm: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
                if (!ngModel) {
                    return;
                }

                // wrap in timeout or we won't see initial value 
                $timeout(() => {
                    $(elm).addClass("ng-clearable");

                    if (elm.val()) {
                        $(elm).addClass("ng-x");
                    } else {
                        $(elm).removeClass("ng-x");
                    }
                });
     
                elm.on("input change", function () {
                    $(this).addClass("ng-clearable");
                    if (this.value) {
                        $(this).addClass("ng-x");
                    } else {
                        $(this).removeClass("ng-x");
                    }
                }).on("mousemove", function (e) {
                    if (elm.hasClass("ng-x")) {

                        const onX = this.offsetWidth - 18 < e.clientX - this.getBoundingClientRect().left;

                        if (onX) {
                            $(this).addClass("ng-onX");
                        } else {
                            $(this).removeClass("ng-onX");
                        }
                    }
                }).on("touchstart click", function (ev) {
                    if ($(this).hasClass("ng-onX")) {

                        ev.preventDefault();
                        $(this).removeClass("ng-x ng-onX");

                        scope.$apply(() => {
                            const parent = scope.$parent as IPropertyOrParameterScope;
                            const viewModel = parent.parameter || parent.property;
                            viewModel.clear();
                            
                            ngModel.$setViewValue("");
                            $(this).val("");

                            // ick but only way I can get color to clear on freeform droppable fields
                            $timeout(() => viewModel.color = "");                        
                        });
                    }
                });
            }
        };
    });

}