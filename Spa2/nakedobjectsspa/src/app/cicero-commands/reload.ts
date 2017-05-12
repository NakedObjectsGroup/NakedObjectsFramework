import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Reload extends Command {

    shortCommand = "re";
    fullCommand = Usermessages.reloadCommand;
    helpText = Usermessages.reloadHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isList();
    }

    doExecute(args: string, chained: boolean): Promise<CommandResult> {
        return Promise.reject("Not Implemented");
    };
}