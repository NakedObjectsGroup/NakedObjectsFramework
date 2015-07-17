using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using NakedObjects;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Menu;
using NakedObjects.Reflect.FacetFactory;

namespace Sdm.App
{
    public class Inspector
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public INakedObjectsFramework Framework { set; protected get; }
        #endregion

        #region Meta-model based actions
        public string ListActionParamsWithFindMenus()
        {
            var div = new TagBuilder("div");
            foreach (var p in AllReferenceParams().Where(p => p.IsFindMenuEnabled))
            {
                var action = p.Action;
                var specImm = GetImmutableSpec(p.Spec);
                if (!action.IsContributedMethod ||
                    !action.GetFacet<IContributedActionFacet>().IsContributedTo(specImm))
                {
                    var par = new TagBuilder("p");
                    par.InnerHtml += action.OnSpec.FullName + "#" + action.Name + "  (" + p.Name + ")";
                    div.InnerHtml += par;
                    var finders = p.Spec.GetFinderActions();
                    var ul = new TagBuilder("ul");
                    if (finders.Count() == 0)
                    {
                        var li = new TagBuilder("li");
                        li.InnerHtml = "No Finder Actions";
                        ul.InnerHtml += li;
                    }
                    else
                    {
                        foreach (var finder in finders)
                        {
                            var li = new TagBuilder("li");
                            li.InnerHtml = finder.Name;
                            ul.InnerHtml += li;
                        }
                    }
                    div.InnerHtml += ul;
                }
            }
            return div + Environment.NewLine;
        }

        public string ListActions([Optionally]string startingWith)
        {
            var div = new TagBuilder("div");
            var actions = AllActions();
            if (startingWith != null)
            {
                actions = actions.Where(a => a.Identifier.MemberName.StartsWith(startingWith));
            }
            if (actions.Count() == 0)
            {
                div.InnerHtml += "No matching actions found";
            }
            foreach (var action in actions.OrderBy(a => a.OnSpec.FullName).ThenBy(a => a.Name))
            {
                var par = new TagBuilder("p");
                par.InnerHtml += action.OnSpec.FullName + "#" + action.Name;
                div.InnerHtml += par;
            }
            return div + Environment.NewLine;
        }

        public string ListObjectSpecs([Optionally] string startingWith)
        {
            var div = new TagBuilder("div");
            var specs = AllObjectSpecs();
            if (startingWith != null)
            {
                specs = specs.Where(a => a.Identifier.ClassName.StartsWith(startingWith));
            }
            if (specs.Count() == 0)
            {
                div.InnerHtml += "No matching objects found";
            }
            foreach (var spec in specs.OrderBy(s => s.Identifier.ClassName))
            {
                var par = new TagBuilder("p");
                par.InnerHtml += spec.FullName;
                div.InnerHtml += par;
            }
            return div + Environment.NewLine;
        }

        #region Private helpers

        private IEnumerable<ITypeSpec> AllObjectSpecs() {
            var mgr = Framework.MetamodelManager;
            return mgr.Metamodel.AllSpecifications.Select(s => mgr.GetSpecification(s));
        }

        private IEnumerable<IActionSpec> AllActions() {
            return AllObjectSpecs().SelectMany(os => os.GetActions());
        }

        private IEnumerable<IActionParameterSpec> AllParams() {
            return AllActions().SelectMany(a => a.Parameters);
        }

        private IEnumerable<IOneToOneActionParameterSpec> AllReferenceParams() {
            return AllParams().OfType<IOneToOneActionParameterSpec>();
        }

        private IObjectSpecImmutable GetImmutableSpec(IObjectSpec spec) {
            return Framework.MetamodelManager.Metamodel.GetSpecification(spec.FullName) as IObjectSpecImmutable;
        }
        #endregion
        #endregion
        #region Reflection-based actions
        public string PossibleNOFRecognisedMethodsMarkedNakedObjectsIgnore(string withinAssemblyNamesStarting)
        {
            var assemblies = MatchingAssemblies(withinAssemblyNamesStarting);
            var allMethods = assemblies.SelectMany(a => a.GetTypes()).SelectMany(x => x.GetMethods()).Where(m => m.IsPublic);
            var matchingMethods = from MethodInfo m in allMethods
                                  from string name in RecognisedMethodsAndPrefixes.RecognisedMethods
                                  from string prefix in RecognisedMethodsAndPrefixes.RecognisedPrefixes
                                  where (m.Name == name || m.Name.StartsWith(prefix)) &&
                                  m.GetCustomAttributes(typeof(NakedObjectsIgnoreAttribute), false).Count() > 0
                                  select m; ;
            var methodNames = matchingMethods.OrderBy(m => m.DeclaringType.FullName).ThenBy(m => m.Name).Select(m => m.DeclaringType.FullName + "#" + m.Name)
                .Distinct();
            return AsSimpleReportWithCount(methodNames, "Potentially NOF-recognised public methods marked [NakedObjectsIgnore]");
        }

