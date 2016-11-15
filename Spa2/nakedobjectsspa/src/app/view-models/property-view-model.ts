import { FieldViewModel } from './field-view-model';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { AttachmentViewModel } from './attachment-view-model';
import * as Models from '../models';

export class PropertyViewModel extends FieldViewModel {

    constructor(propertyRep: Models.PropertyMember, color: ColorService, error: ErrorService) {
        super(propertyRep.extensions(), color, error);
        this.draggableType = propertyRep.extensions().returnType();

        this.propertyRep = propertyRep;
        this.entryType = propertyRep.entryType();
        this.isEditable = !propertyRep.disabledReason();
        this.entryType = propertyRep.entryType();
    }

    propertyRep: Models.PropertyMember;
    isEditable: boolean;
    attachment: AttachmentViewModel;
    refType: "null" | "navigable" | "notNavigable";
    isDirty: () => boolean;
    doClick: (right?: boolean) => void;

    // IDraggableViewModel
    draggableType: string;
    draggableTitle = () => this.formattedValue;

    canDropOn: (targetType: string) => Promise<boolean>;
}