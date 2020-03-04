// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module ObjectProperty16

open NUnit.Framework
open NakedObjects.Rest
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open System.Web
open System
open NakedObjects.Rest.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions
open System.Security.Principal
open NakedObjects.Rest.Snapshot.Utility

let GetValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl

    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(100))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                       
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult
               
let GetFileAttachmentProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAttachments"
    let oid = ktc "1"
    let pid = "FileAttachment"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
  
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let attachRelValue = RelValues.Attachment + makeParm RelParamValues.Property pid
    
    let attLink = 
        [ TProperty(JsonPropertyNames.Title, TObjectVal("afile"))
          TProperty(JsonPropertyNames.Rel, TObjectVal(attachRelValue))
          TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(purl)))
          TProperty(JsonPropertyNames.Type, TObjectVal("application/pdf"))
          TProperty(JsonPropertyNames.Method, TObjectVal("GET")) ]
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      TObjectJson(attLink) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("File Attachment"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("blob"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetFileAttachmentValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAttachments"
    let oid = ktc "1"
    let pid = "FileAttachment"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let mt = "application/pdf"
   
    jsonSetGetMsgWithMediaType api.Request url mt
    let result = api.GetProperty(oType, oid, pid)
    let (content, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.OK statusCode content
    Assert.AreEqual(mt, headers.ContentType.ToString())
    Assert.AreEqual("attachment; filename=afile", headers.ContentDisposition.ToString())
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    Assert.AreEqual("", content)

let GetAttachmentValueWrongMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAttachments"
    let oid = ktc "1"
    let pid = "FileAttachment"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let mt = "image/jpeg"

    jsonSetGetMsgWithMediaType api.Request url mt
    let result = api.GetProperty(oType, oid, pid)
    let (content, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
      
    assertStatusCode HttpStatusCode.NotAcceptable statusCode content

let GetImageAttachmentProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAttachments"
    let oid = ktc "1"
    let pid = "Image"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let attachRelValue = RelValues.Attachment + makeParm RelParamValues.Property pid
    
    let attLink = 
        [ TProperty(JsonPropertyNames.Title, TObjectVal("animage"))
          TProperty(JsonPropertyNames.Rel, TObjectVal(attachRelValue))
          TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(purl)))
          TProperty(JsonPropertyNames.Type, TObjectVal("image/jpeg"))
          TProperty(JsonPropertyNames.Method, TObjectVal("GET")) ]
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      TObjectJson(attLink)   ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Image"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("blob"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetImageAttachmentValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAttachments"
    let oid = ktc "1"
    let pid = "Image"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let mt = "image/jpeg"
    
    jsonSetGetMsgWithMediaType api.Request url mt
    let result = api.GetProperty(oType, oid, pid)
    let (content, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.OK statusCode content
    Assert.AreEqual(mt, headers.ContentType.ToString())
    Assert.AreEqual("attachment; filename=animage", headers.ContentDisposition.ToString())
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    Assert.AreEqual("", content)

let GetImageAttachmentValueWithDefault(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAttachments"
    let oid = ktc "1"
    let pid = "ImageWithDefault"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    let mt = "image/gif"
    
    jsonSetGetMsgWithMediaType api.Request url mt
    let result = api.GetProperty(oType, oid, pid)
    let (content, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.OK statusCode content
    Assert.AreEqual(mt, headers.ContentType.ToString())
    Assert.AreEqual("attachment; filename=animage.gif", headers.ContentDisposition.ToString())
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    Assert.AreEqual("", content)

let GetValuePropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValueViewModel"
    let ticks = (new DateTime(2012, 2, 10)).Ticks.ToString()
    let tsTicks = (new TimeSpan(1,2,3,4,5)).Ticks.ToString()
    let oid = ktc ("1--100--200--4--0----" + ticks + "--" + tsTicks + "--8--0")
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
        
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(100))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed"))
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                    ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetValuePropertyUserAuth(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AUserHiddenValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let p = new GenericPrincipal(new GenericIdentity("viewUser"), [||])
    System.Threading.Thread.CurrentPrincipal <- p
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AUserHiddenValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(0))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                                  
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A User Hidden Value"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetValuePropertySimpleOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let argS = "x-ro-domain-model=simple"
    let url = sprintf "%s?%s" purl argS
    
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    api.DomainModel <- "simple"
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
     
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(100))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                      
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetEnumValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
    let oid = ktc "1"
    let pid = "EnumByAttributeChoices"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
     
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "EnumByAttributeChoices"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(0))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectVal(0)
                             TObjectVal(1) ]))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(true))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                                  
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Enum By Attribute Choices"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.CustomDataType, TObjectVal("custom"))
                                  TProperty(JsonPropertyNames.CustomChoices, 
                                            TObjectJson([ TProperty("Value1", TObjectVal(0))
                                                          TProperty("Value2", TObjectVal(1)) ]))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetStringValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AStringValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let clearRel = RelValues.Clear + makeParm RelParamValues.Property "AStringValue"
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AStringValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(""))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                                                     
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
                             TObjectJson(makeDeleteLinkProp clearRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A String Value"))
                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(101))
                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(@"[A-Z]"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal("A string value for testing"))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(3))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetBlobValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
    let oid = ktc "1"
    let pid = "ByteArray"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ByteArray\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetClobValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
    let oid = ktc "1"
    let pid = "CharArray"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl

    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property CharArray\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

