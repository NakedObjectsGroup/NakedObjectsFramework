// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module RestTestFunctions

open System
open System.Net
open System.Net.Http
open System.IO
open Newtonsoft.Json.Linq
open NakedObjects.Rest.Snapshot.Constants
open NUnit.Framework
open System.Linq
open NakedObjects.Rest.Model
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open NakedObjects.Rest
open Microsoft.AspNetCore.Mvc
open Microsoft.Net.Http.Headers

let mapCodeToType (code : string) : string = code
let mapTypeToCode (typ : string) : string = typ

let mapCodeToKey (code : string) : string = code
let mapKeyToCode (key : string) : string = key

let mutable ctt = mapCodeToType
let mutable ttc = mapTypeToCode

let mutable ctk = mapCodeToKey
let mutable ktc = mapKeyToCode

let testRoot = "http://localhost/"

let oneDay = new TimeSpan(1,0,0,0)

let oneHour = new TimeSpan(1,0,0)

let unprocessableEntity = box(422) :?> System.Net.HttpStatusCode

let preconditionHeaderMissing = box(428) :?> System.Net.HttpStatusCode

type partCmp (s : string) =
    let partS = s 
    override x.Equals (o : obj) =
        partS.StartsWith(o.ToString()) 
    override x.GetHashCode() = 
        partS.GetHashCode()
    override x.ToString() = 
        partS

type hrefType (href : string) =
    let h = href 
    member x.full with get() = sprintf "http://localhost/%s" h  
    member x.fullWithPort with get() = sprintf "http://localhost:80/%s" h  // temp hack for json.net bug ? 
    override x.Equals (o : obj) =
        (x.full  = o.ToString()) || (x.fullWithPort = o.ToString())
    override x.GetHashCode() = 
        x.full.GetHashCode()
    override x.ToString() = 
        x.full
        
type typeType (mt, dt, et, simple) =
    let m = mt 
    let d = dt
    let e = et
    let s = simple
    member x.full 
        with get() =
            let mtString = sprintf "application/json; profile=\"urn:org.restfulobjects:repr-types/%s\"; charset=utf-8" m         
            let parmValue p = if s then sprintf "\"%s\"" p else sprintf "\"http://localhost/%s/%s\"" SegmentValues.DomainTypes  p
                    
            if d <> "" then       
                mtString + sprintf "; x-ro-domain-type=%s" (parmValue d)   
            else if e <> "" then             
                mtString + sprintf "; x-ro-element-type=%s" (parmValue e)
            else 
                mtString

    new (mt) = typeType(mt, "", "", false)
    new (mt, dt) = typeType(mt, dt, "", true)

    override x.Equals (o : obj) =
        x.full  = o.ToString() 
    override x.GetHashCode() = 
        x.full.GetHashCode()
    override x.ToString() = 
        x.full

type errorType (error : string) =
    let e = error 
    member x.getStart s =  e.Split([|" in "|], System.StringSplitOptions.None).[0]
    member x.toCompare with get() = x.getStart e
    override x.Equals (o : obj) =
        x.toCompare  = x.getStart (o.ToString())
    override x.GetHashCode() = 
        x.toCompare.GetHashCode()
    override x.ToString() = 
        x.toCompare

type TObject = 
    | TObjectJson of seq<TProp>
    | TObjectVal of obj 
    | TArray of seq<TObject> 
and TProp = TProperty of string * TObject

let makeProfile s = new StringSegment(sprintf "\"urn:org.restfulobjects:repr-types/%s\"" s)

let makeParm r n = sprintf ";%s=\"%s\"" r n 

let makeFriendly (s : string) = 
    let mutable newS = ""
    for c in s do  if Char.IsUpper(c) then  newS <- newS + " " + new string(c, 1)   else   newS <- newS + new string(c, 1)
    newS.Trim()

let wrap f (a, b, c, d) = f (a, c, d) 

let wrap1 f (a, b, c) = f (a, c) 

let wrap2 f (a, b, c, d, e) = f (a, c, d, e) 

let ComputeMD5HashFromString(s : string) = 
    let crypto = new System.Security.Cryptography.MD5CryptoServiceProvider()
    crypto.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s))

//let ComputeMD5HashAsString(s : string) = 
//    let hash = ComputeMD5HashFromString(s), 0
//    let i = BitConverter.ToInt64(hash)
//    let abs = Math.Abs(i)
//    abs.ToString(System.Globalization.CultureInfo.InvariantCulture)

let createTestHttpContext sp =
    let httpContext = new DefaultHttpContext() 
    httpContext.RequestServices <- sp
    httpContext.Response.Body <- new MemoryStream()
    httpContext

let setMockContext (api : RestfulObjectsControllerBase) sp = 
    let mockContext = new ControllerContext()
    let httpContext = createTestHttpContext sp
    mockContext.HttpContext <- httpContext
    api.ControllerContext <- mockContext
    api

let jsonSetMsgAndMediaType (msg : HttpRequest) url mt repType = 
    msg.Method <- HttpMethod.Get.ToString()
    let accept = new MediaTypeHeaderValue(new StringSegment(mt))
    if not (repType = "") then 
        accept.Parameters.Add(new NameValueHeaderValue(new StringSegment("profile"), (makeProfile repType)))
    msg.Headers.Add("Accept", new StringValues(accept.ToString()))
    let uri = new Uri(url)
    msg.Scheme <- "http"
    msg.Host <- new HostString(uri.Host)
    msg.Path <- new PathString(uri.PathAndQuery)

    //msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt))
    //msg

//let jsonGetMsgAndMediaType mt (url : string) = 
//    let message = new HttpRequest(HttpMethod.Get, url)
//    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt))
//    message

//let jsonGetMsg url = jsonGetMsgAndMediaType "application/json" url

let jsonSetMsg msg url = jsonSetMsgAndMediaType msg url "application/json" ""

let jsonSetMsgWithProfile msg url repType = jsonSetMsgAndMediaType msg url "application/json" repType

//let jsonGetMsgAndTag (url : string) tag = 
//    let message = jsonGetMsgAndMediaType "application/json" url
//    message.Headers.IfMatch.Add(new EntityTagHeaderValue(tag))
//    message

//let xmlGetMsg url =  jsonGetMsgAndMediaType "application/xml" url

let msgWithContent url content = 
    let message = new HttpRequestMessage()
    message.Content <- new StringContent(content)
    message.RequestUri <- new Uri(url)
    message.Content.Headers.ContentLength <- Nullable(int64(content.Length))
    message

//let msgWithoutContent url  = 
//    let message = new HttpRequestMessage()
//    message.RequestUri <- new Uri(url)
//    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"))
//    //message.Content.Headers.ContentLength <- Nullable(int64(0))
//    message

