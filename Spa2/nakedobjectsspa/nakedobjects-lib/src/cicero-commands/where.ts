import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Where extends Command {

    shortCommand = "wh";
    fullCommand = Usermessages.whereCommand;
    helpText = Usermessages.whereHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
        return this.returnResult(null, null);
    }
}