//let GetValuePropertyWithMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonGetMsg (sprintf "http://localhost/%s" purl)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectProperty)))
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(100))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                      
                      
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetChoicesValueProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AChoicesValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AChoicesValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(0))
//          TProperty(JsonPropertyNames.Choices, 
//                    TArray([ TObjectVal(1)
//                             TObjectVal(2)
//                             TObjectVal(3) ]))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(true))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                     
                      
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Choices Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.CustomChoices, 
//                                            TObjectJson([ TProperty("1", TObjectVal(1))
//                                                          TProperty("2", TObjectVal(2))
//                                                          TProperty("3", TObjectVal(3)) ]))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetDisabledValueProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ADisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(200))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
//                       ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Disabled Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetUserDisabledValueProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AUserDisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(0))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit"))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
//                       ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A User Disabled Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetUserDisabledValuePropertyAuthorised(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AUserDisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AUserDisabledValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(0))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                      
                      
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A User Disabled Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetReferenceProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
//    let val1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                      
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetAutoCompleteProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AnAutoCompleteReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let acurl = purl + "/prompt"
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
//    let val1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnAutoCompleteReference"
//    let autoRel = RelValues.Prompt + makeParm RelParamValues.Property "AnAutoCompleteReference"
//    let argP = 
//        TProperty
//            (JsonPropertyNames.Arguments, 
//             TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
//    let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(2)) ]))
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                             
                             
//                             TObjectJson
//                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
//                             TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true) ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Auto Complete Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let InvokeAutoComplete(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AnAutoCompleteReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = new JObject(new JProperty(JsonPropertyNames.XRoSearchTerm, new JObject(new JProperty("value", "12"))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let choiceRel = RelValues.Choice + makeParm RelParamValues.Property "AnAutoCompleteReference"
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
//        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal("AnAutoCompleteReference"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
//                              ]))
//          TProperty(JsonPropertyNames.Choices, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
//    compareObject expected parsedResult

//let InvokeAutoCompleteErrorNoParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AnAutoCompleteReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = new JObject()
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    //Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let InvokeAutoCompleteErrorMalformedParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AnAutoCompleteReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = new JObject(new JProperty("x-ro-search-term", "12"))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    //Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let InvokeAutoCompleteErrorUnrecognisedParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AnAutoCompleteReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = new JObject(new JProperty("x-ro-noSuchParm", new JObject(new JProperty("value", "12"))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    //Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let InvokeConditionalChoicesReference(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AConditionalChoicesReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
//    let acurl = purl + "/prompt"
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
//    let parms = new JObject(new JProperty("areference", new JObject(new JProperty("value", refParm))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
//    let choiceRel = RelValues.Choice + makeParm RelParamValues.Property "AConditionalChoicesReference"
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
//        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
//    let obj3 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("3")) 
//        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "3")) RepresentationTypes.Object roType
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesReference"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
//                             ]))
//          TProperty(JsonPropertyNames.Choices, 
//                    TArray([ TObjectJson(obj2)
//                             TObjectJson(obj3) ]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
//    compareObject expected parsedResult

