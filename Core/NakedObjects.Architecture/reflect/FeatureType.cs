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
        Properties = 2,
        Collections = 4,
        Actions = 8,
        ActionParameters = 16,
        Interfaces = 32,
        ActionsAndActionParameters = Actions | ActionParameters,
        CollectionsAndActions = Collections | Actions,
        ObjectsAndProperties = Objects | Properties,
        ObjectsPropertiesAndCollections = Objects | Properties | Collections,
        ObjectsPropertiesAndActionParameters = Objects | Properties | ActionParameters,
        ObjectsInterfacesAndProperties = Objects | Properties | Interfaces,
        ObjectsInterfacesPropertiesAndCollections = Objects | Properties | Collections | Interfaces,
        ObjectsInterfacesPropertiesAndActionParameters = Objects | Properties | ActionParameters | Interfaces,
        PropertiesAndCollections = Properties | Collections,
        PropertiesAndActionParameters = Properties | ActionParameters,
        PropertiesCollectionsAndActions = Properties | Collections | Actions,
        Everything = Objects | Properties | Collections | Actions | ActionParameters | Interfaces,
        EverythingButActionParameters = Objects | Properties | Collections | Actions | Interfaces,
        EverythingButCollections = Objects | Properties | Actions | ActionParameters | Interfaces,
        ObjectsAndInterfaces = Objects | Interfaces
    }
}