module HomePage5
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
// open System.Json

let simpleLinks = [ TObjectJson(makeGetLinkProp RelValues.Self     SegmentValues.HomePage    RepresentationTypes.HomePage ""); 
                    TObjectJson(makeGetLinkProp RelValues.User     SegmentValues.User        RepresentationTypes.User "");
                    TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Services SegmentValues.Services RepresentationTypes.List "" "System.Object" true);
                    TObjectJson(makeGetLinkProp RelValues.Version  SegmentValues.Version     RepresentationTypes.Version "")]

let formalLinks = [ TObjectJson(makeGetLinkProp RelValues.Self        SegmentValues.HomePage    RepresentationTypes.HomePage ""); 
                    TObjectJson(makeGetLinkProp RelValues.User        SegmentValues.User        RepresentationTypes.User "");
                    TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Services SegmentValues.Services RepresentationTypes.List "" "System.Object" false);
                    TObjectJson(makeGetLinkProp RelValues.Version     SegmentValues.Version     RepresentationTypes.Version "")  
                    TObjectJson(makeGetLinkProp RelValues.DomainTypes SegmentValues.DomainTypes RepresentationTypes.TypeList "")]

let bothLinks = [ TObjectJson(makeGetLinkProp RelValues.Self        SegmentValues.HomePage    RepresentationTypes.HomePage ""); 
                  TObjectJson(makeGetLinkProp RelValues.User        SegmentValues.User        RepresentationTypes.User "");
                  TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Services SegmentValues.Services RepresentationTypes.List "" "System.Object" true);
                  TObjectJson(makeGetLinkProp RelValues.Version     SegmentValues.Version     RepresentationTypes.Version "")  
                  TObjectJson(makeGetLinkProp RelValues.DomainTypes SegmentValues.DomainTypes RepresentationTypes.TypeList "")]


let expectedBoth = [ TProperty(JsonPropertyNames.Links, TArray(bothLinks)); TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
let expectedFormal = [ TProperty(JsonPropertyNames.Links, TArray(formalLinks)); TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
let expectedSimple = [ TProperty(JsonPropertyNames.Links, TArray(simpleLinks)); TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

let GetHomePage  (api : RestfulObjectsControllerBase)  = 
    let url = testRoot 
    api.Request <- jsonGetMsg(url)
    let args = CreateReservedArgs ""
    let result = api.GetHome(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    compareObject expectedBoth parsedResult

let GetHomePageFormal  (api : RestfulObjectsControllerBase)  = 
    let argS =  "x-ro-domain-model=formal"
    let url = sprintf "%s?%s" testRoot argS
    let args = CreateReservedArgs argS
    api.Request <- jsonGetMsg(url)
    let result = api.GetHome(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    compareObject expectedFormal parsedResult

let GetHomePageSimple  (api : RestfulObjectsControllerBase)  =
    let argS = "x-ro-domain-model=simple"
    let url = sprintf "%s?%s" testRoot argS
    let args = CreateReservedArgs argS
    api.Request <- jsonGetMsg(url)
    let result = api.GetHome(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    compareObject expectedSimple parsedResult

let GetHomePageWithMediaType  (api : RestfulObjectsControllerBase)  = 
    let url = testRoot 
    let msg = jsonGetMsg(url)    
    let args = CreateReservedArgs ""    
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.HomePage)))
    api.Request <- msg
    let result = api.GetHome(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.HomePage), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    compareObject expectedBoth parsedResult

// 406   
let NotAcceptableGetHomePage(api : RestfulObjectsControllerBase) = 
    let url = testRoot 
    let msg = jsonGetMsg(url)
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.User)))

    try 
        let args = CreateReservedArgs ""
        api.Request <- msg
        let result = api.GetHome(args)
        Assert.Fail("expect exception")
    with 
        | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

let InvalidDomainModelGetHomePage(api : RestfulObjectsControllerBase) = 
    let argS = "x-ro-domain-model=invalid"
    let url = sprintf "%s?%s" testRoot argS
    let args = CreateReservedArgs argS
    api.Request <- jsonGetMsg(url)
  
    let result = api.GetHome(args)
    let jsonResult = readSnapshotToJson result
  
    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)

    Assert.AreEqual("199 RestfulObjects \"Invalid domainModel: invalid\"", result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

        