//let InvokeConditionalChoicesReferenceErrorMalformedParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AConditionalChoicesReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = new JObject(new JProperty("areference", new JObject(new JProperty("value", "12"))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty
//              ("areference", 
               
//               TObjectJson
//                   ([ TProperty(JsonPropertyNames.Value, TObjectVal("12"))
                      
//                      TProperty
//                          (JsonPropertyNames.InvalidReason, TObjectVal("Argument is of wrong type is System.String expect RestfulObjects.Test.Data.MostSimple")) ])) ]
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual
//        ("199 RestfulObjects \"Argument is of wrong type is System.String expect RestfulObjects.Test.Data.MostSimple\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let InvokeConditionalChoicesReferenceErrorNoParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AConditionalChoicesReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = new JObject()
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesReference"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
//                              ]))
//          TProperty(JsonPropertyNames.Choices, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
//    compareObject expected parsedResult

//let InvokeConditionalChoicesReferenceErrorUnrecognisedParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AConditionalChoicesReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let acurl = purl + "/prompt"
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
//    let parms = new JObject(new JProperty("aunknownreference", new JObject(new JProperty("value", refParm))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Unrecognised conditional argument(s)\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let InvokeConditionalChoicesValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AConditionalChoicesValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = 
//        new JObject(new JProperty("avalue", new JObject(new JProperty("value", "100"))), new JProperty("astringvalue", new JObject(new JProperty("value", "2"))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "int"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesValue"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
//                              ]))
//          TProperty(JsonPropertyNames.Choices, 
//                    TArray([ TObjectVal(100)
//                             TObjectVal(2) ]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    compareObject expected parsedResult

//let InvokeConditionalChoicesValueErrorMalformedParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AConditionalChoicesValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = 
//        new JObject(new JProperty("avalue", new JObject(new JProperty("value", "fred"))), 
//                    new JProperty("astringvalue", new JObject(new JProperty("value", "2"))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty("avalue", 
//                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred"))
//                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("cannot format value fred as Int32")) ]))
//          TProperty("astringvalue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("2")) ])) ]
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"cannot format value fred as Int32\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let InvokeConditionalChoicesValueErrorMissingParm(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AConditionalChoicesValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
//    let acurl = purl + "/prompt"
//    let parms = new JObject(new JProperty("avalue", new JObject(new JProperty("value", 100))))
//    let args = CreateArgMap parms
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" acurl)
//    let result = api.GetPropertyPrompt(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let roType = ttc "int"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesValue"))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
//                              ]))
//          TProperty(JsonPropertyNames.Choices, 
//                    TArray([ TObjectVal(100)
//                             TObjectVal(0) ]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    compareObject expected parsedResult

//let GetReferencePropertyViewModel(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
//    let oid = ktc "1--1--1--1"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
//    let val1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed"))
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
//                    ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetDisabledReferenceProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "ADisabledReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
//    let val1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
//          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
//                       ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Disabled Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//let GetChoicesReferenceProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AChoicesReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let choiceRel = RelValues.Choice + makeParm RelParamValues.Property "AChoicesReference"
//    let valueRel = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
//    let val1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//    let obj1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
//    let obj2 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AChoicesReference"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
//          TProperty(JsonPropertyNames.Choices, 
//                    TArray([ TObjectJson(obj1)
//                             TObjectJson(obj2) ]))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(true))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                    
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Choices Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//// 400   
//let GetInvalidProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = " " // invalid 
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedObjects.Facade.BadRequestNOSException' was thrown.\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let GetNotFoundProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ANonExistentProperty" // doesn't exist 
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentProperty\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let GetHiddenValueProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AHiddenValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let GetUserHiddenValueProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AUserHiddenValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AUserHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let GetHiddenReferenceProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "1"
//    let pid = "AHiddenReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 500 
//let GetErrorValueProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithGetError"
//    let oid = ktc "1"
//    let pid = "AnErrorValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- true
//    let result = api.GetProperty(oType, oid, pid, args)
//    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- false
//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode)
//    // for some resaon stack trace has different depth on my machine when not debugging (only) ! 
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())

