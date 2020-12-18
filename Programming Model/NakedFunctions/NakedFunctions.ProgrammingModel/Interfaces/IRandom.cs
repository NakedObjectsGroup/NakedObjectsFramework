namespace NakedFunctions
{
    public interface IRandom
    {
        //Contains a randomised integer in the range 0 - Int32.MaxValue
        int Value { get; }

        //Returns an int, derived from the Value, in the range 0 - max
        int ValueInRange(int max);

        //Returns an int, derived from the Value, in the range min - max
        int ValueInRange(int min, int max);

        //Returns a new IRandom
        //This method is side-effect free and deterministic.
        IRandom Next();
    }
}
