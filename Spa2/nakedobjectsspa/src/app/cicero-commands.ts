import * as Ro from './models';
import * as Msg from './user-messages';
import * as Rend from './cicero-renderer.service';
import * as Const from './constants';
import * as RtD from './route-data';
import * as Models from './models';
import { ContextService } from './context.service';
import { UrlManagerService } from './url-manager.service';
import { MaskService } from './mask.service';
import { ErrorService } from './error.service';
import { CiceroViewModel } from './view-models/cicero-view-model';
import { Location } from '@angular/common';
import { CiceroCommandFactoryService } from './cicero-command-factory.service';
import { ConfigService, IAppConfig } from './config.service';
import * as moment from 'moment';
import { Dictionary } from 'lodash';
import each from 'lodash/each';
import filter from 'lodash/filter';
import findIndex from 'lodash/findIndex';
import forEach from 'lodash/forEach';
import every from 'lodash/every';
import keys from 'lodash/keys';
import map from 'lodash/map';
import zipObject from 'lodash/zipObject';
import mapValues from 'lodash/mapValues';
import reduce from 'lodash/reduce';
import some from 'lodash/some';
import mapKeys from 'lodash/mapKeys';
import fromPairs from 'lodash/fromPairs';

export function getParametersAndCurrentValue(action: Ro.ActionMember | Models.ActionRepresentation | Models.InvokableActionMember, context: ContextService): Dictionary<Ro.Value> {

    if (action instanceof Models.InvokableActionMember || action instanceof Models.ActionRepresentation) {
        const parms = action.parameters();
        const values = context.getDialogCachedValues(action.actionId());
        return mapValues(parms,
            p => {
                const value = values[p.id()];
                if (value === undefined) {
                    return p.default();
                }
                return value;
            });
    }
    return {};
}

export class CommandResult {
    input: string;
    output: string;
    changeState: () => void = () => { };
    clipboard: any; // todo object ? 
}


export abstract class Command {

    constructor(protected urlManager: UrlManagerService,
        protected location: Location,
        protected commandFactory: CiceroCommandFactoryService,
        protected context: ContextService,
        protected mask: MaskService,
        protected error: ErrorService,
        protected configService: ConfigService
    ) {
        this.keySeparator = configService.config.keySeparator;
    }

    argString: string;
    chained: boolean;


    fullCommand: string;
    helpText: string;
    protected minArguments: number;
    protected maxArguments: number;
    protected cvm: CiceroViewModel;
    protected keySeparator: string;

    execute(argString: string | null, chained: boolean, cvm: CiceroViewModel): void {
        this.cvm = cvm;
        //TODO Create outgoing Vm and copy across values as needed
        if (!this.isAvailableInCurrentContext()) {
            this.clearInputAndSetMessage(Msg.commandNotAvailable(this.fullCommand));
            return;
        }
        //TODO: This could be moved into a pre-parse method as it does not depend on context
        if (argString == null) {
            if (this.minArguments > 0) {
                this.clearInputAndSetMessage(Msg.noArguments);
                return;
            }
        } else {
            const args = argString.split(",");
            if (args.length < this.minArguments) {
                this.clearInputAndSetMessage(Msg.tooFewArguments);
                return;
            } else if (args.length > this.maxArguments) {
                this.clearInputAndSetMessage(Msg.tooManyArguments);
                return;
            }
        }
        this.doExecute(argString, chained);
    }

    executeNew(): Promise<CommandResult> {
        const result = new CommandResult();

        //TODO Create outgoing Vm and copy across values as needed
        if (!this.isAvailableInCurrentContext()) {
            return this.returnResult("", Msg.commandNotAvailable(this.fullCommand));
        }
        //TODO: This could be moved into a pre-parse method as it does not depend on context
        if (this.argString == null) {
            if (this.minArguments > 0) {

                return this.returnResult("", Msg.noArguments);
            }
        } else {
            const args = this.argString.split(",");
            if (args.length < this.minArguments) {

                return this.returnResult("", Msg.tooFewArguments);
            } else if (args.length > this.maxArguments) {
                return this.returnResult("", Msg.tooManyArguments);
            }
        }
        return this.doExecuteNew(this.argString, this.chained, result);
    }

    returnResult(input: string, output: string, changeState?: () => void): Promise<CommandResult> {
        changeState = changeState ? changeState : () => { };
        return Promise.resolve({ input: input, output: output, changeState: changeState });
    }


    abstract doExecute(args: string, chained: boolean): void;

    abstract doExecuteNew(args: string, chained: boolean, result: CommandResult): Promise<CommandResult>;

    abstract isAvailableInCurrentContext(): boolean;

    //Helper methods follow
    protected clearInputAndSetMessage(text: string): void {
        this.cvm.clearInput();
        this.cvm.message = text;
        //TODO this.$route.reload();
    }

    protected mayNotBeChained(rider: string = "") {
        return Msg.mayNotbeChainedMessage(this.fullCommand, rider);
    }

    //TODO: Change this  -  must build up output before setting the outputSource, which will result in refresh
    protected appendAsNewLineToOutput(text: string): void {
        this.cvm.setOutputSource(`/n${text}`);
    }

    checkMatch(matchText: string): void {
        if (this.fullCommand.indexOf(matchText) !== 0) {
            throw new Error(Msg.noSuchCommand(matchText));
        }
    }

    //argNo starts from 0.
    //If argument does not parse correctly, message will be passed to UI and command aborted.
    protected argumentAsString(argString: string, argNo: number, optional: boolean = false, toLower: boolean = true): string | undefined {
        if (!argString) return undefined;
        if (!optional && argString.split(",").length < argNo + 1) {
            throw new Error(Msg.tooFewArguments);
        }
        const args = argString.split(",");
        if (args.length < argNo + 1) {
            if (optional) {
                return undefined;
            } else {
                throw new Error(Msg.missingArgument(argNo + 1));
            }
        }
        return toLower ? args[argNo].trim().toLowerCase() : args[argNo].trim(); // which may be "" if argString ends in a ','
    }

    //argNo starts from 0.
    protected argumentAsNumber(args: string, argNo: number, optional: boolean = false): number | null {
        const arg = this.argumentAsString(args, argNo, optional);
        if (!arg && optional) return null;
        const number = parseInt(arg!);
        if (isNaN(number)) {
            throw new Error(Msg.wrongTypeArgument(argNo + 1));
        }
        return number;
    }

    protected parseInt(input: string): number | null {
        if (!input) {
            return null;
        }
        const number = parseInt(input);
        if (isNaN(number)) {
            throw new Error(Msg.isNotANumber(input));
        }
        return number;
    }

    //Parses '17, 3-5, -9, 6-' into two numbers 
    protected parseRange(arg: string): { start: number | null, end: number | null } {
        if (!arg) {
            arg = "1-";
        }
        const clauses = arg.split("-");
        const range: { start: number | null, end: number | null } = { start: null, end: null };
        switch (clauses.length) {
            case 1:
                range.start = this.parseInt(clauses[0]);
                range.end = range.start;
                break;
            case 2:
                range.start = this.parseInt(clauses[0]);
                range.end = this.parseInt(clauses[1]);
                break;
            default:
                throw new Error(Msg.tooManyDashes);
        }
        if ((range.start != null && range.start < 1) || (range.end != null && range.end < 1)) {
            throw new Error(Msg.mustBeGreaterThanZero);
        }
        return range;
    }

