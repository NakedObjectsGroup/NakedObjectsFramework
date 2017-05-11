import { Injectable } from '@angular/core';
import * as Cicerocommands from './cicero-commands/command-result';
import * as Command from './cicero-commands/Command';

@Injectable()
export class CiceroContextService {

  
    public ciceroClipboard: any;

    public chainedCommands: Command.Command[];
    public nextChainedCommand : Command.Command;

    queueNextCommand() {
        if (this.chainedCommands && this.chainedCommands.length > 0) {
            const [cmd, ...cmds] = this.chainedCommands;
            this.nextChainedCommand = cmd;
            this.chainedCommands = cmds;
        }
    }
}
