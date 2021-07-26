﻿import { Component, ElementRef, Input, QueryList, ViewChildren } from '@angular/core';
import { copy, DragAndDropService, IDraggableViewModel, ItemViewModel, PropertyViewModel, RecentItemViewModel, TableRowColumnViewModel } from '@nakedobjects/view-models';
import { focus } from '../helpers-components';

@Component({
    // tslint:disable-next-line:component-selector
    selector: '[nof-row]',
    templateUrl: 'row.component.html',
    styleUrls: ['row.component.css']
})
export class RowComponent {

    constructor(
        private readonly dragAndDrop: DragAndDropService,
    ) { }

    @Input()
    item: ItemViewModel;

    @Input()
    row: number;

    @Input()
    withCheckbox: boolean;

    @Input()
    isTable: boolean;

    @ViewChildren('focus')
    rowChildren: QueryList<ElementRef>;

    get id() {
        return `${this.item.id || 'item'}${this.item.paneId}-${this.row}`;
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
        return this.item instanceof RecentItemViewModel ? this.item.friendlyName : '';
    }

    tabIndexFirstColumn(i: number | string) {
        if (this.isTable) {
            if (this.hasTableTitle()) {
                return i === 'title' ? 0 : -1;
            } else if (this.friendlyName) {
                return i === 'fname' ? 0 : -1;
            } else if (i === 0) {
                return 0;
            }
        }
        return -1;
    }

    tableTitle = () => this.item.tableRowViewModel ? this.item.tableRowViewModel.title : this.title;
    hasTableTitle = () => (this.item.tableRowViewModel && this.item.tableRowViewModel.showTitle) || (this.item instanceof RecentItemViewModel && this.item.title);
    tableProperties = (): TableRowColumnViewModel[] => this.item.tableRowViewModel && this.item.tableRowViewModel.properties;

    propertyType = (property: PropertyViewModel) => property.type;
    propertyValue = (property: PropertyViewModel) => property.value;
    propertyFormattedValue = (property: PropertyViewModel) => property.formattedValue;
    propertyReturnType = (property: PropertyViewModel) => property.returnType;

    doClick = (right?: boolean) => this.item.doClick(right);

    copy(event: KeyboardEvent, item: IDraggableViewModel) {
        copy(event, item, this.dragAndDrop);
    }

    focus() {
        return !!this.rowChildren && this.rowChildren.length > 0 && focus(this.rowChildren.first);
    }
}