    protected getContextDescription(): string | null {
        //todo
        return null;
    }

    protected routeData(): RtD.PaneRouteData {
        return this.urlManager.getRouteData().pane1;
    }

    //Helpers delegating to RouteData
    protected isHome(): boolean {
        return this.urlManager.isHome();
    }

    protected isObject(): boolean {
        return this.urlManager.isObject();
    }

    protected getObject(): Promise<Ro.DomainObjectRepresentation> {
        const oid = Ro.ObjectIdWrapper.fromObjectId(this.routeData().objectId, this.keySeparator);
        //TODO: Consider view model & transient modes?

        return this.context.getObject(1, oid, this.routeData().interactionMode).then((obj: Ro.DomainObjectRepresentation) => {
            if (this.routeData().interactionMode === RtD.InteractionMode.Edit) {
                return this.context.getObjectForEdit(1, obj);
            } else {
                return obj; //To wrap a known object as a promise
            }
        });
    }

    protected isList(): boolean {
        return this.urlManager.isList();
    }

    protected getList(): Promise<Ro.ListRepresentation> {
        const routeData = this.routeData();
        //TODO: Currently covers only the list-from-menu; need to cover list from object action
        return this.context.getListFromMenu(routeData, routeData.page, routeData.pageSize);
    }

    protected isMenu(): boolean {
        return !!this.routeData().menuId;
    }

    protected getMenu(): Promise<Ro.MenuRepresentation> {
        return this.context.getMenu(this.routeData().menuId);
    }

    protected isDialog(): boolean {
        return !!this.routeData().dialogId;
    }

    protected getActionForCurrentDialog(): Promise<Models.InvokableActionMember | Models.ActionRepresentation> {
        const dialogId = this.routeData().dialogId;
        if (this.isObject()) {
            return this.getObject().then((obj: Ro.DomainObjectRepresentation) => this.context.getInvokableAction(obj.actionMember(dialogId)));
        } else if (this.isMenu()) {
            return this.getMenu().then((menu: Ro.MenuRepresentation) => this.context.getInvokableAction(menu.actionMember(dialogId))); //i.e. return a promise
        }
        return Promise.reject(new Ro.ErrorWrapper(Ro.ErrorCategory.ClientError, Ro.ClientErrorCode.NotImplemented, "List actions not implemented yet"));
    }

    //Tests that at least one collection is open (should only be one). 
    //TODO: assumes that closing collection removes it from routeData NOT sets it to Summary
    protected isCollection(): boolean {
        return this.isObject() && some(this.routeData().collections);
    }

    protected closeAnyOpenCollections() {
        const open = Rend.openCollectionIds(this.routeData());
        forEach(open, id => this.urlManager.setCollectionMemberState(id, RtD.CollectionViewState.Summary));
    }

    protected isTable(): boolean {
        return false; //TODO
    }

    protected isEdit(): boolean {
        return this.routeData().interactionMode === RtD.InteractionMode.Edit;
    }

    protected isForm(): boolean {
        return this.routeData().interactionMode === RtD.InteractionMode.Form;
    }

    protected isTransient(): boolean {
        return this.routeData().interactionMode === RtD.InteractionMode.Transient;
    }

    protected matchingProperties(
        obj: Ro.DomainObjectRepresentation,
        match: string): Ro.PropertyMember[] {
        let props = map(obj.propertyMembers(), prop => prop);
        if (match) {
            props = this.matchFriendlyNameAndOrMenuPath(props, match);
        }
        return props;
    }

    protected matchingCollections(
        obj: Ro.DomainObjectRepresentation,
        match: string): Ro.CollectionMember[] {
        const allColls = map(obj.collectionMembers(), action => action);
        if (match) {
            return this.matchFriendlyNameAndOrMenuPath<Ro.CollectionMember>(allColls, match);
        } else {
            return allColls;
        }
    }

    protected matchingParameters(action: Ro.InvokableActionMember, match: string): Ro.Parameter[] {
        let params = map(action.parameters(), p => p);
        if (match) {
            params = this.matchFriendlyNameAndOrMenuPath(params, match);
        }
        return params;
    }

