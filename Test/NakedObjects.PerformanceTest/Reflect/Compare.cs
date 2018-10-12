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

namespace NakedObjects.SystemTest.Reflect {
    public static class CompareFunctions {

        public static void Compare(IMenuImmutable menu1, IMenuImmutable menu2) {

        }


        public static void Compare(IMemberSpecImmutable assoc1, IMemberSpecImmutable assoc2, string specName) {

            Compare(assoc1.ReturnSpec, assoc2.ReturnSpec);

            // make sure same spec
         
            CompareTypeName(assoc1.Name, assoc2.Name, specName);
            CompareTypeName(assoc1.Description, assoc2.Description, specName);

            // only carry on to compare facets if first time
            if (Compared.Contains(specName)) return;
            Compared.Add(specName);

            Compare(assoc1 as ISpecification, assoc1, specName);
        }

        public static void Compare(IAssociationSpecImmutable assoc1, IAssociationSpecImmutable assoc2, string specName) {
            Compare(assoc1.OwnerSpec, assoc2.OwnerSpec);
            Compare(assoc1 as IMemberSpecImmutable, assoc1, specName);
        }

        public static void Compare(IOneToManyAssociationSpecImmutable assoc1, IOneToManyAssociationSpecImmutable assoc2) {
            var specName = assoc1.Name;    
            Compare(assoc1.ElementSpec, assoc2.ElementSpec);
            Compare(assoc1, assoc1, specName);
        }

        public static void Compare(IOneToOneAssociationSpecImmutable assoc1, IOneToOneAssociationSpecImmutable assoc2) {
            var specName = assoc1.Name;
            Compare(assoc1, assoc1, specName);
        }

        public static void Compare(IActionParameterSpecImmutable actionParameter1, IActionParameterSpecImmutable actionParameter2) {
            var specName = actionParameter1.Identifier.ToIdentityString(IdentifierDepth.ClassNameParams);

            Compare(actionParameter1.Specification, actionParameter2.Specification);
            Compare(actionParameter1.IsChoicesEnabled, actionParameter2.IsChoicesEnabled, specName);
            Compare(actionParameter1.IsMultipleChoicesEnabled, actionParameter2.IsMultipleChoicesEnabled, specName);

            // only carry on to compare facets if first time
            if (Compared.Contains(specName)) return;
            Compared.Add(specName);

            Compare(actionParameter1, actionParameter2, specName);
        }

        public static void Compare(IActionParameterSpecImmutable[] actionParameters1, IActionParameterSpecImmutable[] actionParameters2) {
            CompareCount(actionParameters1.Cast<ISpecification>().ToList(), actionParameters2.Cast<ISpecification>().ToList());

            var oSpecs1 = actionParameters1.OrderBy((i) => i == null ? "" : i.Identifier.ToIdentityString(IdentifierDepth.ClassNameParams)).ToList();
            var oSpecs2 = actionParameters2.OrderBy((i) => i == null ? "" : i.Identifier.ToIdentityString(IdentifierDepth.ClassNameParams)).ToList();

            foreach (var a in oSpecs1.Zip(oSpecs2, (s1, s2) => new { s1, s2 })) {
                Compare(a.s1, a.s2);
            }
        }

        public static void Compare(IActionSpecImmutable action1, IActionSpecImmutable action2) {
            var specName = action1.Name;
            // make sure same spec
            Compare(action1.OwnerSpec, action2.OwnerSpec);
            Compare(action1.ElementSpec, action2.ElementSpec);
            Compare(action1.Parameters, action2.Parameters);
            Compare(action1.ReturnSpec, action2.ReturnSpec);
            Compare(action1.Description, action2.Description, specName);
            Compare(action1.Name, action2.Name, specName);

            // only carry on to compare facets if first time
            if (Compared.Contains(specName)) return;
            Compared.Add(specName);

            Compare(action1, action2, specName);
        }

        public static void Compare(IAssociationSpecImmutable field1, IAssociationSpecImmutable field2) {
            var specName = field1.Name;

            Compare(field1.OwnerSpec, field2.OwnerSpec);
            Compare(field1.ReturnSpec, field2.ReturnSpec);
            Compare(field1.Description, field2.Description, specName);
            Compare(field1.Name, field2.Name, specName);

            Compare(field1, field2, specName);
        }

        public static void Compare(IList<IActionSpecImmutable> actions1, IList<IActionSpecImmutable> actions2) {
            CompareCount(actions1.Cast<ISpecification>().ToList(), actions2.Cast<ISpecification>().ToList());

            var oSpecs1 = actions1.OrderBy((i) => i == null ? "" : i.Name).ToList();
            var oSpecs2 = actions2.OrderBy((i) => i == null ? "" : i.Name).ToList();

            foreach (var a in oSpecs1.Zip(oSpecs2, (s1, s2) => new { s1, s2 })) {
                Compare(a.s1, a.s2);
            }
        }

