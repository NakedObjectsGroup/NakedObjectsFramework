import { Directive, ElementRef, HostListener, Output, EventEmitter, Input } from '@angular/core';
import * as ViewModels from "./nakedobjects.viewmodels";

const draggableVmKey = "dvmk";

@Directive({ selector: '[geminiDrop]' })
export class GeminiDropDirective {
    private el: HTMLElement;
    constructor(el: ElementRef) {
        this.el = el.nativeElement;

        (this.el as any).droppable({
            tolerance: "touch",
            hoverClass: "dropping",
            accept: this.accept
        });
    }

    accept = (draggable: any) => {
      
        const draggableVm: ViewModels.IDraggableViewModel = draggable.data(draggableVmKey);

        if (draggableVm) {
            draggableVm.canDropOn(this.target.returnType).
                then((canDrop: boolean) => {
                    if (canDrop) {
                        this.el.classList.add("candrop");
                    } else {
                        this.el.classList.remove("candrop");
                    }
                }).
                catch(() => this.el.classList.remove("candrop"));
            return true;
        }
        return false;
    };
    

    @HostListener('drop', ['$event']) onDrop(event: KeyboardEvent, ui : any) {
        if (this.el.classList.contains("candrop")) {

            const draggableVm = <ViewModels.IDraggableViewModel>ui.draggable.data(draggableVmKey);
       
            this.target.drop(draggableVm);
            const evt = new Event("change");
            this.el.dispatchEvent(evt);
        }
    }

    @Input('geminiDrop')
    target: ViewModels.IFieldViewModel;

    handle(event: KeyboardEvent) {
        const vKeyCode = 86;
        const deleteKeyCode = 46;
        if (event.keyCode === vKeyCode && event.ctrlKey) {

            const draggableVm = <ViewModels.IDraggableViewModel> (<any>document.querySelector("div.footer div.currentcopy .reference")).data(draggableVmKey) as any;

            if (draggableVm) {
                // only consume event if we are actually dropping on a field
                event.preventDefault();
                this.target.drop(draggableVm);
            }
        }
        if (event.keyCode === deleteKeyCode) {
            this.target.clear();
        }
    }

    @HostListener('keypress', ['$event']) onKeyPress(event: KeyboardEvent) {
        return this.handle(event);
    }

    @HostListener('keydown', ['$event']) onKeyDown(event: KeyboardEvent) {
        return this.handle(event);
    }
}