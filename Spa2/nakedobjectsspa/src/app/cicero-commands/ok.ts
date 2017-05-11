import * as Cicerocommands from './command-result';
import * as Models from '../models';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Dictionary } from 'lodash';
import forEach from 'lodash/forEach';
import { Location } from '@angular/common';

export class OK extends Command.Command {

    fullCommand = Usermessages.okCommand;
    helpText = Usermessages.okHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog();
    }

   

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        return this.getActionForCurrentDialog().then((action: Models.ActionRepresentation | Models.InvokableActionMember) => {

            if (chained && action.invokeLink().method() !== "GET") {
                return this.returnResult("", this.mayNotBeChained(Usermessages.queryOnlyRider), () => { }, true);
            }

            let fieldMap: Dictionary<Models.Value>;
            if (this.isForm()) {
                const obj = action.parent as Models.DomainObjectRepresentation;
                fieldMap = this.context.getObjectCachedValues(obj.id()); //Props passed in as pseudo-params to action
            } else {
                fieldMap = Cicerocommands.getParametersAndCurrentValue(action, this.context);
            }

            return this.context.invokeAction(action, fieldMap).then((result: Models.ActionResultRepresentation) => {
                // todo handle case where result is empty - this is no longer handled 
                // by reject below
                let alert = "";

                const warnings = result.extensions().warnings();
                if (warnings) {
                    forEach(warnings, w => alert += `\nWarning: ${w}`);
                }
                const messages = result.extensions().messages();
                if (messages) {
                    forEach(messages, m => alert += `\n${m}`);
                }

                return this.returnResult("", alert || null, () => this.urlManager.closeDialogReplaceHistory(""));

            }).catch((reject: Models.ErrorWrapper) => {
                if (reject.error instanceof Models.ErrorMap) {
                    const paramFriendlyName = (paramId: string) => Models.friendlyNameForParam(action, paramId);
                    return this.handleErrorResponse(reject.error as Models.ErrorMap, paramFriendlyName);
                }
                return Promise.reject(reject);
            });
        });
    };
}