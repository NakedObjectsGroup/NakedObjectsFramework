namespace NakedObjects.RoInterfaces {
    import ICustomListRepresentation = RoInterfaces.Custom.ICustomListRepresentation;

    export type httpMethodsType = "POST" | "PUT" | "GET" | "DELETE";

    export interface ILink {
        id?: string;
        rel?: string;
        href: string;
        type?: string;
        method?: httpMethodsType;
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
        format?: formatType;
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

    export type scalarValueType = string | number | boolean;
    export type valueType = scalarValueType | ILink;

    export interface IValue {
        value: valueType | valueType[] | Blob;
        invalidReason?: string;
    }

    export interface IValueMap {
        [index: string]: IValue | string;
    }

    export interface IObjectOfType {
        members: IValueMap;
    }

    export interface IPromptMap {
        [index: string]: IValue | string | IValueMap;
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
        value: scalarValueType;
    }

    export type memberTypeType = "action" | "collection" | "property";

    export interface IMember extends IResourceRepresentation {
        memberType: memberTypeType;
        disabledReason?: string;
    }

    export interface IPropertyMember extends IMember {
        value?: valueType;
        choices?: valueType[];
        hasChoices: boolean;
    }

    export interface ICollectionMember extends IMember {
        value?: ILink[];
        size?: number;
    }

    export interface IActionMember extends IMember {
        id: string;
        parameters: { [index: string]: IParameterRepresentation };
    }

    export interface IDomainObjectRepresentation extends IResourceRepresentation {
        domainType?: string;
        instanceId?: string;
        serviceId?: string;
        title: string;
        members: { [index: string]: IMember };
    }

    export type resultTypeType = "object" | "list" | "scalar" | "void";

    export interface IActionInvokeRepresentation extends IResourceRepresentation {
        resultType: resultTypeType;
        result?: IDomainObjectRepresentation | ICustomListRepresentation | IScalarValueRepresentation;
    }

    export interface IParameterRepresentation extends IResourceRepresentation {
        choices?: valueType[];
        default?: valueType;
    }

    export interface IActionRepresentation extends IResourceRepresentation {
        id: string;
        parameters: { [index: string]: IParameterRepresentation };
        disabledReason?: string;
    }

    export interface IPropertyRepresentation extends IResourceRepresentation {
        id: string;
        value?: valueType;
        choices?: valueType[];
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
        choices?: valueType[];
    }
}