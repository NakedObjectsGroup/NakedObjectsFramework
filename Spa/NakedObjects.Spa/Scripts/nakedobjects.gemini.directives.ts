//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    interface ISelectScope extends ng.IScope {
        select: any;
    }

    // based on code in AngularJs, Green and Seshadri, O'Reilly
    app.directive("geminiDatepicker", ($filter : ng.IFilterService): ng.IDirective => {
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
            link(scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                if (!ngModel) return;

                const updateModel = dateTxt => {
                    scope.$apply(() => {
                        // Call the internal AngularJS helper to
                        // update the two way binding

                        ngModel.$parsers.push(val=> new Date(val).toISOString());
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
                const parent = <any>scope.$parent;
                const viewModel = <ValueViewModel> (parent.parameter || parent.property);

                function render(initialChoice?: ChoiceViewModel) {
                    const cvm = ngModel.$modelValue || initialChoice;

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
                        then((cvms: ChoiceViewModel[]) => response(_.map(cvms, cvm => { return { "label": cvm.name, "value": cvm }; }))).
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

                const parent = <any>scope.$parent;
                const viewModel = <ValueViewModel> (parent.parameter || parent.property);
                const pArgs = viewModel.arguments;
                let currentOptions: ChoiceViewModel[] = [];

                function populateArguments() {
                    const nArgs = <IValueMap>{};

                    const dialog = <DialogViewModel> parent.dialog;
                    const object = <DomainObjectViewModel> parent.object;

                    if (dialog) {
                        _.forEach(pArgs, (v, n) => {
                            const parm = _.find(dialog.parameters, p => p.id === n);
                            const newValue = parm.getValue();
                            nArgs[n] = newValue;
                        });
                    }

                    // todo had to add object.properties check to get this working again - find out why
                    if (object && object.properties) {
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
                    _.forEach(pArgs, (v, n) => $(`#${n} :input`).on("change", () => populateDropdown()));
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

    app.directive("geminiRightclick", $parse => (scope, element, attrs) => {
        const fn = $parse(attrs.geminiRightclick);
        element.bind("contextmenu", event => {
            scope.$apply(() => {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
    });

    const draggableVmKey = "dvmk";

    app.directive("geminiDrag", () => (scope, element) => {

        const cloneDraggable = () => {
            const cloned = $(element).clone();

            // make the dragged element look like a reference 
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
    });

    app.directive('geminiEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.geminiEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    });


    app.directive("geminiDrop", () => (scope, element) => {

        const propertyScope = () => scope.$parent.$parent.$parent.$parent;
        const parameterScope = () => scope.$parent.$parent.$parent;

        const accept = (draggable) => {
            const droppableVm: ValueViewModel = propertyScope().property || parameterScope().parameter;
            const draggableVm: IDraggableViewModel = draggable.data(draggableVmKey);
            return draggableVm.canDropOn(droppableVm.returnType);
        }

        element.droppable({
            tolerance: "touch",
            activeClass: "candrop",
            hoverClass: "dropping",
            accept: accept
        });

        element.on("drop", (event, ui) => {

            const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
            const droppableVm: ValueViewModel = droppableScope.property || droppableScope.parameter;
            const draggableVm = <IDraggableViewModel>  ui.draggable.data(draggableVmKey);

            droppableScope.$apply(() => droppableVm.drop(draggableVm));
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
}