//let msgWithoutContentWithTag url tag  = 
//    let message = new HttpRequestMessage()
//    message.RequestUri <- new Uri(url)
//    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"))
//    message.Headers.IfMatch.Add(new EntityTagHeaderValue(tag))
//    //message.Content.Headers.ContentLength <- Nullable(int64(0))
//    message

//let jsonMsgWithContent url content = 
//    let message = msgWithContent url content 
//    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"))
//   // message.Content.Headers.ContentType <- new MediaTypeWithQualityHeaderValue("application/json")
//    message

//let jsonMsgWithContentAndTag url content tag = 
//    let message = msgWithContent url content 
//    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"))
//    message.Headers.IfMatch.Add(new EntityTagHeaderValue(tag))
//   // message.Content.Headers.ContentType <- new MediaTypeWithQualityHeaderValue("application/json")
//    message

//let xmlMsgWithContent url content = 
//    let message = msgWithContent url content 
//    message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"))
//    //message.Content.Headers.ContentType <- new MediaTypeWithQualityHeaderValue("application/xml")
//    message

//let jsonPostMsg url content = 
//    let message = jsonMsgWithContent url content
//    message.Method <- HttpMethod.Post
//    message

//let jsonPostMsgAndTag url content tag = 
//    let message = jsonMsgWithContentAndTag url content tag
//    message.Method <- HttpMethod.Post
//    message

//let jsonDeleteMsg url  = 
//    let message = msgWithoutContent url
//    message.Method <- HttpMethod.Delete
//    message

//let jsonDeleteMsgAndTag url tag = 
//    let message = msgWithoutContentWithTag url tag
//    message.Method <- HttpMethod.Delete
//    message

//let jsonPutMsg url content = 
//    let message = jsonMsgWithContent url content
//    message.Method <- HttpMethod.Put
//    message

//let jsonPutMsgAndTag url content tag = 
//    let message = jsonMsgWithContentAndTag url content tag
//    message.Method <- HttpMethod.Put
//    message

//let xmlPutMsg url content = 
//    let message = xmlMsgWithContent url content
//    message.Method <- HttpMethod.Put
//    message

//let xmlDeleteMsg url content = 
//    let message = xmlMsgWithContent url content
//    message.Method <- HttpMethod.Delete
//    message

//let xmlPostMsg url content = 
//    let message = xmlMsgWithContent url content
//    message.Method <- HttpMethod.Post
//    message

let readSnapshotToJson (ss : HttpResponseMessage) = 
    // ReasAsStringAsync seems to hang so need to do this ......
    use s = ss.Content.ReadAsStreamAsync().Result
    s.Position <- 0L
    use sr = new StreamReader(s)
    sr.ReadToEnd()

let readActionResult (ar : ActionResult) (hc : HttpContext) = 
    let testContext = new ActionContext()
    testContext.HttpContext <- hc
    ar.ExecuteResultAsync testContext |> Async.AwaitTask |> ignore
    use s = testContext.HttpContext.Response.Body
    s.Position <- 0L
    use sr = new StreamReader(s)
    let json = sr.ReadToEnd()
    let statusCode = testContext.HttpContext.Response.StatusCode
    let contentType = testContext.HttpContext.Response.ContentType
    let headers = testContext.HttpContext.Response.GetTypedHeaders()
    (json, statusCode, headers)

let comp (a : obj) (b : obj) e = 
    Assert.AreEqual(a, b, e)   

let listProperties (result : JObject) = 
    (result |> Seq.map (fun i -> (i :?> JProperty).Name) |> Seq.fold (fun a s ->  a + " " + s) "Properties: " )

let listExpected (expected : seq<TProp>) = 
    (expected |> Seq.map (fun r -> match r with | TProperty(s, _) -> s) |> Seq.fold (fun a s ->  a + " " + s) "Expected: " )

let rec compareArray (expected : seq<TObject>) (result : JArray) =
     
    let ps = if expected |> Seq.length <> result.Count then result.ToString() else ""
        
    Assert.AreEqual (expected |> Seq.length, result.Count, "Array - actual " + ps)
    result |> Seq.zip expected 
           |> Seq.iter (fun i ->  compare (fst(i)) (snd(i)) )     

and compareObject (expected : seq<TProp>) (result : JObject) = 
    
    Assert.AreEqual (expected |> Seq.length, result.Count, ((listProperties result) + " " + (listExpected expected)) )

    let orderedResult = result |> Seq.sortBy (fun i -> (i :?> JProperty).Name)
    
    orderedResult  |> Seq.map (fun i -> (i :?> JProperty).Name)  
                   |> Seq.zip (expected |> Seq.sortBy (fun r -> match r with | TProperty(s, _) -> s) |> Seq.map (fun r -> match r with | TProperty(s, _) -> s))                                                    
                   |> Seq.iter (fun i -> comp (fst(i)) (snd(i)) ((listProperties result) + " " + (listExpected expected)) ) 

    orderedResult  |> Seq.map (fun i -> (i :?> JProperty).Value)  
                   |> Seq.zip (expected |> Seq.sortBy (fun r -> match r with | TProperty(s, _) -> s) |> Seq.map (fun r -> match r with | TProperty(_, o) -> o))                                                    
                   |> Seq.iter (fun i ->  compare (fst(i)) (snd(i)) ) 

and compare expected (result : JToken) = 
    match expected  with 
    | TObjectJson(sq) -> compareObject sq (result :?> JObject )
    | TObjectVal(o) ->  Assert.AreEqual(o, (result :?> JValue).Value) 
    | TArray(sq)  -> compareArray sq  (result :?> JArray )    


let getIndent i = 
    let oneIndent = "  "
    let rec makeIndent indent count = 
        if count = 0 then indent else makeIndent (indent + oneIndent) (count - 1)
    makeIndent "" i 

let rec toStringArray (expected : seq<TObject>) indent : string =
    
    (getIndent indent) + "[\n" + ( expected |> Seq.fold (fun i j -> i + (toString j indent)) "") + "\n],"

and toStringProp (expected : TProp) indent : string = 

    match expected with 
    | TProperty(i, j) ->   (getIndent indent) +  i  + ": "  + (toString j indent) + ",\n"

and toStringObject (expected : seq<TProp>) indent : string = 

    (getIndent indent) +  "{\n" +   ( if expected = null then "null" else  expected |> Seq.fold (fun i j -> i + (toStringProp j (indent + 1) )) "") + "\n},"
   
