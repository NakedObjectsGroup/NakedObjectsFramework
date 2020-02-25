// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module DomainObject14

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open System.Net.Http.Headers
open System
open NakedObjects.Rest.Snapshot.Utility
open NakedObjects.Rest.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions
open Newtonsoft.Json.Linq
open Microsoft.AspNetCore.Http.Headers

let GetMostSimpleObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType) 
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))                           
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetMostSimpleObjectWithDetailsFlag(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    api.InlinePropertyDetails <- new Nullable<bool>(false)
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType) 
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))                           
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeNoDetailsPropertyMember "Id" oName (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult


let GetWithAttachmentsObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAttachments"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let arguments = 
        TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType)) 
                             
                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty
                                      ("FileAttachment", 
                                       TObjectJson(makePropertyMemberFullAttachment "FileAttachment" oName "File Attachment" "afile" "application/pdf"))
                                  TProperty("Image", TObjectJson(makePropertyMemberFullAttachment "Image" oName "Image" "animage" "image/jpeg"))
                                  TProperty("ImageWithDefault", TObjectJson(makePropertyMemberFullAttachment "ImageWithDefault" oName "Image With Default" "animage.gif" "image/gif"))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Attachments"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Attachmentses"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetMostSimpleObjectConfiguredSelectable(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid


    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)


    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetMostSimpleObjectSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    api.DomainModel <- "simple"
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               TObjectJson([ TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oName "Id" "" "int" false (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetMostSimpleObjectConfiguredSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               TObjectJson([ TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oName "Id" "" "int" false (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetMostSimpleObjectConfiguredCaching(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    RestfulObjectsControllerBase.CacheSettings <- (2, 100, 200)
   
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertConfigCache 2 headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

//let GetWithDateTimeKeyObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithDateTimeKey"
//    let k = 634835232000000000L
//    let id = ktc (Convert.ToString(k))
//    let oid = oType + "/" + id
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, id, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
//    let dt = (new DateTime(k, DateTimeKind.Utc)).ToUniversalTime()
//    let title = dt.ToString()
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(id))
//          TProperty(JsonPropertyNames.Title, TObjectVal(title))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
               
//               TObjectJson
//                   ([ TProperty
//                          ("Id", 
                           
//                           TObjectJson
//                               (makePropertyMemberDateTime "objects" "Id" oid "Id" "" false (TObjectVal( DateTime.Parse("2012-09-18 00:00:00.000"))) "date-time" )) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Date Time Key"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Date Time Keies"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithGuidKeyObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithGuidKey"
//    let key = "CA761232-ED42-11CE-BACD-00AA0057B223".ToLower()
//    let id = ktc (key)
//    let oid = oType + "/" + id
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, id, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
   
//    let title = key
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(id))
//          TProperty(JsonPropertyNames.Title, TObjectVal(title))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
               
//               TObjectJson
//                   ([ TProperty
//                          ("Id", 
                           
//                           TObjectJson
//                               (makePropertyMemberGuid "objects" oid "Id" (TObjectVal(key)) (ttc "System.Guid") )) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Guid Key"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Guid Keies"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult


//let GetVerySimpleEagerObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.VerySimpleEager"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let pid = "MostSimple"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property pid
//    let clearRel = RelValues.Clear + makeParm RelParamValues.Property pid
    
//    let msDetails = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(null))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                             
                             
                             
//                             TObjectJson
//                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
//                             TObjectJson(makeDeleteLinkProp clearRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ]
    
//    let pid = "SimpleList"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/collections/%s" ourl pid
    
//    let slDetails = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Simple List"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.Value, TArray([]))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                      ])) ]
    
//    let pid = "SimpleSet"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/collections/%s" ourl pid
    
//    let ssDetails = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Simple Set"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.Value, TArray([]))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                       ])) ]
    
//    let pid = "Name"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let clearRel = RelValues.Clear + makeParm RelParamValues.Property pid
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property pid
    
//    let sDetails = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(null))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                             
                             
                             
//                             TObjectJson
//                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
//                             TObjectJson(makeDeleteLinkProp clearRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Name"))
//                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(101))
//                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(@"[A-Z]"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ]
    
//    let args = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("MostSimple", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Name", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("Untitled Very Simple Eager"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty
//                                      ("SimpleList", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMemberTypeValue "SimpleList" oid "Simple List" "" "list" 0 roType "Most Simples" (TArray([])) 
//                                                slDetails))
                                  
//                                  TProperty
//                                      ("SimpleSet", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMemberTypeValue "SimpleSet" oid "Simple Set" "" "set" 0 roType "Most Simples" (TArray([])) ssDetails))
                                  
//                                  TProperty
//                                      ("MostSimple", 
//                                       TObjectJson(makePropertyMemberShort "objects" "MostSimple" oid "Most Simple" "" roType true (TObjectVal(null)) msDetails))
//                                  TProperty("Name", TObjectJson(makePropertyMemberString "objects" "Name" oid "Name" "" true (TObjectVal(null)) sDetails)) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Very Simple Eager"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Very Simple Eagers"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithValueObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let disabledValue = 
//        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//        :: (makeObjectPropertyMember "ADisabledValue" oid "A Disabled Value" (TObjectVal(200)))
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oid "A Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("AConditionalChoicesValue", 
//                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oid "A Conditional Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("ADateTimeValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oid "A Date Time Value" "A datetime value for testing" true 
//                                                (TObjectVal("2012-02-10")) "date"))

//                                  TProperty
//                                      ("ATimeSpanValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oid "A Time Span Value" "A timespan value for testing" true 
//                                                (TObjectVal("02:03:04")) "time"))

//                                  TProperty("ADisabledValue", TObjectJson(disabledValue))
                                  
//                                  TProperty
//                                      ("AStringValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberString "objects" "AStringValue" oid "A String Value" "A string value for testing" true 
//                                                (TObjectVal("")) []))
                                  
//                                  TProperty
//                                      ("AUserDisabledValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
//                                            :: makeObjectPropertyMember "AUserDisabledValue" oid "A User Disabled Value" (TObjectVal(0))))
//                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oid "A Value" (TObjectVal(100))))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithScalarsObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let emptyValue = TArray([])

//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("Bool", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Byte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Char", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                //TProperty("CharArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
//                                TProperty("Decimal", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("DateTime", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Double", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("EnumByAttributeChoices", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Float", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Int", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("IntWithRange", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Long", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Password", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("SByte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                //TProperty("SByteArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
//                                TProperty("Short", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("String", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("UInt", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ULong", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("UShort", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    //let dt = DateTime.Parse("2012-03-27T08:42:36Z").ToUniversalTime()
//    let dt = "2012-03-27"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
               
//               TObjectJson
//                   ([ TProperty("Bool", TObjectJson(makePropertyMemberWithType "objects" "Bool" oid "Bool" "" "boolean" false (TObjectVal(true))))
//                      TProperty("Byte", TObjectJson(makePropertyMemberWithNumber "objects" "Byte" oid "Byte" "" "int" false (TObjectVal(1))))
//                      //TProperty("ByteArray", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) :: makePropertyMemberWithTypeNoValue "objects"  "ByteArray" oid "Byte Array" "" "blob"  false)) ;
//                      TProperty("Char", TObjectJson(makePropertyMemberWithFormat "objects" "Char" oid "Char" "" "string" false (TObjectVal("3"))))
//                      //TProperty("CharArray", TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "CharArray" oid "Char Array" "" "clob"  false)) ;
//                      TProperty("Decimal", TObjectJson(makePropertyMemberWithNumber "objects" "Decimal" oid "Decimal" "" "decimal" false (TObjectVal(5.1))))
                      
//                      TProperty
//                          ("DateTime", TObjectJson(makePropertyMemberWithFormat "objects" "DateTime" oid "Date Time" "" "date" false (TObjectVal(dt))))
//                      TProperty("Double", TObjectJson(makePropertyMemberWithNumber "objects" "Double" oid "Double" "" "decimal" false (TObjectVal(6.2))))
                      
//                      TProperty
//                          ("EnumByAttributeChoices", 
                           
//                           TObjectJson
//                               (makePropertyMemberWithNumber "objects" "EnumByAttributeChoices" oid "Enum By Attribute Choices" "" "int" false 
//                                    (TObjectVal(0))))
//                      TProperty("Float", TObjectJson(makePropertyMemberWithNumber "objects" "Float" oid "Float" "" "decimal" false (TObjectVal(7.3))))
//                      TProperty("Id", TObjectJson(makePropertyMemberWithNumber "objects" "Id" oid "Id" "" "int" false (TObjectVal(1))))
//                      TProperty("Int", TObjectJson(makePropertyMemberWithNumber "objects" "Int" oid "Int" "" "int" false (TObjectVal(8))))
//                      TProperty("IntWithRange", TObjectJson(makePropertyMemberWithNumber "objects" "IntWithRange" oid "Int With Range" "" "int" false (TObjectVal(0))))
//                      TProperty("List", TObjectJson(makeCollectionMember "List" oid "List" "" "list" 0 emptyValue))
//                      TProperty("Long", TObjectJson(makePropertyMemberWithNumber "objects" "Long" oid "Long" "" "int" false (TObjectVal(9))))
//                      TProperty("Password", TObjectJson(makePropertyMemberWithFormat "objects" "Password" oid "Password" "" "string" false (TObjectVal(null))))
//                      TProperty("SByte", TObjectJson(makePropertyMemberWithNumber "objects" "SByte" oid "S Byte" "" "int" false (TObjectVal(10))))
//                      //TProperty("SByteArray",TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "SByteArray" oid "S Byte Array" "" "blob"  false)) ;
//                      TProperty("Set", TObjectJson(makeCollectionMember "Set" oid "Set" "" "set" 0 emptyValue))
//                      TProperty("Short", TObjectJson(makePropertyMemberWithNumber "objects" "Short" oid "Short" "" "int" false (TObjectVal(12))))
//                      TProperty("String", TObjectJson(makePropertyMemberWithFormat "objects" "String" oid "String" "" "string" false (TObjectVal("13"))))
//                      TProperty("UInt", TObjectJson(makePropertyMemberWithNumber "objects" "UInt" oid "U Int" "" "int" false (TObjectVal(14))))
//                      TProperty("ULong", TObjectJson(makePropertyMemberWithNumber "objects" "ULong" oid "U Long" "" "int" false (TObjectVal(15))))
//                      TProperty("UShort", TObjectJson(makePropertyMemberWithNumber "objects" "UShort" oid "U Short" "" "int" false (TObjectVal(16)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Scalars"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Scalarses"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithValueObjectUserAuth(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let disabledValue = 
//        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//        :: (makeObjectPropertyMember "ADisabledValue" oid "A Disabled Value" (TObjectVal(200)))
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AUserHiddenValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oid "A Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("AConditionalChoicesValue", 
//                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oid "A Conditional Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("ADateTimeValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oid "A Date Time Value" "A datetime value for testing" true 
//                                                (TObjectVal("2012-02-10")) "date"))

//                                  TProperty
//                                      ("ATimeSpanValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oid "A Time Span Value" "A timespan value for testing" true 
//                                                (TObjectVal("02:03:04")) "time"))

//                                  TProperty("ADisabledValue", TObjectJson(disabledValue))
                                  
//                                  TProperty
//                                      ("AStringValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberString "objects" "AStringValue" oid "A String Value" "A string value for testing" true 
//                                                (TObjectVal("")) []))
                                  
//                                  TProperty
//                                      ("AUserDisabledValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
//                                            :: makeObjectPropertyMember "AUserDisabledValue" oid "A User Disabled Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("AUserHiddenValue", TObjectJson(makeObjectPropertyMember "AUserHiddenValue" oid "A User Hidden Value" (TObjectVal(0))))
//                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oid "A Value" (TObjectVal(100))))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithValueObjectWithMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let msg = jsonGetMsg (url)
//    let headers = new RequestHeaders(msg.Headers)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.Object)))
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let disabledValue = 
//        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//        :: (makeObjectPropertyMember "ADisabledValue" oid "A Disabled Value" (TObjectVal(200)))
    
//    let args = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oid "A Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("AConditionalChoicesValue", 
//                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oid "A Conditional Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("ADateTimeValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oid "A Date Time Value" "A datetime value for testing" true 
//                                                (TObjectVal("2012-02-10")) "date"))

//                                  TProperty
//                                      ("ATimeSpanValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oid "A Time Span Value" "A timespan value for testing" true 
//                                                (TObjectVal("02:03:04")) "time"))

//                                  TProperty("ADisabledValue", TObjectJson(disabledValue))
                                  
//                                  TProperty
//                                      ("AStringValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberString "objects" "AStringValue" oid "A String Value" "A string value for testing" true 
//                                                (TObjectVal("")) []))
                                  
//                                  TProperty
//                                      ("AUserDisabledValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
//                                            :: makeObjectPropertyMember "AUserDisabledValue" oid "A User Disabled Value" (TObjectVal(0))))
//                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oid "A Value" (TObjectVal(100))))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetMostSimpleObjectWithDomainTypeSimple(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let msg = jsonGetMsg (url)
//    let headers = new RequestHeaders(msg.Headers)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.Object)))
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("x-ro-domain-type", "\"" + oType + "\""))
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
//               TObjectJson([ TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oid "Id" "" "int" false (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult



//let GetWithValueObjectWithDomainTypeNoProfileSimple(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let msg = jsonGetMsg (url)
//    let headers = new RequestHeaders(msg.Headers)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("x-ro-domain-type", "\"RestfulObjects.Test.Data.MostSimple\""))
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
//               TObjectJson([ TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oid "Id" "" "int" false (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetRedirectedObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.RedirectedObject"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    try 
//        let args = CreateReservedArgs ""
//        api.Request <- jsonGetMsg (url)
//        api.GetObject(oType, ktc "1", args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> 
//        Assert.AreEqual(HttpStatusCode.MovedPermanently, ex.Response.StatusCode)
//        Assert.AreEqual(new Uri("http://redirectedtoserver/objects/RedirectedToOid"), ex.Response.Headers.Location)

//let PutWithValueObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))),
//                    new JProperty("ATimeSpanValue", new JObject(new JProperty(JsonPropertyNames.Value, "04:05:06"))),  
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let disabledValue = 
//        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//        :: (makeObjectPropertyMember "ADisabledValue" oid "A Disabled Value" (TObjectVal(200)))
    
//    let args = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oid "A Choices Value" (TObjectVal(333))))
                                  
//                                  TProperty
//                                      ("AConditionalChoicesValue", 
//                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oid "A Conditional Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("ADateTimeValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oid "A Date Time Value" "A datetime value for testing" true 
//                                                (TObjectVal("2012-02-10")) "date"))

//                                  TProperty
//                                      ("ATimeSpanValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oid "A Time Span Value" "A timespan value for testing" true 
//                                                (TObjectVal("04:05:06")) "time"))

//                                  TProperty("ADisabledValue", TObjectJson(disabledValue))
                                  
//                                  TProperty
//                                      ("AStringValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberString "objects" "AStringValue" oid "A String Value" "A string value for testing" true 
//                                                (TObjectVal("")) []))
                                  
//                                  TProperty
//                                      ("AUserDisabledValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
//                                            :: makeObjectPropertyMember "AUserDisabledValue" oid "A User Disabled Value" (TObjectVal(0))))
//                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oid "A Value" (TObjectVal(222))))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithValueObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

//let PutWithValueObjectConcurrencySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 444))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 555))))
//    //RestfulObjectsControllerBase.ConcurrencyChecking <- true
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let tag = result.Headers.ETag.Tag
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsgAndTag url (props.ToString()) tag
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let disabledValue = 
//        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//        :: (makeObjectPropertyMember "ADisabledValue" oid "A Disabled Value" (TObjectVal(200)))
    
//    let args = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oid "A Choices Value" (TObjectVal(555))))
                                  
//                                  TProperty
//                                      ("AConditionalChoicesValue", 
//                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oid "A Conditional Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("ADateTimeValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oid "A Date Time Value" "A datetime value for testing" true 
//                                                (TObjectVal("2012-02-10")) "date"))

//                                  TProperty
//                                      ("ATimeSpanValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oid "A Time Span Value" "A timespan value for testing" true 
//                                                (TObjectVal("02:03:04")) "time"))

//                                  TProperty("ADisabledValue", TObjectJson(disabledValue))
                                  
//                                  TProperty
//                                      ("AStringValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberString "objects" "AStringValue" oid "A String Value" "A string value for testing" true 
//                                                (TObjectVal("")) []))
                                  
//                                  TProperty
//                                      ("AUserDisabledValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
//                                            :: makeObjectPropertyMember "AUserDisabledValue" oid "A User Disabled Value" (TObjectVal(0))))
//                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oid "A Value" (TObjectVal(444))))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithScalarsObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("Bool", new JObject(new JProperty(JsonPropertyNames.Value, false))), 
//                    new JProperty("Byte", new JObject(new JProperty(JsonPropertyNames.Value, 2))), 
//                    new JProperty("Char", new JObject(new JProperty(JsonPropertyNames.Value, "A"))), 
//                    new JProperty("Decimal", new JObject(new JProperty(JsonPropertyNames.Value, 100.9))), 
//                    new JProperty("DateTime", new JObject(new JProperty(JsonPropertyNames.Value, "2011-12-25T12:13:14Z"))), 
//                    new JProperty("Double", new JObject(new JProperty(JsonPropertyNames.Value, 200.8))), 
//                    new JProperty("EnumByAttributeChoices", new JObject(new JProperty(JsonPropertyNames.Value, 1))), 
//                    new JProperty("Float", new JObject(new JProperty(JsonPropertyNames.Value, 300.7))), 
//                    new JProperty("Int", new JObject(new JProperty(JsonPropertyNames.Value, 400))),
//                    new JProperty("IntWithRange", new JObject(new JProperty(JsonPropertyNames.Value, 400))), 
//                    new JProperty("Long", new JObject(new JProperty(JsonPropertyNames.Value, 500))), 
//                    new JProperty("SByte", new JObject(new JProperty(JsonPropertyNames.Value, 3))), 
//                    new JProperty("Short", new JObject(new JProperty(JsonPropertyNames.Value, 4))), 
//                    new JProperty("String", new JObject(new JProperty(JsonPropertyNames.Value, "44"))), 
//                    new JProperty("UInt", new JObject(new JProperty(JsonPropertyNames.Value, 5))), 
//                    new JProperty("ULong", new JObject(new JProperty(JsonPropertyNames.Value, 6))), 
//                    new JProperty("UShort", new JObject(new JProperty(JsonPropertyNames.Value, 7))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let emptyValue = TArray([])

//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("Bool", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Byte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Char", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                //TProperty("CharArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
//                                TProperty("Decimal", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("DateTime", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Double", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("EnumByAttributeChoices", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Float", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Int", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("IntWithRange", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Long", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Password", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("SByte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                //TProperty("SByteArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
//                                TProperty("Short", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("String", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("UInt", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ULong", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("UShort", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
               
//               TObjectJson
//                   ([ TProperty("Bool", TObjectJson(makePropertyMemberWithType "objects" "Bool" oid "Bool" "" "boolean" false (TObjectVal(false))))
//                      TProperty("Byte", TObjectJson(makePropertyMemberWithNumber "objects" "Byte" oid "Byte" "" "int" false (TObjectVal(2))))
//                      //TProperty("ByteArray", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) :: makePropertyMemberWithTypeNoValue "objects"  "ByteArray" oid "Byte Array" "" "blob"  false)) ;
//                      TProperty("Char", TObjectJson(makePropertyMemberWithFormat "objects" "Char" oid "Char" "" "string" false (TObjectVal("3"))))
//                      //TProperty("CharArray", TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "CharArray" oid "Char Array" "" "clob"  false)) ;
//                      TProperty("Decimal", TObjectJson(makePropertyMemberWithNumber "objects" "Decimal" oid "Decimal" "" "decimal" false (TObjectVal(100.9))))
                      
//                      TProperty
//                          ("DateTime", 
                           
//                           TObjectJson
//                               (makePropertyMemberWithFormat "objects" "DateTime" oid "Date Time" "" "date" false 
//                                    (TObjectVal("2011-12-25"))))
//                      TProperty("Double", TObjectJson(makePropertyMemberWithNumber "objects" "Double" oid "Double" "" "decimal" false (TObjectVal(200.8))))
                      
//                      TProperty
//                          ("EnumByAttributeChoices", 
                           
//                           TObjectJson
//                               (makePropertyMemberWithNumber "objects" "EnumByAttributeChoices" oid "Enum By Attribute Choices" "" "int" false 
//                                    (TObjectVal(1))))
//                      TProperty("Float", TObjectJson(makePropertyMemberWithNumber "objects" "Float" oid "Float" "" "decimal" false (TObjectVal(300.7))))
//                      TProperty("Id", TObjectJson(makePropertyMemberWithNumber "objects" "Id" oid "Id" "" "int" false (TObjectVal(1))))
//                      TProperty("Int", TObjectJson(makePropertyMemberWithNumber "objects" "Int" oid "Int" "" "int" false (TObjectVal(400))))
//                      TProperty("IntWithRange", TObjectJson(makePropertyMemberWithNumber "objects" "IntWithRange" oid "Int With Range" "" "int" false (TObjectVal(400))))
//                      TProperty("List", TObjectJson(makeCollectionMember "List" oid "List" "" "list" 0 emptyValue))
//                      TProperty("Long", TObjectJson(makePropertyMemberWithNumber "objects" "Long" oid "Long" "" "int" false (TObjectVal(500))))
//                      TProperty("Password", TObjectJson(makePropertyMemberWithFormat "objects" "Password" oid "Password" "" "string" false (TObjectVal(null))))
//                      TProperty("SByte", TObjectJson(makePropertyMemberWithNumber "objects" "SByte" oid "S Byte" "" "int" false (TObjectVal(3))))
//                      //TProperty("SByteArray",TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "SByteArray" oid "S Byte Array" "" "blob"  false)) ;
//                      TProperty("Set", TObjectJson(makeCollectionMember "Set" oid "Set" "" "set" 0 emptyValue))
//                      TProperty("Short", TObjectJson(makePropertyMemberWithNumber "objects" "Short" oid "Short" "" "int" false (TObjectVal(4))))
//                      TProperty("String", TObjectJson(makePropertyMemberWithFormat "objects" "String" oid "String" "" "string" false (TObjectVal("44"))))
//                      TProperty("UInt", TObjectJson(makePropertyMemberWithNumber "objects" "UInt" oid "U Int" "" "int" false (TObjectVal(5))))
//                      TProperty("ULong", TObjectJson(makePropertyMemberWithNumber "objects" "ULong" oid "U Long" "" "int" false (TObjectVal(6))))
//                      TProperty("UShort", TObjectJson(makePropertyMemberWithNumber "objects" "UShort" oid "U Short" "" "int" false (TObjectVal(7)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Scalars"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Scalarses"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithReferenceObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let roid = roType + "/" + ktc "2"
//    let pid = "AnEagerReference"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AnEagerReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AnAutoCompleteReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args1 = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let msObj = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "2"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("2"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
//                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
//                             TObjectJson(args1 :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
//          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(2)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
//    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
//    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
//    let valueRel5 = RelValues.Value + makeParm RelParamValues.Property "AnAutoCompleteReference"
//    let val1 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val2 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType)
//    let val3 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType)
//    let val4 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType (TObjectJson(msObj)))
//    let val5 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeGetLinkProp valueRel5 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnEagerReference"
    
//    let details = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal("AnEagerReference"))
//          TProperty(JsonPropertyNames.Value, val4)
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                      
                      
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
//    let args = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AConditionalChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ANullReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AnAutoCompleteReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AnEagerReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))  
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty
//                                      ("AChoicesReference", 
//                                       TObjectJson(makePropertyMemberShort "objects" "AChoicesReference" oid "A Choices Reference" "" roType false val2 []))
                                  
//                                  TProperty
//                                      ("AConditionalChoicesReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "AConditionalChoicesReference" oid "A Conditional Choices Reference" "" roType 
//                                                false (TObjectVal(null)) []))
                                  
//                                  TProperty
//                                      ("ADisabledReference", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//                                            :: (makePropertyMemberShort "objects" "ADisabledReference" oid "A Disabled Reference" "" roType false val1 [])))
                                  
//                                  TProperty
//                                      ("ANullReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "ANullReference" oid "A Null Reference" "" roType true (TObjectVal(null)) []))
//                                  TProperty("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" oid "A Reference" "" roType false val3 []))
                                  
//                                  TProperty
//                                      ("AnAutoCompleteReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "AnAutoCompleteReference" oid "An Auto Complete Reference" "" roType false val5 []))
                                  
//                                  TProperty
//                                      ("AnEagerReference", 
//                                       TObjectJson(makePropertyMemberShort "objects" "AnEagerReference" oid "An Eager Reference" "" roType false val4 details))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With References"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithReferenceObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

//let GetWithActionObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
//    let mp r n = sprintf ";%s=\"%s\"" r n
    
//    let makeParm pmid pid fid rt = 
       
        
//        let p = 
//            TObjectJson([ TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithAC pmid pid fid rt = 
        
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
//        let argP = 
//            TProperty
//                (JsonPropertyNames.Arguments, 
//                 TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
//        let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(3)) ]))
//        let ac = TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, 
//                                    TArray([ 
//                                             ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithChoicesAndDefault pmid pid fid rt = 
      
//        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let choice1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let choice2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
//        let obj1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
//                          TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectJson(choice1)
//                                             TObjectJson(choice2) ]))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithChoices pmid pid fid rt = 
   
//        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let choice1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let choice2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectJson(choice1)
//                                             TObjectJson(choice2) ]))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithConditionalChoices pmid pid fid rt = 
        
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
         
//        let argP = 
//            TProperty(JsonPropertyNames.Arguments, 
//                      TObjectJson([ TProperty("parm4", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))
        
//        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, 
//                                    TArray([ 
//                                             ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithDefault pmid pid fid rt = 
     
//        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let obj1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeStringParmWithDefaults pmid pid fid rt et = 
      
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectVal("string1")
//                                             TObjectVal("string2")
//                                             TObjectVal("string3") ]))
//                          TProperty(JsonPropertyNames.Default, 
//                                    TArray([ TObjectVal("string2")
//                                             TObjectVal("string3") ]))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
//                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Strings"))
//                                                  TProperty(JsonPropertyNames.CustomChoices, 
//                                                            TObjectJson([ TProperty("string1", TObjectVal("string1"))
//                                                                          TProperty("string2", TObjectVal("string2"))
//                                                                          TProperty("string3", TObjectVal("string3")) ]))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithDefaults pmid pid fid rt et = 
     
//        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let c1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let c2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
//        //            let c3 =  TProperty(JsonPropertyNames.Title, TObjectVal("3")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "3"))  RepresentationTypes.Object mst
//        let d1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let d2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        
//        //            let d3 =  TProperty(JsonPropertyNames.Title, TObjectVal("3")) :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "3"))  RepresentationTypes.Object mst
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectJson(c1)
//                                             TObjectJson(c2) ]))
//                          TProperty(JsonPropertyNames.Default, 
//                                    TArray([ TObjectJson(d1)
//                                             TObjectJson(d2) ]))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
//                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeValueParm pmid pid fid rt = 
      
        
//        let p = 
//            TObjectJson([ TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParm pmid pid fid rt = 
      
        
//        let p = 
//            TObjectJson([ TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
   


//    let makeIntParmWithHint pmid pid fid rt = 
      
        
//        let p = 
//            TObjectJson([ TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class9 class10"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithRange pmid pid fid rt = 
      
        
//        let p = 
//            TObjectJson([ TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))                                                  
//                                                  TProperty(JsonPropertyNames.CustomRange, TObjectJson([TProperty("min", TObjectVal(1)); TProperty("max", TObjectVal(500))]))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)


//    let makeIntParmWithChoicesAndDefault pmid pid fid rt = 
       
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
//                          TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectVal(1)
//                                             TObjectVal(2)
//                                             TObjectVal(3) ]))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.CustomChoices, 
//                                                            TObjectJson([ TProperty("1", TObjectVal(1))
//                                                                          TProperty("2", TObjectVal(2))
//                                                                          TProperty("3", TObjectVal(3)) ]))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithChoices pmid pid fid rt = 
      
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectVal(1)
//                                             TObjectVal(2)
//                                             TObjectVal(3) ]))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.CustomChoices, 
//                                                            TObjectJson([ TProperty("1", TObjectVal(1))
//                                                                          TProperty("2", TObjectVal(2))
//                                                                          TProperty("3", TObjectVal(3)) ]))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithDefault pmid pid fid rt = 
    
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
                          
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeOptParm pmid pid fid rt d ml p = 
       
        
//        let p = 
//            TObjectJson([ 
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(d))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(ml))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(p))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
//        TProperty(pmid, p)
    
//    let makeDTParm pmid pid = 
    
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal("2016-02-16"))
//                          TProperty
//                              (JsonPropertyNames.Links, 
//                               TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("date"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithConditionalChoices pmid pid fid rt = 
      
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
       
        
//        let argP = 
//            TProperty(JsonPropertyNames.Arguments, 
//                      TObjectJson([ TProperty("parm3", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ]))
//                                    TProperty("parm4", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))
        
