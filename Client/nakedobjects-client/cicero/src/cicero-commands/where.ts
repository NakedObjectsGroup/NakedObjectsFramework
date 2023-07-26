import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Where extends Command {

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

    override shortCommand = 'wh';
    override fullCommand = Usermessages.whereCommand;
    override helpText = Usermessages.whereHelp;
    protected override minArguments = 0;
    protected override maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(_args: string | null, _chained: boolean): Promise<CommandResult> {
        this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
        return this.returnResult(null, null);
    }
}
