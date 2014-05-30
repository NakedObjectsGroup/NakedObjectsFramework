/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.context.ts" />
var Spiro;
(function (Spiro) {
    // tested
    (function (Angular) {
        // based on code in AngularJs, Green and Seshadri, O'Reilly
        Spiro.Angular.app.directive('nogDatepicker', function ($filter) {
            return {
                // Enforce the angularJS default of restricting the directive to
                // attributes only
                restrict: 'A',
                // Always use along with an ng-model
                require: '?ngModel',
                // This method needs to be defined and passed in from the
                // passed in to the directive from the view controller
                scope: {
                    select: '&'
                },
                link: function (scope, element, attrs, ngModel) {
                    if (!ngModel)
                        return;

                    var optionsObj = {};

                    optionsObj.dateFormat = 'd M yy'; // datepicker format
                    var updateModel = function (dateTxt) {
                        scope.$apply(function () {
                            // Call the internal AngularJS helper to
                            // update the two way binding
                            ngModel.$parsers.push(function (val) {
                                return new Date(val).toISOString();
                            });
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
                        var formattedDate = $filter('date')(ngModel.$viewValue, 'd MMM yyyy');

                        // Use the AngularJS internal 'binding-specific' variable
                        element.datepicker('setDate', formattedDate);
                    };
                    element.datepicker(optionsObj);
                }
            };
        });

        Spiro.Angular.app.directive('nogAutocomplete', function ($filter, $parse, context) {
            return {
                // Enforce the angularJS default of restricting the directive to
                // attributes only
                restrict: 'A',
                // Always use along with an ng-model
                require: '?ngModel',
                // This method needs to be defined and passed in from the
                // passed in to the directive from the view controller
                scope: {
                    select: '&'
                },
                link: function (scope, element, attrs, ngModel) {
                    if (!ngModel)
                        return;

                    var optionsObj = {};
                    var parent = scope.$parent;
                    var viewModel = (parent.parameter || parent.property);

                    function render(initialChoice) {
                        var cvm = ngModel.$modelValue || initialChoice;

                        if (cvm) {
                            ngModel.$parsers.push(function (val) {
                                return cvm;
                            });
                            ngModel.$setViewValue(cvm.name);
                            element.val(cvm.name);
                        }
                    }
                    ;

                    ngModel.$render = render;

                    var updateModel = function (cvm) {
                        context.setSelectedChoice(cvm.id, cvm.search, cvm);

                        scope.$apply(function () {
                            ngModel.$parsers.push(function (val) {
                                return cvm;
                            });
                            ngModel.$setViewValue(cvm.name);
                            element.val(cvm.name);
                        });
                    };

                    optionsObj.source = function (request, response) {
                        var prompts = scope.select({ request: request.term });

                        scope.$apply(function () {
                            prompts.then(function (cvms) {
                                response(_.map(cvms, function (cvm) {
                                    return { "label": cvm.name, "value": cvm };
                                }));
                            }, function () {
                                response([]);
                            });
                        });
                    };

                    optionsObj.select = function (event, ui) {
                        updateModel(ui.item.value);
                        return false;
                    };

                    optionsObj.focus = function (event, ui) {
                        return false;
                    };

                    optionsObj.autoFocus = true;
                    optionsObj.minLength = 1;

                    var clearHandler = function () {
                        var value = $(this).val();

                        if (value.length == 0) {
                            updateModel(Spiro.Angular.ChoiceViewModel.create(new Spiro.Value(""), ""));
                        }
                    };

                    element.keyup(clearHandler);
                    element.autocomplete(optionsObj);
                    render(viewModel.choice);
                }
            };
        });

        Spiro.Angular.app.directive('nogConditionalchoices', function ($filter, $parse, context) {
            return {
                // Enforce the angularJS default of restricting the directive to
                // attributes only
                restrict: 'A',
                // Always use along with an ng-model
                require: '?ngModel',
                // This method needs to be defined and passed in from the
                // passed in to the directive from the view controller
                scope: {
                    select: '&'
                },
                link: function (scope, element, attrs, ngModel) {
                    if (!ngModel)
                        return;

                    var parent = scope.$parent;
                    var viewModel = (parent.parameter || parent.property);
                    var pArgs = viewModel.arguments;
                    var currentOptions = [];

                    function populateArguments() {
                        var nArgs = {};

                        var dialog = parent.dialog;
                        var object = parent.object;

                        if (dialog) {
                            _.forEach(pArgs, function (v, n) {
                                var parm = _.find(dialog.parameters, function (p) {
                                    return p.id == n;
                                });

                                var newValue = parm.getValue();
                                nArgs[n] = newValue;
                            });
                        }

                        if (object) {
                            _.forEach(pArgs, function (v, n) {
                                var property = _.find(object.properties, function (p) {
                                    return p.argId == n;
                                });

                                var newValue = property.getValue();
                                nArgs[n] = newValue;
                            });
                        }

                        return nArgs;
                    }

                    function populateDropdown() {
                        var nArgs = populateArguments();

                        var prompts = scope.select({ args: nArgs });

                        prompts.then(function (cvms) {
                            // if unchanged return
                            if (cvms.length === currentOptions.length && _.all(cvms, function (c, i) {
                                return c.equals(currentOptions[i]);
                            })) {
                                return;
                            }

                            element.find("option").remove();
                            var emptyOpt = "<option></option>";
                            element.append(emptyOpt);

                            _.forEach(cvms, function (cvm) {
                                var opt = $("<option></option>");
                                opt.val(cvm.value);
                                opt.text(cvm.name);

                                element.append(opt);
                            });

                            currentOptions = cvms;

                            if (viewModel.choice) {
                                if (_.any(cvms, function (cvm) {
                                    return cvm.name == viewModel.choice.name;
                                })) {
                                    $(element).val(viewModel.choice.value);
                                } else {
                                    viewModel.choice = null;
                                }
                            }
                        }, function () {
                            // error clear everything
                            element.find("option").remove();
                            viewModel.choice = null;
                            currentOptions = [];
                        });
                    }

                    function optionChanged() {
                        var option = $(element).find("option:selected");

                        var val = option.val();
                        var key = option.text();
                        var cvm = Spiro.Angular.ChoiceViewModel.create(new Spiro.Value(val), viewModel.id, key);
                        viewModel.choice = cvm;
                        scope.$apply(function () {
                            ngModel.$parsers.push(function (val) {
                                return cvm;
                            });
                            ngModel.$setViewValue(cvm.name);
                        });
                    }

                    function setListeners() {
                        _.forEach(pArgs, function (v, n) {
                            $("#" + n + " :input").on("change", function (e) {
                                return populateDropdown();
                            });
                        });
                        $(element).on("change", optionChanged);
                    }

                    ngModel.$render = function () {
                        // initial populate
                        //populateDropdown();
                    }; // do on the next event loop,

                    setTimeout(function () {
                        setListeners();

                        // initial populate
                        populateDropdown();
                    }, 1);
                }
            };
        });

        Spiro.Angular.app.directive('nogAttachment', function ($window) {
            return {
                // Enforce the angularJS default of restricting the directive to
                // attributes only
                restrict: 'A',
                // Always use along with an ng-model
                require: '?ngModel',
                link: function (scope, element, attrs, ngModel) {
                    if (!ngModel) {
                        return;
                    }

                    function downloadFile(url, mt, success) {
                        var xhr = new XMLHttpRequest();
                        xhr.open('GET', url, true);
                        xhr.responseType = "blob";
                        xhr.setRequestHeader("Accept", mt);
                        xhr.onreadystatechange = function () {
                            if (xhr.readyState == 4) {
                                success(xhr.response);
                            }
                        };
                        xhr.send(null);
                    }

                    function displayInline(mt) {
                        if (mt === "image/jpeg" || mt === "image/gif" || mt === "application/octet-stream") {
                            return true;
                        }

                        return false;
                    }

                    var clickHandler = function () {
                        var attachment = ngModel.$modelValue;

                        var url = attachment.href;
                        var mt = attachment.mimeType;

                        downloadFile(url, mt, function (resp) {
                            var burl = URL.createObjectURL(resp);
                            $window.location.href = burl;
                        });
                        return false;
                    };
                    ngModel.$render = function () {
                        var attachment = ngModel.$modelValue;

                        var url = attachment.href;
                        var mt = attachment.mimeType;
                        var title = attachment.title;

                        var link = "<a href='" + url + "'><span></span></a>";
                        element.append(link);

                        var anchor = element.find("a");
                        if (displayInline(mt)) {
                            downloadFile(url, mt, function (resp) {
                                var reader = new FileReader();
                                reader.onloadend = function () {
                                    anchor.html("<img src='" + reader.result + "' alt='" + title + "' />");
                                };
                                reader.readAsDataURL(resp);
                            });
                        } else {
                            anchor.html(title);
                        }

                        anchor.on('click', clickHandler);
                    };
                }
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.directives.js.map
