// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace ROSI.Test.Data;

public class SimpleService {
    public IDomainObjectContainer Container { private get; set; }
    public IQueryable<Class> GetClasses() => Container.Instances<Class>();
}

public enum TestChoices {
    ChoiceOne, 
    ChoiceTwo,
    ChoiceThree
}

public class Class {

    public IDomainObjectContainer Container { private get; set; }

    [Key]
    public int Id { get; init; }

    public virtual string Property1 { get; set; }
    public virtual int Property2 { get; set; }

    public virtual Class Property3 => this;

    public ICollection<Class> ChoicesProperty3() => Container.Instances<Class>().ToList();

    public virtual TestChoices PropertyWithScalarChoices { get; set; }

    public virtual IList<Class> Collection1 { get; set; } = new List<Class>();
    public virtual IList<Class> Collection2 { get; set; } = new List<Class>();

    public Class Action1() => this;

    public Class Action2() => this;
}

public class ClassWithActions {
    [Key]
    public int Id { get; init; }

    public IDomainObjectContainer Container { private get; set; }

    [QueryOnly]
    public Class ActionNoParmsReturnsObject() => Container.Instances<Class>().FirstOrDefault();

    [QueryOnly]
    public IQueryable<Class> ActionNoParmsReturnsList() => Container.Instances<Class>();

    [QueryOnly]
    public void ActionNoParmsReturnsVoid() { }

    [QueryOnly]
    public Class ActionWithValueParmsReturnsObject(int index, string str) => Container.Instances<Class>().FirstOrDefault();

    [QueryOnly]
    public Class ActionWithRefParmsReturnsObject(Class class1, Class class2) => Container.Instances<Class>().FirstOrDefault();

    [QueryOnly]
    public Class ActionWithMixedParmsReturnsObject(int index, Class class1) => Container.Instances<Class>().FirstOrDefault();

    [Idempotent]
    public Class IdempotentActionWithValueParmsReturnsObject(int index, string str) => Container.Instances<Class>().FirstOrDefault();

    [Idempotent]
    public Class IdempotentActionWithRefParmsReturnsObject(Class class1, Class class2) => Container.Instances<Class>().FirstOrDefault();

    [Idempotent]
    public Class IdempotentActionWithMixedParmsReturnsObject(int index, Class class1) => Container.Instances<Class>().FirstOrDefault();

    public Class PotentActionWithValueParmsReturnsObject(int index, string str) => Container.Instances<Class>().FirstOrDefault();

    public Class PotentActionWithRefParmsReturnsObject(Class class1, Class class2) => Container.Instances<Class>().FirstOrDefault();

    public Class PotentActionWithMixedParmsReturnsObject(int index, Class class1) => Container.Instances<Class>().FirstOrDefault();
}