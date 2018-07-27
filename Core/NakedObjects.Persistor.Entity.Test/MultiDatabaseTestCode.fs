// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module NakedObjects.MultiDatabaseTestCode

open System
open NUnit.Framework
open TestCodeOnly
open TestCode
open TestTypes
open NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
open NakedObjects.Architecture
open NakedObjects.Architecture.Persist
open System.Data.Entity.Core.Objects
open NakedObjects.Persistor.Entity.Configuration
open NakedObjects.Persistor.Entity
open NakedObjects.Core
open NakedObjects.Persistor.Entity.Component


let CanCreateEntityPersistor multiDatabasePersistor = Assert.IsNotNull(multiDatabasePersistor)

let CodeFirstLoadTestAssembly() = 
    let obj = new Category()
    let obj = new AddressType()
    ()

let MultiDatabaseSetup() = 
    CodeFirstLoadTestAssembly()
    System.Data.Entity.Database.SetInitializer(new CodeOnlyTestCode.CodeFirstInitializer())
    ()

let CanQueryEachConnection<'t, 'u when 't : not struct and 'u : not struct>(multiDatabasePersistor : EntityObjectStore) = 
    let p1 = multiDatabasePersistor.GetInstances<'t>() |> Seq.head
    let p1 = multiDatabasePersistor.GetInstances<'t>() |> Seq.head
    let p2 = multiDatabasePersistor.GetInstances<'u>() |> Seq.head
    let p2 = multiDatabasePersistor.GetInstances<'u>() |> Seq.head
    Assert.IsNotNull(p1)
    Assert.IsNotNull(p2)
    Assert.AreNotEqual(p1, p2)

let CanQueryEachConnectionMulti multiDatabasePersistor = CanQueryEachConnection<TestCodeOnly.Product, NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly.Product> multiDatabasePersistor
let CanQueryEachDomainConnection multiDatabasePersistor = CanQueryEachConnection<SimpleDatabase.Person, NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly.Product> multiDatabasePersistor

let CanCreateEachConnection(multiDatabasePersistor : EntityObjectStore) = 
    let productSetter (pr : TestCodeOnly.Product) = 
        pr.ID <- 1
        pr.Name <- uniqueName()
    CanSaveTransientObject multiDatabasePersistor productSetter
    let setter (sr : ScrapReason) = 
        sr.Name <- uniqueName()
        sr.ModifiedDate <- DateTime.Now
    CanSaveTransientObject multiDatabasePersistor setter

let CanQueryEachConnectionMultiTimes multiDatabasePersistor = 
    CanQueryEachConnectionMulti multiDatabasePersistor
    CanQueryEachConnectionMulti multiDatabasePersistor

let CanQueryEachDomainConnectionMultiTimes multiDatabasePersistor = 
    CanQueryEachDomainConnection multiDatabasePersistor
    CanQueryEachDomainConnection multiDatabasePersistor

let CanCreateEachConnectionMultiTimes multiDatabasePersistor = 
    CanCreateEachConnection multiDatabasePersistor
    CanCreateEachConnection multiDatabasePersistor

let CrossContextTransactionOK(multiDatabasePersistor : EntityObjectStore) = 
    let pr = multiDatabasePersistor.GetInstances<TestCodeOnly.Product>() |> Seq.head
    let sr = multiDatabasePersistor.GetInstances<ScrapReason>() |> Seq.head
    let origPrName = pr.Name
    let origSrName = sr.Name
    pr.Name <- uniqueName()
    sr.Name <- uniqueName()
    let SaveAndEnd o1 o2 = 
        multiDatabasePersistor.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o1 null)
        multiDatabasePersistor.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o2 null)
        multiDatabasePersistor.EndTransaction()
    SaveAndEnd pr sr
    Assert.AreNotEqual(origPrName, pr.Name)
    Assert.AreNotEqual(origSrName, sr.Name)
    pr.Name <- origPrName
    sr.Name <- origSrName
    SaveAndEnd pr sr
    Assert.AreEqual(origPrName, pr.Name)
    Assert.AreEqual(origSrName, sr.Name)

let CrossContextTransactionRollback(multiDatabasePersistor : EntityObjectStore) = 
    let pr = multiDatabasePersistor.GetInstances<TestCodeOnly.Product>() |> Seq.head
    let sr = multiDatabasePersistor.GetInstances<ScrapReason>() |> Seq.head
    let sr1 = multiDatabasePersistor.GetInstances<ScrapReason>() |> Seq.item 1
    let origPrName = pr.Name
    let origSrName = sr.Name
    pr.Name <- uniqueName()
    sr.Name <- sr1.Name
    let SaveAndEnd o1 o2 = 
        multiDatabasePersistor.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o1 null)
        multiDatabasePersistor.ExecuteSaveObjectCommand(GetOrAddAdapterForTest o2 null)
        multiDatabasePersistor.EndTransaction()
    try 
        SaveAndEnd pr sr
        Assert.Fail()
    with expected -> Assert.IsInstanceOf(typeof<DataUpdateException>, expected)
    Assert.AreEqual(origPrName, pr.Name)
    Assert.AreEqual(origSrName, sr.Name)
    let prs = multiDatabasePersistor.GetInstances<TestCodeOnly.Product>()
    let srs = multiDatabasePersistor.GetInstances<ScrapReason>()
    (prs :?> ObjectQuery).MergeOption <- MergeOption.OverwriteChanges
    (srs :?> ObjectQuery).MergeOption <- MergeOption.OverwriteChanges
    let pr = prs |> Seq.head
    let sr = srs |> Seq.head
    Assert.AreEqual(origPrName, pr.Name)
    Assert.AreEqual(origSrName, sr.Name)
    ()
