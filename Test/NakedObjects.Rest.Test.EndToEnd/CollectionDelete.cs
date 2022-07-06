// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class CollectionDelete : CollectionAbstract {
    #region Helpers

    protected override string FilePrefix => "Collection-Delete-";

    #endregion

    [TestInitialize]
    public void AddItemsToCollections() {
        InitializeVerySimple1();
    }

    [TestMethod]
    public void DeleteItemFromList() {
        Helpers.Helpers.TestResponse(SimpleList, $"{FilePrefix}DeleteItemFromList", Simple1AsArgument.ToString(), Methods.Delete, Codes.Forbidden);
    }
}