module NakedObjects.RoInterfaces {

    export interface ILink {
        rel?: string;
        href: string;
        type?: string;
        method?: string;
        title?: string;
        arguments?: Object;
        extensions?: IExtensions;
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

    export interface IHomePageRepresentation extends IResourceRepresentation {

    }

    export interface IUserRepresentation extends IResourceRepresentation {
        userName: string,
        friendlyName: string,
        email: string,
        roles: string[];
    }

    export interface IDomainServicesRepresentation extends IResourceRepresentation {
        value: ILink[];
    }

    export interface IOptionalCapabilities {
        blobsClobs: string;
        deleteObjects: string;
        domainModel: string;
        protoPersistentObjects: string;
        validateOnly: string;
    }

    export interface IVersionRepresentation extends IResourceRepresentation {
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

    export interface IErrorDetailsRepresentation {
        message: string;
        stackTrace?: string[];
    }

    export interface IErrorRepresentation extends IResourceRepresentation, IErrorDetailsRepresentation {
        causedBy?: IErrorDetailsRepresentation;
    }

    export interface IListRepresentation extends IResourceRepresentation {
        value: ILink[];
        // todo custom
        pagination? : IPagination;
    }

    export interface IScalarValueRepresentation extends IResourceRepresentation {
        value: string | number | boolean;
    }

    export interface IMember extends IResourceRepresentation {
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

    export interface IDomainObjectRepresentation extends IResourceRepresentation {
        domainType: string;
        instanceId: string;
        serviceId: string;
        title: string;
        members: { [index: string]: IMember };
    }

    export interface IActionInvokeRepresentation extends IResourceRepresentation {
        resultType: string;
        result?: IDomainObjectRepresentation | ListRepresentation | IScalarValueRepresentation;
    }

    export interface IDomainTypeRepresentation extends IResourceRepresentation {
        name: string;
        domainType: string;
        friendlyName: string;
        pluralName: string;
        description: string;
        isService: string;
        typeActions: string;
        members: { [index: string]: ILink };
    }

    export interface IDomainTypePropertyDescriptionRepresentation extends IResourceRepresentation {
        id: string;
        friendlyName: string;
        pluralName: string;
        description: string;
        optional: boolean;
        maxlength: number;
        pattern: string;
        memberOrder: string;
        format: string;
        isService: string;
        typeActions: string;
    }

    export interface IDomainTypeCollectionDescriptionRepresentation extends IResourceRepresentation {
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

    export interface IDomainTypeActionInvokeRepresentation extends IResourceRepresentation {
        id: string;
        value: boolean;    
    }
}