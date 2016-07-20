/// <reference path="nakedobjects.models.ts" />

namespace NakedObjects {
    import IHasExtensions = Models.IHasExtensions;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ListRepresentation = Models.ListRepresentation;
    import MenuRepresentation = Models.MenuRepresentation;
    import ActionMember = Models.ActionMember;
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import ClientErrorCode = Models.ClientErrorCode;
    import PropertyMember = Models.PropertyMember;
    import CollectionMember = Models.CollectionMember;
    import Parameter = Models.Parameter;
    import Value = Models.Value;
    import ErrorMap = Models.ErrorMap;
    import ErrorValue = Models.ErrorValue;
    import IField = Models.IField;
    import EntryType = Models.EntryType;
    import MenusRepresentation = Models.MenusRepresentation;
    import ActionResultRepresentation = Models.ActionResultRepresentation;
    import IHasLinksAsValue = Models.IHasLinksAsValue;
    import TypePlusTitle = Models.typePlusTitle;
    import FriendlyTypeName = Models.friendlyTypeName;
    import FriendlyNameForParam = Models.friendlyNameForParam;
    import FriendlyNameForProperty = Models.friendlyNameForProperty;
    import isDateOrDateTime = Models.isDateOrDateTime;
    import toDateString = Models.toDateString;
    import ObjectIdWrapper = Models.ObjectIdWrapper;
    import InvokableActionMember = Models.InvokableActionMember;
    import IInvokableAction = Models.IInvokableAction;
    import isIInvokableAction = Models.isIInvokableAction;

    export function getParametersAndCurrentValue(action: ActionMember | IInvokableAction, context: IContext): _.Dictionary<Value> {

        if (isIInvokableAction(action)) {
            const parms = action.parameters();
            const values = context.getCurrentDialogValues(action.actionId());
            return _.mapValues(parms, p => {
                const value = values[p.id()];
                if (value === undefined) {
                    return p.default();
                }
                return value;
            });
        }
        return {};
    }

    export abstract class Command {

        constructor(protected urlManager: IUrlManager,
            protected nglocation: ng.ILocationService,
            protected commandFactory: ICommandFactory,
            protected context: IContext,
            protected navigation: INavigation,
            protected $q: ng.IQService,
            protected $route: ng.route.IRouteService,
            protected mask: IMask,
            protected error: IError
        ) { }

        fullCommand: string;
        helpText: string;
        protected minArguments: number;
        protected maxArguments: number;
        protected vm: ICiceroViewModel;

        //Must be called after construction and before execute is called
        initialiseWithViewModel(cvm: ICiceroViewModel) {
            this.vm = cvm;
        }

        execute(argString: string, chained: boolean): void {
            if (!this.isAvailableInCurrentContext()) {
                this.clearInputAndSetMessage(commandNotAvailable(this.fullCommand));
                return;
            }
            //TODO: This could be moved into a pre-parse method as it does not depend on context
            if (argString == null) {
                if (this.minArguments > 0) {
                    this.clearInputAndSetMessage(noArguments);
                    return;
                }
            } else {
                const args = argString.split(",");
                if (args.length < this.minArguments) {
                    this.clearInputAndSetMessage(tooFewArguments);
                    return;
                } else if (args.length > this.maxArguments) {
                    this.clearInputAndSetMessage(tooManyArguments);
                    return;
                }
            }
            this.doExecute(argString, chained);
        }

        abstract doExecute(args: string, chained: boolean): void;

        abstract isAvailableInCurrentContext(): boolean;

        //Helper methods follow
        protected clearInputAndSetMessage(text: string): void {
            this.vm.clearInput();
            this.vm.message = text;
            this.$route.reload();
        }

        protected mayNotBeChained(rider: string = ""): void {
            this.clearInputAndSetMessage(mayNotbeChainedMessage(this.fullCommand , rider));
        }

        protected appendAsNewLineToOutput(text: string): void {
            this.vm.output.concat(`/n${text}`);
        }

        checkMatch(matchText: string): void {
            if (this.fullCommand.indexOf(matchText) !== 0) {
                throw new Error(noSuchCommand(matchText));
            }
        }

        //argNo starts from 0.
        //If argument does not parse correctly, message will be passed to UI and command aborted.
        protected argumentAsString(argString: string, argNo: number, optional: boolean = false, toLower: boolean = true): string {
            if (!argString) return undefined;
            if (!optional && argString.split(",").length < argNo + 1) {
                throw new Error(tooFewArguments);
            }
            const args = argString.split(",");
            if (args.length < argNo + 1) {
                if (optional) {
                    return undefined;
                } else {
                    throw new Error(missingArgument(argNo + 1));
                }
            }
            return toLower ? args[argNo].trim().toLowerCase() : args[argNo].trim(); // which may be "" if argString ends in a ','
        }

        //argNo starts from 0.
        protected argumentAsNumber(args: string, argNo: number, optional: boolean = false): number {
            const arg = this.argumentAsString(args, argNo, optional);
            if (!arg && optional) return null;
            const number = parseInt(arg);
            if (isNaN(number)) {
                throw new Error(wrongTypeArgument(argNo + 1));
            }
            return number;
        }

        protected parseInt(input: string): number {
            if (!input) {
                return null;
            }
            const number = parseInt(input);
            if (isNaN(number)) {
                throw new Error(isNotANumber(input));
            }
            return number;
        }

