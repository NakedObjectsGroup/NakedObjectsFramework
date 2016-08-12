import * as Nakedobjectsconfig from "./nakedobjects.config";
import * as Nakedobjectsrointerfaces from "./nakedobjects.rointerfaces";
import * as Nakedobjectsconstants from "./nakedobjects.constants";
import * as Nakedobjectsrointerfacescustom from "./nakedobjects.rointerfaces.custom";
import * as Usermessagesconfig from "./user-messages.config";

import * as _ from "lodash";
import * as Maskservice from "./mask.service";
import * as Nakedobjectsviewmodels from "./nakedobjects.viewmodels";
import * as Contextservice from "./context.service";

export function dirtyMarker(context: Contextservice.Context, oid: ObjectIdWrapper) {
    return (Nakedobjectsconfig.showDirtyFlag && context.getIsDirty(oid)) ? "*" : "";
}

export function getOtherPane(paneId: number) {
    return paneId === 1 ? 2 : 1;
}

export function toDateString(dt: Date) {

    const year = dt.getFullYear().toString();
    let month = (dt.getMonth() + 1).toString();
    let day = dt.getDate().toString();

    month = month.length === 1 ? `0${month}` : month;
    day = day.length === 1 ? `0${day}` : day;

    return `${year}-${month}-${day}`;
}

export function toTimeString(dt: Date) {

    let hours = dt.getHours().toString();
    let minutes = dt.getMinutes().toString();
    let seconds = dt.getSeconds().toString();

    hours = hours.length === 1 ? `0${hours}` : hours;
    minutes = minutes.length === 1 ? `0${minutes}` : minutes;
    seconds = seconds.length === 1 ? `0${seconds}` : seconds;


    return `${hours}:${minutes}:${seconds}`;
}

export function getUtcDate(rawDate: string) {
    if (!rawDate || rawDate.length === 0) {
        return null;
    }

    const year = parseInt(rawDate.substring(0, 4));
    const month = parseInt(rawDate.substring(5, 7)) - 1;
    const day = parseInt(rawDate.substring(8, 10));

    if (rawDate.length === 10) {
        return new Date(Date.UTC(year, month, day, 0, 0, 0));
    }

    if (rawDate.length >= 20) {
        const hours = parseInt(rawDate.substring(11, 13));
        const mins = parseInt(rawDate.substring(14, 16));
        const secs = parseInt(rawDate.substring(17, 19));

        return new Date(Date.UTC(year, month, day, hours, mins, secs));
    }

    return null;
}

export function getTime(rawTime: string) {
    if (!rawTime || rawTime.length === 0) {
        return null;
    }

    const hours = parseInt(rawTime.substring(0, 2));
    const mins = parseInt(rawTime.substring(3, 5));
    const secs = parseInt(rawTime.substring(6, 8));

    return new Date(1970, 0, 1, hours, mins, secs);
}


export function isDateOrDateTime(rep: IHasExtensions) {
    const returnType = rep.extensions().returnType();
    const format = rep.extensions().format();

    return (returnType === "string" && ((format === "date-time") || (format === "date")));
}

export function isTime(rep: IHasExtensions) {
    const returnType = rep.extensions().returnType();
    const format = rep.extensions().format();

    return returnType === "string" && format === "time";
}

export function toUtcDate(value: Value) {
    const rawValue = value ? value.toString() : "";
    const dateValue = getUtcDate(rawValue);
    return dateValue ? dateValue : null;
}

export function toTime(value: Value) {
    const rawValue = value ? value.toString() : "";
    const dateValue = getTime(rawValue);
    return dateValue ? dateValue : null;
}


export function compress(toCompress: string) {
    if (toCompress) {
        _.forEach(Nakedobjectsconfig.urlShortCuts, (sc, i) => toCompress = toCompress.replace(sc, `${Nakedobjectsconfig.shortCutMarker}${i}`));
    }
    return toCompress;
}

export function decompress(toDecompress: string) {
    if (toDecompress) {
        _.forEach(Nakedobjectsconfig.urlShortCuts, (sc, i) => toDecompress = toDecompress.replace(`${Nakedobjectsconfig.shortCutMarker}${i}`, sc));
    }
    return toDecompress;
}

export function getClassName(obj: any) {
    const funcNameRegex = /function (.{1,})\(/;
    const results = (funcNameRegex).exec(obj.constructor.toString());
    return (results && results.length > 1) ? results[1] : "";
}

export function typeFromUrl(url: string) {
    const typeRegex = /(objects|services)\/([\w|\.]+)/;
    const results = (typeRegex).exec(url);
    return (results && results.length > 2) ? results[2] : "";
}

export function idFromUrl(href: string) {
    const urlRegex = /(objects|services)\/(.*?)\/([^\/]*)/;
    const results = (urlRegex).exec(href);
    return (results && results.length > 3) ? results[3] : "";
}

export function propertyIdFromUrl(href: string) {
    const urlRegex = /(objects)\/(.*)\/(.*)\/(properties)\/(.*)/;
    const results = (urlRegex).exec(href);
    return (results && results.length > 5) ? results[5] : "";
}

export function friendlyTypeName(fullName: string) {
    const shortName = _.last(fullName.split("."));
    const result = shortName.replace(/([A-Z])/g, " $1").trim();
    return result.charAt(0).toUpperCase() + result.slice(1);
}

export function friendlyNameForParam(action: IInvokableAction, parmId: string) {
    const param = _.find(action.parameters(), (p) => p.id() === parmId);
    return param.extensions().friendlyName();
}

export function friendlyNameForProperty(obj: DomainObjectRepresentation, propId: string) {
    const prop = obj.propertyMember(propId);
    return prop.extensions().friendlyName();
}

export function typePlusTitle(obj: DomainObjectRepresentation) {
    const type = obj.extensions().friendlyName();
    const title = obj.title();
    return type + ": " + title;
}

function isInteger(value: number) {
    return typeof value === "number" && isFinite(value) && Math.floor(value) === value;
}


function validateNumber(model: IHasExtensions, newValue: number, filter: Maskservice.ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
        case ("int"):
            if (!isInteger(newValue)) {
                return "Not an integer";
            }
    }

    const range = model.extensions().range();

    if (range) {
        const min = range.min;
        const max = range.max;

        if (min && newValue < min) {
            return Usermessagesconfig.outOfRange(newValue, min, max, filter);
        }

        if (max && newValue > max) {
            return Usermessagesconfig.outOfRange(newValue, min, max, filter);
        }
    }

    return "";
}

function validateStringFormat(model: IHasExtensions, newValue: string): string {

    const maxLength = model.extensions().maxLength();
    const pattern = model.extensions().pattern();
    const len = newValue ? newValue.length : 0;

    if (maxLength && len > maxLength) {
        return Usermessagesconfig.tooLong;
    }

    if (pattern) {
        const regex = new RegExp(pattern);
        return regex.test(newValue) ? "" : Usermessagesconfig.noPatternMatch;
    }
    return "";
}

function validateDateTimeFormat(model: IHasExtensions, newValue: Date): string {
    return "";
}

function getDate(val: string) : Date {
    //const dt1 = moment(val, "YYYY-MM-DD", "en-GB", true);

    //if (dt1.isValid()) {
    //    return dt1.toDate();
    //}
    return null;
}


function validateDateFormat(model: IHasExtensions, newValue: Date, filter: Maskservice.ILocalFilter): string {
    const range = model.extensions().range();

    if (range && newValue) {
        const min = range.min ? getDate(range.min as string) : null;
        const max = range.max ? getDate(range.max as string) : null;

        if (min && newValue < min) {
            return Usermessagesconfig.outOfRange(toDateString(newValue), getUtcDate(range.min as string), getUtcDate(range.max as string), filter);
        }

        if (max && newValue > max) {
            return Usermessagesconfig.outOfRange(toDateString(newValue), getUtcDate(range.min as string), getUtcDate(range.max as string), filter);
        }
    }

    return "";
}

function validateTimeFormat(model: IHasExtensions, newValue: Date): string {
    return "";
}

function validateString(model: IHasExtensions, newValue: any, filter: Maskservice.ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
        case ("string"):
            return validateStringFormat(model, newValue as string);
        case ("date-time"):
            return validateDateTimeFormat(model, newValue as Date);
        case ("date"):
            return validateDateFormat(model, newValue as Date, filter);
        case ("time"):
            return validateTimeFormat(model, newValue as Date);
        default:
            return "";
    }
}


