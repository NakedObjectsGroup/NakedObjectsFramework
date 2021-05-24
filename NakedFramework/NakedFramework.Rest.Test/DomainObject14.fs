// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.DomainObject14

open NUnit.Framework
open NakedFramework.Rest.Snapshot.Constants
open NakedFramework.Rest.API
open System.Net
open System
open Newtonsoft.Json.Linq
open System.Linq
open RestfulObjects.Test.Data
open Functions

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
    let config = api.GetConfig()
    config.CacheSettings  <- (2, 100, 200)
    api.ResetConfig(config)
   
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

let GetWithDateTimeKeyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithDateTimeKey"
    let k = 634835232000000000L
    let oid = ktc (Convert.ToString(k))
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    let dt = (new DateTime(k, DateTimeKind.Utc)).ToUniversalTime()
    let title = dt.ToString()
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal(title))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))          
          TProperty
              (JsonPropertyNames.Members,                
               TObjectJson
                   ([ TProperty
                          ("Id",                            
                           TObjectJson
                               (makePropertyMemberDateTime "objects" "Id" oName "Id" "" false (TObjectVal( DateTime.Parse("2012-09-18 00:00:00.000"))) "date-time" )) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Date Time Key"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Date Time Keies"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithGuidKeyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithGuidKey"
    let key = "CA761232-ED42-11CE-BACD-00AA0057B223".ToLower()
    let oid = ktc (key)
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
   
    let title = key
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal(title))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))          
          TProperty
              (JsonPropertyNames.Members,                
               TObjectJson
                   ([ TProperty
                          ("Id",                            
                           TObjectJson
                               (makePropertyMemberGuid "objects" oName "Id" (TObjectVal(key)) )) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Guid Key"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Guid Keies"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetVerySimpleEagerObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.VerySimpleEager"
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let pid = "MostSimple"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/properties/%s" ourl pid
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property pid
    let clearRel = RelValues.Clear + makeParm RelParamValues.Property pid
    
    let msDetails = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(null))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                       
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
                             TObjectJson(makeDeleteLinkProp clearRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ]
    
    let pid = "SimpleList"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/collections/%s" ourl pid
    
    let slDetails = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Simple List"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.Value, TArray([]))         
          TProperty
              (JsonPropertyNames.Links, 
               
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
                      ])) ]
    
    let pid = "SimpleSet"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/collections/%s" ourl pid
    
    let ssDetails = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Simple Set"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.Value, TArray([]))         
          TProperty
              (JsonPropertyNames.Links,               
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)]))]
    
    let pid = "Name"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/properties/%s" ourl pid
    let clearRel = RelValues.Clear + makeParm RelParamValues.Property pid
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property pid
    
    let sDetails = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(null))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                           
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
                             TObjectJson(makeDeleteLinkProp clearRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Name"))
                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(101))
                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(@"[A-Z]"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ]
    
    let args = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("MostSimple", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Name", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("Untitled Very Simple Eager"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty
                                      ("SimpleList",                                       
                                       TObjectJson
                                           (makeCollectionMemberTypeValue "SimpleList" oName "Simple List" "" "list" 0 roType "Most Simples" (TArray([])) 
                                                slDetails))                                  
                                  TProperty
                                      ("SimpleSet",                                        
                                       TObjectJson
                                           (makeCollectionMemberTypeValue "SimpleSet" oName "Simple Set" "" "set" 0 roType "Most Simples" (TArray([])) ssDetails))                                  
                                  TProperty
                                      ("MostSimple", 
                                       TObjectJson(makePropertyMemberShort "objects" "MostSimple" oName "Most Simple" "" roType true (TObjectVal(null)) msDetails))
                                  TProperty("Name", TObjectJson(makePropertyMemberString "objects" "Name" oName "Name" "" true (TObjectVal(null)) sDetails)) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Very Simple Eager"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Very Simple Eagers"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithValueObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let disabledValue = 
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
        :: (makeObjectPropertyMember "ADisabledValue" oName "A Disabled Value" (TObjectVal(200)))
    
    let arguments = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oName "A Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("AConditionalChoicesValue", 
                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oName "A Conditional Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("ADateTimeValue",                                        
                                       TObjectJson
                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oName "A Date Time Value" "A datetime value for testing" true 
                                                (TObjectVal("2012-02-10")) "date"))
                                  TProperty
                                      ("ATimeSpanValue",                                        
                                       TObjectJson
                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oName "A Time Span Value" "A timespan value for testing" true 
                                                (TObjectVal("02:03:04")) "time"))
                                  TProperty("ADisabledValue", TObjectJson(disabledValue))                                  
                                  TProperty
                                      ("AStringValue",                                        
                                       TObjectJson
                                           (makePropertyMemberString "objects" "AStringValue" oName "A String Value" "A string value for testing" true 
                                                (TObjectVal("")) []))                                  
                                  TProperty
                                      ("AUserDisabledValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
                                            :: makeObjectPropertyMember "AUserDisabledValue" oName "A User Disabled Value" (TObjectVal(0))))
                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oName "A Value" (TObjectVal(100))))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithScalarsObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let emptyValue = TArray([])

    let arguments = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("Bool", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Byte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Char", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                //TProperty("CharArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
                                TProperty("Decimal", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("DateTime", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Double", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("EnumByAttributeChoices", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Float", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Int", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("IntWithRange", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Long", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Password", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("SByte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                //TProperty("SByteArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
                                TProperty("Short", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("String", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("UInt", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ULong", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("UShort", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    //let dt = DateTime.Parse("2012-03-27T08:42:36Z").ToUniversalTime()
    let dt = "2012-03-27"
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               
               TObjectJson
                   ([ TProperty("Bool", TObjectJson(makePropertyMemberWithType "objects" "Bool" oName "Bool" "" "boolean" false (TObjectVal(true))))
                      TProperty("Byte", TObjectJson(makePropertyMemberWithNumber "objects" "Byte" oName "Byte" "" "int" false (TObjectVal(1))))
                      //TProperty("ByteArray", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) :: makePropertyMemberWithTypeNoValue "objects"  "ByteArray" oid "Byte Array" "" "blob"  false)) ;
                      TProperty("Char", TObjectJson(makePropertyMemberWithFormat "objects" "Char" oName "Char" "" "string" false (TObjectVal("3"))))
                      //TProperty("CharArray", TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "CharArray" oid "Char Array" "" "clob"  false)) ;
                      TProperty("Decimal", TObjectJson(makePropertyMemberWithNumber "objects" "Decimal" oName "Decimal" "" "decimal" false (TObjectVal(5.1))))
                      
                      TProperty
                          ("DateTime", TObjectJson(makePropertyMemberWithFormat "objects" "DateTime" oName "Date Time" "" "date" false (TObjectVal(dt))))
                      TProperty("Double", TObjectJson(makePropertyMemberWithNumber "objects" "Double" oName "Double" "" "decimal" false (TObjectVal(6.2))))
                      
                      TProperty
                          ("EnumByAttributeChoices", 
                           
                           TObjectJson
                               (makePropertyMemberWithNumber "objects" "EnumByAttributeChoices" oName "Enum By Attribute Choices" "" "int" false 
                                    (TObjectVal(0))))
                      TProperty("Float", TObjectJson(makePropertyMemberWithNumber "objects" "Float" oName "Float" "" "decimal" false (TObjectVal(7.3))))
                      TProperty("Id", TObjectJson(makePropertyMemberWithNumber "objects" "Id" oName "Id" "" "int" false (TObjectVal(1))))
                      TProperty("Int", TObjectJson(makePropertyMemberWithNumber "objects" "Int" oName "Int" "" "int" false (TObjectVal(8))))
                      TProperty("IntWithRange", TObjectJson(makePropertyMemberWithNumber "objects" "IntWithRange" oName "Int With Range" "" "int" false (TObjectVal(0))))
                      TProperty("List", TObjectJson(makeCollectionMember "List" oName "List" "" "list" 0 emptyValue))
                      TProperty("Long", TObjectJson(makePropertyMemberWithNumber "objects" "Long" oName "Long" "" "int" false (TObjectVal(9))))
                      TProperty("Password", TObjectJson(makePropertyMemberWithFormat "objects" "Password" oName "Password" "" "string" false (TObjectVal(null))))
                      TProperty("SByte", TObjectJson(makePropertyMemberWithNumber "objects" "SByte" oName "S Byte" "" "int" false (TObjectVal(10))))
                      //TProperty("SByteArray",TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "SByteArray" oid "S Byte Array" "" "blob"  false)) ;
                      TProperty("Set", TObjectJson(makeCollectionMember "Set" oName "Set" "" "set" 0 emptyValue))
                      TProperty("Short", TObjectJson(makePropertyMemberWithNumber "objects" "Short" oName "Short" "" "int" false (TObjectVal(12))))
                      TProperty("String", TObjectJson(makePropertyMemberWithFormat "objects" "String" oName "String" "" "string" false (TObjectVal("13"))))
                      TProperty("UInt", TObjectJson(makePropertyMemberWithNumber "objects" "UInt" oName "U Int" "" "int" false (TObjectVal(14))))
                      TProperty("ULong", TObjectJson(makePropertyMemberWithNumber "objects" "ULong" oName "U Long" "" "int" false (TObjectVal(15))))
                      TProperty("UShort", TObjectJson(makePropertyMemberWithNumber "objects" "UShort" oName "U Short" "" "int" false (TObjectVal(16)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Scalars"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Scalarses"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithValueObjectUserAuth(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let disabledValue = 
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
        :: (makeObjectPropertyMember "ADisabledValue" oName "A Disabled Value" (TObjectVal(200)))
    
    let arguments = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AUserHiddenValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oName "A Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("AConditionalChoicesValue", 
                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oName "A Conditional Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("ADateTimeValue",                                        
                                       TObjectJson
                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oName "A Date Time Value" "A datetime value for testing" true 
                                                (TObjectVal("2012-02-10")) "date"))
                                  TProperty
                                      ("ATimeSpanValue",                                        
                                       TObjectJson
                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oName "A Time Span Value" "A timespan value for testing" true 
                                                (TObjectVal("02:03:04")) "time"))
                                  TProperty("ADisabledValue", TObjectJson(disabledValue))                                  
                                  TProperty
                                      ("AStringValue",                                        
                                       TObjectJson
                                           (makePropertyMemberString "objects" "AStringValue" oName "A String Value" "A string value for testing" true 
                                                (TObjectVal("")) []))                                  
                                  TProperty
                                      ("AUserDisabledValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
                                            :: makeObjectPropertyMember "AUserDisabledValue" oName "A User Disabled Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("AUserHiddenValue", TObjectJson(makeObjectPropertyMember "AUserHiddenValue" oName "A User Hidden Value" (TObjectVal(0))))
                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oName "A Value" (TObjectVal(100))))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithValueObjectWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.Object
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let disabledValue = 
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
        :: (makeObjectPropertyMember "ADisabledValue" oName "A Disabled Value" (TObjectVal(200)))
    
    let args = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oName "A Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("AConditionalChoicesValue", 
                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oName "A Conditional Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("ADateTimeValue",                                        
                                       TObjectJson
                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oName "A Date Time Value" "A datetime value for testing" true 
                                                (TObjectVal("2012-02-10")) "date"))
                                  TProperty
                                      ("ATimeSpanValue",                                        
                                       TObjectJson
                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oName "A Time Span Value" "A timespan value for testing" true 
                                                (TObjectVal("02:03:04")) "time"))
                                  TProperty("ADisabledValue", TObjectJson(disabledValue))                                  
                                  TProperty
                                      ("AStringValue",                                        
                                       TObjectJson
                                           (makePropertyMemberString "objects" "AStringValue" oName "A String Value" "A string value for testing" true 
                                                (TObjectVal("")) []))                                  
                                  TProperty
                                      ("AUserDisabledValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
                                            :: makeObjectPropertyMember "AUserDisabledValue" oName "A User Disabled Value" (TObjectVal(0))))
                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oName "A Value" (TObjectVal(100))))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetMostSimpleObjectWithDomainTypeSimple(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.Object
    let mtParms = [|("profile",  RepresentationTypes.Object);("x-ro-domain-type", "\"" + oType + "\"")|]
    jsonSetGetMsgAndMediaType api.Request url "application/json" mtParms
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
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

let GetWithValueObjectWithDomainTypeNoProfileSimple(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.Object
    let mtParms = [|("x-ro-domain-type", "\"" + oType + "\"")|]
    jsonSetGetMsgAndMediaType api.Request url "application/json" mtParms
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
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

let GetRedirectedObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.RedirectedObject"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s" oName 
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
   
    assertStatusCode HttpStatusCode.MovedPermanently statusCode jsonResult
    Assert.AreEqual(new Uri("http://redirectedtoserver/objects/RedirectedToOid"), headers.Location)
    Assert.AreEqual("", jsonResult)
   
let PutWithValueObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))),
                    new JProperty("ATimeSpanValue", new JObject(new JProperty(JsonPropertyNames.Value, "04:05:06"))),  
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
   
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let disabledValue = 
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
        :: (makeObjectPropertyMember "ADisabledValue" oName "A Disabled Value" (TObjectVal(200)))
    
    let args = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oName "A Choices Value" (TObjectVal(333))))                                  
                                  TProperty
                                      ("AConditionalChoicesValue", 
                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oName "A Conditional Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("ADateTimeValue",                                        
                                       TObjectJson
                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oName "A Date Time Value" "A datetime value for testing" true 
                                                (TObjectVal("2012-02-10")) "date"))
                                  TProperty
                                      ("ATimeSpanValue",                                        
                                       TObjectJson
                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oName "A Time Span Value" "A timespan value for testing" true 
                                                (TObjectVal("04:05:06")) "time"))
                                  TProperty("ADisabledValue", TObjectJson(disabledValue))                                  
                                  TProperty
                                      ("AStringValue",                                        
                                       TObjectJson
                                           (makePropertyMemberString "objects" "AStringValue" oName "A String Value" "A string value for testing" true 
                                                (TObjectVal("")) []))                                  
                                  TProperty
                                      ("AUserDisabledValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
                                            :: makeObjectPropertyMember "AUserDisabledValue" oName "A User Disabled Value" (TObjectVal(0))))
                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oName "A Value" (TObjectVal(222))))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithValueObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))),
                    new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
 
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.IsNull(headers.ContentType)
    Assert.AreEqual("", jsonResult)

