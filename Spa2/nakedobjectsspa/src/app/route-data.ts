import * as Models from './models';
import * as _ from 'lodash';
import { ConfigService } from './config.service';
import { LoggerService } from './logger.service';

export type PaneName = "pane1" | "pane2";
export type PaneType = "single" | "split";

export interface ICustomActivatedRouteData {
    pane: number;
    paneType: PaneType;
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
    constructor(
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService
    ) {
        this.pane1 = new PaneRouteData(1, configService.config.doUrlValidation, loggerService);
        this.pane2 = new PaneRouteData(2, configService.config.doUrlValidation, loggerService);
    }

    pane1: PaneRouteData;
    pane2: PaneRouteData;

    pane = (pane: number) => {
        if (pane === 1) { return this.pane1; }
        if (pane === 2) { return this.pane2; }
        this.loggerService.throw('RouteData:pane ${pane} is not a valid pane index on RouteData');
    }
}

interface ICondition {
    condition: (val: any) => boolean,
    name: string;
}

export class PaneRouteData {
    constructor(
        public paneId: number,
        private readonly doUrlValidation: boolean,
        private readonly loggerService: LoggerService) { }

    rawParms: _.Dictionary<string>;
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
            this.loggerService.throw(`PaneRouteData:isValid ${name} is not a valid property on PaneRouteData`);
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
                this.loggerService.throw(`PaneRouteData:assertMustBe Expect that ${name} ${valueCondition.name} when ${context} ${contextCondition.name} within url "${this.validatingUrl}"`);
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
        if (!this.rawParms || !other || !other.rawParms) {
            return false;
        }
        const thisP = _.omit(this.rawParms, "r1");
        const otherP = _.omit(other.rawParms, "r1");

        return  _.isEqual(thisP, otherP);
    }
}

