//Copyright 2013 Naked Objects Group Ltd
//Licensed under the Apache License, Version 2.0(the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

// ABOUT THIS FILE:
// spiro.models defines a set of classes that correspond directly to the JSON representations returned by Restful Objects
// resources.  These classes provide convenient methods for navigating the contents of those representations, and for
// following links to other resources.

/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.shims.ts" />
/// <reference path="spiro.config.ts" />


module Spiro {

    function isScalarType(typeName: string) {
        return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
    }

    function isListType(typeName: string) {
        return typeName === "list";
    }

    // interfaces 

    export interface HateoasModel {
        hateoasUrl: string;
        method: string;
        url: any;
    }

    export interface Extensions {
        friendlyName: string;
        description: string;
        returnType: string;
        optional: boolean;
        hasParams: boolean;
        elementType: string;
        domainType: string;
        pluralName: string;
        format: string;
        memberOrder: number;
        isService: boolean;
        minLength : number;
    }

    export interface OptionalCapabilities {
        blobsClobs: string;
        deleteObjects: string;
        domainModel: string;
        protoPersistentObjects: string;
        validateOnly: string;
    }

    // rel helper class 
    export class Rel {

        ns: string = "";
        uniqueValue: string;
        parms: string[] = [];

        constructor(public asString: string) {
            this.decomposeRel();
        }

