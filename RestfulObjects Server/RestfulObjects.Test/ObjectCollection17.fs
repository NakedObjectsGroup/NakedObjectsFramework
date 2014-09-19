// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module ObjectCollection17
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

let GetCollectionProperty(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let oid2 = ktc "2"
        let pid = "ACollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2)  RepresentationTypes.Object roType

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection "ACollection"
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection "ACollection"

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List));
                                                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"));
                                                                              TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                                                                              TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"));
                                                                              TProperty(JsonPropertyNames.ElementType, TObjectVal(roType))]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object (oType));
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true); 
                                                                     TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s/collections/%s" oType pid)  RepresentationTypes.CollectionDescription "")]))];                                                     
                                                                    
        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetCollectionPropertyViewModel(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollectionViewModel"
        let oid = ktc "1-2"
        let oid1 = ktc "1"
        let oid2 = ktc "2"
        let pid = "ACollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid1)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2)  RepresentationTypes.Object roType

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection "ACollection"
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection "ACollection"

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List));
                                                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"));
                                                                              TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                                                                              TProperty(JsonPropertyNames.ElementType, TObjectVal(roType))]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object (oType));
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true); 
                                                                     TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s/collections/%s" oType pid)  RepresentationTypes.CollectionDescription "")]))]                                                 
                                                                   
        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult


let GetCollectionPropertyFormalOnly(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let oid2 = ktc "2"
        let pid = "ACollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let argS = "x-ro-domain-model=formal"
        let url = sprintf "%s?%s" purl argS

        let args = CreateReservedArgs argS
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" url)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection pid
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection pid
        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType "" false
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" valueRel (sprintf "objects/%s/%s" roType oid2)  RepresentationTypes.Object roType "" false

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([] ));
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Up ourl  RepresentationTypes.Object (oType) "" false); 
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType false); 
                                                                     TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s/collections/%s" oType pid)  RepresentationTypes.CollectionDescription "")]))]                                                  

        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, false), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetCollectionPropertySimpleOnly(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ACollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let argS = "x-ro-domain-model=simple"
        let url = sprintf "%s?%s" purl argS

        let args = CreateReservedArgs argS
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" url)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
        
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection "ACollection"
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection "ACollection"

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List));
                                                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"));
                                                                              TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                                                                              TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"));
                                                                              TProperty(JsonPropertyNames.ElementType, TObjectVal(roType))]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object (oType)); 
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)]))]                                                     
                                                                    
        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetCollectionSetProperty(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ASet"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection "ASet"
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection "ASet"

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"));
                                                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Set"));
                                                                              TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                                                                              TProperty(JsonPropertyNames.ElementType, TObjectVal(roType))]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object (oType));
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true); 
                                                                     TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s/collections/%s" oType pid)  RepresentationTypes.CollectionDescription "")]))]                                                
                                                                  
        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetCollectionSetPropertyFormalOnly(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ASet"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let argS = "x-ro-domain-model=formal"
        let url = sprintf "%s?%s" purl argS

        let args = CreateReservedArgs argS
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" url)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection pid
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection pid
        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeLinkPropWithMethodAndTypes "GET" valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType "" false
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeLinkPropWithMethodAndTypes "GET" valueRel (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType "" false

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([] ));
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Up ourl  RepresentationTypes.Object (ttc "RestfulObjects.Test.Data.WithCollection") "" false); 
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType false); 
                                                                     TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s/collections/%s" oType pid)  RepresentationTypes.CollectionDescription "")]))]                                               

        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, false), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetCollectionSetPropertySimpleOnly(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ASet"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let argS = "x-ro-domain-model=simple"
        let url = sprintf "%s?%s" purl argS

        let args = CreateReservedArgs argS
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" url)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)

        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
        
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection "ASet"
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection "ASet"

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"));
                                                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Set"));
                                                                              TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                                                                              TProperty(JsonPropertyNames.ElementType, TObjectVal(roType))]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object (oType)); 
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)]))]                                                   
                                                                   
        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetCollectionPropertyWithMediaType(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ACollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let msg = jsonGetMsg(sprintf "http://localhost/%s" purl)
        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.ObjectCollection)))

        let args = CreateReservedArgs ""
        api.Request <- msg
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let addToRel = RelValues.AddTo + makeParm RelParamValues.Collection "ACollection"
        let removeFromRel = RelValues.RemoveFrom + makeParm RelParamValues.Collection "ACollection"

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List));
                                                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"));
                                                                              TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                                                                              TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"));
                                                                              TProperty(JsonPropertyNames.ElementType, TObjectVal(roType))]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                         TProperty(JsonPropertyNames.Links, TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object oType); 
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true); 
                                                                     TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s/collections/%s" oType pid)  RepresentationTypes.CollectionDescription "")]))]                                                   
                                                                    
        
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetDisabledCollectionProperty(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let msg = "Field not editable"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ADisabledCollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2"))  RepresentationTypes.Object roType

        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List));
                                                                              TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Disabled Collection"));
                                                                              TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                                              TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                                                                              TProperty(JsonPropertyNames.ElementType, TObjectVal(roType))]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.DisabledReason, TObjectVal(msg));
                         TProperty(JsonPropertyNames.Links, TArray( [ TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object (oType));
                                                                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true); 
                                                                      TObjectJson(makeGetLinkProp RelValues.DescribedBy (sprintf "domain-types/%s/collections/%s" oType pid)  RepresentationTypes.CollectionDescription "");
                                                      ]))]

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult

let GetCollectionValue(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let oid2 = ktc "2"
        let pid = "ACollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let vurl = sprintf "%s/collections/%s/value" ourl pid
        let rturl =  sprintf "domain-types/%s"  (ttc "list")
        

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollectionValue(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
        
        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
        let eturl =  sprintf "domain-types/%s"  roType

        let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid

        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid)  RepresentationTypes.Object roType
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2)  RepresentationTypes.Object roType

       
        let expected = [ TProperty(JsonPropertyNames.Id, TObjectVal(pid));
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]) );
                         TProperty(JsonPropertyNames.Value,TArray( [ TObjectJson(obj1); TObjectJson(obj2) ] ));
                         TProperty(JsonPropertyNames.Links, TArray([
                                                                     TObjectJson(makeGetLinkProp RelValues.Up ourl  RepresentationTypes.Object (oType));
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self vurl RepresentationTypes.CollectionValue "" "" true); 
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.ReturnType rturl RepresentationTypes.DomainType "" "" true); 
                                                                     TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.ElementType eturl RepresentationTypes.DomainType "" "" true); ]))];                                                     
                                                                    
        
        let ps = parsedResult.ToString()

        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
        Assert.AreEqual(new typeType(RepresentationTypes.CollectionValue, "", "", true), result.Content.Headers.ContentType)
        assertTransactionalCache  result 
        Assert.IsTrue(result.Headers.ETag.Tag.Length > 0) 
        compareObject expected parsedResult


let AddToAndDeleteFromCollectionProperty (api : RestfulObjectsControllerBase) = 
        let desc = "an empty collection for testing"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "AnEmptyCollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let mst = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid )).ToString())) 

        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let arg = CreateSingleValueArg parms
        
        try
            api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
            let result = api.PostCollection(oType, oid, pid, arg)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)
             
        let arg = CreateSingleValueArg parms

        try
            api.Request <-  jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
            let result = api.DeleteCollection(oType, oid, pid, arg)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)
       
       

let AddToAndDeleteFromCollectionPropertyViewModel (api : RestfulObjectsControllerBase) = 
        let desc = "an empty collection for testing"
        let oType = ttc "RestfulObjects.Test.Data.WithCollectionViewModel"
        let oid = ktc "1-2"
        let oid1 = ktc "1"
        let pid = "AnEmptyCollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid
        let mst = ttc "RestfulObjects.Test.Data.MostSimple"

        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid1 )).ToString())) 

        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())

        let arg = CreateSingleValueArg parms

        try
            api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
            let result = api.PostCollection(oType, oid, pid, arg)
            Assert.Fail("expect exception")
         with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)
        
  
        let arg = CreateSingleValueArg parms

        try 
            api.Request <-  jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
            let result = api.DeleteCollection(oType, oid, pid, arg)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)
    

