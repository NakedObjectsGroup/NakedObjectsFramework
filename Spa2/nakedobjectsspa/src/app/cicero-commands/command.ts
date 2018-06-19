import { Location } from '@angular/common';
import { CiceroContextService } from '../cicero-context.service';
import { CommandResult } from './command-result';
import * as Routedata from '../route-data';
import * as Models from '../models';
import * as Usermessages from '../user-messages';
import { UrlManagerService } from '../url-manager.service';
import { CiceroCommandFactoryService } from '../cicero-command-factory.service';
import { ContextService } from '../context.service';
import { MaskService } from '../mask.service';
import { ErrorService } from '../error.service';
import { ConfigService } from '../config.service';
import map from 'lodash-es/map';
import some from 'lodash-es/some';
import filter from 'lodash-es/filter';
import every from 'lodash-es/every';
import each from 'lodash-es/each';
import keys from 'lodash-es/keys';
import forEach from 'lodash-es/forEach';
import findIndex from 'lodash-es/findIndex';
import { Dictionary } from 'lodash';
import * as Commandresult from './command-result';
import { CiceroRendererService } from '../cicero-renderer.service';
import * as Rointerfaces from '../ro-interfaces';

export abstract class Command {

    constructor(protected urlManager: UrlManagerService,
        protected location: Location,
        protected commandFactory: CiceroCommandFactoryService,
        protected context: ContextService,
        protected mask: MaskService,
        protected error: ErrorService,
        protected configService: ConfigService,
        protected ciceroContext: CiceroContextService,
        protected ciceroRenderer: CiceroRendererService,
    ) {
        this.keySeparator = configService.config.keySeparator;
    }

    argString: string | null;
    chained: boolean;

    shortCommand: string;
    fullCommand: string;
    helpText: string;
    protected minArguments: number;
    protected maxArguments: number;
    protected keySeparator: string;

    execute(): Promise<CommandResult> {
        const result = new CommandResult();

        // TODO Create outgoing Vm and copy across values as needed
        if (!this.isAvailableInCurrentContext()) {
            return this.returnResult("", Usermessages.commandNotAvailable(this.fullCommand));
        }
        // TODO: This could be moved into a pre-parse method as it does not depend on context
        if (this.argString == null) {
            if (this.minArguments > 0) {
                return this.returnResult("", Usermessages.noArguments);
            }
        } else {
            const args = this.argString.split(",");
            if (args.length < this.minArguments) {

                return this.returnResult("", Usermessages.tooFewArguments);
            } else if (args.length > this.maxArguments) {
                return this.returnResult("", Usermessages.tooManyArguments);
            }
        }
        return this.doExecute(this.argString, this.chained, result);
    }

    protected returnResult(input: string | null, output: string | null, changeState?: () => void, stopChain?: boolean): Promise<CommandResult> {
        changeState = changeState ? changeState : () => { };
        return Promise.resolve({ input: input, output: output, changeState: changeState, stopChain: !!stopChain });
    }

    protected abstract doExecute(args: string | null, chained: boolean, result: CommandResult): Promise<CommandResult>;

    abstract isAvailableInCurrentContext(): boolean;

    protected mayNotBeChained(rider: string = "") {
        return Usermessages.mayNotbeChainedMessage(this.fullCommand, rider);
    }

    checkMatch(matchText: string): void {
        if (this.fullCommand.indexOf(matchText) !== 0) {
            throw new Error(Usermessages.noSuchCommand(matchText));
        }
    }

    // argNo starts from 0.
    // If argument does not parse correctly, message will be passed to UI and command aborted.
    protected argumentAsString(argString: string | null, argNo: number, optional: boolean = false, toLower: boolean = true): string | undefined {
        if (!argString) { return undefined; }
        if (!optional && argString.split(",").length < argNo + 1) {
            throw new Error(Usermessages.tooFewArguments);
        }
        const args = argString.split(",");
        if (args.length < argNo + 1) {
            if (optional) {
                return undefined;
            } else {
                throw new Error(Usermessages.missingArgument(argNo + 1));
            }
        }
        return toLower ? args[argNo].trim().toLowerCase() : args[argNo].trim(); // which may be "" if argString ends in a ','
    }

    // argNo starts from 0.
    protected argumentAsNumber(args: string | null, argNo: number, optional: boolean = false): number | null {
        const arg = this.argumentAsString(args, argNo, optional);
        if (!arg && optional) { return null; }
        const number = parseInt(arg!, 10);
        if (isNaN(number)) {
            throw new Error(Usermessages.wrongTypeArgument(argNo + 1));
        }
        return number;
    }

    protected parseInt(input: string): number {
        const number = parseInt(input, 10);
        if (isNaN(number)) {
            throw new Error(Usermessages.isNotANumber(input));
        }
        return number;
    }

    // Parses '17, 3-5, -9, 6-' into two numbers
    protected parseRange(arg?: string): { start: number | null; end: number | null } {
        if (!arg) {
            arg = "1-";
        }
        const clauses = arg.split("-");
        const range: { start: number | null; end: number | null } = { start: null, end: null };
        switch (clauses.length) {
            case 1: {
                const firstValue =  clauses[0];
                range.start = firstValue ? this.parseInt(firstValue) : null;
                range.end = range.start;
                break;
            }
            case 2: {
                const firstValue =  clauses[0];
                const secondValue =  clauses[1];
                range.start = firstValue ? this.parseInt(firstValue) : null;
                range.end = secondValue ? this.parseInt(secondValue) : null;
                break;
            }
            default:
                throw new Error(Usermessages.tooManyDashes);
        }
        if ((range.start != null && range.start < 1) || (range.end != null && range.end < 1)) {
            throw new Error(Usermessages.mustBeGreaterThanZero);
        }
        return range;
    }

    protected getContextDescription(): string | null {
        // todo
        return null;
    }

    protected routeData(): Routedata.PaneRouteData {
        return this.urlManager.getRouteData().pane1;
    }

    // Helpers delegating to RouteData
    protected isHome(): boolean {
        return this.urlManager.isHome();
    }

    protected isObject(): boolean {
        return this.urlManager.isObject();
    }

    protected getObject(): Promise<Models.DomainObjectRepresentation> {
        const oid = Models.ObjectIdWrapper.fromObjectId(this.routeData().objectId, this.keySeparator);
        // TODO: Consider view model & transient modes?

        return this.context.getObject(1, oid, this.routeData().interactionMode).then((obj: Models.DomainObjectRepresentation) => {
            if (this.routeData().interactionMode === Routedata.InteractionMode.Edit) {
                return this.context.getObjectForEdit(1, obj);
            } else {
                return obj; // To wrap a known object as a promise
            }
        });
    }

    protected isList(): boolean {
        return this.urlManager.isList();
    }

    protected getList(): Promise<Models.ListRepresentation> {
        const routeData = this.routeData();
        // TODO: Currently covers only the list-from-menu; need to cover list from object action
        return this.context.getListFromMenu(routeData, routeData.page, routeData.pageSize);
    }

    protected isMenu(): boolean {
        return !!this.routeData().menuId;
    }

    protected getMenu(): Promise<Models.MenuRepresentation> {
        return this.context.getMenu(this.routeData().menuId);
    }

    protected isDialog(): boolean {
        return !!this.routeData().dialogId;
    }

    protected isMultiChoiceField(field: Models.IField) {
        const entryType = field.entryType();
        return entryType === Models.EntryType.MultipleChoices || entryType === Models.EntryType.MultipleConditionalChoices;
    }

    protected getActionForCurrentDialog(): Promise<Models.InvokableActionMember | Models.ActionRepresentation> {
        const dialogId = this.routeData().dialogId;
        if (this.isObject()) {
            return this.getObject().then((obj: Models.DomainObjectRepresentation) => this.context.getInvokableAction(obj.actionMember(dialogId)));
        } else if (this.isMenu()) {
            return this.getMenu().then((menu: Models.MenuRepresentation) => this.context.getInvokableAction(menu.actionMember(dialogId))); // i.e. return a promise
        }
        return Promise.reject(new Models.ErrorWrapper(Models.ErrorCategory.ClientError, Models.ClientErrorCode.NotImplemented, "List actions not implemented yet"));
    }

    // Tests that at least one collection is open (should only be one).
    // TODO: assumes that closing collection removes it from routeData NOT sets it to Summary
    protected isCollection(): boolean {
        return this.isObject() && some(this.routeData().collections);
    }

