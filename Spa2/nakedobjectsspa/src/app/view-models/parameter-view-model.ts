import * as Fieldviewmodel from './field-view-model';
import * as Models from '../models';
import * as Colorservice from '../color.service';
import * as Errorservice from '../error.service';

export class ParameterViewModel extends Fieldviewmodel.FieldViewModel {

    constructor(parmRep: Models.Parameter, paneId: number, color: Colorservice.ColorService, error: Errorservice.ErrorService) {
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