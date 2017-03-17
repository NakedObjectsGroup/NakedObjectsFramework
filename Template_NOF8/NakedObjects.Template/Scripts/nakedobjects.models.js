//  Copyright 2013-2014 Naked Objects Group Ltd
//  Licensed under the Apache License, Version 2.0(the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
// ABOUT THIS FILE:
// nakedobjects.models defines a set of classes that correspond directly to the JSON representations returned by Restful Objects
// resources.  These classes provide convenient methods for navigating the contents of those representations, and for
// following links to other resources.
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.config.ts" />
/// <reference path="nakedobjects.rointerfaces.ts" />
var NakedObjects;
(function (NakedObjects) {
    var Models;
    (function (Models) {
        // helper functions 
        function isScalarType(typeName) {
            return typeName === "string" || typeName === "number" || typeName === "boolean" || typeName === "integer";
        }
        function isListType(typeName) {
            return typeName === "list";
        }
        function emptyResource() {
            return { links: [], extensions: {} };
        }
        function isILink(object) {
            return object && object instanceof Object && "href" in object;
        }
        function isIObjectOfType(object) {
            return object && object instanceof Object && "members" in object;
        }
        function isIValue(object) {
            return object && object instanceof Object && "value" in object;
        }
        function isResourceRepresentation(object) {
            return object && object instanceof Object && "links" in object && "extensions" in object;
        }
        Models.isResourceRepresentation = isResourceRepresentation;
        function isErrorRepresentation(object) {
            return isResourceRepresentation(object) && "message" in object;
        }
        Models.isErrorRepresentation = isErrorRepresentation;
        function isIDomainObjectRepresentation(object) {
            return isResourceRepresentation(object) && "domainType" in object && "instanceId" in object && "members" in object;
        }
        Models.isIDomainObjectRepresentation = isIDomainObjectRepresentation;
        function isIInvokableAction(object) {
            return object && "parameters" in object && "extensions" in object;
        }
        Models.isIInvokableAction = isIInvokableAction;
        function getId(prop) {
            if (prop instanceof PropertyRepresentation) {
                return prop.instanceId();
            }
            else {
                return prop.id();
            }
        }
        function wrapLinks(links) {
            return _.map(links, function (l) { return new Link(l); });
        }
        function getLinkByRel(links, rel) {
            return _.find(links, function (i) { return i.rel().uniqueValue === rel.uniqueValue; });
        }
        function linkByRel(links, rel) {
            return getLinkByRel(links, new Rel(rel));
        }
        function linkByNamespacedRel(links, rel) {
            return getLinkByRel(links, new Rel("urn:org.restfulobjects:rels/" + rel));
        }
        (function (ErrorCategory) {
            ErrorCategory[ErrorCategory["HttpClientError"] = 0] = "HttpClientError";
            ErrorCategory[ErrorCategory["HttpServerError"] = 1] = "HttpServerError";
            ErrorCategory[ErrorCategory["ClientError"] = 2] = "ClientError";
        })(Models.ErrorCategory || (Models.ErrorCategory = {}));
        var ErrorCategory = Models.ErrorCategory;
        (function (HttpStatusCode) {
            HttpStatusCode[HttpStatusCode["NoContent"] = 204] = "NoContent";
            HttpStatusCode[HttpStatusCode["BadRequest"] = 400] = "BadRequest";
            HttpStatusCode[HttpStatusCode["Unauthorized"] = 401] = "Unauthorized";
            HttpStatusCode[HttpStatusCode["Forbidden"] = 403] = "Forbidden";
            HttpStatusCode[HttpStatusCode["NotFound"] = 404] = "NotFound";
            HttpStatusCode[HttpStatusCode["MethodNotAllowed"] = 405] = "MethodNotAllowed";
            HttpStatusCode[HttpStatusCode["NotAcceptable"] = 406] = "NotAcceptable";
            HttpStatusCode[HttpStatusCode["PreconditionFailed"] = 412] = "PreconditionFailed";
            HttpStatusCode[HttpStatusCode["UnprocessableEntity"] = 422] = "UnprocessableEntity";
            HttpStatusCode[HttpStatusCode["PreconditionRequired"] = 428] = "PreconditionRequired";
            HttpStatusCode[HttpStatusCode["InternalServerError"] = 500] = "InternalServerError";
        })(Models.HttpStatusCode || (Models.HttpStatusCode = {}));
        var HttpStatusCode = Models.HttpStatusCode;
        (function (ClientErrorCode) {
            ClientErrorCode[ClientErrorCode["ExpiredTransient"] = 0] = "ExpiredTransient";
            ClientErrorCode[ClientErrorCode["WrongType"] = 1] = "WrongType";
            ClientErrorCode[ClientErrorCode["NotImplemented"] = 2] = "NotImplemented";
            ClientErrorCode[ClientErrorCode["SoftwareError"] = 3] = "SoftwareError";
            ClientErrorCode[ClientErrorCode["ConnectionProblem"] = -1] = "ConnectionProblem";
        })(Models.ClientErrorCode || (Models.ClientErrorCode = {}));
        var ClientErrorCode = Models.ClientErrorCode;
        var ErrorWrapper = (function () {
            function ErrorWrapper(rc, code, err) {
                this.handled = false;
                this.category = rc;
                if (rc === ErrorCategory.ClientError) {
                    this.clientErrorCode = code;
                    this.errorCode = ClientErrorCode[this.clientErrorCode];
                    var description = NakedObjects.errorUnknown;
                    switch (this.clientErrorCode) {
                        case ClientErrorCode.ExpiredTransient:
                            description = NakedObjects.errorExpiredTransient;
                            break;
                        case ClientErrorCode.WrongType:
                            description = NakedObjects.errorWrongType;
                            break;
                        case ClientErrorCode.NotImplemented:
                            description = NakedObjects.errorNotImplemented;
                            break;
                        case ClientErrorCode.SoftwareError:
                            description = NakedObjects.errorSoftware;
                            break;
                        case ClientErrorCode.ConnectionProblem:
                            description = NakedObjects.errorConnection;
                            break;
                    }
                    this.description = description;
                    this.title = NakedObjects.errorClient;
                }
                if (rc === ErrorCategory.HttpClientError || rc === ErrorCategory.HttpServerError) {
                    this.httpErrorCode = code;
                    this.errorCode = HttpStatusCode[this.httpErrorCode] + "(" + this.httpErrorCode + ")";
                    this.description = rc === ErrorCategory.HttpServerError
                        ? "A software error has occurred on the server"
                        : "An HTTP error code has been received from the server\n" +
                            "You can look up the meaning of this code in the Restful Objects specification.";
                    this.title = "Error message received from server";
                }
                if (err instanceof ErrorMap) {
                    var em = err;
                    this.message = em.invalidReason() || em.warningMessage;
                    this.error = em;
                    this.stackTrace = [];
                }
                else if (err instanceof ErrorRepresentation) {
                    var er = err;
                    this.message = er.message();
                    this.error = er;
                    this.stackTrace = err.stackTrace();
                }
                else {
                    this.message = err;
                    this.error = null;
                    this.stackTrace = [];
                }
            }
            return ErrorWrapper;
        }());
        Models.ErrorWrapper = ErrorWrapper;
        // abstract classes 
        function toOid(id) {
            return _.reduce(id, function (a, v) { return ("" + a + NakedObjects.keySeparator + v); });
        }
        var ObjectIdWrapper = (function () {
            function ObjectIdWrapper() {
            }
            ObjectIdWrapper.prototype.getKey = function () {
                return this.domainType + NakedObjects.keySeparator + this.instanceId;
            };
            ObjectIdWrapper.safeSplit = function (id) {
                if (id) {
                    return id.split(NakedObjects.keySeparator);
                }
                return [];
            };
            ObjectIdWrapper.fromObject = function (object) {
                var oid = new ObjectIdWrapper();
                oid.domainType = object.domainType();
                oid.instanceId = object.instanceId();
                oid.splitInstanceId = this.safeSplit(oid.instanceId);
                oid.isService = !oid.instanceId;
                return oid;
            };
            ObjectIdWrapper.fromLink = function (link) {
                var href = link.href();
                return this.fromHref(href);
            };
            ObjectIdWrapper.fromHref = function (href) {
                var oid = new ObjectIdWrapper();
                oid.domainType = Models.typeFromUrl(href);
                oid.instanceId = Models.idFromUrl(href);
                oid.splitInstanceId = this.safeSplit(oid.instanceId);
                oid.isService = !oid.instanceId;
                return oid;
            };
            ObjectIdWrapper.fromObjectId = function (objectId) {
                var oid = new ObjectIdWrapper();
                var _a = objectId.split(NakedObjects.keySeparator), dt = _a[0], id = _a.slice(1);
                oid.domainType = dt;
                oid.splitInstanceId = id;
                oid.instanceId = toOid(id);
                oid.isService = !oid.instanceId;
                return oid;
            };
            ObjectIdWrapper.fromRaw = function (dt, id) {
                var oid = new ObjectIdWrapper();
                oid.domainType = dt;
                oid.instanceId = id;
                oid.splitInstanceId = this.safeSplit(oid.instanceId);
                oid.isService = !oid.instanceId;
                return oid;
            };
            ObjectIdWrapper.fromSplitRaw = function (dt, id) {
                var oid = new ObjectIdWrapper();
                oid.domainType = dt;
                oid.splitInstanceId = id;
                oid.instanceId = toOid(id);
                oid.isService = !oid.instanceId;
                return oid;
            };
            ObjectIdWrapper.prototype.isSame = function (other) {
                return other && other.domainType === this.domainType && other.instanceId === this.instanceId;
            };
            return ObjectIdWrapper;
        }());
        Models.ObjectIdWrapper = ObjectIdWrapper;
        var HateosModel = (function () {
            function HateosModel(model) {
                this.model = model;
                this.hateoasUrl = "";
                this.method = "GET";
            }
            HateosModel.prototype.populate = function (model) {
                this.model = model;
            };
            HateosModel.prototype.getBody = function () {
                if (this.method === "POST" || this.method === "PUT") {
                    var m = _.clone(this.model);
                    var up = _.clone(this.urlParms);
                    return _.merge(m, up);
                }
                return {};
            };
            HateosModel.prototype.getUrl = function () {
                var url = this.hateoasUrl;
                var attrAsJson = _.clone(this.model);
                if (this.method === "GET" || this.method === "DELETE") {
                    if (_.keys(attrAsJson).length > 0) {
                        // there are model parms so encode everything into json 
                        var urlParmsAsJson = _.clone(this.urlParms);
                        var asJson = _.merge(attrAsJson, urlParmsAsJson);
                        if (_.keys(asJson).length > 0) {
                            var map = JSON.stringify(asJson);
                            var parmString = encodeURI(map);
                            return url + "?" + parmString;
                        }
                        return url;
                    }
                    if (_.keys(this.urlParms).length > 0) {
                        // there are only url reserved parms so they can just be appended to url
                        var urlParmString = _.reduce(this.urlParms, function (result, n, key) { return (result === "" ? "" : result + "&") + key + "=" + n; }, "");
                        return url + "?" + urlParmString;
                    }
                }
                return url;
            };
            HateosModel.prototype.setUrlParameter = function (name, value) {
                this.urlParms = this.urlParms || {};
                this.urlParms[name] = value;
            };
            return HateosModel;
        }());
        Models.HateosModel = HateosModel;
        var ArgumentMap = (function (_super) {
            __extends(ArgumentMap, _super);
            function ArgumentMap(map, id) {
                _super.call(this, map);
                this.map = map;
                this.id = id;
            }
            ArgumentMap.prototype.populate = function (wrapped) {
                _super.prototype.populate.call(this, wrapped);
            };
            return ArgumentMap;
        }(HateosModel));
        Models.ArgumentMap = ArgumentMap;
        var NestedRepresentation = (function () {
            function NestedRepresentation(model) {
                var _this = this;
                this.model = model;
                this.resource = function () { return _this.model; };
            }
            NestedRepresentation.prototype.links = function () {
                this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
                return this.lazyLinks;
            };
            NestedRepresentation.prototype.update = function (newResource) {
                this.model = newResource;
                this.lazyLinks = null;
            };
            NestedRepresentation.prototype.extensions = function () {
                this.lazyExtensions = this.lazyExtensions || new Extensions(this.model.extensions);
                return this.lazyExtensions;
            };
            return NestedRepresentation;
        }());
        Models.NestedRepresentation = NestedRepresentation;
        // classes
        var RelParm = (function () {
            function RelParm(asString) {
                this.asString = asString;
                this.decomposeParm();
            }
            RelParm.prototype.decomposeParm = function () {
                var regex = /(\w+)\W+(\w+)\W+/;
                var result = regex.exec(this.asString);
                this.name = result[1], this.value = result[2];
            };
            return RelParm;
        }());
        Models.RelParm = RelParm;
        var Rel = (function () {
            function Rel(asString) {
                this.asString = asString;
                this.ns = "";
                this.parms = [];
                this.decomposeRel();
            }
            Rel.prototype.decomposeRel = function () {
                var postFix;
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
                    this.parms = _.map(splitPostFix.slice(1), function (s) { return new RelParm(s); });
                }
            };
            return Rel;
        }());
        Models.Rel = Rel;
        var MediaType = (function () {
            function MediaType(asString) {
                this.asString = asString;
                this.decomposeMediaType();
            }
            MediaType.prototype.decomposeMediaType = function () {
                var parms = this.asString.split(";");
                if (parms.length > 0) {
                    this.applicationType = parms[0];
                }
                for (var i = 1; i < parms.length; i++) {
                    if (parms[i].trim().substring(0, 7) === "profile") {
                        this.profile = parms[i].trim();
                        var profileValue = (this.profile.split("=")[1].replace(/\"/g, "")).trim();
                        this.representationType = (profileValue.split("/")[1]).trim();
                    }
                    if (parms[i].trim().substring(0, 16) === NakedObjects.roDomainType) {
                        this.xRoDomainType = (parms[i]).trim();
                        this.domainType = (this.xRoDomainType.split("=")[1].replace(/\"/g, "")).trim();
                    }
                }
            };
            return MediaType;
        }());
        Models.MediaType = MediaType;
        var Value = (function () {
            function Value(raw) {
                // can only be Link, number, boolean, string or null    
                if (raw instanceof Array) {
                    this.wrapped = raw;
                }
                else if (raw instanceof Link) {
                    this.wrapped = raw;
                }
                else if (isILink(raw)) {
                    this.wrapped = new Link(raw);
                }
                else {
                    this.wrapped = raw;
                }
            }
            Value.prototype.isBlob = function () {
                return this.wrapped instanceof Blob;
            };
            Value.prototype.isScalar = function () {
                return !this.isReference() && !this.isList();
            };
            Value.prototype.isReference = function () {
                return this.wrapped instanceof Link;
            };
            Value.prototype.isFileReference = function () {
                return this.wrapped instanceof Link && this.link().href().indexOf("data") === 0;
            };
            Value.prototype.isList = function () {
                return this.wrapped instanceof Array;
            };
            Value.prototype.isNull = function () {
                return this.wrapped == null;
            };
            Value.prototype.blob = function () {
                return this.isBlob() ? this.wrapped : null;
            };
            Value.prototype.link = function () {
                return this.isReference() ? this.wrapped : null;
            };
            Value.prototype.href = function () {
                return this.link() ? this.link().href() : null;
            };
            Value.prototype.scalar = function () {
                return this.isScalar() ? this.wrapped : null;
            };
            Value.prototype.list = function () {
                return this.isList() ? _.map(this.wrapped, function (i) { return new Value(i); }) : null;
            };
            Value.prototype.toString = function () {
                if (this.isReference()) {
                    return this.link().title();
                }
                if (this.isList()) {
                    var ss = _.map(this.list(), function (v) { return v.toString(); });
                    return ss.length === 0 ? "" : _.reduce(ss, function (m, s) { return m + "-" + s; }, "");
                }
                return (this.wrapped == null) ? "" : this.wrapped.toString();
            };
            Value.prototype.compress = function () {
                if (this.isReference()) {
                    this.link().compress();
                }
                if (this.isList()) {
                    _.forEach(this.list(), function (i) { return i.compress(); });
                }
                ;
                if (this.scalar() && this.wrapped instanceof String) {
                    this.wrapped = Models.compress(this.wrapped);
                }
            };
            Value.prototype.decompress = function () {
                if (this.isReference()) {
                    this.link().decompress();
                }
                if (this.isList()) {
                    _.forEach(this.list(), function (i) { return i.decompress(); });
                }
                ;
                if (this.scalar() && this.wrapped instanceof String) {
                    this.wrapped = Models.decompress(this.wrapped);
                }
            };
            Value.fromJsonString = function (jsonString) {
                var value = new Value(JSON.parse(jsonString));
                value.decompress();
                return value;
            };
            Value.prototype.toValueString = function () {
                if (this.isReference()) {
                    return this.link().href();
                }
                return (this.wrapped == null) ? "" : this.wrapped.toString();
            };
            Value.prototype.toJsonString = function () {
                var cloneThis = _.cloneDeep(this);
                cloneThis.compress();
                var value = cloneThis.wrapped;
                var raw = (value instanceof Link) ? value.wrapped : value;
                return JSON.stringify(raw);
            };
            Value.prototype.setValue = function (target) {
                if (this.isFileReference()) {
                    target.value = this.link().wrapped;
                }
                else if (this.isReference()) {
                    target.value = { "href": this.link().href() };
                }
                else if (this.isList()) {
                    target.value = _.map(this.list(), function (v) { return v.isReference() ? { "href": v.link().href() } : v.scalar(); });
                }
                else if (this.isBlob()) {
                    target.value = this.blob();
                }
                else {
                    target.value = this.scalar();
                }
            };
            Value.prototype.set = function (target, name) {
                var t = target[name] = { value: null };
                this.setValue(t);
            };
            return Value;
        }());
        Models.Value = Value;
        var ErrorValue = (function () {
            function ErrorValue(value, invalidReason) {
                this.value = value;
                this.invalidReason = invalidReason;
            }
            return ErrorValue;
        }());
        Models.ErrorValue = ErrorValue;
        var Result = (function () {
            function Result(wrapped, resultType) {
                this.wrapped = wrapped;
                this.resultType = resultType;
            }
            Result.prototype.object = function () {
                if (!this.isNull() && this.resultType === "object") {
                    var dor = new DomainObjectRepresentation();
                    dor.populate(this.wrapped);
                    return dor;
                }
                return null;
            };
            Result.prototype.list = function () {
                if (!this.isNull() && this.resultType === "list") {
                    var lr = new ListRepresentation();
                    lr.populate(this.wrapped);
                    return lr;
                }
                return null;
            };
            Result.prototype.scalar = function () {
                if (!this.isNull() && this.resultType === "scalar") {
                    return new ScalarValueRepresentation(this.wrapped);
                }
                return null;
            };
            Result.prototype.isNull = function () {
                return this.wrapped == null;
            };
            Result.prototype.isVoid = function () {
                return (this.resultType === "void");
            };
            return Result;
        }());
        Models.Result = Result;
        var ErrorMap = (function () {
            function ErrorMap(map, statusCode, warningMessage) {
                var _this = this;
                this.map = map;
                this.statusCode = statusCode;
                this.warningMessage = warningMessage;
                this.wrapped = function () {
                    var temp = _this.map;
                    if (isIObjectOfType(temp)) {
                        return temp.members;
                    }
                    else {
                        return temp;
                    }
                };
            }
            ErrorMap.prototype.valuesMap = function () {
                var values = _.pickBy(this.wrapped(), function (i) { return isIValue(i); });
                return _.mapValues(values, function (v) { return new ErrorValue(new Value(v.value), v.invalidReason); });
            };
            ErrorMap.prototype.invalidReason = function () {
                var temp = this.map;
                if (isIObjectOfType(temp)) {
                    return temp[NakedObjects.roInvalidReason];
                }
                return this.wrapped()[NakedObjects.roInvalidReason];
            };
            ErrorMap.prototype.containsError = function () {
                return !!this.invalidReason() || !!this.warningMessage || _.some(this.valuesMap(), function (ev) { return !!ev.invalidReason; });
            };
            return ErrorMap;
        }());
        Models.ErrorMap = ErrorMap;
        var UpdateMap = (function (_super) {
            __extends(UpdateMap, _super);
            function UpdateMap(domainObject, map) {
                var _this = this;
                _super.call(this, map, domainObject.instanceId());
                this.domainObject = domainObject;
                domainObject.updateLink().copyToHateoasModel(this);
                _.each(this.properties(), function (value, key) {
                    _this.setProperty(key, value);
                });
            }
            UpdateMap.prototype.properties = function () {
                return _.mapValues(this.map, function (v) { return new Value(v.value); });
            };
            UpdateMap.prototype.setProperty = function (name, value) {
                value.set(this.map, name);
            };
            UpdateMap.prototype.setValidateOnly = function () {
                this.map[NakedObjects.roValidateOnly] = true;
            };
            return UpdateMap;
        }(ArgumentMap));
        Models.UpdateMap = UpdateMap;
        var AddToRemoveFromMap = (function (_super) {
            __extends(AddToRemoveFromMap, _super);
            function AddToRemoveFromMap(collectionResource, map, add) {
                _super.call(this, map, collectionResource.collectionId());
                this.collectionResource = collectionResource;
                var link = add ? collectionResource.addToLink() : collectionResource.removeFromLink();
                link.copyToHateoasModel(this);
            }
            return AddToRemoveFromMap;
        }(ArgumentMap));
        Models.AddToRemoveFromMap = AddToRemoveFromMap;
        var ModifyMap = (function (_super) {
            __extends(ModifyMap, _super);
            function ModifyMap(propertyResource, map) {
                _super.call(this, map, getId(propertyResource));
                this.propertyResource = propertyResource;
                propertyResource.modifyLink().copyToHateoasModel(this);
                propertyResource.value().set(this.map, this.id);
            }
            return ModifyMap;
        }(ArgumentMap));
        Models.ModifyMap = ModifyMap;
        var ClearMap = (function (_super) {
            __extends(ClearMap, _super);
            function ClearMap(propertyResource) {
                _super.call(this, {}, getId(propertyResource));
                propertyResource.clearLink().copyToHateoasModel(this);
            }
            return ClearMap;
        }(ArgumentMap));
        Models.ClearMap = ClearMap;
        // REPRESENTATIONS
        var ResourceRepresentation = (function (_super) {
            __extends(ResourceRepresentation, _super);
            function ResourceRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.resource = function () { return _this.model; };
            }
            ResourceRepresentation.prototype.populate = function (wrapped) {
                _super.prototype.populate.call(this, wrapped);
            };
            ResourceRepresentation.prototype.links = function () {
                this.lazyLinks = this.lazyLinks || wrapLinks(this.resource().links);
                return this.lazyLinks;
            };
            ResourceRepresentation.prototype.extensions = function () {
                this.lazyExtensions = this.lazyExtensions || new Extensions(this.resource().extensions);
                return this.lazyExtensions;
            };
            return ResourceRepresentation;
        }(HateosModel));
        Models.ResourceRepresentation = ResourceRepresentation;
        var Extensions = (function () {
            function Extensions(wrapped) {
                var _this = this;
                this.wrapped = wrapped;
                //Standard RO:
                this.friendlyName = function () { return _this.wrapped.friendlyName; };
                this.description = function () { return _this.wrapped.description; };
                this.returnType = function () { return _this.wrapped.returnType; };
                this.optional = function () { return _this.wrapped.optional; };
                this.hasParams = function () { return _this.wrapped.hasParams; };
                this.elementType = function () { return _this.wrapped.elementType; };
                this.domainType = function () { return _this.wrapped.domainType; };
                this.pluralName = function () { return _this.wrapped.pluralName; };
                this.format = function () { return _this.wrapped.format; };
                this.memberOrder = function () { return _this.wrapped.memberOrder; };
                this.isService = function () { return _this.wrapped.isService; };
                this.minLength = function () { return _this.wrapped.minLength; };
                this.maxLength = function () { return _this.wrapped.maxLength; };
                this.pattern = function () { return _this.wrapped.pattern; };
                //Nof custom:
                this.choices = function () { return _this.wrapped["x-ro-nof-choices"]; };
                this.menuPath = function () { return _this.wrapped["x-ro-nof-menuPath"]; };
                this.mask = function () { return _this.wrapped["x-ro-nof-mask"]; };
                this.tableViewTitle = function () { return _this.wrapped["x-ro-nof-tableViewTitle"]; };
                this.tableViewColumns = function () { return _this.wrapped["x-ro-nof-tableViewColumns"]; };
                this.multipleLines = function () { return _this.wrapped["x-ro-nof-multipleLines"]; };
                this.warnings = function () { return _this.wrapped["x-ro-nof-warnings"]; };
                this.messages = function () { return _this.wrapped["x-ro-nof-messages"]; };
                this.interactionMode = function () { return _this.wrapped["x-ro-nof-interactionMode"]; };
                this.dataType = function () { return _this.wrapped["x-ro-nof-dataType"]; };
                this.range = function () { return _this.wrapped["x-ro-nof-range"]; };
                this.notNavigable = function () { return _this.wrapped["x-ro-nof-notNavigable"]; };
                this.renderEagerly = function () { return _this.wrapped["x-ro-nof-renderEagerly"]; };
                this.presentationHint = function () { return _this.wrapped["x-ro-nof-presentationHint"]; };
            }
            return Extensions;
        }());
        Models.Extensions = Extensions;
        // matches a action invoke resource 19.0 representation 
        var InvokeMap = (function (_super) {
            __extends(InvokeMap, _super);
            function InvokeMap(link) {
                _super.call(this, link.arguments(), "");
                this.link = link;
                link.copyToHateoasModel(this);
            }
            InvokeMap.prototype.setParameter = function (name, value) {
                value.set(this.map, name);
            };
            return InvokeMap;
        }(ArgumentMap));
        Models.InvokeMap = InvokeMap;
        var ActionResultRepresentation = (function (_super) {
            __extends(ActionResultRepresentation, _super);
            function ActionResultRepresentation() {
                var _this = this;
                _super.call(this);
                this.wrapped = function () { return _this.resource(); };
            }
            // links 
            ActionResultRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            // link representations 
            ActionResultRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            // properties 
            ActionResultRepresentation.prototype.resultType = function () {
                return this.wrapped().resultType;
            };
            ActionResultRepresentation.prototype.result = function () {
                return new Result(this.wrapped().result, this.resultType());
            };
            ActionResultRepresentation.prototype.warningsOrMessages = function () {
                var has = function (arr) { return arr && arr.length > 0; };
                var wOrM = has(this.extensions().warnings()) ? this.extensions().warnings() : this.extensions().messages();
                if (has(wOrM)) {
                    return _.reduce(wOrM, function (s, t) { return s + " " + t; }, "");
                }
                return undefined;
            };
            ActionResultRepresentation.prototype.shouldExpectResult = function () {
                return this.result().isNull() && this.resultType() !== "void";
            };
            return ActionResultRepresentation;
        }(ResourceRepresentation));
        Models.ActionResultRepresentation = ActionResultRepresentation;
        // matches 18.2.1
        var Parameter = (function (_super) {
            __extends(Parameter, _super);
            // fix parent type
            function Parameter(wrapped, parent, paramId) {
                var _this = this;
                _super.call(this, wrapped);
                this.parent = parent;
                this.paramId = paramId;
                this.wrapped = function () { return _this.resource(); };
            }
            Parameter.prototype.id = function () {
                return this.paramId;
            };
            // properties 
            Parameter.prototype.choices = function () {
                var customExtensions = this.extensions();
                // use custom choices extension by preference 
                if (customExtensions.choices()) {
                    return _.mapValues(customExtensions.choices(), function (v) { return new Value(v); });
                }
                if (this.wrapped().choices) {
                    var values = _.map(this.wrapped().choices, function (item) { return new Value(item); });
                    return _.fromPairs(_.map(values, function (v) { return [v.toString(), v]; }));
                }
                return null;
            };
            Parameter.prototype.promptLink = function () {
                return linkByNamespacedRel(this.links(), "prompt");
            };
            Parameter.prototype.getPromptMap = function () {
                var pr = this.promptLink().getTarget();
                return new PromptMap(this.promptLink(), pr.instanceId());
            };
            Parameter.prototype.default = function () {
                var dflt = this.wrapped().default == null ? (isScalarType(this.extensions().returnType()) ? "" : null) : this.wrapped().default;
                return new Value(dflt);
            };
            // helper
            Parameter.prototype.isScalar = function () {
                return isScalarType(this.extensions().returnType()) ||
                    (isListType(this.extensions().returnType()) && isScalarType(this.extensions().elementType()));
            };
            Parameter.prototype.isList = function () {
                return isListType(this.extensions().returnType());
            };
            Parameter.prototype.hasPrompt = function () {
                return !!this.promptLink();
            };
            Parameter.prototype.isCollectionContributed = function () {
                var myparent = this.parent;
                var isOnList = (myparent instanceof ActionMember || myparent instanceof ActionRepresentation) &&
                    (myparent.parent instanceof ListRepresentation || myparent.parent instanceof CollectionRepresentation || myparent.parent instanceof CollectionMember);
                var isList = this.isList();
                return isList && isOnList;
            };
            Parameter.prototype.hasChoices = function () { return _.some(this.choices()); };
            Parameter.prototype.entryType = function () {
                if (this.hasPrompt()) {
                    // ConditionalChoices, ConditionalMultipleChoices, AutoComplete 
                    if (!!this.promptLink().arguments()[NakedObjects.roSearchTerm]) {
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
            };
            return Parameter;
        }(NestedRepresentation));
        Models.Parameter = Parameter;
        var ActionRepresentation = (function (_super) {
            __extends(ActionRepresentation, _super);
            function ActionRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links 
            ActionRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            ActionRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            ActionRepresentation.prototype.invokeLink = function () {
                return linkByNamespacedRel(this.links(), "invoke");
            };
            // linked representations 
            ActionRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            ActionRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            ActionRepresentation.prototype.getInvokeMap = function () {
                return new InvokeMap(this.invokeLink());
            };
            // properties 
            ActionRepresentation.prototype.actionId = function () {
                return this.wrapped().id;
            };
            ActionRepresentation.prototype.initParameterMap = function () {
                var _this = this;
                if (!this.parameterMap) {
                    var parameters = this.wrapped().parameters;
                    this.parameterMap = _.mapValues(parameters, function (p, id) { return new Parameter(p, _this, id); });
                }
            };
            ActionRepresentation.prototype.parameters = function () {
                this.initParameterMap();
                return this.parameterMap;
            };
            ActionRepresentation.prototype.disabledReason = function () {
                return this.wrapped().disabledReason;
            };
            return ActionRepresentation;
        }(ResourceRepresentation));
        Models.ActionRepresentation = ActionRepresentation;
        // new in 1.1 15.0 in spec 
        var PromptMap = (function (_super) {
            __extends(PromptMap, _super);
            function PromptMap(link, promptId) {
                _super.call(this, link.arguments(), promptId);
                this.link = link;
                this.promptId = promptId;
                link.copyToHateoasModel(this);
            }
            PromptMap.prototype.promptMap = function () {
                return this.map;
            };
            PromptMap.prototype.setSearchTerm = function (term) {
                this.setArgument(NakedObjects.roSearchTerm, new Value(term));
            };
            PromptMap.prototype.setArgument = function (name, val) {
                val.set(this.map, name);
            };
            PromptMap.prototype.setArguments = function (args) {
                var _this = this;
                _.each(args, function (arg, key) { return _this.setArgument(key, arg); });
            };
            PromptMap.prototype.setMember = function (name, value) {
                value.set(this.promptMap()["x-ro-nof-members"], name);
            };
            PromptMap.prototype.setMembers = function (objectValues) {
                var _this = this;
                if (this.map["x-ro-nof-members"]) {
                    _.forEach(objectValues(), function (v, k) { return _this.setMember(k, v); });
                }
            };
            return PromptMap;
        }(ArgumentMap));
        Models.PromptMap = PromptMap;
        var PromptRepresentation = (function (_super) {
            __extends(PromptRepresentation, _super);
            function PromptRepresentation() {
                var _this = this;
                _super.call(this, emptyResource());
                this.wrapped = function () { return _this.resource(); };
            }
            // links 
            PromptRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            PromptRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            // linked representations 
            PromptRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            PromptRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            // properties 
            PromptRepresentation.prototype.instanceId = function () {
                return this.wrapped().id;
            };
            PromptRepresentation.prototype.choices = function () {
                var ch = this.wrapped().choices;
                if (ch) {
                    var values = _.map(ch, function (item) { return new Value(item); });
                    return _.fromPairs(_.map(values, function (v) { return [v.toString(), v]; }));
                }
                return null;
            };
            return PromptRepresentation;
        }(ResourceRepresentation));
        Models.PromptRepresentation = PromptRepresentation;
        // matches a collection representation 17.0 
        var CollectionRepresentation = (function (_super) {
            __extends(CollectionRepresentation, _super);
            function CollectionRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links 
            CollectionRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            CollectionRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            CollectionRepresentation.prototype.addToLink = function () {
                return linkByNamespacedRel(this.links(), "add-to");
            };
            CollectionRepresentation.prototype.removeFromLink = function () {
                return linkByNamespacedRel(this.links(), "remove-from");
            };
            // linked representations 
            CollectionRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            CollectionRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            CollectionRepresentation.prototype.setFromMap = function (map) {
                //this.set(map.attributes);
                _.assign(this.resource(), map.map);
            };
            CollectionRepresentation.prototype.addToMap = function () {
                return this.addToLink().arguments();
            };
            CollectionRepresentation.prototype.getAddToMap = function () {
                if (this.addToLink()) {
                    return new AddToRemoveFromMap(this, this.addToMap(), true);
                }
                return null;
            };
            CollectionRepresentation.prototype.removeFromMap = function () {
                return this.removeFromLink().arguments();
            };
            CollectionRepresentation.prototype.getRemoveFromMap = function () {
                if (this.removeFromLink()) {
                    return new AddToRemoveFromMap(this, this.removeFromMap(), false);
                }
                return null;
            };
            // properties 
            CollectionRepresentation.prototype.collectionId = function () {
                return this.wrapped().id;
            };
            CollectionRepresentation.prototype.size = function () {
                return this.value().length;
            };
            CollectionRepresentation.prototype.value = function () {
                this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
                return this.lazyValue;
            };
            CollectionRepresentation.prototype.disabledReason = function () {
                return this.wrapped().disabledReason;
            };
            CollectionRepresentation.prototype.actionMembers = function () {
                var _this = this;
                this.actionMemberMap = this.actionMemberMap || _.mapValues(this.wrapped().members, function (m, id) { return Member.wrapMember(m, _this, id); });
                return this.actionMemberMap;
            };
            CollectionRepresentation.prototype.actionMember = function (id) {
                return this.actionMembers()[id];
            };
            return CollectionRepresentation;
        }(ResourceRepresentation));
        Models.CollectionRepresentation = CollectionRepresentation;
        // matches a property representation 16.0 
        var PropertyRepresentation = (function (_super) {
            __extends(PropertyRepresentation, _super);
            function PropertyRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links 
            PropertyRepresentation.prototype.modifyLink = function () {
                return linkByNamespacedRel(this.links(), "modify");
            };
            PropertyRepresentation.prototype.clearLink = function () {
                return linkByNamespacedRel(this.links(), "clear");
            };
            PropertyRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            PropertyRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            PropertyRepresentation.prototype.promptLink = function () {
                return linkByNamespacedRel(this.links(), "prompt");
            };
            PropertyRepresentation.prototype.modifyMap = function () {
                return this.modifyLink().arguments();
            };
            // linked representations 
            PropertyRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            PropertyRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            PropertyRepresentation.prototype.setFromModifyMap = function (map) {
                //this.set(map.attributes);
                _.assign(this.resource(), map.map);
            };
            PropertyRepresentation.prototype.getModifyMap = function () {
                if (this.modifyLink()) {
                    return new ModifyMap(this, this.modifyMap());
                }
                return null;
            };
            PropertyRepresentation.prototype.getClearMap = function () {
                if (this.clearLink()) {
                    return new ClearMap(this);
                }
                return null;
            };
            // properties 
            PropertyRepresentation.prototype.instanceId = function () {
                return this.wrapped().id;
            };
            PropertyRepresentation.prototype.value = function () {
                return new Value(this.wrapped().value);
            };
            PropertyRepresentation.prototype.choices = function () {
                // use custom choices extension by preference 
                if (this.extensions().choices()) {
                    return _.mapValues(this.extensions().choices(), function (v) { return new Value(v); });
                }
                var ch = this.wrapped().choices;
                if (ch) {
                    var values = _.map(ch, function (item) { return new Value(item); });
                    return _.fromPairs(_.map(values, function (v) { return [v.toString(), v]; }));
                }
                return null;
            };
            PropertyRepresentation.prototype.disabledReason = function () {
                return this.wrapped().disabledReason;
            };
            // helper 
            PropertyRepresentation.prototype.isScalar = function () {
                return isScalarType(this.extensions().returnType());
            };
            PropertyRepresentation.prototype.hasPrompt = function () {
                return !!this.promptLink();
            };
            return PropertyRepresentation;
        }(ResourceRepresentation));
        Models.PropertyRepresentation = PropertyRepresentation;
        // matches a domain object representation 14.0 
        // base class for 14.4.1/2/3
        var Member = (function (_super) {
            __extends(Member, _super);
            function Member(wrapped) {
                var _this = this;
                _super.call(this, wrapped);
                this.wrapped = function () { return _this.resource(); };
            }
            Member.prototype.update = function (newValue) {
                _super.prototype.update.call(this, newValue);
            };
            Member.prototype.memberType = function () {
                return this.wrapped().memberType;
            };
            Member.prototype.detailsLink = function () {
                return linkByNamespacedRel(this.links(), "details");
            };
            Member.prototype.disabledReason = function () {
                return this.wrapped().disabledReason;
            };
            Member.prototype.isScalar = function () {
                return isScalarType(this.extensions().returnType());
            };
            Member.wrapMember = function (toWrap, parent, id) {
                if (toWrap.memberType === "property") {
                    return new PropertyMember(toWrap, parent, id);
                }
                if (toWrap.memberType === "collection") {
                    return new CollectionMember(toWrap, parent, id);
                }
                if (toWrap.memberType === "action" && !(parent instanceof Link)) {
                    var member = new ActionMember(toWrap, parent, id);
                    if (member.invokeLink()) {
                        return new InvokableActionMember(toWrap, parent, id);
                    }
                    return member;
                }
                return null;
            };
            return Member;
        }(NestedRepresentation));
        Models.Member = Member;
        // matches 14.4.1
        var PropertyMember = (function (_super) {
            __extends(PropertyMember, _super);
            function PropertyMember(wrapped, parent, propId) {
                var _this = this;
                _super.call(this, wrapped);
                this.parent = parent;
                this.propId = propId;
                this.wrapped = function () { return _this.resource(); };
            }
            // inlined 
            PropertyMember.prototype.id = function () {
                return this.propId;
            };
            PropertyMember.prototype.modifyLink = function () {
                return linkByNamespacedRel(this.links(), "modify");
            };
            PropertyMember.prototype.clearLink = function () {
                return linkByNamespacedRel(this.links(), "clear");
            };
            PropertyMember.prototype.modifyMap = function () {
                return this.modifyLink().arguments();
            };
            PropertyMember.prototype.setFromModifyMap = function (map) {
                var _this = this;
                _.forOwn(map.map, function (v, k) {
                    _this.wrapped[k] = v;
                });
            };
            PropertyMember.prototype.getModifyMap = function (id) {
                if (this.modifyLink()) {
                    return new ModifyMap(this, this.modifyMap());
                }
                return null;
            };
            PropertyMember.prototype.getClearMap = function (id) {
                if (this.clearLink()) {
                    return new ClearMap(this);
                }
                return null;
            };
            PropertyMember.prototype.getPromptMap = function () {
                var pr = this.promptLink().getTarget();
                return new PromptMap(this.promptLink(), pr.instanceId());
            };
            PropertyMember.prototype.value = function () {
                return new Value(this.wrapped().value);
            };
            PropertyMember.prototype.isScalar = function () {
                return isScalarType(this.extensions().returnType());
            };
            PropertyMember.prototype.attachmentLink = function () {
                return linkByNamespacedRel(this.links(), "attachment");
            };
            PropertyMember.prototype.promptLink = function () {
                return linkByNamespacedRel(this.links(), "prompt");
            };
            PropertyMember.prototype.getDetails = function () {
                return this.detailsLink().getTarget();
            };
            PropertyMember.prototype.hasChoices = function () {
                return this.wrapped().hasChoices;
            };
            PropertyMember.prototype.hasPrompt = function () {
                return !!this.promptLink();
            };
            PropertyMember.prototype.choices = function () {
                // use custom choices extension by preference 
                if (this.extensions().choices()) {
                    return _.mapValues(this.extensions().choices(), function (v) { return new Value(v); });
                }
                var ch = this.wrapped().choices;
                if (ch) {
                    var values = _.map(ch, function (item) { return new Value(item); });
                    return _.fromPairs(_.map(values, function (v) { return [v.toString(), v]; }));
                }
                return null;
            };
            PropertyMember.prototype.hasConditionalChoices = function () {
                return !!this.promptLink() && !this.hasPrompt();
            };
            //This is actually not relevant to a property. Slight smell here!
            PropertyMember.prototype.isCollectionContributed = function () {
                return false;
            };
            PropertyMember.prototype.entryType = function () {
                if (this.hasPrompt()) {
                    // ConditionalChoices, ConditionalMultipleChoices, AutoComplete 
                    if (!!this.promptLink().arguments()[NakedObjects.roSearchTerm]) {
                        // autocomplete 
                        return EntryType.AutoComplete;
                    }
                    return EntryType.ConditionalChoices;
                }
                if (this.choices()) {
                    return EntryType.Choices;
                }
                return EntryType.FreeForm;
            };
            return PropertyMember;
        }(Member));
        Models.PropertyMember = PropertyMember;
        // matches 14.4.2 
        var CollectionMember = (function (_super) {
            __extends(CollectionMember, _super);
            function CollectionMember(wrapped, parent, id) {
                var _this = this;
                _super.call(this, wrapped);
                this.parent = parent;
                this.id = id;
                this.wrapped = function () { return _this.resource(); };
                this.etagDigest = parent.etagDigest;
            }
            CollectionMember.prototype.collectionId = function () {
                return this.id;
            };
            CollectionMember.prototype.value = function () {
                this.lazyValue = this.lazyValue || (this.wrapped().value ? wrapLinks(this.wrapped().value) : null);
                return this.lazyValue;
            };
            CollectionMember.prototype.size = function () {
                return this.wrapped().size;
            };
            CollectionMember.prototype.getDetails = function () {
                return this.detailsLink().getTarget();
            };
            CollectionMember.prototype.actionMembers = function () {
                var _this = this;
                if (this.wrapped().members) {
                    this.actionMemberMap = this.actionMemberMap || _.mapValues(this.wrapped().members, function (m, id) { return Member.wrapMember(m, _this, id); });
                    return this.actionMemberMap;
                }
                return {};
            };
            CollectionMember.prototype.actionMember = function (id) {
                return this.actionMembers()[id];
            };
            return CollectionMember;
        }(Member));
        Models.CollectionMember = CollectionMember;
        // matches 14.4.3 
        var ActionMember = (function (_super) {
            __extends(ActionMember, _super);
            function ActionMember(wrapped, parent, id) {
                var _this = this;
                _super.call(this, wrapped);
                this.parent = parent;
                this.id = id;
                this.wrapped = function () { return _this.resource(); };
            }
            ActionMember.prototype.actionId = function () {
                return this.id;
            };
            ActionMember.prototype.getDetails = function () {
                var details = this.detailsLink().getTarget();
                details.parent = this.parent;
                return details;
            };
            // 1.1 inlined 
            ActionMember.prototype.invokeLink = function () {
                return linkByNamespacedRel(this.links(), "invoke");
            };
            ActionMember.prototype.disabledReason = function () {
                return this.wrapped().disabledReason;
            };
            return ActionMember;
        }(Member));
        Models.ActionMember = ActionMember;
        var InvokableActionMember = (function (_super) {
            __extends(InvokableActionMember, _super);
            function InvokableActionMember(wrapped, parent, id) {
                _super.call(this, wrapped, parent, id);
            }
            InvokableActionMember.prototype.getInvokeMap = function () {
                var invokeLink = this.invokeLink();
                if (invokeLink) {
                    return new InvokeMap(this.invokeLink());
                }
                return null;
            };
            InvokableActionMember.prototype.initParameterMap = function () {
                var _this = this;
                if (!this.parameterMap) {
                    var parameters = this.wrapped().parameters;
                    this.parameterMap = _.mapValues(parameters, function (p, id) { return new Parameter(p, _this, id); });
                }
            };
            InvokableActionMember.prototype.parameters = function () {
                this.initParameterMap();
                return this.parameterMap;
            };
            return InvokableActionMember;
        }(ActionMember));
        Models.InvokableActionMember = InvokableActionMember;
        var DomainObjectRepresentation = (function (_super) {
            __extends(DomainObjectRepresentation, _super);
            function DomainObjectRepresentation() {
                var _this = this;
                _super.call(this);
                this.wrapped = function () { return _this.resource(); };
            }
            DomainObjectRepresentation.prototype.id = function () {
                return "" + (this.domainType() || this.serviceId()) + (this.instanceId() ? "" + NakedObjects.keySeparator + this.instanceId() : "");
            };
            DomainObjectRepresentation.prototype.title = function () {
                return this.wrapped().title;
            };
            DomainObjectRepresentation.prototype.domainType = function () {
                return this.wrapped().domainType;
            };
            DomainObjectRepresentation.prototype.serviceId = function () {
                return this.wrapped().serviceId;
            };
            DomainObjectRepresentation.prototype.instanceId = function () {
                return this.wrapped().instanceId;
            };
            DomainObjectRepresentation.prototype.resetMemberMaps = function () {
                var _this = this;
                var members = this.wrapped().members;
                this.memberMap = _.mapValues(members, function (m, id) { return Member.wrapMember(m, _this, id); });
                this.propertyMemberMap = _.pickBy(this.memberMap, function (m) { return m.memberType() === "property"; });
                this.collectionMemberMap = _.pickBy(this.memberMap, function (m) { return m.memberType() === "collection"; });
                this.actionMemberMap = _.pickBy(this.memberMap, function (m) { return m.memberType() === "action"; });
            };
            DomainObjectRepresentation.prototype.initMemberMaps = function () {
                if (!this.memberMap) {
                    this.resetMemberMaps();
                }
            };
            DomainObjectRepresentation.prototype.members = function () {
                this.initMemberMaps();
                return this.memberMap;
            };
            DomainObjectRepresentation.prototype.propertyMembers = function () {
                this.initMemberMaps();
                return this.propertyMemberMap;
            };
            DomainObjectRepresentation.prototype.collectionMembers = function () {
                this.initMemberMaps();
                return this.collectionMemberMap;
            };
            DomainObjectRepresentation.prototype.actionMembers = function () {
                this.initMemberMaps();
                return this.actionMemberMap;
            };
            DomainObjectRepresentation.prototype.member = function (id) {
                return this.members()[id];
            };
            DomainObjectRepresentation.prototype.propertyMember = function (id) {
                return this.propertyMembers()[id];
            };
            DomainObjectRepresentation.prototype.collectionMember = function (id) {
                return this.collectionMembers()[id];
            };
            DomainObjectRepresentation.prototype.actionMember = function (id) {
                return this.actionMembers()[id];
            };
            DomainObjectRepresentation.prototype.updateLink = function () {
                return linkByNamespacedRel(this.links(), "update");
            };
            DomainObjectRepresentation.prototype.isTransient = function () {
                return !!this.persistLink();
            };
            DomainObjectRepresentation.prototype.persistLink = function () {
                return linkByNamespacedRel(this.links(), "persist");
            };
            DomainObjectRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            DomainObjectRepresentation.prototype.updateMap = function () {
                return this.updateLink().arguments();
            };
            DomainObjectRepresentation.prototype.persistMap = function () {
                return this.persistLink().arguments();
            };
            // linked representations 
            DomainObjectRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            DomainObjectRepresentation.prototype.getPersistMap = function () {
                return new PersistMap(this, this.persistMap());
            };
            DomainObjectRepresentation.prototype.getUpdateMap = function () {
                return new UpdateMap(this, this.updateMap());
            };
            DomainObjectRepresentation.prototype.setInlinePropertyDetails = function (flag) {
                this.setUrlParameter(NakedObjects.roInlinePropertyDetails, flag);
            };
            DomainObjectRepresentation.prototype.getOid = function () {
                if (!this.oid) {
                    this.oid = ObjectIdWrapper.fromObject(this);
                }
                return this.oid;
            };
            return DomainObjectRepresentation;
        }(ResourceRepresentation));
        Models.DomainObjectRepresentation = DomainObjectRepresentation;
        var MenuRepresentation = (function (_super) {
            __extends(MenuRepresentation, _super);
            function MenuRepresentation() {
                var _this = this;
                _super.call(this);
                this.wrapped = function () { return _this.resource(); };
            }
            MenuRepresentation.prototype.title = function () {
                return this.wrapped().title;
            };
            MenuRepresentation.prototype.menuId = function () {
                return this.wrapped().menuId;
            };
            MenuRepresentation.prototype.resetMemberMaps = function () {
                var _this = this;
                var members = this.wrapped().members;
                this.memberMap = _.mapValues(members, function (m, id) { return Member.wrapMember(m, _this, id); });
                this.actionMemberMap = _.pickBy(this.memberMap, function (m) { return m.memberType() === "action"; });
            };
            MenuRepresentation.prototype.initMemberMaps = function () {
                if (!this.memberMap) {
                    this.resetMemberMaps();
                }
            };
            MenuRepresentation.prototype.members = function () {
                this.initMemberMaps();
                return this.memberMap;
            };
            MenuRepresentation.prototype.actionMembers = function () {
                this.initMemberMaps();
                return this.actionMemberMap;
            };
            MenuRepresentation.prototype.member = function (id) {
                return this.members()[id];
            };
            MenuRepresentation.prototype.actionMember = function (id) {
                return this.actionMembers()[id];
            };
            MenuRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            // linked representations 
            MenuRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            return MenuRepresentation;
        }(ResourceRepresentation));
        Models.MenuRepresentation = MenuRepresentation;
        // matches scalar representation 12.0 
        var ScalarValueRepresentation = (function (_super) {
            __extends(ScalarValueRepresentation, _super);
            function ScalarValueRepresentation(wrapped) {
                var _this = this;
                _super.call(this, wrapped);
                this.wrapped = function () { return _this.resource(); };
            }
            ScalarValueRepresentation.prototype.value = function () {
                return new Value(this.wrapped().value);
            };
            return ScalarValueRepresentation;
        }(NestedRepresentation));
        Models.ScalarValueRepresentation = ScalarValueRepresentation;
        // matches List Representation 11.0
        var ListRepresentation = (function (_super) {
            __extends(ListRepresentation, _super);
            function ListRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links
            ListRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            // linked representations 
            ListRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            ListRepresentation.prototype.value = function () {
                this.lazyValue = this.lazyValue || wrapLinks(this.wrapped().value);
                return this.lazyValue;
            };
            ListRepresentation.prototype.pagination = function () {
                return this.wrapped().pagination;
            };
            ListRepresentation.prototype.actionMembers = function () {
                var _this = this;
                this.actionMemberMap = this.actionMemberMap || _.mapValues(this.wrapped().members, function (m, id) { return Member.wrapMember(m, _this, id); });
                return this.actionMemberMap;
            };
            ListRepresentation.prototype.actionMember = function (id) {
                return this.actionMembers()[id];
            };
            return ListRepresentation;
        }(ResourceRepresentation));
        Models.ListRepresentation = ListRepresentation;
        // matches the error representation 10.0 
        var ErrorRepresentation = (function (_super) {
            __extends(ErrorRepresentation, _super);
            function ErrorRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            ErrorRepresentation.create = function (message, stackTrace, causedBy) {
                var rawError = {
                    links: [],
                    extensions: {},
                    message: message,
                    stackTrace: stackTrace,
                    causedBy: causedBy
                };
                var error = new ErrorRepresentation();
                error.populate(rawError);
                return error;
            };
            // scalar properties 
            ErrorRepresentation.prototype.message = function () {
                return this.wrapped().message;
            };
            ErrorRepresentation.prototype.stackTrace = function () {
                return this.wrapped().stackTrace;
            };
            ErrorRepresentation.prototype.causedBy = function () {
                var cb = this.wrapped().causedBy;
                return cb ? {
                    message: function () { return cb.message; },
                    stackTrace: function () { return cb.stackTrace; }
                } : undefined;
            };
            return ErrorRepresentation;
        }(ResourceRepresentation));
        Models.ErrorRepresentation = ErrorRepresentation;
        // matches Objects of Type Resource 9.0 
        var PersistMap = (function (_super) {
            __extends(PersistMap, _super);
            function PersistMap(domainObject, map) {
                _super.call(this, map);
                this.domainObject = domainObject;
                this.map = map;
                domainObject.persistLink().copyToHateoasModel(this);
            }
            PersistMap.prototype.setMember = function (name, value) {
                value.set(this.map.members, name);
            };
            PersistMap.prototype.setValidateOnly = function () {
                this.map[NakedObjects.roValidateOnly] = true;
            };
            return PersistMap;
        }(HateosModel));
        Models.PersistMap = PersistMap;
        // matches the version representation 8.0 
        var VersionRepresentation = (function (_super) {
            __extends(VersionRepresentation, _super);
            function VersionRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links 
            VersionRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            VersionRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            // linked representations 
            VersionRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            VersionRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            // scalar properties 
            VersionRepresentation.prototype.specVersion = function () {
                return this.wrapped().specVersion;
            };
            VersionRepresentation.prototype.implVersion = function () {
                return this.wrapped().implVersion;
            };
            VersionRepresentation.prototype.optionalCapabilities = function () {
                return this.wrapped().optionalCapabilities;
            };
            return VersionRepresentation;
        }(ResourceRepresentation));
        Models.VersionRepresentation = VersionRepresentation;
        // matches Domain Services Representation 7.0
        var DomainServicesRepresentation = (function (_super) {
            __extends(DomainServicesRepresentation, _super);
            function DomainServicesRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links
            DomainServicesRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            // linked representations 
            DomainServicesRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            DomainServicesRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            DomainServicesRepresentation.prototype.getService = function (serviceType) {
                var serviceLink = _.find(this.value(), function (link) { return link.rel().parms[0].value === serviceType; });
                return serviceLink.getTarget();
            };
            return DomainServicesRepresentation;
        }(ListRepresentation));
        Models.DomainServicesRepresentation = DomainServicesRepresentation;
        // custom
        var MenusRepresentation = (function (_super) {
            __extends(MenusRepresentation, _super);
            function MenusRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links
            MenusRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            // linked representations 
            MenusRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            MenusRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            MenusRepresentation.prototype.getMenu = function (menuId) {
                var menuLink = _.find(this.value(), function (link) { return link.rel().parms[0].value === menuId; });
                return menuLink.getTarget();
            };
            return MenusRepresentation;
        }(ListRepresentation));
        Models.MenusRepresentation = MenusRepresentation;
        // matches the user representation 6.0
        var UserRepresentation = (function (_super) {
            __extends(UserRepresentation, _super);
            function UserRepresentation() {
                var _this = this;
                _super.apply(this, arguments);
                this.wrapped = function () { return _this.resource(); };
            }
            // links 
            UserRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            UserRepresentation.prototype.upLink = function () {
                return linkByRel(this.links(), "up");
            };
            // linked representations 
            UserRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            UserRepresentation.prototype.getUp = function () {
                return this.upLink().getTarget();
            };
            // scalar properties 
            UserRepresentation.prototype.userName = function () {
                return this.wrapped().userName;
            };
            UserRepresentation.prototype.friendlyName = function () {
                return this.wrapped().friendlyName;
            };
            UserRepresentation.prototype.email = function () {
                return this.wrapped().email;
            };
            UserRepresentation.prototype.roles = function () {
                return this.wrapped().roles;
            };
            return UserRepresentation;
        }(ResourceRepresentation));
        Models.UserRepresentation = UserRepresentation;
        var DomainTypeActionInvokeRepresentation = (function (_super) {
            __extends(DomainTypeActionInvokeRepresentation, _super);
            function DomainTypeActionInvokeRepresentation(againstType, toCheckType) {
                var _this = this;
                _super.call(this);
                this.wrapped = function () { return _this.resource(); };
                this.hateoasUrl = NakedObjects.getAppPath() + "/domain-types/" + toCheckType + "/type-actions/isSubtypeOf/invoke";
                this.urlParms = {};
                this.urlParms["supertype"] = againstType;
            }
            DomainTypeActionInvokeRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            // linked representations 
            DomainTypeActionInvokeRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            DomainTypeActionInvokeRepresentation.prototype.id = function () {
                return this.wrapped().id;
            };
            DomainTypeActionInvokeRepresentation.prototype.value = function () {
                return this.wrapped().value;
            };
            return DomainTypeActionInvokeRepresentation;
        }(ResourceRepresentation));
        Models.DomainTypeActionInvokeRepresentation = DomainTypeActionInvokeRepresentation;
        // matches the home page representation  5.0 
        var HomePageRepresentation = (function (_super) {
            __extends(HomePageRepresentation, _super);
            function HomePageRepresentation() {
                var _this = this;
                _super.call(this);
                this.wrapped = function () { return _this.resource(); };
                this.hateoasUrl = NakedObjects.getAppPath();
            }
            // links 
            HomePageRepresentation.prototype.serviceLink = function () {
                return linkByNamespacedRel(this.links(), "services");
            };
            HomePageRepresentation.prototype.userLink = function () {
                return linkByNamespacedRel(this.links(), "user");
            };
            HomePageRepresentation.prototype.selfLink = function () {
                return linkByRel(this.links(), "self");
            };
            HomePageRepresentation.prototype.versionLink = function () {
                return linkByNamespacedRel(this.links(), "version");
            };
            // custom 
            HomePageRepresentation.prototype.menusLink = function () {
                return linkByNamespacedRel(this.links(), "menus");
            };
            // linked representations 
            HomePageRepresentation.prototype.getSelf = function () {
                return this.selfLink().getTarget();
            };
            HomePageRepresentation.prototype.getUser = function () {
                return this.userLink().getTarget();
            };
            HomePageRepresentation.prototype.getDomainServices = function () {
                // cannot use getTarget here as that will just return a ListRepresentation 
                var domainServices = new DomainServicesRepresentation();
                this.serviceLink().copyToHateoasModel(domainServices);
                return domainServices;
            };
            HomePageRepresentation.prototype.getVersion = function () {
                return this.versionLink().getTarget();
            };
            //  custom 
            HomePageRepresentation.prototype.getMenus = function () {
                // cannot use getTarget here as that will just return a ListRepresentation 
                var menus = new MenusRepresentation();
                this.menusLink().copyToHateoasModel(menus);
                return menus;
            };
            return HomePageRepresentation;
        }(ResourceRepresentation));
        Models.HomePageRepresentation = HomePageRepresentation;
        // matches the Link representation 2.7
        var Link = (function () {
            function Link(wrapped) {
                this.wrapped = wrapped;
                this.repTypeToModel = {
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
            }
            Link.prototype.compress = function () {
                this.wrapped.href = Models.compress(this.wrapped.href);
            };
            Link.prototype.decompress = function () {
                this.wrapped.href = Models.decompress(this.wrapped.href);
            };
            Link.prototype.href = function () {
                return decodeURIComponent(this.wrapped.href);
            };
            Link.prototype.method = function () {
                return this.wrapped.method;
            };
            Link.prototype.rel = function () {
                return new Rel(this.wrapped.rel);
            };
            Link.prototype.type = function () {
                return new MediaType(this.wrapped.type);
            };
            Link.prototype.title = function () {
                return this.wrapped.title;
            };
            //Typically used to set a title on a link that doesn't naturally have one e.g. Self link
            Link.prototype.setTitle = function (title) {
                this.wrapped.title = title;
            };
            Link.prototype.arguments = function () {
                return this.wrapped.arguments;
            };
            Link.prototype.members = function () {
                var _this = this;
                var members = this.wrapped.members;
                return members ? _.mapValues(members, function (m, id) { return Member.wrapMember(m, _this, id); }) : null;
            };
            Link.prototype.extensions = function () {
                this.lazyExtensions = this.lazyExtensions || new Extensions(this.wrapped.extensions);
                return this.lazyExtensions;
            };
            Link.prototype.copyToHateoasModel = function (hateoasModel) {
                hateoasModel.hateoasUrl = this.href();
                hateoasModel.method = this.method();
            };
            Link.prototype.getHateoasTarget = function (targetType) {
                var MatchingType = this.repTypeToModel[targetType];
                var target = new MatchingType();
                return target;
            };
            // get the object that this link points to 
            Link.prototype.getTarget = function () {
                var target = this.getHateoasTarget(this.type().representationType);
                this.copyToHateoasModel(target);
                return target;
            };
            Link.prototype.getOid = function () {
                if (!this.oid) {
                    this.oid = ObjectIdWrapper.fromLink(this);
                }
                return this.oid;
            };
            return Link;
        }());
        Models.Link = Link;
        (function (EntryType) {
            EntryType[EntryType["FreeForm"] = 0] = "FreeForm";
            EntryType[EntryType["Choices"] = 1] = "Choices";
            EntryType[EntryType["MultipleChoices"] = 2] = "MultipleChoices";
            EntryType[EntryType["ConditionalChoices"] = 3] = "ConditionalChoices";
            EntryType[EntryType["MultipleConditionalChoices"] = 4] = "MultipleConditionalChoices";
            EntryType[EntryType["AutoComplete"] = 5] = "AutoComplete";
            EntryType[EntryType["File"] = 6] = "File";
        })(Models.EntryType || (Models.EntryType = {}));
        var EntryType = Models.EntryType;
    })(Models = NakedObjects.Models || (NakedObjects.Models = {}));
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.models.js.map