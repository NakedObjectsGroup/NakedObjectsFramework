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

module NakedObjects {
    import ICustomExtensions = RoInterfaces.Custom.ICustomExtensions;
    import ILink = RoInterfaces.ILink;
    import IMenuRepresentation = RoInterfaces.Custom.IMenuRepresentation;
    import IValue = RoInterfaces.IValue;
    import IResourceRepresentation = RoInterfaces.IResourceRepresentation;
    import ICustomListRepresentation = RoInterfaces.Custom.ICustomListRepresentation;


    // helper functions 

    function isScalarType(typeName: string) {
        return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
    }

    function isListType(typeName: string) {
        return typeName === "list";
    }

    function emptyResource() : IResourceRepresentation {
        return { links: [], extensions: {} };
    }

    function isILink(object: any): object is RoInterfaces.ILink {
        return object && object instanceof Object && "href" in object;
    }

    function isIObjectOfType(object: any): object is RoInterfaces.IObjectOfType {
        return object && object instanceof Object && "members" in object;
    }

    function isIPromptMap(object: any): object is RoInterfaces.IPromptMap {
        return object && object instanceof Object && "members" in object;
    }

    function isIValue(object: any): object is RoInterfaces.IValue {
        return object && object instanceof Object && "value" in object;
    }

    function getId(prop: PropertyRepresentation | PropertyMember) {
        if (prop instanceof PropertyRepresentation) {
            return prop.instanceId();
        } else {
            return (prop as PropertyMember).propertyId();
        }
    }

    function wrapLinks(links: ILink[]) {
        return _.map(links, l => new Link(l));
    }

    function wrapExtensions(links: ILink[]) {
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
        hateoasUrl: string;
        method: string;
        urlParms: _.Dictionary<string>;
        populate(wrapped: RoInterfaces.IRepresentation);
        getBody(): RoInterfaces.IRepresentation;
        getUrl() : string;
    }

    // abstract classes 

    export abstract class HateosModel implements IHateoasModel {

        hateoasUrl: string = "";
        method: string = "GET";
        urlParms: _.Dictionary<string>;

        constructor(protected model?: RoInterfaces.IRepresentation) {
        }

        populate(model: RoInterfaces.IRepresentation) {
            this.model = model;
        }

        getBody(): RoInterfaces.IRepresentation {
            if (this.method === "POST" || this.method === "PUT") {
                return _.clone(this.model);
            }

            return {};
        }

        getDtId() {
            if (this.hateoasUrl) {
                const segments = this.hateoasUrl.split("/");
                if (segments.length >= 2) {
                    segments.reverse();
                    const [idr, dt] = segments;
                    const id = idr.split("-");
                    return {
                        dt,
                        id
                    }
                }
            }
            return { dt: "", id: [] };
        }


