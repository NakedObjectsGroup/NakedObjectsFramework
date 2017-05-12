import { Injectable } from '@angular/core';
import { Command } from './cicero-commands/Command';

@Injectable()
export class CiceroContextService {
    ciceroClipboard: any;
    chainedCommands: Command[];
    nextChainedCommand: Command;
}
