// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
module NakedObjects.WifAuthorisationTest
open NUnit.Framework
open System.Xml.Linq
open System.Xml
open System
open NakedObjects.Reflector.Security.Wif
open NakedObjects.Reflector.Peer
open NakedObjects.Architecture.Facets
open NakedObjects.Architecture.Security
open System.Security.Principal
open System.Security.Claims

type XElement (name:string, [<ParamArray>] values:obj []) = 
    inherit System.Xml.Linq.XElement
        (System.Xml.Linq.XName.op_Implicit(name), values)     

type XAttribute (name:string, [<ParamArray>] values:obj []) = 
    inherit System.Xml.Linq.XAttribute
        (System.Xml.Linq.XName.op_Implicit(name), values)     

type TestSession(p : IPrincipal) =
    let principal = p
    interface ISession with 
        member x.IsAuthenticated with get() = true
        member x.Principal with get() : IPrincipal = principal
        member x.UserName with get() = principal.Identity.Name

[<TestFixture>]
type WifTests() = 

    let claimsPrincipal =
        let cp = new ClaimsPrincipal()
        let ci = new ClaimsIdentity()
        ci.AddClaim(new Claim("claim1type", "claim1value"))
        cp.AddIdentity(ci)
        cp  

    let testSession = new TestSession(claimsPrincipal)

    let testXml = "<testxml><class name=\"class1\" fullname=\"ns.class1\">" + 
                  "<member name=\"member1\" type=\"ViewField\">" + 
                  "<claim value=\"claim1value\" type=\"claim1type\"/>" +
                  "</member>" + 
                  "<member name=\"member2\" type=\"EditField\">" + 
                  "<claim value=\"claim1value\" type=\"claim1type\"/>" +
                  "</member>" +
                  "<member name=\"member3\" type=\"Action\">" + 
                  "<claim value=\"claim1value\" type=\"claim1type\"/>" +
                  "</member>" +
                  "<member name=\"member4\" type=\"ViewField\">" + 
                  "<claim value=\"claim1value\" type=\"claim1type\"/>" +
                  "</member>" +
                  "<member name=\"member4\" type=\"EditField\">" + 
                  "<claim value=\"claim1value\" type=\"claim1type\"/>" +
                  "</member>" +
                  "</class>" +
                  "<class name=\"class2\" fullname=\"ns.class2\">" + 
                  "<member name=\"member1\" type=\"ViewField\">" + 
                  "<claim value=\"claim2value\" type=\"claim2type\"/>" +
                  "</member>" + 
                  "<member name=\"member2\" type=\"EditField\">" + 
                  "<claim value=\"claim2value\" type=\"claim2type\"/>" +
                  "</member>" +
                  "<member name=\"member3\" type=\"Action\">" + 
                  "<claim value=\"claim2value\" type=\"claim2type\"/>" +
                  "</member>" +
                  "</class></testxml>"

    let authContext (id : IIdentifier) (ct : CheckType) = 
        new AuthorizationContext(claimsPrincipal, id.ToIdentityString IdentifierDepth.ClassName, ((int)ct).ToString())
 
    let viewAuthContext (id : IIdentifier) = authContext id CheckType.ViewField
      
    let editAuthContext (id : IIdentifier) = authContext id CheckType.EditField
      
    let actionAuthContext (id : IIdentifier) = authContext id CheckType.Action
      
    member x.CreateClaimsAuthManager() =
       let node = new XmlDocument()
       node.LoadXml testXml
       new NakedObjectsClaimsAuthorizationManager(node.FirstChild.ChildNodes)

    [<Test>] 
    member x.CreateClaimsAuthManagerTest() =
       let node = new XmlDocument()
       node.LoadXml testXml
       let cam = x.CreateClaimsAuthManager()
       ()

    [<Test>] 
    member x.AuthorisedViewProperty() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class1", "member1")
       let authContext = viewAuthContext id
       let auth = cam.CheckAccess authContext
       Assert.IsTrue(auth, "expect to be authorised")
       ()

    [<Test>] 
    member x.AuthorisedEditProperty() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class1", "member2")
       let authContext = editAuthContext id
       let auth = cam.CheckAccess authContext
       Assert.IsTrue(auth, "expect to be authorised")
       ()

    [<Test>] 
    member x.AuthorisedAction() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class1", "member3")
       let authContext = actionAuthContext id
       let auth = cam.CheckAccess authContext
       Assert.IsTrue(auth, "expect to be authorised")
       ()

    [<Test>] 
    member x.NotAuthorisedViewProperty() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class1", "member1")
       let authContext = editAuthContext id
       let auth = cam.CheckAccess authContext
       Assert.IsFalse(auth, "expect not to be authorised")
       ()

    [<Test>] 
    member x.NotAuthorisedEditProperty() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class1", "member2")
       let authContext = viewAuthContext id
       let auth = cam.CheckAccess authContext
       Assert.IsFalse(auth, "expect not to be authorised")
       ()

    [<Test>] 
    member x.NotAuthorisedActionWrongClaim() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class2", "member3")
       let authContext = actionAuthContext id 
       let auth = cam.CheckAccess authContext
       Assert.IsFalse(auth, "expect not to be authorised")
       ()

    [<Test>] 
    member x.NotAuthorisedViewPropertyWrongClaim() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class2", "member1")
       let authContext = viewAuthContext id
       let auth = cam.CheckAccess authContext
       Assert.IsFalse(auth, "expect not to be authorised")
       ()

    [<Test>] 
    member x.NotAuthorisedEditPropertyWrongClaim() = 
       let cam = x.CreateClaimsAuthManager()
       let id = new IdentifierImpl("ns.class2", "member2")
       let authContext = editAuthContext id
       let auth = cam.CheckAccess authContext
       Assert.IsFalse(auth, "expect not to be authorised")
       ()

    [<Test>]
    member x.AuthorisorIsVisible() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class1", "member1")
        let isVisible = authorisor.IsVisible(testSession, null, id)
        Assert.IsTrue(isVisible, "expect to be visible")

    [<Test>]
    member x.AuthorisorIsEditable() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class1", "member2")
        let isUsable = authorisor.IsUsable(testSession, null, id)
        Assert.IsTrue(isUsable, "expect to be visible")

    [<Test>]
    member x.AuthorisorIsNotVisible() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class2", "member1")
        let isVisible = authorisor.IsVisible(testSession, null, id)
        Assert.IsFalse(isVisible, "expect not to be visible")

    [<Test>]
    member x.AuthorisorIsNotEditable() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class1", "member1")
        let isUsable = authorisor.IsUsable(testSession, null, id)
        Assert.IsFalse(isUsable, "expect not to be editable")


    [<Test>]
    member x.AuthorisorActionIsVisible() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class1", "member3", [|""|])
        let isVisible = authorisor.IsVisible(testSession, null, id)
        Assert.IsTrue(isVisible, "expect to be visible")

    [<Test>]
    member x.AuthorisorActionIsEditable() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class1", "member3", [|""|])
        let isUsable = authorisor.IsUsable(testSession, null, id)
        Assert.IsTrue(isUsable, "expect to be usable")

    [<Test>]
    member x.AuthorisorActionIsNotVisible() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class2", "member3", [|""|])
        let isVisible = authorisor.IsVisible(testSession, null, id)
        Assert.IsFalse(isVisible, "expect not to be visible")

    [<Test>]
    member x.AuthorisorActionIsNotEditable() =
        let cam = x.CreateClaimsAuthManager()
        let authorisor = new WifAuthorizer(cam)
        let id =  new IdentifierImpl("ns.class2", "member3", [|""|])
        let isUsable = authorisor.IsUsable(testSession, null, id)
        Assert.IsFalse(isUsable, "expect not to be usable")