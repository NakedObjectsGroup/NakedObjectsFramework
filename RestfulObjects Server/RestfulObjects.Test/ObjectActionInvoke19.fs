module ObjectActionInvoke19
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
//// open System.Json

   // 19.3 post to invoke non-idempotent action no parms 
let VerifyPostInvokeActionReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PostInvokeActionReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnObject "objects" oType oid api.PostInvoke api
  
let PostInvokeActionReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeActionReturnObject  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnObjectViewModel(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnObject "objects" oType oid api.PostInvoke api

let VerifyPostInvokeOverloadedActionReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnOverloadedAction0"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PostInvokeOverloadedActionReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeOverloadedActionReturnObject "objects" oType oid api.PostInvoke api
  
let PostInvokeOverloadedActionReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeOverloadedActionReturnObject  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeOverloadedActionReturnObjectViewModel(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeOverloadedActionReturnObject "objects" oType oid api.PostInvoke api

let VerifyPostInvokeContributedService refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "ANonContributedAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PostInvokeActionContributedService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.ContributorService"
        let oid = oType 
        VerifyPostInvokeContributedService  "services" oType oid (wrap api.PostInvokeOnService) api

let VerifyPostInvokeActionReturnRedirectedObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsRedirectedObject"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid
      
        try 
           let args = CreateArgMap (new JObject())
           api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
           let result = f(oType, ktc "1", pid, args)
           Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.MovedPermanently, ex.Response.StatusCode)
                                                Assert.AreEqual(new Uri("http://redirectedtoserver/objects/RedirectedToOid"), ex.Response.Headers.Location)

let PostInvokeActionReturnRedirectedObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnRedirectedObject "objects" oType oid api.PostInvoke api
  
let PostInvokeActionReturnRedirectedObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeActionReturnRedirectedObject  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnRedirectedObjectViewModel(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnRedirectedObject "objects" oType oid api.PostInvoke api

let VerifyPostInvokeActionReturnViewModel refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsViewModel"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PostInvokeActionReturnViewModelObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnViewModel "objects" oType oid api.PostInvoke api
  
let PostInvokeActionReturnViewModelService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeActionReturnViewModel  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnViewModelViewModel(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnViewModel "objects" oType oid api.PostInvoke api

let VerifyPostInvokeActionReturnObjectConcurrencySuccess refType oType oid f tag (api : RestfulObjectsControllerBase)=
        let pid = "AnAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true      

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsgAndTag (sprintf "http://localhost/%s" purl) "" tag
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

let GetTag oType oid (api : RestfulObjectsControllerBase) =
        let url = sprintf "http://localhost/objects/%s"  oid
        api.Request <-  jsonGetMsg(url)
        let args = CreateReservedArgs ""
        let result = api.GetObject(oType, ktc "1", args)
        result.Headers.ETag.Tag 
   
let PostInvokeActionReturnObjectObjectConcurrencySuccess(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"         
        VerifyPostInvokeActionReturnObjectConcurrencySuccess "objects" oType oid api.PostInvoke (GetTag oType oid api) api
  
let PostInvokeActionReturnObjectServiceConcurrencySuccess(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType      
        VerifyPostInvokeActionReturnObjectConcurrencySuccess  "services" oType oid (wrap api.PostInvokeOnService) "\"any\"" api

let PostInvokeActionReturnObjectViewModelConcurrencySuccess(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType      
        VerifyPostInvokeActionReturnObjectConcurrencySuccess  "objects" oType oid api.PostInvoke (GetTag oType oid api) api

let VerifyPostInvokeUserDisabledActionReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PostInvokeUserDisabledActionReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeUserDisabledActionReturnObject "objects" oType oid api.PostInvoke api
  
let PostInvokeUserDisabledActionReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeUserDisabledActionReturnObject  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeUserDisabledActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPostInvokeUserDisabledActionReturnObject  "objects" oType oid api.PostInvoke api

let PostInvokeContribActionReturnObject (api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        let pid = "AzContributedAction"
        let ourl = sprintf "%s/%s" "objects"  oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = api.PostInvoke(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

let PostInvokeContribActionReturnObjectBaseClass (api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = ktc "1"   
        let pid = "AzContributedActionOnBaseClass"
        let ourl = sprintf "%s/%s" "objects"  oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = api.PostInvoke(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag)  
        compareObject expected parsedResult

let PostInvokeContribActionReturnObjectWithRefParm (api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid =   oType +  "/1"   
        let pid = "AzContributedActionWithRefParm"
        let ourl = sprintf "%s/%s" "objects"  oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" oType (ktc "1"))).ToString())) 
        let parms =  new JObject (new JProperty("withOtherAction", new JObject(new JProperty(JsonPropertyNames.Value, refParm)))) 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = api.PostInvoke(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

let PostInvokeContribActionReturnObjectWithValueParm (api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid =  oType + "/1"   
        let pid = "AzContributedActionWithValueParm"
        let ourl = sprintf "%s/%s" "objects"  oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("parm", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = api.PostInvoke(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

let VerifyPostInvokeActionReturnNullObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsNull"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, TObjectVal(null));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PostInvokeActionReturnNullObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = ktc "1"   
        VerifyPostInvokeActionReturnNullObject "objects" oType oid api.PostInvoke api
  
let PostInvokeActionReturnNullObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeActionReturnNullObject  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnNullObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPostInvokeActionReturnNullObject  "objects" oType oid api.PostInvoke api

let VerifyPostInvokeActionReturnNullViewModel refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsNullViewModel"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
        let roid = roType + "/" + ktc "1"

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, TObjectVal(null));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PostInvokeActionReturnNullViewModelObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = ktc "1"   
        VerifyPostInvokeActionReturnNullViewModel "objects" oType oid api.PostInvoke api
  
let PostInvokeActionReturnNullViewModelService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeActionReturnNullViewModel  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnNullViewModelViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPostInvokeActionReturnNullViewModel  "objects" oType oid api.PostInvoke api



let VerifyPostInvokeActionReturnObjectValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
          
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)   
        Assert.AreEqual("", jsonResult)
      
let PostInvokeActionReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnObjectValidateOnly "objects" oType oid api.PostInvoke api
  
let PostInvokeActionReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPostInvokeActionReturnObjectValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPostInvokeActionReturnObjectValidateOnly  "objects" oType oid api.PostInvoke api


let VerifyPutInvokeActionReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedIdempotent"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPutMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PutInvokeActionReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPutInvokeActionReturnObject "objects" oType oid api.PutInvoke api
  
let PutInvokeActionReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPutInvokeActionReturnObject  "services" oType oid (wrap api.PutInvokeOnService) api

let PutInvokeActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPutInvokeActionReturnObject  "objects" oType oid api.PutInvoke api

let VerifyPutInvokeActionReturnViewModel refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedIdempotentReturnsViewModel"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPutMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PutInvokeActionReturnViewModelObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPutInvokeActionReturnViewModel "objects" oType oid api.PutInvoke api
  
let PutInvokeActionReturnViewModelService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPutInvokeActionReturnViewModel  "services" oType oid (wrap api.PutInvokeOnService) api

let PutInvokeActionReturnViewModelViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPutInvokeActionReturnViewModel "objects" oType oid api.PutInvoke api



let VerifyPutInvokeActionReturnObjectConcurrencySuccess refType oType oid f tag (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedIdempotent"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPutMsgAndTag (sprintf "http://localhost/%s" purl) "" tag
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PutInvokeActionReturnObjectObjectConcurrencySuccess(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPutInvokeActionReturnObjectConcurrencySuccess "objects" oType oid api.PutInvoke (GetTag oType oid api) api
  
let PutInvokeActionReturnObjectServiceConcurrencySuccess(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPutInvokeActionReturnObjectConcurrencySuccess  "services" oType oid (wrap api.PutInvokeOnService) "\"any\"" api

let PutInvokeActionReturnObjectViewModelConcurrencySuccess(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPutInvokeActionReturnObjectConcurrencySuccess "objects" oType oid api.PutInvoke (GetTag oType oid api) api


let VerifyPutInvokeActionReturnNullObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedIdempotentReturnsNull"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPutMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
  
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, TObjectVal(null));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let PutInvokeActionReturnNullObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPutInvokeActionReturnNullObject "objects" oType oid api.PutInvoke api
  
let PutInvokeActionReturnNullObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPutInvokeActionReturnNullObject  "services" oType oid (wrap api.PutInvokeOnService) api

let PutInvokeActionReturnNullObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPutInvokeActionReturnNullObject  "objects" oType oid api.PutInvoke api

let VerifyPutInvokeActionReturnObjectValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedIdempotent"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)
      
let PutInvokeActionReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyPutInvokeActionReturnObjectValidateOnly "objects" oType oid api.PutInvoke api
  
let PutInvokeActionReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyPutInvokeActionReturnObjectValidateOnly  "services" oType oid (wrap api.PutInvokeOnService) api

let PutInvokeActionReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyPutInvokeActionReturnObjectValidateOnly  "objects" oType oid api.PutInvoke api


let VerifyGetInvokeActionReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedQueryOnly"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([])) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true)])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        let s = parsedResult.ToString()
        compareObject expected parsedResult
      
let GetInvokeActionReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnObject "objects"  oType oid api.GetInvoke api
  
let GetInvokeActionReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyGetInvokeActionReturnObject  "services"  oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnObject "objects"  oType oid api.GetInvoke api

let VerifyGetInvokeActionReturnViewModel refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedQueryOnlyReturnsViewModel"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimpleViewModel"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                          TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple View Model"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simple View Models"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([])) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true)])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let GetInvokeActionReturnViewModelObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnViewModel "objects"  oType oid api.GetInvoke api
  
let GetInvokeActionReturnViewModelService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyGetInvokeActionReturnViewModel  "services"  oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionReturnViewModelViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnViewModel "objects"  oType oid api.GetInvoke api



let VerifyGetInvokeActionReturnObjectConcurrencySuccess refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedQueryOnly"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsgAndTag (sprintf "http://localhost/%s" purl) "\"any\""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([])) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true)])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let GetInvokeActionReturnObjectObjectConcurrencySuccess(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnObjectConcurrencySuccess "objects"  oType oid api.GetInvoke api
  
let GetInvokeActionReturnObjectServiceConcurrencySuccess(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyGetInvokeActionReturnObjectConcurrencySuccess  "services"  oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionReturnObjectViewModelConcurrencySuccess(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnObjectConcurrencySuccess  "objects"  oType oid api.GetInvoke api

let VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedQueryOnly"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([])) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true )])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let GetInvokeActionReturnObjectObjectConcurrencyNoIfMatch(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch "objects"  oType oid api.GetInvoke api
  
let GetInvokeActionReturnObjectServiceConcurrencyNoIfMatch(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch  "services"  oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionReturnObjectViewModelConcurrencyNoIfMatch(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnObjectConcurrencyNoIfMatch  "objects"  oType oid api.GetInvoke api

let VerifyGetInvokeActionReturnNullObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedQueryOnlyReturnsNull"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl) 
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([])) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true)])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, TObjectVal(null));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
let GetInvokeActionReturnNullObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnNullObject "objects" oType oid api.GetInvoke api 
  
let GetInvokeActionReturnNullObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyGetInvokeActionReturnNullObject  "services" oType oid (wrap api.GetInvokeOnService) api 

let GetInvokeActionReturnNullObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnNullObject  "objects" oType oid api.GetInvoke api 

let VerifyGetInvokeActionReturnObjectValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedQueryOnly"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)
      
let GetInvokeActionReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnObjectValidateOnly "objects" oType oid api.GetInvoke api
  
let GetInvokeActionReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType 
        VerifyGetInvokeActionReturnObjectValidateOnly  "services" oType oid (wrap api.GetInvokeOnService)  api

let GetInvokeActionReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType 
        VerifyGetInvokeActionReturnObjectValidateOnly  "objects" oType oid api.GetInvoke api


let VerifyPostInvokeActionReturnObjectWithMediaType refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid
        let msg = jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.ActionResult)))

        let args = CreateArgMap (new JObject())

        api.Request <- msg
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid  "Id"  (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
        
let PostInvokeActionReturnObjectObjectWithMediaType(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"     
        VerifyPostInvokeActionReturnObjectWithMediaType "objects" oType oid api.PostInvoke api
   
let PostInvokeActionReturnObjectServiceWithMediaType(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType    
        VerifyPostInvokeActionReturnObjectWithMediaType  "services" oType oid (wrap api.PostInvokeOnService) api
   
let PostInvokeActionReturnObjectViewModelWithMediaType(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType    
        VerifyPostInvokeActionReturnObjectWithMediaType  "objects" oType oid api.PostInvoke api

let VerifyPostInvokeActionReturnScalar refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsScalar"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "integer"
   
        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(999));
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.ReturnType (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "") ])); 
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))])
                                  
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]
                           
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "number", "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
       
let PostInvokeActionReturnScalarObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnScalar "objects" oType oid api.PostInvoke api
   
let PostInvokeActionReturnScalarService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnScalar  "services" oType oid (wrap api.PostInvokeOnService) api
   
let PostInvokeActionReturnScalarViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnScalar "objects" oType oid api.PostInvoke api

let VerifyPostInvokeActionReturnScalarFormalOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsScalar"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.DomainModel <-  RestControlFlags.DomainModelType.Formal

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "integer"
   
        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(999));
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.ReturnType (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "") ])); 
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))])
                                  
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]
                           
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", false), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
       
