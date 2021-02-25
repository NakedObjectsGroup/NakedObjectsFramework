// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.Rest.Test.Functions

open System
open System.Net
open System.Net.Http
open System.IO
open Newtonsoft.Json.Linq
open NakedFramework.Rest.Snapshot.Constants
open NUnit.Framework
open System.Linq
open NakedFramework.Rest.Model
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Microsoft.AspNetCore.Mvc
open Microsoft.Net.Http.Headers
open System.Collections.Generic
open NakedFramework.Rest.API

let internal mapCodeToType (code : string) : string = code
let internal mapTypeToCode (typ : string) : string = typ

let internal mapCodeToKey (code : string) : string = code
let internal mapKeyToCode (key : string) : string = key

let mutable ctt = mapCodeToType
let mutable ttc = mapTypeToCode

let mutable ctk = mapCodeToKey
let mutable ktc = mapKeyToCode

let internal testRoot = "http://localhost/"

let internal oneDay = new TimeSpan(1,0,0,0)

let internal oneHour = new TimeSpan(1,0,0)

let internal unprocessableEntity = box(422) :?> System.Net.HttpStatusCode

let internal preconditionHeaderMissing = box(428) :?> System.Net.HttpStatusCode

type internal  partCmp (s : string) =
    let partS = s 
    override x.Equals (o : obj) =
        partS.StartsWith(o.ToString()) 
    override x.GetHashCode() = 
        partS.GetHashCode()
    override x.ToString() = 
        partS

type internal hrefType (href : string) =
    let h = href 
    member x.full with get() = sprintf "http://localhost/%s" h  
    member x.fullWithPort with get() = sprintf "http://localhost:80/%s" h  // temp hack for json.net bug ? 
    override x.Equals (o : obj) =
        (x.full  = o.ToString()) || (x.fullWithPort = o.ToString())
    override x.GetHashCode() = 
        x.full.GetHashCode()
    override x.ToString() = 
        x.full
        
type internal typeType (mt, dt, et, simple) =
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

type internal errorType (error : string) =
    let e = error 
    member x.getStart _ =  e.Split([|" in "|], System.StringSplitOptions.None).[0]
    member x.toCompare with get() = x.getStart e
    override x.Equals (o : obj) =
        x.toCompare  = x.getStart (o.ToString())
    override x.GetHashCode() = 
        x.toCompare.GetHashCode()
    override x.ToString() = 
        x.toCompare

type internal TObject = 
    | TObjectJson of seq<TProp>
    | TObjectVal of obj 
    | TArray of seq<TObject> 
and internal TProp = TProperty of string * TObject

let internal makeProfile s = new StringSegment(sprintf "\"urn:org.restfulobjects:repr-types/%s\"" s)

let internal makeParm r n = sprintf ";%s=\"%s\"" r n 

let internal makeFriendly (s : string) = 
    let mutable newS = ""
    for c in s do  if Char.IsUpper(c) then  newS <- newS + " " + new string(c, 1)   else   newS <- newS + new string(c, 1)
    newS.Trim()

let internal wrap f (a, _, c) = f (a, c) 

let internal wrap2 f (a, _, c, d, e) = f (a, c, d, e) 

let internal wrap3 f (a, _, c, d) = f (a, c, d) 

let internal ComputeMD5HashFromString(s : string) = 
    let crypto = new System.Security.Cryptography.MD5CryptoServiceProvider()
    crypto.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s))

let internal createTestHttpContext sp =
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

let internal setContent (msg : HttpRequest) (content : string) = 
    msg.Body <- new MemoryStream()
    use writer = new StreamWriter(msg.Body)
    writer.Write(content)
    msg.Body.Position <- 0L

let internal setMethod (msg : HttpRequest) (method : HttpMethod) = 
    msg.Method <- method.ToString()

