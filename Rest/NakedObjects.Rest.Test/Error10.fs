// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module Error10

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open NakedObjects.Rest.Snapshot.Constants
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
    
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)

    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                            TObjectVal(new errorType(" at  in "));
                             ]))
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
          TProperty(JsonPropertyNames.StackTrace, 
                   TArray([TObjectVal(new errorType(" at  in "));
                           TObjectVal(new errorType(" at  in "));
                           TObjectVal(new errorType(" at  in "));
                            ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(new typeType(RepresentationTypes.Error), result.Content.Headers.ContentType)
    compareObject expected parsedResult