        private decomposeRel() {

            var postFix: string;

            if (this.asString.substring(0, 3) === "urn") {
                // namespaced 
                this.ns = this.asString.substring(0, this.asString.indexOf("/") + 1);
                postFix = this.asString.substring(this.asString.indexOf("/") + 1);
            }
            else {
                postFix = this.asString;
            }

            var splitPostFix = postFix.split(";");

            this.uniqueValue = splitPostFix[0];

            if (splitPostFix.length > 1) {
                this.parms = splitPostFix.slice(1);
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

            var parms = this.asString.split(";");

            if (parms.length > 0) {
                this.applicationType = parms[0];
            }

            for (var i = 1; i < parms.length; i++) {
                if ($.trim(parms[i]).substring(0, 7) === "profile") {
                    this.profile = $.trim(parms[i]);
                    var profileValue = $.trim(this.profile.split("=")[1].replace(/\"/g, ''));
                    this.representationType = $.trim(profileValue.split("/")[1]);
                }
                if ($.trim(parms[i]).substring(0, 16) === "x-ro-domain-type") {
                    this.xRoDomainType = $.trim(parms[i]);
                    this.domainType = $.trim(this.xRoDomainType.split("=")[1].replace(/\"/g, ''));
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
            if (this.isReference()) {
                return <Link>this.wrapped;
            }
            return null;
        }

        scalar(): Object {
            if (this.isReference()) {
                return null;
            }
            return this.wrapped;
        }

        list(): Value[] {
            if (this.isList()) {
                return  _.map(this.wrapped, (i) => {
                    return new Value(i);
                });
            }
            return null;
        }

        toString(): string {
            if (this.isReference()) {
                return this.link().title();
            }
            if (this.isList()) {
                var ss =  _.map(this.list(), (v) => {
                    return v.toString();
                });

                return _.reduce(ss, (m : string , s : string) => {
                    return m + "-" + s;
                });
            }

            return (this.wrapped == null) ? "" : this.wrapped.toString();
        }

        toValueString(): string {
            if (this.isReference()) {
                return this.link().href();
            }
            return (this.wrapped == null) ? "" : this.wrapped.toString();
        }

        set (target: Object, name?: string) {
            if (name) {
                var t = target[name] = {};
                this.set(t);
            }
            else {
                if (this.isReference()) {
                    target["value"] = { "href": this.link().href() };
                }
                else {
                    target["value"] = this.scalar();
                }
            }
        }
    }

    export interface ValueMap {
        [index: string]: Value;
    }

    export interface ErrorValue {
        value: Value;
        invalidReason: string;
    }

    export interface ErrorValueMap {
        [index: string]: ErrorValue;
    }

    // helper class for results 
    export class Result {
        constructor(public wrapped, private resultType: string) { }

        object(): DomainObjectRepresentation {
            if (!this.isNull() && this.resultType == "object") {
                return new DomainObjectRepresentation(this.wrapped);
            }
            return null;
        }

        list(): ListRepresentation {
            if (!this.isNull() && this.resultType == "list") {
                return new ListRepresentation(this.wrapped);
            }
            return null;
        }

        scalar(): ScalarValueRepresentation {
            if (!this.isNull() && this.resultType == "scalar") {
                return new ScalarValueRepresentation(this.wrapped);
            }
            return null;
        }

        isNull(): boolean {
            return this.wrapped == null;
        }

        isVoid(): boolean {
            return (this.resultType == "void");
        }
    }

    // base class for nested representations 
    export class NestedRepresentation {
        constructor(public wrapped) { }

        links(): Links {
            return Links.WrapLinks(this.wrapped.links);
        }

        extensions(): Extensions {
            return this.wrapped.extensions;
        }
    }

    // base class for all representations that can be directly loaded from the server 
    export class HateoasModelBase extends HateoasModelBaseShim implements HateoasModel {
        constructor(object?) {
            super(object);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }
    }

   
    export class ErrorMap extends HateoasModelBase {

        constructor(map: Object, public statusCode: string, public warningMessage: string) {
            super(map);
        }

        valuesMap(): ErrorValueMap {
            var vs: ErrorValueMap = {};

            // distinguish between value map and persist map 
            var map = this.attributes.members ? this.attributes.members : this.attributes;

            for (var v in map) {

                if (map[v].hasOwnProperty("value")) {
                    var ev: ErrorValue = {
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


    export class UpdateMap extends ArgumentMap implements HateoasModel {
        constructor(private domainObject: DomainObjectRepresentation, map: Object) {
            super(map, domainObject, domainObject.instanceId());

            domainObject.updateLink().copyToHateoasModel(this);

            for (var member in this.properties()) {
                var currentValue = domainObject.propertyMembers()[member].value();
                this.setProperty(member, currentValue);
            }
        }

        onChange() {
            // if the update map changes as a result of server changes (eg title changes) update the 
            // associated domain object
            this.domainObject.setFromUpdateMap(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }

        properties(): ValueMap {
            var pps = {};

            for (var p in this.attributes) {
                pps[p] = new Value(this.attributes[p].value);
            }

            return <ValueMap>pps;
        }

        setProperty(name: string, value: Value) {
            value.set(this.attributes, name);
        }
    }

    export class AddToRemoveFromMap extends ArgumentMap implements HateoasModel {
        constructor(private collectionResource: CollectionRepresentation, map: Object, add: boolean) {
            super(map, collectionResource, collectionResource.instanceId());

            var link = add ? collectionResource.addToLink() : collectionResource.removeFromLink();

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

    export class ModifyMap extends ArgumentMap implements HateoasModel {
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

    export class ClearMap extends ArgumentMap implements HateoasModel {
        constructor(propertyResource: PropertyRepresentation) {
            super({}, propertyResource, propertyResource.instanceId());

            propertyResource.clearLink().copyToHateoasModel(this);
        }

        onError(map: Object, statusCode: string, warnings: string) {
            return new ErrorMap(map, statusCode, warnings);
        }
    }

    // helper - collection of Links 
    export class Links extends CollectionShim implements HateoasModel {

        // cannot use constructor to initialise as model property is not yet set and so will 
        // not create members of correct type 
        constructor() {
            super(); 
            
            this.url = () => {
                return this.hateoasUrl;
            };
        }
      
        hateoasUrl: string;
        method: string;

        model = Link;

        models: Link[];

        parse(response) {
            return response.value;
        }

        static WrapLinks(links: any): Links {
            var ll = new Links();
            ll.add(links);
            return ll;
        }

        // returns first link of rel
        private getLinkByRel(rel: Rel): Link {
            return _.find(this.models, (i: Link) => {
                return i.rel().uniqueValue === rel.uniqueValue;
            });
        }

        linkByRel(rel: string) {
            return this.getLinkByRel(new Rel(rel));
        }
    }


    // REPRESENTATIONS

    export class ResourceRepresentation extends HateoasModelBase {
        constructor(object?) {
            super(object);
        }

        private lazyLinks: Links; 

        links(): Links {
            this.lazyLinks = this.lazyLinks || Links.WrapLinks(this.get("links"));
            return this.lazyLinks;
        }

        extensions(): Extensions {
            return this.get("extensions");
        }
    }

    // matches a action invoke resource 19.0 representation 

    export class ActionResultRepresentation extends ResourceRepresentation {

        constructor(object?) {
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
    }

    // matches an action representation 18.0 

    // matches 18.2.1
    export class Parameter extends NestedRepresentation {
        constructor(wrapped, public parent : ActionRepresentation) {
            super(wrapped);
        }

        // properties 
        choices(): ValueMap {

            // use custom choices extension by preference 
            // todo wrap extensions 
            if (this.extensions()['x-ro-nof-choices']) {            
                return _.object<ValueMap>(_.map(<_.Dictionary<Object>>this.extensions()['x-ro-nof-choices'], (v, key) => [key, new Value(v)]));
            }

            if (this.wrapped.choices) {
                var values = _.map(this.wrapped.choices, (item) => new Value(item));
                return _.object<ValueMap>(_.map(values, (v) => [v.toString(), v]));
            }
            return null;
        }

        promptLink(): Link {
            return this.links().linkByRel("urn:org.restfulobjects:rels/prompt");
        }

        getPrompts(): PromptRepresentation {
          //  var arguments = this.promptLink().arguments;

            return <PromptRepresentation> this.promptLink().getTarget();
        }

        default(): Value {
            return new Value(this.wrapped.default);
        }

        // helper
        isScalar(): boolean {
            return isScalarType(this.extensions().returnType) || (isListType(this.extensions().returnType) && isScalarType(this.extensions().elementType));
        }

        hasPrompt(): boolean {
            return !!this.promptLink();
        }

    }

    export interface ParameterMap {
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

        private parameterMap: ParameterMap;

        private initParameterMap(): void {

            if (!this.parameterMap) {
                this.parameterMap = {};

                var parameters = this.get("parameters");

                for (var m in parameters) {
                    var parameter = new Parameter(parameters[m], this);
                    this.parameterMap[m] = parameter;
                }
            }
        }

        parameters(): ParameterMap {
            this.initParameterMap();
            return this.parameterMap;
        }

        disabledReason(): string {
            return this.get("disabledReason");
        }
    }

    export interface ListOrCollection {
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

        choices(): ValueMap {
          
            var ch = this.get("choices");
            if (ch) {
                var values = _.map(ch, (item) => new Value(item));
                return _.object<ValueMap>(_.map(values, (v) => [v.toString(), v]));
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

        setArguments(args : ValueMap) {
            _.each(args, (arg, key) => this.setArgument(key, arg)); 
        }
    }

    // matches a collection representation 17.0 
    export class CollectionRepresentation extends ResourceRepresentation implements ListOrCollection {

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
            return Links.WrapLinks(this.get("value"));
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

        choices(): ValueMap{

            // use custom choices extension by preference 
            // todo wrap extensions 
            if (this.extensions()['x-ro-nof-choices']) {
                return _.object<ValueMap>(_.map(<_.Dictionary<Object>>this.extensions()['x-ro-nof-choices'], (v, key) => [key, new Value(v)]));
            }

            var ch = this.get("choices");
            if (ch) {
                var values = _.map(ch, (item) => new Value(item));
                return _.object<ValueMap>(_.map(values, (v) => [v.toString(), v]));
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
        constructor(wrapped, public parent : DomainObjectRepresentation) {
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

        static WrapMember(toWrap, parent): Member {

            if (toWrap.memberType === "property") {
                return new PropertyMember(toWrap, parent);
            }

            if (toWrap.memberType === "collection") {
                return new CollectionMember(toWrap, parent);
            }

            if (toWrap.memberType === "action") {
                return new ActionMember(toWrap, parent);
            }

            return null;
        }
    }

    // matches 14.4.1
    export class PropertyMember extends Member {
        constructor(wrapped, parent) {
            super(wrapped, parent);
        }

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

        choices(): ValueMap {

            // use custom choices extension by preference 
            // todo wrap extensions 
            if (this.extensions()['x-ro-nof-choices']) {
                return _.object<ValueMap>(_.map(<_.Dictionary<Object>>this.extensions()['x-ro-nof-choices'], (v, key) => [key, new Value(v)]));
            }

            var ch = this.wrapped.choices;
            if (ch) {
                var values = _.map(ch, (item) => new Value(item));
                return _.object<ValueMap>(_.map(values, (v) => [v.toString(), v]));
            }
            return null;
        }

    }

    // matches 14.4.2 
    export class CollectionMember extends Member {
        constructor(wrapped, parent) {
            super(wrapped, parent);
        }

        value(): DomainObjectRepresentation[] {

            if (this.wrapped.value && this.wrapped.value.length) {

                var valueArray = [];

                for (var i = 0; i < this.wrapped.value.length; i++) {
                    valueArray[i] = new DomainObjectRepresentation(this.wrapped.value[i]);
                }

                return valueArray;
            }
            return [];
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
        constructor(wrapped, parent) {
            super(wrapped, parent);
        }

        getDetails(): ActionRepresentation {
            return <ActionRepresentation> this.detailsLink().getTarget();
        }
    }

    export interface MemberMap {
        [index: string]: Member;
    }

    export interface PropertyMemberMap {
        [index: string]: PropertyMember;
    }

    export interface CollectionMemberMap {
        [index: string]: CollectionMember;
    }

    export interface ActionMemberMap {
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
            return Links.WrapLinks(this.get("links"));
        }

        instanceId(): string {
            return this.get("instanceId");
        }

        extensions(): Extensions {
            return this.get("extensions");
        }

        private memberMap: MemberMap;
        private propertyMemberMap: PropertyMemberMap;
        private collectionMemberMap: CollectionMemberMap;
        private actionMemberMap: ActionMemberMap;

        private resetMemberMaps() {
            this.memberMap = {};
            this.propertyMemberMap = {};
            this.collectionMemberMap = {};
            this.actionMemberMap = {};

            var members = this.get("members");

            for (var m in members) {
                var member = Member.WrapMember(members[m], this);
                this.memberMap[m] = member;

                if (member.memberType() === "property") {
                    this.propertyMemberMap[m] = <PropertyMember> member;
                }
                else if (member.memberType() === "collection") {
                    this.collectionMemberMap[m] = <CollectionMember> member;
                }
                else if (member.memberType() === "action") {
                    this.actionMemberMap[m] = <ActionMember> member;
                }
            }
        }

        private initMemberMaps() {
            if (!this.memberMap) {
                this.resetMemberMaps();
            }
        }

        members(): MemberMap {
            this.initMemberMaps();
            return this.memberMap;
        }

        propertyMembers(): PropertyMemberMap {
            this.initMemberMaps();
            return this.propertyMemberMap;
        }

        collectionMembers(): CollectionMemberMap {
            this.initMemberMaps();
            return this.collectionMemberMap;
        }

        actionMembers(): ActionMemberMap {
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
            for (var member in this.members()) {
                var m = this.members()[member];
                m.update(map.attributes["members"][member]);
            }

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
    export class ListRepresentation extends ResourceRepresentation implements ListOrCollection {

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
            return Links.WrapLinks(this.get("value"));
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
            var cb = this.get("causedBy");
            return cb ? new ErrorRepresentation(cb) : null;
        }
    }

    // matches Objects of Type Resource 9.0 
    export class PersistMap extends ArgumentMap implements HateoasModel {

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

        optionalCapabilities(): OptionalCapabilities {
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

    // matches the home page representation  5.0 
    export class HomePageRepresentation extends ResourceRepresentation {

        constructor() {
            super();
            this.hateoasUrl = appPath;
        }

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

        // linked representations 
        getSelf(): HomePageRepresentation {
            return <HomePageRepresentation> this.selfLink().getTarget();
        }

        getUser(): UserRepresentation {
            return <UserRepresentation> this.userLink().getTarget();
        }

        getDomainServices(): DomainServicesRepresentation {
            // cannot use getTarget here as that will just return a ListRepresentation 
            var domainServices = new DomainServicesRepresentation();
            this.serviceLink().copyToHateoasModel(domainServices);
            return domainServices;
        }

        getVersion(): VersionRepresentation {
            return <VersionRepresentation> this.versionLink().getTarget();
        }
    }

    // matches the Link representation 2.7
    export class Link extends ModelShim {

        constructor(object?) {
            super(object);
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

        extensions(): Extensions {
            return this.get("extensions");
        }

        copyToHateoasModel(hateoasModel: HateoasModel): void {
            hateoasModel.hateoasUrl = this.href();
            hateoasModel.method = this.method();
        }

        private getHateoasTarget(targetType): HateoasModel {
            var matchingType = this.repTypeToModel[targetType];
            var target: HateoasModel = new matchingType({});
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
        }

        // get the object that this link points to 
        getTarget(): HateoasModel {
            var target = this.getHateoasTarget(this.type().representationType);
            this.copyToHateoasModel(target);
            return target;
        }
    }
}