        //Parses '17, 3-5, -9, 6-' into two numbers 
        protected parseRange(arg: string): { start: number, end: number } {
            if (!arg) {
                arg = "1-";
            }
            const clauses = arg.split("-");
            const range = { start: null as number, end: null as number };
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
                    throw new Error(tooManyDashes);
            }
            if ((range.start != null && range.start < 1) || (range.end != null && range.end < 1)) {
                throw new Error(mustBeGreaterThanZero);
            }
            return range;
        }

        protected getContextDescription(): string {
            //todo
            return null;
        }

        protected routeData(): PaneRouteData {
            return this.urlManager.getRouteData().pane1;
        }

        //Helpers delegating to RouteData
        protected isHome(): boolean {
            return this.vm.viewType === ViewType.Home;
        }

        protected isObject(): boolean {
            return this.vm.viewType === ViewType.Object;
        }

        protected getObject(): ng.IPromise<DomainObjectRepresentation> {
            const oid =  ObjectIdWrapper.fromObjectId(this.routeData().objectId);
            //TODO: Consider view model & transient modes?

            return this.context.getObject(1, oid, this.routeData().interactionMode).then((obj: DomainObjectRepresentation) => {
                if (this.routeData().interactionMode === InteractionMode.Edit) {
                    return this.context.getObjectForEdit(1, obj);
                } else {
                    return this.$q.when(obj); //To wrap a known object as a promise
                }
            });
        }

        protected isList(): boolean {
            return this.vm.viewType === ViewType.List;
        }

        protected getList(): ng.IPromise<ListRepresentation> {
            const routeData = this.routeData();
            //TODO: Currently covers only the list-from-menu; need to cover list from object action
            return this.context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
        }

        protected isMenu(): boolean {
            return !!this.routeData().menuId;
        }

        protected getMenu(): ng.IPromise<MenuRepresentation> {
            return this.context.getMenu(this.routeData().menuId);
        }

        protected isDialog(): boolean {
            return !!this.routeData().dialogId;
        }

