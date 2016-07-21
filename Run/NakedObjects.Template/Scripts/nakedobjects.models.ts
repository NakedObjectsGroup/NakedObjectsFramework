//  Copyright 2013-2014 Naked Objects Group Ltd
//  Licensed under the Apache License, Version 2.0(the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

// ABOUT THIS FILE:
// nakedobjects.models defines a set of classes that correspond directly to the JSON representations returned by Restful Objects
// resources.  These classes provide convenient methods for navigating the contents of those representations, and for
// following links to other resources.

/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.config.ts" />
/// <reference path="nakedobjects.rointerfaces.ts" />

namespace NakedObjects.Models {
    import ICustomExtensions = RoInterfaces.Custom.ICustomExtensions;
    import ILink = RoInterfaces.ILink;
    import IMenuRepresentation = RoInterfaces.Custom.IMenuRepresentation;
    import IValue = RoInterfaces.IValue;
    import IResourceRepresentation = RoInterfaces.IResourceRepresentation;
    import ICustomListRepresentation = RoInterfaces.Custom.ICustomListRepresentation;
    import IRange = RoInterfaces.Custom.IRange;
    import ICustomLink = RoInterfaces.Custom.ICustomLink;
    import httpMethodsType = RoInterfaces.httpMethodsType;
    import resultTypeType = RoInterfaces.resultTypeType;
    import memberTypeType = RoInterfaces.memberTypeType;
    import IMember = RoInterfaces.IMember;
    import scalarValueType = RoInterfaces.scalarValueType;
    import valueType = RoInterfaces.valueType;


    // helper functions 

    function isScalarType(typeName: string) {
        return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
    }

    function isListType(typeName: string) {
        return typeName === "list";
    }

    function emptyResource(): IResourceRepresentation {
        return { links: [] as ILink[], extensions: {} };
    }

    function isILink(object: any): object is ILink {
        return object && object instanceof Object && "href" in object;
    }

    function isIObjectOfType(object: any): object is RoInterfaces.IObjectOfType {
        return object && object instanceof Object && "members" in object;
    }

    function isIValue(object: any): object is RoInterfaces.IValue {
        return object && object instanceof Object && "value" in object;
    }

    export function isResourceRepresentation(object: any): object is RoInterfaces.IResourceRepresentation {
        return object && object instanceof Object && "links" in object && "extensions" in object;
    }

    export function isErrorRepresentation(object: any): object is RoInterfaces.IErrorRepresentation {
        return isResourceRepresentation(object) && "message" in object;
    }

