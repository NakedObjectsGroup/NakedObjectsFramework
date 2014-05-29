// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;
using System.Security.Principal;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    internal interface Interface1 {}

    internal interface Interface2 {}

    public class ObjectWithBasicProgramConventions : Interface1, Interface2 {
        public static string classActionValid;
        private readonly ArrayList collection = new ArrayList();
        public string objectActionValid;

        public string One {
            get { return ""; }
        
        }

        public ReferencedObject FieldTwo {
            get;
            set;
        }

        public ReferencedObject Three {
            get { return null; }
        }

        public static string Four {
            get { return ""; }
          
        }

        public IList Five {
            get { return collection; }
        }

        public string Six {
            get { return ""; }
           
        }

        public string Seven {
            get { return ""; }
           
        }

        public string Eight {
            get { return ""; }
          
        }

       

        public ArrayList Nine {
            get { return collection; }
        }

        public static string PluralName() {
            return "Plural";
        }

        public static string SingularName() {
            return "Singular";
        }

        public string DefaultOne() {
            return "default value";
        }

        public string[] ChoicesOne() {
            return new string[] {"four", "five", "six"};
        }

        public static bool OptionalOne() {
            return true;
        }

        public static string DisableOne(IPrincipal principal) {
            return "no edits";
        }

        public bool HideFieldTwo() {
            return true;
        }

        // field two should be hidden for  the user
        public static bool HideFieldTwo(IPrincipal principal) {
            Assert.AreEqual("unit tester", principal.Identity.Name);
            return true;
        }

        // this field should not be persisted as it has no setter

        public void AddToFive(ReferencedObject person) {}

        public void RemoveFromFive(ReferencedObject person) {}

        public static string NameFive() {
            return "five";
        }

        public static bool AlwaysHideSix() {
            return true;
        }

        public string[] ChoicesSix() {
            return new string[] {"one", "two"};
        }

        public string DisableSeven() {
            return "no changes";
        }

        public static bool ProtectEight() {
            return true;
        }

        public static string NameStop() {
            return "object action name";
        }

        public static string DescriptionStop() {
            return "object action description";
        }

        public string ValidateStart(string param) {
            return objectActionValid;
        }

        public void Stop() {}

        public static bool[] OptionalStart() {
            return new bool[] {true};
        }

        public string[] DefaultStart() {
            return new string[] {"default param"};
        }

        public string[][] ChoicesStart() {
            return new string[][] {new string[] {"one", "two", "three"}};
        }

        public void start2(string name) {}

        public object[] ChoicesStart2() {
            return new object[] {new string[] {"three", "two", "one"}};
        }

        public static string ValidateTop() {
            return classActionValid;
        }

        public static string[] NamesStart() {
            return new string[] {"parameter name"};
        }

        public int Start(string param) {
            return 1;
        }

        public static string NameTop() {
            return "class action name";
        }

        public static string DescriptionTop() {
            return "class action description";
        }

        public static void Top() {}

        public static void Bottom(string param) {}

        public static string ActionOrder() {
            // make sure there is an action which doesn't exist
            return "missing, Start, Stop";
        }

        public static string ClassActionOrder() {
            return "Top, Bottom";
        }

        public static string FieldOrder() {
            return "one, field two ,three, five";
        }

        // tests the hide method with same set of paramaters
        public static bool AlwaysHideHiddenAction(string param) {
            return true;
        }

        public void HiddenAction(string param) {}

        // tests the hide method with no paramaters
        public static bool AlwaysHideHiddenAction2() {
            return true;
        }

        public void HiddenAction2(string param1, string param2) {}

        public static bool HideHiddenToUser(IPrincipal principal) {
            Assert.AreEqual("unit tester", principal.Identity.Name);
            return true;
        }

        public void HiddenToUser() {}

        // method that would be run on client
        public void LocalRunOnClient() {}

        // method that would be run on the server
        public void RemoteRunOnServer() {}

        // method for debug access
        public string DebugTwo(string parameter) {
            return "action two";
        }

        // exploration method
        public void ExplorationSetUp() {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}