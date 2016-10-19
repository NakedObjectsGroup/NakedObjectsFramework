import { Component, Input, ElementRef } from '@angular/core';
import * as _ from "lodash";
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { AbstractDroppableComponent } from '../abstract-droppable/abstract-droppable.component';
import * as Msg from "../user-messages";

@Component({
    selector: 'autocomplete',
    host: {
        '(document:click)': 'handleClick($event)'
    },
    templateUrl: './auto-complete.component.html',
    styleUrls: ['./auto-complete.component.css']
})

export class AutoCompleteComponent extends AbstractDroppableComponent {


    filteredList: ViewModels.ChoiceViewModel[] = [];
    elementRef: ElementRef;

    constructor(myElement: ElementRef) {
        super();
        this.elementRef = myElement;
    }

    field: ViewModels.ValueViewModel;
    currentOptions: ViewModels.ChoiceViewModel[] = [];
    pArgs: _.Dictionary<Models.Value>;

    paneId: number;

    @Input('viewModel')
    set viewModel(vm: ViewModels.ValueViewModel) {
        this.field = vm;
        this.droppable = vm;
        this.paneId = this.field.onPaneId;
    }

    @Input('parent')
    parent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel;


    filter($event: any) {
        const input = $event.target.value as string;

        if (input.length >= this.field.minLength) {
            this.field.prompt(input).
                then((cvms: ViewModels.ChoiceViewModel[]) => {
                    this.filteredList = cvms;
                }).
                catch(() => { });
        }

        this.field.setMessage(Msg.pendingAutoComplete);
        this.field.clientValid = false;

    }

    select(item: ViewModels.ChoiceViewModel) {
        this.filteredList = [];
        this.field.selectedChoice = item;

        this.field.resetMessage();
        this.field.clientValid = true;
    }

    private isInside(clickedComponent: any): boolean {
        if (clickedComponent) {
            return clickedComponent === this.elementRef.nativeElement || this.isInside(clickedComponent.parentNode);
        }
        return false;
    }


    handleClick(event: any) {
        if (!this.isInside(event.target)) {
            this.filteredList = [];
        }
    }

    classes(): string {
        return `${this.field.color}${this.canDrop ? " candrop" : ""}`;
    }
}