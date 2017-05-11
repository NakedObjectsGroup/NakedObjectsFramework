import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Where extends Command {

    fullCommand = Usermessages.whereCommand;
    helpText = Usermessages.whereHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): Promise<CommandResult> {
        return this.returnResult(null, null, () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl());
    };
}