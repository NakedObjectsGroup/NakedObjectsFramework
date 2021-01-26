using NakedFunctions;
using AW.Functions;

namespace AW.Types
{
    [ViewModel(typeof(QuickOrderLine_Functions))]
    public record QuickOrderLine 
    {
        [Hidden]
        public Product Product { get; init; }

        [Hidden]
        public short Number { get; init; }

        public override string ToString() =>  $"{Number} x {Product}";
    }
}
