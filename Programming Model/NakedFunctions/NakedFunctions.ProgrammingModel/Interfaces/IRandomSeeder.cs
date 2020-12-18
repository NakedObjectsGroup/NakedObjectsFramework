namespace NakedFunctions
{
    //Defines a well-known service that may be accessed via the GetService method on IContainer.
    //NakedFunctions provides a default implementation, which may be replaced with a custom one, registered in Services Configuration.
    public interface IRandomSeeder
    {
        //Returns an initial random number, the value of which may be used.
        //Subsequent random numbers should be generated using the Next method(s) on the returned IRandom
        IRandom Seed { get;  }
    }
}
