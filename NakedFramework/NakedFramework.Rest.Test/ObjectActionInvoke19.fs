// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.ObjectActionInvoke19

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open System.Web
open System
open NakedObjects.Rest.Snapshot.Utility
open NakedObjects.Rest.Snapshot.Constants
open System.Web.Http
open System.Linq
open NakedObjects.Rest.Test.Functions

// 19.3 post to invoke non-idempotent action no parms 
let VerifyPostInvokeActionReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl

    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
       
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeActionReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnObject "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnObject "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnObject "objects" oType oName api.PostInvoke api

let VerifyPostInvokeOverloadedActionReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnOverloadedAction0"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeOverloadedActionReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeOverloadedActionReturnObject "objects" oType oName api.PostInvoke api

let PostInvokeOverloadedActionReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeOverloadedActionReturnObject "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeOverloadedActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeOverloadedActionReturnObject "objects" oType oName api.PostInvoke api

let VerifyPostInvokeContributedService refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ANonContributedAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeActionContributedService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.ContributorService"
    let oName = oType
    VerifyPostInvokeContributedService "services" oType oName (wrap3 api.PostInvokeOnService) api

let VerifyPostInvokeCollectionContributedActionContributedService refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ACollectionContributedActionParm"
    let ourl = sprintf "%s/%s" refType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"

    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))   
    let colParm = new JArray([refParm])
    let parms = 
        new JObject(new JProperty("ms", new JObject(new JProperty(JsonPropertyNames.Value, colParm))), 
                    new JProperty("id", new JObject(new JProperty(JsonPropertyNames.Value, 1))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeCollectionContributedActionContributedService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.ContributorService"
    let oName = oType
    VerifyPostInvokeCollectionContributedActionContributedService "services" oType oName (wrap3 api.PostInvokeOnService) api

let VerifyPostInvokeCollectionContributedActionContributedServiceMissingParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "ACollectionContributedActionParm"
    let ourl = sprintf "%s/%s" refType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"

    let msHref = new hrefType((sprintf "objects/%s/%s" roType (oid)));
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, msHref.ToString()))   
    let colParm = new JArray([refParm])
    let parms = 
        new JObject(new JProperty("ms", new JObject(new JProperty(JsonPropertyNames.Value, colParm))), 
                    new JProperty("id", new JObject(new JProperty(JsonPropertyNames.Value, ""))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved parms
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
          
    let expected = 
        [ TProperty("id", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("ms", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TArray([TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(msHref.ToString()) ) ])])) ])) ]

    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let PostInvokeCollectionContributedActionContributedServiceMissingParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.ContributorService"
    let oName = oType
    VerifyPostInvokeCollectionContributedActionContributedServiceMissingParm "services" oType oName (wrap3 api.PostInvokeOnService) api

let VerifyPostInvokeActionReturnRedirectedObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsRedirectedObject"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.MovedPermanently statusCode jsonResult
    Assert.AreEqual(new Uri("http://redirectedtoserver/objects/RedirectedToOid"), headers.Location)

let PostInvokeActionReturnRedirectedObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnRedirectedObject "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnRedirectedObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnRedirectedObject "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnRedirectedObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnRedirectedObject "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnViewModel refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsViewModel"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
    let roid = roType + "/" + oid
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                       ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeActionReturnViewModelObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnViewModel "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnViewModelService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnViewModel "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnViewModelViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnViewModel "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnObjectConcurrencySuccess refType oType oName f tag (api : RestfulObjectsControllerBase)  = 
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request tag
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetTag oType oName (api : RestfulObjectsControllerBase) = 
    let url = sprintf "http://localhost/objects/%s" oName
    jsonSetGetMsg api.Request url
    let result = api.GetObject(oType, ktc "1")
    let (_, _, headers) = readActionResult result api.ControllerContext.HttpContext
    headers.ETag.Tag.ToString()

