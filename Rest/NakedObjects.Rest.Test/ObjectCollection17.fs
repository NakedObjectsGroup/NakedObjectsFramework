//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.
module ObjectCollection17

//open NUnit.Framework
//open NakedObjects.Rest
//open System.Net
//open System.Net.Http.Headers
//open Newtonsoft.Json.Linq
//open System.Web
//open NakedObjects.Rest.Snapshot.Constants
//open System.Web.Http
//open System.Linq
//open RestTestFunctions
//open NakedObjects.Rest.Snapshot.Utility

//let GetCollectionProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let oid2 = ktc "2"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          
//          membersProp((sprintf "%s/%s" oType oid), roType)

//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                      ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetCollectionPropertyWithInlineFlag(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let oid2 = ktc "2"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let argS = RestControlFlags.InlineCollectionItemsReserved + "=true"
//    let url = sprintf "%s?%s" purl argS
//    let args = CreateReservedArgs argS
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let roid1 = roType + "/" + ktc "1"
//    let roid2 = roType + "/" + ktc "2"
    
//    let m1 = TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeTablePropertyMember "Id" (TObjectVal(1)))) ]))
//    let m2 = TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("Id", TObjectJson(makeTablePropertyMember "Id" (TObjectVal(2)))) ]))

//    let obj1 =
//        m1 :: 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) ::
//        makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        m2 :: 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) ::
//        makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          membersProp((sprintf "%s/%s" oType oid), roType)
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                      ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult


//let GetCollectionPropertyViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollectionViewModel"
//    let oid = ktc "1--2"
//    let oid1 = ktc "1"
//    let oid2 = ktc "2"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid1) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                       ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult



//let GetCollectionPropertySimpleOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let argS = "x-ro-domain-model=simple"
//    let url = sprintf "%s?%s" purl argS
//    let args = CreateReservedArgs argS
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" url)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          membersProp((sprintf "%s/%s" oType oid), roType)
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetCollectionSetProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ASet"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Set"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
         
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                      ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult



//let GetCollectionSetPropertySimpleOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ASet"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let argS = "x-ro-domain-model=simple"
//    let url = sprintf "%s?%s" purl argS
//    let args = CreateReservedArgs argS
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" url)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal("set"))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Set"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetCollectionPropertyWithMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let msg = jsonGetMsg (sprintf "http://localhost/%s" purl)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectCollection)))
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8"))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          membersProp((sprintf "%s/%s" oType oid), roType)
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                      ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetDisabledCollectionProperty(api : RestfulObjectsControllerBase) = 
//    let msg = "Field not editable"
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ADisabledCollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//        :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List))
//                                  TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Disabled Collection"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(roType)) ]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal(msg))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                      TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self purl RepresentationTypes.ObjectCollection "" roType true)
                      
//                       ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectCollection, "", roType, true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetCollectionValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let oid2 = ktc "2"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let vurl = sprintf "%s/collections/%s/value" ourl pid
   
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollectionValue(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    
//    let valueRel = RelValues.Value + makeParm RelParamValues.Collection pid
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" roType oid2) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([]))
//          TProperty(JsonPropertyNames.Value, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (oType))
//                             TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self vurl RepresentationTypes.CollectionValue "" "" true)
//                             ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.CollectionValue, "", "", true), headers.ContentType)
//    assertTransactionalCache headers
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let AddToAndDeleteFromCollectionProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "AnEmptyCollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid)).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let arg = CreateSingleValueArg parms
//    try 
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        api.PostCollection(oType, oid, pid, arg) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)
//    let arg = CreateSingleValueArg parms
//    try 
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        api.DeleteCollection(oType, oid, pid, arg) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)

//let AddToAndDeleteFromCollectionPropertyViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollectionViewModel"
//    let oid = ktc "1-2"
//    let oid1 = ktc "1"
//    let pid = "AnEmptyCollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid1)).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let arg = CreateSingleValueArg parms
//    try 
//        api.Request <- jsonPostMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//        api.PostCollection(oType, oid, pid, arg) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)
//    let arg = CreateSingleValueArg parms
//    try 
//        api.Request <- jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//        api.DeleteCollection(oType, oid, pid, arg) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.Forbidden, ex.Response.StatusCode)

//let GetInvalidCollection(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = " " // invalid 
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
//    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedObjects.Facade.BadRequestNOSException' was thrown.\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let GetNotFoundCollection(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ANonExistentCollection" // doesn't exist 
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
//    Assert.AreEqual("199 RestfulObjects \"No such collection ANonExistentCollection\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let GetHiddenValueCollection(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "AHiddenCollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
//    Assert.AreEqual("199 RestfulObjects \"No such collection AHiddenCollection\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let GetNakedObjectsIgnoredCollection(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ANakedObjectsIgnoredCollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
//    Assert.AreEqual("199 RestfulObjects \"No such collection ANakedObjectsIgnoredCollection\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let NotAcceptableGetCollectionWrongMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    try 
//        let msg = jsonGetMsg (sprintf "http://localhost/%s" purl)
//        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectProperty)))
//        let args = CreateReservedArgs ""
//        api.Request <- msg
//        api.GetCollection(oType, oid, pid, args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

//let GetErrorValueCollection(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithGetError"
//    let oid = ktc "1"
//    let pid = "AnErrorCollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- true
//    let result = api.GetCollection(oType, oid, pid, args)
//    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- false
//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode)
//    // for some resaon stack trace has different depth on my machine when not debugging (only) ! 
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())

//let GetCollectionAsProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
//    let oid = ktc "1"
//    let pid = "ACollection"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/collections/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
//    Assert.AreEqual("199 RestfulObjects \"No such property ACollection\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)