//        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, 
//                                    TArray([ 
//                                             ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeStringParmWithConditionalChoices pmid pid fid rt = 
        
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
       
        
//        let argP = 
//            TProperty(JsonPropertyNames.Arguments, 
//                      TObjectJson([ TProperty("parm3", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ]))
//                                    TProperty("parm4", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))
        
//        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, 
//                                    TArray([ 
//                                             ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let p1 = makeIntParm "parm1" "AnActionReturnsObjectWithParameterAnnotatedQueryOnly" "Parm1" (ttc "number")
//    let p2 = makeIntParm "parm1" "AnActionReturnsObjectWithParameters" "Parm1" (ttc "number")
//    let p3 = makeParm "parm2" "AnActionReturnsObjectWithParameters" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p4 = makeIntParm "parm1" "AnActionReturnsObjectWithParametersAnnotatedIdempotent" "Parm1" (ttc "number")
//    let p5 = makeParm "parm2" "AnActionReturnsObjectWithParametersAnnotatedIdempotent" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p6 = makeIntParm "parm1" "AnActionReturnsObjectWithParametersAnnotatedQueryOnly" "Parm1" (ttc "number")
//    let p7 = makeParm "parm2" "AnActionReturnsObjectWithParametersAnnotatedQueryOnly" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p8 = makeOptParm "parm" "AnActionWithOptionalParm" "Optional Parm" (ttc "string") "an optional parm" 101 "[A-Z]"
//    let p9 = makeOptParm "parm" "AnActionWithOptionalParmQueryOnly" "Parm" (ttc "string") "" 0 ""
//    let p10 = makeIntParm "parm1" "AnActionWithParametersWithChoicesWithDefaults" "Parm1" (ttc "number")
//    let p11 = makeIntParmWithChoicesAndDefault "parm7" "AnActionWithParametersWithChoicesWithDefaults" "Parm7" (ttc "number")
//    let p12 = makeParm "parm2" "AnActionWithParametersWithChoicesWithDefaults" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p13 = makeParmWithChoicesAndDefault "parm8" "AnActionWithParametersWithChoicesWithDefaults" "Parm8" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p14 = makeParm "parm2" "AnActionWithReferenceParameter" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p15 = makeParmWithChoices "parm4" "AnActionWithReferenceParameterWithChoices" "Parm4" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p16 = makeParmWithDefault "parm6" "AnActionWithReferenceParameterWithDefault" "Parm6" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p17 = makeParmWithAC "parm0" "AnActionWithReferenceParametersWithAutoComplete" "Parm0" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p18 = makeParmWithAC "parm1" "AnActionWithReferenceParametersWithAutoComplete" "Parm1" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p19 = makeValueParm "parm" "AnOverloadedAction1" "Parm" (ttc "string")
//    let p20 = makeIntParm "parm1" "AnActionWithValueParameter" "Parm1" (ttc "number")
//    let p21 = makeIntParmWithChoices "parm3" "AnActionWithValueParameterWithChoices" "Parm3" (ttc "number")
//    let p22 = makeIntParmWithDefault "parm5" "AnActionWithValueParameterWithDefault" "Parm5" (ttc "number")
//    let p23 = makeParm "withOtherAction" "AzContributedActionWithRefParm" "With Other Action" (ttc "RestfulObjects.Test.Data.WithActionObject")
//    let p24 = makeValueParm "parm" "AzContributedActionWithValueParm" "Parm" (ttc "string")
//    let p25 = makeIntParm "parm1" "AnActionReturnsCollectionWithParameters" "Parm1" (ttc "number")
//    let p26 = makeParm "parm2" "AnActionReturnsCollectionWithParameters" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p27 = makeIntParmWithHint "parm1" "AnActionReturnsCollectionWithScalarParameters" "Parm1" (ttc "number")
//    let p28 = makeValueParm "parm2" "AnActionReturnsCollectionWithScalarParameters" "Parm2" (ttc "string")
//    let p29 = makeIntParm "parm1" "AnActionReturnsQueryableWithParameters" "Parm1" (ttc "number")
//    let p30 = makeParm "parm2" "AnActionReturnsQueryableWithParameters" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p31 = makeIntParm "parm1" "AnActionReturnsQueryableWithScalarParameters" "Parm1" (ttc "number")
//    let p32 = makeValueParm "parm2" "AnActionReturnsQueryableWithScalarParameters" "Parm2" (ttc "string")
//    let p33 = makeIntParm "parm1" "AnActionReturnsScalarWithParameters" "Parm1" (ttc "number")
//    let p34 = makeParm "parm2" "AnActionReturnsScalarWithParameters" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p35 = makeIntParm "parm1" "AnActionReturnsVoidWithParameters" "Parm1" (ttc "number")
//    let p36 = makeParm "parm2" "AnActionReturnsVoidWithParameters" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p37 = makeIntParm "parm1" "AnActionValidateParameters" "Parm1" (ttc "number")
//    let p38 = makeIntParm "parm2" "AnActionValidateParameters" "Parm2" (ttc "number")
//    let p39 = makeDTParm "parm" "AnActionWithDateTimeParm"
//    let p40 = makeParmWithConditionalChoices "parm4" "AnActionWithReferenceParameterWithConditionalChoices" "Parm4" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p41 = makeIntParmWithConditionalChoices "parm3" "AnActionWithValueParametersWithConditionalChoices" "Parm3" (ttc "number")
//    let p42 = makeStringParmWithConditionalChoices "parm4" "AnActionWithValueParametersWithConditionalChoices" "Parm4" (ttc "string")
//    let p43 = makeStringParmWithDefaults "parm" "AnActionWithCollectionParameter" "Parm" (ttc "list") (ttc "string")
//    let p44 = makeParmWithDefaults "parm" "AnActionWithCollectionParameterRef" "Parm" (ttc "list") (ttc "RestfulObjects.Test.Data.MostSimple")
    
