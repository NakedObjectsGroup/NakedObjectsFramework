// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;

namespace NakedObjects.Rest.Snapshot.Utility {
    public static class RestUtils {
        private static readonly Dictionary<Type, PredefinedJsonType> SimpleTypeMap = new Dictionary<Type, PredefinedJsonType> {
            {typeof (sbyte), PredefinedJsonType.Number},
            {typeof (byte), PredefinedJsonType.Number},
            {typeof (short), PredefinedJsonType.Number},
            {typeof (ushort), PredefinedJsonType.Number},
            {typeof (int), PredefinedJsonType.Number},
            {typeof (uint), PredefinedJsonType.Number},
            {typeof (long), PredefinedJsonType.Number},
            {typeof (ulong), PredefinedJsonType.Number},
            {typeof (char), PredefinedJsonType.String},
            {typeof (bool), PredefinedJsonType.Boolean},
            {typeof (string), PredefinedJsonType.String},
            {typeof (float), PredefinedJsonType.Number},
            {typeof (double), PredefinedJsonType.Number},
            {typeof (decimal), PredefinedJsonType.Number},
            {typeof (void), PredefinedJsonType.Void}
        };

        private static readonly Dictionary<Type, PredefinedFormatType> SimpleFormatMap = new Dictionary<Type, PredefinedFormatType> {
            {typeof (sbyte), PredefinedFormatType.Int},
            {typeof (byte), PredefinedFormatType.Int},
            {typeof (short), PredefinedFormatType.Int},
            {typeof (ushort), PredefinedFormatType.Int},
            {typeof (int), PredefinedFormatType.Int},
            {typeof (uint), PredefinedFormatType.Int},
            {typeof (long), PredefinedFormatType.Int},
            {typeof (ulong), PredefinedFormatType.Int},
            {typeof (char), PredefinedFormatType.String},          
            {typeof (string), PredefinedFormatType.String},
            {typeof (float), PredefinedFormatType.Decimal},
            {typeof (double), PredefinedFormatType.Decimal},
            {typeof (decimal), PredefinedFormatType.Decimal},
            {typeof (byte[]), PredefinedFormatType.Blob},
            {typeof (sbyte[]), PredefinedFormatType.Blob},
            {typeof (char[]), PredefinedFormatType.Clob}
        };


        public static MapRepresentation GetExtensions(string friendlyname,
                                                      string description,
                                                      string pluralName,
                                                      string domainType,
                                                      bool? isService,
                                                      bool? hasParams,
                                                      bool? optional,
                                                      int? maxLength,
                                                      string pattern,
                                                      int? memberOrder,
                                                      DataType? dataType,
                                                      string presentationHint,
                                                      IDictionary<string, object> customExtensions,
                                                      ITypeFacade returnType,
                                                      ITypeFacade elementType,
                                                      IOidStrategy oidStrategy,
                                                      bool useDateOverDateTime) {
            var exts = new Dictionary<string, object> {
                {JsonPropertyNames.FriendlyName, friendlyname},
                {JsonPropertyNames.Description, description}
            };

            if (pluralName != null) {
                exts.Add(JsonPropertyNames.PluralName, pluralName);
            }

            if (domainType != null) {
                exts.Add(JsonPropertyNames.DomainType, domainType);
            }

            if (hasParams != null) {
                exts.Add(JsonPropertyNames.HasParams, hasParams);
            }

            if (isService != null) {
                exts.Add(JsonPropertyNames.IsService, isService);
            }

            if (optional != null) {
                exts.Add(JsonPropertyNames.Optional, optional);
            }

            if (memberOrder != null) {
                exts.Add(JsonPropertyNames.MemberOrder, memberOrder);
            }

            if (dataType != null) {
                exts.Add(JsonPropertyNames.CustomDataType, dataType.ToString().ToLower());
            }

            if (!string.IsNullOrEmpty(presentationHint)) {
                exts.Add(JsonPropertyNames.PresentationHint, presentationHint);
            }


            if (returnType != null && !returnType.IsVoid) {
                Tuple<string, string> jsonDataType = SpecToTypeAndFormatString(returnType, oidStrategy, useDateOverDateTime);
                exts.Add(JsonPropertyNames.ReturnType, jsonDataType.Item1);

                if (jsonDataType.Item2 != null) {
                    exts.Add(JsonPropertyNames.Format, jsonDataType.Item2);
                }

                if (jsonDataType.Item1 == PredefinedJsonType.String.ToRoString()) {
                    exts.Add(JsonPropertyNames.MaxLength, maxLength ?? 0);
                    exts.Add(JsonPropertyNames.Pattern, pattern ?? "");
                }
                // blob and clobs are arrays hence additional checks
                else if (returnType.IsCollection && (jsonDataType.Item1 == PredefinedJsonType.List.ToRoString() || jsonDataType.Item1 == PredefinedJsonType.Set.ToRoString())) {
                    exts.Add(JsonPropertyNames.ElementType, SpecToTypeAndFormatString(elementType, oidStrategy, useDateOverDateTime).Item1);
                    exts.Add(JsonPropertyNames.PluralName, elementType.PluralName);
                }
            }

            if (customExtensions != null) {
                foreach (var kvp in customExtensions) {
                    exts.Add(kvp.Key, kvp.Value);
                }
            }

            return CreateMap(exts);
        }

