// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module DomainType21

open NUnit.Framework
open RestfulObjects.Mvc
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open RestfulObjects.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions

let invokeRelTypeSb = RelValues.Invoke + makeParm RelParamValues.TypeAction "isSubtypeOf"
let invokeRelTypeSp = RelValues.Invoke + makeParm RelParamValues.TypeAction "isSupertypeOf"
let invokeRelTypeFb = RelValues.Invoke + makeParm RelParamValues.TypeAction "filterSubtypesFrom"
let invokeRelTypeFp = RelValues.Invoke + makeParm RelParamValues.TypeAction "filterSupertypesFrom"

let GetMostSimpleObjectType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetDomainType(oType, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Name, TObjectVal("RestfulObjects.Test.Data.MostSimple"))
          TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Most Simple"))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.IsService, TObjectVal(false))
          
          TProperty
              (JsonPropertyNames.Links, 
               TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               
               TArray
                   ([ TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "Id") RepresentationTypes.PropertyDescription "") ]))
          
          TProperty
              (JsonPropertyNames.TypeActions, 
               
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")

                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSubtypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFb (sprintf "domain-types/%s/type-actions/filterSubtypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSupertypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFp (sprintf "domain-types/%s/type-actions/filterSupertypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                              
                              
                                     
                                     
                                      ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.DomainType), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetWithActionObjectType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetDomainType(oType, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Name, TObjectVal("RestfulObjects.Test.Data.WithActionObject"))
          TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action Object"))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("With Action Objects"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.IsService, TObjectVal(false))
          
          TProperty
              (JsonPropertyNames.Links, 
               TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               
               TArray
                   ([ TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "Id") RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsVoidWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "ADisabledAction") RepresentationTypes.ActionDescription 
                               "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "ADisabledCollectionAction") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "ADisabledQueryAction") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AHiddenAction") RepresentationTypes.ActionDescription 
                               "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AUserDisabledAction") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnAction") RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedIdempotent") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedIdempotentReturnsNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedIdempotentReturnsViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedQueryOnlyReturnsNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedQueryOnlyReturnsViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollection") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionEmpty") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionWithScalarParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsNullViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParameterAnnotatedQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action 
                               (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParametersAnnotatedIdempotent") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParametersAnnotatedQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsQueryable") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsQueryableWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsQueryableWithScalarParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsRedirectedObject") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalar") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalarEmpty") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalarNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalarWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsVoid") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsWithDateTimeKeyQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionValidateParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithCollectionParameter") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithCollectionParameterRef") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithDateTimeParm") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithOptionalParm") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithOptionalParmQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithParametersWithChoicesWithDefaults") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameter") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameterWithChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameterWithConditionalChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameterWithDefault") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParametersWithAutoComplete") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParameter") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParameterWithChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParameterWithDefault") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParametersWithConditionalChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnError") RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnErrorCollection") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnErrorQuery") RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnOverloadedAction0") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnOverloadedAction1") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AzContributedAction") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AzContributedActionOnBaseClass") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AzContributedActionWithRefParm") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AzContributedActionWithValueParm") 
                               RepresentationTypes.ActionDescription "") ]))
          
          TProperty
              (JsonPropertyNames.TypeActions, 
               
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "") 
        
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSubtypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFb (sprintf "domain-types/%s/type-actions/filterSubtypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSupertypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFp (sprintf "domain-types/%s/type-actions/filterSupertypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                                     
                                     
                                     
                                     
                                     ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.DomainType), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetWithActionServiceType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionService"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetDomainType(oType, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Name, TObjectVal("RestfulObjects.Test.Data.WithActionService"))
          TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Action Service"))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("With Action Services"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.IsService, TObjectVal(true))
          
          TProperty
              (JsonPropertyNames.Links, 
               TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               
               TArray
                   ([ TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsVoidWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "ADisabledAction") RepresentationTypes.ActionDescription 
                               "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "ADisabledCollectionAction") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "ADisabledQueryAction") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AHiddenAction") RepresentationTypes.ActionDescription 
                               "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AUserDisabledAction") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnAction") RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedIdempotent") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedIdempotentReturnsNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedIdempotentReturnsViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedQueryOnlyReturnsNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionAnnotatedQueryOnlyReturnsViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollection") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionEmpty") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsCollectionWithScalarParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsNullViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParameterAnnotatedQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action 
                               (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParametersAnnotatedIdempotent") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsObjectWithParametersAnnotatedQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsQueryable") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsQueryableWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsQueryableWithScalarParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsRedirectedObject") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalar") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalarEmpty") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalarNull") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsScalarWithParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsViewModel") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsVoid") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionReturnsWithDateTimeKeyQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionValidateParameters") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithCollectionParameter") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithCollectionParameterRef") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithDateTimeParm") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithOptionalParm") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithOptionalParmQueryOnly") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithParametersWithChoicesWithDefaults") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameter") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameterWithChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameterWithConditionalChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParameterWithDefault") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithReferenceParametersWithAutoComplete") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParameter") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParameterWithChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParameterWithDefault") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnActionWithValueParametersWithConditionalChoices") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnError") RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnErrorCollection") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnErrorQuery") RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnOverloadedAction0") 
                               RepresentationTypes.ActionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Action (sprintf "domain-types/%s/actions/%s" oType "AnOverloadedAction1") 
                               RepresentationTypes.ActionDescription "") ]))
          
          TProperty
              (JsonPropertyNames.TypeActions, 
               
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                           
          
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSubtypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFb (sprintf "domain-types/%s/type-actions/filterSubtypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSupertypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFp (sprintf "domain-types/%s/type-actions/filterSupertypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                                     
                                     
                                     
                                      ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.DomainType), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetWithReferenceObjectType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetDomainType(oType, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Name, TObjectVal("RestfulObjects.Test.Data.WithReference"))
          TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Reference"))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("With References"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.IsService, TObjectVal(false))
          
          TProperty
              (JsonPropertyNames.Links, 
               TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               
               TArray
                   ([ TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AChoicesReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AConditionalChoicesReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "ADisabledReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AHiddenReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "ANullReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AnAutoCompleteReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AnEagerReference") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "Id") RepresentationTypes.PropertyDescription "") ]))
          
          TProperty
              (JsonPropertyNames.TypeActions, 
               
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                              
       
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSubtypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFb (sprintf "domain-types/%s/type-actions/filterSubtypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSupertypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFp (sprintf "domain-types/%s/type-actions/filterSupertypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                                     
                                     
                                      ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.DomainType), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetWithValueObjectType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetDomainType(oType, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Name, TObjectVal("RestfulObjects.Test.Data.WithValue"))
          TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Value"))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("With Values"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.IsService, TObjectVal(false))
          
          TProperty
              (JsonPropertyNames.Links, 
               TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               
               TArray
                   ([ TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AStringValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "ADateTimeValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AChoicesValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AConditionalChoicesValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "ADisabledValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AHiddenValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AUserDisabledValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AUserHiddenValue") 
                               RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "AValue") RepresentationTypes.PropertyDescription 
                               "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "Id") RepresentationTypes.PropertyDescription "") ]))
          
          TProperty
              (JsonPropertyNames.TypeActions, 
               
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "") 
                                     
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSubtypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFb (sprintf "domain-types/%s/type-actions/filterSubtypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSupertypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFp (sprintf "domain-types/%s/type-actions/filterSupertypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                                     
                                     
                                     ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.DomainType), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetWithCollectionObjectType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithCollection"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetDomainType(oType, args)
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Name, TObjectVal("RestfulObjects.Test.Data.WithCollection"))
          TProperty(JsonPropertyNames.DomainType, TObjectVal(oType))
          TProperty(JsonPropertyNames.FriendlyName, TObjectVal("With Collection"))
          TProperty(JsonPropertyNames.PluralName, TObjectVal("With Collections"))
          TProperty(JsonPropertyNames.Description, TObjectVal(""))
          TProperty(JsonPropertyNames.IsService, TObjectVal(false))
          
          TProperty
              (JsonPropertyNames.Links, 
               TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]))
          
          TProperty
              (JsonPropertyNames.Members, 
               
               TArray
                   ([ TObjectJson
                          (makeGetLinkProp RelValues.Property (sprintf "domain-types/%s/properties/%s" oType "Id") RepresentationTypes.PropertyDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "AnEmptyCollection") 
                               RepresentationTypes.CollectionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "AnEmptySet") 
                               RepresentationTypes.CollectionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "ACollection") 
                               RepresentationTypes.CollectionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "ACollectionViewModels") 
                               RepresentationTypes.CollectionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "ADisabledCollection") 
                               RepresentationTypes.CollectionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "AHiddenCollection") 
                               RepresentationTypes.CollectionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "ASet") 
                               RepresentationTypes.CollectionDescription "")
                      
                      TObjectJson
                          (makeGetLinkProp RelValues.Collection (sprintf "domain-types/%s/collections/%s" oType "AnEagerCollection") 
                               RepresentationTypes.CollectionDescription "") ]))
          
          TProperty
              (JsonPropertyNames.TypeActions, 
               
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                               
      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSubtypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SubTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFb (sprintf "domain-types/%s/type-actions/filterSubtypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")
                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Id, TObjectVal("filterSupertypesFrom")) 
                           :: TProperty
                                  (JsonPropertyNames.Arguments, 
                                   TObjectJson([ TProperty(JsonPropertyNames.SuperTypes, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(null)) ])) ])) 
                              :: makeGetLinkProp invokeRelTypeFp (sprintf "domain-types/%s/type-actions/filterSupertypesFrom/invoke" oType) 
                                     RepresentationTypes.TypeActionResult "")                                    
                                     
                                     
                                     
                                     
                                      ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.DomainType), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let GetPredefinedDomainTypes(api : RestfulObjectsControllerBase) = 
    let allPrefined = 
        [| "number"; "string"; "boolean"; "integer"; "date-time"; "date"; "time"; "utc-millisec"; "big-integer(1)"; "big-integer(167)"; "big-decimal(1,1)"; 
           "big-decimal(101,123)"; "blob"; "clob"; "list"; "set"; "void" |]
    for predefined in allPrefined do
        let url = sprintf "http://localhost/domain-types/%s" predefined
        let msg = jsonGetMsg (url)
        let args = CreateReservedArgs ""
        api.Request <- msg
        let result = api.GetDomainType(predefined, args)
        let jsonResult = readSnapshotToJson result
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, "for " + predefined)
        Assert.AreEqual("", jsonResult)

