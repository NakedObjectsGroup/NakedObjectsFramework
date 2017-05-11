import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import * as Routedata from '../route-data';
import { Location } from '@angular/common';

export class Page extends Command.Command {
    fullCommand = Usermessages.pageCommand;
    helpText = Usermessages.pageHelp;
    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isList();
    }

 

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        const arg = this.argumentAsString(args, 0);
        return this.getList().then(listRep => {
            const numPages = listRep.pagination().numPages;
            const page = this.routeData().page;
            const pageSize = this.routeData().pageSize;
            if (Usermessages.pageFirst.indexOf(arg) === 0) {
                //this.setPage(1);
                return this.returnResult(null, null, () => this.setPage(1));
            } else if (Usermessages.pagePrevious.indexOf(arg) === 0) {
                if (page === 1) {
                    //this.clearInputAndSetMessage(Msg.alreadyOnFirst);
                    return this.returnResult("", Usermessages.alreadyOnFirst);
                } else {
                    //this.setPage(page - 1);
                    return this.returnResult(null, null, () => this.setPage(page - 1));
                }
            } else if (Usermessages.pageNext.indexOf(arg) === 0) {
                if (page === numPages) {
                    //this.clearInputAndSetMessage(Msg.alreadyOnLast);
                    return this.returnResult("", Usermessages.alreadyOnLast);
                } else {
                    //this.setPage(page + 1);
                    return this.returnResult(null, null, () => this.setPage(page + 1));
                }
            } else if (Usermessages.pageLast.indexOf(arg) === 0) {
                //this.setPage(numPages);
                return this.returnResult(null, null, () => this.setPage(numPages));
            } else {
                const number = parseInt(arg);
                if (isNaN(number)) {
                    //this.clearInputAndSetMessage(Msg.pageArgumentWrong);
                    return this.returnResult("", Usermessages.pageArgumentWrong);
                }
                if (number < 1 || number > numPages) {
                    //this.clearInputAndSetMessage(Msg.pageNumberWrong(numPages));
                    return this.returnResult("", Usermessages.pageNumberWrong(numPages));
                }
                //this.setPage(number);
                return this.returnResult(null, null, () => this.setPage(number));
            }
        });
    };

    private setPage(page: number) {
        const pageSize = this.routeData().pageSize;
        this.urlManager.setListPaging(page, pageSize, Routedata.CollectionViewState.List);
    }
}