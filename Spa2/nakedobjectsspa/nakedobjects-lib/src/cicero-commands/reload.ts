import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';
import * as Models from '../models';

export class Reload extends Command {

    shortCommand = "re";
    fullCommand = Usermessages.reloadCommand;
    helpText = Usermessages.reloadHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isList();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {

        return  this.getObject()
            .then(o => this.context.reloadObject(1, o))
            .then((updatedObject: Models.DomainObjectRepresentation) => this.returnResult("", "", () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl() ));
    }
}