        protected getActionForCurrentDialog(): ng.IPromise<Models.IInvokableAction> {
            const dialogId = this.routeData().dialogId;
            if (this.isObject()) {
                return this.getObject().then((obj: DomainObjectRepresentation) => this.context.getInvokableAction(obj.actionMember(dialogId)));
            } else if (this.isMenu()) {
                return this.getMenu().then((menu: MenuRepresentation) => this.context.getInvokableAction(menu.actionMember(dialogId))); //i.e. return a promise
            }
            return this.$q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.NotImplemented, "List actions not implemented yet"));
        }

        //Tests that at least one collection is open (should only be one). 
        //TODO: assumes that closing collection removes it from routeData NOT sets it to Summary
        protected isCollection(): boolean {
            return this.isObject() && _.some(this.routeData().collections);
        }

        protected closeAnyOpenCollections() {
            const open = openCollectionIds(this.routeData());
            _.forEach(open, id => {
                this.urlManager.setCollectionMemberState(id, CollectionViewState.Summary);
            });
        }

        protected isTable(): boolean {
            return false; //TODO
        }

        protected isEdit(): boolean {
            return this.routeData().interactionMode === InteractionMode.Edit;
        }

        protected isForm(): boolean {
            return this.routeData().interactionMode === InteractionMode.Form;
        }

        protected isTransient(): boolean {
            return this.routeData().interactionMode === InteractionMode.Transient;
        }

        protected matchingProperties(
            obj: DomainObjectRepresentation,
            match: string): PropertyMember[] {
            let props = _.map(obj.propertyMembers(), prop => prop);
            if (match) {
                props = this.matchFriendlyNameAndOrMenuPath(props, match);
            }
            return props;
        }

        protected matchingCollections(
            obj: DomainObjectRepresentation,
            match: string): CollectionMember[] {
            const allColls = _.map(obj.collectionMembers(), action => action);
            if (match) {
                return this.matchFriendlyNameAndOrMenuPath<CollectionMember>(allColls, match);
            } else {
                return allColls;
            }
        }

        protected matchingParameters(action: InvokableActionMember, match: string): Parameter[] {
            let params = _.map(action.parameters(), p => p);
            if (match) {
                params = this.matchFriendlyNameAndOrMenuPath(params, match);
            }
            return params;
        }

        protected matchFriendlyNameAndOrMenuPath<T extends IHasExtensions>(
            reps: T[], match: string): T[] {
            const clauses = match.split(" ");
            //An exact match has preference over any partial match
            const exactMatches = _.filter(reps, (rep) => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return match === name ||
                    (!!path && match === path.toLowerCase() + " " + name) ||
                    _.every(clauses, clause => name === clause || (!!path && path.toLowerCase() === clause));
            });
            if (exactMatches.length > 0) return exactMatches;
            return _.filter(reps, rep => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return _.every(clauses, clause => name.indexOf(clause) >= 0 ||  (!!path && path.toLowerCase().indexOf(clause) >= 0));
            });
        }

        protected findMatchingChoicesForRef(choices: _.Dictionary<Value>, titleMatch: string): Value[] {
            return _.filter(choices, v => v.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0);
        }

        protected findMatchingChoicesForScalar(choices: _.Dictionary<Value>, titleMatch: string): Value[] {
            const labels = _.keys(choices);
            const matchingLabels = _.filter(labels, l => l.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0);
            const result = new Array<Value>();
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
                    _.forEach(matchingLabels, label => result.push(new Value(label)));
                    break;
            }
            return result;
        }

        protected handleErrorResponse(err: ErrorMap, getFriendlyName: (id: string) => string) {
            if (err.invalidReason()) {
                this.clearInputAndSetMessage(err.invalidReason());
                return;
            }
            let msg = pleaseCompleteOrCorrect;
            _.each(err.valuesMap(), (errorValue, fieldId) => {
                msg += this.fieldValidationMessage(errorValue, () => getFriendlyName(fieldId));
            });
            this.clearInputAndSetMessage(msg);
        }

        private fieldValidationMessage(errorValue: ErrorValue, fieldFriendlyName: () => string): string {
            let msg = "";
            const reason = errorValue.invalidReason;
            const value = errorValue.value;
            if (reason) {
                msg +=  `${fieldFriendlyName()}: `;
                if (reason === mandatory) {
                    msg += required;
                } else {
                    msg += `${value} ${reason}`;
                }
                msg += "\n";
            }
            return msg;
        }

        protected valueForUrl(val: Value, field: IField): Value {
            if (val.isNull()) return val;
            const fieldEntryType = field.entryType();

            if (fieldEntryType !== EntryType.FreeForm || field.isCollectionContributed()) {

                if (fieldEntryType === EntryType.MultipleChoices || field.isCollectionContributed()) {
                    let valuesFromRouteData = new Array<Value>();
                    if (field instanceof Parameter) {
                        const rd = getParametersAndCurrentValue(field.parent, this.context)[field.id()];
                        if (rd) valuesFromRouteData = rd.list(); //TODO: what if only one?
                    } else if (field instanceof PropertyMember) {
                        const obj = field.parent as DomainObjectRepresentation;
                        const props = this.context.getCurrentObjectValues(obj.id());
                        const rd = props[field.id()];
                        if (rd) valuesFromRouteData = rd.list(); //TODO: what if only one?
                    }
                    let vals: Value[] = [];
                    if (val.isReference() || val.isScalar()) {
                        vals = new Array<Value>(val);
                    } else if (val.isList()) { //Should be!
                        vals = val.list();
                    }
                    _.forEach(vals, v => {
                        this.addOrRemoveValue(valuesFromRouteData, v);
                    });
                    if (vals[0].isScalar()) { //then all must be scalar
                        const scalars = _.map(valuesFromRouteData, v => v.scalar());
                        return new Value(scalars);
                    } else { //assumed to be links
                        const links = _.map(valuesFromRouteData, v => (
                            { href: v.link().href(), title: v.link().title() }
                        ));
                        return new Value(links);
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
                    return new Value("");
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

        private leanLink(val: Value): Value {
            return new Value({ href: val.link().href(), title: val.link().title() });
        }

        private addOrRemoveValue(valuesFromRouteData: Value[], val: Value) {
            let index: number;
            let valToAdd: Value;
            if (val.isScalar()) {
                valToAdd = val;
                index = _.findIndex(valuesFromRouteData, v => v.scalar() === val.scalar());
            } else { //Must be reference
                valToAdd = this.leanLink(val);
                index = _.findIndex(valuesFromRouteData, v => v.link().href() === valToAdd.link().href());
            }
            if (index > -1) {
                valuesFromRouteData.splice(index, 1);
            } else {
                valuesFromRouteData.push(valToAdd);
            }
        }

        protected setFieldValueInContextAndUrl(field: Parameter, urlVal: Value) {
            this.context.setFieldValue(this.routeData().dialogId, field.id(), urlVal);
            this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
        }

        protected setPropertyValueinContextAndUrl(obj : DomainObjectRepresentation, property : PropertyMember, urlVal: Value) {
            this.context.setPropertyValue(obj, property, urlVal);
            this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
        }
    }

    export class Action extends Command {

        fullCommand = actionCommand;
        helpText = actionHelp;
        protected minArguments = 0;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return (this.isMenu() || this.isObject() || this.isForm()) && !this.isDialog() && !this.isEdit(); //TODO add list
        }

        doExecute(args: string, chained: boolean): void {
            const match = this.argumentAsString(args, 0);
            const details = this.argumentAsString(args, 1, true);
            if (details && details !== "?") {
                this.clearInputAndSetMessage(mustbeQuestionMark);
                return;
            }
            if (this.isObject()) {
                this.getObject()
                    .then((obj: DomainObjectRepresentation) => this.processActions(match, obj.actionMembers(), details))
                    .catch((reject: ErrorWrapper) => this.error.handleError(reject));
            } else if (this.isMenu()) {
                this.getMenu()
                    .then((menu: MenuRepresentation) => this.processActions(match, menu.actionMembers(), details))
                    .catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
            //TODO: handle list - CCAs
        }

        private processActions(match: string, actionsMap: _.Dictionary<ActionMember>, details: string) {
            let actions = _.map(actionsMap, action => action);
            if (actions.length === 0) {
                this.clearInputAndSetMessage(noActionsAvailable);
                return;
            }
            if (match) {
                actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
            }
            switch (actions.length) {
                case 0:
                    this.clearInputAndSetMessage(doesNotMatchActions(match));
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
                    let output = match ? matchingActions : actionsMessage;
                    output += this.listActions(actions);
                    this.clearInputAndSetMessage(output);
            }
        }

        private disabledAction(action: ActionMember) {
            const output = `${actionPrefix} ${action.extensions().friendlyName()} ${isDisabled} ${action.disabledReason()}`;       
            this.clearInputAndSetMessage(output);
        }

        private listActions(actions: ActionMember[]): string {
            return _.reduce(actions, (s, t) => {
                const menupath = t.extensions().menuPath() ? `${t.extensions().menuPath()} - ` : "";
                const disabled = t.disabledReason() ? ` (${disabledPrefix} ${t.disabledReason()})` : "";
                return s + menupath + t.extensions().friendlyName() + disabled + "\n";
            }, "");
        }

        private openActionDialog(action: ActionMember) {
            this.context.clearDialogValues();
            this.urlManager.setDialog(action.actionId());
            this.context.getInvokableAction(action).
                then(invokable => _.forEach(invokable.parameters(), p => this.setFieldValueInContextAndUrl(p, p.default()))).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        private renderActionDetails(action: ActionMember) {
            const s = `${descriptionPrefix} ${action.extensions().friendlyName()}\n${action.extensions().description() || noDescription}`;
            this.clearInputAndSetMessage(s);
        }
    }

    export class Back extends Command {

        fullCommand = backCommand;
        helpText = backHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            this.navigation.back();
        };
    }

    export class Cancel extends Command {

        fullCommand = cancelCommand;
        helpText = cancelHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog() || this.isEdit();
        }

        doExecute(args: string, chained: boolean): void {
            if (this.isEdit()) {
                this.urlManager.setInteractionMode(InteractionMode.View);
            }
            if (this.isDialog()) {
                this.urlManager.closeDialogReplaceHistory();
            }
        };
    }

    export class Clipboard extends Command {

        fullCommand = clipboardCommand;
        helpText = clipboardHelp;

        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const sub = this.argumentAsString(args, 0);
            if (clipboardCopy.indexOf(sub) === 0) {
                this.copy();
            } else if (clipboardShow.indexOf(sub) === 0) {
                this.show();
            } else if (clipboardGo.indexOf(sub) === 0) {
                this.go();
            } else if (clipboardDiscard.indexOf(sub) === 0) {
                this.discard();
            } else {
                this.clearInputAndSetMessage(clipboardError);
            }
        };

        private copy(): void {
            if (!this.isObject()) {
                this.clearInputAndSetMessage(clipboardContextError);
                return;
            }
            this.getObject().
                then(obj => {
                    this.vm.clipboard = obj;
                    this.show();
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        private show(): void {
            if (this.vm.clipboard) {
                const label = TypePlusTitle(this.vm.clipboard);
                this.clearInputAndSetMessage(clipboardContents(label));
            } else {
                this.clearInputAndSetMessage(clipboardEmpty);
            }
        }

        private go(): void {
            const link = this.vm.clipboard.selfLink();
            if (link) {
                this.urlManager.setItem(link);
            } else {
                this.show();
            }
        }

        private discard(): void {
            this.vm.clipboard = null;
            this.show();
        }
    }

    export class Edit extends Command {

        fullCommand = editCommand;
        helpText = editHelp;
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
            this.context.clearObjectValues();
            this.urlManager.setInteractionMode(InteractionMode.Edit);
        };
    }

    export class Enter extends Command {

        fullCommand = enterCommand;
        helpText = enterHelp;
        protected minArguments = 2;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog() || this.isEdit() || this.isTransient() || this.isForm();
        }

        doExecute(args: string, chained: boolean): void {
            const fieldName = this.argumentAsString(args, 0);
            const fieldEntry = this.argumentAsString(args, 1, false, false);
            if (this.isDialog()) {
                this.fieldEntryForDialog(fieldName, fieldEntry);
            } else {
                this.fieldEntryForEdit(fieldName, fieldEntry);
            }
        };

        private fieldEntryForEdit(fieldName: string, fieldEntry: string) {
            this.getObject().
                then(obj => {
                    const fields = this.matchingProperties(obj, fieldName);
                    let s: string;
                    switch (fields.length) {
                        case 0:
                            s = doesNotMatchProperties(fieldName);
                            break;
                        case 1:
                            const field = fields[0];
                            if (fieldEntry === "?") {
                                //TODO: does this work in edit mode i.e. show entered value
                                s = this.renderFieldDetails(field, field.value());
                            } else {
                                this.findAndClearAnyDependentFields(field.id(), obj.propertyMembers());
                                this.setField(field, fieldEntry);
                                return;
                            }
                            break;
                        default:
                            s = `${fieldName} ${matchesMultiple}`;
                            s += _.reduce(fields, (s, prop) => s + prop.extensions().friendlyName() + "\n", "");
                    }
                    this.clearInputAndSetMessage(s);
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        private findAndClearAnyDependentFields(changingField: string, allFields: _.Dictionary<IField>) {

            _.forEach(allFields, field => {
                const promptLink = field.promptLink();

                if (promptLink) {
                    const pArgs = promptLink.arguments();
                    const argNames = _.keys(pArgs);

                    if (argNames.indexOf(changingField.toLowerCase()) >= 0) {
                        this.clearField(field);
                    }
                }
            });
        }

        private fieldEntryForDialog(fieldName: string, fieldEntry: string) {
            this.getActionForCurrentDialog().
                then(action => {
                    //TODO: error -  need to get invokable action to get the params.
                    let params = _.map(action.parameters(), param => param);
                    params = this.matchFriendlyNameAndOrMenuPath(params, fieldName);
                    switch (params.length) {
                        case 0:
                            this.clearInputAndSetMessage(doesNotMatchDialog(fieldName));
                            break;
                        case 1:
                            if (fieldEntry === "?") {
                                const p = params[0];
                                const value = getParametersAndCurrentValue(p.parent, this.context)[p.id()];
                                const s = this.renderFieldDetails(p, value);
                                this.clearInputAndSetMessage(s);
                            } else {
                                this.findAndClearAnyDependentFields(fieldName, action.parameters());
                                this.setField(params[0], fieldEntry);
                            }
                            break;
                        default:
                            this.clearInputAndSetMessage(`${multipleFieldMatches} ${fieldName}`); //TODO: list them
                            break;
                    }
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        private clearField(field: IField): void { 
            this.context.setFieldValue(this.routeData().dialogId, field.id(), new Value(null));

            if (field instanceof Parameter) {
                this.context.setFieldValue(this.routeData().dialogId, field.id(), new Value(null));
            } else if (field instanceof PropertyMember) {
                const parent = field.parent as DomainObjectRepresentation;
                this.context.setPropertyValue(parent, field, new Value(null));             
            }
        }

        private setField(field: IField, fieldEntry: string): void {
            if (field instanceof PropertyMember && field.disabledReason()) {
                this.clearInputAndSetMessage(`${field.extensions().friendlyName()} ${isNotModifiable}`);
                return;
            }
            const entryType = field.entryType();
            switch (entryType) {
                case EntryType.FreeForm:
                    this.handleFreeForm(field, fieldEntry);
                    return;
                case EntryType.AutoComplete:
                    this.handleAutoComplete(field, fieldEntry);
                    return;
                case EntryType.Choices:
                    this.handleChoices(field, fieldEntry);
                    return;
                case EntryType.MultipleChoices:
                    this.handleChoices(field, fieldEntry);
                    return;
                case EntryType.ConditionalChoices:
                    this.handleConditionalChoices(field, fieldEntry);
                    return;
                case EntryType.MultipleConditionalChoices:
                    this.handleConditionalChoices(field, fieldEntry);
                    return;
                default:
                    throw new Error(invalidCase);
            }
        }

        private handleFreeForm(field: IField, fieldEntry: string): void {
            if (field.isScalar()) {
                let value: Value = new Value(fieldEntry);
                //TODO: handle a non-parsable date
                if (isDateOrDateTime(field)) {
                    const m = moment(fieldEntry, supportedDateFormats, "en-GB", true);

                    if (m.isValid()) {
                        const dt = m.toDate();
                        value = new Value(toDateString(dt));
                    }
                }
                this.setFieldValue(field, value);
            } else {
                this.handleReferenceField(field, fieldEntry);
            }
        }

        private setFieldValue(field: IField, value: Value): void {
            const urlVal = this.valueForUrl(value, field);
            if (field instanceof Parameter) {
                this.setFieldValueInContextAndUrl(field, urlVal);
            } else if (field instanceof PropertyMember) {
                const parent = field.parent;
                if (parent instanceof DomainObjectRepresentation) {
                    this.setPropertyValueinContextAndUrl(parent, field, urlVal);
                }
            }
        }

        private handleReferenceField(field: IField, fieldEntry: string): void {
            if (this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            } else {
                this.clearInputAndSetMessage(invalidRefEntry);
            }
        }

        private isPaste(fieldEntry: string) {
            return "paste".indexOf(fieldEntry) === 0;
        }

        private handleClipboard(field: IField): void {
            const ref = this.vm.clipboard;
            if (!ref) {
                this.clearInputAndSetMessage(emptyClipboard);
                return;
            }
            const paramType = field.extensions().returnType();
            const refType = ref.domainType();
            this.context.isSubTypeOf(refType, paramType).
                then(isSubType => {
                    if (isSubType) {
                        const obj = this.vm.clipboard;
                        const selfLink = obj.selfLink();
                        //Need to add a title to the SelfLink as not there by default
                        selfLink.setTitle(obj.title());
                        const value = new Value(selfLink);
                        this.setFieldValue(field, value);
                    } else {
                        this.clearInputAndSetMessage(incompatibleClipboard);
                    }
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        private handleAutoComplete(field: IField, fieldEntry: string): void {
            //TODO: Need to check that the minimum number of characters has been entered or fail validation
            if (!field.isScalar() && this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            } else {
                this.context.autoComplete(field, field.id(), null, fieldEntry).
                    then(choices => {
                        const matches = this.findMatchingChoicesForRef(choices, fieldEntry);
                        this.switchOnMatches(field, fieldEntry, matches);
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
        }

        private handleChoices(field: IField, fieldEntry: string): void {
            let matches: Value[];
            if (field.isScalar()) {
                matches = this.findMatchingChoicesForScalar(field.choices(), fieldEntry);
            } else {
                matches = this.findMatchingChoicesForRef(field.choices(), fieldEntry);
            }
            this.switchOnMatches(field, fieldEntry, matches);
        }

        private switchOnMatches(field: IField, fieldEntry: string, matches: Value[]) {
            switch (matches.length) {
                case 0:
                    this.clearInputAndSetMessage(noMatch(fieldEntry));
                    break;
                case 1:
                    this.setFieldValue(field, matches[0]);
                    break;
                default:
                    let msg = multipleMatches;
                    _.forEach(matches, m => msg += m.toString() + "\n");
                    this.clearInputAndSetMessage(msg);
                    break;
            }
        }

        private getPropertiesAndCurrentValue(obj : DomainObjectRepresentation) : _.Dictionary<Value> {
            const props = obj.propertyMembers();
            const values = _.mapValues(props, p => p.value());
            const modifiedProps = this.context.getCurrentObjectValues(obj.id());

            _.forEach(values, (v, k) => {
                const newValue = modifiedProps[k];
                if (newValue) {
                    values[k] = newValue;
                }
            });
            return _.mapKeys(values, (v, k) => k.toLowerCase());
        }

        private handleConditionalChoices(field: IField, fieldEntry: string): void {
            let enteredFields: _.Dictionary<Value>;

            if (field instanceof Parameter) {
                enteredFields = getParametersAndCurrentValue(field.parent, this.context);
            }

            if (field instanceof PropertyMember) {
                enteredFields = this.getPropertiesAndCurrentValue(field.parent as DomainObjectRepresentation);
            }

            const args = _.fromPairs(_.map(field.promptLink().arguments(), (v: any, key: string) => [key, new Value(v.value)])) as _.Dictionary<Value>;
            _.forEach(_.keys(args), key => args[key] = enteredFields[key]);

            this.context.conditionalChoices(field, field.id(), null, args).
                then(choices => {
                    const matches = this.findMatchingChoicesForRef(choices, fieldEntry);
                    this.switchOnMatches(field, fieldEntry, matches);
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        private renderFieldDetails(field: IField, value: Value): string {
            let s = fieldName(field.extensions().friendlyName());
            const desc = field.extensions().description();
            s += desc ? `\n${descriptionFieldPrefix} ${desc}` : "";
            s += `\n${typePrefix} ${FriendlyTypeName(field.extensions().returnType())}`;
            if (field instanceof PropertyMember && field.disabledReason()) {
                s += `\n${unModifiablePrefix(field.disabledReason())}`;
            } else {
                s += field.extensions().optional() ? `\n${optional}` : `\n${mandatory}`;
                if (field.choices()) {
                    const label = `\n${choices}: `;
                    s += _.reduce(field.choices(), (s, cho) =>  s + cho + " ", label);
                }
            }
            return s;
        }
    }

    export class Forward extends Command {

        fullCommand = forwardCommand;
        helpText = forwardHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            this.vm.clearInput(); //To catch case where can't go any further forward and hence url does not change.
            this.navigation.forward();
        };
    }

    export class Gemini extends Command {

        fullCommand = geminiCommand;
        helpText = geminiHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const newPath = `/gemini/${this.nglocation.path().split("/")[2]}`;
            this.nglocation.path(newPath);
        };
    }

    export class Goto extends Command {

        fullCommand = gotoCommand;
        helpText = gotoHelp;
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
                } catch(e) {
                    this.clearInputAndSetMessage(e.message);
                    return;
                }
                this.getList().then((list: ListRepresentation) => {
                    this.attemptGotoLinkNumber(itemNo, list.value());
                    return;
                });
                return;
            }
            if (this.isObject) {

                this.getObject().
                    then((obj: DomainObjectRepresentation) => {
                        if (this.isCollection()) {
                            const itemNo = this.argumentAsNumber(args, 0);
                            const openCollIds = openCollectionIds(this.routeData());
                            const coll = obj.collectionMember(openCollIds[0]);
                            //Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                            this.context.getCollectionDetails(coll, CollectionViewState.List, false).
                                then(details => this.attemptGotoLinkNumber(itemNo, details.value())).
                                catch((reject: ErrorWrapper) => this.error.handleError(reject));
                        } else {
                            const matchingProps = this.matchingProperties(obj, arg0);
                            const matchingRefProps = _.filter(matchingProps, (p) => { return !p.isScalar() });
                            const matchingColls = this.matchingCollections(obj, arg0);
                            let s = "";
                            switch (matchingRefProps.length + matchingColls.length) {
                                case 0:
                                    s = noRefFieldMatch(arg0);
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
                                    const props = _.reduce(matchingRefProps, (s, prop) => {
                                        return s + prop.extensions().friendlyName() + "\n";
                                    }, "");
                                    const colls = _.reduce(matchingColls, (s, coll) => {
                                        return s + coll.extensions().friendlyName() + "\n";
                                    }, "");
                                    s = `Multiple matches for ${arg0}:\n${props}${colls}`;
                            }
                            this.clearInputAndSetMessage(s);
                        }
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
        };

        private attemptGotoLinkNumber(itemNo: number, links: Models.Link[]): void {
            if (itemNo < 1 || itemNo > links.length) {
                this.clearInputAndSetMessage(outOfItemRange(itemNo));
            } else {
                const link = links[itemNo - 1]; // On UI, first item is '1'
                this.urlManager.setItem(link);
            }
        }

        private openCollection(collection: CollectionMember): void {
            this.closeAnyOpenCollections();
            this.vm.clearInput();
            this.urlManager.setCollectionMemberState(collection.collectionId(), CollectionViewState.List);
        }
    }

    export class Help extends Command {

        fullCommand = helpCommand;
        helpText = helpHelp;
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const arg = this.argumentAsString(args, 0);
            if (!arg ) {
                this.clearInputAndSetMessage(basicHelp);
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
    }

    export class Menu extends Command {

        fullCommand = menuCommand;
        helpText = menuHelp;
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const name = this.argumentAsString(args, 0);
            this.context.getMenus()
                .then((menus: MenusRepresentation) => {
                    var links = menus.value();
                    if (name) {
                        //TODO: do multi-clause match
                        const exactMatches = _.filter(links, (t) => { return t.title().toLowerCase() === name; });
                        const partialMatches = _.filter(links, (t) => { return t.title().toLowerCase().indexOf(name) > -1; });
                        links = exactMatches.length === 1 ? exactMatches : partialMatches;
                    }
                    switch (links.length) {
                        case 0:
                            this.clearInputAndSetMessage(doesNotMatchMenu(name));
                            break;
                        case 1:
                            const menuId = links[0].rel().parms[0].value;
                            this.urlManager.setHome();
                            this.urlManager.clearUrlState();
                            this.urlManager.setMenu(menuId);
                            break;
                        default:
                            const label = name ? `${matchingMenus}\n` : `${allMenus}\n`;
                            const s = _.reduce(links, (s, t) => { return s + t.title() + "\n"; }, label);
                            this.clearInputAndSetMessage(s);
                    }
                });
        }
    }

    export class OK extends Command {

        fullCommand = okCommand;
        helpText = okHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog();
        }

        doExecute(args: string, chained: boolean): void {

            this.getActionForCurrentDialog().
                then((action: IInvokableAction) => {

                    if (chained && action.invokeLink().method() !== "GET") {
                        this.mayNotBeChained(queryOnlyRider);
                        return;
                    }
                    let fieldMap: _.Dictionary<Value>;
                    if (this.isForm()) {
                        const obj = action.parent as DomainObjectRepresentation;
                        fieldMap = this.context.getCurrentObjectValues(obj.id()); //Props passed in as pseudo-params to action
                    } else {
                        fieldMap = getParametersAndCurrentValue(action, this.context);
                    }
                    this.context.invokeAction(action, fieldMap).
                        then((result: ActionResultRepresentation) => {
                            // todo handle case where result is empty - this is no longer handled 
                            // by reject below
                            const warnings = result.extensions().warnings();
                            if (warnings) {
                                _.forEach(warnings, w => this.vm.alert += `\nWarning: ${w}`);
                            }
                            const messages = result.extensions().messages();
                            if (messages) {
                                _.forEach(messages, m => this.vm.alert += `\n${m}`);
                            }
                            this.urlManager.closeDialogReplaceHistory();
                        }).
                        catch((reject: ErrorWrapper) => {

                            const display = (em: ErrorMap) => {
                                const paramFriendlyName = (paramId: string) => FriendlyNameForParam(action, paramId);
                                this.handleErrorResponse(em, paramFriendlyName);
                            };
                            this.error.handleErrorAndDisplayMessages(reject, display);
                        });
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }
    }

    export class Page extends Command {
        fullCommand = pageCommand;
        helpText = pageHelp;
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            const arg = this.argumentAsString(args, 0);
            this.getList().
                then(listRep => {
                    const numPages = listRep.pagination().numPages;
                    const page = this.routeData().page;
                    const pageSize = this.routeData().pageSize;
                    if (pageFirst.indexOf(arg) === 0) {
                        this.setPage(1);
                        return;
                    } else if (pagePrevious.indexOf(arg) === 0) {
                        if (page === 1) {
                            this.clearInputAndSetMessage(alreadyOnFirst);
                        } else {
                            this.setPage(page - 1);
                        }
                    } else if (pageNext.indexOf(arg) === 0) {
                        if (page === numPages) {
                            this.clearInputAndSetMessage(alreadyOnLast);
                        } else {
                            this.setPage(page + 1);
                        }
                    } else if (pageLast.indexOf(arg) === 0) {
                        this.setPage(numPages);
                    } else {
                        const number = parseInt(arg);
                        if (isNaN(number)) {
                            this.clearInputAndSetMessage(pageArgumentWrong);
                            return;
                        }
                        if (number < 1 || number > numPages) {
                            this.clearInputAndSetMessage(pageNumberWrong(numPages));
                            return;
                        }
                        this.setPage(number);
                    }
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        private setPage(page : number) {
            const pageSize = this.routeData().pageSize;
            this.urlManager.setListPaging(page, pageSize, CollectionViewState.List);
        }
    }

    export class Reload extends Command {

        fullCommand = reloadCommand;
        helpText = reloadHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            this.clearInputAndSetMessage("Reload command is not yet implemented");
        };
    }

    export class Root extends Command {

        fullCommand = rootCommand;
        helpText = rootHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection();
        }

        doExecute(args: string, chained: boolean): void {
            this.closeAnyOpenCollections();
        };
    }

    export class Save extends Command {

        fullCommand = saveCommand;
        helpText = saveHelp;
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
            this.getObject().
                then((obj: DomainObjectRepresentation) => {
                    const props = obj.propertyMembers();
                    const newValsFromUrl = this.context.getCurrentObjectValues(obj.id());
                    const propIds = new Array<string>();
                    const values = new Array<Value>();
                    _.forEach(props, (propMember, propId) => {
                        if (!propMember.disabledReason()) {
                            propIds.push(propId);
                            const newVal = newValsFromUrl[propId];
                            if (newVal) {
                                values.push(newVal);
                            } else if (propMember.value().isNull() &&
                                propMember.isScalar()) {
                                values.push(new Value(""));
                            } else {
                                values.push(propMember.value());
                            }
                        }
                    });
                    const propMap = _.zipObject(propIds, values) as _.Dictionary<Value>;
                    const mode = obj.extensions().interactionMode();
                    const toSave = mode === "form" || mode === "transient";
                    const saveOrUpdate = toSave ? this.context.saveObject : this.context.updateObject;

                    saveOrUpdate(obj, propMap, 1, true).
                        catch((reject: ErrorWrapper) => {
                            const display = (em: ErrorMap) => this.handleError(em, obj);
                            this.error.handleErrorAndDisplayMessages(reject, display);
                        });
                }).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        };

        private handleError(err: ErrorMap, obj: DomainObjectRepresentation) {
            if (err.containsError()) {
                const propFriendlyName = (propId: string) => FriendlyNameForProperty(obj, propId);
                this.handleErrorResponse(err, propFriendlyName);
            } else {
                this.urlManager.setInteractionMode(InteractionMode.View);
            }
        }
    }

    export class Selection extends Command {

        fullCommand = selectionCommand;
        helpText = selectionHelp;
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            //TODO: Add in sub-commands: Add, Remove, All, Clear & Show
            const arg = this.argumentAsString(args, 0);
            const { start, end } = this.parseRange(arg); //'destructuring'
            this.getList().
                then(list => this.selectItems(list, start, end)).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        };

        private selectItems(list: ListRepresentation, startNo: number, endNo: number): void {
            let itemNo: number;
            for (itemNo = startNo; itemNo <= endNo; itemNo++) {
                this.urlManager.setListItem(itemNo - 1, true);
            }
        }
    }

    export class Show extends Command {

        fullCommand = showCommand;
        helpText = showHelp;
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isCollection() || this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            if (this.isCollection()) {
                const arg = this.argumentAsString(args, 0, true);
                const { start, end } = this.parseRange(arg);
                this.getObject().
                    then(obj => {
                        const openCollIds = openCollectionIds(this.routeData());
                        const coll = obj.collectionMember(openCollIds[0]);
                        this.renderCollectionItems(coll, start, end);
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
                return;
            }
            else if (this.isList()) {
                const arg = this.argumentAsString(args, 0, true);
                const { start, end } = this.parseRange(arg);
                this.getList().
                    then(list => this.renderItems(list, start, end)).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
            else if (this.isObject()) {
                const fieldName = this.argumentAsString(args, 0);
                this.getObject().
                    then((obj: DomainObjectRepresentation) => {
                        const props = this.matchingProperties(obj, fieldName);
                        const colls = this.matchingCollections(obj, fieldName);
                        //TODO -  include these
                        let s: string;
                        switch (props.length + colls.length) {
                            case 0:
                                s = fieldName ? doesNotMatch(fieldName) : noVisible;
                                break;
                            case 1:
                                s = props.length > 0 ? this.renderPropNameAndValue(props[0]) : renderCollectionNameAndSize(colls[0]);
                                break;
                            default:
                                s = _.reduce(props, (s, prop) => s + this.renderPropNameAndValue(prop), "");
                                s += _.reduce(colls, (s, coll) => s + renderCollectionNameAndSize(coll), "");
                        }
                        this.clearInputAndSetMessage(s);
                    }).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
        };

        private renderPropNameAndValue(pm: PropertyMember): string {
            const name = pm.extensions().friendlyName();
            let value: string;
            const parent = pm.parent as DomainObjectRepresentation;
            const props = this.context.getCurrentObjectValues(parent.id());
            const modifiedValue = props[pm.id()];
            if (this.isEdit() && !pm.disabledReason() && modifiedValue) {
                value = renderFieldValue(pm, modifiedValue, this.mask) + ` (${modified})`;
            } else {
                value = renderFieldValue(pm, pm.value(), this.mask);
            }
            return `${name}: ${value}\n`;
        }

        private renderCollectionItems(coll: CollectionMember, startNo: number, endNo: number) {
            if (coll.value()) {
                this.renderItems(coll, startNo, endNo);
            } else {
                this.context.getCollectionDetails(coll, CollectionViewState.List, false).
                    then(details => this.renderItems(details, startNo, endNo)).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
            }
        }

        private renderItems(source: IHasLinksAsValue, startNo: number, endNo: number): void {
            //TODO: problem here is that unless collections are in-lined value will be null.
            const max = source.value().length;
            if (!startNo) {
                startNo = 1;
            }
            if (!endNo) {
                endNo = max;
            }
            if (startNo > max || endNo > max) {
                this.clearInputAndSetMessage(highestItem(source.value().length));
                return;
            }
            if (startNo > endNo) {
                this.clearInputAndSetMessage(startHigherEnd);
                return;
            }
            let output = "";
            let i: number;
            const links = source.value();
            for (i = startNo; i <= endNo; i++) {
                output += `${item} ${i}: ${links[i - 1].title() }\n`;
            }
            this.clearInputAndSetMessage(output);
        }
    }

    export class Where extends Command {

        fullCommand = whereCommand;
        helpText = whereHelp;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            this.$route.reload();
        };
    }
}