let internal setMediaType (msg : HttpRequest) mt parms = 
    let accept = new MediaTypeHeaderValue(new StringSegment(mt))
    for p in parms do
        let (name, value) = p
        let ssName = new StringSegment(name)
        let ssValue = if (name = "profile") then (makeProfile value) else new StringSegment(value)
        accept.Parameters.Add(new NameValueHeaderValue(ssName, ssValue))
    let acceptValue = new StringValues(accept.ToString())
    if (msg.Headers.ContainsKey("Accept")) then 
        msg.Headers.["Accept"] <- acceptValue
    else 
        msg.Headers.Add("Accept", acceptValue)
 
let internal setUrl (msg : HttpRequest) url =
    let uri = new Uri(url)
    msg.Scheme <- "http"
    msg.Host <- new HostString(uri.Host)
    msg.Path <- new PathString(uri.PathAndQuery)

let internal jsonSetGetMsgAndMediaType (msg : HttpRequest) url mt parms = 
    setMethod msg HttpMethod.Get
    setMediaType msg mt parms 
    setUrl msg url

let internal jsonSetDeleteMsgAndMediaType (msg : HttpRequest) url mt parms = 
    setMethod msg HttpMethod.Delete
    setMediaType msg mt parms 
    setUrl msg url

let internal jsonSetPotentMsgAndMediaType (msg : HttpRequest) method url mt parms content = 
    setMethod msg method
    setMediaType msg mt parms 
    setContent msg content
    setUrl msg url

let internal jsonSetGetMsg msg url = jsonSetGetMsgAndMediaType msg url "application/json" []

let internal jsonSetGetMsgWithMediaType msg url mt = jsonSetGetMsgAndMediaType msg url mt []

let internal jsonSetGetMsgWithProfile msg url profVal = jsonSetGetMsgAndMediaType msg url "application/json" [("profile", profVal)]

let internal jsonSetEmptyPostMsg msg url = jsonSetPotentMsgAndMediaType msg HttpMethod.Post url "application/json" [] ""

let internal jsonSetPostMsg msg url content = jsonSetPotentMsgAndMediaType msg HttpMethod.Post url "application/json" [] content

let internal jsonSetEmptyPostMsgWithProfile msg url profVal = jsonSetPotentMsgAndMediaType msg HttpMethod.Post url "application/json" [("profile", profVal)] ""

let internal jsonSetEmptyPutMsg msg url = jsonSetPotentMsgAndMediaType msg HttpMethod.Put url "application/json" [] ""

let internal jsonSetPutMsg msg url content = jsonSetPotentMsgAndMediaType msg HttpMethod.Put url "application/json" [] content

let internal jsonSetPutMsgWithProfile msg url content profVal = jsonSetPotentMsgAndMediaType msg HttpMethod.Put url "application/json" [("profile", profVal)] content

let internal jsonSetDeleteMsg msg url = jsonSetDeleteMsgAndMediaType msg url "application/json" []

let internal jsonSetDeleteMsgWithProfile msg url profVal = jsonSetDeleteMsgAndMediaType msg url "application/json" [("profile", profVal)]

let internal setIfMatch (msg : HttpRequest) etag = 
    msg.GetTypedHeaders().IfMatch <- [| new EntityTagHeaderValue(new StringSegment(etag))|] :> IList<EntityTagHeaderValue>

let internal readSnapshotToJson (ss : HttpResponseMessage) = 
    // ReasAsStringAsync seems to hang so need to do this ......
    use s = ss.Content.ReadAsStreamAsync().Result
    s.Position <- 0L
    use sr = new StreamReader(s)
    sr.ReadToEnd()

let internal readActionResult (ar : ActionResult) (hc : HttpContext) = 
    let testContext = new ActionContext()
    testContext.HttpContext <- hc
    ar.ExecuteResultAsync testContext |> Async.AwaitTask |> Async.RunSynchronously
    use s = testContext.HttpContext.Response.Body
    s.Position <- 0L
    use sr = new StreamReader(s)
    let json = sr.ReadToEnd()
    let statusCode = testContext.HttpContext.Response.StatusCode
    let headers = testContext.HttpContext.Response.GetTypedHeaders()
    (json, statusCode, headers)
     

