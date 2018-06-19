import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Forward extends Command {

    shortCommand = "fo";
    fullCommand = Usermessages.forwardCommand;
    helpText = Usermessages.forwardHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        return this.returnResult("", null, () => this.location.forward());
    }
}
