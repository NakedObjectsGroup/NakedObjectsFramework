import { Directive, ElementRef, HostListener, Output, EventEmitter } from '@angular/core';

@Directive({ selector: '[geminiDrop]' })
export class GeminiDropDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;
    }

//    const propertyScope = () => scope.$parent.$parent.$parent as IPropertyOrParameterScope;
//    const parameterScope = () => scope.$parent.$parent as IPropertyOrParameterScope;

//    const accept = (draggable: any) => {
//        const droppableVm: IFieldViewModel = propertyScope().property || parameterScope().parameter;
//        const draggableVm: IDraggableViewModel = draggable.data(draggableVmKey);

//        if (draggableVm) {
//            draggableVm.canDropOn(droppableVm.returnType).
//                then((canDrop: boolean) => {
//                    if (canDrop) {
//                        element.addClass("candrop");
//                    } else {
//                        element.removeClass("candrop");
//                    }
//                }).
//                catch(() => element.removeClass("candrop"));
//            return true;
//        }
//        return false;
//    };

//        (element as any).droppable({
//    tolerance: "touch",
//    hoverClass: "dropping",
//    accept: accept
//});

//element.on("drop", (event: any, ui: any) => {

//    if (element.hasClass("candrop")) {

//        const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
//        const droppableVm: IFieldViewModel = droppableScope.property || droppableScope.parameter;
//        const draggableVm = <IDraggableViewModel>ui.draggable.data(draggableVmKey);

//        droppableScope.$apply(() => {
//            droppableVm.drop(draggableVm);
//            $timeout(() => $(element).change());
//        });
//    }
//});

//element.on("keydown keypress", (event: any) => {
//    const vKeyCode = 86;
//    const deleteKeyCode = 46;
//    if (event.keyCode === vKeyCode && event.ctrlKey) {


//        const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
//        const droppableVm: IFieldViewModel = droppableScope.property || droppableScope.parameter;
//        const draggableVm = <IDraggableViewModel>($("div.footer div.currentcopy .reference").data(draggableVmKey) as any);

//        if (draggableVm) {
//            // only consume event if we are actually dropping on a field
//            event.preventDefault();
//            droppableScope.$apply(() => droppableVm.drop(draggableVm));
//        }
//    }
//    if (event.keyCode === deleteKeyCode) {
//        const droppableScope = propertyScope().property ? propertyScope() : parameterScope();
//        const droppableVm: IFieldViewModel = droppableScope.property || droppableScope.parameter;

//        scope.$apply(droppableVm.clear);
//    }
//});



}