import * as Fieldviewmodel from './field-view-model';
import * as Usermessages from '../user-messages';
import * as _ from "lodash";

export function tooltip(onWhat: { clientValid: () => boolean }, fields: Fieldviewmodel.FieldViewModel[]): string {
    if (onWhat.clientValid()) {
        return "";
    }

    const missingMandatoryFields = _.filter(fields, p => !p.clientValid && !p.getMessage());

    if (missingMandatoryFields.length > 0) {
        return _.reduce(missingMandatoryFields, (s, t) => s + t.title + "; ", Usermessages.mandatoryFieldsPrefix);
    }

    const invalidFields = _.filter(fields, p => !p.clientValid);

    if (invalidFields.length > 0) {
        return _.reduce(invalidFields, (s, t) => s + t.title + "; ", Usermessages.invalidFieldsPrefix);
    }

    return "";
}