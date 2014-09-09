module User6

open NUnit.Framework
open RestfulObjects.Mvc
open NakedObjects.Surface
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.IO
open Newtonsoft.Json.Linq
open System.Web
open System
open RestfulObjects.Snapshot.Utility
open RestfulObjects.Snapshot.Constants
open System.Web.Http
open System.Collections.Generic
open System.Linq
open RestTestFunctions

let expected = 
    [ TProperty(JsonPropertyNames.Links, 
                TArray([ TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.User RepresentationTypes.User "")
                         TObjectJson(makeGetLinkProp RelValues.Up SegmentValues.HomePage RepresentationTypes.HomePage "") ]))
      TProperty(JsonPropertyNames.UserName, TObjectVal("Test"))
      TProperty(JsonPropertyNames.Roles, TArray([]))
      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

let GetUser(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.User
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetUser(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.User), result.Content.Headers.ContentType)
    assertUserInfoCache result
    compareObject expected parsedResult

let GetUserWithMediaType(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.User
    let msg = jsonGetMsg (url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.User)))
    api.Request <- msg
    let result = api.GetUser(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.User), result.Content.Headers.ContentType)
    assertUserInfoCache result
    compareObject expected parsedResult

// 406   
let NotAcceptableGetUser(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.User
    let msg = jsonGetMsg (url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.HomePage)))
    try 
        api.Request <- msg
        let result = api.GetUser(args)
        Assert.Fail("expect exception")
    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)
