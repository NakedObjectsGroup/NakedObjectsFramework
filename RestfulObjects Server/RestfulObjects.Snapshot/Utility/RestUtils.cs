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
using System.Net.Http;
using System.Net.Mime;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Utility.Restricted;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;

namespace RestfulObjects.Snapshot.Utility {
    public static class RestUtils {
        private static readonly Dictionary<Type, PredefinedType> SimpleTypeMap = new Dictionary<Type, PredefinedType> {
            {typeof (sbyte), PredefinedType.Integer},
            {typeof (byte), PredefinedType.Integer},
            {typeof (short), PredefinedType.Integer},
            {typeof (ushort), PredefinedType.Integer},
            {typeof (int), PredefinedType.Integer},
            {typeof (uint), PredefinedType.Integer},
            {typeof (long), PredefinedType.Integer},
            {typeof (ulong), PredefinedType.Integer},
            {typeof (char), PredefinedType.String},
            {typeof (bool), PredefinedType.Boolean},
            {typeof (string), PredefinedType.String},
            {typeof (float), PredefinedType.Number},
            {typeof (double), PredefinedType.Number},
            {typeof (decimal), PredefinedType.Number},
            {typeof (byte[]), PredefinedType.Blob},
            {typeof (sbyte[]), PredefinedType.Blob},
            {typeof (char[]), PredefinedType.Clob},
            {typeof (void), PredefinedType.Void}
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

                // blob and clobs are arrays so do this check first so they are not caught be the collection test after. 
                if (jsonDataType.Item1 == PredefinedType.Number.ToRoString()) {
                    exts.Add(JsonPropertyNames.Format, jsonDataType.Item2);
                }
                else if (jsonDataType.Item1 == PredefinedType.String.ToRoString()) {
                    exts.Add(JsonPropertyNames.Format, jsonDataType.Item2);
                    exts.Add(JsonPropertyNames.MaxLength, maxLength ?? 0);
                    exts.Add(JsonPropertyNames.Pattern, pattern ?? "");
                }
                else if (returnType.IsCollection) {
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

        public static void AddChoices(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            if (propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled && !propertyContext.Property.GetChoicesParameters().Any()) {
                IObjectFacade[] choices = propertyContext.Property.GetChoices(propertyContext.Target, null);
                object[] choicesArray = choices.Select(c => GetChoiceValue(oidStrategy, req, c, propertyContext.Property, flags)).ToArray();
                optionals.Add(new OptionalProperty(JsonPropertyNames.Choices, choicesArray));
            }
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, IObjectFacade item, ChoiceRelType relType, RestControlFlags flags) {
            string title = SafeGetTitle(item);
            object value = ObjectToPredefinedType(item.GetDomainObject());
            return item.Specification.IsParseable ? value : LinkRepresentation.Create(oidStrategy, relType, flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, HttpRequestMessage req, IObjectFacade item, IAssociationFacade property, RestControlFlags flags) {
            return GetChoiceValue(oidStrategy, item, new ChoiceRelType(property, new UriMtHelper(oidStrategy, req, item)), flags);
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, HttpRequestMessage req, IObjectFacade item, IActionParameterFacade parameter, RestControlFlags flags) {
            return GetChoiceValue(oidStrategy, item, new ChoiceRelType(parameter, new UriMtHelper(oidStrategy, req, item)), flags);
        }

        public static string SafeGetTitle(IObjectFacade no) {
            return no == null ? "" : no.TitleString;
        }

        public static string SafeGetTitle(IAssociationFacade property, IObjectFacade valueNakedObject) {
            return valueNakedObject == null ? "" : property.GetTitle(valueNakedObject);
        }

        public static PredefinedType TypeToPredefinedType(Type toMapType, bool useDateOverDateTime = false) {
            if (SimpleTypeMap.ContainsKey(toMapType)) {
                return SimpleTypeMap[toMapType];
            }

            if (toMapType.IsEnum) {
                Type underlyingType = Enum.GetUnderlyingType(toMapType);
                return SimpleTypeMap[underlyingType];
            }

            if (typeof (DateTime).IsAssignableFrom(toMapType)) {
                return useDateOverDateTime ?  PredefinedType.Date : PredefinedType.Date_time;
            }

            if (IsGenericType(toMapType, typeof (ISet<>))) {
                return PredefinedType.Set;
            }

            if (IsGenericType(toMapType, typeof (ICollection<>)) || IsGenericType(toMapType, typeof (IQueryable<>))) {
                return PredefinedType.List;
            }

            if (typeof (ICollection).IsAssignableFrom(toMapType) || typeof (IQueryable).IsAssignableFrom(toMapType)) {
                return PredefinedType.List;
            }

            // to catch nof2 InternalCollections 

            if (typeof (IEnumerable).IsAssignableFrom(toMapType) && !toMapType.IsArray) {
                return PredefinedType.List;
            }

            return PredefinedType.String;
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

        public static object ObjectToPredefinedType(object toMap, bool useDateOverDateTime = false) {
            PredefinedType predefinedType = TypeToPredefinedType(toMap.GetType(), useDateOverDateTime);
            if (predefinedType == PredefinedType.Date_time) {
                var dt = (DateTime) toMap;
                if (dt.Kind == DateTimeKind.Unspecified) {
                    // default datetimes to utc
                    dt = new DateTime(dt.Ticks, DateTimeKind.Utc);
                }

                return dt.ToUniversalTime();
            }
            if (predefinedType == PredefinedType.Date) {
                return ToDateFormatString((DateTime) toMap);
            }

            return predefinedType == PredefinedType.String ? toMap.ToString() : toMap;
        }

        public static PredefinedType? SpecToPredefinedType(ITypeFacade spec, bool useDateOverDateTime = false) {
            if (spec.IsFileAttachment || spec.IsImage) {
                return PredefinedType.Blob;
            }

            if (spec.IsParseable || spec.IsCollection || spec.IsVoid) {
                Type underlyingType = spec.GetUnderlyingType();
                return TypeToPredefinedType(underlyingType, useDateOverDateTime);
            }
            return null;
        }

        public static string SpecToPredefinedTypeString(ITypeFacade spec, IOidStrategy oidStrategy, bool useDateOverDateTime = false) {
            PredefinedType? pdt = SpecToPredefinedType(spec, useDateOverDateTime);
            return pdt.HasValue ? pdt.Value.ToRoString() : spec.DomainTypeName(oidStrategy);
        }

        public static bool IsPredefined(ITypeFacade spec) {
            PredefinedType? pdt = SpecToPredefinedType(spec);
            return pdt.HasValue;
        }

        public static Tuple<string, string> SpecToTypeAndFormatString(ITypeFacade spec, IOidStrategy oidStrategy, bool useDateOverDateTime) {
            PredefinedType? pdt = SpecToPredefinedType(spec, useDateOverDateTime);

            if (pdt.HasValue) {
                switch (pdt.Value) {
                    case PredefinedType.Number:
                        return new Tuple<string, string>(pdt.Value.ToRoString(), PredefinedType.Decimal.ToRoString());
                    case PredefinedType.Integer:
                        return new Tuple<string, string>(PredefinedType.Number.ToRoString(), pdt.Value.ToRoString());
                    case PredefinedType.Boolean:
                        return new Tuple<string, string>(pdt.Value.ToRoString(), null);
                    case PredefinedType.List:
                        return new Tuple<string, string>(PredefinedType.List.ToRoString(), null);
                    case PredefinedType.Set:
                        return new Tuple<string, string>(PredefinedType.Set.ToRoString(), null);
                    case PredefinedType.String:
                        return new Tuple<string, string>(pdt.Value.ToRoString(), pdt.Value.ToRoString());
                    case PredefinedType.Void:
                        return new Tuple<string, string>(null, null);
                    default:
                        return new Tuple<string, string>(PredefinedType.String.ToRoString(), pdt.Value.ToRoString());
                }
            }
            return new Tuple<string, string>(spec.DomainTypeName(oidStrategy), null);
        }

        public static string DomainTypeName(this ITypeFacade spec, IOidStrategy oidStrategy) {
            return oidStrategy.GetLinkDomainTypeBySpecification(spec);
        }

        public static bool IsBlobOrClob(ITypeFacade spec) {
            if (spec.IsParseable || spec.IsCollection) {
                Type underlyingType = spec.GetUnderlyingType();
                PredefinedType pdt = TypeToPredefinedType(underlyingType);
                return pdt == PredefinedType.Blob || pdt == PredefinedType.Clob;
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

        public static OptionalProperty CreateArgumentProperty(IOidStrategy oidStrategy, HttpRequestMessage req, Tuple<string, ITypeFacade> pnt, RestControlFlags flags) {
            var tempLinks = new List<LinkRepresentation>();

            return new OptionalProperty(pnt.Item1, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object)),
                new OptionalProperty(JsonPropertyNames.Links, tempLinks.ToArray())));
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




    }
}