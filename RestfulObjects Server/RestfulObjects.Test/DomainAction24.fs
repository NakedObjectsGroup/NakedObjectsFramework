// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module DomainAction24
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

let VerifyActionTypeNoParmsScalar oType f (api : RestfulObjectsControllerBase)= 
        let pid = "AnAction"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(false));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetActionTypeObjectNoParmsScalar(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        VerifyActionTypeNoParmsScalar  oType api.GetActionType api

let GetActionTypeServiceNoParmsScalar(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        VerifyActionTypeNoParmsScalar   oType api.GetActionType api

let VerifyOverloadedActionTypeNoParmsScalar oType f (api : RestfulObjectsControllerBase)= 
        let pid = "AnOverloadedAction0"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Overloaded Action"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(false));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetOverloadedActionTypeObjectNoParmsScalar(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        VerifyOverloadedActionTypeNoParmsScalar  oType api.GetActionType api

let GetOverloadedActionTypeServiceNoParmsScalar(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        VerifyOverloadedActionTypeNoParmsScalar   oType api.GetActionType api



let VerifyActionTypeNoParmsVoid oType f (api : RestfulObjectsControllerBase) = 
        let pid = "AnActionReturnsVoid"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" "void"
        let args = CreateReservedArgs ""
      
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Void"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(false));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetActionTypeObjectNoParmsVoid(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        VerifyActionTypeNoParmsVoid  oType api.GetActionType api

let GetActionTypeServiceNoParmsVoid(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        VerifyActionTypeNoParmsVoid   oType api.GetActionType api

let VerifyActionTypeNoParmsCollection oType f (api : RestfulObjectsControllerBase) = 
        let pid = "AnActionReturnsCollection"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" ResultTypes.List
        let eturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
        let args = CreateReservedArgs ""
      
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Collection"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(false));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" );
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ElementType eturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetActionTypeObjectNoParmsCollection(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        VerifyActionTypeNoParmsCollection  oType api.GetActionType api

let GetActionTypeServiceNoParmsCollection(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        VerifyActionTypeNoParmsCollection   oType api.GetActionType api

let VerifyActionTypeParmsScalar oType f (api : RestfulObjectsControllerBase) = 
        let pid = "AnActionReturnsScalarWithParameters"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" "integer"

        let p1url = sprintf "%s/params/parm1" purl 
        let p2url = sprintf "%s/params/parm2" purl 

        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Scalar With Parameters"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(true));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([ TObjectJson(makeGetLinkProp RelValues.ActionParam p1url   RepresentationTypes.ActionParamDescription "" ); 
                                                                          TObjectJson(makeGetLinkProp RelValues.ActionParam p2url  RepresentationTypes.ActionParamDescription "")   ]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetActionTypeObjectParmsScalar(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        VerifyActionTypeParmsScalar  oType api.GetActionType api

let GetActionTypeServiceParmsScalar(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        VerifyActionTypeParmsScalar   oType api.GetActionType api

let VerifyActionTypeParmsVoid oType f (api : RestfulObjectsControllerBase) = 
        let pid = "AnActionReturnsVoidWithParameters"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" "void"
      
        let p1url = sprintf "%s/params/parm1" purl 
        let p2url = sprintf "%s/params/parm2" purl 

        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Void With Parameters"));
                         TProperty(JsonPropertyNames.Description, TObjectVal("an action for testing"));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(true));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(1));
                         TProperty(JsonPropertyNames.Parameters, TArray([TObjectJson(makeGetLinkProp RelValues.ActionParam p1url  RepresentationTypes.ActionParamDescription "" ); 
                                                                         TObjectJson(makeGetLinkProp RelValues.ActionParam p2url  RepresentationTypes.ActionParamDescription "")]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetActionTypeObjectParmsVoid(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        VerifyActionTypeParmsVoid  oType api.GetActionType api

let GetActionTypeServiceParmsVoid(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        VerifyActionTypeParmsVoid   oType api.GetActionType api

let VerifyActionTypeParmsCollection oType f (api : RestfulObjectsControllerBase)= 
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" ResultTypes.List
        let eturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
      
        let p1url = sprintf "%s/params/parm1" purl 
        let p2url = sprintf "%s/params/parm2" purl 

        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Action Returns Collection With Scalar Parameters"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(true));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([TObjectJson(makeGetLinkProp RelValues.ActionParam p1url RepresentationTypes.ActionParamDescription "" ); 
                                                                         TObjectJson(makeGetLinkProp RelValues.ActionParam p2url RepresentationTypes.ActionParamDescription "")]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "" );
                                                                      TObjectJson(makeGetLinkProp RelValues.ElementType eturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetActionTypeObjectParmsCollection(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        VerifyActionTypeParmsCollection  oType api.GetActionType api

let GetActionTypeServiceParmsCollection(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        VerifyActionTypeParmsCollection   oType api.GetActionType api

let GetActionTypeObjectContributedOnContributee(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AzContributedAction"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")

        let args = CreateReservedArgs ""
      
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionType(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(false));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult
    
let GetActionTypeObjectContributedOnContributer(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.RestDataRepository"
        let pid = "AzContributedAction"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid
        let rturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
        let p1url = sprintf "%s/params/withAction" purl 

        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionType(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Az Contributed Action"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.HasParams, TObjectVal(true));
                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                         TProperty(JsonPropertyNames.Parameters, TArray([TObjectJson(makeGetLinkProp RelValues.ActionParam p1url  RepresentationTypes.ActionParamDescription "") ]));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionDescription "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.DomainType "" ); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult




let NotFoundTypeActionType (api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
        let pid = "AnAction"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateReservedArgs ""
        
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionType(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let NotFoundActionType oType f (api : RestfulObjectsControllerBase) = 
        let pid = "NoSuchAction"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateReservedArgs ""
         
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, pid, args)
        
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such domain action NoSuchAction in domain type " + oType + "\"" , result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let NotFoundActionTypeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        NotFoundActionType  oType api.GetActionType api

let NotFoundActionTypeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        NotFoundActionType   oType api.GetActionType api

let NotAcceptableActionType(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnAction"
        let ourl = sprintf "domain-types/%s" oType
        let purl = sprintf "%s/actions/%s" ourl pid    

        let args = CreateReservedArgs ""

        let msg = jsonGetMsg(sprintf "http://localhost/%s" purl)
        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.HomePage)))
      
        try 
            api.Request <- msg
            let result = api.GetActionType(oType, pid, args)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)