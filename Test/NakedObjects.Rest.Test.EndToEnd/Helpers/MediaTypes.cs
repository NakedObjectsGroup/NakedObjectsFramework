// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace RestfulObjects.Test.EndToEnd {
    /// <summary>
    /// 
    /// </summary>
    public static class MediaTypes {
        public const string Json = @"application/json";
        public const string WithProfile = Json + @";profile=""urn:org.restfulobjects:repr-types/";
        public const string Homepage = WithProfile + @"homepage""";
        public const string List = WithProfile + @"list""";
        public const string User = WithProfile + @"user""";
        public const string Version = WithProfile + @"version""";
        public const string ObjectProfile = WithProfile + @"object""";
        public const string Property = WithProfile + @"object-property""";
        public const string ObjectCollection = WithProfile + @"object-collection""";
        public const string ObjectAction = WithProfile + @"object-action""";
        public const string ActionResult = WithProfile + @"action-result""";
    }
}