//    let p45 = makeIntParmWithRange "parm1" "AnActionWithValueParameterWithRange" "Parm1" (ttc "number")

//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
               
//               TObjectJson
//                   ([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1))))
                      
//                      TProperty
//                          ("ADisabledAction", 
                           
//                           TObjectJson
//                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
//                                :: makeObjectActionMemberNoParms "ADisabledAction" oid mst))
                      
//                      TProperty
//                          ("ADisabledCollectionAction", 
                           
//                           TObjectJson
//                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
//                                :: makeObjectActionCollectionMemberNoParms "ADisabledCollectionAction" oid mst))
                      
//                      TProperty
//                          ("ADisabledQueryAction", 
                           
//                           TObjectJson
//                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
//                                :: makeObjectActionCollectionMemberNoParms "ADisabledQueryAction" oid mst))
//                      TProperty("AnAction", TObjectJson(makeObjectActionMemberNoParms "AnAction" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsViewModel", 
//                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsViewModel" oid (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      
//                      TProperty
//                          ("AnActionReturnsRedirectedObject", 
//                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsRedirectedObject" oid (ttc "RestfulObjects.Test.Data.RedirectedObject")))
                      
//                      TProperty
//                          ("AnActionReturnsWithDateTimeKeyQueryOnly", 
//                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsWithDateTimeKeyQueryOnly" oid (ttc "RestfulObjects.Test.Data.WithDateTimeKey")))
//                      TProperty("AnActionAnnotatedIdempotent", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedIdempotent" oid mst))
                      