let internal comp (a : obj) (b : obj) e = 
    Assert.AreEqual(a, b, e)   

let internal listProperties (result : JObject) = 
    (result |> Seq.map (fun i -> (i :?> JProperty).Name) |> Seq.fold (fun a s ->  a + " " + s) "Properties: " )

let internal listExpected (expected : seq<TProp>) = 
    (expected |> Seq.map (fun r -> match r with | TProperty(s, _) -> s) |> Seq.fold (fun a s ->  a + " " + s) "Expected: " )

let rec internal compareArray (expected : seq<TObject>) (result : JArray) =
     
    let ps = if expected |> Seq.length <> result.Count then result.ToString() else ""
        
    Assert.AreEqual (expected |> Seq.length, result.Count, "Array - actual " + ps)
    result |> Seq.zip expected 
           |> Seq.iter (fun i ->  compare (fst(i)) (snd(i)) )     

and internal compareObject (expected : seq<TProp>) (result : JObject) = 
    
    Assert.AreEqual (expected |> Seq.length, result.Count, ((listProperties result) + " " + (listExpected expected)) )

    let orderedResult = result |> Seq.sortBy (fun i -> (i :?> JProperty).Name)
    
    orderedResult  |> Seq.map (fun i -> (i :?> JProperty).Name)  
                   |> Seq.zip (expected |> Seq.sortBy (fun r -> match r with | TProperty(s, _) -> s) |> Seq.map (fun r -> match r with | TProperty(s, _) -> s))                                                    
                   |> Seq.iter (fun i -> comp (fst(i)) (snd(i)) ((listProperties result) + " " + (listExpected expected)) ) 

    orderedResult  |> Seq.map (fun i -> (i :?> JProperty).Value)  
                   |> Seq.zip (expected |> Seq.sortBy (fun r -> match r with | TProperty(s, _) -> s) |> Seq.map (fun r -> match r with | TProperty(_, o) -> o))                                                    
                   |> Seq.iter (fun i ->  compare (fst(i)) (snd(i)) ) 

and internal compare expected (result : JToken) = 
    match expected  with 
    | TObjectJson(sq) -> compareObject sq (result :?> JObject )
    | TObjectVal(o) ->  Assert.AreEqual(o, (result :?> JValue).Value) 
    | TArray(sq)  -> compareArray sq  (result :?> JArray )    


let getIndent i = 
    let oneIndent = "  "
    let rec makeIndent indent count = 
        if count = 0 then indent else makeIndent (indent + oneIndent) (count - 1)
    makeIndent "" i 

let rec internal toStringArray (expected : seq<TObject>) indent : string =
    
    (getIndent indent) + "[\n" + ( expected |> Seq.fold (fun i j -> i + (toString j indent)) "") + "\n],"

and internal toStringProp (expected : TProp) indent : string = 

    match expected with 
    | TProperty(i, j) ->   (getIndent indent) +  i  + ": "  + (toString j indent) + ",\n"

and internal toStringObject (expected : seq<TProp>) indent : string = 

    (getIndent indent) +  "{\n" +   ( if expected = null then "null" else  expected |> Seq.fold (fun i j -> i + (toStringProp j (indent + 1) )) "") + "\n},"
   
and internal toString expected indent : string  = 
    match expected  with 
    | TObjectJson(sq) -> toStringObject sq (indent + 1)
    | TObjectVal(o) ->  if o = null then "null" else  o.ToString() 
    | TArray(sq)  -> toStringArray sq  (indent + 1)


let internal makeLinkPropWithMethodAndTypesValue (meth : string) (rel : string) href typ dTyp eTyp simple vObj = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp, eTyp, simple)));
       TProperty(JsonPropertyNames.Value, vObj);
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]


