import { ICustomListRepresentation } from './ro-interfaces-custom';
export type HttpMethodsType = "POST" | "PUT" | "GET" | "DELETE";

export interface ILink {
    id?: string;
    rel?: string;
    href: string;
    type?: string;
    method?: HttpMethodsType;
    title?: string;
    arguments?: IValue | IValueMap;
    extensions?: IExtensions;
}

export type FormatType = "string" | "date-time" | "date" | "time" | "utc-millisec" | "big-integer" | "big-decimal" | "blob" | "clob" | "decimal" | "int";

export interface IExtensions {
    friendlyName?: string;
    description?: string;
    returnType?: string;
    optional?: boolean;
    hasParams?: boolean;
    elementType?: string;
    domainType?: string;
    pluralName?: string;
    format?: FormatType;
    memberOrder?: number;
    isService?: boolean;
    minLength?: number;
    maxLength?: number;
    pattern?: string;
}

export interface IDomainObjectExtensions extends IExtensions {
    friendlyName: string;
    description: string;
    domainType: string;
    pluralName: string;
    isService: boolean;
}

export interface IPropertyExtensions extends IExtensions {
    friendlyName: string;
    description: string;
    returnType: string;
    optional: boolean;
    memberOrder: number;
}

export interface ICollectionExtensions extends IExtensions {
    friendlyName: string;
    description: string;
    returnType: string;
    elementType: string;
    pluralName: string;
    memberOrder: number;
}

export interface IActionExtensions extends IExtensions {
    friendlyName: string;
    description: string;
    returnType: string;
    hasParams: boolean;
    memberOrder: number;
}

export interface IParameterExtensions extends IExtensions {
    friendlyName: string;
    description: string;
    returnType: string;
    optional: boolean;
}

// tslint:disable-next-line:no-empty-interface
export interface IRepresentation {
}

export interface IResourceRepresentation extends IRepresentation {
    links: ILink[];
    extensions: IExtensions;
}

// tslint:disable-next-line:no-empty-interface
export interface IHomePageRepresentation extends IResourceRepresentation {
}

export interface IUserRepresentation extends IResourceRepresentation {
    userName: string;
    friendlyName: string;
    email: string;
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
    implVersion?: string;
    optionalCapabilities: IOptionalCapabilities;
}

export type ScalarValueType = string | number | boolean | null;
export type ValueType = ScalarValueType | ILink;

export interface IValue {
    value: ValueType | ValueType[] | Blob | null;
    invalidReason?: string;
}

export interface IValueMap {
    [index: string]: IValue | string;
}

export interface IObjectOfType {
    members: IValueMap;
    "x-ro-invalidReason"?: string;
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
    value: ScalarValueType;
}

export type MemberTypeType = "action" | "collection" | "property";

export interface IMember extends IResourceRepresentation {
    memberType: MemberTypeType;
    disabledReason?: string;
}

export interface IPropertyMember extends IMember {
    value?: ValueType;
    choices?: ValueType[];
    hasChoices: boolean;
    extensions: IPropertyExtensions;
}

export interface ICollectionMember extends IMember {
    value?: ILink[];
    size?: number;
    extensions: ICollectionExtensions;
}

export interface IActionMember extends IMember {
    id: string;
    parameters: { [index: string]: IParameterRepresentation };
    extensions: IActionExtensions;
}

export interface IDomainObjectRepresentation extends IResourceRepresentation {
    domainType?: string;
    instanceId?: string;
    serviceId?: string;
    title: string;
    members: { [index: string]: IPropertyMember | IActionMember | ICollectionMember };
    extensions: IDomainObjectExtensions;
}

export type ResultTypeType = "object" | "list" | "scalar" | "void";

export interface IActionInvokeRepresentation extends IResourceRepresentation {
    resultType: ResultTypeType;
    result?: IDomainObjectRepresentation | ICustomListRepresentation | IScalarValueRepresentation;
}

export interface IParameterRepresentation extends IResourceRepresentation {
    choices?: ValueType[];
    default?: ValueType;
    extensions: IParameterExtensions;
}

export interface IActionRepresentation extends IResourceRepresentation {
    id: string;
    parameters: { [index: string]: IParameterRepresentation };
    disabledReason?: string;
    extensions: IActionExtensions;
}

export interface IPropertyRepresentation extends IResourceRepresentation {
    id: string;
    value?: ValueType;
    choices?: ValueType[];
    disabledReason?: string;
    extensions: IPropertyExtensions;
}

export interface ICollectionRepresentation extends IResourceRepresentation {
    id: string;
    value?: ILink[];
    disabledReason?: string;
    extensions: ICollectionExtensions;
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
    choices?: ValueType[];
}
