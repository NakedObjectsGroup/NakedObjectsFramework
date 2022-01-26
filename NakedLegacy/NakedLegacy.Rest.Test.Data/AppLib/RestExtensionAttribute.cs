using System;
using NakedLegacy.Attribute;

namespace NakedLegacy.Rest.Test.Data.AppLib;

public class RestExtensionTestAttribute : System.Attribute, IRestExtensionAttribute {
    public string Name { get; set;  }
    public string Value { get; set;  }
}