let internal makeLinkPropWithMethodAndTypes (meth : string) (rel : string) href typ dTyp eTyp simple = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp, eTyp, simple)));
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]

let internal makeLinkPropWithMethod (meth : string) (rel : string) href typ dTyp = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp)));
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]

let internal makeLinkPropWithMethodValue (meth : string) (rel : string) href typ dTyp vObj = 
     [ TProperty(JsonPropertyNames.Rel, TObjectVal(rel));
       TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href)));
       TProperty(JsonPropertyNames.Type, TObjectVal(new typeType(typ, dTyp)));
       TProperty(JsonPropertyNames.Value, vObj);
       TProperty(JsonPropertyNames.Method, TObjectVal(meth)) ]


let internal makeHref href =  [ TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType(href))) ]

let internal makeGetLinkProp = makeLinkPropWithMethod "GET"

let internal makePutLinkProp = makeLinkPropWithMethod "PUT"

let internal makePostLinkProp = makeLinkPropWithMethod "POST"

let internal makeDeleteLinkProp = makeLinkPropWithMethod "DELETE"
  
let internal makeIconLink() = 
        [ TProperty(JsonPropertyNames.Rel, TObjectVal(RelValues.Icon) );
          TProperty(JsonPropertyNames.Href, TObjectVal(new hrefType("images/Default.gif")) );
          TProperty(JsonPropertyNames.Type, TObjectVal("image/gif") );
          TProperty(JsonPropertyNames.Method, TObjectVal("GET") ) ]

let internal makeArgs parms =
    let getArg pmid = TProperty(pmid, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ]));
    let argVals = parms |> Seq.map (fun i -> match i with | TProperty(s, _) -> s ) |> Seq.map (fun s -> getArg s)
    TObjectJson(argVals) 
  
let internal makeListParm pmid fid rt =        
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

let internal makeStringParm pmid fid rt =      
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

let internal p1 ms = makeListParm "acollection" "Acollection" ms
let internal p2 ms = makeListParm "acollection" "Acollection" ms
let internal p3 = makeStringParm "p1" "P1" (ttc "string")
    
let internal makeActionMember oType  mName (oName : string) fName desc rType parms  =
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

let internal makeActionMemberSimple oType  mName (oName : string) fName desc rType parms =
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

let internal makeActionMemberString oType mName (oName : string) fName desc rType parms =
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

let internal makeActionMemberNumber oType mName (oName : string) fName desc rType parms =
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


let internal makeActionMemberStringSimple oType mName (oName : string) fName desc rType parms =
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

let internal makeActionMemberNumberSimple oType mName (oName : string) fName desc rType parms =
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



let internal makeActionMemberWithType oType mName (oName : string) fName desc rType parms =
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

let internal makeVoidActionMember oType mName (oName : string) fName desc parms =
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

let internal makeVoidActionMemberSimple oType mName (oName : string) fName desc parms =
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

let internal makeActionMemberCollection oType  mName (oName : string)  fName desc rType eType parms =
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

let internal makeActionMemberCollectionSimple oType  mName (oName : string)  fName desc rType eType parms =
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