//                      TProperty
//                          ("AnActionAnnotatedIdempotentReturnsNull", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedIdempotentReturnsNull" oid mst))
                      
//                      TProperty
//                          ("AnActionAnnotatedIdempotentReturnsViewModel", 
                           
//                           TObjectJson
//                               (makeObjectActionMemberNoParms "AnActionAnnotatedIdempotentReturnsViewModel" oid 
//                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
//                      TProperty("AnActionAnnotatedQueryOnly", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedQueryOnly" oid mst))
                      
//                      TProperty
//                          ("AnActionAnnotatedQueryOnlyReturnsNull", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedQueryOnlyReturnsNull" oid mst))
                      
//                      TProperty
//                          ("AnActionAnnotatedQueryOnlyReturnsViewModel", 
                           
//                           TObjectJson
//                               (makeObjectActionMemberNoParms "AnActionAnnotatedQueryOnlyReturnsViewModel" oid 
//                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
//                      TProperty("AnActionReturnsCollection", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsCollection" oid mst))
//                      TProperty("AnActionReturnsCollectionEmpty", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsCollectionEmpty" oid mst))
//                      TProperty("AnActionReturnsCollectionNull", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsCollectionNull" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsCollectionWithParameters", 
//                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsCollectionWithParameters" oid mst [ p25; p26 ]))
                      
//                      TProperty
//                          ("AnActionReturnsCollectionWithScalarParameters", 
//                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsCollectionWithScalarParameters" oid mst [ p27; p28 ]))
//                      TProperty("AnActionReturnsNull", TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsNull" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsNullViewModel", 
//                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsNullViewModel" oid (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParameterAnnotatedQueryOnly", 
//                           TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParameterAnnotatedQueryOnly" oid mst [ p1 ]))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParameters", TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParameters" oid mst [ p2; p3 ]))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParametersAnnotatedIdempotent", 
//                           TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParametersAnnotatedIdempotent" oid mst [ p4; p5 ]))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", 
//                           TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParametersAnnotatedQueryOnly" oid mst [ p6; p7 ]))
//                      TProperty("AnActionReturnsQueryable", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsQueryable" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsQueryableWithParameters", 
//                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsQueryableWithParameters" oid mst [ p29; p30 ]))
                      
//                      TProperty
//                          ("AnActionReturnsQueryableWithScalarParameters", 
//                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsQueryableWithScalarParameters" oid mst [ p31; p32 ]))
                      
//                      TProperty
//                          ("AnActionReturnsScalar", 
//                           TObjectJson(makeActionMemberNumber "objects" "AnActionReturnsScalar" oid "An Action Returns Scalar" "" "int" []))
                      
//                      TProperty
//                          ("AnActionReturnsScalarEmpty", 
//                           TObjectJson(makeActionMemberString "objects" "AnActionReturnsScalarEmpty" oid "An Action Returns Scalar Empty" "" "string" []))
                      
//                      TProperty
//                          ("AnActionReturnsScalarNull", 
//                           TObjectJson(makeActionMemberString "objects" "AnActionReturnsScalarNull" oid "An Action Returns Scalar Null" "" "string" []))
                      
//                      TProperty
//                          ("AnActionReturnsScalarWithParameters", 
                           
//                           TObjectJson
//                               (makeActionMemberNumber "objects" "AnActionReturnsScalarWithParameters" oid "An Action Returns Scalar With Parameters" "" 
//                                    "int" [ p33; p34 ]))
//                      TProperty("AnActionReturnsVoid", TObjectJson(makeObjectActionVoidMember "AnActionReturnsVoid" oid))
                      
//                      TProperty
//                          ("AnActionReturnsVoidWithParameters", 
                           
//                           TObjectJson
//                               (makeVoidActionMember "objects" "AnActionReturnsVoidWithParameters" oid "An Action Returns Void With Parameters" 
//                                    "an action for testing" [ p35; p36 ]))
                      
//                      TProperty
//                          ("AnActionValidateParameters", 
                           
//                           TObjectJson
//                               (makeActionMemberNumber "objects" "AnActionValidateParameters" oid "An Action Validate Parameters" "" "int" [ p37; p38 ]))
                      
//                      TProperty
//                          ("AnActionWithCollectionParameter", 
//                           TObjectJson(makeVoidActionMember "objects" "AnActionWithCollectionParameter" oid "An Action With Collection Parameter" "" [ p43 ]))
                      
//                      TProperty
//                          ("AnActionWithCollectionParameterRef", 
                           
//                           TObjectJson
//                               (makeVoidActionMember "objects" "AnActionWithCollectionParameterRef" oid "An Action With Collection Parameter Ref" "" [ p44 ]))
                      
//                      TProperty
//                          ("AnActionWithDateTimeParm", 
//                           TObjectJson(makeVoidActionMember "objects" "AnActionWithDateTimeParm" oid "An Action With Date Time Parm" "" [ p39 ]))
//                      TProperty("AnActionWithOptionalParm", TObjectJson(makeObjectActionMember "AnActionWithOptionalParm" oid mst [ p8 ]))
//                      TProperty("AnActionWithOptionalParmQueryOnly", TObjectJson(makeObjectActionMember "AnActionWithOptionalParmQueryOnly" oid mst [ p9 ]))
                      
//                      TProperty
//                          ("AnActionWithParametersWithChoicesWithDefaults", 
//                           TObjectJson(makeObjectActionMember "AnActionWithParametersWithChoicesWithDefaults" oid mst [ p10; p11; p12; p13 ]))
//                      TProperty("AnActionWithReferenceParameter", TObjectJson(makeObjectActionMember "AnActionWithReferenceParameter" oid mst [ p14 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParameterWithChoices", 
//                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParameterWithChoices" oid mst [ p15 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParameterWithConditionalChoices", 
//                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParameterWithConditionalChoices" oid mst [ p40 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParameterWithDefault", 
//                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParameterWithDefault" oid mst [ p16 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParametersWithAutoComplete", 
//                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParametersWithAutoComplete" oid mst [ p17; p18 ]))
//                      TProperty("AnOverloadedAction0", TObjectJson(makeActionMember "objects" "AnOverloadedAction0" oid "An Overloaded Action" "" mst []))
//                      TProperty("AnOverloadedAction1", TObjectJson(makeActionMember "objects" "AnOverloadedAction1" oid "An Overloaded Action" "" mst [ p19 ]))
//                      TProperty("AnActionWithValueParameter", TObjectJson(makeObjectActionMember "AnActionWithValueParameter" oid mst [ p20 ]))

//                      TProperty("AnActionWithValueParameterWithRange", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithRange" oid mst [ p45 ]))
                      
//                      TProperty
//                          ("AnActionWithValueParametersWithConditionalChoices", 
//                           TObjectJson(makeObjectActionMember "AnActionWithValueParametersWithConditionalChoices" oid mst [ p41; p42 ]))
                      
//                      TProperty
//                          ("AnActionWithValueParameterWithChoices", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithChoices" oid mst [ p21 ]))
                      
//                      TProperty
//                          ("AnActionWithValueParameterWithDefault", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithDefault" oid mst [ p22 ]))
//                      TProperty("AnError", TObjectJson(makeActionMemberNumber "objects" "AnError" oid "An Error" "" "int" []))
//                      TProperty("AnErrorCollection", TObjectJson(makeObjectActionCollectionMemberNoParms "AnErrorCollection" oid mst))
//                      TProperty("AnErrorQuery", TObjectJson(makeObjectActionCollectionMemberNoParms "AnErrorQuery" oid mst))
//                      TProperty("AzContributedAction", TObjectJson(makeObjectActionMemberNoParms "AzContributedAction" oid mst))
//                      TProperty("AzContributedActionOnBaseClass", TObjectJson(makeObjectActionMemberNoParms "AzContributedActionOnBaseClass" oid mst))
//                      TProperty("AzContributedActionWithRefParm", TObjectJson(makeObjectActionMember "AzContributedActionWithRefParm" oid mst [ p23 ]))
//                      TProperty("AzContributedActionWithValueParm", TObjectJson(makeObjectActionMember "AzContributedActionWithValueParm" oid mst [ p24 ])) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action Object"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Action Objects"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithActionObjectSimpleOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
//    let oid = oType + "/" + ktc "1"
//    let argS = "x-ro-domain-model=simple"
//    let url = sprintf "http://localhost/objects/%s?%s" oid argS
//    let args = CreateReservedArgs argS
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
//    let mp r n = sprintf ";%s=\"%s\"" r n
    