    protected matchFriendlyNameAndOrMenuPath<T extends Ro.IHasExtensions>(
        reps: T[],
        match: string): T[] {
        const clauses = match.split(" ");
        //An exact match has preference over any partial match
        const exactMatches = filter(reps,
            (rep) => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return match === name ||
                    (!!path && match === path.toLowerCase() + " " + name) ||
                    every(clauses, clause => name === clause || (!!path && path.toLowerCase() === clause));
            });
        if (exactMatches.length > 0) return exactMatches;
        return filter(reps,
            rep => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return every(clauses, clause => name.indexOf(clause) >= 0 || (!!path && path.toLowerCase().indexOf(clause) >= 0));
            });
    }

    protected findMatchingChoicesForRef(choices: Dictionary<Ro.Value>, titleMatch: string): Ro.Value[] {
        return filter(choices, v => v.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0);
    }

    protected findMatchingChoicesForScalar(choices: Dictionary<Ro.Value>, titleMatch: string): Ro.Value[] {
        const labels = keys(choices);
        const matchingLabels = filter(labels, l => l.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0);
        const result = new Array<Ro.Value>();
        switch (matchingLabels.length) {
            case 0:
                break; //leave result empty
            case 1:
                //Push the VALUE for the key
                //For simple scalars they are the same, but not for Enums
                result.push(choices[matchingLabels[0]]);
                break;
            default:
                //Push the matching KEYs, wrapped as (pseudo) Values for display in message to user
                //For simple scalars the values would also be OK, but not for Enums
                forEach(matchingLabels, label => result.push(new Ro.Value(label)));
                break;
        }
        return result;
    }

    protected handleErrorResponse(err: Ro.ErrorMap, getFriendlyName: (id: string) => string) {
        if (err.invalidReason()) {
            this.clearInputAndSetMessage(err.invalidReason());
            return;
        }
        let msg = Msg.pleaseCompleteOrCorrect;
        each(err.valuesMap(),
            (errorValue, fieldId) => {
                msg += this.fieldValidationMessage(errorValue, () => getFriendlyName(fieldId!));
            });
        this.clearInputAndSetMessage(msg);
    }

    private fieldValidationMessage(errorValue: Ro.ErrorValue, fieldFriendlyName: () => string): string {
        let msg = "";
        const reason = errorValue.invalidReason;
        const value = errorValue.value;
        if (reason) {
            msg += `${fieldFriendlyName()}: `;
            if (reason === Msg.mandatory) {
                msg += Msg.required;
            } else {
                msg += `${value} ${reason}`;
            }
            msg += "\n";
        }
        return msg;
    }

    protected valueForUrl(val: Ro.Value, field: Ro.IField): Ro.Value | null {
        if (val.isNull()) return val;
        const fieldEntryType = field.entryType();

        if (fieldEntryType !== Ro.EntryType.FreeForm || field.isCollectionContributed()) {

            if (fieldEntryType === Ro.EntryType.MultipleChoices || field.isCollectionContributed()) {
                let valuesFromRouteData: Ro.Value[] | null = new Array<Ro.Value>();
                if (field instanceof Ro.Parameter) {
                    const rd = getParametersAndCurrentValue(field.parent, this.context)[field.id()];
                    if (rd) valuesFromRouteData = rd.list(); //TODO: what if only one?
                } else if (field instanceof Ro.PropertyMember) {
                    const obj = field.parent as Ro.DomainObjectRepresentation;
                    const props = this.context.getObjectCachedValues(obj.id());
                    const rd = props[field.id()];
                    if (rd) valuesFromRouteData = rd.list(); //TODO: what if only one?
                }
                let vals: Ro.Value[] | null = [];
                if (val.isReference() || val.isScalar()) {
                    vals = new Array<Ro.Value>(val);
                } else if (val.isList()) { //Should be!
                    vals = val.list();
                }
                forEach(vals,
                    v => {
                        this.addOrRemoveValue(valuesFromRouteData, v);
                    });
                if (vals[0].isScalar()) { //then all must be scalar
                    const scalars = map(valuesFromRouteData, v => v.scalar());
                    return new Ro.Value(scalars);
                } else { //assumed to be links
                    const links = map(valuesFromRouteData,
                        v => (
                            { href: v.link().href(), title: v.link().title() }
                        ));
                    return new Ro.Value(links);
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
                return new Ro.Value("");
            }
            return val;
            //TODO: consider these options:
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

    private leanLink(val: Ro.Value): Ro.Value {
        return new Ro.Value({ href: val.link()!.href()!, title: val.link()!.title()! });
    }

    private addOrRemoveValue(valuesFromRouteData: Ro.Value[], val: Ro.Value) {
        let index: number;
        let valToAdd: Ro.Value;
        if (val.isScalar()) {
            valToAdd = val;
            index = findIndex(valuesFromRouteData, v => v.scalar() === val.scalar());
        } else { //Must be reference
            valToAdd = this.leanLink(val);
            index = findIndex(valuesFromRouteData, v => v.link()!.href() === valToAdd.link()!.href());
        }
        if (index > -1) {
            valuesFromRouteData.splice(index, 1);
        } else {
            valuesFromRouteData.push(valToAdd);
        }
    }

    protected setFieldValueInContextAndUrl(field: Ro.Parameter, urlVal: Ro.Value) {
        this.context.cacheFieldValue(this.routeData().dialogId, field.id(), urlVal);
        //this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
    }

    protected setPropertyValueinContextAndUrl(obj: Ro.DomainObjectRepresentation, property: Ro.PropertyMember, urlVal: Ro.Value) {
        this.context.cachePropertyValue(obj, property, urlVal);
        //this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
    }
}

export class Action extends Command {

    fullCommand = Msg.actionCommand;
    helpText = Msg.actionHelp;
    protected minArguments = 0;
    protected maxArguments = 2;

    isAvailableInCurrentContext(): boolean {
        return (this.isMenu() || this.isObject() || this.isForm()) && !this.isDialog() && !this.isEdit(); //TODO add list
    }

    doExecute(args: string, chained: boolean): void {
        const match = this.argumentAsString(args, 0);
        const details = this.argumentAsString(args, 1, true);
        if (details && details !== "?") {
            this.clearInputAndSetMessage(Msg.mustbeQuestionMark);
            return;
        }
        if (this.isObject()) {
            this.getObject()
                .then((obj: Ro.DomainObjectRepresentation) => this.processActions(match, obj.actionMembers(), details))
                .catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        } else if (this.isMenu()) {
            this.getMenu()
                .then((menu: Ro.MenuRepresentation) => this.processActions(match, menu.actionMembers(), details))
                .catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        }
        //TODO: handle list - CCAs
    }

    doExecuteNew(args: string, chained: boolean, result: CommandResult): Promise<CommandResult> {
        const match = this.argumentAsString(args, 0);
        const details = this.argumentAsString(args, 1, true);
        if (details && details !== "?") {
            return this.returnResult("", Msg.mustbeQuestionMark);
        }
        if (this.isObject()) {
            return this.getObject().then(obj => this.processActionsNew(match, obj.actionMembers(), details));

        } else if (this.isMenu()) {
            return this.getMenu().then(menu => this.processActionsNew(match, menu.actionMembers(), details));
        }
        //TODO: handle list - CCAs
        return Promise.reject("TODO: handle list - CCAs");
    }

    private processActionsNew(match: string | null, actionsMap: Dictionary<Ro.ActionMember>, details: string): Promise<CommandResult> {
        let actions = map(actionsMap, action => action);
        if (actions.length === 0) {
            return this.returnResult("", Msg.noActionsAvailable);
        }
        if (match) {
            actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
        }
        switch (actions.length) {
            case 0:
                return this.returnResult("", Msg.doesNotMatchActions(match));
            case 1:
                const action = actions[0];
                if (details) {
                    return this.returnResult("", this.renderActionDetails(action));
                } else if (action.disabledReason()) {
                    return this.returnResult("", this.disabledAction(action));
                } else {
                    return this.openActionDialog(action);
                }
            default:
                let output = match ? Msg.matchingActions : Msg.actionsMessage;
                output += this.listActions(actions);
                return this.returnResult("", output);
        }
    }

    private processActions(match: string | null, actionsMap: Dictionary<Ro.ActionMember>, details: string) {
        let actions = map(actionsMap, action => action);
        if (actions.length === 0) {
            this.clearInputAndSetMessage(Msg.noActionsAvailable);
            return;
        }
        if (match) {
            actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
        }
        switch (actions.length) {
            case 0:
                this.clearInputAndSetMessage(Msg.doesNotMatchActions(match));
                break;
            case 1:
                const action = actions[0];
                if (details) {
                    this.renderActionDetails(action);
                } else if (action.disabledReason()) {
                    this.disabledAction(action);
                } else {
                    this.openActionDialog(action);
                }
                break;
            default:
                let output = match ? Msg.matchingActions : Msg.actionsMessage;
                output += this.listActions(actions);
                this.clearInputAndSetMessage(output);
        }
    }

    private disabledAction(action: Ro.ActionMember) {
        return `${Msg.actionPrefix} ${action.extensions().friendlyName()} ${Msg.isDisabled} ${action.disabledReason()}`;
    }

    private listActions(actions: Ro.ActionMember[]): string {
        return reduce(actions,
            (s, t) => {
                const menupath = t.extensions().menuPath() ? `${t.extensions().menuPath()} - ` : "";
                const disabled = t.disabledReason() ? ` (${Msg.disabledPrefix} ${t.disabledReason()})` : "";
                return s + menupath + t.extensions().friendlyName() + disabled + "\n";
            },
            "");
    }

    private openActionDialog(action: Ro.ActionMember): Promise<CommandResult> {

        return this.context.getInvokableAction(action).
            then(invokable => {

                this.context.clearDialogCachedValues();
                this.urlManager.setDialog(action.actionId());
                forEach(invokable.parameters(), p => this.setFieldValueInContextAndUrl(p, p.default()));

                return this.returnResult("", "");
            });
    }

    private renderActionDetails(action: Ro.ActionMember) {
        return `${Msg.descriptionPrefix} ${action.extensions().friendlyName()}\n${action.extensions().description() || Msg.noDescription}`;
    }
}

export class Back extends Command {

    fullCommand = Msg.backCommand;
    helpText = Msg.backHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        this.location.back();
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
       
        return this.returnResult("", "", () => this.location.back());
    };
}

export class Cancel extends Command {

    fullCommand = Msg.cancelCommand;
    helpText = Msg.cancelHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog() || this.isEdit();
    }

    doExecute(args: string, chained: boolean): void {
        if (this.isEdit()) {
            this.urlManager.setInteractionMode(RtD.InteractionMode.View);
        }
        if (this.isDialog()) {
            this.urlManager.closeDialogReplaceHistory(""); //TODO provide ID
        }
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        if (this.isEdit()) {           
            return this.returnResult("", "", () => this.urlManager.setInteractionMode(RtD.InteractionMode.View));
        }

        if (this.isDialog()) {
            return this.returnResult("", "", () => this.urlManager.closeDialogReplaceHistory(this.routeData().dialogId));
        }

        return this.returnResult("", "some sort of error"); // todo
    };
}

