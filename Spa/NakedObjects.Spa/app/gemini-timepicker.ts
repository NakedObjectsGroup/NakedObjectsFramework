import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiTimepicker]' })
export class GeminiTimepickerDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//    // Enforce the angularJS default of restricting the directive to
//    // attributes only
//    restrict: "A",
//    // Always use along with an ng-model
//    require: "?ngModel",

//    // to make sure dynamic ids on element get picked up
//    transclude: true,
//    // This method needs to be defined and passed in from the
//    // passed in to the directive from the view controller
//    scope: {
//        select: "&" // Bind the select function we refer to the right scope
//    },
//    link(scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

//        if (!ngModel) return;
//        // only add datepicker if time field not supported 
//        if (element.prop("type") === "time") return;

//        const parent = scope.$parent as IPropertyOrParameterScope;
//        const viewModel = parent.parameter || parent.property;

//        // also for dynamic ids - need to wrap link in timeout. 
//        $timeout(() => {

//            const updateModel = () => {
//                scope.$apply(() => {
//                    // Call the internal AngularJS helper to
//                    // update the two way binding

//                    ngModel.$setViewValue(element.val());
//                    element.change(); // do this to trigger gemini-clear directive 
//                });
//                return true;
//            };

//            const optionsObj = {
//                timeFormat: "H:i", // timepicker format
//                showOn: null as any
//            };

//            (element as any).timepicker(optionsObj);
//            element.on("changeTime", updateModel);

//            const button = $("<img class='ui-datepicker-trigger' src='images/calendar.png' alt='Select time' title='Select time'>");

//            button.on("click", () => (element as any).timepicker("show"));

//            element.after(button);
//        });
//    }
//};

}