//// 500    
//let GetErrorReferenceProperty(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithGetError"
//    let oid = ktc "1"
//    let pid = "AnErrorReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- true
//    let result = api.GetProperty(oType, oid, pid, args)
//    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- false
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)
//    // for some resaon stack trace has different depth on my machine when not debugging (only) ! 
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())

//let GetPropertyAsCollection(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (sprintf "http://localhost/%s" purl)
//    let result = api.GetCollection(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such collection AValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutValuePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(101))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                             
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////   Assert.AreEqual(101, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let PutClobPropertyBadRequest(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
//    let oid = ktc "1"
//    let pid = "CharArray"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "a char array"))
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Value, TObjectVal("a char array"))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Not a suitable type; must be a Char[]")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + "Not a suitable type; must be a Char[]" + "\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//let PutBlobPropertyBadRequest(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
//    let oid = ktc "1"
//    let pid = "ByteArray"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "a byte array"))
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"" + "Field not editable" + "\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutDateTimeValuePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ADateTimeValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "2012-03-21T14:32:16"))
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let clearRel = RelValues.Clear + makeParm RelParamValues.Property "ADateTimeValue"
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "ADateTimeValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal("2012-03-21"))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
//          TProperty(JsonPropertyNames.Links, 
//                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                             
                          
                             
//                             TObjectJson
//                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
//                             TObjectJson(makeDeleteLinkProp clearRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Date Time Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal("A datetime value for testing"))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("date"))
//                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(4))
//                                  TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////   Assert.AreEqual(101, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let PutValuePropertyConcurrencySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let url = sprintf "http://localhost/objects/%s/%s" oType oid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    //RestfulObjectsControllerBase.ConcurrencyChecking <- true
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (url)
//    let result = api.GetObject(oType, oid, args)
//    let tag = result.Headers.ETag.Tag
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsgAndTag (sprintf "http://localhost/%s" purl) (parms.ToString()) tag
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(101))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                                  
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////   Assert.AreEqual(101, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let PutUserDisabledValuePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AUserDisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AUserDisabledValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(101))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                                         
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A User Disabled Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////   Assert.AreEqual(101, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let PutValuePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101), new JProperty("x-ro-validate-only", true))
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

////   Assert.AreEqual(101, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let DeleteValuePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    //  Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(0))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (ttc "RestfulObjects.Test.Data.WithValue"))
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                                  
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

//// reset Value 
////    Assert.AreEqual(0, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let DeleteValuePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let argS = "x-ro-validate-only=true"
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl argS)
//    let args = CreateReservedArgs argS
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

////    Assert.AreEqual(0, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let PutNullValuePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(0))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                               
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Value"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
//                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4"))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////   Assert.AreEqual(0, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let PutNullValuePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null), new JProperty("x-ro-validate-only", true))
//    //    Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

////   Assert.AreEqual(0, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//let PutReferencePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    //  Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let valueRel = RelValues.Value + makeParm RelParamValues.Property "AReference"
//    let val1 = 
//        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                                    
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////    Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//let PutReferencePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
//    //  Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

////    Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//// 16.3 delete ref property
//let DeleteReferencePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    //   Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(null))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (ttc "RestfulObjects.Test.Data.WithReference"))
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                                 
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////   Assert.AreSame(null, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//let DeleteReferencePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let argS = "x-ro-validate-only=true"
//    //   Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl argS)
//    let args = CreateReservedArgs argS
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

////   Assert.AreSame(null, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//let PutNullReferencePropertySuccess(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
//    //   Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
//    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
//          TProperty(JsonPropertyNames.Value, TObjectVal(null))
//          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          
//          TProperty
//              (JsonPropertyNames.Links, 
               
//               TArray
//                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
//                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                      
                                  