//let AddToAndDeleteFromSetCollectionProperty (api : RestfulObjectsControllerBase) = 
//        let desc = "an empty set for testing"
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptySet"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//    
//        
//        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual(null, result.Content.Headers.ContentType)
//        assertTransactionalCache  result 
//   
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <-  jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual(null, result.Content.Headers.ContentType)
//        assertTransactionalCache  result 


//let AddToAndDeleteFromCollectionPropertyConcurrencySuccess (api : RestfulObjectsControllerBase) = 
//        let desc = "an empty collection for testing"
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let url = sprintf "http://localhost/objects/%s/%s"  oType oid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//      //  Assert.AreEqual(0, (instances<WithCollection>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AnEmptyCollection |> Seq.length )
//        
//        RestfulObjectsControllerBase.ConcurrencyChecking <- true
//
//        let args = CreateReservedArgs ""
//        api.Request <- jsonGetMsg(url)
//        let result = api.GetObject(oType, oid, args)
//        let tag = result.Headers.ETag.Tag 
//
//        api.Request <- jsonPostMsgAndTag (sprintf "http://localhost/%s" purl) (parms.ToString()) tag
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual(null, result.Content.Headers.ContentType)
//        assertTransactionalCache  result 
//  
//        let args = CreateReservedArgs ""
//        api.Request <-  jsonGetMsg(url)
//        let result = api.GetObject(oType, oid, args)
//        let tag = result.Headers.ETag.Tag 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsgAndTag (sprintf "http://localhost/%s?%s" purl parmsEncoded) tag
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual(null, result.Content.Headers.ContentType)
//        assertTransactionalCache  result 

//let AddToAndDeleteFromSetCollectionPropertyConcurrencySuccess (api : RestfulObjectsControllerBase) = 
//        let desc = "an empty set for testing"
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptySet"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let url = sprintf "http://localhost/objects/%s/%s"  oType oid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//      //  Assert.AreEqual(0, (instances<WithCollection>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AnEmptyCollection |> Seq.length )
//        
//        RestfulObjectsControllerBase.ConcurrencyChecking <- true
//
//        let args = CreateReservedArgs ""
//        api.Request <- jsonGetMsg(url)
//        let result = api.GetObject(oType, oid, args)
//        let tag = result.Headers.ETag.Tag 
//
//        api.Request <- jsonPostMsgAndTag (sprintf "http://localhost/%s" purl) (parms.ToString()) tag
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual(null, result.Content.Headers.ContentType)
//        assertTransactionalCache  result 
//
//        let args = CreateReservedArgs ""
//        api.Request <-  jsonGetMsg(url)
//        let result = api.GetObject(oType, oid, args)
//        let tag = result.Headers.ETag.Tag 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsgAndTag (sprintf "http://localhost/%s?%s" purl parmsEncoded) tag
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual(null, result.Content.Headers.ContentType)
//        assertTransactionalCache  result 

//let AddToCollectionPropertyValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple";
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true)) 
//
//        let arg = CreateSingleValueArg parms
//  
//        
//        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual(null, result.Content.Headers.ContentType)
//        assertTransactionalCache  result 

//let DeleteFromCollectionPropertyValidateOnly(api : RestfulObjectsControllerBase)  = 
//        let desc = "an empty collection for testing"
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple";
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        let arg = CreateSingleValueArg parms
//
//    
//        
//        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        let jsonResult = readSnapshotToJson result
//       
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//    
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//       
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

    
let GetInvalidCollection(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = " " // invalid 
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid


        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedObjects.Surface.BadRequestNOSException' was thrown.\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let GetNotFoundCollection(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ANonExistentCollection" // doesn't exist 
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such collection ANonExistentCollection\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

    
let GetHiddenValueCollection(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "AHiddenCollection" 
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetCollection(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such collection AHiddenCollection\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)
 
let NotAcceptableGetCollectionWrongMediaType(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ACollection" 
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        try 
            let msg = jsonGetMsg(sprintf "http://localhost/%s" purl)
            msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.ObjectProperty)))
            let args = CreateReservedArgs ""
            api.Request <- msg
            let result = api.GetCollection(oType, oid, pid, args)
            Assert.Fail("expect exception")
        with 
            | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

