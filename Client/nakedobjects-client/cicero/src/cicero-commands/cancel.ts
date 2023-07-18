import { InteractionMode } from '@nakedobjects/services';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Msg from '../user-messages';
import { Location } from '@angular/common';

export class Cancel extends Command {

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

    override shortCommand = 'ca';
    override fullCommand = Msg.cancelCommand;
    override helpText = Msg.cancelHelp;
    protected override minArguments = 0;
    protected override maxArguments = 0;

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