//    let makeParm pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithAC pmid pid fid rt = 
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
//        let argP = 
//            TProperty
//                (JsonPropertyNames.Arguments, 
//                 TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
//        let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(3)) ]))
//        let ac = TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithChoicesAndDefault pmid pid fid rt = 
//        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let choice1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let choice2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
//        let obj1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
//                          TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectJson(choice1)
//                                             TObjectJson(choice2) ]))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithChoices pmid pid fid rt = 
//        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let choice1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let choice2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectJson(choice1)
//                                             TObjectJson(choice2) ]))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithConditionalChoices pmid pid fid rt = 
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
        
//        let argP = 
//            TProperty(JsonPropertyNames.Arguments, 
//                      TObjectJson([ TProperty("parm4", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([])) ])) ]))
        
//        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithDefault pmid pid fid rt = 
//        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let obj1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeStringParmWithDefaults pmid fid rt et = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectVal("string1")
//                                             TObjectVal("string2")
//                                             TObjectVal("string3") ]))
//                          TProperty(JsonPropertyNames.Default, 
//                                    TArray([ TObjectVal("string2")
//                                             TObjectVal("string3") ]))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
//                                                  TProperty(JsonPropertyNames.CustomChoices, 
//                                                            TObjectJson([ TProperty("string1", TObjectVal("string1"))
//                                                                          TProperty("string2", TObjectVal("string2"))
//                                                                          TProperty("string3", TObjectVal("string3")) ]))
//                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Strings"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeParmWithDefaults pmid pid fid rt et = 
//        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let c1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let c2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
//        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let d1 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//        let d2 = 
//            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectJson(c1)
//                                             TObjectJson(c2) ]))
//                          TProperty(JsonPropertyNames.Default, 
//                                    TArray([ TObjectJson(d1)
//                                             TObjectJson(d2) ]))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
//                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeValueParm pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParm pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithRange pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.CustomRange, TObjectJson([TProperty("min", TObjectVal(1)); TProperty("max", TObjectVal(500))]))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)


//    let makeIntParmWithHint pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class9 class10"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithChoicesAndDefault pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
//                          TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectVal(1)
//                                             TObjectVal(2)
//                                             TObjectVal(3) ]))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.CustomChoices, 
//                                                            TObjectJson([ TProperty("1", TObjectVal(1))
//                                                                          TProperty("2", TObjectVal(2))
//                                                                          TProperty("3", TObjectVal(3)) ]))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithChoices pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
//                                    TArray([ TObjectVal(1)
//                                             TObjectVal(2)
//                                             TObjectVal(3) ]))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.CustomChoices, 
//                                                            TObjectJson([ TProperty("1", TObjectVal(1))
//                                                                          TProperty("2", TObjectVal(2))
//                                                                          TProperty("3", TObjectVal(3)) ]))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithDefault pmid fid rt = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeOptParm pmid fid rt d ml p = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(d))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(ml))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(p))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
//        TProperty(pmid, p)
    
//    let makeDTParm pmid = 
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal("2016-02-16"))
//                          TProperty(JsonPropertyNames.Links, TArray([]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("date"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeIntParmWithConditionalChoices pmid pid fid rt = 
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
        
//        let argP = 
//            TProperty(JsonPropertyNames.Arguments, 
//                      TObjectJson([ TProperty("parm3", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([])) ]))
//                                    TProperty("parm4", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([])) ])) ]))
        
//        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let makeStringParmWithConditionalChoices pmid pid fid rt = 
//        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
//        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
        
//        let argP = 
//            TProperty(JsonPropertyNames.Arguments, 
//                      TObjectJson([ TProperty("parm3", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([])) ]))
//                                    TProperty("parm4", 
//                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
//                                                            TProperty(JsonPropertyNames.Links, TArray([])) ])) ]))
        
//        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)
    
//    let p1 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p2 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p3 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p4 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p5 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p6 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p7 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p8 = makeOptParm "parm" "Optional Parm" (ttc "string") "an optional parm" 101 "[A-Z]"
//    let p9 = makeOptParm "parm" "Parm" (ttc "string") "" 0 ""
//    let p10 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p11 = makeIntParmWithChoicesAndDefault "parm7" "Parm7" (ttc "number")
//    let p12 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p13 = makeParmWithChoicesAndDefault "parm8" "AnActionWithParametersWithChoicesWithDefaults" "Parm8" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p14 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p15 = makeParmWithChoices "parm4" "AnActionWithReferenceParameterWithChoices" "Parm4" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p16 = makeParmWithDefault "parm6" "AnActionWithReferenceParameterWithDefault" "Parm6" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p17 = makeParmWithAC "parm0" "AnActionWithReferenceParametersWithAutoComplete" "Parm0" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p18 = makeParmWithAC "parm1" "AnActionWithReferenceParametersWithAutoComplete" "Parm1" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p19 = makeValueParm "parm" "Parm" (ttc "string")
//    let p20 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p21 = makeIntParmWithChoices "parm3" "Parm3" (ttc "number")
//    let p22 = makeIntParmWithDefault "parm5" "Parm5" (ttc "number")
//    let p23 = makeParm "withOtherAction" "With Other Action" (ttc "RestfulObjects.Test.Data.WithActionObject")
//    let p24 = makeValueParm "parm" "Parm" (ttc "string")
//    let p25 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p26 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p27 = makeIntParmWithHint "parm1" "Parm1" (ttc "number")
//    let p28 = makeValueParm "parm2" "Parm2" (ttc "string")
//    let p29 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p30 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p31 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p32 = makeValueParm "parm2" "Parm2" (ttc "string")
//    let p33 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p34 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p35 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p36 = makeParm "parm2" "Parm2" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p37 = makeIntParm "parm1" "Parm1" (ttc "number")
//    let p38 = makeIntParm "parm2" "Parm2" (ttc "number")
//    let p39 = makeDTParm "parm"
//    let p40 = makeParmWithConditionalChoices "parm4" "AnActionWithReferenceParameterWithConditionalChoices" "Parm4" (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p41 = makeIntParmWithConditionalChoices "parm3" "AnActionWithValueParametersWithConditionalChoices" "Parm3" (ttc "number")
//    let p42 = makeStringParmWithConditionalChoices "parm4" "AnActionWithValueParametersWithConditionalChoices" "Parm4" (ttc "string")
//    let p43 = makeStringParmWithDefaults "parm" "Parm" (ttc "list") (ttc "string")
//    let p44 = makeParmWithDefaults "parm" "AnActionWithCollectionParameterRef" "Parm" (ttc "list") (ttc "RestfulObjects.Test.Data.MostSimple")
//    let p45 = makeIntParmWithRange "parm1" "Parm1" (ttc "number")

//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
          
//          TProperty
//              (JsonPropertyNames.Members, 
               
//               TObjectJson
//                   ([ TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oid "Id" "" "int" false (TObjectVal(1))))
                      
//                      TProperty
//                          ("ADisabledAction", 
                           
//                           TObjectJson
//                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
//                                :: makeObjectActionMemberNoParmsSimple "ADisabledAction" oid mst))
                      
//                      TProperty
//                          ("ADisabledCollectionAction", 
                           
//                           TObjectJson
//                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
//                                :: makeObjectActionCollectionMemberNoParmsSimple "ADisabledCollectionAction" oid mst))
                      
//                      TProperty
//                          ("ADisabledQueryAction", 
                           
//                           TObjectJson
//                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
//                                :: makeObjectActionCollectionMemberNoParmsSimple "ADisabledQueryAction" oid mst))
//                      TProperty("AnAction", TObjectJson(makeObjectActionMemberNoParmsSimple "AnAction" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsViewModel", 
//                           TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionReturnsViewModel" oid (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      
//                      TProperty
//                          ("AnActionReturnsRedirectedObject", 
                           
//                           TObjectJson
//                               (makeObjectActionMemberNoParmsSimple "AnActionReturnsRedirectedObject" oid (ttc "RestfulObjects.Test.Data.RedirectedObject")))
                      
//                      TProperty
//                          ("AnActionReturnsWithDateTimeKeyQueryOnly", 
                           
//                           TObjectJson
//                               (makeObjectActionMemberNoParmsSimple "AnActionReturnsWithDateTimeKeyQueryOnly" oid (ttc "RestfulObjects.Test.Data.WithDateTimeKey")))
//                      TProperty("AnActionAnnotatedIdempotent", TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedIdempotent" oid mst))
                      
//                      TProperty
//                          ("AnActionAnnotatedIdempotentReturnsViewModel", 
                           
//                           TObjectJson
//                               (makeObjectActionMemberNoParmsSimple "AnActionAnnotatedIdempotentReturnsViewModel" oid 
//                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      
//                      TProperty
//                          ("AnActionAnnotatedIdempotentReturnsNull", 
//                           TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedIdempotentReturnsNull" oid mst))
//                      TProperty("AnActionAnnotatedQueryOnly", TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedQueryOnly" oid mst))
                      
//                      TProperty
//                          ("AnActionAnnotatedQueryOnlyReturnsViewModel", 
                           
//                           TObjectJson
//                               (makeObjectActionMemberNoParmsSimple "AnActionAnnotatedQueryOnlyReturnsViewModel" oid 
//                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      
//                      TProperty
//                          ("AnActionAnnotatedQueryOnlyReturnsNull", 
//                           TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedQueryOnlyReturnsNull" oid mst))
//                      TProperty("AnActionReturnsCollection", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsCollection" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsCollectionEmpty", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsCollectionEmpty" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsCollectionNull", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsCollectionNull" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsCollectionWithParameters", 
//                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsCollectionWithParameters" oid mst [ p25; p26 ]))
                      
//                      TProperty
//                          ("AnActionReturnsCollectionWithScalarParameters", 
//                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsCollectionWithScalarParameters" oid mst [ p27; p28 ]))
//                      TProperty("AnActionReturnsNull", TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionReturnsNull" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsNullViewModel", 
                           
//                           TObjectJson
//                               (makeObjectActionMemberNoParmsSimple "AnActionReturnsNullViewModel" oid (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParameterAnnotatedQueryOnly", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParameterAnnotatedQueryOnly" oid mst [ p1 ]))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParameters", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParameters" oid mst [ p2; p3 ]))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParametersAnnotatedIdempotent", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParametersAnnotatedIdempotent" oid mst [ p4; p5 ]))
                      
//                      TProperty
//                          ("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParametersAnnotatedQueryOnly" oid mst [ p6; p7 ]))
//                      TProperty("AnActionReturnsQueryable", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsQueryable" oid mst))
                      
//                      TProperty
//                          ("AnActionReturnsQueryableWithParameters", 
//                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsQueryableWithParameters" oid mst [ p29; p30 ]))
                      
//                      TProperty
//                          ("AnActionReturnsQueryableWithScalarParameters", 
//                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsQueryableWithScalarParameters" oid mst [ p31; p32 ]))
                      
//                      TProperty
//                          ("AnActionReturnsScalar", 
//                           TObjectJson(makeActionMemberNumberSimple "objects" "AnActionReturnsScalar" oid "An Action Returns Scalar" "" "int" []))
                      
//                      TProperty
//                          ("AnActionReturnsScalarEmpty", 
//                           TObjectJson(makeActionMemberStringSimple "objects" "AnActionReturnsScalarEmpty" oid "An Action Returns Scalar Empty" "" "string" []))
                      
