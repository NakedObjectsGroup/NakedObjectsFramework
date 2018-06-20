import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';

export class Help extends Command {

    shortCommand = "he";
    fullCommand = Usermessages.helpCommand;
    helpText = Usermessages.helpHelp;
    protected minArguments = 0;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        const arg = this.argumentAsString(args, 0);
        if (!arg) {
            return this.returnResult("", Usermessages.basicHelp);
        } else if (arg === "?") {
            const commands = this.commandFactory.allCommandsForCurrentContext();
            return this.returnResult("", commands);
        } else {
            try {
                const c = this.commandFactory.getCommand(arg);
                return this.returnResult("", `${c.fullCommand} command:\n${c.helpText}`);
            } catch (e) {
                return this.returnResult("", e.message);
            }
        }
    }
}
