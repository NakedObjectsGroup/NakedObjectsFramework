import { Injectable } from '@angular/core';
import {CiceroViewModel} from './view-models/cicero-view-model';
import {Command} from './cicero-commands';
import * as Cmd from './cicero-commands';
import * as Msg from './user-messages';
import {ContextService} from './context.service';
import {UrlManagerService} from './url-manager.service';
import {MaskService} from './mask.service';
import {ErrorService} from './error.service';
import {ConfigService, IAppConfig} from './config.service';
import {Location} from '@angular/common';
import * as _ from 'lodash';

@Injectable()
export class CiceroCommandFactoryService {

  constructor(protected urlManager: UrlManagerService,
            protected location: Location,
            protected context: ContextService,
            protected mask: MaskService,
            protected error: ErrorService,
            protected configService: ConfigService) { }

 private commandsInitialised = false;

        private commands: _.Dictionary<Command> = {
            "ac": new Cmd.Action(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "ba": new Cmd.Back(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "ca": new Cmd.Cancel(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "cl": new Cmd.Clipboard(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "ed": new Cmd.Edit(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "en": new Cmd.Enter(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "fo": new Cmd.Forward(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "ge": new Cmd.Gemini(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "go": new Cmd.Goto(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "he": new Cmd.Help(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "me": new Cmd.Menu(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "ok": new Cmd.OK(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "pa": new Cmd.Page(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "re": new Cmd.Reload(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "ro": new Cmd.Root(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "sa": new Cmd.Save(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "se": new Cmd.Selection(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "sh": new Cmd.Show(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
            "wh": new Cmd.Where(this.urlManager, this.location, this, this.context, this.mask,this.error, this.configService),
        };


        parseInput(input: string, cvm: CiceroViewModel)  {
            //TODO: sort out whether to process CVMs here, or in the execute method  -  is this ALWAYS called first?
            //Also, must not modify the *input* one here
            cvm.chainedCommands = null; //TODO: Maybe not needed if unexecuted commands are cleared down upon error?
            if (!input) { //Special case for hitting Enter with no input
                this.getCommand("wh").execute(null, false, cvm);
                return;
            }
            this.autoComplete(input, cvm);
            cvm.input = cvm.input!.trim();
            cvm.previousInput = cvm.input!;
            const commands = input.split(";");
            if (commands.length > 1) {
                const first = commands[0];
                commands.splice(0, 1);
                cvm.chainedCommands = commands;
                this.processSingleCommand(first, cvm, false);
            } else {
                this.processSingleCommand(input, cvm, false);
            }
        };

        processSingleCommand = (input: string, cvm: CiceroViewModel, chained: boolean) => {
            try {
                input = input.trim();
                const firstWord = input.split(" ")[0].toLowerCase();
                const command = this.getCommand(firstWord);
                let argString: string | null = null;
                const index = input.indexOf(" ");
                if (index >= 0) {
                    argString = input.substr(index + 1);
                }
                command.execute(argString, chained, cvm);
            } catch (e) {
                cvm.setOutputSource(e.message);
                cvm.input = "";
            }
        };

        //TODO: change the name & functionality to pre-parse or somesuch as could do more than auto
        //complete e.g. reject unrecognised action or one not available in context.
        autoComplete = (input: string, cvm: CiceroViewModel) => {
            if (!input) return;
            let lastInChain = _.last(input.split(";")).toLowerCase();
            const charsTyped = lastInChain.length;
            lastInChain = lastInChain.trim();
            if (lastInChain.length === 0 || lastInChain.indexOf(" ") >= 0) { //i.e. not the first word
                cvm.input += " ";
                return;
            }
            try {
                const command = this.getCommand(lastInChain);
                const earlierChain = input.substr(0, input.length - charsTyped);
                cvm.input = earlierChain + command.fullCommand + " ";
            } catch (e) {
                cvm.setOutputSource(e.message);
            }
        };

        getCommand = (commandWord: string) => {
            if (commandWord.length < 2) {
                throw new Error(Msg.commandTooShort);
            }
            const abbr = commandWord.substr(0, 2);
            const command = this.commands[abbr];
            if (command == null) {
                throw new Error(Msg.noCommandMatch(abbr));
            }
            command.checkMatch(commandWord);
            return command;
        };

        allCommandsForCurrentContext = () => {
            const commandsInContext = _.filter(this.commands, c => c.isAvailableInCurrentContext());
            return _.reduce(commandsInContext, (r, c) => r + c.fullCommand + "\n" , Msg.commandsAvailable);
        };

}
