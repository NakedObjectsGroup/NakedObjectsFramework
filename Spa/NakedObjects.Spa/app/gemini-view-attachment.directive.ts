import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiBoolean]' })
export class GeminiBooleanDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//    // Enforce the angularJS default of restricting the directive to
//    // attributes only
//    restrict: "A",
//    // Always use along with an ng-model
//    require: "?ngModel",
//    link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
//        if(!ngModel) {
//    return;
//}

//ngModel.$render = () => {
//    const attachment: IAttachmentViewModel = ngModel.$modelValue;
//    if (attachment) {
//        const title = attachment.title;
//        element.empty();
//        attachment.downloadFile().
//            then(blob => {
//                const reader = new FileReader();
//                reader.onloadend = () => element.html(`<img src='${reader.result}' alt='${title}' />`);
//                reader.readAsDataURL(blob);
//            }).
//            catch((reject: ErrorWrapper) => error.handleError(reject));
//    }
//};
//            }
//        };



}