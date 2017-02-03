import { ChoiceViewModel } from './choice-view-model';
import * as Ro from '../ro-interfaces';
import { ContextService} from '../context.service';

export interface IDraggableViewModel {
    value: Ro.scalarValueType | Date | null;
    reference: string;
    selectedChoice: ChoiceViewModel | null;
    color: string;
    draggableType: string;
    draggableTitle: () => string;
    canDropOn: (targetType: string) => Promise<boolean>;
}