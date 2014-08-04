module Error10

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
open System.Collections.Generic
open System.Linq
open RestTestFunctions

let Error(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "1"
    let pid = "AnError"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let args = CreateArgMap(new JObject())
    api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
    let result = api.PostInvoke(oType, oid, pid, args)
    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty
              (JsonPropertyNames.StackTrace, 
               
               TArray
                   ([ TObjectVal
                          (new errorType("   at RestfulObjects.Test.Data.WithError.AnError() in C:\Naked Objects Internal\REST\RestfulObjects.Test.Data\WithError.cs:line 12")) ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(new typeType(RepresentationTypes.Error), result.Content.Headers.ContentType)
    compareObject expected parsedResult

let NotAcceptableError(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "1"
    let pid = "AnError"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/actions/%s/invoke" ourl pid
    let args = CreateArgMap(new JObject())
    let msg = jsonPostMsg (sprintf "http://localhost/%s" purl) ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectCollection)))
    api.Request <- msg
    let result = api.PostInvoke(oType, oid, pid, args)
    Assert.AreEqual(HttpStatusCode.NotAcceptable, result.StatusCode)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))
          
          TProperty
              (JsonPropertyNames.StackTrace, 
               
               TArray
                   ([ TObjectVal
                          (new errorType("   at RestfulObjects.Test.Data.WithError.AnError() in C:\Naked Objects Internal\REST\RestfulObjects.Test.Data\WithError.cs:line 12")) ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(new typeType(RepresentationTypes.Error), result.Content.Headers.ContentType)
    compareObject expected parsedResult