and toString expected indent : string  = 
    match expected  with 
    | TObjectJson(sq) -> toStringObject sq (indent + 1)
    | TObjectVal(o) ->  if o = null then "null" else  o.ToString() 
    | TArray(sq)  -> toStringArray sq  (indent + 1)


let makeLinkPropWithMethodAndTypesValue (meth : string) (rel : string) href typ dTyp eTyp simple vObj = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp, eTyp, simple)));
       TProperty(JsonPropertyNames.Value, vObj);
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]


let makeLinkPropWithMethodAndTypes (meth : string) (rel : string) href typ dTyp eTyp simple = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp, eTyp, simple)));
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]

let makeLinkPropWithMethod (meth : string) (rel : string) href typ dTyp = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp)));
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]

let makeLinkPropWithMethodValue (meth : string) (rel : string) href typ dTyp vObj = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp)));
       TProperty(JsonPropertyNames.Value, vObj);
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]


let makeHref href =  [ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href))) ]

let makeGetLinkProp = makeLinkPropWithMethod "GET"

let makePutLinkProp = makeLinkPropWithMethod "PUT"

let makePostLinkProp = makeLinkPropWithMethod "POST"

let makeDeleteLinkProp = makeLinkPropWithMethod "DELETE"
  
let makeIconLink() = 
        [ TProperty(JsonPropertyNames.Rel, TObjectVal(RelValues.Icon) );
          TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType("images/Default.gif")) );
          TProperty(JsonPropertyNames.Type, TObjectVal("image/gif") );
          TProperty(JsonPropertyNames.Method, TObjectVal("GET") ) ]

let makeArgs parms =
    let getArg pmid = TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]));
    let argVals = parms |> Seq.map (fun i -> match i with | TProperty(s, _) -> s ) |> Seq.map (fun s -> getArg s)
    TObjectJson(argVals) 
  
let makeListParm pmid pid fid rt = 
      
        
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("list"))
                                                  TProperty(JsonPropertyNames.ElementType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)

let makeStringParm pmid pid fid rt = 
      
        
        let p = 
            TObjectJson([ TProperty
                              (JsonPropertyNames.Links, 
                               TArray([  ]))
                          TProperty(JsonPropertyNames.Extensions, 
                                    TObjectJson([ TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fid))
                                                  TProperty(JsonPropertyNames.Description, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rt))
                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0))
                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""))
                                                  TProperty(JsonPropertyNames.Format, TObjectVal("string"))
                                                  TProperty(JsonPropertyNames.Optional, TObjectVal(false)) ])) ])
        TProperty(pmid, p)


let p1 ms = makeListParm "acollection" "LocallyContributedAction" "Acollection" ms
let p2 ms = makeListParm "acollection" "LocallyContributedActionWithParm" "Acollection" ms
let p3 = makeStringParm "p1" "LocallyContributedActionWithParm" "P1" (ttc "string")
  
    
let makeActionMember oType  mName (oName : string) fName desc rType parms  =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 1 
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp


        let hParms = Seq.length parms > 0    
        let presHint = mName = "AnAction"
        let multiLine = mName = "AnActionReturnsNull"

        let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                        TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                        TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                        TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                        TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]

        let extArray = if presHint then  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class5 class6")) :: extArray else extArray

        let extArray = if multiLine then  TProperty(JsonPropertyNames.CustomMultipleLines, TObjectVal(1)) :: extArray else extArray

        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "");                                                      
                                                       ]))]

let makeActionMemberSimple oType  mName (oName : string) fName desc rType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 1 
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp


        let hParms = Seq.length parms > 0     
        let presHint = mName = "AnAction"
        let multiLine = mName = "AnActionReturnsNull"

        let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                        TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                        TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                        TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                        TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]

        let extArray = if presHint then  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class5 class6")) :: extArray else extArray

        let extArray = if multiLine then  TProperty(JsonPropertyNames.CustomMultipleLines, TObjectVal(1)) :: extArray else extArray


        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "") ]))]


let makeActionMemberString oType mName (oName : string) fName desc rType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 1   
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp


        let hParms = Seq.length parms > 0       
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                                                                  TProperty(JsonPropertyNames.Format, TObjectVal(rType));
                                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                                                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "");
                                                      
                                                      ]))]

let makeActionMemberNumber oType mName (oName : string) fName desc rType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 1   
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp


        let hParms = Seq.length parms > 0      
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                                                                  TProperty(JsonPropertyNames.Format, TObjectVal(rType));
                                                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "");
                                                      
                                                       ]))]


let makeActionMemberStringSimple oType mName (oName : string) fName desc rType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 1   
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp

        
        let hParms = Seq.length parms > 0     
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                                                                  TProperty(JsonPropertyNames.Format, TObjectVal(rType));
                                                                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                                                                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                                                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult ""); ]))]

let makeActionMemberNumberSimple oType mName (oName : string) fName desc rType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 1   
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp

        
        let hParms = Seq.length parms > 0     
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                                                                  TProperty(JsonPropertyNames.Format, TObjectVal(rType));                                                               
                                                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult ""); ]))]



let makeActionMemberWithType oType mName (oName : string) fName desc rType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index) 
        let order = if desc = "" then 0 else 1  
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp


        let hParms = Seq.length parms > 0      
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                                                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "");
                                                      
                                                       ]))]

let makeVoidActionMember oType mName (oName : string) fName desc parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index) 
        let order = if desc = "" then 0 else 1  
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp


        let hParms = Seq.length parms > 0       
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));                                             
                                                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");                                     
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "");
                                                       ]))]

let makeVoidActionMemberSimple oType mName (oName : string) fName desc parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 1   
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp

        
        let hParms = Seq.length parms > 0           
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));  
                                                                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));                                           
                                                                  TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "")]))]



let makeActionMemberCollection oType  mName (oName : string)  fName desc rType eType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 2    
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let hParms = Seq.length parms > 0    
        
        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp
        
        let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                        TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                        TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                        TProperty(JsonPropertyNames.ElementType, TObjectVal(eType));
                        TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                        TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                        TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]
        
        let exts = 
            if mName = "AnActionReturnsQueryable" then 
                let exts = TProperty(JsonPropertyNames.CustomTableViewTitle, TObjectVal(true)) :: extArray
                let exts = TProperty(JsonPropertyNames.CustomTableViewColumns, TArray([ TObjectVal("Id")])) :: exts
                let exts = TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) :: exts
                exts
            else 
                extArray
              
                     
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson(exts));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "");
                                                      
                                                       ]))]