//                      TObjectJson
//                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
//                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
//          TProperty(JsonPropertyNames.Extensions, 
//                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
//                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
//                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
//                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
//                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
//    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), result.Content.Headers.ContentType)
//    assertTransactionalCache result
//    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
//    compareObject expected parsedResult

////     Assert.AreSame(null, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//let PutNullReferencePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null), new JProperty("x-ro-validate-only", true))
//    //   Assert.AreSame(instances<MostSimple>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

////     Assert.AreSame(null, (instances<WithReference>() |> Seq.filter (fun i -> i.Id = 2) |> Seq.head).AReference)
//// 400
//let PutWithValuePropertyMissingArgs(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) ""
//    let arg = CreateSingleValueArg(new JObject())
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400    
//let PutWithValuePropertyMalformedArgs(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("malformed", 101))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400
//let PutWithValuePropertyInvalidArgsValue(api : RestfulObjectsControllerBase) = 
//    let error = "cannot format value invalid value as Int32"
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 400
//let PutWithValuePropertyFailCrossValidation(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parm1 = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parm1.ToString())
//    let arg = CreateSingleValueArg parm1
//    api.Request <- msg
//    api.PutProperty(oType, oid, pid, arg) |> ignore
//    let pid = "AChoicesValue"
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parm2 = new JObject(new JProperty(JsonPropertyNames.Value, 3))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parm2.ToString())
//    let arg = CreateSingleValueArg parm2
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Value, TObjectVal(3))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 400
//let PutWithReferencePropertyInvalidArgsValue(api : RestfulObjectsControllerBase) = 
//    let error = "Not a suitable type; must be a Most Simple"
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let wvt = (ttc "RestfulObjects.Test.Data.WithValue")
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" wvt (ktc "1"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty
//              (JsonPropertyNames.Value, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt (ktc "1")))) ]))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.First().ToString())
//    compareObject expected parsedResult

//// 400
//let PutWithReferencePropertyFailCrossValidation(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
//    let refParm2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm1))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    api.PutProperty(oType, oid, pid, arg) |> ignore
//    let pid = "AChoicesReference"
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm2))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty
//              (JsonPropertyNames.Value, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" mst (ktc "2")))) ]))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.First().ToString())
//    compareObject expected parsedResult

//let PutWithReferencePropertyMalformedArgs(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, "malformed"))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.First().ToString())
//    Assert.AreEqual("", jsonResult)

//// 401
//let PutWithValuePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ADisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutWithValuePropertyUserDisabledValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AUserDisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Not authorized to edit\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 401    
//let PutWithReferencePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "ADisabledReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let PutWithValuePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AHiddenValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let PutWithReferencePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AHiddenReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let PutWithValuePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 405   
//let PutWithReferencePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let PutWithValuePropertyInvalidArgsName(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ANonExistentValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 406     
//let NotAcceptablePutPropertyWrongMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectCollection)))
//    try 
//        api.Request <- msg
//        api.PutProperty(oType, oid, pid, arg) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

//// new 
//let PutWithValuePropertyMissingArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400    
//let PutWithValuePropertyMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("malformed", 101), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 400
//let PutWithValuePropertyInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let error = "cannot format value invalid value as Int32"
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 400
//let PutWithValuePropertyFailCrossValidationValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parm1 = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parm1.ToString())
//    let arg = CreateSingleValueArg parm1
//    api.Request <- msg
//    api.PutProperty(oType, oid, pid, arg) |> ignore
//    let pid = "AChoicesValue"
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parm2 = new JObject(new JProperty(JsonPropertyNames.Value, 3), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parm2.ToString())
//    let arg = CreateSingleValueArg parm2
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Value, TObjectVal(3))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 400
//let PutWithReferencePropertyInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let error = "Not a suitable type; must be a Most Simple"
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let refParm = 
//        new JObject(new JProperty(JsonPropertyNames.Href, 
//                                  (new hrefType(sprintf "objects/%s/%s" (ttc "RestfulObjects.Test.Data.WithValue") (ktc "1"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty
//              (JsonPropertyNames.Value, 
               
