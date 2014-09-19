// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module DomainTypes20
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

let getExpected() = 
    let value = TArray([TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.IViewModel") RepresentationTypes.DomainType "");   
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.Redirect.IRedirectedObject") RepresentationTypes.DomainType "");             
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.Services.AbstractFactoryAndRepository") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.Value.IStreamResource") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.ContributorService") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.Immutable") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.MostSimple") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.MostSimpleViewModel") RepresentationTypes.DomainType "")
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.RedirectedObject") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.RestDataRepository") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.VerySimple") RepresentationTypes.DomainType "");                    
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.VerySimpleEager") RepresentationTypes.DomainType "");    
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithAction") RepresentationTypes.DomainType "");                      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithActionObject") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithActionService") RepresentationTypes.DomainType "");  
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithAttachments") RepresentationTypes.DomainType "");      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithCollection") RepresentationTypes.DomainType "");                                                       
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithDateTimeKey") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithError") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithGetError") RepresentationTypes.DomainType "");                      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithReference") RepresentationTypes.DomainType "");                                       
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithScalars") RepresentationTypes.DomainType "");                                                                                                                                                                                    
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithValue") RepresentationTypes.DomainType "");                                                                                                                           
                        ])

    let expected = [ TProperty(JsonPropertyNames.Links, TArray( [TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.DomainTypes RepresentationTypes.TypeList "");  
                                                                 TObjectJson(makeGetLinkProp RelValues.Up   SegmentValues.HomePage    RepresentationTypes.HomePage "") ]));
                      TProperty(JsonPropertyNames.Value, value);
                      TProperty(JsonPropertyNames.Extensions, TObjectJson([]))  ]

    expected

let getExpectedMT() = 
    let value = TArray([TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.IViewModel") RepresentationTypes.DomainType "");   
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.IKeyCodeMapper") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.Redirect.IRedirectedObject") RepresentationTypes.DomainType "");             
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.Services.AbstractFactoryAndRepository") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "NakedObjects.Value.IStreamResource") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.ContributorService") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.Immutable") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.MostSimple") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.MostSimpleViewModel") RepresentationTypes.DomainType "")
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.RedirectedObject") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.RestDataRepository") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.VerySimple") RepresentationTypes.DomainType "");                    
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.VerySimpleEager") RepresentationTypes.DomainType "");    
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithAction") RepresentationTypes.DomainType "");                      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithActionObject") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithActionService") RepresentationTypes.DomainType "");  
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithAttachments") RepresentationTypes.DomainType "");      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithCollection") RepresentationTypes.DomainType "");                                                       
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithDateTimeKey") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithError") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithGetError") RepresentationTypes.DomainType "");                      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithReference") RepresentationTypes.DomainType "");                                       
                        // TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithScalars") RepresentationTypes.DomainType "");                                                                                                                                                                                    
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestfulObjects.Test.Data.WithValue") RepresentationTypes.DomainType "");                                                                                                                           
                        ])

    let expected = [ TProperty(JsonPropertyNames.Links, TArray( [TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.DomainTypes RepresentationTypes.TypeList "");  
                                                                 TObjectJson(makeGetLinkProp RelValues.Up   SegmentValues.HomePage    RepresentationTypes.HomePage "") ]));
                      TProperty(JsonPropertyNames.Value, value);
                      TProperty(JsonPropertyNames.Extensions, TObjectJson([]))  ]

    expected



let getExpectedDomainType() = 
    let value = TArray([TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "AbstractFactoryAndRepository") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "ContributorService") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IKeyCodeMapper") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "Immutable") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IRedirectedObject") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IStreamResource") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "ITypeCodeMapper") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IViewModel") RepresentationTypes.DomainType "");  
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "MostSimple") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "MostSimpleViewModel") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RedirectedObject") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestDataRepository") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "TestKeyCodeMapper") RepresentationTypes.DomainType "");                                       
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "TestTypeCodeMapper") RepresentationTypes.DomainType "");                        
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "VerySimple") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "VerySimpleEager") RepresentationTypes.DomainType "");                  
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithAction") RepresentationTypes.DomainType "");                            
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithActionObject") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithActionService") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithAttachments") RepresentationTypes.DomainType "");  
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithCollection") RepresentationTypes.DomainType "");                                                       
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithDateTimeKey") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithError") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithGetError") RepresentationTypes.DomainType "");                      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithReference") RepresentationTypes.DomainType "");                                       
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithScalars") RepresentationTypes.DomainType "");                                                                                                                                                                                    
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithValue") RepresentationTypes.DomainType "");                                        
                        ])

    let expected = [ TProperty(JsonPropertyNames.Links, TArray( [TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.DomainTypes RepresentationTypes.TypeList "");  
                                                                 TObjectJson(makeGetLinkProp RelValues.Up   SegmentValues.HomePage    RepresentationTypes.HomePage "") ]));
                      TProperty(JsonPropertyNames.Value, value);
                      TProperty(JsonPropertyNames.Extensions, TObjectJson([]))  ]

    expected



