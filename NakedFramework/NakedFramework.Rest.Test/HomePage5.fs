// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.HomePage5

open NakedFramework.Rest
open NakedFramework.Rest.Snapshot.Constants
open NakedFramework.Rest.Snapshot.Utility
open Newtonsoft.Json.Linq
open NUnit.Framework
open NakedObjects.Rest.Test.Functions
open System.Net
open NakedFramework.Rest.API


let GetHomePage(api : RestfulObjectsControllerBase) = 
    let url = testRoot
    jsonSetGetMsg api.Request url
    let result = api.GetHome()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), headers.ContentType)
    //assertNonExpiringCache headers
    compareObject expectedSimple parsedResult

let GetHomePageWithMediaType(api : RestfulObjectsControllerBase) =
    let url = testRoot
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.HomePage
    let result = api.GetHome()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), headers.ContentType)
    //assertNonExpiringCache headers
    compareObject expectedSimple parsedResult

// 406   
let NotAcceptableGetHomePage(api : RestfulObjectsControllerBase) = 
    let url = testRoot
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.User
    let result = api.GetHome()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult

    let msg = 
        if (api.DebugWarnings) 
        then "199 RestfulObjects \"Failed outgoing json MT validation ic:  urn:org.restfulobjects:repr-types/user  og:  urn:org.restfulobjects:repr-types/homepage \""
        else "199 RestfulObjects \"Enable DebugWarnings to see message\""

    Assert.AreEqual(msg, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvalidDomainModelGetHomePage(api : RestfulObjectsControllerBase) = 
    let argS = "x-ro-domain-model=invalid"    
    let url = sprintf "%s?%s" testRoot argS
    jsonSetGetMsg api.Request url  
    api.DomainModel <- "invalid"
    let result = api.GetHome()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Invalid domainModel: invalid\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)