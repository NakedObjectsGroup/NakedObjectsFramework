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
/// <reference path="nakedobjects.models.shims.ts" />
/// <reference path="nakedobjects.config.ts" />

module NakedObjects {
    import IExtensions = NakedObjects.RoInterfaces.IExtensions;
    import ILink = NakedObjects.RoInterfaces.ILink;

    function isScalarType(typeName: string) {
        return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
    }

    function isListType(typeName: string) {
        return typeName === "list";
    }

    // interfaces 

    export interface IHateoasModel {
        hateoasUrl: string;
        method: string;
        url: any;
        urlParms: _.Dictionary<string>;
        populate(wrapped : RoInterfaces.IResourceRepresentation);
    }

    export interface IOptionalCapabilities {
        blobsClobs: string;
        deleteObjects: string;
        domainModel: string;
        protoPersistentObjects: string;
        validateOnly: string;
    }

    export interface IPagination {
        page: number;
        pageSize: number;
        numPages: number;
        totalCount: number;
    }



    export class ArgumentMap {

        attributes: any;

        constructor(map: Object, parent: any, public id: string) {
            this.attributes = map;
        }

        populate(wrapped: any) {
            this.attributes = wrapped;
        }

        get(attributeName: string): any {
            return this.attributes[attributeName];
        }
        set(attributeName?: any, value?: any, options?: any) {
            this.attributes[attributeName] = value;
        }

        hateoasUrl: string = "";
        method: string = "GET";
        suffix: string = "";
        url(): string {
            return (this.hateoasUrl || "") + this.suffix;
        }
        preFetch() { }

        urlParms: _.Dictionary<string>;
        onChange() { }
        onError(map: Object, statusCode: string, warnings: string) { }
    }

    export class RelParm {

        name: string;
        value : string;

        constructor(public asString: string) {
            this.decomposeParm();
        }

        private decomposeParm() {
            const regex = /(\w+)\W+(\w+)\W+/;
            const result = regex.exec(this.asString);
            [, this.name, this.value] = result;
        }
    }


    // rel helper class 
    export class Rel {

        ns: string = "";
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

    // Media type helper class 
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
                if (parms[i].trim().substring(0, 16) === "x-ro-domain-type") {
                    this.xRoDomainType = (parms[i]).trim();
                    this.domainType = (this.xRoDomainType.split("=")[1].replace(/\"/g, "")).trim();
                }
            }
        }
    }

    // helper class for values 
    export class Value {

        private wrapped: any;

        constructor(raw: any) {
            // can only be Link, number, boolean, string or null    

            if (raw instanceof Array) {
                this.wrapped = raw;
            } else if (raw instanceof Link) {
                this.wrapped = raw;
            } else if (raw && raw.href) {
                this.wrapped = new Link(raw);
            } else {
                this.wrapped = raw;
            }
        }

        isReference(): boolean {
            return this.wrapped instanceof Link;
        }

        isList(): boolean {
            return this.wrapped instanceof Array;
        }

        isNull(): boolean {
            return this.wrapped == null;
        }

        link(): Link {
            return this.isReference() ? <Link>this.wrapped : null;
        }

        scalar(): Object {
            return this.isReference() ? null : this.wrapped;
        }

        list(): Value[] {
            return this.isList() ? _.map(this.wrapped, i => new Value(i)) : null;
        }

        toString(): string {
            if (this.isReference()) {
                return this.link().title();
            }

            if (this.isList()) {
                const ss = _.map(this.list(), v =>  v.toString());
                return ss.length === 0 ? "" : _.reduce(ss, (m : string, s: string) => m + "-" + s);
            }

            return (this.wrapped == null) ? "" : this.wrapped.toString();
        }

        // todo rethink this - maybe encode value into json and decode directly
        static fromValueString(valueString: string): Value {
            if (valueString.indexOf("http") === 0) {
                return new Value({ href: valueString });
            }
            return new Value(valueString);
        }

        static fromJsonString(jsonString: string): Value {
            return new Value(JSON.parse(jsonString));
        }

        toValueString(): string {
            if (this.isReference()) {
                return this.link().href();
            }
            return (this.wrapped == null) ? "" : this.wrapped.toString();
        }

        toJsonString(): string {
            const raw = (this.wrapped instanceof  Link) ?  (<Link>this.wrapped).wrapped : this.wrapped; 
            return JSON.stringify(raw);
        }

