// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NakedObjects;

namespace ROSI.Test.Data;

public class SimpleService
{
    public IDomainObjectContainer Container { private get; set; }
    public IQueryable<Class> GetClasses() => Container.Instances<Class>();

    [QueryOnly]
    public ClassToPersist GetTransient() => Container.NewTransientInstance<ClassToPersist>();

    public ClassToPersist GetPrePopulatedTransient(string uniqueName) {
        var ctp = Container.NewTransientInstance<ClassToPersist>();
        ctp.Id = 0;
        ctp.Name = uniqueName;
        return ctp;
    }
}

public enum TestChoices
{
    ChoiceOne,
    ChoiceTwo,
    ChoiceThree
}

public class Class
{
    [Title]
    public string Title => $"{nameof(Class)}:{Id}";

    public IDomainObjectContainer Container { private get; set; }

    [Key]
    public int Id { get; init; }

    public virtual string Property1 { get; set; }
    public virtual int Property2 { get; set; }

    public virtual Class Property3 => this;

    public virtual DateTime? Property4 => new DateTime();

    public virtual TestChoices PropertyWithScalarChoices { get; set; }

    public virtual string PropertyWithAutoComplete { get; set; }

    [TableView(true, "NameOne", "NameTwo")]
    public virtual IList<Class> Collection1 { get; set; } = new List<Class>();

    public virtual IList<Class> Collection2 { get; set; } = new List<Class>();

    public ICollection<Class> ChoicesProperty3() => Container.Instances<Class>().ToList();

    public ICollection<string> AutoCompletePropertyWithAutoComplete(string s) => new List<string> { s };

    public Class Action1() => this;

    public Class Action2() => this;
}

public class ClassWithActions
{
    [Title]
    public string Title => $"{nameof(ClassWithActions)}:{Id}";

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

    public ICollection<int> Choices0ActionWithMixedParmsReturnsObject() => new List<int> { 1, 2 };

    public int Default0ActionWithMixedParmsReturnsObject() => 3;

    public ICollection<Class> Choices1ActionWithMixedParmsReturnsObject() => Container.Instances<Class>().ToList();

    public Class Default1ActionWithMixedParmsReturnsObject() => Container.Instances<Class>().First();

    [Idempotent]
    public Class IdempotentActionWithValueParmsReturnsObject(int index, string str) => Container.Instances<Class>().FirstOrDefault();

    [Idempotent]
    public Class IdempotentActionWithRefParmsReturnsObject(Class class1, Class class2) => Container.Instances<Class>().FirstOrDefault();

    [Idempotent]
    public Class IdempotentActionWithMixedParmsReturnsObject(int index, Class class1) => Container.Instances<Class>().FirstOrDefault();

    public Class PotentActionWithValueParmsReturnsObject(int index, string str) => Container.Instances<Class>().FirstOrDefault();

    public Class PotentActionWithRefParmsReturnsObject(Class class1, Class class2) => Container.Instances<Class>().FirstOrDefault();

    public Class PotentActionWithMixedParmsReturnsObject(int index, Class class1) => Container.Instances<Class>().FirstOrDefault();

    [QueryOnly]
    public void ActionThrowsException()
    {
        try
        {
            throw new Exception("Exception 1");
        }
        catch (Exception ex)
        {
            throw new Exception("Exception 2", ex);
        }
    }

    [QueryOnly]
    public Class ActionFailsCrossValidation(int index, string str) => Container.Instances<Class>().FirstOrDefault();

    public string ValidateActionFailsCrossValidation(int index, string str) => "Fail parameter validation";
}

public enum TestEnum
{
    ValueOne,
    ValueTwo
}

public class ClassWithScalars
{
    [Title]
    public string Title => $"{nameof(ClassWithScalars)}:{Id}";

    public IDomainObjectContainer Container { private get; set; }

    [Key]
    public int Id { get; init; }

    [NotMapped]
    public bool Bool1 { get; set; } = true;

    [NotMapped]
    public bool? Bool2 { get; set; } = true;

    [NotMapped]
    public bool? Bool3 { get; set; } = null;

    [NotMapped]
    public short Short1 { get; set; } = 1;

    [NotMapped]
    public short? Short2 { get; set; } = 2;

    [NotMapped]
    public short? Short3 { get; set; } = null;

    [NotMapped]
    public int Int1 { get; set; } = 3;

    [NotMapped]
    public int? Int2 { get; set; } = 4;

    [NotMapped]
    public bool? Int3 { get; set; } = null;

    [NotMapped]
    public long Long1 { get; set; } = 5;

    [NotMapped]
    public long? Long2 { get; set; } = 6;

    [NotMapped]
    public long? Long3 { get; set; } = null;

    [NotMapped]
    public double Double1 { get; set; } = 7.1;

    [NotMapped]
    public double? Double2 { get; set; } = 8.2;

    [NotMapped]
    public double? Double3 { get; set; } = null;

    [NotMapped]
    public decimal Decimal1 { get; set; } = 9.1M;

    [NotMapped]
    public decimal? Decimal2 { get; set; } = 10.2M;

    [NotMapped]
    public decimal? Decimal3 { get; set; } = null;

    [NotMapped]
    public DateTime DateTime1 { get; set; } = new(2023, 01, 11, 11, 54, 00);

    [NotMapped]
    public DateTime? DateTime2 { get; set; } = new DateTime(2024, 02, 12, 13, 54, 00);

    [NotMapped]
    public DateTime? DateTime3 { get; set; } = null;

    [NotMapped]
    [DataType(DataType.DateTime)]

    public DateTime DateTime4 { get; set; } = new(2023, 01, 11, 11, 54, 00);

    [NotMapped]
    [DataType(DataType.DateTime)]
    public DateTime? DateTime5 { get; set; } = new DateTime(2024, 02, 12, 13, 54, 00);

    [NotMapped]
    [DataType(DataType.DateTime)]
    public DateTime? DateTime6 { get; set; } = null;

    [NotMapped]
    public TimeSpan TimeSpan1 { get; set; } = new(0, 1, 2, 3);

    [NotMapped]
    public TimeSpan? TimeSpan2 { get; set; } = new TimeSpan(0, 23, 59, 59);

    [NotMapped]
    public TimeSpan? TimeSpan3 { get; set; } = null;

    [NotMapped]
    public string String { get; set; } = "A String";
}


public class ClassToPersist
{

    public IDomainObjectContainer Container { private get; set; }

    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    [Optionally]
    public virtual ClassToPersist RefClassToPersist { get; set; }
}