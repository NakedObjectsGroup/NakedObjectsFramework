import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Gemini extends Command {

    shortCommand = "ge";
    fullCommand = Usermessages.geminiCommand;
    helpText = Usermessages.geminiHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        return this.returnResult("", "", () => this.urlManager.gemini());
    }
}
