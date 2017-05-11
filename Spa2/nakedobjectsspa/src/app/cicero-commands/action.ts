import * as Cicerocommands from './command-result';
import * as Models from '../models';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Dictionary } from 'lodash';
import map from 'lodash/map';

import forEach from 'lodash/forEach';
import reduce from 'lodash/reduce';

export class Action extends Command.Command {

    fullCommand = Usermessages.actionCommand;
    helpText = Usermessages.actionHelp;
    protected minArguments = 0;
    protected maxArguments = 2;

    isAvailableInCurrentContext(): boolean {
        return (this.isMenu() || this.isObject() || this.isForm()) && !this.isDialog() && !this.isEdit(); //TODO add list
    }

   

    doExecuteNew(args: string, chained: boolean, result: Cicerocommands.CommandResult): Promise<Cicerocommands.CommandResult> {
        const match = this.argumentAsString(args, 0);
        const details = this.argumentAsString(args, 1, true);
        if (details && details !== "?") {
            return this.returnResult("", Usermessages.mustbeQuestionMark);
        }
        if (this.isObject()) {
            return this.getObject().then(obj => this.processActionsNew(match, obj.actionMembers(), details));

        } else if (this.isMenu()) {
            return this.getMenu().then(menu => this.processActionsNew(match, menu.actionMembers(), details));
        }
        //TODO: handle list - CCAs
        return Promise.reject("TODO: handle list - CCAs");
    }

    private processActionsNew(match: string | null, actionsMap: Dictionary<Models.ActionMember>, details: string): Promise<Cicerocommands.CommandResult> {
        let actions = map(actionsMap, action => action);
        if (actions.length === 0) {
            return this.returnResult("", Usermessages.noActionsAvailable);
        }
        if (match) {
            actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
        }
        switch (actions.length) {
        case 0:
            return this.returnResult("", Usermessages.doesNotMatchActions(match));
        case 1:
            const action = actions[0];
            if (details) {
                return this.returnResult("", this.renderActionDetails(action));
            } else if (action.disabledReason()) {
                return this.returnResult("", this.disabledAction(action));
            } else {
                return this.openActionDialog(action);
            }
        default:
            let output = match ? Usermessages.matchingActions : Usermessages.actionsMessage;
            output += this.listActions(actions);
            return this.returnResult("", output);
        }
    }

   

    private disabledAction(action: Models.ActionMember) {
        return `${Usermessages.actionPrefix} ${action.extensions().friendlyName()} ${Usermessages.isDisabled} ${action.disabledReason()}`;
    }

    private listActions(actions: Models.ActionMember[]): string {
        return reduce(actions,
            (s, t) => {
                const menupath = t.extensions().menuPath() ? `${t.extensions().menuPath()} - ` : "";
                const disabled = t.disabledReason() ? ` (${Usermessages.disabledPrefix} ${t.disabledReason()})` : "";
                return s + menupath + t.extensions().friendlyName() + disabled + "\n";
            },
            "");
    }

    private openActionDialog(action: Models.ActionMember): Promise<Cicerocommands.CommandResult> {

        return this.context.getInvokableAction(action).
            then(invokable => {

                this.context.clearDialogCachedValues();
                this.urlManager.setDialog(action.actionId());
                forEach(invokable.parameters(), p => this.setFieldValueInContextAndUrl(p, p.default()));

                return this.returnResult("", "");
            });
    }

    private renderActionDetails(action: Models.ActionMember) {
        return `${Usermessages.descriptionPrefix} ${action.extensions().friendlyName()}\n${action.extensions().description() || Usermessages.noDescription}`;
    }
}