let PostInvokeActionReturnObjectObjectConcurrencySuccess(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnObjectConcurrencySuccess "objects" oType oName api2.PostInvoke (GetTag oType oName api1) api2

let PostInvokeActionReturnObjectServiceConcurrencySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnObjectConcurrencySuccess "services" oType oName (wrap3 api.PostInvokeOnService) "\"any\"" api

let PostInvokeActionReturnObjectViewModelConcurrencySuccess(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnObjectConcurrencySuccess "objects" oType oName api2.PostInvoke (GetTag oType oName api1) api2

let VerifyPostInvokeUserDisabledActionReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeUserDisabledActionReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeUserDisabledActionReturnObject "objects" oType oName api.PostInvoke api

let PostInvokeUserDisabledActionReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeUserDisabledActionReturnObject "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeUserDisabledActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeUserDisabledActionReturnObject "objects" oType oName api.PostInvoke api

let PostInvokeContribActionReturnObject(api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + oid
    let pid = "AzContributedAction"
    let ourl = sprintf "%s/%s" "objects" oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.PostInvoke (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeContribActionReturnObjectBaseClass(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oid = ktc "1"
    let pid = "AzContributedActionOnBaseClass"
    let ourl = sprintf "%s/%s" "objects" oid
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.PostInvoke (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeContribActionReturnObjectWithRefParm(api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/1"
    let pid = "AzContributedActionWithRefParm"
    let ourl = sprintf "%s/%s" "objects" oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" oType (ktc "1"))).ToString()))
    let parms = new JObject(new JProperty("withOtherAction", new JObject(new JProperty(JsonPropertyNames.Value, refParm))))
    
    let args = CreateArgMapWithReserved parms
    let url = sprintf "http://localhost/%s" purl
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PostInvoke (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)


    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeContribActionReturnObjectWithValueParm(api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + oid
    let pid = "AzContributedActionWithValueParm"
    let ourl = sprintf "%s/%s" "objects" oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("parm", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))))
    
    let args = CreateArgMapWithReserved parms
    let url = sprintf "http://localhost/%s" purl
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PostInvoke (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + oid
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let VerifyPostInvokeActionReturnNullObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsNull"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
     
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, TObjectVal(null))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnNullObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = ktc "1"
    VerifyPostInvokeActionReturnNullObject "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnNullObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnNullObject "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnNullObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnNullObject "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnNullViewModel refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsNullViewModel"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, TObjectVal(null))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnNullViewModelObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = ktc "1"
    VerifyPostInvokeActionReturnNullViewModel "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnNullViewModelService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnNullViewModel "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnNullViewModelViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnNullViewModel "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnObjectValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnObjectValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnObjectValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnObjectValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPutInvokeActionReturnObject refType oType oName f (api : RestfulObjectsControllerBase) =
    let oid = ktc "1"
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPutMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutInvokeActionReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutInvokeActionReturnObject "objects" oType oName api.PutInvoke api

let PutInvokeActionReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutInvokeActionReturnObject "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutInvokeActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutInvokeActionReturnObject "objects" oType oName api.PutInvoke api

let VerifyPutInvokeActionReturnViewModel refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionAnnotatedIdempotentReturnsViewModel"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPutMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
     
    let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
    let roid = roType + "/" + ktc "1"
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(oid))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                       ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) :: makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutInvokeActionReturnViewModelObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutInvokeActionReturnViewModel "objects" oType oName api.PutInvoke api

let PutInvokeActionReturnViewModelService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutInvokeActionReturnViewModel "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutInvokeActionReturnViewModelViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutInvokeActionReturnViewModel "objects" oType oName api.PutInvoke api

let VerifyPutInvokeActionReturnObjectConcurrencySuccess refType oType oName f tag (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPutMsg api.Request url
    setIfMatch api.Request tag
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutInvokeActionReturnObjectObjectConcurrencySuccess(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase)= 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutInvokeActionReturnObjectConcurrencySuccess "objects" oType oName api2.PutInvoke (GetTag oType oName api1) api2

let PutInvokeActionReturnObjectServiceConcurrencySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutInvokeActionReturnObjectConcurrencySuccess "services" oType oName (wrap3 api.PutInvokeOnService) "\"any\"" api

let PutInvokeActionReturnObjectViewModelConcurrencySuccess(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase)= 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutInvokeActionReturnObjectConcurrencySuccess "objects" oType oName api2.PutInvoke (GetTag oType oName api1) api2

let VerifyPutInvokeActionReturnNullObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedIdempotentReturnsNull"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPutMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)


    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, TObjectVal(null))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PutInvokeActionReturnNullObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutInvokeActionReturnNullObject "objects" oType oName api.PutInvoke api

let PutInvokeActionReturnNullObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutInvokeActionReturnNullObject "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutInvokeActionReturnNullObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutInvokeActionReturnNullObject "objects" oType oName api.PutInvoke api

let VerifyPutInvokeActionReturnObjectValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PutInvokeActionReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutInvokeActionReturnObjectValidateOnly "objects" oType oName api.PutInvoke api

let PutInvokeActionReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutInvokeActionReturnObjectValidateOnly "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutInvokeActionReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutInvokeActionReturnObjectValidateOnly "objects" oType oName api.PutInvoke api

let VerifyGetInvokeActionReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
      
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))                      
                      TProperty
                          (JsonPropertyNames.Links,                            
                           TArray
                               ([ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" true)                              
                                  TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                  TObjectJson
                                      (args 
                                       :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" 
                                              true) ]))                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetInvokeActionReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnObject "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnObject "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnObject "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionReturnViewModel refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedQueryOnlyReturnsViewModel"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
    let roid = roType + "/" + ktc "1"
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           
                           TArray
                               ([ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" true)
                                  TObjectJson(sb(roType))
                                  TObjectJson(sp(roType))                                  
                                ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed")) ::  makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetInvokeActionReturnViewModelObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnViewModel "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnViewModelService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnViewModel "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnViewModelViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnViewModel "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionReturnObjectConcurrencySuccess refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
     
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    setIfMatch api.Request  "\"any\""
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           
                           TArray
                               ([ TObjectJson
                                      (makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" true)
                                  TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                  
                                  TObjectJson
                                      (args 
                                       :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" 
                                              true) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetInvokeActionReturnObjectObjectConcurrencySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnObjectConcurrencySuccess "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnObjectServiceConcurrencySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnObjectConcurrencySuccess "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnObjectViewModelConcurrencySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnObjectConcurrencySuccess "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))                      
                      TProperty
                          (JsonPropertyNames.Links,                            
                           TArray
                               ([ TObjectJson
                                      (makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" true)
                                  TObjectJson(sb(roType)); TObjectJson(sp(roType))                                  
                                  TObjectJson
                                      (args 
                                       :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" 
                                              true) ]))                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionReturnNullObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedQueryOnlyReturnsNull"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, TObjectVal(null))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetInvokeActionReturnNullObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnNullObject "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnNullObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnNullObject "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnNullObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnNullObject "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionReturnObjectValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s" refType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let GetInvokeActionReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnObjectValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnObjectValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionReturnObjectValidateOnly "objects" oType oName api.GetInvoke api

let VerifyPostInvokeActionReturnObjectWithMediaType refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
     
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsgWithProfile api.Request url RepresentationTypes.ActionResult
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeActionReturnObjectObjectWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnObjectWithMediaType "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnObjectServiceWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnObjectWithMediaType "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnObjectViewModelWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnObjectWithMediaType "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnScalar refType oType oName f (api : RestfulObjectsControllerBase) = 
    
    let pid = "AnActionReturnsScalar"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url 
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
        
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(999))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           TArray([ ]))
                      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "number", "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnScalarObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnScalar "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnScalarService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnScalar "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnScalarViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnScalar "objects" oType oName api.PostInvoke api



let VerifyPostInvokeActionReturnEmptyScalar refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsScalarEmpty"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url 
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "string"
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(""))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnEmptyScalarObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnEmptyScalar "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnEmptyScalarService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnEmptyScalar "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnEmptyScalarViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnEmptyScalar "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnNullScalar refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsScalarNull"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid

    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url 
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
     
    let roType = "string"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar))
          TProperty(JsonPropertyNames.Result, TObjectVal(null))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnNullScalarObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnNullScalar "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnNullScalarService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnNullScalar "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnNullScalarViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnNullScalar "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnScalarValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsScalar"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(parms)
    let url = sprintf "http://localhost/%s" purl
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnScalarObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnScalarValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnScalarServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnScalarValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnScalarViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnScalarValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnVoid refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoid"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url 
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Void))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
   
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnVoidObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnVoid "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnVoidService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnVoid "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnVoidViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnVoid "objects" oType oName api.PostInvoke api



let VerifyPostInvokeActionReturnVoidValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoid"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(parms)
    let url = sprintf "http://localhost/%s" purl
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnVoidObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnVoidValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnVoidServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnVoidValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnVoidViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnVoidValidateOnly "objects" oType oName api.PostInvoke api

let VerifyGetInvokeActionReturnQueryable refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryable"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
     
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let contribName = ttc "RestfulObjects.Test.Data.ContributorService"
    
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType "" true
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType "" true
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           "" roType true) ])
    
    let resultProp = 
        TProperty(JsonPropertyNames.Value, 
                  TArray([ TObjectJson(obj1)
                           TObjectJson(obj2) ]))
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty("pageSize", TObjectVal(20)) 
                               TProperty("numPages", TObjectVal(1)) 
                               TProperty("totalCount", TObjectVal(2))]))

    
    
    let p1 = makeInvokeCollectionParm "ms" "Ms" roType
    let p2 = makeInvokeCollectionParm "ms" "Ms" roType
    let p3 = makeInvokeValueParm "id" "Id"

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("ACollectionContributedActionNoParms", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionNoParms" contribName roType [ p1 ]))
                                                           TProperty("ACollectionContributedActionParm", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionParm" contribName roType [ p2; p3 ]))]))


    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ pageProp
                                  membersProp
                                  resultProp                         
                                  TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([ ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([]) )]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetInvokeActionReturnQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnQueryable "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnQueryable "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnQueryable "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionReturnQueryableWithPaging refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryable"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-page", 2), new JProperty("x-ro-pageSize", 1))
    
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(parms)
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType "" true
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           "" roType true) ])
    
    let resultProp = 
        TProperty(JsonPropertyNames.Value, 
                  TArray([ TObjectJson(obj2) ]))
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(2)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(2)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))
    
    let contribName = ttc "RestfulObjects.Test.Data.ContributorService"

    let p1 = makeInvokeCollectionParm "ms" "Ms" roType
    let p2 = makeInvokeCollectionParm "ms" "Ms" roType
    let p3 = makeInvokeValueParm "id" "Id" 

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("ACollectionContributedActionNoParms", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionNoParms" contribName roType [ p1 ]))
                                                           TProperty("ACollectionContributedActionParm", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionParm" contribName roType [ p2; p3 ]))]))


    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ pageProp
                                  membersProp
                                  resultProp                         
                                  TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetInvokeActionReturnQueryObjectWithPaging(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnQueryableWithPaging "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnQueryServiceWithPaging(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnQueryableWithPaging "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnQueryViewModelWithPaging(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnQueryableWithPaging "objects" oType oName api.GetInvoke api


let VerifyGetInvokeActionReturnQueryableValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryable"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))    
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(parms)
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let GetInvokeActionReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnQueryableValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnQueryableValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionReturnQueryableValidateOnly "objects" oType oName api.GetInvoke api

let VerifyPostInvokeActionReturnCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollection"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))
    
    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([]))

    let resultProp = 
        TProperty(JsonPropertyNames.Value, 
                  TArray([ TObjectJson(obj1)
                           TObjectJson(obj2) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ pageProp
                                  membersProp
                                  resultProp                                  
                                  TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnCollection "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnCollection "objects" oType oName api.PostInvoke api



let VerifyPostInvokeActionReturnEmptyCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionEmpty"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid    
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let resultProp = TProperty(JsonPropertyNames.Value, TArray([]))
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(0))]))

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([]))

    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ pageProp
                                  membersProp
                                  resultProp                                  
                                  TProperty
                                      (JsonPropertyNames.Links,                                        
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnEmptyCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnEmptyCollection "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnEmptyCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnEmptyCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnEmptyCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnEmptyCollection "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnNullCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionNull"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, TObjectVal(null))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnNullCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnNullCollection "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnNullCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnNullCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnNullCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnNullCollection "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnCollectionVerifyOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollection"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnCollectionObjectVerifyOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnCollectionVerifyOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnCollectionServiceVerifyOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnCollectionVerifyOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnCollectionViewModelVerifyOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnCollectionVerifyOnly "objects" oType oName api.PostInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnQuerySimple refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = "parm2=fred&parm1=100"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapFromUrl parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType "" true
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType "" true
    
    let args = 
        TObjectJson([ TProperty("parm1", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(100)) ]))
                      TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, args) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           "" roType true) ])
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))
    
    let contribName = ttc "RestfulObjects.Test.Data.ContributorService"
    
    let p1 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p2 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p3 = makeInvokeValueParm "id" "Id"

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("ACollectionContributedActionNoParms", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionNoParms" contribName roType [ p1 ]))
                                                           TProperty("ACollectionContributedActionParm", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionParm" contribName roType [ p2; p3 ]))]))

    let resultProp = 
        TProperty(JsonPropertyNames.Value, 
                  TArray([ TObjectJson(obj1)
                           TObjectJson(obj2) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ pageProp
                                  membersProp
                                  resultProp                                  
                                  TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetInvokeActionWithScalarParmsReturnQuerySimpleObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnQuerySimple "objects" oType oName api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnQuerySimpleService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnQuerySimple "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnQuerySimple "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithOptionalParmQueryOnly"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = "parm="
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapFromUrl parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty("parm", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("")) ])) ])
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, args) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetInvokeActionWithScalarMissingParmsSimpleObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple "objects" oType oName api.GetInvoke api

let GetInvokeActionWithScalarMissingParmsSimpleService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithScalarMissingParmsSimpleViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = "parm2=fred&parm1=100&x-ro-validate-only=true"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapFromUrl parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly "objects" oType oName api.GetInvoke api


//// 19.1 get to invoke action reference parms in formal form 


let VerifyPostInvokeActionReturnQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryable"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType "" true
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType "" true
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           "" roType true) ])
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))

    let contribName = ttc "RestfulObjects.Test.Data.ContributorService"
    
    let p1 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p2 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p3 = makeInvokeValueParm "id" "Id"

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("ACollectionContributedActionNoParms", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionNoParms" contribName roType [ p1 ]))
                                                           TProperty("ACollectionContributedActionParm", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionParm" contribName roType [ p2; p3 ]))]))



    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
                                  pageProp
                                  membersProp
                                  TProperty(JsonPropertyNames.Value, 
                                            TArray([ TObjectJson(obj1)
                                                     TObjectJson(obj2) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionReturnQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnQuery "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnQuery "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionReturnQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryable"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnQueryValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionReturnQueryValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionReturnQueryValidateOnly "objects" oType oName api.PostInvoke api

// 19.3 post to invoke action with scalar parms 
let VerifyPostInvokeActionWithScalarParmsReturnQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType "" true
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType "" true
    
    let args = 
        TObjectJson([ TProperty("parm1", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(100)) ]))
                      TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, args) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           "" roType true) ])
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))

    let contribName = ttc "RestfulObjects.Test.Data.ContributorService"
    
    let p1 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p2 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p3 = makeInvokeValueParm "id" "Id"

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("ACollectionContributedActionNoParms", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionNoParms" contribName roType [ p1 ]))
                                                           TProperty("ACollectionContributedActionParm", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionParm" contribName roType [ p2; p3 ]))]))


    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
                                  pageProp
                                  membersProp
                                  TProperty(JsonPropertyNames.Value, 
                                            TArray([ TObjectJson(obj1)
                                                     TObjectJson(obj2) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionWithScalarParmsReturnQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithScalarParmsReturnQuery "objects" oType oName api.PostInvoke api

let PostInvokeActionWithScalarParmsReturnQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithScalarParmsReturnQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithScalarParmsReturnQuery "objects" oType oName api.PostInvoke api

let VerifyPostInvokeOverloadedAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnOverloadedAction1"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = new JObject(new JProperty("parm", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args1 = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                    TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                    TObjectJson(args1 :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeOverloadedActionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeOverloadedAction "objects" oType oName api.PostInvoke api

let PostInvokeOverloadedActionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeOverloadedAction "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeOverloadedActionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeOverloadedAction "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithScalarParmsReturnQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true))
    
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithScalarParmsReturnQueryValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithScalarParmsReturnQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithScalarParmsReturnQuery "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithScalarParmsReturnCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))))

    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([]))

    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
                                  pageProp
                                  membersProp
                                  TProperty(JsonPropertyNames.Value, 
                                            TArray([ TObjectJson(obj1)
                                                     TObjectJson(obj2) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionWithScalarParmsReturnCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithScalarParmsReturnCollection "objects" oType oName api.PostInvoke api

let PostInvokeActionWithScalarParmsReturnCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithScalarParmsReturnCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithScalarParmsReturnCollection "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    
    let parsedResult = JObject.Parse(jsonResult)
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType "" true
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType "" true
    
    let args = 
        TObjectJson
            ([ TProperty("parm1", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(101)) ]))
               TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectJson(makeHref (sprintf "objects/%s/%s" roType (ktc "1")))) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, args) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           "" roType true) ])
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))

    let contribName = ttc "RestfulObjects.Test.Data.ContributorService"
    
    let p1 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p2 = makeInvokeCollectionParm  "ms"  "Ms" roType
    let p3 = makeInvokeValueParm "id" "Id" 

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("ACollectionContributedActionNoParms", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionNoParms" contribName roType [ p1 ]))
                                                           TProperty("ACollectionContributedActionParm", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionParm" contribName roType [ p2; p3 ]))]))



    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ TProperty
                                      (JsonPropertyNames.Links, 
                                       
                                       TArray
                                           ([  ]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
                                  pageProp
                                  membersProp
                                  TProperty(JsonPropertyNames.Value, 
                                            TArray([ TObjectJson(obj1)
                                                     TObjectJson(obj2) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionWithReferenceParmsReturnQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnQuery "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnQuery "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let pageProp = 
        TProperty(JsonPropertyNames.Pagination, 
                  TObjectJson([TProperty(JsonPropertyNames.Page, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.PageSize, TObjectVal(20)) 
                               TProperty(JsonPropertyNames.NumPages, TObjectVal(1)) 
                               TProperty(JsonPropertyNames.TotalCount, TObjectVal(2))]))
    
    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([]))

    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List))
          TProperty(JsonPropertyNames.Result, 
                    TObjectJson([ TProperty (JsonPropertyNames.Links,  TArray ([]))
                                  TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
                                  pageProp
                                  membersProp
                                  TProperty(JsonPropertyNames.Value, 
                                            TArray([ TObjectJson(obj1)
                                                     TObjectJson(obj2) ])) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionWithReferenceParmsReturnCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnCollection "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnCollection "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly "objects" oType oName api.PostInvoke api

// w
let VerifyPostInvokeActionWithReferenceParmsReturnScalar refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsScalarWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" mst (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(555))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "number", "", true), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionWithReferenceParmsReturnScalarObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnScalar "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnScalarService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnScalar "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnScalarViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnScalar "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsScalarWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnVoid refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoidWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Void))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let PostInvokeActionWithReferenceParmsReturnVoidObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnVoid "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnVoidService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnVoid "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnVoidViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnVoid "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoidWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsObjectWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PostInvokeActionWithReferenceParmsReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnObject "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnObject "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnObject "objects" oType oName api.PostInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsObjectWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oName api.PostInvoke api

let PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPutInvokeActionWithReferenceParmsReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsObjectWithParametersAnnotatedIdempotent"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      TProperty(JsonPropertyNames.Links, 
                                TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType)
                                         TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                         TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutInvokeActionWithReferenceParmsReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutInvokeActionWithReferenceParmsReturnObject "objects" oType oName api.PutInvoke api

let PutInvokeActionWithReferenceParmsReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutInvokeActionWithReferenceParmsReturnObject "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutInvokeActionWithReferenceParmsReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutInvokeActionWithReferenceParmsReturnObject "objects" oType oName api.PutInvoke api

let VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsObjectWithParametersAnnotatedIdempotent"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oName api.PutInvoke api

let PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oName api.PutInvoke api

let VerifyGetInvokeActionWithReferenceParmsReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsObjectWithParametersAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved parms
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           
                           TArray
                               ([ TObjectJson
                                      (makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" true)
                                  TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                  
                                  TObjectJson
                                      (args 
                                       :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" 
                                              true) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson
            ([ TProperty("parm1", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(101)) ]))
               TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectJson(makeHref (sprintf "objects/%s/%s" roType (ktc "1")))) ])) ])
    
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, args) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetInvokeActionWithReferenceParmsReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithReferenceParmsReturnObject "objects" oType oName api.GetInvoke api

let GetInvokeActionWithReferenceParmsReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithReferenceParmsReturnObject "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithReferenceParmsReturnObject "objects" oType oName api.GetInvoke api

let VerifyPostInvokeActionWithReferenceParmsReturnObjectOnForm (api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.FormViewModel"
    let oName =  ktc "1--1"
    
    let refType = "objects"
   
    let pid = "Step"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.FormViewModel"
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" mst (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("MostSimple", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("Name", new JObject(new JProperty(JsonPropertyNames.Value, "aname"))))

    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    
    let oid = ktc "1--1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PostInvoke (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)

    let roid = oType + "/" +  ktc "2--1"
    
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

    
    let mp r n = sprintf ";%s=\"%s\"" r n

    let makeParm pmid pid fid rt = 
       
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst

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
             :: makeGetLinkProp valueRel3 (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst)

    let p2 = makeParm "MostSimple" "Step" "Most Simple" mst
    let p3 = makeValueParm "Name" "Name" "string"

    let resultObject = 
        [ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "2--1"))
          TProperty(JsonPropertyNames.Title, TObjectVal("Untitled Form View Model"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object oType)
                             TObjectJson(sb(oType)); TObjectJson(sp(oType))
                           ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("MostSimple", TObjectJson(makePropertyMemberShort "objects" "MostSimple" roid "Most Simple" "" mst true val3 []))
                                  TProperty("Name", TObjectJson(makePropertyMemberWithFormat "objects" "Name" roid "Name" "" "string" false (TObjectVal(null))))                           
                                  TProperty("Step", TObjectJson(makeObjectActionMemberSimple "Step" roid oType [ p2; p3 ]))                                                    
                    ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Form View Model"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Form View Models"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InteractionMode, TObjectVal("form"))
                                  TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ]

    let expected = 
        [ TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, TObjectJson(resultObject))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let VerifyPostInvokeActionMissingParmOnForm (api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.FormViewModel"
    let oName =  ktc "1--1" 
    let refType = "objects"  
    let pid = "Step"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("MostSimple", new JObject(new JProperty(JsonPropertyNames.Value, null))), 
                    new JProperty("Name", new JObject(new JProperty(JsonPropertyNames.Value, ""))))

    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded  
    let oid = ktc "1--1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.PostInvoke (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("MostSimple", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                ]))
          TProperty("Name", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]


    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult





let VerifyGetInvokeActionWithParmReturnObject refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsObjectWithParameterAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = new JObject(new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let roid = roType + "/" + ktc "1"
    let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty("Id", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    
    let resultObject = 
        TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                      TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"))
                      TProperty(JsonPropertyNames.Title, TObjectVal("1"))
                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           
                           TArray
                               ([ TObjectJson
                                      (makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" true)
                                  TObjectJson(sb(roType)); TObjectJson(sp(roType))
                                  
                                  TObjectJson
                                      (args 
                                       :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid) RepresentationTypes.Object roType "" 
                                              true) ]))
                      
                      TProperty
                          (JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType))
                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.InteractionMode, TObjectVal("persistent"))
                                              TProperty(JsonPropertyNames.IsService, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty("parm1", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(101)) ])) ])
    let links = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Arguments, args) 
                    :: makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oName pid) RepresentationTypes.ActionResult 
                           roType "" true) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Links, links)
          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object))
          TProperty(JsonPropertyNames.Result, resultObject)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsNull(result.Headers.ETag) - change to spec 22/2/16 
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetInvokeActionWithParmReturnObjectObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithParmReturnObject "objects" oType oName api.GetInvoke api

let GetInvokeActionWithParmReturnObjectService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithParmReturnObject "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithParmReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithParmReturnObject "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsObjectWithParametersAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oName api.GetInvoke api

// get action missing arguments - 400
let VerifyMissingParmsOnGetQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext    
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let MissingParmsOnGetQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMissingParmsOnGetQuery "objects" oType oName api.GetInvoke api

let MissingParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMissingParmsOnGetQuery "services" oType oName (wrap3 api.GetInvokeOnService) api

let MissingParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMissingParmsOnGetQuery "objects" oType oName api.GetInvoke api

let VerifyMissingParmsOnPostCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let MissingParmsOnPostCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMissingParmsOnPostCollection "objects" oType oName api.PostInvoke api

let MissingParmsOnPostCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMissingParmsOnPostCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let MissingParmsOnPostCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMissingParmsOnPostCollection "objects" oType oName api.PostInvoke api

// malformed args 
let VerifyMalformedSimpleParmsOnGetQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = "malformed=fred&parm1=100"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms
     
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapFromUrl parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 

    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedSimpleParmsOnGetQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedSimpleParmsOnGetQuery "objects" oType oName api.GetInvoke api

let MalformedSimpleParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedSimpleParmsOnGetQuery "services" oType oName (wrap3 api.GetInvokeOnService) api

let MalformedSimpleParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedSimpleParmsOnGetQuery "objects" oType oName api.GetInvoke api

let VerifyMalformedFormalParmsOnGetQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnGetQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedFormalParmsOnGetQuery "objects" oType oName api.GetInvoke api

let MalformedFormalParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedFormalParmsOnGetQuery "services" oType oName (wrap3 api.GetInvokeOnService) api

let MalformedFormalParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedFormalParmsOnGetQuery "objects" oType oName api.GetInvoke api

let VerifyMalformedFormalParmsOnPostCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))))
    //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved parms
    setIfMatch api.Request "*"
    jsonSetPostMsg api.Request url (parms.ToString())
    let result = f (oType, oid, pid, args)

    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnPostCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedFormalParmsOnPostCollection "objects" oType oName api.PostInvoke api

let MalformedFormalParmsOnPostCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedFormalParmsOnPostCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let MalformedFormalParmsOnPostCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedFormalParmsOnPostCollection "objects" oType oName api.PostInvoke api

let VerifyInvalidSimpleParmsOnGetQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = "parm2=fred&parm1=invalidvalue"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapFromUrl parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidSimpleParmsOnGetQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidSimpleParmsOnGetQuery "objects" oType oName api.GetInvoke api

let InvalidSimpleParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidSimpleParmsOnGetQuery "services" oType oName (wrap3 api.GetInvokeOnService) api

let InvalidSimpleParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidSimpleParmsOnGetQuery "objects" oType oName api.GetInvoke api

let VerifyInvalidFormalParmsOnGetQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidFormalParmsOnGetQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidFormalParmsOnGetQuery "objects" oType oName api.GetInvoke api

let InvalidFormalParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidFormalParmsOnGetQuery "services" oType oName (wrap3 api.GetInvokeOnService) api

let InvalidFormalParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidFormalParmsOnGetQuery "objects" oType oName api.GetInvoke api

let VerifyInvalidFormalParmsOnPostCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))))
    //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
   
    let purl = sprintf "%s/actions/%s/invoke" ourl pid 
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidFormalParmsOnPostCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidFormalParmsOnPostCollection "objects" oType oName api.PostInvoke api

let InvalidFormalParmsOnPostCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidFormalParmsOnPostCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let InvalidFormalParmsOnPostCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidFormalParmsOnPostCollection "objects" oType oName api.PostInvoke api

let VerifyDisabledActionInvokeQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ADisabledQueryAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Always disabled\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let DisabledActionInvokeQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyDisabledActionInvokeQuery "objects" oType oName api.GetInvoke api

let DisabledActionInvokeQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyDisabledActionInvokeQuery "services" oType oName (wrap3 api.GetInvokeOnService) api

let DisabledActionInvokeQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyDisabledActionInvokeQuery "objects" oType oName api.GetInvoke api

let VerifyDisabledActionInvokeCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let error = "Always disabled"
    let pid = "ADisabledCollectionAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let DisabledActionInvokeCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyDisabledActionInvokeCollection "objects" oType oName api.PostInvoke api

let DisabledActionInvokeCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyDisabledActionInvokeCollection "services" oType oName (wrap3 api.PostInvokeOnService) api

let DisabledActionInvokeCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyDisabledActionInvokeCollection "objects" oType oName api.PostInvoke api

// 404 
let VerifyNotFoundActionInvoke refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ANonExistentAction" // doesn't exist 
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionInvokeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyNotFoundActionInvoke "objects" oType oName api.GetInvoke api

let NotFoundActionInvokeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotFoundActionInvoke "services" oType oName (wrap3 api.GetInvokeOnService) api

let NotFoundActionInvokeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyNotFoundActionInvoke "objects" oType oName api.GetInvoke api

let VerifyHiddenActionInvoke refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AHiddenAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let HiddenActionInvokeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyHiddenActionInvoke "objects" oType oName api.GetInvoke api

let HiddenActionInvokeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyHiddenActionInvoke "services" oType oName (wrap3 api.GetInvokeOnService) api

let HiddenActionInvokeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyHiddenActionInvoke "objects" oType oName api.GetInvoke api

let VerifyGetActionWithSideEffects refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoid"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetActionWithSideEffectsObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetActionWithSideEffects "objects" oType oName api.GetInvoke api

let GetActionWithSideEffectsService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetActionWithSideEffects "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetActionWithSideEffectsViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetActionWithSideEffects "objects" oType oName api.GetInvoke api

let VerifyGetActionWithIdempotent refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetActionWithIdempotentObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetActionWithIdempotent "objects" oType oName api.GetInvoke api

let GetActionWithIdempotentService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetActionWithIdempotent "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetActionWithIdempotentViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetActionWithIdempotent "objects" oType oName api.GetInvoke api

let VerifyPutActionWithQueryOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoid"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
       
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPutMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not idempotent\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutActionWithQueryOnlyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutActionWithQueryOnly "objects" oType oName api.PutInvoke api

let PutActionWithQueryOnlyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutActionWithQueryOnly "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutActionWithQueryOnlyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutActionWithQueryOnly "objects" oType oName api.PutInvoke api

let VerifyNotAcceptableGetInvokeWrongMediaType refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryable"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s" ourl pid

    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ActionDescription
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 

    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult

let NotAcceptableGetInvokeWrongMediaTypeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = ktc "1"
    VerifyNotAcceptableGetInvokeWrongMediaType "objects" oType oName api.GetInvoke api

let NotAcceptableGetInvokeWrongMediaTypeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotAcceptableGetInvokeWrongMediaType "services" oType oName (wrap3 api.GetInvokeOnService) api

let NotAcceptableGetInvokeWrongMediaTypeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyNotAcceptableGetInvokeWrongMediaType "objects" oType oName api.GetInvoke api

let VerifyGetQueryActionWithError refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnErrorQuery"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                          ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let GetQueryActionWithErrorObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetQueryActionWithError "objects" oType oName api.GetInvoke api

let GetQueryActionWithErrorService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetQueryActionWithError "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetQueryActionWithErrorViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetQueryActionWithError "objects" oType oName api.GetInvoke api

let VerifyPostCollectionActionWithError refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnErrorCollection"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                            ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let PostCollectionActionWithErrorObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostCollectionActionWithError "objects" oType oName api.PostInvoke api

let PostCollectionActionWithErrorService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostCollectionActionWithError "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostCollectionActionWithErrorViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostCollectionActionWithError "objects" oType oName api.PostInvoke api

let VerifyMissingParmsOnPost refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsScalarWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let MissingParmsOnPostObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMissingParmsOnPost "objects" oType oName api.PostInvoke api

let MissingParmsOnPostService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMissingParmsOnPost "services" oType oName (wrap3 api.PostInvokeOnService) api

let MissingParmsOnPostViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMissingParmsOnPost "objects" oType oName api.PostInvoke api

let VerifyMalformedFormalParmsOnPostQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 

    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedFormalParmsOnPostQuery "objects" oType oName api.PostInvoke api

let MalformedFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedFormalParmsOnPostQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let MalformedFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedFormalParmsOnPostQuery "objects" oType oName api.PostInvoke api

let VerifyWrongTypeParmsOnPostQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 1))), 
                    new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, 2))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
   
    

    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty("parm1", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(1)) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(2))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Not a suitable type; must be a Most Simple")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Not a suitable type; must be a Most Simple\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let WrongTypeFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyWrongTypeParmsOnPostQuery "objects" oType oName api.PostInvoke api

let WrongTypeFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyWrongTypeParmsOnPostQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let WrongTypeFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyWrongTypeParmsOnPostQuery "objects" oType oName api.PostInvoke api

let VerifyEmptyParmsOnPostQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, ""))), 
                    new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, ""))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
   
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)


    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(""))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\",199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let EmptyFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyEmptyParmsOnPostQuery "objects" oType oName api.PostInvoke api

let EmptyFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyEmptyParmsOnPostQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let EmptyFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyEmptyParmsOnPostQuery "objects" oType oName api.PostInvoke api

let VerifyInvalidFormalParmsOnPostQuery refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
   
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    
    
    let refParm = TObjectJson(makeHref (sprintf "objects/%s/%s" roType (ktc "1")))
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, refParm) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidFormalParmsOnPostQuery "objects" oType oName api.PostInvoke api

let InvalidFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidFormalParmsOnPostQuery "services" oType oName (wrap3 api.PostInvokeOnService) api

let InvalidFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidFormalParmsOnPostQuery "objects" oType oName api.PostInvoke api

// Always disabled
let VerifyDisabledPostActionInvoke refType oType oName f (api : RestfulObjectsControllerBase) = 
    let error = "Always disabled"
    let pid = "ADisabledAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let DisabledActionPostInvokeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyDisabledPostActionInvoke "objects" oType oName api.PostInvoke api

let DisabledActionPostInvokeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyDisabledPostActionInvoke "services" oType oName (wrap3 api.PostInvokeOnService) api

let DisabledActionPostInvokeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyDisabledPostActionInvoke "objects" oType oName api.PostInvoke api

let VerifyUserDisabledPostActionInvoke refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AUserDisabledAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action AUserDisabledAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let UserDisabledActionPostInvokeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyUserDisabledPostActionInvoke "objects" oType oName api.PostInvoke api

let UserDisabledActionPostInvokeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyUserDisabledPostActionInvoke "services" oType oName (wrap3 api.PostInvokeOnService) api

let UserDisabledActionPostInvokeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyUserDisabledPostActionInvoke "objects" oType oName api.PostInvoke api

let VerifyNotFoundActionPostInvoke refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ANonExistentAction" // doesn't exist 
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionPostInvokeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyNotFoundActionPostInvoke "objects" oType oName api.PostInvoke api

let NotFoundActionPostInvokeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotFoundActionPostInvoke "services" oType oName (wrap3 api.PostInvokeOnService) api

let NotFoundActionPostInvokeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyNotFoundActionPostInvoke "objects" oType oName api.PostInvoke api

let VerifyHiddenActionPostInvoke refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AHiddenAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let HiddenActionPostInvokeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyHiddenActionPostInvoke "objects" oType oName api.PostInvoke api

let HiddenActionPostInvokeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyHiddenActionPostInvoke "services" oType oName (wrap3 api.PostInvokeOnService) api

let HiddenActionPostInvokeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyHiddenActionPostInvoke "objects" oType oName api.PostInvoke api

let VerifyNotAcceptablePostInvokeWrongMediaType refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnAction"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/properties/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    

let NotAcceptablePostInvokeWrongMediaTypeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyNotAcceptablePostInvokeWrongMediaType "objects" oType oName api.PostInvoke api

let NotAcceptablePostInvokeWrongMediaTypeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotAcceptablePostInvokeWrongMediaType "services" oType oName (wrap3 api.PostInvokeOnService) api

let NotAcceptablePostInvokeWrongMediaTypeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyNotAcceptablePostInvokeWrongMediaType "objects" oType oName api.PostInvoke api

let VerifyPostQueryActionWithError refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnErrorQuery"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                             ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]
    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let PostQueryActionWithErrorObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostQueryActionWithError "objects" oType oName api.PostInvoke api

let PostQueryActionWithErrorService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostQueryActionWithError "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostQueryActionWithErrorViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostQueryActionWithError "objects" oType oName api.PostInvoke api

let VerifyPostQueryActionWithValidateFail refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionValidateParameters"
    let ourl = sprintf "%s/%s" refType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, 0))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 0))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(0))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Fail validation parm1")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(0)) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Fail validation parm1\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let PostQueryActionWithValidateFailObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostQueryActionWithValidateFail "objects" oType oName api.PostInvoke api

let PostQueryActionWithValidateFailService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostQueryActionWithValidateFail "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostQueryActionWithValidateFailViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostQueryActionWithValidateFail "objects" oType oName api.PostInvoke api