let internal makePropertyMemberShort oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) (dValue : TProp list) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 2  
      let conditionalChoices = mName.Contains("ConditionalChoices")
      let choices = mName.Contains("Choices") && (not conditionalChoices)
      let disabled = mName.Contains("Disabled") || (oTypeName.Contains("ViewModel") && (not ( oTypeName.Contains("Form") || oTypeName.Contains("Edit")    )))      
      let autocomplete = mName.Contains("AutoComplete")
      let findmenu = mName.Contains("FindMenu")
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
      let exts = [TProperty(JsonPropertyNames.FriendlyName, TObjectVal(fName));
                  TProperty(JsonPropertyNames.Description, TObjectVal(desc));
                  TProperty(JsonPropertyNames.ReturnType, TObjectVal(rType));
                  TProperty(JsonPropertyNames.MemberOrder, TObjectVal(order));
                  TProperty(JsonPropertyNames.Optional, TObjectVal(opt))]

      let exts = if findmenu then TProperty(JsonPropertyNames.CustomFindMenu, TObjectVal(true)) :: exts else exts 

      let props = [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
                    TProperty(JsonPropertyNames.Id, TObjectVal(mName));
                    TProperty(JsonPropertyNames.Value, oValue);
                    TProperty(JsonPropertyNames.HasChoices, TObjectVal(choices));                                        
                    TProperty(JsonPropertyNames.Extensions, TObjectJson(exts));
                    TProperty(JsonPropertyNames.Links, TArray(links))]
      
      if choices then 
        let mst = ttc "RestfulObjects.Test.Data.MostSimple"
        let choiceRel = RelValues.Choice + makeParm RelParamValues.Property mName
             
        let obj1 =  TProperty(JsonPropertyNames.Title, TObjectVal("1")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "1"))  RepresentationTypes.Object mst
        let obj2 =  TProperty(JsonPropertyNames.Title, TObjectVal("2")) :: makeGetLinkProp choiceRel (sprintf "objects/%s/%s" mst (ktc "2"))  RepresentationTypes.Object mst

        TProperty(JsonPropertyNames.Choices, TArray([ TObjectJson(obj1); TObjectJson(obj2) ])) :: props;
      else
        props; 

let internal makePropertyMemberShortNoDetails oType (mName : string) (oTypeName : string) fName desc rType opt (oValue : TObject) args =
      let order = if desc = "" then 0 else 2 
      let conditionalChoices = mName.Contains("ConditionalChoices")
      let choices = mName.Contains("Choices") && (not conditionalChoices)
      let autocomplete = mName.Contains("AutoComplete")
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

let internal makePropertyMemberFullAttachment mName  (oName : string) fName title mt =
      let oType = "objects"
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
                    TObjectJson( attLink); ]

  
      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.DisabledReason, TObjectVal("Field not editable"));
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));                   
        TProperty(JsonPropertyNames.Extensions, exts);
        TProperty(JsonPropertyNames.Links, TArray(links))]


let internal makePropertyMemberFull oType mName  (oName : string) fName desc opt (oValue : TObject) =
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

let internal makeTablePropertyMember (mName : string) (oValue : TObject) =
       
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

let internal makeNoDetailsPropertyMember (mName : string)  (oName : string) (oValue : TObject) =
      
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

let internal makePropertyMemberFullNoDetails (mName : string) fName desc opt (oValue : TObject) =
      let order = if desc = "" then 0 else 2 
      let choices = mName.Contains("Choices")

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

let internal makePropertyMemberGuid oType (oName : string) (mName : string) (oValue : TObject) =
               
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
                                                             TProperty(JsonPropertyNames.ReturnType, TObjectVal("string"));
                                                             TProperty(JsonPropertyNames.MaxLength, TObjectVal(0));
                                                             TProperty(JsonPropertyNames.Format, TObjectVal("string"));
                                                             TProperty(JsonPropertyNames.Pattern, TObjectVal(""));
                                                             TProperty(JsonPropertyNames.MemberOrder, TObjectVal(0));                                                   
                                                             TProperty(JsonPropertyNames.Optional, TObjectVal(false))]));
        TProperty(JsonPropertyNames.Links, TArray(links))]

let internal makePropertyMemberDateTime oType (mName : string) (oName : string) fName desc opt (oValue : TObject) format =
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

let internal makePropertyMemberTimeSpan oType (mName : string) (oName : string) fName desc opt (oValue : TObject) format =
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


let internal makePropertyMemberString oType (mName : string) (oName : string) fName desc opt (oValue : TObject) (dValue : TProp list) =
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

let internal makePropertyMemberWithType oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
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

let internal makePropertyMemberWithNumber oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 3
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let choices = mName.Contains("Choices")
      let disabled = mName.Contains("Disabled")  || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit"))))          
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
        