let NotFoundPredefinedDomainTypes(api : RestfulObjectsControllerBase) = 
    let allPrefined = 
        [| "big-decimal"; "big-integer"; "bigdecimal"; "biginteger"; "big-integer1"; "big-decimal1"; "big-integer()"; "big-integer(1,1)"; "big-decimal(,)"; 
           "big-integer(a)"; "big-decimal(a,1)"; "big-decimal(1)"; "big-decimal(1,a)"; "long" |]
    for predefined in allPrefined do
        let url = sprintf "http://localhost/domain-types/%s" predefined
        let msg = jsonGetMsg (url)
        let args = CreateReservedArgs ""
        api.Request <- msg
        let result = api.GetDomainType(predefined, args)
        let jsonResult = readSnapshotToJson result
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, "for " + predefined)
        Assert.AreEqual("199 RestfulObjects \"No such domain type " + predefined + "\"", result.Headers.Warning.ToString())
        Assert.AreEqual("", jsonResult)

let NotFoundGetMostSimpleObjectType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let args = CreateReservedArgs ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetDomainType(oType, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotAcceptableGetMostSimpleObjectType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let url = sprintf "http://localhost/domain-types/%s" oType
    let msg = jsonGetMsg (url)
    let args = CreateReservedArgs ""
    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.HomePage)))
    try 
        api.Request <- msg
        api.GetDomainType(oType, args) |> ignore
        Assert.Fail("expect exception")
    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)