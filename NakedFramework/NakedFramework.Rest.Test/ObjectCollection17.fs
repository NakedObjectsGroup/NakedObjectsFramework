// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.ObjectCollection17

open NUnit.Framework
open System.Net
open Newtonsoft.Json.Linq
open System.Web
open NakedFramework.Rest.Snapshot.Utility
open NakedFramework.Rest.Snapshot.Constants
open NakedFramework.Rest.API
open System
open Functions

let GetCollectionProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let oid2 = ktc "2"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))          
          membersProp((sprintf "%s/%s" oType oid), roType)
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)                      
                      ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionPropertyWithInlineFlag(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let oid2 = ktc "2"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let argS = RestControlFlags.InlineCollectionItemsReserved + "=true"
    let url = sprintf "http://localhost/%s?%s" purl  argS
    
    jsonSetGetMsg api.Request url
    api.InlineCollectionItems <- new Nullable<bool>(true)
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
      
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    
    let m1 = TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeTablePropertyMember "Id" (TObjectVal(1)))) ]))
    let m2 = TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeTablePropertyMember "Id" (TObjectVal(2)))) ]))

    let obj1 =
        m1 :: 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) ::
        makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        m2 :: 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) ::
        makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          membersProp((sprintf "%s/%s" oType oid), roType)
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)                      
                      ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollectionViewModel"
    let oid = ktc "1--2"
    let oid1 = ktc "1"
    let oid2 = ktc "2"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid1) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)                      
                       ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionPropertySimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let argS = "x-ro-domain-model=simple"
    let url = sprintf "http://localhost/%s?%s" purl  argS
    
    jsonSetGetMsg api.Request url
    api.DomainModel <- "simple"
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          membersProp((sprintf "%s/%s" oType oid), roType)
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionSetProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ASet"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Set"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))         
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)                      
                      ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionSetPropertySimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ASet"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let argS = "x-ro-domain-model=simple"
    let url = sprintf "http://localhost/%s?%s" purl  argS
    
    jsonSetGetMsg api.Request url
    api.DomainModel <- "simple"
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Set"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionPropertyWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          membersProp((sprintf "%s/%s" oType oid), roType)
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)                      
                      ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetDisabledCollectionProperty(api : RestfulObjectsControllerBase) = 
    let msg = "Field not editable"
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ADisabledCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Disabled Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal(msg))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)                      
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let oid2 = ktc "2"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let vurl = sprintf "%s/collections/%s/value" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url
    let result = api.GetCollectionValue(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    
    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))
          TProperty(JsonPropertyNames.Value, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self vurl RepresentationTypes.CollectionValue "" "" true)
                             ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.CollectionValue, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let AddToCollectionProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "AnEmptyCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid)).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let arg = CreateSingleValueArgWithReserved parms
    let url = sprintf "http://localhost/%s" purl

    jsonSetPostMsg api.Request url (parms.ToString())
    let result =  api.PostCollection(oType, oid, pid, arg) 
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let DeleteFromCollectionProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "AnEmptyCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid)).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let arg = CreateSingleValueArgWithReserved parms
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
   
    jsonSetDeleteMsg api.Request url
    let result = api.DeleteCollection(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let AddToCollectionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollectionViewModel"
    let oid = ktc "1-2"
    let oid1 = ktc "1"
    let pid = "AnEmptyCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid1)).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let arg = CreateSingleValueArgWithReserved parms
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetPostMsg api.Request url (parms.ToString())
    let result =  api.PostCollection(oType, oid, pid, arg) 
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let DeleteFromCollectionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollectionViewModel"
    let oid = ktc "1-2"
    let oid1 = ktc "1"
    let pid = "AnEmptyCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid1)).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let arg = CreateSingleValueArgWithReserved parms
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    
    jsonSetDeleteMsg api.Request url
    let result = api.DeleteCollection(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let GetInvalidCollection(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = " " // invalid 
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedFramework.Facade.Error.BadRequestNOSException' was thrown.\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetNotFoundCollection(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ANonExistentCollection" // doesn't exist 
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such collection ANonExistentCollection\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetHiddenValueCollection(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "AHiddenCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such collection AHiddenCollection\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetNakedObjectsIgnoredCollection(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ANakedObjectsIgnoredCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such collection ANakedObjectsIgnoredCollection\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotAcceptableGetCollectionWrongMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectProperty
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let GetErrorValueCollection(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithGetError"
    let oid = ktc "1"
    let pid = "AnErrorCollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/collections/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- true
    let result = api.GetCollection(oType, oid, pid)
    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    // for some resaon stack trace has different depth on my machine when not debugging (only) ! 
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())

let GetCollectionAsProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let oid = ktc "1"
    let pid = "ACollection"
    let ourl = sprintf "objects/%s/%s" oType oid
    let url = sprintf "http://localhost/%s" ourl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ACollection\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)
