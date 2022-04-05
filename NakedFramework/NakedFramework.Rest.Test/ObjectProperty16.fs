// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.ObjectProperty16

open NUnit.Framework
open NakedFramework.Rest.Snapshot.Constants
open NakedFramework.Rest.API
open System.Net
open Newtonsoft.Json.Linq
open System.Web
open System

open System.Linq
open NakedObjects.Rest.Test.Functions
open System.Security.Principal

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

let GetValuePropertyWithMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectProperty
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

let GetChoicesValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AChoicesValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AChoicesValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(0))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectVal(1)
                             TObjectVal(2)
                             TObjectVal(3) ]))
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
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Choices Value"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.CustomChoices, 
                                            TObjectJson([ TProperty("1", TObjectVal(1))
                                                          TProperty("2", TObjectVal(2))
                                                          TProperty("3", TObjectVal(3)) ]))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetDisabledValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ADisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(200))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Disabled Value"))
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

let GetUserDisabledValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AUserDisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(0))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Not authorized to edit"))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                      
                       ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A User Disabled Value"))
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

let GetUserDisabledValuePropertyAuthorised(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AUserDisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AUserDisabledValue"
    
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
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A User Disabled Value"))
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

let GetReferenceProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
    let val1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
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
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetAutoCompleteProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AnAutoCompleteReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let acurl = purl + "/prompt"
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
    let val1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AnAutoCompleteReference"
    let autoRel = RelValues.Prompt + makeParm RelParamValues.Property "AnAutoCompleteReference"
    let argP = 
        TProperty
            (JsonPropertyNames.Arguments, 
             TObjectJson([ TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ]))
    let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson([ TProperty(JsonPropertyNames.MinLength, TObjectVal(2)) ]))
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                          
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
                             TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel acurl RepresentationTypes.Prompt "" "" true) ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("An Auto Complete Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let InvokeAutoComplete(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AnAutoCompleteReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl

    let parms = new JObject(new JProperty(JsonPropertyNames.XRoSearchTerm, new JObject(new JProperty("value", "12"))))
    
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let choiceRel = RelValues.Choice + makeParm RelParamValues.Property "AnAutoCompleteReference"
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) 
        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "1")) RepresentationTypes.Object roType
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal("AnAutoCompleteReference"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeAutoCompleteErrorNoParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AnAutoCompleteReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl    
    let parms = new JObject()
    let args = CreateArgMapWithReserved parms

    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    //Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvokeAutoCompleteErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AnAutoCompleteReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl
    let parms = new JObject(new JProperty("x-ro-search-term", "12"))
    let args = CreateArgMapWithReserved parms

    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    //Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvokeAutoCompleteErrorUnrecognisedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AnAutoCompleteReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl
    let parms = new JObject(new JProperty("x-ro-noSuchParm", new JObject(new JProperty("value", "12"))))
    let args = CreateArgMapWithReserved parms

    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    //Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Missing or malformed search term\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvokeConditionalChoicesReference(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AConditionalChoicesReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl
    
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
    let parms = new JObject(new JProperty("areference", new JObject(new JProperty("value", refParm))))
      
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let roType = ttc "RestfulObjects.Test.Data.MostSimple"
    let choiceRel = RelValues.Choice + makeParm RelParamValues.Property "AConditionalChoicesReference"
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) 
        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "2")) RepresentationTypes.Object roType
    let obj3 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("3")) 
        :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" roType (ktc "3")) RepresentationTypes.Object roType
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesReference"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                             ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectJson(obj2)
                             TObjectJson(obj3) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeConditionalChoicesReferenceErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AConditionalChoicesReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl

    let parms = new JObject(new JProperty("areference", new JObject(new JProperty("value", "12"))))
    
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty
              ("areference",                
               TObjectJson
                   ([ TProperty(JsonPropertyNames.Value, TObjectVal("12"))                      
                      TProperty
                          (JsonPropertyNames.InvalidReason, TObjectVal("Argument is of wrong type is System.String expect RestfulObjects.Test.Data.MostSimple")) ])) ]
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual
        ("199 RestfulObjects \"Argument is of wrong type is System.String expect RestfulObjects.Test.Data.MostSimple\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvokeConditionalChoicesReferenceErrorNoParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AConditionalChoicesReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl

    let parms = new JObject()
    
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesReference"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeConditionalChoicesReferenceErrorUnrecognisedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AConditionalChoicesReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl

    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
    let parms = new JObject(new JProperty("aunknownreference", new JObject(new JProperty("value", refParm))))
    
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Unrecognised conditional argument(s)\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let InvokeConditionalChoicesValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AConditionalChoicesValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl

    let parms = 
        new JObject(new JProperty("avalue", new JObject(new JProperty("value", "100"))), new JProperty("astringvalue", new JObject(new JProperty("value", "2"))))
    
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesValue"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectVal(100)
                             TObjectVal(2) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let InvokeConditionalChoicesValueErrorMalformedParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AConditionalChoicesValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl

    let parms = 
        new JObject(new JProperty("avalue", new JObject(new JProperty("value", "fred"))), 
                    new JProperty("astringvalue", new JObject(new JProperty("value", "2"))))
    
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty("avalue", 
                    TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("fred"))
                                  TProperty(JsonPropertyNames.InvalidReason, TObjectVal("cannot format value fred as Int32")) ]))
          TProperty("astringvalue", TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal("2")) ])) ]
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"cannot format value fred as Int32\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let InvokeConditionalChoicesValueErrorMissingParm(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AConditionalChoicesValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let prurl = sprintf "%s/properties/%s/prompt" ourl pid
    let acurl = purl + "/prompt"
    let url = sprintf "http://localhost/%s" acurl

    let parms = new JObject(new JProperty("avalue", new JObject(new JProperty("value", 100))))
    
    let args = CreateArgMapWithReserved parms
    jsonSetGetMsg api.Request url
    let result = api.GetPropertyPrompt(oType, oid, pid, args)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
        
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal("AConditionalChoicesValue"))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self prurl RepresentationTypes.Prompt "")
                              ]))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectVal(100)
                             TObjectVal(0) ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.Prompt, "", "", true), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length = 0) 
    compareObject expected parsedResult

