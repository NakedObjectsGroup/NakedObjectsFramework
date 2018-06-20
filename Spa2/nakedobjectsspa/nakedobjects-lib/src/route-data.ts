import * as Models from './models';

import { ConfigService } from './config.service';
import { LoggerService } from './logger.service';
import { Dictionary } from 'lodash';
import keys from 'lodash-es/keys';
import isEqual from 'lodash-es/isEqual';

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
        this.pane1 = new PaneRouteData(Pane.Pane1, configService.config.doUrlValidation, loggerService);
        this.pane2 = new PaneRouteData(Pane.Pane2, configService.config.doUrlValidation, loggerService);
    }

    pane1: PaneRouteData;
    pane2: PaneRouteData;

    pane = (pane: Pane) => {
        if (pane === Pane.Pane1) { return this.pane1; }
        if (pane === Pane.Pane2) { return this.pane2; }
        return this.loggerService.throw('RouteData:pane ${pane} is not a valid pane index on RouteData');
    }
}

interface ICondition {
    condition: (val: any) => boolean;
    name: string;
}

export enum Pane {
    Pane1 = 1,
    Pane2 = 2
}

export function getOtherPane(paneId: Pane) {
    return paneId === Pane.Pane1 ? Pane.Pane2 : Pane.Pane1;
}

export class PaneRouteData {
    constructor(
        public paneId: Pane,
        private readonly doUrlValidation: boolean,
        private readonly loggerService: LoggerService) { }

    rawParms: Dictionary<string>;
    rawParmsWithoutReload: Dictionary<string>;
    location: ViewType;
    objectId: string;
    menuId: string;
    collections: Dictionary<CollectionViewState>;
    selectedCollectionItems: Dictionary<boolean[]>;
    actionsOpen: string;
    actionId: string;
    // Note that actionParams applies to executed actions. For dialogs see dialogFields
    // we have both because of contributed actions where we have to distinguish the action parms that
    // created the current list and the the parms for the contributed action

    actionParams: Dictionary<Models.Value>;
    state: CollectionViewState;
    dialogId: string;
    dialogFields: Dictionary<Models.Value>;
    page: number;
    pageSize: number;
    interactionMode: InteractionMode;
    errorCategory: Models.ErrorCategory;
    attachmentId: string;

    private validatingUrl: string;

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
        condition: (val: any) => keys(val).length === 0,
        name: "is an empty map"
    };

    isValid(name: string) {
        if (!this.hasOwnProperty(name)) {
            this.loggerService.throw(`PaneRouteData:isValid ${name} is not a valid property on PaneRouteData`);
        }
    }

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

        return  isEqual(this.rawParms, other.rawParms);
    }

    isEqualIgnoringReload(other: PaneRouteData) {
        if (!this.rawParmsWithoutReload || !other || !other.rawParmsWithoutReload) {
            return false;
        }
        return isEqual(this.rawParmsWithoutReload, other.rawParmsWithoutReload);
    }
}
