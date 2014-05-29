// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Snapshot {
    /// <summary>
    /// API for the XmlSnapshot that is defined in the NakedObjects Framework and is returned by
    /// the GenerateSnapshot method on IXmlSnapshotService.
    /// </summary>
    public interface IXmlSnapshot {
        string Xml { get; }
        string Xsd { get; }
        string SchemaLocationFileName { get; }
        void Include(string path);
        void Include(string path, string annotation);
        string TransformedXml(string transform);
        string TransformedXsd(string transform);
    }
}