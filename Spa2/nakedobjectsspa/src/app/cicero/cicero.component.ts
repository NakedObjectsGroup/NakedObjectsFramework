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

@Component({
  selector: 'nof-cicero',
  templateUrl: './cicero.component.html',
  styleUrls: ['./cicero.component.css']
})
export class CiceroComponent implements OnInit {

  constructor(
    protected commandFactory: CiceroCommandFactoryService,
    protected renderer: CiceroRendererService,
    protected urlManager: UrlManagerService) {
    this.cvm = new CiceroViewModel();
  }

  ngOnInit() {
    if (!this.paneRouteDataSub) {
      this.paneRouteDataSub =
        this.urlManager.getPaneRouteDataObservable(1)
          .subscribe((paneRouteData: PaneRouteData) => {
            if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
              this.lastPaneRouteData = paneRouteData;
              switch (paneRouteData.location) {
                case RtD.ViewType.Home: {
                   this.renderer.renderHome(this.cvm, paneRouteData);
                  break;
                }
                case RtD.ViewType.Object: {
                  this.renderer.renderObject(this.cvm, paneRouteData);
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
            }
          });
    };
  }


  renderObject(routeData: RtD.PaneRouteData): void {
    
  }
  renderList(routeData: RtD.PaneRouteData): void {
    
  }
  renderError(): void {
    
  }


  private paneRouteDataSub: ISubscription;
  private lastPaneRouteData: PaneRouteData;
  private inputText: string;
  private outputText: string;
  private message: string | null;
  private output: string | null;
  private alert = ""; //Alert is appended before the output
  private input: string | null;
  private viewType: RtD.ViewType;
  private clipboard: Ro.DomainObjectRepresentation;
  private previousInput: string;
  private chainedCommands: string[];
  private cvm: CiceroViewModel;

  set inp(newValue: string) {
    this.inputText = newValue;
  }
  get inp(): string {
    return this.inputText;
  }

  set out(newValue: string) {
    this.outputText = newValue;
  }
  get out(): string {
    return this.outputText;
  }

  parseInput(input: string): void {
    this.cvm.input = input;
    this.commandFactory.parseInput(input, this.cvm);
    this.out = this.cvm.message;
  };

  selectPreviousInput = () => {
    this.input = this.previousInput;
  };
  clearInput = () => {
    this.input = null;
  };
  private autoComplete(input: string) {
    this.commandFactory.autoComplete(input, this.cvm);
  };
  private outputMessageThenClearIt() {
    this.out = this.message;
    this.message = null;
  }

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

  clearInputRenderOutputAndAppendAlertIfAny(output: string): void {
    this.clearInput();
    this.out = output;
    if (this.alert) {
      this.out += this.alert;
      this.alert = "";
    }
  }
}
