import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiAutocomplete]' })
export class GeminiAutocompleteDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//    // Enforce the angularJS default of restricting the directive to
//    // attributes only
//    restrict: "A",
//    // Always use along with an ng-model
//    require: "?ngModel",
//    // This method needs to be defined and passed in from the
//    // passed in to the directive from the view controller
//    scope: {
//        select: "&" // Bind the select function we refer to the right scope
//    },

//    link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
//        if(!ngModel) return;

//    const optionsObj: { autoFocus?: boolean; minLength?: number; source?: Function; select?: Function; focus?: Function } = {};
//    const parent = scope.$parent as IPropertyOrParameterScope;
//    const viewModel = parent.parameter || parent.property;

//                function render(initialChoice?: IChoiceViewModel) {
//    const cvm = ngModel.$modelValue as IChoiceViewModel || initialChoice;

//    if (cvm) {
//        ngModel.$parsers.push(() => cvm);
//        ngModel.$setViewValue(cvm.name);
//        element.val(cvm.name);
//    }
//};

//ngModel.$render = render;

//const updateModel = (cvm: IChoiceViewModel) => {

//    scope.$apply(() => {
//        viewModel.clear();
//        ngModel.$parsers.push(() => cvm);
//        ngModel.$setViewValue(cvm.name);
//        element.val(cvm.name);
//    });
//};

//optionsObj.source = (request: any, response: any) => {
//    scope.$apply(() =>
//        scope.select({ request: request.term }).
//            then((cvms: ChoiceViewModel[]) => response(_.map(cvms, cvm => ({ "label": cvm.name, "value": cvm })))).
//            catch(() => response([])));
//};

//optionsObj.select = (event: any, ui: any) => {
//    updateModel(ui.item.value);
//    return false;
//};

//optionsObj.focus = () => false;
//optionsObj.autoFocus = true;
//optionsObj.minLength = viewModel.minLength;

//const clearHandler = function () {
//    const value = $(this).val();
//    if (value.length === 0) {
//        updateModel(ChoiceViewModel.create(new Value(""), ""));
//    }
//};

//element.keyup(clearHandler);
//(element as any).autocomplete(optionsObj);
//render(viewModel.selectedChoice);

//(ngModel as any).$validators.geminiAutocomplete = (modelValue: any, viewValue: string) => {
//    // return OK if no value or value is of correct type.
//    if (viewModel.optional && !viewValue) {
//        // optional with no value
//        viewModel.resetMessage();
//        viewModel.clientValid = true;
//    }
//    else if (!viewModel.optional && !viewValue) {
//        // mandatory with no value
//        viewModel.resetMessage();
//        viewModel.clientValid = false;
//    }
//    else if (modelValue instanceof ChoiceViewModel) {
//        // has view model check if it's valid                       
//        if (!modelValue.name) {
//            viewModel.setMessage(pendingAutoComplete);
//            viewModel.clientValid = false;
//        }
//    }
//    else {
//        // has value but not ChoiceViewModel so must be invalid 
//        viewModel.setMessage(pendingAutoComplete);
//        viewModel.clientValid = false;
//    }

//    return viewModel.clientValid;
//};
//            }
//        };




}