let makeActionMemberCollectionSimple oType  mName (oName : string)  fName desc rType eType parms =
        let index = oName.IndexOf("/")
        let oTypeName =  if index = -1 then oName else  oName.Substring(0, index)
        let order = if desc = "" then 0 else 2       
        let detailsRelValue = RelValues.Details + makeParm RelParamValues.Action mName
        let invokeRelValue = RelValues.Invoke + makeParm RelParamValues.Action mName

        let makeLinkProp = if mName.Contains("Query") then makeGetLinkProp else if mName.Contains("Idempotent") then makePutLinkProp else makePostLinkProp
        let hParms = Seq.length parms > 0      

        let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                        TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                        TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                        TProperty(JsonPropertyNames.ElementType, TObjectVal(eType));
                        TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                        TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                        TProperty(JsonPropertyNames.HasParams, TObjectVal(hParms))]

        let exts = 
            if mName = "AnActionReturnsQueryable" then 
                let exts = TProperty(JsonPropertyNames.CustomTableViewTitle, TObjectVal(true)) :: extArray
                let exts = TProperty(JsonPropertyNames.CustomTableViewColumns, TArray([ TObjectVal("Id")])) :: exts
                let exts = TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) :: exts
                exts
            else 
                extArray


           
        [ TProperty(JsonPropertyNames.Parameters, TObjectJson(parms));
          TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Action) );
          TProperty(JsonPropertyNames.Id, TObjectVal(mName));
          TProperty(JsonPropertyNames.Extensions, TObjectJson(exts));
          TProperty(JsonPropertyNames.Links, TArray([ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/actions/%s" oType oName mName) RepresentationTypes.ObjectAction "");
                                                      TObjectJson( TProperty(JsonPropertyNames.Arguments, makeArgs parms)  :: makeLinkProp invokeRelValue (sprintf "%s/%s/actions/%s/invoke" oType oName mName) RepresentationTypes.ActionResult "");
 ]))]






let makePropertyMemberShort oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) (dValue : TProp list) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2  
      let conditionalChoices = mName.Contains("ConditionalChoices")
      let choices = mName.Contains("Choices") && (not conditionalChoices)
      let disabled = mName.Contains("Disabled") || (oTypeName.Contains("ViewModel") && (not ( oTypeName.Contains("Form") || oTypeName.Contains("Edit")    )))      
      let autocomplete = mName.Contains("AutoComplete")    
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName 

      let detailsLink = 
            if dValue |> Seq.isEmpty then 
                makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""
            else 
                makeLinkPropWithMethodValue "GET" detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "" (TObjectJson(dValue))

      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Clear + makeParm RelParamValues.Property mName
      let autoRel = RelValues.Prompt + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( detailsLink) ]

      let acLink = 
        if conditionalChoices then 
           
            
            let argP = TProperty(JsonPropertyNames.Arguments, TObjectJson( [TProperty("areference", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                                                                                 TProperty(JsonPropertyNames.Links, TArray([]))]))]))
            
            TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel (sprintf "%s/%s/properties/%s/prompt" oType oName mName) RepresentationTypes.Prompt "" "" true); 
        else   
            let argP = TProperty(JsonPropertyNames.Arguments, TObjectJson( [TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))
            let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson( [TProperty(JsonPropertyNames.MinLength, TObjectVal(2))]))
            TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel (sprintf "%s/%s/properties/%s/prompt" oType oName mName) RepresentationTypes.Prompt "" "" true);
   
      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt && not disabled then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()
      let links = if autocomplete || conditionalChoices then (Seq.append links [acLink]).ToList() else links.ToList()

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
                    TProperty(JsonPropertyNames.Id, TObjectVal(mName));
                    TProperty(JsonPropertyNames.Value, oValue);
                    TProperty(JsonPropertyNames.HasChoices, TObjectVal(choices));                                        
                    TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                         TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                         TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                                                                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                                         TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
                    TProperty(JsonPropertyNames.Links, TArray(links))]
      

      if choices then 
        let mst = ttc "RestfulObjects.Test.Data.MostSimple"
        let choiceRel = RelValues.Choice + makeParm RelParamValues.Property mName
             
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1"))  RepresentationTypes.Object mst
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2"))  RepresentationTypes.Object mst

        TProperty(JsonPropertyNames.Choices, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) :: props;
      else
        props; 

let makePropertyMemberShortNoDetails oType (mName : string) (oTypeName : string) fName desc rType opt (oValue : TObject) args =
      let order = if desc = "" then 0 else 2 
      let conditionalChoices = mName.Contains("ConditionalChoices")
      let choices = mName.Contains("Choices") && (not conditionalChoices)
      let disabled = mName.Contains("Disabled") || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))            
      let autocomplete = mName.Contains("AutoComplete")
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let autoRel = RelValues.Prompt + makeParm RelParamValues.Property mName

      let acLink = 
        if conditionalChoices then 
           
            
            let argP = TProperty(JsonPropertyNames.Arguments, TObjectJson( [ args
                                                                             TProperty("areference", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                                                                                 TProperty(JsonPropertyNames.Links, TArray([]))]))]))
            
            TObjectJson(argP :: makeLinkPropWithMethodAndTypes "PUT" autoRel (sprintf "%s/%s/properties/%s/prompt" oType oTypeName  mName) RepresentationTypes.Prompt "" "" true); 
        else   
            let argP = TProperty(JsonPropertyNames.Arguments, TObjectJson( [args
                                                                            TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))
            let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson( [TProperty(JsonPropertyNames.MinLength, TObjectVal(2))]))
            TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "PUT" autoRel (sprintf "%s/%s/properties/%s/prompt" oType oTypeName mName) RepresentationTypes.Prompt "" "" true);


      let links = [ ]
      let links = if autocomplete || conditionalChoices then (Seq.append links [acLink]).ToList() else links.ToList()
  

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
                    TProperty(JsonPropertyNames.Id, TObjectVal(mName));
                    TProperty(JsonPropertyNames.Value, oValue);
                    TProperty(JsonPropertyNames.HasChoices, TObjectVal(choices));
                    
                    TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                                         TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                                         TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                                                                         TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                                         TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
                    TProperty(JsonPropertyNames.Links, TArray(links))]

      if choices then 
        let mst = ttc "RestfulObjects.Test.Data.MostSimple"
        let choiceRel = RelValues.Choice + makeParm RelParamValues.Property mName
             
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1"))  RepresentationTypes.Object mst
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2"))  RepresentationTypes.Object mst

        TProperty(JsonPropertyNames.Choices, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) :: props;
      else
        props;                                                       

