// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module Menus7

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open Newtonsoft.Json.Linq
open NakedObjects.Rest.Snapshot.Constants
open RestTestFunctions

let getExpected() = 
    let sName1 = "RestDataRepository"
    let sName2 = "WithActionService"
    let sName3 = "ContributorService"
    let sName4 = "TestTypeCodeMapper"
    let sName5 = "TestKeyCodeMapper"
    let srvRel1 = RelValues.Menu + makeParm RelParamValues.MenuId sName1
    let srvRel2 = RelValues.Menu + makeParm RelParamValues.MenuId sName2
    let srvRel3 = RelValues.Menu + makeParm RelParamValues.MenuId sName3
    let srvRel4 = RelValues.Menu + makeParm RelParamValues.MenuId sName4
    let srvRel5 = RelValues.Menu + makeParm RelParamValues.MenuId sName5
    
    let simpleLinks = 
        TArray([ TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Self SegmentValues.Menus RepresentationTypes.List "" (ttc "System.Object") true)
                 TObjectJson(makeGetLinkProp RelValues.Up SegmentValues.HomePage RepresentationTypes.HomePage "") ])
        
    let value = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Rest Data Repository")) 
                    :: makeGetLinkProp srvRel1 (sprintf "menus/%s" sName1) RepresentationTypes.Menu "")
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("With Action Service")) 
                    :: makeGetLinkProp srvRel2 (sprintf "menus/%s" sName2) RepresentationTypes.Menu "")
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Contributor Service")) 
                    :: makeGetLinkProp srvRel3 (sprintf "menus/%s" sName3) RepresentationTypes.Menu "") ])


    let valueWithTTC = 
        TArray
            ([ TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Rest Data Repository")) 
                    :: makeGetLinkProp srvRel1 (sprintf "menus/%s" sName1) RepresentationTypes.Menu "")
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("With Action Service")) 
                    :: makeGetLinkProp srvRel2 (sprintf "menus/%s" sName2) RepresentationTypes.Menu "")
               
               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Contributor Service")) 
                    :: makeGetLinkProp srvRel3 (sprintf "menus/%s" sName3) RepresentationTypes.Menu "") 

               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Test Type Code Mapper")) 
                    :: makeGetLinkProp srvRel4 (sprintf "menus/%s" sName4) RepresentationTypes.Menu "") 

               TObjectJson
                   (TProperty(JsonPropertyNames.Title, TObjectVal("Test Key Code Mapper")) 
                    :: makeGetLinkProp srvRel5 (sprintf "menus/%s" sName5) RepresentationTypes.Menu "") ])
    
    let simpleExpected = 
        [ TProperty(JsonPropertyNames.Links, simpleLinks)
          TProperty(JsonPropertyNames.Value, value)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    let simpleExpectedWithTTC = 
        [ TProperty(JsonPropertyNames.Links, simpleLinks)
          TProperty(JsonPropertyNames.Value, valueWithTTC)
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]   

    (simpleExpected, simpleExpectedWithTTC)

let GetMenus(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Menus
    jsonSetMsg api.Request url
    let result = api.GetMenus()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected = fst (getExpected())
    compareObject expected parsedResult

let GetMenusWithTTC(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Menus
    jsonSetMsg api.Request url
    let result = api.GetMenus()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected =  snd (getExpected())
    compareObject expected parsedResult

let GetMenusWithMediaType(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Menus
    jsonSetMsgWithProfile api.Request url RepresentationTypes.List
    let result = api.GetMenus()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected =  fst (getExpected())
    compareObject expected parsedResult

let GetMenusWithMediaTypeWithTTC(api : RestfulObjectsControllerBase) = 
    let url = testRoot + SegmentValues.Menus
    jsonSetMsgWithProfile api.Request url RepresentationTypes.List
    let result = api.GetMenus()
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.List, "", ttc "System.Object", true), headers.ContentType)
    //assertNonExpiringCache headers
    let expected =  snd (getExpected())
    compareObject expected parsedResult

// 406   
let NotAcceptableGetMenus(api : RestfulObjectsControllerBase) = 
   let url = testRoot + SegmentValues.Menus
   jsonSetMsgWithProfile api.Request url RepresentationTypes.User
   let result = api.GetMenus()
   let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
   assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
   Assert.AreEqual("199 RestfulObjects \"Failed outgoing json MT validation ic:  urn:org.restfulobjects:repr-types/user  og:  urn:org.restfulobjects:repr-types/list \"", headers.Headers.["Warning"].ToString())
   Assert.AreEqual("", jsonResult)