    protected closeAnyOpenCollections() {
        const open = this.ciceroRenderer.openCollectionIds(this.routeData());
        forEach(open, id => this.urlManager.setCollectionMemberState(id, Routedata.CollectionViewState.Summary));
    }

    protected isTable(): boolean {
        return false; // TODO
    }

    protected isEdit(): boolean {
        return this.routeData().interactionMode === Routedata.InteractionMode.Edit;
    }

    protected isForm(): boolean {
        return this.routeData().interactionMode === Routedata.InteractionMode.Form;
    }

    protected isTransient(): boolean {
        return this.routeData().interactionMode === Routedata.InteractionMode.Transient;
    }

    protected matchingProperties(obj: Models.DomainObjectRepresentation, match?: string): Models.PropertyMember[] {
        let props = map(obj.propertyMembers(), prop => prop);
        if (match) {
            props = this.matchFriendlyNameAndOrMenuPath(props, match);
        }
        return props;
    }

    protected matchingCollections(obj: Models.DomainObjectRepresentation, match?: string): Models.CollectionMember[] {
        const allColls = map(obj.collectionMembers(), action => action);
        if (match) {
            return this.matchFriendlyNameAndOrMenuPath<Models.CollectionMember>(allColls, match);
        } else {
            return allColls;
        }
    }

    protected matchingParameters(action: Models.InvokableActionMember, match: string): Models.Parameter[] {
        let params = map(action.parameters(), p => p);
        if (match) {
            params = this.matchFriendlyNameAndOrMenuPath(params, match);
        }
        return params;
    }