        public static void Compare(IList<IAssociationSpecImmutable> fields1, IList<IAssociationSpecImmutable> fields2) {
            CompareCount(fields1.Cast<ISpecification>().ToList(), fields2.Cast<ISpecification>().ToList());

            var oSpecs1 = fields1.OrderBy((i) => i == null ? "" : i.Name).ToList();
            var oSpecs2 = fields2.OrderBy((i) => i == null ? "" : i.Name).ToList();

            foreach (var a in oSpecs1.Zip(oSpecs2, (s1, s2) => new { s1, s2 })) {
                Compare(a.s1, a.s2);
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
            var fromS1 = s1.IndexOf('`');
            var fromS2 = s2.IndexOf('`');
            var trimmedS1 = fromS1 == -1 ? s1 : s1.Remove(fromS1);
            var trimmedS2 = fromS2 == -1 ? s2 : s2.Remove(fromS2);
            Compare(trimmedS1, trimmedS2, parent);
        }

        public static void Compare(string s1, string s2, string parent) {
            Assert.AreEqual(s1, s2, $"on {parent} no match {s1} {s2}");
        }

        public static void Compare(bool b1, bool b2, string parent) {
            Assert.AreEqual(b1, b2, $"on {parent} no match {b1} {b2}");
        }

        public static void Compare(IIdentifier i1, IIdentifier i2, string parent) {
            Assert.AreEqual(i1.ToIdentityString(IdentifierDepth.ClassNameParams), i2.ToIdentityString(IdentifierDepth.ClassNameParams), $"on {parent} no match {i1} {i2}");
        }

        public static IList<string> Compared = new List<string>();

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
                    Assert.AreEqual(s, o2 as string);
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
                    Assert.AreEqual(os.Name, os2.Name);
                    Assert.AreEqual(os.DeclaringType, os2.DeclaringType);
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
            var properties1 = facet1.GetType().GetProperties();
            var properties2 = facet2.GetType().GetProperties();

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
                    Compare(v1, v2, specName);
                }
            }
        }

        public static void Compare(IFacet facet1, IFacet facet2, string specName) {
            //Compare(facet1.Specification.Identifier, facet2.Specification.Identifier, specName);
            Compare(facet1.FacetType, facet2.FacetType, specName);
            Compare(facet1.IsNoOp, facet2.IsNoOp, specName);
            Compare(facet1.CanAlwaysReplace, facet2.CanAlwaysReplace, specName);
            Compare(facet1.GetType(), facet2.GetType(), specName);

            CompareReflectively(facet1, facet2, specName);
        }

        public static void Compare(IEnumerable<IFacet> facets1, IEnumerable<IFacet> facets2, string specName) {
            Assert.AreEqual(facets1.Count(), facets2.Count(), "facets count no match");

            var oFacets1 = facets1.OrderBy((i) => i == null ? "" : i.ToString()).ToList();
            var oFacets2 = facets2.OrderBy((i) => i == null ? "" : i.ToString()).ToList();

            foreach (var a in oFacets1.Zip(oFacets2, (f1, f2) => new { f1, f2 })) {
               Compare(a.f1, a.f2, specName);
            }
        }

        public static void Compare(ISpecification spec1, ISpecification spec2, string specName) {

            Compare(spec1.FacetTypes, spec2.FacetTypes, specName);
            //Compare(spec1.Identifier, spec2.Identifier, specName);

            Compare(spec1.GetFacets(), spec2.GetFacets(), specName);
        }


        public static void Compare(ITypeSpecImmutable spec1, ITypeSpecImmutable spec2) {

            if (spec1 == null && spec2 == null) return;

            var specName = spec1.FullName;

            // make sure same spec
            Compare(spec1.GetType(), spec2.GetType(), specName);
            Compare(spec1.Type, spec2.Type, specName);
            CompareTypeName(spec1.FullName, spec2.FullName, specName);

            // only carry on down if first time
            if (Compared.Contains(specName)) return;
            Compared.Add(specName);
         
            Compare(spec1.ShortName, spec2.ShortName, specName);
            Compare(spec1.ObjectMenu, spec2.ObjectMenu);
            Compare(spec1.ObjectActions, spec2.ObjectActions);
            Compare(spec1.ContributedActions, spec2.ContributedActions);
            Compare(spec1.CollectionContributedActions, spec2.CollectionContributedActions);
            Compare(spec1.FinderActions, spec2.FinderActions);
            Compare(spec1.Fields, spec2.Fields);
            Compare(spec1.Interfaces, spec2.Interfaces);
            Compare(spec1.Subclasses, spec2.Subclasses); // todo possible bug here with populating subclasses
            Compare(spec1.Superclass, spec2.Superclass);
            Compare(spec1.IsObject, spec2.IsObject, specName);
            Compare(spec1.IsCollection, spec2.IsCollection, specName);
            Compare(spec1.IsQueryable, spec2.IsQueryable, specName);
            Compare(spec1.IsParseable, spec2.IsParseable, specName);
            Compare(spec1, spec2, specName);
        }

    }
}
