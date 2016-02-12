module NakedObjects.RoInterfaces.Custom {
 
    

    export interface ICustomExtensions extends RoInterfaces.IExtensions {
        nofChoices?: { [index: string]: (string | number | boolean | ILink)[];}
        nofMenuPath?: string;
        nofMask?: string;
        nofRenderInEditMode?: boolean;
        nofTableViewTitle?: boolean;
        nofTableViewColumns?: string[];
        nofMultipleLines? : number; 
    }  

    export interface IPagination {
        page: number;
        pageSize: number;
        numPages: number;
        totalCount: number;
    }

    export interface ICustomListRepresentation extends RoInterfaces.IListRepresentation {
        pagination? : IPagination;
        members: { [index: string]: IActionMember };
    }

    export interface IMenuRepresentation extends IResourceRepresentation {
        members: { [index: string]: IActionMember };
        title: string;
        menuId: string;
    }
}