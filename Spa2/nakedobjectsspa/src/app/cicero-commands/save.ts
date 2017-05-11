import * as Cicerocommands from './command-result';
import * as Models from '../models';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import * as Routedata from '../route-data';
import { Dictionary } from 'lodash';
import map from 'lodash/map';
import some from 'lodash/some';
import filter from 'lodash/filter';
import every from 'lodash/every';
import each from 'lodash/each';
import forEach from 'lodash/forEach';
import findIndex from 'lodash/findIndex';
import zipObject from 'lodash/zipObject';
import { Location } from '@angular/common';

export class Save extends Command.Command {

    fullCommand = Usermessages.saveCommand;
    helpText = Usermessages.saveHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isEdit() || this.isTransient();
    }

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        if (chained) {
            //this.mayNotBeChained();
            return this.returnResult("", this.mayNotBeChained(), () => {}, true);
        }
        return this.getObject().then((obj: Models.DomainObjectRepresentation) => {
            const props = obj.propertyMembers();
            const newValsFromUrl = this.context.getObjectCachedValues(obj.id());
            const propIds = new Array<string>();
            const values = new Array<Models.Value>();
            forEach(props,
                (propMember, propId) => {
                    if (!propMember.disabledReason()) {
                        propIds.push(propId);
                        const newVal = newValsFromUrl[propId];
                        if (newVal) {
                            values.push(newVal);
                        } else if (propMember.value().isNull() &&
                            propMember.isScalar()) {
                            values.push(new Models.Value(""));
                        } else {
                            values.push(propMember.value());
                        }
                    }
                });
            const propMap = zipObject(propIds, values) as Dictionary<Models.Value>;
            const mode = obj.extensions().interactionMode();
            const toSave = mode === "form" || mode === "transient";
            const saveOrUpdate = toSave ? this.context.saveObject : this.context.updateObject;

            return saveOrUpdate(obj, propMap, 1, true).then(() => {
                return this.returnResult(null, null);
            }).catch((reject: Models.ErrorWrapper) => {
                if (reject.error instanceof Models.ErrorMap) {
                    const propFriendlyName = (propId: string) => Models.friendlyNameForProperty(obj, propId);
                    return this.handleErrorResponse(reject.error, propFriendlyName);
                }
                return Promise.reject(reject);
            });
        });
    };

    private handleError(err: Models.ErrorMap, obj: Models.DomainObjectRepresentation) {
        if (err.containsError()) {
            const propFriendlyName = (propId: string) => Models.friendlyNameForProperty(obj, propId);
            this.handleErrorResponse(err, propFriendlyName);
        } else {
            this.urlManager.setInteractionMode(Routedata.InteractionMode.View);
        }
    }
}