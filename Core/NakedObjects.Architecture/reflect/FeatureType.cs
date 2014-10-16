// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Enumerates the features that a particular annotation can be applied to.
    /// </summary>
    // TODO: should rationalize this and NakedObjectSpecification
    // notice though that we don't distinguish value properties and reference properties
    // (and we probably shouldn't in  NakedObjectSpecification, either).
    public sealed class FeatureType {
        public static readonly FeatureType Action;
        public static readonly FeatureType ActionParameter;
        public static readonly FeatureType[] ActionsAndParameters;
        public static readonly FeatureType[] ActionsOnly;
        public static readonly FeatureType Collection;
        public static readonly FeatureType[] CollectionsAndActions;
        public static readonly FeatureType[] CollectionsOnly;
        public static readonly FeatureType[] Everything;
        public static readonly FeatureType[] EverythingButParameters;
        public static readonly FeatureType Objects;
        public static readonly FeatureType[] ObjectsAndProperties;
        public static readonly FeatureType[] ObjectsOnly;
        public static readonly FeatureType[] ObjectsPropertiesAndCollections;
        public static readonly FeatureType[] ObjectsPropertiesAndParameters;
        public static readonly FeatureType[] ParametersOnly;
        public static readonly FeatureType[] PropertiesAndCollections;
        public static readonly FeatureType[] PropertiesAndParameters;
        public static readonly FeatureType[] PropertiesCollectionsAndActions;
        public static readonly FeatureType[] PropertiesOnly;
        public static readonly FeatureType Property;

        static FeatureType() {
            Objects = new FeatureType(0, "Object");
            Property = new FeatureType(1, "Property");
            Collection = new FeatureType(2, "Collection");
            Action = new FeatureType(3, "Action");
            ActionParameter = new FeatureType(4, "Parameter");

            ActionsOnly = new[] {Action};
            ActionsAndParameters = new[] {Action, ActionParameter};
            CollectionsOnly = new[] {Collection};
            CollectionsAndActions = new[] {Collection, Action};
            ObjectsOnly = new[] {Objects};
            ObjectsAndProperties = new[] {Objects, Property};
            ObjectsPropertiesAndCollections = new[] {Objects, Property, Collection};
            ObjectsPropertiesAndParameters = new[] {Objects, Property, ActionParameter};
            PropertiesOnly = new[] {Property};
            PropertiesAndCollections = new[] {Property, Collection};
            PropertiesAndParameters = new[] {Property, ActionParameter};
            PropertiesCollectionsAndActions = new[] {Property, Collection, Action};
            ParametersOnly = new[] {ActionParameter};
            Everything = new[] {Objects, Property, Collection, Action, ActionParameter};
            EverythingButParameters = new[] {Objects, Property, Collection, Action};
        }

        private FeatureType(int number, string nameInCode) {
            Number = number;
            Name = nameInCode;
        }

        public int Number { get; private set; }

        public string Name { get; private set; }

        public override string ToString() {
            return Name;
        }
    }
}