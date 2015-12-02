/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {
   

    interface ISelectObj {
        request?: string;
        args?: _.Dictionary<Value>;
        date? : string;
    }

    interface ISelectScope extends ng.IScope {
        select: (obj : ISelectObj) => ng.IPromise<ChoiceViewModel[]>;
    }

    interface IPropertyOrParameterScope extends INakedObjectsScope {
        property?: ValueViewModel;
        parameter?: ValueViewModel;
    }


    // based on code in AngularJs, Green and Seshadri, O'Reilly
    app.directive("geminiDatepicker", ($filter : ng.IFilterService, $timeout : ng.ITimeoutService): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",

            // to make sure dynamic ids on element get picked up
            transclude : true,
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: "&"        // Bind the select function we refer to the right scope
            },
            link(scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                // also for dynamic ids - need to wrap link in timeout. 
                $timeout(() => {
                    if (!ngModel) return;

                    const updateModel = dateTxt => {
                        scope.$apply(() => {
                            // Call the internal AngularJS helper to
                            // update the two way binding

                            ngModel.$parsers.push(val => new Date(val).toISOString());
                            ngModel.$setViewValue(dateTxt);
                        });
                    };

                    const onSelect = dateTxt => {
                        updateModel(dateTxt);
                        if (scope.select) {
                            scope.$apply(() => scope.select({ date: dateTxt }));
                        }
                    };

                    const optionsObj = {
                        dateFormat: "d M yy", // datepicker format
                        onSelect: onSelect
                    };

                    ngModel.$render = () => {
                        const formattedDate = $filter("date")(ngModel.$viewValue, "d MMM yyyy"); // angularjs format
                        // Use the AngularJS internal 'binding-specific' variable
                        element.datepicker("setDate", formattedDate);
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
                select: "&"        // Bind the select function we refer to the right scope
            },

            link: (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) => {
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

                    //context.setSelectedChoice(cvm.id, cvm.search, cvm);

                    scope.$apply(() => {
                        ngModel.$parsers.push(() => cvm);
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    });
                };

                optionsObj.source = (request, response) => {
                    scope.$apply(() =>
                        scope.select({ request: request.term }).
                        then((cvms: ChoiceViewModel[]) => response(_.map(cvms, cvm => ({ "label": cvm.name, "value": cvm })))).
                        catch(() => response([])));
                };

                optionsObj.select = (event, ui) => {
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
                select: "&"        // Bind the select function we refer to the right scope
            },
            link: (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) => {
                if (!ngModel) return;

                const parent = scope.$parent as IPropertyOrParameterScope;
                const viewModel = parent.parameter || parent.property;
                const pArgs = viewModel.arguments;
                let currentOptions: ChoiceViewModel[] = [];

                function isDomainObjectViewModel(object : any) : object is DomainObjectViewModel {
                    return object && "properties" in object;
                }

                function populateArguments() {
                    const nArgs = <_.Dictionary<Value>>{};

                    const dialog = parent.dialog;
                    const object = parent.object;

                    // todo replace with _.mapValue 
                    if (dialog) {
                        _.forEach(pArgs, (v, n) => {
                            const parm = _.find(dialog.parameters, p => p.id === n);
                            const newValue = parm.getValue();
                            nArgs[n] = newValue;
                        });
                    }

                    if (isDomainObjectViewModel(object)) {
                        _.forEach(pArgs, (v, n) => {
                            const property = _.find(object.properties, p => p.argId === n);
                            const newValue = property.getValue();
                            nArgs[n] = newValue;
                        });
                    }

                    return nArgs;
                }

                function populateDropdown() {
                    const nArgs = populateArguments();
                    const prompts = scope.select({ args: nArgs });
                    prompts.then((cvms: ChoiceViewModel[]) => {
                        // if unchanged return 
                        if (cvms.length === currentOptions.length && _.all(cvms, (c, i) =>  c.equals(currentOptions[i]))) {
                            return;
                        }

                        element.find("option").remove();
                        const emptyOpt = "<option></option>";
                        element.append(emptyOpt);

                        _.forEach(cvms, cvm => {
                           
                            const opt = $("<option></option>");
                            opt.val(cvm.value);
                            opt.text(cvm.name);

                            element.append(opt);
                        });

                        currentOptions = cvms;

                        if (viewModel.isMultipleChoices && viewModel.multiChoices) {
                            const vals = _.map(viewModel.multiChoices, c => c.value);
                            $(element).val(vals);
                        } else if (viewModel.choice) {
                            $(element).val(viewModel.choice.value);
                        } 
                    }).catch(() => {
                        // error clear everything 
                        element.find("option").remove();
                        viewModel.choice = null;
                        currentOptions = []; 
                    });
                }

                function optionChanged() {

                    if (viewModel.isMultipleChoices) {
                        const options = $(element).find("option:selected");
                        const kvps = [];

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
                    _.forEach(pArgs, (v, n) => $(`#${n}`).on("change", () => populateDropdown()));
                    $(element).on("change", optionChanged);
                }

                ngModel.$render = () => {
                    // initial populate
                    //populateDropdown();
                }; // do on the next event loop,

                setTimeout(() => {
                    setListeners();
                    // initial populate
                    populateDropdown();
                }, 1); 
            }
        };
    });

    //The 'right-click' functionality is also triggered by shift-enter
    app.directive("geminiRightclick", $parse => (scope, element, attrs) => {
        const fn = $parse(attrs.geminiRightclick);
        element.bind("contextmenu", event => {
            scope.$apply(() => {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
        element.bind("keydown keypress", event => {
            const enterKeyCode = 13;
            if (event.keyCode === enterKeyCode && event.shiftKey) {
                scope.$apply(() => scope.$eval(attrs.geminiRightclick));
                event.preventDefault();
            }
        });
    });

    const draggableVmKey = "dvmk";

    app.directive("geminiDrag", ($compile) => (scope, element) => {

        const cloneDraggable = () => {
            let cloned: JQuery;

             // make the dragged element look like a reference 
            if ($(element)[0].nodeName.toLowerCase() === "tr") {
                const title = $(element).find("td:first").text();
                cloned = $(`<div>${title}</div>`);
            } else {
                cloned = $(element).clone();
            }
   
            cloned.removeClass();
            cloned.addClass("reference");
            return cloned;
        }

        element.draggable({
            helper: cloneDraggable,
            zIndex: 9999
        });

        element.on("dragstart", (event, ui) => {
            const draggableVm = scope.property || scope.item || scope.$parent.object;

            // make sure dragged element is correct color (object will not be set yet)
            ui.helper.addClass(draggableVm.color);

            // add vm to helper and original elements as accept and drop use different ones 
            ui.helper.data(draggableVmKey, draggableVm);
            element.data(draggableVmKey, draggableVm);
        });

        element.on("keydown keypress", event => {
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

    app.directive("geminiEnter", () => (scope, element, attrs) => {
        element.bind("keydown keypress", event => {
            const enterKeyCode = 13;
            if (event.which === enterKeyCode) {
                scope.$apply(() => scope.$eval(attrs.geminiEnter));
                event.preventDefault();
            }
        });
    });

    app.directive("geminiFocuson", ($timeout: ng.ITimeoutService, focusManager : IFocusManager) => (scope, elem, attr) => {
        scope.$on(geminiFocusEvent, (e, target: FocusTarget, paneId : number, count : number) => {

            $timeout(() => {

                let focusElement: JQuery;

                switch (target) {
                    case FocusTarget.FirstMenu:
                        focusElement = $(elem).find(`#pane${paneId}.split div.home div.menu, div.single div.home div.menu`).first();
                        break;
                    case FocusTarget.FirstSubAction:
                        focusElement = $(elem).find(`#pane${paneId}.split div.actions div.action, div.single div.actions div.action`).first();
                        break;
                    case FocusTarget.FirstAction:
                        focusElement = $(elem).find(`#pane${paneId}.split div.action, div.single div.action`).first();
                        break;
                    case FocusTarget.ObjectTitle:
                        focusElement = $(elem).find(`#pane${paneId}.split div.object div.title, div.single div.object div.title`).first();
                        break;
                    case FocusTarget.Dialog:
                        focusElement = $(elem).find(`#pane${paneId}.split div.parameters div.parameter :input[type!='hidden'], div.single div.parameters div.parameter :input[type!='hidden']`).first();
                        break;
                    case FocusTarget.FirstListItem:
                        focusElement = $(elem).find(`#pane${paneId}.split div.collection td.reference, div.single div.collection td.reference`).first();
                        break;
                    case FocusTarget.FirstProperty:
                        focusElement = $(elem).find(`#pane${paneId}.split div.properties div.property :input[type!='hidden'], div.single div.properties div.property :input[type!='hidden']`).first();
                        break;
                    case FocusTarget.FirstTableItem:
                        focusElement = $(elem).find(`#pane${paneId}.split div.collection tbody tr, div.single div.collection tbody tr`).first();
                        break;
                    case FocusTarget.FirstInput:
                        focusElement = $(elem).find("input").first();
                        break;
                }

                if (focusElement) {
                    if (focusElement.length > 0) {
                        focusElement.focus();
                    } else {
                        // haven't found anything to focus - try again - but not forever
                        if (count < 10) {
                            focusManager.focusOn(target, paneId, ++count);
                        }
                    }
                }
            }, 0, false);
        });
    });

    app.directive("geminiDrop", () => (scope, element) => {

        const propertyScope = () => scope.$parent.$parent.$parent.$parent;
        const parameterScope = () => scope.$parent.$parent.$parent;

        const accept = (draggable) => {
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
        }

        element.droppable({
            tolerance: "touch",
            hoverClass: "dropping",
            accept: accept
        });

        element.on("drop", (event, ui) => {

            if (element.hasClass("candrop")) {

                const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
                const droppableVm: ValueViewModel = droppableScope.property || droppableScope.parameter;
                const draggableVm = <IDraggableViewModel> ui.draggable.data(draggableVmKey);

                droppableScope.$apply(() => droppableVm.drop(draggableVm));
            }
        });

        element.on("keydown keypress", event => {
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

    app.directive("geminiAttachment", ($window : ng.IWindowService): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",
            // Always use along with an ng-model
            require: "?ngModel",
            link: (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) => {
                if (!ngModel) {
                    return;
                }

                function downloadFile(url : string, mt : string, success : (resp : Blob) => void ) {
                    const xhr = new XMLHttpRequest();
                    xhr.open("GET", url, true);
                    xhr.responseType = "blob";
                    xhr.setRequestHeader("Accept", mt); 
                    xhr.onreadystatechange = () => {
                        if (xhr.readyState === 4) {
                            success(<Blob>xhr.response);
                        }
                    };
                    xhr.send(null);
                }

                function displayInline(mt: string) {
                    return mt === "image/jpeg" ||
                        mt === "image/gif"  ||
                        mt === "application/octet-stream";
                }

                const clickHandler = () => {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;
                    const url = attachment.href;
                    const mt = attachment.mimeType;
                    downloadFile(url, mt, resp => {
                        const burl = URL.createObjectURL(resp); 
                        $window.location.href = burl;                    
                    });
                    return false; 
                };

                ngModel.$render = () => {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;
                    const url = attachment.href;
                    const mt = attachment.mimeType;
                    const title = attachment.title;
                    const link = `<a href='${url}'><span></span></a>`;
                    element.append(link);

                    const anchor = element.find("a");
                    if (displayInline(mt)) {

                        downloadFile(url, mt, resp => {
                            const reader = new FileReader();
                            reader.onloadend = () => anchor.html(`<img src='${reader.result}' alt='${title}' />`);
                            reader.readAsDataURL(resp);
                        });
                    }
                    else {
                        anchor.html(title); 
                    }
                  
                    anchor.on("click", clickHandler);                 
                };
            }
        };
    });

    app.directive("ciceroDown", () => (scope, element, attrs) => {
        element.bind("keydown keypress", event => {
            const enterKeyCode = 40;
            if (event.which === enterKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroDown));
                event.preventDefault();
            }
        });
    });

    app.directive("ciceroUp", () => (scope, element, attrs) => {
        element.bind("keydown keypress", event => {
            const enterKeyCode = 38;
            if (event.which === enterKeyCode) {
                scope.$apply(() => scope.$eval(attrs.ciceroUp));
                event.preventDefault();
            }
        });
    });
}