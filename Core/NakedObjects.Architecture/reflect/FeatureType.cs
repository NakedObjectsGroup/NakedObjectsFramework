// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Enumerates the features that a particular annotation can be applied to.
    /// </summary>
    [Flags]
    public enum FeatureType {
        None = 0,
        Objects = 1,
        Property = 2,
        Collections = 4,
        Action = 8,
        ActionParameter = 16,
        ActionsAndParameters = Action | ActionParameter,
        CollectionsAndActions = Collections | Action,
        ObjectsAndProperties = Objects | Property,
        ObjectsPropertiesAndCollections = Objects | Property | Collections,
        ObjectsPropertiesAndParameters = Objects | Property | ActionParameter,
        PropertiesAndCollections = Property | Collections,
        PropertiesAndParameters = Property | ActionParameter,
        PropertiesCollectionsAndActions = Property | Collections | Action,
        Everything = Objects | Property | Collections | Action | ActionParameter,
        EverythingButParameters = Objects | Property | Collections | Action
    }
}