import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Back extends Command {

    fullCommand = Usermessages.backCommand;
    helpText = Usermessages.backHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): Promise<CommandResult> {
        return this.returnResult("", "", () => this.location.back());
    };
}