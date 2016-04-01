
using NakedObjects;

namespace TinyApp
{
    public class Customer
    {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [Title]
        public virtual string Name { get; set; }
    }

}