//                      TProperty
//                          ("AnActionReturnsScalarNull", 
//                           TObjectJson(makeActionMemberStringSimple "objects" "AnActionReturnsScalarNull" oid "An Action Returns Scalar Null" "" "string" []))
                      
//                      TProperty
//                          ("AnActionReturnsScalarWithParameters", 
                           
//                           TObjectJson
//                               (makeActionMemberNumberSimple "objects" "AnActionReturnsScalarWithParameters" oid "An Action Returns Scalar With Parameters" "" 
//                                    "int" [ p33; p34 ]))
//                      TProperty("AnActionReturnsVoid", TObjectJson(makeObjectActionVoidMemberSimple "AnActionReturnsVoid" oid))
                      
//                      TProperty
//                          ("AnActionReturnsVoidWithParameters", 
                           
//                           TObjectJson
//                               (makeVoidActionMemberSimple "objects" "AnActionReturnsVoidWithParameters" oid "An Action Returns Void With Parameters" 
//                                    "an action for testing" [ p35; p36 ]))
                      
//                      TProperty
//                          ("AnActionValidateParameters", 
                           
//                           TObjectJson
//                               (makeActionMemberNumberSimple "objects" "AnActionValidateParameters" oid "An Action Validate Parameters" "" "int" 
//                                    [ p37; p38 ]))
                      
//                      TProperty
//                          ("AnActionWithCollectionParameter", 
                           
//                           TObjectJson
//                               (makeVoidActionMemberSimple "objects" "AnActionWithCollectionParameter" oid "An Action With Collection Parameter" "" [ p43 ]))
                      
//                      TProperty
//                          ("AnActionWithCollectionParameterRef", 
                           
//                           TObjectJson
//                               (makeVoidActionMemberSimple "objects" "AnActionWithCollectionParameterRef" oid "An Action With Collection Parameter Ref" "" 
//                                    [ p44 ]))
                      
//                      TProperty
//                          ("AnActionWithDateTimeParm", 
//                           TObjectJson(makeVoidActionMemberSimple "objects" "AnActionWithDateTimeParm" oid "An Action With Date Time Parm" "" [ p39 ]))
//                      TProperty("AnActionWithOptionalParm", TObjectJson(makeObjectActionMemberSimple "AnActionWithOptionalParm" oid mst [ p8 ]))
                      
//                      TProperty
//                          ("AnActionWithOptionalParmQueryOnly", TObjectJson(makeObjectActionMemberSimple "AnActionWithOptionalParmQueryOnly" oid mst [ p9 ]))
                      
//                      TProperty
//                          ("AnActionWithParametersWithChoicesWithDefaults", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithParametersWithChoicesWithDefaults" oid mst [ p10; p11; p12; p13 ]))
//                      TProperty("AnActionWithReferenceParameter", TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameter" oid mst [ p14 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParameterWithChoices", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameterWithChoices" oid mst [ p15 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParameterWithConditionalChoices", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameterWithConditionalChoices" oid mst [ p40 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParameterWithDefault", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameterWithDefault" oid mst [ p16 ]))
                      
//                      TProperty
//                          ("AnActionWithReferenceParametersWithAutoComplete", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParametersWithAutoComplete" oid mst [ p17; p18 ]))
//                      TProperty("AnOverloadedAction0", TObjectJson(makeActionMemberSimple "objects" "AnOverloadedAction0" oid "An Overloaded Action" "" mst []))
                      
//                      TProperty
//                          ("AnOverloadedAction1", TObjectJson(makeActionMemberSimple "objects" "AnOverloadedAction1" oid "An Overloaded Action" "" mst [ p19 ]))
//                      TProperty("AnActionWithValueParameter", TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParameter" oid mst [ p20 ]))
                      
//                      TProperty("AnActionWithValueParameterWithRange", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithRange" oid mst [ p45 ]))


//                      TProperty
//                          ("AnActionWithValueParametersWithConditionalChoices", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParametersWithConditionalChoices" oid mst [ p41; p42 ]))
                      
//                      TProperty
//                          ("AnActionWithValueParameterWithChoices", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParameterWithChoices" oid mst [ p21 ]))
                      
//                      TProperty
//                          ("AnActionWithValueParameterWithDefault", 
//                           TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParameterWithDefault" oid mst [ p22 ]))
//                      TProperty("AnError", TObjectJson(makeActionMemberNumberSimple "objects" "AnError" oid "An Error" "" "int" []))
//                      TProperty("AnErrorCollection", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnErrorCollection" oid mst))
//                      TProperty("AnErrorQuery", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnErrorQuery" oid mst))
//                      TProperty("AzContributedAction", TObjectJson(makeObjectActionMemberNoParmsSimple "AzContributedAction" oid mst))
//                      TProperty("AzContributedActionOnBaseClass", TObjectJson(makeObjectActionMemberNoParmsSimple "AzContributedActionOnBaseClass" oid mst))
//                      TProperty("AzContributedActionWithRefParm", TObjectJson(makeObjectActionMemberSimple "AzContributedActionWithRefParm" oid mst [ p23 ]))
                      
//                      TProperty
//                          ("AzContributedActionWithValueParm", TObjectJson(makeObjectActionMemberSimple "AzContributedActionWithValueParm" oid mst [ p24 ])) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action Object"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Action Objects"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult



//let GetWithReferenceObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let pid = "AnEagerReference"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
//    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
//    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
//    let valueRel5 = RelValues.Value + makeParm RelParamValues.Property "AnAutoCompleteReference"
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let roid = roType + "/" + ktc "1"
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let msObj = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
//                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
//          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    let val1 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val2 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val3 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val4 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType (TObjectJson(msObj)))
//    let val5 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel5 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AConditionalChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ANullReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AnAutoCompleteReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AnEagerReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnEagerReference"
    
//    let details = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, val4)
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                     
                      
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty
//                                      ("AChoicesReference", 
//                                       TObjectJson(makePropertyMemberShort "objects" "AChoicesReference" oid "A Choices Reference" "" roType false val1 []))
                                  
//                                  TProperty
//                                      ("AConditionalChoicesReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "AConditionalChoicesReference" oid "A Conditional Choices Reference" "" roType 
//                                                false (TObjectVal(null)) []))
                                  
//                                  TProperty
//                                      ("ADisabledReference", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//                                            :: (makePropertyMemberShort "objects" "ADisabledReference" oid "A Disabled Reference" "" roType false val2 [])))
                                  
//                                  TProperty
//                                      ("ANullReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "ANullReference" oid "A Null Reference" "" roType true (TObjectVal(null)) []))
//                                  TProperty("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" oid "A Reference" "" roType false val3 []))
                                  
//                                  TProperty
//                                      ("AnAutoCompleteReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "AnAutoCompleteReference" oid "An Auto Complete Reference" "" roType false val5 []))
                                  
//                                  TProperty
//                                      ("AnEagerReference", 
//                                       TObjectJson(makePropertyMemberShort "objects" "AnEagerReference" oid "An Eager Reference" "" roType false val4 details))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With References"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithCollectionObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let pid = "AnEagerCollection"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
//    let mstv = (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")

//    let moid1 = mst + "/" + ktc "1"
//    let moid2 = mst + "/" + ktc "2"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection "ACollection"
    
//    let val1 = 
//        TObjectJson(TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val2 = 
//        TObjectJson(TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
    
   
//    let moid3 = mstv + "/" + ktc "1"
//    let moid4 = mstv + "/" + ktc "2"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection "ACollection"    
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Collection "ACollectionViewModels"    
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Collection "ADisabledCollection"    
//    let valueRel3 = RelValues.Value + makeParm RelParamValues.Collection "ASet"    
//    let valueRel4 = RelValues.Value + makeParm RelParamValues.Collection "AnEagerCollection"    

   
//    let val3 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val4 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)

//    let val5 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid3) RepresentationTypes.Object mstv)
//    let val6 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid4) RepresentationTypes.Object mstv)

//    let val7 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val8 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)

//    let val9 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val10 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)

//    let val11 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val12 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)


    
//    let value = TArray([val3;val4])
//    let valuevm = TArray([val5;val6])
//    let valued = TArray([val7;val8])
//    let valueset = TArray([val9;val10])
//    let valuee = TArray([val11;val12])
//    let emptyValue = TArray([])



//    let details = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true))]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.Value, valuee)
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" mst true)
                      
//                       ])) ]
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
   



//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ 
                                                 
//                                  TProperty("ACollection", TObjectJson(makeCollectionMember "ACollection" oid "A Collection" "" "list" 2 value))
                                  
//                                  TProperty
//                                      ("ACollectionViewModels", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMemberType "ACollectionViewModels" oid "A Collection View Models" "" "list" 2 
//                                                (ttc "RestfulObjects.Test.Data.MostSimpleViewModel") "Most Simple View Models" valuevm))
                                  
//                                  TProperty
//                                      ("ADisabledCollection", TObjectJson((makeCollectionMember "ADisabledCollection" oid "A Disabled Collection" "" "list" 2 valued)))
                                  
//                                  TProperty
//                                      ("AnEmptyCollection", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMember "AnEmptyCollection" oid "An Empty Collection" "an empty collection for testing" "list" 0 emptyValue))
                                  
//                                  TProperty
//                                      ("AnEagerCollection", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMemberTypeValue "AnEagerCollection" oid "An Eager Collection" "" "list" 2 mst "Most Simples" 
//                                                valuee details))
//                                  TProperty("ASet", TObjectJson(makeCollectionMember "ASet" oid "A Set" "" "set" 2 valueset))
//                                  TProperty("AnEmptySet", TObjectJson(makeCollectionMember "AnEmptySet" oid "An Empty Set" "an empty set for testing" "set" 0 emptyValue))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Collection"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Collections"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult



//let GetWithCollectionObjectSimpleOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = oType + "/" + ktc "1"
//    let argS = "x-ro-domain-model=simple"
//    let url = sprintf "http://localhost/objects/%s?%s" oid argS
//    let pid = "AnEagerCollection"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs argS
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
//    let mstv = (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")

//    let moid1 = mst + "/" + ktc "1"
//    let moid2 = mst + "/" + ktc "2"
    
   
//    let moid1 = mst + "/" + ktc "1"
//    let moid2 = mst + "/" + ktc "2"
//    let moid3 = mstv + "/" + ktc "1"
//    let moid4 = mstv + "/" + ktc "2"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection "ACollection"    
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Collection "ACollectionViewModels"    
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Collection "ADisabledCollection"    
//    let valueRel3 = RelValues.Value + makeParm RelParamValues.Collection "ASet"    
//    let valueRel4 = RelValues.Value + makeParm RelParamValues.Collection "AnEagerCollection"    

//    let val1 = 
//        TObjectJson(TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val2 = 
//        TObjectJson(TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
  

   
//    let val3 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val4 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)

//    let val5 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid3) RepresentationTypes.Object mstv)
//    let val6 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid4) RepresentationTypes.Object mstv)

//    let val7 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val8 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)

//    let val9 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val10 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)

//    let val11 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
//    let val12 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)


    
//    let value = TArray([val3;val4])
//    let valuevm = TArray([val5;val6])
//    let valued = TArray([val7;val8])
//    let valueset = TArray([val9;val10])
//    let valuee = TArray([val11;val12])
//    let emptyValue = TArray([])




//    let details = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true))]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.Value, valuee)
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" mst true) ])) ]
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    

    

