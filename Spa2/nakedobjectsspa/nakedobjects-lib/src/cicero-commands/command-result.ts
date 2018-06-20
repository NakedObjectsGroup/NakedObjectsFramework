import { Dictionary } from 'lodash';
import map from 'lodash-es/map';
import mapValues from 'lodash-es/mapValues';
import { Result } from './result';
import { ContextService } from '../context.service';
import * as Ro from '../models';
import * as Models from '../models';

// todo move this
export function getParametersAndCurrentValue(action: Ro.ActionMember | Models.ActionRepresentation | Models.InvokableActionMember, context: ContextService):  Dictionary<Ro.Value> {

    if (action instanceof Models.InvokableActionMember || action instanceof Models.ActionRepresentation) {
        const parms = action.parameters();
        const cachedValues = context.getDialogCachedValues(action.actionId());
        const values = mapValues(parms, p => {
            const value = cachedValues[p.id()];
            return value === undefined ? p.default() : value;
        });
        return  values;
    }
    return {};
}

export function getFields(field: Models.IField):  Models.IField[] {

    if (field instanceof Models.Parameter) {
        const action = field.parent;
        if (action instanceof Models.InvokableActionMember || action instanceof Models.ActionRepresentation) {
            const parms = action.parameters();
            return map(parms, p => p as Models.IField);
        }
    }

    if (field instanceof Models.PropertyMember) {
        // todo
        return [];
    }

    return [];
}

export class CommandResult extends Result {
    stopChain: boolean;
    changeState: () => void = () => { };
}
