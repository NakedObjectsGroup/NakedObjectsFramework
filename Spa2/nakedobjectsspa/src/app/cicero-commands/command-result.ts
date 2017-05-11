import * as Ro from '../models';
import * as Models from '../models';
import { ContextService } from '../context.service';
import { Dictionary } from 'lodash';
import mapValues from 'lodash/mapValues';
import {Result} from './result';

// todo move this 
export function getParametersAndCurrentValue(action: Ro.ActionMember | Models.ActionRepresentation | Models.InvokableActionMember, context: ContextService): Dictionary<Ro.Value> {

    if (action instanceof Models.InvokableActionMember || action instanceof Models.ActionRepresentation) {
        const parms = action.parameters();
        const values = context.getDialogCachedValues(action.actionId());
        return mapValues(parms, p => {
            const value = values[p.id()];
            return value === undefined ? p.default() : value;
        });
    }
    return {};
}

export class CommandResult extends Result{
    changeState: () => void = () => { };
    stopChain: boolean;
}