let GetReferencePropertyViewModel(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReferenceViewModel"
    let oid = ktc "1--1--1--1"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
      
    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
    let val1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field disabled as object cannot be changed"))
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")
                    ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetDisabledReferenceProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "ADisabledReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let valueRel = RelValues.Value + makeParm RelParamValues.Property pid
    let val1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
          TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                      
                       ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Disabled Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let GetChoicesReferenceProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AChoicesReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let choiceRel = RelValues.Choice + makeParm RelParamValues.Property "AChoicesReference"
    let valueRel = RelValues.Value + makeParm RelParamValues.Property "AChoicesReference"
    let val1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    let obj1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1")) RepresentationTypes.Object mst
    let obj2 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AChoicesReference"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
          TProperty(JsonPropertyNames.Choices, 
                    TArray([ TObjectJson(obj1)
                             TObjectJson(obj2) ]))
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
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Choices Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

// 400   
let GetInvalidProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = " " // invalid 
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Exception of type 'NakedFramework.Facade.Error.BadRequestNOSException' was thrown.\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let GetNotFoundProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ANonExistentProperty" // doesn't exist 
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentProperty\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let GetHiddenValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AHiddenValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let GetUserHiddenValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AUserHiddenValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AUserHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let GetHiddenReferenceProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "1"
    let pid = "AHiddenReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 500 
let GetErrorValueProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithGetError"
    let oid = ktc "1"
    let pid = "AnErrorValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- true
    let result = api.GetProperty(oType, oid, pid)
    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
  
    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    // for some resaon stack trace has different depth on my machine when not debugging (only) ! 
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())

// 500    
let GetErrorReferenceProperty(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithGetError"
    let oid = ktc "1"
    let pid = "AnErrorReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- true
    let result = api.GetProperty(oType, oid, pid)
    RestfulObjects.Test.Data.WithGetError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
   
    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    // for some resaon stack trace has different depth on my machine when not debugging (only) ! 
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())