let PutWithValueObjectConcurrencySuccess(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 444))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 555))))
    
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api1.Request url
    let result = api1.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api1.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.OK statusCode jsonResult

    let tag = headers.ETag.Tag
    let args = CreateArgMapWithReserved props

    jsonSetPutMsg api2.Request url (props.ToString())
    setIfMatch api2.Request (tag.ToString())
    let result = api2.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let disabledValue = 
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
        :: (makeObjectPropertyMember "ADisabledValue" oName "A Disabled Value" (TObjectVal(200)))
    
    let args = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AConditionalChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ADateTimeValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ATimeSpanValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AStringValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" oName "A Choices Value" (TObjectVal(555))))                                  
                                  TProperty
                                      ("AConditionalChoicesValue", 
                                       TObjectJson(makeObjectPropertyMember "AConditionalChoicesValue" oName "A Conditional Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("ADateTimeValue",                                        
                                       TObjectJson
                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" oName "A Date Time Value" "A datetime value for testing" true 
                                                (TObjectVal("2012-02-10")) "date"))
                                  TProperty
                                      ("ATimeSpanValue",                                        
                                       TObjectJson
                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oName "A Time Span Value" "A timespan value for testing" true 
                                                (TObjectVal("02:03:04")) "time"))
                                  TProperty("ADisabledValue", TObjectJson(disabledValue))                                  
                                  TProperty
                                      ("AStringValue",                                        
                                       TObjectJson
                                           (makePropertyMemberString "objects" "AStringValue" oName "A String Value" "A string value for testing" true 
                                                (TObjectVal("")) []))                                  
                                  TProperty
                                      ("AUserDisabledValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
                                            :: makeObjectPropertyMember "AUserDisabledValue" oName "A User Disabled Value" (TObjectVal(0))))
                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" oName "A Value" (TObjectVal(444))))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class1 class2"))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithScalarsObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let props = 
        new JObject(new JProperty("Bool", new JObject(new JProperty(JsonPropertyNames.Value, false))), 
                    new JProperty("Byte", new JObject(new JProperty(JsonPropertyNames.Value, 2))), 
                    new JProperty("Char", new JObject(new JProperty(JsonPropertyNames.Value, "A"))), 
                    new JProperty("Decimal", new JObject(new JProperty(JsonPropertyNames.Value, 100.9))), 
                    new JProperty("DateTime", new JObject(new JProperty(JsonPropertyNames.Value, "2011-12-25T12:13:14Z"))), 
                    new JProperty("Double", new JObject(new JProperty(JsonPropertyNames.Value, 200.8))), 
                    new JProperty("EnumByAttributeChoices", new JObject(new JProperty(JsonPropertyNames.Value, 1))), 
                    new JProperty("Float", new JObject(new JProperty(JsonPropertyNames.Value, 300.7))), 
                    new JProperty("Int", new JObject(new JProperty(JsonPropertyNames.Value, 400))),
                    new JProperty("IntWithRange", new JObject(new JProperty(JsonPropertyNames.Value, 400))), 
                    new JProperty("Long", new JObject(new JProperty(JsonPropertyNames.Value, 500))), 
                    new JProperty("SByte", new JObject(new JProperty(JsonPropertyNames.Value, 3))), 
                    new JProperty("Short", new JObject(new JProperty(JsonPropertyNames.Value, 4))), 
                    new JProperty("String", new JObject(new JProperty(JsonPropertyNames.Value, "44"))), 
                    new JProperty("UInt", new JObject(new JProperty(JsonPropertyNames.Value, 5))), 
                    new JProperty("ULong", new JObject(new JProperty(JsonPropertyNames.Value, 6))), 
                    new JProperty("UShort", new JObject(new JProperty(JsonPropertyNames.Value, 7))))
    
    let args = CreateArgMapWithReserved props    
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let emptyValue = TArray([])

    let arguments = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("Bool", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Byte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Char", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                //TProperty("CharArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
                                TProperty("Decimal", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("DateTime", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Double", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("EnumByAttributeChoices", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Float", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Int", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("IntWithRange", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Long", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Password", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("SByte", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                //TProperty("SByteArray", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]));
                                TProperty("Short", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("String", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("UInt", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ULong", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("UShort", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             TObjectJson(arguments :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))          
          TProperty
              (JsonPropertyNames.Members,                
               TObjectJson
                   ([ TProperty("Bool", TObjectJson(makePropertyMemberWithType "objects" "Bool" oName "Bool" "" "boolean" false (TObjectVal(false))))
                      TProperty("Byte", TObjectJson(makePropertyMemberWithNumber "objects" "Byte" oName "Byte" "" "int" false (TObjectVal(2))))
                      //TProperty("ByteArray", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) :: makePropertyMemberWithTypeNoValue "objects"  "ByteArray" oid "Byte Array" "" "blob"  false)) ;
                      TProperty("Char", TObjectJson(makePropertyMemberWithFormat "objects" "Char" oName "Char" "" "string" false (TObjectVal("3"))))
                      //TProperty("CharArray", TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "CharArray" oid "Char Array" "" "clob"  false)) ;
                      TProperty("Decimal", TObjectJson(makePropertyMemberWithNumber "objects" "Decimal" oName "Decimal" "" "decimal" false (TObjectVal(100.9))))                      
                      TProperty
                          ("DateTime",                            
                           TObjectJson
                               (makePropertyMemberWithFormat "objects" "DateTime" oName "Date Time" "" "date" false 
                                    (TObjectVal("2011-12-25"))))
                      TProperty("Double", TObjectJson(makePropertyMemberWithNumber "objects" "Double" oName "Double" "" "decimal" false (TObjectVal(200.8))))                      
                      TProperty
                          ("EnumByAttributeChoices",                            
                           TObjectJson
                               (makePropertyMemberWithNumber "objects" "EnumByAttributeChoices" oName "Enum By Attribute Choices" "" "int" false 
                                    (TObjectVal(1))))
                      TProperty("Float", TObjectJson(makePropertyMemberWithNumber "objects" "Float" oName "Float" "" "decimal" false (TObjectVal(300.7))))
                      TProperty("Id", TObjectJson(makePropertyMemberWithNumber "objects" "Id" oName "Id" "" "int" false (TObjectVal(1))))
                      TProperty("Int", TObjectJson(makePropertyMemberWithNumber "objects" "Int" oName "Int" "" "int" false (TObjectVal(400))))
                      TProperty("IntWithRange", TObjectJson(makePropertyMemberWithNumber "objects" "IntWithRange" oName "Int With Range" "" "int" false (TObjectVal(400))))
                      TProperty("List", TObjectJson(makeCollectionMember "List" oName "List" "" "list" 0 emptyValue))
                      TProperty("Long", TObjectJson(makePropertyMemberWithNumber "objects" "Long" oName "Long" "" "int" false (TObjectVal(500))))
                      TProperty("Password", TObjectJson(makePropertyMemberWithFormat "objects" "Password" oName "Password" "" "string" false (TObjectVal(null))))
                      TProperty("SByte", TObjectJson(makePropertyMemberWithNumber "objects" "SByte" oName "S Byte" "" "int" false (TObjectVal(3))))
                      //TProperty("SByteArray",TObjectJson(makePropertyMemberWithTypeNoValue "objects"  "SByteArray" oid "S Byte Array" "" "blob"  false)) ;
                      TProperty("Set", TObjectJson(makeCollectionMember "Set" oName "Set" "" "set" 0 emptyValue))
                      TProperty("Short", TObjectJson(makePropertyMemberWithNumber "objects" "Short" oName "Short" "" "int" false (TObjectVal(4))))
                      TProperty("String", TObjectJson(makePropertyMemberWithFormat "objects" "String" oName "String" "" "string" false (TObjectVal("44"))))
                      TProperty("UInt", TObjectJson(makePropertyMemberWithNumber "objects" "UInt" oName "U Int" "" "int" false (TObjectVal(5))))
                      TProperty("ULong", TObjectJson(makePropertyMemberWithNumber "objects" "ULong" oName "U Long" "" "int" false (TObjectVal(6))))
                      TProperty("UShort", TObjectJson(makePropertyMemberWithNumber "objects" "UShort" oName "U Short" "" "int" false (TObjectVal(7)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Scalars"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Scalarses"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithReferenceObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = ktc "2"
    let roName = sprintf "%s/%s" roType roid
    let pid = "AnEagerReference"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/properties/%s" ourl pid
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AnEagerReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AnAutoCompleteReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props    
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let args1 = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let msObj = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(roid))
          TProperty(JsonPropertyNames.Title, TObjectVal("2"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roName) RepresentationTypes.Object roType)
                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
                             TObjectJson(args1 :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roName) RepresentationTypes.Object roType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roName "Id" (TObjectVal(2)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
    let valueRel5 = RelValues.Value + makeParm RelParamValues.Property "AnAutoCompleteReference"
    
    let val1 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (oid)) RepresentationTypes.Object roType)
    let val2 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType (roid)) RepresentationTypes.Object roType)
    let val3 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType (roid)) RepresentationTypes.Object roType)
    let val4 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType (roid)) RepresentationTypes.Object roType (TObjectJson(msObj)))
    let val5 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeGetLinkProp valueRel5 (sprintf "objects/%s/%s" roType (roid)) RepresentationTypes.Object roType)
    
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnEagerReference"
    
    let details = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal("AnEagerReference"))
          TProperty(JsonPropertyNames.Value, val4)
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                                  
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    let args = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("AChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AConditionalChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AFindMenuReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ANullReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AnAutoCompleteReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AnEagerReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))  
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" oName) RepresentationTypes.Object oType) ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty
                                      ("AChoicesReference", 
                                       TObjectJson(makePropertyMemberShort "objects" "AChoicesReference" oName "A Choices Reference" "" roType false val2 []))                                  
                                  TProperty
                                      ("AConditionalChoicesReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "AConditionalChoicesReference" oName "A Conditional Choices Reference" "" roType 
                                                false (TObjectVal(null)) []))
                                  TProperty
                                      ("AFindMenuReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "AFindMenuReference" oName "A Find Menu Reference" "" roType 
                                                false (TObjectVal(null)) []))
                                  TProperty
                                      ("ADisabledReference",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                            :: (makePropertyMemberShort "objects" "ADisabledReference" oName "A Disabled Reference" "" roType false val1 [])))                                  
                                  TProperty
                                      ("ANullReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "ANullReference" oName "A Null Reference" "" roType true (TObjectVal(null)) []))
                                  TProperty("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" oName "A Reference" "" roType false val3 []))                                  
                                  TProperty
                                      ("AnAutoCompleteReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "AnAutoCompleteReference" oName "An Auto Complete Reference" "" roType false val5 []))                                  
                                  TProperty
                                      ("AnEagerReference", 
                                       TObjectJson(makePropertyMemberShort "objects" "AnEagerReference" oName "An Eager Reference" "" roType false val4 details))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With References"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithReferenceObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = ktc "2"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType roid))).ToString()))
    
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))),
                    new JProperty("x-ro-validate-only", true), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props    
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.IsNull(headers.ContentType)
    Assert.AreEqual("", jsonResult)

let GetWithActionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid
    
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let args = 
        TProperty
            (JsonPropertyNames.Arguments,
            TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))
    
    let mp r n = sprintf ";%s=\"%s\"" r n
    
    let makeParm pmid fid rt =       
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let makeDisabledParm pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled"))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)


    let makeParmWithFindMenu pmid fid rt =       
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false))
                                                  TProperty(JsonPropertyNames.CustomFindMenu, TObjectVal(true))])) ])
        TProperty(pmid, p)
    
    let makeParmWithAC pmid pid fid rt =      
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid
        let argP = 
            TProperty
                (JsonPropertyNames.Arguments, 
                 TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
        let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(3)) ]))
        let ac = TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithChoicesAndDefault pmid pid fid rt =      
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choice1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let choice2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        let obj1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst   
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
                          TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(choice1)
                                             TObjectJson(choice2) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithChoices pmid pid fid rt =    
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choice1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let choice2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(choice1)
                                             TObjectJson(choice2) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithConditionalChoices pmid pid fid rt =         
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid         
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithDefault pmid pid fid rt =      
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let obj1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeStringParmWithDefaults pmid fid rt et =               
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal("string1")
                                             TObjectVal("string2")
                                             TObjectVal("string3") ]))
                          TProperty(JsonPropertyNames.Default, 
                                    TArray([ TObjectVal("string2")
                                             TObjectVal("string3") ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Strings"))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("string1", TObjectVal("string1"))
                                                                          TProperty("string2", TObjectVal("string2"))
                                                                          TProperty("string3", TObjectVal("string3")) ]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithDefaults pmid pid fid rt et =      
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let c1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let c2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        //            let c3 =  TProperty(JsonPropertyNames.Title, TObjectVal("3")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "3"))  RepresentationTypes.Object mst
        let d1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let d2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        
        //            let d3 =  TProperty(JsonPropertyNames.Title, TObjectVal("3")) :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "3"))  RepresentationTypes.Object mst
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(c1)
                                             TObjectJson(c2) ]))
                          TProperty(JsonPropertyNames.Default, 
                                    TArray([ TObjectJson(d1)
                                             TObjectJson(d2) ]))
                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeValueParm pmid fid rt =              
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParm pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
   
    let makeIntParmWithHint pmid fid rt =              
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class9 class10"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithRange pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))                                                  
                                                  TProperty(JsonPropertyNames.CustomRange, TObjectJson([TProperty("min", TObjectVal(1)); TProperty("max", TObjectVal(500))]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let makeIntParmWithChoicesAndDefault pmid fid rt =                
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
                          TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal(1)
                                             TObjectVal(2)
                                             TObjectVal(3) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("1", TObjectVal(1))
                                                                          TProperty("2", TObjectVal(2))
                                                                          TProperty("3", TObjectVal(3)) ]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithChoices pmid fid rt =               
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal(1)
                                             TObjectVal(2)
                                             TObjectVal(3) ]))
                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("1", TObjectVal(1))
                                                                          TProperty("2", TObjectVal(2))
                                                                          TProperty("3", TObjectVal(3)) ]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithDefault pmid fid rt dv =             
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(dv))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeOptParm pmid fid rt d ml p =                
        let p = 
            TObjectJson([ 
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(d))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(ml))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(p))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
        TProperty(pmid, p)
    
    let makeDTParm pmid =             
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal("2016-02-16"))
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("date"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithConditionalChoices pmid pid fid rt =       
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid               
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm3", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ]))
                                    TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeStringParmWithConditionalChoices pmid pid fid rt =         
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (ktc "1") pid pmid               
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm3", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ]))
                                    TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let num = ttc "number"
    let str = ttc "string"
    let lst = ttc "list"

    let p1 = makeIntParm "parm1" "Parm1" num
    let p2 = makeIntParm "parm1" "Parm1" num
    let p3 = makeParm "parm2" "Parm2" mst
    let p4 = makeIntParm "parm1" "Parm1" num
    let p5 = makeParm "parm2" "Parm2" mst
    let p6 = makeIntParm "parm1" "Parm1" num
    let p7 = makeParm "parm2" "Parm2" mst
    let p8 = makeOptParm "parm" "Optional Parm" str "an optional parm" 101 "[A-Z]"
    let p9 = makeOptParm "parm" "Parm" str "" 0 ""
    let p10 = makeIntParm "parm1" "Parm1" num
    let p11 = makeIntParmWithChoicesAndDefault "parm7" "Parm7" num
    let p12 = makeParm "parm2" "Parm2" mst
    let p13 = makeParmWithChoicesAndDefault "parm8" "AnActionWithParametersWithChoicesWithDefaults" "Parm8" mst
    let p14 = makeParm "parm2" "Parm2" mst
    let p15 = makeParmWithChoices "parm4" "AnActionWithReferenceParameterWithChoices" "Parm4" mst
    let p16 = makeParmWithDefault "parm6" "AnActionWithReferenceParameterWithDefault" "Parm6" mst
    let p17 = makeParmWithAC "parm0" "AnActionWithReferenceParametersWithAutoComplete" "Parm0" mst
    let p18 = makeParmWithAC "parm1" "AnActionWithReferenceParametersWithAutoComplete" "Parm1" mst
    let p19 = makeValueParm "parm" "Parm" str
    let p20 = makeIntParm "parm1" "Parm1" num
    let p21 = makeIntParmWithChoices "parm3" "Parm3" num
    let p22 = makeIntParmWithDefault "parm5" "Parm5" num 4
    let p23 = makeParm "withOtherAction" "With Other Action" (ttc "RestfulObjects.Test.Data.WithActionObject")
    let p24 = makeValueParm "parm" "Parm" str
    let p25 = makeIntParm "parm1" "Parm1" num
    let p26 = makeParm "parm2" "Parm2" mst
    let p27 = makeIntParmWithHint "parm1" "Parm1" num
    let p28 = makeValueParm "parm2" "Parm2" str
    let p29 = makeIntParm "parm1" "Parm1" num
    let p30 = makeParm "parm2" "Parm2" mst
    let p31 = makeIntParm "parm1" "Parm1" num
    let p32 = makeValueParm "parm2" "Parm2" str
    let p33 = makeIntParm "parm1" "Parm1" num
    let p34 = makeParm "parm2" "Parm2" mst
    let p35 = makeIntParm "parm1" "Parm1" num
    let p36 = makeParm "parm2" "Parm2" mst
    let p37 = makeIntParm "parm1" "Parm1" num
    let p38 = makeIntParm "parm2" "Parm2" num
    let p39 = makeDTParm "parm"
    let p40 = makeParmWithConditionalChoices "parm4" "AnActionWithReferenceParameterWithConditionalChoices" "Parm4" mst
    let p41 = makeIntParmWithConditionalChoices "parm3" "AnActionWithValueParametersWithConditionalChoices" "Parm3" num
    let p42 = makeStringParmWithConditionalChoices "parm4" "AnActionWithValueParametersWithConditionalChoices" "Parm4" str
    let p43 = makeStringParmWithDefaults "parm" "Parm" lst str
    let p44 = makeParmWithDefaults "parm" "AnActionWithCollectionParameterRef" "Parm" lst mst    
    let p45 = makeIntParmWithRange "parm1" "Parm1" num
    let p46 = makeParmWithFindMenu "parm2" "Parm2" mst
    let p47 = makeDisabledParm "parm2" "Parm2" mst
    let p48 = makeIntParmWithDefault "id" "Id" num 1
 
    let valueRel1 pName  = RelValues.Value + sprintf ";%s=\"%s\"" RelParamValues.Property pName
    let val1 pName = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp (valueRel1 pName) (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst)

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
               TObjectJson
                   ([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1))))
                      TProperty
                          ("AzContributedDisplayAsPropertyAction", 
                           TObjectJson
                                (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                 :: (makePropertyMemberShort "objects" "AzContributedDisplayAsPropertyAction" oName "Az Contributed Display As Property Action" "" mst false (val1 "AzContributedDisplayAsPropertyAction") [])))
                      TProperty
                          ("AnObjectActionWithDisplayAsPropertyAnnotation", 
                           TObjectJson
                                (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                 :: (makePropertyMemberShort "objects" "AnObjectActionWithDisplayAsPropertyAnnotation" oName "An Object Action With Display As Property Annotation" "" mst false (val1 "AnObjectActionWithDisplayAsPropertyAnnotation") [])))
                      TProperty
                          ("ADisabledAction",                            
                           TObjectJson
                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
                                :: makeObjectActionMemberNoParms "ADisabledAction" oName mst))                      
                      TProperty
                          ("ADisabledCollectionAction",                            
                           TObjectJson
                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
                                :: makeObjectActionCollectionMemberNoParms "ADisabledCollectionAction" oName mst))                      
                      TProperty
                          ("ADisabledQueryAction",                            
                           TObjectJson
                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
                                :: makeObjectActionCollectionMemberNoParms "ADisabledQueryAction" oName mst))
                      TProperty("AnAction", TObjectJson(makeObjectActionMemberNoParms "AnAction" oName mst))                      
                      TProperty
                          ("AnActionReturnsViewModel", 
                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsViewModel" oName (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                      
                      TProperty
                          ("AnActionReturnsRedirectedObject", 
                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsRedirectedObject" oName (ttc "RestfulObjects.Test.Data.RedirectedObject")))                      
                      TProperty
                          ("AnActionReturnsWithDateTimeKeyQueryOnly", 
                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsWithDateTimeKeyQueryOnly" oName (ttc "RestfulObjects.Test.Data.WithDateTimeKey")))
                      TProperty("AnActionAnnotatedIdempotent", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedIdempotent" oName mst))                      
                      TProperty
                          ("AnActionAnnotatedIdempotentReturnsNull", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedIdempotentReturnsNull" oName mst))                      
                      TProperty
                          ("AnActionAnnotatedIdempotentReturnsViewModel",                            
                           TObjectJson
                               (makeObjectActionMemberNoParms "AnActionAnnotatedIdempotentReturnsViewModel" oName 
                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      TProperty("AnActionAnnotatedQueryOnly", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedQueryOnly" oName mst))                      
                      TProperty
                          ("AnActionAnnotatedQueryOnlyReturnsNull", TObjectJson(makeObjectActionMemberNoParms "AnActionAnnotatedQueryOnlyReturnsNull" oName mst))                      
                      TProperty
                          ("AnActionAnnotatedQueryOnlyReturnsViewModel",                            
                           TObjectJson
                               (makeObjectActionMemberNoParms "AnActionAnnotatedQueryOnlyReturnsViewModel" oName 
                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))
                      TProperty("AnActionReturnsCollection", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsCollection" oName mst))
                      TProperty("AnActionReturnsCollectionEmpty", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsCollectionEmpty" oName mst))
                      TProperty("AnActionReturnsCollectionNull", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsCollectionNull" oName mst))                      
                      TProperty
                          ("AnActionReturnsCollectionWithParameters", 
                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsCollectionWithParameters" oName mst [ p25; p26 ]))                      
                      TProperty
                          ("AnActionReturnsCollectionWithScalarParameters", 
                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsCollectionWithScalarParameters" oName mst [ p27; p28 ]))
                      TProperty("AnActionReturnsNull", TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsNull" oName mst))                      
                      TProperty
                          ("AnActionReturnsNullViewModel", 
                           TObjectJson(makeObjectActionMemberNoParms "AnActionReturnsNullViewModel" oName (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                      
                      TProperty
                          ("AnActionReturnsObjectWithParameterAnnotatedQueryOnly", 
                           TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParameterAnnotatedQueryOnly" oName mst [ p1 ]))                      
                      TProperty
                          ("AnActionReturnsObjectWithParameters", TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParameters" oName mst [ p2; p3 ]))                      
                      TProperty
                          ("AnActionReturnsObjectWithParametersAnnotatedIdempotent", 
                           TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParametersAnnotatedIdempotent" oName mst [ p4; p5 ]))                      
                      TProperty
                          ("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", 
                           TObjectJson(makeObjectActionMember "AnActionReturnsObjectWithParametersAnnotatedQueryOnly" oName mst [ p6; p7 ]))
                      TProperty("AnActionReturnsQueryable", TObjectJson(makeObjectActionCollectionMemberNoParms "AnActionReturnsQueryable" oName mst))                      
                      TProperty
                          ("AnActionReturnsQueryableWithParameters", 
                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsQueryableWithParameters" oName mst [ p29; p30 ]))                      
                      TProperty
                          ("AnActionReturnsQueryableWithScalarParameters", 
                           TObjectJson(makeObjectActionCollectionMember "AnActionReturnsQueryableWithScalarParameters" oName mst [ p31; p32 ]))                      
                      TProperty
                          ("AnActionReturnsScalar", 
                           TObjectJson(makeActionMemberNumber "objects" "AnActionReturnsScalar" oName "An Action Returns Scalar" "" "int" []))                      
                      TProperty
                          ("AnActionReturnsScalarEmpty", 
                           TObjectJson(makeActionMemberString "objects" "AnActionReturnsScalarEmpty" oName "An Action Returns Scalar Empty" "" "string" []))                      
                      TProperty
                          ("AnActionReturnsScalarNull", 
                           TObjectJson(makeActionMemberString "objects" "AnActionReturnsScalarNull" oName "An Action Returns Scalar Null" "" "string" []))                      
                      TProperty
                          ("AnActionReturnsScalarWithParameters",                            
                           TObjectJson
                               (makeActionMemberNumber "objects" "AnActionReturnsScalarWithParameters" oName "An Action Returns Scalar With Parameters" "" 
                                    "int" [ p33; p34 ]))
                      TProperty("AnActionReturnsVoid", TObjectJson(makeObjectActionVoidMember "AnActionReturnsVoid" oName))                      
                      TProperty
                          ("AnActionReturnsVoidWithParameters",                            
                           TObjectJson
                               (makeVoidActionMember "objects" "AnActionReturnsVoidWithParameters" oName "An Action Returns Void With Parameters" 
                                    "an action for testing" [ p35; p36 ]))                      
                      TProperty
                          ("AnActionValidateParameters", 
                           TObjectJson
                               (makeActionMemberNumber "objects" "AnActionValidateParameters" oName "An Action Validate Parameters" "" "int" [ p37; p38 ]))                      
                      TProperty
                          ("AnActionWithCollectionParameter", 
                           TObjectJson(makeVoidActionMember "objects" "AnActionWithCollectionParameter" oName "An Action With Collection Parameter" "" [ p43 ]))                      
                      TProperty
                          ("AnActionWithCollectionParameterRef",                            
                           TObjectJson
                               (makeVoidActionMember "objects" "AnActionWithCollectionParameterRef" oName "An Action With Collection Parameter Ref" "" [ p44 ]))                      
                      TProperty
                          ("AnActionWithDateTimeParm", 
                           TObjectJson(makeVoidActionMember "objects" "AnActionWithDateTimeParm" oName "An Action With Date Time Parm" "" [ p39 ]))
                      TProperty("AnActionWithOptionalParm", TObjectJson(makeObjectActionMember "AnActionWithOptionalParm" oName mst [ p8 ]))
                      TProperty("AnActionWithOptionalParmQueryOnly", TObjectJson(makeObjectActionMember "AnActionWithOptionalParmQueryOnly" oName mst [ p9 ]))                      
                      TProperty
                          ("AnActionWithParametersWithChoicesWithDefaults", 
                           TObjectJson(makeObjectActionMember "AnActionWithParametersWithChoicesWithDefaults" oName mst [ p10; p11; p12; p13 ]))
                      TProperty("AnActionWithReferenceParameter", TObjectJson(makeObjectActionMember "AnActionWithReferenceParameter" oName mst [ p14 ]))
                      TProperty("AnActionWithDisabledReferenceParameter", TObjectJson(makeObjectActionMember "AnActionWithDisabledReferenceParameter" oName mst [ p47 ])) 
                      TProperty("AnActionWithFindMenuParameter", TObjectJson(makeObjectActionMember "AnActionWithFindMenuParameter" oName mst [ p46 ]))                      
                      TProperty
                          ("AnActionWithReferenceParameterWithChoices", 
                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParameterWithChoices" oName mst [ p15 ]))                      
                      TProperty
                          ("AnActionWithReferenceParameterWithConditionalChoices", 
                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParameterWithConditionalChoices" oName mst [ p40 ]))                      
                      TProperty
                          ("AnActionWithReferenceParameterWithDefault", 
                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParameterWithDefault" oName mst [ p16 ]))                      
                      TProperty
                          ("AnActionWithReferenceParametersWithAutoComplete", 
                           TObjectJson(makeObjectActionMember "AnActionWithReferenceParametersWithAutoComplete" oName mst [ p17; p18 ]))
                      TProperty("AnOverloadedAction0", TObjectJson(makeActionMember "objects" "AnOverloadedAction0" oName "An Overloaded Action" "" mst []))
                      TProperty("AnOverloadedAction1", TObjectJson(makeActionMember "objects" "AnOverloadedAction1" oName "An Overloaded Action" "" mst [ p19 ]))
                      TProperty("AnActionWithValueParameter", TObjectJson(makeObjectActionMember "AnActionWithValueParameter" oName mst [ p20 ]))
                      TProperty("AnActionWithValueParameterWithRange", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithRange" oName mst [ p45 ]))                      
                      TProperty
                          ("AnActionWithValueParametersWithConditionalChoices", 
                           TObjectJson(makeObjectActionMember "AnActionWithValueParametersWithConditionalChoices" oName mst [ p41; p42 ]))                      
                      TProperty
                          ("AnActionWithValueParameterWithChoices", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithChoices" oName mst [ p21 ]))                      
                      TProperty
                          ("AnActionWithValueParameterWithDefault", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithDefault" oName mst [ p22 ]))
                      TProperty("AnError", TObjectJson(makeActionMemberNumber "objects" "AnError" oName "An Error" "" "int" []))
                      TProperty("AnErrorCollection", TObjectJson(makeObjectActionCollectionMemberNoParms "AnErrorCollection" oName mst))
                      TProperty("AnErrorQuery", TObjectJson(makeObjectActionCollectionMemberNoParms "AnErrorQuery" oName mst))
                      TProperty("AnActionWithCreateNewAnnotation", TObjectJson(makeObjectActionMember "AnActionWithCreateNewAnnotation" oName (ttc "RestfulObjects.Test.Data.WithValue") [] ))
                      TProperty("AnActionWithEditAnnotation", TObjectJson(makeObjectActionMember "AnActionWithEditAnnotation" oName (ttc "RestfulObjects.Test.Data.WithActionObject") [p48] ))
                      TProperty("AzContributedAction", TObjectJson(makeObjectActionMemberNoParms "AzContributedAction" oName mst))
                      TProperty("AzContributedActionWithCreateNewAnnotation", TObjectJson(makeObjectActionMemberNoParms "AzContributedActionWithCreateNewAnnotation" oName (ttc "RestfulObjects.Test.Data.WithValue") ))
                      TProperty("AzContributedActionOnBaseClass", TObjectJson(makeObjectActionMemberNoParms "AzContributedActionOnBaseClass" oName mst))
                      TProperty("AzContributedActionWithRefParm", TObjectJson(makeObjectActionMember "AzContributedActionWithRefParm" oName mst [ p23 ]))
                      TProperty("AzContributedActionWithValueParm", TObjectJson(makeObjectActionMember "AzContributedActionWithValueParm" oName mst [ p24 ])) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action Object"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Action Objects"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithActionObjectSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oid = ktc "1"
    let oid2 = ktc "2"
    let oName = sprintf "%s/%s" oType oid

    let argS = "x-ro-domain-model=simple"
    let url = sprintf "http://localhost/objects/%s/%s?%s" oType oid argS
    jsonSetGetMsg api.Request url
    api.DomainModel <- "simple"
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let args = 
        TProperty
            (JsonPropertyNames.Arguments,
            TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

    let mp r n = sprintf ";%s=\"%s\"" r n
    
    let makeParm pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeDisabledParm pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled"))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let makeParmWithFindMenu pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false))
                                                  TProperty(JsonPropertyNames.CustomFindMenu, TObjectVal(true))])) ])
        TProperty(pmid, p)

    let makeParmWithAC pmid pid fid rt = 
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (oid) pid pmid
        let argP = 
            TProperty
                (JsonPropertyNames.Arguments, 
                 TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
        let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(3)) ]))
        let ac = TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)
        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithChoicesAndDefault pmid pid fid rt = 
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choice1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst
        let choice2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (oid2)) RepresentationTypes.Object mst
        let obj1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst       
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
                          TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(choice1)
                                             TObjectJson(choice2) ]))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithChoices pmid pid fid rt = 
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choice1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst
        let choice2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (oid2)) RepresentationTypes.Object mst    
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(choice1)
                                             TObjectJson(choice2) ]))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithConditionalChoices pmid pid fid rt = 
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (oid) pid pmid      
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithDefault pmid pid fid rt = 
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let obj1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeStringParmWithDefaults pmid fid rt et = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal("string1")
                                             TObjectVal("string2")
                                             TObjectVal("string3") ]))
                          TProperty(JsonPropertyNames.Default, 
                                    TArray([ TObjectVal("string2")
                                             TObjectVal("string3") ]))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("string1", TObjectVal("string1"))
                                                                          TProperty("string2", TObjectVal("string2"))
                                                                          TProperty("string3", TObjectVal("string3")) ]))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Strings"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithDefaults pmid pid fid rt et = 
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let c1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst
        let c2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (oid2)) RepresentationTypes.Object mst
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let d1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst
        let d2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (oid2)) RepresentationTypes.Object mst
        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(c1)
                                             TObjectJson(c2) ]))
                          TProperty(JsonPropertyNames.Default, 
                                    TArray([ TObjectJson(d1)
                                             TObjectJson(d2) ]))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeValueParm pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParm pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithRange pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomRange, TObjectJson([TProperty("min", TObjectVal(1)); TProperty("max", TObjectVal(500))]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let makeIntParmWithHint pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class9 class10"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithChoicesAndDefault pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
                          TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal(1)
                                             TObjectVal(2)
                                             TObjectVal(3) ]))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("1", TObjectVal(1))
                                                                          TProperty("2", TObjectVal(2))
                                                                          TProperty("3", TObjectVal(3)) ]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithChoices pmid fid rt = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal(1)
                                             TObjectVal(2)
                                             TObjectVal(3) ]))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("1", TObjectVal(1))
                                                                          TProperty("2", TObjectVal(2))
                                                                          TProperty("3", TObjectVal(3)) ]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithDefault pmid fid rt dv = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(dv))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeOptParm pmid fid rt d ml p = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(d))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(ml))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(p))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
        TProperty(pmid, p)
    
    let makeDTParm pmid = 
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal("2016-02-16"))
                          TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("date"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithConditionalChoices pmid pid fid rt = 
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (oid) pid pmid        
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm3", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([])) ]))
                                    TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeStringParmWithConditionalChoices pmid pid fid rt = 
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "objects/%s/%s/actions/%s/params/%s/prompt" oType (oid) pid pmid        
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm3", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([])) ]))
                                    TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let num = ttc "number"
    let str = ttc "string"
    let lst = ttc "list"

    let p1 = makeIntParm "parm1" "Parm1" num
    let p2 = makeIntParm "parm1" "Parm1" num
    let p3 = makeParm "parm2" "Parm2" mst
    let p4 = makeIntParm "parm1" "Parm1" num
    let p5 = makeParm "parm2" "Parm2" mst
    let p6 = makeIntParm "parm1" "Parm1" num
    let p7 = makeParm "parm2" "Parm2" mst
    let p8 = makeOptParm "parm" "Optional Parm" str "an optional parm" 101 "[A-Z]"
    let p9 = makeOptParm "parm" "Parm" str "" 0 ""
    let p10 = makeIntParm "parm1" "Parm1" num
    let p11 = makeIntParmWithChoicesAndDefault "parm7" "Parm7" num
    let p12 = makeParm "parm2" "Parm2" mst
    let p13 = makeParmWithChoicesAndDefault "parm8" "AnActionWithParametersWithChoicesWithDefaults" "Parm8" mst
    let p14 = makeParm "parm2" "Parm2" mst
    let p15 = makeParmWithChoices "parm4" "AnActionWithReferenceParameterWithChoices" "Parm4" mst
    let p16 = makeParmWithDefault "parm6" "AnActionWithReferenceParameterWithDefault" "Parm6" mst
    let p17 = makeParmWithAC "parm0" "AnActionWithReferenceParametersWithAutoComplete" "Parm0" mst
    let p18 = makeParmWithAC "parm1" "AnActionWithReferenceParametersWithAutoComplete" "Parm1" mst
    let p19 = makeValueParm "parm" "Parm" str
    let p20 = makeIntParm "parm1" "Parm1" num
    let p21 = makeIntParmWithChoices "parm3" "Parm3" num
    let p22 = makeIntParmWithDefault "parm5" "Parm5" num 4
    let p23 = makeParm "withOtherAction" "With Other Action" (ttc "RestfulObjects.Test.Data.WithActionObject")
    let p24 = makeValueParm "parm" "Parm" str
    let p25 = makeIntParm "parm1" "Parm1" num
    let p26 = makeParm "parm2" "Parm2" mst
    let p27 = makeIntParmWithHint "parm1" "Parm1" num
    let p28 = makeValueParm "parm2" "Parm2" str
    let p29 = makeIntParm "parm1" "Parm1" num
    let p30 = makeParm "parm2" "Parm2" mst
    let p31 = makeIntParm "parm1" "Parm1" num
    let p32 = makeValueParm "parm2" "Parm2" str
    let p33 = makeIntParm "parm1" "Parm1" num
    let p34 = makeParm "parm2" "Parm2" mst
    let p35 = makeIntParm "parm1" "Parm1" num
    let p36 = makeParm "parm2" "Parm2" mst
    let p37 = makeIntParm "parm1" "Parm1" num
    let p38 = makeIntParm "parm2" "Parm2" num
    let p39 = makeDTParm "parm"
    let p40 = makeParmWithConditionalChoices "parm4" "AnActionWithReferenceParameterWithConditionalChoices" "Parm4" mst
    let p41 = makeIntParmWithConditionalChoices "parm3" "AnActionWithValueParametersWithConditionalChoices" "Parm3" num
    let p42 = makeStringParmWithConditionalChoices "parm4" "AnActionWithValueParametersWithConditionalChoices" "Parm4" str
    let p43 = makeStringParmWithDefaults "parm" "Parm" lst str
    let p44 = makeParmWithDefaults "parm" "AnActionWithCollectionParameterRef" "Parm" lst mst
    let p45 = makeIntParmWithRange "parm1" "Parm1" num
    let p46 = makeParmWithFindMenu "parm2" "Parm2" mst
    let p47 = makeDisabledParm "parm2" "Parm2" mst
    let p48 = makeIntParmWithDefault "id" "Id" num 1

    let valueRel1 pName  = RelValues.Value + sprintf ";%s=\"%s\"" RelParamValues.Property pName
    let val1 pName = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp (valueRel1 pName) (sprintf "objects/%s/%s" mst (oid)) RepresentationTypes.Object mst)

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
               TObjectJson
                   ([ TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oName "Id" "" "int" false (TObjectVal(1))))
                      TProperty
                          ("AzContributedDisplayAsPropertyAction", 
                           TObjectJson
                                (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                 :: (makePropertyMemberShort "objects" "AzContributedDisplayAsPropertyAction" oName "Az Contributed Display As Property Action" "" mst false (val1 "AzContributedDisplayAsPropertyAction" ) [])))
                      TProperty
                          ("AnObjectActionWithDisplayAsPropertyAnnotation", 
                           TObjectJson
                                (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                 :: (makePropertyMemberShort "objects" "AnObjectActionWithDisplayAsPropertyAnnotation" oName "An Object Action With Display As Property Annotation" "" mst false (val1 "AnObjectActionWithDisplayAsPropertyAnnotation" ) [])))
                      TProperty
                          ("ADisabledAction",                            
                           TObjectJson
                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
                                :: makeObjectActionMemberNoParmsSimple "ADisabledAction" oName mst))                      
                      TProperty
                          ("ADisabledCollectionAction",                            
                           TObjectJson
                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
                                :: makeObjectActionCollectionMemberNoParmsSimple "ADisabledCollectionAction" oName mst))                      
                      TProperty
                          ("ADisabledQueryAction",                            
                           TObjectJson
                               (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Always disabled")) 
                                :: makeObjectActionCollectionMemberNoParmsSimple "ADisabledQueryAction" oName mst))
                      TProperty("AnAction", TObjectJson(makeObjectActionMemberNoParmsSimple "AnAction" oName mst))                      
                      TProperty
                          ("AnActionReturnsViewModel", 
                           TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionReturnsViewModel" oName (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                      
                      TProperty
                          ("AnActionReturnsRedirectedObject",                            
                           TObjectJson
                               (makeObjectActionMemberNoParmsSimple "AnActionReturnsRedirectedObject" oName (ttc "RestfulObjects.Test.Data.RedirectedObject")))                      
                      TProperty
                          ("AnActionReturnsWithDateTimeKeyQueryOnly",                            
                           TObjectJson
                               (makeObjectActionMemberNoParmsSimple "AnActionReturnsWithDateTimeKeyQueryOnly" oName (ttc "RestfulObjects.Test.Data.WithDateTimeKey")))
                      TProperty("AnActionAnnotatedIdempotent", TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedIdempotent" oName mst))                      
                      TProperty
                          ("AnActionAnnotatedIdempotentReturnsViewModel",                            
                           TObjectJson
                               (makeObjectActionMemberNoParmsSimple "AnActionAnnotatedIdempotentReturnsViewModel" oName 
                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                      
                      TProperty
                          ("AnActionAnnotatedIdempotentReturnsNull", 
                           TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedIdempotentReturnsNull" oName mst))
                      TProperty("AnActionAnnotatedQueryOnly", TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedQueryOnly" oName mst))                      
                      TProperty
                          ("AnActionAnnotatedQueryOnlyReturnsViewModel",                            
                           TObjectJson
                               (makeObjectActionMemberNoParmsSimple "AnActionAnnotatedQueryOnlyReturnsViewModel" oName 
                                    (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                      
                      TProperty
                          ("AnActionAnnotatedQueryOnlyReturnsNull", 
                           TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionAnnotatedQueryOnlyReturnsNull" oName mst))
                      TProperty("AnActionReturnsCollection", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsCollection" oName mst))                      
                      TProperty
                          ("AnActionReturnsCollectionEmpty", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsCollectionEmpty" oName mst))                      
                      TProperty
                          ("AnActionReturnsCollectionNull", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsCollectionNull" oName mst))                      
                      TProperty
                          ("AnActionReturnsCollectionWithParameters", 
                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsCollectionWithParameters" oName mst [ p25; p26 ]))                      
                      TProperty
                          ("AnActionReturnsCollectionWithScalarParameters", 
                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsCollectionWithScalarParameters" oName mst [ p27; p28 ]))
                      TProperty("AnActionReturnsNull", TObjectJson(makeObjectActionMemberNoParmsSimple "AnActionReturnsNull" oName mst))                      
                      TProperty
                          ("AnActionReturnsNullViewModel",                            
                           TObjectJson
                               (makeObjectActionMemberNoParmsSimple "AnActionReturnsNullViewModel" oName (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                      
                      TProperty
                          ("AnActionReturnsObjectWithParameterAnnotatedQueryOnly", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParameterAnnotatedQueryOnly" oName mst [ p1 ]))                      
                      TProperty
                          ("AnActionReturnsObjectWithParameters", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParameters" oName mst [ p2; p3 ]))                      
                      TProperty
                          ("AnActionReturnsObjectWithParametersAnnotatedIdempotent", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParametersAnnotatedIdempotent" oName mst [ p4; p5 ]))                      
                      TProperty
                          ("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionReturnsObjectWithParametersAnnotatedQueryOnly" oName mst [ p6; p7 ]))
                      TProperty("AnActionReturnsQueryable", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnActionReturnsQueryable" oName mst))                      
                      TProperty
                          ("AnActionReturnsQueryableWithParameters", 
                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsQueryableWithParameters" oName mst [ p29; p30 ]))                      
                      TProperty
                          ("AnActionReturnsQueryableWithScalarParameters", 
                           TObjectJson(makeObjectActionCollectionMemberSimple "AnActionReturnsQueryableWithScalarParameters" oName mst [ p31; p32 ]))                      
                      TProperty
                          ("AnActionReturnsScalar", 
                           TObjectJson(makeActionMemberNumberSimple "objects" "AnActionReturnsScalar" oName "An Action Returns Scalar" "" "int" []))                      
                      TProperty
                          ("AnActionReturnsScalarEmpty", 
                           TObjectJson(makeActionMemberStringSimple "objects" "AnActionReturnsScalarEmpty" oName "An Action Returns Scalar Empty" "" "string" []))                      
                      TProperty
                          ("AnActionReturnsScalarNull", 
                           TObjectJson(makeActionMemberStringSimple "objects" "AnActionReturnsScalarNull" oName "An Action Returns Scalar Null" "" "string" []))                      
                      TProperty
                          ("AnActionReturnsScalarWithParameters",                            
                           TObjectJson
                               (makeActionMemberNumberSimple "objects" "AnActionReturnsScalarWithParameters" oName "An Action Returns Scalar With Parameters" "" 
                                    "int" [ p33; p34 ]))
                      TProperty("AnActionReturnsVoid", TObjectJson(makeObjectActionVoidMemberSimple "AnActionReturnsVoid" oName))                      
                      TProperty
                          ("AnActionReturnsVoidWithParameters",                            
                           TObjectJson
                               (makeVoidActionMemberSimple "objects" "AnActionReturnsVoidWithParameters" oName "An Action Returns Void With Parameters" 
                                    "an action for testing" [ p35; p36 ]))                      
                      TProperty
                          ("AnActionValidateParameters",                            
                           TObjectJson
                               (makeActionMemberNumberSimple "objects" "AnActionValidateParameters" oName "An Action Validate Parameters" "" "int" 
                                    [ p37; p38 ]))                      
                      TProperty
                          ("AnActionWithCollectionParameter",                            
                           TObjectJson
                               (makeVoidActionMemberSimple "objects" "AnActionWithCollectionParameter" oName "An Action With Collection Parameter" "" [ p43 ]))                      
                      TProperty
                          ("AnActionWithCollectionParameterRef",                            
                           TObjectJson
                               (makeVoidActionMemberSimple "objects" "AnActionWithCollectionParameterRef" oName "An Action With Collection Parameter Ref" "" 
                                    [ p44 ]))                      
                      TProperty
                          ("AnActionWithDateTimeParm", 
                           TObjectJson(makeVoidActionMemberSimple "objects" "AnActionWithDateTimeParm" oName "An Action With Date Time Parm" "" [ p39 ]))
                      TProperty("AnActionWithOptionalParm", TObjectJson(makeObjectActionMemberSimple "AnActionWithOptionalParm" oName mst [ p8 ]))                      
                      TProperty
                          ("AnActionWithOptionalParmQueryOnly", TObjectJson(makeObjectActionMemberSimple "AnActionWithOptionalParmQueryOnly" oName mst [ p9 ]))                      
                      TProperty
                          ("AnActionWithParametersWithChoicesWithDefaults", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithParametersWithChoicesWithDefaults" oName mst [ p10; p11; p12; p13 ]))
                      TProperty("AnActionWithReferenceParameter", TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameter" oName mst [ p14 ])) 
                      TProperty("AnActionWithDisabledReferenceParameter", TObjectJson(makeObjectActionMemberSimple "AnActionWithDisabledReferenceParameter" oName mst [ p47 ]))
                      TProperty("AnActionWithFindMenuParameter", TObjectJson(makeObjectActionMemberSimple "AnActionWithFindMenuParameter" oName mst [ p46 ]))                      
                      TProperty
                          ("AnActionWithReferenceParameterWithChoices", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameterWithChoices" oName mst [ p15 ]))                      
                      TProperty
                          ("AnActionWithReferenceParameterWithConditionalChoices", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameterWithConditionalChoices" oName mst [ p40 ]))                      
                      TProperty
                          ("AnActionWithReferenceParameterWithDefault", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParameterWithDefault" oName mst [ p16 ]))                      
                      TProperty
                          ("AnActionWithReferenceParametersWithAutoComplete", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithReferenceParametersWithAutoComplete" oName mst [ p17; p18 ]))
                      TProperty("AnOverloadedAction0", TObjectJson(makeActionMemberSimple "objects" "AnOverloadedAction0" oName "An Overloaded Action" "" mst []))                      
                      TProperty
                          ("AnOverloadedAction1", TObjectJson(makeActionMemberSimple "objects" "AnOverloadedAction1" oName "An Overloaded Action" "" mst [ p19 ]))
                      TProperty("AnActionWithValueParameter", TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParameter" oName mst [ p20 ]))                      
                      TProperty("AnActionWithValueParameterWithRange", TObjectJson(makeObjectActionMember "AnActionWithValueParameterWithRange" oName mst [ p45 ]))
                      TProperty
                          ("AnActionWithValueParametersWithConditionalChoices", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParametersWithConditionalChoices" oName mst [ p41; p42 ]))                      
                      TProperty
                          ("AnActionWithValueParameterWithChoices", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParameterWithChoices" oName mst [ p21 ]))                      
                      TProperty
                          ("AnActionWithValueParameterWithDefault", 
                           TObjectJson(makeObjectActionMemberSimple "AnActionWithValueParameterWithDefault" oName mst [ p22 ]))
                      TProperty("AnError", TObjectJson(makeActionMemberNumberSimple "objects" "AnError" oName "An Error" "" "int" []))
                      TProperty("AnErrorCollection", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnErrorCollection" oName mst))
                      TProperty("AnErrorQuery", TObjectJson(makeObjectActionCollectionMemberNoParmsSimple "AnErrorQuery" oName mst))
                      TProperty("AnActionWithCreateNewAnnotation", TObjectJson(makeObjectActionMemberSimple "AnActionWithCreateNewAnnotation" oName (ttc "RestfulObjects.Test.Data.WithValue") [] ))
                      TProperty("AnActionWithEditAnnotation", TObjectJson(makeObjectActionMemberSimple "AnActionWithEditAnnotation" oName (ttc "RestfulObjects.Test.Data.WithActionObject") [p48] ))
                      TProperty("AzContributedAction", TObjectJson(makeObjectActionMemberNoParmsSimple "AzContributedAction" oName mst))
                      TProperty("AzContributedActionWithCreateNewAnnotation", TObjectJson(makeObjectActionMemberNoParms "AzContributedActionWithCreateNewAnnotation" oName (ttc "RestfulObjects.Test.Data.WithValue") ))
                      TProperty("AzContributedActionOnBaseClass", TObjectJson(makeObjectActionMemberNoParmsSimple "AzContributedActionOnBaseClass" oName mst))
                      TProperty("AzContributedActionWithRefParm", TObjectJson(makeObjectActionMemberSimple "AzContributedActionWithRefParm" oName mst [ p23 ]))                      
                      TProperty
                          ("AzContributedActionWithValueParm", TObjectJson(makeObjectActionMemberSimple "AzContributedActionWithValueParm" oName mst [ p24 ])) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action Object"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Action Objects"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithReferenceObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let pid = "AnEagerReference"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/properties/%s" ourl pid

    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
    let valueRel5 = RelValues.Value + makeParm RelParamValues.Property "AnAutoCompleteReference"
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let msObj = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    let val1 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (oid)) RepresentationTypes.Object roType)
    let val2 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType (oid)) RepresentationTypes.Object roType)
    let val3 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType (oid)) RepresentationTypes.Object roType)
    let val4 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType (oid)) RepresentationTypes.Object roType (TObjectJson(msObj)))
    let val5 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel5 (sprintf "objects/%s/%s" roType (oid)) RepresentationTypes.Object roType)
    
    let arguments = 
        TProperty(JsonPropertyNames.Arguments, 
                  TObjectJson([ TProperty("AChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AConditionalChoicesReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AFindMenuReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("ANullReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AnAutoCompleteReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("AnEagerReference", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                                TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnEagerReference"
    
    let details = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, val4)
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                                 
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
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
                                      ("AChoicesReference", 
                                       TObjectJson(makePropertyMemberShort "objects" "AChoicesReference" oName "A Choices Reference" "" roType false val1 []))                                  
                                  TProperty
                                      ("AConditionalChoicesReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "AConditionalChoicesReference" oName "A Conditional Choices Reference" "" roType 
                                                false (TObjectVal(null)) []))
                                  TProperty
                                      ("AFindMenuReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "AFindMenuReference" oName "A Find Menu Reference" "" roType 
                                                false (TObjectVal(null)) []))
                                  TProperty
                                      ("ADisabledReference",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                            :: (makePropertyMemberShort "objects" "ADisabledReference" oName "A Disabled Reference" "" roType false val2 [])))                                  
                                  TProperty
                                      ("ANullReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "ANullReference" oName "A Null Reference" "" roType true (TObjectVal(null)) []))
                                  TProperty("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" oName "A Reference" "" roType false val3 []))                                  
                                  TProperty
                                      ("AnAutoCompleteReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "AnAutoCompleteReference" oName "An Auto Complete Reference" "" roType false val5 []))                                  
                                  TProperty
                                      ("AnEagerReference", 
                                       TObjectJson(makePropertyMemberShort "objects" "AnEagerReference" oName "An Eager Reference" "" roType false val4 details))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With References"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid
       
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
    let mstv = (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")

    let pid = "AnEagerCollection"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/collections/%s" ourl pid
    let oid2 = ktc "2"
    let moid1 = mst + "/" + oid
    let moid2 = mst + "/" + oid2
      
    let moid3 = mstv + "/" + oid
    let moid4 = mstv + "/" + oid2
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection "ACollection"    
    let valueRel1 = RelValues.Value + makeParm RelParamValues.Collection "ACollectionViewModels"    
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Collection "ADisabledCollection"    
    let valueRel3 = RelValues.Value + makeParm RelParamValues.Collection "ASet"    
    let valueRel4 = RelValues.Value + makeParm RelParamValues.Collection "AnEagerCollection"    
  
    let val3 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val4 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
    let val5 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid3) RepresentationTypes.Object mstv)
    let val6 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid4) RepresentationTypes.Object mstv)
    let val7 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val8 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
    let val9 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val10 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
    let val11 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val12 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)

    let value = TArray([val3;val4])
    let valuevm = TArray([val5;val6])
    let valued = TArray([val7;val8])
    let valueset = TArray([val9;val10])
    let valuee = TArray([val11;val12])
    let emptyValue = TArray([])

    let details = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true))]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.Value, valuee)          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" mst true)                      
                       ])) ]
    
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
                    TObjectJson([                                                  
                                  TProperty("ACollection", TObjectJson(makeCollectionMember "ACollection" oName "A Collection" "" "list" 2 value))                                  
                                  TProperty
                                      ("ACollectionViewModels",                                        
                                       TObjectJson
                                           (makeCollectionMemberType "ACollectionViewModels" oName "A Collection View Models" "" "list" 2 
                                                (ttc "RestfulObjects.Test.Data.MostSimpleViewModel") "Most Simple View Models" valuevm))                                  
                                  TProperty
                                      ("ADisabledCollection", TObjectJson((makeCollectionMember "ADisabledCollection" oName "A Disabled Collection" "" "list" 2 valued)))                                  
                                  TProperty
                                      ("AnEmptyCollection",                                        
                                       TObjectJson
                                           (makeCollectionMember "AnEmptyCollection" oName "An Empty Collection" "an empty collection for testing" "list" 0 emptyValue))                                  
                                  TProperty
                                      ("AnEagerCollection",                                        
                                       TObjectJson
                                           (makeCollectionMemberTypeValue "AnEagerCollection" oName "An Eager Collection" "" "list" 2 mst "Most Simples" 
                                                valuee details))
                                  TProperty("ASet", TObjectJson(makeCollectionMember "ASet" oName "A Set" "" "set" 2 valueset))
                                  TProperty("AnEmptySet", TObjectJson(makeCollectionMember "AnEmptySet" oName "An Empty Set" "an empty set for testing" "set" 0 emptyValue))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Collection"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Collections"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithCollectionObjectSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let oid2 = ktc "2"
    let oName = sprintf "%s/%s" oType oid

    let argS = "x-ro-domain-model=simple"
    let url = sprintf "http://localhost/objects/%s/%s?%s" oType oid argS
    jsonSetGetMsg api.Request url
    api.DomainModel <- "simple"
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let pid = "AnEagerCollection"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/collections/%s" ourl pid
    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
    let mstv = (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")
   
    let moid1 = mst + "/" + oid
    let moid2 = mst + "/" + oid2
    let moid3 = mstv + "/" + oid
    let moid4 = mstv + "/" + oid2
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection "ACollection"    
    let valueRel1 = RelValues.Value + makeParm RelParamValues.Collection "ACollectionViewModels"    
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Collection "ADisabledCollection"    
    let valueRel3 = RelValues.Value + makeParm RelParamValues.Collection "ASet"    
    let valueRel4 = RelValues.Value + makeParm RelParamValues.Collection "AnEagerCollection"    

    let val3 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val4 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
    let val5 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid3) RepresentationTypes.Object mstv)
    let val6 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel1 (sprintf "objects/%s" moid4) RepresentationTypes.Object mstv)
    let val7 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val8 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel2 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
    let val9 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val10 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel3 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
    let val11 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid1) RepresentationTypes.Object mst)
    let val12 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel4 (sprintf "objects/%s" moid2) RepresentationTypes.Object mst)
  
    let value = TArray([val3;val4])
    let valuevm = TArray([val5;val6])
    let valued = TArray([val7;val8])
    let valueset = TArray([val9;val10])
    let valuee = TArray([val11;val12])
    let emptyValue = TArray([])

    let details = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true))]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.Value, valuee)
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" mst true) ])) ]
    
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
                    TObjectJson([                                                     
                                  TProperty("ACollection", TObjectJson(makeCollectionMemberSimple "ACollection" oName "A Collection" "" ResultTypes.List 2 value))                                  
                                  TProperty
                                      ("ACollectionViewModels",                                        
                                       TObjectJson
                                           (makeCollectionMemberSimpleType "ACollectionViewModels" oName "A Collection View Models" "" "list" 2 
                                                (ttc "RestfulObjects.Test.Data.MostSimpleViewModel") "Most Simple View Models" valuevm))                                  
                                  TProperty
                                      ("ADisabledCollection", 
                                       TObjectJson((makeCollectionMemberSimple "ADisabledCollection" oName "A Disabled Collection" "" ResultTypes.List 2 valued)))                                  
                                  TProperty
                                      ("AnEmptyCollection",                                        
                                       TObjectJson
                                           (makeCollectionMemberSimple "AnEmptyCollection" oName "An Empty Collection" "an empty collection for testing" 
                                                ResultTypes.List 0 emptyValue))                                  
                                  TProperty
                                      ("AnEagerCollection",                                        
                                       TObjectJson
                                           (makeCollectionMemberSimpleTypeValue "AnEagerCollection" oName "An Eager Collection" "" ResultTypes.List 2 mst 
                                                "Most Simples" valuee details))
                                  TProperty("ASet", TObjectJson(makeCollectionMemberSimple "ASet" oName "A Set" "" "set" 2 valueset))                                  
                                  TProperty
                                      ("AnEmptySet", TObjectJson(makeCollectionMemberSimple "AnEmptySet" oName "An Empty Set" "an empty set for testing" "set" 0 emptyValue))
                                  TProperty("Id", TObjectJson(makePropertyMemberSimpleNumber "objects" "Id" oName "Id" "" "int" false (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Collection"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Collections"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetMostSimpleViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
      
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                           ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetFormViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.FormViewModel"
    let vmid = ktc "1--1"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType vmid
    
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, vmid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
        
    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "MostSimple"

    let makeValueParm pmid fid rt =      
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let mp r n = sprintf ";%s=\"%s\"" r n

    let makeParm pmid pid fid rt =      
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst oid) RepresentationTypes.Object mst
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
                          TProperty(JsonPropertyNames.Links,  TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
        TProperty(pmid, p)

    let val3 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" mst oid) RepresentationTypes.Object mst)

    let p2 = makeParm "MostSimple" "Step" "Most Simple" mst
    let p3 = makeValueParm "Name" "Name" "string"

    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(vmid))
          TProperty(JsonPropertyNames.Title, TObjectVal("Untitled Form View Model"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                           ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("MostSimple", TObjectJson(makePropertyMemberShort "objects" "MostSimple" oName "Most Simple" "" mst true val3 []))
                                  TProperty("Name", TObjectJson(makePropertyMemberWithFormat "objects" "Name" oName "Name" "" "string" false (TObjectVal(null))))                           
                                  TProperty("Step", TObjectJson(makeObjectActionMemberSimple "Step" oName oType [ p2; p3 ]))                                                    
                    ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Form View Model"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Form View Models"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithValueViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValueViewModel"
    let ticks = (new DateTime(2012, 2, 10)).Ticks.ToString()
    let tsTicks = (new TimeSpan(1,2,3,4,5)).Ticks.ToString()
    let oid = ktc ("1--100--200--4--0----" + ticks + "--" + tsTicks + "--8--0")
    let oName = sprintf "%s/%s" oType oid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let disabledValue = 
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
        :: (makeObjectPropertyMember "ADisabledValue" oName "A Disabled Value" (TObjectVal(200)))
        
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                             ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "AChoicesValue" oName "A Choices Value" (TObjectVal(0))))                                  
                                  TProperty
                                      ("ADateTimeValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberDateTime "objects" "ADateTimeValue" oName "A Date Time Value" "A datetime value for testing" true 
                                                (TObjectVal("2012-02-10")) "date"))
                                  TProperty
                                      ("ATimeSpanValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberTimeSpan "objects" "ATimeSpanValue" oName "A Time Span Value" "A timespan value for testing" true 
                                                (TObjectVal("02:03:04")) "time"))
                                  TProperty("ADisabledValue", TObjectJson(disabledValue))                                  
                                  TProperty
                                      ("AStringValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::makePropertyMemberString "objects" "AStringValue" oName "A String Value" "A string value for testing" true 
                                                (TObjectVal("")) []))                                  
                                  TProperty
                                      ("AUserDisabledValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) 
                                            :: makeObjectPropertyMember "AUserDisabledValue" oName "A User Disabled Value" (TObjectVal(0))))
                                  TProperty("AValue", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "AValue" oName "A Value" (TObjectVal(100))))
                                  TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value View Model"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Value View Models"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithReferenceViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
    let vmid = ktc "1--1--1--1"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType vmid

    let url = sprintf "http://localhost/objects/%s/%s" oType vmid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, vmid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let pid = "AnEagerReference"
    let ourl = sprintf "objects/%s" oName
    let purl = sprintf "%s/properties/%s" ourl pid

    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let msObj = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
                             TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    let val1 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType)
    let val2 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType)
    let val3 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType)
    let val4 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType (TObjectJson(msObj))) 
    
    let details = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, val4)
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) 
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(vmid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                              ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty
                                      ("AChoicesReference", 
                                       TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AChoicesReference" oName "A Choices Reference" "" roType false val1 []))                                  
                                  TProperty
                                      ("ADisabledReference",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                            :: (makePropertyMemberShort "objects" "ADisabledReference" oName "A Disabled Reference" "" roType false val2 [])))                                  
                                  TProperty
                                      ("ANullReference",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "ANullReference" oName "A Null Reference" "" roType true (TObjectVal(null)) []))
                                  TProperty("AReference", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AReference" oName "A Reference" "" roType false val3 []))                                  
                                  TProperty
                                      ("AnEagerReference", 
                                       TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AnEagerReference" oName "An Eager Reference" "" roType false val4 details))
                                  TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference View Model"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Reference View Models"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetWithNestedViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithNestedViewModel"
    let vmid = ktc "1--1--1--1--1"
    let oid = ktc "1"
    let oName = sprintf "%s/%s" oType vmid
    
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, vmid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AReference"
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AViewModelReference"
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roType1 = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
    let vmid1 = ktc "1--1--1--1"
    let val1 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType)
    let val2 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType1 vmid1) RepresentationTypes.Object roType1)
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(vmid))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" oName) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                           ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AReference", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makePropertyMemberShort "objects" "AReference" oName "A Reference" "" roType false val1 []))                                  
                                  TProperty
                                      ("AViewModelReference",                                   
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::makePropertyMemberShort "objects" "AViewModelReference" oName "A View Model Reference" "" roType1 false val2 []))
                                  TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "Id" oName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Nested View Model"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Nested View Models"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithReferenceViewModelEdit(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModelEdit"
    let vmid1 = ktc "1--1--1--1"
    let vmid2 = ktc "2--1--1--2"
    let oName2 = oType + "/" + vmid2

    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid1 = ktc "1"
    let roid2 = ktc "2"
    let roName = roType + "/" + roid2
    let pid = "AnEagerReference"
    let ourl = sprintf "objects/%s" oName2
    let purl = sprintf "%s/properties/%s" ourl pid
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AnEagerReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid1
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, vmid1, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let args1 = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let msObj = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(roid2))
          TProperty(JsonPropertyNames.Title, TObjectVal("2"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roName) RepresentationTypes.Object roType)
                             TObjectJson(sb(roType)); TObjectJson(sp(roType))
                             TObjectJson(args1 :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roName) RepresentationTypes.Object roType) ]))
          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roName "Id" (TObjectVal(2)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "ADisabledReference"
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
    let valueRel3 = RelValues.Value + makeParm RelParamValues.Property "AReference"
    let valueRel4 = RelValues.Value + makeParm RelParamValues.Property "AnEagerReference"
    let val1 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType roid1) RepresentationTypes.Object roType)
    let val2 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType roid2) RepresentationTypes.Object roType)
    let val3 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" roType roid2) RepresentationTypes.Object roType)
    let val4 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeLinkPropWithMethodValue "GET" valueRel4 (sprintf "objects/%s/%s" roType roid2) RepresentationTypes.Object roType (TObjectJson(msObj)))
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnEagerReference"
    
    let details = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal("AnEagerReference"))
          TProperty(JsonPropertyNames.Value, val4)
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                                  
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Eager Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(roType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(vmid2))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
                              ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty
                                      ("AChoicesReference", 
                                       TObjectJson(makePropertyMemberShort "objects" "AChoicesReference" oName2 "A Choices Reference" "" roType false val2 []))                                  
                                  TProperty
                                      ("ADisabledReference",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
                                            :: (makePropertyMemberShort "objects" "ADisabledReference" oName2 "A Disabled Reference" "" roType false val1 [])))                                  
                                  TProperty
                                      ("ANullReference",                                        
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "ANullReference" oName2 "A Null Reference" "" roType true (TObjectVal(null)) []))                                  
                                  TProperty
                                      ("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" oName2 "A Reference" "" roType false val3 []))                                  
                                  TProperty
                                      ("AnEagerReference", 
                                       TObjectJson(makePropertyMemberShort "objects" "AnEagerReference" oName2 "An Eager Reference" "" roType false val4 details))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" oName2 "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference View Model Edit"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Reference View Model Edits"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithNestedViewModelEdit(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithNestedViewModelEdit"
    let vmid1 = ktc "1--1--1--1--1"
    let vmid2 = ktc "2--2--1--1--2"
    
    let roName = oType + "/" + vmid2
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roType1 = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "2")))).ToString()))
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType1 (ktc "2--1--1--2")))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))), 
                    new JProperty("AViewModelReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))

    let args = CreateArgMapWithReserved props
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid1
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, vmid1, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let valueRel1 = RelValues.Value + makeParm RelParamValues.Property "AReference"
    let valueRel2 = RelValues.Value + makeParm RelParamValues.Property "AViewModelReference"
    let val1 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeGetLinkProp valueRel1 (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType)
    let val2 = 
        TObjectJson
            (TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
             :: makeGetLinkProp valueRel2 (sprintf "objects/%s/%s" roType1 (ktc "2--1--1--2")) RepresentationTypes.Object roType1)
       
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(vmid2))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
                           ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty
                                      ("AReference", TObjectJson(makePropertyMemberShort "objects" "AReference" roName "A Reference" "" roType false val1 []))
                                  
                                  TProperty
                                      ("AViewModelReference", 
                                       
                                       TObjectJson
                                           (makePropertyMemberShort "objects" "AViewModelReference" roName "A View Model Reference" "" roType1 false val2 []))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Nested View Model Edit"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Nested View Model Edits"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithValueViewModelEdit(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValueViewModelEdit"
    let ticks = (new DateTime(2012, 2, 10)).Ticks.ToString()
    let ticksTs = (new TimeSpan(2, 3, 4)).Ticks.ToString()
    let vmid1 = ktc ("1--100--200--4--0----" + ticks + "--" + ticksTs + "--0--2")
    let vmid2 = ktc ("1--222--200--4--333----" + ticks + "--" + ticksTs + "--0--2")   
    let roName = oType + "/" + vmid2
    
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid1
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, vmid1, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let disabledValue = 
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable")) 
        :: (makeObjectPropertyMember "ADisabledValue" roName "A Disabled Value" (TObjectVal(200)))
       
    let expected = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(vmid2))
          TProperty(JsonPropertyNames.Title, TObjectVal("1"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(sb(oType)); TObjectJson(sp(oType))
                              ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("AChoicesValue", TObjectJson(makeObjectPropertyMember "AChoicesValue" roName "A Choices Value" (TObjectVal(333))))                                  
                                  TProperty
                                      ("ADateTimeValue",                                        
                                       TObjectJson
                                           (makePropertyMemberDateTime "objects" "ADateTimeValue" roName "A Date Time Value" "A datetime value for testing" true 
                                                (TObjectVal("2012-02-10")) "date"))
                                  TProperty
                                      ("ATimeSpanValue",                                        
                                       TObjectJson
                                           (makePropertyMemberTimeSpan "objects" "ATimeSpanValue" roName "A Time Span Value" "A timespan value for testing" true 
                                                (TObjectVal("02:03:04")) "time"))
                                  TProperty("ADisabledValue", TObjectJson(disabledValue))                                  
                                  TProperty
                                      ("AStringValue",                                        
                                       TObjectJson
                                           (makePropertyMemberString "objects" "AStringValue" roName "A String Value" "A string value for testing" true 
                                                (TObjectVal("")) []))                                  
                                  TProperty
                                      ("AUserDisabledValue",                                        
                                       TObjectJson
                                           (TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit")) 
                                            :: makeObjectPropertyMember "AUserDisabledValue" roName "A User Disabled Value" (TObjectVal(2))))
                                  TProperty("AValue", TObjectJson(makeObjectPropertyMember "AValue" roName "A Value" (TObjectVal(222))))
                                  TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roName "Id" (TObjectVal(1)))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value View Model Edit"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("With Value View Model Edits"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Object, oType), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutWithReferenceViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
    let vmid1 = ktc "1--1--1--1"
    let oid = ktc "2"
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType oid))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AnEagerReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid1
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, vmid1, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult

    Assert.AreEqual("199 RestfulObjects \"Field disabled as object cannot be changed\",199 RestfulObjects \"Field disabled as object cannot be changed\",199 RestfulObjects \"Field disabled as object cannot be changed\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithNestedViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithNestedViewModel"
    let vmid1 = ktc "1--1--1--1--1"
    let vmid2 = ktc "2--1--1--2"
    let oid = ktc "2"
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roType1 = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType oid))).ToString()))
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType1 vmid2))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))), 
                    new JProperty("AViewModelReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid1
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, vmid1, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field disabled as object cannot be changed\",199 RestfulObjects \"Field disabled as object cannot be changed\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValueViewModel"
    let ticks = (new DateTime(2012, 2, 10)).Ticks.ToString()
    let ticksTs = (new TimeSpan(2, 3, 4)).Ticks.ToString()
    let vmid1 = ktc ("1--100--200--4--0----" + ticks + "--" + ticksTs + "--0--2")
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    let url = sprintf "http://localhost/objects/%s/%s" oType vmid1
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, vmid1, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field disabled as object cannot be changed\",199 RestfulObjects \"Field disabled as object cannot be changed\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400    
let InvalidGetObject(api : RestfulObjectsControllerBase) = 
    let oid = " " // invalid 
    let oid1 = ktc "1"
    let oType = ttc " " // invalid

    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid1)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedObjects.Facade.BadRequestNOSException' was thrown.\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400
