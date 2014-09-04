using System;
using System.Linq;

namespace NakedObjects.Services
{
   public interface IPolymorphicNavigatorWithOid
    {
        TLink AddLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TLink : class, NakedObjects.IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, NakedObjects.IHasIntegerId;

        IQueryable<TOwner> FindOwners<TLink, TRole, TOwner>(TRole value)
            where TLink : class, NakedObjects.IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, NakedObjects.IHasIntegerId;

        TLink NewTransientLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TLink : class, NakedObjects.IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, NakedObjects.IHasIntegerId;

        void RemoveLink<TLink, TRole, TOwner>(TRole value, TOwner owner)
            where TLink : class, NakedObjects.IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, NakedObjects.IHasIntegerId;

        TLink UpdateAddOrDeleteLink<TLink, TRole, TOwner>(TRole value, TLink link, TOwner owner)
            where TLink : class, NakedObjects.IPolymorphicLinkWithOid<TRole, TOwner>, new()
            where TRole : class
            where TOwner : class, NakedObjects.IHasIntegerId;
    }
}
