import { Renderer, ElementRef } from '@angular/core';
import { ISubscription } from 'rxjs/Subscription';
import { Dictionary } from 'lodash';
import { FormBuilder, AbstractControl, FormGroup } from '@angular/forms';
import { DialogViewModel } from './view-models/dialog-view-model';
import { ParameterViewModel } from './view-models/parameter-view-model';
import forEach from 'lodash/forEach';
import map from 'lodash/map';
import zipObject from 'lodash/zipObject';
import mapValues from 'lodash/mapValues';


export function safeUnsubscribe(sub: ISubscription) {
    if (sub) {
        sub.unsubscribe();
    }
}


export function focus(renderer: Renderer, element: ElementRef) {
    setTimeout(() => renderer.invokeElementMethod(element.nativeElement, "focus"));
    return true;
}


export function createForm(dialog: DialogViewModel, formBuilder: FormBuilder): { form: FormGroup, dialog: DialogViewModel, parms: Dictionary<ParameterViewModel>, sub : ISubscription } {
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
