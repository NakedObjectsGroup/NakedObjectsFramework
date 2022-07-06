﻿// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Rest.Test.EndToEnd.Helpers;

/// <summary>
/// </summary>
public static class Urls {
    public const string BaseUrlRemote = @"http://nakedobjectsrotest.azurewebsites.net/";
    public const string BaseUrlLocal = @"http://localhost:5000/";

    public const string NameSpace = "";
    public const string Actions = @"/actions/";
    public const string Properties = @"/properties/";
    public const string Collections = @"/collections/";
    public const string Invoke = "/invoke";
    public const string TypeActions = @"/type-actions/";

    //Specific objects used
    public const string WithValue1 = $@"{NameSpace}WithValue/1";
    public const string WithValue2 = $@"{NameSpace}WithValue/2";
    public const string Immutable1 = $@"{NameSpace}Immutable/1";
    public const string WithReference1 = $@"{NameSpace}WithReference/1";
    public const string WithCollection1 = $@"{NameSpace}WithCollection/1";
    public const string WithScalars1 = $@"{NameSpace}WithScalars/1";
    public const string MostSimple1 = $@"{NameSpace}MostSimple/1";
    public const string VerySimple1 = $@"{NameSpace}VerySimple/1";
    public const string VerySimple2 = $@"{NameSpace}VerySimple/2";
    public const string WithActionService = $@"{NameSpace}WithActionService";
    public const string WithActionObject1 = $@"{NameSpace}WithActionObject/1";

    public const string WithAttachments1 = $@"{NameSpace}WithAttachments/1";

    //ViewModels
    public const string VmMostSimple = $@"{NameSpace}MostSimpleViewModel/";
    public const string VmWithValue = $@"{NameSpace}WithValueViewModel/";
    public const string VmWithReference = $@"{NameSpace}WithReferenceViewModel/";
    public const string VmWithCollection = $@"{NameSpace}WithCollectionViewModel/";

    public const string VmWithAction = $@"{NameSpace}WithActionViewModel/";

    //Eager rendering
    public const string VerySimpleEager1 = $@"{NameSpace}VerySimpleEager/1";

    public static readonly string User = $@"{BaseUrl}user/";
    public static readonly string Version = $@"{BaseUrl}version/";
    public static readonly string Services = $@"{BaseUrl}services/";
    public static readonly string Objects = $@"{BaseUrl}objects/";
    public static readonly string DomainTypes = $@"{BaseUrl}domain-types/";

    public static readonly string RestDataRepository = $@"{Services}{NameSpace}RestDataRepository";

    public static string BaseUrl => Helpers.UseLocalUrl ? BaseUrlLocal : BaseUrlRemote;
}