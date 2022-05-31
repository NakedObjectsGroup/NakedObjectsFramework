using System.Text;

public class Generator
{
    public Generator(string nameSpace, int numTypes)
    {
        this.numTypes = numTypes;
        this.nameSpacePrefix = nameSpace;
        rand = new Random();
    }
    Random rand;
    int numTypes;
    const int maxMembers = 20;
    const int maxParams = 5;
    const int nameRange = maxMembers * 100000;
    public string nameSpacePrefix;
    const string typeNamePrefix = "Type";
    const string propNamePrefix = "Prop";
    const string actionNamePrefix = "Action";
    const string paramNamePrefix = "param";

    public string GenerateClass(int i) => $"namespace {nameSpacePrefix}\n{{\n{AddAttributesFrom(classAttrbutes)}\npublic class {typeNamePrefix}{i}\n{{\n{Members()}}}\n}}\n";

    string Members()
    {
        var sb = new StringBuilder();
        var n = rand.Next(maxMembers) + 1;
        for (int i = 0; i < n; i++)
        {
            sb.Append(Member());
        }
        return sb.ToString();
    }

    string Member() => rand.Next(2) > 0 ? Action() : Property();

    string Property() => $"  {AddAttributesFrom(propertyAttributes)}\n  public {ValueOrDomainTypeOrColl()} {PropertyName()} {{ get; set; }}\n";

    string PropertyName() => $"{propNamePrefix}{NameSuffix()}";

    string AddAttributesFrom(string[] attributes)
    {
        var sb = new StringBuilder();
        var n = rand.Next(attributes.Count()+1);
        if (n > 0)
        {
            sb.Append("[");
            for (int i = 0; i < n; i++)
            {
                sb.Append(attributes[i]);
                if (i < n - 1) sb.Append(",");
            }
            sb.Append("]");
        }
        return sb.ToString();
    }

    string[] propertyAttributes = new[] { "MemberOrder(1)", "Disabled" };

    string Action() => $"  {AddAttributesFrom(actionAttributes)}\n  public {ReturnType()} {ActionName()} ({ParamList()}) {{throw new NotImplementedException();}}\n";

    string[] actionAttributes = new[] { "MemberOrder(1)", "DescribedAs(\"Foo\")", "QueryOnly" };

    string ActionName() => $"{actionNamePrefix}{NameSuffix()}";

    string ParamList()
    {
        var sb = new StringBuilder();
        int n = rand.Next(maxParams);
        for (int i = 0; i < n; i++)
        {
            sb.Append(Param());
            if (i < n - 1) sb.Append(", ");
        }
        return sb.ToString();
    }

    string Param() => $"{AddAttributesFrom(paramAttrbutes)} {ValueOrDomainTypeOrColl()} {ParamName()}";

    string ParamName() => $"{paramNamePrefix}{NameSuffix()}";

    string ValueOrDomainTypeOrColl()
    {
        var c = rand.Next(3);
        if (c > 2) return ValueType();
        if (c > 1) return DomainType();
        return Collection();
    }

    string ReturnType()
    {
        var c = rand.Next(3);
        if (c > 2) return DomainType();
        if (c > 1) return Collection();
        return "void";
    }

    string DomainType()
    {
        int n = rand.Next(numTypes);
        return $"{nameSpacePrefix}.{typeNamePrefix}{n}";
    } 

    string[] valueTypes = new[] { "int", "double", "string", "DateTime" };

    string ValueType() => valueTypes[rand.Next(valueTypes.Count() - 1)];

    string[] collTypes = new[] { "List", "HashSet" };

    string CollType() => collTypes[rand.Next(collTypes.Count())];

    string Collection() => $"{CollType()}<{DomainType()}>";

    string NameSuffix() => $"{rand.Next(nameRange)}";

    string[] classAttrbutes = new[] { "Bounded", "Immutable" };

    string[] paramAttrbutes = new[] { "Optionally", };


}
