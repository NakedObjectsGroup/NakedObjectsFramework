// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module DomainProperty22

open NUnit.Framework
open RestfulObjects.Mvc
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open RestfulObjects.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions

let GetValuePropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let pid = "AValue"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/properties/%s" ourl pid
    let rturl = sprintf "domain-types/%s" "integer"
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetPropertyType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
          TProperty(JsonPropertyNames.Optional, TObjectVal(false))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.PropertyDescription "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ReturnType rturl RepresentationTypes.DomainType "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.PropertyDescription), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetValueStringPropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let pid = "AStringValue"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/properties/%s" ourl pid
    let rturl = sprintf "domain-types/%s" "string"
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetPropertyType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A String Value"))
          TProperty(JsonPropertyNames.Description, TObjectVal("A string value for testing"))
          TProperty(JsonPropertyNames.Optional, TObjectVal(true))
          TProperty(JsonPropertyNames.MaxLength, TObjectVal(101))
          TProperty(JsonPropertyNames.Pattern, TObjectVal("[A-Z]"))
          TProperty(JsonPropertyNames.MemberOrder, TObjectVal(3))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.PropertyDescription "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ReturnType rturl RepresentationTypes.DomainType "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.PropertyDescription), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetValueDateTimePropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let pid = "ADateTimeValue"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/properties/%s" ourl pid
    let rturl = sprintf "domain-types/%s" "date-time"
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetPropertyType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Date Time Value"))
          TProperty(JsonPropertyNames.Description, TObjectVal("A datetime value for testing"))
          TProperty(JsonPropertyNames.Optional, TObjectVal(true))
          TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
          TProperty(JsonPropertyNames.Format, TObjectVal("date-time"))
          TProperty(JsonPropertyNames.MemberOrder, TObjectVal(4))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.PropertyDescription "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ReturnType rturl RepresentationTypes.DomainType "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.PropertyDescription), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetReferencePropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let pid = "AReference"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/properties/%s" ourl pid
    let rturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetPropertyType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.Optional, TObjectVal(false))
          TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.PropertyDescription "")
                             TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.DomainType "")
                             TObjectJson(makeGetLinkProp RelValues.ReturnType rturl RepresentationTypes.DomainType "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.PropertyDescription), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let NotFoundTypePropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let pid = "AValue"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/properties/%s" ourl pid
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetPropertyType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundPropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let pid = "NoSuchValue"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/properties/%s" ourl pid
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
    let result = api.GetPropertyType(oType, pid, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain property NoSuchValue in domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotAcceptableGetValuePropertyType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let pid = "AValue"
    let ourl = sprintf "domain-types/%s" oType
    let purl = sprintf "%s/properties/%s" ourl pid
    let args = CreateReservedArgs ""
    let msg = jsonGetMsg (sprintf "http://localhost/%s" purl)
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.HomePage)))
    try 
        api.Request <- msg
        api.GetPropertyType(oType, pid, args) |> ignore
        Assert.Fail("expect exception")
    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)