export class Clipboard extends Command {

    fullCommand = Msg.clipboardCommand;
    helpText = Msg.clipboardHelp;

    protected minArguments = 1;
    protected maxArguments = 1;

    private contents : any;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        const sub = this.argumentAsString(args, 0);
        if (Msg.clipboardCopy.indexOf(sub) === 0) {
            this.copy();
        } else if (Msg.clipboardShow.indexOf(sub) === 0) {
            this.show();
        } else if (Msg.clipboardGo.indexOf(sub) === 0) {
            this.go();
        } else if (Msg.clipboardDiscard.indexOf(sub) === 0) {
            this.discard();
        } else {
            this.clearInputAndSetMessage(Msg.clipboardError);
        }
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        const sub = this.argumentAsString(args, 0);
        if (Msg.clipboardCopy.indexOf(sub) === 0) {
            return this.copy();
        } else if (Msg.clipboardShow.indexOf(sub) === 0) {
            return this.show();
        } else if (Msg.clipboardGo.indexOf(sub) === 0) {
            return this.go();
        } else if (Msg.clipboardDiscard.indexOf(sub) === 0) {
            return this.discard();
        } else {
            return this.returnResult("", Msg.clipboardError);
        }
    };

    private copy(): Promise<CommandResult> {
        if (!this.isObject()) {         
            return this.returnResult("", Msg.clipboardContextError);
        }
        return this.getObject().then(obj => {
            this.contents = obj;
            const label = Ro.typePlusTitle(obj);
            return this.returnResult("", Msg.clipboardContents(label));
        });
    }

    private show(): Promise<CommandResult> {
        if (this.contents) {
            const label = Ro.typePlusTitle(this.contents);
            return this.returnResult("", Msg.clipboardContents(label));
        } else {
           
            return this.returnResult("", Msg.clipboardEmpty);
        }
    }

    private go(): Promise<CommandResult>  {
        const link = this.contents && this.contents.selfLink();
        if (link) {
            //this.urlManager.setItem(link);
            return this.returnResult("", "", () => this.urlManager.setItem(link));
        } else {
            return this.show();
        }
    }

    private discard(): Promise<CommandResult> {
        this.contents = null;
        return this.show();
    }
}

export class Edit extends Command {

    fullCommand = Msg.editCommand;
    helpText = Msg.editHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() && !this.isEdit();
    }

    doExecute(args: string, chained: boolean): void {
        if (chained) {
            this.mayNotBeChained();
            return;
        }
        this.context.clearObjectCachedValues();
        this.urlManager.setInteractionMode(RtD.InteractionMode.Edit);
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        if (chained) {
            return this.returnResult("", this.mayNotBeChained());
        }
        const newState = () => {
            this.context.clearObjectCachedValues();
            this.urlManager.setInteractionMode(RtD.InteractionMode.Edit);
        }

        return this.returnResult("", "", newState);
    };
}

export class Enter extends Command {