let makePropertyMemberFullAttachment mName  (oName : string) fName title mt =
      let oType = "objects"
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName 
      let attachRelValue = RelValues.Attachment + makeParm RelParamValues.Property mName 

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(""));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                      TProperty(JsonPropertyNames.Format, TObjectVal("blob"));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                      TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                      TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                      TProperty(JsonPropertyNames.Optional, TObjectVal(false))];

      let exts = TObjectJson(extArray);

      let attLink = [ TProperty(JsonPropertyNames.Title, TObjectVal(title));
                      TProperty(JsonPropertyNames.Rel, TObjectVal(attachRelValue));
                      TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(sprintf "%s/%s/properties/%s" oType oName mName)));
                      TProperty(JsonPropertyNames.Type, TObjectVal(mt));
                      TProperty(JsonPropertyNames.Method, TObjectVal("GET")) ]


      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                    TObjectJson( attLink);
                   ]

  
      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));                   
        TProperty(JsonPropertyNames.Extensions, exts);
        TProperty(JsonPropertyNames.Links, TArray(links))]


let makePropertyMemberFull oType mName  (oName : string) fName desc opt (oValue : TObject) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2 
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName 
      let conditionalChoices = mName.Contains("ConditionalChoices")
      let choices = mName.Contains("Choices") && not conditionalChoices
      let disabled = mName.Contains("Disabled") || (oTypeName.Contains("ViewModel") && (not ( oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit") ))) 
      let autocomplete = mName.Contains("AutoComplete")
      let presHint = mName = "AValue"

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                      TProperty(JsonPropertyNames.Format, TObjectVal("int"));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.Optional, TObjectVal(opt))];

      let extArray = if choices then TProperty(JsonPropertyNames.CustomChoices, TObjectJson([TProperty("1", TObjectVal(1)); TProperty("2", TObjectVal(2)); TProperty("3", TObjectVal(3))])) :: extArray else extArray;
      
      let extArray = if presHint then TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class3 class4")) :: extArray else extArray

      let exts = TObjectJson(extArray);

      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Clear + makeParm RelParamValues.Property mName
      let autoRel = RelValues.Prompt + makeParm RelParamValues.Property mName

      let acLink = 
        if conditionalChoices then 
           
            
            let argP = TProperty(JsonPropertyNames.Arguments, TObjectJson( [TProperty("avalue", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                                                                                 TProperty(JsonPropertyNames.Links, TArray([]))]));
                                                                            TProperty("astringvalue", TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null));
                                                                                                                 TProperty(JsonPropertyNames.Links, TArray([]))]))
                                                                                                                 ]))
            
            TObjectJson(argP :: makeLinkPropWithMethodAndTypes "GET" autoRel (sprintf "%s/%s/properties/%s/prompt" oType oName mName) RepresentationTypes.Prompt "" "" true); 
        else   
            let argP = TProperty(JsonPropertyNames.Arguments, TObjectJson( [TProperty(JsonPropertyNames.XRoSearchTerm, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))]))]))
            let extP = TProperty(JsonPropertyNames.Extensions, TObjectJson( [TProperty(JsonPropertyNames.MinLength, TObjectVal(0))]))
            TObjectJson(argP :: extP :: makeLinkPropWithMethodAndTypes "GET" autoRel (sprintf "%s/%s/properties/%s/prompt" oType oName mName) RepresentationTypes.Prompt "" "" true);


      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                    ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()                             
      let links = if opt then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()
      let links = if autocomplete || conditionalChoices then (Seq.append links [acLink]).ToList() else links.ToList()

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
                    TProperty(JsonPropertyNames.Id, TObjectVal(mName));
                    TProperty(JsonPropertyNames.Value, oValue);
                    TProperty(JsonPropertyNames.HasChoices, TObjectVal(choices));                   
                    TProperty(JsonPropertyNames.Extensions, exts);
                    TProperty(JsonPropertyNames.Links, TArray(links))]

       
      if choices then       
        TProperty(JsonPropertyNames.Choices, TArray([ TObjectVal(1); TObjectVal(2); TObjectVal(3) ])) :: props;
      else
        props;

let makeTablePropertyMember (mName : string) (oValue : TObject) =
     
    
      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(mName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(""));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                      TProperty(JsonPropertyNames.Format, TObjectVal("int"));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                      TProperty(JsonPropertyNames.Optional, TObjectVal(false))];
     
      let exts = TObjectJson(extArray);
      
      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.Extensions, exts);
        TProperty(JsonPropertyNames.Links, TArray([  ]))]

let makeNoDetailsPropertyMember (mName : string)  (oName : string) (oValue : TObject) =
     
    
      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(mName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(""));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                      TProperty(JsonPropertyNames.Format, TObjectVal("int"));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));
                      TProperty(JsonPropertyNames.Optional, TObjectVal(false))];
     
      let exts = TObjectJson(extArray);
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" "objects" oName mName) RepresentationTypes.ObjectProperty "");]
   
      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.Extensions, exts);
        TProperty(JsonPropertyNames.Links, TArray(links))]

let makePropertyMemberFullNoDetails oType (mName : string) (oTypeName : string) fName desc opt (oValue : TObject) =
      let order = if desc = "" then 0 else 2 
      let choices = mName.Contains("Choices")
      let disabled = mName.Contains("Disabled")  || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))       
      let autocomplete = mName.Contains("AutoComplete")

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                      TProperty(JsonPropertyNames.Format, TObjectVal("int"));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.Optional, TObjectVal(opt))];

      let extArray = if choices then TProperty(JsonPropertyNames.CustomChoices, TObjectJson([TProperty("Value1", TObjectVal(0)); TProperty("Value2", TObjectVal(1))])) :: extArray else extArray;
      
      let exts = TObjectJson(extArray);
      
      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(choices));
      
        TProperty(JsonPropertyNames.Extensions, exts);
        TProperty(JsonPropertyNames.Links, TArray([  ]))]

let makePropertyMemberGuid oType (oName : string) (mName : string) (oValue : TObject) tName =
     
            
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                     ]

      let links =  (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal("Id"));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
      
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal("Id"));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(""));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal(tName));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));                                                   
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(false))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]

let makePropertyMemberDateTime oType (mName : string) (oName : string) fName desc opt (oValue : TObject) format =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 4
      let disabled = mName.Contains("Disabled") || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))        
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Clear + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                     ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt && not disabled then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
      
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                                                             TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                                                             TProperty(JsonPropertyNames.Format, TObjectVal(format));
                                                             TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"));
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]

let makePropertyMemberTimeSpan oType (mName : string) (oName : string) fName desc opt (oValue : TObject) format =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 5
      let disabled = mName.Contains("Disabled") || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit") )))        
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Clear + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                     ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt && not disabled then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
      
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                                                             TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                                                             TProperty(JsonPropertyNames.Format, TObjectVal(format));
                                                             TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.CustomMask, TObjectVal("d"));
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]