let getExpectedDomainTypeMT() = 
    let value = TArray([TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "AbstractFactoryAndRepository") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "ContributorService") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IKeyCodeMapper") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "Immutable") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IRedirectedObject") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IStreamResource") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "ITypeCodeMapper") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "IViewModel") RepresentationTypes.DomainType "");  
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "MostSimple") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "MostSimpleViewModel") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RedirectedObject") RepresentationTypes.DomainType ""); 
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "RestDataRepository") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "TestKeyCodeMapper") RepresentationTypes.DomainType "");                                       
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "TestTypeCodeMapper") RepresentationTypes.DomainType "");                        
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "VerySimple") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "VerySimpleEager") RepresentationTypes.DomainType "");                  
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithAction") RepresentationTypes.DomainType "");                            
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithActionObject") RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithActionService") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithAttachments") RepresentationTypes.DomainType "");  
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithCollection") RepresentationTypes.DomainType "");                                                       
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithDateTimeKey") RepresentationTypes.DomainType ""); 
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithError") RepresentationTypes.DomainType "");
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithGetError") RepresentationTypes.DomainType "");                      
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithReference") RepresentationTypes.DomainType "");                                       
                        //TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithScalars") RepresentationTypes.DomainType "");                                                                                                                                                                                    
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" "WithValue") RepresentationTypes.DomainType "");                                        
                        ])


    let expected = [ TProperty(JsonPropertyNames.Links, TArray( [TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.DomainTypes RepresentationTypes.TypeList "");  
                                                                 TObjectJson(makeGetLinkProp RelValues.Up   SegmentValues.HomePage    RepresentationTypes.HomePage "") ]));
                      TProperty(JsonPropertyNames.Value, value);
                      TProperty(JsonPropertyNames.Extensions, TObjectJson([]))  ]

    expected

let GetDomainTypes (api : RestfulObjectsControllerBase)  = 
    let url = testRoot + SegmentValues.DomainTypes 
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg(url)
    let result = api.GetDomainTypes(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.TypeList), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    let expected = getExpected()
    compareObject expected parsedResult

let GetDomainTypesWithMediaType (api : RestfulObjectsControllerBase)  = 
    let url = testRoot + SegmentValues.DomainTypes 
    let msg = jsonGetMsg(url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.TypeList)))
    api.Request <- msg
    let result = api.GetDomainTypes(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.TypeList), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    let expected = getExpectedMT()
    compareObject expected parsedResult

let GetDomainTypesDomainType (api : RestfulObjectsControllerBase)  = 
    let url = testRoot + SegmentValues.DomainTypes 
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg(url)
    let result = api.GetDomainTypes(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.TypeList), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    let expected = getExpectedDomainType()
    compareObject expected parsedResult

let GetDomainTypesWithMediaTypeDomainType (api : RestfulObjectsControllerBase)  = 
    let url = testRoot + SegmentValues.DomainTypes 
    let msg = jsonGetMsg(url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", (makeProfile RepresentationTypes.TypeList)))
    api.Request <- msg
    let result = api.GetDomainTypes(args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)

    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode)
    Assert.AreEqual(new typeType(RepresentationTypes.TypeList), result.Content.Headers.ContentType)
    assertNonExpiringCache result 
    let expected = getExpectedDomainTypeMT()
    compareObject expected parsedResult

// 406   
let NotAcceptableGetDomainTypes(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.DomainTypes 
    let msg = jsonGetMsg(url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue ("profile", "\"urn:org.restfulobjects/homepage\""))

    try 
        api.Request <- msg
        let result = api.GetDomainTypes(args)
        Assert.Fail("expect exception")
    with 
        | :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)



