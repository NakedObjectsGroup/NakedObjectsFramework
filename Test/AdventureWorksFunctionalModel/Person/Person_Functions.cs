namespace AW.Functions;

public static class Person_Functions
{

    [Edit]
    public static IContext EditName(this Person p,
                                    [Optionally] string title,
                                    string firstName,
                                    [Optionally] string middleName,
                                    string lastName,
                                    [Optionally] string suffix,
                                    [Named("Reverse name order")] bool nameStyle,
                                    IContext context) =>
          context.WithUpdated(p, new(p)
          {
              Title = title,
              FirstName = firstName,
              MiddleName = middleName,
              LastName = lastName,
              Suffix = suffix,
              NameStyle = nameStyle,
              ModifiedDate = context.Now()
          });


    //Yes, this could be done with a Hidden attribute, but is done as a function here for testing.
    public static bool HideContacts(this Person p) => true;

    public static IContext CreateNewPhoneNumber(this Person p,
                                                PhoneNumberType type,
                                                [RegEx(@"^([0-9][0-9\s-]+)$")] string phoneNumber,
                                                IContext context)
        => context.WithNew(new PersonPhone
        {
            BusinessEntityID = p.BusinessEntityID,
            PhoneNumberType = type,
            PhoneNumber = phoneNumber
        });

    #region CreditCards

    #region CreateNewCreditCard

    public static IContext CreateNewCreditCard(this Person p,
                                               string cardType,
                                               [RegEx("^[0-9]{16}$")] [DescribedAs("No spaces")]
                                                   string cardNumber,
                                               [RegEx("^[0-9]{2}/[0-9]{2}")] [DescribedAs("mm/yy")]
                                                   string expires,
                                               IContext context
    )
    {
        var expMonth = Convert.ToByte(expires.Substring(0, 2));
        var expYear = Convert.ToByte(expires.Substring(3, 2));
        var cc = new CreditCard { CardType = cardType, CardNumber = cardNumber, ExpMonth = expMonth, ExpYear = expYear };
        var link = new PersonCreditCard { CreditCard = cc, Person = p };
        return context.WithNew(cc).WithNew(link);
    }

    public static IList<string> Choices1CreateNewCreditCard(this Person p) =>
        new[] { "Vista", "Distinguish", "SuperiorCard", "ColonialVoice" };

    #endregion

    public static IList<CreditCard> ListCreditCards(this Person p, IQueryable<PersonCreditCard> pccs)
    {
        var id = p.BusinessEntityID;
        return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).ToList();
    }

    public static IQueryable<CreditCard> RecentCreditCards(this Person p, IQueryable<PersonCreditCard> pccs)
    {
        var id = p.BusinessEntityID;
        return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).OrderByDescending(cc => cc.ModifiedDate);
    }

    #endregion
}