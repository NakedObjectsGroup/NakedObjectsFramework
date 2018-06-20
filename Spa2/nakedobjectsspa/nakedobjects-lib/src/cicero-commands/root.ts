import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Root extends Command {

    shortCommand = "ro";
    fullCommand = Usermessages.rootCommand;
    helpText = Usermessages.rootHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isCollection();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        return this.returnResult(null, null, () => this.closeAnyOpenCollections());
    }
}
