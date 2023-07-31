import { Injectable } from '@angular/core';
import { Command } from './cicero-commands/command';

@Injectable()
export class CiceroContextService {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    ciceroClipboard: any;
    chainedCommands?: Command[];
    nextChainedCommand?: Command;
}
