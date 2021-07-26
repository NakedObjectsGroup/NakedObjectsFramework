import * as Ro from '@nakedobjects/restful-objects';
import { ErrorWrapper, InteractionMode } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import forEach from 'lodash-es/forEach';
import zipObject from 'lodash-es/zipObject';
import { Command } from './Command';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';

export class Save extends Command {

    shortCommand = 'sa';
    fullCommand = Usermessages.saveCommand;
    helpText = Usermessages.saveHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isEdit() || this.isTransient();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        if (chained) {
            return this.returnResult('', this.mayNotBeChained(), () => { }, true);
        }
        return this.getObject().then((obj: Ro.DomainObjectRepresentation) => {
            const props = obj.propertyMembers();
            const newValsFromUrl = this.context.getObjectCachedValues(obj.id());
            const propIds = new Array<string>();
            const values = new Array<Ro.Value>();
            forEach(props,
                (propMember, propId) => {
                    if (!propMember.disabledReason()) {
                        propIds.push(propId);
                        const newVal = newValsFromUrl[propId];
                        if (newVal) {
                            values.push(newVal);
                        } else if (propMember.value().isNull() &&
                            propMember.isScalar()) {
                            values.push(new Ro.Value(''));
                        } else {
                            values.push(propMember.value());
                        }
                    }
                });
            const propMap = zipObject(propIds, values) as Dictionary<Ro.Value>;
            const mode = obj.extensions().interactionMode();
            const toSave = mode === 'form' || mode === 'transient';
            const saveOrUpdate = toSave ? this.context.saveObject : this.context.updateObject;

            return saveOrUpdate(obj, propMap, 1, true).then(() => {
                return this.returnResult(null, null);
            }).catch((reject: ErrorWrapper) => {
                if (reject.error instanceof Ro.ErrorMap) {
                    const propFriendlyName = (propId: string) => Ro.friendlyNameForProperty(obj, propId);
                    return this.handleErrorResponse(reject.error, propFriendlyName);
                }
                return Promise.reject(reject);
            });
        });
    }

    private handleError(err: Ro.ErrorMap, obj: Ro.DomainObjectRepresentation) {
        if (err.containsError()) {
            const propFriendlyName = (propId: string) => Ro.friendlyNameForProperty(obj, propId);
            this.handleErrorResponse(err, propFriendlyName);
        } else {
            this.urlManager.setInteractionMode(InteractionMode.View);
        }
    }
}
