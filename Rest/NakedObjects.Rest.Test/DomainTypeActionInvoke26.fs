//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.
module DomainTypeActionInvoke26

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open System.Web
open NakedObjects.Rest.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions
open Microsoft.AspNetCore.Mvc

let VerifyResult result  resultValue oType oRel ooType ooRel (api : RestfulObjectsControllerBase) =
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    let parsedResult = JObject.Parse(jsonResult)
    let rurl = sprintf "http://localhost/domain-types/%s" ooType
    let args = TObjectJson([ TProperty(ooRel, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(oRel))
          TProperty(JsonPropertyNames.Value, TObjectVal(resultValue))
          TProperty
              (JsonPropertyNames.Links, 
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) 
                           :: makeGetLinkProp RelValues.Self (sprintf "domain-types/%s/type-actions/%s/invoke" oType oRel) RepresentationTypes.TypeActionResult  "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.TypeActionResult), headers.ContentType)
    //assertNonExpiringCache headers
    compareObject expected parsedResult

let GetIsSubTypeOfReturnFalseSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = false
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs

    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api

let GetIsSuperTypeOfReturnFalseSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = false
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api

let GetIsSubTypeOfReturnTrueSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = true
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api

let GetIsSuperTypeOfReturnTrueSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = true
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api



let GetIsSubTypeOfReturnFalseFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = false
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api

let GetIsSuperTypeOfReturnFalseFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = false
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api

let GetIsSubTypeOfReturnTrueFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = true
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api

let GetIsSuperTypeOfReturnTrueFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = true
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel api

// filters 














let NotFoundTypeIsSubTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundTypeIsSuperTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundTypeIsSubTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundTypeIsSuperTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "noSuchAction"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type action noSuchAction in domain type %s\"" oType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "noSuchAction"
    let ooRel = JsonPropertyNames.SuperType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type action noSuchAction in domain type %s\"" oType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSuperTypeIsSubTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSubTypeIsSuperTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSuperTypeIsSubTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSubTypeIsSuperTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// not acceptable 
let MissingParmsIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oRel = "isSubtypeOf"
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke" oType oRel
    let args = CreateArgMapFromUrl ""
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", headers.Headers.["Warning"].First().ToString())
    Assert.AreEqual("", jsonResult)

let MalformedSimpleParmsIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = "nosuchtype"
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].First().ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = "nosuchtype"
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    jsonSetGetMsg api.Request url
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].First().ToString())
    Assert.AreEqual("", jsonResult)

let NotAcceptableIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ActionDescription
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext 

    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult

