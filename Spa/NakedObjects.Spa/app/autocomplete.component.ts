import { Component, Input, ElementRef } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as _ from "lodash";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import { GeminiBooleanDirective } from './gemini-boolean.directive';
import { GeminiConditionalChoicesDirective } from './gemini-conditional-choices.directive';
import { AbstractDroppableComponent } from './abstract-droppable.component';

import "./rxjs-extensions";
import { Subject } from 'rxjs/Subject';
import * as Geminicleardirective from './gemini-clear.directive';

@Component({
    selector: 'autocomplete',
    host: {
        '(document:click)': 'handleClick($event)'
    },
    directives: [Geminicleardirective.GeminiClearDirective],
    template: `     
           <input id="{{field.paneArgId}}" class="{{field.status}} value droppable" dnd-droppable [allowDrop]="accept"  (onDropSuccess)="drop($event.dragData)" [ngClass]="classes()" placeholder="{{field.description}}" type="text" [(ngModel)]="field.selectedChoice" name="field.id" (keyup)="filter($event)" [geminiClear]="field" />   
            <div class="suggestions" *ngIf="filteredList.length > 0">
                <ul *ngFor="let item of filteredList" >
                    <li >
                        <a (click)="select(item)" >{{item.name}}</a>
                    </li>
                </ul>
            </div>   
        `
})

export class AutoCompleteComponent extends AbstractDroppableComponent {
    
  
    public filteredList: ViewModels.ChoiceViewModel[] = [];
    public elementRef : ElementRef;

    constructor(myElement: ElementRef) {
        super();
        this.elementRef = myElement;
    }

    field: ViewModels.IFieldViewModel;
    currentOptions: ViewModels.ChoiceViewModel[] = [];
    pArgs: _.Dictionary<Models.Value>;

    paneId: number;

    @Input('viewModel')
    set viewModel(vm: ViewModels.IFieldViewModel) {
        this.field = vm;
        this.droppable = vm;
        //this.pArgs = _.omit(this.model.promptArguments, "x-ro-nof-members") as _.Dictionary<Models.Value>;
        this.paneId = this.field.onPaneId;
    }

    @Input('parent')
    parent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel;


    filter($event : any) {
        const input = $event.target.value as string;

        if (input.length >= this.field.minLength) {
            this.field.prompt(input).
                then((cvms: ViewModels.ChoiceViewModel[]) => {
                    this.filteredList = cvms;
                }).
                catch(() => { });
        }
    }

    select(item : ViewModels.ChoiceViewModel) {
        this.filteredList = [];
        this.field.selectedChoice = item;
    }

    private isInside(clickedComponent: any) : boolean {
        if (clickedComponent) {
            return clickedComponent === this.elementRef.nativeElement || this.isInside(clickedComponent.parentNode);
        }
        return false;
    }


    handleClick(event : any) {
        if (!this.isInside(event.target)) {
            this.filteredList = [];
        }
    }

    classes(): string {
        return `${this.field.color}${this.canDrop ? " candrop" : ""}`;
    }
}