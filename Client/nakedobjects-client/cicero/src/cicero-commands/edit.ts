import { InteractionMode } from '@nakedobjects/services';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Edit extends Command {

    constructor(urlManager: UrlManagerService,
        location: Location,
        commandFactory: CiceroCommandFactoryService,
        context: ContextService,
        mask: MaskService,
        error: ErrorService,
        configService: ConfigService,
        ciceroContext: CiceroContextService,
        ciceroRenderer: CiceroRendererService,
    ) {
        super(urlManager, location, commandFactory, context, mask, error, configService, ciceroContext, ciceroRenderer);
    }

    override shortCommand = 'ed';
    override fullCommand = Usermessages.editCommand;
    override helpText = Usermessages.editHelp;
    protected override minArguments = 0;
    protected override maxArguments = 0;

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
