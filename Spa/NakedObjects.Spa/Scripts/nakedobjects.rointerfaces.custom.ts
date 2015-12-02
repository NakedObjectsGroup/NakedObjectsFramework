module NakedObjects.RoInterfaces.Custom {

 
    export interface IExtensions extends RoInterfaces.IExtensions {
        // ReSharper disable InconsistentNaming
        "x-ro-nof-choices"?: { [index: string]: (string | number | boolean | ILink)[];}
        "x-ro-nof-menuPath"?: string;
        "x-ro-nof-mask"?: string;
        // ReSharper restore InconsistentNaming
    }  

    export interface IPagination {
        page: number;
        pageSize: number;
        numPages: number;
        totalCount: number;
    }

  
    export interface IListRepresentation extends RoInterfaces.IListRepresentation {
        pagination? : IPagination;
    }

  
    export interface IMenuRepresentation extends IResourceRepresentation {
        members: { [index: string]: IActionMember };
        title: string;
        menuId: string;
    }
}