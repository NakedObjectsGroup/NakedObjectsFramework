namespace AW.Types;

public abstract class BusinessEntity : IBusinessEntity
{

    public BusinessEntity() { }

    public BusinessEntity(BusinessEntity cloneFrom)
    {
        BusinessEntityID = cloneFrom.BusinessEntityID;
        BusinessEntityRowguid = cloneFrom.BusinessEntityRowguid;
        BusinessEntityModifiedDate = cloneFrom.BusinessEntityModifiedDate;
        Addresses = cloneFrom.Addresses;
        Contacts = cloneFrom.Contacts;
    }

    [Hidden]
    public int BusinessEntityID { get; init; }

    [Hidden]
    public Guid BusinessEntityRowguid { get; init; }

    [Hidden]
    public DateTime BusinessEntityModifiedDate { get; init; }

    [RenderEagerly]
    [TableView(false, nameof(BusinessEntityAddress.AddressType), nameof(BusinessEntityAddress.Address))]
    public virtual ICollection<BusinessEntityAddress> Addresses { get; init; } = new List<BusinessEntityAddress>();

    [RenderEagerly]
    [TableView(false, nameof(BusinessEntityContact.ContactType), nameof(BusinessEntityContact.Person))]
    public virtual ICollection<BusinessEntityContact> Contacts { get; init; } = new List<BusinessEntityContact>();
}