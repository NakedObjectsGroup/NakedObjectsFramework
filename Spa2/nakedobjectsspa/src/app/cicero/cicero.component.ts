import * as Ro from '../models';
import * as RtD from '../route-data';
import { Component, OnInit } from '@angular/core';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { CiceroRendererService } from '../cicero-renderer.service';
import { CiceroViewModel } from '../view-models/cicero-view-model';
import { ISubscription } from 'rxjs/Subscription';
import { RouteData, PaneRouteData, ICustomActivatedRouteData, PaneType, PaneName } from '../route-data'; //TODO trim
import { UrlManagerService } from '../url-manager.service';
import * as Cicerorendererservice from '../cicero-renderer.service';

@Component({
    selector: 'nof-cicero',
    template: require('./cicero.component.html'),
    styles: [require('./cicero.component.css')]
})
export class CiceroComponent implements OnInit {

    constructor(
        protected commandFactory: CiceroCommandFactoryService,
        protected renderer: CiceroRendererService,
        protected urlManager: UrlManagerService) {
        this.cvm = new CiceroViewModel();
    }

    ngOnInit() {
        //Set up subscriptions to observables on CiceroViewModel
        //TODO:  Message, and other props?

        if (!this.cvmSub) {
            this.cvmSub = this.cvm.output$.subscribe((op: string) => {
                this.outputText = op;
            });
        }

        if (!this.paneRouteDataSub) {
            this.paneRouteDataSub =
                this.urlManager.getPaneRouteDataObservable(1)
                    .subscribe((paneRouteData: PaneRouteData) => {
                        if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
                            this.lastPaneRouteData = paneRouteData;

                            let renderResult: Promise<Cicerorendererservice.RenderResult>;
                            switch (paneRouteData.location) {
                                case RtD.ViewType.Home: {
                                    renderResult = this.renderer.renderHome(this.cvm.message, paneRouteData);
                                    break;
                                }
                                case RtD.ViewType.Object: {
                                    renderResult = this.renderer.renderObject(this.cvm.message, paneRouteData);
                                    break;
                                }
                                case RtD.ViewType.List: {
                                    this.renderer.renderList(this.cvm, paneRouteData);
                                    break;
                                }
                                default: {
                                    this.renderer.renderError(this.cvm);
                                    break;
                                }
                            }

                            renderResult.then(rr => {
                                this.inputText = rr.input;
                                this.outputText = rr.output;
                            });

                        }
                    });
        };
    }

    ngOnDestroy(): void {
        if (this.cvmSub) {
            this.cvmSub.unsubscribe();
        }

        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    }

    private cvmSub: ISubscription;
    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData: PaneRouteData;
    private inputText: string;
    private outputText: string;
    private message: string | null;
    private output: string | null;
    private alert = ""; //Alert is appended before the output
    private viewType: RtD.ViewType;
    private clipboard: Ro.DomainObjectRepresentation;
    private previousInput: string;
    private chainedCommands: string[];
    private cvm: CiceroViewModel;

    parseInput(input: string): void {
        //this.cvm.input = input;
        //this.commandFactory.parseInput(input, this.cvm);
        ////TODO: check this  -  why not writing straight to output?
        //this.cvm.setOutputSource(this.cvm.message);

        const parseResult = this.commandFactory.getCommandNew(input);

        if (parseResult.command) {

            parseResult.command.executeNew().
                then(r => {
                    this.inputText = r.input;
                    this.outputText = r.output;
                    r.changeState();
                }).
                catch(e => this.outputText = e.toString());
        }
        else if (parseResult.error) {
            this.outputText = parseResult.error;
        }
    };

    selectPreviousInput = () => {
        this.inputText = this.previousInput;
    };

    clearInput = () => {
        this.inputText = null;
    };

    private ifSpaceThenAutoComplete(input: string) {
        //TODO: recognise tab also?
        if (input.substring(input.length - 1) == " ") {
            input = input.substr(0, input.length - 2);
            this.commandFactory.autoComplete(input, this.cvm);
        }
    };
    executeNextChainedCommandIfAny() {
        if (this.chainedCommands && this.chainedCommands.length > 0) {
            const next = this.popNextCommand();
            this.commandFactory.processSingleCommand(next, this.cvm, true);
        }
    };

    popNextCommand(): string | null {
        if (this.chainedCommands) {
            const next = this.chainedCommands[0];
            this.chainedCommands.splice(0, 1);
            return next;

        }
        return null;
    }
}
