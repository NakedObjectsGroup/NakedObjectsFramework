﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.ObjectAction18

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open Newtonsoft.Json.Linq
open NakedObjects.Rest.Snapshot.Constants
open NakedObjects.Rest.Test.Functions
open System.Security.Principal

// 18.2 get action no parms 
let VerifyActionProperty refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class5 class6"))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                             
                              ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    compareObject expected parsedResult

let GetActionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    VerifyActionProperty "objects" oType oName api.GetAction api

let GetActionPropertyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    VerifyActionProperty "objects" oType oName api.GetAction api

let GetActionPropertyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionProperty "services" oType oName (wrap api.GetServiceAction) api

let VerifyActionPropertyWithDateTime refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionWithDateTimeParm"
    let pmid = "parm"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
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
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm1) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Date Time Parm"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                 
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    compareObject expected parsedResult

let GetActionPropertyDateTimeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyWithDateTime "objects" oType oName api.GetAction api

let GetActionPropertyDateTimeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyWithDateTime "objects" oType oName api.GetAction api

let GetActionPropertyDateTimeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionPropertyWithDateTime "services" oType oName (wrap api.GetServiceAction) api

let VerifyActionPropertyWithCollection refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionWithCollectionParameter"
    let pmid = "parm"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl

    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
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
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("list"))
                                              TProperty(JsonPropertyNames.ElementType, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Strings"))
                                              TProperty(JsonPropertyNames.CustomChoices, 
                                                        TObjectJson([ TProperty("string1", TObjectVal("string1"))
                                                                      TProperty("string2", TObjectVal("string2"))
                                                                      TProperty("string3", TObjectVal("string3")) ]))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm1) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Collection Parameter"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                  
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionPropertyCollectionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    VerifyActionPropertyWithCollection "objects" oType oName api.GetAction api

let GetActionPropertyCollectionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oid = ktc "1"
    let oName = oType + "/" + oid
    VerifyActionPropertyWithCollection "objects" oType oName api.GetAction api

let GetActionPropertyCollectionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionPropertyWithCollection "services" oType oName (wrap api.GetServiceAction) api

let VerifyOverloadedActionProperty refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnOverloadedAction0"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Overloaded Action"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                             
                              ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    compareObject expected parsedResult

let GetOverloadedActionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyOverloadedActionProperty "objects" oType oName api.GetAction api

let GetOverloadedActionPropertyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyOverloadedActionProperty "objects" oType oName api.GetAction api

let GetOverloadedActionPropertyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyOverloadedActionProperty "services" oType oName (wrap api.GetServiceAction) api

let VerifyContributedServiceAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "ANonContributedAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl

    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Non Contributed Action"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                 
                              ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
   // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionContributedService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.ContributorService"
    let oid = oType
    //let f = wrap api.GetServiceAction
    VerifyContributedServiceAction "services" oType oid (wrap api.GetServiceAction) api

let VerifyUserDisabledActionProperty refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let p = new GenericPrincipal(new GenericIdentity("editUser"), [||])
    System.Threading.Thread.CurrentPrincipal <- p
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class5 class6"))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                            
                              ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetUserDisabledActionPropertyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyUserDisabledActionProperty "objects" oType oName api.GetAction api

let GetUserDisabledActionPropertyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyUserDisabledActionProperty "services" oType oName (wrap api.GetServiceAction) api

let GetUserDisabledActionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyUserDisabledActionProperty "objects" oType oName api.GetAction api

let VerifyActionPropertyQueryOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionAnnotatedQueryOnly"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl

    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Annotated Query Only"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makeGetLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                             
                              ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
   // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionPropertyQueryOnlyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyQueryOnly "objects" oType oName api.GetAction api

let GetActionPropertyQueryOnlyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionPropertyQueryOnly "services" oType oName (wrap api.GetServiceAction) api

let GetActionPropertyQueryOnlyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyQueryOnly "objects" oType oName api.GetAction api

let VerifyActionPropertyIdempotent refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionAnnotatedIdempotent"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Annotated Idempotent"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePutLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                             
                              ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionPropertyIdempotentObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyIdempotent "objects" oType oName api.GetAction api

