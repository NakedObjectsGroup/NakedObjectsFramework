Console.Write("Model name (used for namespace & filename): ");
var modelName = Console.ReadLine();
Console.Write("How many types? ");
int n = Convert.ToInt32(Console.ReadLine());
var g = new Generator(modelName, n);

using (StreamWriter writer1 = new StreamWriter(@"..\..\..\" + modelName+".cs"))
{
    writer1.WriteLine("using NakedObjects;");
    using (StreamWriter writer2 = new StreamWriter(@"..\..\..\"+ modelName + "_Model.cs"))
    {
        writer2.WriteLine($"namespace {modelName}\n{{\n  public static class {modelName}_Config\n{{public static Type[] Types() => new []\n{{");
        for (int i = 0; i < n; i++)
        {
            writer1.WriteLine(g.GenerateClass(i));
            writer2.WriteLine($"typeof(Type{i}), ");
        }
        writer2.WriteLine("};\n}\n}");
        writer2.Flush();
    }
    writer1.Flush();
}


