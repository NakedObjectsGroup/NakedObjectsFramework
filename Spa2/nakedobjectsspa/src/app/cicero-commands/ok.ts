import { CommandResult, getParametersAndCurrentValue } from './command-result';
import { Command } from './Command';
import * as Models from '../models';
import * as Usermessages from '../user-messages';
import { Dictionary } from 'lodash';

export class OK extends Command {

    shortCommand = "ok";
    fullCommand = Usermessages.okCommand;
    helpText = Usermessages.okHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        return this.getActionForCurrentDialog().then((action: Models.ActionRepresentation | Models.InvokableActionMember) => {

            if (chained && action.isNotQueryOnly()) {
                return this.returnResult("", this.mayNotBeChained(Usermessages.queryOnlyRider), () => { }, true);
            }

            let fieldMap: Dictionary<Models.Value>;
            if (this.isForm()) {
                const obj = action.parent as Models.DomainObjectRepresentation;
                fieldMap = this.context.getObjectCachedValues(obj.id()); // Props passed in as pseudo-params to action
            } else {
                fieldMap = getParametersAndCurrentValue(action, this.context);
            }

            return this.context.invokeAction(action, fieldMap).then((result: Models.ActionResultRepresentation) => {

               return this.returnResult("", null, () =>  this.urlManager.closeDialogReplaceHistory(this.routeData().dialogId));

            }).catch((reject: Models.ErrorWrapper) => {
                if (reject.error instanceof Models.ErrorMap) {
                    const paramFriendlyName = (paramId: string) => Models.friendlyNameForParam(action, paramId);
                    return this.handleErrorResponse(reject.error as Models.ErrorMap, paramFriendlyName);
                }
                return Promise.reject(reject);
            });
        });
    }
}
