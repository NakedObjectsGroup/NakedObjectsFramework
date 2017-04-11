import { Component, Input, ViewChildren, QueryList, ElementRef, Renderer } from '@angular/core';
import { ItemViewModel } from '../view-models/item-view-model';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import * as Helpers from '../view-models/helpers-view-models';
import { ContextService } from '../context.service';
import { PropertyViewModel } from '../view-models/property-view-model';
import { TableRowColumnViewModel } from '../view-models/table-row-column-view-model';
import { RecentItemViewModel } from '../view-models/recent-item-view-model';

@Component({
    selector: '[nof-row]',
    template: require('./row.component.html'),
    styles: [require('./row.component.css')]
})
export class RowComponent {

    constructor(
        private readonly context: ContextService,
        private readonly renderer: Renderer,
        private readonly element: ElementRef,
    ) { }

    @Input()
    item: ItemViewModel;

    @Input()
    index: number;

    @Input()
    withCheckbox: boolean;

    @Input()
    isTable: boolean;

    get id() {
        return `${this.item.id || "item"}${this.item.paneId}-${this.index}`;
    }

    get color() {
        return this.item.color;
    }

    get selected() {
        return this.item.selected;
    }

    get title() {
        return this.item.title;
    }

    get friendlyName() {
        return this.item instanceof RecentItemViewModel ? this.item.friendlyName : "";
    }

    tableTitle = () => this.item.tableRowViewModel ? this.item.tableRowViewModel.title : this.title;
    hasTableTitle = () => (this.item.tableRowViewModel && this.item.tableRowViewModel.showTitle) || (this.item instanceof RecentItemViewModel && this.item.title);
    tableProperties = () => this.item.tableRowViewModel && this.item.tableRowViewModel.properties;

    propertyType = (property: PropertyViewModel) => property.type;
    propertyValue = (property: PropertyViewModel) => property.value;
    propertyFormattedValue = (property: PropertyViewModel) => property.formattedValue;
    propertyReturnType = (property: PropertyViewModel) => property.returnType;

    doClick = (right?: boolean) => this.item.doClick(right);

    copy(event: KeyboardEvent, item: IDraggableViewModel) {
        Helpers.copy(event, item, this.context);
    }
    
    focus() {
        return !!this.element && Helpers.focus(this.renderer, this.element);
    }
}
