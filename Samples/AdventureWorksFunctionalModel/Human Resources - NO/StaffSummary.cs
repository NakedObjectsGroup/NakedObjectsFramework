using NakedObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorksModel
{

    //Used to test creation/rendering of NotPersisted object
    //Note, however, that recommended pattern would be to use a ViewModel for this
    [NotPersisted, NotMapped]
    public class StaffSummary
    {
        #region Injected services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public override string ToString()
        {
            var t = Container.NewTitleBuilder();
            t.Append("Staff Summary");
            return t.ToString();
        }
        public string DisablePropertyDefault()
        {
            return "Not editable";
        }
        [MemberOrder(1)]
        public virtual int Female { get; set; }

        [MemberOrder(2)]
        public virtual int Male { get; set; }

        [MemberOrder(3)]
        public virtual int TotalStaff { get; set; }

    }
}
