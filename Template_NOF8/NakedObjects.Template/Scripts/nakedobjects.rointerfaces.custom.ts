namespace NakedObjects.RoInterfaces.Custom {

    //NOF custom RO constants

    export interface ICustomExtensions extends RoInterfaces.IExtensions {
        "x-ro-nof-choices"?: { [index: string]: valueType[]; };
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
        "x-ro-nof-presentationHint"? : string;
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

    export interface ICustomListRepresentation extends RoInterfaces.IListRepresentation {
        pagination?: IPagination;
        members: { [index: string]: IActionMember };
    }

    export interface ICustomLink extends RoInterfaces.ILink {
        members: { [index: string]: IPropertyMember };
    }

    export interface IMenuRepresentation extends IResourceRepresentation {
        members: { [index: string]: IActionMember };
        title: string;
        menuId: string;
    }

    export interface ICustomCollectionRepresentation extends RoInterfaces.ICollectionRepresentation {
        members: { [index: string]: IActionMember };
    }

    export interface ICustomCollectionMember extends ICollectionMember {
        members? : { [index: string]: IActionMember };
    }
}