let GetPropertyAsCollection(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetGetMsg api.Request url
    let result = api.GetCollection(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such collection AValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutValuePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(101))
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
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutDateTimeValuePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ADateTimeValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "2012-03-21T14:32:16"))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    let clearRel = RelValues.Clear + makeParm RelParamValues.Property "ADateTimeValue"
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "ADateTimeValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal("2012-03-21"))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))
          TProperty(JsonPropertyNames.Links, 
                    TArray([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object oType)
                             TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                                                    
                             TObjectJson
                                 (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                                  :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "")
                             TObjectJson(makeDeleteLinkProp clearRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Date Time Value"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal("A datetime value for testing"))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("date"))
                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(4))
                                  TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(true)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutUserDisabledValuePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AUserDisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AUserDisabledValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(101))
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
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A User Disabled Value"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"))
                                  TProperty(JsonPropertyNames.Format, TObjectVal("int"))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutValuePropertyConcurrencySuccess(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let gurl = sprintf "http://localhost/objects/%s/%s" oType oid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    
    jsonSetGetMsg api1.Request gurl
    let result = api1.GetObject(oType, oid)
    let (_, _, headers) = readActionResult result api1.ControllerContext.HttpContext
    
    let tag = headers.ETag.Tag

    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api2.Request url (parms.ToString())
    setIfMatch api2.Request (tag.ToString())

    let result = api2.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
      
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(101))
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
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutClobPropertyBadRequest(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
    let oid = ktc "1"
    let pid = "CharArray"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "a char array"))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Value, TObjectVal("a char array"))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Not a suitable type; must be a Char[]")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + "Not a suitable type; must be a Char[]" + "\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

let PutBlobPropertyBadRequest(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithScalars"
    let oid = ktc "1"
    let pid = "ByteArray"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "a byte array"))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"" + "Field not editable" + "\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutValuePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
       
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
       
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let DeleteValuePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid    
    let url = sprintf "http://localhost/%s" purl
    
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
        
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(0))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (ttc "RestfulObjects.Test.Data.WithValue"))
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
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let DeleteValuePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let argS = "x-ro-validate-only=true"
    let url = sprintf "http://localhost/%s?%s" purl argS

    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PutNullValuePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
            
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AValue"
    
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
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutNullValuePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PutReferencePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    let valueRel = RelValues.Value + makeParm RelParamValues.Property "AReference"
    let val1 = 
        TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp valueRel (sprintf "objects/%s/%s" mst (ktc "2")) RepresentationTypes.Object mst
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectJson(val1))
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
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutReferencePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm),
                            new JProperty("x-ro-validate-only", true))
    
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// 16.3 delete ref property
let DeleteReferencePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"

    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
      
    let parsedResult = JObject.Parse(jsonResult)
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(null))
          TProperty(JsonPropertyNames.HasChoices, TObjectVal(false))          
          TProperty
              (JsonPropertyNames.Links,                
               TArray
                   ([ TObjectJson(makeGetLinkProp RelValues.Up ourl RepresentationTypes.Object (ttc "RestfulObjects.Test.Data.WithReference"))
                      TObjectJson(makeGetLinkProp RelValues.Self purl RepresentationTypes.ObjectProperty "")                                                       
                      TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) 
                           :: makePutLinkProp modifyRel purl RepresentationTypes.ObjectProperty "") ]))
          TProperty(JsonPropertyNames.Extensions, 
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]

    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let DeleteReferencePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let argS = "x-ro-validate-only=true"
       
    let url = sprintf "http://localhost/%s?%s" purl argS
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
      
    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PutNullReferencePropertySuccess(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    let modifyRel = RelValues.Modify + makeParm RelParamValues.Property "AReference"
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(pid))
          TProperty(JsonPropertyNames.Value, TObjectVal(null))
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
                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal("A Reference"))
                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(mst))
                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0))
                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ]
    
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.ObjectProperty), headers.ContentType)
    assertTransactionalCache headers
    //Assert.IsTrue(result.Headers.ETag.Tag.Length > 0)
    compareObject expected parsedResult

let PutNullReferencePropertySuccessValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null),
                            new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
  
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NoContent statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// 400
let PutWithValuePropertyMissingArgs(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved(new JObject())
    jsonSetPutMsg api.Request url ""
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400    
let PutWithValuePropertyMalformedArgs(api : RestfulObjectsControllerBase) =
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("malformed", 101))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments : Enable DebugWarnings to see message\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400
let PutWithValuePropertyInvalidArgsValue(api : RestfulObjectsControllerBase) = 
    let error = "cannot format value invalid value as Int32"
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 400
let PutWithValuePropertyFailCrossValidation(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parm1 = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parm1
    jsonSetPutMsg api1.Request url (parm1.ToString())
    setIfMatch api1.Request "*"
    let result = api1.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api1.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    
    let pid2 = "AChoicesValue"
    let purl2 = sprintf "%s/properties/%s" ourl pid2
    let parm2 = new JObject(new JProperty(JsonPropertyNames.Value, 3))
    
    let url = sprintf "http://localhost/%s" purl2
    let arg2 = CreateSingleValueArgWithReserved parm2
    jsonSetPutMsg api2.Request url (parm2.ToString())
    setIfMatch api2.Request "*"
    let result = api2.PutProperty(oType, oid, pid2, arg2)
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty(JsonPropertyNames.Value, TObjectVal(3))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 400
let PutWithReferencePropertyInvalidArgsValue(api : RestfulObjectsControllerBase) = 
    let error = "Not a suitable type; must be a Most Simple"
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let wvt = (ttc "RestfulObjects.Test.Data.WithValue")
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" wvt (ktc "1"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty
              (JsonPropertyNames.Value, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" wvt (ktc "1")))) ]))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

// 400
let PutWithReferencePropertyFailCrossValidation(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "3"
    let oid1 = ktc "1"
    let oid2 = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid1)).ToString()))
    let refParm2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst oid2)).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm1))
    
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api1.Request url (parms.ToString())
    setIfMatch api1.Request "*"
    let result = api1.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api1.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    
    let pid2 = "AChoicesReference"
    let purl2 = sprintf "%s/properties/%s" ourl pid2
    let parms2 = new JObject(new JProperty(JsonPropertyNames.Value, refParm2))
    
    let url = sprintf "http://localhost/%s" purl2
    let arg2 = CreateSingleValueArgWithReserved parms2 
    jsonSetPutMsg api2.Request url (parms2.ToString())
    setIfMatch api2.Request "*"

    let result = api2.PutProperty(oType, oid, pid2, arg2)
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty
              (JsonPropertyNames.Value, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" mst oid2))) ]))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

let PutWithReferencePropertyMalformedArgs(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, "malformed"))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", headers.Headers.["Warning"].First().ToString())
    Assert.AreEqual("", jsonResult)

// 401
let PutWithValuePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ADisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
  
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutWithValuePropertyUserDisabledValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AUserDisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Not authorized to edit\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 401    
let PutWithReferencePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "ADisabledReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let PutWithValuePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AHiddenValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let PutWithReferencePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AHiddenReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let PutWithValuePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333))
    let url = sprintf "http://localhost/%s" purl
   
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 405   
let PutWithReferencePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let PutWithValuePropertyInvalidArgsName(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ANonExistentValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 406     
let NotAcceptablePutPropertyWrongMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    
    let url = sprintf "http://localhost/%s" purl
       
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsgWithProfile api.Request url (parms.ToString()) RepresentationTypes.ObjectCollection
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// new 
let PutWithValuePropertyMissingArgsValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400    
let PutWithValuePropertyMalformedArgsValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("malformed", 101), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.BadRequest statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments : Enable DebugWarnings to see message\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 400
let PutWithValuePropertyInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let error = "cannot format value invalid value as Int32"
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, "invalid value"), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
   
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty(JsonPropertyNames.Value, TObjectVal("invalid value"))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 400
let PutWithValuePropertyFailCrossValidationValidateOnly(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parm1 = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parm1
    jsonSetPutMsg api1.Request url (parm1.ToString())
    setIfMatch api1.Request "*"
    let result = api1.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api1.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    
    let pid2 = "AChoicesValue"
    let purl2 = sprintf "%s/properties/%s" ourl pid2
    let parms2 = new JObject(new JProperty(JsonPropertyNames.Value, 3), 
                             new JProperty("x-ro-validate-only", true))
    
    let url = sprintf "http://localhost/%s" purl2
    let arg2 = CreateSingleValueArgWithReserved parms2
    jsonSetPutMsg api2.Request url (parms2.ToString())
    setIfMatch api2.Request "*"
    let result = api2.PutProperty(oType, oid, pid2, arg2)
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty(JsonPropertyNames.Value, TObjectVal(3))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]

    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 400
let PutWithReferencePropertyInvalidArgsValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let error = "Not a suitable type; must be a Most Simple"
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let refParm = 
        new JObject(new JProperty(JsonPropertyNames.Href, 
                                  (new hrefType(sprintf "objects/%s/%s" (ttc "RestfulObjects.Test.Data.WithValue") (ktc "1"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
    
    let url = sprintf "http://localhost/%s" purl
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let expected = 
        [ TProperty
              (JsonPropertyNames.Value,            
               TObjectJson
                   ([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" (ttc "RestfulObjects.Test.Data.WithValue") (ktc "1")))) ]))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal(error)) ]
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"" + error + "\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

// 400
let PutWithReferencePropertyFailCrossValidationValidateOnly(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm1 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "1"))).ToString()))
    let refParm2 = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    
    let parm1 = new JObject(new JProperty(JsonPropertyNames.Value, refParm1))
    
    let url = sprintf "http://localhost/%s" purl
    let arg = CreateSingleValueArgWithReserved parm1
    jsonSetPutMsg api1.Request url (parm1.ToString())
    setIfMatch api1.Request "*"
    let result = api1.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api1.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.OK statusCode jsonResult
    
    let pid2 = "AChoicesReference"
    let purl2 = sprintf "%s/properties/%s" ourl pid2
    let parms2 = new JObject(new JProperty(JsonPropertyNames.Value, refParm2), 
                             new JProperty("x-ro-validate-only", true))
    
    let url = sprintf "http://localhost/%s" purl2
    let arg2 = CreateSingleValueArgWithReserved parms2
    jsonSetPutMsg api2.Request url (parms2.ToString())
    setIfMatch api2.Request "*"
    let result = api2.PutProperty(oType, oid, pid2, arg2)
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty
              (JsonPropertyNames.Value, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "objects/%s/%s" mst (ktc "2")))) ]))
          TProperty(JsonPropertyNames.InvalidReason, TObjectVal("Cross validation failed")) ]
    
    assertStatusCode unprocessableEntity statusCode jsonResult
    Assert.AreEqual(new typeType(RepresentationTypes.BadArguments), headers.ContentType)
    Assert.AreEqual("199 RestfulObjects \"Cross validation failed\"", headers.Headers.["Warning"].First().ToString())
    compareObject expected parsedResult

// 401
let PutWithValuePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ADisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 401    
let PutWithReferencePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "ADisabledReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let PutWithValuePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AHiddenValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let PutWithReferencePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AHiddenReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let PutWithValuePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 333), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 405   
let PutWithReferencePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let PutWithValuePropertyInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ANonExistentValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null), new JProperty("x-ro-validate-only", true))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"

    let result = api.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 500    
