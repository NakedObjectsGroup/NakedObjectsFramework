// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module HomePage5

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open Newtonsoft.Json.Linq
open NakedObjects.Rest.Snapshot.Constants
open RestTestFunctions

let simpleLinks = 
    [ TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.HomePage RepresentationTypes.HomePage "")
      TObjectJson(makeGetLinkProp RelValues.User SegmentValues.User RepresentationTypes.User "")
      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Services SegmentValues.Services RepresentationTypes.List "" "System.Object" true)
      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Menus SegmentValues.Menus RepresentationTypes.List "" "System.Object" true)
      TObjectJson(makeGetLinkProp RelValues.Version SegmentValues.Version RepresentationTypes.Version "") ]

let expectedSimple = 
    [ TProperty(JsonPropertyNames.Links, TArray(simpleLinks))
      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

let GetHomePage(api : RestfulObjectsControllerBase) = 
    setMockContext api
    let url = testRoot
    jsonSetMsg api.Request url
    let args = CreateReservedArgs ""
    let result = api.GetHome(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), result.Content.Headers.ContentType)
    // assertNonExpiringCache result
    compareObject expectedSimple parsedResult


//let GetHomePageWithMediaType(api : RestfulObjectsControllerBase) = 
//    let url = testRoot
//    let msg = jsonGetMsg (url)
//    let args = CreateReservedArgs ""
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.HomePage)))
//    api.Request <- msg
//    let result = api.GetHome(args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), result.Content.Headers.ContentType)
//    assertNonExpiringCache result
//    compareObject expectedSimple parsedResult

//// 406   
//let NotAcceptableGetHomePage(api : RestfulObjectsControllerBase) = 
//    let url = testRoot
//    let msg = jsonGetMsg (url)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.User)))
//    try 
//        let args = CreateReservedArgs ""
//        api.Request <- msg
//        api.GetHome(args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

//let InvalidDomainModelGetHomePage(api : RestfulObjectsControllerBase) = 
//    let argS = "x-ro-domain-model=invalid"
//    let url = sprintf "%s?%s" testRoot argS
//    let args = CreateReservedArgs argS
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetHome(args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Invalid domainModel: invalid\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)