    fullCommand = Msg.enterCommand;
    helpText = Msg.enterHelp;
    protected minArguments = 2;
    protected maxArguments = 2;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog() || this.isEdit() || this.isTransient() || this.isForm();
    }

    doExecute(args: string, chained: boolean): void {
        //const fieldName = this.argumentAsString(args, 0);
        //const fieldEntry = this.argumentAsString(args, 1, false, false);
        //if (this.isDialog()) {
        //    this.fieldEntryForDialog(fieldName, fieldEntry);
        //} else {
        //    this.fieldEntryForEdit(fieldName, fieldEntry);
        //}
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        const fieldName = this.argumentAsString(args, 0);
        const fieldEntry = this.argumentAsString(args, 1, false, false);
        if (this.isDialog()) {
            return this.fieldEntryForDialog(fieldName, fieldEntry);
        } else {
            return this.fieldEntryForEdit(fieldName, fieldEntry);
        }
    };

    private fieldEntryForEdit(fieldName: string, fieldEntry: string) {
        return this.getObject().then(obj => {
            const fields = this.matchingProperties(obj, fieldName);

            switch (fields.length) {
                case 0:
                    const s = Msg.doesNotMatchProperties(fieldName);
                    return this.returnResult("", s);
                case 1:
                    const field = fields[0];
                    if (fieldEntry === "?") {
                        //TODO: does this work in edit mode i.e. show entered value
                        const s = this.renderFieldDetails(field, field.value());
                        return this.returnResult("", s);
                    } else {
                        this.findAndClearAnyDependentFields(field.id(), obj.propertyMembers());
                        return this.setField(field, fieldEntry);
                    }
                default:
                    const ss = reduce(fields, (s, prop) => s + prop.extensions().friendlyName() + "\n", `${fieldName} ${Msg.matchesMultiple}`);
                    return this.returnResult("", ss);
            }
        });
    }

    private findAndClearAnyDependentFields(changingField: string, allFields: Dictionary<Ro.IField>) {

        forEach(allFields,
            field => {
                const promptLink = field.promptLink();

                if (promptLink) {
                    const pArgs = promptLink.arguments();
                    const argNames = keys(pArgs);

                    if (argNames.indexOf(changingField.toLowerCase()) >= 0) {
                        this.clearField(field);
                    }
                }
            });
    }

    private fieldEntryForDialog(fieldName: string, fieldEntry: string) {
        return this.getActionForCurrentDialog().then(action => {
            //TODO: error -  need to get invokable action to get the params.
            let params = map(action.parameters(), param => param);
            params = this.matchFriendlyNameAndOrMenuPath(params, fieldName);
            switch (params.length) {
                case 0:
                    return this.returnResult("", Msg.doesNotMatchDialog(fieldName));
                case 1:
                    if (fieldEntry === "?") {
                        const p = params[0];
                        const value = getParametersAndCurrentValue(p.parent, this.context)[p.id()];
                        const s = this.renderFieldDetails(p, value);
                        return this.returnResult("", s);
                    } else {
                        this.findAndClearAnyDependentFields(fieldName, action.parameters());
                        return this.setField(params[0], fieldEntry);
                    }
                default:
                    return this.returnResult("", `${Msg.multipleFieldMatches} ${fieldName}`);//TODO: list them
            }
        });
    }

    private clearField(field: Ro.IField): void {
        this.context.cacheFieldValue(this.routeData().dialogId, field.id(), new Ro.Value(null));

        if (field instanceof Ro.Parameter) {
            this.context.cacheFieldValue(this.routeData().dialogId, field.id(), new Ro.Value(null));
        } else if (field instanceof Ro.PropertyMember) {
            const parent = field.parent as Ro.DomainObjectRepresentation;
            this.context.cachePropertyValue(parent, field, new Ro.Value(null));
        }
    }

    private setField(field: Ro.IField, fieldEntry: string) {
        if (field instanceof Ro.PropertyMember && field.disabledReason()) {
            return this.returnResult("", `${field.extensions().friendlyName()} ${Msg.isNotModifiable}`);
        }
        const entryType = field.entryType();
        switch (entryType) {
            case Ro.EntryType.FreeForm:
                return this.handleFreeForm(field, fieldEntry);
            case Ro.EntryType.AutoComplete:
                return this.handleAutoComplete(field, fieldEntry);
              
            case Ro.EntryType.Choices:
                return this.handleChoices(field, fieldEntry);
               
            case Ro.EntryType.MultipleChoices:
                return this.handleChoices(field, fieldEntry);
           
            case Ro.EntryType.ConditionalChoices:
                return this.handleConditionalChoices(field, fieldEntry);
              
            case Ro.EntryType.MultipleConditionalChoices:
                return this.handleConditionalChoices(field, fieldEntry);
                
            default:
              
                return this.returnResult("", Msg.invalidCase);
        }
    }

    private handleFreeForm(field: Ro.IField, fieldEntry: string) {
        if (field.isScalar()) {
            let value: Ro.Value = new Ro.Value(fieldEntry);
            //TODO: handle a non-parsable date
            if (Ro.isDateOrDateTime(field)) {
                const m = moment(fieldEntry, Const.supportedDateFormats, "en-GB", true); //TODO get actual locale

                if (m.isValid()) {
                    const dt = m.toDate();
                    value = new Ro.Value(Ro.toDateString(dt));
                }
            }
            this.setFieldValue(field, value);
            return this.returnResult("", "", () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl());
        } else {
            return this.handleReferenceField(field, fieldEntry);
        }
    }

    private setFieldValue(field: Ro.IField, value: Ro.Value): void {
        const urlVal = this.valueForUrl(value, field);
        if (field instanceof Ro.Parameter) {
            this.setFieldValueInContextAndUrl(field, urlVal);
        } else if (field instanceof Ro.PropertyMember) {
            const parent = field.parent;
            if (parent instanceof Ro.DomainObjectRepresentation) {
                this.setPropertyValueinContextAndUrl(parent, field, urlVal);
            }
        }
    }

    private handleReferenceField(field: Ro.IField, fieldEntry: string) {
        if (this.isPaste(fieldEntry)) {
            return this.handleClipboard(field);
        } else {         
            return this.returnResult("", Msg.invalidRefEntry);
        }
    }

    private isPaste(fieldEntry: string) {
        return "paste".indexOf(fieldEntry) === 0;
    }

    private handleClipboard(field: Ro.IField) {
        const ref = this.cvm.clipboard;
        if (!ref) {
            
            return this.returnResult("", Msg.emptyClipboard);
        }
        const paramType = field.extensions().returnType();
        const refType = ref.domainType();
        return this.context.isSubTypeOf(refType, paramType).then(isSubType => {
            if (isSubType) {
                const obj = this.cvm.clipboard;
                const selfLink = obj.selfLink();
                //Need to add a title to the SelfLink as not there by default
                selfLink.setTitle(obj.title());
                const value = new Ro.Value(selfLink);
                this.setFieldValue(field, value);

                return this.returnResult("", "", () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl());
            } else {
                
                return this.returnResult("", Msg.incompatibleClipboard);
            }
        });
    }

    private handleAutoComplete(field: Ro.IField, fieldEntry: string) {
        //TODO: Need to check that the minimum number of characters has been entered or fail validation
        if (!field.isScalar() && this.isPaste(fieldEntry)) {
            return this.handleClipboard(field);
        } else {
            return this.context.autoComplete(field, field.id(), null, fieldEntry).then(choices => {
                const matches = this.findMatchingChoicesForRef(choices, fieldEntry);
                return this.switchOnMatches(field, fieldEntry, matches);
            });
        }
    }

    private handleChoices(field: Ro.IField, fieldEntry: string) {
        let matches: Ro.Value[];
        if (field.isScalar()) {
            matches = this.findMatchingChoicesForScalar(field.choices(), fieldEntry);
        } else {
            matches = this.findMatchingChoicesForRef(field.choices(), fieldEntry);
        }
        return this.switchOnMatches(field, fieldEntry, matches);
    }

    private switchOnMatches(field: Ro.IField, fieldEntry: string, matches: Ro.Value[]) {
        switch (matches.length) {
            case 0:          
                return this.returnResult("", Msg.noMatch(fieldEntry));
            case 1:
                this.setFieldValue(field, matches[0]);
                return this.returnResult("", "", () => this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl());
            default:
                let msg = Msg.multipleMatches;
                forEach(matches, m => msg += m.toString() + "\n");
                return this.returnResult("", msg);
        }
    }

    private getPropertiesAndCurrentValue(obj: Ro.DomainObjectRepresentation): Dictionary<Ro.Value> {
        const props = obj.propertyMembers();
        const values = mapValues(props, p => p.value());
        const modifiedProps = this.context.getObjectCachedValues(obj.id());

        forEach(values,
            (v, k) => {
                const newValue = modifiedProps[k];
                if (newValue) {
                    values[k] = newValue;
                }
            });
        return mapKeys(values, (v, k) => k.toLowerCase());
    }

    private handleConditionalChoices(field: Ro.IField, fieldEntry: string) {
        let enteredFields: Dictionary<Ro.Value>;

        if (field instanceof Ro.Parameter) {
            enteredFields = getParametersAndCurrentValue(field.parent, this.context);
        }

        if (field instanceof Ro.PropertyMember) {
            enteredFields = this.getPropertiesAndCurrentValue(field.parent as Ro.DomainObjectRepresentation);
        }

        const args = fromPairs(map(field.promptLink().arguments(), (v: any, key: string) => [key, new Ro.Value(v.value)])) as Dictionary<Ro.Value>;
        forEach(keys(args), key => args[key] = enteredFields[key]);

        return this.context.conditionalChoices(field, field.id(), null, args).then(choices => {
            const matches = this.findMatchingChoicesForRef(choices, fieldEntry);
            return this.switchOnMatches(field, fieldEntry, matches);
        });
    }

    private renderFieldDetails(field: Ro.IField, value: Ro.Value): string {
        let s = Msg.fieldName(field.extensions().friendlyName());
        const desc = field.extensions().description();
        s += desc ? `\n${Msg.descriptionFieldPrefix} ${desc}` : "";
        s += `\n${Msg.typePrefix} ${Ro.friendlyTypeName(field.extensions().returnType())}`;
        if (field instanceof Ro.PropertyMember && field.disabledReason()) {
            s += `\n${Msg.unModifiablePrefix(field.disabledReason())}`;
        } else {
            s += field.extensions().optional() ? `\n${Msg.optional}` : `\n${Msg.mandatory}`;
            if (field.choices()) {
                const label = `\n${Msg.choices}: `;
                s += reduce(field.choices(), (s, cho) => s + cho + " ", label);
            }
        }
        return s;
    }
}

export class Forward extends Command {

    fullCommand = Msg.forwardCommand;
    helpText = Msg.forwardHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        this.cvm.clearInput(); //To catch case where can't go any further forward and hence url does not change.
        this.location.forward();
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return this.returnResult("", null, () => this.location.forward());
    };
}

export class Gemini extends Command {

    fullCommand = Msg.geminiCommand;
    helpText = Msg.geminiHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        this.urlManager.gemini();
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return this.returnResult("", "", () => this.urlManager.gemini());
    };
}

