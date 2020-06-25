// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.DomainServices7

open NakedObjects.Rest
open NakedObjects.Rest.Snapshot.Constants
open NakedObjects.Rest.Snapshot.Utility
open Newtonsoft.Json.Linq
open NUnit.Framework
open NakedObjects.Rest.Test.Functions
open System.Net

let internal getExpected() = 
    let sName1 = ttc "RestfulObjects.Test.Data.RestDataRepository"
    let sName2 = ttc "RestfulObjects.Test.Data.WithActionService"
    let sName3 = ttc "RestfulObjects.Test.Data.ContributorService"
    let sName4 = ttc "RestfulObjects.Test.Data.TestTypeCodeMapper"
    let sName5 = ttc "RestfulObjects.Test.Data.TestKeyCodeMapper"
    let srvRel1 = RelValues.Service + makeParm RelParamValues.ServiceId sName1
    let srvRel2 = RelValues.Service + makeParm RelParamValues.ServiceId sName2
    let srvRel3 = RelValues.Service + makeParm RelParamValues.ServiceId sName3
    let srvRel4 = RelValues.Service + makeParm RelParamValues.ServiceId sName4
    let srvRel5 = RelValues.Service + makeParm RelParamValues.ServiceId sName5
    
    let simpleLinks = 
        TArray([ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self SegmentValues.Services RepresentationTypes.List "" (ttc "System.Object") true)
                 TObjectJson(makeGetLinkProp RelValues.Up SegmentValues.HomePage RepresentationTypes.HomePage "") ])
   
    
    let value = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Rest Data Repository")) 
                    :: makeGetLinkProp srvRel1 (sprintf "services/%s" sName1) RepresentationTypes.Object sName1)
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("With Action Service")) 
                    :: makeGetLinkProp srvRel2 (sprintf "services/%s" sName2) RepresentationTypes.Object sName2)
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Contributor Service")) 
                    :: makeGetLinkProp srvRel3 (sprintf "services/%s" sName3) RepresentationTypes.Object sName3) ])


    let valueWithTTC = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Rest Data Repository")) 
                    :: makeGetLinkProp srvRel1 (sprintf "services/%s" sName1) RepresentationTypes.Object sName1)
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("With Action Service")) 
                    :: makeGetLinkProp srvRel2 (sprintf "services/%s" sName2) RepresentationTypes.Object sName2)
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Contributor Service")) 
                    :: makeGetLinkProp srvRel3 (sprintf "services/%s" sName3) RepresentationTypes.Object sName3) 

               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Test Type Code Mapper")) 
                    :: makeGetLinkProp srvRel4 (sprintf "services/%s" sName4) RepresentationTypes.Object sName4) 

               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Test Key Code Mapper")) 
                    :: makeGetLinkProp srvRel5 (sprintf "services/%s" sName5) RepresentationTypes.Object sName5) ])
   
    let simpleExpected = 
        [ TProperty(JsonPropertyNames.Links, simpleLinks)
          TProperty(JsonPropertyNames.Value, value)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
   

    let simpleExpectedWithTTC = 
        [ TProperty(JsonPropertyNames.Links, simpleLinks)
          TProperty(JsonPropertyNames.Value, valueWithTTC)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    
    (simpleExpected, simpleExpectedWithTTC)

let GetDomainServices(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Services
    jsonSetGetMsg api.Request url
    let result = api.GetServices()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected = fst (getExpected())
    compareObject expected parsedResult

let GetDomainServicesWithTTC(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Services
    jsonSetGetMsg api.Request url
    let result = api.GetServices()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected = snd (getExpected())
    compareObject expected parsedResult

let GetDomainServicesWithMediaType(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Services
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.List
    let result = api.GetServices()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected = fst (getExpected())
    compareObject expected parsedResult

let GetDomainServicesWithMediaTypeWithTTC(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Services
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.List
    let result = api.GetServices()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected = snd (getExpected())
    compareObject expected parsedResult

// 406   
let NotAcceptableGetDomainServices(api : RestfulObjectsControllerBase) = 
   let url = testRoot + SegmentValues.User
   jsonSetGetMsgWithProfile api.Request url RepresentationTypes.User
   let result = api.GetServices()
   let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
   assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
   
   let msg = 
       if (RestSnapshot.DebugWarnings) 
       then "199 RestfulObjects \"Failed outgoing json MT validation ic:  urn:org.restfulobjects:repr-types/user  og:  urn:org.restfulobjects:repr-types/list \""
       else "199 RestfulObjects \"Enable DebugWarnings to see message\""
   
   Assert.AreEqual(msg, headers.Headers.["Warning"].ToString())
   Assert.AreEqual("", jsonResult)