        set(target: Object, name?: string) {
            if (name) {
                const t = target[name] = {};
                this.set(t);
            } else {
                if (this.isReference()) {
                    target["value"] = { "href": this.link().href() };
                } else {
                    target["value"] = this.scalar();
                }
            }
        }
    }

    export interface IValueMap {
        [index: string]: Value;
    }

    export interface IErrorValue {
        value: Value;
        invalidReason: string;
    }

    export interface IErrorValueMap {
        [index: string]: IErrorValue;
    }

    // helper class for results 
    export class Result {
        constructor(public wrapped : any, private resultType: string) { }

        object(): DomainObjectRepresentation {
            if (!this.isNull() && this.resultType === "object") {
                return new DomainObjectRepresentation(this.wrapped);
            }
            return null;
        }

        list(): ListRepresentation {
            if (!this.isNull() && this.resultType === "list") {
                return new ListRepresentation(this.wrapped);
            }
            return null;
        }

        scalar(): ScalarValueRepresentation {
            if (!this.isNull() && this.resultType === "scalar") {
                return new ScalarValueRepresentation(this.wrapped);
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

    // base class for nested representations 
    export class NestedRepresentation {
        constructor(public wrapped : any) { }

        links(): Links {
            return Links.wrapLinks(this.wrapped.links);
        }

        extensions(): IExtensions {
            return this.wrapped.extensions;
        }
    }

    // base class for all representations that can be directly loaded from the server 
    export class HateoasModelBase implements IHateoasModel {

        attributes: any;

        constructor(object?: any) {
            this.attributes = object;
        }

        get(attributeName: string): any {
            return this.attributes[attributeName];
        }
        set(attributeName?: any, value?: any, options?: any) {
            this.attributes[attributeName] = value;
        }


        hateoasUrl: string = "";
        method: string = "GET";
        suffix: string = "";
        url(): string {
            return (this.hateoasUrl || "") + this.suffix;
        }
        preFetch() { }

        populate(wrapped: RoInterfaces.IResourceRepresentation) {
            this.attributes = wrapped;
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }

        urlParms : _.Dictionary<string>;
    }

    export class ErrorMap extends HateoasModelBase {

        constructor(map: Object, public statusCode: string, public warningMessage: string) {
            super(map);
        }

        valuesMap(): IErrorValueMap {
            const vs: IErrorValueMap = {}; // distinguish between value map and persist map 
            const map = this.attributes.members ? this.attributes.members : this.attributes;
            for (let v in map) {

                if (map[v].hasOwnProperty("value")) {
                    const ev: IErrorValue = {
                        value: new Value(map[v].value),
                        invalidReason: map[v].invalidReason
                    };
                    vs[v] = ev;
                }
            }

            return vs;
        }

        invalidReason() {
            return this.get("x-ro-invalid-reason");
        }
    }


    export class UpdateMap extends ArgumentMap implements IHateoasModel {
        constructor(private domainObject: DomainObjectRepresentation, map: Object) {
            super(map, domainObject, domainObject.instanceId());

            domainObject.updateLink().copyToHateoasModel(this);

            _.each(this.properties(), (value : Value, key : string) => {
                this.setProperty(key, value);
            });
        }

        onChange() {
            // if the update map changes as a result of server changes (eg title changes) update the 
            // associated domain object
            this.domainObject.setFromUpdateMap(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }

        properties(): IValueMap {
            return <IValueMap>_.mapValues(this.attributes, (v : any) => new Value(v.value));
        }

        setProperty(name: string, value: Value) {
            value.set(this.attributes, name);
        }
    }

    export class AddToRemoveFromMap extends ArgumentMap implements IHateoasModel {
        constructor(private collectionResource: CollectionRepresentation, map: Object, add: boolean) {
            super(map, collectionResource, collectionResource.instanceId());
            const link = add ? collectionResource.addToLink() : collectionResource.removeFromLink();
            link.copyToHateoasModel(this);
        }

        onChange() {
            // if the update map changes as a result of server changes (eg title changes) update the 
            // associated property
            this.collectionResource.setFromMap(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }

        setValue(value: Value) {
            value.set(this.attributes);
        }
    }

    export class ModifyMap extends ArgumentMap implements IHateoasModel {
        constructor(private propertyResource: PropertyRepresentation, map: Object) {
            super(map, propertyResource, propertyResource.instanceId());

            propertyResource.modifyLink().copyToHateoasModel(this);

            this.setValue(propertyResource.value());
        }

        onChange() {
            // if the update map changes as a result of server changes (eg title changes) update the 
            // associated property
            this.propertyResource.setFromModifyMap(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }


        setValue(value: Value) {
            value.set(this.attributes);
        }
    }

    export class ModifyMapv11 extends ArgumentMap implements IHateoasModel {
        constructor(private propertyResource: PropertyMember, id: string, map: Object) {
            super(map, propertyResource, id);

            propertyResource.modifyLink().copyToHateoasModel(this);

            this.setValue(propertyResource.value());
        }

        onChange() {
            // if the update map changes as a result of server changes (eg title changes) update the 
            // associated property
            this.propertyResource.setFromModifyMap(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }

        setValue(value: Value) {
            value.set(this.attributes);
        }
    }

    //export class DomainTypeActionMap extends ArgumentMap implements IHateoasModel {
    //    constructor(private actionInvoke: DomainTypeActionInvokeRepresentation, id: string, map: Object) {
    //        super(map, actionInvoke, id);

    //        //actionInvoke.modifyLink().copyToHateoasModel(this);

    //        //this.setValue(propertyResource.value());
    //        actionInvoke.selfLink().copyToHateoasModel(this);
    //    }

    //    onError(map: Object, statusCode: string, warnings: string) {
    //        return new ErrorMap(map, statusCode, warnings);
    //    }

    //    setValue(value: Value) {
    //        value.set(this.attributes);
    //    }
    //}


    export class ClearMap extends ArgumentMap implements IHateoasModel {
        constructor(propertyResource: PropertyRepresentation) {
            super({}, propertyResource, propertyResource.instanceId());

            propertyResource.clearLink().copyToHateoasModel(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }
    }

    export class ClearMapv11 extends ArgumentMap implements IHateoasModel {
        constructor(propertyResource: PropertyMember, id: string) {
            super({}, propertyResource, id);

            propertyResource.clearLink().copyToHateoasModel(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }
    }

    // helper - collection of Links 
    export class Links implements IHateoasModel {

       


        url(): any { }
       

        add(models: any, options?: any) {
            this.models = this.models || [];

            for (var i = 0; i < models.length; i++) {
                var m = new this.model(models[i]);
                this.models.push(m);
            }
        }




        // cannot use constructor to initialise as model property is not yet set and so will 
        // not create members of correct type 
        constructor() {

            this.url = () => {
                return this.hateoasUrl;
            };
        }

        populate(wrapped: RoInterfaces.IResourceRepresentation) {
            //super.populate(wrapped);
        }

        hateoasUrl: string;
        method: string;

        model = Link;

        models: Link[];

        parse(response : any) {
            return response.value;
        }

        static wrapLinks(links: ILink[]): Links {
            const ll = new Links();       
            ll.add(links || []);          
            return ll;
        }

        // returns first link of rel
        private getLinkByRel = (rel: Rel) => _.find(this.models, i => i.rel().uniqueValue === rel.uniqueValue);

        linkByRel = (rel: string) => this.getLinkByRel(new Rel(rel));

        urlParms: _.Dictionary<string>;
    }


    // REPRESENTATIONS

    export abstract class ResourceRepresentation extends HateoasModelBase {
        constructor(object? : any) {
            super(object);
        }

        protected resource : RoInterfaces.IResourceRepresentation;

        populate(wrapped: RoInterfaces.IResourceRepresentation) {
            this.resource = wrapped; 
            super.populate(wrapped);
        }

        private lazyLinks: Links;

        links(): Links {
            this.lazyLinks = this.lazyLinks || Links.wrapLinks(this.resource.links);
            return this.lazyLinks;
        }

        extensions(): IExtensions {
            return this.resource.extensions;
        }
    }

    // matches a action invoke resource 19.0 representation 

    export class ActionResultRepresentation extends ResourceRepresentation {

        constructor(object? : any) {
            super(object);
        }

        // links 
        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        // link representations 
        getSelf(): ActionResultRepresentation {
            return <ActionResultRepresentation> this.selfLink().getTarget();
        }

        // properties 
        resultType(): string {
            return this.get("resultType");
        }

        result(): Result {
            return new Result(this.get("result"), this.resultType());
        }

        // helper
        setParameter(name: string, value: Value) {
            value.set(this.attributes, name);
        }

        setUrlParameter(name: string, value : string) {
            this.urlParms = this.urlParms || {};
            this.urlParms[name] = value;
        }
    }

    // matches an action representation 18.0 

    // matches 18.2.1
    export class Parameter extends NestedRepresentation {
        // fix parent type
        constructor(wrapped : any, public parent: any, private id : string) {
            super(wrapped);
        }

        parameterId() {
            return this.id;
        }

        // properties 
        choices(): IValueMap {

            // use custom choices extension by preference 
            if (this.extensions()["x-ro-nof-choices"]) {
                return  <IValueMap> _.mapValues(this.extensions()["x-ro-nof-choices"], (v : any) => new Value(v));
            }

            if (this.wrapped.choices) {
                const values = _.map(this.wrapped.choices, (item : any) => new Value(item));
                return _.object<IValueMap>(_.map(values, (v : any) => [v.toString(), v]));
            }
            return null;
        }

        promptLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/prompt");
        }

        getPrompts(): PromptRepresentation {
            return <PromptRepresentation> this.promptLink().getTarget();
        }

        default(): Value {
            return new Value(this.wrapped.default);
        }

        // helper
        isScalar(): boolean {
            return isScalarType(this.extensions().returnType) ||
                   (isListType(this.extensions().returnType) && isScalarType(this.extensions().elementType));
        }

        hasPrompt(): boolean {
            return !!this.promptLink();
        }

    }

    export interface IParameterMap {
        [index: string]: Parameter;
    }

    export class ActionRepresentation extends ResourceRepresentation {

        // links 
        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        upLink(): Link {
            return this.links().linkByRel("up");
        }

        invokeLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/invoke");
        }

        // linked representations 
        getSelf(): ActionRepresentation {
            return <ActionRepresentation> this.selfLink().getTarget();
        }

        getUp(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.upLink().getTarget();
        }

        getInvoke(): ActionResultRepresentation {
            return <ActionResultRepresentation> this.invokeLink().getTarget();
        }

        // properties 

        actionId(): string {
            return this.get("id");
        }

        private parameterMap: IParameterMap;

        private initParameterMap(): void {

            if (!this.parameterMap) {
                const parameters = this.get("parameters");
                this.parameterMap = _.mapValues(parameters, (p, id) => new Parameter(p, this, id));
            }
        }

        parameters(): IParameterMap {
            this.initParameterMap();
            return this.parameterMap;
        }

        disabledReason(): string {
            return this.get("disabledReason");
        }
    }

    export interface IListOrCollection {
        value(): Links;
    }

    // new in 1.1 15.0 in spec 

    export class PromptRepresentation extends ResourceRepresentation {

        // links 
        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        upLink(): Link {
            return this.links().linkByRel("up");
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
            return this.get("id");
        }

        choices(): IValueMap {
            const ch = this.get("choices");
            if (ch) {
                const values = _.map(ch, (item : any) => new Value(item));
                return _.object<IValueMap>(_.map(values, (v : Value) => [v.toString(), v]));
            }
            return null;
        }

        reset() {
            this.attributes = {};
        }

        setSearchTerm(term: string) {
            this.set("x-ro-searchTerm", { "value": term });
        }

        setArgument(name: string, val: Value) {
            val.set(this.attributes, name);
        }

        setArguments(args: IValueMap) {
            _.each(args, (arg, key) => this.setArgument(key, arg));
        }
    }

    // matches a collection representation 17.0 
    export class CollectionRepresentation extends ResourceRepresentation implements IListOrCollection {

        // links 
        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        upLink(): Link {
            return this.links().linkByRel("up");
        }

        addToLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/add-to");
        }

        removeFromLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/remove-from");
        }

        // linked representations 
        getSelf(): CollectionRepresentation {
            return <CollectionRepresentation> this.selfLink().getTarget();
        }

        getUp(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.upLink().getTarget();
        }

        setFromMap(map: AddToRemoveFromMap) {
            this.set(map.attributes);
        }

        private addToMap() {
            return this.addToLink().arguments();
        }

        getAddToMap(): AddToRemoveFromMap {
            if (this.addToLink()) {
                return new AddToRemoveFromMap(this, this.addToMap(), true);
            }
            return null;
        }

        private removeFromMap() {
            return this.removeFromLink().arguments();
        }

        getRemoveFromMap(): AddToRemoveFromMap {
            if (this.removeFromLink()) {
                return new AddToRemoveFromMap(this, this.removeFromMap(), false);
            }
            return null;
        }

        // properties 

        instanceId(): string {
            return this.get("id");
        }

        value(): Links {
            return Links.wrapLinks(this.get("value"));
        }

        disabledReason(): string {
            return this.get("disabledReason");
        }
    }

    // matches a property representation 16.0 
    export class PropertyRepresentation extends ResourceRepresentation {

        // links 
        modifyLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/modify");
        }

        clearLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/clear");
        }

        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        upLink(): Link {
            return this.links().linkByRel("up");
        }

        promptLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/prompt");
        }

        private modifyMap() {
            return this.modifyLink().arguments();
        }

        // linked representations 
        getSelf(): PropertyRepresentation {
            return <PropertyRepresentation> this.selfLink().getTarget();
        }

        getUp(): DomainObjectRepresentation {
            return <DomainObjectRepresentation> this.upLink().getTarget();
        }

        setFromModifyMap(map: ModifyMap) {
            this.set(map.attributes);
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

        getPrompts(): PromptRepresentation {
            return <PromptRepresentation> this.promptLink().getTarget();
        }

        // properties 

        instanceId(): string {
            return this.get("id");
        }

        value(): Value {
            return new Value(this.get("value"));
        }

        choices(): IValueMap {

            // use custom choices extension by preference 
            if (this.extensions()["x-ro-nof-choices"]) {
                return <IValueMap> _.mapValues(this.extensions()["x-ro-nof-choices"], (v) => new Value(v));
            }
            const ch = this.get("choices");
            if (ch) {
                const values = _.map(ch, (item) => new Value(item));
                return _.object<IValueMap>(_.map(values, (v) => [v.toString(), v]));
            }
            return null;
        }

        disabledReason(): string {
            return this.get("disabledReason");
        }

        // helper 
        isScalar(): boolean {
            return isScalarType(this.extensions().returnType);
        }

        hasPrompt(): boolean {
            return !!this.promptLink();
        }
    }


    // matches a domain object representation 14.0 

    // base class for 14.4.1/2/3
    export class Member extends NestedRepresentation {
        constructor(wrapped, public parent: DomainObjectRepresentation | MenuRepresentation) {
            super(wrapped);
        }

        update(newValue) {
            this.wrapped = newValue;
        }

        memberType(): string {
            return this.wrapped.memberType;
        }

        detailsLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/details");
        }

        disabledReason(): string {
            return this.wrapped.disabledReason;
        }

        isScalar(): boolean {
            return isScalarType(this.extensions().returnType);
        }

        static wrapMember(toWrap, parent, id): Member {

            if (toWrap.memberType === "property") {
                return new PropertyMember(toWrap, parent, id);
            }

            if (toWrap.memberType === "collection") {
                return new CollectionMember(toWrap, parent, id);
            }

            if (toWrap.memberType === "action") {
                return new ActionMember(toWrap, parent, id);
            }

            return null;
        }
    }

    // matches 14.4.1
    export class PropertyMember extends Member {
        constructor(wrapped, parent, private id : string) {
            super(wrapped, parent);
        }

        // inlined 

        propertyId(): string {
            return this.id;
        }

        modifyLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/modify");
        }

        clearLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/clear");
        }

