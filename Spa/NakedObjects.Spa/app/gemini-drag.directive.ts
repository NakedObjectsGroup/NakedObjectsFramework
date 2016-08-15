import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiDrag]' })
export class GeminiDragDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//    const cloneDraggable = () => {
//        let cloned: JQuery;

//        // make the dragged element look like a reference 
//        if ($(element)[0].nodeName.toLowerCase() === "tr") {
//            const title = $(element).find("td.cell:first").text();
//            cloned = $(`<div>${title}</div>`);
//        } else {
//            cloned = $(element).clone();
//        }

//        cloned.removeClass();
//        cloned.addClass("reference");
//        return cloned;
//    };
//    element.draggable({
//        helper: cloneDraggable,
//        zIndex: 9999
//    });

//    element.on("dragstart", (event: any, ui: any) => {
//        const draggableVm = scope.property || scope.item || scope.$parent.object;

//        // make sure dragged element is correct color (object will not be set yet)
//        ui.helper.addClass(draggableVm.color);

//        // add vm to helper and original elements as accept and drop use different ones 
//        ui.helper.data(draggableVmKey, draggableVm);
//        element.data(draggableVmKey, draggableVm);
//    });

//element.on("keydown keypress", (event: any) => {
//    const cKeyCode = 67;
//    if (event.keyCode === cKeyCode && event.ctrlKey) {
//        const draggableVm = scope.property || scope.item || scope.$parent.object;
//        const compiledClone = $compile(`<div class='reference ${draggableVm.color}' gemini-drag=''>${element[0].textContent}</div>`)(scope);
//        compiledClone.data(draggableVmKey, draggableVm);
//        $("div.footer div.currentcopy").empty();
//        $("div.footer div.currentcopy").append("<span>Copying...</span>").append(compiledClone);
//        event.preventDefault();
//    }
//});


}