// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module DomainCollection23

open NUnit.Framework
open RestfulObjects.Mvc
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open RestfulObjects.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions

let GetCollectionPropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let pid = "ACollection"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/collections/%s" ourl pid
    let eturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
    let rturl = sprintf "domain-types/%s" "list"
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetCollectionType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
          TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.CollectionDescription "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ReturnType rturl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ElementType eturl RepresentationTypes.DomainType "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.CollectionDescription), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetSetCollectionPropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let pid = "ASet"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/collections/%s" ourl pid
    let eturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
    let rturl = sprintf "domain-types/%s" "set"
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetCollectionType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Set"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
          TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.CollectionDescription "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ReturnType rturl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ElementType eturl RepresentationTypes.DomainType "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.CollectionDescription), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetCollectionPropertyTypeWithDescription(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let pid = "AnEmptyCollection"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/collections/%s" ourl pid
    let eturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
    let rturl = sprintf "domain-types/%s" "list"
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetCollectionType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Empty Collection"))
          TProperty(JsonPropertyNames.Description, TObjectVal("an empty collection for testing"))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
          TProperty(JsonPropertyNames.MemberOrder, TObjectVal(2))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.CollectionDescription "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ReturnType rturl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ElementType eturl RepresentationTypes.DomainType "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.CollectionDescription), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let NotFoundTypeCollectionPropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let pid = "ACollection"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/collections/%s" ourl pid
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetCollectionType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundCollectionPropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let pid = "NoSuchValue"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/collections/%s" ourl pid
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetCollectionType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain collection NoSuchValue in domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotAcceptableGetCollectionPropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let pid = "ACollection"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/collections/%s" ourl pid
    let args = CreateReservedArgs ""
    let msg = jsonGetMsg (sprintf "http://localhost/%s" purl)
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.HomePage)))
    try 
        api.Request <- msg
        api.GetCollectionType(oType, pid, args) |> ignore
        Assert.Fail("expect exception")
    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)