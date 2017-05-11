import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Gemini extends Command.Command {

    fullCommand = Usermessages.geminiCommand;
    helpText = Usermessages.geminiHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }


    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        return this.returnResult("", "", () => this.urlManager.gemini());
    };
}