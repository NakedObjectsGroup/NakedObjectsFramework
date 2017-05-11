import { Injectable } from '@angular/core';
import * as Cicerocommands from './cicero-commands';

@Injectable()
export class CiceroContextService {

  
    public ciceroClipboard: any;

    public chainedCommands: Cicerocommands.Command[];
    public nextChainedCommand : Cicerocommands.Command;

    queueNextCommand() {
        if (this.chainedCommands && this.chainedCommands.length > 0) {
            const [cmd, ...cmds] = this.chainedCommands;
            this.nextChainedCommand = cmd;
            this.chainedCommands = cmds;
        }
    }
}
