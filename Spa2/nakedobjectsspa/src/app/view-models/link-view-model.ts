import { IDraggableViewModel } from './idraggable-view-model';
import { ChoiceViewModel } from './choice-view-model';
import * as Ro from "../ro-interfaces";
import * as Models from "../models";

export class LinkViewModel implements IDraggableViewModel {

    title: string;
    domainType: string;
    link: Models.Link;

    doClick: (right?: boolean) => void;

    // IDraggableViewModel 
    color: string;
    value: Ro.scalarValueType;
    reference: string;
    selectedChoice: ChoiceViewModel;
    draggableType: string;

    draggableTitle = () => this.title;

    canDropOn: (targetType: string) => Promise<boolean>;
}