export class Goto extends Command {

    fullCommand = Msg.gotoCommand;
    helpText = Msg.gotoHelp;
    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isList();
    }



    doExecute(args: string, chained: boolean): void {
        const arg0 = this.argumentAsString(args, 0);
        if (this.isList()) {
            let itemNo: number;
            try {
                itemNo = this.parseInt(arg0);
            } catch (e) {
                this.clearInputAndSetMessage(e.message);
                return;
            }
            this.getList().then((list: Ro.ListRepresentation) => {
                this.attemptGotoLinkNumber(itemNo, list.value());
                return;
            });
            return;
        }
        if (this.isObject) {

            this.getObject().then((obj: Ro.DomainObjectRepresentation) => {
                if (this.isCollection()) {
                    const itemNo = this.argumentAsNumber(args, 0);
                    const openCollIds = Rend.openCollectionIds(this.routeData());
                    const coll = obj.collectionMember(openCollIds[0]);
                    //Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                    this.context.getCollectionDetails(coll, RtD.CollectionViewState.List, false).then(details => this.attemptGotoLinkNumber(itemNo, details.value()))
                        .catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
                } else {
                    const matchingProps = this.matchingProperties(obj, arg0);
                    const matchingRefProps = filter(matchingProps, (p) => { return !p.isScalar() });
                    const matchingColls = this.matchingCollections(obj, arg0);
                    let s = "";
                    switch (matchingRefProps.length + matchingColls.length) {
                        case 0:
                            s = Msg.noRefFieldMatch(arg0);
                            break;
                        case 1:
                            //TODO: Check for any empty reference
                            if (matchingRefProps.length > 0) {
                                const link = matchingRefProps[0].value().link();
                                this.urlManager.setItem(link);
                            } else { //Must be collection
                                this.openCollection(matchingColls[0]);
                            }
                            break;
                        default:
                            const props = reduce(matchingRefProps,
                                (s, prop) => {
                                    return s + prop.extensions().friendlyName() + "\n";
                                },
                                "");
                            const colls = reduce(matchingColls,
                                (s, coll) => {
                                    return s + coll.extensions().friendlyName() + "\n";
                                },
                                "");
                            s = `Multiple matches for ${arg0}:\n${props}${colls}`;
                    }
                    this.clearInputAndSetMessage(s);
                }
            }).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        }
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        const arg0 = this.argumentAsString(args, 0);
        if (this.isList()) {
            let itemNo: number;
            try {
                itemNo = this.parseInt(arg0);
            } catch (e) {
                return this.returnResult("", e.message);
            }
            return this.getList().then((list: Ro.ListRepresentation) => this.attemptGotoLinkNumber(itemNo, list.value()));
        }
        if (this.isObject) {

            return this.getObject().then((obj: Ro.DomainObjectRepresentation) => {
                if (this.isCollection()) {
                    const itemNo = this.argumentAsNumber(args, 0);
                    const openCollIds = Rend.openCollectionIds(this.routeData());
                    const coll = obj.collectionMember(openCollIds[0]);
                    //Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                    return this.context.getCollectionDetails(coll, RtD.CollectionViewState.List, false).then(details => this.attemptGotoLinkNumber(itemNo, details.value()));

                } else {
                    const matchingProps = this.matchingProperties(obj, arg0);
                    const matchingRefProps = filter(matchingProps, (p) => { return !p.isScalar() });
                    const matchingColls = this.matchingCollections(obj, arg0);

                    switch (matchingRefProps.length + matchingColls.length) {
                        case 0:

                            return this.returnResult("", Msg.noRefFieldMatch(arg0));
                        case 1:
                            //TODO: Check for any empty reference
                            if (matchingRefProps.length > 0) {
                                const link = matchingRefProps[0].value().link();
                                this.urlManager.setItem(link);

                                return this.returnResult("", "", () => this.urlManager.setItem(link));

                            } else { //Must be collection

                                return this.returnResult("", "", () => this.openCollection(matchingColls[0]));
                            }

                        default:
                            const props = reduce(matchingRefProps, (s, prop) => s + prop.extensions().friendlyName() + "\n", "");
                            const colls = reduce(matchingColls, (s, coll) => s + coll.extensions().friendlyName() + "\n", "");
                            const s = `Multiple matches for ${arg0}:\n${props}${colls}`;
                            return this.returnResult("", s);
                    }

                }
            });
        }
    };

    private attemptGotoLinkNumber(itemNo: number, links: Ro.Link[]): Promise<CommandResult> {
        if (itemNo < 1 || itemNo > links.length) {

            return this.returnResult("", Msg.outOfItemRange(itemNo));
        } else {
            const link = links[itemNo - 1]; // On UI, first item is '1'


            return this.returnResult("", "", () => this.urlManager.setItem(link));
        }
    }

    private openCollection(collection: Ro.CollectionMember): void {
        this.closeAnyOpenCollections();
        this.urlManager.setCollectionMemberState(collection.collectionId(), RtD.CollectionViewState.List);
    }
}

export class Help extends Command {

    fullCommand = Msg.helpCommand;
    helpText = Msg.helpHelp;
    protected minArguments = 0;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        const arg = this.argumentAsString(args, 0);
        if (!arg) {
            this.clearInputAndSetMessage(Msg.basicHelp);
        } else if (arg === "?") {
            const commands = this.commandFactory.allCommandsForCurrentContext();
            this.clearInputAndSetMessage(commands);
        } else {
            try {
                const c = this.commandFactory.getCommand(arg);
                this.clearInputAndSetMessage(c.fullCommand + " command:\n" + c.helpText);
            } catch (e) {
                this.clearInputAndSetMessage(e.message);
            }
        }
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        const arg = this.argumentAsString(args, 0);
        if (!arg) {
            return this.returnResult("", Msg.basicHelp);
        } else if (arg === "?") {
            const commands = this.commandFactory.allCommandsForCurrentContext();
            return this.returnResult("", commands);
        } else {
            try {
                const c = this.commandFactory.getCommand(arg);
                return this.returnResult("", `${c.fullCommand} command:\n${c.helpText}`);
            } catch (e) {
                return this.returnResult("", e.message);
            }
        }
    };
}

export class Menu extends Command {

    fullCommand = Msg.menuCommand;
    helpText = Msg.menuHelp;
    protected minArguments = 0;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        const name = this.argumentAsString(args, 0);
        this.context.getMenus()
            .then((menus: Ro.MenusRepresentation) => {
                var links = menus.value();
                if (name) {
                    //TODO: do multi-clause match
                    const exactMatches = filter(links, (t) => { return t.title().toLowerCase() === name; });
                    const partialMatches = filter(links, (t) => { return t.title().toLowerCase().indexOf(name) > -1; });
                    links = exactMatches.length === 1 ? exactMatches : partialMatches;
                }
                switch (links.length) {
                    case 0:
                        this.clearInputAndSetMessage(Msg.doesNotMatchMenu(name));
                        break;
                    case 1:
                        const menuId = links[0].rel().parms[0].value;
                        this.urlManager.setHome();
                        this.urlManager.clearUrlState(1);
                        this.urlManager.setMenu(menuId);
                        break;
                    default:
                        const label = name ? `${Msg.matchingMenus}\n` : `${Msg.allMenus}\n`;
                        const s = reduce(links, (s, t) => { return s + t.title() + "\n"; }, label);
                        this.clearInputAndSetMessage(s);
                }
            });
    }

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        const name = this.argumentAsString(args, 0);
        return this.context.getMenus()
            .then((menus: Ro.MenusRepresentation) => {
                var links = menus.value();
                if (name) {
                    //TODO: do multi-clause match
                    const exactMatches = filter(links, (t) => { return t.title().toLowerCase() === name; });
                    const partialMatches = filter(links, (t) => { return t.title().toLowerCase().indexOf(name) > -1; });
                    links = exactMatches.length === 1 ? exactMatches : partialMatches;
                }
                switch (links.length) {
                    case 0:
                        return this.returnResult("", Msg.doesNotMatchMenu(name));
                    case 1:
                        const menuId = links[0].rel().parms[0].value;
                        this.urlManager.setHome();
                        this.urlManager.clearUrlState(1);
                        return this.returnResult("", "", () => this.urlManager.setMenu(menuId));

                    default:
                        const label = name ? `${Msg.matchingMenus}\n` : `${Msg.allMenus}\n`;
                        const ss = reduce(links, (s, t) => { return s + t.title() + "\n"; }, label);
                        return this.returnResult("", ss);
                }
            });
    };
}

