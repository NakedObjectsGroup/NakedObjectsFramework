import * as Cicerocommands from './command-result';
import * as Command from './Command';
import * as Usermessages from '../user-messages';
import * as Models from '../models';
import { Location } from '@angular/common';

export class Clipboard extends Command.Command {

    fullCommand = Usermessages.clipboardCommand;
    helpText = Usermessages.clipboardHelp;

    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

   

    doExecuteNew(args: string, chained: boolean): Promise<Cicerocommands.CommandResult> {
        const sub = this.argumentAsString(args, 0);
        if (Usermessages.clipboardCopy.indexOf(sub) === 0) {
            return this.copy();
        } else if (Usermessages.clipboardShow.indexOf(sub) === 0) {
            return this.show();
        } else if (Usermessages.clipboardGo.indexOf(sub) === 0) {
            return this.go();
        } else if (Usermessages.clipboardDiscard.indexOf(sub) === 0) {
            return this.discard();
        } else {
            return this.returnResult("", Usermessages.clipboardError);
        }
    };

    private copy(): Promise<Cicerocommands.CommandResult> {
        if (!this.isObject()) {         
            return this.returnResult("", Usermessages.clipboardContextError);
        }
        return this.getObject().then(obj => {
            this.ciceroContext.ciceroClipboard = obj;
            const label = Models.typePlusTitle(obj);
            return this.returnResult("", Usermessages.clipboardContents(label));
        });
    }

    private show(): Promise<Cicerocommands.CommandResult> {
        if (this.ciceroContext.ciceroClipboard) {
            const label = Models.typePlusTitle(this.ciceroContext.ciceroClipboard);
            return this.returnResult("", Usermessages.clipboardContents(label));
        } else {
           
            return this.returnResult("", Usermessages.clipboardEmpty);
        }
    }

    private go(): Promise<Cicerocommands.CommandResult>  {
        const link = this.ciceroContext.ciceroClipboard && this.ciceroContext.ciceroClipboard.selfLink();
        if (link) {
            //this.urlManager.setItem(link);
            return this.returnResult("", "", () => this.urlManager.setItem(link));
        } else {
            return this.show();
        }
    }

    private discard(): Promise<Cicerocommands.CommandResult> {
        this.ciceroContext.ciceroClipboard = null;
        return this.show();
    }
}