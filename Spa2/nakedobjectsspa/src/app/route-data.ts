import * as Models from "./models";
import * as _ from "lodash";
import * as Configservice from './config.service';

export interface ICustomActivatedRouteData {
    pane: number;
    class: string;
    dynamicType?: ViewType.Object | ViewType.List;
}

export enum ViewType {
    Home,
    Object,
    List,
    Error,
    Recent,
    Attachment,
    ApplicationProperties,
    MultiLineDialog
}

export enum CollectionViewState {
    Summary,
    List,
    Table
}

export enum ApplicationMode {
    Gemini,
    Cicero
}

export enum InteractionMode {
    View,
    Edit,
    Transient,
    Form,
    NotPersistent
}

export class RouteData {
    constructor(private readonly configService: Configservice.ConfigService) {
        this.pane1 = new PaneRouteData(1, configService.config.doUrlValidation);
        this.pane2 = new PaneRouteData(2, configService.config.doUrlValidation);
    }

    pane1: PaneRouteData;
    pane2: PaneRouteData;

    pane = (pane: number) => {
        if (pane === 1) { return this.pane1; }
        if (pane === 2) { return this.pane2; }
        throw new Error(`${pane} is not a valid pane index on RouteData`);
    }
}

interface ICondition {
    condition: (val: any) => boolean,
    name: string;
}

export class PaneRouteData {
    constructor(public paneId: number, private readonly doUrlValidation: boolean) { }

    location: ViewType;
    objectId: string;
    menuId: string;
    collections: _.Dictionary<CollectionViewState>;
    selectedCollectionItems: _.Dictionary<boolean[]>;
    actionsOpen: string;
    actionId: string;
    // Note that actionParams applies to executed actions. For dialogs see dialogFields
    // we have both because of contributed actions where we have to distinguish the action parms that 
    // created the current list and the the parms for the contributed action

    actionParams: _.Dictionary<Models.Value>;
    state: CollectionViewState;
    dialogId: string;
    dialogFields: _.Dictionary<Models.Value>;
    page: number;
    pageSize: number;
    interactionMode: InteractionMode;
    errorCategory: Models.ErrorCategory;
    attachmentId: string;

    private validatingUrl: string;

    isValid(name: string) {
        if (!this.hasOwnProperty(name)) {
            throw new Error(`${name} is not a valid property on PaneRouteData`);
        }
    }

    private isNull = {
        condition: (val: any) => !val,
        name: "is null"
    };

    private isNotNull = {
        condition: (val: any) => val,
        name: "is not null"
    };

    private isLength0 = {
        condition: (val: any) => val && val.length === 0,
        name: "is length 0"
    };

    private isEmptyMap = {
        condition: (val: any) => _.keys(val).length === 0,
        name: "is an empty map"
    };

    private assertMustBe(context: string, name: string, contextCondition: ICondition, valueCondition: ICondition) {
        // make sure context and name are valid
        this.isValid(context);
        this.isValid(name);

        if (contextCondition.condition((<any>this)[context])) {
            if (!valueCondition.condition((<any>this)[name])) {
                throw new Error(`Expect that ${name} ${valueCondition.name} when ${context} ${contextCondition.name} within url "${this.validatingUrl}"`);
            }
        }
    }

    assertMustBeEmptyOutsideContext(context: string, name: string) {
        this.assertMustBe(context, name, this.isNull, this.isEmptyMap);
    }

    assertMustBeNullOutsideContext(context: string, name: string) {
        this.assertMustBe(context, name, this.isNull, this.isNull);
    }

    assertMustBeNullInContext(context: string, name: string) {
        this.assertMustBe(context, name, this.isNotNull, this.isNull);
    }

    assertMustBeZeroLengthInContext(context: string, name: string) {
        this.assertMustBe(context, name, this.isNotNull, this.isLength0);
    }

    validate(url: string) {

        this.validatingUrl = url;

        if (this.doUrlValidation) {
            // Can add more conditions here 
            this.assertMustBeNullInContext("objectId", "menuId");
            this.assertMustBeNullInContext("menuId", "objectId");
        }
    }

    isEqual(other: PaneRouteData) {
        return _.isEqual(this, other);
    }
}

