import * as Ro from './ro-interfaces';
import * as Constants from './constants';
import * as RoCustom from './ro-interfaces-custom';
import * as Msg from './user-messages';
import { Dictionary } from 'lodash';
import each from 'lodash-es/each';
import find from 'lodash-es/find';
import assign from 'lodash-es/assign';
import clone from 'lodash-es/clone';
import keys from 'lodash-es/keys';
import map from 'lodash-es/map';
import concat from 'lodash-es/concat';
import mapValues from 'lodash-es/mapValues';
import pickBy from 'lodash-es/pickBy';
import reduce from 'lodash-es/reduce';
import some from 'lodash-es/some';
import fromPairs from 'lodash-es/fromPairs';
import forOwn from 'lodash-es/forOwn';
import last from 'lodash-es/last';
import merge from 'lodash-es/merge';
import forEach from 'lodash-es/forEach';

// do not couple this back to angular by imports

// log directly to avoid coupling back to angular
function error(message: string, ): never {
    console.error(message);
    throw new Error(message);
}

export class Extensions {

    constructor(private wrapped: RoCustom.ICustomExtensions) { }

    // Standard RO:
    friendlyName = () => this.wrapped.friendlyName || "";
    description = () => this.wrapped.description || "";
    returnType = () => this.wrapped.returnType || null;
    optional = () => this.wrapped.optional || false;
    hasParams = () => this.wrapped.hasParams || false;
    elementType = () => this.wrapped.elementType || null;
    domainType = () => this.wrapped.domainType || null;
    pluralName = () => this.wrapped.pluralName || "";
    format = () => this.wrapped.format;
    memberOrder = () => this.wrapped.memberOrder;
    isService = () => this.wrapped.isService || false;
    minLength = () => this.wrapped.minLength;
    maxLength = () => this.wrapped.maxLength;
    pattern = () => this.wrapped.pattern;

    // Nof custom:
    choices = () => this.wrapped["x-ro-nof-choices"] as { [index: string]: Ro.ValueType[]; };
    menuPath = () => this.wrapped["x-ro-nof-menuPath"] as string;
    mask = () => this.wrapped["x-ro-nof-mask"] as string;
    tableViewTitle = () => this.wrapped["x-ro-nof-tableViewTitle"] as boolean;
    tableViewColumns = () => this.wrapped["x-ro-nof-tableViewColumns"] as string[];
    multipleLines = () => this.wrapped["x-ro-nof-multipleLines"] as number;
    warnings = () => this.wrapped["x-ro-nof-warnings"] as string[];
    messages = () => this.wrapped["x-ro-nof-messages"] as string[];
    interactionMode = () => this.wrapped["x-ro-nof-interactionMode"] as string;
    dataType = () => this.wrapped["x-ro-nof-dataType"] as string;
    range = () => this.wrapped["x-ro-nof-range"] as RoCustom.IRange;
    notNavigable = () => this.wrapped["x-ro-nof-notNavigable"] as boolean;
    renderEagerly = () => this.wrapped["x-ro-nof-renderEagerly"] as boolean;
    presentationHint = () => this.wrapped["x-ro-nof-presentationHint"] as string;
}

export interface IHasExtensions {
    extensions(): Extensions;
}

export interface IHasActions extends IHasExtensions {
    etagDigest?: string;
    actionMembers(): Dictionary<ActionMember>;
    actionMember(id: string): ActionMember;
    hasActionMember(id: string): boolean;
}

export interface IHasLinksAsValue {
    value(): Link[] | null;
}

// coerce undefined to null
export function withNull<T>(v: T | undefined | null): T | null {
    return v === undefined ? null : v;
}

export function withUndefined<T>(v: T | undefined | null): T | undefined {
    return v === null ? undefined : v;
}

function validateExists<T>(obj: T | null | undefined, name: string) {
    if (obj) { return obj!; }
    return error(`validateExists - Expected ${name} does not exist`);
}

function getMember<T>(members: Dictionary<T>, id: string, owner: string) {
    const member = members[id];
    if (member) { return member; }
    return error(`getMember - no member ${id} on ${owner}`);
}

export function checkNotNull<T>(v: T | undefined | null) {
    if (v != null) { return v!; }
    return error("checkNotNull - Unexpected null");
}

export function toDateString(dt: Date) {

    const year = dt.getFullYear().toString();
    let month = (dt.getMonth() + 1).toString();
    let day = dt.getDate().toString();

    month = month.length === 1 ? `0${month}` : month;
    day = day.length === 1 ? `0${day}` : day;

    return `${year}-${month}-${day}`;
}