    export function isIDomainObjectRepresentation(object: any): object is RoInterfaces.IDomainObjectRepresentation {
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

    function wrapLinks(links: ILink[]) {
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
        method: httpMethodsType;
        populate(wrapped: RoInterfaces.IRepresentation) : void;
        getBody(): RoInterfaces.IRepresentation;
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
                let description = errorUnknown;

                switch (this.clientErrorCode) {
                    case ClientErrorCode.ExpiredTransient:
                        description = errorExpiredTransient;
                        break;
                    case ClientErrorCode.WrongType:
                        description = errorWrongType;
                        break;
                    case ClientErrorCode.NotImplemented:
                        description = errorNotImplemented;
                        break;
                    case ClientErrorCode.SoftwareError:
                        description = errorSoftware;
                        break;
                    case ClientErrorCode.ConnectionProblem:
                        description = errorConnection;
                        break;
                }

                this.description = description;
                this.title = errorClient;
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

        title : string;
        description : string;
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
        return _.reduce(id, (a, v) => `${a}${a ? keySeparator : ""}${v}`, "");
    }

    export class ObjectIdWrapper {

        domainType: string;
        instanceId: string;
        splitInstanceId: string[];
        isService : boolean;

        getKey() {
            return this.domainType + keySeparator + this.instanceId;
        }

        static safeSplit(id: string) {
            if (id) {
                return id.split(keySeparator);
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

        static fromObjectId(objectId : string) {
            const oid = new ObjectIdWrapper();
            const [dt, ...id] =objectId.split(keySeparator);
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
        method: httpMethodsType = "GET";
        urlParms: _.Dictionary<Object>;

        constructor(protected model?: RoInterfaces.IRepresentation) {
        }

        populate(model: RoInterfaces.IRepresentation) {
            this.model = model;
        }

        getBody(): RoInterfaces.IRepresentation {
            if (this.method === "POST" || this.method === "PUT") {
                const m = _.clone(this.model);
                const up = _.clone(this.urlParms);
                return _.merge(m, up);
            }

            return {};
        }

        getUrl() {
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

                    return  url + "?" + urlParmString;
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

        constructor(public map: RoInterfaces.IValueMap, public id: string) {
            super(map);
        }

        populate(wrapped: RoInterfaces.IValueMap) {
            super.populate(wrapped);
        }
    }

    export abstract class NestedRepresentation<T extends RoInterfaces.IResourceRepresentation> {

        protected resource = () => this.model as T;

        constructor(private model: T) {}

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
                if (parms[i].trim().substring(0, 16) === roDomainType) {
                    this.xRoDomainType = (parms[i]).trim();
                    this.domainType = (this.xRoDomainType.split("=")[1].replace(/\"/g, "")).trim();
                }
            }
        }
    }

    export class Value {

        // note this is different from constructor parm as we wrap ILink
        private wrapped: Link | Array<Link | valueType> | scalarValueType | Blob;

        constructor(raw: Link | Array<Link | valueType> | valueType | Blob) {
            // can only be Link, number, boolean, string or null    

            if (raw instanceof Array) {
                this.wrapped = raw as Array<Link | valueType>;
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

        scalar(): scalarValueType {
            return this.isScalar() ? this.wrapped as scalarValueType : null;
        }

        list(): Value[] {
            return this.isList() ? _.map(this.wrapped as Array<Link | valueType>, i => new Value(i)) : null;
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

        setValue(target: IValue) {
            if (this.isFileReference()) {
                target.value = this.link().wrapped;
            }
            else if (this.isReference()) {
                target.value = { "href": this.link().href() };
            } else if (this.isList()) {
                target.value = _.map(this.list(), (v) => v.isReference() ? { "href": v.link().href() } : v.scalar());
            }
            else if (this.isBlob()){
                target.value = this.blob();
            }
            else {
                target.value = this.scalar();
            }
        }

        set(target: _.Dictionary<IValue | string>, name: string) {
            const t = target[name] = <IValue>{ value: null };
            this.setValue(t);
        }
    }

    export class ErrorValue {
        constructor(public value: Value, public invalidReason: string) {}
    }

    export class Result {
        constructor(public wrapped: RoInterfaces.IDomainObjectRepresentation | RoInterfaces.Custom.ICustomListRepresentation | RoInterfaces.IScalarValueRepresentation, private resultType: resultTypeType) {}

        object(): DomainObjectRepresentation {
            if (!this.isNull() && this.resultType === "object") {
                const dor = new DomainObjectRepresentation();
                dor.populate(this.wrapped as RoInterfaces.IDomainObjectRepresentation);
                return dor;
            }
            return null;
        }

        list(): ListRepresentation {
            if (!this.isNull() && this.resultType === "list") {
                const lr = new ListRepresentation();
                lr.populate(this.wrapped as RoInterfaces.Custom.ICustomListRepresentation);
                return lr;
            }
            return null;
        }

        scalar(): ScalarValueRepresentation {
            if (!this.isNull() && this.resultType === "scalar") {
                return new ScalarValueRepresentation(this.wrapped as RoInterfaces.IScalarValueRepresentation);
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

        constructor(private map: RoInterfaces.IValueMap | RoInterfaces.IObjectOfType, public statusCode: number, public warningMessage: string) {

        }

        valuesMap(): _.Dictionary<ErrorValue> {

            const values = _.pickBy(this.wrapped(), i => isIValue(i)) as _.Dictionary<IValue>;
            return _.mapValues(values, v => new ErrorValue(new Value(v.value), v.invalidReason));
        }

        invalidReason(): string {

            const temp = this.map;
            if (isIObjectOfType(temp)) {
                return (<any>temp)[roInvalidReason] as string;
            }

            return this.wrapped()[roInvalidReason] as string;
        }

        containsError() {
            return !!this.invalidReason() || !!this.warningMessage || _.some(this.valuesMap(), ev => !!ev.invalidReason);
        }

    }

    export class UpdateMap extends ArgumentMap implements IHateoasModel {
        constructor(private domainObject: DomainObjectRepresentation, map: RoInterfaces.IValueMap) {
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
            (<any>this.map)[roValidateOnly] = true;
        }
    }

    export class AddToRemoveFromMap extends ArgumentMap implements IHateoasModel {
        constructor(private collectionResource: CollectionRepresentation, map: RoInterfaces.IValueMap, add: boolean) {
            super(map, collectionResource.collectionId());
            const link = add ? collectionResource.addToLink() : collectionResource.removeFromLink();
            link.copyToHateoasModel(this);
        }

    }

    export class ModifyMap extends ArgumentMap implements IHateoasModel {
        constructor(private propertyResource: PropertyRepresentation | PropertyMember, map: RoInterfaces.IValueMap) {
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

    export abstract class ResourceRepresentation<T extends RoInterfaces.IResourceRepresentation> extends HateosModel {

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

        constructor(private wrapped: ICustomExtensions) {}

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
        choices = () => this.wrapped["x-ro-nof-choices"] as { [index: string]: valueType[]; };
        menuPath = () => this.wrapped["x-ro-nof-menuPath"] as string;
        mask = () => this.wrapped["x-ro-nof-mask"] as string;
        tableViewTitle = () => this.wrapped["x-ro-nof-tableViewTitle"] as boolean;
        tableViewColumns = () => this.wrapped["x-ro-nof-tableViewColumns"] as string[];
        multipleLines = () => this.wrapped["x-ro-nof-multipleLines"] as number;
        warnings = () => this.wrapped["x-ro-nof-warnings"] as string[];
        messages = () => this.wrapped["x-ro-nof-messages"] as string[];
        interactionMode = () => this.wrapped["x-ro-nof-interactionMode"] as string;
        dataType = () => this.wrapped["x-ro-nof-dataType"] as string;
        range = () => this.wrapped["x-ro-nof-range"] as IRange;
        notNavigable = () => this.wrapped["x-ro-nof-notNavigable"] as boolean;
        renderEagerly = () => this.wrapped["x-ro-nof-renderEagerly"] as boolean;
        presentationHint = () => this.wrapped["x-ro-nof-presentationHint"] as string;
    }

    // matches a action invoke resource 19.0 representation 

    export class InvokeMap extends ArgumentMap implements IHateoasModel {
        constructor(private link: Link) {
            super(link.arguments() as RoInterfaces.IValueMap, "");
            link.copyToHateoasModel(this);
        }

        setParameter(name: string, value: Value) {
            value.set(this.map, name);
        }

       
    }

    export class ActionResultRepresentation extends ResourceRepresentation<RoInterfaces.IActionInvokeRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IActionInvokeRepresentation;

        constructor() {
            super();
        }

        // links 
        selfLink(): Link {
            return linkByRel(this.links(), "self");
        }

        // link representations 
        getSelf(): ActionResultRepresentation {
            return <ActionResultRepresentation> this.selfLink().getTarget();
        }

        // properties 
        resultType(): resultTypeType {
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
    extends NestedRepresentation<RoInterfaces.IParameterRepresentation>
    implements IField {

        wrapped = () => this.resource() as RoInterfaces.IParameterRepresentation;

        // fix parent type
        constructor(wrapped: RoInterfaces.IParameterRepresentation, public parent: ActionMember | ActionRepresentation, private paramId: string) {
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
                return _.fromPairs(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
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

                if (!!(<any>this.promptLink().arguments())[roSearchTerm]) {
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
    export interface IInvokableAction extends IHasExtensions{
        parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation;
        actionId(): string;
        invokeLink(): Link;
        getInvokeMap(): InvokeMap;
        parameters(): _.Dictionary<Parameter>;
        disabledReason(): string;
    }

    export class ActionRepresentation extends ResourceRepresentation<RoInterfaces.IActionRepresentation> implements IInvokableAction {

        wrapped = () => this.resource() as RoInterfaces.IActionRepresentation;

        parent : DomainObjectRepresentation | MenuRepresentation | ListRepresentation;

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
            return <ActionRepresentation> this.selfLink().getTarget();
        }

        getUp(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.upLink().getTarget();
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
            super(link.arguments() as RoInterfaces.IValueMap, promptId);
            link.copyToHateoasModel(this);
        }

        private promptMap() {
            return this.map as RoInterfaces.IPromptMap;
        }

        setSearchTerm(term: string) {
            this.setArgument(roSearchTerm, new Value(term));
        }

        setArgument(name: string, val: Value) {
            val.set(this.map, name);
        }

        setArguments(args: _.Dictionary<Value>) {
            _.each(args, (arg, key) => this.setArgument(key, arg));
        }

        setMember(name: string, value: Value) {
            value.set(this.promptMap()["x-ro-nof-members"] as RoInterfaces.IValueMap, name);
        }

        setMembers(objectValues: () => _.Dictionary<Value>) {
            if (this.map["x-ro-nof-members"]) {
                _.forEach(objectValues(), (v, k) => this.setMember(k, v));
            }
        }
    }

    export class PromptRepresentation extends ResourceRepresentation<RoInterfaces.IPromptRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IPromptRepresentation;

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
            return <PromptRepresentation> this.selfLink().getTarget();
        }

        getUp(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.upLink().getTarget();
        }

        // properties 

        instanceId(): string {
            return this.wrapped().id;
        }

        choices(): _.Dictionary<Value> {
            const ch = this.wrapped().choices;
            if (ch) {
                const values = _.map(ch, item => new Value(item));
                return _.fromPairs(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
            }
            return null;
        }
    }

    // matches a collection representation 17.0 
    export class CollectionRepresentation extends ResourceRepresentation<RoInterfaces.ICollectionRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.ICollectionRepresentation;

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
            return <CollectionRepresentation> this.selfLink().getTarget();
        }

        getUp(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.upLink().getTarget();
        }

        setFromMap(map: AddToRemoveFromMap) {
            //this.set(map.attributes);
            _.assign(this.resource(), map.map);
        }

        private addToMap() {
            return this.addToLink().arguments() as RoInterfaces.IValueMap;
        }

        getAddToMap(): AddToRemoveFromMap {
            if (this.addToLink()) {
                return new AddToRemoveFromMap(this, this.addToMap(), true);
            }
            return null;
        }

        private removeFromMap() {
            return this.removeFromLink().arguments() as RoInterfaces.IValueMap;
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
    export class PropertyRepresentation extends ResourceRepresentation<RoInterfaces.IPropertyRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IPropertyRepresentation;


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
            return this.modifyLink().arguments() as RoInterfaces.IValueMap;
        }

        // linked representations 
        getSelf(): PropertyRepresentation {
            return <PropertyRepresentation> this.selfLink().getTarget();
        }

        getUp(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.upLink().getTarget();
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
                return _.fromPairs(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
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
    export class Member<T extends RoInterfaces.IMember> extends NestedRepresentation<RoInterfaces.IMember> {

        wrapped = () => this.resource() as RoInterfaces.IMember;

        constructor(wrapped: T) {
            super(wrapped);
        }

        update(newValue: RoInterfaces.IMember) {
            super.update(newValue);
        }

        memberType(): memberTypeType {
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

        static wrapMember(toWrap: RoInterfaces.IPropertyMember | RoInterfaces.ICollectionMember | RoInterfaces.IActionMember, parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation | Link, id: string): Member<RoInterfaces.IMember> {

            if (toWrap.memberType === "property") {
                return new PropertyMember(toWrap as RoInterfaces.IPropertyMember, parent as DomainObjectRepresentation | Link, id);
            }

            if (toWrap.memberType === "collection") {
                return new CollectionMember(toWrap as RoInterfaces.ICollectionMember, parent as DomainObjectRepresentation, id);
            }

            if (toWrap.memberType === "action") { 
                const member = new ActionMember(toWrap as RoInterfaces.IActionMember, parent as DomainObjectRepresentation | MenuRepresentation | ListRepresentation, id);

                if (member.invokeLink()) {
                    return new InvokableActionMember(toWrap as RoInterfaces.IActionMember, parent as DomainObjectRepresentation | MenuRepresentation | ListRepresentation, id);
                }

                return member;
            }

            return null;
        }
    }

    // matches 14.4.1
    export class PropertyMember extends Member<RoInterfaces.IPropertyMember> implements IField {

        wrapped = () => this.resource() as RoInterfaces.IPropertyMember;

        constructor(wrapped: RoInterfaces.IPropertyMember, public parent: DomainObjectRepresentation | Link, private propId: string) {
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
            return this.modifyLink().arguments() as RoInterfaces.IValueMap;
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
            return <PropertyRepresentation> this.detailsLink().getTarget();
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
                return _.fromPairs(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
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

                if (!!(<any>this.promptLink().arguments())[roSearchTerm]) {
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
    extends Member<RoInterfaces.ICollectionMember>
    implements IHasLinksAsValue {

        wrapped = () => this.resource() as RoInterfaces.ICollectionMember;

        constructor(wrapped: RoInterfaces.ICollectionMember, public parent: DomainObjectRepresentation, private id: string) {
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
            return <CollectionRepresentation> this.detailsLink().getTarget();
        }
    }

    // matches 14.4.3 
    export class ActionMember extends Member<RoInterfaces.IActionMember> {

        wrapped = () => this.resource() as RoInterfaces.IActionMember;

        constructor(wrapped: RoInterfaces.IActionMember, public parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation, private id: string) {
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

        
        constructor(wrapped: RoInterfaces.IActionMember,  parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation,  id: string) {
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


    export class DomainObjectRepresentation extends ResourceRepresentation<RoInterfaces.IDomainObjectRepresentation> implements IHasActions {

        wrapped = () => this.resource() as RoInterfaces.IDomainObjectRepresentation;

        constructor() {
            super();
        }

        id(): string {
            return `${this.domainType() || this.serviceId()}${this.instanceId() ? `${keySeparator}${this.instanceId()}` : ""}`;
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

        private memberMap: _.Dictionary<Member<RoInterfaces.IMember>>;
        private propertyMemberMap: _.Dictionary<PropertyMember>;
        private collectionMemberMap: _.Dictionary<CollectionMember>;
        private actionMemberMap: _.Dictionary<ActionMember>;

        private resetMemberMaps() {
            const members = this.wrapped().members;
            this.memberMap = _.mapValues(members, (m, id) => Member.wrapMember(m, this, id));
            this.propertyMemberMap = _.pickBy(this.memberMap, (m : Member<IMember>) => m.memberType() === "property") as _.Dictionary<PropertyMember>;
            this.collectionMemberMap = _.pickBy(this.memberMap, (m: Member<IMember>) => m.memberType() === "collection") as _.Dictionary<CollectionMember>;
            this.actionMemberMap = _.pickBy(this.memberMap, (m: Member<IMember>) => m.memberType() === "action") as _.Dictionary<ActionMember>;
        }

        private initMemberMaps() {
            if (!this.memberMap) {
                this.resetMemberMaps();
            }
        }

        members(): _.Dictionary<Member<RoInterfaces.IMember>> {
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

        member(id: string): Member<RoInterfaces.IMember> {
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
            return this.updateLink().arguments() as RoInterfaces.IValueMap;
        }

        private persistMap() {
            return this.persistLink().arguments() as RoInterfaces.IObjectOfType;
        }

        // linked representations 
        getSelf(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.selfLink().getTarget();
        }

        getPersistMap(): PersistMap {
            return new PersistMap(this, this.persistMap());
        }

        getUpdateMap(): UpdateMap {
            return new UpdateMap(this, this.updateMap());
        }

        setInlinePropertyDetails(flag: boolean) {
            this.setUrlParameter(roInlinePropertyDetails, flag);
        }

        private oid: ObjectIdWrapper;
        getOid(): ObjectIdWrapper {
            if (!this.oid) {
                this.oid = ObjectIdWrapper.fromObject(this);
            }

            return this.oid;
        } 
    }

    export class MenuRepresentation extends ResourceRepresentation<RoInterfaces.Custom.IMenuRepresentation> implements IHasActions {

        wrapped = () => this.resource() as IMenuRepresentation;

        constructor() {
            super();
        }

        title(): string {
            return this.wrapped().title;
        }

        menuId(): string {
            return this.wrapped().menuId;
        }

        private memberMap: _.Dictionary<Member<RoInterfaces.IMember>>;

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

        members(): _.Dictionary<Member<RoInterfaces.IMember>> {
            this.initMemberMaps();
            return this.memberMap;
        }

        actionMembers(): _.Dictionary<ActionMember> {
            this.initMemberMaps();
            return this.actionMemberMap;
        }

        member(id: string): Member<RoInterfaces.IMember> {
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
            return <MenuRepresentation> this.selfLink().getTarget();
        }

    }

    // matches scalar representation 12.0 
    export class ScalarValueRepresentation extends NestedRepresentation<RoInterfaces.IScalarValueRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IScalarValueRepresentation;

        constructor(wrapped: RoInterfaces.IScalarValueRepresentation) {
            super(wrapped);
        }

        value(): Value {
            return new Value(this.wrapped().value);
        }
    }

    // matches List Representation 11.0
    export class ListRepresentation
    extends ResourceRepresentation<RoInterfaces.Custom.ICustomListRepresentation>
    implements IHasLinksAsValue {

        wrapped = () => this.resource() as RoInterfaces.Custom.ICustomListRepresentation;

        // links
        selfLink(): Link {
            return linkByRel(this.links(), "self");
        }

        // linked representations 
        getSelf(): ListRepresentation {
            return <ListRepresentation> this.selfLink().getTarget();
        }

        private lazyValue: Link[];

        value(): Link[] {
            this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
            return this.lazyValue;
        }

        pagination(): RoInterfaces.Custom.IPagination {
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
    export class ErrorRepresentation extends ResourceRepresentation<RoInterfaces.IErrorRepresentation> implements IErrorDetails {

        wrapped = () => this.resource() as RoInterfaces.IErrorRepresentation;

        static create(message: string, stackTrace?: string[], causedBy?: RoInterfaces.IErrorDetailsRepresentation) {
            const rawError = {
                links : [] as any[],
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

        constructor(private domainObject: DomainObjectRepresentation, private map: RoInterfaces.IObjectOfType) {
            super(map);
            domainObject.persistLink().copyToHateoasModel(this);
        }

        setMember(name: string, value: Value) {
            value.set(this.map.members, name);
        }

        setValidateOnly() {
            (<any>this.map)[roValidateOnly] = true;
        }
    }

    // matches the version representation 8.0 
    export class VersionRepresentation extends ResourceRepresentation<RoInterfaces.IVersionRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IVersionRepresentation;

        // links 
        selfLink(): Link {
            return linkByRel(this.links(), "self");
        }

        upLink(): Link {
            return linkByRel(this.links(), "up");
        }

        // linked representations 
        getSelf(): VersionRepresentation {
            return <VersionRepresentation> this.selfLink().getTarget();
        }

        getUp(): HomePageRepresentation {
            return <HomePageRepresentation> this.upLink().getTarget();
        }

        // scalar properties 
        specVersion(): string {
            return this.wrapped().specVersion;
        }

        implVersion(): string {
            return this.wrapped().implVersion;
        }

        optionalCapabilities(): RoInterfaces.IOptionalCapabilities {
            return this.wrapped().optionalCapabilities;
        }
    }

    // matches Domain Services Representation 7.0
    export class DomainServicesRepresentation extends ListRepresentation {

        wrapped = () => this.resource() as ICustomListRepresentation;

        // links
        upLink(): Link {
            return linkByRel(this.links(), "up");
        }

        // linked representations 
        getSelf(): DomainServicesRepresentation {
            return <DomainServicesRepresentation> this.selfLink().getTarget();
        }

        getUp(): HomePageRepresentation {
            return <HomePageRepresentation> this.upLink().getTarget();
        }

        getService(serviceType: string): DomainObjectRepresentation {
            const serviceLink = _.find(this.value(), link => link.rel().parms[0].value === serviceType);
            return <DomainObjectRepresentation> serviceLink.getTarget();
        }
    }

    // custom
    export class MenusRepresentation extends ListRepresentation {

        wrapped = () => this.resource() as ICustomListRepresentation;

        // links
        upLink(): Link {
            return linkByRel(this.links(), "up");
        }

        // linked representations 
        getSelf(): MenusRepresentation {
            return <MenusRepresentation> this.selfLink().getTarget();
        }

        getUp(): HomePageRepresentation {
            return <HomePageRepresentation> this.upLink().getTarget();
        }

        getMenu(menuId: string): MenuRepresentation {
            const menuLink = _.find(this.value(), link => link.rel().parms[0].value === menuId);
            return <MenuRepresentation> menuLink.getTarget();
        }
    }

    // matches the user representation 6.0
    export class UserRepresentation extends ResourceRepresentation<RoInterfaces.IUserRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IUserRepresentation;

        // links 
        selfLink(): Link {
            return linkByRel(this.links(), "self");
        }

        upLink(): Link {
            return linkByRel(this.links(), "up");
        }

        // linked representations 
        getSelf(): UserRepresentation {
            return <UserRepresentation> this.selfLink().getTarget();
        }

        getUp(): HomePageRepresentation {
            return <HomePageRepresentation> this.upLink().getTarget();
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

    export class DomainTypeActionInvokeRepresentation extends ResourceRepresentation<RoInterfaces.IDomainTypeActionInvokeRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IDomainTypeActionInvokeRepresentation;

        constructor(againstType: string, toCheckType: string) {
            super();
            this.hateoasUrl = `${getAppPath()}/domain-types/${toCheckType}/type-actions/isSubtypeOf/invoke`;
            this.urlParms = {};
            this.urlParms["supertype"] = againstType;
        }

        selfLink(): Link {
            return linkByRel(this.links(), "self");
        }

        // linked representations 
        getSelf(): DomainTypeActionInvokeRepresentation {
            return <DomainTypeActionInvokeRepresentation> this.selfLink().getTarget();
        }

        id(): string {
            return this.wrapped().id;
        }

        value(): boolean {
            return this.wrapped().value;
        }
    }

    // matches the home page representation  5.0 
    export class HomePageRepresentation extends ResourceRepresentation<RoInterfaces.IHomePageRepresentation> {

        constructor() {
            super();
            this.hateoasUrl = getAppPath();
        }

        wrapped = () => this.resource() as RoInterfaces.IHomePageRepresentation;

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
            return <HomePageRepresentation> this.selfLink().getTarget();
        }

        getUser(): UserRepresentation {
            return <UserRepresentation> this.userLink().getTarget();
        }

        getDomainServices(): DomainServicesRepresentation {
            // cannot use getTarget here as that will just return a ListRepresentation 
            const domainServices = new DomainServicesRepresentation();
            this.serviceLink().copyToHateoasModel(domainServices);
            return domainServices;
        }

        getVersion(): VersionRepresentation {
            return <VersionRepresentation> this.versionLink().getTarget();
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

        constructor(public wrapped: RoInterfaces.ILink) {}

        compress() {
            this.wrapped.href = compress(this.wrapped.href);
        }

        decompress() {
            this.wrapped.href = decompress(this.wrapped.href);
        }

        href(): string {
            return decodeURIComponent(this.wrapped.href);
        }

        method(): httpMethodsType {
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

        arguments(): IValue | RoInterfaces.IValueMap | RoInterfaces.IObjectOfType | RoInterfaces.IPromptMap {
            return this.wrapped.arguments;
        }

        members(): _.Dictionary<PropertyMember> {
            const members = (this.wrapped as ICustomLink).members ;
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

        private getHateoasTarget(targetType : any): IHateoasModel {
            const MatchingType = this.repTypeToModel[targetType];
            const target: IHateoasModel = new MatchingType();
            return target;
        }

        private repTypeToModel : any = {
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

    export enum EntryType {FreeForm, Choices, MultipleChoices, ConditionalChoices, MultipleConditionalChoices, AutoComplete, File}
}