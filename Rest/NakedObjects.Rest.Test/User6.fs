//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.
module User6

//open NUnit.Framework
//open NakedObjects.Rest
//open System.Net
//open System.Net.Http.Headers
//open Newtonsoft.Json.Linq
//open NakedObjects.Rest.Snapshot.Constants
//open System.Web.Http
//open System.Linq
//open RestTestFunctions

//let expected = 
//    [ TProperty(JsonPropertyNames.Links, 
//                TArray([ TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.User RepresentationTypes.User "")
//                         TObjectJson(makeGetLinkProp RelValues.Up SegmentValues.HomePage RepresentationTypes.HomePage "") ]))
//      TProperty(JsonPropertyNames.UserName, TObjectVal("Test"))
//      TProperty(JsonPropertyNames.Roles, TArray([]))
//      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//let GetUser(api : RestfulObjectsControllerBase) = 
//    let url = testRoot + SegmentValues.User
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetUser(args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.User), result.Content.Headers.ContentType)
//    assertUserInfoCache result
//    compareObject expected parsedResult

//let GetUserWithMediaType(api : RestfulObjectsControllerBase) = 
//    let url = testRoot + SegmentValues.User
//    let msg = jsonGetMsg (url)
//    let args = CreateReservedArgs ""
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.User)))
//    api.Request <- msg
//    let result = api.GetUser(args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.User), result.Content.Headers.ContentType)
//    assertUserInfoCache result
//    compareObject expected parsedResult

//// 406   
//let NotAcceptableGetUser(api : RestfulObjectsControllerBase) = 
//    let url = testRoot + SegmentValues.User
//    let msg = jsonGetMsg (url)
//    let args = CreateReservedArgs ""
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.HomePage)))
//    try 
//        api.Request <- msg
//        api.GetUser(args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

