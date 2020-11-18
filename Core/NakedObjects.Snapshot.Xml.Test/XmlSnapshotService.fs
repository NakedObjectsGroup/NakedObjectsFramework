// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

module NakedObjects.XmlSnapshotService

open NakedObjects.Core.Util
open NakedObjects.Persistor.Entity.Configuration
open NakedObjects.Services
open NakedObjects.Snapshot
open NakedObjects.Snapshot.Xml.Service
open NakedObjects.Snapshot.Xml.Utility
open NakedObjects.Xat
open NUnit.Framework
open Snapshot.Xml.Test
open System
open System.Data.Entity
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq

let appveyorServer = @"Data Source=(local)\SQL2017;"
let localServer =  @"Data Source=(localdb)\MSSQLLocalDB;"

#if APPVEYOR 
let server = appveyorServer
#else
let server = localServer
#endif


let cs = server + @"Initial Catalog=TestObject;Integrated Security=True;"

let normalizeData d1 d2 =
    // ignore keys 
    let pattern = "TEOID#\\d+";
    let replacement = "TEOID#X";
    let rgx = new Regex(pattern);
    let nd1 = rgx.Replace(d1, replacement);
    let nd2 = rgx.Replace(d2, replacement); 
    (nd1, nd2)

let writetests = false
let dir() = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testfiles");
let getFile name = Path.Combine(dir(), name) + ".htm"
let getTestData name = File.ReadAllText(getFile name)
let writeTestData name data = File.WriteAllText(getFile name, data)
let checkResults resultsFile s = 
    if writetests then writeTestData resultsFile s
    else 
        let actionView = getTestData resultsFile
        let nd = normalizeData actionView s
        Assert.AreEqual(fst(nd), snd(nd))
                