let PutWithValueObjectMissingArgs(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/objects/%s/%s" oType oid
    jsonSetPutMsg api.Request url ""
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueObjectMissingArgsValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"

    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = new JObject(new JProperty("x-ro-validate-only", true))
    let args = CreateArgMapWithReserved props

    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
      
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400    
let PutWithValueObjectMalformedArgs(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty("malformed", 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueObjectMalformedDateTimeArgs(api : RestfulObjectsControllerBase) = 
    let error = "cannot format value cannot parse as date as DateTime"
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"

    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("ADateTimeValue", new JObject(new JProperty(JsonPropertyNames.Value, "cannot parse as date"))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("ADateTimeValue", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("cannot parse as date"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let PutWithValueObjectMalformedTimeArgs(api : RestfulObjectsControllerBase) = 
    let error = "cannot format value cannot parse as time as TimeSpan"
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("ATimeSpanValue", new JObject(new JProperty(JsonPropertyNames.Value, "cannot parse as time"))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("ATimeSpanValue", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("cannot parse as time"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let PutWithValueObjectMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty("malformed", 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))),
                    new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.IsNull(headers.ContentType)
    Assert.AreEqual("", jsonResult)

// 400    
let PutWithValueObjectInvalidArgsValue(api : RestfulObjectsControllerBase) = 
    let error = "cannot format value invalid value as Int32"
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("AValue", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
   
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    compareObject expected parsedResult

let PutWithValueObjectInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let error = "cannot format value invalid value as Int32"
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))),
                    new JProperty("x-ro-validate-only", true))

    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("AValue", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(333)) ])) ]
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 400    
let PutWithReferenceObjectInvalidArgsValue(api : RestfulObjectsControllerBase) = 
    let error = "Not a suitable type; must be a Most Simple"
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let wvt = (ttc "RestfulObjects.Test.Data.WithValue")
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" wvt oid)).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("AReference", 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Value, 
                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt oid))) ]))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
          TProperty("AChoicesReference", 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Value, 
                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt oid))) ]))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ])) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].First().ToString())
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].Skip(1).First().ToString())
    compareObject expected parsedResult

let PutWithReferenceObjectNotFoundArgsValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" oType (ktc "100"))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult

    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain object %s-%s: null adapter\"" oType (ktc "100"), headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithReferenceObjectInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let error = "Not a suitable type; must be a Most Simple"
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let wvt = (ttc "RestfulObjects.Test.Data.WithValue")
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" wvt oid)).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))),
                    new JProperty("x-ro-validate-only", true), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("AReference", 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Value, 
                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt oid))) ]))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]))
          TProperty("AChoicesReference", 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Value, 
                                       TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt oid))) ]))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ])) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].First().ToString())
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].Skip(1).First().ToString())
    compareObject expected parsedResult

let PutWithReferenceObjectMalformedArgs(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, "malformed"))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithReferenceObjectMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, "malformed"))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))),
                    new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400    
let PutWithValueObjectFailCrossValidation(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 101))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 3))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(101)) ]))
          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(3)) ]))
          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let PutWithValueObjectFailCrossValidationValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 101))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 3))),
                    new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty("AValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(101)) ]))
          TProperty("AChoicesValue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(3)) ]))
          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 400    
let PutWithReferenceObjectFailsCrossValidation(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oid2 = ktc "2"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType oid))).ToString()))
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType oid2))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty
              ("AReference",                
               TObjectJson
                   ([ TProperty
                          (JsonPropertyNames.Value, 
                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType oid)))) ])) ]))          
          TProperty
              ("AChoicesReference",                
               TObjectJson
                   ([ TProperty
                          (JsonPropertyNames.Value, 
                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType oid2)))) ])) ]))
          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult  
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let PutWithReferenceObjectFailsCrossValidationValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oid2 = ktc "2"

    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType oid))).ToString()))
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType oid2))).ToString()))
    let props = 
        new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref1))),
                    new JProperty("x-ro-validate-only", true), 
                    new JProperty("AChoicesReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty
              ("AReference",                
               TObjectJson
                   ([ TProperty
                          (JsonPropertyNames.Value, 
                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType oid)))) ])) ]))          
          TProperty
              ("AChoicesReference",                
               TObjectJson
                   ([ TProperty
                          (JsonPropertyNames.Value, 
                           TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType((sprintf "objects/%s/%s" roType oid2)))) ])) ]))
          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 401    
let PutWithValueObjectDisabledValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = new JObject(new JProperty("ADisabledValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueObjectDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("ADisabledValue", 
                        new JObject(new JProperty(JsonPropertyNames.Value, 333))),
                                    new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 401     
let PutWithReferenceObjectDisabledValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (oid)))).ToString()))
    let props = new JObject(new JProperty("ADisabledReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithReferenceObjectDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (oid)))).ToString()))
    let props = 
        new JObject(new JProperty("ADisabledReference",
                        new JObject(new JProperty(JsonPropertyNames.Value, ref2))), 
                                    new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let PutWithValueObjectInvisibleValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = new JObject(new JProperty("AHiddenValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueObjectInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = new JObject(new JProperty("AHiddenValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404     
let PutWithReferenceObjectInvisibleValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (oid)))).ToString()))
    let props = new JObject(new JProperty("AHiddenReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithReferenceObjectInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (oid)))).ToString()))
    let props = 
        new JObject(new JProperty("AHiddenReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404  
let PutWithValueObjectInvalidArgsName(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("ANonExistentValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueObjectInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("ANonExistentValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))),
                    new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404
let ObjectNotFoundWrongKey(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "100"
    let oName = sprintf "%s/%s" oType oid

    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))),
                    new JProperty("ATimeSpanValue", new JObject(new JProperty(JsonPropertyNames.Value, "04:05:06"))),  
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
   
    let args = CreateArgMapWithReserved props
    let url = sprintf "http://localhost/objects/%s" oName
    jsonSetPutMsg api.Request url (props.ToString())
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// 404    
let ObjectNotFoundWrongType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.DoesNotExist"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let ObjectNotFoundAbstractType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
  
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// 404     
let NotFoundGetObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "44"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain object %s-%s: null adapter\"" oType (ktc "44"), headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 405   
let PutWithValueImmutableObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let PutWithReferenceImmutableObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType oid))).ToString()))
    let props = new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))), new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let PutWithReferenceImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (oid)))).ToString()))
    let props = new JObject(new JProperty("AReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))), new JProperty("x-ro-validate-only", true))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 406     
let NotAcceptablePutObjectWrongMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
     
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
        
    let args = CreateArgMapWithReserved props
    jsonSetPutMsgWithProfile api.Request url (props.ToString()) RepresentationTypes.ObjectCollection
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    Assert.AreEqual("", jsonResult)


let NotAcceptableGetObjectWrongMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
   
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    Assert.AreEqual("", jsonResult)
      
// was notacceptable is now ignored v69 of spec 
let GetObjectIgnoreWrongDomainType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let dType = "\"http://localhost/domain-types/RestfulObjects.Test.Data.WithValue\""
    let oid = ktc "1"

    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName

    let mtParms = [|("profile",  RepresentationTypes.Object);("x-ro-domain-type", dType)|]
    jsonSetGetMsgAndMediaType api.Request url "application/json" mtParms
    let result = api.GetObject(oType, oid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult

// 500    
let PutWithValueInternalError(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "4"

    let oName = "RestfulObjects.Test.Data.WithError/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = new JObject(new JProperty("AnErrorValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    WithError.ThrowErrors <- true

    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)   
    
    WithError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([ TObjectVal(new errorType(" at  in "))
                             TObjectVal(new errorType(" at  in ")) ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult

    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 500    
let PutWithReferenceInternalError(api : RestfulObjectsControllerBase) = 
    
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "3"

    let oName = "RestfulObjects.Test.Data.WithError/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ref2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (oid)))).ToString()))
    let props = new JObject(new JProperty("AnErrorReference", new JObject(new JProperty(JsonPropertyNames.Value, ref2))))
    
    WithError.ThrowErrors <- true
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request "*"
    let result = api.PutObject(oType, oid, args)   
        
    WithError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let arr1 = [ for _ in 1 .. 5 ->   TObjectVal(new errorType(" at  in ")) ]
    let arr2 = [ for _ in 1 .. 4 ->   TObjectVal(new errorType(" at  in ")) ]
    let arr3 = [ for _ in 1 .. 6 ->   TObjectVal(new errorType(" at  in ")) ]

    let expected1 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace,   TArray(arr1))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    let expected2 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace,   TArray(arr2))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    let expected3 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, TArray(arr3))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    
    try
        compareObject expected1 parsedResult
    with e -> 
        try 
            compareObject expected2 parsedResult
        with e -> 
            compareObject expected3 parsedResult

let PutWithValueObjectConcurrencyFail(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    let tag = "\"fail\""
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    setIfMatch api.Request tag
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.PreconditionFailed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValueObjectMissingIfMatch(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    let url = sprintf "http://localhost/objects/%s" oName
    let props = 
        new JObject(new JProperty("AValue", new JObject(new JProperty(JsonPropertyNames.Value, 222))), 
                    new JProperty("AChoicesValue", new JObject(new JProperty(JsonPropertyNames.Value, 333))))
    
    let args = CreateArgMapWithReserved props
    jsonSetPutMsg api.Request url (props.ToString())
    let result = api.PutObject(oType, oid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode preconditionHeaderMissing statusCode jsonResult
    Assert.AreEqual
        ("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", 
         headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)