let VerifyPostQueryActionWithCrossValidateFail refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionValidateParameters"
    let ourl = sprintf "%s/%s" refType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, 1))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 2))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(2)) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(1)) ]))
          TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed")) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let PostQueryActionWithCrossValidateFailObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostQueryActionWithCrossValidateFail "objects" oType oName api.PostInvoke api

let PostQueryActionWithCrossValidateFailService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostQueryActionWithCrossValidateFail "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostQueryActionWithCrossValidateFailViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostQueryActionWithCrossValidateFail "objects" oType oName api.PostInvoke api

let VerifyGetInvokeActionReturnCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollection"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionReturnCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnCollection "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnCollection "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionReturnCollection "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke?parm2=fred&parm1=100" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithScalarParmsReturnCollectionSimpleObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple "objects" oType oName api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnCollectionSimpleService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithScalarParmsReturnCollectionFormalObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal "objects" oType oName api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnCollectionFormalService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithReferenceParmsReturnCollectionFormalObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal "objects" oType oName api.GetInvoke api

let GetInvokeActionWithReferenceParmsReturnCollectionFormalService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal "objects" oType oName api.GetInvoke api

// new validate only
let VerifyMissingParmsOnGetQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let MissingParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMissingParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let MissingParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMissingParmsOnGetQueryValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let MissingParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMissingParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let VerifyMissingParmsOnPostCollectionValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let MissingParmsOnPostCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMissingParmsOnPostCollectionValidateOnly "objects" oType oName api.PostInvoke api

let MissingParmsOnPostCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMissingParmsOnPostCollectionValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let MissingParmsOnPostCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMissingParmsOnPostCollectionValidateOnly "objects" oType oName api.PostInvoke api

//// malformed args 
let VerifyMalformedSimpleParmsOnGetQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = "malformed=fred&parm1=100&x-ro-validate-only=true"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms
       
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapFromUrl parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedSimpleParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedSimpleParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let MalformedSimpleParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedSimpleParmsOnGetQueryValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let MalformedSimpleParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedSimpleParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let VerifyMalformedFormalParmsOnGetQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    let args = CreateArgMapWithReserved(parms)
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedFormalParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let MalformedFormalParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedFormalParmsOnGetQueryValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let MalformedFormalParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedFormalParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let VerifyMalformedFormalParmsOnGetCollectionValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true))
    //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnPostCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedFormalParmsOnGetCollectionValidateOnly "objects" oType oName api.PostInvoke api

let MalformedFormalParmsOnPostCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedFormalParmsOnGetCollectionValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let MalformedFormalParmsOnPostCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedFormalParmsOnGetCollectionValidateOnly "objects" oType oName api.PostInvoke api

let VerifyInvalidSimpleParmsOnGetQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = "parm2=fred&parm1=invalidvalue&x-ro-validate-only=true"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl 
    let args = CreateArgMapFromUrl parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidSimpleParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidSimpleParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let InvalidSimpleParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidSimpleParmsOnGetQueryValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let InvalidSimpleParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidSimpleParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let VerifyInvalidFormalParmsOnGetQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))), new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl 
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidFormalParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidFormalParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let InvalidFormalParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidFormalParmsOnGetQueryValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let InvalidFormalParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidFormalParmsOnGetQueryValidateOnly "objects" oType oName api.GetInvoke api

let VerifyInvalidFormalParmsOnPostCollectionValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))), new JProperty("x-ro-validate-only", true))
    //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred")) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidFormalParmsOnPostCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidFormalParmsOnPostCollectionValidateOnly "objects" oType oName api.PostInvoke api

let InvalidFormalParmsOnPostCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidFormalParmsOnPostCollectionValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let InvalidFormalParmsOnPostCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidFormalParmsOnPostCollectionValidateOnly "objects" oType oName api.PostInvoke api

let VerifyDisabledActionInvokeQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ADisabledQueryAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    

    let oid = ktc "1"
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 


    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Always disabled\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let DisabledActionInvokeQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyDisabledActionInvokeQueryValidateOnly "objects" oType oName api.GetInvoke api

let DisabledActionInvokeQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyDisabledActionInvokeQueryValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let DisabledActionInvokeQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyDisabledActionInvokeQueryValidateOnly "objects" oType oName api.GetInvoke api

let VerifyDisabledActionInvokeCollectionValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let error = "Always disabled"
    let pid = "ADisabledCollectionAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let DisabledActionInvokeCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyDisabledActionInvokeCollectionValidateOnly "objects" oType oName api.PostInvoke api

let DisabledActionInvokeCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyDisabledActionInvokeCollectionValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let DisabledActionInvokeCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyDisabledActionInvokeCollectionValidateOnly "objects" oType oName api.PostInvoke api

// 404 
let VerifyNotFoundActionInvokeValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ANonExistentAction" // doesn't exist 
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
       
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyNotFoundActionInvokeValidateOnly "objects" oType oName api.GetInvoke api

let NotFoundActionInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotFoundActionInvokeValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let NotFoundActionInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyNotFoundActionInvokeValidateOnly "objects" oType oName api.GetInvoke api

let VerifyHiddenActionInvokeValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AHiddenAction"
    let ourl = sprintf "%s/%s" refType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let HiddenActionInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyHiddenActionInvokeValidateOnly "objects" oType oName api.GetInvoke api

let HiddenActionInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyHiddenActionInvokeValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let HiddenActionInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyHiddenActionInvokeValidateOnly "objects" oType oName api.GetInvoke api

let VerifyGetActionWithSideEffectsValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoid"
    let ourl = sprintf "%s/%s" refType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetActionWithSideEffectsObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetActionWithSideEffectsValidateOnly "objects" oType oName api.GetInvoke api

let GetActionWithSideEffectsServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetActionWithSideEffectsValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetActionWithSideEffectsViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetActionWithSideEffectsValidateOnly "objects" oType oName api.GetInvoke api

let VerifyGetActionWithIdempotentValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetActionWithIdempotentObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetActionWithIdempotentValidateOnly "objects" oType oName api.GetInvoke api

let GetActionWithIdempotentServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetActionWithIdempotentValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetActionWithIdempotentViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetActionWithIdempotentValidateOnly "objects" oType oName api.GetInvoke api

let VerifyPutActionWithQueryOnlyValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsVoid"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext    
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not idempotent\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutActionWithQueryOnlyObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPutActionWithQueryOnlyValidateOnly "objects" oType oName api.PutInvoke api

let PutActionWithQueryOnlyServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPutActionWithQueryOnlyValidateOnly "services" oType oName (wrap3 api.PutInvokeOnService) api

let PutActionWithQueryOnlyViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPutActionWithQueryOnlyValidateOnly "objects" oType oName api.PutInvoke api

let VerifyGetQueryActionWithErrorValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnErrorQuery"
    let ourl = sprintf "%s/%s" refType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved parms
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let GetQueryActionWithErrorObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetQueryActionWithErrorValidateOnly "objects" oType oName api.GetInvoke api

let GetQueryActionWithErrorServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetQueryActionWithErrorValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetQueryActionWithErrorViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetQueryActionWithErrorValidateOnly "objects" oType oName api.GetInvoke api

let VerifyPostCollectionActionWithErrorValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnErrorCollection"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostCollectionActionWithErrorObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostCollectionActionWithErrorValidateOnly "objects" oType oName api.PostInvoke api

let PostCollectionActionWithErrorServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostCollectionActionWithErrorValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostCollectionActionWithErrorViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostCollectionActionWithErrorValidateOnly "objects" oType oName api.PostInvoke api

let VerifyMissingParmsOnPostValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsScalarWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ]))
          TProperty("parm2", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory")) ])) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Mandatory\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let MissingParmsOnPostObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMissingParmsOnPostValidateOnly "objects" oType oName api.PostInvoke api

let MissingParmsOnPostServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMissingParmsOnPostValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let MissingParmsOnPostViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMissingParmsOnPostValidateOnly "objects" oType oName api.PostInvoke api

let VerifyMalformedFormalParmsOnPostQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnPostQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyMalformedFormalParmsOnPostQueryValidateOnly "objects" oType oName api.PostInvoke api

let MalformedFormalParmsOnPostQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyMalformedFormalParmsOnPostQueryValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let MalformedFormalParmsOnPostQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyMalformedFormalParmsOnPostQueryValidateOnly "objects" oType oName api.PostInvoke api

let VerifyInvalidFormalParmsOnPostQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    
    let refParm = TObjectJson(makeHref (sprintf "objects/%s/%s" roType (ktc "1")))
    
    let expected = 
        [ TProperty("parm1", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry")) ]))
          TProperty("parm2", TObjectJson([ TProperty(JsonPropertyNames.Value, refParm) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvalidFormalParmsOnPostQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidFormalParmsOnPostQueryValidateOnly "objects" oType oName api.PostInvoke api

let InvalidFormalParmsOnPostQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidFormalParmsOnPostQueryValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let InvalidFormalParmsOnPostQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidFormalParmsOnPostQueryValidateOnly "objects" oType oName api.PostInvoke api

let VerifyInvalidUrlOnPostQueryValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsQueryableWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType (ktc "10"))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, ""))), new JProperty("x-ro-validate-only", true))
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain object %s-%s: null adapter\"" roType (ktc "10"), headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvalidUrlOnPostQueryObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidUrlOnPostQueryValidateOnly "objects" oType oName api.PostInvoke api

let InvalidUrlOnPostQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidUrlOnPostQueryValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let InvalidUrlOnPostQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyInvalidUrlOnPostQueryValidateOnly "objects" oType oName api.PostInvoke api

// Always disabled
let VerifyDisabledPostActionInvokeValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let error = "Always disabled"
    let pid = "ADisabledAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let DisabledActionPostInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyDisabledPostActionInvokeValidateOnly "objects" oType oName api.PostInvoke api

let DisabledActionPostInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyDisabledPostActionInvokeValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let DisabledActionPostInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyDisabledPostActionInvokeValidateOnly "objects" oType oName api.PostInvoke api

let VerifyNotFoundActionPostInvokeValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "ANonExistentAction" // doesn't exist 
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionPostInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyNotFoundActionPostInvokeValidateOnly "objects" oType oName api.PostInvoke api

let NotFoundActionPostInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotFoundActionPostInvokeValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let NotFoundActionPostInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyNotFoundActionPostInvokeValidateOnly "objects" oType oName api.PostInvoke api

let VerifyHiddenActionPostInvokeValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AHiddenAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let HiddenActionPostInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyHiddenActionPostInvokeValidateOnly "objects" oType oName api.PostInvoke api

let HiddenActionPostInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyHiddenActionPostInvokeValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let HiddenActionPostInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyHiddenActionPostInvokeValidateOnly "objects" oType oName api.PostInvoke api

let VerifyPostQueryActionWithErrorValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnErrorQuery"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(parms)
    jsonSetPostMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PostQueryActionWithErrorObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostQueryActionWithErrorValidateOnly "objects" oType oName api.PostInvoke api

let PostQueryActionWithErrorServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyPostQueryActionWithErrorValidateOnly "services" oType oName (wrap3 api.PostInvokeOnService) api

let PostQueryActionWithErrorViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyPostQueryActionWithErrorValidateOnly "objects" oType oName api.PostInvoke api

let VerifyGetInvokeActionReturnCollectionValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollection"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionReturnCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionReturnCollectionValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionReturnCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionReturnCollectionValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionReturnCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionReturnCollectionValidateOnly "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithScalarParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly "objects" oType oName api.GetInvoke api

let VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionReturnsCollectionWithParameters"
    let ourl = sprintf "%s/%s/%s" refType oType oName
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString()))
    let parms = 
        new JObject(new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), 
                    new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let oid = ktc "1"
    let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
    let args = CreateArgMapWithReserved (new JObject())
    let url = sprintf "http://localhost/%s" purl
    jsonSetGetMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly "objects" oType oName api.GetInvoke api

let GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly "services" oType oName (wrap3 api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType
    VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly "objects" oType oName api.GetInvoke api

let VerifyPostInvokeActionReturnObjectConcurrencyFail refType oType oName f tag (api : RestfulObjectsControllerBase) = 
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request tag
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    
    assertStatusCode HttpStatusCode.PreconditionFailed statusCode  jsonResult

    Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnObjectObjectConcurrencyFail(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnObjectConcurrencyFail "objects" oType oName api.PostInvoke "\"fail\"" api

let VerifyPutInvokeActionReturnObjectConcurrencyFail refType oType oName f tag (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
   
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPutMsg api.Request url
    setIfMatch api.Request tag
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
     
    assertStatusCode HttpStatusCode.PreconditionFailed statusCode  jsonResult

    Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutInvokeActionReturnObjectObjectConcurrencyFail(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = ktc "1"
    VerifyPutInvokeActionReturnObjectConcurrencyFail "objects" oType oName api.PutInvoke "\"fail\"" api

let VerifyPostInvokeActionReturnObjectMissingIfMatch refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPostMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode preconditionHeaderMissing statusCode  jsonResult
    Assert.AreEqual
        ("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", 
         headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnObjectObjectMissingIfMatch(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyPostInvokeActionReturnObjectMissingIfMatch "objects" oType oName api.PostInvoke api

let VerifyPutInvokeActionReturnObjectMissingIfMatch refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    let args = CreateArgMapWithReserved(new JObject())
    jsonSetEmptyPutMsg api.Request url
    let result = f (oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode preconditionHeaderMissing statusCode  jsonResult

    Assert.AreEqual
        ("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", 
         headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutInvokeActionReturnObjectObjectMissingIfMatch(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = ktc "1"
    VerifyPutInvokeActionReturnObjectMissingIfMatch "objects" oType oName api.PutInvoke api
