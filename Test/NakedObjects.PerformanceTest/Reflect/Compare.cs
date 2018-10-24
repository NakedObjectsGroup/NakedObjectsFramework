using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Menu;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Menu;
using Sdm.Test.Fixtures.Clusters.Payments.PaymentContributedActions.Actions;

namespace NakedObjects.SystemTest.Reflect {
    public static class CompareFunctions {

        public static void Compare(IMenu menu1, IMenu menu2, string name) {

            if (menu1 == null && menu2 == null) return;

            Compare(menu1.Type, menu2.Type, name);
        }

        public static void Compare(MenuAction menu1, MenuAction menu2, string name) {
            name = name + ":" + menu1.Action.Name;
            Compare(menu1.Action, menu2.Action, name);
        }

        public static void Compare(MenuImpl menu1, MenuImpl menu2, string name) {
            Compare(menu1 as IMenu, menu2, name);
            Compare(menu1.SuperMenu, menu2.SuperMenu, name);           
        }

        public static void Compare(IMenuItemImmutable menu1, IMenuItemImmutable menu2) {

            if (menu1 == null && menu2 == null) return;

            var name = menu1.Name;
            Compare(menu1.Name, menu2.Name, name);
            Compare(menu1.Id, menu2.Id, name);

            switch (menu1) {
                case MenuAction m:
                    Compare(m, menu2 as MenuAction, name);
                    break;
                case MenuImpl m:
                    Compare(m, menu2 as MenuImpl, name);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        public static string ToOrderString(this IMenuItemImmutable menu, bool print = false) {
            if (menu == null) return "";
            string id = "";
            switch (menu) {
                case MenuAction m:
                    var facet = m.Action.GetFacet<IActionInvocationFacet>();
                    var suffix = "";
                    if (facet != null) {
                        suffix = facet.OrderString();
                    }

                    id = m.Name + ":" + m.Id + ":" + m.Action.ToIdString() + suffix;
                    break;

                default:
                    id = menu.Name + ":" + menu.Id;
                    break;
            }

            if (print) {
                Console.WriteLine("menu: " + id);
            }

            return id;
        }


        public static void Compare(IList<IMenuItemImmutable> menu1, IList<IMenuItemImmutable> menu2) {

            var oMenus1 = menu1.OrderBy(i => i.ToOrderString()).ToList();
            var oMenus2 = menu2.OrderBy(i => i.ToOrderString()).ToList();

            // check ordering 

            var os1 = oMenus1.Select(o => o.ToOrderString()).ToArray();
            var os2 = oMenus2.Select(o => o.ToOrderString()).ToArray();

            foreach (var a in os1.Zip(os2, (m1, m2) => new { m1, m2 })) {
                Assert.AreEqual(a.m1, a.m2);
                Assert.AreEqual(1, os1.Count(o => o == a.m1), a.m1);
                Assert.AreEqual(1, os2.Count(o => o == a.m2), a.m2);

                //DebugMenu(os1, a.m1, oMenus1);
            }

            //foreach (var a in oMenus1.Zip(oMenus2, (m1, m2) => new { m1, m2 })) {
            //    Compare(a.m1, a.m2);
            //}
        }

        private static void DebugMenu(string[] os1, string m1, List<IMenuItemImmutable> oMenus1) {
            if (os1.Count(o => o == m1) > 1) {
                var os = oMenus1.Where(i => i.ToOrderString() == m1).ToArray();
                var o1 = os.ElementAt(0);
                var o2 = os.ElementAt(1);

                Compare(o1, o2);
            }
        }

        public static void Compare(IMenuImmutable menu1, IMenuImmutable menu2) {
            if (menu1 == null && menu2 == null) return;

            Compare(menu1 as IMenuItemImmutable, menu2);
            Compare(menu1.MenuItems, menu2.MenuItems);
        }

        public static string ToIdString(this IIdentifier id) {
            return id.ToIdentityString(IdentifierDepth.ClassNameParams);
        }

        public static string ToIdString(this ISpecification spec) {
            var id = spec.Identifier.ToIdString();
            //Console.WriteLine("spec id :" + id);
            return id;
        }

        public static string ToIdString(this IAssociationSpecImmutable spec) {
            return spec.Identifier.ToIdString() + spec.ReturnSpec.Identifier.ToIdString();
        }

        public static bool HasBeenCompared(ISpecification spec, object to) {

            Assert.AreNotSame(spec, to);

            // only carry on to compare facets if first time
            if (Compared.ContainsKey(spec)) {
                Assert.AreSame(Compared[spec], to);
                return true;
            }

            Compared[spec] = to;
            return false;
        }

        public static void Compare(IMemberSpecImmutable assoc1, IMemberSpecImmutable assoc2, string specName) {



            // make sure same spec
            Compare(assoc1.Identifier, assoc2.Identifier, specName);

            CompareTypeName(assoc1.Name, assoc2.Name, specName);
            CompareTypeName(assoc1.Description, assoc2.Description, specName);


            Compare(assoc1.ReturnSpec, assoc2.ReturnSpec, specName);

            // only carry on to compare facets if first time
            if (HasBeenCompared(assoc1, assoc2)) {
                return;
            }

            Compare(assoc1 as ISpecification, assoc2, specName);
        }

        //public static void Compare(IAssociationSpecImmutable assoc1, IAssociationSpecImmutable assoc2, string specName) {
        //    Compare(assoc1 as IMemberSpecImmutable, assoc2, specName);
        //    Compare(assoc1.OwnerSpec, assoc2.OwnerSpec);
        //}

        public static void Compare(IOneToManyAssociationSpecImmutable assoc1, IOneToManyAssociationSpecImmutable assoc2) {
            var specName = assoc1.Name;    
            Compare(assoc1, assoc2, specName);
            Compare(assoc1.ElementSpec, assoc2.ElementSpec);
        }

        public static void Compare(IOneToOneAssociationSpecImmutable assoc1, IOneToOneAssociationSpecImmutable assoc2) {
            var specName = assoc1.Name;
            Compare(assoc1, assoc2, specName);
        }

        public static void Compare(IActionParameterSpecImmutable actionParameter1, IActionParameterSpecImmutable actionParameter2) {
            var specName = actionParameter1.Identifier.ToIdString();

            Compare(actionParameter1.Specification, actionParameter2.Specification);
            Compare(actionParameter1.IsChoicesEnabled, actionParameter2.IsChoicesEnabled, specName);
            Compare(actionParameter1.IsMultipleChoicesEnabled, actionParameter2.IsMultipleChoicesEnabled, specName);

            // only carry on to compare facets if first time
            if (HasBeenCompared(actionParameter1, actionParameter2)) {
                return;
            }

            Compare(actionParameter1, actionParameter2, specName);
        }

        public static void Compare(IActionParameterSpecImmutable[] actionParameters1, IActionParameterSpecImmutable[] actionParameters2) {
            CompareCount(actionParameters1.Cast<ISpecification>().ToList(), actionParameters2.Cast<ISpecification>().ToList());

            var oSpecs1 = actionParameters1.OrderBy((i) => i == null ? "" : i.ToIdString()).ToList();
            var oSpecs2 = actionParameters2.OrderBy((i) => i == null ? "" : i.ToIdString()).ToList();

            foreach (var a in oSpecs1.Zip(oSpecs2, (s1, s2) => new { s1, s2 })) {
                Compare(a.s1, a.s2);
            }
        }

        public static void Compare(IActionSpecImmutable action1, IActionSpecImmutable action2) {
            var specName = action1.Identifier.ToIdString();
            // make sure same spec
            Compare(action1.Name, action2.Name, specName);
            Compare(action1.Description, action2.Description, specName);
            Compare(action1.OwnerSpec, action2.OwnerSpec, specName);
            Compare(action1.ElementSpec, action2.ElementSpec, specName);
            Compare(action1.Parameters, action2.Parameters);
            Compare(action1.ReturnSpec, action2.ReturnSpec);
          

            // only carry on to compare facets if first time
            if (HasBeenCompared(action1, action2)) {
                return;
            }

            Compare(action1, action2, specName);
        }

        public static void Compare(IAssociationSpecImmutable field1, IAssociationSpecImmutable field2, string ownerName) {
            var specName = ownerName + ":" + field1.Name;

            Compare(field1.Name, field2.Name, specName);
            Compare(field1.Description, field2.Description, specName);
            Compare(field1.OwnerSpec, field2.OwnerSpec);
            Compare(field1.ReturnSpec, field2.ReturnSpec);

            Compare(field1 as IMemberSpecImmutable, field2, specName);
        

            //Compare(field1, field2, specName);
        }

        public static string ToOrderString(this IActionSpecImmutable spec) {
            if (spec == null) return "";

            var facet = spec.GetFacet<IActionInvocationFacet>();
            var suffix = "";
            if (facet != null) {
                suffix = facet.OrderString();
            }


            return spec.ToIdString() + " : " + suffix;
        }


        public static void Compare(IList<IActionSpecImmutable> actions1, IList<IActionSpecImmutable> actions2) {
            CompareCount(actions1.Cast<ISpecification>().ToList(), actions2.Cast<ISpecification>().ToList());

            var oSpecs1 = actions1.OrderBy(i => i.ToOrderString()).ToList();
            var oSpecs2 = actions2.OrderBy(i => i.ToOrderString()).ToList();


            var os1 = oSpecs1.Select(o => o.ToOrderString()).ToArray();
            var os2 = oSpecs2.Select(o => o.ToOrderString()).ToArray();

            foreach (var a in os1.Zip(os2, (m1, m2) => new { m1, m2 })) {
                Assert.AreEqual(a.m1, a.m2);
                Assert.AreEqual(1, os1.Count(o => o == a.m1), a.m1);
                Assert.AreEqual(1, os2.Count(o => o == a.m2), a.m2);

                //DebugMenu(os1, a.m1, oMenus1);
            }

            foreach (var a in oSpecs1.Zip(oSpecs2, (s1, s2) => new { s1, s2 })) {
                Compare(a.s1, a.s2);
            }
        }

        public static void Compare(IList<IAssociationSpecImmutable> fields1, IList<IAssociationSpecImmutable> fields2, string ownerName) {
            CompareCount(fields1.Cast<ISpecification>().ToList(), fields2.Cast<ISpecification>().ToList());

            var oSpecs1 = fields1.OrderBy((i) => i == null ? "" : i.ToIdString()).ToList();
            var oSpecs2 = fields2.OrderBy((i) => i == null ? "" : i.ToIdString()).ToList();

            foreach (var a in oSpecs1.Zip(oSpecs2, (s1, s2) => new { s1, s2 })) {
                //Compare(a.s1, a.s2, ownerName);
            }
        }

        public static void CompareCount(IList<ISpecification> specs1, IList<ISpecification> specs2) {
            if (specs1.Count == specs2.Count) return;

            var ids1 = specs1.Select(s => s.Identifier).ToList();
            var ids2 = specs2.Select(s => s.Identifier).ToList();

            foreach (var id in ids1) {
                if (!ids2.Contains(id)) {
                    Console.WriteLine($"missing {id} in ids2");
                }
            }

            foreach (var id in ids2) {
                if (!ids1.Contains(id)) {
                    Console.WriteLine($"missing {id} in ids1");
                }
            }

            if (specs1.Count != specs2.Count) {
                var names1 = specs1.Select(s => s.ToIdString()).ToArray();

                var names2 = specs2.Select(s => s.ToIdString()).ToArray();

                foreach (var name in names1) {
                    if (!names2.Contains(name)) {
                        Console.WriteLine("missing spec in specs2: " + name);
                    }
                }

                foreach (var name in names2) {
                    if (!names1.Contains(name)) {
                        Console.WriteLine("missing spec in specs1: " + name);
                    }
                }
            }


            Assert.AreEqual(specs1.Count, specs2.Count, "specs count no match");

        }

        public static void CompareCount(Type[] types1, Type[] types2, string specName) {
            if (types1.Length == types2.Length) return;

            var ids1 = types1.Select(s => s.FullName).ToList();
            var ids2 = types2.Select(s => s.FullName).ToList();

            foreach (var id in ids1) {
                if (!ids2.Contains(id)) {
                    Console.WriteLine($"missing {id} in ids2");
                }
            }

            foreach (var id in ids2) {
                if (!ids1.Contains(id)) {
                    Console.WriteLine($"missing {id} in ids1");
                }
            }

            Assert.AreEqual(types1.Length, types2.Length, $"{specName} types count no match");
        }


        public static void Compare(Type t1, Type t2, string specName) {
            if (t1.IsGenericType && t2.IsGenericType) {
                Assert.AreEqual(t1.GetGenericTypeDefinition(), t2.GetGenericTypeDefinition(), $"on {specName} no match {t1} {t2}");
            }
            else if (t1.IsArray && t2.IsArray) {
                // ok
            }
            else {
                Assert.AreEqual(t1, t2, $"on {specName} no match {t1} {t2}");
            }
        }


        public static void Compare(IList<ITypeSpecImmutable> specs1, IList<ITypeSpecImmutable> specs2) {
            CompareCount(specs1.Cast<ISpecification>().ToList(), specs2.Cast<ISpecification>().ToList());
            // ordering doesn't matter
            var oSpecs1 = specs1.OrderBy((i) => i == null ? "" : i.FullName).ToList();
            var oSpecs2 = specs2.OrderBy((i) => i == null ? "" : i.FullName).ToList();

            foreach (var a in oSpecs1.Zip(oSpecs2, (s1, s2) => new { s1, s2 })) {
                Compare(a.s1, a.s2);
            }
        }

        public static void CompareTypeName(string s1, string s2, string parent) {
            //if (s1.Contains("[]") && s2.Contains("[]")) {
            //    return;
            //}

            var fromS1 = s1.IndexOf('`');
            var fromS2 = s2.IndexOf('`');
            var trimmedS1 = fromS1 == -1 ? s1 : s1.Remove(fromS1);
            var trimmedS2 = fromS2 == -1 ? s2 : s2.Remove(fromS2);
            Compare(trimmedS1, trimmedS2, parent);
        }

        public static void Compare(string s1, string s2, string parent) {
            if (s1 == null && s2 == null) {
                return;
            }

            if (s1.Contains("[]") && s2.Contains("[]")) {
                return;
            }

            Assert.AreEqual(s1, s2, $"on {parent} no match {s1} {s2}");
        }

        public static void Compare(bool b1, bool b2, string parent) {
            Assert.AreEqual(b1, b2, $"on {parent} no match {b1} {b2}");
        }

        public static void Compare(IIdentifier i1, IIdentifier i2, string parent) {
            Assert.AreEqual(i1.ToIdString(), i2.ToIdString(), $"on {parent} no match {i1} {i2}");
        }

        public static IDictionary<object, object> Compared = new Dictionary<object, object>();

        public static void Compare(Type[] types1, Type[] types2, string specName) {
            CompareCount(types1, types2, specName);

            var oTypes1 = types1.OrderBy((i) => i == null ? "" : i.FullName).ToList();
            var oTypes2 = types2.OrderBy((i) => i == null ? "" : i.FullName).ToList();

            foreach (var a in oTypes1.Zip(oTypes2, (t1, t2) => new { t1, t2 })) {
                Assert.AreEqual(a.t1, a.t2, $"on {specName} no match {a.t1} {a.t2}");
            }
        }

        public static void Compare(IEnumerable e1, IEnumerable e2, string specName) {
            foreach (var a in e1.Cast<object>().Zip(e2.Cast<object>(), (o1, o2) => new {o1, o2})) {
                Compare(a.o1, a.o2, specName);
            }
        }

        public static void Compare(object o1, object o2, string specName) {
            switch (o1) {
                case string s:
                    Assert.AreEqual(s, o2 as string, specName);
                    break;
                case IEnumerable e:
                    Compare(e, o2 as IEnumerable, specName);
                    break;
                case ITypeSpecImmutable os:
                    Compare(os, o2 as ITypeSpecImmutable);
                    break;
                case IActionSpecImmutable os:
                    Compare(os, o2 as IActionSpecImmutable);
                    break;
                case IOneToManyAssociationSpecImmutable os:
                    Compare(os, o2 as IOneToManyAssociationSpecImmutable);
                    break;
                case IOneToOneAssociationSpecImmutable os:
                    Compare(os, o2 as IOneToOneAssociationSpecImmutable);
                    break;
                case IActionParameterSpecImmutable os:
                    Compare(os, o2 as IActionParameterSpecImmutable);
                    break;
                case IFacet os:
                    Compare(os, o2 as IFacet, specName);
                    break;
                case Type os:
                    Compare(os, o2 as Type, specName);
                    break;
                case MethodInfo os:
                    var os2 = o2 as MethodInfo;
                    Assert.AreEqual(os.Name, os2.Name, specName);
                    Assert.AreEqual(os.DeclaringType, os2.DeclaringType, specName + ":" + os.Name);
                    break;
                case Regex re:
                    Assert.AreEqual(re.ToString(), o2.ToString());
                    break;
                case Tuple<string, IObjectSpecImmutable> t:
                    var t2 = o2 as Tuple<string, IObjectSpecImmutable>;
                    Assert.AreEqual(t.Item1, t2.Item1);
                    Compare(t.Item2, t2.Item2);
                    break;
                default:
                    Assert.Fail("debug here");
                    break;
            }
        }

        public static void CompareReflectively(object facet1, object facet2, string specName) {
            var properties1 = facet1.GetType().GetProperties().OrderBy(p => p.Name);
            var properties2 = facet2.GetType().GetProperties().OrderBy(p => p.Name);

            foreach (var a in properties1.Zip(properties2, (p1, p2) => new { p1, p2 })) {
                var v1 = a.p1.GetValue(facet1);
                var v2 = a.p2.GetValue(facet2);

                if (v1 == null) {
                    Assert.IsNull(v2);
                }
                else if (v1.GetType().IsValueType) {
                    Assert.AreEqual(v1, v2);
                }
                else {
                    Compare(v1, v2, specName + a.p1.Name);
                }
            }
        }

        public static void Compare(IFacet facet1, IFacet facet2, string specName) {
            //Compare(facet1.Specification.Identifier, facet2.Specification.Identifier, specName);
            Compare(facet1.FacetType, facet2.FacetType, specName);
            Compare(facet1.GetType(), facet2.GetType(), specName);
            Compare(facet1.IsNoOp, facet2.IsNoOp, specName);
            Compare(facet1.CanAlwaysReplace, facet2.CanAlwaysReplace, specName);
           
            CompareReflectively(facet1, facet2, facet1.GetType() + " : " +  specName);
        }

        public static string OrderString(this IFacet facet) {
            if (facet == null) return "";


            if (facet.GetType() == typeof(ActionInvocationFacetViaMethod)) {
                var f = (ActionInvocationFacetViaMethod) facet;
                return f.GetType().ToString() +
                       f.ActionMethod.Name +
                       f.ActionMethod.DeclaringType.FullName;
            }

            if (facet.GetType() == typeof(INamedFacet)) {
                var f = (INamedFacet)facet;
                return f.GetType().ToString() + f.NaturalName;
            }


            return facet.GetType().ToString();
        }


        public static void Compare(IEnumerable<IFacet> facets1, IEnumerable<IFacet> facets2, string specName) {
            Assert.AreEqual(facets1.Count(), facets2.Count(), "facets count no match");

            var oFacets1 = facets1.OrderBy(i => i.OrderString()).ToList();
            var oFacets2 = facets2.OrderBy(i => i.OrderString()).ToList();

            foreach (var a in oFacets1.Zip(oFacets2, (f1, f2) => new { f1, f2 })) {
               Compare(a.f1, a.f2, specName);
            }
        }

        public static void Compare(ISpecification spec1, ISpecification spec2, string specName) {

            Compare(spec1.FacetTypes, spec2.FacetTypes, specName);
            CompareTypeName(spec1.ToIdString(), spec2.ToIdString(), specName);

            Compare(spec1.GetFacets(), spec2.GetFacets(), specName);
        }


        public static void Compare(ITypeSpecImmutable spec1, ITypeSpecImmutable spec2, string name = "") {

            if (spec1 == null && spec2 == null) return;

            if (spec1 == null) {
                Assert.Fail("Missing spec1 " + name + " : " +spec2.FullName);
            }

            if (spec2 == null) {
                Assert.Fail("Missing spec2 " + name + " : " + spec1.FullName);
            }

            var specName = name + " : " + spec1.FullName;

            // make sure same spec
            CompareTypeName(spec1.FullName, spec2.FullName, specName);
            Compare(spec1.GetType(), spec2.GetType(), specName);
            Compare(spec1.Type, spec2.Type, specName);
            
            // only carry on to compare facets if first time
            if (HasBeenCompared(spec1, spec2)) {
                return;
            }

            Compare(spec1.ShortName, spec2.ShortName, specName);
            Compare(spec1.ObjectMenu, spec2.ObjectMenu);
            Compare(spec1.ObjectActions, spec2.ObjectActions);
            Compare(spec1.ContributedActions, spec2.ContributedActions);
            Compare(spec1.CollectionContributedActions, spec2.CollectionContributedActions);
            Compare(spec1.FinderActions, spec2.FinderActions);
            Compare(spec1.Fields, spec2.Fields, specName);
            Compare(spec1.Interfaces, spec2.Interfaces);
            //Compare(spec1.Subclasses, spec2.Subclasses); // todo possible bug here with populating subclasses
            //Compare(spec1.Superclass, spec2.Superclass);
            Compare(spec1.IsObject, spec2.IsObject, specName);
            Compare(spec1.IsCollection, spec2.IsCollection, specName);
            Compare(spec1.IsQueryable, spec2.IsQueryable, specName);
            Compare(spec1.IsParseable, spec2.IsParseable, specName);
            Compare(spec1 as ISpecification, spec2, specName);
        }

    }
}
