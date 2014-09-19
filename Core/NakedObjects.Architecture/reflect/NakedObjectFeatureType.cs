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
    public sealed class NakedObjectFeatureType {
        public static readonly NakedObjectFeatureType Action;
        public static readonly NakedObjectFeatureType ActionParameter;
        public static readonly NakedObjectFeatureType[] ActionsAndParameters;
        public static readonly NakedObjectFeatureType[] ActionsOnly;
        public static readonly NakedObjectFeatureType Collection;
        public static readonly NakedObjectFeatureType[] CollectionsAndActions;
        public static readonly NakedObjectFeatureType[] CollectionsOnly;
        public static readonly NakedObjectFeatureType[] Everything;
        public static readonly NakedObjectFeatureType[] EverythingButParameters;
        public static readonly NakedObjectFeatureType Objects;
        public static readonly NakedObjectFeatureType[] ObjectsAndProperties;
        public static readonly NakedObjectFeatureType[] ObjectsOnly;
        public static readonly NakedObjectFeatureType[] ObjectsPropertiesAndCollections;
        public static readonly NakedObjectFeatureType[] ObjectsPropertiesAndParameters;
        public static readonly NakedObjectFeatureType[] ParametersOnly;
        public static readonly NakedObjectFeatureType[] PropertiesAndCollections;
        public static readonly NakedObjectFeatureType[] PropertiesAndParameters;
        public static readonly NakedObjectFeatureType[] PropertiesCollectionsAndActions;
        public static readonly NakedObjectFeatureType[] PropertiesOnly;
        public static readonly NakedObjectFeatureType Property;

        static NakedObjectFeatureType() {
            Objects = new NakedObjectFeatureType(0, "Object");
            Property = new NakedObjectFeatureType(1, "Property");
            Collection = new NakedObjectFeatureType(2, "Collection");
            Action = new NakedObjectFeatureType(3, "Action");
            ActionParameter = new NakedObjectFeatureType(4, "Parameter");

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

        private NakedObjectFeatureType(int number, string nameInCode) {
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