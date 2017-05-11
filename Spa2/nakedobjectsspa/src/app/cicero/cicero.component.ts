import * as Ro from '../models';
import * as RtD from '../route-data';
import { Component, OnInit } from '@angular/core';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { ISubscription } from 'rxjs/Subscription';
import { PaneRouteData } from '../route-data'; //TODO trim
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import * as Usermessages from '../user-messages';
import each from 'lodash/each';
import { CiceroContextService } from '../cicero-context.service';
import { Command } from '../cicero-commands/Command';
import { Result } from '../cicero-commands/result';

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
        private readonly ciceroContext: CiceroContextService) {     
    }

    ngOnInit() {
        if (!this.paneRouteDataSub) {
            this.paneRouteDataSub =
                this.urlManager.getPaneRouteDataObservable(1)
                    .subscribe((paneRouteData: PaneRouteData) => {
                        if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
                            this.lastPaneRouteData = paneRouteData;

                            let renderResult: Promise<Result>;
                            switch (paneRouteData.location) {
                                case RtD.ViewType.Home: {
                                    renderResult = this.renderer.renderHome(paneRouteData);
                                    break;
                                }
                                case RtD.ViewType.Object: {
                                    renderResult = this.renderer.renderObject(paneRouteData);
                                    break;
                                }
                                case RtD.ViewType.List: {
                                    renderResult = this.renderer.renderList(paneRouteData);
                                    break;
                                }
                                default: {
                                    renderResult = this.renderer.renderError("");
                                    break;
                                }
                            }

                            renderResult.then(rr => {
                                this.writeInputOutput(rr);
                                this.executeCommands(this.ciceroContext.chainedCommands);

                            }).catch(reject => {
                                if (reject instanceof Ro.ErrorWrapper) {
                                    if (reject.category === Ro.ErrorCategory.ClientError && reject.clientErrorCode === Ro.ClientErrorCode.ExpiredTransient) {
                                        this.outputText = "The requested view of unsaved object details has expired.";

                                    } else {
                                        this.error.handleErrorAndDisplayMessages(reject, (em: Ro.ErrorMap) => {
                                            this.outputText = em.invalidReason();
                                        });
                                    }

                                }


                            });

                        }
                    });
        };
    }

    ngOnDestroy(): void {

        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    }

    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData: PaneRouteData;
  
    private previousInput: string;

    private fieldValidationMessage(errorValue: Ro.ErrorValue, fieldFriendlyName: () => string): string {
        let msg = "";
        const reason = errorValue.invalidReason;
        const value = errorValue.value;
        if (reason) {
            msg += `${fieldFriendlyName()}: `;
            if (reason === Usermessages.mandatory) {
                msg += Usermessages.required;
            } else {
                msg += `${value} ${reason}`;
            }
            msg += "\n";
        }
        return msg;
    }

    private handleErrorResponse(err: Ro.ErrorMap, getFriendlyName: (id: string) => string) {
        if (err.invalidReason()) {
            this.inputText = "";
            this.outputText = err.invalidReason();
            return;
        }
        let msg = Usermessages.pleaseCompleteOrCorrect;
        each(err.valuesMap(),
            (errorValue, fieldId) => {
                msg += this.fieldValidationMessage(errorValue, () => getFriendlyName(fieldId!));
            });


        this.inputText = "";
        this.outputText = msg;
    }


    private executeCommand(cmd: Command) {
        cmd.execute().then(r => {
            this.writeInputOutput(r);
            r.changeState();
        }).catch(e => {
            if (e instanceof Ro.ErrorWrapper) {
                const display = (em: Ro.ErrorMap) => {
                    const paramFriendlyName = (paramId: string) => "todo " + paramId; // Ro.friendlyNameForParam(action, paramId);
                    this.handleErrorResponse(em, paramFriendlyName);
                };
                this.error.handleErrorAndDisplayMessages(e, display);
            }

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
            this.outputText = result.output;
        }
    }

    // template API 
    inputText: string;
    outputText: string;

    parseInput(input: string): void {
        this.previousInput = this.commandFactory.autoComplete(input).input.trim();
        const parseResult = this.commandFactory.getCommandNew(input);

        if (parseResult.command) {
            this.executeCommands(parseResult.command);
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
