import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';
import * as Routedata from '../route-data';

export class Edit extends Command {

    shortCommand = "ed";
    fullCommand = Usermessages.editCommand;
    helpText = Usermessages.editHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() && !this.isEdit();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        if (chained) {
            return this.returnResult("", this.mayNotBeChained(), () => { }, true);
        }
        const newState = () => {
            this.context.clearObjectCachedValues();
            this.urlManager.setInteractionMode(Routedata.InteractionMode.Edit);
        };

        return this.returnResult("", "", newState);
    }
}
