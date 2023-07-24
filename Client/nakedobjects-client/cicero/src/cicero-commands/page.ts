import { CollectionViewState } from '@nakedobjects/services';
import { Command } from './command';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroContextService } from '../cicero-context.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { UrlManagerService, ContextService, MaskService, ErrorService, ConfigService } from '@nakedobjects/services';
import { CommandResult } from './command-result';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Page extends Command {

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

    override shortCommand = 'pa';
    override fullCommand = Usermessages.pageCommand;
    override helpText = Usermessages.pageHelp;
    protected override minArguments = 1;
    protected override maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isList();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        const arg = this.argumentAsString(args, 0);
        if (arg === undefined) {
            return this.returnResult('', Usermessages.pageArgumentWrong);
        }

        return this.getList().then(listRep => {
            const paginationData = listRep.pagination();

            if (!paginationData) {
                return this.returnResult('', Usermessages.cannotPage);
            }

            const numPages = paginationData.numPages;
            const page = this.routeData().page;
            if (Usermessages.pageFirst.indexOf(arg) === 0) {
                return this.returnResult(null, null, () => this.setPage(1));
            } else if (Usermessages.pagePrevious.indexOf(arg) === 0) {
                if (page === 1) {
                    return this.returnResult('', Usermessages.alreadyOnFirst);
                } else {
                    return this.returnResult(null, null, () => this.setPage(page! - 1));
                }
            } else if (Usermessages.pageNext.indexOf(arg) === 0) {
                if (page === numPages) {
                    return this.returnResult('', Usermessages.alreadyOnLast);
                } else {
                    return this.returnResult(null, null, () => this.setPage(page! + 1));
                }
            } else if (Usermessages.pageLast.indexOf(arg) === 0) {
                return this.returnResult(null, null, () => this.setPage(numPages));
            } else {
                const number = parseInt(arg, 10);
                if (isNaN(number)) {
                    return this.returnResult('', Usermessages.pageArgumentWrong);
                }
                if (number < 1 || number > numPages) {
                    return this.returnResult('', Usermessages.pageNumberWrong(numPages));
                }
                return this.returnResult(null, null, () => this.setPage(number));
            }
        });
    }

    private setPage(page: number) {
        const pageSize = this.routeData().pageSize;
        this.urlManager.setListPaging(page, pageSize!, CollectionViewState.List);
    }
}
