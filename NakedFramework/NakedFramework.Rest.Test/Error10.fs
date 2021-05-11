// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.Error10

open NakedFramework.Rest.Snapshot.Constants
open NakedFramework.Rest.API
open Newtonsoft.Json.Linq
open NUnit.Framework
open System.Net
open Functions

let Error(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "1"
    let pid = "AnError"

    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/objects/%s/%s/actions/%s/invoke" oType oid pid
    jsonSetEmptyPostMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.PostInvoke(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult

    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                             ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(new typeType(RepresentationTypes.Error), headers.ContentType)
    compareObject expected parsedResult

let NotAcceptableError(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "1"
    let pid = "AnError"

    let args = CreateArgMapWithReserved(new JObject())
    let url = sprintf "http://localhost/objects/%s/%s/actions/%s/invoke" oType oid pid
    jsonSetEmptyPostMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    setIfMatch api.Request "*"
    let result = api.PostInvoke(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    
    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, 
                   TArray([TObjectVal(new errorType(" at  in "));
                           TObjectVal(new errorType(" at  in "));
                           TObjectVal(new errorType(" at  in "));
                            ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(new typeType(RepresentationTypes.Error), headers.ContentType)
    compareObject expected parsedResult
