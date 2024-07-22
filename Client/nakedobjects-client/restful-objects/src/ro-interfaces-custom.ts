import * as Ro from './ro-interfaces';

// NOF custom RO constants

export interface ICustomExtensions extends Ro.IExtensions {
    'x-ro-nof-choices'?: Record<string, Ro.ValueType[]>;
    'x-ro-nof-menuPath'?: string;
    'x-ro-nof-mask'?: string;
    'x-ro-nof-interactionMode'?: string;
    'x-ro-nof-tableViewTitle'?: boolean;
    'x-ro-nof-tableViewColumns'?: string[];
    'x-ro-nof-multipleLines'?: number;
    'x-ro-nof-warnings'?: string[];
    'x-ro-nof-messages'?: string[];
    'x-ro-nof-dataType'?: string;
    'x-ro-nof-range'?: IRange;
    'x-ro-nof-notNavigable'?: boolean;
    'x-ro-nof-renderEagerly'?: boolean;
    'x-ro-nof-presentationHint'?: string;
    'x-ro-nof-editProperties'?: string;
    'x-ro-nof-createNew'?: string;
    'x-ro-nof-urlLink'?: string;
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
    members: Record<string, Ro.IActionMember>;
}

export interface ICustomLink extends Ro.ILink {
    members: Record<string, Ro.IPropertyMember | Ro.ICollectionMember>;
}

export interface IMenuRepresentation extends Ro.IResourceRepresentation {
    members: Record<string, Ro.IActionMember>;
    title: string;
    menuId: string;
}

export interface ICustomCollectionRepresentation extends Ro.ICollectionRepresentation {
    members: Record<string, Ro.IActionMember>;
}

export interface ICustomCollectionMember extends Ro.ICollectionMember {
    members?: Record<string, Ro.IActionMember>;
}
