import * as Cicerocommands from './command-result';
import * as Models from '../models';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import { Location } from '@angular/common';

export class Selection extends Command.Command {

    fullCommand = Usermessages.selectionCommand;
    helpText = Usermessages.selectionHelp;
    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isList();
    }

    //doExecute(args: string, chained: boolean): void {
    //    //TODO: Add in sub-commands: Add, Remove, All, Clear & Show
    //    const arg = this.argumentAsString(args, 0);
    //    const { start, end } = this.parseRange(arg); //'destructuring'
    //    this.getList().then(list => this.selectItems(list, start, end)).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
    //};

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        return Promise.reject("Not Implemented");
    };

    private selectItems(list: Models.ListRepresentation, startNo: number, endNo: number): void {
        let itemNo: number;
        for (itemNo = startNo; itemNo <= endNo; itemNo++) {
            this.urlManager.setItemSelected(itemNo - 1, true, "");
        }
    }
}