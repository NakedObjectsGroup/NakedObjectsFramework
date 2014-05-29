// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.IO;

namespace NakedObjects.Value {
    /// <summary>
    /// Interface implemented by both FileAttachment and Image
    /// (Not intended to be used directly within domain code.) 
    /// </summary>
    public interface IStreamResource {
        string MimeType { get; }

        Stream GetResourceAsStream();
    }
}