let makePropertyMemberString oType (mName : string) (oName : string) fName desc opt (oValue : TObject) (dValue : TProp list) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 3
      let disabled = mName.Contains("Disabled")    || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))       
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Clear + makeParm RelParamValues.Property mName
      let detailsLink = 
            if dValue |> Seq.isEmpty then 
                makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""
            else 
                makeLinkPropWithMethodValue "GET" detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "" (TObjectJson(dValue))

      let links = [ TObjectJson( detailsLink)  ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt && not disabled then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
       
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                                                             TProperty(JsonPropertyNames.MaxLength, TObjectVal(101));
                                                             TProperty(JsonPropertyNames.Pattern, TObjectVal("[A-Z]"));
                                                             TProperty(JsonPropertyNames.Format, TObjectVal("string"));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]

let makePropertyMemberWithType oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 3
      let disabled = mName.Contains("Disabled")   || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))        
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Clear + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                    ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
       
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]

let makePropertyMemberWithNumber oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 3
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let choices = mName.Contains("Choices")
      let disabled = mName.Contains("Disabled")  || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))          
      let autocomplete = mName.Contains("AutoComplete")
      let range = mName.Contains("Range")

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                              TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                              TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                              TProperty(JsonPropertyNames.Format, TObjectVal(rType));
                              TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                              TProperty(JsonPropertyNames.Optional, TObjectVal(opt))];

      let extArray = if choices then TProperty(JsonPropertyNames.CustomChoices, TObjectJson([TProperty("Value1", TObjectVal(0)); TProperty("Value2", TObjectVal(1))])) :: extArray else extArray;
      
      let extArray = if mName = "EnumByAttributeChoices" then  TProperty(JsonPropertyNames.CustomDataType, TObjectVal("custom")) :: extArray else extArray

      let extArray = if range then TProperty(JsonPropertyNames.CustomRange, TObjectJson([TProperty("min", TObjectVal(0)); TProperty("max", TObjectVal(400))])) :: extArray else extArray

      let exts = TObjectJson(extArray);
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Clear + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                     ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
                    TProperty(JsonPropertyNames.Id, TObjectVal(mName));
                    TProperty(JsonPropertyNames.Value, oValue);
                    TProperty(JsonPropertyNames.HasChoices, TObjectVal(choices));                   
                    TProperty(JsonPropertyNames.Extensions, exts);
                    TProperty(JsonPropertyNames.Links, TArray(links))]

      if choices then        
        TProperty(JsonPropertyNames.Choices, TArray([ TObjectVal(0); TObjectVal(1); ])) :: props;
      else
        props; 



      
let makePropertyMemberWithFormat oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 3
      let disabled = mName.Contains("Disabled")    || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit")))) 
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                    ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      let exts = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                  TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                  TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                  TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                  TProperty(JsonPropertyNames.Format, TObjectVal(rType));
                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                  TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]


      let exts = if mName = "Password" then  TProperty(JsonPropertyNames.CustomDataType, TObjectVal("password")) :: exts else exts


      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
        
        TProperty(JsonPropertyNames.Extensions, TObjectJson(exts));
        TProperty(JsonPropertyNames.Links, TArray(links))]



let makePropertyMemberWithTypeNoValue oType (mName : string) (oName : string) fName desc rType opt =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 3
      let disabled = mName.Contains("Disabled")  || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))         
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "") ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()                                                                                  
      let links = if opt then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()


      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
      
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                                                             TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                                                             TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                                                             TProperty(JsonPropertyNames.Format, TObjectVal(rType));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]





let makePropertyMemberSimple oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
      let order = if desc = "" then 0 else 3
      let disabled = mName.Contains("Disabled")   || (oName.Contains("ViewModel") && (not (oName.Contains("FormViewModel")))) 
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
       
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]

let makePropertyMemberSimpleNumber oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
      let order = if desc = "" then 0 else 3
      let disabled = mName.Contains("Disabled")   || (oName.Contains("ViewModel") && (not (oName.Contains("FormViewModel"))))        
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "") ]

      let links = if disabled then links.ToList() else (Seq.append links [(TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""))]).ToList()               
      let links = if opt then (Seq.append links [TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "")]).ToList() else links.ToList()

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));
       
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal("number"));
                                                             TProperty(JsonPropertyNames.Format, TObjectVal(rType));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]



let makePropertyMemberNone oType mName (oName : string)  (oValue : TObject) =
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName  
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                    TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""); ]

      //let links = if opt then TObjectJson(makeDeleteLinkProp clearRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "") :: links else links

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));    
        TProperty(JsonPropertyNames.Extensions, TObjectJson([]));
        TProperty(JsonPropertyNames.Links, TArray(links))]


let makePropertyMember oType mName oName fName (oValue : TObject) = makePropertyMemberFull "objects" mName oName fName "" false oValue 








let makeCollectionMemberNoDetails mName oName fName desc =
      let order = if desc = "" then 0 else 2
      let mst = ttc "RestfulObjects.Test.Data.MostSimple"
      let valueRelValue = RelValues.CollectionValue + makeParm RelParamValues.Collection mName
      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Collection) );
        TProperty(JsonPropertyNames.Id, TObjectVal( mName) );
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
        TProperty(JsonPropertyNames.Extensions, TObjectJson([TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                                                             TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal(ResultTypes.List));
                                                             TProperty(JsonPropertyNames.PluralName, TObjectVal("Most Simples"));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                                                             TProperty(JsonPropertyNames.ElementType, TObjectVal(mst))]));
        TProperty(JsonPropertyNames.Links, TArray ([ TObjectJson( makeLinkPropWithMethodAndTypes "GET" valueRelValue (sprintf "objects/%s/collections/%s/value" oName mName) RepresentationTypes.CollectionValue "" "" true)  ]))]


let makeCollectionMemberNoValue mName (oName : string) fName desc rType size cType cText =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2
     
      let presHint = mName = "ACollection"
      let renderEagerly = mName = "AnEagerCollection"

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                      TProperty(JsonPropertyNames.PluralName, TObjectVal(cText));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.ElementType, TObjectVal(cType))]

      let extArray = if presHint then  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8")) :: extArray else extArray
      let extArray = if renderEagerly then  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) :: extArray else extArray


      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Collection) );
        TProperty(JsonPropertyNames.Id, TObjectVal( mName) );
        TProperty(JsonPropertyNames.Size, TObjectVal( size) );
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
        TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
        TProperty(JsonPropertyNames.Links, TArray ([  ]))]




