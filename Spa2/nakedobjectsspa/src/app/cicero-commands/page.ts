import { CommandResult } from './command-result';
import { Command } from './Command';
import * as Usermessages from '../user-messages';
import * as Routedata from '../route-data';

export class Page extends Command {

    shortCommand = "pa";
    fullCommand = Usermessages.pageCommand;
    helpText = Usermessages.pageHelp;
    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isList();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        const arg = this.argumentAsString(args, 0);
        if (arg === undefined) {
            return this.returnResult("", Usermessages.pageArgumentWrong);
        }

        return this.getList().then(listRep => {
            const paginationData = listRep.pagination();

            if (!paginationData) {
                return this.returnResult("", Usermessages.cannotPage);
            }

            const numPages = paginationData.numPages;
            const page = this.routeData().page;
            if (Usermessages.pageFirst.indexOf(arg) === 0) {
                return this.returnResult(null, null, () => this.setPage(1));
            } else if (Usermessages.pagePrevious.indexOf(arg) === 0) {
                if (page === 1) {
                    return this.returnResult("", Usermessages.alreadyOnFirst);
                } else {
                    return this.returnResult(null, null, () => this.setPage(page - 1));
                }
            } else if (Usermessages.pageNext.indexOf(arg) === 0) {
                if (page === numPages) {
                    return this.returnResult("", Usermessages.alreadyOnLast);
                } else {
                    return this.returnResult(null, null, () => this.setPage(page + 1));
                }
            } else if (Usermessages.pageLast.indexOf(arg) === 0) {
                return this.returnResult(null, null, () => this.setPage(numPages));
            } else {
                const number = parseInt(arg, 10);
                if (isNaN(number)) {
                    return this.returnResult("", Usermessages.pageArgumentWrong);
                }
                if (number < 1 || number > numPages) {
                    return this.returnResult("", Usermessages.pageNumberWrong(numPages));
                }
                return this.returnResult(null, null, () => this.setPage(number));
            }
        });
    }

    private setPage(page: number) {
        const pageSize = this.routeData().pageSize;
        this.urlManager.setListPaging(page, pageSize, Routedata.CollectionViewState.List);
    }
}