export function validateMandatory(model: IHasExtensions, viewValue: string | Nakedobjectsviewmodels.ChoiceViewModel): string {
    // first check 
    const isMandatory = !model.extensions().optional();

    if (isMandatory && (viewValue === "" || viewValue == null)) {
        return Usermessagesconfig.mandatory;
    }

    return "";
}


export function validate(model: IHasExtensions, modelValue: any, viewValue: string, filter: Maskservice.ILocalFilter): string {
    // first check 

    const mandatory = validateMandatory(model, viewValue);

    if (mandatory) {
        return mandatory;
    }

    // if optional but empty always valid 
    if (modelValue == null || modelValue === "") {
        return "";
    }

    // check type 
    const returnType = model.extensions().returnType();

    switch (returnType) {
        case ("number"):
            //if (!$.isNumeric(modelValue)) {
            //    return Usermessagesconfig.notANumber;
            //}
            return validateNumber(model, parseFloat(modelValue), filter);
        case ("string"):
            return validateString(model, modelValue, filter);
        case ("boolean"):
            return "";
        default:
            return "";
    }
}


// helper functions 

function isScalarType(typeName: string) {
    return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
}

function isListType(typeName: string) {
    return typeName === "list";
}

function emptyResource(): Nakedobjectsrointerfaces.IResourceRepresentation {
    return { links: [] as Nakedobjectsrointerfaces.ILink[], extensions: {} };
}

function isILink(object: any): object is Nakedobjectsrointerfaces.ILink {
    return object && object instanceof Object && "href" in object;
}

function isIObjectOfType(object: any): object is Nakedobjectsrointerfaces.IObjectOfType {
    return object && object instanceof Object && "members" in object;
}

function isIValue(object: any): object is Nakedobjectsrointerfaces.IValue {
    return object && object instanceof Object && "value" in object;
}

export function isResourceRepresentation(object: any): object is Nakedobjectsrointerfaces.IResourceRepresentation {
    return object && object instanceof Object && "links" in object && "extensions" in object;
}

export function isErrorRepresentation(object: any): object is Nakedobjectsrointerfaces.IErrorRepresentation {
    return isResourceRepresentation(object) && "message" in object;
}

export function isIDomainObjectRepresentation(object: any): object is Nakedobjectsrointerfaces.IDomainObjectRepresentation {
    return isResourceRepresentation(object) && "domainType" in object && "instanceId" in object && "members" in object;
}

export function isIInvokableAction(object: any): object is IInvokableAction {
    return object && "parameters" in object && "extensions" in object;
}

function getId(prop: PropertyRepresentation | PropertyMember) {
    if (prop instanceof PropertyRepresentation) {
        return prop.instanceId();
    } else {
        return (prop as PropertyMember).id();
    }
}

function wrapLinks(links: Nakedobjectsrointerfaces.ILink[]) {
    return _.map(links, l => new Link(l));
}

function getLinkByRel(links: Link[], rel: Rel) {
    return _.find(links, i => i.rel().uniqueValue === rel.uniqueValue);
}

function linkByRel(links: Link[], rel: string) {
    return getLinkByRel(links, new Rel(rel));
}

function linkByNamespacedRel(links: Link[], rel: string) {
    return getLinkByRel(links, new Rel(`urn:org.restfulobjects:rels/${rel}`));
}


// interfaces 

export interface IHateoasModel {
    etagDigest: string;
    hateoasUrl: string;
    method: Nakedobjectsrointerfaces.httpMethodsType;
    populate(wrapped: Nakedobjectsrointerfaces.IRepresentation): void;
    getBody(): Nakedobjectsrointerfaces.IRepresentation;
    getUrl(): string;
}

export enum ErrorCategory {
    HttpClientError,
    HttpServerError,
    ClientError
}

export enum HttpStatusCode {
    NoContent = 204,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    NotAcceptable = 406,
    PreconditionFailed = 412,
    UnprocessableEntity = 422,
    PreconditionRequired = 428,
    InternalServerError = 500
}

export enum ClientErrorCode {
    ExpiredTransient,
    WrongType,
    NotImplemented,
    SoftwareError,
    ConnectionProblem = -1
}

export class ErrorWrapper {
    constructor(rc: ErrorCategory, code: HttpStatusCode | ClientErrorCode, err: string | ErrorMap | ErrorRepresentation) {
        this.category = rc;

        if (rc === ErrorCategory.ClientError) {
            this.clientErrorCode = code as ClientErrorCode;
            this.errorCode = ClientErrorCode[this.clientErrorCode];
            let description = Usermessagesconfig.errorUnknown;

            switch (this.clientErrorCode) {
                case ClientErrorCode.ExpiredTransient:
                    description = Usermessagesconfig.errorExpiredTransient;
                    break;
                case ClientErrorCode.WrongType:
                    description = Usermessagesconfig.errorWrongType;
                    break;
                case ClientErrorCode.NotImplemented:
                    description = Usermessagesconfig.errorNotImplemented;
                    break;
                case ClientErrorCode.SoftwareError:
                    description = Usermessagesconfig.errorSoftware;
                    break;
                case ClientErrorCode.ConnectionProblem:
                    description = Usermessagesconfig.errorConnection;
                    break;
            }

            this.description = description;
            this.title = Usermessagesconfig.errorClient;
        }

        if (rc === ErrorCategory.HttpClientError || rc === ErrorCategory.HttpServerError) {
            this.httpErrorCode = code as HttpStatusCode;
            this.errorCode = `${HttpStatusCode[this.httpErrorCode]}(${this.httpErrorCode})`;

            this.description = rc === ErrorCategory.HttpServerError
                ? "A software error has occurred on the server"
                : "An HTTP error code has been received from the server\n" +
                "You can look up the meaning of this code in the Restful Objects specification.";

            this.title = "Error message received from server";
        }

        if (err instanceof ErrorMap) {
            const em = err as ErrorMap;
            this.message = em.invalidReason() || em.warningMessage;
            this.error = em;
            this.stackTrace = [];
        } else if (err instanceof ErrorRepresentation) {
            const er = err as ErrorRepresentation;
            this.message = er.message();
            this.error = er;
            this.stackTrace = err.stackTrace();
        } else {
            this.message = (err as string);
            this.error = null;
            this.stackTrace = [];
        }
    }

    title: string;
    description: string;
    errorCode: string;
    httpErrorCode: HttpStatusCode;
    clientErrorCode: ClientErrorCode;

    category: ErrorCategory;
    message: string;
    error: ErrorMap | ErrorRepresentation;

    stackTrace: string[];

    handled = false;
}

// abstract classes 

function toOid(id: string[]) {
    return _.reduce(id, (a, v) => `${a}${a ? Nakedobjectsconfig.keySeparator : ""}${v}`, "");
}

export class ObjectIdWrapper {

    domainType: string;
    instanceId: string;
    splitInstanceId: string[];
    isService: boolean;

    getKey() {
        return this.domainType + Nakedobjectsconfig.keySeparator + this.instanceId;
    }

    static safeSplit(id: string) {
        if (id) {
            return id.split(Nakedobjectsconfig.keySeparator);
        }
        return [];
    }