        public string PropertiesMarkedNakedObjectsIgnore(string withinAssemblyNamesStarting, bool excludePropsEndingInId)
        {
            var matchingProps = GetIgnoredProperties(withinAssemblyNamesStarting, excludePropsEndingInId);
            var propNames = PropertiesAsReportLines(matchingProps); 
            return AsSimpleReportWithCount(propNames, "Properties marked [NakedObjectsIgnore]");
        }

        public string OverriddenPropertiesMarkedNakedObjectsIgnore(string withinAssemblyNamesStarting, bool excludePropsEndingInId)
        {
            var matchingProps = GetIgnoredProperties(withinAssemblyNamesStarting, excludePropsEndingInId);
            matchingProps = matchingProps.Where(p => PropertyIsOverridden(p));
            var propNames = PropertiesAsReportLines(matchingProps);
            return AsSimpleReportWithCount(propNames, "Overridden properties marked [NakedObjectsIgnore]");
        }

        public string TypesThatImplementIViewModel(string withinAssemblyNamesStarting)
        {
            var assemblies = MatchingAssemblies(withinAssemblyNamesStarting);
            Type vm = typeof(IViewModel);
            var types = assemblies.SelectMany(a => a.GetTypes()).Where(t =>vm.IsAssignableFrom(t));
            //TODO:  abstract types?
            var typeNames = types.OrderBy(t =>t.FullName).Select(t => t.FullName).Distinct();
            return AsSimpleReportWithCount(typeNames, "Types implementing IViewModel");
        }

        public string TypesMarkedNotPersisted(string withinAssemblyNamesStarting)
        {
            var assemblies = MatchingAssemblies(withinAssemblyNamesStarting);
            var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.GetCustomAttributes(typeof(NotPersistedAttribute), false).Count() > 0);
            var typeNames = types.OrderBy(t => t.FullName).Select(t => t.FullName).Distinct();
            return AsSimpleReportWithCount(typeNames, "Types marked [NotPersisted]");
        }

        public string AllTypesInACluster(string clusterName)
        {
            var assemblies = MatchingAssemblies("Sdm.Cluster."+clusterName);
            var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.GetCustomAttributes(typeof(NotPersistedAttribute), false).Count() > 0);
            var typeNames = types.OrderBy(t => t.FullName).Select(t => t.FullName).Distinct();
            return AsSimpleReportWithCount(typeNames, "Types marked [NotPersisted]");
        }

        #region Private Helpers
        private bool PropertyIsOverridden(PropertyInfo prop)
        {
            string name = prop.Name;
            var baseType = prop.DeclaringType.BaseType;
            return baseType == null ? false : baseType.GetProperties().Any(p => p.Name == name);
        }

        private static IEnumerable<PropertyInfo> GetIgnoredProperties(string withinAssemblyNamesStarting, bool excludePropsEndingInId)
        {
            var assemblies = MatchingAssemblies(withinAssemblyNamesStarting);
            var allProps = assemblies.SelectMany(a => a.GetTypes()).SelectMany(x => x.GetProperties());
            var matchingProps = from PropertyInfo p in allProps
                                where p.GetCustomAttributes(typeof(NakedObjectsIgnoreAttribute), false).Count() > 0 &&
                                (!excludePropsEndingInId || !p.Name.ToUpper().EndsWith("ID"))
                                select p;
            return matchingProps;
        }

        private static IEnumerable<Assembly> MatchingAssemblies(string withinAssemblyNamesStarting)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.ToUpper().StartsWith(withinAssemblyNamesStarting.ToUpper()));
            return assemblies;
        }

        private string AsSimpleReportWithCount(IEnumerable<string> lines, string heading = null)
        {
            var div = new TagBuilder("div");
            if (heading != null)
            {
                var h3 = new TagBuilder("h3");
                h3.InnerHtml += lines.Count() +" "+ heading;
                div.InnerHtml += h3;
            }
            foreach (var line in lines)
            {
                var par = new TagBuilder("p");
                par.InnerHtml += line;
                div.InnerHtml += par;
            }
            return div + Environment.NewLine;
        }

        private static IEnumerable<string> PropertiesAsReportLines(IEnumerable<PropertyInfo> matchingProps)
        {
            return matchingProps.OrderBy(p => p.DeclaringType.FullName).ThenBy(p => p.Name).Select(p => p.DeclaringType.FullName + "#" + p.Name + " (" + p.PropertyType.Name + ")").Distinct();
        }
        #endregion
        #endregion
    }
}