/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="nakedobjects.app.ts" />
/// <reference path="typings/moment/moment.d.ts"/>

module NakedObjects {
    import Value = Models.Value;
    import toDateString = Models.toDateString;
    import EntryType = Models.EntryType;

    interface ISelectObj {
        request?: string;
        args?: _.Dictionary<Value>;
        date?: string;
    }

    interface ISelectScope extends ng.IScope {
        select: (obj: ISelectObj) => ng.IPromise<ChoiceViewModel[]>;
    }

    interface IPropertyOrParameterScope extends INakedObjectsScope {
        property?: ValueViewModel;
        parameter?: ValueViewModel;
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
            link(scope: ISelectScope, element : any, attrs : any, ngModel: ng.INgModelController) {

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

                // also for dynamic ids - need to wrap link in timeout. 
                $timeout(() => {

                    const updateModel = (dateTxt : any) => {
                        scope.$apply(() => {
                            // Call the internal AngularJS helper to
                            // update the two way binding

                            ngModel.$setViewValue(dateTxt);
                        });
                    };

                    const onSelect = (dateTxt : any) => updateModel(dateTxt);

                    const optionsObj = {
                        dateFormat: "d M yy", // datepicker format
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

    app.directive("geminiAutocomplete", (): ng.IDirective => {
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

            link: (scope: ISelectScope, element : any, attrs : any, ngModel: ng.INgModelController) => {
                if (!ngModel) return;

                const optionsObj: { autoFocus?: boolean; minLength?: number; source?: Function; select?: Function; focus?: Function } = {};
                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;

                function render(initialChoice?: ChoiceViewModel) {
                    const cvm = ngModel.$modelValue as ChoiceViewModel || initialChoice;

                    if (cvm) {
                        ngModel.$parsers.push(() => cvm);
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    }
                };

                ngModel.$render = render;

                const updateModel = (cvm: ChoiceViewModel) => {

                    scope.$apply(() => {
                        ngModel.$parsers.push(() => cvm);
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    });
                };

                optionsObj.source = (request : any, response : any) => {
                    scope.$apply(() =>
                        scope.select({ request: request.term }).
                        then((cvms: ChoiceViewModel[]) => response(_.map(cvms, cvm => ({ "label": cvm.name, "value": cvm })))).
                        catch(() => response([])));
                };

                optionsObj.select = (event : any, ui : any) => {
                    updateModel(ui.item.value);
                    return false;
                };

                optionsObj.focus = () => false;
                optionsObj.autoFocus = true;
                optionsObj.minLength = viewModel.minLength;

                const clearHandler = function() {
                    const value = $(this).val();
                    if (value.length === 0) {
                        updateModel(ChoiceViewModel.create(new Value(""), ""));
                    }
                };

                element.keyup(clearHandler);
                element.autocomplete(optionsObj);
                render(viewModel.choice);
            }
        };
    });

    app.directive("geminiConditionalchoices", (): ng.IDirective => {
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
            link: (scope: ISelectScope, element : any, attrs : any, ngModel: ng.INgModelController) => {
                if (!ngModel) return;

                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;
                const pArgs = _.omit(viewModel.arguments, "x-ro-nof-members") as _.Dictionary<Value>;
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
                            opt.val(cvm.value);
                            opt.text(cvm.name);

                            element.append(opt);
                        });

                        currentOptions = cvms;

                        if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
                            const vals = _.map(viewModel.multiChoices, c => c.value);
                            $(element).val(vals);
                        } else if (viewModel.choice) {
                            $(element).val(viewModel.choice.value);
                        }
                        else  {
                            $(element).val("");
                        }

                        setTimeout(() => {
                            $(element).change();
                        }, 1);


                    }).catch(() => {
                        // error clear everything 
                        element.find("option").remove();
                        viewModel.choice = null;
                        currentOptions = [];
                    });
                }