let makeServiceActionMember mName oName rType parms = makeActionMember "services" mName oName (makeFriendly(mName)) "" rType parms

let makeServiceActionMemberSimple mName oName rType parms = makeActionMemberSimple "services" mName oName (makeFriendly(mName)) "" rType parms

let makeObjectActionMember mName oName rType parms = makeActionMember "objects" mName oName (makeFriendly(mName)) "" rType parms

let makeObjectActionMemberSimple mName oName rType parms = makeActionMemberSimple "objects" mName oName (makeFriendly(mName)) "" rType parms

let makeObjectActionCollectionMember mName oName eType parms = makeActionMemberCollection "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType parms

let makeObjectActionCollectionMemberSimple mName oName eType parms  = makeActionMemberCollectionSimple "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType parms

let makeObjectActionCollectionMemberNoParms mName oName eType  = makeActionMemberCollection "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType []

let makeObjectActionCollectionMemberNoParmsSimple mName oName eType  = makeActionMemberCollectionSimple "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType []

let membersProp (oName : string, oType : string) = 
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("LocallyContributedAction", TObjectJson(makeObjectActionCollectionMember "LocallyContributedAction" oName oType [ p1 oType ]))
                                  TProperty("LocallyContributedActionWithParm", TObjectJson(makeObjectActionCollectionMember "LocallyContributedActionWithParm" oName oType [ p2 oType; p3 ])) ]))



let makeCollectionMemberTypeValue mName (oName : string) fName desc rType size cType cName cValue (dValue : TProp list) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Collection mName

      let detailsLink = makeLinkPropWithMethodAndTypesValue "GET" detailsRelValue (sprintf "objects/%s/collections/%s" oName mName) RepresentationTypes.ObjectCollection "" cType true (TObjectJson(dValue))
      let renderEagerly = mName = "AnEagerCollection"

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.PluralName, TObjectVal(cName));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                      TProperty(JsonPropertyNames.ElementType, TObjectVal(cType))]

      let extArray = if renderEagerly then  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) :: extArray else extArray

      let members = mName = "ACollection"

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Collection) );
                    TProperty(JsonPropertyNames.Id, TObjectVal( mName) );
                    TProperty(JsonPropertyNames.Size, TObjectVal( size) );
                    TProperty(JsonPropertyNames.Value, cValue );
                    TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                    TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
                    TProperty(JsonPropertyNames.Links, TArray ([ TObjectJson( detailsLink);
                                                                  ]))]

      let props = if members then membersProp(oName, cType) :: props else props
      props
let makeCollectionMemberWithValue mName (oName : string) fName desc rType values size cType cText =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2
     
      let presHint = mName = "ACollection"
      let renderEagerly = mName = "AnEagerCollection"

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                      TProperty(JsonPropertyNames.PluralName, TObjectVal(cText));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.ElementType, TObjectVal(cType))]

      let extArray = if presHint then  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8")) :: extArray else extArray
      let extArray = if renderEagerly then  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) :: extArray else extArray

     
    
      
    

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Collection) );
                    TProperty(JsonPropertyNames.Id, TObjectVal( mName) );
                    TProperty(JsonPropertyNames.Size, TObjectVal( size) );
                    TProperty(JsonPropertyNames.Value, values);
                    TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                    TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
                    TProperty(JsonPropertyNames.Links, TArray ([  ]))]

      
      props 


let makeCollectionMemberSimpleType mName (oName : string) fName desc rType size cType cName value =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Collection mName
      let valueRelValue = RelValues.CollectionValue + makeParm RelParamValues.Collection mName

      let presHint = mName = "ACollection"
      let renderEagerly = mName = "AnEagerCollection"

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.PluralName, TObjectVal(cName));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                      TProperty(JsonPropertyNames.ElementType, TObjectVal(cType))]

      let extArray = if presHint then  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8")) :: extArray else extArray
      let extArray = if renderEagerly then  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) :: extArray else extArray

     

     

      let props =  [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Collection) );
                     TProperty(JsonPropertyNames.Id, TObjectVal( mName) );
                     TProperty(JsonPropertyNames.Size, TObjectVal( size) );
                     TProperty(JsonPropertyNames.Value, value );
                     TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                     TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
                     TProperty(JsonPropertyNames.Links, TArray ([ TObjectJson( makeLinkPropWithMethodAndTypes "GET" valueRelValue (sprintf "objects/%s/collections/%s/value" oName mName) RepresentationTypes.CollectionValue "" "" true);
                                                                     TObjectJson( makeLinkPropWithMethodAndTypes "GET" detailsRelValue (sprintf "objects/%s/collections/%s" oName mName) RepresentationTypes.ObjectCollection "" cType true) ]))]
      let members = mName = "ACollection"
      
      let props = if members then membersProp(oName, cType) :: props else  props
      props 

let makeCollectionMemberSimpleTypeValue mName (oName : string) fName desc rType size cType cName cValue (dValue : TProp list) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Collection mName
     
      let detailsLink = makeLinkPropWithMethodAndTypesValue "GET" detailsRelValue (sprintf "objects/%s/collections/%s" oName mName) RepresentationTypes.ObjectCollection "" cType true (TObjectJson(dValue))
      let renderEagerly = mName = "AnEagerCollection"

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.PluralName, TObjectVal(cName));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                      TProperty(JsonPropertyNames.ElementType, TObjectVal(cType))]

      let extArray = if renderEagerly then  TProperty(JsonPropertyNames.CustomRenderEagerly, TObjectVal(true)) :: extArray else extArray

  


      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Collection) );
                    TProperty(JsonPropertyNames.Id, TObjectVal( mName) );
                    TProperty(JsonPropertyNames.Size, TObjectVal( size) );
                    TProperty(JsonPropertyNames.Value, cValue );
                    TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                    TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
                    TProperty(JsonPropertyNames.Links, TArray ([ TObjectJson(detailsLink) ]))]

     

      let members = mName = "ACollection"

      let props = if members then membersProp(oName, cType) :: props else props
      props

let makeCollectionMemberSimple mName (oName : string) fName desc rType size value = makeCollectionMemberSimpleType mName oName fName desc rType size (ttc "RestfulObjects.Test.Data.MostSimple") "Most Simples" value

