import * as Rointerfaces from '../ro-interfaces';
import * as Choiceviewmodel from './choice-view-model';

export interface IDraggableViewModel {
    value: Rointerfaces.scalarValueType | Date;
    reference: string;
    selectedChoice: Choiceviewmodel.ChoiceViewModel;
    color: string;
    draggableType: string;

    draggableTitle: () => string;

    canDropOn: (targetType: string) => Promise<boolean>;
}