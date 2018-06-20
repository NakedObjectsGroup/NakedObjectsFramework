import * as Ro from './ro-interfaces';

// NOF custom RO constants

export interface ICustomExtensions extends Ro.IExtensions {
    "x-ro-nof-choices"?: { [index: string]: Ro.ValueType[]; };
    "x-ro-nof-menuPath"?: string;
    "x-ro-nof-mask"?: string;
    "x-ro-nof-interactionMode"?: string;
    "x-ro-nof-tableViewTitle"?: boolean;
    "x-ro-nof-tableViewColumns"?: string[];
    "x-ro-nof-multipleLines"?: number;
    "x-ro-nof-warnings"?: string[];
    "x-ro-nof-messages"?: string[];
    "x-ro-nof-dataType"?: string;
    "x-ro-nof-range"?: IRange;
    "x-ro-nof-notNavigable"?: boolean;
    "x-ro-nof-renderEagerly"?: boolean;
    "x-ro-nof-presentationHint"?: string;
}

export interface IRange {
    min: number | string;
    max: number | string;
}

export interface IPagination {
    page: number;
    pageSize: number;
    numPages: number;
    totalCount: number;
}

export interface ICustomListRepresentation extends Ro.IListRepresentation {
    pagination?: IPagination;
    members: { [index: string]: Ro.IActionMember };
}

export interface ICustomLink extends Ro.ILink {
    members: { [index: string]: Ro.IPropertyMember | Ro.ICollectionMember };
}

export interface IMenuRepresentation extends Ro.IResourceRepresentation {
    members: { [index: string]: Ro.IActionMember };
    title: string;
    menuId: string;
}

export interface ICustomCollectionRepresentation extends Ro.ICollectionRepresentation {
    members: { [index: string]: Ro.IActionMember };
}

export interface ICustomCollectionMember extends Ro.ICollectionMember {
    members?: { [index: string]: Ro.IActionMember };
}
