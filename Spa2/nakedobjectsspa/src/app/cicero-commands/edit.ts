import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import * as Routedata from '../route-data';
import { Location } from '@angular/common';

export class Edit extends Command.Command {

    fullCommand = Usermessages.editCommand;
    helpText = Usermessages.editHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() && !this.isEdit();
    }

    doExecute(args: string, chained: boolean): void {
        if (chained) {
            this.mayNotBeChained();
            return;
        }
        this.context.clearObjectCachedValues();
        this.urlManager.setInteractionMode(Routedata.InteractionMode.Edit);
    };

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        if (chained) {
            return this.returnResult("", this.mayNotBeChained(), () => {}, true);
        }
        const newState = () => {
            this.context.clearObjectCachedValues();
            this.urlManager.setInteractionMode(Routedata.InteractionMode.Edit);
        }

        return this.returnResult("", "", newState);
    };
}