let GetActionPropertyIdempotentService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionPropertyIdempotent "services" oType oName (wrap api.GetServiceAction) api

let GetActionPropertyIdempotentViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyIdempotent "objects" oType oName api.GetAction api

let VerifyActionPropertyWithOptParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionWithOptionalParm"
    let pmid = "parm"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Optional Parm"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal("an optional parm"))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.MaxLength, TObjectVal(101))
                                              TProperty(JsonPropertyNames.Pattern, TObjectVal(@"[A-Z]"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm1) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Optional Parm"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                  
                      ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
   // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionPropertyWithOptObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oid = oType + "/" + ktc "1"
    VerifyActionPropertyWithOptParm "objects" oType oid api.GetAction api

let GetActionPropertyWithOptService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oid = oType
    VerifyActionPropertyWithOptParm "services" oType oid (wrap api.GetServiceAction) api

let GetActionPropertyWithOptViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oid = oType + "/" + ktc "1"
    VerifyActionPropertyWithOptParm "objects" oType oid api.GetAction api

let VerifyActionPropertyWithOptParmSimpleOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionWithOptionalParm"
    let pmid = "parm"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let argS = "x-ro-domain-model=simple"
    let url = sprintf "http://localhost/%s?%s" purl argS
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Optional Parm"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal("an optional parm"))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.MaxLength, TObjectVal(101))
                                              TProperty(JsonPropertyNames.Pattern, TObjectVal(@"[A-Z]"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm1) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Optional Parm"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "") ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionPropertyWithOptObjectSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyWithOptParmSimpleOnly "objects" oType oName api.GetAction api

let GetActionPropertyWithOptServiceSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionPropertyWithOptParmSimpleOnly "services" oType oName (wrap api.GetServiceAction) api

let GetActionPropertyWithOptViewModelSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyWithOptParmSimpleOnly "objects" oType oName api.GetAction api

let VerifyActionPropertyWithMediaType refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectAction
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class5 class6"))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                             
                              ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
   // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionPropertyObjectWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyWithMediaType "objects" oType oName api.GetAction api

let GetActionPropertyServiceWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionPropertyWithMediaType "services" oType oName (wrap api.GetServiceAction) api

let GetActionPropertyViewModelWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionPropertyWithMediaType "objects" oType oName api.GetAction api

// 18.2 get action returns scalar no parms 
let VerifyScalarAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsScalar"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Scalar"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                             
                              ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetScalarActionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyScalarAction "objects" oType oName api.GetAction api

let GetScalarActionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyScalarAction "services" oType oName (wrap api.GetServiceAction) api

let GetScalarActionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyScalarAction "objects" oType oName api.GetAction api

// 18.2 get action returns collection no parms 
let VerifyQueryAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsQueryable"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Queryable"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false))
                                  TProperty(JsonPropertyNames.CustomTableViewTitle, TObjectVal(true))
                                  TProperty(JsonPropertyNames.CustomTableViewColumns, TArray([ TObjectVal("Id")]))
                                  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makeGetLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                            
                              ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
   // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetQueryActionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyQueryAction "objects" oType oName api.GetAction api

let GetQueryActionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyQueryAction "services" oType oName (wrap api.GetServiceAction) api

let GetQueryActionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyQueryAction "objects" oType oName api.GetAction api

let VerifyCollectionAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsCollection"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Collection"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([])) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                            
                              ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
   // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionActionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyCollectionAction "objects" oType oName api.GetAction api

let GetCollectionActionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyCollectionAction "services" oType oName (wrap api.GetServiceAction) api

let GetCollectionActionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyCollectionAction "objects" oType oName api.GetAction api