let internal makePropertyMemberWithFormat oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
      let oTypeName = oName.Substring(0, oName.IndexOf("/"))
      let order = if desc = "" then 0 else 3
      let disabled = mName.Contains("Disabled")    || (oTypeName.Contains("ViewModel") && (not (oTypeName.Contains("FormViewModel") || oTypeName.Contains("Edit")))) 
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName
      let clearRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""); ]

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

let internal makePropertyMemberWithTypeNoValue oType (mName : string) (oName : string) fName desc rType opt =
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

let internal makePropertyMemberSimple oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
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

let internal makePropertyMemberSimpleNumber oType (mName : string) (oName : string) fName desc rType opt (oValue : TObject) =
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

let internal makePropertyMemberNone oType mName (oName : string)  (oValue : TObject) =
      let detailsRelValue = RelValues.Details + makeParm RelParamValues.Property mName  
      let modifyRel = RelValues.Modify + makeParm RelParamValues.Property mName

      let links = [ TObjectJson( makeGetLinkProp detailsRelValue (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty "");
                    TObjectJson(TProperty(JsonPropertyNames.Arguments, TObjectJson([TProperty(JsonPropertyNames.Value, TObjectVal(null))])) :: makePutLinkProp modifyRel (sprintf "%s/%s/properties/%s" oType oName mName) RepresentationTypes.ObjectProperty ""); ]

      [ TProperty(JsonPropertyNames.MemberType, TObjectVal(MemberTypes.Property) );
        TProperty(JsonPropertyNames.Id, TObjectVal(mName));
        TProperty(JsonPropertyNames.Value, oValue);
        TProperty(JsonPropertyNames.HasChoices, TObjectVal(false));    
        TProperty(JsonPropertyNames.Extensions, TObjectJson([]));
        TProperty(JsonPropertyNames.Links, TArray(links))]

let internal makePropertyMember mName oName fName (oValue : TObject) = makePropertyMemberFull "objects" mName oName fName "" false oValue 

let internal makeCollectionMemberNoDetails mName oName fName desc =
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


let internal makeCollectionMemberNoValue mName fName desc rType size cType cText =
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

let internal makeServiceActionMember mName oName rType parms = makeActionMember "services" mName oName (makeFriendly(mName)) "" rType parms

let internal makeServiceActionMemberSimple mName oName rType parms = makeActionMemberSimple "services" mName oName (makeFriendly(mName)) "" rType parms

let internal makeObjectActionMember mName oName rType parms = makeActionMember "objects" mName oName (makeFriendly(mName)) "" rType parms

let internal makeObjectActionMemberSimple mName oName rType parms = makeActionMemberSimple "objects" mName oName (makeFriendly(mName)) "" rType parms

let internal makeObjectActionCollectionMember mName oName eType parms = makeActionMemberCollection "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType parms

let internal makeObjectActionCollectionMemberSimple mName oName eType parms  = makeActionMemberCollectionSimple "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType parms

let internal makeObjectActionCollectionMemberNoParms mName oName eType  = makeActionMemberCollection "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType []

let internal makeObjectActionCollectionMemberNoParmsSimple mName oName eType  = makeActionMemberCollectionSimple "objects" mName oName (makeFriendly(mName)) "" ResultTypes.List eType []

let internal membersProp (oName : string, oType : string) = 
          TProperty(JsonPropertyNames.Members, 
                    TObjectJson([ TProperty("LocallyContributedAction", TObjectJson(makeObjectActionCollectionMember "LocallyContributedAction" oName oType [ p1 oType ]))
                                  TProperty("LocallyContributedActionWithParm", TObjectJson(makeObjectActionCollectionMember "LocallyContributedActionWithParm" oName oType [ p2 oType; p3 ])) ]))


let internal makeCollectionMemberTypeValue mName (oName : string) fName desc rType size cType cName cValue (dValue : TProp list) =
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

let internal makeCollectionMemberWithValue mName fName desc rType values size cType cText =
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


let internal makeCollectionMemberSimpleType mName (oName : string) fName desc rType size cType cName value =
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

let internal makeCollectionMemberSimpleTypeValue mName (oName : string) fName desc rType size cType cName cValue (dValue : TProp list) =
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

let internal makeCollectionMemberSimple mName (oName : string) fName desc rType size value = makeCollectionMemberSimpleType mName oName fName desc rType size (ttc "RestfulObjects.Test.Data.MostSimple") "Most Simples" value

let internal makeCollectionMemberType mName (oName : string) fName desc rType size cType cName value=
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

let internal makeCollectionMember mName (oName : string) fName desc rType size value = makeCollectionMemberType mName (oName : string) fName desc rType size (ttc "RestfulObjects.Test.Data.MostSimple") "Most Simples" value

let internal makeObjectActionVoidMember mName oName  = makeVoidActionMember "objects" mName oName (makeFriendly(mName)) ""  []

let internal makeObjectActionVoidMemberSimple mName oName  = makeVoidActionMemberSimple "objects" mName oName (makeFriendly(mName)) ""  []

let internal makeServiceActionMemberNoParms mName oName rType  = makeActionMember "services" mName oName (makeFriendly(mName)) "" rType []

let internal makeServiceActionMemberNoParmsSimple mName oName rType  = makeActionMemberSimple "services" mName oName (makeFriendly(mName)) "" rType []

let internal makeObjectActionMemberNoParms mName oName rType  = makeActionMember "objects" mName oName (makeFriendly(mName)) "" rType []

let internal makeObjectActionMemberNoParmsSimple mName oName rType  = makeActionMemberSimple "objects" mName oName (makeFriendly(mName)) "" rType []

let internal makeObjectPropertyMember mName oName fName (oValue : TObject) =  makePropertyMember mName oName fName oValue 

let internal makeObjectPropertyMemberWithDesc mName oName fName desc opt (oValue : TObject) =  makePropertyMemberFull "objects" mName oName fName desc opt oValue 

let internal makeServiceActionCollectionMember mName oName eType parms = makeActionMemberCollection "services" mName oName (makeFriendly(mName)) "" ResultTypes.List eType parms

let internal makeServiceActionCollectionMemberNoParms mName oName eType  = makeActionMemberCollection "services" mName oName (makeFriendly(mName)) "" ResultTypes.List eType []

let internal makeServiceActionVoidMember mName oName  = makeVoidActionMember "services" mName oName (makeFriendly(mName)) ""  []

let internal assertTransactionalCache  (headers : Headers.ResponseHeaders) = 
    Assert.AreEqual(true, headers.CacheControl.NoCache)
    Assert.AreEqual("no-cache", headers.Headers.["Pragma"].ToString())
    Assert.AreEqual(headers.Date, headers.Expires)

let internal assertConfigCache secs  (headers : Headers.ResponseHeaders) = 
    let ts = new TimeSpan(0, 0, secs)
    Assert.AreEqual(ts, headers.CacheControl.MaxAge)
    Assert.IsTrue(headers.Date.HasValue)
    let expire = headers.Date.Value + ts
    Assert.AreEqual(expire, headers.Expires)

let internal assertUserInfoCache (headers : Headers.ResponseHeaders) = 
    Assert.AreEqual(oneHour, headers.CacheControl.MaxAge)
    Assert.IsTrue(headers.Date.HasValue)
    let expire = headers.Date.Value + oneHour
    Assert.AreEqual(expire, headers.Expires)

let internal assertNonExpiringCache (headers : Headers.ResponseHeaders) = 
    Assert.AreEqual(oneDay, headers.CacheControl.MaxAge)
    Assert.IsTrue(headers.Date.HasValue)
    let expire = headers.Date.Value + oneDay
    Assert.AreEqual(expire, headers.Expires)

let internal assertStatusCode (sc : HttpStatusCode) iSc msg= 
    Assert.AreEqual((int)sc, iSc, msg)

let internal CreateSingleValueArgWithReserved (m : JObject) = ModelBinderUtils.CreateSingleValueArgument(m, true)
     
let internal CreateArgMapWithReserved (m : JObject) = ModelBinderUtils.CreateArgumentMap(m, true)
   
let internal CreateArgMapFromUrl (s : string) = ModelBinderUtils.CreateSimpleArgumentMap(s)

let internal CreatePersistArgMapWithReserved (m : JObject) = ModelBinderUtils.CreatePersistArgMap(m, true)

let internal GetDomainType (m : JObject) = 
  
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
               
let internal invokeRelTypeSb = RelValues.Invoke + makeParm RelParamValues.TypeAction "isSubtypeOf"
let internal invokeRelTypeSp = RelValues.Invoke + makeParm RelParamValues.TypeAction "isSupertypeOf"

// create isSubtypeOf rep
let internal sb (oType : string) = 
    TProperty(JsonPropertyNames.Id, TObjectVal("isSubtypeOf")) 
    :: (TProperty
           (JsonPropertyNames.Arguments, 
            TObjectJson
                ([ TProperty
                       (JsonPropertyNames.SuperType, 
                        TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])) 
       :: makeGetLinkProp invokeRelTypeSb (sprintf "domain-types/%s/type-actions/isSubtypeOf/invoke" oType) 
              RepresentationTypes.TypeActionResult "")

// create isSupertypeOf rep
let internal sp (oType : string) = 
    TProperty(JsonPropertyNames.Id, TObjectVal("isSupertypeOf")) 
    :: (TProperty
           (JsonPropertyNames.Arguments, 
            TObjectJson
                ([ TProperty
                       (JsonPropertyNames.SubType, TObjectJson([ TProperty(JsonPropertyNames.Value, TObjectVal(null)) ])) ])) 
       :: makeGetLinkProp invokeRelTypeSp (sprintf "domain-types/%s/type-actions/isSupertypeOf/invoke" oType) 
              RepresentationTypes.TypeActionResult "")

let internal simpleLinks = 
  [ TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.HomePage RepresentationTypes.HomePage "")
    TObjectJson(makeGetLinkProp RelValues.User SegmentValues.User RepresentationTypes.User "")
    TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Services SegmentValues.Services RepresentationTypes.List "" "System.Object" true)
    TObjectJson(makeLinkPropWithMethodAndTypes "GET" RelValues.Menus SegmentValues.Menus RepresentationTypes.List "" "System.Object" true)
    TObjectJson(makeGetLinkProp RelValues.Version SegmentValues.Version RepresentationTypes.Version "") ]
              
let internal expectedSimple = 
  [ TProperty(JsonPropertyNames.Links, TArray(simpleLinks))
    TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

let internal makeInvokeCollectionParm pmid fid rt = 
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

let internal makeInvokeValueParm pmid fid =    
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

let internal expectedUser = 
    [ TProperty(JsonPropertyNames.Links, 
                TArray([ TObjectJson(makeGetLinkProp RelValues.Self SegmentValues.User RepresentationTypes.User "")
                         TObjectJson(makeGetLinkProp RelValues.Up SegmentValues.HomePage RepresentationTypes.HomePage "") ]))
      TProperty(JsonPropertyNames.UserName, TObjectVal("Test"))
      TProperty(JsonPropertyNames.Roles, TArray([]))
      TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]

let resetCache (api : RestfulObjectsControllerBase) =
    let config = api.GetConfig()
    config.CacheSettings  <- (0, 3600, 86400)
    api.ResetConfig(config)
    ()

let setDebugWarnings (api : RestfulObjectsControllerBase) flag =
    let config = api.GetConfig()
    config.DebugWarnings  <- flag
    api.ResetConfig(config)
    ()