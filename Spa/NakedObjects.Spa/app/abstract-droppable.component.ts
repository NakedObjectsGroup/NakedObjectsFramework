import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";

export class AbstractDroppableComponent {

    canDrop = false;

    droppable : ViewModels.IFieldViewModel;

    accept = (draggableVm: ViewModels.IDraggableViewModel) => {

        if (draggableVm) {
            draggableVm.canDropOn(this.droppable.returnType).
                then((canDrop: boolean) => this.canDrop = canDrop).
                catch(() => this.canDrop = false);
            return true;
        }
        return false;
    };

    drop(draggableVm: ViewModels.IDraggableViewModel) {
        if (this.canDrop) {
            this.droppable.drop(draggableVm);
            //const evt = new Event("change");
            //this.el.dispatchEvent(evt);
        }
    }
}