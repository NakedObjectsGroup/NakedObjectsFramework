module NakedObjects.RoInterfaces {

    

    export interface ILink {
        rel?: string;
        href: string;
        type?: string;
        method?: string;
        title?: string;
    }

    export interface IExtensions {
        friendlyName?: string;
        description?: string;
        returnType?: string;
        optional?: boolean;
        hasParams?: boolean;
        elementType?: string;
        domainType?: string;
        pluralName?: string;
        format?: string;
        memberOrder?: number;
        isService?: boolean;
        minLength?: number;
        // custom extensions with "x-ro-nof-" prefix 
        // ReSharper disable InconsistentNaming
        "x-ro-nof-choices"?: Object;
        "x-ro-nof-menuPath"?: string;
        "x-ro-nof-mask"?: string;
        // ReSharper restore InconsistentNaming
    }

    export interface IResourceRepresentation {
        links: ILink[],
        extensions: IExtensions;
    }


    export interface IHomePageRepresentation extends IResourceRepresentation{
 
    }

    export interface IUserRepresentation extends IResourceRepresentation {
        userName: string,
        friendlyName: string,
        email: string,
        roles: string[];
    }

    export interface IDomainServicesRepresentation {
        links: ILink[],
        extensions: IExtensions,
        value: ILink[];
    }

    export interface IOptionalCapabilities {
        blobsClobs: string;
        deleteObjects: string;
        domainModel: string;
        protoPersistentObjects: string;
        validateOnly: string;
    }

    export interface IVersionRepresentation {
        links: ILink[],
        extensions: IExtensions;
        specVersion: string;
        implVersion: string;
        optionalCapabilities: IOptionalCapabilities;
    }

    export interface IValue {
        value: string | number | boolean | ILink;
        invalidReason?: string;
    }

    export interface IObjectOfType {
        members: { [index: string]: IValue };
    }

    export interface IErrorRepresentation {
        links: ILink[];
        extensions: IExtensions;
        message: string;
        stackTrace: string[];
        causedBy: {
            message: string;
            stackTrace: string[];
        };
    }

    export interface IListRepresentation {
        links: ILink[];
        extensions: IExtensions;
        value: ILink[];
    }

    export interface IScalarValueRepresentation {
        links: ILink[];
        extensions: IExtensions;
        value: string | number | boolean;
    }

    export interface IMember {
        links: ILink[];
        extensions: IExtensions;
        memberType: string;
        disabledReason?: string;
    }

    export interface IPropertyMember extends IMember {
        value?: string | number | boolean | ILink;
    }

    export interface ICollectionMember extends IMember {
        value?: ILink[];
        size?: number;
    }

    export interface IActionMember extends IMember {
    }

    export interface IDomainObjectRepresentation {
        links: ILink[];
        extensions: IExtensions;
        domainType: string;
        instanceId: string;
        serviceId: string;
        title: string;
        members: { [index: string]: IMember };
    }

    export interface IActionInvokeRepresentation {
        links: ILink[];
        extensions: IExtensions;
        resultType: string;
        result?: IDomainObjectRepresentation | ListRepresentation | IScalarValueRepresentation;
    }

    export interface IDomainTypeRepresentation {
        links: ILink[];
        extensions: IExtensions;
        name: string;
        domainType: string;
        friendlyName: string;
        pluralName: string;
        description: string;
        isService: string;
        typeActions: string;
        members: { [index: string]: ILink };
    }

    export interface IDomainTypePropertyDescriptionRepresentation {
        links: ILink[];
        extensions: IExtensions;
        id: string;
        friendlyName: string;
        pluralName: string;
        description: string;
        optional: boolean;
        maxlength: number;
        pattern: string;
        memberOrder: string;
        format : string;
        isService: string;
        typeActions: string;
    }

    export interface IDomainTypeCollectionDescriptionRepresentation {
        links: ILink[];
        extensions: IExtensions;
        id: string;
        friendlyName: string;
        pluralForm: string;
        description: string;
        optional: boolean;
        maxlength: number;
        pattern: string;
        memberOrder: string;
        format: string;
        typeActions: string;
    }


}