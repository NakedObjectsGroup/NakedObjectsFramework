import { Injectable } from '@angular/core';
import { Command } from './cicero-commands/Command';

@Injectable()
export class CiceroContextService {


    public ciceroClipboard: any;

    public chainedCommands: Command[];
    public nextChainedCommand: Command;

    queueNextCommand() {
        if (this.chainedCommands && this.chainedCommands.length > 0) {
            const [cmd, ...cmds] = this.chainedCommands;
            this.nextChainedCommand = cmd;
            this.chainedCommands = cmds;
        }
    }
}
