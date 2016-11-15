import { ChoiceViewModel } from './choice-view-model';
import * as Ro from '../ro-interfaces';

export interface IDraggableViewModel {
    value: Ro.scalarValueType | Date;
    reference: string;
    selectedChoice: ChoiceViewModel;
    color: string;
    draggableType: string;

    draggableTitle: () => string;

    canDropOn: (targetType: string) => Promise<boolean>;
}