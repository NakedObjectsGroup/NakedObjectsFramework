import { FieldViewModel } from './field-view-model';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import * as Models from '../models';

export class ParameterViewModel extends FieldViewModel {

    constructor(parmRep: Models.Parameter, paneId: number, color: ColorService, error: ErrorService) {
        super(parmRep.extensions(), color, error);
        this.parameterRep = parmRep;
        this.onPaneId = paneId;
        this.type = parmRep.isScalar() ? "scalar" : "ref";
        this.dflt = parmRep.default().toString();
        this.id = parmRep.id();
        this.argId = `${this.id.toLowerCase()}`;
        this.paneArgId = `${this.argId}${this.onPaneId}`;
        this.isCollectionContributed = parmRep.isCollectionContributed();
        this.entryType = parmRep.entryType();
        this.value = null;
    }

    parameterRep: Models.Parameter;
    dflt: string;
}