    protected matchFriendlyNameAndOrMenuPath<T extends Models.IHasExtensions>(
        reps: T[],
        match: string | undefined): T[] {
        const clauses = match ? match.split(" ") : [];
        // An exact match has preference over any partial match
        const exactMatches = filter(reps,
            (rep) => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return match === name ||
                    (!!path && match === path.toLowerCase() + " " + name) ||
                    every(clauses, clause => name === clause || (!!path && path.toLowerCase() === clause));
            });
        if (exactMatches.length > 0) { return exactMatches; }
        return filter(reps,
            rep => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return every(clauses, clause => name.indexOf(clause) >= 0 || (!!path && path.toLowerCase().indexOf(clause) >= 0));
            });
    }

    protected findMatchingChoicesForRef(choices: Dictionary<Models.Value> | null, titleMatch: string): Models.Value[] {
        return choices ? filter(choices, v => v.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0) : [];
    }

    protected findMatchingChoicesForScalar(choices: Dictionary<Models.Value> | null, titleMatch: string): Models.Value[] {
        if (choices == null) {
            return [];
        }

        const labels = keys(choices);
        const matchingLabels = filter(labels, l => l.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0);
        const result = new Array<Models.Value>();
        switch (matchingLabels.length) {
            case 0:
                break; // leave result empty
            case 1:
                // Push the VALUE for the key
                // For simple scalars they are the same, but not for Enums
                result.push(choices[matchingLabels[0]]);
                break;
            default:
                // Push the matching KEYs, wrapped as (pseudo) Values for display in message to user
                // For simple scalars the values would also be OK, but not for Enums
                forEach(matchingLabels, label => result.push(new Models.Value(label)));
                break;
        }
        return result;
    }

    protected handleErrorResponse(err: Models.ErrorMap, getFriendlyName: (id: string) => string) {
        if (err.invalidReason()) {
            return this.returnResult("", err.invalidReason());
        }
        let msg = Usermessages.pleaseCompleteOrCorrect;
        each(err.valuesMap(),
            (errorValue, fieldId) => {
                msg += this.fieldValidationMessage(errorValue, () => getFriendlyName(fieldId!));
            });
        return this.returnResult("", msg);
    }

    protected handleErrorResponseNew(err: Models.ErrorMap, getFriendlyName: (id: string) => string) {
        if (err.invalidReason()) {
            return this.returnResult("", err.invalidReason());
        }
        let msg = Usermessages.pleaseCompleteOrCorrect;
        each(err.valuesMap(),
            (errorValue, fieldId) => {
                msg += this.fieldValidationMessage(errorValue, () => getFriendlyName(fieldId!));
            });
        return this.returnResult("", msg);
    }

    protected validationMessage(reason: string | null, value: Models.Value, fieldFriendlyName:  string): string {
        if (reason) {
            const prefix = `${fieldFriendlyName}: `;
            const suffix = reason === Usermessages.mandatory ? Usermessages.required : `${value} ${reason}`;
            return `${prefix}${suffix}\n`;
        }
        return "";
    }

    private fieldValidationMessage(errorValue: Models.ErrorValue, fieldFriendlyName: () => string): string {
        const reason = errorValue.invalidReason;
        return this.validationMessage(reason, errorValue.value, fieldFriendlyName());
    }

    protected valueForUrl(val: Models.Value, field: Models.IField): Models.Value | null {
        if (val.isNull()) { return val; }
        const fieldEntryType = field.entryType();

        if (fieldEntryType !== Models.EntryType.FreeForm || field.isCollectionContributed()) {

            if (this.isMultiChoiceField(field) || field.isCollectionContributed()) {
                let valuesFromRouteData: Models.Value[] | null = new Array<Models.Value>();
                if (field instanceof Models.Parameter) {
                    const rd = Commandresult.getParametersAndCurrentValue(field.parent, this.context)[field.id()];
                    if (rd) { valuesFromRouteData = rd.list(); } // TODO: what if only one?
                } else if (field instanceof Models.PropertyMember) {
                    const obj = field.parent as Models.DomainObjectRepresentation;
                    const props = this.context.getObjectCachedValues(obj.id());
                    const rd = props[field.id()];
                    if (rd) { valuesFromRouteData = rd.list(); } // TODO: what if only one?
                }
                let vals: Models.Value[] = [];
                if (val.isReference() || val.isScalar()) {
                    vals = new Array<Models.Value>(val);
                } else if (val.isList()) { // Should be!
                    vals = val.list()!;
                }
                valuesFromRouteData = valuesFromRouteData || [];

                forEach(vals, v => this.addOrRemoveValue(valuesFromRouteData!, v));

                if (vals[0].isScalar()) { // then all must be scalar
                    const scalars = map(valuesFromRouteData, v => v.scalar());
                    return new Models.Value(scalars);
                } else { // assumed to be links
                    const links: Rointerfaces.ILink[] = map(valuesFromRouteData, v => ({ href: v.link()!.href(), title: Models.withUndefined(v.link()!.title()) }));
                    return new Models.Value(links.length > 0 ? links : null);
                }
            }
            if (val.isScalar()) {
                return val;
            }
            // reference
            return this.leanLink(val);
        }

        if (val.isScalar()) {
            if (val.isNull()) {
                return new Models.Value("");
            }
            return val;
            // TODO: consider these options:
            //    if (from.value instanceof Date) {
            //        return new Value((from.value as Date).toISOString());
            //    }

            //    return new Value(from.value as number | string | boolean);
        }
        if (val.isReference()) {
            return this.leanLink(val);
        }
        return null;
    }

    private leanLink(val: Models.Value): Models.Value {
        return new Models.Value({ href: val.link()!.href()!, title: val.link()!.title()! });
    }

    private addOrRemoveValue(valuesFromRouteData: Models.Value[], val: Models.Value) {
        let index: number;
        let valToAdd: Models.Value;
        if (val.isScalar()) {
            valToAdd = val;
            index = findIndex(valuesFromRouteData, v => v.scalar() === val.scalar());
        } else { // Must be reference
            valToAdd = this.leanLink(val);
            index = findIndex(valuesFromRouteData, v => v.link()!.href() === valToAdd.link()!.href());
        }
        if (index > -1) {
            valuesFromRouteData.splice(index, 1);
        } else {
            valuesFromRouteData.push(valToAdd);
        }
    }

    protected setFieldValueInContext(field: Models.Parameter, val: Models.Value) {
        this.context.cacheFieldValue(this.routeData().dialogId, field.id(), val);
    }

    protected setPropertyValueinContext(obj: Models.DomainObjectRepresentation, property: Models.PropertyMember, urlVal: Models.Value) {
        this.context.cachePropertyValue(obj, property, urlVal);
    }
}
