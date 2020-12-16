import { ElementRef } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup } from '@angular/forms';
import { DialogViewModel, FieldViewModel, IDraggableViewModel, ParameterViewModel } from '@nakedobjects/view-models';
import { Dictionary } from 'lodash';
import forEach from 'lodash-es/forEach';
import map from 'lodash-es/map';
import mapValues from 'lodash-es/mapValues';
import zipObject from 'lodash-es/zipObject';
import { SubscriptionLike as ISubscription } from 'rxjs';

export function safeUnsubscribe(sub: ISubscription) {
    if (sub) {
        sub.unsubscribe();
    }
}

function safeFocus(nativeElement: any) {
    if (nativeElement && nativeElement.focus) {
        nativeElement.focus();
    }
}

export function focus(element: ElementRef) {
    setTimeout(() => safeFocus(element.nativeElement));
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

    return { form: form, dialog: dialog, parms: parms, sub: sub };
}

export function accept(droppableVm: FieldViewModel, component: { canDrop: boolean }, draggableVm: IDraggableViewModel) {
    if (draggableVm) {
        draggableVm.canDropOn(droppableVm.returnType).
            then(canDrop => component.canDrop = canDrop).
            catch(() => component.canDrop = false);
        return true;
    }
    return false;
}

export function dropOn(draggableVm: IDraggableViewModel, droppable: FieldViewModel, component: { canDrop: boolean, control: AbstractControl }) {
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
