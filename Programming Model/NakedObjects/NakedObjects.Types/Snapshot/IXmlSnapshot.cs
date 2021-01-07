// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Snapshot {
    /// <summary>
    ///     API for the XmlSnapshot that is defined in the NakedObjects Framework and is returned by
    ///     the GenerateSnapshot method on IXmlSnapshotService.
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