import * as Models from '@nakedobjects/restful-objects';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Clipboard extends Command {

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

    override shortCommand = 'cl';
    override fullCommand = Usermessages.clipboardCommand;
    override helpText = Usermessages.clipboardHelp;

    protected override minArguments = 1;
    protected override maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string | null, _chained: boolean): Promise<CommandResult> {
        const sub = this.argumentAsString(args, 0);

        if (sub === undefined) {
            return this.returnResult('', Usermessages.clipboardError);
        }

        if (Usermessages.clipboardCopy.indexOf(sub) === 0) {
            return this.copy();
        } else if (Usermessages.clipboardShow.indexOf(sub) === 0) {
            return this.show();
        } else if (Usermessages.clipboardGo.indexOf(sub) === 0) {
            return this.go();
        } else if (Usermessages.clipboardDiscard.indexOf(sub) === 0) {
            return this.discard();
        } else {
            return this.returnResult('', Usermessages.clipboardError);
        }
    }

    private copy(): Promise<CommandResult> {
        if (!this.isObject()) {
            return this.returnResult('', Usermessages.clipboardContextError);
        }
        return this.getObject().then(obj => {
            this.ciceroContext.ciceroClipboard = obj;
            const label = Models.typePlusTitle(obj);
            return this.returnResult('', Usermessages.clipboardContents(label));
        });
    }

    private show(): Promise<CommandResult> {
        if (this.ciceroContext.ciceroClipboard) {
            const label = Models.typePlusTitle(this.ciceroContext.ciceroClipboard);
            return this.returnResult('', Usermessages.clipboardContents(label));
        } else {

            return this.returnResult('', Usermessages.clipboardEmpty);
        }
    }

    private go(): Promise<CommandResult> {
        const link = this.ciceroContext.ciceroClipboard && this.ciceroContext.ciceroClipboard.selfLink();
        if (link) {
            return this.returnResult('', '', () => this.urlManager.setItem(link));
        } else {
            return this.show();
        }
    }

    private discard(): Promise<CommandResult> {
        this.ciceroContext.ciceroClipboard = null;
        return this.show();
    }
}
