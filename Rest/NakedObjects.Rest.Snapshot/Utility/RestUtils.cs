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
using Microsoft.Extensions.Logging;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;

namespace NakedObjects.Rest.Snapshot.Utility {
    public static class RestUtils {
        private static readonly Dictionary<Type, PredefinedJsonType> SimpleTypeMap = new Dictionary<Type, PredefinedJsonType> {
            {typeof(sbyte), PredefinedJsonType.Number},
            {typeof(byte), PredefinedJsonType.Number},
            {typeof(short), PredefinedJsonType.Number},
            {typeof(ushort), PredefinedJsonType.Number},
            {typeof(int), PredefinedJsonType.Number},
            {typeof(uint), PredefinedJsonType.Number},
            {typeof(long), PredefinedJsonType.Number},
            {typeof(ulong), PredefinedJsonType.Number},
            {typeof(char), PredefinedJsonType.String},
            {typeof(bool), PredefinedJsonType.Boolean},
            {typeof(string), PredefinedJsonType.String},
            {typeof(float), PredefinedJsonType.Number},
            {typeof(double), PredefinedJsonType.Number},
            {typeof(decimal), PredefinedJsonType.Number},
            {typeof(void), PredefinedJsonType.Void}
        };

        private static readonly Dictionary<Type, PredefinedFormatType> SimpleFormatMap = new Dictionary<Type, PredefinedFormatType> {
            {typeof(sbyte), PredefinedFormatType.Int},
            {typeof(byte), PredefinedFormatType.Int},
            {typeof(short), PredefinedFormatType.Int},
            {typeof(ushort), PredefinedFormatType.Int},
            {typeof(int), PredefinedFormatType.Int},
            {typeof(uint), PredefinedFormatType.Int},
            {typeof(long), PredefinedFormatType.Int},
            {typeof(ulong), PredefinedFormatType.Int},
            {typeof(char), PredefinedFormatType.String},
            {typeof(string), PredefinedFormatType.String},
            {typeof(float), PredefinedFormatType.Decimal},
            {typeof(double), PredefinedFormatType.Decimal},
            {typeof(decimal), PredefinedFormatType.Decimal},
            {typeof(byte[]), PredefinedFormatType.Blob},
            {typeof(sbyte[]), PredefinedFormatType.Blob},
            {typeof(char[]), PredefinedFormatType.Clob}
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
                var (typeString, formatString) = SpecToTypeAndFormatString(returnType, oidStrategy, useDateOverDateTime);
                exts.Add(JsonPropertyNames.ReturnType, typeString);

                if (formatString != null) {
                    exts.Add(JsonPropertyNames.Format, formatString);
                }

                if (typeString == PredefinedJsonType.String.ToRoString()) {
                    exts.Add(JsonPropertyNames.MaxLength, maxLength ?? 0);
                    exts.Add(JsonPropertyNames.Pattern, pattern ?? "");
                }
                // blob and clobs are arrays hence additional checks
                else if (returnType.IsCollection && (typeString == PredefinedJsonType.List.ToRoString() || typeString == PredefinedJsonType.Set.ToRoString())) {
                    exts.Add(JsonPropertyNames.ElementType, SpecToTypeAndFormatString(elementType, oidStrategy, useDateOverDateTime).typeString);
                    exts.Add(JsonPropertyNames.PluralName, elementType.PluralName);
                }
            }

            if (customExtensions != null) {
                foreach (var (key, value) in customExtensions) {
                    exts.Add(key, value);
                }
            }

            return CreateMap(exts);
        }

        public static MapRepresentation CreateMap(Dictionary<string, object> exts) {
            var parms = exts.Select(e => new OptionalProperty(e.Key, e.Value)).ToArray();
            return MapRepresentation.Create(parms);
        }

        public static void AddChoices(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, IList<OptionalProperty> optionals, RestControlFlags flags) {
            if (propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled && !propertyContext.Property.GetChoicesParameters().Any()) {
                var choices = propertyContext.Property.GetChoices(propertyContext.Target, null);
                var choicesArray = choices.Select(c => GetChoiceValue(oidStrategy, req, c, propertyContext.Property, flags)).ToArray();
                optionals.Add(new OptionalProperty(JsonPropertyNames.Choices, choicesArray));
            }
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, IObjectFacade item, Func<ChoiceRelType> relType, RestControlFlags flags) {
            var title = SafeGetTitle(item);
            var value = ObjectToPredefinedType(item.Object);
            return item.Specification.IsParseable ? value : LinkRepresentation.Create(oidStrategy, relType(), flags, new OptionalProperty(JsonPropertyNames.Title, title));
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade item, IAssociationFacade property, RestControlFlags flags) {
            return GetChoiceValue(oidStrategy, item, () => new ChoiceRelType(property, new UriMtHelper(oidStrategy, req, item)), flags);
        }