let PutWithValuePropertyInternalError(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "2"
    let pid = "AnErrorValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, null))
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
    let result = api.PutProperty(oType, oid, pid, arg)
    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([ TObjectVal(new errorType(" at  in "))
                             TObjectVal(new errorType(" at  in "))
                             TObjectVal(new errorType(" at  in "))
                             TObjectVal(new errorType(" at  in ")) ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 500    
let PutWithReferencePropertyInternalError(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "2"
    let pid = "AnErrorReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    
    let url = sprintf "http://localhost/%s" purl
    
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request "*"
    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
    let result = api.PutProperty(oType, oid, pid, arg)
    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let arr1 = [ for _ in 1 .. 5 ->   TObjectVal(new errorType(" at  in ")) ]
    let arr2 = [ for _ in 1 .. 4 ->   TObjectVal(new errorType(" at  in ")) ]
    let arr3 = [ for _ in 1 .. 6 ->   TObjectVal(new errorType(" at  in ")) ]

    let expected1 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace,  TArray(arr1))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    let expected2 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace,  TArray(arr2))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    let expected3 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace,  TArray(arr3))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    
    // match arrays 3 and 6 deep
    try
        compareObject expected1 parsedResult
    with e -> 
        try 
            compareObject expected2 parsedResult
        with e -> 
            compareObject expected3 parsedResult

// 401    
let DeleteValuePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ADisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 401    
let DeleteReferencePropertyDisabledValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "ADisabledReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let DeleteValuePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AHiddenValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let DeleteReferencePropertyInvisibleValue(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AHiddenReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let DeleteValuePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let DeleteReferencePropertyOnImmutableObject(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

//404
let DeleteValuePropertyInvalidArgsName(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ANonExistentValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 406     
let NotAcceptableDeletePropertyWrongMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    
    let url = sprintf "http://localhost/%s" purl
    jsonSetDeleteMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    setIfMatch api.Request "*"
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext
      
    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// new 
let DeleteValuePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ADisabledValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 401    
let DeleteReferencePropertyDisabledValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "ADisabledReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.Forbidden statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"Field not editable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let DeleteValuePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AHiddenValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 404    
let DeleteReferencePropertyInvisibleValueValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithReference"
    let oid = ktc "2"
    let pid = "AHiddenReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property AHiddenReference\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let DeleteValuePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    
    jsonSetDeleteMsg api.Request url
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
      
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

// 405    
let DeleteReferencePropertyOnImmutableObjectValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.Immutable"
    let oid = ktc "1"
    let pid = "AReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    
    jsonSetDeleteMsg api.Request url
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode HttpStatusCode.MethodNotAllowed statusCode jsonResult  
    Assert.AreEqual("199 RestfulObjects \"object is immutable\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("GET", headers.Headers.["Allow"].ToString())
    Assert.AreEqual("", jsonResult)

//404
let DeleteValuePropertyInvalidArgsNameValidateOnly(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ANonExistentValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let parms = new JObject(new JProperty("x-ro-validate-only", true))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
   
    let url = sprintf "http://localhost/%s?%s" purl parmsEncoded
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    api.ValidateOnly <- true
    let result = api.DeleteProperty(oType, oid, pid)
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
       
    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"No such property ANonExistentValue\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

// 500    
let DeleteValuePropertyInternalError(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "2"
    let pid = "AnErrorValue"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let url = sprintf "http://localhost/%s" purl 
    
    jsonSetDeleteMsg api.Request url
    setIfMatch api.Request "*"
    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
    let result = api.DeleteProperty(oType, oid, pid)
    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)

    let expected = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, 
                    TArray([ TObjectVal(new errorType(" at  in "))
                             TObjectVal(new errorType(" at  in "))
                             TObjectVal(new errorType(" at  in "))
                             TObjectVal(new errorType(" at  in ")) ]))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())
    compareObject expected parsedResult

// 500    
let DeleteReferencePropertyInternalError(api1 : RestfulObjectsControllerBase) (api2 : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithError"
    let oid = ktc "1"
    let pid = "AnErrorReference"
    let ourl = sprintf "objects/%s/%s" oType oid
    let purl = sprintf "%s/properties/%s" ourl pid
    let mst = ttc "RestfulObjects.Test.Data.MostSimple"
    let refParm = new JObject(new JProperty(JsonPropertyNames.Href, (new hrefType(sprintf "objects/%s/%s" mst (ktc "2"))).ToString()))
    let parms = new JObject(new JProperty(JsonPropertyNames.Value, refParm))
    let url = sprintf "http://localhost/%s" purl 
      
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api1.Request url (parms.ToString())
    setIfMatch api1.Request "*"
    let result = api1.PutProperty(oType, oid, pid, arg)
    let (jsonResult, statusCode, _) = readActionResult result api1.ControllerContext.HttpContext
    assertStatusCode HttpStatusCode.OK statusCode jsonResult

    jsonSetDeleteMsg api2.Request url
    setIfMatch api2.Request "*"
    RestfulObjects.Test.Data.WithError.ThrowErrors <- true
    let result = api2.DeleteProperty(oType, oid, pid)
    RestfulObjects.Test.Data.WithError.ThrowErrors <- false
    let (jsonResult, statusCode, headers) = readActionResult result api2.ControllerContext.HttpContext
    let parsedResult = JObject.Parse(jsonResult)
    
    let arr1 = [ for _ in 1 .. 5 ->   TObjectVal(new errorType(" at  in ")) ]
    let arr2 = [ for _ in 1 .. 4 ->   TObjectVal(new errorType(" at  in ")) ]
    let arr3 = [ for _ in 1 .. 6 ->   TObjectVal(new errorType(" at  in ")) ]

    let expected1 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, TArray(arr1))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    let expected2 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, TArray(arr2))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    let expected3 = 
        [ TProperty(JsonPropertyNames.Message, TObjectVal("An error exception"))          
          TProperty(JsonPropertyNames.StackTrace, TArray(arr3))
          TProperty(JsonPropertyNames.Links, TArray([]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

    assertStatusCode HttpStatusCode.InternalServerError statusCode jsonResult
    Assert.AreEqual("199 RestfulObjects \"An error exception\"", headers.Headers.["Warning"].ToString())

    // match arrays 3 , 4 and 6 deep
    try
        compareObject expected1 parsedResult
    with e -> 
        try 
            compareObject expected2 parsedResult
        with e -> 
            compareObject expected3 parsedResult

//// 406       
let NotAcceptableGetPropertyWrongMediaType(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let url = sprintf "http://localhost/objects/%s/properties/%s" oid pid
    jsonSetGetMsgWithProfile api.Request url RepresentationTypes.ObjectCollection
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotAcceptable statusCode jsonResult
    Assert.AreEqual("", jsonResult)

// 404            
let PropertyNotFound(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "ADoesNotExistValue"
    let url = sprintf "http://localhost/objects/%s/properties/%s" oid pid
   
    jsonSetGetMsg api.Request url
    let result = api.GetProperty(oType, oid, pid)
    let (jsonResult, statusCode, _) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.NotFound statusCode jsonResult
    Assert.AreEqual("", jsonResult)

let PutValuePropertyConcurrencyFail(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let url = sprintf "http://localhost/objects/%s/properties/%s" oid pid

    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    let tag = "\"fail\""      
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    setIfMatch api.Request tag
    let result = api.PutProperty(oType, oid, pid, arg)    
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext

    assertStatusCode HttpStatusCode.PreconditionFailed statusCode jsonResult

    Assert.AreEqual("199 RestfulObjects \"Object changed by another user\"", headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)

let PutValuePropertyMissingIfMatch(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithValue"
    let oid = ktc "1"
    let pid = "AValue"
    let url = sprintf "http://localhost/objects/%s/properties/%s" oid pid

    let parms = new JObject(new JProperty(JsonPropertyNames.Value, 101))
    let arg = CreateSingleValueArgWithReserved parms
    jsonSetPutMsg api.Request url (parms.ToString())
    let result = api.PutProperty(oType, oid, pid, arg)    
    let (jsonResult, statusCode, headers) = readActionResult result api.ControllerContext.HttpContext
    
    assertStatusCode preconditionHeaderMissing statusCode jsonResult

    Assert.AreEqual
        ("199 RestfulObjects \"If-Match header required with last-known value of ETag for the resource in order to modify its state\"", 
         headers.Headers.["Warning"].ToString())
    Assert.AreEqual("", jsonResult)