module NakedObjects.RoInterfaces.Custom {
 
    export interface ICustomExtensions extends RoInterfaces.IExtensions {
        x_ro_nof_choices?: { [index: string]: (string | number | boolean | ILink)[];}
        x_ro_nof_menuPath?: string;
        x_ro_nof_mask?: string;
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