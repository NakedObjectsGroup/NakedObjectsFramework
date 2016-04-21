// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace RestfulObjects.Test.EndToEnd {
    /// <summary>
    /// 
    /// </summary>
    public enum Codes {
        Succeeded = 200,
        SucceededNewRepresentation = 201,
        SucceededValidation = 204,
        SyntacticallyInvalid = 400,
        Forbidden = 403,
        NotFound = 404,
        MethodNotValid = 405,
        WrongMediaType = 406,
        ValidationFailed = 422,
        ServerException = 500
    }
}