        public static MapRepresentation CreateMap(Dictionary<string, object> exts) {
            OptionalProperty[] parms = exts.Select(e => new OptionalProperty(e.Key, e.Value)).ToArray();
            return MapRepresentation.Create(parms);
        }

        public static void AddChoices(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            if (propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled && !propertyContext.Property.GetChoicesParameters().Any()) {
                IObjectFacade[] choices = propertyContext.Property.GetChoices(propertyContext.Target, null);
                object[] choicesArray = choices.Select(c => GetChoiceValue(oidStrategy, req, c, propertyContext.Property, flags)).ToArray();
                optionals.Add(new OptionalProperty(JsonPropertyNames.Choices, choicesArray));
            }
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, IObjectFacade item, Func<ChoiceRelType> relType, RestControlFlags flags) {
            string title = SafeGetTitle(item);
            object value = ObjectToPredefinedType(item.Object);
            return item.Specification.IsParseable ? value : LinkRepresentation.Create(oidStrategy, relType(), flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade item, IAssociationFacade property, RestControlFlags flags) {
            return GetChoiceValue(oidStrategy, item, () => new ChoiceRelType(property, new UriMtHelper(oidStrategy, req, item)), flags);
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade item, IActionParameterFacade parameter, RestControlFlags flags) {
            return GetChoiceValue(oidStrategy, item, () => new ChoiceRelType(parameter, new UriMtHelper(oidStrategy, req, item)), flags);
        }

        public static string SafeGetTitle(IObjectFacade no) {
            return no == null ? "" : no.TitleString;
        }

        public static string SafeGetTitle(IAssociationFacade property, IObjectFacade valueNakedObject) {
            return valueNakedObject == null ? "" : property.GetTitle(valueNakedObject);
        }

        private static PredefinedJsonType? TypeToPredefinedJsonType(Type toMapType) {

            if (SimpleTypeMap.ContainsKey(toMapType)) {
                return SimpleTypeMap[toMapType];
            }

            if (toMapType.IsEnum) {
                Type underlyingType = Enum.GetUnderlyingType(toMapType);
                return SimpleTypeMap[underlyingType];
            }

            if (typeof(DateTime).IsAssignableFrom(toMapType) || typeof(TimeSpan).IsAssignableFrom(toMapType)) {
                return PredefinedJsonType.String;
            }

            if (IsGenericType(toMapType, typeof(ISet<>))) {
                return PredefinedJsonType.Set;
            }

            if (IsGenericType(toMapType, typeof(ICollection<>)) || IsGenericType(toMapType, typeof(IQueryable<>))) {
                return PredefinedJsonType.List;
            }

            if (typeof(ICollection).IsAssignableFrom(toMapType) || typeof(IQueryable).IsAssignableFrom(toMapType)) {
                return PredefinedJsonType.List;
            }

            // to catch nof2 InternalCollections 

            if (typeof(IEnumerable).IsAssignableFrom(toMapType) && !toMapType.IsArray) {
                return PredefinedJsonType.List;
            }

            return null;
        }

        public static PredefinedFormatType? TypeToPredefinedFormatType(Type toMapType, bool useDateOverDateTime = false) {
         
            if (SimpleFormatMap.ContainsKey(toMapType)) {
                return SimpleFormatMap[toMapType];
            }

            if (toMapType.IsEnum) {
                Type underlyingType = Enum.GetUnderlyingType(toMapType);
                return SimpleFormatMap[underlyingType];
            }

            if (typeof(DateTime).IsAssignableFrom(toMapType)) {
                return useDateOverDateTime ? PredefinedFormatType.Date : PredefinedFormatType.Date_time;
            }

            if (typeof(TimeSpan).IsAssignableFrom(toMapType)) {
                return  PredefinedFormatType.Time;
            }

            return null;
        }


        public static Tuple<PredefinedJsonType, PredefinedFormatType?> TypeToPredefinedTypes (Type toMapType, bool useDateOverDateTime = false) {
            PredefinedJsonType? pst = TypeToPredefinedJsonType(toMapType);

            if (pst.HasValue) {
                PredefinedFormatType? pft = TypeToPredefinedFormatType(toMapType, useDateOverDateTime);
                return new Tuple<PredefinedJsonType, PredefinedFormatType?>(pst.Value, pft);
            }

            return null;
        }

        private static bool IsGenericType(Type type, Type toMatch) {
            if (type.IsGenericType) {
                return type.GetGenericTypeDefinition() == toMatch || type.GetInterfaces().Any(interfaceType => IsGenericType(interfaceType, toMatch));
            }
            return false;
        }

        public static string ToDateFormatString(DateTime date) {
            return date.Date.ToString("yyyy-MM-dd");
        }

        public static string ToTimeFormatString(TimeSpan time) {
            return time.ToString(@"hh\:mm\:ss");
        }


        public static object ObjectToPredefinedType(object toMap, bool useDateOverDateTime = false) {
            PredefinedFormatType? predefinedFormatType = TypeToPredefinedFormatType(toMap.GetType(), useDateOverDateTime);
            if (predefinedFormatType == PredefinedFormatType.Date_time) {
                var dt = (DateTime) toMap;
                if (dt.Kind == DateTimeKind.Unspecified) {
                    // default datetimes to utc
                    dt = new DateTime(dt.Ticks, DateTimeKind.Utc);
                }

                return dt.ToUniversalTime();
            }
            if (predefinedFormatType == PredefinedFormatType.Date) {
                return ToDateFormatString((DateTime) toMap);
            }

            if (predefinedFormatType == PredefinedFormatType.Time) {
                return ToTimeFormatString((TimeSpan)toMap);
            }

            return predefinedFormatType == PredefinedFormatType.String ? toMap.ToString() : toMap;
        }

        public static Tuple<PredefinedJsonType, PredefinedFormatType?> SpecToPredefinedTypes(ITypeFacade spec, bool useDateOverDateTime = false) {
            if (spec.IsFileAttachment || spec.IsImage) {
                return new Tuple<PredefinedJsonType, PredefinedFormatType?>(PredefinedJsonType.String, PredefinedFormatType.Blob);
            }

            if (spec.IsParseable || spec.IsCollection || spec.IsVoid) {
                Type underlyingType = spec.GetUnderlyingType();
                return TypeToPredefinedTypes(underlyingType, useDateOverDateTime);
            }
            return null;
        }

        public static string SpecToPredefinedTypeString(ITypeFacade spec, IOidStrategy oidStrategy, bool useDateOverDateTime = false) {
            if (!spec.IsVoid) {
                var pdt = SpecToPredefinedTypes(spec);
                return pdt != null ? pdt.Item1.ToRoString() : spec.DomainTypeName(oidStrategy);
            }
            return null;
        }

        public static bool IsPredefined(ITypeFacade spec) {
            var pdts = SpecToPredefinedTypes(spec);
            return pdts != null;
        }

        public static Tuple<string, string> SpecToTypeAndFormatString(ITypeFacade spec, IOidStrategy oidStrategy, bool useDateOverDateTime) {
            var types = SpecToPredefinedTypes(spec, useDateOverDateTime);

            if (types != null) {
                var pdtString = types.Item1.ToRoString();
                var pftString = types.Item2.HasValue ? types.Item2.Value.ToRoString() : null;

                return new Tuple<string, string>(pdtString, pftString);
            }
            return new Tuple<string, string>(spec.DomainTypeName(oidStrategy), null);
        }

        public static string DomainTypeName(this ITypeFacade spec, IOidStrategy oidStrategy) {
            return oidStrategy.GetLinkDomainTypeBySpecification(spec);
        }

        public static bool IsBlobOrClob(ITypeFacade spec) {
            if (spec.IsParseable || spec.IsCollection) {
                Type underlyingType = spec.GetUnderlyingType();
                PredefinedFormatType? pdt = TypeToPredefinedFormatType(underlyingType);
                return pdt == PredefinedFormatType.Blob || pdt == PredefinedFormatType.Clob;
            }
            return false;
        }

        public static bool IsAttachment(ITypeFacade spec) {
            return (spec.IsImage || spec.IsFileAttachment);
        }

        public static bool IsJsonMediaType(string mediaType) {
            return mediaType == "*/*" ||
                   mediaType == "application/*" ||
                   mediaType == "application/json";
        }

        public static OptionalProperty CreateArgumentProperty(IOidStrategy oidStrategy, HttpRequest req, Tuple<string, ITypeFacade> pnt, RestControlFlags flags) {
           
            return new OptionalProperty(pnt.Item1, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object)),
                new OptionalProperty(JsonPropertyNames.Links, new LinkRepresentation[] {})));
        }

        public static string DefaultMimeType(this AttachmentContextFacade attachment) {
            //attempt an intelligent default

            var ext = (attachment == null ? "" : Path.GetExtension(attachment.FileName)) ?? "";

            switch (ext) {
                case ".jpg":
                case ".jpeg":
                    return MediaTypeNames.Image.Jpeg;
                case ".gif":
                    return MediaTypeNames.Image.Gif;
                default:
                    return AttachmentContextFacade.DefaultMimeType;
            }
        }

        public static string GuidAsKey(this Guid guid) {
            return guid.ToString("N");
        }

        public static IDictionary<string, object> AddRangeExtension(IFieldFacade field, IDictionary<string, object> customExtensions ) {
            var range = field.Range;

            if (range != null) {
                customExtensions = customExtensions ?? new Dictionary<string, object>();

                var propertyType = field.Specification.GetUnderlyingType();

                object min;
                object max;

                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)) {
                    var minDays = (double)range.Item1.ToType(typeof(double), null);
                    var maxDays = (double)range.Item2.ToType(typeof(double), null);

                    DateTime earliest = DateTime.Today.AddDays(minDays);
                    DateTime latest = DateTime.Today.AddDays(maxDays);

                    min = ToDateFormatString(earliest);
                    max = ToDateFormatString(latest);
                }
                else {
                    min = range.Item1.ToType(propertyType, null);
                    max = range.Item2.ToType(propertyType, null);
                }

                OptionalProperty[] op = { new OptionalProperty("min", min), new OptionalProperty("max", max) };
                MapRepresentation map = MapRepresentation.Create(op);
                customExtensions[JsonPropertyNames.CustomRange] = map;
            }
            return customExtensions;
        }

        private static LinkRepresentation CreateTableRowValueLink(IObjectFacade no,
                                                                 string[] columns,
                                                                 RelType rt,
                                                                 IOidStrategy oidStrategy,
                                                                 HttpRequest req,
                                                                 RestControlFlags flags) {
            var optionals = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.Title, SafeGetTitle(no))};

            columns = columns ?? no.Specification.Properties.Select(p => p.Id).ToArray();

            var properties = columns.Select( c => no.Specification.Properties.SingleOrDefault(p => p.Id == c)).Where(p => p != null && p.IsVisible(no)).Select(p => new PropertyContextFacade { Property = p, Target = no });

            var propertyReps = properties.Select(p => InlineMemberAbstractRepresentation.Create(oidStrategy, req, p, flags, true)).ToArray();
            var members = CreateMap(propertyReps.ToDictionary(m => m.Id, m => (object)m));

            optionals.Add(new OptionalProperty(JsonPropertyNames.Members, members));

            return LinkRepresentation.Create(oidStrategy,
                                             rt,
                                             flags,
                                             optionals.ToArray());
        }

        public static LinkRepresentation CreateTableRowValueLink(IObjectFacade no,
                                                                 PropertyContextFacade propertyContext,
                                                                 IOidStrategy oidStrategy,
                                                                 HttpRequest req,
                                                                 RestControlFlags flags) {
            var columns = propertyContext.Property.TableViewData?.Item2;
            var rt = new ValueRelType(propertyContext.Property, new UriMtHelper(oidStrategy, req, no));
            return CreateTableRowValueLink(no, columns, rt, oidStrategy, req, flags);
        }

        public static LinkRepresentation CreateTableRowValueLink(IObjectFacade no,
                                                                 ActionContextFacade actionContext,
                                                                 IOidStrategy oidStrategy,
                                                                 HttpRequest req,
                                                                 RestControlFlags flags) {
            var columns = actionContext?.Action?.TableViewData?.Item2;
            var helper = new UriMtHelper(oidStrategy, req, no);
            ObjectRelType rt = no.Specification.IsService ? new ServiceRelType(helper) : new ObjectRelType(RelValues.Element, helper);
            return CreateTableRowValueLink(no, columns, rt, oidStrategy, req, flags);
        }

        public static WarningHeaderValue ToWarningHeaderValue(int code, string warning) {
            const string agent = "RestfulObjects";
            try {
                // remove all \" within warning message as they cause format exception 
                return new WarningHeaderValue(code, agent, "\"" + warning.Replace('"', ' ') + "\"");
            }
            catch (FormatException) {
                //logger.WarnFormat("Failed to parse warning message: {0} : {1}", w, fe.Message);
                return new WarningHeaderValue(code, agent, "\"" + "Failed to parse warning message" + "\"");
            }
        }
    }
}