let PostInvokeActionReturnScalarObjectFormalOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnScalarFormalOnly "objects" oType oid api.PostInvoke api
   
let PostInvokeActionReturnScalarServiceFormalOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnScalarFormalOnly  "services" oType oid (wrap api.PostInvokeOnService) api
   
let PostInvokeActionReturnScalarViewModelFormalOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnScalarFormalOnly  "objects" oType oid api.PostInvoke api



let VerifyPostInvokeActionReturnEmptyScalar refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsScalarEmpty"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = ttc "string"
   
        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(""));
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.ReturnType (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "") ])); 
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))])
                                      
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]
                           
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
       
let PostInvokeActionReturnEmptyScalarObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnEmptyScalar "objects" oType oid api.PostInvoke api
   
let PostInvokeActionReturnEmptyScalarService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnEmptyScalar  "services" oType oid (wrap api.PostInvokeOnService) api
   
let PostInvokeActionReturnEmptyScalarViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnEmptyScalar "objects" oType oid api.PostInvoke api

let VerifyPostInvokeActionReturnNullScalar refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsScalarNull"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = "string"
      
        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("")); ])
                                      
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar));
                          TProperty(JsonPropertyNames.Result, TObjectVal(null));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]
                           
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
       
let PostInvokeActionReturnNullScalarObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnNullScalar "objects" oType oid api.PostInvoke api
   
let PostInvokeActionReturnNullScalarService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnNullScalar  "services" oType oid (wrap api.PostInvokeOnService) api
   
let PostInvokeActionReturnNullScalarViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnNullScalar "objects" oType oid api.PostInvoke api


let VerifyPostInvokeActionReturnScalarValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsScalar"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)
       
let PostInvokeActionReturnScalarObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnScalarValidateOnly "objects" oType oid api.PostInvoke api
   
let PostInvokeActionReturnScalarServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnScalarValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api
   
let PostInvokeActionReturnScalarViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnScalarValidateOnly "objects" oType oid api.PostInvoke api


let VerifyPostInvokeActionReturnVoid refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsVoid"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        //let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = "void"
    
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Void));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
    
let PostInvokeActionReturnVoidObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnVoid "objects" oType oid api.PostInvoke api

    
let PostInvokeActionReturnVoidService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnVoid  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnVoidViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnVoid "objects" oType oid api.PostInvoke api


let VerifyPostInvokeActionReturnVoidFormalOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsVoid"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.DomainModel <-  RestControlFlags.DomainModelType.Formal

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let roType = "void"
    
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Void));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", false), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
      
    
let PostInvokeActionReturnVoidObjectFormalOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnVoidFormalOnly "objects" oType oid api.PostInvoke api

let PostInvokeActionReturnVoidServiceFormalOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnVoidFormalOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnVoidViewModelFormalOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnVoidFormalOnly  "objects" oType oid api.PostInvoke api



let VerifyPostInvokeActionReturnVoidValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsVoid"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)
      
let PostInvokeActionReturnVoidObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnVoidValidateOnly "objects" oType oid api.PostInvoke api

let PostInvokeActionReturnVoidServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnVoidValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnVoidViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnVoidValidateOnly  "objects" oType oid api.PostInvoke api


let VerifyGetInvokeActionReturnQueryable refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryable"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" true
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" true

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([])) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult  "" roType  true)])
     
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp; 
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
       
let GetInvokeActionReturnQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionReturnQueryable "objects" oType oid api.GetInvoke api
   
let GetInvokeActionReturnQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionReturnQueryable  "services" oType oid (wrap api.GetInvokeOnService) api
    
let GetInvokeActionReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionReturnQueryable   "objects" oType oid api.GetInvoke api
  

let VerifyGetInvokeActionReturnQueryableValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryable"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)    
        Assert.AreEqual("", jsonResult)
       
let GetInvokeActionReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionReturnQueryableValidateOnly "objects" oType oid api.GetInvoke api
   
let GetInvokeActionReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionReturnQueryableValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api
    
let GetInvokeActionReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionReturnQueryableValidateOnly  "objects" oType oid api.GetInvoke api

let VerifyPostInvokeActionReturnCollection refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollection"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType
         
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
     
    
let PostInvokeActionReturnCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnCollection "objects" oType oid api.PostInvoke api
    
let PostInvokeActionReturnCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnCollection  "services" oType oid (wrap api.PostInvokeOnService) api
  
let PostInvokeActionReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnCollection  "objects" oType oid api.PostInvoke api
 
let VerifyPostInvokeActionReturnCollectionFormalOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollection"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.DomainModel <-  RestControlFlags.DomainModelType.Formal

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET"  RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" false
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET"  RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" false
         
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, false), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
     
    
let PostInvokeActionReturnCollectionObjectFormalOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnCollectionFormalOnly "objects" oType oid api.PostInvoke api
    
let PostInvokeActionReturnCollectionServiceFormalOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnCollectionFormalOnly  "services" oType oid (wrap api.PostInvokeOnService) api
  
let PostInvokeActionReturnCollectionViewModelFormalOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnCollectionFormalOnly  "objects" oType oid api.PostInvoke api
 

let VerifyPostInvokeActionReturnEmptyCollection refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionEmpty"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
        
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([  ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true ), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
     
    
let PostInvokeActionReturnEmptyCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnEmptyCollection "objects" oType oid api.PostInvoke api
    
let PostInvokeActionReturnEmptyCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnEmptyCollection  "services" oType oid (wrap api.PostInvokeOnService) api
  
let PostInvokeActionReturnEmptyCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnEmptyCollection  "objects" oType oid api.PostInvoke api
 

let VerifyPostInvokeActionReturnNullCollection refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionNull"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
        
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectVal(null));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
     
    
let PostInvokeActionReturnNullCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnNullCollection "objects" oType oid api.PostInvoke api
    
let PostInvokeActionReturnNullCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnNullCollection  "services" oType oid (wrap api.PostInvokeOnService) api
  
let PostInvokeActionReturnNullCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnNullCollection  "objects" oType oid api.PostInvoke api

let VerifyPostInvokeActionReturnCollectionVerifyOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollection"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)   
        Assert.AreEqual("", jsonResult)
     
let PostInvokeActionReturnCollectionObjectVerifyOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnCollectionVerifyOnly "objects" oType oid api.PostInvoke api
    
let PostInvokeActionReturnCollectionServiceVerifyOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnCollectionVerifyOnly  "services" oType oid (wrap api.PostInvokeOnService) api
  
let PostInvokeActionReturnCollectionViewModelVerifyOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnCollectionVerifyOnly "objects" oType oid api.PostInvoke api
 
    
let VerifyGetInvokeActionWithScalarParmsReturnQuerySimple refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms = "parm2=fred&parm1=100"

        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms

        let args = CreateArgMapFromUrl parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" true
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" true

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(100))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("fred"))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "" roType true)])
     
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag)  
        compareObject expected parsedResult

    
let GetInvokeActionWithScalarParmsReturnQuerySimpleObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnQuerySimple "objects" oType oid api.GetInvoke api

    
let GetInvokeActionWithScalarParmsReturnQuerySimpleService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnQuerySimple  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnQuerySimpleViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithScalarParmsReturnQuerySimple   "objects" oType oid api.GetInvoke api

   
let VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionWithOptionalParmQueryOnly"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms = "parm="

        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms

        let args = CreateArgMapFromUrl parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let args = TObjectJson([TProperty("parm", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(""))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true)])
      

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

    
let GetInvokeActionWithScalarMissingParmsSimpleObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple "objects" oType oid api.GetInvoke api

    
let GetInvokeActionWithScalarMissingParmsSimpleService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarMissingParmsSimpleViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithMissingScalarParmsReturnQuerySimple  "objects" oType oid api.GetInvoke api


let VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms = "parm2=fred&parm1=100&x-ro-validate-only=true"

        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms

        let args = CreateArgMapFromUrl parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)     
        Assert.AreEqual("", jsonResult)

    
let GetInvokeActionWithScalarParmsReturnQuerySimpleObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly "objects" oType oid api.GetInvoke api

    
let GetInvokeActionWithScalarParmsReturnQuerySimpleServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnQuerySimpleViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnQuerySimpleValidateOnly "objects" oType oid api.GetInvoke api

    


let VerifyGetInvokeActionWithScalarParmsReturnQueryFormal refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
        
        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" true
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" true 

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(100))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("fred"))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "" roType true )])
     
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType,  true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

    
let GetInvokeActionWithScalarParmsReturnQueryFormalObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnQueryFormal "objects" oType oid api.GetInvoke api
    
let GetInvokeActionWithScalarParmsReturnQueryFormalService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnQueryFormal  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnQueryFormalViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithScalarParmsReturnQueryFormal "objects" oType oid api.GetInvoke api


let VerifyGetInvokeActionWithScalarMissingParmsFormal refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionWithOptionalParmQueryOnly"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm", new JObject(new JProperty(JsonPropertyNames.Value, "")))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded
        
        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])


        let args = TObjectJson([TProperty("parm", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(""))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true)])
        
        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

    
let GetInvokeActionWithScalarMissingParmsFormalObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarMissingParmsFormal "objects" oType oid api.GetInvoke api
    
let GetInvokeActionWithScalarMissingParmsFormalService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarMissingParmsFormal  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarMissingParmsFormalViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithScalarMissingParmsFormal   "objects" oType oid api.GetInvoke api





let VerifyGetInvokeActionWithScalarParmsReturnQueryFormalValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)       
        Assert.AreEqual("", jsonResult)
    
let GetInvokeActionWithScalarParmsReturnQueryFormalObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnQueryFormalValidateOnly "objects" oType oid api.GetInvoke api
    
let GetInvokeActionWithScalarParmsReturnQueryFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnQueryFormalValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnQueryFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnQueryFormalValidateOnly  "objects" oType oid api.GetInvoke api



let VerifyPostInvokeActionWithScalarParmsReturnCollectionFormal refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100)))) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(100))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("fred"))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeGetLinkProp RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "")])
     
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult,  "", roType,true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

    
let PostInvokeActionWithScalarParmsReturnCollectionFormalObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithScalarParmsReturnCollectionFormal "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithScalarParmsReturnCollectionFormalService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollectionFormal  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnCollectionFormalViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollectionFormal   "objects" oType oid api.PostInvoke api

    



let VerifyPostInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true)) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode) 
        Assert.AreEqual("", jsonResult)

let PostInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly "objects" oType oid api.PostInvoke api
    
let PostInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly  "objects" oType oid api.PostInvoke api

    





    // 19.1 get to invoke action reference parms in formal form 

let VerifyGetInvokeActionWithReferenceParmsReturnQueryFormal refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"


        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

      

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" true
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" true

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(101))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectJson(makeHref  (sprintf "objects/%s/%s" roType (ktc "1")) ))]))]);


        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "" roType true)])
     
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

    
let GetInvokeActionWithReferenceParmsReturnQueryFormalObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithReferenceParmsReturnQueryFormal "objects" oType oid api.GetInvoke api

    
let GetInvokeActionWithReferenceParmsReturnQueryFormalService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnQueryFormal  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnQueryFormalViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithReferenceParmsReturnQueryFormal  "objects" oType oid api.GetInvoke api

    


let VerifyGetInvokeActionWithReferenceParmsReturnQueryFormalValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)
   
let GetInvokeActionWithReferenceParmsReturnQueryFormalObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithReferenceParmsReturnQueryFormalValidateOnly "objects" oType oid api.GetInvoke api

let GetInvokeActionWithReferenceParmsReturnQueryFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnQueryFormalValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnQueryFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnQueryFormalValidateOnly "objects" oType oid api.GetInvoke api

    





let VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormal refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

       
        let roid = roType + "/" + ktc "1"

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(101))]));
                                TProperty("parm2", TObjectJson(makeGetLinkProp "argument" (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType))])


        //let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeGetLinkProp RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult )])
     
        let resultProp = TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) 
   
        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([resultProp;
                                                                           TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult,  "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

    
let PostInvokeActionWithReferenceParmsReturnCollectionFormalObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormal "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnCollectionFormalService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormal  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormal "objects" oType oid api.PostInvoke api

    


let VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)    
        Assert.AreEqual("", jsonResult)

    
let PostInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly  "objects" oType oid api.PostInvoke api

    




let VerifyPostInvokeActionReturnQuery refType oType oid f (api : RestfulObjectsControllerBase) =
   
        let pid = "AnActionReturnsQueryable"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
   
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" true
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" true

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([])) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "" roType true)])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                            TProperty(JsonPropertyNames.Extensions, TObjectJson([]))
                                                                            TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
         
let PostInvokeActionReturnQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnQuery "objects" oType oid api.PostInvoke api
  
let PostInvokeActionReturnQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnQuery  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionReturnQuery "objects" oType oid api.PostInvoke api
  


let VerifyPostInvokeActionReturnQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
   
        let pid = "AnActionReturnsQueryable"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid
        let parms =  new JObject (new JProperty("x-ro-validate-only", true)) 
        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)    
        Assert.AreEqual("", jsonResult)
     
    
let PostInvokeActionReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionReturnQueryValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionReturnQueryValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionReturnQueryValidateOnly  "objects" oType oid api.PostInvoke api

    




    // 19.3 post to invoke action with scalar parms 

let VerifyPostInvokeActionWithScalarParmsReturnQuery refType oType oid f (api : RestfulObjectsControllerBase) =
     
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
                   
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                                  new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100)))) 
      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" true
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" true


        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(100))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("fred"))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "" roType true)])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]));
                                                                           TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult,  "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
 
let PostInvokeActionWithScalarParmsReturnQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithScalarParmsReturnQuery "objects" oType oid api.PostInvoke api
   
let PostInvokeActionWithScalarParmsReturnQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnQuery  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionWithScalarParmsReturnQuery "objects" oType oid api.PostInvoke api
    

let VerifyPostInvokeOverloadedAction refType oType oid f (api : RestfulObjectsControllerBase) =
     
        let pid = "AnOverloadedAction1"
        let ourl = sprintf "%s/%s/%s" refType oType oid
                   
        let parms =  new JObject (new JProperty("parm", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))))
      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"

        let args = TObjectJson([ TProperty("parm", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("fred"))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "" roType true)])

        let args1 = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args1 :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
 
let PostInvokeOverloadedActionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeOverloadedAction "objects" oType oid api.PostInvoke api
   
let PostInvokeOverloadedActionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeOverloadedAction  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeOverloadedActionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeOverloadedAction "objects" oType oid api.PostInvoke api





let VerifyPostInvokeActionWithScalarParmsReturnQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
     
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
            

        
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                                  new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), 
                                  new JProperty("x-ro-validate-only", true)) 

        let args = CreateArgMap parms

        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)  
        Assert.AreEqual("", jsonResult)

    
let PostInvokeActionWithScalarParmsReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithScalarParmsReturnQueryValidateOnly "objects" oType oid api.PostInvoke api
   
let PostInvokeActionWithScalarParmsReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnQuery  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionWithScalarParmsReturnQuery   "objects" oType oid api.PostInvoke api

    



