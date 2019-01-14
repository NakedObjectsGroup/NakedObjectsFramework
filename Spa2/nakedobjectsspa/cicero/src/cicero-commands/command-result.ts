import * as Ro from '@nakedobjects/restful-objects';
import { ContextService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import map from 'lodash-es/map';
import mapValues from 'lodash-es/mapValues';
import { Result } from './result';

// todo move this
export function getParametersAndCurrentValue(action: Ro.ActionMember | Ro.ActionRepresentation | Ro.InvokableActionMember, context: ContextService): Dictionary<Ro.Value> {

    if (action instanceof Ro.InvokableActionMember || action instanceof Ro.ActionRepresentation) {
        const parms = action.parameters();
        const cachedValues = context.getDialogCachedValues(action.actionId());
        const values = mapValues(parms, p => {
            const value = cachedValues[p.id()];
            return value === undefined ? p.default() : value;
        });
        return values;
    }
    return {};
}

export function getFields(field: Ro.IField): Ro.IField[] {

    if (field instanceof Ro.Parameter) {
        const action = field.parent;
        if (action instanceof Ro.InvokableActionMember || action instanceof Ro.ActionRepresentation) {
            const parms = action.parameters();
            return map(parms, p => p as Ro.IField);
        }
    }

    if (field instanceof Ro.PropertyMember) {
        // todo
        return [];
    }

    return [];
}

export class CommandResult extends Result {
    stopChain: boolean;
    changeState: () => void = () => { };
}