//               TObjectJson
//                   ([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" (ttc "RestfulObjects.Test.Data.WithValue") (ktc "1")))) ]))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", result.Headers.Warning.First().ToString())
//    compareObject expected parsedResult

//// 400
//let PutWithReferencePropertyFailCrossValidationValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
//    let refParm2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm1), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    api.PutProperty(oType, oid, pid, arg) |> ignore
//    let pid = "AChoicesReference"
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm2))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty
//              (JsonPropertyNames.Value, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" mst (ktc "2")))) ]))
//          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]
//    Assert.AreEqual(unprocessableEntity, result.StatusCode, jsonResult)
//    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), result.Content.Headers.ContentType)
//    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", result.Headers.Warning.First().ToString())
//    compareObject expected parsedResult

//// 401
//let PutWithValuePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ADisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 401    
//let PutWithReferencePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "ADisabledReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let PutWithValuePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AHiddenValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let PutWithReferencePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AHiddenReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let PutWithValuePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 405   
//let PutWithReferencePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let PutWithValuePropertyInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ANonExistentValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null), new JProperty("x-ro-validate-only", true))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 500    
//let PutWithValuePropertyInternalError(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithError"
//    let oid = ktc "2"
//    let pid = "AnErrorValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
//    let result = api.PutProperty(oType, oid, pid, arg)
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace, 
//                    TArray([ TObjectVal(new errorType(" at  in "))
//                             TObjectVal(new errorType(" at  in ")) ]))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 500    
//let PutWithReferencePropertyInternalError(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithError"
//    let oid = ktc "2"
//    let pid = "AnErrorReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
//    let result = api.PutProperty(oType, oid, pid, arg)
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let arr1 = [ for i in 1 .. 3 ->   TObjectVal(new errorType(" at  in ")) ]
//    let arr2 = [ for i in 1 .. 4 ->   TObjectVal(new errorType(" at  in ")) ]
//    let arr3 = [ for i in 1 .. 6 ->   TObjectVal(new errorType(" at  in ")) ]


//    let expected1 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace,  TArray(arr1))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    let expected2 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace,  TArray(arr2))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    let expected3 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace,  TArray(arr3))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]


//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
    
//    // match arrays 3 and 6 deep
//    try
//        compareObject expected2 parsedResult
//    with e -> 
//        try 
//            compareObject expected2 parsedResult
//        with e -> 
//            compareObject expected3 parsedResult

//// 401    
//let DeleteValuePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ADisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 401    
//let DeleteReferencePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "ADisabledReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let DeleteValuePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AHiddenValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let DeleteReferencePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AHiddenReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let DeleteValuePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let DeleteReferencePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

////404
//let DeleteValuePropertyInvalidArgsName(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ANonExistentValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 406     
//let NotAcceptableDeletePropertyWrongMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectCollection)))
//    try 
//        let args = CreateReservedArgs ""
//        api.Request <- msg
//        api.DeleteProperty(oType, oid, pid, args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

//// new 
//let DeleteValuePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ADisabledValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 401    
//let DeleteReferencePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "ADisabledReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let DeleteValuePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AHiddenValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 404    
//let DeleteReferencePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithReference"
//    let oid = ktc "2"
//    let pid = "AHiddenReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let DeleteValuePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

//// 405    
//let DeleteReferencePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.Immutable"
//    let oid = ktc "1"
//    let pid = "AReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("GET", result.Content.Headers.Allow.First())
//    Assert.AreEqual("", jsonResult)

////404
//let DeleteValuePropertyInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ANonExistentValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty("x-ro-validate-only", true))
//    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s?%s" purl parmsEncoded)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//// 500    
//let DeleteValuePropertyInternalError(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithError"
//    let oid = ktc "2"
//    let pid = "AnErrorValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let expected = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace, 
//                    TArray([ TObjectVal(new errorType(" at  in "))
//                             TObjectVal(new errorType(" at  in ")) ]))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())
//    compareObject expected parsedResult

