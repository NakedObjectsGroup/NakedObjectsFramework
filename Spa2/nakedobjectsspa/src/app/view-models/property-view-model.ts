import * as Fieldviewmodel from './field-view-model';
import * as Idraggableviewmodel from './idraggable-view-model';
import * as Models from '../models';
import * as Colorservice from '../color.service';
import * as Errorservice from '../error.service';
import * as Attachmentviewmodel from './attachment-view-model';

export class PropertyViewModel extends Fieldviewmodel.FieldViewModel implements Idraggableviewmodel.IDraggableViewModel {

    constructor(propertyRep: Models.PropertyMember, color: Colorservice.ColorService, error: Errorservice.ErrorService) {
        super(propertyRep.extensions(), color, error);
        this.draggableType = propertyRep.extensions().returnType();

        this.propertyRep = propertyRep;
        this.entryType = propertyRep.entryType();
        this.isEditable = !propertyRep.disabledReason();
        this.entryType = propertyRep.entryType();
    }


    propertyRep: Models.PropertyMember;
    isEditable: boolean;
    attachment: Attachmentviewmodel.AttachmentViewModel;
    refType: "null" | "navigable" | "notNavigable";
    isDirty: () => boolean;
    doClick: (right?: boolean) => void;

    // IDraggableViewModel
    draggableType: string;
    draggableTitle = () => this.formattedValue;

    canDropOn: (targetType: string) => Promise<boolean>;
}
