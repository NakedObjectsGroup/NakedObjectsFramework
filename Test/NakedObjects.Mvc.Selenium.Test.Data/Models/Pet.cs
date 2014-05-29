using System.ComponentModel.DataAnnotations;

namespace NakedObjects.Web.UnitTests.Models {
    [NotPersisted]
    public class Pet {
        #region Injected Services

        #endregion

        #region Life Cycle Methods

        #endregion

        [Key, Hidden]
        public virtual int PetId { get; set; }

        [Title]
        public virtual string Name { get; set; }

        [Optionally]
        public virtual string Species { get; set; }
    }
}