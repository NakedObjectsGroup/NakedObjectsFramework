import * as Ro from '@nakedobjects/restful-objects';
import { ChoiceViewModel } from './choice-view-model';

export interface IDraggableViewModel {
    value: Ro.ScalarValueType | Date | null;
    reference: string;
    selectedChoice: ChoiceViewModel | null;
    color: string;
    draggableType: string;
    draggableTitle: () => string;
    canDropOn: (targetType: string) => Promise<boolean>;
}
