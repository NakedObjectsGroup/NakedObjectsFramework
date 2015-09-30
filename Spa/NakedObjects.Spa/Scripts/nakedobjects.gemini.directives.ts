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


// tested 
module NakedObjects.Angular.Gemini {

    interface ISelectScope extends ng.IScope {
        select: any;
    }

    // based on code in AngularJs, Green and Seshadri, O'Reilly
    app.directive('nogDatepicker', function ($filter : ng.IFilterService) : ng.IDirective {
            return {
                // Enforce the angularJS default of restricting the directive to
                // attributes only
                restrict: 'A',
                // Always use along with an ng-model
                require: '?ngModel',
                // This method needs to be defined and passed in from the
                // passed in to the directive from the view controller
                scope: {
                    select: '&'        // Bind the select function we refer to the right scope
                },
                link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                    if (!ngModel) return;
                    const optionsObj: { dateFormat?: string; onSelect?: Function } = {};
                    optionsObj.dateFormat = 'd M yy'; // datepicker format
                    var updateModel = function (dateTxt) {
                        scope.$apply(function () {
                            // Call the internal AngularJS helper to
                            // update the two way binding

                            ngModel.$parsers.push((val) => { return new Date(val).toISOString(); });
                            ngModel.$setViewValue(dateTxt);
                        });
                    };

                    optionsObj.onSelect = function (dateTxt, picker) {
                        updateModel(dateTxt);
                        if (scope.select) {
                            scope.$apply(function () {
                                scope.select({ date: dateTxt });
                            });
                        }
                    };


                    ngModel.$render = function () {
                        const formattedDate = $filter('date')(ngModel.$viewValue, 'd MMM yyyy'); // angularjs format

                        // Use the AngularJS internal 'binding-specific' variable
                        element.datepicker('setDate', formattedDate);
                    };
                    element.datepicker(optionsObj);
                }
            };
        });

    app.directive('nogAutocomplete', function ($filter: ng.IFilterService, $parse, context: IContext): ng.IDirective {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: 'A',
            // Always use along with an ng-model
            require: '?ngModel',
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: '&'        // Bind the select function we refer to the right scope
            },
            link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                if (!ngModel) return;
                const optionsObj: { autoFocus?: boolean; minLength?: number; source?: Function; select?: Function; focus?: Function } = {};
                const parent = <any>scope.$parent;
                const viewModel = <ValueViewModel> (parent.parameter || parent.property);

                function render ( initialChoice? :ChoiceViewModel) {
                    var cvm = ngModel.$modelValue || initialChoice;

                    if (cvm) {
                        ngModel.$parsers.push((val) => { return cvm; });
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    }
                };

                ngModel.$render = render;
             
                var updateModel = function (cvm: ChoiceViewModel) {

                    //context.setSelectedChoice(cvm.id, cvm.search, cvm);

                    scope.$apply(function () {

                        ngModel.$parsers.push((val) => { return cvm; });
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    });
                };

                optionsObj.source = (request, response) => {

                    var prompts = scope.select({ request: request.term });

                    scope.$apply(function () {
                        prompts.then(function (cvms: ChoiceViewModel[]) {
                            response(_.map(cvms, (cvm) => {
                                return { "label": cvm.name, "value": cvm };
                            }));
                        }, function () {
                            response([]);
                        });
                    });
                };

                optionsObj.select = (event, ui) => {
                    updateModel(ui.item.value);
                    return false; 
                };

                optionsObj.focus = (event, ui) => {
                    return false;
                };

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

    app.directive('nogConditionalchoices', function ($filter: ng.IFilterService, $parse, context: IContext): ng.IDirective {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: 'A',
            // Always use along with an ng-model
            require: '?ngModel',
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: '&'        // Bind the select function we refer to the right scope
            },
            link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                if (!ngModel) return;

                var parent = <any>scope.$parent;
                var viewModel = <ValueViewModel> (parent.parameter || parent.property);
                var pArgs = viewModel.arguments;
                var currentOptions: ChoiceViewModel[] = [];

                function populateArguments() {
                    var nArgs = <IValueMap>{};

                    var dialog = <DialogViewModel> parent.dialog;
                    var object = <DomainObjectViewModel> parent.object;

                    if (dialog) {
                        _.forEach(<_.Dictionary<Value>>pArgs, (v, n) => {

                            var parm = _.find(dialog.parameters, (p: ParameterViewModel) => p.id === n);

                            var newValue = parm.getValue();
                            nArgs[n] = newValue;
                        });
                    }

                    // todo had to add object.properties check to get this working again - find out why
                    if (object && object.properties) {
                        _.forEach(<_.Dictionary<Value>>pArgs, (v, n) => {

                            var property = _.find(object.properties, (p: PropertyViewModel) => p.argId === n);

                            var newValue = property.getValue();
                            nArgs[n] = newValue;
                        });
                    }

                    return nArgs;
                }

                function populateDropdown() {
                    const nArgs = populateArguments();
                    const prompts = scope.select({ args: nArgs });
                    prompts.then(function (cvms: ChoiceViewModel[]) {
                        // if unchanged return 
                        if (cvms.length === currentOptions.length && _.all(cvms, (c : ChoiceViewModel, i) => { return c.equals(currentOptions[i]); })) {
                            return;
                        }

                        element.find("option").remove();
                            const emptyOpt = "<option></option>";
                            element.append(emptyOpt);

                        _.forEach(cvms, (cvm) => {
                           
                            var opt = $("<option></option>");
                            opt.val(cvm.value);
                            opt.text(cvm.name);

                            element.append(opt);
                        });

                        currentOptions = cvms;

                        if (viewModel.isMultipleChoices && viewModel.multiChoices) {
                            const vals = _.map(viewModel.multiChoices, (c: ChoiceViewModel) => {
                                return c.value;
                            });
                            $(element).val(vals);
                        } else if (viewModel.choice) {
                            $(element).val(viewModel.choice.value);
                        } 
                    },
                    function () {
                          // error clear everything 
                          element.find("option").remove();
                          viewModel.choice = null;
                          currentOptions = []; 
                    });
                }

                function optionChanged() {

                    if (viewModel.isMultipleChoices) {
                        const options = $(element).find("option:selected");
                        var kvps = [];

                        options.each((n, e) => {
                            kvps.push({ key: $(e).text(), value: $(e).val() });
                        });
                        const cvms = _.map(kvps, (o) =>  ChoiceViewModel.create(new Value(o.value), viewModel.id, o.key));
                        viewModel.multiChoices = cvms;

                    } else {
                        const option = $(element).find("option:selected");
                        const val = option.val();
                        const key = option.text();
                        var cvm = ChoiceViewModel.create(new Value(val), viewModel.id, key);
                        viewModel.choice = cvm;
                        scope.$apply(function() {
                            ngModel.$parsers.push((val) => { return cvm; });
                            ngModel.$setViewValue(cvm.name);
                        });
                    }
                }


                function setListeners() {
                    _.forEach(<_.Dictionary<Value>>pArgs, (v, n) => {
                        $("#" + n + " :input").on("change", (e: JQueryEventObject) => populateDropdown() );
                    });
                    $(element).on("change", optionChanged); 
                }

                ngModel.$render = function () {
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

    app.directive('nogRightclick', function ($parse) {
        return function (scope, element, attrs) {
            var fn = $parse(attrs.nogRightclick);
            element.bind('contextmenu', function (event) {
                scope.$apply(function () {
                    event.preventDefault();
                    fn(scope, { $event: event });
                });
            });
        };
    });

    app.directive("nogDrag", $parse => (scope, element, attrs) => {

        element.draggable({helper : "clone", zIndex : 9999});

        element.on("dragstart", function (event, ui) {
            const sc = scope;
            const sourceVm = sc.property;

            ui.helper.vm = sourceVm;
        });

    });

    app.directive("nogDrop", $parse => (scope, element, attrs) => {

       
        element.droppable({
            tolerance: "touch"
        });

        element.on("drop", (event, ui) => {
            const sc = scope;
            const p = sc.$parent.$parent.$parent.$parent.property;
            const vm = <PropertyViewModel>  ui.helper.vm;

            element[0].value = vm.value;
        });
    });

    app.directive('nogAttachment', function ($window : ng.IWindowService): ng.IDirective {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: 'A',
            // Always use along with an ng-model
            require: '?ngModel',
            link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                if (!ngModel) {
                    return;
                }

                function downloadFile(url : string, mt : string, success : (resp : Blob) => void ) {
                    var xhr = new XMLHttpRequest();
                    xhr.open('GET', url, true);
                    xhr.responseType = "blob";
                    xhr.setRequestHeader("Accept", mt); 
                    xhr.onreadystatechange = function () {
                        if (xhr.readyState === 4) {
                            success(<Blob>xhr.response);
                        }
                    };
                    xhr.send(null);
                }

                function displayInline(mt: string) {

                    if (mt === "image/jpeg" ||
                        mt === "image/gif" ||
                        mt === "application/octet-stream") {
                        return true;
                    }

                    return false;
                }

                var clickHandler = function () {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;
                    const url = attachment.href;
                    const mt = attachment.mimeType;
                    downloadFile(url, mt, (resp : Blob) => {
                        var burl = URL.createObjectURL(resp); 
                        $window.location.href = burl;                    
                    });
                    return false; 
                };
                ngModel.$render = function () {
                    const attachment: AttachmentViewModel = ngModel.$modelValue;
                    const url = attachment.href;
                    const mt = attachment.mimeType;
                    var title = attachment.title;
                    const link = "<a href='" + url + "'><span></span></a>";
                    element.append(link);

                    var anchor = element.find("a");
                    if (displayInline(mt)) {

                        downloadFile(url, mt, (resp: Blob) => {
                            var reader = new FileReader();
                            reader.onloadend = function () {
                                anchor.html("<img src='" + reader.result + "' alt='" + title + "' />");
                            };
                            reader.readAsDataURL(resp);
                        });
                    }
                    else {
                        anchor.html(title); 
                    }
                  
                    anchor.on('click', clickHandler);                 
                };
            }
        };
    });
}