let VerifyPostInvokeActionWithScalarParmsReturnCollection refType oType oid f (api : RestfulObjectsControllerBase) =
     
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
            

        
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                                  new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100)))) 


        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType


        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(100))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("fred"))]))])

        //let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeGetLinkProp RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult )])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]));
                                                                           TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult,  "", roType,true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult

    
let PostInvokeActionWithScalarParmsReturnCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithScalarParmsReturnCollection "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithScalarParmsReturnCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollection  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollection  "objects" oType oid api.PostInvoke api

    




let VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
     
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
                  
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), 
                                  new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), 
                                  new JProperty("x-ro-validate-only", true)) 

        let args = CreateArgMap parms

        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode) 
        Assert.AreEqual("", jsonResult)

    
let PostInvokeActionWithScalarParmsReturnCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithScalarParmsReturnCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithScalarParmsReturnCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithScalarParmsReturnCollectionValidateOnly  "objects" oType oid api.PostInvoke api

    





let VerifyPostInvokeActionWithReferenceParmsReturnQuery refType oType oid f (api : RestfulObjectsControllerBase) =
    
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

      
        let roid = roType + "/" + ktc "1"

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType "" true
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" true

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(101))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectJson(makeHref  (sprintf "objects/%s/%s" roType (ktc "1")) ))]))]);

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult "" roType true)])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                            TProperty(JsonPropertyNames.Extensions, TObjectJson([]));
                                                                            TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult,  "", roType,true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
    
    
let PostInvokeActionWithReferenceParmsReturnQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnQuery "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnQuery  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyPostInvokeActionWithReferenceParmsReturnQuery  "objects" oType oid api.PostInvoke api

    


let VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =  
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)    
        Assert.AreEqual("", jsonResult)
    
    
let PostInvokeActionWithReferenceParmsReturnQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnQueryValidateOnly  "objects" oType oid api.PostInvoke api

    






     
let VerifyPostInvokeActionWithReferenceParmsReturnCollection refType oType oid f (api : RestfulObjectsControllerBase) =
    
        let pid = "AnActionReturnsCollectionWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
       

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

      

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(101))]));
                                TProperty("parm2", TObjectJson(makeGetLinkProp "argument" (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType))])

       // let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeGetLinkProp RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult )])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.List));
                          TProperty(JsonPropertyNames.Result, TObjectJson([TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp RelValues.ElementType (sprintf "domain-types/%s" roType) RepresentationTypes.DomainType "")]));
                                                                           TProperty(JsonPropertyNames.Extensions, TObjectJson([]));
                                                                           TProperty(JsonPropertyNames.Value, TArray([ TObjectJson(obj1); TObjectJson(obj2) ]))]));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))] 

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult,  "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
    
    
let PostInvokeActionWithReferenceParmsReturnCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnCollection "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollection  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollection  "objects" oType oid api.PostInvoke api

    

 
let VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)     
        Assert.AreEqual("", jsonResult)
    
    
let PostInvokeActionWithReferenceParmsReturnCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnCollectionValidateOnly  "objects" oType oid api.PostInvoke api

    

 
 
        
  // w
let VerifyPostInvokeActionWithReferenceParmsReturnScalar refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsScalarWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let mst = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" mst (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 
        
        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = "integer"
   
        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(555));
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.ReturnType (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "") ])); 
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))])

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Scalar));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, "number", "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
    
    
let PostInvokeActionWithReferenceParmsReturnScalarObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnScalar "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnScalarService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnScalar  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnScalarViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnScalar  "objects" oType oid api.PostInvoke api

    

      

let VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsScalarWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 
       
        let args = CreateArgMap parms

        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode) 
        Assert.AreEqual("", jsonResult)
    
    
let PostInvokeActionWithReferenceParmsReturnScalarObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnScalarServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnScalarViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnScalarValidateOnly  "objects" oType oid api.PostInvoke api

    



        
  
let VerifyPostInvokeActionWithReferenceParmsReturnVoid refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsVoidWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

      
        let roid = roType + "/" + ktc "1"

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "1"))  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp RelValues.Element (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Void));
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
     
    
let PostInvokeActionWithReferenceParmsReturnVoidObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnVoid "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnVoidService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnVoid  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnVoidViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnVoid "objects" oType oid api.PostInvoke api

    

 
let VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsVoidWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)
     
    
let PostInvokeActionWithReferenceParmsReturnVoidObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnVoidServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnVoidViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnVoidValidateOnly  "objects" oType oid api.PostInvoke api

    

        
let VerifyPostInvokeActionWithReferenceParmsReturnObject refType oType oid f (api : RestfulObjectsControllerBase)=
        let pid = "AnActionReturnsObjectWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

      
        let roid = roType + "/" + ktc "1"

        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [TProperty("Id",  TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])


        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
        
let PostInvokeActionWithReferenceParmsReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnObject "objects" oType oid api.PostInvoke api
 
let PostInvokeActionWithReferenceParmsReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnObject  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnObject  "objects" oType oid api.PostInvoke api
   


let VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsObjectWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)      
        Assert.AreEqual("", jsonResult)
    
    
let PostInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oid api.PostInvoke api

    
let PostInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostInvokeActionWithReferenceParmsReturnObjectValidateOnly  "objects" oType oid api.PostInvoke api

    


 
let VerifyPutInvokeActionWithReferenceParmsReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsObjectWithParametersAnnotatedIdempotent"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        
        let roid = roType + "/" + ktc "1"


        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makePutLinkProp RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson( [ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])


        let expected =  [ TProperty(JsonPropertyNames.Links, TArray([]));
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
    
    
let PutInvokeActionWithReferenceParmsReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPutInvokeActionWithReferenceParmsReturnObject "objects" oType oid api.PutInvoke api

    
let PutInvokeActionWithReferenceParmsReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPutInvokeActionWithReferenceParmsReturnObject  "services" oType oid (wrap api.PutInvokeOnService) api

let PutInvokeActionWithReferenceParmsReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPutInvokeActionWithReferenceParmsReturnObject  "objects" oType oid api.PutInvoke api

    

 

let VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsObjectWithParametersAnnotatedIdempotent"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 
        
        let args = CreateArgMap parms

        api.Request <- jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)      
        Assert.AreEqual("", jsonResult)
    
    
let PutInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oid api.PutInvoke api

    
let PutInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly  "services" oType oid (wrap api.PutInvokeOnService) api

let PutInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPutInvokeActionWithReferenceParmsReturnObjectValidateOnly  "objects" oType oid api.PutInvoke api

    
let VerifyGetInvokeActionWithReferenceParmsReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsObjectWithParametersAnnotatedQueryOnly"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded 

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        
        let roid = roType + "/" + ktc "1"


        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(101))]));
                                TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectJson(makeHref (sprintf "objects/%s/%s" roType (ktc "1")) ))]))]);

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true )])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
    
    
let GetInvokeActionWithReferenceParmsReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithReferenceParmsReturnObject "objects" oType oid api.GetInvoke api
  
let GetInvokeActionWithReferenceParmsReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnObject  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithReferenceParmsReturnObject  "objects" oType oid api.GetInvoke api
    

let VerifyGetInvokeActionWithParmReturnObject refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsObjectWithParameterAnnotatedQueryOnly"
        let ourl = sprintf "%s/%s/%s" refType oType oid

       
        let parms =  new JObject (new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded 

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"


        let args = TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty("Id", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))

        let resultObject =  TObjectJson([ TProperty(JsonPropertyNames.DomainType, TObjectVal(roType)); TProperty(JsonPropertyNames.InstanceId, TObjectVal(ktc "1"));
                                          TProperty(JsonPropertyNames.Title, TObjectVal("1") );
                                          TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true);
                                                                                       TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s" roType)  RepresentationTypes.DomainType "");
                                                                                       //TObjectJson(makeIconLink());
                                                                                       TObjectJson(args :: makeLinkPropWithMethodAndTypes "PUT" RelValues.Update (sprintf "objects/%s" roid)  RepresentationTypes.Object roType "" true) ]));
                                          TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeObjectPropertyMember "Id" roid "Id" (TObjectVal(1)))) ]));
                                          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.DomainType, TObjectVal(roType));
                                                                                               TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"));
                                                                                               TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                                               TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                                               TProperty(JsonPropertyNames.IsService, TObjectVal(false))]) ) ])

        let args = TObjectJson([TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(101))]))])

        let links = TArray([ TObjectJson(TProperty(JsonPropertyNames.Arguments, args) ::  makeLinkPropWithMethodAndTypes "GET" RelValues.Self (sprintf "%s/%s/actions/%s/invoke" refType oid pid) RepresentationTypes.ActionResult roType "" true )])

        let expected =  [ TProperty(JsonPropertyNames.Links, links);
                          TProperty(JsonPropertyNames.ResultType, TObjectVal(ResultTypes.Object));
                          TProperty(JsonPropertyNames.Result, resultObject);
                          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ActionResult, roType, "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsNull(result.Headers.ETag) 
        compareObject expected parsedResult
    
    
let GetInvokeActionWithParmReturnObjectObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithParmReturnObject "objects" oType oid api.GetInvoke api
  
let GetInvokeActionWithParmReturnObjectService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithParmReturnObject  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithParmReturnObjectViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithParmReturnObject  "objects" oType oid api.GetInvoke api



let VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsObjectWithParametersAnnotatedQueryOnly"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded 

        let args = CreateArgMap parms
        api.Request <-  jsonGetMsg (sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode) 
        Assert.AreEqual("", jsonResult)
    
    
let GetInvokeActionWithReferenceParmsReturnObjectObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly "objects" oType oid api.GetInvoke api

    
let GetInvokeActionWithReferenceParmsReturnObjectServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnObjectViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnObjectValidateOnly  "objects" oType oid api.GetInvoke api

   
    // get action missing arguments - 400

let VerifyMissingParmsOnGetQuery refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))]))]

        Assert.AreEqual( unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Mandatory\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult

    
let MissingParmsOnGetQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMissingParmsOnGetQuery "objects" oType oid api.GetInvoke api

    
let MissingParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMissingParmsOnGetQuery  "services" oType oid (wrap api.GetInvokeOnService) api

let MissingParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMissingParmsOnGetQuery  "objects" oType oid api.GetInvoke api

    



let VerifyMissingParmsOnPostCollection refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))]))]

        Assert.AreEqual( unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Mandatory\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult

    
let MissingParmsOnPostCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMissingParmsOnPostCollection "objects" oType oid api.PostInvoke api

    
let MissingParmsOnPostCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMissingParmsOnPostCollection  "services" oType oid (wrap api.PostInvokeOnService) api

let MissingParmsOnPostCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMissingParmsOnPostCollection   "objects" oType oid api.PostInvoke api

    


    // malformed args 

let VerifyMalformedSimpleParmsOnGetQuery refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms = "malformed=fred&parm1=100"
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms

        let args = CreateArgMapFromUrl parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let MalformedSimpleParmsOnGetQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedSimpleParmsOnGetQuery "objects" oType oid api.GetInvoke api
    
let MalformedSimpleParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedSimpleParmsOnGetQuery  "services" oType oid (wrap api.GetInvokeOnService) api

let MalformedSimpleParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedSimpleParmsOnGetQuery  "objects" oType oid api.GetInvoke api

let VerifyMalformedFormalParmsOnGetQuery refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
  
let MalformedFormalParmsOnGetQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedFormalParmsOnGetQuery "objects" oType oid api.GetInvoke api
 
let MalformedFormalParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedFormalParmsOnGetQuery  "services" oType oid (wrap api.GetInvokeOnService) api

let MalformedFormalParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedFormalParmsOnGetQuery  "objects" oType oid api.GetInvoke api
 


let VerifyMalformedFormalParmsOnGetCollection refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100)))) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <-  jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnPostCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedFormalParmsOnGetCollection "objects" oType oid api.PostInvoke api

let MalformedFormalParmsOnPostCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedFormalParmsOnGetCollection  "services" oType oid (wrap api.PostInvokeOnService) api

let MalformedFormalParmsOnPostCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedFormalParmsOnGetCollection  "objects" oType oid api.PostInvoke api
  


let VerifyInvalidSimpleParmsOnGetQuery refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms = "parm2=fred&parm1=invalidvalue"
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms

        let args = CreateArgMapFromUrl parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal("fred"))]))]

        Assert.AreEqual( HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
  
let InvalidSimpleParmsOnGetQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidSimpleParmsOnGetQuery "objects" oType oid api.GetInvoke api
 
let InvalidSimpleParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidSimpleParmsOnGetQuery  "services" oType oid (wrap api.GetInvokeOnService) api

let InvalidSimpleParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidSimpleParmsOnGetQuery "objects" oType oid api.GetInvoke api
   


let VerifyInvalidFormalParmsOnGetQuery refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue")))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal("fred"))]))]

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
    
let InvalidFormalParmsOnGetQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidFormalParmsOnGetQuery "objects" oType oid api.GetInvoke api
   
let InvalidFormalParmsOnGetQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidFormalParmsOnGetQuery  "services" oType oid (wrap api.GetInvokeOnService) api

let InvalidFormalParmsOnGetQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidFormalParmsOnGetQuery  "objects" oType oid api.GetInvoke api
    


let VerifyInvalidFormalParmsOnPostCollection refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue")))) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 
        
        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal("fred"))]))]

        Assert.AreEqual( HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult

    
let InvalidFormalParmsOnPostCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidFormalParmsOnPostCollection "objects" oType oid api.PostInvoke api

    
let InvalidFormalParmsOnPostCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidFormalParmsOnPostCollection  "services" oType oid (wrap api.PostInvokeOnService) api

let InvalidFormalParmsOnPostCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidFormalParmsOnPostCollection "objects" oType oid api.PostInvoke api

    


let VerifyDisabledActionInvokeQuery refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "ADisabledQueryAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Always disabled\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
 
let DisabledActionInvokeQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyDisabledActionInvokeQuery "objects" oType oid api.GetInvoke api
   
let DisabledActionInvokeQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyDisabledActionInvokeQuery  "services" oType oid (wrap api.GetInvokeOnService) api

let DisabledActionInvokeQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyDisabledActionInvokeQuery  "objects" oType oid api.GetInvoke api
  


let VerifyDisabledActionInvokeCollection refType oType oid f (api : RestfulObjectsControllerBase) =  
        let error = "Always disabled" 
        let pid = "ADisabledCollectionAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

    
let DisabledActionInvokeCollectionObject(api : RestfulObjectsControllerBase)  =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyDisabledActionInvokeCollection "objects" oType oid api.PostInvoke  api

    
let DisabledActionInvokeCollectionService(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyDisabledActionInvokeCollection  "services" oType oid (wrap api.PostInvokeOnService)  api

let DisabledActionInvokeCollectionViewModel(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyDisabledActionInvokeCollection  "objects" oType oid api.PostInvoke  api

  
    // 404 
    
let VerifyNotFoundActionInvoke refType oType oid f (api : RestfulObjectsControllerBase)=   
        let pid = "ANonExistentAction" // doesn't exist 
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

    
let NotFoundActionInvokeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyNotFoundActionInvoke "objects" oType oid api.GetInvoke api

    
let NotFoundActionInvokeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyNotFoundActionInvoke  "services" oType oid (wrap api.GetInvokeOnService) api

let NotFoundActionInvokeViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyNotFoundActionInvoke  "objects" oType oid api.GetInvoke api

    


let VerifyHiddenActionInvoke refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AHiddenAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let HiddenActionInvokeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyHiddenActionInvoke "objects" oType oid api.GetInvoke api
    
let HiddenActionInvokeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyHiddenActionInvoke  "services" oType oid (wrap api.GetInvokeOnService) api

let HiddenActionInvokeViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyHiddenActionInvoke  "objects" oType oid api.GetInvoke api

let VerifyGetActionWithSideEffects refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsVoid" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let GetActionWithSideEffectsObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetActionWithSideEffects "objects" oType oid api.GetInvoke api
    
let GetActionWithSideEffectsService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetActionWithSideEffects  "services" oType oid (wrap api.GetInvokeOnService) api

let GetActionWithSideEffectsViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetActionWithSideEffects  "objects" oType oid api.GetInvoke api
    
let VerifyGetActionWithIdempotent refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionAnnotatedIdempotent" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let GetActionWithIdempotentObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetActionWithIdempotent "objects" oType oid api.GetInvoke api
    
let GetActionWithIdempotentService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetActionWithIdempotent  "services" oType oid (wrap api.GetInvokeOnService) api

let GetActionWithIdempotentViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetActionWithIdempotent  "objects" oType oid api.GetInvoke api
    
let VerifyPutActionWithQueryOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsVoid" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPutMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not idempotent\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let PutActionWithQueryOnlyObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPutActionWithQueryOnly "objects" oType oid api.PutInvoke api
    
let PutActionWithQueryOnlyService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPutActionWithQueryOnly  "services" oType oid (wrap api.PutInvokeOnService) api

let PutActionWithQueryOnlyViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPutActionWithQueryOnly  "objects" oType oid api.PutInvoke api

    


let VerifyNotAcceptableGetInvokeWrongMediaType refType oType oid f (api : RestfulObjectsControllerBase) =  
        let pid = "AnActionReturnsQueryable" 
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s" ourl pid

        try 
            let args = CreateArgMap (new JObject())
            let msg = jsonGetMsg(sprintf "http://localhost/%s" purl)
            msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.ActionDescription)))
            api.Request <- msg
            let result = f(oType, ktc "1", pid, args)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)
    
let NotAcceptableGetInvokeWrongMediaTypeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = ktc "1"
        VerifyNotAcceptableGetInvokeWrongMediaType "objects" oType oid api.GetInvoke api
     
let NotAcceptableGetInvokeWrongMediaTypeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyNotAcceptableGetInvokeWrongMediaType  "services" oType oid (wrap api.GetInvokeOnService) api

let NotAcceptableGetInvokeWrongMediaTypeViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyNotAcceptableGetInvokeWrongMediaType  "objects" oType oid api.GetInvoke api



let VerifyGetQueryActionWithError refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnErrorQuery" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let expected = [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"));
                         TProperty(JsonPropertyNames.StackTrace, TArray([ TObjectVal( new errorType("   at RestfulObjects.Test.Data.WithError.AnError() in C:\Naked Objects Internal\REST\RestfulObjects.Test.Data\WithError.cs:line 12"))]));
                         TProperty(JsonPropertyNames.Links, TArray([]))
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult

    
let GetQueryActionWithErrorObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetQueryActionWithError "objects" oType oid api.GetInvoke api

    
let GetQueryActionWithErrorService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetQueryActionWithError  "services" oType oid (wrap api.GetInvokeOnService) api

let GetQueryActionWithErrorViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetQueryActionWithError  "objects" oType oid api.GetInvoke api

    


