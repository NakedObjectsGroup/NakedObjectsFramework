import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Root extends Command.Command {

    fullCommand = Usermessages.rootCommand;
    helpText = Usermessages.rootHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isCollection();
    }

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        return this.returnResult(null, null, () => this.closeAnyOpenCollections());
    };
}