let makeCollectionMemberType mName (oName : string) fName desc rType size cType cName value=
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Collection mName
      let valueRelValue = RelValues.CollectionValue + makeParm RelParamValues.Collection mName
      let presHint = mName = "ACollection"

      let extArray = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                      TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                      TProperty(JsonPropertyNames.PluralName, TObjectVal(cName));
                      TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                      TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                      TProperty(JsonPropertyNames.ElementType, TObjectVal(cType))]

      let extArray = if presHint then  TProperty(JsonPropertyNames.PresentationHint, TObjectVal("class7 class8")) :: extArray else extArray

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Collection) );
                    TProperty(JsonPropertyNames.Id, TObjectVal( mName) );
                    TProperty(JsonPropertyNames.Size, TObjectVal( size) );
                    TProperty(JsonPropertyNames.Value, value);
                    TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
                    TProperty(JsonPropertyNames.Extensions, TObjectJson(extArray));
                    TProperty(JsonPropertyNames.Links, TArray ([
                                                                 TObjectJson( makeLinkPropWithMethodAndTypes "GET" valueRelValue (sprintf "objects/%s/collections/%s/value" oName mName) RepresentationTypes.CollectionValue "" "" true);
                                                                 TObjectJson( makeLinkPropWithMethodAndTypes "GET" detailsRelValue (sprintf "objects/%s/collections/%s" oName mName) RepresentationTypes.ObjectCollection "" cType true);
                                                                  ]))]
      let members = mName = "ACollection";
      
      let props = if members then membersProp(oName, cType) :: props else  props

      props



let makeCollectionMember mName (oName : string) fName desc rType size value = makeCollectionMemberType mName (oName : string) fName desc rType size (ttc "RestfulObjects.Test.Data.MostSimple") "Most Simples" value

let makeObjectActionVoidMember mName oName  = makeVoidActionMember "objects" mName oName (makeFriendly(mName)) ""  []

let makeObjectActionVoidMemberSimple mName oName  = makeVoidActionMemberSimple "objects" mName oName (makeFriendly(mName)) ""  []

let makeServiceActionMemberNoParms mName oName rType  = makeActionMember "services" mName oName (makeFriendly(mName)) "" rType []

let makeServiceActionMemberNoParmsSimple mName oName rType  = makeActionMemberSimple "services" mName oName (makeFriendly(mName)) "" rType []

let makeObjectActionMemberNoParms mName oName rType  = makeActionMember "objects" mName oName (makeFriendly(mName)) "" rType []

let makeObjectActionMemberNoParmsSimple mName oName rType  = makeActionMemberSimple "objects" mName oName (makeFriendly(mName)) "" rType []

let makeObjectPropertyMember mName oName fName (oValue : TObject) =  makePropertyMember "objects" mName oName fName oValue 

let makeObjectPropertyMemberWithDesc mName oName fName desc opt (oValue : TObject) =  makePropertyMemberFull "objects" mName oName fName desc opt oValue 

let makeServiceActionCollectionMember mName oName eType parms = makeActionMemberCollection "services" mName oName (makeFriendly(mName)) "" ResultTypes.List eType parms

let makeServiceActionCollectionMemberNoParms mName oName eType  = makeActionMemberCollection "services" mName oName (makeFriendly(mName)) "" ResultTypes.List eType []

let makeServiceActionVoidMember mName oName  = makeVoidActionMember "services" mName oName (makeFriendly(mName)) ""  []

let assertTransactionalCache  (message : HttpResponseMessage) = 
    Assert.AreEqual(true, message.Headers.CacheControl.NoCache)
    Assert.AreEqual("no-cache", message.Headers.Pragma.ToString())
    Assert.AreEqual(message.Headers.Date, message.Content.Headers.Expires)

let assertConfigCache secs  (message : HttpResponseMessage) = 
    let ts = new TimeSpan(0, 0, secs)
    Assert.AreEqual(ts, message.Headers.CacheControl.MaxAge)
    Assert.IsTrue(message.Headers.Date.HasValue)
    let expire = message.Headers.Date.Value + ts
    Assert.AreEqual(expire, message.Content.Headers.Expires)

let assertUserInfoCache (headers : Headers.ResponseHeaders) = 
    Assert.AreEqual(oneHour, headers.CacheControl.MaxAge)
    Assert.IsTrue(headers.Date.HasValue)
    let expire = headers.Date.Value + oneHour
    Assert.AreEqual(expire, headers.Expires)

let assertNonExpiringCache (headers : Headers.ResponseHeaders) = 
    Assert.AreEqual(oneDay, headers.CacheControl.MaxAge)
    Assert.IsTrue(headers.Date.HasValue)
    let expire = headers.Date.Value + oneDay
    Assert.AreEqual(expire, headers.Expires)

let assertStatusCode (sc : HttpStatusCode) iSc msg= 
    Assert.AreEqual((int)sc, iSc, msg)


let CreateSingleValueArg (m : JObject) = ModelBinderUtils.CreateSingleValueArgument(m, false)
     
let CreateArgMap (m : JObject) = ModelBinderUtils.CreateArgumentMap(m, false)

//let CreateReservedArgs (s : string) = ModelBinderUtils.CreateReservedArguments(s)
   
let CreateArgMapFromUrl (s : string) = ModelBinderUtils.CreateSimpleArgumentMap(s)

let CreatePersistArgMap (m : JObject) = ModelBinderUtils.CreatePersistArgMap(m, false)

let GetDomainType (m : JObject) = 
  
    let dm = m.["x-ro-domain-model"]
    let dt = m.["domainType"]
    let lks = m.["links"]
   
    let dmm = if dm = null then null else (dm :?> JValue).Value   :?> string
 
    if (dt <> null && dmm <> "formal") then 
        if dt = null then "" else (dt :?> JValue).Value   :?> string
    else if (lks <> null && dmm <> "simple" ) then 
        let rel = ((lks.First.["rel"] :?> JValue).Value :?> string)
        if (rel <> "describedby") then raise ( new Exception())
        let href = lks.First.["href"]
        let dtRef = (href :?> JValue).Value :?> string
        if dtRef.StartsWith("http://localhost/domain-types/") then 
            let ss = dtRef.Substring("http://localhost/domain-types/".Length )
            ss
        else 
            ""
    else 
        ""
               
let invokeRelTypeSb = RelValues.Invoke + makeParm RelParamValues.TypeAction "isSubtypeOf"
let invokeRelTypeSp = RelValues.Invoke + makeParm RelParamValues.TypeAction "isSupertypeOf"

let sb (oType : string) = 
    TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
    :: (TProperty
           (JsonPropertyNames.Arguments, 
            TObjectJson
                ([ TProperty
                       (JsonPropertyNames.SuperType, 
                        TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])) 
       :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
              RepresentationTypes.TypeActionResult "")

let sp (oType : string) = 
    TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
    :: (TProperty
           (JsonPropertyNames.Arguments, 
            TObjectJson
                ([ TProperty
                       (JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])) 
       :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
              RepresentationTypes.TypeActionResult "")
   