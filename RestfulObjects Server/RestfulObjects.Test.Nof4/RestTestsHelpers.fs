// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module NakedObjects.Rest.Test.RestTestsHelpers

open NUnit.Framework
open NakedObjects
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Core.Adapter.Map
open NakedObjects.Boot
open NakedObjects.Architecture.Adapter
open NakedObjects.Architecture.Persist
open NakedObjects.Architecture.Reflect
open NakedObjects.Core.Context
open NakedObjects.Core.Persist
open NakedObjects.Persistor
open NakedObjects.Persistor.Objectstore
open NakedObjects.Persistor.Objectstore.Inmemory
open RestfulObjects.Test.Data
open RestfulObjects.Mvc
open RestfulObjects.Mvc.Media
open System
open RestfulObjects.Snapshot.Utility
open RestfulObjects.Snapshot.Constants
open System.Threading
open System.Security.Principal
open System.Web.Http
open NakedObjects.Core.Context
open NakedObjects.Core.Util
open Microsoft.Practices.Unity
open NakedObjects.EntityObjectStore
open RestfulObjects.Test.Data
open NakedObjects.Surface.Nof4.Implementation
open NakedObjects.Surface.Nof4.Utility
open NakedObjects.Surface
open MvcTestApp.Controllers

let seedCodeFirstDatabase (context : CodeFirstContext) = 
    let ms1 = new MostSimple(Id = 1)
    let ms2 = new MostSimple(Id = 2)
    let ms3 = new MostSimple(Id = 4)
    context.MostSimples.Add(ms1) |> ignore
    context.MostSimples.Add(ms2) |> ignore
    context.MostSimples.Add(ms3) |> ignore
    let wr1 = new WithReference(Id = 1)
    wr1.AReference <- ms1
    wr1.ADisabledReference <- ms1
    wr1.AChoicesReference <- ms1
    wr1.AnEagerReference <- ms1
    wr1.AnAutoCompleteReference <- ms1
    let wr2 = new WithReference(Id = 2)
    wr2.AReference <- ms1
    wr2.ADisabledReference <- ms1
    wr2.AChoicesReference <- ms1
    wr2.AnEagerReference <- ms1
    wr2.AnAutoCompleteReference <- ms1
    context.WithReferences.Add(wr1) |> ignore
    context.WithReferences.Add(wr2) |> ignore
    let wv1 = new WithValue(Id = 1)
    wv1.AValue <- 100
    wv1.ADisabledValue <- 200
    wv1.AStringValue <- ""
    context.WithValues.Add(wv1) |> ignore
    let ws1 = new WithScalars(Id = 1)
    ws1.Bool <- true
    ws1.Byte <- (byte) 1
    ws1.ByteArray <- [| (byte) 2 |]
    ws1.Char <- '3'
    ws1.CharArray <- [| (char) 4 |]
    ws1.Decimal <- 5.1M
    ws1.Double <- 6.2
    ws1.Float <- 7.3F
    ws1.Int <- 8
    ws1.Long <- 9L
    ws1.SByte <- (sbyte) 10
    ws1.SByteArray <- [| (sbyte) 11 |]
    ws1.Short <- (int16) 12
    ws1.String <- "13"
    ws1.UInt <- (uint32) 14
    ws1.ULong <- (uint64) 15
    ws1.UShort <- (uint16) 16
    context.WithScalarses.Add(ws1) |> ignore
    let wa1 = new WithActionObject(Id = 1)
    context.WithActionObjects.Add(wa1) |> ignore
    let wc1 = new WithCollection(Id = 1)
    wc1.ACollection.Add(ms1)
    wc1.ACollection.Add(ms2)
    wc1.ACollection.Add(ms3)
    wc1.ACollectionViewModels.Add(new MostSimpleViewModel(Id = 1))
    wc1.ACollectionViewModels.Add(new MostSimpleViewModel(Id = 2))
    wc1.ASet.Add(ms1) |> ignore
    wc1.ASet.Add(ms2) |> ignore
    wc1.ADisabledCollection.Add(ms1)
    wc1.ADisabledCollection.Add(ms2)
    wc1.AHiddenCollection.Add(ms1)
    wc1.AHiddenCollection.Add(ms2)
    wc1.AnEagerCollection.Add(ms1)
    wc1.AnEagerCollection.Add(ms2)
    context.WithCollections.Add(wc1) |> ignore
    let we1 = new WithError(Id = 1)
    context.WithErrors.Add(we1) |> ignore
    let wge1 = new WithGetError(Id = 1)
    context.WithGetErrors.Add(wge1) |> ignore
    let i1 = new Immutable(Id = 1)
    context.Immutables.Add(i1) |> ignore
    let vs1 = new VerySimple(Id = 1)
    context.VerySimples.Add(vs1) |> ignore
    let vse1 = new VerySimpleEager(Id = 1)
    context.VerySimpleEagers.Add(vse1) |> ignore
    let dt1 = new WithDateTimeKey()
    dt1.Id <- (new DateTime(634835232000000000L)).Date
    context.WithDateTimeKeys.Add(dt1) |> ignore
    let rdo1 = new RedirectedObject(Id = 1)
    rdo1.ServerName <- "RedirectedToServer"
    rdo1.Oid <- "RedirectedToOid"
    context.RedirectedObjects.Add(rdo1) |> ignore
    let wat1 = new WithAttachments(Id = 1)
    context.WithAttachments.Add(wat1) |> ignore
    let added = context.SaveChanges()
    Assert.AreEqual(42, added)
    wc1.ACollection.Remove(ms3) |> ignore
    context.SaveChanges() |> ignore
    ()

type CodeFirstInitializer() = 
    inherit System.Data.Entity.DropCreateDatabaseAlways<CodeFirstContext>()
    override x.Seed(context : CodeFirstContext) = seedCodeFirstDatabase context

let CodeFirstSetup() = 
    System.Data.Entity.Database.SetInitializer(new CodeFirstInitializer())
    ()

let mapper = new TestTypeCodeMapper()
let keyMapper = new TestKeyCodeMapper()