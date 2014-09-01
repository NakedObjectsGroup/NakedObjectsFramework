module NakedObjects.XmlSnapshotService

open NUnit.Framework
open NakedObjects
open NakedObjects.Core.NakedObjectsSystem
open NakedObjects.Core.Persist
open NakedObjects.Core.Adapter.Map
open NakedObjects.Services
open NakedObjects.Boot
open Snapshot.Xml.Test
open NakedObjects.Snapshot
open NakedObjects.Architecture.Adapter
open NakedObjects.Architecture.Persist
open XmlTestData
open Snapshot.Xml.Test
open System.Xml.Linq
open NakedObjects.Persistor.Objectstore
open NakedObjects.Persistor.Objectstore.Inmemory
open NakedObjects.Persistor

[<TestFixture>]
type DomainTests() = 
    class
        inherit NakedObjects.Xat.AcceptanceTestCase()

        override x.RegisterTypes(container) = 
            base.RegisterTypes(container);
            // replace INakedObjectStore types
            let sink = container.RegisterType(typeof<IOidGenerator>, typeof<SimpleOidGenerator>, "reflector", null, [||])
            let sink = container.RegisterType(typeof<IPersistAlgorithm>, typeof<DefaultPersistAlgorithm>, null, null, [||])
            let sink = container.RegisterType(typeof<INakedObjectStore>, typeof<MemoryObjectStore>, null, null, [||])
            let sink = container.RegisterType(typeof<IIdentityMap>, typeof<IdentityMapImpl>, null, null, [||])
            ()
             
        [<TestFixtureSetUp>]
        member x.FixtureSetup() = x.InitializeNakedObjectsFramework()
        
        [<TearDown>]
        member x.FixtureTearDown() = x.CleanupNakedObjectsFramework()
        
        [<SetUp>]
        member x.Setup() = x.StartTest()
        
        [<TearDown>]
        member x.TearDown() = ()
        
        override x.MenuServices = 
            let testService = new SimpleRepository<TestObject>()
            let xmlService = new XmlSnapshotService()
            let transformService = new TransformRepository()
            box (new ServicesInstaller([| (box testService)
                                          (box xmlService)
                                          (box transformService) |])) :?> IServicesInstaller : IServicesInstaller
        
        override x.Persistor = 
            let inst = new InMemoryObjectPersistorInstaller()
            inst.SimpleOidGeneratorStart <- new System.Nullable<int>(100)
            box (inst) :?> IObjectPersistorInstaller : IObjectPersistorInstaller
        
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
            Assert.AreEqual(simpleTestData, ss.Xml)
            ()
        
        [<Test>]
        member x.XmlForComplexObject() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            Assert.AreEqual(complexTestData, ss.Xml)
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeReference() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include "TestReference"
            Assert.AreEqual(complexTestDataWithReference, ss.Xml)
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeCollection() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include "TestCollection"
            Assert.AreEqual(complexTestDataWithCollection, ss.Xml)
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeReferenceWithAnnotation() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include("TestReference", "test annotation")
            Assert.AreEqual(complexTestDataWithReferenceAnnotation, ss.Xml)
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeCollectionWithAnnotation() = 
            let testObject = x.ComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include("TestCollection", "test annotation")
            Assert.AreEqual(complexTestDataWithCollectionAnnotation, ss.Xml)
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeNestedReference() = 
            let testObject = x.NestedComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include "TestReference/TestReference"
            Assert.AreEqual(complexTestDataWithNestedReference, ss.Xml)
            ()
        
        [<Test>]
        member x.XmlForComplexObjectIncludeNestedCollection() = 
            let testObject = x.NestedComplexTestObject()
            let ss = x.GenerateSnapshot testObject
            ss.Include "TestCollection/TestReference"
            Assert.AreEqual(complexTestDataWithNestedCollection, ss.Xml)
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
            Assert.AreEqual(doc1.DescendantNodes() |> Seq.length, doc2.DescendantNodes() |> Seq.length)
            for node in doc1.Descendants() do
                let matchingNode = doc2.Descendants() |> Seq.find (fun n -> n.Name = node.Name)
                Assert.IsNotNull matchingNode
                Assert.IsTrue(x.CompareElementValues node matchingNode)
                Assert.AreEqual(node.Attributes() |> Seq.length, matchingNode.Attributes() |> Seq.length)
                for attr in node.Attributes() do
                    let matchingAttr = matchingNode.Attributes() |> Seq.find (fun a -> a.Name = attr.Name)
                    Assert.IsNotNull matchingAttr
                    Assert.AreEqual(attr.Value, matchingAttr.Value)
        
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
            let nestedTransformedXml = nestedTransformedXml.Replace("TOID#FFFFFFFFFFFFFF9E", "TOID#FFFFFFFFFFFFFF9C")
            try 
                x.CompareXml fullXml nestedXml
                Assert.Fail("expected not equal")
            with expected -> ()
            x.CompareXml fullXml nestedTransformedXml
        
        [<Test>]
        [<Ignore>]
        member x.TransformXsdToIdentity() = 
            let nestedTestObject = x.TransFormWithSubObject()
            let nestedSS = x.GenerateSnapshot nestedTestObject
            nestedSS.Include "Content"
            let nestedXsd = nestedSS.Xsd
            let nestedTransformedXsd = nestedSS.TransformedXsd x.stylesheet2
            x.CompareXml nestedXsd nestedTransformedXsd
    end

