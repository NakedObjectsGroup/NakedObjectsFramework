import * as Ro from '../models';
import * as RtD from '../route-data';
import { Component, OnInit } from '@angular/core';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { ISubscription } from 'rxjs/Subscription';
import { PaneRouteData } from '../route-data'; //TODO trim
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { CiceroContextService } from '../cicero-context.service';
import { Command } from '../cicero-commands/Command';
import { Result } from '../cicero-commands/result';
import * as Contextservice from '../context.service';
import reduce from 'lodash/reduce';
import {safeUnsubscribe} from '../helpers-components';

@Component({
    selector: 'nof-cicero',
    template: require('./cicero.component.html'),
    styles: [require('./cicero.component.css')]
})
export class CiceroComponent implements OnInit {

    constructor(
        private readonly commandFactory: CiceroCommandFactoryService,
        private readonly renderer: CiceroRendererService,
        private readonly error: ErrorService,
        private readonly urlManager: UrlManagerService,
        private readonly ciceroContext: CiceroContextService,
        private readonly context : Contextservice.ContextService) {     
    }

    private warnings : string[];
    private messages : string[];

    private render() {
        switch (this.lastPaneRouteData.location) {
            case RtD.ViewType.Home:
                return this.renderer.renderHome(this.lastPaneRouteData);
            case RtD.ViewType.Object:
                return this.renderer.renderObject(this.lastPaneRouteData);
            case RtD.ViewType.List:
                return this.renderer.renderList(this.lastPaneRouteData);
            default:
                return this.renderer.renderError("unknown render error");
        }
    }

    ngOnInit() {
        if (!this.paneRouteDataSub) {
            this.paneRouteDataSub =
                this.urlManager.getPaneRouteDataObservable(1)
                    .subscribe((paneRouteData: PaneRouteData) => {
                        if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
                            this.lastPaneRouteData = paneRouteData;

                            this.render().
                                then(result => {
                                    this.writeInputOutput(result);
                                    this.executeCommands(this.ciceroContext.chainedCommands);
                                }).
                                catch((reject: Ro.ErrorWrapper) => {
                                    if (reject.category === Ro.ErrorCategory.ClientError && reject.clientErrorCode === Ro.ClientErrorCode.ExpiredTransient) {
                                        this.outputText = "The requested view of unsaved object details has expired.";
                                    } else {
                                        const display = (em: Ro.ErrorMap) => this.outputText = em.invalidReason() || em.warningMessage;
                                        this.error.handleErrorAndDisplayMessages(reject, display);
                                    }
                                });
                        }
                    });
        };

        this.warnSub = this.context.warning$.subscribe(ws => this.warnings = ws);
        this.errorSub = this.context.messages$.subscribe(ms => this.messages = ms);
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.paneRouteDataSub);
        safeUnsubscribe(this.warnSub);
        safeUnsubscribe(this.errorSub);
    }

    private paneRouteDataSub: ISubscription;
    private warnSub: ISubscription;
    private errorSub : ISubscription;

    private lastPaneRouteData: PaneRouteData;
  
    private previousInput: string;

    private executeCommand(cmd: Command) {
        cmd.execute().
            then(result => {
                this.writeInputOutput(result);
                result.changeState();
            }).
            catch((reject: Ro.ErrorWrapper) => {
                const display = (em: Ro.ErrorMap) => this.outputText =  em.invalidReason() || em.warningMessage;
                this.error.handleErrorAndDisplayMessages(reject, display);
            });
    }

    private executeCommands(cmds: Command[]) {
        if (cmds && cmds.length > 0) {
            const [cmd, ...chained] = cmds;
            this.ciceroContext.chainedCommands = chained;
            this.executeCommand(cmd);
        }
    }

    private writeInputOutput(result: Result) {
        if (result.input != null) {
            this.inputText = result.input;
        }

        if (result.output != null) {
            const warning = this.warnings && this.warnings.length > 0 ? reduce(this.warnings, (s, w) => `${s}${w}\n`, "Warning: ") : "";
            const messages = this.messages && this.messages.length > 0 ? reduce(this.messages, (s, w) => `${s}${w}\n`, "") : "";
            const prefix = `${warning}${messages}`;
            const output = result.output != null ? result.output : "";

            this.outputText = `${prefix}${output}`;
        }
    }

    // template API 
    inputText: string;
    outputText: string;

    parseInput(input: string): void {
        this.previousInput = this.commandFactory.autoComplete(input).input.trim();
        const parseResult = this.commandFactory.getCommands(input);

        if (parseResult.commands) {
            this.executeCommands(parseResult.commands);
        }
        else if (parseResult.error) {
            this.outputText = parseResult.error;
            this.inputText = "";
        }
    };

    selectPreviousInput = () => {
        setTimeout(() => this.inputText = this.previousInput);
    };

    clearInput = () => {
        this.inputText = "";
    };

    autocomplete(input: string): string {
        //TODO: recognise tab also?
        if (input.substring(input.length - 1) === " ") {
            input = input.substr(0, input.length - 1);
            const res = this.commandFactory.autoComplete(input);
            this.writeInputOutput(res);
            return res.input;
        }
        return input;
    };
}
