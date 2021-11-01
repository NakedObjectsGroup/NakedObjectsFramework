namespace Legacy.NakedObjects.Application.ValueHolder {
    public abstract class Magnitude : BusinessValueHolder {
        private const long serialVersionUID = 1;

        public virtual bool isBetween(Magnitude minMagnitude, Magnitude maxMagnitude) => isGreaterThanOrEqualTo(minMagnitude) && isLessThanOrEqualTo(maxMagnitude);

        public abstract bool isEqualTo(Magnitude magnitude);

        public virtual bool isGreaterThan(Magnitude magnitude) => magnitude.isLessThan(this);

        public virtual bool isGreaterThanOrEqualTo(Magnitude magnitude) => ((isLessThan(magnitude) ? 1 : 0) ^ 1) != 0;

        public abstract bool isLessThan(Magnitude magnitude);

        public virtual bool isLessThanOrEqualTo(Magnitude magnitude) => ((isGreaterThan(magnitude) ? 1 : 0) ^ 1) != 0;

        public virtual Magnitude max(Magnitude magnitude) => isGreaterThan(magnitude) ? this : magnitude;

        public virtual Magnitude min(Magnitude magnitude) => isLessThan(magnitude) ? this : magnitude;

        //[JavaFlags(17)]
        public sealed override bool isSameAs(BusinessValueHolder @object) => @object is Magnitude && isEqualTo((Magnitude)@object);
    }
}