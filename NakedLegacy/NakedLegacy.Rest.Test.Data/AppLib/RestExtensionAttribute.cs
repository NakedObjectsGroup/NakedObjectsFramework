using System;

namespace NakedLegacy.Rest.Test.Data.AppLib;

public class RestExtensionTestAttribute : Attribute, IRestExtensionAttribute {
    public string Name { get; set;  }
    public string Value { get; set;  }
}