module DomainActionParameter25
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


let GetActionParameterTypeInt (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let pmid = "parm1"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let rturl = sprintf "domain-types/%s" "integer"
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.Number, TObjectVal(0));
                         TProperty(JsonPropertyNames.Name, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm1"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.Optional, TObjectVal(false));
                         TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionParamDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up turl  RepresentationTypes.ActionDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionParamDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult

let GetActionParameterTypeString (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let pmid = "parm2"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let rturl = sprintf "domain-types/%s" "string"
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.Number, TObjectVal(1));
                         TProperty(JsonPropertyNames.Name, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm2"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.Optional, TObjectVal(false));
                         TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionParamDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up turl  RepresentationTypes.ActionDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionParamDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult    

let GetOverloadedActionParameterTypeString (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnOverloadedAction1"
        let pmid = "parm"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let rturl = sprintf "domain-types/%s" "string"
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.Number, TObjectVal(0));
                         TProperty(JsonPropertyNames.Name, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.Optional, TObjectVal(false));
                         TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionParamDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up turl  RepresentationTypes.ActionDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionParamDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult    


let GetActionParameterTypeDateTime (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnActionWithDateTimeParm"
        let pmid = "parm"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let rturl = sprintf "domain-types/%s" "date-time"
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.Number, TObjectVal(0));
                         TProperty(JsonPropertyNames.Name, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.Optional, TObjectVal(false));
                         TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                         TProperty(JsonPropertyNames.Format, TObjectVal("date-time"));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionParamDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up turl  RepresentationTypes.ActionDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionParamDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult    


let GetActionParameterTypeReference (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnActionReturnsScalarWithParameters"
        let pmid = "parm2"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let rturl = sprintf "domain-types/%s" (ttc "RestfulObjects.Test.Data.MostSimple")
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.Number, TObjectVal(1));
                         TProperty(JsonPropertyNames.Name, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm2"));
                         TProperty(JsonPropertyNames.Description, TObjectVal(""));
                         TProperty(JsonPropertyNames.Optional, TObjectVal(false));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionParamDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up turl  RepresentationTypes.ActionDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionParamDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult    


let GetActionParameterTypeStringOptional (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnActionWithOptionalParm"
        let pmid = "parm"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let rturl = sprintf "domain-types/%s" "string"
        let args = CreateReservedArgs ""

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.Number, TObjectVal(0));
                         TProperty(JsonPropertyNames.Name, TObjectVal(pmid));
                         TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Optional Parm"));
                         TProperty(JsonPropertyNames.Description, TObjectVal("an optional parm"));
                         TProperty(JsonPropertyNames.Optional, TObjectVal(true));
                         TProperty(JsonPropertyNames.MaxLength, TObjectVal(101));
                         TProperty(JsonPropertyNames.Pattern, TObjectVal(@"[A-Z]"));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ActionParamDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.Up turl  RepresentationTypes.ActionDescription ""); 
                                                                      TObjectJson(makeGetLinkProp RelValues.ReturnType rturl  RepresentationTypes.DomainType "")]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionParamDescription), result.Content.Headers.ContentType)
        assertNonExpiringCache result 
        compareObject expected parsedResult  
        
let NotFoundType (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let pmid = "parm1"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let args = CreateReservedArgs ""
       
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
        
let NotFoundAction (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "NoSuchAction"
        let pmid = "parm1"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let args = CreateReservedArgs ""
       
        api.Request <-  jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)    
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such domain action NoSuchAction in domain type " + oType + "\"" , result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let NotFoundParm (api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let pmid = "noSuchParm"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let args = CreateReservedArgs ""
        
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetActionParameterType (oType, pid, pmid, args)
        
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such parameter name\"" , result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let NotAcceptableActionParameterType(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let pmid = "parm1"
        let ourl = sprintf "domain-types/%s" oType
        let turl = sprintf "%s/actions/%s" ourl pid 
        let purl = sprintf "%s/params/%s" turl pmid 
        let args = CreateReservedArgs ""
        
        let msg = jsonGetMsg(sprintf "http://localhost/%s" purl)
        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.HomePage)))
      
        try 
            api.Request <- msg
            let result = api.GetActionParameterType (oType, pid, pmid, args)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)