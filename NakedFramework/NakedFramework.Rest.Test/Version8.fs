// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.Version8

open NakedObjects.Rest
open NakedFramework.Rest.Snapshot.Constants
open NakedFramework.Rest.Snapshot.Utility
open Newtonsoft.Json.Linq
open NUnit.Framework
open NakedObjects.Rest.Test.Functions
open System.Net
open NakedFramework.Rest.API

let internal capabilities = 
    TObjectJson([ TProperty("protoPersistentObjects", TObjectVal("yes"))
                  TProperty("deleteObjects", TObjectVal("no"))
                  TProperty("validateOnly", TObjectVal("yes"))
                  TProperty("domainModel", TObjectVal("simple"))
                  TProperty("inlinedMemberRepresentations", TObjectVal("yes"))
                  TProperty("blobsClobs", TObjectVal("no")) ])


let internal links = 
    TArray([ TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.Version RepresentationTypes.Version "")
             TObjectJson(makeGetLinkProp RelValues.Up SegmentValues.HomePage RepresentationTypes.HomePage "") ])

let internal expected = 
    [ TProperty(JsonPropertyNames.Links, links)
      TProperty(JsonPropertyNames.SpecVersion, TObjectVal("1.1"))
      TProperty(JsonPropertyNames.ImplVersion, TObjectVal("Naked Objects 12.0.0-beta03"))
      TProperty(JsonPropertyNames.OptionalCapabilities, capabilities)
      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

let GetVersion(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Version
    jsonSetGetMsg api.Request url    
    let result = api.GetVersion()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Version), headers.ContentType)
    assertNonExpiringCache headers
    compareObject expected parsedResult


let GetVersionWithMediaType(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Version
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.Version
    let result = api.GetVersion()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Version), headers.ContentType)
    assertNonExpiringCache headers
    compareObject expected parsedResult

// 406   
let NotAcceptableGetVersion(api : RestfulObjectsControllerBase) = 
   let url = testRoot + SegmentValues.Menus
   jsonSetGetMsgWithProfile api.Request url RepresentationTypes.User
   let result = api.GetVersion()
   let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
   assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult

   let msg = 
       if (api.DebugWarnings) 
       then "199 RestfulObjects \"Failed outgoing json MT validation ic:  urn:org.restfulobjects:repr-types/user  og:  urn:org.restfulobjects:repr-types/version \""
       else "199 RestfulObjects \"Enable DebugWarnings to see message\""

   Assert.AreEqual(msg, headers.Headers.["Warning"].ToString())
   Assert.AreEqual("", jsonResult)
