import { Renderer, ElementRef } from '@angular/core';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { Dictionary } from 'lodash';
import { FormBuilder, AbstractControl, FormGroup } from '@angular/forms';
import { DialogViewModel } from './view-models/dialog-view-model';
import { ParameterViewModel } from './view-models/parameter-view-model';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import zipObject from 'lodash-es/zipObject';
import mapValues from 'lodash-es/mapValues';
import {FieldViewModel} from './view-models/field-view-model';
import {IDraggableViewModel} from './view-models/idraggable-view-model';

export function safeUnsubscribe(sub: ISubscription) {
    if (sub) {
        sub.unsubscribe();
    }
}

export function focus(renderer: Renderer, element: ElementRef) {
    setTimeout(() => renderer.invokeElementMethod(element.nativeElement, "focus"));
    return true;
}

export function createForm(dialog: DialogViewModel, formBuilder: FormBuilder): { form: FormGroup, dialog: DialogViewModel, parms: Dictionary<ParameterViewModel>, sub: ISubscription } {
    const pps = dialog.parameters;
    const parms = zipObject(map(pps, p => p.id), map(pps, p => p)) as Dictionary<ParameterViewModel>;
    const controls = mapValues(parms, p => [p.getValueForControl(), (a: AbstractControl) => p.validator(a)]);
    const form = formBuilder.group(controls);

    const sub = form.valueChanges.subscribe((data: any) => {
        // cache parm values
        forEach(data, (v, k) => parms[k!].setValueFromControl(v));
        dialog.setParms();
    });

    return { form: form, dialog: dialog, parms: parms, sub : sub };
}

export function accept(droppableVm: FieldViewModel, component: { canDrop: boolean }) {

    return (draggableVm: IDraggableViewModel) => {
        if (draggableVm) {
            draggableVm.canDropOn(droppableVm.returnType).
                then(canDrop => component.canDrop = canDrop).
                catch(() => component.canDrop = false);
            return true;
        }
        return false;
    };
}

export function dropOn(draggableVm: IDraggableViewModel, droppable: FieldViewModel,  component: { canDrop: boolean, control: AbstractControl }) {
    if (component.canDrop) {
        droppable.drop(draggableVm)
            .then((success) => {
                component.control.setValue(droppable.selectedChoice);
            });
    }
}

export function paste(event: KeyboardEvent, droppable: FieldViewModel, component: { control: AbstractControl }, get: () => IDraggableViewModel | null, clear: () => void) {
    const vKeyCode = 86;
    const deleteKeyCode = 46;
    if (event && (event.keyCode === vKeyCode && event.ctrlKey)) {
        const cvm = get();

        if (cvm) {
            droppable.drop(cvm)
                .then((success) => {
                    component.control.setValue(droppable.selectedChoice);
                });
            event.preventDefault();
        }
    }
    if (event && event.keyCode === deleteKeyCode) {
        clear();
    }
}