    static fromObject(object: DomainObjectRepresentation) {
        const oid = new ObjectIdWrapper();
        oid.domainType = object.domainType();
        oid.instanceId = object.instanceId();
        oid.splitInstanceId = this.safeSplit(oid.instanceId);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromLink(link: Link) {
        const href = link.href();
        return this.fromHref(href);
    }

    static fromHref(href: string) {
        const oid = new ObjectIdWrapper();
        oid.domainType = typeFromUrl(href);
        oid.instanceId = idFromUrl(href);
        oid.splitInstanceId = this.safeSplit(oid.instanceId);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromObjectId(objectId: string) {
        const oid = new ObjectIdWrapper();
        const [dt, ...id] = objectId.split(Nakedobjectsconfig.keySeparator);
        oid.domainType = dt;
        oid.splitInstanceId = id;
        oid.instanceId = toOid(id);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromRaw(dt: string, id: string) {
        const oid = new ObjectIdWrapper();
        oid.domainType = dt;
        oid.instanceId = id;
        oid.splitInstanceId = this.safeSplit(oid.instanceId);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromSplitRaw(dt: string, id: string[]) {
        const oid = new ObjectIdWrapper();
        oid.domainType = dt;
        oid.splitInstanceId = id;
        oid.instanceId = toOid(id);
        oid.isService = !oid.instanceId;
        return oid;
    }

    isSame(other: ObjectIdWrapper) {
        return other && other.domainType === this.domainType && other.instanceId === this.instanceId;
    }

}


export abstract class HateosModel implements IHateoasModel {

    etagDigest: string;
    hateoasUrl = "";
    method: Nakedobjectsrointerfaces.httpMethodsType = "GET";
    urlParms: _.Dictionary<Object>;

    constructor(protected model?: Nakedobjectsrointerfaces.IRepresentation) {
    }

    populate(model: Nakedobjectsrointerfaces.IRepresentation) {
        this.model = model;
    }

    getBody(): Nakedobjectsrointerfaces.IRepresentation {
        if (this.method === "POST" || this.method === "PUT") {
            const m = _.clone(this.model);
            const up = _.clone(this.urlParms);
            return _.merge(m, up);
        }

        return {};
    }

    getUrl() : string {
        const url = this.hateoasUrl;
        const attrAsJson = _.clone(this.model);

        if (this.method === "GET" || this.method === "DELETE") {

            if (_.keys(attrAsJson).length > 0) {
                // there are model parms so encode everything into json 

                const urlParmsAsJson = _.clone(this.urlParms);
                const asJson = _.merge(attrAsJson, urlParmsAsJson);
                if (_.keys(asJson).length > 0) {
                    const map = JSON.stringify(asJson);
                    const parmString = encodeURI(map);
                    return url + "?" + parmString;
                }
                return url;
            }
            if (_.keys(this.urlParms).length > 0) {
                // there are only url reserved parms so they can just be appended to url
                const urlParmString = _.reduce(this.urlParms, (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n, "");

                return url + "?" + urlParmString;
            }
        }

        return url;
    }

    setUrlParameter(name: string, value: Object) {
        this.urlParms = this.urlParms || {};
        this.urlParms[name] = value;
    }
}

export abstract class ArgumentMap extends HateosModel {

    constructor(public map: Nakedobjectsrointerfaces.IValueMap, public id: string) {
        super(map);
    }

    populate(wrapped: Nakedobjectsrointerfaces.IValueMap) {
        super.populate(wrapped);
    }
}

export abstract class NestedRepresentation<T extends Nakedobjectsrointerfaces.IResourceRepresentation> {

    protected resource = () => this.model as T;

    constructor(private model: T) { }

    private lazyLinks: Link[];

    links(): Link[] {
        this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
        return this.lazyLinks;
    }

    protected update(newResource: T) {
        this.model = newResource;
        this.lazyLinks = null;
    }

    private lazyExtensions: Extensions;

    extensions(): Extensions {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.model.extensions);
        return this.lazyExtensions;
    }
}

// classes

export class RelParm {

    name: string;
    value: string;

    constructor(public asString: string) {
        this.decomposeParm();
    }

    private decomposeParm() {
        const regex = /(\w+)\W+(\w+)\W+/;
        const result = regex.exec(this.asString);
        [, this.name, this.value] = result;
    }
}

export class Rel {

    ns = "";
    uniqueValue: string;
    parms: RelParm[] = [];

    constructor(public asString: string) {
        this.decomposeRel();
    }

    private decomposeRel() {

        let postFix: string;

        if (this.asString.substring(0, 3) === "urn") {
            // namespaced 
            this.ns = this.asString.substring(0, this.asString.indexOf("/") + 1);
            postFix = this.asString.substring(this.asString.indexOf("/") + 1);
        } else {
            postFix = this.asString;
        }
        const splitPostFix = postFix.split(";");
        this.uniqueValue = splitPostFix[0];

        if (splitPostFix.length > 1) {
            this.parms = _.map(splitPostFix.slice(1), s => new RelParm(s));
        }
    }
}

export class MediaType {

    applicationType: string;
    profile: string;
    xRoDomainType: string;
    representationType: string;
    domainType: string;

    constructor(public asString: string) {
        this.decomposeMediaType();
    }

    private decomposeMediaType() {
        const parms = this.asString.split(";");
        if (parms.length > 0) {
            this.applicationType = parms[0];
        }

        for (let i = 1; i < parms.length; i++) {
            if (parms[i].trim().substring(0, 7) === "profile") {
                this.profile = parms[i].trim();
                const profileValue = (this.profile.split("=")[1].replace(/\"/g, "")).trim();
                this.representationType = (profileValue.split("/")[1]).trim();
            }
            if (parms[i].trim().substring(0, 16) === Nakedobjectsconstants.roDomainType) {
                this.xRoDomainType = (parms[i]).trim();
                this.domainType = (this.xRoDomainType.split("=")[1].replace(/\"/g, "")).trim();
            }
        }
    }
}

export class Value {

    // note this is different from constructor parm as we wrap ILink
    private wrapped: Link | Array<Link | Nakedobjectsrointerfaces.valueType> | Nakedobjectsrointerfaces.scalarValueType | Blob;

    constructor(raw: Link | Array<Link | Nakedobjectsrointerfaces.valueType> | Nakedobjectsrointerfaces.valueType | Blob) {
        // can only be Link, number, boolean, string or null    

        if (raw instanceof Array) {
            this.wrapped = raw as Array<Link | Nakedobjectsrointerfaces.valueType>;
        } else if (raw instanceof Link) {
            this.wrapped = raw;
        } else if (isILink(raw)) {
            this.wrapped = new Link(raw);
        } else {
            this.wrapped = raw;
        }
    }

    isBlob(): boolean {
        return this.wrapped instanceof Blob;
    }

    isScalar(): boolean {
        return !this.isReference() && !this.isList();
    }

    isReference(): boolean {
        return this.wrapped instanceof Link;
    }

    isFileReference(): boolean {
        return this.wrapped instanceof Link && this.link().href().indexOf("data") === 0;
    }

    isList(): boolean {
        return this.wrapped instanceof Array;
    }

    isNull(): boolean {
        return this.wrapped == null;
    }

    blob(): Blob {
        return this.isBlob() ? <Blob>this.wrapped : null;
    }

    link(): Link {
        return this.isReference() ? <Link>this.wrapped : null;
    }

    href(): string {
        return this.link() ? this.link().href() : null;
    }

    scalar(): Nakedobjectsrointerfaces.scalarValueType {
        return this.isScalar() ? this.wrapped as Nakedobjectsrointerfaces.scalarValueType : null;
    }

    list(): Value[] {
        return this.isList() ? _.map(this.wrapped as Array<Link | Nakedobjectsrointerfaces.valueType>, i => new Value(i)) : null;
    }

    toString(): string {
        if (this.isReference()) {
            return this.link().title();
        }

        if (this.isList()) {
            const ss = _.map(this.list(), v => v.toString());
            return ss.length === 0 ? "" : _.reduce(ss, (m, s) => m + "-" + s, "");
        }

        return (this.wrapped == null) ? "" : this.wrapped.toString();
    }

    compress() {
        if (this.isReference()) {
            this.link().compress();
        }
        if (this.isList()) {
            _.forEach(this.list(), i => i.compress());
        };

        if (this.scalar() && this.wrapped instanceof String) {
            this.wrapped = compress(this.wrapped as string);
        }
    }

    decompress() {
        if (this.isReference()) {
            this.link().decompress();
        }
        if (this.isList()) {
            _.forEach(this.list(), i => i.decompress());
        };

        if (this.scalar() && this.wrapped instanceof String) {
            this.wrapped = decompress(this.wrapped as string);
        }
    }

    static fromJsonString(jsonString: string): Value {
        const value = new Value(JSON.parse(jsonString));
        value.decompress();
        return value;
    }

    toValueString(): string {
        if (this.isReference()) {
            return this.link().href();
        }
        return (this.wrapped == null) ? "" : this.wrapped.toString();
    }

    toJsonString(): string {

        const cloneThis = _.cloneDeep(this) as Value;
        cloneThis.compress();
        const value = cloneThis.wrapped;
        const raw = (value instanceof Link) ? value.wrapped : value;
        return JSON.stringify(raw);
    }

    setValue(target: Nakedobjectsrointerfaces.IValue) {
        if (this.isFileReference()) {
            target.value = this.link().wrapped;
        }
        else if (this.isReference()) {
            target.value = { "href": this.link().href() };
        } else if (this.isList()) {
            target.value = _.map(this.list(), (v) => v.isReference() ? { "href": v.link().href() } : v.scalar());
        }
        else if (this.isBlob()) {
            target.value = this.blob();
        }
        else {
            target.value = this.scalar();
        }
    }

    set(target: _.Dictionary<Nakedobjectsrointerfaces.IValue | string>, name: string) {
        const t = target[name] = <Nakedobjectsrointerfaces.IValue>{ value: null };
        this.setValue(t);
    }
}

export class ErrorValue {
    constructor(public value: Value, public invalidReason: string) { }
}

export class Result {
    constructor(public wrapped: Nakedobjectsrointerfaces.IDomainObjectRepresentation | Nakedobjectsrointerfacescustom.ICustomListRepresentation | Nakedobjectsrointerfaces.IScalarValueRepresentation, private resultType: Nakedobjectsrointerfaces.resultTypeType) { }

    object(): DomainObjectRepresentation {
        if (!this.isNull() && this.resultType === "object") {
            const dor = new DomainObjectRepresentation();
            dor.populate(this.wrapped as Nakedobjectsrointerfaces.IDomainObjectRepresentation);
            return dor;
        }
        return null;
    }

    list(): ListRepresentation {
        if (!this.isNull() && this.resultType === "list") {
            const lr = new ListRepresentation();
            lr.populate(this.wrapped as Nakedobjectsrointerfacescustom.ICustomListRepresentation);
            return lr;
        }
        return null;
    }

    scalar(): ScalarValueRepresentation {
        if (!this.isNull() && this.resultType === "scalar") {
            return new ScalarValueRepresentation(this.wrapped as Nakedobjectsrointerfaces.IScalarValueRepresentation);
        }
        return null;
    }

    isNull(): boolean {
        return this.wrapped == null;
    }

    isVoid(): boolean {
        return (this.resultType === "void");
    }
}

export class ErrorMap {

    wrapped = () => {
        const temp = this.map;
        if (isIObjectOfType(temp)) {
            return temp.members;
        } else {
            return temp;
        }
    };

    constructor(private map: Nakedobjectsrointerfaces.IValueMap | Nakedobjectsrointerfaces.IObjectOfType, public statusCode: number, public warningMessage: string) {

    }

    valuesMap(): _.Dictionary<ErrorValue> {

        const values = _.pickBy(this.wrapped(), i => isIValue(i)) as _.Dictionary<Nakedobjectsrointerfaces.IValue>;
        return _.mapValues(values, v => new ErrorValue(new Value(v.value), v.invalidReason));
    }

    invalidReason(): string {

        const temp = this.map;
        if (isIObjectOfType(temp)) {
            return (<any>temp)[Nakedobjectsconstants.roInvalidReason] as string;
        }

        return this.wrapped()[Nakedobjectsconstants.roInvalidReason] as string;
    }

    containsError() {
        return !!this.invalidReason() || !!this.warningMessage || _.some(this.valuesMap(), ev => !!ev.invalidReason);
    }

}

export class UpdateMap extends ArgumentMap implements IHateoasModel {
    constructor(private domainObject: DomainObjectRepresentation, map: Nakedobjectsrointerfaces.IValueMap) {
        super(map, domainObject.instanceId());

        domainObject.updateLink().copyToHateoasModel(this);

        _.each(this.properties(), (value: Value, key: string) => {
            this.setProperty(key, value);
        });
    }

    properties(): _.Dictionary<Value> {
        return _.mapValues(this.map, (v: any) => new Value(v.value));
    }

    setProperty(name: string, value: Value) {
        value.set(this.map, name);
    }

    setValidateOnly() {
        (<any>this.map)[Nakedobjectsconstants.roValidateOnly] = true;
    }
}

export class AddToRemoveFromMap extends ArgumentMap implements IHateoasModel {
    constructor(private collectionResource: CollectionRepresentation, map: Nakedobjectsrointerfaces.IValueMap, add: boolean) {
        super(map, collectionResource.collectionId());
        const link = add ? collectionResource.addToLink() : collectionResource.removeFromLink();
        link.copyToHateoasModel(this);
    }

}

export class ModifyMap extends ArgumentMap implements IHateoasModel {
    constructor(private propertyResource: PropertyRepresentation | PropertyMember, map: Nakedobjectsrointerfaces.IValueMap) {
        super(map, getId(propertyResource));
        propertyResource.modifyLink().copyToHateoasModel(this);
        propertyResource.value().set(this.map, this.id);
    }
}

export class ClearMap extends ArgumentMap implements IHateoasModel {
    constructor(propertyResource: PropertyRepresentation | PropertyMember) {
        super({}, getId(propertyResource));

        propertyResource.clearLink().copyToHateoasModel(this);
    }
}


// REPRESENTATIONS

export abstract class ResourceRepresentation<T extends Nakedobjectsrointerfaces.IResourceRepresentation> extends HateosModel {

    protected resource = () => this.model as T;

    populate(wrapped: T) {
        super.populate(wrapped);
    }

    private lazyLinks: Link[];

    links(): Link[] {
        this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
        return this.lazyLinks;
    }

    private lazyExtensions: Extensions;

    extensions(): Extensions {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.resource().extensions);
        return this.lazyExtensions;
    }
}

export class Extensions {

    constructor(private wrapped: Nakedobjectsrointerfacescustom.ICustomExtensions) { }

    //Standard RO:
    friendlyName = () => this.wrapped.friendlyName;
    description = () => this.wrapped.description;
    returnType = () => this.wrapped.returnType;
    optional = () => this.wrapped.optional;
    hasParams = () => this.wrapped.hasParams;
    elementType = () => this.wrapped.elementType;
    domainType = () => this.wrapped.domainType;
    pluralName = () => this.wrapped.pluralName;
    format = () => this.wrapped.format;
    memberOrder = () => this.wrapped.memberOrder;
    isService = () => this.wrapped.isService;
    minLength = () => this.wrapped.minLength;
    maxLength = () => this.wrapped.maxLength;
    pattern = () => this.wrapped.pattern;

    //Nof custom:
    choices = () => this.wrapped["x-ro-nof-choices"] as { [index: string]: Nakedobjectsrointerfaces.valueType[]; };
    menuPath = () => this.wrapped["x-ro-nof-menuPath"] as string;
    mask = () => this.wrapped["x-ro-nof-mask"] as string;
    tableViewTitle = () => this.wrapped["x-ro-nof-tableViewTitle"] as boolean;
    tableViewColumns = () => this.wrapped["x-ro-nof-tableViewColumns"] as string[];
    multipleLines = () => this.wrapped["x-ro-nof-multipleLines"] as number;
    warnings = () => this.wrapped["x-ro-nof-warnings"] as string[];
    messages = () => this.wrapped["x-ro-nof-messages"] as string[];
    interactionMode = () => this.wrapped["x-ro-nof-interactionMode"] as string;
    dataType = () => this.wrapped["x-ro-nof-dataType"] as string;
    range = () => this.wrapped["x-ro-nof-range"] as Nakedobjectsrointerfacescustom.IRange;
    notNavigable = () => this.wrapped["x-ro-nof-notNavigable"] as boolean;
    renderEagerly = () => this.wrapped["x-ro-nof-renderEagerly"] as boolean;
    presentationHint = () => this.wrapped["x-ro-nof-presentationHint"] as string;
}

// matches a action invoke resource 19.0 representation 

export class InvokeMap extends ArgumentMap implements IHateoasModel {
    constructor(private link: Link) {
        super(link.arguments() as Nakedobjectsrointerfaces.IValueMap, "");
        link.copyToHateoasModel(this);
    }

    setParameter(name: string, value: Value) {
        value.set(this.map, name);
    }


}

export class ActionResultRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IActionInvokeRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IActionInvokeRepresentation;

    constructor() {
        super();
    }

    // links 
    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    // link representations 
    getSelf(): ActionResultRepresentation {
        return <ActionResultRepresentation>this.selfLink().getTarget();
    }

    // properties 
    resultType(): Nakedobjectsrointerfaces.resultTypeType {
        return this.wrapped().resultType;
    }

    result(): Result {
        return new Result(this.wrapped().result, this.resultType());
    }

    warningsOrMessages(): string {

        const has = (arr: string[]) => arr && arr.length > 0;
        const wOrM = has(this.extensions().warnings()) ? this.extensions().warnings() : this.extensions().messages();

        if (has(wOrM)) {
            return _.reduce(wOrM, (s, t) => s + " " + t, "");
        }

        return undefined;
    }

    shouldExpectResult(): boolean {
        return this.result().isNull() && this.resultType() !== "void";
    }

}

export interface IHasExtensions {
    extensions(): Extensions;
}

// matches an action representation 18.0 

export interface IField extends IHasExtensions {
    id(): string;
    choices(): _.Dictionary<Value>;
    isScalar(): boolean;
    isCollectionContributed(): boolean;

    entryType(): EntryType;
    getPromptMap(): PromptMap;
    promptLink(): Link;
}

// matches 18.2.1
export class Parameter
    extends NestedRepresentation<Nakedobjectsrointerfaces.IParameterRepresentation>
    implements IField {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IParameterRepresentation;

    // fix parent type
    constructor(wrapped: Nakedobjectsrointerfaces.IParameterRepresentation, public parent: ActionMember | ActionRepresentation, private paramId: string) {
        super(wrapped);
    }

    id() {
        return this.paramId;
    }

    // properties 
    choices(): _.Dictionary<Value> {
        const customExtensions = this.extensions();
        // use custom choices extension by preference 
        if (customExtensions.choices()) {
            return _.mapValues(customExtensions.choices(), v => new Value(v));
        }

        if (this.wrapped().choices) {
            const values = _.map(this.wrapped().choices, item => new Value(item));
            return (<any>_.fromPairs)(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
        }
        return null;
    }

    promptLink(): Link {
        return linkByNamespacedRel(this.links(), "prompt");
    }

    getPromptMap(): PromptMap {
        const pr = <PromptRepresentation>this.promptLink().getTarget();
        return new PromptMap(this.promptLink(), pr.instanceId());
    }

    default(): Value {
        const dflt = this.wrapped().default == null ? (isScalarType(this.extensions().returnType()) ? "" : null) : this.wrapped().default;
        return new Value(dflt);
    }

    // helper
    isScalar(): boolean {
        return isScalarType(this.extensions().returnType()) ||
            (isListType(this.extensions().returnType()) && isScalarType(this.extensions().elementType()));
    }

    isList(): boolean {
        return isListType(this.extensions().returnType());
    }

    private hasPrompt(): boolean {
        return !!this.promptLink();
    }

    isCollectionContributed(): boolean {
        const myparent = this.parent;
        const isOnList = (myparent instanceof ActionMember || myparent instanceof ActionRepresentation) && myparent.parent instanceof ListRepresentation;
        const isList = this.isList();
        return isList && isOnList;
    }

    private hasChoices(): boolean { return _.some(this.choices()); }

    entryType(): EntryType {
        if (this.hasPrompt()) {
            // ConditionalChoices, ConditionalMultipleChoices, AutoComplete 

            if (!!(<any>this.promptLink().arguments())[Nakedobjectsconstants.roSearchTerm]) {
                // autocomplete 
                return EntryType.AutoComplete;
            }

            if (isListType(this.extensions().returnType())) {
                return EntryType.MultipleConditionalChoices;
            }
            return EntryType.ConditionalChoices;
        }

        if (this.choices()) {
            if (isListType(this.extensions().returnType())) {
                return EntryType.MultipleChoices;
            }
            return EntryType.Choices;
        }

        if (this.extensions().format() === "blob") {
            return EntryType.File;
        }

        return EntryType.FreeForm;
    }
}

// this interface guarantees that an action can be invoked. 
// An ActionRepresentation is always invokable 
// An ActionMember is not 
export interface IInvokableAction extends IHasExtensions {
    parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation;
    actionId(): string;
    invokeLink(): Link;
    getInvokeMap(): InvokeMap;
    parameters(): _.Dictionary<Parameter>;
    disabledReason(): string;
}

export class ActionRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IActionRepresentation> implements IInvokableAction {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IActionRepresentation;

    parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation;

    // links 
    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    invokeLink(): Link {
        return linkByNamespacedRel(this.links(), "invoke");
    }

    // linked representations 
    getSelf(): ActionRepresentation {
        return <ActionRepresentation>this.selfLink().getTarget();
    }

    getUp(): DomainObjectRepresentation {
        return <DomainObjectRepresentation>this.upLink().getTarget();
    }

    getInvokeMap(): InvokeMap {
        return new InvokeMap(this.invokeLink());
    }

    // properties 

    actionId(): string {
        return this.wrapped().id;
    }

    private parameterMap: _.Dictionary<Parameter>;

    private initParameterMap(): void {

        if (!this.parameterMap) {
            const parameters = this.wrapped().parameters;
            this.parameterMap = _.mapValues(parameters, (p, id) => new Parameter(p, this, id));
        }
    }

    parameters(): _.Dictionary<Parameter> {
        this.initParameterMap();
        return this.parameterMap;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason;
    }
}

// new in 1.1 15.0 in spec 

export class PromptMap extends ArgumentMap implements IHateoasModel {
    constructor(private link: Link, private promptId: string) {
        super(link.arguments() as Nakedobjectsrointerfaces.IValueMap, promptId);
        link.copyToHateoasModel(this);
    }

    private promptMap() {
        return this.map as Nakedobjectsrointerfaces.IPromptMap;
    }

    setSearchTerm(term: string) {
        this.setArgument(Nakedobjectsconstants.roSearchTerm, new Value(term));
    }

    setArgument(name: string, val: Value) {
        val.set(this.map, name);
    }

    setArguments(args: _.Dictionary<Value>) {
        _.each(args, (arg, key) => this.setArgument(key, arg));
    }

    setMember(name: string, value: Value) {
        value.set(this.promptMap()["x-ro-nof-members"] as Nakedobjectsrointerfaces.IValueMap, name);
    }

    setMembers(objectValues: () => _.Dictionary<Value>) {
        if (this.map["x-ro-nof-members"]) {
            _.forEach(objectValues(), (v, k) => this.setMember(k, v));
        }
    }
}

export class PromptRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IPromptRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IPromptRepresentation;

    constructor() {
        super(emptyResource());
    }

    // links 
    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    // linked representations 
    getSelf(): PromptRepresentation {
        return <PromptRepresentation>this.selfLink().getTarget();
    }

    getUp(): DomainObjectRepresentation {
        return <DomainObjectRepresentation>this.upLink().getTarget();
    }

    // properties 

    instanceId(): string {
        return this.wrapped().id;
    }

    choices(): _.Dictionary<Value> {
        const ch = this.wrapped().choices;
        if (ch) {
            const values = _.map(ch, item => new Value(item));
            return (<any>_.fromPairs)(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
        }
        return null;
    }
}

// matches a collection representation 17.0 
export class CollectionRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.ICollectionRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.ICollectionRepresentation;

    // links 
    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    addToLink(): Link {
        return linkByNamespacedRel(this.links(), "add-to");
    }

    removeFromLink(): Link {
        return linkByNamespacedRel(this.links(), "remove-from");
    }

    // linked representations 
    getSelf(): CollectionRepresentation {
        return <CollectionRepresentation>this.selfLink().getTarget();
    }

    getUp(): DomainObjectRepresentation {
        return <DomainObjectRepresentation>this.upLink().getTarget();
    }

    setFromMap(map: AddToRemoveFromMap) {
        //this.set(map.attributes);
        _.assign(this.resource(), map.map);
    }

    private addToMap() {
        return this.addToLink().arguments() as Nakedobjectsrointerfaces.IValueMap;
    }

    getAddToMap(): AddToRemoveFromMap {
        if (this.addToLink()) {
            return new AddToRemoveFromMap(this, this.addToMap(), true);
        }
        return null;
    }

    private removeFromMap() {
        return this.removeFromLink().arguments() as Nakedobjectsrointerfaces.IValueMap;
    }

    getRemoveFromMap(): AddToRemoveFromMap {
        if (this.removeFromLink()) {
            return new AddToRemoveFromMap(this, this.removeFromMap(), false);
        }
        return null;
    }

    // properties 

    collectionId(): string {
        return this.wrapped().id;
    }

    size(): number {
        return this.value().length;
    }

    private lazyValue: Link[];

    value(): Link[] {
        this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
        return this.lazyValue;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason;
    }
}

// matches a property representation 16.0 
export class PropertyRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IPropertyRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IPropertyRepresentation;


    // links 
    modifyLink(): Link {
        return linkByNamespacedRel(this.links(), "modify");
    }

    clearLink(): Link {
        return linkByNamespacedRel(this.links(), "clear");
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    promptLink(): Link {
        return linkByNamespacedRel(this.links(), "prompt");
    }

    private modifyMap() {
        return this.modifyLink().arguments() as Nakedobjectsrointerfaces.IValueMap;
    }

    // linked representations 
    getSelf(): PropertyRepresentation {
        return <PropertyRepresentation>this.selfLink().getTarget();
    }

    getUp(): DomainObjectRepresentation {
        return <DomainObjectRepresentation>this.upLink().getTarget();
    }

    setFromModifyMap(map: ModifyMap) {
        //this.set(map.attributes);
        _.assign(this.resource(), map.map);
    }

    getModifyMap(): ModifyMap {
        if (this.modifyLink()) {
            return new ModifyMap(this, this.modifyMap());
        }
        return null;
    }

    getClearMap(): ClearMap {
        if (this.clearLink()) {
            return new ClearMap(this);
        }
        return null;
    }


    // properties 

    instanceId(): string {
        return this.wrapped().id;
    }

    value(): Value {
        return new Value(this.wrapped().value);
    }

    choices(): _.Dictionary<Value> {

        // use custom choices extension by preference 
        if (this.extensions().choices()) {
            return _.mapValues(this.extensions().choices(), v => new Value(v));
        }
        const ch = this.wrapped().choices;
        if (ch) {
            const values = _.map(ch, item => new Value(item));
            return (<any>_.fromPairs)(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
        }
        return null;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason;
    }

    // helper 
    isScalar(): boolean {
        return isScalarType(this.extensions().returnType());
    }

    hasPrompt(): boolean {
        return !!this.promptLink();
    }
}

// matches a domain object representation 14.0 

// base class for 14.4.1/2/3
export class Member<T extends Nakedobjectsrointerfaces.IMember> extends NestedRepresentation<Nakedobjectsrointerfaces.IMember> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IMember;

    constructor(wrapped: T) {
        super(wrapped);
    }

    update(newValue: Nakedobjectsrointerfaces.IMember) {
        super.update(newValue);
    }

    memberType(): Nakedobjectsrointerfaces.memberTypeType {
        return this.wrapped().memberType;
    }

    detailsLink(): Link {
        return linkByNamespacedRel(this.links(), "details");
    }

    disabledReason(): string {
        return this.wrapped().disabledReason;
    }

    isScalar(): boolean {
        return isScalarType(this.extensions().returnType());
    }

    static wrapMember(toWrap: Nakedobjectsrointerfaces.IPropertyMember | Nakedobjectsrointerfaces.ICollectionMember | Nakedobjectsrointerfaces.IActionMember, parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation | Link, id: string): Member<Nakedobjectsrointerfaces.IMember> {

        if (toWrap.memberType === "property") {
            return new PropertyMember(toWrap as Nakedobjectsrointerfaces.IPropertyMember, parent as DomainObjectRepresentation | Link, id);
        }

        if (toWrap.memberType === "collection") {
            return new CollectionMember(toWrap as Nakedobjectsrointerfaces.ICollectionMember, parent as DomainObjectRepresentation, id);
        }

        if (toWrap.memberType === "action") {
            const member = new ActionMember(toWrap as Nakedobjectsrointerfaces.IActionMember, parent as DomainObjectRepresentation | MenuRepresentation | ListRepresentation, id);

            if (member.invokeLink()) {
                return new InvokableActionMember(toWrap as Nakedobjectsrointerfaces.IActionMember, parent as DomainObjectRepresentation | MenuRepresentation | ListRepresentation, id);
            }

            return member;
        }

        return null;
    }
}

// matches 14.4.1
export class PropertyMember extends Member<Nakedobjectsrointerfaces.IPropertyMember> implements IField {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IPropertyMember;

    constructor(wrapped: Nakedobjectsrointerfaces.IPropertyMember, public parent: DomainObjectRepresentation | Link, private propId: string) {
        super(wrapped);
    }

    // inlined 

    id(): string {
        return this.propId;
    }

    modifyLink(): Link {
        return linkByNamespacedRel(this.links(), "modify");
    }

    clearLink(): Link {
        return linkByNamespacedRel(this.links(), "clear");
    }

    private modifyMap() {
        return this.modifyLink().arguments() as Nakedobjectsrointerfaces.IValueMap;
    }

    setFromModifyMap(map: ModifyMap) {
        _.forOwn(map.map, (v, k) => {
            (<any>this.wrapped)[k] = v;
        });
    }

    getModifyMap(id: string): ModifyMap {
        if (this.modifyLink()) {
            return new ModifyMap(this, this.modifyMap());
        }
        return null;
    }

    getClearMap(id: string): ClearMap {
        if (this.clearLink()) {
            return new ClearMap(this);
        }
        return null;
    }


    getPromptMap(): PromptMap {
        const pr = <PromptRepresentation>this.promptLink().getTarget();
        return new PromptMap(this.promptLink(), pr.instanceId());
    }


    value(): Value {
        return new Value(this.wrapped().value);
    }

    isScalar(): boolean {
        return isScalarType(this.extensions().returnType());
    }

    attachmentLink(): Link {
        return linkByNamespacedRel(this.links(), "attachment");
    }

    promptLink(): Link {
        return linkByNamespacedRel(this.links(), "prompt");
    }

    getDetails(): PropertyRepresentation {
        return <PropertyRepresentation>this.detailsLink().getTarget();
    }

    private hasChoices(): boolean {
        return this.wrapped().hasChoices;
    }

    private hasPrompt(): boolean {
        return !!this.promptLink();
    }

    choices(): _.Dictionary<Value> {

        // use custom choices extension by preference 
        if (this.extensions().choices()) {
            return _.mapValues(this.extensions().choices(), v => new Value(v));
        }
        const ch = this.wrapped().choices;
        if (ch) {
            const values = _.map(ch, (item) => new Value(item));
            return (<any>_.fromPairs)(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
        }
        return null;
    }

    private hasConditionalChoices(): boolean {
        return !!this.promptLink() && !this.hasPrompt();
    }

    //This is actually not relevant to a property. Slight smell here!

    isCollectionContributed(): boolean {
        return false;
    }

    entryType(): EntryType {
        if (this.hasPrompt()) {
            // ConditionalChoices, ConditionalMultipleChoices, AutoComplete 

            if (!!(<any>this.promptLink().arguments())[Nakedobjectsconstants.roSearchTerm]) {
                // autocomplete 
                return EntryType.AutoComplete;
            }
            return EntryType.ConditionalChoices;
        }

        if (this.choices()) {
            return EntryType.Choices;
        }

        return EntryType.FreeForm;
    }
}

// matches 14.4.2 
export class CollectionMember
    extends Member<Nakedobjectsrointerfaces.ICollectionMember>
    implements IHasLinksAsValue {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.ICollectionMember;

    constructor(wrapped: Nakedobjectsrointerfaces.ICollectionMember, public parent: DomainObjectRepresentation, private id: string) {
        super(wrapped);
    }

    collectionId(): string {
        return this.id;
    }

    private lazyValue: Link[];

    value(): Link[] {
        this.lazyValue = this.lazyValue || (this.wrapped().value ? wrapLinks(this.wrapped().value) : null);
        return this.lazyValue;
    }

    size(): number {
        return this.wrapped().size;
    }

    getDetails(): CollectionRepresentation {
        return <CollectionRepresentation>this.detailsLink().getTarget();
    }
}

// matches 14.4.3 
export class ActionMember extends Member<Nakedobjectsrointerfaces.IActionMember> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IActionMember;

    constructor(wrapped: Nakedobjectsrointerfaces.IActionMember, public parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation, private id: string) {
        super(wrapped);
    }

    actionId(): string {
        return this.id;
    }

    getDetails(): ActionRepresentation {
        const details = <ActionRepresentation>this.detailsLink().getTarget();
        details.parent = this.parent;
        return details;
    }

    // 1.1 inlined 

    invokeLink(): Link {
        return linkByNamespacedRel(this.links(), "invoke");
    }

    disabledReason(): string {
        return this.wrapped().disabledReason;
    }
}

export class InvokableActionMember extends ActionMember {


    constructor(wrapped: Nakedobjectsrointerfaces.IActionMember, parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation, id: string) {
        super(wrapped, parent, id);
    }

    getInvokeMap(): InvokeMap {
        const invokeLink = this.invokeLink();

        if (invokeLink) {
            return new InvokeMap(this.invokeLink());
        }
        return null;
    }

    // properties 

    private parameterMap: _.Dictionary<Parameter>;

    private initParameterMap(): void {

        if (!this.parameterMap) {
            const parameters = this.wrapped().parameters;
            this.parameterMap = _.mapValues(parameters, (p, id) => new Parameter(p, this, id));
        }
    }

    parameters(): _.Dictionary<Parameter> {
        this.initParameterMap();
        return this.parameterMap;
    }
}


export class DomainObjectRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IDomainObjectRepresentation> implements IHasActions {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IDomainObjectRepresentation;

    constructor() {
        super();
    }

    id(): string {
        return `${this.domainType() || this.serviceId()}${this.instanceId() ? `${Nakedobjectsconfig.keySeparator}${this.instanceId()}` : ""}`;
    }

    title(): string {
        return this.wrapped().title;
    }

    domainType(): string {
        return this.wrapped().domainType;
    }

    serviceId(): string {
        return this.wrapped().serviceId;
    }

    instanceId(): string {
        return this.wrapped().instanceId;
    }

    private memberMap: _.Dictionary<Member<Nakedobjectsrointerfaces.IMember>>;
    private propertyMemberMap: _.Dictionary<PropertyMember>;
    private collectionMemberMap: _.Dictionary<CollectionMember>;
    private actionMemberMap: _.Dictionary<ActionMember>;

    private resetMemberMaps() {
        const members = this.wrapped().members;
        this.memberMap = _.mapValues(members, (m, id) => Member.wrapMember(m, this, id));
        this.propertyMemberMap = _.pickBy(this.memberMap, (m: Member<Nakedobjectsrointerfaces.IMember>) => m.memberType() === "property") as _.Dictionary<PropertyMember>;
        this.collectionMemberMap = _.pickBy(this.memberMap, (m: Member<Nakedobjectsrointerfaces.IMember>) => m.memberType() === "collection") as _.Dictionary<CollectionMember>;
        this.actionMemberMap = _.pickBy(this.memberMap, (m: Member<Nakedobjectsrointerfaces.IMember>) => m.memberType() === "action") as _.Dictionary<ActionMember>;
    }

    private initMemberMaps() {
        if (!this.memberMap) {
            this.resetMemberMaps();
        }
    }

    members(): _.Dictionary<Member<Nakedobjectsrointerfaces.IMember>> {
        this.initMemberMaps();
        return this.memberMap;
    }

    propertyMembers(): _.Dictionary<PropertyMember> {
        this.initMemberMaps();
        return this.propertyMemberMap;
    }

    collectionMembers(): _.Dictionary<CollectionMember> {
        this.initMemberMaps();
        return this.collectionMemberMap;
    }

    actionMembers(): _.Dictionary<ActionMember> {
        this.initMemberMaps();
        return this.actionMemberMap;
    }

    member(id: string): Member<Nakedobjectsrointerfaces.IMember> {
        return this.members()[id];
    }

    propertyMember(id: string): PropertyMember {
        return this.propertyMembers()[id];
    }

    collectionMember(id: string): CollectionMember {
        return this.collectionMembers()[id];
    }

    actionMember(id: string): ActionMember {
        return this.actionMembers()[id];
    }

    updateLink(): Link {
        return linkByNamespacedRel(this.links(), "update");
    }

    isTransient(): boolean {
        return !!this.persistLink();
    }

    persistLink(): Link {
        return linkByNamespacedRel(this.links(), "persist");
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    private updateMap() {
        return this.updateLink().arguments() as Nakedobjectsrointerfaces.IValueMap;
    }

    private persistMap() {
        return this.persistLink().arguments() as Nakedobjectsrointerfaces.IObjectOfType;
    }

    // linked representations 
    getSelf(): DomainObjectRepresentation {
        return <DomainObjectRepresentation>this.selfLink().getTarget();
    }

    getPersistMap(): PersistMap {
        return new PersistMap(this, this.persistMap());
    }

    getUpdateMap(): UpdateMap {
        return new UpdateMap(this, this.updateMap());
    }

    setInlinePropertyDetails(flag: boolean) {
        this.setUrlParameter(Nakedobjectsconstants.roInlinePropertyDetails, flag);
    }

    private oid: ObjectIdWrapper;
    getOid(): ObjectIdWrapper {
        if (!this.oid) {
            this.oid = ObjectIdWrapper.fromObject(this);
        }

        return this.oid;
    }
}

export class MenuRepresentation extends ResourceRepresentation<Nakedobjectsrointerfacescustom.IMenuRepresentation> implements IHasActions {

    wrapped = () => this.resource() as Nakedobjectsrointerfacescustom.IMenuRepresentation;

    constructor() {
        super();
    }

    title(): string {
        return this.wrapped().title;
    }

    menuId(): string {
        return this.wrapped().menuId;
    }

    private memberMap: _.Dictionary<Member<Nakedobjectsrointerfaces.IMember>>;

    private actionMemberMap: _.Dictionary<ActionMember>;

    private resetMemberMaps() {
        const members = this.wrapped().members;
        this.memberMap = _.mapValues(members, (m, id) => Member.wrapMember(m, this, id));
        this.actionMemberMap = _.pickBy(this.memberMap, m => m.memberType() === "action") as _.Dictionary<ActionMember>;
    }

    private initMemberMaps() {
        if (!this.memberMap) {
            this.resetMemberMaps();
        }
    }

    members(): _.Dictionary<Member<Nakedobjectsrointerfaces.IMember>> {
        this.initMemberMaps();
        return this.memberMap;
    }

    actionMembers(): _.Dictionary<ActionMember> {
        this.initMemberMaps();
        return this.actionMemberMap;
    }

    member(id: string): Member<Nakedobjectsrointerfaces.IMember> {
        return this.members()[id];
    }

    actionMember(id: string): ActionMember {
        return this.actionMembers()[id];
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    // linked representations 
    getSelf(): MenuRepresentation {
        return <MenuRepresentation>this.selfLink().getTarget();
    }

}

// matches scalar representation 12.0 
export class ScalarValueRepresentation extends NestedRepresentation<Nakedobjectsrointerfaces.IScalarValueRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IScalarValueRepresentation;

    constructor(wrapped: Nakedobjectsrointerfaces.IScalarValueRepresentation) {
        super(wrapped);
    }

    value(): Value {
        return new Value(this.wrapped().value);
    }
}

// matches List Representation 11.0
export class ListRepresentation
    extends ResourceRepresentation<Nakedobjectsrointerfacescustom.ICustomListRepresentation>
    implements IHasLinksAsValue {

    wrapped = () => this.resource() as Nakedobjectsrointerfacescustom.ICustomListRepresentation;

    // links
    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    // linked representations 
    getSelf(): ListRepresentation {
        return <ListRepresentation>this.selfLink().getTarget();
    }

    private lazyValue: Link[];

    value(): Link[] {
        this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
        return this.lazyValue;
    }

    pagination(): Nakedobjectsrointerfacescustom.IPagination {
        return this.wrapped().pagination;
    }

    private actionMemberMap: _.Dictionary<ActionMember>;

    actionMembers() {
        this.actionMemberMap = this.actionMemberMap || _.mapValues(this.wrapped().members, (m, id) => Member.wrapMember(m, this, id)) as _.Dictionary<ActionMember>;
        return this.actionMemberMap;
    }

    actionMember(id: string): ActionMember {
        return this.actionMembers()[id];
    }
}

export interface IErrorDetails {
    message(): string;
    stackTrace(): string[];
}

// matches the error representation 10.0 
export class ErrorRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IErrorRepresentation> implements IErrorDetails {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IErrorRepresentation;

    static create(message: string, stackTrace?: string[], causedBy?: Nakedobjectsrointerfaces.IErrorDetailsRepresentation) {
        const rawError = {
            links: [] as any[],
            extensions: {},
            message: message,
            stackTrace: stackTrace,
            causedBy: causedBy
        };
        const error = new ErrorRepresentation();
        error.populate(rawError);
        return error;
    }


    // scalar properties 
    message(): string {
        return this.wrapped().message;
    }

    stackTrace(): string[] {
        return this.wrapped().stackTrace;
    }

    causedBy(): IErrorDetails {
        const cb = this.wrapped().causedBy;
        return cb ? {
            message: () => cb.message,
            stackTrace: () => cb.stackTrace
        } : undefined;
    }
}

// matches Objects of Type Resource 9.0 
export class PersistMap extends HateosModel implements IHateoasModel {

    constructor(private domainObject: DomainObjectRepresentation, private map: Nakedobjectsrointerfaces.IObjectOfType) {
        super(map);
        domainObject.persistLink().copyToHateoasModel(this);
    }

    setMember(name: string, value: Value) {
        value.set(this.map.members, name);
    }

    setValidateOnly() {
        (<any>this.map)[Nakedobjectsconstants.roValidateOnly] = true;
    }
}

// matches the version representation 8.0 
export class VersionRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IVersionRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IVersionRepresentation;

    // links 
    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    // linked representations 
    getSelf(): VersionRepresentation {
        return <VersionRepresentation>this.selfLink().getTarget();
    }

    getUp(): HomePageRepresentation {
        return <HomePageRepresentation>this.upLink().getTarget();
    }

    // scalar properties 
    specVersion(): string {
        return this.wrapped().specVersion;
    }

    implVersion(): string {
        return this.wrapped().implVersion;
    }

    optionalCapabilities(): Nakedobjectsrointerfaces.IOptionalCapabilities {
        return this.wrapped().optionalCapabilities;
    }
}

// matches Domain Services Representation 7.0
export class DomainServicesRepresentation extends ListRepresentation {

    wrapped = () => this.resource() as Nakedobjectsrointerfacescustom.ICustomListRepresentation;

    // links
    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    // linked representations 
    getSelf(): DomainServicesRepresentation {
        return <DomainServicesRepresentation>this.selfLink().getTarget();
    }

    getUp(): HomePageRepresentation {
        return <HomePageRepresentation>this.upLink().getTarget();
    }

    getService(serviceType: string): DomainObjectRepresentation {
        const serviceLink = _.find(this.value(), link => link.rel().parms[0].value === serviceType);
        return <DomainObjectRepresentation>serviceLink.getTarget();
    }
}

// custom
export class MenusRepresentation extends ListRepresentation {

    wrapped = () => this.resource() as Nakedobjectsrointerfacescustom.ICustomListRepresentation;

    // links
    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    // linked representations 
    getSelf(): MenusRepresentation {
        return <MenusRepresentation>this.selfLink().getTarget();
    }

    getUp(): HomePageRepresentation {
        return <HomePageRepresentation>this.upLink().getTarget();
    }

    getMenu(menuId: string): MenuRepresentation {
        const menuLink = _.find(this.value(), link => link.rel().parms[0].value === menuId);
        return <MenuRepresentation>menuLink.getTarget();
    }
}

// matches the user representation 6.0
export class UserRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IUserRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IUserRepresentation;

    // links 
    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    upLink(): Link {
        return linkByRel(this.links(), "up");
    }

    // linked representations 
    getSelf(): UserRepresentation {
        return <UserRepresentation>this.selfLink().getTarget();
    }

    getUp(): HomePageRepresentation {
        return <HomePageRepresentation>this.upLink().getTarget();
    }

    // scalar properties 
    userName(): string {
        return this.wrapped().userName;
    }

    friendlyName(): string {
        return this.wrapped().friendlyName;
    }

    email(): string {
        return this.wrapped().email;
    }

    roles(): string[] {
        return this.wrapped().roles;
    }
}

export class DomainTypeActionInvokeRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IDomainTypeActionInvokeRepresentation> {

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IDomainTypeActionInvokeRepresentation;

    constructor(againstType: string, toCheckType: string) {
        super();
        this.hateoasUrl = `${Nakedobjectsconfig.getAppPath()}/domain-types/${toCheckType}/type-actions/isSubtypeOf/invoke`;
        this.urlParms = {};
        this.urlParms["supertype"] = againstType;
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    // linked representations 
    getSelf(): DomainTypeActionInvokeRepresentation {
        return <DomainTypeActionInvokeRepresentation>this.selfLink().getTarget();
    }

    id(): string {
        return this.wrapped().id;
    }

    value(): boolean {
        return this.wrapped().value;
    }
}

// matches the home page representation  5.0 
export class HomePageRepresentation extends ResourceRepresentation<Nakedobjectsrointerfaces.IHomePageRepresentation> {

    constructor(rep: Nakedobjectsrointerfaces.IRepresentation) {
        super(rep);
        this.hateoasUrl = Nakedobjectsconfig.getAppPath();
    }

    wrapped = () => this.resource() as Nakedobjectsrointerfaces.IHomePageRepresentation;

    // links 
    serviceLink(): Link {
        return linkByNamespacedRel(this.links(), "services");
    }

    userLink(): Link {
        return linkByNamespacedRel(this.links(), "user");
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self");
    }

    versionLink(): Link {
        return linkByNamespacedRel(this.links(), "version");
    }

    // custom 
    menusLink(): Link {
        return linkByNamespacedRel(this.links(), "menus");
    }

    // linked representations 
    getSelf(): HomePageRepresentation {
        return <HomePageRepresentation>this.selfLink().getTarget();
    }

    getUser(): UserRepresentation {
        return <UserRepresentation>this.userLink().getTarget();
    }

    getDomainServices(): DomainServicesRepresentation {
        // cannot use getTarget here as that will just return a ListRepresentation 
        const domainServices = new DomainServicesRepresentation();
        this.serviceLink().copyToHateoasModel(domainServices);
        return domainServices;
    }

    getVersion(): VersionRepresentation {
        return <VersionRepresentation>this.versionLink().getTarget();
    }

    //  custom 

    getMenus(): MenusRepresentation {
        // cannot use getTarget here as that will just return a ListRepresentation 
        const menus = new MenusRepresentation();
        this.menusLink().copyToHateoasModel(menus);
        return menus;
    }

}


// matches the Link representation 2.7
export class Link {

    constructor(public wrapped: Nakedobjectsrointerfaces.ILink) { }

    compress() {
        this.wrapped.href = compress(this.wrapped.href);
    }

    decompress() {
        this.wrapped.href = decompress(this.wrapped.href);
    }

    href(): string {
        return decodeURIComponent(this.wrapped.href);
    }

    method(): Nakedobjectsrointerfaces.httpMethodsType {
        return this.wrapped.method;
    }

    rel(): Rel {
        return new Rel(this.wrapped.rel);
    }

    type(): MediaType {
        return new MediaType(this.wrapped.type);
    }

    title(): string {
        return this.wrapped.title;
    }

    //Typically used to set a title on a link that doesn't naturally have one e.g. Self link
    setTitle(title: string): void {
        this.wrapped.title = title;
    }

    arguments(): Nakedobjectsrointerfaces.IValue | Nakedobjectsrointerfaces.IValueMap | Nakedobjectsrointerfaces.IObjectOfType | Nakedobjectsrointerfaces.IPromptMap {
        return this.wrapped.arguments;
    }

    members(): _.Dictionary<PropertyMember> {
        const members = (this.wrapped as Nakedobjectsrointerfacescustom.ICustomLink).members;
        return members ? _.mapValues(members, (m, id) => Member.wrapMember(m, this, id) as PropertyMember) : null;
    }

    private lazyExtensions: Extensions;

    extensions(): Extensions {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.wrapped.extensions);
        return this.lazyExtensions;
    }

    copyToHateoasModel(hateoasModel: IHateoasModel): void {
        hateoasModel.hateoasUrl = this.href();
        hateoasModel.method = this.method();
    }

    private getHateoasTarget(targetType: any): IHateoasModel {
        const MatchingType = this.repTypeToModel[targetType];
        const target: IHateoasModel = new MatchingType();
        return target;
    }

    private repTypeToModel: any = {
        "homepage": HomePageRepresentation,
        "user": UserRepresentation,
        "version": VersionRepresentation,
        "list": ListRepresentation,
        "object": DomainObjectRepresentation,
        "object-property": PropertyRepresentation,
        "object-collection": CollectionRepresentation,
        "object-action": ActionRepresentation,
        "action-result": ActionResultRepresentation,
        "error": ErrorRepresentation,
        "prompt": PromptRepresentation,
        // custom 
        "menu": MenuRepresentation
    };

    // get the object that this link points to 
    getTarget(): IHateoasModel {
        const target = this.getHateoasTarget(this.type().representationType);
        this.copyToHateoasModel(target);
        return target;
    }

    // helper 

    private oid: ObjectIdWrapper;
    getOid(): ObjectIdWrapper {
        if (!this.oid) {
            this.oid = ObjectIdWrapper.fromLink(this);
        }

        return this.oid;
    }
}

export interface IHasActions extends IHasExtensions {
    actionMembers(): _.Dictionary<ActionMember>;
    actionMember(id: string): ActionMember;
}

export interface IHasLinksAsValue {
    value(): Link[];
}

export enum EntryType { FreeForm, Choices, MultipleChoices, ConditionalChoices, MultipleConditionalChoices, AutoComplete, File }
