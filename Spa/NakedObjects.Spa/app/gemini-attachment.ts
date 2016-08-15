import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiBoolean]' })
export class GeminiBooleanDirective {
    private el: HTMLElement;

    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//// Enforce the angularJS default of restricting the directive to
//// attributes only
//restrict: "A",
//    // Always use along with an ng-model
//    require: "?ngModel",
//        link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {
//            if (!ngModel) {
//                return;
//            }

//            const clickHandler = () => {
//                const attachment: AttachmentViewModel = ngModel.$modelValue;

//                if (!attachment.displayInline()) {
//                    attachment.downloadFile().
//                        then(blob => {
//                            if (window.navigator.msSaveBlob) {
//                                // internet explorer 
//                                window.navigator.msSaveBlob(blob, attachment.title);
//                            } else {
//                                const burl = URL.createObjectURL(blob);
//                                $window.location.href = burl;
//                            }
//                        }).
//                        catch((reject: ErrorWrapper) => error.handleError(reject));
//                }

//                return false;
//            };

//            ngModel.$render = () => {
//                const attachment: AttachmentViewModel = ngModel.$modelValue;

//                if (attachment) {

//                    const title = attachment.title;

//                    element.empty();

//                    const anchor = element.find("div");
//                    if (attachment.displayInline()) {
//                        attachment.downloadFile().
//                            then(blob => {
//                                const reader = new FileReader();
//                                reader.onloadend = () => {
//                                    if (reader.result) {
//                                        element.html(`<img src='${reader.result}' alt='${title}' />`);
//                                    }
//                                }
//                                reader.readAsDataURL(blob);
//                            }).
//                            catch((reject: ErrorWrapper) => error.handleError(reject));
//                    } else {
//                        anchor.html(title);
//                        attachment.doClick = clickHandler;
//                    }

//                } else {
//                    element.append("<div>Attachment not yet supported on transient</div>");
//                }
//            };
//        }
//        };

}