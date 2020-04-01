// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.User6

open NakedObjects.Rest
open NakedObjects.Rest.Snapshot.Constants
open NakedObjects.Rest.Snapshot.Utility
open Newtonsoft.Json.Linq
open NUnit.Framework
open NakedObjects.Rest.Test.Functions
open System.Net


let GetUser(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.User
    jsonSetGetMsg api.Request url
    let result = api.GetUser()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.User), headers.ContentType)
    //assertUserInfoCache headers
    compareObject expectedUser parsedResult

let GetUserWithMediaType(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.User
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.User
    let result = api.GetUser()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.User), headers.ContentType)
    //assertUserInfoCache headers
    compareObject expectedUser parsedResult

// 406   
let NotAcceptableGetUser(api : RestfulObjectsControllerBase) = 
   let url = testRoot + SegmentValues.User
   jsonSetGetMsgWithProfile api.Request url RepresentationTypes.HomePage
   let result = api.GetUser()
   let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
   assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult

   let msg = 
       if (RestSnapshot.DebugWarnings) 
       then "199 RestfulObjects \"Failed outgoing json MT validation ic:  urn:org.restfulobjects:repr-types/homepage  og:  urn:org.restfulobjects:repr-types/user \""
       else "199 RestfulObjects \"\""

   Assert.AreEqual(msg, headers.Headers.["Warning"].ToString())
   Assert.AreEqual("", jsonResult)