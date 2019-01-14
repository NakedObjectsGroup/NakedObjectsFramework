import { InteractionMode } from '@nakedobjects/services';
import { Command } from './Command';
import { CommandResult } from './command-result';
import * as Msg from '../user-messages';

export class Cancel extends Command {

    shortCommand = 'ca';
    fullCommand = Msg.cancelCommand;
    helpText = Msg.cancelHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog() || this.isEdit();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        if (this.isEdit()) {
            return this.returnResult('', '', () => this.urlManager.setInteractionMode(InteractionMode.View));
        }

        if (this.isDialog()) {
            return this.returnResult('', '', () => this.urlManager.closeDialogReplaceHistory(this.routeData().dialogId));
        }

        return this.returnResult('', 'some sort of error'); // todo
    }
}
