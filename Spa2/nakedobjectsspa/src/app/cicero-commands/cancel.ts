import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Msg from '../user-messages';
import * as RtD from '../route-data';

export class Cancel extends Command {

    shortCommand = "ca";
    fullCommand = Msg.cancelCommand;
    helpText = Msg.cancelHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog() || this.isEdit();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        if (this.isEdit()) {
            return this.returnResult("", "", () => this.urlManager.setInteractionMode(RtD.InteractionMode.View));
        }

        if (this.isDialog()) {
            return this.returnResult("", "", () => this.urlManager.closeDialogReplaceHistory(this.routeData().dialogId));
        }

        return this.returnResult("", "some sort of error"); // todo
    }
}