let VerifyPostCollectionActionWithError refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnErrorCollection" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let expected = [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"));
                         TProperty(JsonPropertyNames.StackTrace, TArray([ TObjectVal( new errorType("   at RestfulObjects.Test.Data.WithError.AnError() in C:\Naked Objects Internal\REST\RestfulObjects.Test.Data\WithError.cs:line 12"))]));
                         TProperty(JsonPropertyNames.Links, TArray([]))
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult

    
let PostCollectionActionWithErrorObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostCollectionActionWithError "objects" oType oid api.PostInvoke api

    
let PostCollectionActionWithErrorService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostCollectionActionWithError  "services" oType oid (wrap api.PostInvokeOnService) api

let PostCollectionActionWithErrorViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostCollectionActionWithError  "objects" oType oid api.PostInvoke api

    


let VerifyMissingParmsOnPost refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsScalarWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

   
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))]))]

        Assert.AreEqual( unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Mandatory\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult

    
let MissingParmsOnPostObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMissingParmsOnPost "objects" oType oid api.PostInvoke api

    
let MissingParmsOnPostService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMissingParmsOnPost  "services" oType oid (wrap api.PostInvokeOnService) api

let MissingParmsOnPostViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMissingParmsOnPost  "objects" oType oid api.PostInvoke api

    


let VerifyMalformedFormalParmsOnPostQuery refType oType oid f (api : RestfulObjectsControllerBase) =
    
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
     
let MalformedFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedFormalParmsOnPostQuery "objects" oType oid api.PostInvoke api
    
let MalformedFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedFormalParmsOnPostQuery  "services" oType oid (wrap api.PostInvokeOnService) api

let MalformedFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedFormalParmsOnPostQuery  "objects" oType oid api.PostInvoke api



let VerifyWrongTypeParmsOnPostQuery refType oType oid f (api : RestfulObjectsControllerBase) =
    
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid

        let parms =  new JObject (new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 1))), new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, 2)))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(1))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(2)); 
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Not a suitable type; must be a Most Simple"))]))];

        Assert.AreEqual(unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Not a suitable type; must be a Most Simple\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
    
    
let WrongTypeFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyWrongTypeParmsOnPostQuery "objects" oType oid api.PostInvoke api
    
let WrongTypeFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyWrongTypeParmsOnPostQuery  "services" oType oid (wrap api.PostInvokeOnService) api   


let WrongTypeFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyWrongTypeParmsOnPostQuery  "objects" oType oid api.PostInvoke api

let VerifyEmptyParmsOnPostQuery refType oType oid f (api : RestfulObjectsControllerBase) =
    
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid

        let parms =  new JObject (new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, ""))), new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "")))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(""));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))])) 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("")); 
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))]))];

        Assert.AreEqual(unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Mandatory\", 199 RestfulObjects \"Mandatory\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
    
    
let EmptyFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyEmptyParmsOnPostQuery "objects" oType oid api.PostInvoke api
    
let EmptyFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyEmptyParmsOnPostQuery  "services" oType oid (wrap api.PostInvokeOnService) api   

let EmptyFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyEmptyParmsOnPostQuery "objects" oType oid api.PostInvoke api




let VerifyInvalidFormalParmsOnPostQuery refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue")))) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

       

        let refParm = TObjectJson(makeHref (sprintf "objects/%s/%s" roType (ktc "1")));

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, refParm)]))]

        Assert.AreEqual( HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
  
let InvalidFormalParmsOnPostQueryObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidFormalParmsOnPostQuery "objects" oType oid api.PostInvoke api

    
let InvalidFormalParmsOnPostQueryService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidFormalParmsOnPostQuery  "services" oType oid (wrap api.PostInvokeOnService) api

let InvalidFormalParmsOnPostQueryViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidFormalParmsOnPostQuery  "objects" oType oid api.PostInvoke api
       
       // Always disabled
let VerifyDisabledPostActionInvoke refType oType oid f (api : RestfulObjectsControllerBase)=   
        let error = "Always disabled"
        let pid = "ADisabledAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

    
let DisabledActionPostInvokeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyDisabledPostActionInvoke "objects" oType oid api.PostInvoke  api

    
let DisabledActionPostInvokeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyDisabledPostActionInvoke  "services" oType oid (wrap api.PostInvokeOnService)  api

let DisabledActionPostInvokeViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyDisabledPostActionInvoke "objects" oType oid api.PostInvoke  api



let VerifyUserDisabledPostActionInvoke refType oType oid f (api : RestfulObjectsControllerBase) = 
        let error = "Always disabled"  
        let pid = "AUserDisabledAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action AUserDisabledAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
  
let UserDisabledActionPostInvokeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyUserDisabledPostActionInvoke "objects" oType oid api.PostInvoke  api
    
let UserDisabledActionPostInvokeService(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyUserDisabledPostActionInvoke  "services" oType oid (wrap api.PostInvokeOnService)  api

let UserDisabledActionPostInvokeViewModel(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyUserDisabledPostActionInvoke  "objects" oType oid api.PostInvoke  api

let VerifyNotFoundActionPostInvoke refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "ANonExistentAction" // doesn't exist 
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let NotFoundActionPostInvokeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyNotFoundActionPostInvoke "objects" oType oid api.PostInvoke api
    
let NotFoundActionPostInvokeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyNotFoundActionPostInvoke  "services" oType oid (wrap api.PostInvokeOnService) api

let NotFoundActionPostInvokeViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyNotFoundActionPostInvoke "objects" oType oid api.PostInvoke api

let VerifyHiddenActionPostInvoke refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AHiddenAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let HiddenActionPostInvokeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyHiddenActionPostInvoke "objects" oType oid api.PostInvoke api
    
let HiddenActionPostInvokeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyHiddenActionPostInvoke  "services" oType oid (wrap api.PostInvokeOnService) api

let HiddenActionPostInvokeViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyHiddenActionPostInvoke  "objects" oType oid api.PostInvoke api

let VerifyNotAcceptablePostInvokeWrongMediaType refType oType oid f (api : RestfulObjectsControllerBase) =  
        let pid = "AnAction" 
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/properties/%s" ourl pid

        let args = CreateArgMap (new JObject())

        try 
            let msg = jsonPostMsg(sprintf "http://localhost/%s" purl) ""
            msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.ObjectCollection)))
            api.Request <- msg
            let result = f(oType, ktc "1", pid, args)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)
  
let NotAcceptablePostInvokeWrongMediaTypeObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyNotAcceptablePostInvokeWrongMediaType "objects" oType oid api.PostInvoke api
     
let NotAcceptablePostInvokeWrongMediaTypeService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyNotAcceptablePostInvokeWrongMediaType  "services" oType oid (wrap api.PostInvokeOnService) api

let NotAcceptablePostInvokeWrongMediaTypeViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyNotAcceptablePostInvokeWrongMediaType  "objects" oType oid api.PostInvoke api

     


let VerifyPostQueryActionWithError refType oType oid f (api : RestfulObjectsControllerBase)=   
        let pid = "AnErrorQuery" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let expected = [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"));
                         TProperty(JsonPropertyNames.StackTrace, TArray([ TObjectVal( new errorType("   at RestfulObjects.Test.Data.WithError.AnError() in C:\Naked Objects Internal\REST\RestfulObjects.Test.Data\WithError.cs:line 12"))]));
                         TProperty(JsonPropertyNames.Links, TArray([]))
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
   
let PostQueryActionWithErrorObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostQueryActionWithError "objects" oType oid api.PostInvoke api
   
let PostQueryActionWithErrorService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostQueryActionWithError  "services" oType oid (wrap api.PostInvokeOnService) api

let PostQueryActionWithErrorViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostQueryActionWithError  "objects" oType oid api.PostInvoke api
  


let VerifyPostQueryActionWithValidateFail refType oType oid f (api : RestfulObjectsControllerBase)=   
        let pid = "AnActionValidateParameters" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, 0))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 0)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(0));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Fail validation parm1"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal(0))]))]

        Assert.AreEqual( unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Fail validation parm1\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult
   
let PostQueryActionWithValidateFailObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostQueryActionWithValidateFail "objects" oType oid api.PostInvoke api
   
let PostQueryActionWithValidateFailService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostQueryActionWithValidateFail  "services" oType oid (wrap api.PostInvokeOnService) api

let PostQueryActionWithValidateFailViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostQueryActionWithValidateFail  "objects" oType oid api.PostInvoke api

let VerifyPostQueryActionWithCrossValidateFail refType oType oid f (api : RestfulObjectsControllerBase)=   
        let pid = "AnActionValidateParameters" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, 1))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 2)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(2))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(1))]));
                        TProperty(JsonPropertyNames.XRoInvalidReason, TObjectVal("Cross validation failed"))]

        Assert.AreEqual(unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult
   
let PostQueryActionWithCrossValidateFailObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostQueryActionWithCrossValidateFail "objects" oType oid api.PostInvoke api
   
let PostQueryActionWithCrossValidateFailService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostQueryActionWithCrossValidateFail  "services" oType oid (wrap api.PostInvokeOnService) api

let PostQueryActionWithCrossValidateFailViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostQueryActionWithCrossValidateFail  "objects" oType oid api.PostInvoke api

let VerifyGetInvokeActionReturnCollection refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollection"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
        
let GetInvokeActionReturnCollectionObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionReturnCollection "objects" oType oid api.GetInvoke api
    
let GetInvokeActionReturnCollectionService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionReturnCollection  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionReturnCollectionViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionReturnCollection  "objects" oType oid api.GetInvoke api
  
let VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke?parm2=fred&parm1=100" ourl pid

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let GetInvokeActionWithScalarParmsReturnCollectionSimpleObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple "objects" oType oid api.GetInvoke api
    
let GetInvokeActionWithScalarParmsReturnCollectionSimpleService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithScalarParmsReturnCollectionSimple  "objects" oType oid api.GetInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap (new JObject())
        api.Request <-  jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let GetInvokeActionWithScalarParmsReturnCollectionFormalObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal "objects" oType oid api.GetInvoke api
    
let GetInvokeActionWithScalarParmsReturnCollectionFormalService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionFormalViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithScalarParmsReturnCollectionFormal  "objects" oType oid api.GetInvoke api

let VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101)))) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
  
