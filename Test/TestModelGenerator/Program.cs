Console.Write("FileName to write to (incl. extension): ");
var fileName = Console.ReadLine();
Console.Write("How many types? ");
int n = Convert.ToInt32(Console.ReadLine());
var g = new Generator(n);


using (StreamWriter writer = new StreamWriter(@"..\..\..\"+fileName))
{
    for (int i = 0; i < n; i++)
    {
        writer.WriteLine(g.GenerateClass(i));
    }
    writer.Flush();
}


