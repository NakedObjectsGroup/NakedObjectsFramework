import * as Itemviewmodel from './item-view-model';
import * as Routedata from '../route-data';
import * as Actionviewmodel from './action-view-model';
import * as Menuitemviewmodel from './menu-item-view-model';
import * as Models from '../models';
import * as _ from "lodash";

export class CollectionViewModel {

    title: string;
    details: string;
    pluralName: string;
    color: string;
    mayHaveItems: boolean;
    editing: boolean;
    items: Itemviewmodel.ItemViewModel[];
    header: string[];
    onPaneId: number;
    currentState: Routedata.CollectionViewState;
    //requestedState: CollectionViewState;
    presentationHint: string;
    template: string;
    actions: Actionviewmodel.ActionViewModel[];
    menuItems: Menuitemviewmodel.MenuItemViewModel[];
    messages: string;
    collectionRep: Models.CollectionMember | Models.CollectionRepresentation;

    doSummary: () => void;
    doTable: () => void;
    doList: () => void;
    hasTableData: () => boolean;

    description = () => this.details.toString();
    refresh: (routeData: Routedata.PaneRouteData, resetting: boolean) => void;

    disableActions = () => this.editing || !this.actions || this.actions.length === 0;
    allSelected = () => _.every(this.items, item => item.selected);
    selectAll() { }
}