let GetInvokeActionWithReferenceParmsReturnCollectionFormalObject(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal "objects" oType oid api.GetInvoke api
    
let GetInvokeActionWithReferenceParmsReturnCollectionFormalService(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModel(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType + "/" + ktc "1"   
        VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormal "objects" oType oid api.GetInvoke api

// new validate only

let VerifyMissingParmsOnGetQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap (new JObject())
        api.Request <-  jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))]))]

        Assert.AreEqual( unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Mandatory\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult
    
let MissingParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMissingParmsOnGetQueryValidateOnly "objects" oType oid api.GetInvoke api
    
let MissingParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMissingParmsOnGetQueryValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let MissingParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMissingParmsOnGetQueryValidateOnly  "objects" oType oid api.PostInvoke api

let VerifyMissingParmsOnPostCollectionValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))]))]

        Assert.AreEqual( unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Mandatory\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult

    
let MissingParmsOnPostCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMissingParmsOnPostCollectionValidateOnly "objects" oType oid api.PostInvoke api

    
let MissingParmsOnPostCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMissingParmsOnPostCollectionValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let MissingParmsOnPostCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMissingParmsOnPostCollectionValidateOnly  "objects" oType oid api.PostInvoke api

    


    // malformed args 

let VerifyMalformedSimpleParmsOnGetQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms = "malformed=fred&parm1=100&x-ro-validate-only=true"
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms

        let args = CreateArgMapFromUrl parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

    
let MalformedSimpleParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedSimpleParmsOnGetQueryValidateOnly "objects" oType oid api.GetInvoke api

    
let MalformedSimpleParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedSimpleParmsOnGetQueryValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let MalformedSimpleParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedSimpleParmsOnGetQueryValidateOnly "objects" oType oid api.PostInvoke api

    


let VerifyMalformedFormalParmsOnGetQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedFormalParmsOnGetQueryValidateOnly "objects" oType oid api.GetInvoke api
 
let MalformedFormalParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedFormalParmsOnGetQueryValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api
 
let MalformedFormalParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedFormalParmsOnGetQueryValidateOnly  "objects" oType oid api.PostInvoke api
 
let VerifyMalformedFormalParmsOnGetCollectionValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true)) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 
        
        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let MalformedFormalParmsOnPostCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedFormalParmsOnGetCollectionValidateOnly "objects" oType oid api.PostInvoke api

let MalformedFormalParmsOnPostCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedFormalParmsOnGetCollectionValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let MalformedFormalParmsOnPostCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedFormalParmsOnGetCollectionValidateOnly  "objects" oType oid api.PostInvoke api
  


let VerifyInvalidSimpleParmsOnGetQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms = "parm2=fred&parm1=invalidvalue&x-ro-validate-only=true"
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parms

        let args = CreateArgMapFromUrl parms
        api.Request <-  jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let roid = roType + "/" + ktc "1"
    
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal("fred"))]))]

        Assert.AreEqual( HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
  
let InvalidSimpleParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidSimpleParmsOnGetQueryValidateOnly "objects" oType oid api.GetInvoke api
 
let InvalidSimpleParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidSimpleParmsOnGetQueryValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let InvalidSimpleParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidSimpleParmsOnGetQueryValidateOnly  "objects" oType oid api.PostInvoke api
   


let VerifyInvalidFormalParmsOnGetQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsQueryableWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))), new JProperty("x-ro-validate-only", true)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap parms
        api.Request <-  jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal("fred"))]))]

        Assert.AreEqual( HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
    
let InvalidFormalParmsOnGetQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidFormalParmsOnGetQueryValidateOnly "objects" oType oid api.GetInvoke api
   
let InvalidFormalParmsOnGetQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidFormalParmsOnGetQueryValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let InvalidFormalParmsOnGetQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidFormalParmsOnGetQueryValidateOnly  "objects" oType oid api.PostInvoke api
    


let VerifyInvalidFormalParmsOnPostCollectionValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))), new JProperty("x-ro-validate-only", true)) 
        //let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal("fred"))]))]

        Assert.AreEqual( HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult

    
let InvalidFormalParmsOnPostCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidFormalParmsOnPostCollectionValidateOnly "objects" oType oid api.PostInvoke api

    
let InvalidFormalParmsOnPostCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidFormalParmsOnPostCollectionValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let InvalidFormalParmsOnPostCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidFormalParmsOnPostCollectionValidateOnly  "objects" oType oid api.PostInvoke api

    


let VerifyDisabledActionInvokeQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "ADisabledQueryAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Always disabled\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
 
let DisabledActionInvokeQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyDisabledActionInvokeQueryValidateOnly "objects" oType oid api.GetInvoke api
   
let DisabledActionInvokeQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyDisabledActionInvokeQueryValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let DisabledActionInvokeQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyDisabledActionInvokeQueryValidateOnly  "objects" oType oid api.PostInvoke api
  


let VerifyDisabledActionInvokeCollectionValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =  
        let error = "Always disabled" 
        let pid = "ADisabledCollectionAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <-  jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let DisabledActionInvokeCollectionObjectValidateOnly(api : RestfulObjectsControllerBase)  =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyDisabledActionInvokeCollectionValidateOnly "objects" oType oid api.PostInvoke  api
    
let DisabledActionInvokeCollectionServiceValidateOnly(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyDisabledActionInvokeCollectionValidateOnly  "services" oType oid (wrap api.PostInvokeOnService)  api

let DisabledActionInvokeCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyDisabledActionInvokeCollectionValidateOnly  "objects" oType oid api.PostInvoke api

// 404 
let VerifyNotFoundActionInvokeValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "ANonExistentAction" // doesn't exist 
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let NotFoundActionInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyNotFoundActionInvokeValidateOnly "objects" oType oid api.GetInvoke api
    
let NotFoundActionInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyNotFoundActionInvokeValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let NotFoundActionInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyNotFoundActionInvokeValidateOnly  "objects" oType oid api.GetInvoke api

let VerifyHiddenActionInvokeValidateOnly refType oType oid f (api : RestfulObjectsControllerBase)=   
        let pid = "AHiddenAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())


        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let HiddenActionInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyHiddenActionInvokeValidateOnly "objects" oType oid api.GetInvoke api
    
let HiddenActionInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyHiddenActionInvokeValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let HiddenActionInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyHiddenActionInvokeValidateOnly "objects" oType oid api.GetInvoke api

let VerifyGetActionWithSideEffectsValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsVoid" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let GetActionWithSideEffectsObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetActionWithSideEffectsValidateOnly "objects" oType oid api.GetInvoke api
    
let GetActionWithSideEffectsServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetActionWithSideEffectsValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetActionWithSideEffectsViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetActionWithSideEffectsValidateOnly  "objects" oType oid api.GetInvoke api
    
let VerifyGetActionWithIdempotentValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionAnnotatedIdempotent" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let GetActionWithIdempotentObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetActionWithIdempotentValidateOnly "objects" oType oid api.GetInvoke api
    
let GetActionWithIdempotentServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetActionWithIdempotentValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetActionWithIdempotentViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetActionWithIdempotentValidateOnly  "objects" oType oid api.GetInvoke api
    
let VerifyPutActionWithQueryOnlyValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsVoid" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <-  jsonPutMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not idempotent\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
   
let PutActionWithQueryOnlyObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPutActionWithQueryOnlyValidateOnly "objects" oType oid api.PutInvoke api
    
let PutActionWithQueryOnlyServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPutActionWithQueryOnlyValidateOnly  "services" oType oid (wrap api.PutInvokeOnService) api

let PutActionWithQueryOnlyViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPutActionWithQueryOnlyValidateOnly  "objects" oType oid api.PutInvoke api
    
let VerifyGetQueryActionWithErrorValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnErrorQuery" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid   
        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap parms

        api.Request <-  jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded )
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)     
        Assert.AreEqual("", jsonResult)

    
let GetQueryActionWithErrorObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetQueryActionWithErrorValidateOnly "objects" oType oid api.GetInvoke api

    
let GetQueryActionWithErrorServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetQueryActionWithErrorValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetQueryActionWithErrorViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetQueryActionWithErrorValidateOnly  "objects" oType oid api.GetInvoke api

    


let VerifyPostCollectionActionWithErrorValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnErrorCollection" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)

    
let PostCollectionActionWithErrorObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostCollectionActionWithErrorValidateOnly "objects" oType oid api.PostInvoke api

    
let PostCollectionActionWithErrorServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostCollectionActionWithErrorValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let PostCollectionActionWithErrorViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostCollectionActionWithErrorValidateOnly  "objects" oType oid api.PostInvoke api
    


let VerifyMissingParmsOnPostValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsScalarWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

   
        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value,TObjectVal(null));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Mandatory"))]))]

        Assert.AreEqual( unprocessableEntity, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
        Assert.AreEqual("199 RestfulObjects \"Mandatory\"", result.Headers.Warning.First().ToString())
        compareObject expected parsedResult

    
let MissingParmsOnPostObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMissingParmsOnPostValidateOnly "objects" oType oid api.PostInvoke api

    
let MissingParmsOnPostServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMissingParmsOnPostValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let MissingParmsOnPostViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMissingParmsOnPostValidateOnly "objects" oType oid api.PostInvoke api

    



let VerifyMalformedFormalParmsOnPostQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
    
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("malformed", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
      
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let MalformedFormalParmsOnPostQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyMalformedFormalParmsOnPostQueryValidateOnly "objects" oType oid api.PostInvoke api

let MalformedFormalParmsOnPostQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyMalformedFormalParmsOnPostQueryValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let MalformedFormalParmsOnPostQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyMalformedFormalParmsOnPostQueryValidateOnly  "objects" oType oid api.PostInvoke api

let VerifyInvalidFormalParmsOnPostQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, "invalidvalue"))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let refParm = TObjectJson(makeHref (sprintf "objects/%s/%s" roType (ktc "1")));

        let expected = [TProperty("parm1", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal("invalidvalue"));
                                                        TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Invalid Entry"))])); 
                        TProperty("parm2", TObjectJson([TProperty(JsonPropertyNames.Value, refParm)]))]

        Assert.AreEqual( HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", result.Headers.Warning.ToString())
        compareObject expected parsedResult
  
let InvalidFormalParmsOnPostQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidFormalParmsOnPostQueryValidateOnly "objects" oType oid api.PostInvoke api

    
let InvalidFormalParmsOnPostQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidFormalParmsOnPostQueryValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let InvalidFormalParmsOnPostQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidFormalParmsOnPostQueryValidateOnly "objects" oType oid api.PostInvoke api



let VerifyInvalidUrlOnPostQueryValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =
        let pid = "AnActionReturnsQueryableWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType (ktc "10") )).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, ""))), new JProperty("x-ro-validate-only", true)) 
        let purl = sprintf "%s/actions/%s/invoke" ourl pid 

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain object %s-%s\"" roType (ktc "10"),  result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
  
let InvalidUrlOnPostQueryObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyInvalidUrlOnPostQueryValidateOnly "objects" oType oid api.PostInvoke api
    
let InvalidUrlOnPostQueryServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyInvalidUrlOnPostQueryValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let InvalidUrlOnPostQueryViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyInvalidUrlOnPostQueryValidateOnly  "objects" oType oid api.PostInvoke api
       
// Always disabled
let VerifyDisabledPostActionInvokeValidateOnly refType oType oid f  (api : RestfulObjectsControllerBase)= 
        let error = "Always disabled"  
        let pid = "ADisabledAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let DisabledActionPostInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyDisabledPostActionInvokeValidateOnly "objects" oType oid api.PostInvoke  api
    
let DisabledActionPostInvokeServiceValidateOnly(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyDisabledPostActionInvokeValidateOnly  "services" oType oid (wrap api.PostInvokeOnService)  api

let DisabledActionPostInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase)  = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyDisabledPostActionInvokeValidateOnly  "objects" oType oid api.PostInvoke  api

let VerifyNotFoundActionPostInvokeValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "ANonExistentAction" // doesn't exist 
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action ANonExistentAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let NotFoundActionPostInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyNotFoundActionPostInvokeValidateOnly "objects" oType oid api.PostInvoke api
    
let NotFoundActionPostInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyNotFoundActionPostInvokeValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let NotFoundActionPostInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyNotFoundActionPostInvokeValidateOnly  "objects" oType oid api.PostInvoke api

let VerifyHiddenActionPostInvokeValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AHiddenAction" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such action AHiddenAction\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let HiddenActionPostInvokeObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyHiddenActionPostInvokeValidateOnly "objects" oType oid api.PostInvoke api
    
let HiddenActionPostInvokeServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyHiddenActionPostInvokeValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api

let HiddenActionPostInvokeViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyHiddenActionPostInvokeValidateOnly  "objects" oType oid api.PostInvoke api

let VerifyPostQueryActionWithErrorValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnErrorQuery" 
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))

        let args = CreateArgMap parms

        api.Request <- jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode)
        Assert.AreEqual("", jsonResult)
   
let PostQueryActionWithErrorObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyPostQueryActionWithErrorValidateOnly "objects" oType oid api.PostInvoke api
   
let PostQueryActionWithErrorServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyPostQueryActionWithErrorValidateOnly  "services" oType oid (wrap api.PostInvokeOnService) api


let PostQueryActionWithErrorViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyPostQueryActionWithErrorValidateOnly  "objects" oType oid api.PostInvoke api
  


let VerifyGetInvokeActionReturnCollectionValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollection"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
        
let GetInvokeActionReturnCollectionObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionReturnCollectionValidateOnly "objects" oType oid api.GetInvoke api
    
let GetInvokeActionReturnCollectionServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionReturnCollectionValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionReturnCollectionViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionReturnCollectionValidateOnly  "objects" oType oid api.GetInvoke api
  
let VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly refType oType oid f (api : RestfulObjectsControllerBase)=   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let purl = sprintf "%s/actions/%s/invoke?parm2=fred&parm1=100" ourl pid

        let parms =  new JObject (new JProperty("x-ro-validate-only", true))
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let GetInvokeActionWithScalarParmsReturnCollectionSimpleObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly "objects" oType oid api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnCollectionSimpleServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionSimpleViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnCollectionSimpleValidateOnly  "objects" oType oid api.GetInvoke api

let VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly refType oType oid f (api : RestfulObjectsControllerBase)=   
        let pid = "AnActionReturnsCollectionWithScalarParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, "fred"))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 100))), new JProperty("x-ro-validate-only", true)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap (new JObject())
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let GetInvokeActionWithScalarParmsReturnCollectionFormalObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly "objects" oType oid api.GetInvoke api

let GetInvokeActionWithScalarParmsReturnCollectionFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithScalarParmsReturnCollectionFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionWithScalarParmsReturnCollectionFormalValidateOnly  "objects" oType oid api.GetInvoke api

let VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly refType oType oid f (api : RestfulObjectsControllerBase) =   
        let pid = "AnActionReturnsCollectionWithParameters"
        let ourl = sprintf "%s/%s/%s" refType oType oid
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType((sprintf "objects/%s/%s" roType (ktc "1")))).ToString())) 

        let parms =  new JObject (new JProperty("parm2", new JObject(new JProperty(JsonPropertyNames.Value, refParm))), new JProperty("parm1", new JObject(new JProperty(JsonPropertyNames.Value, 101))), new JProperty("x-ro-validate-only", true)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
        let purl = sprintf "%s/actions/%s/invoke?%s" ourl pid parmsEncoded

        let args = CreateArgMap (new JObject())

        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
       
        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"action is not side-effect free\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
    
let GetInvokeActionWithReferenceParmsReturnCollectionFormalObjectValidateOnly(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"
        VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly "objects" oType oid api.GetInvoke api
    
let GetInvokeActionWithReferenceParmsReturnCollectionFormalServiceValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionService"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly  "services" oType oid (wrap api.GetInvokeOnService) api

let GetInvokeActionWithReferenceParmsReturnCollectionFormalViewModelValidateOnly(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithActionViewModel"
        let oid = oType
        VerifyGetInvokeActionWithReferenceParmsReturnCollectionFormalValidateOnly  "objects" oType oid api.GetInvoke api
    
let VerifyPostInvokeActionReturnObjectConcurrencyFail refType oType oid f tag (api : RestfulObjectsControllerBase) =
        let pid = "AnAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true      

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPostMsgAndTag (sprintf "http://localhost/%s" purl) "" tag
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.PreconditionFailed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnObjectObjectConcurrencyFail(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"         
        VerifyPostInvokeActionReturnObjectConcurrencyFail "objects" oType oid api.PostInvoke "\"fail\"" api

let VerifyPutInvokeActionReturnObjectConcurrencyFail refType oType oid f tag (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedIdempotent"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPutMsgAndTag (sprintf "http://localhost/%s" purl) "" tag
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.PreconditionFailed, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
      
let PutInvokeActionReturnObjectObjectConcurrencyFail(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = ktc "1"   
        VerifyPutInvokeActionReturnObjectConcurrencyFail "objects" oType oid api.PutInvoke "\"fail\"" api

let VerifyPostInvokeActionReturnObjectMissingIfMatch refType oType oid f tag (api : RestfulObjectsControllerBase) =
        let pid = "AnAction"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true     
        
        let args = CreateArgMap (new JObject()) 

        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(preconditionHeaderMissing, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let PostInvokeActionReturnObjectObjectMissingIfMatch(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = oType + "/" + ktc "1"         
        VerifyPostInvokeActionReturnObjectMissingIfMatch "objects" oType oid api.PostInvoke "\"fail\"" api

let VerifyPutInvokeActionReturnObjectMissingIfMatch refType oType oid f tag (api : RestfulObjectsControllerBase) =
        let pid = "AnActionAnnotatedIdempotent"
        let ourl = sprintf "%s/%s" refType oid      
        let purl = sprintf "%s/actions/%s/invoke" ourl pid

        RestfulObjectsControllerBase.ConcurrencyChecking <- true

        let args = CreateArgMap (new JObject())

        api.Request <- jsonPutMsg (sprintf "http://localhost/%s" purl) ""
        let result = f(oType, ktc "1", pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(preconditionHeaderMissing, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
      
let PutInvokeActionReturnObjectObjectMissingIfMatch(api : RestfulObjectsControllerBase) =
        let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
        let oid = ktc "1"   
        VerifyPutInvokeActionReturnObjectMissingIfMatch "objects" oType oid api.PutInvoke "\"fail\"" api
  

  