//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oid) RepresentationTypes.Object oType) ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ 
                                 
                    
//                                  TProperty("ACollection", TObjectJson(makeCollectionMemberSimple "ACollection" oid "A Collection" "" ResultTypes.List 2 value))
                                  
//                                  TProperty
//                                      ("ACollectionViewModels", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMemberSimpleType "ACollectionViewModels" oid "A Collection View Models" "" "list" 2 
//                                                (ttc "RestfulObjects.Test.Data.MostSimpleViewModel") "Most Simple View Models" valuevm))
                                  
//                                  TProperty
//                                      ("ADisabledCollection", 
//                                       TObjectJson((makeCollectionMemberSimple "ADisabledCollection" oid "A Disabled Collection" "" ResultTypes.List 2 valued)))
                                  
//                                  TProperty
//                                      ("AnEmptyCollection", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMemberSimple "AnEmptyCollection" oid "An Empty Collection" "an empty collection for testing" 
//                                                ResultTypes.List 0 emptyValue))
                                  
//                                  TProperty
//                                      ("AnEagerCollection", 
                                       
//                                       TObjectJson
//                                           (makeCollectionMemberSimpleTypeValue "AnEagerCollection" oid "An Eager Collection" "" ResultTypes.List 2 mst 
//                                                "Most Simples" valuee details))
//                                  TProperty("ASet", TObjectJson(makeCollectionMemberSimple "ASet" oid "A Set" "" "set" 2 valueset))
                                  
//                                  TProperty
//                                      ("AnEmptySet", TObjectJson(makeCollectionMemberSimple "AnEmptySet" oid "An Empty Set" "an empty set for testing" "set" 0 emptyValue))
//                                  TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oid "Id" "" "int" false (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Collection"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Collections"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetMostSimpleViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                           ]))
//          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetFormViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.FormViewModel"
//    let oid = oType + "/" + ktc "1--1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1--1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "MostSimple"

//    let makeValueParm pmid pid fid rt = 
      
        
//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
//                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
//        TProperty(pmid, p)

//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let mp r n = sprintf ";%s=\"%s\"" r n

//    let makeParm pmid pid fid rt = 
       
//        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid

//        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst

//        let p = 
//            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
//                          TProperty(JsonPropertyNames.Links,  TArray([  ]))
//                          TProperty(JsonPropertyNames.Extensions, 
//                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
//                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
//                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
//        TProperty(pmid, p)

//    let val3 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst)

//    let p2 = makeParm "MostSimple" "Step" "Most Simple" mst
//    let p3 = makeValueParm "Name" "Step" "Name" "string"

//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1--1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("Untitled Form View Model"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                           ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("MostSimple", TObjectJson(makePropertyMemberShort "objects" "MostSimple" oid "Most Simple" "" mst true val3 []))
//                                  TProperty("Name", TObjectJson(makePropertyMemberWithFormat "objects" "Name" oid "Name" "" "string" false (TObjectVal(null))))                           
//                                  TProperty("Step", TObjectJson(makeObjectActionMemberSimple "Step" oid oType [ p2; p3 ]))                                                    
//                    ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Form View Model"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Form View Models"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult



//let GetWithValueViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValueViewModel"
//    let ticks = (new DateTime(2012, 2, 10)).Ticks.ToString()
//    let tsTicks = (new TimeSpan(1,2,3,4,5)).Ticks.ToString()
//    let key = ktc ("1--100--200--4--0----" + ticks + "--" + tsTicks + "--8--0")
//    let oid = oType + "/" + key
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, key, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let disabledValue = 
//        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//        :: (makeObjectPropertyMember "ADisabledValue" oid "A Disabled Value" (TObjectVal(200)))
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(key))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                             ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "AChoicesValue" oid "A Choices Value" (TObjectVal(0))))
                                  
//                                  TProperty
//                                      ("ADateTimeValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberDateTime "objects" "ADateTimeValue" oid "A Date Time Value" "A datetime value for testing" true 
//                                                (TObjectVal("2012-02-10")) "date"))

//                                  TProperty
//                                      ("ATimeSpanValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oid "A Time Span Value" "A timespan value for testing" true 
//                                                (TObjectVal("02:03:04")) "time"))

//                                  TProperty("ADisabledValue", TObjectJson(disabledValue))
                                  
//                                  TProperty
//                                      ("AStringValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::makePropertyMemberString "objects" "AStringValue" oid "A String Value" "A string value for testing" true 
//                                                (TObjectVal("")) []))
                                  
//                                  TProperty
//                                      ("AUserDisabledValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) 
//                                            :: makeObjectPropertyMember "AUserDisabledValue" oid "A User Disabled Value" (TObjectVal(0))))
//                                  TProperty("AValue", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "AValue" oid "A Value" (TObjectVal(100))))
//                                  TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value View Model"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Value View Models"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithReferenceViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
//    let oid = oType + "/" + ktc "1--1--1--1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let pid = "AnEagerReference"
//    let ourl = sprintf "objects/%s" oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1--1--1--1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
//    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
//    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let roid = roType + "/" + ktc "1"
//    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let msObj = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
//                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
//                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
//          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    let val1 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val2 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val3 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val4 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType (TObjectJson(msObj)))
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ANullReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AnEagerReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnEagerReference"
    
//    let details = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, val4)
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) 
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                      
                      
//                       ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1--1--1--1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                              ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty
//                                      ("AChoicesReference", 
//                                       TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AChoicesReference" oid "A Choices Reference" "" roType false val1 []))
                                  
//                                  TProperty
//                                      ("ADisabledReference", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//                                            :: (makePropertyMemberShort "objects" "ADisabledReference" oid "A Disabled Reference" "" roType false val2 [])))
                                  
//                                  TProperty
//                                      ("ANullReference", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "ANullReference" oid "A Null Reference" "" roType true (TObjectVal(null)) []))
//                                  TProperty("AReference", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AReference" oid "A Reference" "" roType false val3 []))
                                  
//                                  TProperty
//                                      ("AnEagerReference", 
//                                       TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AnEagerReference" oid "An Eager Reference" "" roType false val4 details))
//                                  TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference View Model"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Reference View Models"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetWithNestedViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithNestedViewModel"
//    let oid = oType + "/" + ktc "1--1--1--1--1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1--1--1--1--1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AReference"
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AViewModelReference"
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let roType1 = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
//    let val1 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val2 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType1 (ktc "1--1--1--1")) RepresentationTypes.Object roType1)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1--1--1--1--1"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oid) RepresentationTypes.Object oType)
//                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                           ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AReference", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AReference" oid "A Reference" "" roType false val1 []))
                                  
//                                  TProperty
//                                      ("AViewModelReference",                                   
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::makePropertyMemberShort "objects" "AViewModelReference" oid "A View Model Reference" "" roType1 false val2 []))
//                                  TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "Id" oid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Nested View Model"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Nested View Models"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithReferenceViewModelEdit(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModelEdit"
//    let oid = oType + "/" + ktc "1--1--1--1"
//    let rOid = oType + "/" + ktc "2--1--1--2"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let rooid = roType + "/" + ktc "2"
//    let pid = "AnEagerReference"
//    let ourl = sprintf "objects/%s" rOid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AnEagerReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1--1--1--1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let args1 = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let msObj = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "2"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("2"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" rooid) RepresentationTypes.Object roType)
//                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
//                             TObjectJson(args1 :: makePutLinkProp RelValues.Update (sprintf "objects/%s" rooid) RepresentationTypes.Object roType) ]))
//          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" rooid "Id" (TObjectVal(2)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
//    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
//    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
//    let val1 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType)
//    let val2 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType)
//    let val3 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType)
//    let val4 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType (TObjectJson(msObj)))
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnEagerReference"
    
//    let details = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal("AnEagerReference"))
//          TProperty(JsonPropertyNames.Value, val4)
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                      
                      
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
//    let args = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ANullReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AnEagerReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "2--1--1--2"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                              ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty
//                                      ("AChoicesReference", 
//                                       TObjectJson(makePropertyMemberShort "objects" "AChoicesReference" rOid "A Choices Reference" "" roType false val2 []))
                                  
//                                  TProperty
//                                      ("ADisabledReference", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//                                            :: (makePropertyMemberShort "objects" "ADisabledReference" rOid "A Disabled Reference" "" roType false val1 [])))
                                  
//                                  TProperty
//                                      ("ANullReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "ANullReference" rOid "A Null Reference" "" roType true (TObjectVal(null)) []))
                                  
//                                  TProperty
//                                      ("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" rOid "A Reference" "" roType false val3 []))
                                  
//                                  TProperty
//                                      ("AnEagerReference", 
//                                       TObjectJson(makePropertyMemberShort "objects" "AnEagerReference" rOid "An Eager Reference" "" roType false val4 details))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" rOid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference View Model Edit"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Reference View Model Edits"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithNestedViewModelEdit(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithNestedViewModelEdit"
//    let oid = oType + "/" + ktc "1--1--1--1--1"
//    let rOid = oType + "/" + ktc "2--2--1--1--2"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let roType1 = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
//    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType1 (ktc "2--1--1--2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))), 
//                    new JProperty("AViewModelReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1--1--1--1--1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AReference"
//    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AViewModelReference"
//    let val1 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType)
//    let val2 = 
//        TObjectJson
//            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType1 (ktc "2--1--1--2")) RepresentationTypes.Object roType1)
    
//    let arguments = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AViewModelReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "2--2--1--1--2"))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                           ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty
//                                      ("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" rOid "A Reference" "" roType false val1 []))
                                  
//                                  TProperty
//                                      ("AViewModelReference", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberShort "objects" "AViewModelReference" rOid "A View Model Reference" "" roType1 false val2 []))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" rOid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Nested View Model Edit"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Nested View Model Edits"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithValueViewModelEdit(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValueViewModelEdit"
//    let ticks = (new DateTime(2012, 2, 10)).Ticks.ToString()
//    let ticksTs = (new TimeSpan(2, 3, 4)).Ticks.ToString()
//    let key = ktc ("1--100--200--4--0----" + ticks + "--" + ticksTs + "--0--2")
//    let rKey = ktc ("1--222--200--4--333----" + ticks + "--" + ticksTs + "--0--2")
//    let oid = oType + "/" + key
//    let rOid = oType + "/" + rKey
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, key, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let disabledValue = 
//        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
//        :: (makeObjectPropertyMember "ADisabledValue" rOid "A Disabled Value" (TObjectVal(200)))
    
//    let args = 
//        TProperty(JsonPropertyNames.Arguments, 
//                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
//                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//          TProperty(JsonPropertyNames.InstanceId, TObjectVal(rKey))
//          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
//                              ]))
//          TProperty(JsonPropertyNames.Members, 
//                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" rOid "A Choices Value" (TObjectVal(333))))
                                  
//                                  TProperty
//                                      ("ADateTimeValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" rOid "A Date Time Value" "A datetime value for testing" true 
//                                                (TObjectVal("2012-02-10")) "date"))

//                                  TProperty
//                                      ("ATimeSpanValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" rOid "A Time Span Value" "A timespan value for testing" true 
//                                                (TObjectVal("02:03:04")) "time"))

//                                  TProperty("ADisabledValue", TObjectJson(disabledValue))
                                  
//                                  TProperty
//                                      ("AStringValue", 
                                       
