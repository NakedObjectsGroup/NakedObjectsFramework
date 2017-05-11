import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Forward extends Command.Command {

    fullCommand = Usermessages.forwardCommand;
    helpText = Usermessages.forwardHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

   

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        return this.returnResult("", null, () => this.location.forward());
    };
}