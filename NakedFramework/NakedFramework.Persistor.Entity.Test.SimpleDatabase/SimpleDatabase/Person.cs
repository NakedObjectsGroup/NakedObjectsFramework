// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace SimpleDatabase;

public abstract class AbstractTestCode {
    #region test code

    private IDictionary<string, int> callbackStatus;

    public virtual void Created() {
        SetupStatus();
        callbackStatus["Created"]++;
    }

    public virtual void Updating() {
        SetupStatus();
        callbackStatus["Updating"]++;
    }

    public virtual void Updated() {
        SetupStatus();
        callbackStatus["Updated"]++;
    }

    public virtual void Loading() {
        SetupStatus();
        callbackStatus["Loading"]++;
    }

    public virtual void Loaded() {
        SetupStatus();
        callbackStatus["Loaded"]++;
    }

    public virtual void Persisting() {
        SetupStatus();
        callbackStatus["Persisting"]++;
    }

    public virtual void Persisted() {
        SetupStatus();
        callbackStatus["Persisted"]++;
    }

    private void SetupStatus() =>
        callbackStatus ??= new Dictionary<string, int> {
            { "Created", 0 },
            { "Updating", 0 },
            { "Updated", 0 },
            { "Loading", 0 },
            { "Loaded", 0 },
            { "Persisting", 0 },
            { "Persisted", 0 }
        };

    public void ResetCallbackStatus() {
        callbackStatus = null;
    }

    [NakedObjectsIgnore]
    public IDictionary<string, int> GetCallbackStatus() => callbackStatus;

    #endregion
}

public class Person : AbstractTestCode {
    [Root]
    public object Parent { get; set; }

    public IDomainObjectContainer Container { protected get; set; }

    #region Primitive Properties

    public virtual int Id { get; set; }

    #endregion

    #region Complex Properties

// ReSharper disable InconsistentNaming

    public virtual NameType ComplexProperty { get; set; } = new();

    public virtual ComplexType1 ComplexProperty_1 { get; set; } = new();

    #endregion

    #region Navigation Properties

    private readonly List<Food> _food = new();

    public virtual ICollection<Food> Food => _food;

    #endregion

    public object ExposeContainerForTest() => Container;
}

// ReSharper restore InconsistentNaming