        public static object GetChoiceValue(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade item, IActionParameterFacade parameter, RestControlFlags flags) {
            return GetChoiceValue(oidStrategy, item, () => new ChoiceRelType(parameter, new UriMtHelper(oidStrategy, req, item)), flags);
        }

        public static string SafeGetTitle(IObjectFacade no) => no == null ? "" : no.TitleString;

        public static string SafeGetTitle(IAssociationFacade property, IObjectFacade valueNakedObject) => valueNakedObject == null ? "" : property.GetTitle(valueNakedObject);

        private static PredefinedJsonType? TypeToPredefinedJsonType(Type toMapType) {
            if (SimpleTypeMap.ContainsKey(toMapType)) {
                return SimpleTypeMap[toMapType];
            }

            if (toMapType.IsEnum) {
                var underlyingType = Enum.GetUnderlyingType(toMapType);
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
                var underlyingType = Enum.GetUnderlyingType(toMapType);
                return SimpleFormatMap[underlyingType];
            }

            if (typeof(DateTime).IsAssignableFrom(toMapType)) {
                return useDateOverDateTime ? PredefinedFormatType.Date : PredefinedFormatType.Date_time;
            }

            if (typeof(TimeSpan).IsAssignableFrom(toMapType)) {
                return PredefinedFormatType.Time;
            }

            return null;
        }

        public static (PredefinedJsonType, PredefinedFormatType?)? TypeToPredefinedTypes(Type toMapType, bool useDateOverDateTime = false) {
            var pst = TypeToPredefinedJsonType(toMapType);

            if (pst.HasValue) {
                var pft = TypeToPredefinedFormatType(toMapType, useDateOverDateTime);
                return (pst.Value, pft);
            }

            return null;
        }

        private static bool IsGenericType(Type type, Type toMatch) {
            if (type.IsGenericType) {
                return type.GetGenericTypeDefinition() == toMatch || type.GetInterfaces().Any(interfaceType => IsGenericType(interfaceType, toMatch));
            }

            return false;
        }

        public static string ToDateFormatString(DateTime date) => date.Date.ToString("yyyy-MM-dd");

        public static string ToTimeFormatString(TimeSpan time) => time.ToString(@"hh\:mm\:ss");

        public static object ObjectToPredefinedType(object toMap, bool useDateOverDateTime = false) {
            static object ToUniversalTime(DateTime dt) => dt.Kind == DateTimeKind.Unspecified
                ? new DateTime(dt.Ticks, DateTimeKind.Utc).ToUniversalTime()
                : dt.ToUniversalTime();

            var predefinedFormatType = TypeToPredefinedFormatType(toMap.GetType(), useDateOverDateTime);
            return predefinedFormatType switch {
                PredefinedFormatType.Date_time => ToUniversalTime((DateTime) toMap),
                PredefinedFormatType.Date => ToDateFormatString((DateTime) toMap),
                PredefinedFormatType.Time => ToTimeFormatString((TimeSpan) toMap),
                _ => predefinedFormatType == PredefinedFormatType.String ? toMap.ToString() : toMap
            };
        }

        public static (PredefinedJsonType pdt, PredefinedFormatType? pft)? SpecToPredefinedTypes(ITypeFacade spec, bool useDateOverDateTime = false) {
            if (spec.IsFileAttachment || spec.IsImage) {
                return (PredefinedJsonType.String, PredefinedFormatType.Blob);
            }

            if (spec.IsParseable || spec.IsCollection || spec.IsVoid) {
                var underlyingType = spec.GetUnderlyingType();
                return TypeToPredefinedTypes(underlyingType, useDateOverDateTime);
            }

            return null;
        }

        public static string SpecToPredefinedTypeString(ITypeFacade spec, IOidStrategy oidStrategy, bool useDateOverDateTime = false) {
            if (!spec.IsVoid) {
                var types = SpecToPredefinedTypes(spec);
                return types != null ? types.Value.pdt.ToRoString() : spec.DomainTypeName(oidStrategy);
            }

            return null;
        }

        public static (string typeString, string formatString) SpecToTypeAndFormatString(ITypeFacade spec, IOidStrategy oidStrategy, bool useDateOverDateTime) {
            var types = SpecToPredefinedTypes(spec, useDateOverDateTime);

            if (types != null) {
                var (pdt, pft) = types.Value;
                var pdtString = pdt.ToRoString();
                var pftString = pft?.ToRoString();

                return (pdtString, pftString);
            }

            return (spec.DomainTypeName(oidStrategy), null);
        }

