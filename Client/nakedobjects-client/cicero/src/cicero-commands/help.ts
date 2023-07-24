import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';
import { messageFrom } from '../helpers-components';

export class Help extends Command {

    constructor(urlManager: UrlManagerService,
        location: Location,
        commandFactory: CiceroCommandFactoryService,
        context: ContextService,
        mask: MaskService,
        error: ErrorService,
        configService: ConfigService,
        ciceroContext: CiceroContextService,
        ciceroRenderer: CiceroRendererService,
    )  {
        super(urlManager, location, commandFactory, context, mask, error, configService, ciceroContext, ciceroRenderer);
    }

    override shortCommand = 'he';
    override fullCommand = Usermessages.helpCommand;
    override helpText = Usermessages.helpHelp;
    protected override minArguments = 0;
    protected override maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        const arg = this.argumentAsString(args, 0);
        if (!arg) {
            return this.returnResult('', Usermessages.basicHelp);
        } else if (arg === '?') {
            const commands = this.commandFactory.allCommandsForCurrentContext();
            return this.returnResult('', commands);
        } else {
            try {
                const c = this.commandFactory.getCommand(arg);
                return this.returnResult('', `${c.fullCommand} command:\n${c.helpText}`);
            } catch (e) {
                return this.returnResult('', messageFrom(e));
            }
        }
    }
}