//                                       TObjectJson
//                                           (makePropertyMemberString "objects" "AStringValue" rOid "A String Value" "A string value for testing" true 
//                                                (TObjectVal("")) []))
                                  
//                                  TProperty
//                                      ("AUserDisabledValue", 
                                       
//                                       TObjectJson
//                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
//                                            :: makeObjectPropertyMember "AUserDisabledValue" rOid "A User Disabled Value" (TObjectVal(2))))
//                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" rOid "A Value" (TObjectVal(222))))
//                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" rOid "Id" (TObjectVal(1)))) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value View Model Edit"))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Value View Model Edits"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
//                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let PutWithReferenceViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
//    let oid = oType + "/" + ktc "1--1--1--1"
//    let rOid = oType + "/" + ktc "2--1--1--2"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let rooid = roType + "/" + ktc "2"
//    let pid = "AnEagerReference"
//    let ourl = sprintf "objects/%s" rOid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AnEagerReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1--1--1--1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field disabled as object cannot be changed\", 199 RestfulObjects \"Field disabled as object cannot be changed\", 199 RestfulObjects \"Field disabled as object cannot be changed\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithNestedViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithNestedViewModel"
//    let oid = oType + "/" + ktc "1--1--1--1--1"
//    let rOid = oType + "/" + ktc "2--2--1--1--2"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let roType1 = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
//    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType1 (ktc "2--1--1--2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))), 
//                    new JProperty("AViewModelReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1--1--1--1--1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field disabled as object cannot be changed\", 199 RestfulObjects \"Field disabled as object cannot be changed\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValueViewModel"
//    let ticks = (new DateTime(2012, 2, 10)).Ticks.ToString()
//    let ticksTs = (new TimeSpan(2, 3, 4)).Ticks.ToString()
//    let key = ktc ("1--100--200--4--0----" + ticks + "--" + ticksTs + "--0--2")
//    let rKey = ktc ("1--222--200--4--333----" + ticks + "--" + ticksTs + "--0--2")
//    let oid = oType + "/" + key
//    let rOid = oType + "/" + rKey
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, key, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field disabled as object cannot be changed\", 199 RestfulObjects \"Field disabled as object cannot be changed\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400    
//let InvalidGetObject(api : RestfulObjectsControllerBase) = 
//    let oid = " " // invalid 
//    let oType = ttc " " // invalid
//    let url = sprintf "http://localhost/objects/%s/%s" oType oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oid, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedObjects.Facade.BadRequestNOSException' was thrown.\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400
//let PutWithValueObjectMissingArgs(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateArgMap(new JObject())
//    api.Request <- jsonPutMsg url ""
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueObjectMissingArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = new JObject(new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400    
//let PutWithValueObjectMalformedArgs(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty("malformed", 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueObjectMalformedDateTimeArgs(api : RestfulObjectsControllerBase) = 
//    let error = "cannot format value cannot parse as date as DateTime"
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("ADateTimeValue", new JObject(new JProperty(JsonPropertyNames.Value, "cannot parse as date"))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("ADateTimeValue", 
//                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("cannot parse as date"))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
//          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let PutWithValueObjectMalformedTimeArgs(api : RestfulObjectsControllerBase) = 
//    let error = "cannot format value cannot parse as time as TimeSpan"
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("ATimeSpanValue", new JObject(new JProperty(JsonPropertyNames.Value, "cannot parse as time"))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("ATimeSpanValue", 
//                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("cannot parse as time"))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
//          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let PutWithValueObjectMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty("malformed", 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400    
//let PutWithValueObjectInvalidArgsValue(api : RestfulObjectsControllerBase) = 
//    let error = "cannot format value invalid value as Int32"
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("AValue", 
//                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
//          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let PutWithValueObjectInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let error = "cannot format value invalid value as Int32"
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("AValue", 
//                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
//          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 400    
//let PutWithReferenceObjectInvalidArgsValue(api : RestfulObjectsControllerBase) = 
//    let error = "Not a suitable type; must be a Most Simple"
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let wvt = (ttc "RestfulObjects.Test.Data.WithValue")
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" wvt (ktc "1"))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("AReference", 
//                    TObjectJson([ TProperty
//                                      (JsonPropertyNames.Value, 
//                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt (ktc "1")))) ]))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
//          TProperty("AChoicesReference", 
//                    TObjectJson([ TProperty
//                                      (JsonPropertyNames.Value, 
//                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt (ktc "1")))) ]))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ])) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.First().ToString())
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.Skip(1).First().ToString())
//    compareObject expected parsedResult

//let PutWithReferenceObjectNotFoundArgsValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" oType (ktc "100"))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain object %s-%s: null adapter\"" oType (ktc "100"), result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithReferenceObjectInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let error = "Not a suitable type; must be a Most Simple"
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let wvt = (ttc "RestfulObjects.Test.Data.WithValue")
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" wvt (ktc "1"))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("AReference", 
//                    TObjectJson([ TProperty
//                                      (JsonPropertyNames.Value, 
//                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt (ktc "1")))) ]))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
//          TProperty("AChoicesReference", 
//                    TObjectJson([ TProperty
//                                      (JsonPropertyNames.Value, 
//                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt (ktc "1")))) ]))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ])) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.First().ToString())
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.Skip(1).First().ToString())
//    compareObject expected parsedResult

//let PutWithReferenceObjectMalformedArgs(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, "malformed"))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithReferenceObjectMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, "malformed"))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400    
//let PutWithValueObjectFailCrossValidation(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 101))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 3))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(101)) ]))
//          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(3)) ]))
//          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let PutWithValueObjectFailCrossValidationValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 101))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 3))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(101)) ]))
//          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(3)) ]))
//          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 400    
//let PutWithReferenceObjectFailsCrossValidation(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty
//              ("AReference", 
               
//               TObjectJson
//                   ([ TProperty
//                          (JsonPropertyNames.Value, 
//                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType (ktc "1"))))) ])) ]))
          
//          TProperty
//              ("AChoicesReference", 
               
//               TObjectJson
//                   ([ TProperty
//                          (JsonPropertyNames.Value, 
//                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType (ktc "2"))))) ])) ]))
//          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let PutWithReferenceObjectFailsCrossValidationValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))), new JProperty("x-ro-validate-only", true), 
//                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty
//              ("AReference", 
               
//               TObjectJson
//                   ([ TProperty
//                          (JsonPropertyNames.Value, 
//                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType (ktc "1"))))) ])) ]))
          
//          TProperty
//              ("AChoicesReference", 
               
//               TObjectJson
//                   ([ TProperty
//                          (JsonPropertyNames.Value, 
//                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType (ktc "2"))))) ])) ]))
//          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 401    
//let PutWithValueObjectDisabledValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = new JObject(new JProperty("ADisabledValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueObjectDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("ADisabledValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 401     
//let PutWithReferenceObjectDisabledValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let props = new JObject(new JProperty("ADisabledReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithReferenceObjectDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let props = 
//        new JObject(new JProperty("ADisabledReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let PutWithValueObjectInvisibleValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = new JObject(new JProperty("AHiddenValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueObjectInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = new JObject(new JProperty("AHiddenValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404     
//let PutWithReferenceObjectInvisibleValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let props = new JObject(new JProperty("AHiddenReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithReferenceObjectInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let props = 
//        new JObject(new JProperty("AHiddenReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404  
//let PutWithValueObjectInvalidArgsName(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("ANonExistentValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueObjectInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("ANonExistentValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404
//let ObjectNotFoundWrongKey(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let oid = oType + "/" + ktc "100"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "100", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

//// 404    
//let ObjectNotFoundWrongType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.DoesNotExist"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

//let ObjectNotFoundAbstractType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithAction"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

//// 404     
//let NotFoundGetObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let oid = oType + "/" + ktc "44"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, ktc "44", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain object %s-%s: null adapter\"" oType (ktc "44"), result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 405   
//let PutWithValueImmutableObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let PutWithReferenceImmutableObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let props = new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let PutWithReferenceImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let props = new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 406     
//let NotAcceptablePutObjectWrongMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    try 
//        let props = 
//            new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                        new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//        let msg = jsonPutMsg url (props.ToString())
//        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectCollection)))
//        let args = CreateArgMap props
//        api.Request <- msg
//        api.PutObject(oType, ktc "1", args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

//let NotAcceptableGetObjectWrongMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    try 
//        let args = CreateReservedArgs ""
//        let msg = jsonGetMsg (url)
//        let headers = new RequestHeaders(msg.Headers)
//        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectCollection)))
//        api.Request <- msg
//        api.GetObject(oType, ktc "1", args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

//// was notacceptable is now ignored v69 of spec 
//let GetObjectIgnoreWrongDomainType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let args = CreateReservedArgs ""
//    let msg = jsonGetMsg (url)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.Object)))
//    msg.Headers.Accept.Single()
//       .Parameters.Add(new NameValueHeaderValue("x-ro-domain-type", "\"http://localhost/domain-types/RestfulObjects.Test.Data.WithValue\""))
//    api.Request <- msg
//    let result = api.GetObject(oType, ktc "1", args)
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)

//// 500    
//let PutWithValueInternalError(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithError"
//    let oid = "RestfulObjects.Test.Data.WithError/4"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = new JObject(new JProperty("AnErrorValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
//    let result = api.PutObject(oType, ktc "4", args)
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))
//          TProperty(JsonPropertyNames.StackTrace, 
//                    TArray([ TObjectVal(new errorType(" at  in "))
//                             TObjectVal(new errorType(" at  in ")) ]))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 500    
//let PutWithReferenceInternalError(api : RestfulObjectsControllerBase) = 
    
//    let oType = ttc "RestfulObjects.Test.Data.WithError"
//    let oid = "RestfulObjects.Test.Data.WithError/3"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
//    let props = new JObject(new JProperty("AnErrorReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
//    let result = api.PutObject(oType, ktc "3", args)
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let arr1 = [ for i in 1 .. 3 ->   TObjectVal(new errorType(" at  in ")) ]
//    let arr2 = [ for i in 1 .. 4 ->   TObjectVal(new errorType(" at  in ")) ]
//    let arr3 = [ for i in 1 .. 6 ->   TObjectVal(new errorType(" at  in ")) ]


//    let expected1 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace,   TArray(arr1))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    let expected2 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace,   TArray(arr2))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    let expected3 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace, TArray(arr3))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
    
//    try
//        compareObject expected2 parsedResult
//    with e -> 
//        try 
//            compareObject expected2 parsedResult
//        with e -> 
//            compareObject expected3 parsedResult

//let PutWithValueObjectConcurrencyFail(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    //RestfulObjectsControllerBase.ConcurrencyChecking <- true
//    let tag = "\"fail\""
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsgAndTag url (props.ToString()) tag
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.PreconditionFailed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValueObjectMissingIfMatch(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = oType + "/" + ktc "1"
//    let url = sprintf "http://localhost/objects/%s" oid
//    let props = 
//        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
//                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
//    //RestfulObjectsControllerBase.ConcurrencyChecking <- true
//    let args = CreateArgMap props
//    api.Request <- jsonPutMsg url (props.ToString())
//    let result = api.PutObject(oType, ktc "1", args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(preconditionHeaderMissing, result.StatusCode, jsonResult)
//    Assert.AreEqual
//        ("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", 
//         result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)