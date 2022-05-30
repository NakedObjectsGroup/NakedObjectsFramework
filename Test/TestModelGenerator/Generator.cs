using System.Text;

public class Generator
{
    public Generator(int numTypes)
    {
        this.numTypes = numTypes;
        rand = new Random();
    }
    Random rand;
    int numTypes;
    const int maxMembers = 20;
    const int maxParams = 5;
    const int nameRange = maxMembers * 100000;
    public string nameSpacePrefix = "Long.Name.Space.N";
    const string typeNamePrefix = "Type";
    const string propNamePrefix = "Prop";
    const string actionNamePrefix = "Action";
    const string paramNamePrefix = "param";

    public string GenerateClass(int i) => $"namespace {nameSpacePrefix}{i}\n{{\npublic class {typeNamePrefix}{i}\n{{\n{Members()}}}\n}}\n";

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

    string Property() => $"  public {ValueOrDomainTypeOrColl()} {PropertyName()} {{ get; set; }}\n";

    string PropertyName() => $"{propNamePrefix}{NameSuffix()}";

    string Action() => $"  public {ReturnType()} {ActionName()} ({ParamList()}) {{throw new NotImplementedException();}}\n";

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

    string Param() => $"{ValueOrDomainTypeOrColl()} {ParamName()}";

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
        return $"{nameSpacePrefix}{n}.{typeNamePrefix}{n}";
    } 

    string[] valueTypes = new[] { "int", "double", "string", "DateTime" };

    string ValueType() => valueTypes[rand.Next(valueTypes.Count() - 1)];

    string[] collTypes = new[] { "List", "HashSet" };

    string CollType() => collTypes[rand.Next(collTypes.Count())];

    string Collection() => $"{CollType()}<{DomainType()}>";

    string NameSuffix() => $"{rand.Next(nameRange)}";

}