// 18.2 get action returns collection with scalar and ref parms 
let VerifyQueryActionWithParms refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsQueryableWithParameters"
    let pmid1 = "parm1"
    let pmid2 = "parm2"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm1"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm2"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson([ TProperty(pmid1, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid2, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, 
                    TObjectJson([ TProperty(pmid1, parm1)
                                  TProperty(pmid2, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Queryable With Parameters"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makeGetLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                             
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetQueryActionWithParmsObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyQueryActionWithParms "objects" oType oName api.GetAction api

let GetQueryActionWithParmsService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyQueryActionWithParms "services" oType oName (wrap api.GetServiceAction) api

let GetQueryActionWithParmsViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyQueryActionWithParms "objects" oType oName api.GetAction api

let VerifyCollectionActionWithParms refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsCollectionWithParameters"
    let pmid1 = "parm1"
    let pmid2 = "parm2"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
 
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm1"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm2"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson([ TProperty(pmid1, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid2, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, 
                    TObjectJson([ TProperty(pmid1, parm1)
                                  TProperty(pmid2, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Collection With Parameters"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                               
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionActionWithParmsObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyCollectionActionWithParms "objects" oType oName api.GetAction api

let GetCollectionActionWithParmsService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyCollectionActionWithParms "services" oType oName (wrap api.GetServiceAction) api

let GetCollectionActionWithParmsViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyCollectionActionWithParms "objects" oType oName api.GetAction api

let VerifyCollectionActionWithParmsSimpleOnly refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionReturnsCollectionWithParameters"
    let pmid1 = "parm1"
    let pmid2 = "parm2"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let argS = "x-ro-domain-model=simple"
    let url = sprintf "http://localhost/%s?%s" purl argS
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm1"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm2 = 
        TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm2"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson([ TProperty(pmid1, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid2, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, 
                    TObjectJson([ TProperty(pmid1, parm1)
                                  TProperty(pmid2, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Collection With Parameters"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "") ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetCollectionActionWithParmsObjectSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyCollectionActionWithParmsSimpleOnly "objects" oType oName api.GetAction api

let GetCollectionActionWithParmsServiceSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyCollectionActionWithParmsSimpleOnly "services" oType oName (wrap api.GetServiceAction) api

let GetCollectionActionWithParmsViewModelSimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyCollectionActionWithParmsSimpleOnly "objects" oType oName api.GetAction api

// 18.2 get action  with scalar parm 
let VerifyActionWithValueParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionWithValueParameter"
    let pmid = "parm1"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
  
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([ ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm1"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm1) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Value Parameter"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))
          
          TProperty
              (JsonPropertyNames.Links, 
               
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")
                     
                     
                      
                      ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithValueParmObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithValueParm "objects" oType oName api.GetAction api

let GetActionWithValueParmService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithValueParm "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithValueParmViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithValueParm "objects" oType oName api.GetAction api

// 18.2 get action  with scalar parm with choices 
let VerifyActionWithValueParmWithChoices refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionWithValueParameterWithChoices"
    let pmid = "parm3"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm3 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm3"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.CustomChoices, 
                                                        TObjectJson([ TProperty("1", TObjectVal(1))
                                                                      TProperty("2", TObjectVal(2))
                                                                      TProperty("3", TObjectVal(3)) ]))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ]))
                      TProperty(JsonPropertyNames.Choices, 
                                TArray([ TObjectVal(1)
                                         TObjectVal(2)
                                         TObjectVal(3) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm3) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Value Parameter With Choices"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                 
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithValueParmWithChoicesObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithValueParmWithChoices "objects" oType oName api.GetAction api

let GetActionWithValueParmWithChoicesService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithValueParmWithChoices "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithValueParmWithChoicesViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithValueParmWithChoices "objects" oType oName api.GetAction api

// 18.2 get action  with scalar parm with default 
let VerifyActionWithValueParmWithDefault refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnActionWithValueParameterWithDefault"
    let pmid = "parm5"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm5 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm5"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ]))
                      TProperty(JsonPropertyNames.Default, TObjectVal(4)) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm5) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Value Parameter With Default"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                
                      ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithValueParmWithDefaultObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithValueParmWithDefault "objects" oType oName api.GetAction api

let GetActionWithValueParmWithDefaultService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithValueParmWithDefault "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithValueParmWithDefaultViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithValueParmWithDefault "objects" oType oName api.GetAction api

// 18.2 get action  with ref parm 
let VerifyActionWithReferenceParm refType oType oName f (api : RestfulObjectsControllerBase) =
    let oid = ktc "1"
    let pid = "AnActionWithReferenceParameter"
    let pmid = "parm2"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm2"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Reference Parameter"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.MostSimple"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                 
                      TObjectJson(TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                      
                       ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithReferenceParmObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParm "objects" oType oName api.GetAction api

let GetActionWithReferenceParmService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithReferenceParm "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithReferenceParmViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParm "objects" oType oName api.GetAction api

// 18.2 get action  with ref parm with choices
let VerifyActionWithReferenceParmWithChoices refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParameterWithChoices"
    let pmid = "parm4"
    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
  
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let choiceRel = RelValues.Choice + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
    
    let parm4 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm4"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ]))
                      TProperty(JsonPropertyNames.Choices, 
                                TArray([ TObjectJson(obj1)
                                         TObjectJson(obj2) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm4) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Reference Parameter With Choices"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                
                    ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithReferenceParmWithChoicesObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParmWithChoices "objects" oType oName api.GetAction api

let GetActionWithReferenceParmWithChoicesService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithReferenceParmWithChoices "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithReferenceParmWithChoicesViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParmWithChoices "objects" oType oName api.GetAction api

let VerifyActionWithReferenceParmsWithAutoComplete refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParametersWithAutoComplete"
    let pmid0 = "parm0"
    let pmid1 = "parm1"
    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let pmurl = sprintf "%s/%s/actions/%s" refType oName pid
    let pmurl0 = sprintf "%s/params/%s" pmurl pmid0
    let pmurl1 = sprintf "%s/params/%s" pmurl pmid1
    let acurl0 = sprintf "%s/%s" pmurl0 JsonPropertyNames.Prompt
    let acurl1 = sprintf "%s/%s" pmurl1 JsonPropertyNames.Prompt
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
  
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    
    let autoRel0 = RelValues.Prompt + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid0
    let autoRel1 = RelValues.Prompt + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid1
    let argP = 
        TProperty
            (JsonPropertyNames.Arguments, 
             TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(3)) ]))
    let ac0 = TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel0 acurl0 RepresentationTypes.Prompt "" "" true)
    let ac1 = TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel1 acurl1 RepresentationTypes.Prompt "" "" true)
    
    let parm0 = 
        TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                TArray([  ac0 ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm0"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm1 = 
        TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                TArray([ ac1 ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm1"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson([ TProperty(pmid0, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid1, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, 
                    TObjectJson([ TProperty(pmid0, parm0)
                                  TProperty(pmid1, parm1) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Reference Parameters With Auto Complete"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                 
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithReferenceParmsWithAutoCompleteObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParmsWithAutoComplete "objects" oType oName api.GetAction api

let GetActionWithReferenceParmsWithAutoCompleteService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithReferenceParmsWithAutoComplete "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithReferenceParmsWithAutoCompleteViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParmsWithAutoComplete "objects" oType oName api.GetAction api

let VerifyInvokeParmWithAutoComplete refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParametersWithAutoComplete"
    let pmid0 = "parm0"
    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/params/%s/autoComplete" ourl pid pmid0
    let prurl = sprintf "%s/actions/%s/params/%s/prompt" ourl pid pmid0
    let parms = new JObject(new JProperty(JsonPropertyNames.XRoSearchTerm, new JObject(new JProperty("value", "12"))))
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let choiceRel = RelValues.Choice + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid0
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeParmWithAutoCompleteObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoComplete "objects" oType oName api.GetParameterPrompt api

let InvokeParmWithAutoCompleteService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeParmWithAutoComplete "services" oType oName (wrap2 api.GetParameterPromptOnService) api

let InvokeParmWithAutoCompleteViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoComplete "objects" oType oName api.GetParameterPrompt api

let VerifyInvokeParmWithAutoCompleteErrorNoParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParametersWithAutoComplete"
    let pmid0 = "parm0"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/params/%s/autoComplete" ourl pid pmid0
    let parms = new JObject()
    
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvokeParmWithAutoCompleteObjectErrorNoParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoCompleteErrorNoParm "objects" oType oName api.GetParameterPrompt api

let InvokeParmWithAutoCompleteServiceErrorNoParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeParmWithAutoCompleteErrorNoParm "services" oType oName (wrap2 api.GetParameterPromptOnService) api

let InvokeParmWithAutoCompleteViewModelErrorNoParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoCompleteErrorNoParm "objects" oType oName api.GetParameterPrompt api

let VerifyInvokeParmWithAutoCompleteErrorMalformedParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParametersWithAutoComplete"
    let pmid0 = "parm0"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/params/%s/autoComplete" ourl pid pmid0
    let parms = new JObject(new JProperty("x-ro-search-term", new JObject("12")))
    
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvokeParmWithAutoCompleteObjectErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoCompleteErrorNoParm "objects" oType oName api.GetParameterPrompt api

let InvokeParmWithAutoCompleteServiceErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeParmWithAutoCompleteErrorNoParm "services" oType oName (wrap2 api.GetParameterPromptOnService) api

let InvokeParmWithAutoCompleteViewModelErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoCompleteErrorNoParm "objects" oType oName api.GetParameterPrompt api

let VerifyInvokeParmWithAutoCompleteErrorUnrecognisedParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParametersWithAutoComplete"
    let pmid0 = "parm0"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s/params/%s/autoComplete" ourl pid pmid0
    let parms = new JObject(new JProperty("x-ro-unknown", new JObject(new JProperty("value", "12"))))
    
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvokeParmWithAutoCompleteObjectErrorUnrecognisedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoCompleteErrorNoParm "objects" oType oName api.GetParameterPrompt api

let InvokeParmWithAutoCompleteServiceErrorUnrecognisedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeParmWithAutoCompleteErrorNoParm "services" oType oName (wrap2 api.GetParameterPromptOnService) api

let InvokeParmWithAutoCompleteViewModelErrorUnrecognisedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithAutoCompleteErrorNoParm "objects" oType oName api.GetParameterPrompt api

let VerifyInvokeParmWithConditionalChoices refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParameterWithConditionalChoices"
    let pmid0 = "parm4"
    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
    let ourl = sprintf "%s/%s" refType oName
    let prurl = sprintf "%s/actions/%s/params/%s/prompt" ourl pid pmid0
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
    let parms = new JObject(new JProperty("parm4", new JObject(new JProperty("value", refParm))))
    
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" prurl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let choiceRel = RelValues.Choice + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid0
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("3")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "3")) RepresentationTypes.Object mst
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeParmWithConditionalChoicesObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithConditionalChoices "objects" oType oName api.GetParameterPrompt api

let InvokeParmWithConditionalChoicesService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeParmWithConditionalChoices "services" oType oName (wrap2 api.GetParameterPromptOnService) api

let InvokeParmWithConditionalChoicesViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithConditionalChoices "objects" oType oName api.GetParameterPrompt api

let VerifyInvokeParmWithConditionalChoicesErrorMalformedParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParameterWithConditionalChoices"
    let pmid0 = "parm4"
    let ourl = sprintf "%s/%s" refType oName
    let prurl = sprintf "%s/actions/%s/params/%s/prompt" ourl pid pmid0
    let parms = new JObject(new JProperty("parm4", new JObject(new JProperty("value", "12"))))
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" prurl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty
              ("parm4", 
               
               TObjectJson
                   ([ TProperty(JsonPropertyNames.Value, TObjectVal("12"))
                      
                      TProperty
                          (JsonPropertyNames.InvalidReason, TObjectVal("Argument is of wrong type is System.String expect RestfulObjects.Test.Data.MostSimple")) ])) ]
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual
        ("199 RestfulObjects \"Argument is of wrong type is System.String expect RestfulObjects.Test.Data.MostSimple\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvokeParmWithConditionalChoicesObjectErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithConditionalChoicesErrorMalformedParm "objects" oType oName api.GetParameterPrompt api

let InvokeParmWithConditionalChoicesServiceErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeParmWithConditionalChoicesErrorMalformedParm "services" oType oName (wrap2 api.GetParameterPromptOnService) api

let InvokeParmWithConditionalChoicesViewModelErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeParmWithConditionalChoicesErrorMalformedParm "objects" oType oName api.GetParameterPrompt api

let VerifyInvokeValueParmWithConditionalChoices refType oType oName f1 f2 (api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithValueParametersWithConditionalChoices"
    let pmid0 = "parm3"
    let ourl = sprintf "%s/%s" refType oName
    let prurl = sprintf "%s/actions/%s/params/%s/prompt" ourl pid pmid0
    let parms = 
        new JObject(new JProperty("parm3", new JObject(new JProperty("value", "100"))), new JProperty("parm4", new JObject(new JProperty("value", "33"))))
    
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" prurl
    
    jsonSetGetMsg api1.Request url   
    let result = f1 (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api1.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectVal(100)
                             TObjectVal(33) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult
    // repeat for second parm
    let pmid0 = "parm4"
    let prurl = sprintf "%s/actions/%s/params/%s/prompt" ourl pid pmid0
    let url = sprintf "http://localhost/%s" prurl
    
    jsonSetGetMsg api2.Request url   
    let result = f2 (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectVal("100")
                             TObjectVal("33") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeValueParmWithConditionalChoicesObject(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeValueParmWithConditionalChoices "objects" oType oName api1.GetParameterPrompt api2.GetParameterPrompt api1 api2

let InvokeValueParmWithConditionalChoicesService(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeValueParmWithConditionalChoices "services" oType oName (wrap2 api1.GetParameterPromptOnService) (wrap2 api2.GetParameterPromptOnService) api1 api2

let InvokeValueParmWithConditionalChoicesViewModel(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeValueParmWithConditionalChoices "objects" oType oName api1.GetParameterPrompt api2.GetParameterPrompt api1 api2

let VerifyInvokeValueParmWithConditionalChoicesMissingParm refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithValueParametersWithConditionalChoices"
    let pmid0 = "parm3"
    let ourl = sprintf "%s/%s" refType oName
    let prurl = sprintf "%s/actions/%s/params/%s/prompt" ourl pid pmid0
    let parms = new JObject(new JProperty("parm3", new JObject(new JProperty("value", 100))))
    
    let args = CreateArgMapWithReserved parms
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" prurl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid, pmid0, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectVal(100)
                             TObjectVal(0) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeValueParmWithConditionalChoicesObjectErrorMissingParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeValueParmWithConditionalChoicesMissingParm "objects" oType oName api.GetParameterPrompt api

let InvokeValueParmWithConditionalChoicesServiceErrorMissingParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvokeValueParmWithConditionalChoicesMissingParm "services" oType oName (wrap2 api.GetParameterPromptOnService) api

let InvokeValueParmWithConditionalChoicesViewModelErrorMissingParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvokeValueParmWithConditionalChoicesMissingParm "objects" oType oName api.GetParameterPrompt api

// 18.2 get action  with ref parm with default
let VerifyActionWithReferenceParmWithDefault refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithReferenceParameterWithDefault"
    let pmid = "parm6"
    let mst = (ttc "RestfulObjects.Test.Data.MostSimple")
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
   
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let defaultRel = RelValues.Default + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid
    
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    
    let parm6 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm6"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ]))
                      TProperty(JsonPropertyNames.Default, TObjectJson(obj1)) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm6) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Reference Parameter With Default"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                 
                      ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithReferenceParmWithDefaultObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParmWithDefault "objects" oType oName api.GetAction api

let GetActionWithReferenceParmWithDefaultService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithReferenceParmWithDefault "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithReferenceParmWithDefaultViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithReferenceParmWithDefault "objects" oType oName api.GetAction api

// 18.2 get action  with  and scalar parms with defaults and choices
let VerifyActionWithChoicesAndDefault refType oType oName f (api : RestfulObjectsControllerBase) = 
    let pid = "AnActionWithParametersWithChoicesWithDefaults"
    let pmid1 = "parm1"
    let pmid2 = "parm2"
    let pmid7 = "parm7"
    let pmid8 = "parm8"
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let oid = ktc "1"
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let choiceRel = RelValues.Choice + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid8
    let defaultRel = RelValues.Default + makeParm RelParamValues.Action pid + makeParm RelParamValues.Param pmid8
    let choice1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    let choice2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    
    let parm1 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm1"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm7 = 
        TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
                      TProperty(JsonPropertyNames.Choices, 
                                TArray([ TObjectVal(1)
                                         TObjectVal(2)
                                         TObjectVal(3) ]))                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm7"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                              TProperty(JsonPropertyNames.CustomChoices, 
                                                        TObjectJson([ TProperty("1", TObjectVal(1))
                                                                      TProperty("2", TObjectVal(2))
                                                                      TProperty("3", TObjectVal(3)) ]))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm2"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm8 = 
        TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
                      TProperty(JsonPropertyNames.Choices, 
                                TArray([ TObjectJson(choice1)
                                         TObjectJson(choice2) ]))                      
                      TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm8"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson([ TProperty(pmid1, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid7, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid2, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid8, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, 
                    TObjectJson([ TProperty(pmid1, parm1)
                                  TProperty(pmid7, parm7)
                                  TProperty(pmid2, parm2)
                                  TProperty(pmid8, parm8) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action With Parameters With Choices With Defaults"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                             
                      ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
   // Assert.IsTrue(refType = "services" || result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetActionWithChoicesAndDefaultObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithChoicesAndDefault "objects" oType oName api.GetAction api

let GetActionWithChoicesAndDefaultService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyActionWithChoicesAndDefault "services" oType oName (wrap api.GetServiceAction) api

let GetActionWithChoicesAndDefaultViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyActionWithChoicesAndDefault "objects" oType oName api.GetAction api

// 18.2 get contributed action on object 
let GetContributedActionOnContributee(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let oid = ktc "1"
    let pid = "AzContributedAction"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/actions/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = api.GetAction(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)


    let args = TObjectJson([])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, args) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                             
                             ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetContributedActionOnContributeeBaseClass(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let oid = ktc "1"
    let pid = "AzContributedActionOnBaseClass"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/actions/%s" ourl pid 
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = api.GetAction(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let args = TObjectJson([])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action On Base Class"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(false)) ]))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, args) 
                                  :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")
                             
                              ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetContributedActionOnContributeeWithRef(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let pmid = "withOtherAction"
    let oid = ktc "1"
    let pid = "AzContributedActionWithRefParm"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
      
    jsonSetGetMsg api.Request url   
    let result = api.GetAction(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Other Action"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(oType))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action With Ref Parm"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                 
                      ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetContributedActionOnContributeeWithValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let oid = ktc "1"
    let pid = "AzContributedActionWithValueParm"
    let pmid = "parm"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = api.GetAction(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                              TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action With Value Parm"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                                 
                       ])) ]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

// 18.2 get contributed action on owning service 
let GetContributedActionOnContributer(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.RestDataRepository"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let pmid = "withAction"
    let oName = oType
    let pid = "AzContributedAction"
    let ourl = sprintf "services/%s" oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = api.GetServiceAction(oType, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([ ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.WithActionObject"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                               
                       ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    compareObject expected parsedResult

let GetContributedActionOnContributerBaseClass(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.RestDataRepository"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let pmid = "withAction"
    let oName = oType
    let pid = "AzContributedActionOnBaseClass"
    let ourl = sprintf "services/%s" oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
 
    jsonSetGetMsg api.Request url   
    let result = api.GetServiceAction(oType, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.WithAction"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = TObjectJson([ TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, TObjectJson([ TProperty(pmid, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action On Base Class"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                    
                      TObjectJson  (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                   
                    ]))]
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    compareObject expected parsedResult

let GetContributedActionOnContributerWithRef(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.RestDataRepository"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let oName = oType
    let pid = "AzContributedActionWithRefParm"
    let pmid1 = "withAction"
    let pmid2 = "withOtherAction"
    let ourl = sprintf "services/%s" oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
   
    jsonSetGetMsg api.Request url   
    let result = api.GetServiceAction(oType, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.WithActionObject"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Other Action"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.WithActionObject"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson([ TProperty(pmid1, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid2, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, 
                    TObjectJson([ TProperty(pmid1, parm1)
                                  TProperty(pmid2, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action With Ref Parm"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                  
                      TObjectJson  (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")
                       ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsTrue(headers.ETag = null)
    compareObject expected parsedResult

let GetContributedActionOnContributerWithValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.RestDataRepository"
    let rType = (ttc "RestfulObjects.Test.Data.MostSimple")
    let oName = oType
    let pid = "AzContributedActionWithValueParm"
    let pmid1 = "withAction"
    let pmid2 = "parm"
    let ourl = sprintf "services/%s" oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = api.GetServiceAction(oType, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let parm1 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([  ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal(ttc "RestfulObjects.Test.Data.WithActionObject"))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let parm2 = 
        TObjectJson([ TProperty
                          (JsonPropertyNames.Links, 
                           TArray([ ]))
                      TProperty(JsonPropertyNames.Extensions, 
                                TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
                                              TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                              TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                              TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                              TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
    
    let args = 
        TObjectJson([ TProperty(pmid1, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]))
                      TProperty(pmid2, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])
    
    let invokeRelType = RelValues.Invoke + makeParm RelParamValues.Action pid
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Parameters, 
                    TObjectJson([ TProperty(pmid1, parm1)
                                  TProperty(pmid2, parm2) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action With Value Parm"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(true)) ]))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectAction "")
                      TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) :: makePostLinkProp invokeRelType (purl + "/invoke") RepresentationTypes.ActionResult "")                                                               
                       ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectAction), headers.ContentType)
    assertTransactionalCache headers
    Assert.IsTrue(headers.ETag = null)
    compareObject expected parsedResult

// working 
// 18.2 400 - todo need mandatory header to test (eg if-match) 
let VerifyInvalidAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = " " // invalid 
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedObjects.Facade.BadRequestNOSException' was thrown.\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetInvalidActionPropertyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidAction "objects" oType oName api.GetAction api

let GetInvalidActionPropertyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyInvalidAction "services" oType oName (wrap api.GetServiceAction) api

let GetInvalidActionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyInvalidAction "objects" oType oName api.GetAction api

// 18.2 404 
let VerifyNotFoundAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "ANonExistentAction" // doesn't exist 
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetNotFoundActionPropertyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyNotFoundAction "objects" oType oName api.GetAction api

let GetNotFoundActionPropertyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotFoundAction "services" oType oName (wrap api.GetServiceAction) api

let GetNotFoundActionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyNotFoundAction "objects" oType oName api.GetAction api

let VerifyUserDisabledAction refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AUserDisabledAction" // doesn't exist 
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action AUserDisabledAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetUserDisabledActionObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyUserDisabledAction "objects" oType oName api.GetAction api

let GetUserDisabledActionService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyUserDisabledAction "services" oType oName (wrap api.GetServiceAction) api

let GetUserDisabledActionViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyUserDisabledAction "objects" oType oName api.GetAction api

let VerifyHiddenAction refType oType oName f (api : RestfulObjectsControllerBase) =
    let oid = ktc "1"
    let pid = "AHiddenAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url   
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetHiddenActionPropertyObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyHiddenAction "objects" oType oName api.GetAction api

let GetHiddenActionPropertyService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyHiddenAction "services" oType oName (wrap api.GetServiceAction) api

let GetHiddenActionPropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyHiddenAction "objects" oType oName api.GetAction api

let ActionNotFound(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = ktc "1"
    let pid = "ADoesNotExistAction"
    let url = sprintf "http://localhost/objects/%s/%s/actions/%s" oType oName pid

    jsonSetGetMsg api.Request url   
    let result = api.GetAction(oType, oName, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let VerifyNotAcceptableGetActionWrongMediaType refType oType oName f (api : RestfulObjectsControllerBase) = 
    let oid = ktc "1"
    let pid = "AnAction"
    let ourl = sprintf "%s/%s" refType oName
    let purl = sprintf "%s/actions/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl

    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    let result = f (oType, oid, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
       
    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// 406       
let NotAcceptableGetActionWrongMediaTypeObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oName = oType + "/" + ktc "1"
    VerifyNotAcceptableGetActionWrongMediaType "objects" oType oName api.GetAction api

// 406     
let NotAcceptableGetActionWrongMediaTypeService(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let oName = oType
    VerifyNotAcceptableGetActionWrongMediaType "services" oType oName (wrap api.GetServiceAction) api

let NotAcceptableGetActionWrongMediaTypeViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
    let oName = oType + "/" + ktc "1"
    VerifyNotAcceptableGetActionWrongMediaType "objects" oType oName api.GetAction api
