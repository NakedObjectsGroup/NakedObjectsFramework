import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Back extends Command.Command {

    fullCommand = Usermessages.backCommand;
    helpText = Usermessages.backHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        this.location.back();
    };

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
       
        return this.returnResult("", "", () => this.location.back());
    };
}