        getUrl() {
            const url = this.hateoasUrl;
            const attrAsJson = _.clone(this.model);

            if (_.keys(attrAsJson).length > 0 && (this.method === "GET" || this.method === "DELETE")) {

                const urlParmsAsJson = _.clone(this.urlParms);
                const asJson = _.merge(attrAsJson, urlParmsAsJson);
                if (_.keys(asJson).length > 0) {
                    const map = JSON.stringify(asJson);
                    const parmString = encodeURI(map);
                    return url + "?" + parmString;
                }
                return url;
            }

            const urlParmString = _.reduce(this.urlParms || {}, (result, n, key) => (result === "" ? "" : result + "&") + key + "=" + n, "");

            return urlParmString !== "" ? url + "?" + urlParmString : url;
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
 
    export class Value {

        private wrapped: Link | Array<Link | ILink | number | string | boolean> | number | string | boolean;
    
        constructor(raw: Link | Array<Link | ILink | number | string | boolean> | RoInterfaces.ILink | number | string | boolean ) {
            // can only be Link, number, boolean, string or null    

            if (raw instanceof Array) {
                this.wrapped = raw as Array<Link | ILink | number | string | boolean>;
            } else if (raw instanceof Link) {
                this.wrapped = raw;
            } else if (isILink(raw)) {
                this.wrapped = new Link(raw);
            } else {
                this.wrapped = raw;
            }
        }

        isScalar(): boolean {
            return !this.isReference() && !this.isList();
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

        scalar(): number | string | boolean {
            return this.isScalar() ? this.wrapped as number | string | boolean : null;
        }

        list(): Value[] {
            return this.isList() ? _.map(this.wrapped as Array<Link | number | string | boolean>, i => new Value(i)) : null;
        }

        toString(): string {
            if (this.isReference()) {
                return this.link().title();
            }

            if (this.isList()) {
                const ss = _.map(this.list(), v =>  v.toString());
                return ss.length === 0 ? "" : _.reduce(ss, (m, s) => m + "-" + s, "");
            }

            return (this.wrapped == null) ? "" : this.wrapped.toString();
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

        setValue(target: IValue) {
            if (this.isReference()) {
                target.value = { "href": this.link().href() };
            }
            else if (this.isList()) {
                target.value = _.map(this.list(), (v) => v.isReference() ? { "href": v.link().href() } : v.scalar());
            } else {
                target.value = this.scalar();
            }
        }

        set(target: _.Dictionary<IValue | string>, name: string) {
            const t = target[name] = { "value": null };
            this.setValue(t);
        }
    }

    export class ErrorValue {
        constructor(public value : Value, public invalidReason : string) { }
    }

    export class Result {
        constructor(public wrapped: RoInterfaces.IDomainObjectRepresentation | RoInterfaces.Custom.ICustomListRepresentation | RoInterfaces.IScalarValueRepresentation, private resultType: string) { }

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
        }

        constructor(private map: RoInterfaces.IValueMap | RoInterfaces.IObjectOfType, public statusCode: number, public warningMessage: string) {

        }

        valuesMap(): _.Dictionary<ErrorValue> {

            const values = _.pick(this.wrapped(), i => isIValue(i)) as _.Dictionary<IValue>;
            return _.mapValues(values, v => new ErrorValue(new Value(v.value), v.invalidReason));
        }

        invalidReason() {
            
            const temp = this.map;
            if (isIObjectOfType(temp)) {
                return temp["x-ro-invalidReason"];
            }

            return this.wrapped()["x-ro-invalidReason"] as string;
        }

        containsError() {
            return !!this.invalidReason() || !!this.warningMessage ||  _.any(this.valuesMap(), ev => !!ev.invalidReason);
        }

    }

    export class UpdateMap extends ArgumentMap implements IHateoasModel {
        constructor(private domainObject: DomainObjectRepresentation, map: RoInterfaces.IValueMap) {
            super(map, domainObject.instanceId());

            domainObject.updateLink().copyToHateoasModel(this);

            _.each(this.properties(), (value : Value, key : string) => {
                this.setProperty(key, value);
            });
        }   

        properties(): _.Dictionary<Value> {
            return _.mapValues(this.map, (v : any) => new Value(v.value));
        }

        setProperty(name: string, value: Value) {
            value.set(this.map, name);
        }
    }

    export class AddToRemoveFromMap extends ArgumentMap implements IHateoasModel {
        constructor(private collectionResource: CollectionRepresentation, map: RoInterfaces.IValueMap, add: boolean) {
            super(map, collectionResource.instanceId());
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

        constructor(private wrapped: ICustomExtensions) {
        }
        //Standard RO:
        friendlyName(): string{return this.wrapped.friendlyName; }
        description(): string { return this.wrapped.description  ;}
        returnType(): string { return this.wrapped.returnType  ;}
        optional(): boolean { return this.wrapped.optional  ;}
        hasParams(): boolean { return this.wrapped.hasParams  ;}
        elementType(): string { return this.wrapped.elementType  ;}
        domainType(): string { return this.wrapped.domainType  ;}
        pluralName(): string { return this.wrapped.pluralName  ;}
        format(): string { return this.wrapped.format  ;}
        memberOrder(): number { return this.wrapped.memberOrder  ;}
        isService(): boolean { return this.wrapped.isService  ;}
        minLength(): number { return this.wrapped.minLength; }
        //Nof custom:
        choices(): { [index: string]: (string | number | boolean | ILink)[]; } { return this.wrapped["x-ro-nof-choices"]  ; }
        menuPath(): string { return this.wrapped["x-ro-nof-menuPath"]  ;}
        mask(): string { return this.wrapped["x-ro-nof-mask"];}
        renderInEdit(): boolean { return this.wrapped["x-ro-nof-renderInEditMode"]; }
    } 

    // matches a action invoke resource 19.0 representation 

    export class InvokeMap extends ArgumentMap implements IHateoasModel {
        constructor(private invoke: ActionResultRepresentation, map: RoInterfaces.IValueMap) {
            super(map, "");

            // todo must be better way
            this.hateoasUrl = invoke.hateoasUrl;
            this.method = invoke.method;
        }

        setParameter(name: string, value: Value) {
            value.set(this.map, name);
        }

        setUrlParameter(name: string, value: string) {
            this.urlParms = this.urlParms || {};
            this.urlParms[name] = value;
        }
    }

    export class ActionResultRepresentation extends ResourceRepresentation<RoInterfaces.IActionInvokeRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IActionInvokeRepresentation;

        constructor() {
            super();
        }

        getInvokeMap(): InvokeMap {
            // needs to be initialised 
            return null;
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
        resultType(): string {
            return this.wrapped().resultType;
        }

        result(): Result {
            return new Result(this.wrapped().result, this.resultType());
        }
    }

    // matches an action representation 18.0 

    // matches 18.2.1
    export class Parameter
        extends NestedRepresentation<RoInterfaces.IParameterRepresentation>
        implements IField {

        wrapped = () => this.resource() as RoInterfaces.IParameterRepresentation;

        // fix parent type
        constructor(wrapped: RoInterfaces.IParameterRepresentation, public parent: ActionMember | ActionRepresentation, private id : string) {
            super(wrapped);
        }

        parameterId() {
            return this.id;
        }

        // properties 
        choices(): _.Dictionary<Value> {
            const customExtensions = this.extensions();
            // use custom choices extension by preference 
            if (customExtensions.choices()) {
                return _.mapValues(customExtensions.choices(), v  => new Value(v));
            }

            if (this.wrapped().choices) {
                const values = _.map(this.wrapped().choices, item => new Value(item));
                return _.object(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
            }
            return null;
        }

        promptLink(): Link {
            return linkByNamespacedRel(this.links(), "prompt");
        }

        getPrompts(): PromptRepresentation {
            const pr = <PromptRepresentation>this.promptLink().getTarget();
            pr.getPromptMap = () => new PromptMap(pr, this.promptLink().arguments() as RoInterfaces.IPromptMap);
            return pr;
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

        hasPrompt(): boolean {
            //TODO: I think should be: !!this.promptLink() && !!this.promptLink().arguments()["x-ro-searchTerm"];
            return !!this.promptLink() && !!this.promptLink().arguments()["x-ro-searchTerm"];
        }

        isCollectionContributed(): boolean {
            const myparent = this.parent;
            const isOnList = myparent instanceof ActionMember &&  myparent.parent  instanceof ListRepresentation;
            const isList = this.isList(); 
            // todo also need to check element types and ordering perhaps ?

            return isList && isOnList;
        }

        hasChoices(): boolean { return _.any(this.choices()); }
        isMultipleChoices(): boolean {
            //TODO: Make literal a constant
            return (this.hasChoices() || this.hasConditionalChoices()) && this.extensions().returnType() === "list";
        }
        hasConditionalChoices(): boolean {
            return !!this.promptLink() && !this.hasPrompt();
        }
    }

   export class ActionRepresentation extends ResourceRepresentation<RoInterfaces.IActionRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IActionRepresentation;

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

        getInvoke(): ActionResultRepresentation {
         
            const ar =<ActionResultRepresentation>this.invokeLink().getTarget();
            ar.getInvokeMap = () => new InvokeMap(ar, this.invokeLink().arguments() as RoInterfaces.IValueMap);
            return ar;
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
        constructor(private prompt: PromptRepresentation, private promptMap: RoInterfaces.IPromptMap) {
            super(promptMap as RoInterfaces.IValueMap, prompt.instanceId());

            // todo must be better way
            this.hateoasUrl = prompt.hateoasUrl;
            this.method = prompt.method;
        } 

        setSearchTerm(term: string) {
            this.setArgument("x-ro-searchTerm", new Value(term));
        }

        setArgument(name: string, val: Value) {
            val.set(this.map, name);
        }

        setArguments(args: _.Dictionary<Value>) {
            _.each(args, (arg, key) => this.setArgument(key, arg));
        }

        setMember(name: string, value: Value) {
            value.set(this.promptMap["members"] as RoInterfaces.IValueMap, name);
        }    

        setMembers(objectValues: () => _.Dictionary<Value>) {
            if (this.promptMap["members"]) {
                _.forEach(objectValues(), (v, k) => this.setMember(k, v));
            }
        }
    }

    export class PromptRepresentation extends ResourceRepresentation<RoInterfaces.IPromptRepresentation> {

        wrapped = () => this.resource() as RoInterfaces.IPromptRepresentation;

        constructor() {
            super(emptyResource());
        }

        getPromptMap(): PromptMap {
            // needs to be initialised 
            throw "Prompt map function must be initialized";
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
                return _.object<_.Dictionary<Value>>(_.map(values, v  => [v.toString(), v]));
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

        instanceId(): string {
            return this.wrapped().id;
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

        getPrompts(): PromptRepresentation {
            const pr = <PromptRepresentation>this.promptLink().getTarget();
            pr.getPromptMap = () => new PromptMap(pr, this.promptLink().arguments() as RoInterfaces.IValueMap);
            return pr;
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
                return _.object(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
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

        constructor(wrapped: T, public parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation) {
            super(wrapped);
        }

        update(newValue: RoInterfaces.IMember) {
            super.update(newValue);
        }

        memberType(): string {
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

        static wrapMember(toWrap: RoInterfaces.IPropertyMember | RoInterfaces.ICollectionMember | RoInterfaces.IActionMember , parent: DomainObjectRepresentation | MenuRepresentation | ListRepresentation, id : string): Member<RoInterfaces.IMember> {

            if (toWrap.memberType === "property") {
                return new PropertyMember(toWrap as RoInterfaces.IPropertyMember, parent as DomainObjectRepresentation, id);
            } 

            if (toWrap.memberType === "collection") {
                return new CollectionMember(toWrap as RoInterfaces.ICollectionMember, parent as DomainObjectRepresentation, id);
            }

            if (toWrap.memberType === "action") {
                return new ActionMember(toWrap as RoInterfaces.IActionMember, parent, id);
            }

            return null;
        }
    }

    // matches 14.4.1
    export class PropertyMember extends Member<RoInterfaces.IPropertyMember> implements IField {

        wrapped = () => this.resource() as RoInterfaces.IPropertyMember;

        constructor(wrapped: RoInterfaces.IPropertyMember, parent: DomainObjectRepresentation, private id : string) {
            super(wrapped, parent);
        }

        // inlined 

        propertyId(): string {
            return this.id;
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
                this.wrapped[k] = v;
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

        getPrompts(): PromptRepresentation {
            const pr = <PromptRepresentation>this.promptLink().getTarget();
            pr.getPromptMap = () => new PromptMap(pr, this.promptLink().arguments() as RoInterfaces.IValueMap);
            return pr;
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

        hasChoices(): boolean {
            return this.wrapped().hasChoices;
        }

        hasPrompt(): boolean {
            //TODO: I think this is wrong, and should have: && !!this.promptLink().arguments()["x-ro-searchTerm"];
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
                return _.object(_.map(values, v => [v.toString(), v])) as _.Dictionary<Value>;
            }
            return null;
        }

        hasConditionalChoices(): boolean {
            return !!this.promptLink() && !this.hasPrompt();
        }
        //This is actually not relevant to a property. Slight smell here!
        isMultipleChoices(): boolean {
            return false;
        }
        //Same as for isMultipleChoices
        isCollectionContributed(): boolean {
            return false; 
        }
    }

    // matches 14.4.2 
    export class CollectionMember
        extends Member<RoInterfaces.ICollectionMember>
        implements IHasLinksAsValue
    {

        wrapped = () => this.resource() as RoInterfaces.ICollectionMember;

        constructor(wrapped : RoInterfaces.ICollectionMember, parent : DomainObjectRepresentation, private id : string) {
            super(wrapped, parent);
        }

        collectionId(): string {
            return this.id;
        }

        private lazyValue: Link[];

        value(): Link[] {
            this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
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

        constructor(wrapped: RoInterfaces.IActionMember, parent :  DomainObjectRepresentation | MenuRepresentation | ListRepresentation, private id : string) {
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
            return linkByNamespacedRel(this.links(), "invoke");
        }


        getInvoke(): ActionResultRepresentation {
            const ar = <ActionResultRepresentation>this.invokeLink().getTarget();
            ar.getInvokeMap = () => new InvokeMap(ar, this.invokeLink().arguments() as RoInterfaces.IValueMap);
            return ar;
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

        disabledReason(): string {
            return this.wrapped().disabledReason;
        }
    }

    export class DomainObjectRepresentation extends ResourceRepresentation<RoInterfaces.IDomainObjectRepresentation> implements IHasActions {

        wrapped = () => this.resource() as RoInterfaces.IDomainObjectRepresentation;

        constructor() {
            super();
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
            this.propertyMemberMap =  _.pick(this.memberMap, m => m.memberType() === "property") as _.Dictionary<PropertyMember>;
            this.collectionMemberMap = _.pick(this.memberMap, m => m.memberType() === "collection") as _.Dictionary<CollectionMember>;
            this.actionMemberMap = _.pick(this.memberMap, m => m.memberType() === "action") as _.Dictionary<ActionMember>;
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
            this.actionMemberMap = _.pick(this.memberMap, m => m.memberType() === "action") as _.Dictionary<ActionMember>;
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
        message() : string;
        stackTrace() : string[];
    }

    // matches the error representation 10.0 
    export class ErrorRepresentation extends ResourceRepresentation<RoInterfaces.IErrorRepresentation> implements IErrorDetails {
   
        wrapped = () => this.resource() as RoInterfaces.IErrorRepresentation;

        static create(message: string, stackTrace?: string[], causedBy?: RoInterfaces.IErrorDetailsRepresentation) {
            const rawError = {
                links: [],
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

        getMenu(menuId : string): MenuRepresentation {
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

        constructor(againstType : string, toCheckType : string) {
            super();
            this.hateoasUrl = `${appPath}/domain-types/${againstType}/type-actions/isSubtypeOf/invoke`;
            this.urlParms = {};
            this.urlParms["supertype"] = toCheckType;
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
            this.hateoasUrl = appPath;
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
    export class Link  {

        constructor(public wrapped : RoInterfaces.ILink) { }

        href(): string {
            return decodeURIComponent(this.wrapped.href);
        }

        method(): string {
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

        private lazyExtensions: Extensions;

        extensions(): Extensions {
            this.lazyExtensions = this.lazyExtensions ||new Extensions(this.wrapped.extensions);
            return this.lazyExtensions;
        }

        copyToHateoasModel(hateoasModel: IHateoasModel): void {
            hateoasModel.hateoasUrl = this.href();
            hateoasModel.method = this.method();
        }

        private getHateoasTarget(targetType): IHateoasModel {
            const MatchingType = this.repTypeToModel[targetType];
            const target: IHateoasModel = new MatchingType();
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

    export interface IHasActions extends IHasExtensions {
        actionMembers(): _.Dictionary<ActionMember>;
        actionMember(id: string): ActionMember;
    }

    export interface IHasLinksAsValue {
        value(): Link[];
    }

    //TODO: Review name (common capbilities of PropertyMember and Parameter)
    export interface IField extends IHasExtensions {
        choices(): _.Dictionary<Value>;
        isScalar(): boolean;

        hasChoices(): boolean;
        hasPrompt(): boolean;
        isMultipleChoices(): boolean;
        hasConditionalChoices(): boolean;
        isCollectionContributed(): boolean;
    }

    export interface IHasExtensions {
        extensions(): Extensions;
    }
}