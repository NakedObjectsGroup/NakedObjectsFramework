import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiDatepicker]' })
export class GeminiDatepickerDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

    //// Enforce the angularJS default of restricting the directive to
    //// attributes only
    //restrict: "A",
    //// Always use along with an ng-model
    //require: "?ngModel",

    //// to make sure dynamic ids on element get picked up
    //transclude: true,
    //// This method needs to be defined and passed in from the
    //// passed in to the directive from the view controller
    //scope: {
    //    select: "&" // Bind the select function we refer to the right scope
    //},




    //link(scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

    //    if (!ngModel) return;
    //    // only add datepicker if date field not supported 
    //    if (element.prop("type") === "date") return;

    //    const parent = scope.$parent as IPropertyOrParameterScope;
    //    const viewModel = parent.parameter || parent.property;

    //    // adding parser at the front that converts to a format angluar parsers understand
    //    ngModel.$parsers.reverse();
    //    ngModel.$parsers.push(val => {

    //        const dt1 = moment(val, supportedDateFormats, "en-GB", true);

    //        if (dt1.isValid()) {
    //            const dt = dt1.toDate();
    //            return toDateString(dt);
    //        }

    //        return "";
    //    });
    //    ngModel.$parsers.reverse();

    //    // add our formatter that converts from date to our format
    //    ngModel.$formatters = [];

    //    // use viewmodel filter if we've been given one 
    //    const localFilter = viewModel && viewModel.localFilter ? viewModel.localFilter : mask.defaultLocalFilter("date");
    //    ngModel.$formatters.push(val => localFilter.filter(val));

    //    // put on viewmodel for error message formatting
    //    if (viewModel && !viewModel.localFilter) {
    //        viewModel.localFilter = localFilter;
    //    }

    //    // also for dynamic ids - need to wrap link in timeout. 
    //    $timeout(() => {

    //        const updateModel = (dateTxt: any) => {
    //            scope.$apply(() => {
    //                // Call the internal AngularJS helper to
    //                // update the two way binding

    //                ngModel.$setViewValue(dateTxt);
    //                element.change(); // do this to trigger gemini-clear directive  
    //            });
    //        };

    //        const onSelect = (dateTxt: any) => updateModel(dateTxt);

    //        const optionsObj = {
    //            dateFormat: "d M yy", // datepicker format
    //            onSelect: onSelect,
    //            showOn: "button",
    //            buttonImage: "images/calendar.png",
    //            buttonImageOnly: true,
    //            buttonText: "Select date"
    //        };

    //        (element as any).datepicker(optionsObj);

    //    });
    //}
//};







}