export class OK extends Command {

    fullCommand = Msg.okCommand;
    helpText = Msg.okHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isDialog();
    }

    doExecute(args: string, chained: boolean): void {

        //this.getActionForCurrentDialog().then((action: Models.ActionRepresentation | Models.InvokableActionMember) => {

        //    if (chained && action.invokeLink().method() !== "GET") {
        //        this.mayNotBeChained(Msg.queryOnlyRider);
        //        return;
        //    }
        //    let fieldMap: Dictionary<Ro.Value>;
        //    if (this.isForm()) {
        //        const obj = action.parent as Ro.DomainObjectRepresentation;
        //        fieldMap = this.context.getObjectCachedValues(obj.id()); //Props passed in as pseudo-params to action
        //    } else {
        //        fieldMap = getParametersAndCurrentValue(action, this.context);
        //    }
        //    this.context.invokeAction(action, fieldMap).then((result: Ro.ActionResultRepresentation) => {
        //        // todo handle case where result is empty - this is no longer handled 
        //        // by reject below
        //        const warnings = result.extensions().warnings();
        //        if (warnings) {
        //            forEach(warnings, w => this.cvm.alert += `\nWarning: ${w}`);
        //        }
        //        const messages = result.extensions().messages();
        //        if (messages) {
        //            forEach(messages, m => this.cvm.alert += `\n${m}`);
        //        }
        //        this.urlManager.closeDialogReplaceHistory(""); //TODO provide Id
        //    }).catch((reject: Ro.ErrorWrapper) => {

        //        const display = (em: Ro.ErrorMap) => {
        //            const paramFriendlyName = (paramId: string) => Ro.friendlyNameForParam(action, paramId);
        //            this.handleErrorResponse(em, paramFriendlyName);
        //        };
        //        this.error.handleErrorAndDisplayMessages(reject, display);
        //    });
        //}).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
    }

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return this.getActionForCurrentDialog().then((action: Models.ActionRepresentation | Models.InvokableActionMember) => {

            if (chained && action.invokeLink().method() !== "GET") {
                return this.returnResult("", this.mayNotBeChained(Msg.queryOnlyRider));
            }

            let fieldMap: Dictionary<Ro.Value>;
            if (this.isForm()) {
                const obj = action.parent as Ro.DomainObjectRepresentation;
                fieldMap = this.context.getObjectCachedValues(obj.id()); //Props passed in as pseudo-params to action
            } else {
                fieldMap = getParametersAndCurrentValue(action, this.context);
            }

            return this.context.invokeAction(action, fieldMap).then((result: Ro.ActionResultRepresentation) => {
                // todo handle case where result is empty - this is no longer handled 
                // by reject below
                let alert = "";

                const warnings = result.extensions().warnings();
                if (warnings) {
                    forEach(warnings, w => alert += `\nWarning: ${w}`);
                }
                const messages = result.extensions().messages();
                if (messages) {
                    forEach(messages, m => alert += `\n${m}`);
                }

                return this.returnResult("", alert, () => this.urlManager.closeDialogReplaceHistory(""));

            });
        });
    };
}

export class Page extends Command {
    fullCommand = Msg.pageCommand;
    helpText = Msg.pageHelp;
    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isList();
    }

    doExecute(args: string, chained: boolean): void {
        const arg = this.argumentAsString(args, 0);
        this.getList().then(listRep => {
            const numPages = listRep.pagination().numPages;
            const page = this.routeData().page;
            const pageSize = this.routeData().pageSize;
            if (Msg.pageFirst.indexOf(arg) === 0) {
                this.setPage(1);
                return;
            } else if (Msg.pagePrevious.indexOf(arg) === 0) {
                if (page === 1) {
                    this.clearInputAndSetMessage(Msg.alreadyOnFirst);
                } else {
                    this.setPage(page - 1);
                }
            } else if (Msg.pageNext.indexOf(arg) === 0) {
                if (page === numPages) {
                    this.clearInputAndSetMessage(Msg.alreadyOnLast);
                } else {
                    this.setPage(page + 1);
                }
            } else if (Msg.pageLast.indexOf(arg) === 0) {
                this.setPage(numPages);
            } else {
                const number = parseInt(arg);
                if (isNaN(number)) {
                    this.clearInputAndSetMessage(Msg.pageArgumentWrong);
                    return;
                }
                if (number < 1 || number > numPages) {
                    this.clearInputAndSetMessage(Msg.pageNumberWrong(numPages));
                    return;
                }
                this.setPage(number);
            }
        }).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
    }

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return Promise.reject("Not Implemented");
    };

    private setPage(page: number) {
        const pageSize = this.routeData().pageSize;
        this.urlManager.setListPaging(page, pageSize, RtD.CollectionViewState.List);
    }
}

export class Reload extends Command {

    fullCommand = Msg.reloadCommand;
    helpText = Msg.reloadHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isList();
    }

    doExecute(args: string, chained: boolean): void {
        this.clearInputAndSetMessage("Reload command is not yet implemented");
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return Promise.reject("Not Implemented");
    };
}

export class Root extends Command {

    fullCommand = Msg.rootCommand;
    helpText = Msg.rootHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isCollection();
    }

    doExecute(args: string, chained: boolean): void {
        this.closeAnyOpenCollections();
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return Promise.reject("Not Implemented");
    };
}

export class Save extends Command {

