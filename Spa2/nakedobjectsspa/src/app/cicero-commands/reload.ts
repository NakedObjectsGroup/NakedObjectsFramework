import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Reload extends Command.Command {

    fullCommand = Usermessages.reloadCommand;
    helpText = Usermessages.reloadHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isList();
    }

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        return Promise.reject("Not Implemented");
    };
}