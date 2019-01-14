import { InteractionMode } from '@nakedobjects/services';
import { Command } from './Command';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';

export class Edit extends Command {

    shortCommand = 'ed';
    fullCommand = Usermessages.editCommand;
    helpText = Usermessages.editHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() && !this.isEdit();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        if (chained) {
            return this.returnResult('', this.mayNotBeChained(), () => { }, true);
        }
        const newState = () => {
            this.context.clearObjectCachedValues();
            this.urlManager.setInteractionMode(InteractionMode.Edit);
        };

        return this.returnResult('', '', newState);
    }
}
