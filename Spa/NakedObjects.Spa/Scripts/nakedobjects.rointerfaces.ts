module NakedObjects.RoInterfaces {
    import ICustomListRepresentation = RoInterfaces.Custom.ICustomListRepresentation;

    export interface ILink {
        id? : string;
        rel?: string;
        href: string;
        type?: string;
        method?: string;
        title?: string;
        arguments?: IValue | IValueMap;
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
        maxLength?: number;
        pattern?: string;
    }

    export interface IRepresentation {
    }

    export interface IResourceRepresentation extends IRepresentation {
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
        value: string | number | boolean | ILink | (string | number | boolean | ILink)[];
        invalidReason?: string;
    }

    export interface IValueMap {
        [index : string] : IValue | string;
    }

    export interface IObjectOfType {
        members: IValueMap;
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
        choices?: (string | number | boolean | ILink)[];
        hasChoices : boolean;
    }

    export interface ICollectionMember extends IMember {
        value?: ILink[];
        size?: number;
    }

    export interface IActionMember extends IMember {
        id : string;
        parameters: { [index: string]: IParameterRepresentation };
    }

    export interface IDomainObjectRepresentation extends IResourceRepresentation {
        domainType?: string;
        instanceId?: string;
        serviceId?: string;
        title: string;
        members: { [index: string]: IMember };
    }

    export interface IActionInvokeRepresentation extends IResourceRepresentation {
        resultType: string;
        result?: IDomainObjectRepresentation | ICustomListRepresentation | IScalarValueRepresentation;
    }

    export interface IParameterRepresentation extends IResourceRepresentation {
        choices?: (string | number | boolean | ILink)[];
        default?: number | string | boolean | ILink;
    }

    export interface IActionRepresentation extends IResourceRepresentation {
        id: string;
        parameters: { [index: string]: IParameterRepresentation };
        disabledReason? : string;
    }

    export interface IPropertyRepresentation extends IResourceRepresentation {
        id: string;
        value?: string | number | boolean | ILink;
        choices?: (string | number | boolean | ILink)[];
        disabledReason?: string;
    }

    export interface ICollectionRepresentation extends IResourceRepresentation {
        id: string;
        value?: ILink[];
        disabledReason?: string;
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

    export interface IPromptRepresentation extends IResourceRepresentation {
        id: string;
        choices? : (string | number | boolean | ILink)[];
    }
}