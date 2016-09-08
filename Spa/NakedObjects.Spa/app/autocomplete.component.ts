import { Component, Input, ElementRef } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as _ from "lodash";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import { GeminiDropDirective } from "./gemini-drop.directive";
import { GeminiBooleanDirective } from './gemini-boolean.directive';
import {GeminiConditionalChoicesDirective} from './gemini-conditional-choices.directive';


import "./rxjs-extensions";
import { Subject } from 'rxjs/Subject';


@Component({
    selector: 'autocomplete',
    host: {
        '(document:click)': 'handleClick($event)'
    },
    template: `     
           <input id="{{parameter.paneArgId}}" class="{{parameter.status}} value droppable" [ngClass]="parameter.color" placeholder="{{parameter.description}}" type="text" [(ngModel)]="parameter.selectedChoice" name="parameter.id" (keyup)="filter($event)" />   
            <div class="suggestions" *ngIf="filteredList.length > 0">
                <ul *ngFor="let item of filteredList" >
                    <li >
                        <a (click)="select(item)" >{{item.name}}</a>
                    </li>
                </ul>
            </div>   
        `
})

export class AutocompleteComponent {
    
  

    public filteredList: ViewModels.ChoiceViewModel[] = [];
    public elementRef : ElementRef;

    constructor(myElement: ElementRef) {
        this.elementRef = myElement;
    }

    parameter: ViewModels.ParameterViewModel;
    currentOptions: ViewModels.ChoiceViewModel[] = [];
    pArgs: _.Dictionary<Models.Value>;

    paneId: number;

    @Input('viewModel')
    set viewModel(vm: ViewModels.ParameterViewModel) {
        this.parameter = vm;
        //this.pArgs = _.omit(this.model.promptArguments, "x-ro-nof-members") as _.Dictionary<Models.Value>;
        this.paneId = this.parameter.onPaneId;
    }

    @Input('parent')
    parent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel;

    //ngOnInit(): void {

    //}


    filter($event : any) {
        const input = $event.target.value as string;

        if (input.length >= this.parameter.minLength) {
            this.parameter.prompt(input).
                then((cvms: ViewModels.ChoiceViewModel[]) => {
                    //response(_.map(cvms, cvm => ({ "label": cvm.name, "value": cvm })));
                    this.filteredList = cvms;
                }).
                catch(() => { });
        }
    }

    select(item : ViewModels.ChoiceViewModel) {
        //this.query = item;
        this.filteredList = [];
        this.parameter.selectedChoice = item;
    }

    handleClick(event : any) {
        var clickedComponent = event.target;
        var inside = false;
        do {
            if (clickedComponent === this.elementRef.nativeElement) {
                inside = true;
            }
            clickedComponent = clickedComponent.parentNode;
        } while (clickedComponent);
        if (!inside) {
            this.filteredList = [];
        }
    }
}