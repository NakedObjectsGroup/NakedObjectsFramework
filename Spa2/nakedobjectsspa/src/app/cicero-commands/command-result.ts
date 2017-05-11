import * as Ro from '../models';
import * as Msg from '../user-messages';
import * as Rend from '../cicero-renderer.service';
import * as Const from '../constants';
import * as RtD from '../route-data';
import * as Models from '../models';
import { ContextService } from '../context.service';
import { UrlManagerService } from '../url-manager.service';
import { MaskService } from '../mask.service';
import { ErrorService } from '../error.service';
import { Location } from '@angular/common';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { ConfigService, IAppConfig } from '../config.service';
import * as moment from 'moment';
import { Dictionary } from 'lodash';
import each from 'lodash/each';
import filter from 'lodash/filter';
import findIndex from 'lodash/findIndex';
import forEach from 'lodash/forEach';
import every from 'lodash/every';
import keys from 'lodash/keys';
import map from 'lodash/map';
import zipObject from 'lodash/zipObject';
import mapValues from 'lodash/mapValues';
import reduce from 'lodash/reduce';
import some from 'lodash/some';
import mapKeys from 'lodash/mapKeys';
import fromPairs from 'lodash/fromPairs';
import * as Cicerocontextservice from '../cicero-context.service';

export function getParametersAndCurrentValue(action: Ro.ActionMember | Models.ActionRepresentation | Models.InvokableActionMember, context: ContextService): Dictionary<Ro.Value> {

    if (action instanceof Models.InvokableActionMember || action instanceof Models.ActionRepresentation) {
        const parms = action.parameters();
        const values = context.getDialogCachedValues(action.actionId());
        return mapValues(parms,
            p => {
                const value = values[p.id()];
                if (value === undefined) {
                    return p.default();
                }
                return value;
            });
    }
    return {};
}

export class CommandResult {
    input: string;
    output: string;
    changeState: () => void = () => { };
    stopChain : boolean; 
}