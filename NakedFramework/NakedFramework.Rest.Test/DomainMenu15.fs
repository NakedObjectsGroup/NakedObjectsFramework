// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.DomainMenu15

open NakedFramework.Rest.Snapshot.Utility
open NakedFramework.Rest.Snapshot.Constants
open NakedFramework.Rest.API
open Newtonsoft.Json.Linq
open NUnit.Framework
open NakedObjects.Rest.Test.Functions
open System.Net

let GetMenu(api : RestfulObjectsControllerBase) =
    let sName =  ttc "RestfulObjects.Test.Data.RestDataRepository" 
    let mName =  "RestDataRepository"
    let url = sprintf "http://localhost/menus/%s" mName
    
    jsonSetGetMsg api.Request url
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let mstp = ttc "RestfulObjects.Test.Data.MostSimplePersist"
    let wa = ttc "RestfulObjects.Test.Data.WithAction"
    let wao = ttc "RestfulObjects.Test.Data.WithActionObject"
    let str = ttc "string"
    
    let makeParm pmid fid rt =     
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeValueParm pmid fid rt =              
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let p1 = makeParm "withAction" "With Action" wao
    let p2 = makeParm "withAction" "With Action" wa
    let p3 = makeParm "withAction" "With Action" wao
    let p4 = makeParm "withOtherAction" "With Other Action" wao
    let p5 = makeParm "withAction" "With Action" wao
    let p6 = makeValueParm "parm" "Parm" str
    
    let expected = 
        [ TProperty(JsonPropertyNames.MenuId, TObjectVal(mName))
          TProperty(JsonPropertyNames.Title, TObjectVal("Rest Data Repository"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "menus/%s" mName) RepresentationTypes.Menu "")   ]))          
          TProperty
              (JsonPropertyNames.Members,                
               TObjectJson
                   ([ TProperty("AzContributedAction", TObjectJson(makeServiceActionMember "AzContributedAction" sName mst [ p1 ]))
                      TProperty("AzContributedActionOnBaseClass", TObjectJson(makeServiceActionMember "AzContributedActionOnBaseClass" sName mst [ p2 ]))
                      TProperty("AzContributedActionWithRefParm", TObjectJson(makeServiceActionMember "AzContributedActionWithRefParm" sName mst [ p3; p4 ]))                      
                      TProperty
                          ("AzContributedActionWithValueParm", TObjectJson(makeServiceActionMember "AzContributedActionWithValueParm" sName mst [ p5; p6 ]))
                      TProperty("CreateTransientMostSimple", TObjectJson(makeServiceActionMemberNoParms "CreateTransientMostSimple" sName mstp))                      
                      TProperty
                          ("CreateTransientWithValue", 
                           TObjectJson(makeServiceActionMemberNoParms "CreateTransientWithValue" sName (ttc "RestfulObjects.Test.Data.WithValuePersist")))                      
                      TProperty
                          ("CreateTransientWithReference", 
                           TObjectJson(makeServiceActionMemberNoParms "CreateTransientWithReference" sName (ttc "RestfulObjects.Test.Data.WithReferencePersist")))                      
                      TProperty
                          ("CreateTransientWithCollection", 
                           TObjectJson(makeServiceActionMemberNoParms "CreateTransientWithCollection" sName (ttc "RestfulObjects.Test.Data.WithCollectionPersist"))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([  ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Menu, ""), headers.ContentType)
    assertNonExpiringCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetContributorMenu(api : RestfulObjectsControllerBase) = 
    let sName =  ttc "RestfulObjects.Test.Data.ContributorService" 
    let mName =  "ContributorService"
    let url = sprintf "http://localhost/menus/%s" mName
    
    jsonSetGetMsg api.Request url
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    
    let makeCollectionParm pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([ ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("list"))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let makeValueParm pmid fid =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let contribName = ttc "RestfulObjects.Test.Data.ContributorService"

    let p1 = makeCollectionParm "ms" "Ms" mst
    let p2 = makeCollectionParm "ms" "Ms" mst
    let p3 = makeValueParm "id" "Id"

    let membersProp = 
        TProperty(JsonPropertyNames.Members, TObjectJson([ TProperty("ANonContributedAction", TObjectJson(makeServiceActionMemberNoParms "ANonContributedAction" sName mst))
                                                           TProperty("ACollectionContributedActionNoParms", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionNoParms" contribName mst [ p1 ]))
                                                           TProperty("ACollectionContributedActionParm", 
                                                                     TObjectJson(makeServiceActionMember "ACollectionContributedActionParm" contribName mst [ p2; p3 ]))]))

    let expected = 
        [ TProperty(JsonPropertyNames.MenuId, TObjectVal(mName))
          TProperty(JsonPropertyNames.Title, TObjectVal("Contributor Service"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "menus/%s" mName) RepresentationTypes.Menu "") ]))          
          membersProp
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([  ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Menu, ""), headers.ContentType)
    assertNonExpiringCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetMenuSimpleOnly(api : RestfulObjectsControllerBase) = 
    let sName =  ttc "RestfulObjects.Test.Data.RestDataRepository" 
    let mName =  "RestDataRepository"
    
    let argS = "x-ro-domain-model=simple"
    let url = sprintf "http://localhost/menus/%s?%s" mName argS
    
    jsonSetGetMsg api.Request url
    api.DomainModel <- "simple"
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
      
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let mstp = ttc "RestfulObjects.Test.Data.MostSimplePersist"
    let wa = ttc "RestfulObjects.Test.Data.WithAction"
    let wao = ttc "RestfulObjects.Test.Data.WithActionObject"
    let str = ttc "string"
    
    let makeParm pmid fid rt =                
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeValueParm pmid fid rt =                
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, TArray([]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let p1 = makeParm "withAction" "With Action" wao
    let p2 = makeParm "withAction" "With Action" wa
    let p3 = makeParm "withAction" "With Action" wao
    let p4 = makeParm "withOtherAction" "With Other Action" wao
    let p5 = makeParm "withAction" "With Action" wao
    let p6 = makeValueParm "parm" "Parm" str
    
    let expected = 
        [ TProperty(JsonPropertyNames.MenuId, TObjectVal(mName))
          TProperty(JsonPropertyNames.Title, TObjectVal("Rest Data Repository"))          
          TProperty
              (JsonPropertyNames.Links, TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "menus/%s" mName) RepresentationTypes.Menu "") ]))          
          TProperty
              (JsonPropertyNames.Members,                
               TObjectJson
                   ([ TProperty("AzContributedAction", TObjectJson(makeServiceActionMemberSimple "AzContributedAction" sName mst [ p1 ]))
                      TProperty("AzContributedActionOnBaseClass", TObjectJson(makeServiceActionMemberSimple "AzContributedActionOnBaseClass" sName mst [ p2 ]))                      
                      TProperty
                          ("AzContributedActionWithRefParm", TObjectJson(makeServiceActionMemberSimple "AzContributedActionWithRefParm" sName mst [ p3; p4 ]))                      
                      TProperty
                          ("AzContributedActionWithValueParm", 
                           TObjectJson(makeServiceActionMemberSimple "AzContributedActionWithValueParm" sName mst [ p5; p6 ]))
                      TProperty("CreateTransientMostSimple", TObjectJson(makeServiceActionMemberNoParmsSimple "CreateTransientMostSimple" sName mstp))                      
                      TProperty
                          ("CreateTransientWithValue", 
                           TObjectJson(makeServiceActionMemberNoParmsSimple "CreateTransientWithValue" sName (ttc "RestfulObjects.Test.Data.WithValuePersist")))                      
                      TProperty
                          ("CreateTransientWithReference", 
                           TObjectJson(makeServiceActionMemberNoParmsSimple "CreateTransientWithReference" sName (ttc "RestfulObjects.Test.Data.WithReferencePersist")))                      
                      TProperty
                          ("CreateTransientWithCollection",                            
                           TObjectJson
                               (makeServiceActionMemberNoParmsSimple "CreateTransientWithCollection" sName (ttc "RestfulObjects.Test.Data.WithCollectionPersist"))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([  ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Menu, ""), headers.ContentType)
    assertNonExpiringCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetMenuWithMediaType(api : RestfulObjectsControllerBase) = 
    let sName =  ttc "RestfulObjects.Test.Data.RestDataRepository" 
    let mName =  "RestDataRepository"
    let url = sprintf "http://localhost/menus/%s" mName
    
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.Menu
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let mstp = ttc "RestfulObjects.Test.Data.MostSimplePersist"
    let wa = ttc "RestfulObjects.Test.Data.WithAction"
    let wao = ttc "RestfulObjects.Test.Data.WithActionObject"
    let str = ttc "string"
    
    let makeParm pmid fid rt =                
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeValueParm pmid fid rt =                
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let p1 = makeParm "withAction" "With Action" wao
    let p2 = makeParm "withAction" "With Action" wa
    let p3 = makeParm "withAction" "With Action" wao
    let p4 = makeParm "withOtherAction" "With Other Action" wao
    let p5 = makeParm "withAction" "With Action" wao
    let p6 = makeValueParm "parm" "Parm" str
    
    let expected = 
        [ TProperty(JsonPropertyNames.MenuId, TObjectVal(mName))
          TProperty(JsonPropertyNames.Title, TObjectVal("Rest Data Repository"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "menus/%s" mName) RepresentationTypes.Menu "")
                              ]))          
          TProperty
              (JsonPropertyNames.Members,                
               TObjectJson
                   ([ TProperty("AzContributedAction", TObjectJson(makeServiceActionMember "AzContributedAction" sName mst [ p1 ]))
                      TProperty("AzContributedActionOnBaseClass", TObjectJson(makeServiceActionMember "AzContributedActionOnBaseClass" sName mst [ p2 ]))
                      TProperty("AzContributedActionWithRefParm", TObjectJson(makeServiceActionMember "AzContributedActionWithRefParm" sName mst [ p3; p4 ]))                      
                      TProperty
                          ("AzContributedActionWithValueParm", TObjectJson(makeServiceActionMember "AzContributedActionWithValueParm" sName mst [ p5; p6 ]))
                      TProperty("CreateTransientMostSimple", TObjectJson(makeServiceActionMemberNoParms "CreateTransientMostSimple" sName mstp))                      
                      TProperty
                          ("CreateTransientWithValue", 
                           TObjectJson(makeServiceActionMemberNoParms "CreateTransientWithValue" sName (ttc "RestfulObjects.Test.Data.WithValuePersist")))                      
                      TProperty
                          ("CreateTransientWithReference", 
                           TObjectJson(makeServiceActionMemberNoParms "CreateTransientWithReference" sName (ttc "RestfulObjects.Test.Data.WithReferencePersist")))                      
                      TProperty
                          ("CreateTransientWithCollection", 
                           TObjectJson(makeServiceActionMemberNoParms "CreateTransientWithCollection" sName (ttc "RestfulObjects.Test.Data.WithCollectionPersist"))) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([  ])) ]
   
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Menu, ""), headers.ContentType)
    assertNonExpiringCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let GetWithActionMenu(api : RestfulObjectsControllerBase) = 
    let sName =  ttc "RestfulObjects.Test.Data.WithActionService" 
    let mName =  "WithActionService"

    let url = sprintf "http://localhost/menus/%s" mName
    jsonSetGetMsg api.Request url
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let mp r n = sprintf ";%s=\"%s\"" r n
    
    let makeParm pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let makeParmWithFindMenu pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false))
                                                  TProperty(JsonPropertyNames.CustomFindMenu, TObjectVal(true))])) ])
        TProperty(pmid, p)
    
    let makeParmWithAC pmid pid fid rt =     
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "services/%s/actions/%s/params/%s/prompt" sName pid pmid
        let argP = 
            TProperty
                (JsonPropertyNames.Arguments, 
                 TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
        let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(3)) ]))
        let ac = TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithChoicesAndDefault pmid pid fid rt =       
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choice1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let choice2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        let obj1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))
                          TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(choice1)
                                             TObjectJson(choice2) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithChoices pmid pid fid rt =         
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choice1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let choice2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(choice1)
                                             TObjectJson(choice2) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithDefault pmid pid fid rt =       
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let obj1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectJson(obj1))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeStringParmWithDefaults pmid fid rt et =               
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal("string1")
                                             TObjectVal("string2")
                                             TObjectVal("string3") ]))
                          TProperty(JsonPropertyNames.Default, 
                                    TArray([ TObjectVal("string2")
                                             TObjectVal("string3") ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("string1", TObjectVal("string1"))
                                                                          TProperty("string2", TObjectVal("string2"))
                                                                          TProperty("string3", TObjectVal("string3")) ]))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Strings"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithDefaults pmid pid fid rt et =       
        let defaultRel = RelValues.Default + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let choiceRel = RelValues.Choice + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let c1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let c2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        let d1 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
        let d2 = 
            TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
            :: makeGetLinkProp defaultRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectJson(c1)
                                             TObjectJson(c2) ]))
                          TProperty(JsonPropertyNames.Default, 
                                    TArray([ TObjectJson(d1)
                                             TObjectJson(d2) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(et))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeValueParm pmid fid rt =              
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParm pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithRange pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomRange, TObjectJson([TProperty("min", TObjectVal(1)); TProperty("max", TObjectVal(500))]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

    let makeIntParmWithHint pmid fid rt =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class9 class10"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithChoicesAndDefault pmid fid rt =                
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
                          TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal(1)
                                             TObjectVal(2)
                                             TObjectVal(3) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("1", TObjectVal(1))
                                                                          TProperty("2", TObjectVal(2))
                                                                          TProperty("3", TObjectVal(3)) ]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithChoices pmid fid rt =               
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Choices, 
                                    TArray([ TObjectVal(1)
                                             TObjectVal(2)
                                             TObjectVal(3) ]))                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([ ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.CustomChoices, 
                                                            TObjectJson([ TProperty("1", TObjectVal(1))
                                                                          TProperty("2", TObjectVal(2))
                                                                          TProperty("3", TObjectVal(3)) ]))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithDefault pmid fid rt =                
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal(4))
                          
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeOptParm pmid fid rt d ml p =               
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([ ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(d))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(ml))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(p))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ])
        TProperty(pmid, p)
    
    let makeDTParm pmid =                
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Default, TObjectVal("2016-02-16"))
                          TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Parm"))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("date"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeParmWithConditionalChoices pmid pid fid rt =        
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "services/%s/actions/%s/params/%s/prompt" sName pid pmid            
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeIntParmWithConditionalChoices pmid pid fid rt =       
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "services/%s/actions/%s/params/%s/prompt" sName pid pmid             
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm3", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ]))
                                    TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let makeStringParmWithConditionalChoices pmid pid fid rt =        
        let autoRel = RelValues.Prompt + mp RelParamValues.Action pid + mp RelParamValues.Param pmid
        let acurl = sprintf "services/%s/actions/%s/params/%s/prompt" sName pid pmid      
        let argP = 
            TProperty(JsonPropertyNames.Arguments, 
                      TObjectJson([ TProperty("parm3", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ]))
                                    TProperty("parm4", 
                                              TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null))
                                                            TProperty(JsonPropertyNames.Links, TArray([  ])) ])) ]))        
        let ac = TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true)        
        let p = 
            TObjectJson([ TProperty(JsonPropertyNames.Links, 
                                    TArray([ 
                                             ac ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)
    
    let num = ttc "number"
    let str = ttc "string"
    let lst = ttc "list"

    let p1 = makeIntParm "parm1" "Parm1" num
    let p2 = makeIntParm "parm1" "Parm1" num
    let p3 = makeParm "parm2" "Parm2" mst
    let p4 = makeIntParm "parm1" "Parm1" num
    let p5 = makeParm "parm2" "Parm2" mst
    let p6 = makeIntParm "parm1" "Parm1" num
    let p7 = makeParm "parm2" "Parm2" mst
    let p8 = makeOptParm "parm" "Optional Parm" str "an optional parm" 101 "[A-Z]"
    let p9 = makeOptParm "parm" "Parm" str "" 0 ""
    let p10 = makeIntParm "parm1" "Parm1" num
    let p11 = makeIntParmWithChoicesAndDefault "parm7" "Parm7" num
    let p12 = makeParm "parm2" "Parm2" mst
    let p13 = makeParmWithChoicesAndDefault "parm8" "AnActionWithParametersWithChoicesWithDefaults" "Parm8" mst
    let p14 = makeParm "parm2" "Parm2" mst
    let p15 = makeParmWithChoices "parm4" "AnActionWithReferenceParameterWithChoices" "Parm4" mst
    let p16 = makeParmWithDefault "parm6" "AnActionWithReferenceParameterWithDefault" "Parm6" mst
    let p17 = makeParmWithAC "parm0" "AnActionWithReferenceParametersWithAutoComplete" "Parm0" mst
    let p18 = makeParmWithAC "parm1" "AnActionWithReferenceParametersWithAutoComplete" "Parm1" mst
    let p20 = makeIntParm "parm1" "Parm1" num
    let p21 = makeIntParmWithChoices "parm3" "Parm3" num
    let p22 = makeIntParmWithDefault "parm5" "Parm5" num
    let p25 = makeIntParm "parm1" "Parm1" num
    let p26 = makeParm "parm2" "Parm2" mst
    let p27 = makeIntParmWithHint "parm1" "Parm1" num
    let p28 = makeValueParm "parm2" "Parm2" str
    let p29 = makeIntParm "parm1" "Parm1" num
    let p30 = makeParm "parm2"  "Parm2" mst
    let p31 = makeIntParm "parm1" "Parm1" num
    let p32 = makeValueParm "parm2" "Parm2" str
    let p33 = makeIntParm "parm1" "Parm1" num
    let p34 = makeParm "parm2" "Parm2" mst
    let p35 = makeIntParm "parm1" "Parm1" num
    let p36 = makeParm "parm2" "Parm2" mst
    let p37 = makeIntParm "parm1" "Parm1" num
    let p38 = makeIntParm "parm2" "Parm2" num
    let p39 = makeDTParm "parm"
    let p40 = makeParmWithConditionalChoices "parm4" "AnActionWithReferenceParameterWithConditionalChoices" "Parm4" mst
    let p41 = makeIntParmWithConditionalChoices "parm3" "AnActionWithValueParametersWithConditionalChoices" "Parm3" num
    let p42 = makeStringParmWithConditionalChoices "parm4" "AnActionWithValueParametersWithConditionalChoices" "Parm4" str
    let p43 = makeStringParmWithDefaults "parm" "Parm" lst str
    let p44 = makeParmWithDefaults "parm" "AnActionWithCollectionParameterRef" "Parm" lst mst
    let p45 = makeIntParmWithRange "parm1" "Parm1" num
    let p46 = makeParmWithFindMenu "parm2" "Parm2" mst

    let expected = 
        [ TProperty(JsonPropertyNames.MenuId, TObjectVal(mName))
          TProperty(JsonPropertyNames.Title, TObjectVal("With Action Service"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Self (sprintf "menus/%s" mName) RepresentationTypes.Menu "") ]))
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ 
                                  TProperty("AnAction", TObjectJson(makeServiceActionMemberNoParms "AnAction" sName mst))                                  
                                  TProperty
                                      ("AnActionReturnsViewModel",                                        
                                       TObjectJson
                                           (makeServiceActionMemberNoParms "AnActionReturnsViewModel" sName (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                                  
                                  TProperty
                                      ("AnActionReturnsRedirectedObject",                                        
                                       TObjectJson
                                           (makeServiceActionMemberNoParms "AnActionReturnsRedirectedObject" sName 
                                                (ttc "RestfulObjects.Test.Data.RedirectedObject")))                                  
                                  TProperty
                                      ("AnActionReturnsWithDateTimeKeyQueryOnly",                                        
                                       TObjectJson
                                           (makeServiceActionMemberNoParms "AnActionReturnsWithDateTimeKeyQueryOnly" sName (ttc "RestfulObjects.Test.Data.WithDateTimeKey")))
                                  TProperty("AnActionAnnotatedIdempotent", TObjectJson(makeServiceActionMemberNoParms "AnActionAnnotatedIdempotent" sName mst))                                  
                                  TProperty
                                      ("AnActionAnnotatedIdempotentReturnsViewModel",                                        
                                       TObjectJson
                                           (makeServiceActionMemberNoParms "AnActionAnnotatedIdempotentReturnsViewModel" sName 
                                                (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                                  
                                  TProperty
                                      ("AnActionAnnotatedIdempotentReturnsNull", 
                                       TObjectJson(makeServiceActionMemberNoParms "AnActionAnnotatedIdempotentReturnsNull" sName mst))
                                  TProperty("AnActionAnnotatedQueryOnly", TObjectJson(makeServiceActionMemberNoParms "AnActionAnnotatedQueryOnly" sName mst))                                  
                                  TProperty
                                      ("AnActionAnnotatedQueryOnlyReturnsViewModel",                                        
                                       TObjectJson
                                           (makeServiceActionMemberNoParms "AnActionAnnotatedQueryOnlyReturnsViewModel" sName 
                                                (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                                  
                                  TProperty
                                      ("AnActionAnnotatedQueryOnlyReturnsNull", 
                                       TObjectJson(makeServiceActionMemberNoParms "AnActionAnnotatedQueryOnlyReturnsNull" sName mst))                                  
                                  TProperty
                                      ("AnActionReturnsCollection", TObjectJson(makeServiceActionCollectionMemberNoParms "AnActionReturnsCollection" sName mst))                                  
                                  TProperty
                                      ("AnActionReturnsCollectionEmpty", 
                                       TObjectJson(makeServiceActionCollectionMemberNoParms "AnActionReturnsCollectionEmpty" sName mst))                                  
                                  TProperty
                                      ("AnActionReturnsCollectionNull", 
                                       TObjectJson(makeServiceActionCollectionMemberNoParms "AnActionReturnsCollectionNull" sName mst))                                  
                                  TProperty
                                      ("AnActionReturnsCollectionWithParameters", 
                                       TObjectJson(makeServiceActionCollectionMember "AnActionReturnsCollectionWithParameters" sName mst [ p25; p26 ]))                                  
                                  TProperty
                                      ("AnActionReturnsCollectionWithScalarParameters", 
                                       TObjectJson(makeServiceActionCollectionMember "AnActionReturnsCollectionWithScalarParameters" sName mst [ p27; p28 ]))
                                  TProperty("AnActionReturnsNull", TObjectJson(makeServiceActionMemberNoParms "AnActionReturnsNull" sName mst))                                  
                                  TProperty
                                      ("AnActionReturnsNullViewModel",                                        
                                       TObjectJson
                                           (makeServiceActionMemberNoParms "AnActionReturnsNullViewModel" sName 
                                                (ttc "RestfulObjects.Test.Data.MostSimpleViewModel")))                                  
                                  TProperty
                                      ("AnActionReturnsObjectWithParameterAnnotatedQueryOnly", 
                                       TObjectJson(makeServiceActionMember "AnActionReturnsObjectWithParameterAnnotatedQueryOnly" sName mst [ p1 ]))                                  
                                  TProperty
                                      ("AnActionReturnsObjectWithParameters", 
                                       TObjectJson(makeServiceActionMember "AnActionReturnsObjectWithParameters" sName mst [ p2; p3 ]))                                  
                                  TProperty
                                      ("AnActionReturnsObjectWithParametersAnnotatedIdempotent", 
                                       TObjectJson(makeServiceActionMember "AnActionReturnsObjectWithParametersAnnotatedIdempotent" sName mst [ p4; p5 ]))                                  
                                  TProperty
                                      ("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", 
                                       TObjectJson(makeServiceActionMember "AnActionReturnsObjectWithParametersAnnotatedQueryOnly" sName mst [ p6; p7 ]))                                  
                                  TProperty
                                      ("AnActionReturnsQueryable", TObjectJson(makeServiceActionCollectionMemberNoParms "AnActionReturnsQueryable" sName mst))                                  
                                  TProperty
                                      ("AnActionReturnsQueryableWithParameters", 
                                       TObjectJson(makeServiceActionCollectionMember "AnActionReturnsQueryableWithParameters" sName mst [ p29; p30 ]))                                  
                                  TProperty
                                      ("AnActionReturnsQueryableWithScalarParameters", 
                                       TObjectJson(makeServiceActionCollectionMember "AnActionReturnsQueryableWithScalarParameters" sName mst [ p31; p32 ]))                                  
                                  TProperty
                                      ("AnActionReturnsScalar", 
                                       TObjectJson(makeActionMemberNumber "services" "AnActionReturnsScalar" sName "An Action Returns Scalar" "" "int" []))                                  
                                  TProperty
                                      ("AnActionReturnsScalarEmpty",                                        
                                       TObjectJson
                                           (makeActionMemberString "services" "AnActionReturnsScalarEmpty" sName "An Action Returns Scalar Empty" "" "string" []))                                  
                                  TProperty
                                      ("AnActionReturnsScalarNull",                                        
                                       TObjectJson
                                           (makeActionMemberString "services" "AnActionReturnsScalarNull" sName "An Action Returns Scalar Null" "" "string" []))                                  
                                  TProperty
                                      ("AnActionReturnsScalarWithParameters",                                        
                                       TObjectJson
                                           (makeActionMemberNumber "services" "AnActionReturnsScalarWithParameters" sName 
                                                "An Action Returns Scalar With Parameters" "" "int" [ p33; p34 ]))
                                  TProperty("AnActionReturnsVoid", TObjectJson(makeServiceActionVoidMember "AnActionReturnsVoid" sName))                                  
                                  TProperty
                                      ("AnActionReturnsVoidWithParameters",                                        
                                       TObjectJson
                                           (makeVoidActionMember "services" "AnActionReturnsVoidWithParameters" sName "An Action Returns Void With Parameters" 
                                                "an action for testing" [ p35; p36 ]))                                  
                                  TProperty
                                      ("AnActionValidateParameters",                                        
                                       TObjectJson
                                           (makeActionMemberNumber "services" "AnActionValidateParameters" sName "An Action Validate Parameters" "" "int" 
                                                [ p37; p38 ]))                                  
                                  TProperty
                                      ("AnActionWithCollectionParameter",                                        
                                       TObjectJson
                                           (makeVoidActionMember "services" "AnActionWithCollectionParameter" sName "An Action With Collection Parameter" "" 
                                                [ p43 ]))                                  
                                  TProperty
                                      ("AnActionWithCollectionParameterRef",                                        
                                       TObjectJson
                                           (makeVoidActionMember "services" "AnActionWithCollectionParameterRef" sName "An Action With Collection Parameter Ref" 
                                                "" [ p44 ]))                                  
                                  TProperty
                                      ("AnActionWithDateTimeParm", 
                                       TObjectJson(makeVoidActionMember "services" "AnActionWithDateTimeParm" sName "An Action With Date Time Parm" "" [ p39 ]))
                                  TProperty("AnActionWithOptionalParm", TObjectJson(makeServiceActionMember "AnActionWithOptionalParm" sName mst [ p8 ]))                                  
                                  TProperty
                                      ("AnActionWithOptionalParmQueryOnly", 
                                       TObjectJson(makeServiceActionMember "AnActionWithOptionalParmQueryOnly" sName mst [ p9 ]))                                  
                                  TProperty
                                      ("AnActionWithParametersWithChoicesWithDefaults", 
                                       TObjectJson(makeServiceActionMember "AnActionWithParametersWithChoicesWithDefaults" sName mst [ p10; p11; p12; p13 ]))                                  
                                  TProperty
                                      ("AnActionWithReferenceParameter", TObjectJson(makeServiceActionMember "AnActionWithReferenceParameter" sName mst [ p14 ]))                                  
                                  TProperty
                                       ("AnActionWithFindMenuParameter", TObjectJson(makeServiceActionMember "AnActionWithFindMenuParameter" sName mst [ p46 ]))                                  
                                  TProperty
                                      ("AnActionWithReferenceParameterWithChoices", 
                                       TObjectJson(makeServiceActionMember "AnActionWithReferenceParameterWithChoices" sName mst [ p15 ]))                                  
                                  TProperty
                                      ("AnActionWithReferenceParameterWithConditionalChoices", 
                                       TObjectJson(makeServiceActionMember "AnActionWithReferenceParameterWithConditionalChoices" sName mst [ p40 ]))                                  
                                  TProperty
                                      ("AnActionWithReferenceParameterWithDefault", 
                                       TObjectJson(makeServiceActionMember "AnActionWithReferenceParameterWithDefault" sName mst [ p16 ]))                                  
                                  TProperty
                                      ("AnActionWithReferenceParametersWithAutoComplete", 
                                       TObjectJson(makeServiceActionMember "AnActionWithReferenceParametersWithAutoComplete" sName mst [ p17; p18 ]))                                  
                                  TProperty
                                      ("AnOverloadedAction", 
                                       TObjectJson(makeActionMember "services" "AnOverloadedAction" sName "An Overloaded Action" "" mst []))                                  
                                  TProperty("AnActionWithValueParameter", TObjectJson(makeServiceActionMember "AnActionWithValueParameter" sName mst [ p20 ]))                                  
                                  TProperty("AnActionWithValueParameterWithRange", TObjectJson(makeServiceActionMember "AnActionWithValueParameterWithRange" sName mst [ p45 ]))
                                  TProperty
                                      ("AnActionWithValueParametersWithConditionalChoices", 
                                       TObjectJson(makeServiceActionMember "AnActionWithValueParametersWithConditionalChoices" sName mst [ p41; p42 ]))                                  
                                  TProperty
                                      ("AnActionWithValueParameterWithChoices", 
                                       TObjectJson(makeServiceActionMember "AnActionWithValueParameterWithChoices" sName mst [ p21 ]))                                  
                                  TProperty
                                      ("AnActionWithValueParameterWithDefault", 
                                       TObjectJson(makeServiceActionMember "AnActionWithValueParameterWithDefault" sName mst [ p22 ]))
                                  TProperty("AnError", TObjectJson(makeActionMemberNumber "services" "AnError" sName "An Error" "" "int" []))
                                  TProperty("AnErrorCollection", TObjectJson(makeServiceActionCollectionMemberNoParms "AnErrorCollection" sName mst))
                                  TProperty("AnErrorQuery", TObjectJson(makeServiceActionCollectionMemberNoParms "AnErrorQuery" sName mst)) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([  ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Menu, ""), headers.ContentType)
    assertNonExpiringCache headers
    Assert.IsNull(headers.ETag)
    compareObject expected parsedResult

let InvalidGetMenu(api : RestfulObjectsControllerBase) = 
    let mName = ""
    let url = sprintf "http://localhost/menus/%s" mName

    jsonSetGetMsg api.Request url
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedFramework.Facade.Error.BadRequestNOSException' was thrown.\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundGetMenu(api : RestfulObjectsControllerBase) = 
    let mName =  "NoSuchMenu"
    let url = sprintf "http://localhost/menus/%s" mName
    jsonSetGetMsg api.Request url 
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such menu %s\"" mName, headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let NotAcceptableGetMenuWrongMediaType(api : RestfulObjectsControllerBase) = 
    let mName =  "RestDataRepository"
    let url = sprintf "http://localhost/menus/%s" mName
    
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    let result = api.GetMenu(mName)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult