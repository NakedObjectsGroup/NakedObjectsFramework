import * as Ro from '../models';
import * as RtD from '../route-data';
import { Component, OnInit } from '@angular/core';
import {CiceroCommandFactoryService} from '../cicero-command-factory.service';
import {ViewModelFactoryService} from '../view-model-factory.service';
import {CiceroRendererService} from '../cicero-renderer.service';
import {CiceroViewModel} from '../view-models/cicero-view-model';

@Component({
  selector: 'nof-cicero',
  templateUrl: './cicero.component.html',
  styleUrls: ['./cicero.component.css']
})
export class CiceroComponent implements OnInit {

  constructor(
    protected commandFactory : CiceroCommandFactoryService,
    protected renderer : CiceroRendererService) 
    { 
    }

  ngOnInit() {
  }



   private inputText : string;
    private outputText: string;
    private message: string | null;
    private output: string | null;
    private alert = ""; //Alert is appended before the output
    private input: string | null;
    private viewType: RtD.ViewType;
    private clipboard: Ro.DomainObjectRepresentation;
    private previousInput: string;
    private chainedCommands: string[];
    private cvm: CiceroViewModel; //TODO: set up

      set inp(newValue: string) {
    this.inputText = newValue;
    //TODO: Act only on the four keys of Enter, up, down, and space (or tab?)
  }
  get inp() :string {
    return this.inputText;
  }

  set out(newValue: string) {
    this.outputText = newValue;
  }
  get out() : string {
    return this.outputText;
  }

    parseInput(input: string) {
      //TODO: Create new lightweight CVM, to instantiate
        return this.commandFactory.parseInput(input, this.cvm);
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

    renderHome(routeData: RtD.PaneRouteData): void {
      this.renderer.renderHome(this.cvm, routeData);
    }
    renderObject(routeData: RtD.PaneRouteData) : void {
      this.renderer.renderObject(this.cvm, routeData);
    }
    renderList(routeData: RtD.PaneRouteData) :void {
      this.renderer.renderList(this.cvm, routeData);
    }
    renderError() : void {
      this.renderer.renderError(this.cvm);
    }

     executeNextChainedCommandIfAny()  {
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
