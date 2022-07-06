// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd.Helpers;

/// <summary>
/// </summary>
public static class JsonRep {
    //Factored out so that we can test this returning a Null instead

    public const string FormalDomainModeAsQueryString = $@"?{XRoDomainModel}={Formal}";
    public const string DomainModeQueryStringMalformed = $@"?{XRoDomainModel}=foo";
    public const string SimpleDomainModeAsQueryString = $@"?{XRoDomainModel}={Simple}";
    public const string Href = "href";
    public const string Members = "members";
    public const string DomainType = "domainType";
    private const string XRoDomainModel = "x-ro-domain-model";
    private const string XRoValidateOnly = "x-ro-validate-only";
    private const string Formal = "formal";
    private const string Simple = "simple";

    public static int NextId => ++CurrentId;

    public static int CurrentId { get; private set; } = 10000;

    public static string Empty() =>
        //return (new JObject()).ToString();
        null;

    public static JObject MostSimple1AsRef() => ObjectAsRef(@"MostSimple/1");

    public static JObject MostSimple2AsRef() => ObjectAsRef(@"MostSimple/2");

    public static JObject MostSimple3AsRef() => ObjectAsRef(@"MostSimple/3");

    public static JObject WithValue1AsRef() => ObjectAsRef(@"WithValue/1");

    private static JObject ObjectAsRef(string oid) => new(new JProperty(Href, Urls.Objects + Urls.NameSpace + oid));

    public static JProperty ValidateOnly() => new(XRoValidateOnly, true);

    public static JProperty FormalDomainModel() => new(XRoDomainModel, Formal);

    public static JProperty SimpleDomainModel() => new(XRoDomainModel, Simple);
}