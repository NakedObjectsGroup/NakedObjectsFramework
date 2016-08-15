import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiFileUpload]' })
export class GeminiFileUploadDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//    restrict: "A",
//    scope: true,
//    require: "?ngModel",
//    link: (scope: ISelectScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) => {

//        if(!ngModel) {
//    return;
//}

//element.bind("change", () => {
//    const file = (element[0] as any).files[0] as File;

//    const fileReader = new FileReader();
//    fileReader.onloadend = () => {
//        const link = new Link({
//            href: fileReader.result,
//            type: file.type,
//            title: file.name
//        } as RoInterfaces.ILink);

//        ngModel.$setViewValue(link);
//    };

//    fileReader.readAsDataURL(file);
//});
//            }
//        };

}