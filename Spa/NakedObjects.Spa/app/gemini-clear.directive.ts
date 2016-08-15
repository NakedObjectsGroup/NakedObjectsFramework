import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiClear]' })
export class GeminiClearDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//    // Enforce the angularJS default of restricting the directive to
//    // attributes only
//    restrict: "A",
//    // Always use along with an ng-model
//    require: "?ngModel",
//    link: (scope: ISelectScope, elm: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
//        if(!ngModel) {
//    return;
//}

//// wrap in timeout or we won't see initial value 
//$timeout(() => {
//    $(elm).addClass("ng-clearable");

//    if (elm.val()) {
//        $(elm).addClass("ng-x");
//    } else {
//        $(elm).removeClass("ng-x");
//    }
//});

//elm.on("input change", function () {
//    $(this).addClass("ng-clearable");
//    if (this.value) {
//        $(this).addClass("ng-x");
//    } else {
//        $(this).removeClass("ng-x");
//    }
//}).on("mousemove", function (e) {
//    if (elm.hasClass("ng-x")) {

//        const onX = this.offsetWidth - 18 < e.clientX - this.getBoundingClientRect().left;

//        if (onX) {
//            $(this).addClass("ng-onX");
//        } else {
//            $(this).removeClass("ng-onX");
//        }
//    }
//}).on("touchstart click", function (ev) {
//    if ($(this).hasClass("ng-onX")) {

//        ev.preventDefault();
//        $(this).removeClass("ng-x ng-onX");

//        scope.$apply(() => {
//            const parent = scope.$parent as IPropertyOrParameterScope;
//            const viewModel = parent.parameter || parent.property;
//            viewModel.clear();

//            ngModel.$setViewValue("");
//            $(this).val("");

//            // ick but only way I can get color to clear on freeform droppable fields
//            $timeout(() => viewModel.color = "");
//        });
//    }
//});
//            }
//        };
//    });
}