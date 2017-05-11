import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import * as Routedata from '../route-data';
import { Location } from '@angular/common';

export class Cancel extends Command.Command {

    fullCommand = Usermessages.cancelCommand;
    helpText = Usermessages.cancelHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog() || this.isEdit();
    }

    doExecute(args: string, chained: boolean): void {
        if (this.isEdit()) {
            this.urlManager.setInteractionMode(Routedata.InteractionMode.View);
        }
        if (this.isDialog()) {
            this.urlManager.closeDialogReplaceHistory(""); //TODO provide ID
        }
    };

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        if (this.isEdit()) {           
            return this.returnResult("", "", () => this.urlManager.setInteractionMode(Routedata.InteractionMode.View));
        }

        if (this.isDialog()) {
            return this.returnResult("", "", () => this.urlManager.closeDialogReplaceHistory(this.routeData().dialogId));
        }

        return this.returnResult("", "some sort of error"); // todo
    };
}