                function optionChanged() {

                    if (viewModel.entryType === EntryType.MultipleConditionalChoices) {
                        const options = $(element).find("option:selected");
                        const kvps = [] as any[];

                        options.each((n, e) => kvps.push({ key: $(e).text(), value: $(e).val() }));
                        const cvms = _.map(kvps, o => ChoiceViewModel.create(new Value(o.value), viewModel.id, o.key));
                        viewModel.multiChoices = cvms;

                    } else {
                        const option = $(element).find("option:selected");
                        const val = option.val();
                        const key = option.text();
                        const cvm = ChoiceViewModel.create(new Value(val), viewModel.id, key);
                        viewModel.choice = cvm;
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

                ngModel.$render = () => {}; // do on the next event loop,

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

    //The 'right-click' functionality is also triggered by shift-enter
    app.directive("geminiRightclick", $parse => (scope : any, element : any, attrs : any) => {
        const fn = $parse(attrs.geminiRightclick);
        element.bind("contextmenu", (event : any) => {
            scope.$apply(() => {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
        element.bind("keydown keypress", (event : any) => {
            const enterKeyCode = 13;
            if (event.keyCode === enterKeyCode && event.shiftKey) {
                scope.$apply(() => scope.$eval(attrs.geminiRightclick));
                event.preventDefault();
            }
        });
    });

    const draggableVmKey = "dvmk";

    app.directive("geminiDrag", ($compile) => (scope : any, element : any) => {

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

        element.on("dragstart", (event : any, ui : any) => {
            const draggableVm = scope.property || scope.item || scope.$parent.object;

            // make sure dragged element is correct color (object will not be set yet)
            ui.helper.addClass(draggableVm.color);

            // add vm to helper and original elements as accept and drop use different ones 
            ui.helper.data(draggableVmKey, draggableVm);
            element.data(draggableVmKey, draggableVm);
        });

        element.on("keydown keypress", (event : any) => {
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

    app.directive("geminiEnter", () => (scope : any, element : any, attrs : any) => {
        element.bind("keydown keypress", (event : any) => {
            const enterKeyCode = 13;
            if (event.which === enterKeyCode && !event.shiftKey) {
                scope.$apply(() => scope.$eval(attrs.geminiEnter));
                event.preventDefault();
            }
        });
    });

    app.directive("geminiPlaceholder", $parse => (scope: any, element: any, attrs: any) => {
        const fn = $parse(attrs.geminiPlaceholder);
        element.attr("placeholder", fn(scope));
    });

    app.directive("geminiFocuson", ($timeout: ng.ITimeoutService, focusManager: IFocusManager) => (scope : any, elem : any, attr: any) => {
        scope.$on(geminiFocusEvent, (e : any, target: FocusTarget, index: number, paneId: number, count: number) => {

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

    app.directive("geminiDrop", () => (scope : any, element: any) => {

        const propertyScope = () => scope.$parent.$parent.$parent;
        const parameterScope = () => scope.$parent.$parent;

        const accept = (draggable : any) => {
            const droppableVm: ValueViewModel = propertyScope().property || parameterScope().parameter;
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
        element.droppable({
            tolerance: "touch",
            hoverClass: "dropping",
            accept: accept
        });

        element.on("drop", (event : any, ui : any) => {

            if (element.hasClass("candrop")) {

                const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                const droppableVm: ValueViewModel = droppableScope.property || droppableScope.parameter;
                const draggableVm = <IDraggableViewModel> ui.draggable.data(draggableVmKey);

                droppableScope.$apply(() => droppableVm.drop(draggableVm));
            }
        });

        element.on("keydown keypress", (event: any) => {
            const vKeyCode = 86;
            const deleteKeyCode = 46;
            if (event.keyCode === vKeyCode && event.ctrlKey) {
                event.preventDefault();

                const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                const droppableVm: ValueViewModel = droppableScope.property || droppableScope.parameter;
                const draggableVm = <IDraggableViewModel>($("div.footer div.currentcopy .reference").data(draggableVmKey) as any);

                if (draggableVm) {
                    droppableScope.$apply(() => droppableVm.drop(draggableVm));
                }
            }
            if (event.keyCode === deleteKeyCode) {
                const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                const droppableVm: ValueViewModel = droppableScope.property || droppableScope.parameter;

                scope.$apply(droppableVm.clear());
            }
        });
    });

    app.directive("geminiAttachment", ($window: ng.IWindowService, $http : ng.IHttpService): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: (scope: ISelectScope, element : any, attrs : any, ngModel: ng.INgModelController) => {
                if (!ngModel) {
                    return;
                }

                function downloadFile(avm: AttachmentViewModel, success: (resp: Blob) => void) {
                    avm.downloadFile().then(blob => success(blob));
                }

                function displayInline(mt: string) {
                    return mt === "image/jpeg" ||
                        mt === "image/gif" ||
                        mt === "application/octet-stream";
                }

                const clickHandler = () => {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;
                    const url = attachment.href;
                    const mt = attachment.mimeType;
                    downloadFile(attachment, resp => {
                        if (window.navigator.msSaveBlob) {
                            // internet explorer 
                            window.navigator.msSaveBlob(resp, attachment.title);
                        } else {
                            const burl = URL.createObjectURL(resp);
                            $window.location.href = burl;
                        }
                    });
                    return false;
                };

                ngModel.$render = () => {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;

                    if (attachment) {
                        const url = attachment.href;
                        const mt = attachment.mimeType;
                        const title = attachment.title;
                        const link = `<a href='${url}'><span></span></a>`;
                        element.append(link);

                        const anchor = element.find("a");
                        if (displayInline(mt)) {

                            downloadFile(attachment, resp => {
                                const reader = new FileReader();
                                reader.onloadend = () => anchor.html(`<img src='${reader.result}' alt='${title}' />`);
                                reader.readAsDataURL(resp);
                            });
                        } else {
                            anchor.html(title);
                        }

                        anchor.on("click", clickHandler);
                    } else {
                        element.append("<div>Attachment not yet supported on transient</div>");
                    }
                };
            }
        };
    });

    app.directive("ciceroDown", () => (scope : any, element : any, attrs : any) => {
        element.bind("keydown keypress", (event: any) => {
            const enterKeyCode = 40;
            if (event.which === enterKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroDown));
                event.preventDefault();
            }
        });
    });

    app.directive("ciceroUp", () => (scope : any, element : any, attrs : any) => {
        element.bind("keydown keypress", (event: any) => {
            const enterKeyCode = 38;
            if (event.which === enterKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroUp));
                event.preventDefault();
            }
        });
    });

    app.directive("ciceroSpace", () => (scope : any, element: any, attrs: any) => {
        element.bind("keydown keypress", (event : any) => {
            const tabKeyCode = 32;
            if (event.which === tabKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroSpace));
                event.preventDefault();
            }
        });
    });

    app.directive("geminiFieldvalidate", () => ({
        require: "ngModel",
        link(scope : any, elm : any, attrs : any, ctrl : any) {
            ctrl.$validators.geminiFieldvalidate = (modelValue: any, viewValue: string) => {
                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;
                return viewModel.validate(modelValue, viewValue, false);
            };
        }
    }));

    app.directive("geminiFieldmandatorycheck", () => ({
        require: "ngModel",
        link(scope: any, elm: any, attrs: any, ctrl: any) {
            ctrl.$validators.geminiFieldmandatorycheck = (modelValue: any, viewValue: string | ChoiceViewModel | string[] | ChoiceViewModel[]) => {
                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;
                let val: string;

                if (viewValue instanceof ChoiceViewModel) {
                    val = viewValue.value;
                }
                else if (viewValue instanceof Array) {
                    if ((viewValue as any).length) {
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
        link(scope: any, el: any, attrs: any, ctrl: any) {

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
                switch (el.data("checked")) {
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
                switch (el.data("checked")) {
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


}