    fullCommand = Msg.saveCommand;
    helpText = Msg.saveHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return this.isEdit() || this.isTransient();
    }

    doExecute(args: string, chained: boolean): void {
        if (chained) {
            this.mayNotBeChained();
            return;
        }
        this.getObject().then((obj: Ro.DomainObjectRepresentation) => {
            const props = obj.propertyMembers();
            const newValsFromUrl = this.context.getObjectCachedValues(obj.id());
            const propIds = new Array<string>();
            const values = new Array<Ro.Value>();
            forEach(props,
                (propMember, propId) => {
                    if (!propMember.disabledReason()) {
                        propIds.push(propId);
                        const newVal = newValsFromUrl[propId];
                        if (newVal) {
                            values.push(newVal);
                        } else if (propMember.value().isNull() &&
                            propMember.isScalar()) {
                            values.push(new Ro.Value(""));
                        } else {
                            values.push(propMember.value());
                        }
                    }
                });
            const propMap = zipObject(propIds, values) as Dictionary<Ro.Value>;
            const mode = obj.extensions().interactionMode();
            const toSave = mode === "form" || mode === "transient";
            const saveOrUpdate = toSave ? this.context.saveObject : this.context.updateObject;

            saveOrUpdate(obj, propMap, 1, true).catch((reject: Ro.ErrorWrapper) => {
                const display = (em: Ro.ErrorMap) => this.handleError(em, obj);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });
        }).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return Promise.reject("Not Implemented");
    };

    private handleError(err: Ro.ErrorMap, obj: Ro.DomainObjectRepresentation) {
        if (err.containsError()) {
            const propFriendlyName = (propId: string) => Ro.friendlyNameForProperty(obj, propId);
            this.handleErrorResponse(err, propFriendlyName);
        } else {
            this.urlManager.setInteractionMode(RtD.InteractionMode.View);
        }
    }
}

export class Selection extends Command {

    fullCommand = Msg.selectionCommand;
    helpText = Msg.selectionHelp;
    protected minArguments = 1;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isList();
    }

    doExecute(args: string, chained: boolean): void {
        //TODO: Add in sub-commands: Add, Remove, All, Clear & Show
        const arg = this.argumentAsString(args, 0);
        const { start, end } = this.parseRange(arg); //'destructuring'
        this.getList().then(list => this.selectItems(list, start, end)).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return Promise.reject("Not Implemented");
    };

    private selectItems(list: Ro.ListRepresentation, startNo: number, endNo: number): void {
        let itemNo: number;
        for (itemNo = startNo; itemNo <= endNo; itemNo++) {
            this.urlManager.setItemSelected(itemNo - 1, true, "");
        }
    }
}

export class Show extends Command {

    fullCommand = Msg.showCommand;
    helpText = Msg.showHelp;
    protected minArguments = 0;
    protected maxArguments = 1;

    isAvailableInCurrentContext(): boolean {
        return this.isObject() || this.isCollection() || this.isList();
    }

    doExecute(args: string, chained: boolean): void {
        if (this.isCollection()) {
            const arg = this.argumentAsString(args, 0, true);
            const { start, end } = this.parseRange(arg);
            this.getObject().then(obj => {
                const openCollIds = Rend.openCollectionIds(this.routeData());
                const coll = obj.collectionMember(openCollIds[0]);
                this.renderCollectionItems(coll, start, end);
            }).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
            return;
        } else if (this.isList()) {
            const arg = this.argumentAsString(args, 0, true);
            const { start, end } = this.parseRange(arg);
            this.getList().then(list => this.renderItems(list, start, end)).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        } else if (this.isObject()) {
            const fieldName = this.argumentAsString(args, 0);
            this.getObject().then((obj: Ro.DomainObjectRepresentation) => {
                const props = this.matchingProperties(obj, fieldName);
                const colls = this.matchingCollections(obj, fieldName);
                //TODO -  include these
                let s: string;
                switch (props.length + colls.length) {
                    case 0:
                        s = fieldName ? Msg.doesNotMatch(fieldName) : Msg.noVisible;
                        break;
                    case 1:
                        s = props.length > 0 ? this.renderPropNameAndValue(props[0]) : Rend.renderCollectionNameAndSize(colls[0]);
                        break;
                    default:
                        s = reduce(props, (s, prop) => s + this.renderPropNameAndValue(prop), "");
                        s += reduce(colls, (s, coll) => s + Rend.renderCollectionNameAndSize(coll), "");
                }
                this.clearInputAndSetMessage(s);
            }).catch((reject: Ro.ErrorWrapper) => this.error.handleError(reject));
        }
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        if (this.isCollection()) {
            const arg = this.argumentAsString(args, 0, true);
            const { start, end } = this.parseRange(arg);
            return this.getObject().then(obj => {
                const openCollIds = Rend.openCollectionIds(this.routeData());
                const coll = obj.collectionMember(openCollIds[0]);
                return this.renderCollectionItems(coll, start, end);
            });
           
        } else if (this.isList()) {
            const arg = this.argumentAsString(args, 0, true);
            const { start, end } = this.parseRange(arg);
            return this.getList().then(list => this.renderItems(list, start, end));
        } else if (this.isObject()) {
            const fieldName = this.argumentAsString(args, 0);
            return this.getObject().then((obj: Ro.DomainObjectRepresentation) => {
                const props = this.matchingProperties(obj, fieldName);
                const colls = this.matchingCollections(obj, fieldName);
                //TODO -  include these
                let s: string;
                switch (props.length + colls.length) {
                case 0:
                    s = fieldName ? Msg.doesNotMatch(fieldName) : Msg.noVisible;
                    break;
                case 1:
                    s = props.length > 0 ? this.renderPropNameAndValue(props[0]) : Rend.renderCollectionNameAndSize(colls[0]);
                    break;
                default:
                    s = reduce(props, (s, prop) => s + this.renderPropNameAndValue(prop), "");
                    s += reduce(colls, (s, coll) => s + Rend.renderCollectionNameAndSize(coll), "");
                }
                return this.returnResult("", s);
            });
        }
    };

    private renderPropNameAndValue(pm: Ro.PropertyMember): string {
        const name = pm.extensions().friendlyName();
        let value: string;
        const parent = pm.parent as Ro.DomainObjectRepresentation;
        const props = this.context.getObjectCachedValues(parent.id());
        const modifiedValue = props[pm.id()];
        if (this.isEdit() && !pm.disabledReason() && modifiedValue) {
            value = Rend.renderFieldValue(pm, modifiedValue, this.mask) + ` (${Msg.modified})`;
        } else {
            value = Rend.renderFieldValue(pm, pm.value(), this.mask);
        }
        return `${name}: ${value}\n`;
    }

    private renderCollectionItems(coll: Ro.CollectionMember, startNo: number, endNo: number) {
        if (coll.value()) {
            return this.renderItems(coll, startNo, endNo);
        } else {
            return this.context.getCollectionDetails(coll, RtD.CollectionViewState.List, false).
                then(details => this.renderItems(details, startNo, endNo));
        }
    }

    private renderItems(source: Ro.IHasLinksAsValue, startNo: number, endNo: number) {
        //TODO: problem here is that unless collections are in-lined value will be null.
        const max = source.value().length;
        if (!startNo) {
            startNo = 1;
        }
        if (!endNo) {
            endNo = max;
        }
        if (startNo > max || endNo > max) {
           
            return this.returnResult("", Msg.highestItem(source.value().length));
        }
        if (startNo > endNo) {
            
            return this.returnResult("", Msg.startHigherEnd);
        }
        let output = "";
        let i: number;
        const links = source.value();
        for (i = startNo; i <= endNo; i++) {
            output += `${Msg.item} ${i}: ${links[i - 1].title()}\n`;
        }
       
        return this.returnResult("", output);
    }
}

export class Where extends Command {

    fullCommand = Msg.whereCommand;
    helpText = Msg.whereHelp;
    protected minArguments = 0;
    protected maxArguments = 0;

    isAvailableInCurrentContext(): boolean {
        return true;
    }

    doExecute(args: string, chained: boolean): void {
        //this.$route.reload(); TODO
    };

    doExecuteNew(args: string, chained: boolean): Promise<CommandResult> {
        return Promise.reject("Not Implemented");
    };
}