export function toDateTimeString(dt: Date) {
    return `${toDateString(dt)} ${toTimeString(dt)}`;
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

export function getTime(rawTime: string): Date | null {
    if (!rawTime || rawTime.length === 0) {
        return null;
    }

    const hours = parseInt(rawTime.substring(0, 2), 10);
    const mins = parseInt(rawTime.substring(3, 5), 10);
    const secs = parseInt(rawTime.substring(6, 8), 10);

    return new Date(1970, 0, 1, hours, mins, secs);
}

export function isDate(rep: IHasExtensions) {
    const returnType = rep.extensions().returnType();
    const format = rep.extensions().format();

    return (returnType === "string" && format === "date");
}

export function isDateTime(rep: IHasExtensions) {
    const returnType = rep.extensions().returnType();
    const format = rep.extensions().format();

    return (returnType === "string" && format === "date-time");
}

export function isDateOrDateTime(rep: IHasExtensions) {
    return isDate(rep) || isDateTime(rep);
}

export function isTime(rep: IHasExtensions) {
    const returnType = rep.extensions().returnType();
    const format = rep.extensions().format();

    return returnType === "string" && format === "time";
}

export function toTime(value: Value) {
    const rawValue = value ? value.toString() : "";
    const dateValue = getTime(rawValue);
    return dateValue ? dateValue : null;
}

export function toUtcDate(value: Value) {
    const rawValue = value ? value.toString() : "";
    const dateValue = getUtcDate(rawValue);
    return dateValue ? dateValue : null;
}

export function getUtcDate(rawDate: string): Date | null {
    if (!rawDate || rawDate.length === 0) {
        return null;
    }

    const year = parseInt(rawDate.substring(0, 4), 10);
    const month = parseInt(rawDate.substring(5, 7), 10) - 1;
    const day = parseInt(rawDate.substring(8, 10), 10);

    if (rawDate.length === 10) {
        return new Date(Date.UTC(year, month, day, 0, 0, 0));
    }

    if (rawDate.length >= 20) {
        const hours = parseInt(rawDate.substring(11, 13), 10);
        const mins = parseInt(rawDate.substring(14, 16), 10);
        const secs = parseInt(rawDate.substring(17, 19), 10);

        return new Date(Date.UTC(year, month, day, hours, mins, secs));
    }

    return null;
}

export function compress(toCompress: string, shortCutMarker: string, urlShortCuts: string[]) {
    if (toCompress) {
        forEach(urlShortCuts, (sc, i) => toCompress = toCompress.replace(sc, `${shortCutMarker}${i}`));
    }
    return toCompress;
}

export function decompress(toDecompress: string, shortCutMarker: string, urlShortCuts: string[]) {
    if (toDecompress) {
        forEach(urlShortCuts, (sc, i) => toDecompress = toDecompress.replace(`${shortCutMarker}${i}`, sc));
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
    // TODO Fix "!"
    const shortName = last(fullName.split("."))!;
    const result = shortName.replace(/([A-Z])/g, " $1").trim();
    return result.charAt(0).toUpperCase() + result.slice(1);
}

export function friendlyNameForParam(action: ActionRepresentation | InvokableActionMember, parmId: string) {
    const param = find(action.parameters(), (p) => p.id() === parmId);
    return param ? param.extensions().friendlyName() : "";
}

export function friendlyNameForProperty(obj: DomainObjectRepresentation, propId: string) {
    const prop = obj.propertyMember(propId);
    return prop ? prop.extensions().friendlyName() : propId;
}

export function typePlusTitle(obj: DomainObjectRepresentation) {
    const type = obj.extensions().friendlyName();
    const title = obj.title();
    return type + ": " + title;
}

// helper functions

function isAutoComplete(args: Ro.IValue | Ro.IValueMap | Ro.IObjectOfType | Ro.IPromptMap | null) {
    return args && args.hasOwnProperty(Constants.roSearchTerm);
}

function isScalarType(typeName: string | null) {
    return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
}

function isListType(typeName: string | null) {
    return typeName === "list";
}

function emptyResource(): Ro.IResourceRepresentation {
    return { links: [] as Ro.ILink[], extensions: {} };
}

function isILink(object: any): object is Ro.ILink {
    return object && object instanceof Object && "href" in object;
}

function isIObjectOfType(object: any): object is Ro.IObjectOfType {
    return object && object instanceof Object && "members" in object;
}

function isIValue(object: any): object is Ro.IValue {
    return object && object instanceof Object && "value" in object;
}

export function isResourceRepresentation(object: any): object is Ro.IResourceRepresentation {
    return object && object instanceof Object && "links" in object && "extensions" in object;
}

export function isErrorRepresentation(object: any): object is Ro.IErrorRepresentation {
    return isResourceRepresentation(object) && "message" in object;
}

export function isIDomainObjectRepresentation(object: any): object is Ro.IDomainObjectRepresentation {
    return isResourceRepresentation(object) && "domainType" in object && "instanceId" in object && "members" in object;
}

function getId(prop: PropertyRepresentation | PropertyMember) {
    if (prop instanceof PropertyRepresentation) {
        return prop.instanceId();
    } else {
        return (prop as PropertyMember).id();
    }
}

function wrapLinks(links: Ro.ILink[]) {
    return map(links, l => new Link(l));
}

function getLinkByRel(links: Link[], rel: Rel) {
    return find(links, i => i.rel().uniqueValue === rel.uniqueValue);
}

function linkByRel(links: Link[], rel: string) {
    return getLinkByRel(links, new Rel(rel));
}

function linkByNamespacedRel(links: Link[], rel: string) {
    return getLinkByRel(links, new Rel(`urn:org.restfulobjects:rels/${rel}`));
}

// interfaces

export interface IHateoasModel {
    keySeparator: string;
    etagDigest?: string;
    hateoasUrl: string;
    method: Ro.HttpMethodsType;
    populate(wrapped: Ro.IRepresentation): void;
    getBody(): Ro.IRepresentation;
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
    ConnectionProblem = 0
}

export class ErrorWrapper {
    constructor(
        readonly category: ErrorCategory,
        code: HttpStatusCode | ClientErrorCode,
        err: string | ErrorMap | ErrorRepresentation,
        readonly originalUrl?: string
    ) {

        if (category === ErrorCategory.ClientError) {
            this.clientErrorCode = code as ClientErrorCode;
            this.errorCode = ClientErrorCode[this.clientErrorCode];
            let description = Msg.errorUnknown;

            switch (this.clientErrorCode) {
                case ClientErrorCode.ExpiredTransient:
                    description = Msg.errorExpiredTransient;
                    break;
                case ClientErrorCode.WrongType:
                    description = Msg.errorWrongType;
                    break;
                case ClientErrorCode.NotImplemented:
                    description = Msg.errorNotImplemented;
                    break;
                case ClientErrorCode.SoftwareError:
                    description = Msg.errorSoftware;
                    break;
                case ClientErrorCode.ConnectionProblem:
                    description = Msg.errorConnection;
                    break;
            }

            this.description = description;
            this.title = Msg.errorClient;
        }

        if (category === ErrorCategory.HttpClientError || category === ErrorCategory.HttpServerError) {
            this.httpErrorCode = code as HttpStatusCode;
            this.errorCode = `${HttpStatusCode[this.httpErrorCode]}(${this.httpErrorCode})`;

            this.description = category === ErrorCategory.HttpServerError
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

    message: string;
    error: ErrorMap | ErrorRepresentation | null;

    stackTrace: string[];

    handled = false;
}

// abstract classes

function toOid(id: string[], keySeparator: string) {
    return reduce(id, (a, v) => `${a}${keySeparator}${v}`) as string;
}

export class ObjectIdWrapper {

    domainType: string;
    instanceId: string;
    splitInstanceId: string[];
    isService: boolean;

    static safeSplit(id: string, keySeparator: string): string[] {
      return id ? id.split(keySeparator) : [];
    }

    static fromObject(object: DomainObjectRepresentation) {
        const oid = new ObjectIdWrapper(object.keySeparator);
        oid.domainType = object.domainType() || "";
        oid.instanceId = object.instanceId() || "";
        oid.splitInstanceId = this.safeSplit(oid.instanceId, object.keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromLink(link: Link, keySeparator: string) {
        const href = link.href();
        return this.fromHref(href, keySeparator);
    }

    static fromHref(href: string, keySeparator: string) {
        const oid = new ObjectIdWrapper(keySeparator);
        oid.domainType = typeFromUrl(href);
        oid.instanceId = idFromUrl(href);
        oid.splitInstanceId = this.safeSplit(oid.instanceId, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromObjectId(objectId: string, keySeparator: string) {
        const oid = new ObjectIdWrapper(keySeparator);
        const [dt, ...id] = objectId.split(keySeparator);
        oid.domainType = dt;
        oid.splitInstanceId = id;
        oid.instanceId = toOid(id, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromRaw(dt: string, id: string, keySeparator: string) {
        const oid = new ObjectIdWrapper(keySeparator);
        oid.domainType = dt;
        oid.instanceId = id;
        oid.splitInstanceId = this.safeSplit(oid.instanceId, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    }

    static fromSplitRaw(dt: string, id: string[], keySeparator: string) {
        const oid = new ObjectIdWrapper(keySeparator);
        oid.domainType = dt;
        oid.splitInstanceId = id;
        oid.instanceId = toOid(id, keySeparator);
        oid.isService = !oid.instanceId;
        return oid;
    }

    constructor(private readonly keySeparator: string) {
        if (keySeparator == null) {
            error("ObjectIdWrapper must have a keySeparator");
        }
    }

    getKey() {
        return this.domainType + this.keySeparator + this.instanceId;
    }

    isSame(other: ObjectIdWrapper) {
        return other && other.domainType === this.domainType && other.instanceId === this.instanceId;
    }
}

export abstract class HateosModel implements IHateoasModel {

    etagDigest?: string;
    hateoasUrl = "";
    method: Ro.HttpMethodsType = "GET";
    urlParms: Dictionary<Object>;
    keySeparator: string;

    protected constructor(protected model?: Ro.IRepresentation) {
    }

    populate(model: Ro.IRepresentation) {
        this.model = model;
    }

    getBody(): Ro.IRepresentation {
        if (this.method === "POST" || this.method === "PUT") {
            const m = clone(this.model);
            const up = clone(this.urlParms);
            return merge(m, up);
        }

        return {};
    }

    getUrl(): string {
        const url = this.hateoasUrl;
        const attrAsJson = clone(this.model);

        if (this.method === "GET" || this.method === "DELETE") {

            if (keys(attrAsJson).length > 0) {
                // there are model parms so encode everything into json

                const urlParmsAsJson = clone(this.urlParms);
                const asJson = merge(attrAsJson, urlParmsAsJson);
                if (keys(asJson).length > 0) {
                    const jsonMap = JSON.stringify(asJson);
                    const parmString = encodeURI(jsonMap);
                    return url + "?" + parmString;
                }
                return url;
            }
            if (keys(this.urlParms).length > 0) {
                // there are only url reserved parms so they can just be appended to url
                const urlParmString = reduce(this.urlParms, (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n, "");

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

    protected constructor(public valueMap: Ro.IValueMap, public id: string) {
        super(valueMap);
    }

    populate(wrapped: Ro.IValueMap) {
        super.populate(wrapped);
    }
}

export abstract class NestedRepresentation<T extends Ro.IResourceRepresentation> {

    private lazyLinks: Link[] | null;
    private lazyExtensions: Extensions;

    protected resource = () => this.model as T;

    protected constructor(private model: T) { }

    links(): Link[] {
        this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
        return this.lazyLinks as Link[];
    }

    protected update(newResource: T) {
        this.model = newResource;
        this.lazyLinks = null;
    }

    extensions(): Extensions {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.model.extensions);
        return this.lazyExtensions;
    }
}

// classes

export class RelParm {

    name: string | undefined;
    value: string | undefined;

    constructor(public asString: string) {
        this.decomposeParm();
    }

    private decomposeParm() {
        const regex = /(\w+)\W+(\w+)\W+/;
        const result = regex.exec(this.asString) || [];
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
            this.parms = map(splitPostFix.slice(1), s => new RelParm(s));
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
            if (parms[i].trim().substring(0, 16) === Constants.roDomainType) {
                this.xRoDomainType = (parms[i]).trim();
                this.domainType = (this.xRoDomainType.split("=")[1].replace(/\"/g, "")).trim();
            }
        }
    }
}

export class Value {

    // note this is different from constructor parm as we wrap ILink
    private wrapped: Link | Value[] | Ro.ScalarValueType | Blob;

    constructor(raw: Link | (Link | Ro.ValueType | Value)[] | Ro.ValueType | Blob | Value) {
        // can only be Link, number, boolean, string or null

        if (raw instanceof Array) {
            this.wrapped = map(raw as (Link | Ro.ValueType | Value)[], i => new Value(i));
        } else if (raw instanceof Link) {
            this.wrapped = raw;
        } else if (isILink(raw)) {
            this.wrapped = new Link(raw);
        } else if (raw instanceof Value) {
            this.wrapped = raw.wrapped;
        } else {
            this.wrapped = raw;
        }
    }

    static fromJsonString(jsonString: string, shortCutMarker: string, urlShortCuts: string[]): Value {
        const value = new Value(JSON.parse(jsonString));
        return value.decompress(shortCutMarker, urlShortCuts);
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
        const href = this.getHref();
        return href ? href.indexOf("data") === 0 : false;
    }

    isList(): boolean {
        return this.wrapped instanceof Array;
    }

    isNull(): boolean {
        return this.wrapped == null;
    }

    blob(): Blob | null {
        return this.isBlob() ? <Blob>this.wrapped : null;
    }

    link(): Link | null {
        return this.isReference() ? <Link>this.wrapped : null;
    }

    getHref(): string | null {
        const link = this.link();
        return link ? link.href() : null;
    }

    scalar(): Ro.ScalarValueType {
        return this.isScalar() ? this.wrapped as Ro.ScalarValueType : null;
    }

    list(): Value[] | null {
        return this.isList() ? this.wrapped as Value[] : null;
    }

    toString(): string {
        if (this.isReference()) {
            return this.link() !.title() || ""; // know true
        }

        if (this.isList()) {
            const list = this.list() !; // know true
            const ss = map(list, v => v.toString());
            return ss.length === 0 ? "" : reduce(ss, (m, s) => m + "-" + s, "");
        }

        return (this.wrapped == null) ? "" : this.wrapped.toString();
    }

    private compress(shortCutMarker: string, urlShortCuts: string[]): Value {
        if (this.isReference()) {
            const link = this.link()!.compress(shortCutMarker, urlShortCuts); // know true
            return new Value(link);
        }
        if (this.isList()) {
            const list = map(this.list()!, i => i.compress(shortCutMarker, urlShortCuts));
            return new Value(list);
        }

        if (this.scalar() && this.wrapped instanceof String) {
            const scalar = compress(this.wrapped as string, shortCutMarker, urlShortCuts);
            return new Value(scalar);
        }

        return this;
    }

    private decompress(shortCutMarker: string, urlShortCuts: string[]): Value {
        if (this.isReference()) {
            const link = this.link()!.decompress(shortCutMarker, urlShortCuts);  // know true
            return new Value(link);
        }
        if (this.isList()) {
            const list = map(this.list()!, i => i.decompress(shortCutMarker, urlShortCuts));
            return new Value(list);
        }

        if (this.scalar() && this.wrapped instanceof String) {
            const scalar = decompress(this.wrapped as string, shortCutMarker, urlShortCuts);
            return new Value(scalar);
        }

        return this;
    }

    toValueString(): string {
        if (this.isReference()) {
            return this.link() !.href();  // know true
        }
        return (this.wrapped == null) ? "" : this.wrapped.toString();
    }

    toJsonString(shortCutMarker: string, urlShortCuts: string[]): string {

        const cloneThis = this.compress(shortCutMarker, urlShortCuts);
        const value = cloneThis.wrapped;
        const raw = (value instanceof Link) ? value.wrapped : value;
        return JSON.stringify(raw);
    }

    setValue(target: Ro.IValue) {
        if (this.isFileReference()) {
            target.value = this.link() !.wrapped; // know true
        } else if (this.isReference()) {

            target.value = { "href": (this.link() as Link).href() }; // know true
        } else if (this.isList()) {
            const list = this.list() as Value[]; // know true
            target.value = map(list, v => v.isReference() ? <Ro.ILink>{ "href": v.link() !.href() } : v.scalar()) as Ro.ValueType[];
        } else if (this.isBlob()) {
            target.value = this.blob();
        } else {
            target.value = this.scalar();
        }
    }

    set(target: Dictionary<Ro.IValue | string>, name: string) {
        const t = target[name] = <Ro.IValue>{ value: null };
        this.setValue(t);
    }
}

export class ErrorValue {
    constructor(public value: Value, public invalidReason: string | null) { }
}

export class Result {
    constructor(
        public readonly wrapped: Ro.IDomainObjectRepresentation | RoCustom.ICustomListRepresentation | Ro.IScalarValueRepresentation | null,
        private readonly resultType: Ro.ResultTypeType
    ) { }

    object(): DomainObjectRepresentation | null {
        if (!this.isNull() && this.resultType === "object") {
            return new DomainObjectRepresentation(this.wrapped as Ro.IDomainObjectRepresentation);
        }
        return null;
    }

    list(): ListRepresentation | null {
        if (!this.isNull() && this.resultType === "list") {
            return new ListRepresentation(this.wrapped as RoCustom.ICustomListRepresentation);
        }
        return null;
    }

    scalar(): ScalarValueRepresentation | null {
        if (!this.isNull() && this.resultType === "scalar") {
            return new ScalarValueRepresentation(this.wrapped as Ro.IScalarValueRepresentation);
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
        const temp = this.valueMap;
        if (isIObjectOfType(temp)) {
            return temp.members;
        } else {
            return temp;
        }
    }

    constructor(private readonly valueMap: Ro.IValueMap | Ro.IObjectOfType, public statusCode: number, public warningMessage: string) {

    }

    valuesMap(): Dictionary<ErrorValue> {

        const values = pickBy(this.wrapped(), i => isIValue(i)) as Dictionary<Ro.IValue>;
        return mapValues(values, v => new ErrorValue(new Value(v.value), withNull(v.invalidReason)));
    }

    invalidReason(): string {

        const temp = this.valueMap;
        if (isIObjectOfType(temp)) {
            return temp[Constants.roInvalidReason] as string;
        }

        return this.wrapped()[Constants.roInvalidReason] as string;
    }

    containsError() {
        return !!this.invalidReason() || !!this.warningMessage || some(this.valuesMap(), ev => !!ev.invalidReason);
    }

}

export class UpdateMap extends ArgumentMap implements IHateoasModel {
    constructor(private readonly domainObject: DomainObjectRepresentation, valueMap: Ro.IValueMap) {
        super(valueMap, checkNotNull(domainObject.instanceId()));
        const link = domainObject.updateLink();

        if (link) {
            link.copyToHateoasModel(this);
        } else {
            error("UpdateMap - attempting to create update map for object without update link");
        }

        each(this.properties(), (value: Value, key: string) => {
            this.setProperty(key, value);
        });
    }

    properties(): Dictionary<Value> {
        // TODO fix any cast - broken by 'setValidateOnly below
        return mapValues(this.valueMap, (v: any) => new Value(v.value));
        // return mapValues(this.map, (v: Ro.IValue) => new Value(v.value));
    }

    setProperty(name: string, value: Value) {
        value.set(this.valueMap, name);
    }

    setValidateOnly() {
        // TODO a boolean is not assignable to an IValueMap - fix - confuses types system !
        (<any>this.valueMap)[Constants.roValidateOnly] = true;
    }
}

export class AddToRemoveFromMap extends ArgumentMap implements IHateoasModel {
    constructor(private readonly collectionResource: CollectionRepresentation, valueMap: Ro.IValueMap, add: boolean) {
        super(valueMap, collectionResource.collectionId());
        const link = add ? collectionResource.addToLink() : collectionResource.removeFromLink();
        if (link) {
            link.copyToHateoasModel(this);
        } else {
            const type = add ? "add" : "remove";
            error(`AddToRemoveFromMap attempting to create ${type} map for object without ${type} link`);
        }
    }
}

export class ModifyMap extends ArgumentMap implements IHateoasModel {
    constructor(private readonly propertyResource: PropertyRepresentation | PropertyMember, valueMap: Ro.IValueMap) {
        super(valueMap, getId(propertyResource));
        const link = propertyResource.modifyLink();

        if (link) {
            link.copyToHateoasModel(this);
        } else {
            error("ModifyMap attempting to create modify map for object without modify link");
        }
        propertyResource.value().set(this.valueMap, this.id);
    }
}

export class ClearMap extends ArgumentMap implements IHateoasModel {
    constructor(propertyResource: PropertyRepresentation | PropertyMember) {
        super({}, getId(propertyResource));
        const link = propertyResource.clearLink();

        if (link) {
            link.copyToHateoasModel(this);
        } else {
            error("ClearMap attempting to create clear map for object without clear link");
        }
    }
}

// REPRESENTATIONS

export abstract class ResourceRepresentation<T extends Ro.IResourceRepresentation> extends HateosModel {

    private lazyExtensions: Extensions;
    private lazyLinks: Link[] | null;
    protected resource = () => this.model as T;

    links(): Link[] {
        this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
        return this.lazyLinks as Link[];
    }

    populate(wrapped: T) {
        super.populate(wrapped);
    }

    extensions(): Extensions {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.resource().extensions);
        return this.lazyExtensions;
    }
}

// matches a action invoke resource 19.0 representation

export class InvokeMap extends ArgumentMap implements IHateoasModel {
    constructor(private readonly link: Link) {
        super(link.arguments() as Ro.IValueMap, "");
        link.copyToHateoasModel(this);
    }

    setParameter(name: string, value: Value) {
        value.set(this.valueMap, name);
    }
}

export class ActionResultRepresentation extends ResourceRepresentation<Ro.IActionInvokeRepresentation> {

    wrapped = () => this.resource() as Ro.IActionInvokeRepresentation;

    constructor() {
        super();
    }

    // links
    selfLink(): Link | null {
        return linkByRel(this.links(), "self") || null;
    }

    // link representations
    getSelf(): ActionResultRepresentation | null {
        const self = this.selfLink();
        return self ? self.getTargetAs<ActionResultRepresentation>() : null;
    }

    // properties
    resultType(): Ro.ResultTypeType {
        return this.wrapped().resultType;
    }

    result(): Result {
        return new Result(withNull(this.wrapped().result), this.resultType());
    }

    warningsOrMessages(): string | undefined {

        const has = (arr: string[]) => arr && arr.length > 0;
        const wOrM = has(this.extensions().warnings()) ? this.extensions().warnings() : this.extensions().messages();

        if (has(wOrM)) {
            return reduce(wOrM, (s, t) => s + " " + t, "");
        }

        return undefined;
    }

    shouldExpectResult(): boolean {
        return this.result().isNull() && this.resultType() !== "void";
    }

}

// matches an action representation 18.0

export interface IField extends IHasExtensions {
    id(): string;
    choices(): Dictionary<Value> | null;
    isScalar(): boolean;
    isCollectionContributed(): boolean;

    entryType(): EntryType;
    getPromptMap(): PromptMap | null;
    promptLink(): Link | null;
}

// matches 18.2.1
export class Parameter
    extends NestedRepresentation<Ro.IParameterRepresentation>
    implements IField {

    wrapped = () => this.resource() as Ro.IParameterRepresentation;

    // fix parent type
    constructor(wrapped: Ro.IParameterRepresentation, public parent: ActionMember | ActionRepresentation, private readonly paramId: string) {
        super(wrapped);
    }

    id() {
        return this.paramId;
    }

    // properties
    choices(): Dictionary<Value> | null {
        const customExtensions = this.extensions();
        // use custom choices extension by preference
        if (customExtensions.choices()) {
            return mapValues(customExtensions.choices(), v => new Value(v));
        }

        const choices = this.wrapped().choices;
        if (choices) {
            const values = map(choices, item => new Value(item));
            return fromPairs(map(values, v => [v.toString(), v])) as Dictionary<Value>;
        }
        return null;
    }

    promptLink(): Link | null {
        return linkByNamespacedRel(this.links(), "prompt") || null;
    }

    getPromptMap(): PromptMap | null {
        const promptLink = this.promptLink();
        if (promptLink) {
            const pr = promptLink.getTargetAs<PromptRepresentation>();
            return new PromptMap(promptLink, pr.instanceId());
        }
        return null;
    }

    default(): Value {
        const dflt = this.wrapped().default == null ? (isScalarType(this.extensions().returnType()) ? "" : null) : this.wrapped().default;
        return new Value(withNull(dflt));
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
        const isOnList = (myparent instanceof ActionMember || myparent instanceof ActionRepresentation) &&
            (myparent.parent instanceof ListRepresentation || myparent.parent instanceof CollectionRepresentation || myparent.parent instanceof CollectionMember);
        const isList = this.isList();
        return isList && isOnList;
    }

    private hasChoices(): boolean { return some(this.choices() || {}); }

    entryType(): EntryType {

        const promptLink = this.promptLink() as Link;
        if (promptLink) {
            // ConditionalChoices, ConditionalMultipleChoices, AutoComplete

            if (isAutoComplete(promptLink.arguments())) {

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

export class ActionRepresentation extends ResourceRepresentation<Ro.IActionRepresentation> {

    private parameterMap: Dictionary<Parameter>;
    parent: IHasActions;
    wrapped = () => this.resource() as Ro.IActionRepresentation;

    // links
    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link; // known to exist
    }

    upLink(): Link {
        return linkByRel(this.links(), "up") as Link; // known to exist
    }

    invokeLink(): Link | null {
        return linkByNamespacedRel(this.links(), "invoke") || null; // may not exist if disabled
    }

    // linked representations
    getSelf(): ActionRepresentation {
        return this.selfLink().getTargetAs<ActionRepresentation>();
    }

    getUp(): DomainObjectRepresentation {
        return this.upLink().getTargetAs<DomainObjectRepresentation>();
    }

    getInvokeMap(): InvokeMap | null {
        const link = this.invokeLink();
        return link ? new InvokeMap(link) : null;
    }

    // properties

    actionId(): string {
        return this.wrapped().id;
    }

    private initParameterMap(): void {

        if (!this.parameterMap) {
            const parameters = this.wrapped().parameters;
            this.parameterMap = mapValues(parameters, (p, id) => new Parameter(p, this, id!));
        }
    }

    parameters(): Dictionary<Parameter> {
        this.initParameterMap();
        return this.parameterMap;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason || "";
    }

    isQueryOnly(): boolean {
        const invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "GET";
    }

    isNotQueryOnly(): boolean {
        const invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() !== "GET";
    }

    isPotent(): boolean {
        const invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "POST";
    }
}

// new in 1.1 15.0 in spec

export class PromptMap extends ArgumentMap implements IHateoasModel {
    constructor(private readonly link: Link, private readonly promptId: string) {
        super(link.arguments() as Ro.IValueMap, promptId);
        link.copyToHateoasModel(this);
    }

    private promptMap() {
        return this.valueMap as Ro.IPromptMap;
    }

    setSearchTerm(term: string) {
        this.setArgument(Constants.roSearchTerm, new Value(term));
    }

    setArgument(name: string, val: Value) {
        val.set(this.valueMap, name);
    }

    setArguments(args: Dictionary<Value>) {
        each(args, (arg, key) => this.setArgument(key!, arg));
    }

    setMember(name: string, value: Value) {
        value.set(this.promptMap()["x-ro-nof-members"] as Ro.IValueMap, name);
    }

    setMembers(objectValues: () => Dictionary<Value>) {
        if (this.valueMap["x-ro-nof-members"]) {
            forEach(objectValues(), (v, k) => this.setMember(k!, v));
        }
    }
}

export class PromptRepresentation extends ResourceRepresentation<Ro.IPromptRepresentation> {

    wrapped = () => this.resource() as Ro.IPromptRepresentation;

    constructor() {
        super(emptyResource());
    }

    // links
    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link; // known to exist
    }

    upLink(): Link {
        return linkByRel(this.links(), "up") as Link; // known to exist
    }

    // linked representations
    getSelf(): PromptRepresentation {
        return this.selfLink().getTargetAs<PromptRepresentation>();
    }

    getUp(): DomainObjectRepresentation {
        return this.upLink().getTargetAs<DomainObjectRepresentation>();
    }

    // properties

    instanceId(): string {
        return this.wrapped().id;
    }

    choices(addEmpty: boolean): Dictionary<Value> | null {
        const ch = this.wrapped().choices;
        if (ch) {
            let values = map(ch, item => new Value(item));

            if (addEmpty) {
                const emptyValue = new Value("");
                values = concat<Value>([emptyValue], values);
            }

            return fromPairs(map(values, v => [v.toString(), v])) as Dictionary<Value>;
        }
        return null;
    }
}

// matches a collection representation 17.0
export class CollectionRepresentation extends ResourceRepresentation<RoCustom.ICustomCollectionRepresentation> implements IHasActions {
    private actionMemberMap: Dictionary<ActionMember>;
    private lazyValue: Link[] | null;
    wrapped = () => this.resource() as RoCustom.ICustomCollectionRepresentation;

    // links
    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link; // known to exist
    }

    upLink(): Link {
        return linkByRel(this.links(), "up") as Link; // known to exist
    }

    addToLink(): Link | null {
        return linkByNamespacedRel(this.links(), "add-to") || null;
    }

    removeFromLink(): Link | null {
        return linkByNamespacedRel(this.links(), "remove-from") || null;
    }

    // linked representations
    getSelf(): CollectionRepresentation {
        return this.selfLink().getTargetAs<CollectionRepresentation>();
    }

    getUp(): DomainObjectRepresentation {
        return this.upLink().getTargetAs<DomainObjectRepresentation>();
    }

    setFromMap(addToRemoveFromMap: AddToRemoveFromMap) {
        // this.set(map.attributes);
        assign(this.resource(), addToRemoveFromMap.valueMap);
    }

    private addToMap() {
        const link = this.addToLink();
        return link ? link.arguments() as Ro.IValueMap : null;
    }

    getAddToMap(): AddToRemoveFromMap | null {
        const addToMap = this.addToMap();
        return addToMap ? new AddToRemoveFromMap(this, addToMap, true) : null;
    }

    private removeFromMap() {
        const link = this.addToLink();
        return link ? link.arguments() as Ro.IValueMap : null;
    }

    getRemoveFromMap(): AddToRemoveFromMap | null {
        const removeFromMap = this.removeFromMap();
        return removeFromMap ? new AddToRemoveFromMap(this, removeFromMap, false) : null;
    }

    // properties

    collectionId(): string {
        return this.wrapped().id;
    }

    size(): number {
        return this.value().length;
    }

    value(): Link[] {
        this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value!);
        return this.lazyValue!;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason || "";
    }

    hasTableData = () => {
        const valueLinks = this.value();
        return valueLinks && some(valueLinks, (i: Link) => i.members());
    }

    actionMembers() {
        this.actionMemberMap = this.actionMemberMap || mapValues(this.wrapped().members, (m, id) => Member.wrapMember(m, this, id!)) as Dictionary<ActionMember>;
        return this.actionMemberMap;
    }

    actionMember(id: string): ActionMember {
        return getMember(this.actionMembers(), id, this.collectionId()) !;
    }

    hasActionMember(id: string): boolean {
        return !!this.actionMembers()[id];
    }
}

// matches a property representation 16.0
export class PropertyRepresentation extends ResourceRepresentation<Ro.IPropertyRepresentation> {

    wrapped = () => this.resource() as Ro.IPropertyRepresentation;

    // links
    modifyLink(): Link | null {
        return linkByNamespacedRel(this.links(), "modify") || null;
    }

    clearLink(): Link | null {
        return linkByNamespacedRel(this.links(), "clear") || null;
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link; // known to exist
    }

    upLink(): Link {
        return linkByRel(this.links(), "up") as Link; // known to exist
    }

    promptLink(): Link | null {
        return linkByNamespacedRel(this.links(), "prompt") || null;
    }

    private modifyMap() {
        const link = this.modifyLink();
        return link ? link.arguments() as Ro.IValueMap : null;
    }

    // linked representations
    getSelf(): PropertyRepresentation {
        return this.selfLink().getTargetAs<PropertyRepresentation>();
    }

    getUp(): DomainObjectRepresentation {
        return this.upLink().getTargetAs<DomainObjectRepresentation>();
    }

    setFromModifyMap(modifymap: ModifyMap) {
        // this.set(map.attributes);
        assign(this.resource(), modifymap.valueMap);
    }

    getModifyMap(): ModifyMap | null {
        const modifymap = this.modifyMap();
        return modifymap ? new ModifyMap(this, modifymap) : null;
    }

    getClearMap(): ClearMap | null {
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
        return new Value(withNull(this.wrapped().value));
    }

    choices(): Dictionary<Value> | null {

        // use custom choices extension by preference

        const choices = this.extensions().choices();
        if (choices) {
            return mapValues(choices, v => new Value(v));
        }
        const ch = this.wrapped().choices;
        if (ch) {
            const values = map(ch, item => new Value(item));
            return fromPairs(map(values, v => [v.toString(), v])) as Dictionary<Value>;
        }
        return null;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason || "";
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
export class Member<T extends Ro.IMember> extends NestedRepresentation<Ro.IMember> {

    constructor(wrapped: T) {
        super(wrapped);
    }

    static wrapLinkMember(toWrap: Ro.IPropertyMember | Ro.ICollectionMember, parent: Link, id: string): PropertyMember | CollectionMember {

        if (toWrap.memberType === "property") {
            return new PropertyMember(toWrap as Ro.IPropertyMember, parent, id);
        }

        if (toWrap.memberType === "collection") {
            return new CollectionMember(toWrap as Ro.ICollectionMember, parent, id);
        }

        return error('Unexpected member: ${id}');
    }

    static wrapMember(toWrap: Ro.IPropertyMember | Ro.ICollectionMember | Ro.IActionMember, parent: DomainObjectRepresentation | IHasActions, id: string): Member<Ro.IMember> {

        if (toWrap.memberType === "property" && parent instanceof DomainObjectRepresentation) {
            return new PropertyMember(toWrap as Ro.IPropertyMember, parent, id);
        }

        if (toWrap.memberType === "collection" && parent instanceof DomainObjectRepresentation) {
            return new CollectionMember(toWrap as Ro.ICollectionMember, parent, id);
        }

        if (toWrap.memberType === "action") {
            const member = new ActionMember(toWrap as Ro.IActionMember, parent, id);

            if (member.invokeLink()) {
                return new InvokableActionMember(toWrap as Ro.IActionMember, parent, id);
            }

            return member;
        }

        return error('Unexpected member: ${id}');
    }

    wrapped = () => this.resource() as Ro.IMember;

    update(newValue: Ro.IMember) {
        super.update(newValue);
    }

    memberType(): Ro.MemberTypeType {
        return this.wrapped().memberType;
    }

    detailsLink(): Link | null {
        return linkByNamespacedRel(this.links(), "details") || null;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason || "";
    }

    isScalar(): boolean {
        return isScalarType(this.extensions().returnType());
    }
}

// matches 14.4.1
export class PropertyMember extends Member<Ro.IPropertyMember> implements IField {

    wrapped = () => this.resource() as Ro.IPropertyMember;

    constructor(wrapped: Ro.IPropertyMember, public parent: DomainObjectRepresentation | Link, private readonly propId: string) {
        super(wrapped);
    }

    // inlined

    id(): string {
        return this.propId;
    }

    modifyLink(): Link | null {
        return linkByNamespacedRel(this.links(), "modify") || null;
    }

    clearLink(): Link | null {
        return linkByNamespacedRel(this.links(), "clear") || null;
    }

    private modifyMap() {
        const link = this.modifyLink();
        return link ? link.arguments() as Ro.IValueMap : null;
    }

    setFromModifyMap(modifyMap: ModifyMap) {
        forOwn(modifyMap.valueMap, (v, k) => {
            (<any>this.wrapped)[k!] = v;
        });
    }

    getModifyMap(id: string): ModifyMap | null {
        const modifyMap = this.modifyMap();
        return modifyMap ? new ModifyMap(this, modifyMap) : null;
    }

    getClearMap(id: string): ClearMap | null {
        return this.clearLink() ? new ClearMap(this) : null;
    }

    getPromptMap(): PromptMap | null {
        const link = this.promptLink();
        if (link) {
            const pr = link.getTargetAs<PromptRepresentation>();
            return new PromptMap(link, pr.instanceId());
        }
        return null;
    }

    value(): Value {
        return new Value(withNull(this.wrapped().value));
    }

    isScalar(): boolean {
        return isScalarType(this.extensions().returnType());
    }

    attachmentLink(): Link | null {
        return linkByNamespacedRel(this.links(), "attachment") || null;
    }

    promptLink(): Link | null {
        return linkByNamespacedRel(this.links(), "prompt") || null;
    }

    getDetails(): PropertyRepresentation | null {
        const link = this.detailsLink();
        return link ? link.getTargetAs<PropertyRepresentation>() : null;
    }

    private hasChoices(): boolean {
        return this.wrapped().hasChoices;
    }

    private hasPrompt(): boolean {
        return !!this.promptLink();
    }

    choices(): Dictionary<Value> | null {

        // use custom choices extension by preference
        const choices = this.extensions().choices();
        if (choices) {
            return mapValues(choices, v => new Value(v));
        }
        const ch = this.wrapped().choices;
        if (ch) {
            const values = map(ch, (item) => new Value(item));
            return fromPairs(map(values, v => [v.toString(), v])) as Dictionary<Value>;
        }
        return null;
    }

    private hasConditionalChoices(): boolean {
        return !!this.promptLink() && !this.hasPrompt();
    }

    // This is actually not relevant to a property. Slight smell here!

    isCollectionContributed(): boolean {
        return false;
    }

    entryType(): EntryType {

        const link = this.promptLink();
        if (link) {
            // ConditionalChoices, ConditionalMultipleChoices, AutoComplete

            if (isAutoComplete(link.arguments())) {
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
    extends Member<RoCustom.ICustomCollectionMember>
    implements IHasLinksAsValue, IHasActions {

    private lazyValue: Link[] | null;
    private actionMemberMap: Dictionary<ActionMember>;
    etagDigest?: string;

    wrapped = () => this.resource() as RoCustom.ICustomCollectionMember;

    constructor(wrapped: Ro.ICollectionMember, public parent: DomainObjectRepresentation | Link, private readonly id: string) {
        super(wrapped);
        this.etagDigest = parent instanceof DomainObjectRepresentation ?  parent.etagDigest : undefined;
    }

    collectionId(): string {
        return this.id;
    }

    value(): Link[] | null {
        this.lazyValue = this.lazyValue || (this.wrapped().value ? wrapLinks(this.wrapped().value!) : null);
        return this.lazyValue;
    }

    size(): number | null {
        return withNull(this.wrapped().size);
    }

    getDetails(): CollectionRepresentation | null {
        const link = this.detailsLink();
        return link ? link.getTargetAs<CollectionRepresentation>() : null;
    }

    hasTableData = () => {
        const valueLinks = this.value();
        return valueLinks && some(valueLinks, (i: Link) => i.members());
    }

    actionMembers(): Dictionary<ActionMember> {
        const members = this.wrapped().members;
        if (members) {
            return this.actionMemberMap || mapValues(members, (m, id) => Member.wrapMember(m, this, id!)) as Dictionary<ActionMember>;
        }
        return {};
    }

    hasActionMember(id: string): boolean {
        return !!this.actionMembers()[id];
    }

    actionMember(id: string): ActionMember {
        return getMember(this.actionMembers(), id, this.collectionId()) !;
    }
}

// matches 14.4.3
export class ActionMember extends Member<Ro.IActionMember> {

    wrapped = () => this.resource() as Ro.IActionMember;

    constructor(wrapped: Ro.IActionMember, public parent: IHasActions, private readonly id: string) {
        super(wrapped);
    }

    actionId(): string {
        return this.id;
    }

    getDetails(): ActionRepresentation | null {
        const link = this.detailsLink();
        if (link) {
            const details = link.getTargetAs<ActionRepresentation>();
            details.parent = this.parent;
            return details;
        }
        return null;
    }

    // 1.1 inlined

    invokeLink(): Link | null {
        return linkByNamespacedRel(this.links(), "invoke") || null;
    }

    disabledReason(): string {
        return this.wrapped().disabledReason || "";
    }
}

export class InvokableActionMember extends ActionMember {

    private parameterMap: Dictionary<Parameter>;

    constructor(wrapped: Ro.IActionMember, parent: IHasActions, id: string) {
        super(wrapped, parent, id);
    }

    getInvokeMap(): InvokeMap | null {
        const invokeLink = this.invokeLink();

        return invokeLink ? new InvokeMap(invokeLink) : null;
    }

    isQueryOnly(): boolean {
        const invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "GET";
    }

    isNotQueryOnly(): boolean {
        const invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() !== "GET";
    }

    isPotent(): boolean {
        const invokeLink = this.invokeLink();
        return !!invokeLink && invokeLink.method() === "POST";
    }

    private initParameterMap(): void {

        if (!this.parameterMap) {
            const parameters = this.wrapped().parameters;
            this.parameterMap = mapValues(parameters, (p, id) => new Parameter(p, this, id!));
        }
    }

    parameters(): Dictionary<Parameter> {
        this.initParameterMap();
        return this.parameterMap;
    }
}

export class DomainObjectRepresentation extends ResourceRepresentation<Ro.IDomainObjectRepresentation> implements IHasActions {

    private memberMap: Dictionary<Member<Ro.IMember>>;
    private propertyMemberMap: Dictionary<PropertyMember>;
    private collectionMemberMap: Dictionary<CollectionMember>;
    private actionMemberMap: Dictionary<ActionMember>;
    private oid: ObjectIdWrapper;

    wrapped = () => this.resource() as Ro.IDomainObjectRepresentation;

    constructor(model?: Ro.IRepresentation) {
        super(model);
    }

    id(): string {
        return `${this.domainType() || this.serviceId()}${this.instanceId() ? `${this.keySeparator}${this.instanceId()}` : ""}`;
    }

    title(): string {
        return this.wrapped().title;
    }

    domainType(): string | null {
        return withNull(this.wrapped().domainType);
    }

    serviceId(): string | null {
        return withNull(this.wrapped().serviceId);
    }

    instanceId(): string | null {
        return withNull(this.wrapped().instanceId);
    }

    private resetMemberMaps() {
        const members = this.wrapped().members;
        this.memberMap = mapValues(members, (m, id) => Member.wrapMember(m, this, id!) !);
        this.propertyMemberMap = pickBy(this.memberMap, (m: Member<Ro.IMember>) => m.memberType() === "property") as Dictionary<PropertyMember>;
        this.collectionMemberMap = pickBy(this.memberMap, (m: Member<Ro.IMember>) => m.memberType() === "collection") as Dictionary<CollectionMember>;
        this.actionMemberMap = pickBy(this.memberMap, (m: Member<Ro.IMember>) => m.memberType() === "action") as Dictionary<ActionMember>;
    }

    private initMemberMaps() {
        if (!this.memberMap) {
            this.resetMemberMaps();
        }
    }

    members(): Dictionary<Member<Ro.IMember>> {
        this.initMemberMaps();
        return this.memberMap;
    }

    propertyMembers(): Dictionary<PropertyMember> {
        this.initMemberMaps();
        return this.propertyMemberMap;
    }

    collectionMembers(): Dictionary<CollectionMember> {
        this.initMemberMaps();
        return this.collectionMemberMap;
    }

    actionMembers(): Dictionary<ActionMember> {
        this.initMemberMaps();
        return this.actionMemberMap;
    }

    member(id: string): Member<Ro.IMember> {
        return this.members()[id];
    }

    propertyMember(id: string): PropertyMember {
        return this.propertyMembers()[id];
    }

    collectionMember(id: string): CollectionMember {
        return this.collectionMembers()[id];
    }

    hasActionMember(id: string): boolean {
        return !!this.actionMembers()[id];
    }

    actionMember(id: string): ActionMember {
        return getMember(this.actionMembers(), id, this.id()) !;
    }

    updateLink(): Link | null {
        return linkByNamespacedRel(this.links(), "update") || null;
    }

    isTransient(): boolean {
        return !!this.persistLink();
    }

    persistLink(): Link | null {
        return linkByNamespacedRel(this.links(), "persist") || null;
    }

    selfLink(): Link | null {
        return linkByRel(this.links(), "self") || null;
    }

    private updateMap() {
        const link = this.updateLink();
        return link ? link.arguments() as Ro.IValueMap : null;
    }

    private persistMap() {
        const link = this.persistLink();
        return link ? link.arguments() as Ro.IObjectOfType : null;
    }

    // linked representations
    getSelf() {
        const link = this.selfLink();
        return link ? link.getTargetAs<DomainObjectRepresentation>() : null;
    }

    getPersistMap() {
        const persistMap = validateExists(this.persistMap(), "PersistMap");
        return new PersistMap(this, persistMap);
    }

    getUpdateMap() {
        const updateMap = validateExists(this.updateMap(), "UpdateMap");
        return new UpdateMap(this, updateMap);
    }

    setInlinePropertyDetails(flag: boolean) {
        this.setUrlParameter(Constants.roInlinePropertyDetails, flag);
    }

    getOid(): ObjectIdWrapper {
        if (!this.oid) {
            this.oid = ObjectIdWrapper.fromObject(this);
        }

        return this.oid;
    }

    updateSelfLinkWithTitle() {
        const link = this.selfLink();
        if (link) {
            link.setTitle(this.title());
        }
        return link;
    }
}

export class MenuRepresentation extends ResourceRepresentation<RoCustom.IMenuRepresentation> implements IHasActions {

    private memberMap: Dictionary<Member<Ro.IMember>>;
    private actionMemberMap: Dictionary<ActionMember>;
    wrapped = () => this.resource() as RoCustom.IMenuRepresentation;

    constructor() {
        super();
    }

    title(): string {
        return this.wrapped().title;
    }

    menuId(): string {
        return this.wrapped().menuId;
    }

    private resetMemberMaps() {
        const members = this.wrapped().members;
        this.memberMap = mapValues(members, (m, id) => Member.wrapMember(m, this, id!));
        this.actionMemberMap = pickBy(this.memberMap, m => m.memberType() === "action") as Dictionary<ActionMember>;
    }

    private initMemberMaps() {
        if (!this.memberMap) {
            this.resetMemberMaps();
        }
    }

    members(): Dictionary<Member<Ro.IMember>> {
        this.initMemberMaps();
        return this.memberMap;
    }

    actionMembers(): Dictionary<ActionMember> {
        this.initMemberMaps();
        return this.actionMemberMap;
    }

    member(id: string): Member<Ro.IMember> {
        return this.members()[id];
    }

    hasActionMember(id: string): boolean {
        return !!this.actionMembers()[id];
    }

    actionMember(id: string): ActionMember {
        return getMember(this.actionMembers(), id, this.menuId()) !;
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link; // mandatory
    }

    // linked representations
    getSelf(): MenuRepresentation {
        return this.selfLink().getTargetAs<MenuRepresentation>();
    }

}

// matches scalar representation 12.0
export class ScalarValueRepresentation extends NestedRepresentation<Ro.IScalarValueRepresentation> {

    wrapped = () => this.resource() as Ro.IScalarValueRepresentation;

    constructor(wrapped: Ro.IScalarValueRepresentation) {
        super(wrapped);
    }

    value(): Value {
        return new Value(this.wrapped().value);
    }
}

// matches List Representation 11.0
export class ListRepresentation
    extends ResourceRepresentation<RoCustom.ICustomListRepresentation> implements IHasLinksAsValue, IHasActions {

    private actionMemberMap: Dictionary<ActionMember>;
    private lazyValue: Link[];

    constructor(model?: Ro.IRepresentation) {
        super(model);
    }

    wrapped = () => this.resource() as RoCustom.ICustomListRepresentation;

    // links
    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link;
    }

    // linked representations
    getSelf(): ListRepresentation {
        return this.selfLink().getTargetAs<ListRepresentation>();
    }

    value(): Link[] {
        this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
        return this.lazyValue;
    }

    pagination(): RoCustom.IPagination | null {
        return this.wrapped().pagination || null;
    }

    actionMembers() {
        this.actionMemberMap = this.actionMemberMap || mapValues(this.wrapped().members, (m, id) => Member.wrapMember(m, this, id!)) as Dictionary<ActionMember>;
        return this.actionMemberMap;
    }

    actionMember(id: string): ActionMember {
        return getMember(this.actionMembers(), id, "list") !;
    }

    hasActionMember(id: string): boolean {
        return !!this.actionMembers()[id];
    }

    hasTableData = () => {
        const valueLinks = this.value();
        return valueLinks && some(valueLinks, (i: Link) => i.members());
    }
}

export interface IErrorDetails {
    message(): string;
    stackTrace(): string[];
}

// matches the error representation 10.0
export class ErrorRepresentation extends ResourceRepresentation<Ro.IErrorRepresentation> implements IErrorDetails {

    constructor() {
        super();
    }

    static create(message: string, stackTrace?: string[], causedBy?: Ro.IErrorDetailsRepresentation) {
        const rawError = {
            links: [] as any[],
            extensions: {},
            message: message,
            stackTrace: stackTrace,
            causedBy: causedBy
        };
        const errorRep = new ErrorRepresentation();
        errorRep.populate(rawError);
        return errorRep;
    }

    wrapped = () => this.resource() as Ro.IErrorRepresentation;

    // scalar properties
    message(): string {
        return this.wrapped().message;
    }

    stackTrace(): string[] {
        return this.wrapped().stackTrace || [];
    }

    causedBy(): IErrorDetails | undefined {
        const cb = this.wrapped().causedBy;
        return cb ? {
            message: () => cb.message,
            stackTrace: () => cb.stackTrace || []
        } : undefined;
    }
}

// matches Objects of Type Resource 9.0
export class PersistMap extends HateosModel implements IHateoasModel {

    constructor(private readonly domainObject: DomainObjectRepresentation, private readonly objectOfType: Ro.IObjectOfType) {
        super(objectOfType);
        const link = domainObject.persistLink();
        if (link) {
            link.copyToHateoasModel(this);
        } else {
            error("PersistMap attempting to create persist map for object with no persist link");
        }
    }

    setMember(name: string, value: Value) {
        value.set(this.objectOfType.members, name);
    }

    setValidateOnly() {
        (<any>this.objectOfType)[Constants.roValidateOnly] = true;
    }
}

// matches the version representation 8.0
export class VersionRepresentation extends ResourceRepresentation<Ro.IVersionRepresentation> {

    wrapped = () => this.resource() as Ro.IVersionRepresentation;

    // links
    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link; // mandatory
    }

    upLink(): Link {
        return linkByRel(this.links(), "up") as Link; // mandatory
    }

    // linked representations
    getSelf(): VersionRepresentation {
        return this.selfLink().getTargetAs<VersionRepresentation>();
    }

    getUp(): HomePageRepresentation {
        return this.upLink().getTargetAs<HomePageRepresentation>();
    }

    // scalar properties
    specVersion(): string {
        return this.wrapped().specVersion;
    }

    implVersion(): string | null {
        return this.wrapped().implVersion || null;
    }

    optionalCapabilities(): Ro.IOptionalCapabilities {
        return this.wrapped().optionalCapabilities;
    }
}

// matches Domain Services Representation 7.0
export class DomainServicesRepresentation extends ListRepresentation {

    wrapped = () => this.resource() as RoCustom.ICustomListRepresentation;

    // links
    upLink(): Link {
        return linkByRel(this.links(), "up") as Link; // mandatory
    }

    // linked representations
    getSelf(): DomainServicesRepresentation {
        return this.selfLink().getTargetAs<DomainServicesRepresentation>();
    }

    getUp(): HomePageRepresentation {
        return this.upLink().getTargetAs<HomePageRepresentation>();
    }

    getService(serviceType: string) {
        const serviceLink = find(this.value(), link => link.rel().parms[0].value === serviceType);
        return serviceLink ? serviceLink.getTargetAs<DomainObjectRepresentation>() : null;
    }
}

// custom
export class MenusRepresentation extends ListRepresentation {

    wrapped = () => this.resource() as RoCustom.ICustomListRepresentation;

    // links
    upLink(): Link {
        return linkByRel(this.links(), "up") as Link; // mandatory
    }

    // linked representations
    getSelf(): MenusRepresentation {
        return this.selfLink().getTargetAs<MenusRepresentation>();
    }

    getUp(): HomePageRepresentation {
        return this.upLink().getTargetAs<HomePageRepresentation>();
    }

    getMenu(menuId: string) {
        const menuLink = find(this.value(), link => link.rel().parms[0].value === menuId);
        if (menuLink) {
            return menuLink.getTargetAs<MenuRepresentation>();
        }
        error(`MenusRepresentation:getMenu Failed to find menu ${menuId}`);
    }
}

// matches the user representation 6.0
export class UserRepresentation extends ResourceRepresentation<Ro.IUserRepresentation> {

    wrapped = () => this.resource() as Ro.IUserRepresentation;

    // links
    selfLink(): Link {
        return linkByRel(this.links(), "self")!; // mandatory
    }

    upLink(): Link {
        return linkByRel(this.links(), "up")!; // mandatory
    }

    // linked representations
    getSelf(): UserRepresentation {
        return this.selfLink().getTargetAs<UserRepresentation>();
    }

    getUp(): HomePageRepresentation {
        return this.upLink().getTargetAs<HomePageRepresentation>();
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

export class DomainTypeActionInvokeRepresentation extends ResourceRepresentation<Ro.IDomainTypeActionInvokeRepresentation> {

    wrapped = () => this.resource() as Ro.IDomainTypeActionInvokeRepresentation;

    constructor(againstType: string, toCheckType: string, appPath: string) {
        super();
        this.hateoasUrl = `${appPath}/domain-types/${toCheckType}/type-actions/isSubtypeOf/invoke`;
        this.urlParms = {};
        this.urlParms["supertype"] = againstType;
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link;
    }

    // linked representations
    getSelf(): DomainTypeActionInvokeRepresentation {
        return this.selfLink().getTargetAs<DomainTypeActionInvokeRepresentation>();
    }

    id(): string {
        return this.wrapped().id;
    }

    value(): boolean {
        return this.wrapped().value;
    }
}

// matches the home page representation  5.0
export class HomePageRepresentation extends ResourceRepresentation<Ro.IHomePageRepresentation> {

    constructor(rep: Ro.IRepresentation, appPath: string) {
        super(rep);
        this.hateoasUrl = appPath;
    }

    wrapped = () => this.resource() as Ro.IHomePageRepresentation;

    // links
    serviceLink(): Link {
        return linkByNamespacedRel(this.links(), "services") as Link;
    }

    userLink(): Link {
        return linkByNamespacedRel(this.links(), "user") as Link;
    }

    selfLink(): Link {
        return linkByRel(this.links(), "self") as Link;
    }

    versionLink(): Link {
        return linkByNamespacedRel(this.links(), "version") as Link;
    }

    // custom
    menusLink(): Link {
        return linkByNamespacedRel(this.links(), "menus") as Link;
    }

    // linked representations
    getSelf(): HomePageRepresentation {
        return this.selfLink().getTargetAs<HomePageRepresentation>();
    }

    getUser(): UserRepresentation {
        return this.userLink().getTargetAs<UserRepresentation>();
    }

    getDomainServices(): DomainServicesRepresentation {
        // cannot use getTarget here as that will just return a ListRepresentation
        const domainServices = new DomainServicesRepresentation();
        this.serviceLink().copyToHateoasModel(domainServices);
        return domainServices;
    }

    getVersion(): VersionRepresentation {
        return this.versionLink().getTargetAs<VersionRepresentation>();
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
    private oid: ObjectIdWrapper;
    private lazyExtensions: Extensions;
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

    constructor(public wrapped: Ro.ILink) { }

    compress(shortCutMarker: string, urlShortCuts: string[]) {
        const href = compress(this.wrapped.href, shortCutMarker, urlShortCuts);
        return new Link({ href: href });
    }

    decompress(shortCutMarker: string, urlShortCuts: string[]) {
        const href = decompress(this.wrapped.href, shortCutMarker, urlShortCuts);
        return new Link({ href: href });
    }

    href(): string {
        return decodeURIComponent(this.wrapped.href);
    }

    method(): Ro.HttpMethodsType {
        return this.wrapped.method!;
    }

    rel(): Rel {
        return new Rel(this.wrapped.rel!);
    }

    type(): MediaType {
        return new MediaType(this.wrapped.type!);
    }

    title(): string | null {
        return withNull(this.wrapped.title);
    }

    // Typically used to set a title on a link that doesn't naturally have one e.g. Self link
    setTitle(title: string): void {
        this.wrapped.title = title;
    }

    arguments(): Ro.IValue | Ro.IValueMap | Ro.IObjectOfType | Ro.IPromptMap | null {
        return withNull(this.wrapped.arguments);
    }

    members(): Dictionary<PropertyMember | CollectionMember> | null {
        const members = (this.wrapped as RoCustom.ICustomLink).members;
        return members ? mapValues(members, (m, id) => Member.wrapLinkMember(m, this, id!)) : null;
    }

    extensions(): Extensions {
        this.lazyExtensions = this.lazyExtensions || new Extensions(this.wrapped.extensions!);
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

    // get the object that this link points to
    getTarget(): IHateoasModel {
        const target = this.getHateoasTarget(this.type().representationType);
        this.copyToHateoasModel(target);
        return target;
    }

    getTargetAs<T extends IHateoasModel>(): T {
        const target = this.getHateoasTarget(this.type().representationType);
        this.copyToHateoasModel(target);
        return target as T;
    }

    // helper
    getOid(keySeparator: string): ObjectIdWrapper {
        if (!this.oid) {
            this.oid = ObjectIdWrapper.fromLink(this, keySeparator);
        }

        return this.oid;
    }
}

export enum EntryType { FreeForm, Choices, MultipleChoices, ConditionalChoices, MultipleConditionalChoices, AutoComplete, File }
