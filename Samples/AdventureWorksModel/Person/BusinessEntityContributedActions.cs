using NakedObjects;

namespace AdventureWorksModel {
    /// <summary>
    /// Defines actions contributed to a business entity for managing associated addresses and contacts
    /// </summary>
    public class BusinessEntityContributedActions {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion
    }
}