        public static string DomainTypeName(this ITypeFacade spec, IOidStrategy oidStrategy) => oidStrategy.GetLinkDomainTypeBySpecification(spec);

        public static bool IsBlobOrClob(ITypeFacade spec) {
            if (spec.IsParseable || spec.IsCollection) {
                var underlyingType = spec.GetUnderlyingType();
                var pdt = TypeToPredefinedFormatType(underlyingType);
                return pdt == PredefinedFormatType.Blob || pdt == PredefinedFormatType.Clob;
            }

            return false;
        }

        public static bool IsAttachment(ITypeFacade spec) => spec.IsImage || spec.IsFileAttachment;

        public static bool IsJsonMediaType(string mediaType) =>
            mediaType == "*/*" ||
            mediaType == "application/*" ||
            mediaType == "application/json";

        public static OptionalProperty CreateArgumentProperty(IOidStrategy oidStrategy, HttpRequest req, (string name, ITypeFacade type) pnt, RestControlFlags flags) {
            return new OptionalProperty(pnt.name, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof(object)),
                new OptionalProperty(JsonPropertyNames.Links, new LinkRepresentation[] { })));
        }

        public static string DefaultMimeType(this AttachmentContextFacade attachment) {
            //attempt an intelligent default

            var ext = (attachment == null ? "" : Path.GetExtension(attachment.FileName)) ?? "";

            return ext switch {
                ".jpg" => MediaTypeNames.Image.Jpeg,
                ".jpeg" => MediaTypeNames.Image.Jpeg,
                ".gif" => MediaTypeNames.Image.Gif,
                _ => AttachmentContextFacade.DefaultMimeType
            };
        }

        public static IDictionary<string, object> AddRangeExtension(IFieldFacade field, IDictionary<string, object> customExtensions) {
            var range = field.Range;

            if (range != null) {
                var (minRange, maxRange, _) = range.Value;
                customExtensions ??= new Dictionary<string, object>();

                var propertyType = field.Specification.GetUnderlyingType();

                object min;
                object max;

                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)) {
                    var minDays = (double) minRange.ToType(typeof(double), null);
                    var maxDays = (double) maxRange.ToType(typeof(double), null);

                    var earliest = DateTime.Today.AddDays(minDays);
                    var latest = DateTime.Today.AddDays(maxDays);

                    min = ToDateFormatString(earliest);
                    max = ToDateFormatString(latest);
                }
                else {
                    min = minRange.ToType(propertyType, null);
                    max = maxRange.ToType(propertyType, null);
                }

                OptionalProperty[] op = {new OptionalProperty("min", min), new OptionalProperty("max", max)};
                var map = MapRepresentation.Create(op);
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

            columns ??= no.Specification.Properties.Select(p => p.Id).ToArray();

            var properties = columns.Select(c => no.Specification.Properties.SingleOrDefault(p => p.Id == c)).Where(p => p != null && p.IsVisible(no)).Select(p => new PropertyContextFacade {Property = p, Target = no});

            var propertyReps = properties.Select(p => InlineMemberAbstractRepresentation.Create(oidStrategy, req, p, flags, true)).ToArray();
            var members = CreateMap(propertyReps.ToDictionary(m => m.Id, m => (object) m));

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
            var columns = propertyContext.Property.TableViewData?.columns;
            var rt = new ValueRelType(propertyContext.Property, new UriMtHelper(oidStrategy, req, no));
            return CreateTableRowValueLink(no, columns, rt, oidStrategy, req, flags);
        }

        public static LinkRepresentation CreateTableRowValueLink(IObjectFacade no,
                                                                 ActionContextFacade actionContext,
                                                                 IOidStrategy oidStrategy,
                                                                 HttpRequest req,
                                                                 RestControlFlags flags) {
            var columns = actionContext?.Action?.TableViewData?.columns;
            var helper = new UriMtHelper(oidStrategy, req, no);
            var rt = no.Specification.IsService ? new ServiceRelType(helper) : new ObjectRelType(RelValues.Element, helper);
            return CreateTableRowValueLink(no, columns, rt, oidStrategy, req, flags);
        }

        private static string CleanWarning(string warning) => warning.Replace('"', ' ').Replace('\r', ' ').Replace('\n', ' ');

        public static WarningHeaderValue ToWarningHeaderValue(int code, string warning, ILogger logger) {
            const string agent = "RestfulObjects";
            try {
                // remove all \" within warning message as they cause format exception 
                return new WarningHeaderValue(code, agent, $"\"{CleanWarning(warning)}\"");
            }
            catch (Exception e) {
                logger.LogWarning(e, $"Failed to parse warning message: {warning}");
                return new WarningHeaderValue(code, agent, $"\"" + "Failed to parse warning message" + "\"");
            }
        }
    }
}