import { ItemViewModel } from './item-view-model';
import { PaneRouteData, CollectionViewState } from '../route-data';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import * as Models from '../models';
import * as _ from "lodash";

export class CollectionViewModel {

    title: string;
    details: string;
    pluralName: string;
    color: string;
    mayHaveItems: boolean;
    editing: boolean;
    items: ItemViewModel[];
    header: string[];
    onPaneId: number;
    currentState: CollectionViewState;
    //requestedState: CollectionViewState;
    presentationHint: string;
    template: string;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    messages: string;
    collectionRep: Models.CollectionMember | Models.CollectionRepresentation;

    doSummary: () => void;
    doTable: () => void;
    doList: () => void;
    hasTableData: () => boolean;

    description = () => this.details.toString();
    refresh: (routeData: PaneRouteData, resetting: boolean) => void;

    disableActions = () => this.editing || !this.actions || this.actions.length === 0;
    allSelected = () => _.every(this.items, item => item.selected);

    selectAll() { }
}