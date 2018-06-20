import { Injectable } from '@angular/core';
import { Command } from './cicero-commands/Command';
import * as Msg from './user-messages';
import { ContextService } from './context.service';
import { UrlManagerService } from './url-manager.service';
import { MaskService } from './mask.service';
import { ErrorService } from './error.service';
import { ConfigService } from './config.service';
import { Location } from '@angular/common';
import { CiceroContextService } from './cicero-context.service';
import { Action } from './cicero-commands/action';
import { Back } from './cicero-commands/back';
import { Cancel } from './cicero-commands/cancel';
import { Clipboard } from './cicero-commands/clipboard';
import { Edit } from './cicero-commands/edit';
import { Enter } from './cicero-commands/enter';
import { Forward } from './cicero-commands/forward';
import { Gemini } from './cicero-commands/gemini';
import { Goto } from './cicero-commands/goto';
import { Help } from './cicero-commands/help';
import { Menu } from './cicero-commands/menu';
import { OK } from './cicero-commands/ok';
import { Page } from './cicero-commands/page';
import { Reload } from './cicero-commands/reload';
import { Root } from './cicero-commands/root';
import { Save } from './cicero-commands/save';
import { Selection } from './cicero-commands/selection';
import { Show } from './cicero-commands/show';
import { Where } from './cicero-commands/where';
import { Result } from './cicero-commands/result';
import { Dictionary } from 'lodash';
import filter from 'lodash-es/filter';
import reduce from 'lodash-es/reduce';
import last from 'lodash-es/last';
import map from 'lodash-es/map';
import fromPairs from 'lodash-es/fromPairs';
import {CiceroRendererService} from './cicero-renderer.service';

export class ParseResult {
    commands?: Command[];
    error?: string;

    static create(commands: Command[]): ParseResult {
        return { commands: commands };
    }
    static createError(msg: string): ParseResult {
        return { error: msg };
    }
}

@Injectable()
export class CiceroCommandFactoryService {

    constructor(private readonly urlManager: UrlManagerService,
                private readonly location: Location,
                private readonly context: ContextService,
                private readonly mask: MaskService,
                private readonly error: ErrorService,
                private readonly configService: ConfigService,
                private readonly ciceroContext: CiceroContextService,
                private readonly ciceroRenderer: CiceroRendererService) { }

    private commandsInitialised = false;

    private commandTypes = [Action, Back, Cancel, Clipboard, Edit, Enter, Forward, Gemini, Goto, Help, Menu, OK, Page, Reload, Root, Save, Selection, Show, Where];

    private allCommands: Command[] = map(this.commandTypes, T => new T(this.urlManager, this.location, this, this.context, this.mask, this.error, this.configService, this.ciceroContext, this.ciceroRenderer));

    private commands: Dictionary<Command> = fromPairs(map(this.allCommands, c => [c.shortCommand, c]));

    private mapInputToCommands(input: string) {
        if (!input) {
            // Special case for hitting Enter with no input
            return [this.getCommand("wh")];
        }
        const commands = input.split(";");
        return map(commands, c => this.getSingleCommand(c, commands.length > 1));
    }

    getCommands(input: string): ParseResult {
        try {
            return ParseResult.create(this.mapInputToCommands(input));
        } catch (e) {
            return ParseResult.createError(e.message);
        }
    }

    getArgs(input: string) {
        const index = input.indexOf(" ");
        return index >= 0 ? input.substr(index + 1) : null;
    }

    getSingleCommand = (input: string, chained: boolean) => {

        input = input.trim();
        const [firstWord] = input.split(" ");
        const command = this.getCommand(firstWord);
        command.argString = this.getArgs(input);
        command.chained = chained;
        return command;
    }

    // TODO:  could do more than auto complete e.g. reject unrecognised action or one not available in context.
    preParse = (input: string): Result => {
        if (!input) {
            return Result.create(input, null);
        }
        let lastInChain = (last(input.split(";")) || "").toLowerCase();
        const charsTyped = lastInChain.length;
        lastInChain = lastInChain.trim();
        if (lastInChain.length === 0 || lastInChain.indexOf(" ") >= 0) { // i.e. not the first word
            return Result.create(`${input} `, null);
        }
        try {
            const command = this.getCommand(lastInChain);
            const earlierChain = input.substr(0, input.length - charsTyped);
            return Result.create(`${earlierChain}${command.fullCommand} `, null);
        } catch (e) {
            return Result.create("", e.message);
        }
    }

    getCommand = (commandWord: string) => {
        if (commandWord.length < 2) {
            throw new Error(Msg.commandTooShort);
        }
        commandWord = commandWord.toLowerCase();
        const abbr = commandWord.substr(0, 2);
        const command = this.commands[abbr];
        if (command == null) {
            throw new Error(Msg.noCommandMatch(abbr));
        }
        command.checkMatch(commandWord);
        return command;
    }

    allCommandsForCurrentContext = () => {
        const commandsInContext = filter(this.commands, c => c.isAvailableInCurrentContext());
        return reduce<Command, string>(commandsInContext, (r, c) => `${r}${c.fullCommand}\n`, Msg.commandsAvailable);
    }
}