        private modifyMap() {
            return this.modifyLink().arguments();
        }

        setFromModifyMap(map: ModifyMapv11) {
            _.forOwn(map.attributes, (v, k) => {
                this.wrapped[k] = v;
            });
        }

        getModifyMap(id: string): ModifyMapv11 {
            if (this.modifyLink()) {
                return new ModifyMapv11(this, id, this.modifyMap());
            }
            return null;
        }

        getClearMap(id: string): ClearMapv11 {
            if (this.clearLink()) {
                return new ClearMapv11(this, id);
            }
            return null;
        }

        getPrompts(): PromptRepresentation {
            return <PromptRepresentation> this.promptLink().getTarget();
        }

        //


        value(): Value {
            return new Value(this.wrapped.value);
        }

        update(newValue): void {
            super.update(newValue);
        }

        attachmentLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/attachment");
        }

        promptLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/prompt");
        }

        getDetails(): PropertyRepresentation {
            return <PropertyRepresentation> this.detailsLink().getTarget();
        }

        hasChoices(): boolean {
            return this.wrapped.hasChoices;
        }

        hasPrompt(): boolean {
            return !!this.promptLink();
        }

        choices(): IValueMap {

            // use custom choices extension by preference 
            if (this.extensions()["x-ro-nof-choices"]) {
                return <IValueMap> _.mapValues(this.extensions()["x-ro-nof-choices"], (v) => new Value(v));
            }
            const ch = this.wrapped.choices;
            if (ch) {
                const values = _.map(ch, (item) => new Value(item));
                return _.object<IValueMap>(_.map(values, (v) => [v.toString(), v]));
            }
            return null;
        }

    }

    // matches 14.4.2 
    export class CollectionMember extends Member {
        constructor(wrapped, parent, private id : string) {
            super(wrapped, parent);
        }

        collectionId(): string {
            return this.id;
        }

        value(): Links {
            return Links.wrapLinks(this.wrapped.value);
        }

        size(): number {
            return this.wrapped.size;
        }

        getDetails(): CollectionRepresentation {
            return <CollectionRepresentation> this.detailsLink().getTarget();
        }
    }

    // matches 14.4.3 
    export class ActionMember extends Member {
        constructor(wrapped, parent, private id : string) {
            super(wrapped, parent);
        }

        actionId(): string {
            return this.id;
        }

        getDetails(): ActionRepresentation {
            return <ActionRepresentation> this.detailsLink().getTarget();
        }

        // 1.1 inlined 


        invokeLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/invoke");
        }


        getInvoke(): ActionResultRepresentation {
            return <ActionResultRepresentation> this.invokeLink().getTarget();
        }

        // properties 

        private parameterMap: IParameterMap;

        private initParameterMap(): void {

            if (!this.parameterMap) {
                const parameters = this.wrapped.parameters;
                this.parameterMap = _.mapValues(parameters, (p, id) => new Parameter(p, this, id));
            }
        }

        parameters(): IParameterMap {
            this.initParameterMap();
            return this.parameterMap;
        }

        disabledReason(): string {
            return this.wrapped.disabledReason;
        }
    }

    export interface IMemberMap {
        [index: string]: Member;
    }

    export interface IPropertyMemberMap {
        [index: string]: PropertyMember;
    }

    export interface ICollectionMemberMap {
        [index: string]: CollectionMember;
    }

    export interface IActionMemberMap {
        [index: string]: ActionMember;
    }

    export class DomainObjectRepresentation extends ResourceRepresentation {

        constructor(object?) {
            super(object);
            this.url = this.getUrl;
        }

        getUrl(): string {
            return this.hateoasUrl || this.selfLink().href();
        }

        title(): string {
            return this.get("title");
        }

        domainType(): string {
            return this.get("domainType");
        }

        serviceId(): string {
            return this.get("serviceId");
        }

        links(): Links {
            return Links.wrapLinks(this.get("links"));
        }

        instanceId(): string {
            return this.get("instanceId");
        }

        extensions(): IExtensions {
            return this.get("extensions");
        }

        private memberMap: IMemberMap;
        private propertyMemberMap: IPropertyMemberMap;
        private collectionMemberMap: ICollectionMemberMap;
        private actionMemberMap: IActionMemberMap;

        private resetMemberMaps() {
            const members = this.get("members");
            this.memberMap = _.mapValues(members, (m, id) => Member.wrapMember(m, this, id));
            this.propertyMemberMap = <IPropertyMemberMap> _.pick(this.memberMap, (m: Member) => m.memberType() === "property");
            this.collectionMemberMap = <ICollectionMemberMap> _.pick(this.memberMap, (m: Member) => m.memberType() === "collection");
            this.actionMemberMap = <IActionMemberMap> _.pick(this.memberMap, (m: Member) => m.memberType() === "action");
        }

        private initMemberMaps() {
            if (!this.memberMap) {
                this.resetMemberMaps();
            }
        }

        members(): IMemberMap {
            this.initMemberMaps();
            return this.memberMap;
        }

        propertyMembers(): IPropertyMemberMap {
            this.initMemberMaps();
            return this.propertyMemberMap;
        }

        collectionMembers(): ICollectionMemberMap {
            this.initMemberMaps();
            return this.collectionMemberMap;
        }

        actionMembers(): IActionMemberMap {
            this.initMemberMaps();
            return this.actionMemberMap;
        }

        member(id: string): Member {
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
            return this.links().linkByRel("urn:org.restfulobjects:rels/update");
        }

        persistLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/persist");
        }

        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        private updateMap() {
            return this.updateLink().arguments();
        }

        private persistMap() {
            return this.persistLink().arguments();
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

        setFromUpdateMap(map: UpdateMap) {

            _.forOwn(this.members(), (m, k) => {
                m.update(map.attributes.members[k]);
            });

            // to trigger an update on the domainobject
            this.set(map.attributes);
        }

        setFromPersistMap(map: PersistMap) {
            // to trigger an update on the domainobject
            this.set(map.attributes);
            this.resetMemberMaps();
        }

        preFetch() {
            this.memberMap = null; // to ensure everything gets reset
        }
    }

    export class MenuRepresentation extends ResourceRepresentation {

        constructor(object?) {
            super(object);
            this.url = this.getUrl;
        }

        getUrl(): string {
            return this.hateoasUrl || this.selfLink().href();
        }

        title(): string {
            return this.get("title");
        }

        menuId(): string {
            return this.get("menuId");
        }
       
        links(): Links {
            return Links.wrapLinks(this.get("links"));
        }

        extensions(): IExtensions {
            return this.get("extensions");
        }

        private memberMap: IMemberMap;
      
        private actionMemberMap: IActionMemberMap;

        private resetMemberMaps() {
            const members = this.get("members");
            this.memberMap = _.mapValues(members, (m, id) => Member.wrapMember(m, this, id));
            this.actionMemberMap = <IActionMemberMap> _.pick(this.memberMap, (m: Member) => m.memberType() === "action");
        }

        private initMemberMaps() {
            if (!this.memberMap) {
                this.resetMemberMaps();
            }
        }

        members(): IMemberMap {
            this.initMemberMaps();
            return this.memberMap;
        }


        actionMembers(): IActionMemberMap {
            this.initMemberMaps();
            return this.actionMemberMap;
        }

        member(id: string): Member {
            return this.members()[id];
        }


        actionMember(id: string): ActionMember {
            return this.actionMembers()[id];
        }


        selfLink(): Link {
            return this.links().linkByRel("self");
        }
   
        // linked representations 
        getSelf(): MenuRepresentation {
            return <MenuRepresentation> this.selfLink().getTarget();
        }

        preFetch() {
            this.memberMap = null; // to ensure everything gets reset
        }
    }



    // matches scalar representation 12.0 
    export class ScalarValueRepresentation extends NestedRepresentation {
        constructor(wrapped) {
            super(wrapped);
        }

        value(): Value {
            return new Value(this.wrapped.value);
        }
    }

    // matches List Representation 11.0
    export class ListRepresentation extends ResourceRepresentation implements IListOrCollection {

        constructor(object?) {
            super(object);
        }

        // links
        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        // linked representations 
        getSelf(): ListRepresentation {
            return <ListRepresentation> this.selfLink().getTarget();
        }

        // list of links to services 
        value(): Links {
            return Links.wrapLinks(this.get("value"));
        }

        pagination(): IPagination {
            return this.get("pagination");
        }
    }

    // matches the error representation 10.0 
    export class ErrorRepresentation extends ResourceRepresentation {
        constructor(object?) {
            super(object);
        }

        // scalar properties 
        message(): string {
            return this.get("message");
        }

        stacktrace(): string[] {
            return this.get("stackTrace");
        }

        causedBy(): ErrorRepresentation {
            const cb = this.get("causedBy");
            return cb ? new ErrorRepresentation(cb) : null;
        }
    }

    // matches Objects of Type Resource 9.0 
    export class PersistMap extends ArgumentMap implements IHateoasModel {

        constructor(private domainObject: DomainObjectRepresentation, map: Object) {
            super(map, domainObject, domainObject.instanceId());
            domainObject.persistLink().copyToHateoasModel(this);
        }

        onChange() {
            this.domainObject.setFromPersistMap(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }

        setMember(name: string, value: Value) {
            value.set(this.attributes["members"], name);
        }
    }

    // matches the version representation 8.0 
    export class VersionRepresentation extends ResourceRepresentation {
        constructor() {
            super();
        }

        // links 
        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        upLink(): Link {
            return this.links().linkByRel("up");
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
            return this.get("specVersion");
        }

        implVersion(): string {
            return this.get("implVersion");
        }

        optionalCapabilities(): IOptionalCapabilities {
            return this.get("optionalCapabilities");
        }
    }

    // matches Domain Services Representation 7.0
    export class DomainServicesRepresentation extends ListRepresentation {

        // links
        upLink(): Link {
            return this.links().linkByRel("up");
        }

        // linked representations 
        getSelf(): DomainServicesRepresentation {
            return <DomainServicesRepresentation> this.selfLink().getTarget();
        }

        getUp(): HomePageRepresentation {
            return <HomePageRepresentation> this.upLink().getTarget();
        }

        getService(serviceType: string): DomainObjectRepresentation {
            const serviceLink = _.find(this.value().models, model => model.rel().parms[0].value === serviceType);
            return <DomainObjectRepresentation> serviceLink.getTarget();
        }
    }

    // custom
    export class MenusRepresentation extends ListRepresentation {

        // links
        upLink(): Link {
            return this.links().linkByRel("up");
        }

        // linked representations 
        getSelf(): MenusRepresentation {
            return <MenusRepresentation> this.selfLink().getTarget();
        }

        getUp(): HomePageRepresentation {
            return <HomePageRepresentation> this.upLink().getTarget();
        }

        getMenu(menuId : string): MenuRepresentation {
            const menuLink = _.find(this.value().models, model => model.rel().parms[0].value === menuId);
            return <MenuRepresentation> menuLink.getTarget();
        }
    }

    // matches the user representation 6.0
    export class UserRepresentation extends ResourceRepresentation {

        // links 
        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        upLink(): Link {
            return this.links().linkByRel("up");
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
            return this.get("userName");
        }
        friendlyName(): string {
            return this.get("friendlyName");
        }
        email(): string {
            return this.get("email");
        }
        roles(): string[] {
            return this.get("roles");
        }
    }

    export class DomainTypeActionInvokeRepresentation extends ResourceRepresentation {

        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        // linked representations 
        getSelf(): DomainTypeActionInvokeRepresentation {
            return <DomainTypeActionInvokeRepresentation> this.selfLink().getTarget();
        }

        id(): string {
            return this.get("id");
        }

        value(): boolean {
            return this.get("value");
        }
    }

    // matches the home page representation  5.0 
    export class HomePageRepresentation extends ResourceRepresentation {

        constructor() {
            super();
            this.hateoasUrl = appPath;
        }

        homePage : RoInterfaces.IHomePageRepresentation = this.resource;

        // links 
        serviceLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/services");
        }

        userLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/user");
        }

        selfLink(): Link {
            return this.links().linkByRel("self");
        }

        versionLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/version");
        }

        // custom 
        menusLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/menus");
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
    export class Link  {

        attributes: any;

        populate(wrapped: any) {
            this.attributes = wrapped;
        }

        url(): string {
            return "";
        }
        get(attributeName: string): any {
            return this.attributes[attributeName];
        }
        set(attributeName?: any, value?: any, options?: any) {
            this.attributes[attributeName] = value;
        }


        wrapped : any;

        constructor(object?) {
            this.attributes = object;
            this.wrapped = object;
        }

        href(): string {
            return this.get("href");
        }

        method(): string {
            return this.get("method");
        }

        rel(): Rel {
            return new Rel(this.get("rel"));
        }

        type(): MediaType {
            return new MediaType(this.get("type"));
        }

        title(): string {
            return this.get("title");
        }

        arguments(): Object {
            return this.get("arguments");
        }

        extensions(): IExtensions {
            return this.get("extensions");
        }

        copyToHateoasModel(hateoasModel: IHateoasModel): void {
            hateoasModel.hateoasUrl = this.href();
            hateoasModel.method = this.method();
        }

        private getHateoasTarget(targetType): IHateoasModel {
            const matchingType = this.repTypeToModel[targetType];
            const target: IHateoasModel = new matchingType({});
            return target;
        }

        private repTypeToModel = {
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
        }

        // get the object that this link points to 
        getTarget(): IHateoasModel {
            const target = this.getHateoasTarget(this.type().representationType);
            this.copyToHateoasModel(target);
            return target;
        }
    }
}