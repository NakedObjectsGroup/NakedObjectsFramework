// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module Version8

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open NakedObjects.Rest.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions

let capabilities = 
    TObjectJson([ TProperty("protoPersistentObjects", TObjectVal("yes"))
                  TProperty("deleteObjects", TObjectVal("no"))
                  TProperty("validateOnly", TObjectVal("yes"))
                  TProperty("domainModel", TObjectVal("simple"))
                  TProperty("inlinedMemberRepresentations", TObjectVal("yes"))
                  TProperty("blobsClobs", TObjectVal("attachments")) ])


let links = 
    TArray([ TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.Version RepresentationTypes.Version "")
             TObjectJson(makeGetLinkProp RelValues.Up SegmentValues.HomePage RepresentationTypes.HomePage "") ])

let expected = 
    [ TProperty(JsonPropertyNames.Links, links)
      TProperty(JsonPropertyNames.SpecVersion, TObjectVal("1.1"))
      TProperty(JsonPropertyNames.ImplVersion, TObjectVal("8.1.3\r\n"))
      TProperty(JsonPropertyNames.OptionalCapabilities, capabilities)
      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]


let GetVersion(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Version
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetVersion(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.Version), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult


let GetVersionWithMediaType(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Version
    let msg = jsonGetMsg (url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.Version)))
    api.Request <- msg
    let result = api.GetVersion(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.Version), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

// 406   
let NotAcceptableGetVersion(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Version
    let msg = jsonGetMsg (url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.User)))
    try 
        api.Request <- msg
        api.GetVersion(args) |> ignore
        Assert.Fail("expect exception")
    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)