let GetErrorValueCollection(api : RestfulObjectsControllerBase) = 
        let oType = ttc "RestfulObjects.Test.Data.WithGetError"
        let oid = ktc "1"
        let pid = "AnErrorCollection" 
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        RestfulObjects.Test.Data.WithGetError.ThrowErrors <- true
        let result = api.GetCollection(oType, oid, pid, args)
        RestfulObjects.Test.Data.WithGetError.ThrowErrors <- false
        let jsonResult = readSnapshotToJson result
        let parsedResult = JObject.Parse(jsonResult)
      
        let expected = [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"));
                         TProperty(JsonPropertyNames.StackTrace, TArray([ TObjectVal( new errorType("   at RestfulObjects.Test.Data.WithGetError.AnError() in C:\Naked Objects Internal\REST\RestfulObjects.Test.Data\WithError.cs:line 12"))]));
                         TProperty(JsonPropertyNames.Links, TArray([]))
                         TProperty(JsonPropertyNames.Extensions, TObjectJson([]))]

        Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode)
        // for some resaon stack trace has different depth on my machine when not debugging (only) ! 
        Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
        //compareObject expected  parsedResult

let GetCollectionAsProperty(api : RestfulObjectsControllerBase) = 
        let collectionType = "System.Collections.Generic.List`1[[RestfulObjects.Test.Data.MostSimple"
        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
        let oid = ktc "1"
        let pid = "ACollection"
        let ourl = sprintf "objects/%s/%s"  oType oid
        let purl = sprintf "%s/collections/%s" ourl pid

        let args = CreateReservedArgs ""
        api.Request <- jsonGetMsg(sprintf "http://localhost/%s" purl)
        let result = api.GetProperty(oType, oid, pid, args)
        let jsonResult = readSnapshotToJson result
        
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode)
        Assert.AreEqual("199 RestfulObjects \"No such property ACollection\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

//let AddToCollectionMissingArgs(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//
//        let arg = CreateSingleValueArg (new JObject())
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) ""
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//       
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionMalformedArgs(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty("malformed", refParm)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
  
//let AddToCollectionInvalidArgs(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.WithValue"
//        let mst = ttc "RestfulObjects.Test.Data.MostSimple" 
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionDisabledValue(api : RestfulObjectsControllerBase) =
//        let msg = "Always Disabled" 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ADisabledCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"" +  msg + "\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionInvisibleValue(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AHiddenCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionImmutableObject(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.Immutable"
//        let oid = ktc "1"
//        let pid = "ACollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href,  (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//       
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionInvalidArgsName(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ANonExistentCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//       
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let NotAcceptableAddCollectionWrongMediaType(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ACollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        let arg = CreateSingleValueArg parms
//
//       
//          
//        let msg = jsonPostMsg(sprintf "http://localhost/%s" purl) (parms.ToString())
//        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.ObjectProperty)))
//        api.Request <- msg
//        let result = api.PostCollection(oType, oid, pid, arg)
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//     
    
     
//let AddToCollectionMissingArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//
//        let parms =  new JObject (new JProperty("x-ro-validate-only", true)) 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//       
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
  
//let AddToCollectionMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty("malformed", refParm), new JProperty("x-ro-validate-only", true)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
 
//let AddToCollectionInvalidArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.WithValue"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//        let msg = "Always Disabled"
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ADisabledCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"" +  msg + "\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AHiddenCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.Immutable"
//        let oid = ktc "1"
//        let pid = "ACollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true)) 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <-  jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//       
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ANonExistentCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true)) 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//let AddToCollectionInternalError(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithGetError"
//        let oid = ktc "1"
//        let pid = "AnErrorCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//let AddToCollectionForPreTest(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        ()


//    
//let DeleteFromCollectionMissingArgs(api : RestfulObjectsControllerBase) = 
//        AddToCollectionForPreTest api
//
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//
//        let arg = CreateSingleValueArg (new JObject())
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s" purl ) 
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//       
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//    
//let DeleteFromCollectionMalformedArgs(api : RestfulObjectsControllerBase) = 
//        AddToCollectionForPreTest api
//
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty("malformed", refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

//    
//let DeleteFromCollectionInvalidArgs(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.WithValue"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

    // 401 

    // Field not editable
//let DeleteFromCollectionDisabledValue(api : RestfulObjectsControllerBase) =
//        let msg = "Always Disabled" 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ADisabledCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"" + msg + "\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

    // 14.2 405 

//
//    
//let DeleteFromCollectionInvisibleValue(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AHiddenCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//let DeleteFromCollectionImmutableObject(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.Immutable"
//        let oid = ktc "1"
//        let pid = "ACollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

    // 14.2 404 
//
//    
//let DeleteFromCollectionInvalidArgsName(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ANonExistentCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//    // 14.2 40 
//let NotAcceptableDeleteFromCollectionWrongMediaType(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ACollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//        let msg = jsonDeleteMsg(sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.ObjectProperty)))
//        api.Request <- msg
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//       
     
     
 // new 
//
//let DeleteFromCollectionMissingArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//        AddToCollectionForPreTest api
//
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//
//        let parms =  new JObject (new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//    
//let DeleteFromCollectionMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//        AddToCollectionForPreTest api
//
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty("malformed", refParm),  new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//    
//let DeleteFromCollectionInvalidArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.WithValue"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm),  new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

    // 401 

    // Field not editable
//let DeleteFromCollectionDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//        let msg = "Always Disabled"
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ADisabledCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm),  new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"" + msg + "\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

    // 14.2 405 

//  
//let DeleteFromCollectionInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AHiddenCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm),  new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//  
//let DeleteFromCollectionImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.Immutable"
//        let oid = ktc "1"
//        let pid = "ACollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm),  new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)

    // 14.2 404 

//    
//let DeleteFromCollectionInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "ANonExistentCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm),  new JProperty("x-ro-validate-only", true)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded )
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//let DeleteFromCollectionInternalError(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithGetError"
//        let oid = ktc "1"
//        let pid = "AnErrorCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//
//        let arg = CreateSingleValueArg parms
//
//        api.Request <-  jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        let result = api.DeleteCollection(oType, oid, pid, arg)
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//let AddToCollectionPropertyConcurrencyFail(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        RestfulObjectsControllerBase.ConcurrencyChecking <- true
//   
//        let tag = "\"fail\""
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsgAndTag (sprintf "http://localhost/%s" purl) (parms.ToString()) tag
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//let AddToCollectionPropertyMissingIfMatchHeader(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        RestfulObjectsControllerBase.ConcurrencyChecking <- true
//
//        let arg = CreateSingleValueArg parms
//  
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        let jsonResult = readSnapshotToJson result
//       
//        Assert.AreEqual(preconditionHeaderMissing, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//let DeleteFromCollectionPropertyConcurrencyFail(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//
//        let arg = CreateSingleValueArg parms
//        
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)
//
//let DeleteFromCollectionPropertyMissingIfMatchHeader(api : RestfulObjectsControllerBase) = 
//        let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//        let oid = ktc "1"
//        let pid = "AnEmptyCollection"
//        let ourl = sprintf "objects/%s/%s"  oType oid
//        let purl = sprintf "%s/collections/%s" ourl pid
//        let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//
//        let refParm = new JObject( new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" roType oid)).ToString())) 
//
//        let parms =  new JObject (new JProperty(JsonPropertyNames.Value, refParm)) 
//        
//        let arg = CreateSingleValueArg parms
//
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        let result = api.PostCollection(oType, oid, pid, arg)
//        
//        let jsonResult = readSnapshotToJson result
//        
//        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode)
//        Assert.AreEqual("199 RestfulObjects \"Always Disabled\"", result.Headers.Warning.ToString())
//        Assert.AreEqual("", jsonResult)