import * as Ro from '@nakedobjects/restful-objects';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Selection extends Command {

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

    override shortCommand = 'se';
    override fullCommand = Usermessages.selectionCommand;
    override helpText = Usermessages.selectionHelp;
    protected override minArguments = 1;
    protected override maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isList();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        //    //TODO: Add in sub-commands: Add, Remove, All, Clear & Show
        //    const arg = this.argumentAsString(args, 0);
        //    const { start, end } = this.parseRange(arg); //'destructuring'
        //    this.getList().then(list => this.selectItems(list, start, end)).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        return Promise.reject('Not Implemented');
    }

    private selectItems(list: Ro.ListRepresentation, startNo: number, endNo: number): void {
        let itemNo: number;
        for (itemNo = startNo; itemNo <= endNo; itemNo++) {
            this.urlManager.setItemSelected(itemNo - 1, true, '');
        }
    }
}