[<TestFixture>]
type DomainTests() = 
    class
        inherit AcceptanceTestCase()
        
        override x.Persistor =
            let config = new EntityObjectStoreConfiguration()
            config.EnforceProxies <- false
          
            let config = new EntityObjectStoreConfiguration()
            let f = (fun () -> new TestObjectContext(cs) :> DbContext)
            config.UsingContext(Func<DbContext>(f)) |> ignore
            config
        
        [<OneTimeSetUp>]
        member x.FixtureSetup() = 
            AcceptanceTestCase.InitializeNakedObjectsFramework(x)
            System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<TestObjectContext>())
                
        [<SetUp>]
        member x.Setup() = x.StartTest()
                     
        override x.Types = 
            [| typeof<IXmlSnapshot>;
               typeof<XmlSnapshot>;
               typeof<TestObject>; 
               typeof<Snapshot.Xml.Test.Two.TransformSubObject>; 
               typeof<TestObject[]>;
               typeof<System.Collections.Generic.List<TestObject>>;
               typeof<One.TransformFull>;
               typeof<Two.TransformFull>;
               typeof<IXmlSnapshotService>  |]
        
        override x.Namespaces = 
            [| "Snapshot.Xml.Test";
               "Snapshot.Xml.Test.Two";
               "Snapshot.Xml.Test.One";
               "NakedObjects.XmlSnapshotService.DomainTests";
               "NakedObjects.Snapshot" |]

        override x.Services =         
            [| typeof<SimpleRepository<TestObject>>
               typeof<XmlSnapshotService>
               typeof<TransformRepository> |]
        
        member x.TestService = x.GetTestService("Test Objects")
        
        member x.TransformService = x.GetTestService("Transform Repository")
        
        member x.GenerateSnapshot(o : obj) = 
            x.GetTestService(typeof<IXmlSnapshotService>).GetAction("Generate Snapshot").InvokeReturnObject(o).NakedObject.GetDomainObject<IXmlSnapshot>()
        
        member x.SimpleTestObject() = x.TestService.GetAction("New Instance").InvokeReturnObject().NakedObject.GetDomainObject<TestObject>()
        
        member x.ComplexTestObject() = 
            let testObject1 = x.SimpleTestObject()
            let testObject2 = x.SimpleTestObject()
            let testObject3 = x.SimpleTestObject()
            let testObject4 = x.SimpleTestObject()
            testObject1.TestInt <- 1
            testObject1.TestString <- "test value"
            testObject1.TestReference <- testObject2
            testObject1.TestCollection.Add testObject3
            testObject1.TestCollection.Add testObject4
            testObject1
        
        member x.NestedComplexTestObject() = 
            let testObject1 = x.SimpleTestObject()
            let testObject2 = x.SimpleTestObject()
            let testObject3 = x.SimpleTestObject()
            let testObject4 = x.SimpleTestObject()
            let testObject5 = x.SimpleTestObject()
            let testObject6 = x.SimpleTestObject()
            let testObject7 = x.SimpleTestObject()
            testObject1.TestInt <- 1
            testObject1.TestString <- "test value"
            testObject1.TestReference <- testObject2
            testObject1.TestCollection.Add testObject3
            testObject1.TestCollection.Add testObject4
            testObject2.TestReference <- testObject5
            testObject3.TestReference <- testObject6
            testObject4.TestReference <- testObject7
            testObject1
        
        member x.TransFormFullObject() = x.TransformService.GetAction("Transform Full").InvokeReturnObject().NakedObject.GetDomainObject<One.TransformFull>()
        
        member x.TransFormWithSubObject() = 
            x.TransformService.GetAction("Transform With Sub Object").InvokeReturnObject().NakedObject.GetDomainObject<Two.TransformFull>()
        
        [<Test>]
        member x.XmlForSimpleObject() = 
            let testObject = x.SimpleTestObject()
            let ss = x.GenerateSnapshot testObject
            checkResults "simpleTestData" ss.Xml
            ()
        
        [<Test>]
        member x.XmlForComplexObject() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            checkResults "complexTestData" ss.Xml
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeReference() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include "TestReference"
            checkResults "complexTestDataWithReference" ss.Xml
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeCollection() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include "TestCollection"
            checkResults "complexTestDataWithCollection" ss.Xml
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeReferenceWithAnnotation() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include("TestReference", "test annotation")
            checkResults "complexTestDataWithReferenceAnnotation" ss.Xml
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeCollectionWithAnnotation() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include("TestCollection", "test annotation")
            checkResults "complexTestDataWithCollectionAnnotation" ss.Xml
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeNestedReference() = 
            let testObject = x.NestedComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include "TestReference/TestReference"
            checkResults "complexTestDataWithNestedReference" ss.Xml
            ()
        
        
        member x.stylesheet1 = @"<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" version=""1.0"" xmlns:app=""http://www.nakedobjects.org/ns/app/Snapshot.Xml.Test.Two/TransformFull"">
            <xsl:output method=""xml""/>
            <xsl:template match=""@*|node()"">
              <xsl:copy>
                <xsl:apply-templates select=""@*|node()""/>
              </xsl:copy>
            </xsl:template>
            <xsl:template match=""app:Content"">
                <xsl:copy-of select=""*/app:*""/> 
            </xsl:template>     
         </xsl:stylesheet>"
        member x.stylesheet2 = @"<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" version=""1.0"" xmlns:app=""http://www.nakedobjects.org/ns/app/Snapshot.Xml.Test.Two/TransformFull"">
            <xsl:output method=""xml""/>
            <xsl:template match=""@*|node()"">
              <xsl:copy>
                <xsl:apply-templates select=""@*|node()""/>
              </xsl:copy>
            </xsl:template>
         </xsl:stylesheet>"
        
        member x.CompareElementValues (elem1 : XElement) (elem2 : XElement) = 
            let text1 = System.Linq.Enumerable.OfType<XText>(elem1.Nodes())
            let text2 = System.Linq.Enumerable.OfType<XText>(elem2.Nodes())
            if (text1
                |> Seq.isEmpty && text2 |> Seq.isEmpty) then true
            elif (not (text1 |> Seq.isEmpty) && not (text2 |> Seq.isEmpty)) then (text1 |> Seq.head).Value = (text2 |> Seq.head).Value
            else false
        
        member x.CompareXml xml1 xml2 = 
            let doc1 = XDocument.Parse xml1
            let doc2 = XDocument.Parse xml2
            if (doc1.DescendantNodes() |> Seq.length) = (doc2.DescendantNodes() |> Seq.length) then 
                for node in doc1.Descendants() do
                    let matchingNode = doc2.Descendants() |> Seq.find (fun n -> n.Name = node.Name)
                    Assert.IsNotNull matchingNode
                    Assert.IsTrue(x.CompareElementValues node matchingNode)
                    Assert.AreEqual(node.Attributes() |> Seq.length, matchingNode.Attributes() |> Seq.length)
                    for attr in node.Attributes() do
                        let matchingAttr = matchingNode.Attributes() |> Seq.find (fun a -> a.Name = attr.Name)
                        Assert.IsNotNull matchingAttr
                        let nd = normalizeData attr.Value matchingAttr.Value
                        Assert.AreEqual(fst(nd), snd(nd))
            else
                raise (Exception())
        
        [<Test>]
        member x.TransformXmlToMatchFull() = 
            let fullTestObject = x.TransFormFullObject()
            let nestedTestObject = x.TransFormWithSubObject()
            let fullSS = x.GenerateSnapshot fullTestObject
            let nestedSS = x.GenerateSnapshot nestedTestObject
            nestedSS.Include "Content"
            let fullXml = fullSS.Xml
            let nestedXml = nestedSS.Xml
            let nestedTransformedXml = nestedSS.TransformedXml x.stylesheet1
            let nestedTransformedXml = nestedTransformedXml.Replace("Snapshot.Xml.Test.Two", "Snapshot.Xml.Test.One")
            let nestedTransformedXml = nestedTransformedXml.Replace("TEOID#3", "TEOID#1")
            try 
                x.CompareXml fullXml nestedXml
                Assert.Fail("expected not equal")
            with expected -> ()
            x.CompareXml fullXml nestedTransformedXml
        
    end

[<EntryPoint>]
let main _ =
    0