//// 500    
//let DeleteReferencePropertyInternalError(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithError"
//    let oid = ktc "1"
//    let pid = "AnErrorReference"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
//    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    api.PutProperty(oType, oid, pid, arg) |> ignore
//    let msg = jsonDeleteMsg (sprintf "http://localhost/%s" purl)
//    let args = CreateReservedArgs ""
//    api.Request <- msg
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
//    let result = api.DeleteProperty(oType, oid, pid, args)
//    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
//    let jsonResult = readSnapshotToJson result
//    let parsedResult = JObject.Parse(jsonResult)
    
//    let arr1 = [ for i in 1 .. 3 ->   TObjectVal(new errorType(" at  in ")) ]
//    let arr2 = [ for i in 1 .. 4 ->   TObjectVal(new errorType(" at  in ")) ]
//    let arr3 = [ for i in 1 .. 6 ->   TObjectVal(new errorType(" at  in ")) ]

//    let expected1 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace, TArray(arr1))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    let expected2 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace, TArray(arr2))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    let expected3 = 
//        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
//          TProperty(JsonPropertyNames.StackTrace, TArray(arr3))
//          TProperty(JsonPropertyNames.Links, TArray([]))
//          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

//    Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"An error exception\"", result.Headers.Warning.ToString())

//    // match arrays 3 , 4 and 6 deep
//    try
//        compareObject expected2 parsedResult
//    with e -> 
//        try 
//            compareObject expected2 parsedResult
//        with e -> 
//            compareObject expected3 parsedResult

//// 406
//let VerifyNotAcceptableGetPropertyWrongMediaType refType oType oid f (api : RestfulObjectsControllerBase) = 
//    let pid = "AValue"
//    let ourl = sprintf "%s/%s/%s" refType oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    try 
//        let args = CreateReservedArgs ""
//        let msg = jsonGetMsg (sprintf "http://localhost/%s" purl)
//        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ObjectCollection)))
//        api.Request <- msg
//        f (oType, ktc "1", pid, args) |> ignore
//        Assert.Fail("expect exception")
//    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)

//// 406       
//let NotAcceptableGetPropertyWrongMediaType(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    VerifyNotAcceptableGetPropertyWrongMediaType "objects" oType oid api.GetProperty api

//// 404            
//let PropertyNotFound(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "ADoesNotExistValue"
//    let purl = sprintf "http://localhost/objects/%s/properties/%s" oid pid
//    let args = CreateReservedArgs ""
//    api.Request <- jsonGetMsg (purl)
//    let result = api.GetProperty(oType, oid, pid, args)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
//    Assert.AreEqual("", jsonResult)

//let PutValuePropertyConcurrencyFail(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    //RestfulObjectsControllerBase.ConcurrencyChecking <- true
//    let tag = "\"fail\""
//    // Assert.AreEqual(100, (instances<WithValue>() |> Seq.filter (fun i -> i.Id = 1) |> Seq.head).AValue)
//    let msg = jsonPutMsgAndTag (sprintf "http://localhost/%s" purl) (parms.ToString()) tag
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(HttpStatusCode.PreconditionFailed, result.StatusCode, jsonResult)
//    Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)

//let PutValuePropertyMissingIfMatch(api : RestfulObjectsControllerBase) = 
//    let oType = ttc "RestfulObjects.Test.Data.WithValue"
//    let oid = ktc "1"
//    let pid = "AValue"
//    let ourl = sprintf "objects/%s/%s" oType oid
//    let purl = sprintf "%s/properties/%s" ourl pid
//    let url = sprintf "http://localhost/objects/%s/%s" oType oid
//    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
//    //RestfulObjectsControllerBase.ConcurrencyChecking <- true
//    let msg = jsonPutMsg (sprintf "http://localhost/%s" purl) (parms.ToString())
//    let arg = CreateSingleValueArg parms
//    api.Request <- msg
//    let result = api.PutProperty(oType, oid, pid, arg)
//    let jsonResult = readSnapshotToJson result
//    Assert.AreEqual(preconditionHeaderMissing, result.StatusCode, jsonResult)
//    Assert.AreEqual
//        ("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", 
//         result.Headers.Warning.ToString())
//    Assert.AreEqual("", jsonResult)