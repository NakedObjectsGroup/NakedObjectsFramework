import { Dictionary } from 'lodash';
import forEach from 'lodash-es/forEach';
import fromPairs from 'lodash-es/fromPairs';
import keys from 'lodash-es/keys';
import last from 'lodash-es/last';
import map from 'lodash-es/map';
import mapKeys from 'lodash-es/mapKeys';
import mapValues from 'lodash-es/mapValues';
import reduce from 'lodash-es/reduce';
import { Command } from './Command';
import * as Commandresult from './command-result';
import { CommandResult } from './command-result';
import * as Constants from '../constants';
import * as Models from '../models';
import * as Usermessages from '../user-messages';
import * as Validate from '../validate';

export class Enter extends Command {

    shortCommand = "en";
    fullCommand = Usermessages.enterCommand;
    helpText = Usermessages.enterHelp;
    protected minArguments = 2;
    protected maxArguments = 2;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog() || this.isEdit() || this.isTransient() || this.isForm();
    }

    doExecute(args: string | null, chained: boolean): Promise<CommandResult> {
        const fieldName = this.argumentAsString(args, 0);
        const fieldEntry = this.argumentAsString(args, 1, false, false);

        if (fieldName === undefined) {
            return this.returnResult("", Usermessages.doesNotMatchDialog(fieldName));
        }

        if (fieldEntry === undefined) {
            return this.returnResult("", Usermessages.tooFewArguments);
        }

        if (this.isDialog()) {
            return this.fieldEntryForDialog(fieldName, fieldEntry);
        } else {
            return this.fieldEntryForEdit(fieldName, fieldEntry);
        }
    }

    private fieldEntryForEdit(fieldName: string | undefined, fieldEntry: string) {
        return this.getObject().then(obj => {
            const fields = this.matchingProperties(obj, fieldName);

            switch (fields.length) {
                case 0:
                    const s = Usermessages.doesNotMatchProperties(fieldName);
                    return this.returnResult("", s);
                case 1:
                    const field = fields[0];
                    if (fieldEntry === "?") {
                        // TODO: does this work in edit mode i.e. show entered value
                        const details = this.renderFieldDetails(field, field.value());
                        return this.returnResult("", details);
                    } else {
                        this.findAndClearAnyDependentFields(field.id(), obj.propertyMembers());
                        return this.setField(field, fieldEntry);
                    }
                default:
                    const ss = reduce(fields, (str, prop) => str + prop.extensions().friendlyName() + "\n", `${fieldName} ${Usermessages.matchesMultiple}`);
                    return this.returnResult("", ss);
            }
        });
    }

    private isDependentField(fieldName: string, possibleDependent: Models.IField): boolean {
        const promptLink = possibleDependent.promptLink();

        if (promptLink) {
            const pArgs = promptLink.arguments();
            const argNames = keys(pArgs);

            return (argNames.indexOf(fieldName.toLowerCase()) >= 0);
        }
        return false;
    }

    private findAndClearAnyDependentFields(changingField: string, allFields: Dictionary<Models.IField>) {

        forEach(allFields, field => {
            if (this.isDependentField(changingField, field)) {
                if (!this.isMultiChoiceField(field)) {
                    this.clearField(field);
                }
            }
        });
    }

    private fieldEntryForDialog(fieldName: string, fieldEntry: string) {
        return this.getActionForCurrentDialog().then(action => {
            //
            let params = map(action.parameters(), param => param);
            params = this.matchFriendlyNameAndOrMenuPath(params, fieldName);
            switch (params.length) {
                case 0:
                    return this.returnResult("", Usermessages.doesNotMatchDialog(fieldName));
                case 1:
                    if (fieldEntry === "?") {
                        const p = params[0];
                        const value = Commandresult.getParametersAndCurrentValue(p.parent, this.context)[p.id()];
                        const s = this.renderFieldDetails(p, value);
                        return this.returnResult("", s);
                    } else {
                        this.findAndClearAnyDependentFields(fieldName, action.parameters());
                        return this.setField(params[0], fieldEntry);
                    }
                default:
                    return this.returnResult("", `${Usermessages.multipleFieldMatches} ${fieldName}`); // TODO: list them
            }
        });
    }

    private clearField(field: Models.IField): void {
        this.context.cacheFieldValue(this.routeData().dialogId, field.id(), new Models.Value(null));

        if (field instanceof Models.Parameter) {
            this.context.cacheFieldValue(this.routeData().dialogId, field.id(), new Models.Value(null));
        } else if (field instanceof Models.PropertyMember) {
            const parent = field.parent as Models.DomainObjectRepresentation;
            this.context.cachePropertyValue(parent, field, new Models.Value(null));
        }
    }

    private setField(field: Models.IField, fieldEntry: string) {
        if (field instanceof Models.PropertyMember && field.disabledReason()) {
            return this.returnResult("", `${field.extensions().friendlyName()} ${Usermessages.isNotModifiable}`);
        }
        const entryType = field.entryType();
        switch (entryType) {
            case Models.EntryType.FreeForm:
                return this.handleFreeForm(field, fieldEntry);
            case Models.EntryType.AutoComplete:
                return this.handleAutoComplete(field, fieldEntry);
            case Models.EntryType.Choices:
                return this.handleChoices(field, fieldEntry);
            case Models.EntryType.MultipleChoices:
                return this.handleChoices(field, fieldEntry);
            case Models.EntryType.ConditionalChoices:
                return this.handleConditionalChoices(field, false, fieldEntry);
            case Models.EntryType.MultipleConditionalChoices:
                return this.handleConditionalChoices(field, false, fieldEntry);
            default:
                return this.returnResult("", Usermessages.invalidCase);
        }
    }

    private handleFreeForm(field: Models.IField, fieldEntry: string) {
        if (field.isScalar()) {

            const mandatoryError = Validate.validateMandatory(field, fieldEntry);

            if (mandatoryError) {
                return this.returnResult("", this.validationMessage(mandatoryError, new Models.Value(""), field.extensions().friendlyName()));
            }

            let value = new Models.Value(fieldEntry);
            if (Models.isDateOrDateTime(field)) {
                const dt = Validate.validateDate(fieldEntry, Constants.supportedDateFormats);

                if (dt) {
                    value = new Models.Value(Models.toDateString(dt.toDate()));
                }
            }

            // if optional but empty always valid
            if (fieldEntry != null && fieldEntry !== "") {

                const remoteMask = field.extensions().mask();
                const localFilter = this.mask.toLocalFilter(remoteMask, field.extensions().format()!);

                const validateError = Validate.validateMandatoryAgainstType(field, fieldEntry, localFilter);

                if (validateError) {
                    return this.returnResult("", this.validationMessage(validateError, value, field.extensions().friendlyName()));
                }
            }

            this.setFieldValue(field, value);
            return this.returnResult("", "", () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl());
        } else {
            return this.handleReferenceField(field, fieldEntry);
        }
    }

    private setFieldValue(field: Models.IField, value: Models.Value): void {
        const urlVal = this.valueForUrl(value, field);
        if (urlVal != null) {
            if (field instanceof Models.Parameter) {
                this.setFieldValueInContext(field, urlVal);
            } else if (field instanceof Models.PropertyMember) {
                const parent = field.parent;
                if (parent instanceof Models.DomainObjectRepresentation) {
                    this.setPropertyValueinContext(parent, field, urlVal);
                }
            }
        }
    }

    private handleReferenceField(field: Models.IField, fieldEntry: string) {
        if (this.isPaste(fieldEntry)) {
            return this.handleClipboard(field);
        } else {
            return this.returnResult("", Usermessages.invalidRefEntry);
        }
    }

    private isPaste(fieldEntry: string) {
        return "paste".indexOf(fieldEntry) === 0;
    }

    private handleClipboard(field: Models.IField) {
        const ref = this.ciceroContext.ciceroClipboard;
        if (!ref) {
            return this.returnResult("", Usermessages.emptyClipboard);
        }
        const paramType = field.extensions().returnType()!;
        const refType = ref.domainType();
        return this.context.isSubTypeOf(refType, paramType).then(isSubType => {
            if (isSubType) {
                const obj = this.ciceroContext.ciceroClipboard as any;
                const selfLink = obj.selfLink();
                // Need to add a title to the SelfLink as not there by default
                selfLink.setTitle(obj.title());
                const value = new Models.Value(selfLink);
                this.setFieldValue(field, value);
                return this.returnResult("", "", () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl());
            } else {
                return this.returnResult("", Usermessages.incompatibleClipboard);
            }
        });
    }

    private handleAutoComplete(field: Models.IField, fieldEntry: string) {
        // TODO: Need to check that the minimum number of characters has been entered or fail validation
        if (!field.isScalar() && this.isPaste(fieldEntry)) {
            return this.handleClipboard(field);
        } else {
            return this.context.autoComplete(field, field.id(), () => ({}), fieldEntry).then((choices: Dictionary<Models.Value>) => {
                const matches = this.findMatchingChoicesForRef(choices, fieldEntry);
                const allFields = Commandresult.getFields(field);
                return this.switchOnMatches(field, allFields, fieldEntry, matches);
            });
        }
    }

    private handleChoices(field: Models.IField,  fieldEntry: string) {
        let matches: Models.Value[];
        if (field.isScalar()) {
            matches = this.findMatchingChoicesForScalar(field.choices(), fieldEntry);
        } else {
            matches = this.findMatchingChoicesForRef(field.choices(), fieldEntry);
        }
        const allFields = Commandresult.getFields(field);
        return this.switchOnMatches(field, allFields, fieldEntry, matches);
    }

    private updateDependentField(field: Models.IField): Promise<CommandResult> {
        return this.handleConditionalChoices(field, true);
    }

    private setFieldAndCheckDependencies(field: Models.IField, allFields: Models.IField[], match: Models.Value): Promise<CommandResult[]> {
        this.setFieldValue(field, match);
        const promises: Promise<CommandResult>[] = [];

        // find any dependent multi choice fields and update
        // non multi choice we will have just cleared
        forEach(allFields, depField => {
            if (this.isMultiChoiceField(depField)) {
                if (this.isDependentField(field.id().toLowerCase(), depField)) {
                    promises.push(this.updateDependentField(depField));
                }
            }
        });

        promises.push(this.returnResult("", "", () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl()));
        return Promise.all(promises);
    }

    private switchOnMatches(field: Models.IField, allFields: Models.IField[], fieldEntry: string, matches: Models.Value[]) {
        switch (matches.length) {
            case 0:
                return this.returnResult("", Usermessages.noMatch(fieldEntry));
            case 1:
                // TODO fix "!""
                return this.setFieldAndCheckDependencies(field, allFields, matches[0]).then((crs: CommandResult[]) => last(crs)!);
            default:
                let msg = Usermessages.multipleMatches;
                forEach(matches, m => msg += m.toString() + "\n");
                return this.returnResult("", msg);
        }
    }

    private getPropertiesAndCurrentValue(obj: Models.DomainObjectRepresentation): Dictionary<Models.Value> {
        const props = obj.propertyMembers();
        const values = mapValues(props, p => p.value());
        const modifiedProps = this.context.getObjectCachedValues(obj.id());

        forEach(values,  (v, k) => {
                const newValue = modifiedProps[k];
                if (newValue) {
                    values[k] = newValue;
                }
            });
        return mapKeys(values, (v, k) => k.toLowerCase());
    }

     private updateOnMatches(field: Models.IField, allFields: Models.IField[], fieldEntry: string, matches: Models.Value[]) {
        switch (matches.length) {
            case 0:
            case 1:
                const match = matches.length === 0 ? new Models.Value(null) : matches[0];
                // TODO fix "!""
                return this.setFieldAndCheckDependencies(field, allFields, match).then((crs: CommandResult[]) => last(crs)!);
            default:
                // shouldn't happen - ignore
                return this.returnResult("", "");
        }
    }

    private handleConditionalChoices(field: Models.IField, updating: boolean, fieldEntry?: string, ) {
        let enteredFields: Dictionary<Models.Value>;
        const allFields = Commandresult.getFields(field);

        if (field instanceof Models.Parameter) {
           enteredFields = Commandresult.getParametersAndCurrentValue(field.parent, this.context);
        }

        if (field instanceof Models.PropertyMember) {
            enteredFields = this.getPropertiesAndCurrentValue(field.parent as Models.DomainObjectRepresentation);
        }

        // TODO fix this any cast
        const args = fromPairs(map(field.promptLink()!.arguments()! as any, (v: any, key: string) => [key, new Models.Value(v.value)])) as Dictionary<Models.Value>;
        forEach(keys(args), key => args[key] = enteredFields[key]);

        let fieldEntryOrExistingValue: string;

        if (fieldEntry === undefined) {
            const def = args[field.id()];
            fieldEntryOrExistingValue = def ? def.toValueString() : "";
        } else {
            fieldEntryOrExistingValue = fieldEntry;
        }

        return this.context.conditionalChoices(field, field.id(), () => ({}), args).then((choices: Dictionary<Models.Value>) => {
            const matches = this.findMatchingChoicesForRef(choices, fieldEntryOrExistingValue);

            if (updating) {
                return this.updateOnMatches(field, allFields, fieldEntryOrExistingValue, matches);
            }

            return this.switchOnMatches(field, allFields, fieldEntryOrExistingValue, matches);
        });
    }

    private renderFieldDetails(field: Models.IField, value: Models.Value): string {

        const fieldName = Usermessages.fieldName(field.extensions().friendlyName());
        const desc = field.extensions().description();
        const descAndPrefix = desc ? `\n${Usermessages.descriptionFieldPrefix} ${desc}` : "";
        const types = `\n${Usermessages.typePrefix} ${Models.friendlyTypeName(field.extensions().returnType()!)}`;

        let postFix = "";
        if (field instanceof Models.PropertyMember && field.disabledReason()) {
            postFix = `\n${Usermessages.unModifiablePrefix(field.disabledReason())}`;
        } else {
            postFix = field.extensions().optional() ? `\n${Usermessages.optional}` : `\n${Usermessages.mandatory}`;
            const choices = field.choices();
            if (choices) {
                const label = `\n${Usermessages.choices}: `;
                const labelAndChoices = reduce(choices, (ss, cho) => ss + cho + " ", label);
                postFix = `${postFix}${labelAndChoices}`;
            }
        }
        return `${fieldName}${descAndPrefix}${types}${postFix}`;
    }
}
